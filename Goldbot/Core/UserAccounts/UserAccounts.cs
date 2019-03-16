using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;

namespace Goldbot.Core.UserAccounts {
    public static class UserAccounts {
        private static List<UserAccount> accounts;

        private static string accountsFile = "Resources/accounts.json";

        static UserAccounts() {
            if (DataStorage.SaveExists(accountsFile))
                accounts = DataStorage.LoadUserAccounts(accountsFile).ToList();
            else {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() {
            DataStorage.SaveUserAccounts(accounts, accountsFile);
        }

        public static UserAccount GetAccount(SocketUser user) {
            return GetOrCreateAccount(user.Id);
        }

        private static UserAccount GetOrCreateAccount(ulong id) {
            var result = from a in accounts
                         where a.id == id
                         select a;

            var account = result.FirstOrDefault();
            if (account == null)
                account = CreateUserAccount(id);

            return account;
        }

        private static UserAccount CreateUserAccount(ulong id_) {
            var newAccount = new UserAccount() {
                id = id_,
                friendCode = "",
                streamProfile = ""
            };
            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
