using System;
using System.IO;
using System.Reflection;
using CommandLine;
using EncompassDeploymentTool.Actions;
using Microsoft.Win32;

namespace EncompassDeploymentTool
{
    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<GetFormOptions, PackOptions, ImportPackageOptions, UpdateCDOOptions, LinkFormOptions>(args)
                .WithParsed<EncompassConnectionOptions>(ConfigureSmartClient)
                .MapResult(
                    (GetFormOptions opts) => new GetFormHandler().Execute(opts),
                    (PackOptions opts) => new PackHandler().Execute(opts),
                    (ImportPackageOptions opts) => new ImportHandler().Execute(opts),
                    (UpdateCDOOptions opts) => new UpdateCDOHandler().Execute(opts),
                    (LinkFormOptions opts) => new LinkFormHandler().Execute(opts),
                    errs => 1
                );
        }

        static void ConfigureSmartClient(EncompassConnectionOptions options)
        {
            // This tool is meant to be run in automated environments.
            // That means no picking your instance with the AppLauncher.
            // If you regularly connect to multiple environments and they are running different
            // Encompass versions, you're likely to encounter VersionMismatchExceptions.

            // To remedy, let's manipulate the registry in the same way that AppLauncher does
            var path = Assembly.GetExecutingAssembly().Location;
            var dir = Path.GetDirectoryName(path).Replace('\\', '/');

            var keyName = Environment.Is64BitProcess ? @"SOFTWARE\WOW6432Node\Ellie Mae\SmartClient" : @"SOFTWARE\Ellie Mae\SmartClient";
            var scKey = Registry.LocalMachine.OpenSubKey(keyName, writable: true) ??
                throw new Exception("Could not locate SmartClient registry key. Is SmartClient installed?");
            var key = scKey.OpenSubKey(dir, writable: true) ?? scKey.CreateSubKey(dir, writable: true) ??
                throw new Exception("Could not locate or create registry key for this application. Check permissions?");

            key.SetValue("AutoSignOn", "1", RegistryValueKind.String);
            key.SetValue("Credentials", GetCredentials(options.InstanceName), RegistryValueKind.String);
            key.SetValue("SmartClientIDs", options.InstanceName, RegistryValueKind.String);
            key.Close();

            // Now we can fire up the Ellie stuff
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();
            StartEncompassSession(options);
        }

        // Must start the session in a different method because .NET can't bind ClientSession.dll
        // into the execution context *at all* until the Encompass runtime is initialized
        static void StartEncompassSession(EncompassConnectionOptions options)
        {
            EllieMae.EMLite.RemotingServices.Session.Start(
                $"https://{options.InstanceName}.ea.elliemae.net${options.InstanceName}",
                options.UserId,
                options.Password,
                "encompass-deploy"
            );
        }

        // When you have a static encryption key, and you put it right next to your
        // encryption code, you don't have encryption code anymore.
        static string GetCredentials(string instanceId)
        {
            var credentials = $"{instanceId}\n{instanceId}";

            var encryptorType = Type.GetType("EllieMae.Encompass.AsmResolver.Utils.XT, EllieMae.Encompass.AsmResolver, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d11ef57bba4acf91");
            var resolverConstsType = typeof(EllieMae.Encompass.AsmResolver.Utils.ResolverConsts);

            var keyProperty = resolverConstsType.GetField("KB64", BindingFlags.Static | BindingFlags.NonPublic);
            var key = keyProperty.GetValue(null) as string;

            var encyptorMethod = encryptorType.GetMethod("ESB64", BindingFlags.Static | BindingFlags.NonPublic);

            return encyptorMethod.Invoke(null, new[]
            {
                credentials, key
            }) as string;
        }
    }
}
