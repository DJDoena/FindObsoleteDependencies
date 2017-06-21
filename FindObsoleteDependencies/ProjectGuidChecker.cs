namespace FindObsoleteDependencies
{
    using System;
    using System.Collections.Generic;

    internal sealed class ProjectGuidChecker
    {
        private Dictionary<String, DependencyInfo> ProjsByFullName
            => (Program.Projs);

        internal void Check()
        {
            var projsByProjectGuid = new Dictionary<String, DependencyInfo>(ProjsByFullName.Count);

            foreach (var di in ProjsByFullName.Values)
            {
                try
                {
                    projsByProjectGuid.Add(di.ProjectGuid, di);
                }
                catch (ArgumentException)
                {
                    var logger = Logger.GetLogger(LogType.ProjectGuid);

                    logger.Log("Duplicate ProjectGuid:");
                    logger.Log(di.ProjectGuid, 1);
                    logger.Log(di.Proj.DirectoryName, 1);
                    logger.Log(di.Proj.Name, 1);

                    var existing = projsByProjectGuid[di.ProjectGuid];

                    logger.Log(existing.Proj.DirectoryName, 1);
                    logger.Log(existing.Proj.Name, 1);
                    logger.Log();
                }
            }
        }
    }
}