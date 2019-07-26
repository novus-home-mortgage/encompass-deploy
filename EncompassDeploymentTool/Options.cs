using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace EncompassDeploymentTool
{
    public class EncompassConnectionOptions
    {
        [Option('i', "instance", Required = true, HelpText = "The Instance Name of your Encompass server")]
        public string InstanceName { get; set; }

        [Option('u', "userid", Required = true, HelpText = "The User ID to use to login to Encompass")]
        public string UserId { get; set; }

        [Option('p', "password", Required = true, HelpText = "The password to use to login to Encompass")]
        public string Password { get; set; }
    }

    [Verb("get-form", HelpText = "Download a custom input form from Encompass")]
    public class GetFormOptions : EncompassConnectionOptions
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

    [Verb("link-form", HelpText = "Link a form to a specified codebase version")]
    public class LinkFormOptions
    {
        [Value(0, Required = true, HelpText = "The form file (*.emfrm) to update")]
        public string FormFileName { get; set; }

        [Value(1, Default = null, HelpText = "The codebase DLL to link to. If omitted, searches for the assembly already referenced in the form in the working directory")]
        public string CodebaseFileName { get; set; }

        [Value(2, Default = null, HelpText = "The full class name within the codebase assembly. If omitted, uses the class name already referenced in the form.\nWARNING: This tool does not check whether or not the specified class exists in the specified assembly.")]
        public string ClassName { get; set; }
    }

    [Verb("import", HelpText = "Import a customization package into Encompass")]
    public class ImportPackageOptions : EncompassConnectionOptions
    {
        [Value(0, Required = true, HelpText = "The path to the package(s) to import. Wildcards supported. If you do not specify a file, all .empkg files in the given directory will be imported.")]
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

    [Verb("pack", HelpText = "Create a package of customizations")]
    public class PackOptions
    {
        [Value(0, Default = ".", HelpText = "The path to the manifest.xml file that defines the package")]
        public string ManifestPath { get; set; }

        [Option('o', "output", Default = "./package.empkg", HelpText = "The folder where the package should be saved")]
        public string OutputPath { get; set; }

        [Option("set-assembly-versions", Default = true, HelpText = "If true, the manifest.xml will be updated with the version numbers of referenced assemblies")]
        public bool SetAssemblyVersions { get; set; }
    }

    [Verb("update-cdo", HelpText = "Update XML values in a packaged Custom Data Object")]
    public class UpdateCDOOptions
    {
        [Value(0, Required = true, HelpText = "The path to the package file")]
        public string PackagePath { get; set; }

        [Value(1, Required = true, HelpText = "The name of the CDO to update")]
        public string CustomDataObjectName { get; set; }

        [Value(2, Required = true, HelpText = "The XPath of the value to update")]
        public string DataXPath { get; set; }

        [Value(3, Required = true, HelpText = "The value to save")]
        public string NewValue { get; set; }
    }
}
