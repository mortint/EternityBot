using Eternity.Configs.Logger;
using Eternity.Engine.Accounts;
using Eternity.Engine.Helpers;
using Eternity.Enums.Logging;
using Eternity.Objects.Model.User;
using Eternity.Utils.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eternity.Forms {
    public partial class MainForm {
        /// <summary>
        /// Коллекция списка чатов
        /// </summary>
        private List<(long, string)> ChatsList;
        /// <summary>
        /// Восстановить настройки чатов
        /// </summary>
        public void LoadConfigConversations() {
            var account = Accounts[comboBox_accountsList.SelectedIndex];
            var cs = account.ConversationsSettings;

            dataGridView_conversations.Rows.Clear();

            foreach (var item in cs.Targets)
                dataGridView_conversations.Rows.Add(item.ChatId, item.Title, item.Method);

            checkBox_conversationsEnabled.Checked = cs.Enabled;
            numeric_DelayConversationsMin.Text = cs.Delay;
        }

        private void button_SaveConversations_Click(object sender, EventArgs e) {
            if (comboBox_accountsList.SelectedIndex == -1 || Accounts.Count == 0)
                return;

            var account = Accounts[comboBox_accountsList.SelectedIndex];
            var cs = account.ConversationsSettings;
            cs.Delay = numeric_DelayConversationsMin.Text;
            cs.Enabled = checkBox_conversationsEnabled.Checked;
            cs.ParseDataGrid(dataGridView_conversations);
            account.Save();
        }
        /// <summary>
        /// Получить последние чаты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button_GetConversations_Click(object sender, EventArgs e) {
            ChatsList = new List<(long, string)>();
            comboBox_ActiveChat.Items.Clear();

            try {
                ChatsList.Clear();
                var account = Accounts[comboBox_accountsList.SelectedIndex];
                var cs = account.ConversationsSettings;
                var conversations = cs.GetConversations(account).ResponseChat;

                Logger.Push($"Всего чатов на аккаунте: {conversations.Count}");

                if (conversations.Item.Count() == 0) {
                    Logger.Show("На аккаунте нет чатов...", TypeLogShow.Error);
                    return;
                }

                var tasks = conversations.Item
                    .Where(item => (item.Conversation.Peers.Type == "user" && checkBox_ConversationUser.Checked) ||
                                   (item.Conversation.Peers.Type == "chat" && checkBox_ConversationsChats.Checked))
                    .Select(item => {
                        if (item.Conversation.Peers.Type == "user")
                            return GetUserAndAddToComboBoxAsync(item.Conversation.Peers.Id, account);

                        if (item.Conversation.Peers.Type == "chat") {
                            var title = item.Conversation.ChatSettings.Title;
                            var localId = item.Conversation.Peers.LocalId;
                            ChatsList.Add((localId, title));
                        }

                        return Task.CompletedTask;
                    }).ToList();

                await Task.WhenAll(tasks);

                ChatsList.Sort();

                if (checkBox_ConversationsChats.Checked) {
                    foreach (var item in ChatsList) {
                        comboBox_ActiveChat.Items.Add($"im?sel=c{item.Item1} ({item.Item2})");
                    }
                }

                Logger.Push($"Получено чатов: {comboBox_ActiveChat.Items.Count}");

                comboBox_ActiveChat.SelectedIndex = 0;

                GC.Collect();
            }
            catch {
                Logger.Push("vk.com не может обработать запрос");
            }
        }
        /// <summary>
        /// Добавить последние чаты в ComboBox
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task GetUserAndAddToComboBoxAsync(long id, Account account) {
            var response = await Task.Run(() => Server.APIRequest("users.get", $"user_ids={id}", account.Token));
            var json = JsonConvert.DeserializeObject<ResponsesUser>(response);
            comboBox_ActiveChat.Items.Add($"im?sel={id} ({json.UserResponse[0].FirstName} {json.UserResponse[0].LastName})");
        }
        private void comboBox_ActiveChat_Click(object sender, EventArgs e) {
            try {
                Clipboard.SetText(comboBox_ActiveChat.Text.Replace(StrWrk.GetBetween(comboBox_ActiveChat.Text, "(", ")"), "").Replace("()", ""));
            }
            catch {
                // ignored
            }
        }
    }
}
