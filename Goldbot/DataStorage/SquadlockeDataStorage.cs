using Goldbot.Global;
using Newtonsoft.Json;
using System.IO;

namespace Goldbot.DataStorage {
    public static class SquadlockeDataStorage {
        private static SquadlockePersistentDataObject dataObject = new SquadlockePersistentDataObject();

        static SquadlockeDataStorage() {
            if (!ValidateStorageFile("Resources/SquadlockeDataStorage.json")) return;

            string json = File.ReadAllText("SquadlockeDataStorage.json");
            dataObject = JsonConvert.DeserializeObject<SquadlockePersistentDataObject>(json);

            SquadlockeGlobal.currentTournamentName = dataObject.currentTournamentName;
            SquadlockeGlobal.ready = dataObject.ready;
            SquadlockeGlobal.encounters = dataObject.encounters;
        }

        public static void LoadData() { }

        public static void SaveData() {
            dataObject.currentTournamentName = SquadlockeGlobal.currentTournamentName;
            dataObject.ready = SquadlockeGlobal.ready;
            dataObject.encounters = SquadlockeGlobal.encounters;

            string json = JsonConvert.SerializeObject(dataObject, Formatting.Indented);
            File.WriteAllText("Resources/SquadlockeDataStorage.json", json);
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
