using System;
using System.Collections.Generic;
using System.IO;

namespace FindObsoleteDependencies
{
    internal abstract class ProjProcessorBase
    {
        protected readonly FileInfo Proj;

        private Dictionary<String, DependencyInfo> Projs
            => (Program.Projs);

        protected String BranchRoot
            => (Program.BranchRoot);

        protected StreamReader ProjReader { get; set; }

        protected ProjProcessorBase(String fullName)
        {
            Proj = new FileInfo(fullName);
        }

        public void Process()
        {
            String assemblyName;
            String projectGuid;
            List<FileInfo> dependencies;
            using (ProjReader = new StreamReader(Proj.FullName))
            {
                ProcessStream(out assemblyName, out projectGuid, out dependencies);
            }

            if (assemblyName == null)
            {
                LogNoAssemblyName();
            }
            else if (projectGuid == null)
            {
                LogNoProjectGuid();
            }
            else
            {
                var di = new DependencyInfo(Proj, assemblyName, projectGuid, dependencies);

                var key = Proj.FullName.ToLower();

                Projs.Add(key, di);
            }
        }

        private void LogNoProjectGuid()
        {
            var logger = Logger.GetLogger(LogType.ProjectGuid);

            logger.Log("No ProjectGuid:");
            logger.Log(Proj.DirectoryName, 1);
            logger.Log(Proj.Name, 1);
            logger.Log();
        }

        private void LogNoAssemblyName()
        {
            var logger = Logger.GetLogger(LogType.NoAssemblyName);

            logger.Log("No AssemblyName:");
            logger.Log(Proj.DirectoryName, 1);
            logger.Log(Proj.Name, 1);
            logger.Log();
        }

        protected abstract void ProcessStream(out String assemblyName
            , out String projectGuid
            , out List<FileInfo> dependencies);

        protected static String GetProjectGuid(String line)
            => (RemoveTag(line, "ProjectGuid"));

        protected static String RemoveTag(String line
            , String tagName)
            => (line.Replace("<" + tagName + ">", String.Empty).Replace("</" + tagName + ">", String.Empty));
    }
}