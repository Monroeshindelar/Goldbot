using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using Goldbot.Global;
using RestSharp;
using Discord;
using System.Linq;

namespace Goldbot {
    class Utilities {
        private static Dictionary<string, string> alerts;

        static Utilities() {
            string json = File.ReadAllText("SystemLang/alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetAlert(string key) {
            if (alerts.ContainsKey(key)) return alerts[key];
            return "";
        }

        public static RestRequest InitRequestWithApiKey(string url, Method method) {
            var data = readIni(Global.Global.configPath);
            TournamentGlobal.api_key = data["api_key"];
            RestRequest request = new RestRequest(url, method);
            request.AddParameter("api_key", TournamentGlobal.api_key);
            return request;
        }

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
