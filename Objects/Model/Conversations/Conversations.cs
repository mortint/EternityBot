using Newtonsoft.Json;

namespace Eternity.Objects.Model.Conversations {
    public class Conversations {
        [JsonProperty("chat_settings")] public SettingsChat ChatSettings { get; set; }

        [JsonProperty("peer")] public Peer Peers { get; set; }
    }
}
