using Eternity.Configs.Logger;
using Eternity.Engine.Accounts.Enums;
using Eternity.Tasks;
using Eternity.Tasks.Settings;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Eternity.Engine.Accounts {
    public sealed class Account {
        /// <summary>
        /// Объект FlooderSettings для работы с параметрами флудера
        /// </summary>
        public FlooderSettings FlooderSettings;
        /// <summary>
        /// Объект ConversationsSettings для работы с параметрами чатов
        /// </summary>
        public ConversationsSettings ConversationsSettings;
        /// <summary>
        /// Флаг запущен ли бот
        /// </summary>
        public static bool IsRunning { get; set; }
        /// <summary>
        /// Логин аккаунта
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль аккаунта
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Токен аккаунта
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Информация об аккаунте
        /// </summary>
        public string Info { get; set; }

        public string TimeMinutes, CountTarget;

        ~Account() => GC.Collect(2);
        public Account() {
            ConversationsSettings = new ConversationsSettings();
            FlooderSettings = new FlooderSettings();
        }
        /// <summary>
        /// Авторизация
        /// </summary>
        /// <returns></returns>
        public async Task<AuthOfStatus> Authorize() {
            try {
                var am = new AuthManager {
                    Login = Login,
                    Password = Password
                };

                await Task.Run(() => am.Authorize());
                Token = am.Token;
                Info = am.Info;

                if (am.VkApi != null && !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(Info)) {
                    Save();
                    return AuthOfStatus.Ok;
                }
                else {
                    return AuthOfStatus.Invalid;
                }
            }
            catch (Exception ex) {
                Logger.Push($"Ошибка при авторизации аккаунта — {Login}: {ex.Message}", TypeLogger.File);
                return AuthOfStatus.Other;
            }
        }
        /// <summary>
        /// Сохранение всех параметров
        /// </summary>
        public void Save()
            => File.WriteAllText(Path.Combine("AccsData", $"{Login}.json"), JsonConvert.SerializeObject(this, Formatting.Indented));

        private Thread _threadingFlooder { get; set; }

        private Thread _threadingConversations { get; set; }

        /// <summary>
        /// Запуск активных функций
        /// </summary>
        public void AsyncWorker() {
            if (_threadingFlooder != null && _threadingFlooder.IsAlive)
                _threadingFlooder.Join();
            if (_threadingConversations != null && _threadingConversations.IsAlive)
                _threadingConversations.Join();

            var flooder = new FlooderTask();
            var conversations = new ConversationsTask();

            if (FlooderSettings.Enabled) {
                _threadingFlooder = new Thread(() => flooder.Running(this)) { IsBackground = true };
                _threadingFlooder.Start();
            }

            if (ConversationsSettings.Enabled) {
                _threadingConversations = new Thread(() => conversations.Running(this)) { IsBackground = true };
                _threadingConversations.Start();
            }
        }
        public void StopThreads() {
            _threadingFlooder?.Abort();
            _threadingConversations?.Abort();
        }
    }
}
