using Eternity.Enums.Settings;
using System.Collections.Generic;

namespace Eternity.Utils.Edit.Settings {
    /// <summary>
    /// Класс конфигурации настроек приватности
    /// </summary>
    public static class GetPrivate {
        /// <summary>
        /// Закрытый профиль
        /// </summary>
        public static string ClosedProfile { get; set; } = "closed_profile";
        /// <summary>
        /// Кто может найти мой профиль при импорте контактов
        /// </summary>
        public static string SearchByRegPhone { get; set; } = "search_by_reg_phone";
        /// <summary>
        /// Все пользователи
        /// </summary>
        public static string All { get; set; } = "all";
        /// <summary>
        /// Только я
        /// </summary>
        public static string OnlyMe { get; set; } = "only_me";
        /// <summary>
        /// Только друзья
        /// </summary>
        public static string Friends { get; set; } = "friends";
        /// <summary>
        /// Кто может найти мой профиль при импорте контактов
        /// </summary>
        public static string NoBody { get; set; } = "nobody";
        /// <summary>
        /// Перечисление прав доступа
        /// </summary>
        public static Value Values { get; set; }
        /// <summary>
        /// Закрыть/открыть профиль
        /// </summary>
        public static bool EnabledClosedProfile { get; set; }
        public static bool EnabledSearchByRegPhone { get; set; }

        public static List<string> Key
            = new List<string>
            {
                "profile",
                "photos_with",
                "photos_saved",
                "groups",
                "audios",
                "gifts",
                "places",
                "hidden_friends",
                "wall",
                "wall_send",
                "replies_view",
                "status_replies",
                "photos_tagme",
                "mail_send",
                "mini_apps_sympathy_button",
                "appscall",
                "groups_invite",
                "apps_invite",
                "friends_requests",
                "search_by_reg_phone",
                "chat_invite_user",
                "vkrun_steps",
                "games_feed",
                "mini_apps",
                "calls_ip",
            };
    }
}
