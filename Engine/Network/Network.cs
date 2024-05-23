using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Eternity.Engine.Network {
    internal static class Network {
        public static string GET(string url) {
            return new StreamReader(((HttpWebRequest)WebRequest.Create(url)).GetResponse().GetResponseStream()).ReadToEnd();
        }

        public static async Task<string> APIRequest(string method, string param) {
            using var http = new HttpClient();

            var response = await http.GetAsync($"https://gpt.isbsystem.ru/api/{method}?{param}");
            if (response.IsSuccessStatusCode) {
                return await response.Content.ReadAsStringAsync();
            }

            return null;
        }
    }
}
