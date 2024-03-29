using Eternity.Configs.Logger;
using Eternity.Forms.Dialogs;
using Eternity.Tasks.Settings;
using System;
using System.IO;
using System.Timers;

namespace Eternity.Forms {
    public partial class MainForm {
        /// <summary>
        /// Восстановить настройки 
        /// </summary>
        public void LoadConfigGlobal() {
            ReloadTxtFile();

            var account = Accounts[comboBox_accountsList.SelectedIndex];

            countTarget.Text = account.CountTarget;
            timeMinutesFlooder.Text = account.TimeMinutes;
        }
        /// <summary>
        /// Загрузить данные из файлов
        /// </summary>
        private void ReloadTxtFile() {
            comboBox_PhrasesSource.Items.Clear();

            foreach (var item in Directory.GetFiles("Files\\Phrases"))
                comboBox_PhrasesSource.Items.Add(Path.GetFileName(item));

            if (comboBox_PhrasesSource.Items.Count > 0)
                comboBox_PhrasesSource.SelectedIndex = 0;
            comboBox_appId.SelectedIndex = 0;
        }
        /// <summary>
        /// Счетчик времени (сколько включен бот)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CounterTime(object sender, ElapsedEventArgs e) {
            Invoke(new Action(() => {
                Second += 1;
                if (Second == 60) {
                    Second = 0;
                    Minutes += 1;
                }
                if (Minutes == 60) {
                    Minutes = 0;
                    Hours += 1;
                }

                WorksTime.Text =
                    $"Время работы: {Hours.ToString().PadLeft(2, '0')}:{Minutes.ToString().PadLeft(2, '0')}:{Second.ToString().PadLeft(2, '0')}";
            }));
        }
        /// <summary>
        /// Включить или отключить логирование
        /// </summary>
        /// <param name="sender"></param>
        private void checkBox_loggerOfFile_CheckedChanged(object sender) {
            if (checkBox_loggerOfFile.Checked)
                Logger.EnabledLogFile = true;
            else
                Logger.EnabledLogFile = false;
        }
        /// <summary>
        /// Выбор приложения авторизации
        /// </summary>
        private void comboBox_appId_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox_appId.SelectedIndex == 1) {
                var tokenForm = new TokenForm();
                tokenForm._form = this;
                tokenForm._accounts = comboBox_accountsList;
                tokenForm.Show(this);
            }
        }
        /// <summary>
        /// Отключение логирования
        /// </summary>
        private void checkBox_LogOff_CheckedChanged(object sender) {
            Logger.Enabled = !checkBox_LogOff.Checked;
            Logger.Logs = new System.Collections.Concurrent.ConcurrentQueue<string>();
        }
        /// <summary>
        /// Отключить или включить счетчик времени 
        /// </summary>
        /// <param name="sender"></param>
        private void checkBox_isWorkBot_CheckedChanged(object sender) {
            if (!checkBox_isWorkBot.Checked)
                Timer.Start();
            else Timer.Stop();

            IsWorkBot = !IsWorkBot;
        }
        /// <summary>
        /// Включить или отключить вывод отправленных сообщений
        /// </summary>
        /// <param name="sender"></param>
        private void checkBox_SendMessageFalse_CheckedChanged(object sender) {
            if (checkBox_SendMessageFalse.Checked)
                FlooderSettings.IsSendFalse = false;
            else
                FlooderSettings.IsSendFalse = true;
        }
    }
}
