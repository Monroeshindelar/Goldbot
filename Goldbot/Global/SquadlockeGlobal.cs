using Goldbot.Modules.Model.Challonge;
using System.Collections.Generic;

namespace Goldbot.Global {
    internal static class SquadlockeGlobal {
        internal static string participantRoleName { get; set; }
        internal static string currentTournamentName { get; set; }
        internal static Tournament currentTournament { get; set; }
        internal static Dictionary<string, bool> ready { get; set; }
        internal static List<string> encounters { get; set; }

        //Voting
        //I feel like there is probably a better way to do this
        //One where you dont have to use so many global variables
        //Will figure it out later

        //I can probably move this into SquadlockeHelper?
        internal static string routeName { get; set; }
        internal static ulong voteMessageId { get; set; }
        internal static List<ulong> votedUsers { get; set; }
        internal static uint voteYayCount { get; set; }
        internal static uint voteNayCount { get; set; }
    }
}
