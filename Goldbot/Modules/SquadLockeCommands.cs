using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Goldbot.DataStorage;
using Goldbot.Global;
using Goldbot.Modules.CommandHelpers;
using Goldbot.Modules.Model.Challonge;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goldbot.Modules {
    public class SquadLockeCommands : ModuleBase<SocketCommandContext> {
        //TODO: Command to init locke with participants, set up first bracket
        [Command("sl_init")]
        public async Task SquadlockeInit([Remainder]string args = null) {
            string stringBuilder = "Welcome to the Pokemon Sword and Shield Squadlocke!\nThe First checkpoint has been set up.\n";
            SquadlockeGlobal.ready = new Dictionary<string, bool>();
            string[] tokens;
            if (args != null) tokens = args.Split(' ');
            else tokens = new string[]{ "PersistentDataTestForSL" };


            var data = Utilities.readIni(Global.Global.configPath);
            SquadlockeGlobal.participantRoleName = data["participant_role"];
            SocketRole participantRole = Context.Guild.Roles.First(x => x.Name == SquadlockeGlobal.participantRoleName);

            SquadlockeGlobal.currentTournamentName = tokens[0];

            Tournament t = TournamentHelper.CreateTournament(SquadlockeGlobal.currentTournamentName);
            SquadlockeGlobal.currentTournament = t;

            List<string> pList = new List<string>();

            foreach(SocketGuildUser user in Context.Guild.Users) {
                if(user.Roles.Contains(participantRole)) {
                    pList.Add(user.Username);
                    SquadlockeGlobal.ready.Add(user.Username, false);
                }
            }

            SquadlockeHelper.InitializeSquadlocke(SquadlockeGlobal.currentTournamentName, pList);

            stringBuilder += $"Squadlocke info:\n# of participants: {t.participants_count}\nTournament Type: {t.tournament_type}\nCurrent checkpoint URL: {t.full_challonge_url}";

            RestUserMessage startLocke = await Context.Channel.SendMessageAsync(stringBuilder);
            SquadlockeDataStorage.SaveData();
            //Tournament T = TournamentHelper.GetTournamentByName(SquadlockeGlobal.currentTournamentName);
            //SquadlockeGlobal.currentTournament = T;

            //string message = "Welcome to the Pokemon Sword and Shield Squadlocke!\nThe First checkpoint has been set up.\n";
            //string participants = "";

            //List<string> pList = new List<string>();

            //foreach (SocketGuildUser user in Context.Guild.Users) {
            //    if (user.Roles.Contains(participantRole)) {
            //        //TournamentHelper.AddParticipantsToTournament(user.Username, T);
            //        participants += $"{user.Username}, ";
            //        pList.Add(user.Username);
            //        SquadlockeGlobal.ready.Add(user.Username, false); 
            //    }
            //}

            //TournamentHelper.AddParticipantsToTournament(pList, SquadlockeGlobal.currentTournament);

            //participants = participants.TrimEnd(new char[] { ',' });

            //Tournament t = TournamentHelper.GetTournamentByName(SquadlockeGlobal.currentTournamentName);
            //message += $"Squadlocke info:\n# of participants: {t.participants_count}\nParticipants: {participants}\nTournament Type: {t.tournament_type}\nCurrent checkpoint URL: {t.full_challonge_url}";

            //RestUserMessage startLocke = await Context.Channel.SendMessageAsync(message);

            //TournamentHelper.ShuffleSeeds(t);
            //SquadlockeDataStorage.SaveData();
        }

        [Command("sl_update_match")]
        public async Task UpdateMatch([Remainder]string args) {
            string[] tokens = args.Split(' ');

            Participant p1 = TournamentHelper.GetParticipantByName(tokens[0], SquadlockeGlobal.currentTournament);
            Participant p2 = TournamentHelper.GetParticipantByName(tokens[1], SquadlockeGlobal.currentTournament);

            int score1, score2;
            if (!int.TryParse(tokens[2], out score1)) return;
            if (!int.TryParse(tokens[3], out score2)) return;

            TournamentHelper.UpdateMatch(p1, p2, score1, score2, SquadlockeGlobal.currentTournament);

            string msg = "";
            if (score1 > score2)
                msg = $"{tokens[0]} has defeated {p1.name} {score1}-{score2}";
            else
                msg = $"Match has been updated" +
                    $"{tokens[1]} has taken down {tokens[0]} {score2}-{score1}";

            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("sl_vote_on_encounter")]
        public async Task VoteOnEncounter([Remainder]string args) {
            SquadlockeGlobal.votedUsers = new List<ulong>();
            SquadlockeGlobal.voteYayCount = 0;
            SquadlockeGlobal.voteNayCount = 0;
            SquadlockeGlobal.routeName = args;
            RestUserMessage msg = await Context.Channel.SendMessageAsync($"**{Context.User.Username}** is proposing that " +
                $"**{SquadlockeGlobal.routeName}** become an encounter.\n Vote 👍 to approve or 👎 to decline.");
            await msg.AddReactionAsync(new Emoji("👍"), new RequestOptions());
            await msg.AddReactionAsync(new Emoji("👎"), new RequestOptions());

            SquadlockeGlobal.voteMessageId = msg.Id;
        }

        [Command("sl_show_encounter_list")]
        public async Task ShowEncounterList() {
            string msg = "```";

            foreach (string location in SquadlockeGlobal.encounters)
                msg += $"{location}\n";

            msg += "```";
        }

        [Command("sl_ready_up")]
        public async Task ReadyUp() {
            string callerUsername = Context.Message.Author.Username;

            if (SquadlockeGlobal.ready[callerUsername] == false) {
                SquadlockeGlobal.ready[callerUsername] = true;
                EmbedBuilder embed = Utilities.EmbedHelper("Ready", $"{callerUsername} has completed the checkpoint", Context.Message.Author.GetAvatarUrl(), new Color(0, 255, 0));
                await Context.Channel.SendMessageAsync("", false, embed);
            }

            foreach (string key in SquadlockeGlobal.ready.Keys)
                if (SquadlockeGlobal.ready[key] == false) return;

            RestUserMessage msg = await Context.Channel.SendMessageAsync("Every particiant is ready. Starting the tournament.");

            TournamentHelper.StartTournament(SquadlockeGlobal.currentTournament);
            SquadlockeDataStorage.SaveData();
        }

        [Command("sl_get_ready_status")]
        public async Task GetReadyStatus() {
            EmbedBuilder embed = null;
            Color red = new Color(255, 0, 0);
            Color green = new Color(0, 255, 0);

            foreach(string key in SquadlockeGlobal.ready.Keys) {
                string imageUrl = "";
                foreach (SocketGuildUser user in Context.Guild.Users)
                    if (user.Username == key) imageUrl = user.GetAvatarUrl();
                if (SquadlockeGlobal.ready[key])
                    embed = Utilities.EmbedHelper($"{key} ready status", $"{key} has completed the checkpoint", imageUrl, green);
                else
                    embed = Utilities.EmbedHelper($"{key} ready status", $"{key} has not completed the checkpoint", imageUrl, red);

                await Context.Channel.SendMessageAsync("", false, embed);
            }
        } 

        [Command("sl_get_checkpoint_url")]
        public async Task GetCheckpointURL() {
            await Context.Channel.SendMessageAsync(SquadlockeGlobal.currentTournament.full_challonge_url);
        }
    }
}
