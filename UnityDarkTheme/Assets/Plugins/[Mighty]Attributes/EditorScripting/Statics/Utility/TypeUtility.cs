#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;

namespace MightyAttributes.Editor
{
    public static class TypeUtility
    {
        public static Assembly GetAssemblyByName(string name) => AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.GetName().Name == name);

        public static Assembly GetMainAssembly() => GetAssemblyByName(MightySettingsServices.MainAssemblyName);
        public static Assembly GetPluginsAssembly() => GetAssemblyByName(MightySettingsServices.PluginsAssemblyName);

        public static bool GetTypeInAssembly(string typeName, string assemblyName, out Type type)
        {
            if (!string.IsNullOrEmpty(typeName))
            {
                var assembly = GetAssemblyByName(assemblyName);
                if (assembly != null && (type = assembly.GetType(typeName)) != null)
                    return true;
            }

            type = null;
            return false;
        }

        public static bool GetTypeInAssemblies(string typeName, out Type type, params Assembly[] assemblies)
        {
            if (!string.IsNullOrEmpty(typeName))
                foreach (var assembly in assemblies)
                    if (assembly != null && (type = assembly.GetType(typeName)) != null)
                        return true;

            type = null;
            return false;
        }

        public static Type[] GetChildrenTypes(this Type baseType, Assembly assembly = null)
        {
            if (assembly == null) assembly = GetMainAssembly();
            return assembly.GetTypes().Where(t => t.BaseType == baseType).ToArray();
        }
    }
}
#endif