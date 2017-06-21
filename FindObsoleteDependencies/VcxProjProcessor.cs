using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FindObsoleteDependencies
{
    internal sealed class VcxProjProcessor : ProjProcessorBase
    {
        public VcxProjProcessor(String fullName)
            : base(fullName)
        { }

        protected override void ProcessStream(out String assemblyName
            , out String projectGuid
            , out List<FileInfo> dependencies)
        {
            String rootNamespace = null;
            String projectName = null;
            assemblyName = null;
            projectGuid = null;
            dependencies = new List<FileInfo>(0);

            while (ProjReader.EndOfStream == false)
            {
                var line = ProjReader.ReadLine().Trim();

                if (line.StartsWith("<ProjectName>"))
                {
                    projectName = GetProjectName(line);
                }
                else if (line.StartsWith("<RootNamespace>"))
                {
                    rootNamespace = GetRootNamespace(line);
                }
                else if (line.StartsWith("<TargetName>"))
                {
                    assemblyName = GetTargetName(line);
                }
                else if (line.StartsWith("<ProjectGuid>"))
                {
                    projectGuid = GetProjectGuid(line);
                }
            }

            if (assemblyName != null)
            {
                var pn = projectName ?? String.Empty;

                assemblyName = Regex.Replace(assemblyName, @"\$\(ProjectName\)", pn, RegexOptions.IgnoreCase);

                assemblyName = Regex.Replace(assemblyName, @"\$\(RootNamespace\)", pn, RegexOptions.IgnoreCase);
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

            if (assemblyName == null)
            {
                assemblyName = projectName;
            }
        }

        private String GetRootNamespace(String line)
            => (RemoveTag(line, "RootNamespace"));

        private static String GetProjectName(String line)
            => (RemoveTag(line, "ProjectName"));

        private static String GetTargetName(String line)
            => (RemoveTag(line, "TargetName"));
    }
}