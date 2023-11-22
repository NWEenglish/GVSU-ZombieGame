using System;

namespace Assets.Scripts.Singletons
{
    // Per Unity, this will output to both the Unity terminal and in the "AppData\Roaming" directory.
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
            UnityEngine.Debug.Log(message);
        }

        public void LogError(Exception exception, string message)
        {
            UnityEngine.Debug.LogError($"<color=red>Error: </color>{message} | ExceptionMessage: {exception?.Message}");
        }
    }
}
