using System;

namespace EncompassDeploymentTool.Actions
{
    public class ImportHandler
    {
        public int Execute(ImportPackageOptions options)
        {
            Console.WriteLine($"Importing Package {options.PackagePath}");
            return 0;
        }
    }
}
