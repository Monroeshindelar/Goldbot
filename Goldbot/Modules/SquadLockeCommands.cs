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
            //SquadlockeGlobal.ready = new Dictionary<string, bool>();
            string[] tokens;
            if (args != null) tokens = args.Split(' ');
            else tokens = new string[]{ "PersistentDataTestForSL" };


            var data = Utilities.readIni(Global.Global.configPath);
            SquadlockeGlobal.participantRoleName = data["participant_role"];
            SocketRole participantRole = Context.Guild.Roles.First(x => x.Name == SquadlockeGlobal.participantRoleName);

            //SquadlockeGlobal.currentTournamentName = tokens[0];

            Tournament t = TournamentHelper.CreateTournament(tokens[0]);
            //SquadlockeGlobal.currentTournament = t;

            List<string> pList = new List<string>();

            foreach(SocketGuildUser user in Context.Guild.Users) {
                if(user.Roles.Contains(participantRole))
                    pList.Add(user.Username);
            }

            SquadlockeHelper.InitializeSquadlocke(tokens[0], pList);

            stringBuilder += $"Squadlocke info:\n# of participants: {t.participants_count}\nTournament Type: {t.tournament_type}\nCurrent checkpoint URL: {t.full_challonge_url}";

            RestUserMessage startLocke = await Context.Channel.SendMessageAsync(stringBuilder);
        }

        [Command("sl_update_match")]
        public async Task UpdateMatch([Remainder]string args) {
            string[] tokens = args.Split(' ');

            Participant p1 = SquadlockeHelper.GetSquadlockeParticipantByName(tokens[0]);
            Participant p2 = SquadlockeHelper.GetSquadlockeParticipantByName(tokens[1]);

            int score1, score2;
            if (!int.TryParse(tokens[2], out score1)) return;
            if (!int.TryParse(tokens[3], out score2)) return;

            SquadlockeHelper.UpdateMatch(p1, p2, score1, score2);

            string msg = "";
            if (score1 > score2)
                msg = $"{p1.name} has defeated {p2.name} {score1}-{score2}";
            else
                msg = $"Match has been updated" +
                    $"{p2.name} has taken down {p2.name} {score2}-{score1}";

            await Context.Channel.SendMessageAsync(msg);
        }

        [Command("sl_start_tournament")]
        public async Task StartTournamentSL() {
            SquadlockeHelper.StartTournament();
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
            string msg = "There are no additional agreed upon encounters.\nUse \n`!sl_vote_on_encounter [location]`\nto add an encounter to the list";
            if (!SquadlockeHelper.GetEncounterList().Equals(string.Empty))
                msg = "```" + SquadlockeHelper.GetEncounterList() + "```";
            await Context.Channel.SendMessageAsync(msg);
        } 

        [Command("sl_ready_up")]
        public async Task ReadyUp() {
            string callerUsername = Context.Message.Author.Username;
            bool readySuccess = SquadlockeHelper.ReadyUpSL(callerUsername);

            if (readySuccess) {
                EmbedBuilder embed = Utilities.EmbedHelper("Ready", $"{callerUsername} has completed the checkpoint", Context.Message.Author.GetAvatarUrl(), new Color(0, 255, 0));
                await Context.Channel.SendMessageAsync("", false, embed);
            }

            if(SquadlockeHelper.AllPlayersReady()) {
                await Context.Channel.SendMessageAsync($"All participants are ready, starting the tournament" +
                    $"\n{SquadlockeHelper.GetCurrentTournament().full_challonge_url}");
                SquadlockeHelper.StartTournament();
            }
        }

        [Command("sl_get_ready_status")]
        public async Task GetReadyStatus() {
            EmbedBuilder embed = null;
            Color red = new Color(255, 0, 0);
            Color green = new Color(0, 255, 0);

            Dictionary<string, bool> ready = SquadlockeHelper.GetReadyList();

            foreach(string key in ready.Keys) {
                string imageUrl = "";
                foreach (SocketGuildUser user in Context.Guild.Users)
                    if (user.Username == key) imageUrl = user.GetAvatarUrl();
                if (ready[key])
                    embed = Utilities.EmbedHelper($"{key} ready status", $"{key} has completed the checkpoint", imageUrl, green);
                else
                    embed = Utilities.EmbedHelper($"{key} ready status", $"{key} has not completed the checkpoint", imageUrl, red);

                await Context.Channel.SendMessageAsync("", false, embed);
            }
        } 

        [Command("sl_get_checkpoint_url")]
        public async Task GetCheckpointURL() {
            await Context.Channel.SendMessageAsync(SquadlockeHelper.GetCurrentTournament().full_challonge_url);
        }
    }
}
