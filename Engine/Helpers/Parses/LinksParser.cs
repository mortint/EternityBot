using Eternity.Enums.Target;
using Eternity.Targets;
using System.Text.RegularExpressions;

namespace Eternity.Utils {
    public class LinksParser {
        private static readonly Regex _patternChat = new Regex(@"im\?sel=c([0-9]+)", RegexOptions.Compiled);
        private static readonly Regex _patternPm = new Regex(@"im\?sel=([0-9]+)", RegexOptions.Compiled);
        private static readonly Regex _patternWall = new Regex(@"wall(-?[0-9]+)_([0-9]+)", RegexOptions.Compiled);
        /// <summary>
        /// Метод для разбора ссылки и получения данных о цели
        /// </summary>
        internal static TargetData Parse(string link) {
            var match = _patternChat.Match(link);
            if (match.Success)
                return new TargetData(TypeTarget.Chat, match.Groups[1].Value, "0");

            match = _patternPm.Match(link);
            if (match.Success)
                return new TargetData(TypeTarget.User, match.Groups[1].Value, "0");

            match = _patternWall.Match(link);
            if (match.Success)
                return new TargetData(TypeTarget.Wall, match.Groups[1].Value, match.Groups[2].Value);

            return new TargetData(TypeTarget.Unknown);
        }
    }
}
