using EllieMae.EMLite.Common;
using EllieMae.EMLite.ClientServer;
using System;
using System.IO;

namespace EncompassDeploymentTool.Actions
{
    public class GetFormHandler
    {
        private readonly IFormManager formManager;

        public GetFormHandler() : this(EllieMae.EMLite.RemotingServices.Session.FormManager) { }

        public GetFormHandler(IFormManager formManager)
        {
            this.formManager = formManager;
        }

        public int Execute(GetFormOptions options)
        {
            Console.WriteLine($"Getting Form \"{options.FormName}\"");

            var form = formManager.GetFormInfoByName(options.FormName);

            var outputPath = Path.GetFullPath(options.OutputPath);
            var fileName = FileSystem.EncodeFilename(form.Name, false) + ".emfrm";

            formManager.GetCustomForm(form.FormID).Write(Path.Combine(outputPath, fileName));

            return 0;
        }
    }
}
