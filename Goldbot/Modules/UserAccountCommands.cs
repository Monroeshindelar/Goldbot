using Discord.Commands;
using Discord.WebSocket;
using Goldbot.Core.UserAccounts;
using System.Threading.Tasks;

namespace Goldbot.Modules {
    public class UserAccountCommands : ModuleBase<SocketCommandContext> {
        [Command("Set_name")]
        public async Task SetName([Remainder]string args) {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            account.name = args;
            UserAccounts.SaveAccounts();
            await Context.Channel.SendMessageAsync($"{Context.User.Username}'s name set to {args}");
        }


        [Command("Get_name")]
        public async Task GetName([Remainder]string args = null) {
            string outputMessage = "";
            foreach (SocketUser user in Context.Message.MentionedUsers) {
                UserAccount account = UserAccounts.GetAccount(user);
                if (account.name != null)
                    outputMessage += $"{user.Username}'s name: {account.name}";
                else
                    outputMessage += $"{user.Username} has not added a name";
            }
            await Context.Channel.SendMessageAsync(outputMessage);
        }

        [Command("Set_friend_code")]
        public async Task AddFriendCode([Remainder]string args) {
            UserAccount account = UserAccounts.GetAccount(Context.User);
            account.friendCode = args;
            UserAccounts.SaveAccounts();
            await Context.Channel.SendMessageAsync($"Friend code saved for {Context.User.Username}.\nYou an access peoples friend codes by using\n`!Get_friend_code [mention: username]`");
        }

        [Command("Get_friend_code")]
        public async Task GetFriendCode([Remainder]string args = null) {
            string outputMessage = "";
            foreach(SocketUser user in Context.Message.MentionedUsers) {
                UserAccount account = UserAccounts.GetAccount(user);
                if (!account.friendCode.Equals(string.Empty))
                    outputMessage += $"{user.Username}'s Friend Code: {account.friendCode}\n";
                else
                    outputMessage = $"{user.Username} has not added a friend code.\n";

            }
            await Context.Channel.SendMessageAsync(outputMessage);
        }
    }
}
