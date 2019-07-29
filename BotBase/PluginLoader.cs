using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BotBase
{
    public static class PluginLoader
    {
        public static IEnumerable<Type> LoadPlugins(string path, Type pluginType)
        {
            if (Directory.Exists(path))
            {
                var assemblyFiles = Directory.GetFiles(path).Where(f => Path.GetExtension(f) == ".exe" || Path.GetExtension(f) == ".dll").ToArray();

                ICollection<Assembly> assemblies = new List<Assembly>(assemblyFiles.Length);
                foreach (var dllFile in assemblyFiles)
                {
                    var an = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }
                
                var pluginTypes = new List<Type>();
                foreach (var assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        var types = assembly.GetTypes();

                        foreach (var type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.BaseType == pluginType)
                                {
                                    pluginTypes.Add(type);
                                }
                            }
                        }
                    }
                }

                return pluginTypes;
            }

            return null;
        }

        public static Type LoadType(string path, string typeFullName)
        {
            if (Directory.Exists(path))
            {
                var assemblyFiles = Directory.GetFiles(path).Where(f => Path.GetExtension(f) == ".exe" || Path.GetExtension(f) == ".dll").ToArray();

                ICollection<Assembly> assemblies = new List<Assembly>(assemblyFiles.Length);
                foreach (var dllFile in assemblyFiles)
                {
                    var an = AssemblyName.GetAssemblyName(dllFile);
                    var assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                }

                var pluginTypes = new List<Type>();
                foreach (var assembly in assemblies)
                {
                    if (assembly != null)
                    {
                        var types = assembly.GetTypes();

                        foreach (var type in types)
                        {
                            if (type.IsInterface || type.IsAbstract)
                            {
                                continue;
                            }
                            else
                            {
                                if (type.FullName == typeFullName)
                                    return type;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static Type LoadType(string typeFullName) => LoadType(
            Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), typeFullName);
    }
}