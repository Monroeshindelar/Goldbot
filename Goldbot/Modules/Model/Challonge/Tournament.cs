using Newtonsoft.Json;
using System;

namespace Goldbot.Modules.Model.Challonge {
    public class Tournament {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string description { get; set; }
        public string tournament_type { get; set; }
        public Nullable<DateTime> started_at { get; set; }
        public Nullable<DateTime> completed_at { get; set; }
        public bool require_score_agreement { get; set; }
        public bool notify_users_when_matches_open { get; set; }
        public DateTime created_at { get; set; }
        public Nullable<DateTime> updated_at { get; set; }
        public string state { get; set; }
        public bool open_signup { get; set; }
        public bool notify_users_when_tournament_ends { get; set; }
        public Nullable<int> progress_meter { get; set; }
        public bool quick_advance { get; set; }
        public bool hold_third_place_match { get; set; }
        public string pts_for_game_win { get; set; }
        public string pts_for_game_time { get; set; }
        public string pts_for_match_win { get; set; }
        public string pts_for_match_tie { get; set; }
        public string pts_for_bye { get; set; }
        public int swiss_rounds { get; set; }
        [JsonProperty(PropertyName = "private")]
        public bool private_ { get; set; }
        public string ranked_by { get; set; }
        public bool show_rounds { get; set; }
        public bool hide_forum { get; set; }
        public bool sequential_pairings { get; set; }
        public bool accept_attachments { get; set; }
        public string rr_pts_for_game_win { get; set; }
        public string rr_pts_for_game_tie { get; set; }
        public string rr_pts_for_match_win { get; set; }
        public string rr_pts_for_match_tie { get; set; }
        public bool created_by_api { get; set; }
        public bool credit_capped { get; set; }
        public string category { get; set; }
        public bool hide_seeds { get; set; }
        public int preditction_method { get; set; }
        public Nullable<DateTime> predictions_opened_at { get; set; }
        public bool anonymous_voting { get; set; }
        public int max_predicition_per_user { get; set; }
        public Nullable<int> signup_cap { get; set; }
        public Nullable<int>  game_id { get; set; }
        public int participants_count { get; set; }
        public bool groups_stages_enabled { get; set; }
        public bool allow_participant_match_reporting { get; set; }
        public Nullable<bool> teams { get; set; }
        public Nullable<DateTime> check_in_duration { get; set; }
        public Nullable<DateTime> start_at { get; set; }
        public Nullable<DateTime> started_checking_in_at { get; set; }
        public string[] tie_breaks { get; set; }
        public Nullable<DateTime> locked_at { get; set; }
        public Nullable<int> event_id { get; set; }
        public Nullable<bool> public_predictions_before_start_time { get; set; }
        public bool ranked { get; set; }
        public Nullable<int>  grand_finals_modifier { get; set; }
        public Nullable<bool> predict_the_losers_bracket { get; set; }
        public string spam { get; set; }
        public string ham { get; set; }
        public Nullable<int> rr_iterations { get; set; }
        public Nullable<int> tournament_registration_id { get; set; }
        public Nullable<bool> donation_contest_enabled { get; set; }
        public Nullable<int> mandatory_donation { get; set; }
        public Nullable<bool> auto_assign_stations { get; set; }
        public Nullable<bool> only_start_matches_with_stations { get; set; }
        public string registration_fee { get; set; }
        public string registration_type { get; set; }
        public string description_source { get; set; }
        public string subdomain { get; set; }
        public string full_challonge_url { get; set; }
        public string live_image_url { get; set; }
        public string sign_up_url { get; set; }
        public bool review_before_finalizing { get; set; }
        public bool accepting_predictions { get; set; }
        public bool participants_locked { get; set; }
        public string game_name { get; set; }
        public bool participants_swappable { get; set; }
        public bool team_convertable { get; set; }
        public bool group_stage_were_started { get; set; }
    }
}
