using CommandLine;
using EncompassDeploymentTool.Actions;
using EncompassDeploymentTool.Services;

namespace EncompassDeploymentTool
{
    class Program
    {
        static int Main(string[] args)
        {
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();

            var sessionManager = new EncompassConnectionManager();

            return Parser.Default.ParseArguments<GetFormOptions, ImportPackageOptions>(args)
                .WithParsed<BaseOptions>(opts => sessionManager.StartEncompassSession(opts))
                .MapResult(
                    (GetFormOptions opts) => new GetFormHandler(sessionManager.Session).Execute(opts),
                    (ImportPackageOptions opts) => new ImportHandler().Execute(opts),
                    errs => 1
                );
        }
    }
}
