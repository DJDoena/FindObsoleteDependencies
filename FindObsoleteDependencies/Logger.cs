namespace FindObsoleteDependencies
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    internal sealed class Logger : IDisposable
    {
        private static readonly Dictionary<LogType, Logger> Loggers;

        private StreamWriter FileLogger { get; set; }

        private Boolean Disposed { get; set; }

        static Logger()
        {
            Loggers = new Dictionary<LogType, Logger>(6);
        }

        private Logger(LogType logType)
        {
            var fileName = Enum.GetName(typeof(LogType), logType) + ".log";

            FileLogger = new StreamWriter(fileName, false, Encoding.GetEncoding(1252));

            Disposed = false;
        }

        internal static Logger GetLogger(LogType logType)
        {
            Logger logger;
            if (Loggers.TryGetValue(logType, out logger) == false)
            {
                logger = new Logger(logType);

                Loggers.Add(logType, logger);
            }

            return (logger);
        }

        internal void Log(String line = null
            , Int32 indentation = 0)
        {
            var prefix = String.Empty;

            for (Int32 i = 0; i < indentation; i++)
            {
                prefix += "\t";
            }

            var output = prefix + line;

            FileLogger.WriteLine(output);

            Console.WriteLine(output);
        }

        void IDisposable.Dispose()
        {
            if (Disposed == false)
            {
                FileLogger.Dispose();

                FileLogger = null;

                Disposed = true;
            }
        }

        public static void Dispose()
        {
            foreach (IDisposable logger in Loggers.Values)
            {
                logger.Dispose();
            }

            Loggers.Clear();
        }
    }
}