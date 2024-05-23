using Eternity.Enums;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace Eternity.Engine.Helpers {
    public static class Internet {
        [DllImport("wininet.dll")] static extern bool InternetGetConnectedState(ref InternetConnectionState lpdwFlags, int dwReserved);

        /// <summary>
        /// Объект для синхронизации потоков
        /// </summary>
        static object _syncObj = new object();

        /// <summary>
        /// Метод для проверки интернет-соединения
        /// </summary>
        /// <returns></returns>
        public static bool CheckConnection() {
            lock (_syncObj) {
                try {
                    var flags = InternetConnectionState.INTERNET_CONNECTION_CONFIGURED | 0;
                    bool checkStatus = InternetGetConnectedState(ref flags, 0);

                    if (checkStatus) {
                        // Проверка доступности серверов
                        return PingServer(new string[]
                        {
                            @"ok.ru",
                            @"ya.ru",
                            @"vk.com"
                        });
                    }

                    return checkStatus;
                }
                catch {
                    return false;
                }
            }
        }

        /// <summary>
        /// Метод для проверки доступности серверов по списку
        /// </summary>
        private static bool PingServer(string[] serverList) {
            var haveAnInternetConnection = false;
            var ping = new Ping();

            for (int i = 0; i < serverList.Length; i++) {
                var pingReply = ping.Send(serverList[i]);
                haveAnInternetConnection = (pingReply.Status == IPStatus.Success);

                if (haveAnInternetConnection)
                    break;
            }

            return haveAnInternetConnection;
        }
    }
}
