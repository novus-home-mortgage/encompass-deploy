using EllieMae.EMLite.Client;
using EllieMae.EMLite.Packages;
using System;

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
            Console.WriteLine($"Importing Package {options.PackagePath}");

            var importer = new PackageImporter(conn, PackageImportConflictOption.Overwrite);
            var package = new ExportPackage(options.PackagePath);

            var success = importer.Import(package);

            return success ? 0 : 1;
        }
    }
}
