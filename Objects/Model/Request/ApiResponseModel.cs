using Newtonsoft.Json;

namespace Eternity.Objects.Model.Request {
    public class ApiResponseModel {
        [JsonProperty("error")] public ApiError Error { get; set; }
    }
}
