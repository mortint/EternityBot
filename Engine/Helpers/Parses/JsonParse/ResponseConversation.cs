using Newtonsoft.Json;
using System;

namespace Eternity.Engine.Helpers.Parses.JsonParse {
    public class ResponseConversation {
        /// <summary>
        /// Финализатор класса для явного вызова сборщика мусора
        /// </summary>
        ~ResponseConversation() => GC.Collect(2);

        /// <summary>
        /// Вложенный класс Response для хранения данных ответа
        /// </summary>
        public class Response {
            /// <summary>
            /// Вложенный класс Items для хранения коллекции элементов
            /// </summary>
            public class Items {
                /// <summary>
                ///  Вложенный класс Conversations для хранения информации о чате
                /// </summary>
                public class Conversations {
                    /// <summary>
                    /// Вложенный класс Peer для хранения информации о собеседнике
                    /// </summary>
                    public class Peer {
                        /// <summary>
                        /// ID пользователя
                        /// </summary>
                        [JsonProperty("id")] public long Id { get; set; }
                        /// <summary>
                        /// Локальный ID
                        /// </summary>
                        [JsonProperty("local_id")] public long LocalId { get; set; }
                        /// <summary>
                        /// Тип
                        /// </summary>
                        [JsonProperty("type")] public string Type { get; set; }
                    }

                    /// <summary>
                    /// Вложенный класс SettingsChat для хранения настроек чата
                    /// </summary>
                    public class SettingsChat {
                        /// <summary>
                        /// Название чата
                        /// </summary>
                        [JsonProperty("title")] public string Title { get; set; }
                    }
                    /// <summary>
                    /// Настройки чата
                    /// </summary>
                    [JsonProperty("chat_settings")] public SettingsChat ChatSettings { get; set; }
                    /// <summary>
                    /// Настройки чата
                    /// </summary>
                    [JsonProperty("peer")] public Peer Peers { get; set; }
                }
                /// <summary>
                /// Список чатов
                /// </summary>
                [JsonProperty("conversation")] public Conversations Conversation { get; set; }
            }
            /// <summary>
            /// Количество чатов
            /// </summary>
            [JsonProperty("count")] public long Count { get; set; }
            /// <summary>
            /// Информация о чате (хранит в себе все параметры чата)
            /// </summary>
            [JsonProperty("items")] public Items[] Item { get; set; }
        }
        /// <summary>
        /// Ответ сервера при успешном выполнении
        /// </summary>
        [JsonProperty("response")] public Response ResponseChat { get; set; }
    }
}
