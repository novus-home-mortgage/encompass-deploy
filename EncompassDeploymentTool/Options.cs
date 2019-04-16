using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace EncompassDeploymentTool
{
    public class BaseOptions
    {
        [Option('i', "instance", Required = true, HelpText = "The Instance Name of your Encompass server")]
        public string InstanceName { get; set; }

        [Option('u', "userid", Required = true, HelpText = "The User ID to use to login to Encompass")]
        public string UserId { get; set; }

        [Option('p', "password", Required = true, HelpText = "The password to use to login to Encompass")]
        public string Password { get; set; }
    }

    [Verb("get-form", HelpText = "Download a custom input form from Encompass")]
    public class GetFormOptions : BaseOptions
    {
        [Value(0, Required = true, HelpText = "The name of the form to download")]
        public string FormName { get; set; }

        [Option('o', "output", Default = ".", HelpText = "The folder where the downloaded form should be saved")]
        public string OutputPath { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Download a form to the current directory", new GetFormOptions
                {
                    InstanceName = "TEBE12345678",
                    UserId = "appdeployer",
                    Password = "xxxxxxx",
                    FormName = "My Custom Form"
                });
            }
        }
    }

    [Verb("import", HelpText = "Import a customization package into Encompass")]
    public class ImportPackageOptions : BaseOptions
    {
        [Value(0, Required = true, HelpText = "The path of the package to import")]
        public string PackagePath { get; set; }

        [Usage]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("Import a package to the server", new ImportPackageOptions
                {
                    InstanceName = "BE12345678",
                    UserId = "appdeployer",
                    Password = "xxxxxxx",
                    PackagePath = "MyCustomizations.empkg"
                });
            }
        }
    }
}
