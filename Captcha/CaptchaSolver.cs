using Eternity.Captcha.capLib;
using Eternity.Configs;
using Eternity.Configs.Logger;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Eternity.Captcha {
    internal static class CaptchaSolver {
        /// <summary>
        /// Флаг включена ли антикапча или нет
        /// </summary>
        public static bool Enabled = false;
        /// <summary>
        /// Объект для работы с RuCaptcha
        /// </summary>
        private static RuCaptchaClient CaptchaClient = null;
        /// <summary>
        /// Поле для хранение ключа антикапчи
        /// </summary>
        public static string ApiKey;
        /// <summary>
        /// Коллекция для хранение капчи руччного ввода
        /// </summary>
        public static Queue<string> ToSolve =
            new Queue<string>();
        /// <summary>
        /// Поле для работы с капчей ручного ввода
        /// </summary>
        private static readonly Dictionary<string, string> Answer =
            new Dictionary<string, string>();

        private static readonly Random _random =
            new Random();
        /// <summary>
        /// Объект HttpClient для выполнения HTTP запросов.
        /// </summary>
        private static readonly HttpClient _httpClient;
        static CaptchaSolver() => _httpClient = new HttpClient(new HttpClientHandler()) {
            BaseAddress = new Uri("http://185.103.109.185:3010/api/v1/solver"),
            Timeout = TimeSpan.FromMinutes(2)
        };

        private static (string result, double score, string key) Solver(MemoryStream image, string key) {
            var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StreamContent(image) {
                Headers = {
                    ContentType = new MediaTypeHeaderValue("image/jpeg")
                }
            }, "captcha", "captcha.jpg");

            requestContent.Add(new StringContent(key), "key");

            var request = new HttpRequestMessage {
                Method = HttpMethod.Post,
                Content = requestContent
            };

            var response = _httpClient.SendAsync(request).Result;

            var content = response.Content.ReadAsStringAsync().Result;
            var json = JObject.Parse(content);

            if (!json.Value<bool>("response"))
                return ("Error: " + json.Value<string>("message"), -1, key);

            var data = json.GetValue("data");
            if (data != null && data.HasValues) {
                var solved = data.Value<string>("solved");
                return (solved, -1, key);
            }
            else
                throw new Exception($"Error: {json.Value<string>("message")}");
        }
        /// <summary>
        /// Метод для решения капчи с помощтю выбранного сервиса
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string Solve(string url) {
            var stopwatch = new Stopwatch();

            var solved = string.Empty;
            var capId = string.Empty;

            var fileName = $"{_random.Next()}.png";
            var key = ControllerConfig.CaptchaConfig.RucaptchaKey;
            var rcc = new RuCaptchaClient(key);

            using (var wc = new WebClient()) {
                Directory.CreateDirectory("Tmp");

                wc.DownloadFile(url, $"Tmp\\{fileName}");
            }


            if (ControllerConfig.CaptchaConfig.Mode == SelectedMode.RuCaptcha) {
                bool timerElapsed = false;

                stopwatch.Start();
                capId = rcc.UploadCaptchaFile("Tmp\\" + fileName);

                while (!timerElapsed || string.IsNullOrEmpty(solved)) {
                    if (stopwatch.Elapsed.TotalSeconds >= 20)
                        timerElapsed = true;

                    if (timerElapsed) {
                        try {
                            solved = rcc.GetCaptcha(capId);

                            if (!string.IsNullOrEmpty(solved)) {
                                Logger.Push($"[RuCaptcha]: Капча {solved} решена за {stopwatch.Elapsed.TotalSeconds} сек.");
                            }
                        }
                        catch {

                        }
                    }

                    Thread.Sleep(1000);
                }

                stopwatch.Stop();

                return solved;
            }


            if (ControllerConfig.CaptchaConfig.Mode == SelectedMode.ISBgpt) {
                var apiKey = ControllerConfig.CaptchaConfig.ISBgptKey;

                var sw = new Stopwatch();
                sw.Start();

                var res = string.Empty;

                while (string.IsNullOrEmpty(res)) {
                    try {
                        double score;

                        (res, score, key) = Solver(new MemoryStream(File.ReadAllBytes("Tmp\\" + fileName)), apiKey);

                        sw.Stop();

                        Logger.Push($"[ISB GPT]: Капча [{res}] решена за {sw.Elapsed.TotalSeconds} сек.");

                        return res;
                    }
                    catch (Exception ex) {
                        Logger.Push($"[ISB GPT]: {ex.Message}");
                    }

                    Thread.Sleep(1000);
                }

                return res;
            }

            File.Delete($"Tmp\\{fileName}");

            return string.Empty;
        }
        /// <summary>
        /// Метод для установки ключа антикапчи
        /// </summary>
        /// <param name="key"></param>
        public static void SetKey(string key) {
            ApiKey = key;
            CaptchaClient = new RuCaptchaClient(key);
        }
        /// <summary>
        /// Метод для добавления в очередь капчи ручного ввода
        /// </summary>
        public static void SendManual(string key, string ans) {
            Answer.Add(key, ans);
        }
        /// <summary>
        /// Метод для решения капчи ручного ввода
        /// </summary>
        public static string SolveManual(string url, string id) {
            new WebClient().DownloadFile(url, $"Tmp\\{id}.png");
            ToSolve.Enqueue(id);

            while (!Answer.ContainsKey(id)) Thread.Sleep(1000);

            var ans = Answer[id];
            Answer.Remove(id);
            return ans;
        }
        /// <summary>
        /// Получение баланса ключа RuCaptcha
        /// </summary>
        /// <returns></returns>
        public static string GetBalance() {
            var res = string.Empty;

            if (CaptchaClient != null)
                res = CaptchaClient.GetBalance().ToString();
            else res = "?";

            return res;
        }
    }
}
