using Eternity.Captcha;
using Eternity.Configs.Logger;
using Eternity.Engine.Network;
using Eternity.Enums.Logging;
using Eternity.Objects.Model.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;

namespace Eternity.Utils.API {
    public static class Server {
        /// <summary>
        /// Поле для хранения версии VK API
        /// </summary>
        public static string Version = "5.131";
        /// <summary>
        /// Метод запроса к VK API
        /// </summary>
        /// <param name="method">Метод VK API</param>
        /// <param name="param">Передаваемые параметры</param>
        /// <param name="token">Токен VK</param>
        /// <param name="captcha_data">Капча</param>
        /// <returns></returns>
        public static string APIRequest(string method, string param, string token, string captcha_data = "") {
            if (string.IsNullOrEmpty(token)) {
                throw new ArgumentException("Invalid access token");
            }

            var response = string.Empty;

            while (true) {
                response = Network.GET($"https://api.vk.com/method/{method}?{param}&access_token={token}{captcha_data}&v={Version}");

                if (!response.Contains("\"error_code\":6"))
                    break;
                Thread.Sleep(333);
            }

            try {
                if (string.IsNullOrEmpty(response)) {
                    throw new Exception("Empty response from server");
                }

                if (response.Contains("\"error_code\":14")) {
                    var json = JsonConvert.DeserializeObject<ApiResponseModel>(response);
                    if (CaptchaSolver.Enabled) {
                        Logger.Push("Капча...");
                        var captchaKey = CaptchaSolver.Solve(json.Error.CaptchaImage);
                        response = Network.GET($"https://api.vk.com/method/{method}?{param}&access_token={token}&v={Version}&captcha_sid={json.Error.CaptchaSid}&captcha_key={captchaKey}");
                    }
                    else {
                        Logger.Push("Капча поставлена на ручной ввод");
                        var captchaKey = CaptchaSolver.SolveManual(json.Error.CaptchaImage, json.Error.CaptchaSid.ToString());
                        response = Network.GET($"https://api.vk.com/method/{method}?{param}&access_token={token}&v={Version}&captcha_sid={json.Error.CaptchaSid}&captcha_key={captchaKey}");
                    }
                }
                else if (response.Contains("\"error_code\":15")) {
                    var json = JObject.Parse(response);
                    var errorMsg = json["execute_errors"][0]["error_msg"].Value<string>();
                    throw new Exception($"[VK API - Беседы]: {errorMsg}");
                }
                else if (response.Contains("\"error_code\":100")) {
                    return string.Empty;
                }
                else if (response.Contains("\"error_code\"")) {
                    var json = JsonConvert.DeserializeObject<ApiResponseModel>(response);
                    throw new Exception($"[VK API chat={json.Error.Response[1].Value}]: {json.Error.ErrorMessage}");
                }
                else
                    return response;
            }
            catch (Exception ex) {
                Logger.Push(ex.Message, TypeLogger.File);
                throw;
            }

            return response;
        }
    }
}