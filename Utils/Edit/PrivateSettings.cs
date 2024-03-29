using Eternity.Configs.Logger;
using Eternity.Engine.Accounts;
using Eternity.Utils.Edit.Settings;
using System;
using System.Threading;
using VkNet;
using VkNet.Enums.StringEnums;
using VkNet.Model;

namespace Eternity.Utils.Edit {
    public class PrivateSettings {
        /// <summary>
        /// Установка настроек приватности профиля
        /// </summary>
        /// <param name="account"></param>
        public void SetPrivateSettings(Account account) {
            try {
                var api = new VkApi();
                api.Authorize(new ApiAuthParams { AccessToken = account.Token });

                api.Account.SetPrivacy(PrivacyKey.ClosedProfile, GetPrivate.EnabledClosedProfile ? "true" : "false");

                var searchByRegPhonePrivacy = GetPrivate.EnabledSearchByRegPhone ? GetPrivate.NoBody : GetPrivate.All;
                api.Account.SetPrivacy(PrivacyKey.Search_by_reg_phone, searchByRegPhonePrivacy);

                var value = GetPrivate.Values switch {
                    GetPrivate.Value.All => GetPrivate.All,
                    GetPrivate.Value.Friends => GetPrivate.Friends,
                    GetPrivate.Value.OnlyMe => GetPrivate.OnlyMe,
                    _ => string.Empty
                };

                foreach (var key in GetPrivate.Key) {
                    if (Enum.TryParse(key, out PrivacyKey privacyKey)) {
                        api.Account.SetPrivacy(privacyKey, value);
                    }
                }
            }
            catch (Exception ex) {
                Logger.Push("Ошибка при установке настроек приватности аккаунта: " + ex.Message);
            }
        }

        /// <summary>
        /// Запуск функции "Смена настроек приватности"
        /// </summary>
        public void AsyncWorker(Account account) {
            new Thread(() => SetPrivateSettings(account)).Start();
        }
    }
}
