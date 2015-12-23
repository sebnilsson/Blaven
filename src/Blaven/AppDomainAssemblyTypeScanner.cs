using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blaven
{
    public static class AppDomainAssemblyTypeScanner
    {
        private static readonly IEnumerable<Type> ExportedTypes;

        static AppDomainAssemblyTypeScanner()
        {
            var assemblies = LoadAssemblies();

            LoadUnloadedAssemblies(assemblies);

            assemblies = LoadAssemblies();

            var types =
                (from assembly in assemblies from type in assembly.GetExportedTypes() where !type.IsAbstract select type)
                    .ToList();

            ExportedTypes = types;
        }

        public static IEnumerable<Type> GetTypesOf<T>()
        {
            return GetTypesOf(typeof(T));
        }

        public static IEnumerable<Type> GetTypesOf(Type baseType)
        {
            return ExportedTypes.Where(x => x.IsAssignableFrom(baseType)).ToList();
        }

        private static IEnumerable<Assembly> LoadAssemblies()
        {
            var assemblies = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             where !assembly.IsDynamic
                             where !assembly.ReflectionOnly
                             select assembly;
            return assemblies;
        }

        private static void LoadUnloadedAssemblies(IEnumerable<Assembly> assemblies)
        {
            var existingAssemblyPaths = assemblies.Select(x => x.Location).ToArray();

            var assemblyDirectories = GetAssemblyDirectories();
            foreach (var directory in assemblyDirectories)
            {
                var unloadedAssemblies =
                    Directory.GetFiles(directory, "*.dll")
                        .Where(f => !existingAssemblyPaths.Contains(f, StringComparer.InvariantCultureIgnoreCase))
                        .ToArray();

                foreach (var unloadedAssembly in unloadedAssemblies)
                {
                    Assembly inspectedAssembly = null;
                    try
                    {
                        inspectedAssembly = Assembly.ReflectionOnlyLoadFrom(unloadedAssembly);
                    }
                    catch (BadImageFormatException)
                    {
                    }

                    if (inspectedAssembly == null
                        || !inspectedAssembly.GetReferencedAssemblies()
                                .Any(r => r.Name.StartsWith("Blaven", StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    try
                    {
                        Assembly.Load(inspectedAssembly.GetName());
                    }
                    catch
                    {
                    }
                }
            }

        }

        private static IEnumerable<string> GetAssemblyDirectories()
        {
            var privateBinPathDirectories = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath == null
                                                ? new string[] { }
                                                : AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(';');

            foreach (var privateBinPathDirectory in privateBinPathDirectories)
            {
                if (!string.IsNullOrWhiteSpace(privateBinPathDirectory))
                {
                    yield return privateBinPathDirectory;
                }
            }

            if (AppDomain.CurrentDomain.SetupInformation.PrivateBinPathProbe == null)
            {
                yield return AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            }
        }
    }
}