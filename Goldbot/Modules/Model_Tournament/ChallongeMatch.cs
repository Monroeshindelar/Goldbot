using System;

namespace Goldbot.Modules.Model_Tournament {
    public class ChallongeMatch {
        public int id { get; set; }
        public int tournament_id { get; set; }
        public string state { get; set; }
        public int player1_id { get; set; }
        public int player2_id { get; set; }
        public Nullable<int> player1_prereq_match_id { get; set; }
        public Nullable<int> player2_prereq_match_id { get; set; }
        public bool player1_is_prereq_match_loser { get; set; }
        public bool player2_is_prereq_match_loser { get; set; }
        public Nullable<int> winner_id { get; set; }
        public Nullable<int> loser_id { get; set; }
        public DateTime started_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string identifier { get; set; }
        public bool has_attachment { get; set; }
        public int round { get; set; }
        public string player1_votes { get; set; }
        public string player2_votes { get; set; }
        public Nullable<int> group_id { get; set; }
        public Nullable<int> attachment_count { get; set; }
        public Nullable<DateTime> start_time { get; set; }
        public string Location { get; set; }
        public Nullable<DateTime> underway_at { get; set; }
        public bool optional { get; set; }
        public Nullable<int> rushb_id { get; set; }
        public Nullable<DateTime> completed_at { get; set; }
        public int suggested_play_order { get; set; }
        public Nullable<bool> forfeited { get; set; }
        public string prerequisite_match_ids_csv { get; set; }
        public string scores_csv { get; set; }
    }
}
