using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord;
using Goldbot.Global;
using Goldbot.Modules.CommandHelpers;

namespace Goldbot {
    class Program {
        DiscordSocketClient _client;
        CommandHandler _handler;

        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync() {
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig {
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += Log;
            _client.ReactionAdded += OnReactionAdded;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);
            await Task.Delay(-1);
        }

        //This seems terrible
        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction) {
            if(reaction.MessageId == SquadlockeGlobal.voteMessageId) {
                if (SquadlockeGlobal.votedUsers.Contains(reaction.UserId)) return;

                if (reaction.Emote.Name == "👍")
                    SquadlockeGlobal.voteYayCount++;
                else if (reaction.Emote.Name == "👎")
                    SquadlockeGlobal.voteNayCount++;

                //Store a tournament object and get the size of the tournament instead of 8
                if (SquadlockeGlobal.voteYayCount > SquadlockeHelper.GetCurrentTournament().participants_count / 2)
                    SquadlockeHelper.AddEncounter(SquadlockeGlobal.routeName);
            }
        }

        private async Task Log(LogMessage msg) {
            Console.WriteLine(msg.Message);
        }
    }
}
