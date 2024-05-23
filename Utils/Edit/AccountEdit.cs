using Eternity.Configs.Logger;
using Eternity.Enums.Logging;
using Eternity.Utils.API;
using System;

namespace Eternity.Utils.Edit {
    public class AccountEdit {
        /// <summary>
        /// Изменение имени аккаунта
        /// </summary>
        /// <param name="firstName"></param>
        public void EditName(string firstName, string lastName, string token) {
            var response = string.Empty;

            try {
                var requestParameters = $"first_name={firstName}&last_name={lastName}";
                response = Server.APIRequest("account.saveProfileInfo", requestParameters, token);
                var message = GetResponseMessage(response);
                if (response.Contains("success"))
                    Logger.Show(message);
                else Logger.Show(message, TypeLogShow.Error);
            }
            catch {
                Logger.Show($"Ошибка при изменении имени: {GetResponseMessage(response)}", TypeLogShow.Error);
            }

            string GetResponseMessage(string response) {
                switch (response) {
                    case var s when s.Contains("success"):
                        return "Параметры успешно применены";
                    case var s when s.Contains("processing"):
                        return "Заявка рассматривается";
                    case var s when s.Contains("declined"):
                        return "Заявка отклонена";
                    case var s when s.Contains("was_accepted"):
                        return "Недавно уже была одобрена заявка";
                    case var s when s.Contains("was_declined"):
                        return "Недавно заявка была отклонена, повторно подать нельзя";
                    case var s when s.Contains("specified was missing or invalid"):
                        return "Недопустимое значение";
                    default:
                        return "Неизвестная ошибка";
                }
            }
        }
        /// <summary>
        /// Изменение семейного положения
        /// </summary>
        public void EditRelation(string text, string token) {
            try {
                var relationCode = GetRelationCode(text);

                if (relationCode == -1)
                    return;

                Server.APIRequest("account.saveProfileInfo", $"relation={relationCode}", token);
            }
            catch (Exception ex) {
                Logger.Show("Ошибка при изменении отношений: " + ex.Message, TypeLogShow.Error);
            }

            int GetRelationCode(string text) {
                switch (text) {
                    case "не женат/не замужем": return 1;
                    case "есть друг/есть подруга": return 2;
                    case "помолвлен/помолвлена": return 3;
                    case "женат/замужем": return 4;
                    case "всё сложно": return 5;
                    case "в активном поиске": return 6;
                    case "влюблён/влюблена": return 7;
                    case "в гражданском браке": return 8;
                    case "не указано": return 0;
                    default: return -1;
                }
            }
        }
        /// <summary>
        /// Изменение города
        /// </summary>
        public void EditCity(string city, string token) {
            try {
                if (!string.IsNullOrEmpty(city))
                    Server.APIRequest("account.saveProfileInfo", $"home_town={city}", token);
            }
            catch (Exception ex) {
                Logger.Show("Ошибка при изменении города: " + ex.Message, TypeLogShow.Error);
            }
        }

        /// <summary>
        /// Изменение статуса
        /// </summary>
        public void EditStatus(string text, string token) {
            try {
                if (!string.IsNullOrEmpty(text))
                    Server.APIRequest("account.saveProfileInfo", $"status={text}", token);
            }
            catch (Exception ex) {
                Logger.Show("Ошибка при изменении статуса: " + ex.Message, TypeLogShow.Error);
            }
        }
        /// <summary>
        /// Изменение даты рождения
        /// </summary>
        public void EditBDate(string text, string token) {
            try {
                if (!string.IsNullOrEmpty(text))
                    Server.APIRequest("account.saveProfileInfo", $"bdate={text}", token);
            }
            catch (Exception ex) {
                Logger.Show("Ошибка при изменении даты рождения: " + ex.Message, TypeLogShow.Error);
            }
        }
    }
}
