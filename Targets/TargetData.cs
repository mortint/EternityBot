using Eternity.Enums.Target;

namespace Eternity.Targets {
    /// <summary>
    /// Внутренняя структура данных для хранения информации о цели ссылки
    /// </summary>
    internal struct TargetData {

        // Поля структуры для хранения типа и идентификаторов цели
        public readonly TypeTarget Type;

        public string Id1, Id2;

        /// <summary>
        /// Конструктор для структуры TargetData с двумя идентификаторами
        /// </summary>
        public TargetData(TypeTarget type, string id1, string id2) {
            Type = type;
            Id1 = id1;
            Id2 = id2;
        }

        /// <summary>
        /// Конструктор для структуры TargetData с одним идентификатором
        /// </summary>
        public TargetData(TypeTarget type) {
            Type = type;
            Id1 = Id2 = null;
        }
    }
}
