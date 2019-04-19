using CommandLine;
using EncompassDeploymentTool.Actions;

namespace EncompassDeploymentTool
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<GetFormOptions, PackOptions, ImportPackageOptions>(args)
                .WithParsed<EncompassConnectionOptions>(StartEncompassSession)
                .MapResult(
                    (GetFormOptions opts) => new GetFormHandler().Execute(opts),
                    (PackOptions opts) => new PackHandler().Execute(opts),
                    (ImportPackageOptions opts) => new ImportHandler().Execute(opts),
                    errs => 1
                );
        }

        static void StartEncompassSession(EncompassConnectionOptions options)
        {
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();

            EllieMae.EMLite.RemotingServices.Session.Start(
                $"https://{options.InstanceName}.ea.elliemae.net${options.InstanceName}",
                options.UserId,
                options.Password,
                "encompass-deploy"
            );
        }
    }
}
