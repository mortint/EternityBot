﻿using Eternity.Configs.Logger;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VkNet;
using VkNet.AudioBypassService.Extensions;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace Eternity.Engine.Accounts {
    internal sealed class AuthManager {
        /// <summary>
        /// Поле для хранения информации об аккаунте
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// Поле для хранения логина
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Поле для хранения пароля
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Объект VK API для работы с VK API
        /// </summary>
        public VkApi VkApi { get; set; }
        /// <summary>
        /// Поле для хранение токена аккаунта
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Авторизация ВКонтакте
        /// </summary>
        public void Authorize() {
            try {
                var serv = new ServiceCollection();

                VkApi = new VkApi(serv.AddAudioBypass());
                var aup = new ApiAuthParams();

                if (Password.Contains("vk1")) {
                    aup = new ApiAuthParams {
                        AccessToken = Password,
                        Settings = Settings.All | Settings.Offline
                    };
                }
                else {
                    aup = new ApiAuthParams {
                        ApplicationId = 6121396,
                        Login = Login,
                        Password = Password,
                        Settings = Settings.All | Settings.Offline
                    };
                }

                VkApi.Authorize(aup);

                if (!string.IsNullOrEmpty(VkApi.Token) && VkApi != null) {
                    var user = VkApi.Account.GetProfileInfo();

                    Info = $"{user.FirstName} {user.LastName}";
                    Token = VkApi.Token;
                }
                else Token = null;
            }
            catch (Exception ex) {
                if (ex.Message.Contains("Group authorization")) {
                    Logger.Push($"[{Login}]: Обнаружен токен группы! Для авторизации сообществ - воспользуйтесь окношком 'По токену'...");
                }
                else
                    Logger.Push($"[{Login}]: " + ex.Message);
            }
        }
    }

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
