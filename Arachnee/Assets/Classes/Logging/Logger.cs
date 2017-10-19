using System;
using UnityEngine;

namespace Assets.Classes.Logging
{
    public enum LogMode
    {
        UnityConsole,
        SystemConsole
    }

    public static class Logger
    {
        public static LogMode Mode { get; set; }

        public static void LogInfo(string message)
        {
            switch (Mode)
            {
                case LogMode.UnityConsole:
                    Debug.Log(message);
                    break;

                case LogMode.SystemConsole:
                    Console.WriteLine($"INFO: {message}");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Mode), "Log mode is not set.");
            }
        }

        public static void LogError(string message)
        {
            switch (Mode)
            {
                case LogMode.UnityConsole:
                    Debug.LogError(message);
                    break;

                case LogMode.SystemConsole:
                    Console.WriteLine($"ERROR: {message}");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Mode), "Log mode is not set.");
            }
        }

        public static void LogException(Exception exception)
        {
            switch (Mode)
            {
                case LogMode.UnityConsole:
                    Debug.LogException(exception);
                    break;

                case LogMode.SystemConsole:
                    Console.WriteLine($"EXCEPTION THROWN: {exception.Message}\n{exception.StackTrace}");
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Mode), "Log mode is not set.");
            }
        }
    }
}