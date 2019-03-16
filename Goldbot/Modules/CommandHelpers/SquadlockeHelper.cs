using Goldbot.Global;
using Goldbot.Modules.Model.Challonge;
using System.Collections.Generic;

namespace Goldbot.Modules.CommandHelpers {
    public static class SquadlockeHelper {
        private static string currentTournamentName;
        private static Tournament CurrentTournament;
        private static Dictionary<string, bool> readiedParticipants;

        static SquadlockeHelper() {
            CurrentTournament = TournamentHelper.GetTournamentByName(SquadlockeGlobal.currentTournamentName);
        }

        public static void InitializeSquadlocke(string initialTournamentName, List<string> participants, string remainingArgs = null, string tournamentUrl = null) {
            string url = tournamentUrl != null ? tournamentUrl : initialTournamentName;
            TournamentHelper.CreateTournament(initialTournamentName, url, remainingArgs);
            TournamentHelper.AddParticipantsToTournament(participants, CurrentTournament);
            //TournamentHelper.ShuffleSeeds(CurrentTournament);
        }

        public static void UpdateMatch(Participant p1, Participant p2, int score1, int score2) {
            TournamentHelper.UpdateMatch(p1, p2, score1, score2, CurrentTournament);
        }

        public
    }
}
