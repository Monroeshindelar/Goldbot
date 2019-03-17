using System.Collections.Generic;

namespace Goldbot.Global {
    internal static class SquadlockeGlobal {
        internal static string participantRoleName { get; set; }
        internal static string routeName { get; set; }
        internal static ulong voteMessageId { get; set; }
        internal static List<ulong> votedUsers { get; set; }
        internal static uint voteYayCount { get; set; }
        internal static uint voteNayCount { get; set; }
    }
}
