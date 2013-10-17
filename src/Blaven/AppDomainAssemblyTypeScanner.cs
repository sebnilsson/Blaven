using System;
using System.Collections.Generic;
using System.Linq;

namespace Blaven
{
    public static class AppDomainAssemblyTypeScanner
    {
        private static readonly IEnumerable<Type> ExportedTypes;

        static AppDomainAssemblyTypeScanner()
        {
            ExportedTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             where !assembly.IsDynamic && !assembly.ReflectionOnly
                             from type in assembly.GetExportedTypes()
                             select type).ToList();
        }

        public static IEnumerable<Type> GetTypesOf<T>()
        {
            return GetTypesOf(typeof(T));
        }

        public static IEnumerable<Type> GetTypesOf(Type baseType)
        {
            return ExportedTypes.Where(x => x.IsAssignableFrom(baseType)).ToList();
        }
    }
}