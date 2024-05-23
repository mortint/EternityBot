using Eternity.Engine.Accounts;
using Eternity.Objects.Model.Conversations;
using Eternity.Targets.Chat;
using Eternity.Utils.API;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Eternity.Tasks.Settings {
    public class ConversationsSettings {
        public string Delay { get; set; }
        public bool Enabled { get; set; }
        public List<ConversationsTarget> Targets { get; set; }

        public ConversationsSettings() => Targets = new List<ConversationsTarget>();

        public void ParseDataGrid(DataGridView view) {
            Targets.Clear();

            foreach (DataGridViewRow row in view.Rows) {
                if (row.Cells[0].Value != null)
                    Targets.Add(new ConversationsTarget {
                        ChatId = (row.Cells[0].Value ?? "").ToString(),
                        Title = (row.Cells[1].Value ?? "").ToString(),
                        Method = (row.Cells[2].Value ?? "").ToString()
                    });
            }
        }

        public ResponseConversation GetConversations(Account account) {
            var response = Server.APIRequest("messages.getConversations", "count=200&filter=all", account.Token);
            var json = JsonConvert.DeserializeObject<ResponseConversation>(response);
            return json;
        }
    }
}
