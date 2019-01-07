using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSGSI;
using CSGSI.Nodes;


namespace Goldbot.Modules {
    public class Misc : ModuleBase<SocketCommandContext> {

        [Command("help")]
        public async Task Help([Remainder]string args = null) {
            string toBuild = "```";
            if (args == null) {
                toBuild += "==========================================================================\n" +
                           "                       Goldbot - Created by Monroe Shidelar               \n" +
                           "==========================================================================\n" +
                           "Commands:\n" +
                           "    Name\n" +
                           "        tm - Ten Man\n" +
                           "    Description\n" +
                           "        tm is a series of commands for creating and organizing a ten man\n" +
                           "        instance. These commands include picking captains, picking players\n" +
                           "        and map pick bans. For more information about tm and its commands\n" +
                           "        use the command !help tm\n\n" +
                           "    Name\n" +
                           "        help - Help\n" +
                           "    Synopsis\n" +
                           "        !help [Command]\n" +
                           "    Description\n" +
                           "        Help is a command that details the capabilities of Goldbot and\n" +
                           "        provides information for correct usage of the provided commands.\n" +
                           "        Help takes an optional command argument. If a command is given\n" +
                           "        as an argument, help will provide more detailed information about\n" +
                           "        that command. Excluding the optional message will give you this\n" +
                           "        message.```";
            } else {
                switch(args) {
                    case "tm":
                        toBuild += "Commands:\n" +
                                   "    Name\n" +
                                   "        init - Initialize\n" +
                                   "    Synopsis\n" +
                                   "        !tm init [10 Mentioned Users]\n" +
                                   "    Description\n" +
                                   "        This command intializes a 10 man instance. This command requires\n" +
                                   "        you to directly mention the 10 users that will participate in the\n" +
                                   "        ten man. Other ten man commands will be locked to these users.\n" +
                                   "        This commands sets up all the environment variables for the program\n\n" +
                                   "    Name\n" +
                                   "        free - Free\n" +
                                   "    Synopsis\n" +
                                   "        !tm free [option]\n" +
                                   "    Description\n" +
                                   "        This command will deconstruct the 10 man, removing all roles from the\n" +
                                   "        participating users." +
                                   "    Options\n" +
                                   "        -p, -players\n" +
                                   "            Free non-captain participants only (NOT IMPLEMENTED).\n\n" +
                                   "    Name\n" +
                                   "        pickcaptains - Pick captains\n" +
                                   "    Synopsis\n" +
                                   "        !tm pickcaptains [-m 2 Mentioned Players]"; 

                        
                        break;
                }
            }

            await Context.Channel.SendMessageAsync(toBuild);
        }

        [Command("tm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TenMan([Remainder]string args) {
            string[] tokens = args.Split(' ');

            if (tokens.Length == 0) return;
            string command = tokens[0].ToLower();

            SocketRole master = Context.Guild.Roles.First(x => x.Name == "Monroe"),
            capA = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team A Captain"),
            capB = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team B Captain"),
            teamA = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team A"),
            teamB = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team B");

            SocketGuildUser caller = Context.User as SocketGuildUser;

            bool a = true;
            if (caller.Roles.Contains(capB)) a = false;

            EmbedBuilder embed = null;

            switch (command) {
                case "init":
                    if(Context.Message.MentionedUsers.Count() < 10) {
                        await Context.Channel.SendMessageAsync("You have no listed the correct amount of players to initialize a ten man.\nIt's in the name.");
                        return;
                    }
                    
                    Global.participants = new List<SocketGuildUser>();
                    Global.mapThumbnails = new Dictionary<string, string>();
                    Global.playersRemaining = new List<string>();
                    Global.mapsRemaining = new List<string>();

                    Global.mapsRemaining.Add("mirage"); Global.mapsRemaining.Add("cache"); Global.mapsRemaining.Add("inferno");
                    Global.mapsRemaining.Add("overpass"); Global.mapsRemaining.Add("train"); Global.mapsRemaining.Add("nuke");
                    Global.mapsRemaining.Add("dust2");

                    Global.mapThumbnails.Add("mirage", "https://vignette.wikia.nocookie.net/cswikia/images/a/a7/CSGO_de_Mirage.jpg/revision/latest?cb=20140316221852");
                    Global.mapThumbnails.Add("cache", "https://www.killping.com/blog/wp-content/uploads/2018/04/Cache.jpg");
                    Global.mapThumbnails.Add("inferno", "https://vignette.wikia.nocookie.net/cswikia/images/f/f0/Inferno.jpg/revision/latest?cb=20161014013320");
                    Global.mapThumbnails.Add("overpass", "https://vignette.wikia.nocookie.net/cswikia/images/6/6e/Csgo-de-overpass.png/revision/latest?cb=20140820130544");
                    Global.mapThumbnails.Add("train", "https://liquipedia.net/commons/images/thumb/5/56/Train_csgo.jpg/600px-Train_csgo.jpg");
                    Global.mapThumbnails.Add("nuke", "https://vignette.wikia.nocookie.net/cswikia/images/5/51/De_nuke_thumbnail.jpg/revision/latest?cb=20180209112248");
                    Global.mapThumbnails.Add("dust2", "https://cdn.vox-cdn.com/thumbor/5668HDDEe6lGfyp96HMJGa7IV6I=/0x0:2560x1600/1200x800/filters:focal(1076x596:1484x1004)/cdn.vox-cdn.com/uploads/chorus_image/image/57107595/csgo_dust2.0.jpg");

                    foreach (SocketUser participant in Context.Message.MentionedUsers) {
                        Global.participants.Add(participant as SocketGuildUser);
                        Global.playersRemaining.Add(participant.Username);
                    }
                    string rp = "";
                    foreach (string playername in Global.playersRemaining) rp += playername + "\n";
                    string rm = "";
                    foreach (string mapname in Global.mapsRemaining) rm += mapname + "\n";

                    RestUserMessage remainingPlayers = await Context.Channel.SendMessageAsync("```css\nRemaining Players:\n" + rp + "```");
                    RestUserMessage teamACompositionMessage = await Context.Channel.SendMessageAsync("```css\nTeam A: ```");
                    RestUserMessage teamBCompositionMessage = await Context.Channel.SendMessageAsync("```css\nTeam B: ```");
                    RestUserMessage remainingMapMessage = await (Context.Channel.SendMessageAsync("```css\nMaps Remaining:\n" + rm + "```"));
                    RestUserMessage mapPickBanMessage = await Context.Channel.SendMessageAsync("```css\nMap Pick/Ban: ```");
                    Global.TeamAMessageID = teamACompositionMessage.Id;
                    Global.TeamBMessageID = teamBCompositionMessage.Id;
                    Global.MapPickMessaegID = mapPickBanMessage.Id;
                    Global.RemainingPlayersMessageID = remainingPlayers.Id;
                    Global.RemainingMapsMessageID = remainingMapMessage.Id;
                    Global.IsBanPhase = true;
                    Global.TeamATurn = true;
                    SocketRole[] roles = { teamA, teamB, capA, capB };
                    break;
                case "free":
                    if (Global.participants.Count == 0) {
                        await Context.Channel.SendMessageAsync("A pug has not been instantiated. There is nothing to free.\nPlease initialize a game using the init command before calling free.");
                        return;
                    }
                    await Context.Channel.SendMessageAsync("Beginning deconstruction.");
                    SocketRole[] rolesToRemove = { teamA, teamB, capA, capB };
                    foreach (SocketGuildUser participant in Global.participants) {
                        await participant.RemoveRolesAsync(rolesToRemove);
                    }
                    Global.participants = null;
                    await Context.Channel.SendMessageAsync("Players freed.");
                    break;
                case "pickcaptains":
                    if (Global.participants.Count < 10) {
                        await Context.Channel.SendMessageAsync("A pug has not been instantiated. \nCaptains cannot be selected. \nPlease initialize a game using the init command before trying to pick captains");
                        return;
                    }
                    Random r = new Random();
                    int capAIndex = r.Next(0, Global.participants.Count);
                    SocketGuildUser cap = Global.participants[0];
                    embed = EmbedHelper("Team A Captain: ", cap.Username, cap.GetAvatarUrl(), new Color(255, 0, 0));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    await cap.AddRoleAsync(capA);
                    var msg = await (Context.Channel.GetMessageAsync(Global.TeamAMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```" + msg.Content.ToString().Trim(new char[] { '`' }) + "\n" + cap.Username + "```");

                    Global.playersRemaining.Remove(cap.Username);

                    int capBIndex = capAIndex;
                    while (capBIndex == capAIndex) capBIndex = r.Next(0, Global.participants.Count);
                    cap = Global.participants[capBIndex];
                    embed = EmbedHelper("Team B Captain: ", cap.Username, cap.GetAvatarUrl(), new Color(0, 0, 255));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    await cap.AddRoleAsync(capB);

                    msg = await (Context.Channel.GetMessageAsync(Global.TeamBMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```" + msg.Content.ToString().Trim(new char[] { '`' }) + "\n" + cap.Username + " - Captain" + "```");
                    Global.playersRemaining.Remove(cap.Username);

                    rp = "";
                    foreach (string playername in Global.playersRemaining) rp += playername + "\n";
                    msg = await (Context.Channel.GetMessageAsync(Global.RemainingPlayersMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nPlayers Remaining:\n" + rp + "```");
                    break;
                case "pickplayer":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }
                    if (tokens.Length == 1) return;

                    if(a && !Global.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to pick a player.\nPlease wait your turn.");
                        return;
                    } else if(!a && Global.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to pick a player.\nPlease wait your turn.");
                        return;
                    }
                    
                    if(Global.playersRemaining.Count() == 0) {
                        await Context.Channel.SendMessageAsync("All the players have been picked.\nIt's time to start the pick/ban phase.\nTo start banning and picking maps use the banmap and pickmap commands.\nThe pick ban order is B-B-P-P-B-B");
                        return;
                    }

                    var pick = Context.Message.MentionedUsers.First();

                    var user = pick as SocketGuildUser;
                    if (user.Roles.Contains(teamB) || user.Roles.Contains(capB)
                        || user.Roles.Contains(teamA) || user.Roles.Contains(capA) || 
                        !Global.participants.Contains(user)) 
                    {
                        await Context.Channel.SendMessageAsync(pick.Username + " is unavailable.");
                        return;
                    }

                    if (a) await (pick as IGuildUser).AddRoleAsync(teamA);
                    else await (pick as IGuildUser).AddRoleAsync(teamB);

                    Global.playersRemaining.Remove(pick.Username);

                    embed = EmbedHelper((a ? "Team A Picked: " : "Team B Picked:"), pick.Username, pick.GetAvatarUrl(), (a ? new Color(255, 0, 0) : new Color(0, 0, 255)));
                    await Context.Channel.SendMessageAsync("", false, embed);

                    RestUserMessage message;
                    if (a) message = await Context.Channel.GetMessageAsync(Global.TeamAMessageID) as RestUserMessage;
                    else message = await Context.Channel.GetMessageAsync(Global.TeamBMessageID) as RestUserMessage;
                    await message.ModifyAsync(x => x.Content = "```" + message.Content.ToString().Trim(new char[] { '`' }) + "\n" + pick.Username + "```");

                    rp = "";
                    foreach (string playername in Global.playersRemaining) rp += playername + "\n";
                    msg = await (Context.Channel.GetMessageAsync(Global.RemainingPlayersMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nPlayers Remaining:\n" + rp + "```");

                    if (Global.playersRemaining.Count() == 0) {
                        await Context.Channel.SendMessageAsync("All player have been pick.\nBegin the map pick/ban phase");
                        Global.IsBanPhase = true;
                    }
                    Global.TeamATurn = !Global.TeamATurn; 
                    break;
                case "pickmap":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }
                    string mappick = tokens[1].ToLower();
                    if (!Global.mapThumbnails.ContainsKey(mappick)) {
                        await Context.Channel.SendMessageAsync(mappick + " does not exist in the competitive pool.");
                        return;
                    }

                    if(Global.mapsRemaining.Count <= 1) {
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is over.\nYou can start the game now.");
                        return;
                    }

                    if (Global.playersRemaining.Count() != 0) {
                        await Context.Channel.SendMessageAsync("It is currently still the player pick stage.\nPlease wait for all players to be picked before moving on to the map pick/ban phase");
                    }

                    if(Global.IsBanPhase) {
                        await Context.Channel.SendMessageAsync("It is currently the ban phase.\nPlease wait for both teams to ban their maps before picking a map.");
                        return;
                    }

                    if(!Global.mapsRemaining.Contains(mappick)) {
                        await Context.Channel.SendMessageAsync(mappick + " has already been selected.\nPlease select another map.");
                        return;
                    }

                    if (a && !Global.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to pick a map.\nPlease wait your turn to pick a map.");
                        return;
                    }
                    else if (!a && Global.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to pick a map.\nPlease wait your turn to pick a map.");
                        return;
                    }

                    embed = EmbedHelper((a ? "Team A Picked: " : "Team B Picked: "), mappick, Global.mapThumbnails[mappick], new Color(0, 255, 0)); ;
                    await Context.Channel.SendMessageAsync("", false, embed);

                    Global.mapsRemaining.Remove(mappick);

                    RestUserMessage mapPickMessage = await (Context.Channel.GetMessageAsync(Global.MapPickMessaegID)) as RestUserMessage;
                    await mapPickMessage.ModifyAsync(x => x.Content = "```" + mapPickMessage.Content.ToString().Trim(new char[] { '`' }) + "\n" + mappick +" - Picked by " + (a ? "Team A" : "Team B") + "```");

                    rm = "";
                    foreach (string mapname in Global.mapsRemaining) rm += mapname + "\n";
                    msg = await (Context.Channel.GetMessageAsync(Global.RemainingMapsMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nRemaining Maps:\n" + rm + "```");

                    if (!a) Global.IsBanPhase = true;
                    Global.TeamATurn = !Global.TeamATurn;
                    break;
                case "banmap":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }

                    if(Global.mapsRemaining.Count() <= 1) {
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is over.\nYou can start the game now.");
                        return;
                    }

                    if (Global.playersRemaining.Count() != 0) {
                        await Context.Channel.SendMessageAsync("It is currently still the player pick stage.\nPlease wait for all players to be picked before moving on to the map pick/ban phase");
                        return;
                    }

                    if (!Global.IsBanPhase) {
                        await Context.Channel.SendMessageAsync("It is currently the pick phase.\nPlease wait for both teams to pick their maps before banning a map.");
                        return;
                    }

                    if(a && !Global.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to ban a map.\nPlease wait your turn to ban a map.");
                        return;
                    } else if (!a && Global.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to ban a map.\nPlease wait your turn to ban a map.");
                        return;
                    }

                    string mapban = tokens[1].ToLower();
                    if (!Global.mapThumbnails.ContainsKey(mapban)) {
                        await Context.Channel.SendMessageAsync(mapban + " does not exist in the competitive pool.");
                        return;
                    }

                    if(!Global.mapsRemaining.Contains(mapban)) {
                        await Context.Channel.SendMessageAsync(mapban + " has already been selected.\nPlease select another map.");
                        return;
                    }

                    embed = EmbedHelper((a ? "Team A Banned: " : "Team B Banned: "), mapban, Global.mapThumbnails[mapban], new Color(255, 0, 0));;
                    await Context.Channel.SendMessageAsync("", false, embed);

                    Global.mapsRemaining.Remove(mapban);

                    RestUserMessage mapBanMessage = await (Context.Channel.GetMessageAsync(Global.MapPickMessaegID)) as RestUserMessage;
                    await mapBanMessage.ModifyAsync(x => x.Content = "```" + mapBanMessage.Content.ToString().Trim(new char[] { '`' }) + "\n" + mapban + " - Banned by " + (a ? "Team A" : "Team B") + "```");

                    rm = "";
                    foreach (string mapname in Global.mapsRemaining) rm += mapname + "\n";

                    msg = await (Context.Channel.GetMessageAsync(Global.RemainingMapsMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nRemaining Maps:\n" + rm + "```");

                    if (!a) Global.IsBanPhase = false;
                    Global.TeamATurn = !Global.TeamATurn;

                    if(Global.mapsRemaining.Count == 1) {
                        string name = Global.mapsRemaining.First();
                        embed = EmbedHelper("Decider: ", name, Global.mapThumbnails[name], new Color(0, 255, 0));
                        await Context.Channel.SendMessageAsync("", false, embed);
                        RestUserMessage decider = (await Context.Channel.GetMessageAsync(Global.MapPickMessaegID)) as RestUserMessage;
                        await decider.ModifyAsync(x => x.Content = "```" + decider.Content.ToString().Trim(new char[] { '`' }) + "\n" + name + " - Decider Map ```");
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is completed.\nJoin your respective voice channels and begin the game!");
                    }
                    break;
                case "startgsl":
                case "startgamestatelistener":
                    Global.gsl = new GameStateListener("http://localhost:3000");
                    Global.gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
                    if (!Global.gsl.Start()) {
                        Console.WriteLine("Failed to listen to the game");
                        return;
                    }
                    Console.WriteLine("Listening...");
                    break;
                default:
                    await Context.Channel.SendMessageAsync("Command is not recognized.");
                    break;
            }
        }

        void OnNewGameState(GameState gs) {
            if(gs.Map.TeamCT.Score == 16 || gs.Map.TeamT.Score == 16 ||
                (gs.Map.TeamCT.Score == 15 && gs.Map.TeamT.Score == 15)) {
                foreach (var player in gs.AllPlayers) {
                    Console.WriteLine(player.Name);
                    Console.WriteLine(player.SteamID);
                    Console.WriteLine(player.MatchStats.Kills);
                    Console.WriteLine(player.MatchStats.Deaths);
                }
            }
        }

        private EmbedBuilder EmbedHelper(string title, string desc, string url, Color color) {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(desc);
            embed.WithColor(color);
            embed.WithThumbnailUrl(url);
            return embed;
        }
    }
}
