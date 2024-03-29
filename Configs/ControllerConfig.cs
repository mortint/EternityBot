using Newtonsoft.Json;
using System.IO;

namespace Eternity.Configs {
    internal static class ControllerConfig {
        /// <summary>
        /// Объект CaptchaConfig для работы с конфигурацией настроек капчи
        /// </summary>
        public static CaptchaConfig CaptchaConfig;
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
        }
    }
}
