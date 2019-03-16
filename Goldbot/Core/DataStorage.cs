using Goldbot.Core.UserAccounts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Goldbot.Core {
    public static class DataStorage {
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath) {
            string json = JsonConvert.SerializeObject(accounts);

            //wrap in a try catch block later, for error handling.
            File.WriteAllText(filePath, json);
        }

        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath) {
            if (!File.Exists(filePath)) return null;

            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        public static bool SaveExists(string filePath) {
            return File.Exists(filePath);
        }
    }
}
