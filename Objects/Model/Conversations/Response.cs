using Newtonsoft.Json;

namespace Eternity.Objects.Model.Conversations {
    public class Response {
        [JsonProperty("count")] public long Count { get; set; }

        [JsonProperty("items")] public Items[] Item { get; set; }
    }
}
