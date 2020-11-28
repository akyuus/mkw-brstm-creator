using System;
using System.Collections.Generic;

namespace brstm_maker 
{

    class Tracks
    {
        public static readonly Dictionary<string, string> trackNames = new Dictionary<string, string>() 
        {
            {"finish-first", "o_FanfareGP1_32"},
            {"finish-ok", "o_FanfareGP2_32"},
            {"finish-bad", "o_FanfareGPdame_32"},
            {"race-intro-wifi", "o_Crs_In_Fan_Wifi"},
            {"lc", "n_circuit32_n"},
            {"mmm", "n_farm_n"},
            {"mg", "n_kinoko_n"},
            {"tf", "STRM_N_FACTORY_N"},
            {"mc", "n_circuit32_n"},
            {"cm", "n_shopping32_n"},
            {"dks", "n_snowboard32_n"},
            {"dksc", "n_snowboard32_n"},
            {"wgm", "STRM_N_TRUCK_N"},
            {"dc", "n_daisy32_n"},
            {"kc", "STRM_N_WATER_N"},
            {"mt", "n_maple_n"},
            {"maple", "n_maple_n"},
            {"gv", "n_volcano32_n"},
            {"ddr", "STRM_N_DESERT_N"},
            {"mh", "STRM_N_RIDGEHIGHWAY_N"},
            {"moonview", "STRM_N_RIDGEHIGHWAY_N"},
            {"bcwii", "STRM_N_KOOPA_N"},
            {"rr", "n_rainbow32_n"},
            {"rainbow", "n_rainbow32_n"},
            {"rpb", "r_gc_beach32_n"},
            {"ryf", "r_ds_jungle32_n"},
            {"gv2", "r_sfc_obake32_n"},
            {"rmr", "r_64_circuit32_n"},
            {"raceway", "r_64_circuit32_n"},
            {"rsl", "r_64_sherbet32_n"},
            {"sherbie", "r_64_sherbet32_n"},
            {"sgb", "r_agb_beach32_n"},
            {"rds", "r_ds_town32_n"},
            {"delfino", "r_ds_town32_n"},
            {"rws", "r_gc_stadium32_n"},
            {"rdh", "r_ds_desert32_n"},
            {"bc3", "r_agb_kuppa32_n"},
            {"rmc", "r_gc_circuit32_n"},
            {"mc3", "r_sfc_circuit32_n"},
            {"rpg", "r_ds_garden32_n"},
            {"gardens", "r_ds_garden32_n"},
            {"dkm", "r_gc_mountain32_n"},
            {"rbc", "r_64_kuppa32_n"},
            {"FORT", "r_64_kuppa32_n"}
        };

        //maps the relevant key above to the number of channels they need in their brstm (ex. "kc" gets mapped to 8)
        public static readonly Dictionary<string, int> multiChannelTracks = new Dictionary<string, int>() 
        {
            {"tf", 4},
            {"wgm", 4},
            {"kc", 8},
            {"ddr", 4},
            {"mh", 4},
            {"moonview", 4},
            {"bcwii", 8} //technically only needs to be 6, but 8 is safer
        };

        public static int getChannelCount(string trackAbbreviation) 
        {
            
            if(multiChannelTracks.ContainsKey(trackAbbreviation)) 
            {
                return multiChannelTracks[trackAbbreviation];
            }

            else return 2;
        }

        public static string getFilename(string trackAbbreviation)
        {
            if(trackNames.ContainsKey(trackAbbreviation)) {
                return trackNames[trackAbbreviation];
            }
            else throw new System.ArgumentException("That track abbreviation was not found.");
        }
    }
}