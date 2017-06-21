namespace FindObsoleteDependencies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class Program
    {
        private static IEnumerable<DependencyInfo> s_DependencyInfos;

        internal static String BranchRoot { get; private set; }

        internal static String SourceRoot { get; private set; }

        internal static String BinaryRoot { get; private set; }

        internal static Dictionary<String, DependencyInfo> Projs { get; private set; }

        internal static IEnumerable<DependencyInfo> DependencyInfos
        {
            get
            {
                if (s_DependencyInfos == null)
                {
                    IEnumerable<DependencyInfo> dependcyInfos = Projs.Values;

                    dependcyInfos = dependcyInfos.Where(di => (di.AssemblyName.EndsWith(".resources") == false));

                    s_DependencyInfos = dependcyInfos.ToList();

                }

                return (s_DependencyInfos);
            }
        }

        static Program()
        {
            s_DependencyInfos = null;

            Projs = new Dictionary<String, DependencyInfo>();
        }

        public static void Main(String[] args)
        {
            if (GetPaths(args) == false)
            {
                return;
            }

            ProcessProjects<CsProjsProcessor>();

            ProcessProjects<VcxProjsProcessor>();

            var projectGuidChecker = new ProjectGuidChecker();

            projectGuidChecker.Check();

            var binaryProcessor = new AssembliesProcessor();

            binaryProcessor.Process();

            Console.WriteLine("Finished.");

            Console.ReadLine();

            Logger.Dispose();
        }

        private static Boolean GetPaths(String[] args)
        {
            var logger = Logger.GetLogger(LogType.Error);

            var success = true;

            if (args?.Length != 3)
            {
                logger.Log("You must give three arguments: branch root path, configuration, platform.");
                logger.Log(@"e.g. FindObsoleteDependencies.exe D:\WS\TM5_BugFix Debug x64");

                success = false;
            }

            var di = new DirectoryInfo(args[0]);

            if (di.Exists == false)
            {
                logger.Log("Path does not exist.");

                success = false;
            }

            if (success)
            {
                BranchRoot = di.FullName;

                if (BranchRoot.EndsWith(@"\") == false)
                {
                    BranchRoot += @"\";
                }

                SourceRoot = Path.Combine(BranchRoot, "src");

                BinaryRoot = Path.Combine(BranchRoot, "binaries", args[1], args[2]);

                if (Directory.Exists(BinaryRoot) == false)
                {
                    logger.Log("Path does not exist.");

                    success = false;
                }
            }

            if (success == false)
            {
                Console.WriteLine("Exiting.");
            }

            return (success);
        }

        private static void ProcessProjects<T>()
            where T : ProjsProcessorBase, new()
        {
            var processor = new T();

            processor.Process();
        }
    }
}