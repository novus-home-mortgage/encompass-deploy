using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EncompassDeploymentTool
{
    public class EllieMaeIdpClient : IDisposable
    {
        // Until they let us have our own...use theirs!
        private const string client_id = "n35xg3ze";
        private const string redirect_uri = "https://encompass.elliemae.com/homepage/atest.asp";

        private readonly HttpClient client;

        public EllieMaeIdpClient()
        {
            client = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
            })
            {
                BaseAddress = new Uri("https://idp.elliemae.com"),
            };
        }

        public async Task<string> GetAuthCode(string instanceId, string username, string password)
        {
            // First, get the login page to fill the EM cookie and provide us the postback URL (which contains state)
            var loginPage = await client.GetAsync(
                $"/authorize?client_id={client_id}&response_type=code&redirect_uri={redirect_uri}&scope=sc&instance_id={instanceId}")
                .ConfigureAwait(false);

            loginPage.EnsureSuccessStatusCode();

            var loginPageContent = await loginPage.Content.ReadAsStringAsync();

            // Remember, never try to parse HTML with Regex
            var parsedHtmlWithRegex = Regex.Match(loginPageContent, "<form name=\"loginForm\"[^>]*action=\"(?'post_uri'[^\"]*)\"[^>]*>");
            if (parsedHtmlWithRegex.Success == false)
                throw new InvalidOperationException("Could not find the post URL in the login page");
            var postUri = parsedHtmlWithRegex.Groups["post_uri"].Value;

            var loginFormData = new Dictionary<string, string>
            {
                ["pf.pass"] = password,
                ["login"] = string.Empty,
                ["pf.adapterId"] = "sc",
                ["pf.username"] = $"{username}@{instanceId}#sc",
                ["current_scope"] = "sc",
                ["redirect_uri"] = redirect_uri,
                ["response_type"] = "code",
            };

            // When we send the form, we expect a 302 with the auth code in the Location header
            var loginResponse = await client.PostAsync(postUri, new FormUrlEncodedContent(loginFormData))
                .ConfigureAwait(false);
            if (loginResponse.StatusCode != HttpStatusCode.Redirect)
                throw new HttpRequestException("Login attempt did not return the expected redirect. Could be incorrect password.");

            // Find the "code" parameter in the query string
            var locationQuery = loginResponse.Headers.Location.Query.Substring(1); // trim the '?'
            var hasCode = locationQuery.Split('&')
                .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1])
                .TryGetValue("code", out var code);

            if (!hasCode)
                throw new InvalidOperationException("Could not find auth code in login response");

            return code;
        }

        public void Dispose()
        {
            client?.Dispose();
        }
    }
}