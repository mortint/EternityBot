using System.Collections.Generic;
using System.Text;

namespace Eternity.Utils.API {
    internal class ExecuteManager {
        private string Token;

        private List<string> Execs;

        public ExecuteManager(string token) {
            Token = token;
            Execs = new List<string>();
        }

        /// <summary>
        /// Добавление команды в список для выполнения, выполняется сразу при достижении 25 команд
        /// </summary>
        /// <param name="command"></param>
        public void Add(string command) {
            Execs.Add(command);
            if (Execs.Count == 25)
                ForceExecute();
        }

        /// <summary>
        /// Выполнение накопленных команд
        /// </summary>
        public void Execute() {
            if (Execs.Count > 0)
                ForceExecute();
        }

        /// <summary>
        /// Выполнение команд из списка
        /// </summary>
        private void ForceExecute() {
            var textBuilder = new StringBuilder("code=");

            foreach (string current in Execs) {
                textBuilder.Append(current);
            }

            textBuilder.Append("return 0;");

            string text = textBuilder.ToString();
            Server.APIRequest("execute", text, Token);

            Execs.Clear();
        }
    }

}
