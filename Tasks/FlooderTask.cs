using Eternity.Configs.Logger;
using Eternity.Engine.Accounts;
using Eternity.Engine.Helpers;
using Eternity.Enums.Exception;
using Eternity.Enums.Logging;
using Eternity.Enums.Target;
using Eternity.Targets;
using Eternity.Tasks.Settings;
using Eternity.Utils;
using Eternity.Utils.API;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace Eternity.Tasks {
    internal class FlooderTask {
        /// <summary>
        /// Сгенерировать сообщение
        /// </summary>
        private string GenerateMessage(Account acc, FlooderTarget ft) {
            var fs = acc.FlooderSettings;

            var message = fs.RandomPhrase();

            switch (fs.LocationName) {
                case "Начало":
                    return ft.Name + message;
                case "Конец":
                    return message + ft.Name;
                default:
                    return message;
            }
        }
        /// <summary>
        /// Отправить сообщений
        /// </summary>
        private void SendMessage(Account acc, FlooderTarget ft) {
            var fs = acc.FlooderSettings;
            var message = GenerateMessage(acc, ft);

            var reply = ft.Link.Contains(":") ? ft.Link.Split(':')[1] : string.Empty;

            switch (ft.Contains) {
                case "Текст":
                    SendTextMessage(acc, ft.Link, message, reply);
                    break;
                case "Текст+стикер":
                    SendTextWithStickerMessage(acc, ft.Link, message, reply, fs);
                    break;
                case "Текст+контент":
                    SendTextWithContentMessage(acc, ft.Link, message, reply, fs);
                    break;
                default:
                    Logger.Push($"\"{ft.Contains}\" — неверный формат содержимого", TypeLogger.File);
                    break;
            }
        }
        /// <summary>
        /// Отправка текстового сообщения
        /// </summary>
        private void SendTextMessage(Account acc, string target, string message, string reply) {
            var resp = string.Empty;

            var sendLink = LinksParser.Parse(target);
            var requestParams = string.Empty;

            if (string.IsNullOrEmpty(message))
                return;

            switch (sendLink.Type) {
                case TypeTarget.Chat:
                    requestParams = $"message={message}&chat_id={sendLink.Id1}&random_id=0";
                    break;
                case TypeTarget.User:
                    requestParams = $"message={message}&user_id={sendLink.Id1}&random_id=0";
                    break;
                case TypeTarget.Wall:
                    requestParams = $"owner_id={sendLink.Id1}&post_id={sendLink.Id2}&message={message}&reply_to_comment={reply}";
                    break;
                default:
                    Logger.Push($"'{target}' — некорректный формат цели");
                    return;
            }

            resp = Server.APIRequest(sendLink.Type == TypeTarget.Wall ? "wall.createComment" : "messages.send", requestParams, acc.Token);
            if (!resp.Contains("error")) {
                if (FlooderSettings.IsSendFalse)
                    Logger.Push($"постинг «{message}» в {target}", TypeLogger.File);
            }
        }
        /// <summary>
        /// Отправка сообщения + стикер
        /// </summary>
        private void SendTextWithStickerMessage(Account acc, string target, string message, string reply, FlooderSettings fs) {
            var sendLink = LinksParser.Parse(target);

            SendTextMessage(acc, target, message, reply);

            Thread.Sleep(700);

            Server.APIRequest(sendLink.Type == TypeTarget.Wall ? "wall.createComment" : "messages.send", $"sticker_id={fs.RandomStickers()}&{(sendLink.Type == TypeTarget.Chat ? "chat_id" : "user_id")}={sendLink.Id1}&random_id=0", acc.Token);
        }
        /// <summary>
        /// Отправка сообщения+вложение
        /// </summary>
        private void SendTextWithContentMessage(Account acc, string target, string message, string reply, FlooderSettings fs) {
            var resp = string.Empty;
            var sendLink = LinksParser.Parse(target);

            if (sendLink.Type == TypeTarget.Unknown) {
                Logger.Push($"\"{sendLink}\" — неверный формат ссылки", TypeLogger.File);
                return;
            }

            var requestParams = sendLink.Type switch {
                TypeTarget.Chat => $"message={message}&attachment={fs.RandomContains()}&chat_id={sendLink.Id1}&random_id=0",
                TypeTarget.User => $"message={message}&attachment={fs.RandomContains()}&user_id={sendLink.Id1}&random_id=0",
                TypeTarget.Wall => $"attachments={fs.RandomContains()}&owner_id={sendLink.Id1}&post_id={sendLink.Id2}&message={message}&reply_to_comment={reply}",
                _ => throw new NotImplementedException()
            };

            if (sendLink.Type == TypeTarget.Chat) {
                resp = Server.APIRequest("messages.send", requestParams, acc.Token);

                if (!IsMessageSentSuccessfully(resp)) {
                    Logger.Push("[VK API]: Не удалось отправить сообщение. Повторная попытка...");

                    if (IsRetryAllowed(resp)) {
                        var respRetry = Server.APIRequest("messages.send", requestParams, acc.Token);

                        if (!IsMessageSentSuccessfully(respRetry))
                            Logger.Push("[VK API]: Не удалось отправить сообщение во второй раз.");
                    }
                    else
                        Logger.Push("[VK API]: Повторная отправка невозможна из-за ошибки или сбоя.");
                }
            }
            else
                resp = Server.APIRequest(sendLink.Type == TypeTarget.Wall ? "wall.createComment" : "messages.send", requestParams, acc.Token);

            if (!resp.Contains("error")) {
                if (FlooderSettings.IsSendFalse)
                    Logger.Push($"постинг «{message}» в {target}", TypeLogger.File);
            }
        }
        bool IsMessageSentSuccessfully(string response) {
            return response.Contains("response");
        }
        bool IsRetryAllowed(string response) {
            if (response.Contains("error_code")) {
                JObject jsonResponse = JObject.Parse(response);
                int errorCode = (int)jsonResponse["error"]["error_code"];
                if (errorCode == 10)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Запуск функции
        /// </summary>
        public void Running(Account account) {
            var fs = account.FlooderSettings;
            var target = fs.Targets;
            fs.LoadSettings();

            if (target.Count == 0 && fs.Enabled) {
                Logger.Push($"[Флудер {account.Login}]: Отсутствуют цели...");
                return;
            }

            int iterIndex = -1;
            int delay = 0;

            while (Account.IsRunning && fs.Enabled) {
                iterIndex = (iterIndex + 1) % target.Count;

                try {
                    var fts = target[iterIndex];

                    SendMessage(account, fts);
                }
                catch (Exception ex) {
                    HandleException(ex);
                }

                switch (fs.SelectDelay) {
                    case "Обычная":
                        delay = StrWrk.IsInteger(fs.DelayMin, 333);
                        break;
                    case "Рандомная":
                        delay = new Random().Next(StrWrk.IsInteger(fs.DelayMin, 333), StrWrk.IsInteger(fs.DelayMax, 333));
                        break;
                }

                Thread.Sleep(delay);
            }
        }
        /// <summary>
        /// Вывод ошибки
        /// </summary>
        /// <param name="ex"></param>
        private void HandleException(Exception ex) {
            var exceptionType = GetExceptionType(ex);

            switch (exceptionType) {
                case ExceptionType.Kicked:
                case ExceptionType.Blocked:
                case ExceptionType.Other:
                    break;
                case ExceptionType.BrowserIssue:
                    Logger.Push("Возможно, был отвязан номер", TypeLogger.File);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Получить тип ошибки
        /// </summary>
        private ExceptionType GetExceptionType(Exception ex) {
            if (ex.Message.ToLower().Contains("this chat") || ex.Message.ToLower().Contains("kicked"))
                return ExceptionType.Kicked;
            else if (ex.Message.Contains("blocked"))
                return ExceptionType.Blocked;
            else if (ex.Message.ToLower().Contains("browser"))
                return ExceptionType.BrowserIssue;
            return ExceptionType.Other;
        }
    }
}
