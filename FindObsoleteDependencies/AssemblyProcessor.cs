using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FindObsoleteDependencies
{
    internal class AssemblyProcessor
    {
        private readonly IAssemblyReflectionManager Manager;

        private readonly FileInfo Dll;

        private IEnumerable<DependencyInfo> DependencyInfos
            => (Program.DependencyInfos);

        private Dictionary<String, DependencyInfo> Projs
            => (Program.Projs);

        public AssemblyProcessor(String fullName
            , IAssemblyReflectionManager manager)
        {
            Manager = manager;

            Dll = new FileInfo(fullName);
        }

        internal void Process()
        {
            IEnumerable<String> binaryReferences = GetBinaryReferences();

            var dllName = Dll.Name.Replace(".dll", String.Empty);

            var dependencyInfos = GetDependencyInfosByName(dllName).ToList();

            if (dependencyInfos.Count > 1)
            {
                LogMultipleMatchingProjects(dependencyInfos);
            }

            Process(binaryReferences, dependencyInfos);
        }

        private void LogMultipleMatchingProjects(List<DependencyInfo> dependencyInfos)
        {
            var logger = Logger.GetLogger(LogType.UnnecessaryReference);

            logger.Log("Multiple Possible Source Projects:");
            logger.Log("DLL:", 1);
            logger.Log(Dll.DirectoryName, 2);
            logger.Log(Dll.Name, 2);

            foreach (var di in dependencyInfos)
            {
                logger.Log("Project:", 1);
                logger.Log(di.Proj.DirectoryName, 2);
                logger.Log(di.Proj.Name, 2);
            }

            logger.Log();
        }

        private List<String> GetBinaryReferences()
        {
            var binaryReferences = Manager.Reflect(Dll.FullName, (assembly) =>
                    {
                        var names = assembly?.GetReferencedAssemblies() ?? Enumerable.Empty<AssemblyName>();

                        var assemblyNames = names.Select(name => name.Name).ToList();

                        return (assemblyNames);
                    }
                );

            return (binaryReferences);
        }

        private void Process(IEnumerable<String> binaryReferences
            , IEnumerable<DependencyInfo> dependencyInfos)
        {
            foreach (var di in dependencyInfos)
            {
                Process(binaryReferences, di);
            }
        }

        private void Process(IEnumerable<String> binaryReferences
            , DependencyInfo dependencyInfo)
        {
            foreach (var fi in dependencyInfo.Dependencies)
            {
                var key = fi.FullName.ToLower();

                DependencyInfo referenceDependencyInfo;
                if (Projs.TryGetValue(key, out referenceDependencyInfo))
                {
                    Process(binaryReferences, dependencyInfo, referenceDependencyInfo);
                }
                else
                {
                    var logger = Logger.GetLogger(LogType.ResolvingError);

                    logger.Log("Resolving Error:");
                    logger.Log(dependencyInfo.Proj.DirectoryName, 1);
                    logger.Log(dependencyInfo.Proj.Name, 1);
                    logger.Log(fi.DirectoryName, 1);
                    logger.Log(fi.Name, 1);
                    logger.Log();
                }
            }
        }

        private void Process(IEnumerable<String> binaryReferences
            , DependencyInfo sourceDependencyInfo
            , DependencyInfo referenceDependencyInfo)
        {
            if (binaryReferences.Contains(referenceDependencyInfo.AssemblyName) == false)
            {
                var logger = Logger.GetLogger(LogType.UnnecessaryReference);

                logger.Log("Probable Unnecessary Project Reference:");
                logger.Log(Dll.DirectoryName, 1);
                logger.Log(Dll.Name, 1);
                logger.Log("From", 1);
                logger.Log(sourceDependencyInfo.Proj.DirectoryName, 2);
                logger.Log(sourceDependencyInfo.Proj.Name, 2);
                logger.Log("To", 1);
                logger.Log(referenceDependencyInfo.Proj.DirectoryName, 2);
                logger.Log(referenceDependencyInfo.Proj.Name, 2);
                logger.Log();
            }
        }

        private IEnumerable<DependencyInfo> GetDependencyInfosByName(String name)
        {
            foreach (var di in DependencyInfos)
            {
                if (name == di.AssemblyName)
                {
                    yield return (di);
                }
            }
        }
    }
}