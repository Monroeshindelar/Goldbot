using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Goldbot.Global;
using Goldbot.Modules.Model.Tournament;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Goldbot.Modules {
    public class SquadLockeCommands : ModuleBase<SocketCommandContext> {
        //TODO: Command to init locke with participants, set up first bracket
        [Command("sl_init")]
        public async Task SquadlockeInit([Remainder]string args = null) {
            SquadlockeGlobal.ready = new Dictionary<string, bool>();

            var data = Helper.readIni(Global.Global.configPath);
            SquadlockeGlobal.participantRoleName = data["participant_role"];
            SocketRole participantRole = Context.Guild.Roles.First(x => x.Name == SquadlockeGlobal.participantRoleName);

            TournamentCommands tc = new TournamentCommands();

            SquadlockeGlobal.currentTournamentName = "SwSh_Squadlocke_Checkpoint_1_SWISS_TEST";

            await tc.CreateTournament($"{SquadlockeGlobal.currentTournamentName} {SquadlockeGlobal.currentTournamentName}");

            string message = "Welcome to the **Pokemon Sword and Shield** Squadlocke!\nThe First checkpoint has been set up.\n" +
                "";
            string participants = "";

            foreach (SocketGuildUser user in Context.Guild.Users) {
                if (user.Roles.Contains(participantRole)) {
                    await tc.AddParticipant($"{user.Username} {SquadlockeGlobal.currentTournamentName}");
                    participants += $"{user.Username}, ";
                    SquadlockeGlobal.ready.Add(user.Username, false);
                }
            }
            participants = participants.TrimEnd(new char[] { ',' });

            Tournament t = tc.GetTournamentByName(SquadlockeGlobal.currentTournamentName);
            message += $"**Squadlocke info**:\n**# of participants**: {t.participants_count}\n**Participants**: {participants}\n**Current checkpoint URL**: {t.full_challonge_url}";

            RestUserMessage startLocke = await Context.Channel.SendMessageAsync(message);

            await tc.ShuffleSeeds(SquadlockeGlobal.currentTournamentName);
        }

        [Command("sl_update_match")]
        public async Task UpdateMatch([Remainder]string args) {
            TournamentCommands tc = new TournamentCommands();

            string[] tokens = args.Split();

            Participant p1 = tc.GetParticipantByName(tokens[0], SquadlockeGlobal.currentTournamentName);
            Participant p2 = tc.GetParticipantByName(tokens[1], SquadlockeGlobal.currentTournamentName);

            if (p1.seed < p2.seed)
                await tc.UpdateMatch($"{p1.name} {p2.name} {tokens[2]} {tokens[3]} {SquadlockeGlobal.currentTournamentName}");
            else
                await tc.UpdateMatch($"{p2.name} {p1.name} {tokens[3]} {tokens[2]} {SquadlockeGlobal.currentTournamentName}");


        }

        //TODO: Command to store route encounters
        //      Maybe put it to a vote?
        [Command("sl_vote_on_encounter")]
        public async Task VoteOnEncounter([Remainder]string args) {
            RestUserMessage msg = await Context.Channel.SendMessageAsync($"**{Context.User.Username}** is proposing that **{args}** become an encounter.\n Vote 👍 to approve or 👎 to decline.");
            await msg.AddReactionAsync(new Emoji("👍"), new Discord.RequestOptions());
            await msg.AddReactionAsync(new Emoji("👎"), new Discord.RequestOptions());

            //TODO: Monitor the message until emoji has recieved more than 50% votes?
            //      Somehow prevent people from voting twice?
            //      Make a list of encounters and commit it to memory in Json

        }

        [Command("sl_ready_up")]
        public async Task ReadyUp() {
            string callerUsername = Context.Message.Author.Username;

            if (SquadlockeGlobal.ready[callerUsername] == false) {
                SquadlockeGlobal.ready[callerUsername] = true;
                EmbedBuilder embed = Helper.EmbedHelper("Ready", $"{callerUsername} has completed the checkpoint", Context.Message.Author.GetAvatarUrl(), new Color(0, 255, 0));
                await Context.Channel.SendMessageAsync("", false, embed);
            }

            foreach (string key in SquadlockeGlobal.ready.Keys)
                if (SquadlockeGlobal.ready[key] == false) return;

            TournamentCommands tc = new TournamentCommands();

            RestUserMessage msg = await Context.Channel.SendMessageAsync("Every particiant is ready. Starting the tournament.");

            await tc.StartTournament(SquadlockeGlobal.currentTournamentName);
        }

        [Command("sl_get_ready_status")]
        public async Task GetReadyStatus() {
            EmbedBuilder embed = null;
            Color red = new Color(255, 0, 0);
            Color green = new Color(0, 255, 0);

            foreach(string key in SquadlockeGlobal.ready.Keys) {
                string imageUrl = "";
                foreach (SocketGuildUser user in Context.Guild.Users) {
                    if (user.Username == key) imageUrl = user.GetAvatarUrl();
                }
                if (SquadlockeGlobal.ready[key])
                    embed = Helper.EmbedHelper($"{key} ready status", $"{key} has completed the checkpoint", imageUrl, green);
                else
                    embed = Helper.EmbedHelper($"{key} ready status", $"{key} has not completed the checkpoint", imageUrl, red);

                await Context.Channel.SendMessageAsync("", false, embed);
            }
        } 

        [Command("sl_get_checkpoint_url")]
        public async Task GetCheckpointURL() {
            Tournament t = (new TournamentCommands().GetTournamentByName(SquadlockeGlobal.currentTournamentName));
            await Context.Channel.SendMessageAsync(t.full_challonge_url);
        }

    }
}
