using CommandLine;
using EncompassDeploymentTool.Actions;

namespace EncompassDeploymentTool
{
    class Program
    {
        static int Main(string[] args)
        {
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();

            return Parser.Default.ParseArguments<GetFormOptions, ImportPackageOptions>(args)
                .WithParsed<BaseOptions>(StartEncompassSession)
                .MapResult(
                    (GetFormOptions opts) => new GetFormHandler().Execute(opts),
                    (ImportPackageOptions opts) => new ImportHandler().Execute(opts),
                    errs => 1
                );
        }

        static void StartEncompassSession(BaseOptions options)
        {
            EllieMae.EMLite.RemotingServices.Session.Start(
                $"https://{options.InstanceName}.ea.elliemae.net${options.InstanceName}",
                options.UserId,
                options.Password,
                "encompass-deploy"
            );
        }
    }
}
