using Eternity.Configs.Logger;
using Eternity.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Eternity.Tasks.Settings {
    public class FlooderSettings {
        /// <summary>
        /// Коллекция целей
        /// </summary>
        public List<FlooderTarget> Targets;
        /// <summary>
        /// Настройки флудера
        /// </summary>
        public string SelectDelay, DelayMin, DelayMax, LocationName, PhrasesFile;
        /// <summary>
        /// Поля для хранение количества целей и времени в минутах
        /// </summary>
        public string TimeMinutes, CountTarget;
        /// <summary>
        /// Флаг включено ли вывод в лог отправленных сообщений
        /// </summary>
        public static bool IsSendFalse = true;
        /// <summary>
        /// Флаг активна ли функций
        /// </summary>
        public bool Enabled;
        /// <summary>
        /// Коллекции содержимого
        /// </summary>
        public List<string> Phrases, Stickers, Contains;

        private readonly Random _random = new Random();
        /// <summary>
        /// Применить настройки
        /// </summary>
        /// 
        public FlooderSettings() {
            Targets = new List<FlooderTarget>();
        }
        public void LoadSettings() {
            Stickers = new List<string>(File.ReadAllLines("Files\\stickers.txt", Encoding.UTF8).ToList());
            if (!string.IsNullOrEmpty(PhrasesFile))
                Phrases = new List<string>(File.ReadAllLines($"Files\\Phrases\\{PhrasesFile}"));
            else
                Phrases = new List<string>();

            Contains = new List<string>(File.ReadAllLines("Files\\content.txt"));
        }
        public static class ContainerIndexed {
            public static int PhraseIndex = -1;
        }
        /// <summary>
        /// Парсер DataGrid - целей флудера
        /// </summary>
        /// <param name="view"></param>
        public void ParseDataGridFlooder(DataGridView view) {
            Targets.Clear();

            foreach (DataGridViewRow row in view.Rows) {
                if (row.Cells[1].Value != null)
                    Targets.Add(new FlooderTarget {
                        Name = (row.Cells[0].Value ?? "").ToString(),
                        Link = (row.Cells[1].Value ?? "").ToString(),
                        Contains = (row.Cells[2].Value ?? "").ToString()
                    });
            }
        }
        /// <summary>
        /// Получить случайные фразы из текстового документа
        /// </summary>
        /// <returns></returns>
        public string RandomPhrase() {
            if (Phrases.Count == 0) {
                Logger.Push("[Флудер]: Отсутствуют фразы...");
                return null;
            }

            ContainerIndexed.PhraseIndex = (ContainerIndexed.PhraseIndex + 1) % Phrases.Count;

            return Phrases[ContainerIndexed.PhraseIndex];
        }
        /// <summary>
        /// Получить случайный стикер из текстового документа
        /// </summary>
        /// <returns></returns>
        public string RandomStickers() {
            if (Stickers.Count == 0) {
                Logger.Push("[Флудер]: Отсутствуют стикеры...");
                return null;
            }

            return Stickers[_random.Next(Stickers.Count)];
        }
        /// <summary>
        /// Получить случайное содержимое
        /// </summary>
        /// <returns></returns>
        public string RandomContains() {
            if (Contains.Count == 0) {
                Logger.Push("[Флудер]: Отсутствуют вложения...");
                return null;
            }

            return Contains[_random.Next(Contains.Count)];
        }
    }
}
