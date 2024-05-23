using Eternity.Enums.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eternity.Configs.Logger {
    internal static class Logger {
        internal static ConcurrentQueue<string> Logs;
        /// <summary>
        /// Флаг включен ли лог
        /// </summary>
        public static bool Enabled = true;
        /// <summary>
        /// Флаг включен ли лог в файл
        /// </summary>
        public static bool EnabledLogFile = false;

        static Logger() => Logs = new ConcurrentQueue<string>();
        /// <summary>
        /// Вывод логирования
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Push(string message, TypeLogger type = TypeLogger.Push) {
            var timeFormat = "dd.MM.yyyy, HH:mm:ss";
            var timeStamp = DateTime.Now.ToString(timeFormat);
            var logEntry = $"[{timeStamp}] {message}";

            if (Enabled)
                Logs.Enqueue(logEntry);

            if (EnabledLogFile && type == TypeLogger.File) {
                try {
                    Task.Run(() => {
                        var logFilePath = "Configs\\access.log";

                        if (!File.Exists(logFilePath))
                            File.WriteAllText(logFilePath, string.Empty);

                        var newLogEntry = $"[{DateTime.Now}] {message}";
                        var lines = new List<string> { newLogEntry };
                        lines.AddRange(File.ReadAllLines(logFilePath));

                        File.WriteAllLines(logFilePath, lines);
                    });
                }
                catch (Exception ex) {
                    Logs.Enqueue($"Ошибка записи лога в файл: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Логирование в MessageBox
        /// </summary>
        /// <param name="message"></param>
        /// <param name="typeLogShow"></param>
        public static void Show(string message, TypeLogShow typeLogShow = TypeLogShow.Info) {
            MessageBoxIcon icon = typeLogShow switch {
                TypeLogShow.Error => MessageBoxIcon.Error,
                TypeLogShow.Warning => MessageBoxIcon.Warning,
                TypeLogShow.Info => MessageBoxIcon.Information,
                _ => MessageBoxIcon.None
            };

            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, icon);
        }
    }
}
