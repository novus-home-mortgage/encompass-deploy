using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace EncompassDeploymentTool.Actions
{
    public class PackHandler
    {
        public int Execute(PackOptions options)
        {
            var sourcePath = Path.GetFullPath(options.ManifestPath);
            var outputPath = Path.GetDirectoryName(Path.GetFullPath(options.OutputPath));

            var stagingPath = Path.Combine(outputPath, Path.GetFileNameWithoutExtension(options.OutputPath));

            var doc = new XmlDocument();
            doc.Load(Path.Combine(sourcePath, "manifest.xml"));

            if (!Directory.Exists(stagingPath))
            {
                Directory.CreateDirectory(stagingPath);
            }
            if (Directory.EnumerateFileSystemEntries(stagingPath).Any())
            {
                throw new ApplicationException("Staging folder was not cleaned up");
            }

            var package = doc["package"];

            // Stage Forms

            if (package["FormList"] != null)
            {
                foreach (var form in package["FormList"].ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Form"))
                {
                    Console.WriteLine($"Staging Form {form.Attributes["mname"].Value}");
                    var sourceFile = form.Attributes["fname"].Value;
                    File.Copy(Path.Combine(sourcePath, sourceFile), Path.Combine(stagingPath, sourceFile));
                }
            }

            // Stage Form Codebase Assemblies

            if (package["AssemblyList"] != null)
            {
                foreach (var codebase in package["AssemblyList"].ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Assembly"))
                {
                    Console.WriteLine($"Staging Form Codebase {codebase.Attributes["name"].Value}");
                    var sourceFile = codebase.Attributes["fname"].Value;

                    if (options.SetAssemblyVersions)
                    {
                        var assemblyVersion = Assembly.ReflectionOnlyLoadFrom(Path.Combine(sourcePath, sourceFile)).GetName().Version;
                        Console.WriteLine("Assembly Version {0}", assemblyVersion);

                        if (codebase.Attributes["version"] == null)
                            codebase.Attributes.Append(doc.CreateAttribute("version"));

                        codebase.Attributes["version"].Value = assemblyVersion.ToString();
                    }

                    File.Copy(Path.Combine(sourcePath, sourceFile), Path.Combine(stagingPath, sourceFile));
                }
            }

            // Stage Plugin Assemblies

            if (package["PluginList"] != null)
            {
                Directory.CreateDirectory(Path.Combine(stagingPath, "Plugins"));

                foreach (var plugin in package["PluginList"].ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "Plugin"))
                {
                    Console.WriteLine($"Staging Plugin {plugin.Attributes["name"].Value}");
                    var sourceFile = plugin.Attributes["fname"].Value;

                    if (options.SetAssemblyVersions)
                    {
                        var assemblyVersion = Assembly.ReflectionOnlyLoadFrom(Path.Combine(sourcePath, sourceFile)).GetName().Version;
                        Console.WriteLine("Assembly Version {0}", assemblyVersion);

                        if (plugin.Attributes["version"] == null)
                            plugin.Attributes.Append(doc.CreateAttribute("version"));

                        plugin.Attributes["version"].Value = assemblyVersion.ToString();
                    }

                    File.Copy(Path.Combine(sourcePath, sourceFile), Path.Combine(stagingPath, "Plugins", sourceFile));
                }
            }


            // Stage Custom Data Objects

            if (package["CustomDataObjectList"] != null)
            {
                Directory.CreateDirectory(Path.Combine(stagingPath, "CustomData"));

                foreach (var cdo in package["CustomDataObjectList"].ChildNodes.Cast<XmlNode>().Where(n => n.LocalName == "CustomDataObject"))
                {
                    Console.WriteLine($"Staging Custom Data Object {cdo.Attributes["name"].Value}");
                    var sourceFile = cdo.Attributes["fname"].Value;
                    File.Copy(Path.Combine(sourcePath, "CustomData", sourceFile), Path.Combine(stagingPath, "CustomData", sourceFile));
                }
            }

            // Stage Manifest

            Console.WriteLine("Staging Manifest");
            using (var writer = XmlWriter.Create(Path.Combine(stagingPath, "manifest.xml"), new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
            }))
            {
                doc.WriteTo(writer);
            }

            // Make the Package File

            ZipFile.CreateFromDirectory(stagingPath, options.OutputPath, CompressionLevel.Optimal, includeBaseDirectory: false);

            // Clean up the staging directory

            Directory.Delete(stagingPath, recursive: true);

            return 0;
        }
    }
}
