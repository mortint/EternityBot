using Newtonsoft.Json;
using System.Collections.Generic;

namespace Eternity.Objects.Model.User {
    public class ResponsesUser {
        public class Users {
            /// <summary>
            /// Имя аккаунта
            /// </summary>
            [JsonProperty("first_name")] public string FirstName { get; set; }
            /// <summary>
            /// Фамилия аккаунта
            /// </summary>
            [JsonProperty("last_name")] public string LastName { get; set; }
        }
        /// <summary>
        /// Список пользователей, полученный в ответе от сервера.
        /// </summary>
        [JsonProperty("response")] public List<Users> UserResponse;
        /// <summary>
        /// Токен аккаунта
        /// </summary>
        [JsonProperty("access_token")] public string AccessToken;
    }
}
