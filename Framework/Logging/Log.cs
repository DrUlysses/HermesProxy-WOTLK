using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using ThreadingState = System.Threading.ThreadState;

namespace Framework.Logging
{
    public enum LogType
    {
        Server,
        Network,
        Debug,
        Error,
        Warn,
        Storage
    }

    public enum LogNetDir // Network direction
    {
        C2P, // C>P S
        P2S, // C P>S
        S2P, // C P<S
        P2C, // C<P S
    }

    public static class Log
    {
        static Dictionary<LogType, (ConsoleColor Color, string Type)> LogToColorType = new()
        {
            { LogType.Debug,    (ConsoleColor.DarkBlue, " Debug   ") },
            { LogType.Server,   (ConsoleColor.Blue,     " Server  ") },
            { LogType.Network,  (ConsoleColor.Green,    " Network ") },
            { LogType.Error,    (ConsoleColor.Red,      " Error   ") },
            { LogType.Warn,     (ConsoleColor.Yellow,   " Warning ") },
            { LogType.Storage,  (ConsoleColor.Cyan,     " Storage ") },
        };

        static BlockingCollection<(LogType Type, string Message)> logQueue = new();
        private static Thread? _logOutputThread = null;
        public static bool IsLogging => _logOutputThread != null && !logQueue.IsCompleted;

        public static bool DebugLogEnabled { get; set; }

        private static string _logDir;
        private static StreamWriter _logWriter;
        private static readonly object _logWriterLock = new();

        /// <summary>
        /// Start the logging Thread and take logs out of the <see cref="BlockingCollection{T}"/>
        /// </summary>
        public static void Start()
        {
            InitFileLogging();

            if (_logOutputThread == null)
            {
                _logOutputThread = new Thread(() =>
                {
                    foreach (var msg in logQueue.GetConsumingEnumerable())
                    {
                        PrintInternalDirectly(msg.Type, msg.Message);
                    }
                });

                _logOutputThread.IsBackground = true;
                _logOutputThread.Start();
            }
        }

        private static void InitFileLogging()
        {
            try
            {
                _logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                Directory.CreateDirectory(_logDir);
                string logFile = Path.Combine(_logDir, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");
                _logWriter = new StreamWriter(logFile, append: true, encoding: Encoding.UTF8) { AutoFlush = true };
                _logWriter.WriteLine($"=== HermesProxy Log Started {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Log] Failed to init file logging: {ex.Message}");
            }
        }

        private static void WriteToFile(string line)
        {
            if (_logWriter == null) return;
            try
            {
                lock (_logWriterLock)
                {
                    _logWriter.WriteLine(line);
                }
            }
            catch { /* don't crash on log write failure */ }
        }

        private static void PrintInternalDirectly(LogType type, string text)
        {
            if (type == LogType.Debug && !DebugLogEnabled)
                return;

            string timestamp;
#if DEBUG
            timestamp = $"{DateTime.Now:HH:mm:ss.ff}";
#else
            timestamp = $"{DateTime.Now:HH:mm:ss}";
#endif
            string typeStr = LogToColorType[type].Type;
            string fullLine = $"{timestamp} |{typeStr}| {text}";

            // Write to file (always, no color codes)
            WriteToFile(fullLine);

            // Write to console (with color)
            Console.Write($"{timestamp} | ");
            Console.ForegroundColor = LogToColorType[type].Color;
            Console.Write($"{typeStr}");
            Console.ResetColor();
            Console.WriteLine($"| {text}");
        }

        /// <param name="type">Type of the log message</param>
        /// <param name="text">Text of the log message</param>
        /// <param name="method">Name of the method the log is called from</param>
        /// <param name="path">The relative path of the file.</param>
        public static void Print(LogType type, object text, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
        {
            string formattedText = $"{FormatCaller(method, path)} | {text}";
#if DEBUG
            // Fastpath when using breakpoints we want to see the log results immediately
            if (Debugger.IsAttached)
            {
                lock (logQueue)
                {
                    PrintInternalDirectly(type, formattedText);
                }
                return;
            }
#endif
            logQueue.Add((type, formattedText));
        }

        public static void PrintNet(LogType type, LogNetDir netDirection, object text, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
        {
            string directionText = netDirection switch
            {
                LogNetDir.C2P => "C>P S",
                LogNetDir.P2S => "C P>S",
                LogNetDir.S2P => "C P<S",
                LogNetDir.P2C => "C<P S",
            };
            Print(type, $"{directionText} | {text}", method, path);
        }

        public static void outException(Exception err, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
        {
            Print(LogType.Error, err.ToString(), method, path);
        }

        private static string FormatCaller(string method, string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            return fileName.PadRight(15, ' ');
        }
    }
}
