using System.IO;
using System.Net;

namespace Eternity.Engine.Network {
    internal static class Network {
        public static string GET(string url) {
            return new StreamReader(((HttpWebRequest)WebRequest.Create(url)).GetResponse().GetResponseStream()).ReadToEnd();
        }
    }
}
