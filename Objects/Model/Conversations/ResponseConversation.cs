using Newtonsoft.Json;
using System;

namespace Eternity.Objects.Model.Conversations {
    public class ResponseConversation {
        [JsonProperty("response")] public Response ResponseChat { get; set; }

        ~ResponseConversation() => GC.Collect(2);
    }
}
