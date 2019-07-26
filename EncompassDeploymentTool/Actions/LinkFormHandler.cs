using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace EncompassDeploymentTool.Actions
{
    public class LinkFormHandler
    {
        public int Execute(LinkFormOptions options)
        {
            if (!File.Exists(options.FormFileName))
            {
                throw new ArgumentException("The specified form file was not found", nameof(options.FormFileName));
            }

            using (var zipArchive = ZipFile.Open(options.FormFileName, ZipArchiveMode.Update))
            {
                var formEntry = zipArchive.Entries.SingleOrDefault(entry => entry.FullName == "FORM.htm");
                if (formEntry == null)
                {
                    throw new ArgumentException("The specified form file does not contain an Encompass form");
                }

                using (var stream = formEntry.Open())
                {
                    var doc = new XmlDocument();
                    doc.Load(stream);

                    var bodyNode = doc.SelectSingleNode("/HTML/BODY");

                    if (bodyNode == null)
                        throw new ArgumentException("The specified form file does not contain a valid Encompass form");

                    var codebaseNode = bodyNode.SelectSingleNode("EMCODEBASE");

                    if (codebaseNode == null)
                    {
                        // If there is no codebase node in the form, we need to have a filename and classname given in the options
                        if (string.IsNullOrEmpty(options.CodebaseFileName))
                            throw new InvalidOperationException("The Form does not contain an EMCODEBASE element and no codebase assembly was specified");
                        if (string.IsNullOrEmpty(options.ClassName))
                            throw new InvalidOperationException("The Form does not contain an EMCODEBASE element and no class type was specified");

                        // Create a new element to populate values into
                        codebaseNode = bodyNode.AppendChild(doc.CreateElement("EMCODEBASE"));
                        codebaseNode.Attributes?.Append(doc.CreateAttribute("path"));
                        codebaseNode.Attributes?.Append(doc.CreateAttribute("assembly"));
                        codebaseNode.Attributes?.Append(doc.CreateAttribute("version"));
                        codebaseNode.Attributes?.Append(doc.CreateAttribute("typeName"));

                        codebaseNode.Attributes["path"].Value = options.CodebaseFileName;
                    }

                    var assemblyPath = FindCodebaseAssembly(options.CodebaseFileName, codebaseNode);

                    var assemblyName = Assembly.ReflectionOnlyLoadFrom(assemblyPath).GetName();
                    codebaseNode.Attributes["assembly"].Value = assemblyName.Name;
                    codebaseNode.Attributes["version"].Value = assemblyName.Version.ToString();

                    if (!string.IsNullOrEmpty(options.ClassName))
                        codebaseNode.Attributes["typeName"].Value = options.ClassName;

                    stream.SetLength(0);
                    doc.Save(stream);
                }
            }

            return 0;
        }

        private string FindCodebaseAssembly(string providedFileName, XmlNode codebaseElement)
        {
            // If an assembly file was provided, use that
            if (!string.IsNullOrEmpty(providedFileName))
            {
                if (!File.Exists(providedFileName))
                {
                    throw new ArgumentException("The specified assembly file was not found");
                }
                return providedFileName;
            }

            // If the path in the form points to an existing assembly, use that
            var formPath = codebaseElement.Attributes["path"].Value;
            if (!string.IsNullOrEmpty(formPath) && File.Exists(formPath))
                return formPath;

            // If the form points to an assembly name, look for it
            var formAssemblyName = codebaseElement.Attributes["assembly"].Value + ".dll";
            if (File.Exists(formAssemblyName))
                return formAssemblyName;

            throw new ArgumentException("Could not find a matching codebase assembly for the form");
        }
    }
}
