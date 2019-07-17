using EllieMae.EMLite.Client;
using EllieMae.EMLite.Packages;
using System;
using System.IO;
using System.Linq;

namespace EncompassDeploymentTool.Actions
{
    public class ImportHandler
    {
        private readonly IConnection conn;

        public ImportHandler() : this(EllieMae.EMLite.RemotingServices.Session.Connection) { }

        public ImportHandler(IConnection conn)
        {
            this.conn = conn;
        }

        public int Execute(ImportPackageOptions options)
        {
            var directoryName = Path.GetDirectoryName(options.PackagePath);
            if (string.IsNullOrEmpty(directoryName)) directoryName = ".\\";

            var searchPattern = Path.GetFileName(options.PackagePath);
            if (string.IsNullOrEmpty(searchPattern)) searchPattern = "*.empkg";

            var packagePaths = Directory.EnumerateFiles(directoryName, searchPattern)
                .Select(Path.GetFullPath);

            foreach (var path in packagePaths)
            {
                Console.WriteLine($"Importing Package at {path}");

                var importer = new PackageImporter(conn, PackageImportConflictOption.Overwrite);
                var package = new ExportPackage(path);

                if (!importer.Import(package))
                {
                    throw new InvalidOperationException($"Could not import the package at {path}");
                }
            }

            return 0;
        }
    }
}
