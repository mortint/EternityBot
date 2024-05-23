using Newtonsoft.Json;

namespace Eternity.Objects.Model.Conversations {
    public class Items {
        [JsonProperty("conversation")]
        public Conversations Conversation { get; set; }
    }
}
