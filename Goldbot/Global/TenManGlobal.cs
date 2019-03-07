using CSGSI;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Goldbot.Global {
    internal static class TenManGlobal {
        internal static GameStateListener gsl { get; set; }
        internal static List<SocketGuildUser> participants { get; set; }
        internal static Dictionary<string, string> mapThumbnails { get; set; }
        internal static Dictionary<string, bool> PickBanTracker { get; set; }
        internal static List<SocketRole> rolesToRemove { get; set; }
        internal static List<string> playersRemaining { get; set; }
        internal static List<string> mapsRemaining { get; set; }
        internal static ulong TeamAMessageID { get; set; }
        internal static ulong TeamBMessageID { get; set; }
        internal static ulong RemainingPlayersMessageID { get; set; }
        internal static ulong RemainingMapsMessageID { get; set; }
        internal static ulong MapPickMessaegID { get; set; }
        internal static bool TeamATurn { get; set; }
        internal static bool IsBanPhase { get; set; }
    }
}