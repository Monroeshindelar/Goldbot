using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Goldbot.DataStorage {
    public abstract class DataStorage {
        private static Dictionary<string, string> pairs = new Dictionary<string, string>();

        static DataStorage() {
            if (!ValidateStorageFile("DataStorage.json")) return;

            string json = File.ReadAllText("DataStorage.json");
            pairs = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static void AddPairToStorage(string key, string value) {
            pairs.Add(key, value);
            SaveData();
        }

        public static void SaveData() {
            string json = JsonConvert.SerializeObject(pairs, Formatting.Indented);
            File.WriteAllText("DataStorage.json", json);
        }

        private static bool ValidateStorageFile(string file) {
            if (!File.Exists(file)) {
                File.WriteAllText(file, "");
                SaveData();
                return false;
            }
            return true;
        }
    }
}
