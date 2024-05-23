using Eternity.Configs;
using System;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Eternity.Engine.Network {
    internal class IsbGptNetwork {
        private Timer Timer;

        internal IsbGptNetwork() {
            Timer = new Timer { Interval = 18000000 };
            Timer.Elapsed += OnTimer;
        }

        public void OnTimer(object sender, ElapsedEventArgs e) {
            try {
                var response = Network.GET(Globals.IsbGptUrl.ToString());
                var url = ControllerConfig.IsbGptConfig.Uri;

                if (url == new Uri(response))
                    return;

                ControllerConfig.IsbGptConfig.Uri = new Uri(response);
                ControllerConfig.IsbGptConfig.Save();
            }
            catch (Exception) {
                // ignored
            }
        }

        internal void Start() {
            if (!Timer.Enabled)
                Timer.Start();
        }
    }
}
