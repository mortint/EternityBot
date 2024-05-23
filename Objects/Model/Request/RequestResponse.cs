using Newtonsoft.Json;

namespace Eternity.Objects.Model.Request {
    public class RequestResponse {
        [JsonProperty("key")] public string Key { get; set; }

        [JsonProperty("value")] public string Value { get; set; }
    }
}
