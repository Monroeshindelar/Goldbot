using Discord.Commands;
using System.Threading.Tasks;
using RestSharp;
using Goldbot.Global;
using System;
using Goldbot.Modules.Json_Model_Response;
using Newtonsoft.Json;
using System.Collections.Generic;
using Goldbot.Modules.Model.Tournament;
using Discord;

namespace Goldbot.Modules {
    public class TournamentCommands : ModuleBase<SocketCommandContext> {

        //TODO: Add a lot more customizability for creating tournaments
        //      Maybe read from a config with a flag
        //      Add error handeling for a bunch of stuff
        [Command("tournament_create")]
        public async Task CreateTournament([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            string[] tokens = args.Split(' ');

            var request = InitRequestWithApiKey("tournaments", Method.POST);
            request.AddParameter("tournament[name]", tokens[0]);
            if (tokens.Length > 1)
                request.AddParameter("tournament[url]", tokens[1]);
            else
                request.AddParameter("tournament[url]", tokens[0]);

            request.AddParameter("tournament[tournament_type]", "swiss");

            if (tokens.Length > 1) {
                for (int i = 2; i < tokens.Length; i++) {
                    string[] newParam = tokens[i].Split('=');
                    request.AddParameter($"tournament[{newParam[0]}]", newParam[1]);
                }
            }
            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [Command("add_participant")]
        public async Task AddParticipant([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            string[] tokens = args.Split(' ');
            string makeName = "";

            for (int i = 0; i < tokens.Length - 2; i++)
                makeName += tokens[i] + " ";

            makeName.TrimEnd(new char[] { ' ' });

            var request = InitRequestWithApiKey($"tournaments/{tokens[tokens.Length - 1]}/participants", Method.POST);
            request.AddParameter("participant[name]", tokens[0]);

            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }
        //TODO: Probably get rid of this or fix it
        //      Doesnt add the last participant
        [Command("add_participants")]
        public async Task AddParticipants([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            string[] tokens = args.Split(' ');
            for (int i = 0; i < tokens.Length - 2; i++)
                await AddParticipant($"{tokens[i]} {tokens[tokens.Length - 1]}");

        }

        [Command("start_tournament")]
        public async Task StartTournament([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            var request = InitRequestWithApiKey($"tournaments/{args}/start", Method.POST);
            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [Command("upcoming_match")]
        public async Task GetUpcomingMatches([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            var request = InitRequestWithApiKey($"tournaments/{args}/matches", Method.GET);
            request.AddParameter("state", "open");

            IRestResponse response = TournamentGlobal.client.Execute(request);

            List<JsonResponseMatch> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseMatch>>(response.Content);

            foreach (JsonResponseMatch deserialized in deserialize) {
                Match match = deserialized.match;

                string desc = $"{GetParticipantNameById(match.player1_id, args)} vs. {GetParticipantNameById(match.player2_id, args)}";
                EmbedBuilder embed = Helper.EmbedHelper("Upcoming Match", desc, null, new Color(0, 255, 0));
                await Context.Channel.SendMessageAsync("", false, embed);
            }
        }

        [Command("update_match")]
        public async Task UpdateMatch([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            string[] tokens = args.Split();

            if (tokens.Length < 5) return;

            int player1Id = GetParticipantIdByName(tokens[0], tokens[tokens.Length - 1]);
            int player2Id = GetParticipantIdByName(tokens[1], tokens[tokens.Length - 1]);
            int player1Score = -1;
            if (!int.TryParse(tokens[2], out player1Score)) return;
            int player2Score = -1;
            if (!int.TryParse(tokens[3], out player2Score)) return;

            int matchToUpdate = GetMatchIdByParticipants(player1Id, player2Id, tokens[tokens.Length - 1]);

            var request = InitRequestWithApiKey($"tournaments/{tokens[tokens.Length - 1]}/matches/{matchToUpdate}", Method.PUT);

            request.AddParameter($"match[scores_csv]", $"{player1Score}-{player2Score}");
            if (player1Score > player2Score)
                request.AddParameter("match[winner_id]", player1Id);
            else if (player2Score > player1Score)
                request.AddParameter("match[winner_id]", player2Id);

            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [Command("shuffle_seeds")]
        public async Task ShuffleSeeds([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            var request = InitRequestWithApiKey($"tournaments/{args}/participants", Method.GET);

            IRestResponse response = TournamentGlobal.client.Execute(request);

            List<JsonResponseParticipant> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            foreach(JsonResponseParticipant deserialized in deserialize) {
                deserialize.Remove(deserialized);
                Random r = new Random();
                int rInt = r.Next(0, deserialize.Count);

                JsonResponseParticipant rand = deserialize[rInt];
                deserialize.Remove(rand);

                request = InitRequestWithApiKey($"tournaments/{args}/participants/{deserialized.participant.id}", Method.PUT);
                request.AddParameter("participant[seed]", rand.participant.seed);
                response = TournamentGlobal.client.Execute(request);
            }
        }

        [Command("finalize_tournament")]
        public async Task FinalizeTournament([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            var request = InitRequestWithApiKey($"tournaments/{args}/finalize", Method.POST);
            request.AddParameter("include_participants", 1);
            request.AddParameter("include_matches", 1);

            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [Command("reset_tournament")]
        public async Task ResetTournament([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            var request = InitRequestWithApiKey($"tournaments/{args}/reset", Method.POST);
            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }

        [Command("delete_tournament")]
        public async Task DeleteTournament([Remainder]string args) {
            if (TournamentGlobal.client == null) InitTournamentVariables();

            var request = InitRequestWithApiKey($"tournaments/{args}", Method.DELETE);
            IRestResponse response = TournamentGlobal.client.Execute(request);
            Console.WriteLine(response.Content);
        }

        private void InitTournamentVariables() {
            TournamentGlobal.client = new RestClient(TournamentGlobal.api_base_url);
        }

        private RestRequest InitRequestWithApiKey(string url, Method method) {
            var data = Helper.readIni(Global.Global.configPath);
            TournamentGlobal.api_key = data["api_key"];
            RestRequest request = new RestRequest(url, method);
            request.AddParameter("api_key", TournamentGlobal.api_key);
            return request;
        }


        public Tournament GetTournamentByName(string tournamentName) {
            Tournament retVal = null;

            tournamentName = tournamentName.ToLower();

            var request = InitRequestWithApiKey($"tournaments", Method.GET);

            IRestResponse response = TournamentGlobal.client.Execute(request);

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            List<JsonResponseTournament> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseTournament>>(response.Content, jsonSerializerSettings);

            foreach(JsonResponseTournament deserialized in deserialize) {
                Tournament tournament = deserialized.tournament;
                string currentName = tournament.name.ToLower();

                if(currentName.Equals(tournamentName)) {
                    retVal = tournament;
                    break;
                }
            }
            return retVal;
        }

        public Participant GetParticipantByName(string participantName, string tournamentName) {
            Participant retVal = null;

            participantName = participantName.ToLower();

            var request = InitRequestWithApiKey($"tournaments/{tournamentName}/participants", Method.GET);

            IRestResponse response = TournamentGlobal.client.Execute(request);

            List<JsonResponseParticipant> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            foreach (JsonResponseParticipant deserialized in deserialize) {
                Participant participant = deserialized.participant;
                string currentName = participant.name.ToLower();

                if (currentName.Equals(participantName)) {
                    retVal = participant;
                    break; 
                }
            }

            return retVal;
        }

        private int GetParticipantIdByName(string participantName, string tournamentName) {
            int id = -1;
            participantName = participantName.ToLower();

            var request = InitRequestWithApiKey($"tournaments/{tournamentName}/participants", Method.GET);

            IRestResponse response = TournamentGlobal.client.Execute(request);

            List<JsonResponseParticipant> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            foreach (JsonResponseParticipant deserialized in deserialize) {
                Participant participant = deserialized.participant;
                string currentName = participant.name.ToLower();

                if (currentName.Equals(participantName)) {
                    id = participant.id;
                    break;
                }
            }
            return id;
        }

        private string GetParticipantNameById(int id, string tournamentName) {
            string name = "";

            var request = InitRequestWithApiKey($"tournaments/{tournamentName}/participants", Method.GET);

            IRestResponse response = TournamentGlobal.client.Execute(request);

            List<JsonResponseParticipant> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            foreach (JsonResponseParticipant deserialized in deserialize) {
                Participant participant = deserialized.participant;
                if(participant.id == id) {
                    name = participant.name;
                    break;
                }
            }

            return name;
        }

        private int GetMatchIdByParticipants(int p1Id, int p2Id, string tournamentName) {
            int matchId = -1;

            var request = InitRequestWithApiKey($"tournaments/{tournamentName}/matches", Method.GET);
            request.AddParameter("state", "open");

            IRestResponse response = TournamentGlobal.client.Execute(request);

            List<JsonResponseMatch> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseMatch>>(response.Content);

            foreach(JsonResponseMatch deserialized in deserialize) {
                Match match = deserialized.match;
                if(p1Id == match.player1_id && p2Id == match.player2_id){
                    matchId = match.id;
                    break;
                }
            }

            return matchId;
        }
    }
}
