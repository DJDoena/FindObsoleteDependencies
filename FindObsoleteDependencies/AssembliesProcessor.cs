namespace FindObsoleteDependencies
{
    using System;
    using System.IO;

    internal sealed class AssembliesProcessor
    {
        private String BinaryRoot
            => (Program.SourceRoot);

        public void Process()
        {
            var dlls = Directory.GetFiles(BinaryRoot, "*.dll", SearchOption.AllDirectories);

            foreach (var dll in dlls)
            {
                var lower = dll.ToLower();

                if (lower.EndsWith(".resources.dll"))
                {
                    continue;
                }
                else if (lower.Contains(@"\obj\"))
                {
                    continue;
                }

                Process(dll);
            }
        }

        private void Process(String fullName)
        {
            try
            {
                TryProcess(fullName);
            }
            catch (Exception ex)
            {
                var logger = Logger.GetLogger(LogType.Error);

                logger.Log("DLL Load Exception:");

                var fi = new FileInfo(fullName);

                logger.Log(fi.DirectoryName, 1);
                logger.Log(fi.Name, 1);
                logger.Log(ex.GetType().Name, 1);
                logger.Log(ex.Message, 1);
                logger.Log();
            }
        }

        private void TryProcess(String fullName)
        {
            using (IAssemblyReflectionManager manager = new AssemblyReflectionManager())
            {
                var guid = Guid.NewGuid().ToString();

                var success = manager.LoadAssembly(fullName, guid);

                if (success)
                {
                    Process(fullName, manager);
                }

                manager.UnloadDomain(guid);

            }

            GC.Collect();

            GC.WaitForPendingFinalizers();
        }

        private void Process(String fullName
            , IAssemblyReflectionManager manager)
        {
            var processor = new AssemblyProcessor(fullName, manager);

            processor.Process();
        }
    }
}