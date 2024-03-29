using Newtonsoft.Json;
using System.IO;

namespace Eternity.Configs {
    /// <summary>
    /// Перечисление (выбор) сервиса капчи
    /// </summary>
    internal enum SelectedMode {
        Manual,
        RuCaptcha,
        ISBgpt
    }
    internal class CaptchaConfig {
        /// <summary>
        /// Ключ RuCaptcha
        /// </summary>
        [JsonProperty("rucaptcha_key")] public string RucaptchaKey { get; set; }
        /// <summary>
        /// Ключ ISB GPT
        /// </summary>
        [JsonProperty("isb_gpt_key")] public string ISBgptKey { get; set; }
        /// <summary>
        /// Выбранный сервис антикапчи
        /// </summary>
        [JsonProperty("mode")] public SelectedMode Mode { get; set; } = SelectedMode.Manual;
        /// <summary>
        /// Сохранение параметров
        /// </summary>
        public void Save() => File.WriteAllText(
            Path.Combine("Configs", "captcha.json"), JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}
