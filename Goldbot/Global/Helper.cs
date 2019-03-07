using Discord;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Goldbot.Global {
    public static class Helper {
        public static EmbedBuilder EmbedHelper(string title, string desc, string url, Color color) {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(desc);
            embed.WithColor(color);
            embed.WithThumbnailUrl(url);
            return embed;
        }

        public static Dictionary<string, string> readIni(string file) {
            Dictionary<string, string> ini = new Dictionary<string, string>();
            foreach (var row in File.ReadAllLines(file))
                ini.Add(row.Split('=')[0], string.Join("=", row.Split('=').Skip(1).ToArray()));
            return ini;
        }
    }
}
