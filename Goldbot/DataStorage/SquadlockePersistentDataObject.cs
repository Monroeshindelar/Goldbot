using System.Collections.Generic;

namespace Goldbot.DataStorage {
    public class SquadlockePersistentDataObject {
        public string currentTournamentName { get; set; }
        public Dictionary<string, bool> ready { get; set; }
        public List<string> encounters { get; set; }
    }
}
