using System;

namespace Assets.Scripts.Singletons
{
    internal class Logger
    {
        private static Logger instance = null;

        private Logger() { }

        public static Logger GetLogger()
        {
            if (instance == null)
            {
                instance = new Logger();
            }

            return instance;
        }

        public void LogDebug(string message)
        {
            throw new NotImplementedException();
        }

        public void LogError(Exception exception, string message)
        {
            throw new NotImplementedException();
        }
    }
}
