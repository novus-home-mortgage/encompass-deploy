using EllieMae.Encompass.Client;
using System;

namespace EncompassDeploymentTool.Actions
{
    public class GetFormHandler
    {
        private readonly ISession encompassSession;

        public GetFormHandler(ISession encompassSession)
        {
            this.encompassSession = encompassSession;
        }

        public int Execute(GetFormOptions options)
        {
            Console.WriteLine($"Getting Form \"{options.FormName}\"");
            return 0;
        }
    }
}
