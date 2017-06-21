namespace FindObsoleteDependencies
{
    using System;
    using System.IO;

    internal abstract class ProjsProcessorBase
    {
        private String SourceRoot
            => (Program.SourceRoot);

        protected abstract String Extension { get; }

        public void Process()
        {
            var projs = Directory.GetFiles(SourceRoot, "*." + Extension, SearchOption.AllDirectories);

            foreach (var proj in projs)
            {
                Process(proj);
            }
        }

        protected abstract void Process(String fullName);
    }
}