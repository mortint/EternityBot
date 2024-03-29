using Eternity.Configs.Logger;
using Eternity.Engine.Accounts.Enums;
using FlatUI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Eternity.Engine.Accounts {
    /// <summary>
    /// Класс для работы с авторизацией аккаунтов
    /// </summary>
    internal static class AccountManager {
        /// <summary>
        /// Метод для авторизации одного или нескольких аккаунтов
        /// </summary>
        public static void AuthorizeSelectedAccount(List<Account> accounts, ComboBox comboBox_accountsList, FlatCheckBox checkBox_AuthorizeAllAccount) {
            try {
                if (!checkBox_AuthorizeAllAccount.Checked)
                    AuthorizeSingleAccount(accounts, comboBox_accountsList);
                else
                    AuthorizeAllAccounts(accounts, comboBox_accountsList);
            }
            catch (Exception ex) {
                Logger.Push($"Ошибка при авторизации: {ex.Message}", TypeLogger.File);
            }
        }

        /// <summary>
        /// Метод для авторизации одного аккаунта
        /// </summary>
        private static async void AuthorizeSingleAccount(List<Account> accounts, ComboBox comboBox_accountsList) {
            var selectedAccount = accounts[comboBox_accountsList.SelectedIndex];
            selectedAccount.Save();

            var authStatus = await selectedAccount.Authorize();

            if (authStatus == AuthOfStatus.Ok) {
                comboBox_accountsList.Items[comboBox_accountsList.SelectedIndex] = $"{selectedAccount.Login} ({selectedAccount.Info})";
                Logger.Push($"Авторизация завершена: {selectedAccount.Login}", TypeLogger.File);
            }
        }

        /// <summary>
        /// Метод для авторизации всех аккаунтов
        /// </summary>
        private static async void AuthorizeAllAccounts(List<Account> accounts, ComboBox comboBox_accountsList) {
            comboBox_accountsList.Items.Clear();

            foreach (var account in accounts) {
                var authStatus = await account.Authorize();
                if (authStatus == AuthOfStatus.Ok) {
                    comboBox_accountsList.Items.Add($"{account.Login} ({account.Info})");
                    Logger.Push($"Авторизация завершена: {account.Login}", TypeLogger.File);
                    comboBox_accountsList.SelectedIndex = 0;
                }
            }
        }
    }

}
