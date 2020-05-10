﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TeamworkTF.Sharp
{
    public class TeamworkClient
    {
        private const string teamworkUrl = "https://teamwork.tf/api/v1/";
        private string apiKey;

        private readonly List<string> mapList = new List<string>
        {
            "arena_badlands",
            "arena_byre",
            "arena_granary",
            "arena_lumberyard",
            "arena_nucleus",
            "arena_offblast_final",
            "arena_ravine",
            "arena_sawmill",
            "arena_watchtower",
            "arena_well",
            "cp_5gorge",
            "cp_badlands",
            "cp_cloak",
            "cp_coldfront",
            "cp_degrootkeep",
            "cp_dustbowl",
            "cp_egypt_final",
            "cp_fastlane",
            "cp_foundry",
            "cp_freight_final1",
            "cp_gorge",
            "cp_gorge_event",
            "cp_granary",
            "cp_gravelpit",
            "cp_gullywash_final1",
            "cp_junction_final",
            "cp_manor_event",
            "cp_mercenarypark",
            "cp_metalworks",
            "cp_mossrock",
            "cp_mountainlab",
            "cp_powerhouse",
            "cp_process_final",
            "cp_snakewater_final1",
            "cp_snowplow",
            "cp_standin_final",
            "cp_steel",
            "cp_sunshine",
            "cp_sunshine_event",
            "cp_vanguard",
            "cp_well",
            "cp_yukon_final",
            "ctf_2fort",
            "ctf_2fort_invasion",
            "ctf_doublecross",
            "ctf_foundry",
            "ctf_gorge",
            "ctf_hellfire",
            "ctf_landfall",
            "ctf_sawmill",
            "ctf_thundermountain",
            "ctf_turbine",
            "ctf_well",
            "itemtest",
            "koth_badlands",
            "koth_bagel_event",
            "koth_brazil",
            "koth_harvest_event",
            "koth_harvest_final",
            "koth_highpass",
            "koth_king",
            "koth_lakeside_event",
            "koth_lakeside_final",
            "koth_lazarus",
            "koth_maple_ridge_event",
            "koth_moonshine_event",
            "koth_nucleus",
            "koth_probed",
            "koth_sawmill",
            "koth_slasher",
            "koth_suijin",
            "koth_viaduct",
            "koth_viaduct_event",
            "mvm_bigrock",
            "mvm_coaltown",
            "mvm_decoy",
            "mvm_example",
            "mvm_ghost_town",
            "mvm_mannhattan",
            "mvm_mannworks",
            "mvm_rottenburg",
            "pass_brickyard",
            "pass_district",
            "pass_timbertown",
            "pd_cursed_cove_event",
            "pd_monster_bash",
            "pd_pit_of_death_event",
            "pd_watergate",
            "pl_badwater",
            "pl_barnblitz",
            "pl_borneo",
            "pl_cactuscanyon",
            "pl_enclosure_final",
            "pl_fifthcurve_event",
            "pl_frontier_final",
            "pl_goldrush",
            "pl_hoodoo_final",
            "pl_millstone_event",
            "pl_rumble_event",
            "pl_snowycoast",
            "pl_swiftwater_final1",
            "pl_thundermountain",
            "pl_upward",
            "plr_bananabay",
            "plr_hightower",
            "plr_hightower_event",
            "plr_nightfall_final",
            "plr_pipeline",
            "rd_asteroid",
            "sd_doomsday",
            "sd_doomsday_event",
            "tc_hydro",
            "tr_dustbowl",
            "tr_target"
        };

        public TeamworkClient(string apiKey)
        {
            this.apiKey = apiKey ?? throw new ArgumentNullException(apiKey, "An API key must be provided. See: https://teamwork.tf/api");
        }

        private async Task<T> Request<T>(string query)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(teamworkUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var delimiter = (query.Contains("?")) ? "&" : "?";
            var response = await client.GetAsync($"{teamworkUrl}{query}{delimiter}key={apiKey}").ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return (T)JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
                {
                    Error = HandleDeserializationError
                });
            }
            else
                return default(T);
        }

        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            _ = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

        public List<string> NormalizedMapName(string query)
        {
            query = (query.Contains('_')) ? query.ToLowerInvariant().Split('_')[1] : query;
            return mapList.Where(x => x.Contains(query)).ToList();
        }

        public string NormalizedGameMode(string query)
        {
            switch (query.ToLowerInvariant().Trim().Replace(' ', '-'))
            {
                case "ad":
                case "attack-defense":
                    return "attack-defend";

                case "capture-the-flag":
                    return "ctf";

                case "cp":
                    return "control-point";

                case "king-of-the-hill":
                    return "koth";

                case "mann-vs-machine":
                    return "mvm";

                case "pl":
                    return "payload";

                case "plr":
                    return "payload-race";

                default:
                    return query;
            }
        }

        #region NEWS

        public async Task<List<News>> GetNewsOverviewAsync()
        {
            return await Request<List<News>>("news").ConfigureAwait(false);
        }

        public async Task<News> GetNewsPostAsync(string hash)
        {
            return await Request<News>($"news/hash/{hash}").ConfigureAwait(false);
        }

        public async Task<List<News>> GetNewsByPageAsync(int hash)
        {
            return await Request<List<News>>($"news?page={hash}").ConfigureAwait(false);
        }

        public async Task<List<News>> GetNewsByProviderAsync(string hash)
        {
            return await Request<List<News>>($"news?provider={hash}").ConfigureAwait(false);
        }

        #endregion NEWS

        #region CREATORS

        public async Task<List<Creator>> GetCreatorByIDAsync(string steamID)
        {
            return await Request<List<Creator>>($"youtube-creator/steamid/{steamID}").ConfigureAwait(false);
        }

        #endregion CREATORS

        #region QUICKPLAY

        public async Task<GameMode> GetGameModeAsync(string gamemode)
        {
            return await Request<GameMode>($"quickplay/{NormalizedGameMode(gamemode)}").ConfigureAwait(false);
        }

        public async Task<GameMode> GetGameModeListAsync()
        {
            return await Request<GameMode>("quickplay").ConfigureAwait(false);
        }

        public async Task<List<Server>> GetGameModeServerAsync(string gamemode)
        {
            return await Request<List<Server>>($"quickplay/{NormalizedGameMode(gamemode)}/servers").ConfigureAwait(false);
        }

        public async Task<List<Server>> GetGameServerInfoAsync(string ip, int port = 0)
        {
            var endpoint = $"quickplay/server?ip={ip}" + ((port > 0) ? $"&port={port}" : "");
            return await Request<List<Server>>(endpoint).ConfigureAwait(false);
        }

        #endregion QUICKPLAY

        #region SERVERS

        public async Task<Provider> GetCommunityProviderAsync(string provider)
        {
            return await Request<Provider>($"community/provider/{provider}").ConfigureAwait(false);
        }

        public async Task<List<Server>> GetCommunityProviderServersAsync(string provider)
        {
            return await Request<List<Server>>($"community/provider/{provider}/servers").ConfigureAwait(false);
        }

        public async Task<ProviderStats> GetCommunityProviderStatsAsync(string provider)
        {
            return await Request<ProviderStats>($"community/provider/{provider}/stats").ConfigureAwait(false);
        }

        public async Task<CompProvider> GetCompetitiveProviderAsync(string provider)
        {
            return await Request<CompProvider>($"competitive/provider/{provider}").ConfigureAwait(false);
        }

        private async Task<List<Server>> GetCompetitiveProviderServersAsync(string provider)
        {
            // Endpoint is deprecated
            return await Request<List<Server>>($"competitive/provider/{provider}/servers").ConfigureAwait(false);
        }

        public async Task<CompProviderStats> GetCompetitiveProviderStatsAsync(string provider)
        {
            return await Request<CompProviderStats>($"competitive/provider/{provider}/stats").ConfigureAwait(false);
        }

        public async Task<List<ServerList>> GetCustomServerListsAsync()
        {
            return await Request<List<ServerList>>($"customserverlist").ConfigureAwait(false);
        }

        public async Task<ServerList> GetSpecificServerListAsync(int id)
        {
            return await Request<ServerList>($"customserverlist/{id}").ConfigureAwait(false);
        }

        public async Task<List<Server>> GetServersFromServerListAsync(int id)
        {
            return await Request<List<Server>>($"customserverlist/{id}/servers").ConfigureAwait(false);
        }

        #endregion SERVERS

        #region GAMEMAPS

        public async Task<Map> GetMapStatsAsync(string mapName)
        {
            return await Request<Map>($"map-stats/map/{NormalizedMapName(mapName).FirstOrDefault()}").ConfigureAwait(false);
        }

        public async Task<MapThumbnail> GetMapThumbnailAsync(string mapName)
        {
            return await Request<MapThumbnail>($"map-stats/mapthumbnail/{mapName}").ConfigureAwait(false);
        }

        public async Task<ThumbnailContext> GetMapThumbnailContextAsync(string mapName)
        {
            return await Request<ThumbnailContext>($"map-stats/mapimages/{mapName}").ConfigureAwait(false);
        }

        public async Task<List<Map>> GetMapsBySearchAsync(string search)
        {
            return await Request<List<Map>>($"map-stats/search?search_term={search}").ConfigureAwait(false);
        }

        #endregion GAMEMAPS
    }
}