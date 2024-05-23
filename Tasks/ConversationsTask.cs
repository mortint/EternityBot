using Eternity.Configs.Logger;
using Eternity.Engine.Accounts;
using Eternity.Engine.Helpers;
using Eternity.Enums.Logging;
using Eternity.Targets.Chat;
using Eternity.Utils.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;

namespace Eternity.Tasks {
    public class ConversationsTask {
        private void ProcConversations(Account account, ConversationsTarget ct) {
            var cs = account.ConversationsSettings;
            if (cs.Targets.Count != 0) {
                var js = new JavaScriptSerializer();
                var chatId = ct.ChatId;
                var title = ct.Title;
                var method = ct.Method;

                var titles = File.ReadAllLines("Files\\titles.txt").ToList();
                chatId.Substring(0, chatId.Length - 1);
                var response = Server.APIRequest("messages.getChat", $"chat_ids={chatId}", account.Token);
                var parseResponses = StrWrk.QSubstr(response, "\"response\":", false);

                parseResponses = parseResponses.Substring(0, parseResponses.Length - 1);
                var chatInfo = js.Deserialize<List<ChatInfo>>(parseResponses).GetEnumerator();
                var execute = new ExecuteManager(account.Token);

                while (chatInfo.MoveNext()) {
                    if (method.Contains("Удалять") && chatInfo.Current.photo_50 != null) {
                        execute.Add("API.messages.deleteChatPhoto({\"chat_id\":" + chatId + "});");
                        Logger.Push($"Фото чата [{chatId}] удалено");
                    }
                    if (method.Contains("Менять") && title != "" && title != chatInfo.Current.title) {
                        execute.Add("API.messages.editChat({\"chat_id\":" + chatId +
                           ", \"title\":\"" + title.Replace("\"", "\\\"") + "\"});");
                        Logger.Push($"Название чата №{chatId} изменено на \"{title}\"");
                    }

                    if (method.Contains("Флуд")) {
                        if (titles.Count == 0)
                            Logger.Push("Отсутствуют названия для флуда");
                        else {
                            var updateTitle = titles[new Random().Next(titles.Count)];
                            execute.Add("API.messages.editChat({\"chat_id\":" + chatId +
                               ", \"title\":\"" + updateTitle.Replace("\"", "\\\"") + "\"});");
                            Logger.Push($"Название чата №{chatId} изменено на \"{updateTitle}\"");
                        }
                    }
                }

                execute.Execute();
            }
        }

        public void Running(Account account) {
            var cs = account.ConversationsSettings;
            var target = cs.Targets;
            int index = -1;

            if (target.Count == 0) {
                Logger.Push("[Беседы]: Отсутствуют цели...");
                return;
            }


            while (Account.IsRunning && cs.Enabled) {
                try {
                    index = (index + 1) % target.Count;
                    var css = target[index];
                    ProcConversations(account, css);
                }
                catch (Exception ex) {
                    Logger.Push("[Беседы]: " + ex.Message, TypeLogger.File);
                }

                Thread.Sleep(StrWrk.IsInteger(cs.Delay, 333));
            }
        }
    }
}
