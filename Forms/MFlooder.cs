using Eternity.Configs.Logger;
using Eternity.Engine.Helpers;
using Eternity.Enums.Logging;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eternity.Forms {
    public partial class MainForm {
        /// <summary>
        /// Восстановление настроек флудера
        /// </summary>
        public void LoadConfigFlooder() {
            try {
                var account = Accounts[comboBox_accountsList.SelectedIndex];
                var fs = account.FlooderSettings;

                dataGridView_flooder.Rows.Clear();

                foreach (var item in fs.Targets)
                    dataGridView_flooder.Rows.Add(item.Name, item.Link, item.Contains);

                checkBox_EnabledFlooder.Checked = fs.Enabled;
                numeric_DelayMaxFlooder.Text = fs.DelayMax;
                numeric_DelayMinFlooder.Text = fs.DelayMin;

                if (string.IsNullOrEmpty(fs.LocationName))
                    comboBox_locationName.SelectedIndex = 0;
                else
                    comboBox_locationName.Text = fs.LocationName;
                comboBox_SelectDelay.Text = fs.SelectDelay;

                comboBox_PhrasesSource.Text = fs.PhrasesFile;
            }
            catch {

            }
        }
        private void comboBox_SelectDelay_SelectedIndexChanged(object sender, EventArgs e) {
            if (comboBox_SelectDelay.SelectedIndex == 0) {
                numeric_DelayMaxFlooder.Enabled = false;
                numeric_DelayMaxFlooder.Text = "";
            }
            if (comboBox_SelectDelay.SelectedIndex == 1) {
                numeric_DelayMaxFlooder.Enabled = true;
            }
        }
        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_SaveFlood_Click(object sender, EventArgs e) {
            if (comboBox_accountsList.SelectedIndex == -1 || Accounts.Count == 0)
                return;

            if (!string.IsNullOrEmpty(numeric_DelayMaxFlooder.Text))
                if (int.Parse(numeric_DelayMinFlooder.Text) > int.Parse(numeric_DelayMaxFlooder.Text)) {
                    Logger.Show("Минимальный интервал не может быть больше максимального", TypeLogShow.Error);
                    return;
                }

            if (!CheckIfNotEmpty(dataGridView_flooder.Rows.Count == 1, "Заполните цели")
                || !CheckIfNotEmpty(string.IsNullOrEmpty(comboBox_SelectDelay.Text), "Заполните тип интервала")
                || (!string.IsNullOrEmpty(numeric_DelayMaxFlooder.Text)
                || !comboBox_SelectDelay.Text.Contains("Рандом"))
                && !CheckIfNotEmpty(string.IsNullOrEmpty(numeric_DelayMinFlooder.Text), "Заполните минимальный интервал")
                || !CheckIfNotEmpty(string.IsNullOrEmpty(comboBox_PhrasesSource.Text), "Заполните источник фраз") ||
                !CheckIfNotEmpty(string.IsNullOrEmpty(numeric_DelayMinFlooder.Text), "Укажите интервал")) {
                return;
            }

            var account = Accounts[comboBox_accountsList.SelectedIndex];
            var fs = account.FlooderSettings;

            fs.DelayMin = numeric_DelayMinFlooder.Text;
            fs.DelayMax = numeric_DelayMaxFlooder.Text;
            fs.Enabled = checkBox_EnabledFlooder.Checked;
            fs.PhrasesFile = comboBox_PhrasesSource.Text;
            fs.LocationName = comboBox_locationName.Text;
            fs.SelectDelay = comboBox_SelectDelay.Text;
            fs.ParseDataGridFlooder(dataGridView_flooder);

            account.Save();
            countTarget.Text = "Кол-во целей: " + account.FlooderSettings.Targets.Count;
            LeaveNumeric(null, null);
        }
        /// <summary>
        /// Применить автоцели
        /// </summary>
        private async void button_Apply_Click(object sender, EventArgs e) {
            try {
                var from = int.Parse(textBox_FromTarget.Text);
                var before = int.Parse(textBox_BeforeTarget.Text);
                var text = dataGridView_flooder.CurrentRow.Cells[0].Value?.ToString() ?? "";
                var contains = (dataGridView_flooder.CurrentRow.Cells[2].Value ?? "").ToString();

                if (string.IsNullOrEmpty(contains)) {
                    Logger.Show("Содержимое отсутствует. Установите, минимум, одну цель, чтобы применить автоцели", TypeLogShow.Error);
                    return;
                }

                if (from == 0) {
                    MessageBox.Show("Нельзя установить автоцели с 0", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (from > before) {
                    MessageBox.Show("Максимальная цель не должна превышать минимальную", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dataGridView_flooder.Invoke((MethodInvoker)delegate {
                    dataGridView_flooder.Rows.Clear();
                });

                textBox_BeforeTarget.Text = textBox_FromTarget.Text = "";

                await Task.Run(() => {
                    for (int i = from; i <= before; i++) {
                        var newUrl = $"im?sel=c{i}";
                        var newContains = contains;

                        dataGridView_flooder.Invoke((MethodInvoker)delegate {
                            dataGridView_flooder.Rows.Add(text, newUrl, newContains);
                        });
                    }
                });

                var account = Accounts[comboBox_accountsList.SelectedIndex];
                account.FlooderSettings.ParseDataGridFlooder(dataGridView_flooder);
                account.Save();
            }
            catch {
                MessageBox.Show("Не удалось установить автоцели...", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LeaveNumeric(object o, EventArgs e) {
            try {
                var account = Accounts[comboBox_accountsList.SelectedIndex];
                timeMinutesFlooder.Text = "Время в минутах (флудер): " + (StrWrk.IsInteger(numeric_DelayMinFlooder.Text, 333) / 60000) * account.FlooderSettings.Targets.Count + " мин.";
                account.TimeMinutes = timeMinutesFlooder.Text;
                account.CountTarget = countTarget.Text;
            }
            catch {

            }
        }
    }
}
