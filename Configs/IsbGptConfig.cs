using Eternity.Interfaces;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Eternity.Configs {
    internal class IsbGptConfig : IConfig {
        [JsonProperty("url")] public Uri Uri { get; set; }

        public void Save() => File.WriteAllText(Path.Combine("Configs", "gptConfig.json"), JsonConvert.SerializeObject(this, Formatting.Indented));
    }
}
