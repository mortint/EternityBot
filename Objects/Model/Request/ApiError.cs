using Newtonsoft.Json;

namespace Eternity.Objects.Model.Request {
    public class ApiError {
        /// <summary>
        /// Текст ошибки
        /// </summary>
        [JsonProperty("error_msg")] public string ErrorMessage { get; set; }

        /// <summary>
        /// Изображение капчи
        /// </summary>
        [JsonProperty("captcha_img")] public string CaptchaImage { get; set; }

        /// <summary>
        /// Код с картинки
        /// </summary>
        [JsonProperty("captcha_sid")] public long CaptchaSid { get; set; }

        [JsonProperty("request_params")] public RequestResponse[] Response { get; set; }
    }
}
