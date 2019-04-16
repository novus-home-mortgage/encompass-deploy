using EllieMae.Encompass.Client;

namespace EncompassDeploymentTool.Services
{
    public class EncompassConnectionManager
    {
        public ISession Session { get; private set; }

        public void StartEncompassSession(BaseOptions options)
        {
            Session = new Session();
            Session.Start($"https://{options.InstanceName}.ea.elliemae.net${options.InstanceName}", options.UserId, options.Password);
        }
    }
}
