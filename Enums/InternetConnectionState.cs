using System;

namespace Eternity.Enums {
    [Flags]
    internal enum InternetConnectionState : int {
        INTERNET_CONNECTION_MODEM = 0x1,
        INTERNET_CONNECTION_LAN = 0x2,
        INTERNET_CONNECTION_PROXY = 0x4,
        INTERNET_RAS_INSTALLED = 0x10,
        INTERNET_CONNECTION_OFFLINE = 0x20,
        INTERNET_CONNECTION_CONFIGURED = 0x40
    }
}
