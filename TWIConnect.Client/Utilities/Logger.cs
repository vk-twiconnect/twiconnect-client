using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWIConnect.Client.Utilities
{
    public static class Logger
    {
        private static System.Lazy<NLog.Logger> _nLogger = new Lazy<NLog.Logger>(() =>
            {
                return NLog.LogManager.GetLogger(Constants.FileNames.NLogName);
            });

        public static void Log(Exception ex)
        {
            Logger.Log(NLog.LogLevel.Error, ex.ToString());
        }

        public static void Log(string message, params object[] arguments)
        {
            Logger.Log(NLog.LogLevel.Info, message, arguments);
        }

        public static void Log(NLog.LogLevel level, string message, params object[] arguments)
        {
            try
            {
                if ((arguments != null) && (arguments.Length > 0))
                    message = string.Format(message, arguments);

                Logger._nLogger.Value.Log(level, message);
            }
            catch
            {
                //Intentionally empty catch block to suppress any exception
            }
        }

        public static long GetTimeElapsed(System.Diagnostics.Stopwatch stopWatch)
        {
            stopWatch.Stop();
            return stopWatch.ElapsedMilliseconds;
        }
    }
}
