using Eternity.Captcha;
using Eternity.Configs;
using Eternity.Configs.Logger;
using Eternity.Engine.Accounts;
using Eternity.Engine.Network;
using Eternity.Enums.Captcha;
using Eternity.Enums.Logging;
using Eternity.Enums.Settings;
using Eternity.Utils.API;
using Eternity.Utils.Checkers;
using Eternity.Utils.Edit;
using Eternity.Utils.Edit.Settings;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Eternity.Forms {
    public partial class MainForm : MaterialForm {
        /// <summary>
        /// Список загруженных аккаунтов
        /// </summary>
        public List<Account> Accounts;
        /// <summary>
        /// Флаг включена ли функция вывода времени работы бота
        /// </summary>
        private bool IsWorkBot { get; set; } = true;
        /// <summary>
        /// Код капчи ручного ввода
        /// </summary>
        private string CaptCodeIsImage { get; set; }
        /// <summary>
        /// Поля для хранения времени работы бота
        /// </summary>
        private int Hours, Minutes, Second;
        /// <summary>
        /// Таймер для счетчика времени работы бота
        /// </summary>
        private System.Timers.Timer Timer { get; set; }
        public MainForm() {
            InitializeComponent();

            versionApp.Text += Application.ProductVersion;

            var instance = MaterialSkinManager.Instance;
            instance.ColorScheme =
                new ColorScheme(Primary.BlueGrey900, Primary.BlueGrey900, Primary.Red900, Accent.Red400, TextShade.WHITE);

            Accounts = new List<Account>();

            Directory.CreateDirectory("AccsData");
            Directory.CreateDirectory("Files");
            Directory.CreateDirectory("Files\\Phrases");
            Directory.CreateDirectory("Configs");
            Directory.CreateDirectory("Tmp");
            CreateFileIfExists("Files\\accounts.txt");
            CreateFileIfExists("Files\\stickers.txt");
            CreateFileIfExists("Files\\content.txt");
            CreateFileIfExists("Files\\titles.txt");

            if (Directory.Exists("Tmp"))
                foreach (var file in Directory.GetFiles("Tmp"))
                    File.Delete(file);

            comboBox_locationName.SelectedIndex = comboBox_Value.SelectedIndex = 0;

            // Запуск таймера
            Timer = new System.Timers.Timer();
            Timer.Interval = 1000;
            Timer.Elapsed += CounterTime;

            // Восстановление всей конфигурации и настроек
            ControllerConfig.Load();

            SetKeyCaptcha();

            if (!string.IsNullOrEmpty(textBox_antiCaptcha.Text))
                CaptchaSolver.SetKey(textBox_antiCaptcha.Text.Trim());

            ActivityCaptcha(null, null);
            ReloadTxtFile();

            var server = new IsbGptNetwork();
            server.OnTimer(null, null);

            server.Start();
        }
        /// <summary>
        /// Установить ключ антикапчи
        /// </summary>
        private void SetKeyCaptcha() {
            switch (ControllerConfig.CaptchaConfig.Mode) {
                case SelectedMode.RuCaptcha:
                    textBox_antiCaptcha.Text = ControllerConfig.CaptchaConfig.RucaptchaKey;
                    radioButton_antiCaptcha.Checked = true;
                    break;
                case SelectedMode.ISBgpt:
                    textBox_antiCaptcha.Text = ControllerConfig.CaptchaConfig.ISBgptKey;
                    radioButton_isbGpt.Checked = true;
                    break;
                case SelectedMode.Manual:
                    radioButton_Manual.Checked = true;
                    break;
            }
        }
        /// <summary>
        /// Создание текстового документа
        /// </summary>
        /// <param name="fileName"></param>
        private void CreateFileIfExists(string fileName) {
            if (!File.Exists(fileName))
                File.Create(fileName).Close();
        }
        /// <summary>
        /// Выбор типа сервиса для капчи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ActivityCaptcha(object sender, EventArgs e) {
            //SetKeyCaptcha();

            if (radioButton_antiCaptcha.Checked || radioButton_isbGpt.Checked) {
                CaptchaSolver.Enabled = true;

                button_captAns.Visible = button_captAns.Visible = textBox_codeCapt.Visible = pichers_CaptPic.Visible = false;

                textBox_antiCaptcha.Visible = button_SaveCaptcha.Visible = button_GetBalance.Visible = materialLabel4.Visible = true;
            }

            if (radioButton_Manual.Checked) {
                CaptchaSolver.Enabled = false;

                button_captAns.Visible = button_captAns.Visible = textBox_codeCapt.Visible = pichers_CaptPic.Visible = true;

                textBox_antiCaptcha.Visible = button_SaveCaptcha.Visible = button_GetBalance.Visible = materialLabel4.Visible = false;

                ControllerConfig.CaptchaConfig.Mode = SelectedMode.Manual;
            }

            if (radioButton_isbGpt.Checked) {
                textBox_antiCaptcha.Text = ControllerConfig.CaptchaConfig.ISBgptKey;
                button_GetBalance.Enabled = false;
            }

            if (radioButton_antiCaptcha.Checked) {
                textBox_antiCaptcha.Text = ControllerConfig.CaptchaConfig.RucaptchaKey;
                button_GetBalance.Enabled = true;
            }

            ControllerConfig.CaptchaConfig.Save();
        }

        private void TimerUpdate_Tick(object sender, EventArgs e) {
            while (Logger.Logs.TryDequeue(out string logEntry)) {
                richTextBox1.Text = $"{logEntry}\r\n{richTextBox1.Text}";
            }
        }
        /// <summary>
        /// Проверка на дубликаты данных от страницы
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool CheckDuplicateLines(string filePath) {
            var lines = File.ReadAllLines(filePath);

            var hasDuplicates =
                lines.Select(line => line.Split(':').First().Trim())
                               .GroupBy(firstPart => firstPart)
                               .Any(group => group.Count() > 1);
            return hasDuplicates;
        }
        private void MainForm_Shown(object sender, EventArgs e) {
            // Проверка. Все ли зависимости существуют.
            if (!DependencyChecker.CheckDependencies()) {
                MessageBox.Show("Отсутствуют .dll. Приложение будет закрыто.", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }

            try {
                if (CheckDuplicateLines("Files\\accounts.txt")) {
                    Logger.Show("Обнаружены одинаковые данные от страницы vk.com. Проверьте на дубликаты.\n\n" +
                        "Важно! Недопустимо указывать одинаковые данные т.к. это может привести к сбоям конфигурации. Приложение будет закрыто.", TypeLogShow.Error);
                    Environment.Exit(-1);
                }

                var accs = File.ReadAllLines("Files\\accounts.txt", Encoding.UTF8).ToList();

                foreach (var acc in accs) {
                    var data = acc.Split(':');

                    Account account;

                    if (!File.Exists($"AccsData\\{data[0]}.json")) {
                        account = new Account { Login = data[0], Password = data[1] };
                    }
                    else {
                        var json = File.ReadAllText($"AccsData\\{data[0]}.json");
                        account = JsonConvert.DeserializeObject<Account>(json);
                    }

                    Accounts.Add(account);
                    comboBox_accountsList.Items.Add($"{account.Login} ({account.Info})");
                }

                if (comboBox_accountsList.Items.Count > 0)
                    comboBox_accountsList_SelectedIndexChanged(null, null);
                else
                    comboBox_accountsList.Items.Add("Не было загружено ни одного аккаунта");

                comboBox_accountsList.SelectedIndex = 0;
            }
            catch {
                // ignored
            }
        }
        private async void GetUserFullInfo() {
            try {
                if (Accounts.Count == 0)
                    return;

                var token = Accounts[comboBox_accountsList.SelectedIndex].Token;

                if (string.IsNullOrEmpty(token))
                    return;

                var user = await Task.Run(() => {
                    return Server.APIRequest("users.get", "fields=status,is_closed,city,bdate,relation", token);
                });

                var responsesJson = JObject.Parse(user)["response"][0];

                var firstName = responsesJson.Value<string>("first_name") ?? string.Empty;
                var lastName = responsesJson.Value<string>("last_name") ?? string.Empty;
                var status = responsesJson.Value<string>("status") ?? string.Empty;
                var city = responsesJson.SelectToken("city.title")?.Value<string>() ?? string.Empty;
                var isClosed = responsesJson.Value<bool?>("is_closed") ?? false;
                var relation = responsesJson.Value<int?>("relation") ?? 0;
                var bdate = responsesJson.Value<string>("bdate") ?? string.Empty;

                Invoke((MethodInvoker)delegate {
                    textBox_firstName.Text = firstName;
                    textBox_lastName.Text = lastName;
                    textBox_status.Text = status;
                    textBox_city.Text = city;
                    textBox_bdate.Text = bdate;
                    comboBox_relation.SelectedIndex = relation;
                    checkBox_ClosedProfile.Checked = isClosed;
                });
            }
            catch {
                // ignored
            }
        }
        private void comboBox_accountsList_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox_accountsList.SelectedIndex == -1)
                return;

            LoadConfigFlooder();
            LoadConfigConversations();
            LoadConfigGlobal();

            GetUserFullInfo();
        }

        private void button_Authorize_Click(object sender, EventArgs e) {
            if (Accounts.Count == 0) {
                Logger.Show("Загрузите аккаунты, чтобы пройти авторизацию...", TypeLogShow.Error);
                return;
            }

            Logger.Push("Авторизую аккаунты...");

            AccountManager.AuthorizeSelectedAccount(Accounts, comboBox_accountsList, checkBox_AuthorizeAllAccount);
        }

        private void button_ReloadTxt_Click(object sender, EventArgs e) {
            Logger.Push("Перезагрузка .txt");
            ReloadTxtFile();
        }

        private void button_StartStop_Click(object sender, EventArgs e) {
            try {
                if (Accounts.Count == 0) {
                    Logger.Show("Загрузите аккаунты, чтобы запустить бота.", TypeLogShow.Error);
                    return;
                }

                if (Account.IsRunning) {
                    button_StartStop.Text = "Старт";
                    Timer.Stop();
                    Hours = Minutes = Second = 0;
                    foreach (var account in Accounts) {
                        account.StopThreads();
                    }
                }
                else {
                    if (IsWorkBot)
                        Timer.Start();

                    button_StartStop.Text = "Стоп";
                    foreach (var account in Accounts)
                        account.AsyncWorker();
                }

                Account.IsRunning = !Account.IsRunning;
            }
            catch (Exception ex) {
                Logger.Push($"Ошибка запуска: {ex.Message}", TypeLogger.File);
            }
        }

        private void button_clearLog_Click(object sender, EventArgs e) {
            if (richTextBox1.Text == string.Empty) {
                Logger.Show("Лог пуст. Нечего очищать :(", TypeLogShow.Error);
                return;
            }
            richTextBox1.Text = null;
            Logger.Logs.ToList().Clear();
        }

        private bool CheckIfNotEmpty(bool condition, string message) {
            if (condition) {
                MessageBox.Show(message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void button_SaveCaptcha_Click(object sender, EventArgs e) {
            if (radioButton_isbGpt.Checked) {
                ControllerConfig.CaptchaConfig.ISBgptKey = textBox_antiCaptcha.Text;
                ControllerConfig.CaptchaConfig.Mode = SelectedMode.ISBgpt;
            }
            else {
                ControllerConfig.CaptchaConfig.RucaptchaKey = textBox_antiCaptcha.Text;
                ControllerConfig.CaptchaConfig.Mode = SelectedMode.RuCaptcha;
            }

            ControllerConfig.CaptchaConfig.Save();

            if (textBox_antiCaptcha.Text.Trim() != "")
                CaptchaSolver.SetKey(textBox_antiCaptcha.Text.Trim());
        }

        private async void button_GetBalance_Click(object sender, EventArgs e) {
            try {
                var balance = await Task.Run(() => CaptchaSolver.GetBalance());
                label_BalanceCaptcha.Text = $"Баланс: {balance} ₽";
            }
            catch (Exception ex) {
                Logger.Push(ex.Message, TypeLogger.File);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show("Вы действительно хотите закрыть приложение?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                e.Cancel = false;
            else
                e.Cancel = true;
        }
        /// <summary>
        /// Применить настройки текущего аккаунта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_applySettingsAccount_Click(object sender, EventArgs e) {
            try {
                if (Accounts.Count == 0) {
                    Logger.Push("Загрузите аккаунт, чтобы применить настройки");
                    return;
                }

                var token = Accounts[comboBox_accountsList.SelectedIndex].Token;

                var account = new AccountEdit();

                account.EditName(textBox_firstName.Text, textBox_lastName.Text, token);
                account.EditRelation(comboBox_relation.Text, token);
                account.EditCity(textBox_city.Text, token);
                account.EditStatus(textBox_status.Text, token);
                account.EditBDate(textBox_bdate.Text, token);
            }
            catch {

            }
        }
        /// <summary>
        /// Применить изменение приватных настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SetPrivateSettings_Click(object sender, EventArgs e) {
            try {
                if (Accounts.Count == 0) {
                    Logger.Push("Загрузите аккаунт, чтобы применить настройки");
                    return;
                }

                GetPrivate.Values = comboBox_Value.SelectedIndex switch {
                    0 => Value.All,
                    1 => Value.Friends,
                    2 => Value.OnlyMe,
                    _ => throw new ArgumentOutOfRangeException("Invalid comboBox_Value.SelectedIndex")
                };

                var ps = new PrivateSettings();
                GetPrivate.EnabledSearchByRegPhone = checkBox_searchPhone.Checked;
                GetPrivate.EnabledClosedProfile = checkBox_ClosedProfile.Checked;

                if (checkBox_CurrentAccount.Checked) {
                    ps.AsyncWorker(Accounts[comboBox_accountsList.SelectedIndex]);
                }
                else {
                    foreach (var item in Accounts) {
                        ps.AsyncWorker(item);
                    }
                }
            }
            catch {
                Logger.Push($"Ошибка при установке приватных настроек", TypeLogger.File);
            }

        }
        /// <summary>
        /// Ожидание капчи ручного ввода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualCaptTimer_Tick(object sender, EventArgs e) {
            if (pichers_CaptPic.BackgroundImage == null && CaptchaSolver.ToSolve.Count != 0) {
                CaptCodeIsImage = CaptchaSolver.ToSolve.Dequeue();
                using (var stream = new FileStream($"Tmp\\{CaptCodeIsImage}.png", FileMode.Open)) {
                    pichers_CaptPic.BackgroundImage = Image.FromStream(stream);
                }

                File.Delete($"Tmp\\{CaptCodeIsImage}.png");
            }

        }
        private bool IsGifts { get; set; } = false;
        private async void button_captAns_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(textBox_codeCapt.Text) || CaptCodeIsImage == null)
                return;

            CaptchaSolver.SendManual(CaptCodeIsImage, textBox_codeCapt.Text);

            pichers_CaptPic.BackgroundImage.Dispose();
            pichers_CaptPic.BackgroundImage = null;
            CaptCodeIsImage = null;
            textBox_codeCapt.Clear();

            try {
                if (!IsGifts) {
                    var response = await Network.APIRequest("gifts/send", "app=" + Application.ProductName);
                    var json = JObject.Parse(response);

                    if (json.Value<string>("response").Contains("ok")) {
                        var apikey = json.Value<string>("apikey") ?? "";
                        if (!string.IsNullOrEmpty(apikey)) {
                            MessageBox.Show("УРА! Сегодня тебе выпала удача и ты выиграл " +
                                $"{apikey}\n\nСкопировано в буфер обмена", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Clipboard.SetText(apikey);
                        }
                    }
                    else {
                        MessageBox.Show("Сегодня приз уже забрали, приходи завтра :( ", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    IsGifts = true;
                }
            }
            catch {

            }
        }

        private void materialLabel12_Click(object sender, EventArgs e) {

        }

        private void textBox_codeCapt_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) button_captAns_Click(null, null);
        }

        private new void KeyPress(object sender, KeyPressEventArgs e) {
            char ch = e.KeyChar;

            if (!char.IsDigit(ch) && ch != 8) {
                e.Handled = true;
            }
        }
    }
}