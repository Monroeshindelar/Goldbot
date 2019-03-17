using Goldbot.DataStorage;
using Goldbot.Modules.Model.Challonge;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Goldbot.Modules.CommandHelpers {
    public static class SquadlockeHelper {
        private static string currentTournamentName;
        private static Tournament currentTournament;
        private static Dictionary<string, bool> readiedParticipants;
        private static List<string> encounters;

        //I really hate this but what are you gonna do
        static SquadlockeHelper() {
            SquadlockePersistentDataObject data =
                new SquadlockePersistentDataObject();

            if (File.Exists("Resources/SquadlockeDataStorage.json")) {
                string json = File.ReadAllText("Resources/SquadlockeDataStorage.json");
                data = JsonConvert.DeserializeObject<SquadlockePersistentDataObject>(json);
                currentTournamentName = data.currentTournamentName;
                readiedParticipants = data.ready;
                encounters = data.encounters;
                currentTournament = TournamentHelper.GetTournamentByName(currentTournamentName);
            } else File.WriteAllText("Resources/SquadlockeDataStorage.json", "");
        }

        public static void InitializeSquadlocke(string initialTournamentName, List<string> participants, string remainingArgs = null, string tournamentUrl = null) {
            string url = tournamentUrl != null ? tournamentUrl : initialTournamentName;
            readiedParticipants = new Dictionary<string, bool>();
            encounters = new List<string>();
            foreach (string participant in participants)
                readiedParticipants.Add(participant, false);

            TournamentHelper.CreateTournament(initialTournamentName, url, remainingArgs);
            TournamentHelper.AddParticipantsToTournament(participants, currentTournament);
            SaveData();
        }

        public static void UpdateMatch(Participant p1, Participant p2, int score1, int score2) {
            TournamentHelper.UpdateMatch(p1, p2, score1, score2, currentTournament);
        }

        public static void StartTournament() {
            TournamentHelper.StartTournament(currentTournament);
        }

        public static bool ReadyUpSL(string username) {
            if (readiedParticipants[username] == true) return false;
            else {
                readiedParticipants[username] = true;
                SaveData();
                return true;
            }
        }

        public static string GetEncounterList() {
            string e = "";
            foreach (string enc in encounters)
                e += $"{enc}\n";
            return e;
        }

        public static Tournament GetCurrentTournament() {
            return currentTournament;
        }

        public static bool AllPlayersReady() {
            foreach (string participant in readiedParticipants.Keys)
                if (!readiedParticipants[participant]) return false;
            return true; 
        }

        public static Dictionary<string, bool> GetReadyList() {
            return readiedParticipants;
        }

        public static Participant GetSquadlockeParticipantByName(string participantName) {
            return TournamentHelper.GetParticipantByName(participantName, currentTournament);
        }

        public static void AddEncounter(string encounterLocation) {
            encounters.Add(encounterLocation);
        }

        private static void SaveData() {
            SquadlockePersistentDataObject saveData =
                new SquadlockePersistentDataObject();

            saveData.currentTournamentName = currentTournamentName;
            saveData.encounters = encounters;
            saveData.ready = readiedParticipants;

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText("Resources/SquadlockeDataStorage.json", json);
        }

    }
}
