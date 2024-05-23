using Newtonsoft.Json;
using System.IO;

namespace Eternity.Configs {
    internal static class ControllerConfig {
        /// <summary>
        /// Объект CaptchaConfig для работы с конфигурацией настроек капчи
        /// </summary>
        public static CaptchaConfig CaptchaConfig;
        public static IsbGptConfig IsbGptConfig;
        /// <summary>
        /// Десериализация параметров капчи
        /// </summary>
        public static void Load() {
            try {
                CaptchaConfig = JsonConvert.DeserializeObject<CaptchaConfig>(File.ReadAllText(Path.Combine("Configs", "captcha.json")));
            }
            catch {
                CaptchaConfig = new CaptchaConfig();
            }

            try {
                IsbGptConfig = JsonConvert.DeserializeObject<IsbGptConfig>(File.ReadAllText(Path.Combine("Configs", "gptConfig.json")));
            }
            catch {
                IsbGptConfig = new IsbGptConfig();
            }
        }
    }
}
