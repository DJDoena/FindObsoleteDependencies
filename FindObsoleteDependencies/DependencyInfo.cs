using System;
using System.Collections.Generic;
using System.IO;

namespace FindObsoleteDependencies
{
    internal sealed class DependencyInfo
    {
        public FileInfo Proj { get; private set; }

        public String AssemblyName { get; private set; }

        public String ProjectGuid { get; private set; }

        public List<FileInfo> Dependencies { get; private set; }

        public DependencyInfo(FileInfo proj
            , String assemblyName
            , String projectGuid
            , List<FileInfo> dependencies)
        {
            Proj = proj;
            AssemblyName = assemblyName;
            ProjectGuid = projectGuid;
            Dependencies = dependencies;
        }
    }
}