using Newtonsoft.Json;

namespace Eternity.Objects.Model.Conversations {
    public class Peer {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("local_id")]
        public long LocalId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
