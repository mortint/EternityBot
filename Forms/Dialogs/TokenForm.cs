using Eternity.Configs.Logger;
using Eternity.Engine.Accounts;
using Eternity.Engine.Helpers;
using Eternity.Enums.Logging;
using Eternity.Utils.API;
using FlatUI;
using MaterialSkin.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;

namespace Eternity.Forms.Dialogs {
    public partial class TokenForm : MaterialForm {
        public MainForm _form;

        public FlatComboBox _accounts;
        public TokenForm() {
            InitializeComponent();
        }

        private void TokenForm_Shown(object sender, System.EventArgs e) {
            try {
                var account = _form.Accounts[_accounts.SelectedIndex];
                textBox_Token.Text = account.Token;
            }
            catch {

            }
        }
        private void link_GetToken_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start("https://vkhost.github.io");
        }
        private async void button_Apply_Click(object sender, EventArgs e) {
            try {
                if (_form.Accounts.Count == 0) {
                    Logger.Show("Загрузите как минимум 1 аккаунт. Можно вписать в accounts.txt любые слова через двоеточие\n\n" +
                        "Например, тест:текст, а затем в этом окне вставить токен для авторизации.", TypeLogShow.Error);
                    return;
                }

                var token = textBox_Token.Text;

                if (token.Contains("access_token_")) {
                    Logger.Show("Возможно, Вы вставили токен от группы. Но формат неверный. Скопируйте от " +
                        "'<ТУТ ВАШ ID группы>=' и до конца.", TypeLogShow.Error);
                    return;
                }

                if (token.Contains("access_token="))
                    token = StrWrk.GetBetween(token, "access_token=", "&");

                var account = _form.Accounts[_accounts.SelectedIndex];

                using (var api = new VkApi()) {
                    api.Authorize(new ApiAuthParams {
                        AccessToken = token,
                        Settings = Settings.All
                    });

                    var group = await GetGroupInfo(api);
                    if (group != null && group.Id != 0)
                        UpdateAccountWithGroupInfo(account, group, token);
                    else
                        UpdateAccountWithUserInfo(account, GetUserInfo(token), token);

                    IsValidateToken("Токен изменен");
                }
            }
            catch (Exception ex) {
                Logger.Show(ex.ToString());
                label_statusToken.Text = "Токен неверный";
                label_statusToken.ForeColor = Color.Red;
            }
        }
        /// <summary>
        /// Получить информацию о группе
        /// </summary>
        private async Task<Group> GetGroupInfo(VkApi api) {
            try {
                return await Task.Run(() => api.Groups.GetById(null, null, null).FirstOrDefault());
            }
            catch {
                return new Group();
            }
        }
        /// <summary>
        /// Получить информацию о пользователе
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private JObject GetUserInfo(string token) {
            var response = Server.APIRequest("users.get", "", token);
            return JObject.Parse(response);
        }
        /// <summary>
        /// Обновить информацию о группе
        /// </summary>
        private void UpdateAccountWithGroupInfo(Account account, Group group, string token) {
            var infoGroup = $"ID {group.Id} ({group.Name})";
            _accounts.Items[_accounts.SelectedIndex] = account.Info = infoGroup;
            account.Token = token;
            account.Save();

            Logger.Show($"Группа {group.Name} активирована", TypeLogShow.Info);
        }
        /// <summary>
        /// Обновить информации об аккаунте
        /// </summary>
        private void UpdateAccountWithUserInfo(Account account, JObject userInfo, string token) {
            account.Token = token;
            account.Info = $"{userInfo["response"][0]["first_name"]} {userInfo["response"][0]["last_name"]}";
            account.Save();

            _accounts.Items[_accounts.SelectedIndex] = $"{account.Login} ({account.Info})";
        }

        /// <summary>
        /// Применить другой цвет к label, если успешно выполнено действие
        /// </summary>
        /// <param name="text"></param>
        private void IsValidateToken(string text) {
            label_statusToken.Text = text;
            label_statusToken.ForeColor = Color.Green;
        }
    }
}
