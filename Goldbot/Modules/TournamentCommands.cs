using Discord.Commands;
using System.Threading.Tasks;
using System.Collections.Generic;
using Goldbot.Modules.Model.Challonge;
using Discord;
using Goldbot.Modules.CommandHelpers;

namespace Goldbot.Modules {
    public class TournamentCommands : ModuleBase<SocketCommandContext> {
        [Command("tournament_create")]
        public async Task CreateTournament([Remainder]string args) {
            string[] tokens = args.Split(' ');
            TournamentHelper.CreateTournament(tokens[0], tokens[0]);
        }

        [Command("add_participants")]
        public async Task AddParticipants([Remainder]string args) {
            string[] tokens = args.Split(' ');
            Tournament t = TournamentHelper.GetTournamentByName(tokens[tokens.Length - 1]);

            List<string> particpants = new List<string>();
            for (int i = 0; i < tokens.Length - 1; i++)
                particpants.Add(tokens[i]);

            TournamentHelper.AddParticipantsToTournament(particpants, t);
        }

        [Command("start_tournament")]
        public async Task StartTournament([Remainder]string args) {
            Tournament t = TournamentHelper.GetTournamentByName(args);

            if (t != null)
                TournamentHelper.StartTournament(t);
        }

        [Command("upcoming_match")]
        public async Task GetUpcomingMatches([Remainder]string args) {
            EmbedBuilder embed = null;

            Tournament t = TournamentHelper.GetTournamentByName(args);
            List<Match> matches = new List<Match>();

            if (t != null)
                matches = TournamentHelper.GetUpcomingMatches(t);

            foreach(Match match in matches) {
                Participant p1 = TournamentHelper.GetParticipantById(match.player1_id, t);
                Participant p2 = TournamentHelper.GetParticipantById(match.player2_id, t);

                string desc = $"{p1.name} vs. {p2.name}";

                embed = Utilities.EmbedHelper("Upcoming Match", desc, 
                    "https://www.google.com/url?sa=i&source=images&cd=&cad=rja&uact=8&ved=2ahUKEwjO6dy8lIXhAhXLxVQKHasjCvEQjRx6BAgBEAU&url=https%3A%2F%2Fpngtree.com%2Ffreepng%2Fvs-match_2376253.html&psig=AOvVaw0O0JJaQ7z-i6Ul7NN33LOS&ust=1552774101509167",
                    new Color(0, 0, 0));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }

        [Command("update_match")]
        public async Task UpdateMatch([Remainder]string args) {
            string[] tokens = args.Split();

            if (tokens.Length < 5) return;

            Tournament t = TournamentHelper.GetTournamentByName(tokens[tokens.Length - 1]);
            Participant p1 = TournamentHelper.GetParticipantByName(tokens[0], t);
            Participant p2 = TournamentHelper.GetParticipantByName(tokens[1], t);

            int score1, score2;
            if (!int.TryParse(tokens[2], out score1)) return;
            if (!int.TryParse(tokens[3], out score2)) return;

            TournamentHelper.UpdateMatch(p1, p2, score1, score2, t);
        }

        [Command("shuffle_seeds")]
        public async Task ShuffleSeeds([Remainder]string args) {
            Tournament t = TournamentHelper.GetTournamentByName(args);
            if (t != null)
                TournamentHelper.ShuffleSeeds(t);
        }

        [Command("finalize_tournament")]
        public async Task FinalizeTournament([Remainder]string args) {
            Tournament t = TournamentHelper.GetTournamentByName(args);
            if (t != null)
                TournamentHelper.FinalizeTournament(t);
        }

        [Command("reset_tournament")]
        public async Task ResetTournament([Remainder]string args) {
            Tournament t = TournamentHelper.GetTournamentByName(args);
            if (t != null)
                TournamentHelper.ResetTournament(t);
        }

        [Command("delete_tournament")]
        public async Task DeleteTournament([Remainder]string args) {
            Tournament t = TournamentHelper.GetTournamentByName(args);
            if (t != null)
                TournamentHelper.DeleteTournament(t);
        }
    }
}
