
namespace Eternity.Engine.Helpers {
    internal static class StrWrk {
        /// <summary>
        /// Возвращает подстроку до или после указанной подстроки в зависимости от флага before
        /// </summary>
        public static string QSubstr(string str, string substr, bool before = false) {
            if (before)
                return str.Substring(0, str.IndexOf(substr));

            return str.Substring(str.IndexOf(substr) + substr.Length);
        }

        /// <summary>
        /// Возвращает подстроку между двумя заданными подстроками left и right
        /// </summary>
        public static string GetBetween(string str, string left, string right) {
            return QSubstr(QSubstr(str, left, false), right, true);
        }

        /// <summary>
        /// Преобразует строку в целое число, при этом устанавливает минимальное значение delay
        /// </summary>
        public static int IsInteger(string value, int delay) {
            var result = 0;

            if (int.TryParse(value, out result)) {
                return result > delay ? result : delay;
            }

            return delay;
        }
    }
}
