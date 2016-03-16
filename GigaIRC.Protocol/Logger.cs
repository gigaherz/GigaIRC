using System.IO;
using GigaIRC.Events;
using GigaIRC.Protocol;

namespace GigaIRC
{
    public class Logger
    {
        private const int FileLogLevel = 1;
        private const int ConLogLevel = 0;

        public readonly Session Session;
        public readonly Connection Connection;

        readonly FileStream logFile;
        readonly StreamWriter logWriter;

        public Logger(Connection c, string logfile)
        {
            Session = c.Session;
            Connection = c;
            logFile = new FileStream(logfile, FileMode.Append);
            logWriter = new StreamWriter(logFile);
        }

        public void LogLine(int level, string fmt, params object[] args)
        {
            var line = string.Format(fmt, args);
            if (level >= FileLogLevel)
            {
                logWriter.WriteLine(line);
                logWriter.Flush();
            }
            if (level >= ConLogLevel)
            {
                Session.OnLogLine.Invoke(Connection, new LogEventArgs(line));
            }
        }
    }
}
