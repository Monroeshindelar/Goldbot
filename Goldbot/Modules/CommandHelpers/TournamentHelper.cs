using Goldbot.Modules.Json_Model_Response;
using Goldbot.Modules.Model.Challonge;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace Goldbot.Modules.CommandHelpers {
    public static class TournamentHelper {
        private static RestClient client;
        private static string api_base_url = "https://api.challonge.com/v1/";

        //Think about making some of these return some meaningful value rather than just void. CreateTournament true or false for example

        static TournamentHelper() {
            client = new RestClient(api_base_url);
        }

        public static Tournament CreateTournament(string tournamentName, string tournamentUrl = null, string remainingArgs = null) {
            var request = Utilities.InitRequestWithApiKey("tournaments", Method.POST);
            request.AddParameter("tournament[name]", tournamentName);
            if (tournamentUrl != null)
                request.AddParameter("tournament[url]", tournamentUrl);
            else
                request.AddParameter("tournament[url]", tournamentName);

            if (remainingArgs != null) {
                string[] tokens = remainingArgs.Split(' ');
                foreach (string token in tokens) {
                    string[] newParam = token.Split('=');
                    if (newParam[0].Equals("tournament_type")) {
                        string[] cleanInput = token.Split('_');
                        newParam[1] = $"{cleanInput[0]} {cleanInput[1]}";
                    }
                    request.AddParameter($"tournament[{newParam[0]}]", newParam[1]);
                }
            }

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            return GetTournamentByName(tournamentName);
        }

        public static void AddParticipantsToTournament(List<string> participants, Tournament tournament) {
            foreach (string participant in participants) {
                var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/participants", Method.POST);
                request.AddParameter("participant[name]", participant);

                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
            }
        }

        public static List<Match> GetUpcomingMatches(Tournament tournament) {
            List<Match> matches = new List<Match>();

            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/matches", Method.GET);
            request.AddParameter("state", "open");

            IRestResponse response = client.Execute(request);

            List<JsonResponseMatch> deserialize =
                JsonConvert.DeserializeObject<List<JsonResponseMatch>>(response.Content);

            foreach (JsonResponseMatch deserialized in deserialize)
                matches.Add(deserialized.match);

            return matches;
        }

        public static void UpdateMatch(Participant p1, Participant p2, int participant1Score, int participant2Score, Tournament tournament) {
            Match m = GetMatchByParticipants(p1, p2, tournament);
                
            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/matches/{m.id}", Method.PUT);

            if(p1.id != m.player1_id) {
                Participant temp = p2;
                p2 = p1;
                p1 = temp;

                int tempS = participant2Score;
                participant2Score = participant1Score;
                participant1Score = tempS;
            }

            request.AddParameter($"match[scores_csv]", $"{participant1Score}-{participant2Score}");
            if (participant1Score > participant2Score)
                request.AddParameter("match[winner_id]", p1.id);
            else if (participant2Score > participant1Score)
                request.AddParameter("match[winner_id]", p2.id);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void ShuffleSeeds(Tournament tournament) {
            //var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/participants", Method.GET);

            //IRestResponse response = client.Execute(request);

            //List<JsonResponseParticipant> deserialize
            //    = JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            //foreach(JsonResponseParticipant deserialized in deserialize) {
            //    deserialize.Remove(deserialized);
            //    Random r = new Random();
            //    int rInt = r.Next(0, deserialize.Count);

            //    JsonResponseParticipant rand = deserialize[rInt];
            //    deserialize.Remove(rand);

            //    request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/participants/{deserialized.participant.id}", Method.PUT);
            //    request.AddParameter("participant[seed]", rand.participant.seed);
            //    response = client.Execute(request);

            //}
            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/participants/randomize", Method.POST);
            IRestResponse response = client.Execute(request);
        }

        public static void StartTournament(Tournament tournament) {
            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/start", Method.POST);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void FinalizeTournament(Tournament tournament) {
            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/finalize", Method.POST);
            request.AddParameter("include_participants", 1);
            request.AddParameter("include_matches", 1);

            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void ResetTournament(Tournament tournament) {
            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/reset", Method.POST);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static void DeleteTournament(Tournament tournament) {
            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}", Method.DELETE);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }

        public static Participant GetParticipantById(int id, Tournament tournament) {
            Participant p = null;

            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/participants", Method.GET);

            IRestResponse response = client.Execute(request);

            List<JsonResponseParticipant> deserialize =
                JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            foreach(JsonResponseParticipant deserialized in deserialize) {
                if(deserialized.participant.id == id) {
                    p = deserialized.participant;
                    break;
                }
            }

            return p;
        }

        public static Participant GetParticipantByName(string participantName, Tournament tournament) {
            Participant p = null;

            participantName = participantName.ToLower();

            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/participants", Method.GET);

            IRestResponse response = client.Execute(request);

            List<JsonResponseParticipant> deserialize =
                JsonConvert.DeserializeObject<List<JsonResponseParticipant>>(response.Content);

            foreach (JsonResponseParticipant deserialized in deserialize) {
                string currentParticipantName = deserialized.participant.name.ToLower();
                if(currentParticipantName.Equals(participantName)) {
                    p = deserialized.participant;
                }
            }

            return p;
        }

        public static Match GetMatchByParticipants(Participant p1, Participant p2, Tournament tournament) {
            Match m = null;

            var request = Utilities.InitRequestWithApiKey($"tournaments/{tournament.name}/matches", Method.GET);
            request.AddParameter("state", "open");

            IRestResponse response = client.Execute(request);

            List<JsonResponseMatch> deserialize =
                JsonConvert.DeserializeObject<List<JsonResponseMatch>>(response.Content);

            foreach(JsonResponseMatch deserialized in deserialize) {
                List<int> participantIds = new List<int>();
                participantIds.Add(deserialized.match.player1_id);
                participantIds.Add(deserialized.match.player2_id);
                if(participantIds.Contains(p1.id) && participantIds.Contains(p2.id)) {
                    m = deserialized.match;
                    break;
                }
            }

            return m;
        }

        public static Tournament GetTournamentByName(string tournamentName) {
            Tournament t = null;

            tournamentName = tournamentName.ToLower();

            var request = Utilities.InitRequestWithApiKey($"tournaments", Method.GET);

            IRestResponse response = client.Execute(request);

            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            List<JsonResponseTournament> deserialize
                = JsonConvert.DeserializeObject<List<JsonResponseTournament>>(response.Content, jsonSerializerSettings);

            foreach(JsonResponseTournament deserialized in deserialize) {
                Tournament current = deserialized.tournament;
                string currentName = current.name.ToLower();

                if(currentName.Equals(tournamentName)) {
                    t = current;
                    break;
                }
            }
            return t;
        }
    }
}
