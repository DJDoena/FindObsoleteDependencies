namespace FindObsoleteDependencies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    internal sealed class CsProjProcessor : ProjProcessorBase
    {
        public CsProjProcessor(String fullName)
            : base(fullName)
        { }

        protected override void ProcessStream(out String assemblyName
            , out String projectGuid
            , out List<FileInfo> dependencies)
        {
            assemblyName = null;
            projectGuid = null;
            dependencies = new List<FileInfo>(20);

            while (ProjReader.EndOfStream == false)
            {
                var line = ProjReader.ReadLine().Trim();

                if (line.StartsWith("<AssemblyName>"))
                {
                    assemblyName = GetAssemblyName(line);
                }
                else if (line.StartsWith("<ProjectGuid>"))
                {
                    projectGuid = GetProjectGuid(line);
                }
                else if (line.StartsWith("<ProjectReference"))
                {
                    var dependency = TryGetDependency(line);

                    if (dependency != null)
                    {
                        dependencies.Add(dependency);
                    }
                }
            }

            if ((assemblyName != null) && (assemblyName.Contains("$")))
            {
                var logger = Logger.GetLogger(LogType.NoAssemblyName);

                logger.Log("AssemblyName Resolving Error:");
                logger.Log(Proj.DirectoryName, 1);
                logger.Log(Proj.Name, 1);
                logger.Log(assemblyName, 1);
                logger.Log();
            }
        }

        private FileInfo TryGetDependency(String line)
        {
            FileInfo dependencyFile = null;

            var parts = line.Split('"');

            if (parts.Length >= 3)
            {
                String dependency = GetDependencyFullName(line, parts[1]);

                var fi = new FileInfo(dependency);

                if (fi.Exists)
                {
                    dependencyFile = fi;
                }
            }

            if (dependencyFile == null)
            {
                var logger = Logger.GetLogger(LogType.ResolvingError);

                logger.Log("Unresolved:");
                logger.Log(Proj.DirectoryName, 1);
                logger.Log(Proj.Name, 1);
                logger.Log(line, 1);
                logger.Log();
            }

            return (dependencyFile);
        }

        private String GetDependencyFullName(String line
            , String dependency)
        {
            var dotdot = dependency.StartsWith(@"..\");

            if (dotdot)
            {
                dependency = Path.Combine(Proj.DirectoryName, dependency);
            }
            else if ((File.Exists(dependency) == false) && (line.EndsWith("/>") == false))
            {
                dependency = TryAddHintPath(line, dependency);
            }

            dependency = Regex.Replace(dependency, @"\$\(TIAEnvRoot\)", BranchRoot, RegexOptions.IgnoreCase);

            if ((dotdot == false) && File.Exists(dependency) == false)
            {
                dependency = Path.Combine(Proj.DirectoryName, dependency);
            }

            return (dependency);
        }

        private String TryAddHintPath(String line
            , String dependency)
        {
            while (ProjReader.EndOfStream == false)
            {
                var nextLine = ProjReader.ReadLine().Trim();

                if (nextLine == "</ProjectReference>")
                {
                    break;
                }

                if (line.StartsWith("<HintPath>"))
                {
                    var hintPath = RemoveTag(line, "HintPath");

                    dependency = Path.Combine(hintPath, dependency);

                    break;
                }
            }

            return (dependency);
        }

        private static String GetAssemblyName(String line)
            => (RemoveTag(line, "AssemblyName"));
    }
}