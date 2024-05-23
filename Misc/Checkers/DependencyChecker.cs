using System.IO;
using System.Linq;

namespace Eternity.Utils.Checkers {
    internal static class DependencyChecker {
        /// <summary>
        /// Проверка на наличие зависимостей для работы приложения
        /// </summary>
        /// <returns></returns>
        public static bool CheckDependencies() {
            var path = "Library";

            string[] libs = {
                "FlatUI.dll",
                "HtmlAgilityPack.dll",
                "MaterialFramework.dll",
                "MaterialSkin.dll",
                "Microsoft.Bcl.AsyncInterfaces.dll",
                "Microsoft.Extensions.DependencyInjection.Abstractions.dll",
                "Microsoft.Extensions.DependencyInjection.dll",
                "Microsoft.Extensions.Logging.Abstractions.dll",
                "Newtonsoft.Json.dll",
                "protobuf-net.Core.dll",
                "protobuf-net.dll",
                "System.Buffers.dll",
                "System.Collections.Immutable.dll",
                "System.Memory.dll",
                "System.Numerics.Vectors.dll",
                "System.Runtime.CompilerServices.Unsafe.dll",
                "System.Text.Encoding.CodePages.dll",
                "System.Threading.Tasks.Extensions.dll",
                "VkNet.AudioBypassService.dll",
                "VkNet.dll"
            };

            bool IsExists() {
                return libs.Select(lib => Path.Combine(path, lib))
                    .All(dllPath => File.Exists(dllPath));
            }

            return IsExists();
        }
    }
}
