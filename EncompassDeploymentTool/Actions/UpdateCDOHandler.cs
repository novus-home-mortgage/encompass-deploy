using System;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Xml;

namespace EncompassDeploymentTool.Actions
{
    public class UpdateCDOHandler
    {
        public int Execute(UpdateCDOOptions options)
        {
            if (!File.Exists(options.PackagePath))
            {
                throw new ArgumentException("The specified package file was not found");
            }

            using (var zipArchive = ZipFile.Open(options.PackagePath, ZipArchiveMode.Update))
            {
                var archiveEntry = zipArchive.Entries.SingleOrDefault(entry => entry.FullName == $"CustomData/{options.CustomDataObjectName}");
                if (archiveEntry == null)
                {
                    throw new ArgumentException("The specified custom data object was not found in the package");
                }

                using (var stream = archiveEntry.Open())
                {
                    var doc = new XmlDocument();
                    doc.Load(stream);

                    var nodes = doc.SelectNodes(options.DataXPath);
                    if (nodes == null || nodes.Count == 0)
                    {
                        throw new ArgumentException("No elements were found with the specified XPath");
                    }

                    foreach (XmlNode node in nodes)
                    {
                        node.InnerText = options.NewValue;
                    }

                    stream.SetLength(0);
                    doc.Save(stream);
                }
            }

            return 0;
        }
    }
}
