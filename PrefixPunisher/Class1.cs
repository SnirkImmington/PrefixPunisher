using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using Hooks;

namespace PrefixPunisher
{
    [APIVersion(1, 12)]
    public class PluginMain : TerrariaPlugin
    {
        private static ConfigFile Config = new ConfigFile(); private static DateTime LastCheck = DateTime.UtcNow;

        private static List<OkayCombo> OkayStuff = new List<OkayCombo>();

        private static string ConfigPath { get { return Path.Combine(TShock.SavePath, "PrefixPunisher Config.json"); } }
        private static string DataPath { get { return Path.Combine(TShock.SavePath, "PrefixPunisher OK Prefixes.txt"); } }

        // I have made this and class Prefix public so anyone referecing my code can make use of so many hours of typing.
        // And also, Inan, this code is "clean" :P
        // I admit that it took too much time to be practical but simple funtionality sometimes takes lots of coding.
        public static Prefix[] Prefixes = new Prefix[] 
        {
            #region Prefixes!
            new Prefix("None", 0, false), // none
            new Prefix("Large", 2), // Large                 1
            new Prefix("Massive", 2), // Massive
            new Prefix("Dangerous", 2), // Dangerous
            new Prefix("Savage", 2), // Savage
            new Prefix("Sharp", 2), // Sharp                5
            new Prefix("Pointy", 2), // Pointy                
            new Prefix("Tiny", 2, true), // Tiny
            new Prefix("Terrible", 2, true), // Terrible
            new Prefix("Small", 2, true), // Small
            new Prefix("Dull", 2, true), // Dull            10
            new Prefix("Unhappy", 2, true), // Unhappy         
            new Prefix("Bulky", 2), // Bulky
            new Prefix("Shameful", 2, true), // Shameful
            new Prefix("Heavy", 2), // Heavy
            new Prefix("Light", 2), // Light                15 
            new Prefix("Sighted", 4), // Sighted              
            new Prefix("Rapid", 4), // Rapid
            new Prefix("Hasty", 4), // Hasty
            new Prefix("Intimidating", 4), // Indimidating
            new Prefix("Deadly", 4), // Deadly                20
            new Prefix("Staunch", 4), // Staunch               
            new Prefix("Awful", 4, true), // Awful
            new Prefix("Lethargic", 4, true), // Lethargic
            new Prefix("Awkward", 4, true), // Awkward
            new Prefix("Powerful", 4), // Powerful             25      
            new Prefix("Mystic", 3), // Mystic
            new Prefix("Adept", 3), // Adept
            new Prefix("Masterful", 3), // Masterful
            new Prefix("Inept", 3, true), // Inept            
            new Prefix("Ignorant", 3, true), // Ignorant      30 
            new Prefix("Deranged", 3, true), // Deranged
            new Prefix("Intense", 3), // Intense
            new Prefix("Taboo", 3), // Taboo
            new Prefix("Celestial", 3), // Celestial            
            new Prefix("Furious", 3), // Furious                35
            new Prefix("Manic", 3), // f u regigit
            new Prefix("Keen", 0), // Keen
            new Prefix("Superior", 0), // Superior
            new Prefix("Forceful", 0), // Forceful              
            new Prefix("Broken", 0, true), // Broken
            new Prefix("Damaged", 0, true), // Damaged          
            new Prefix("Shoddy", 0, true), // Shoddy
            new Prefix("Quick", 5), // Quick
            new Prefix("Deadly", 5), // Deadly                  
            new Prefix("Agile", 5), // Agile
            new Prefix("Nimble", 5), // Nimble                  
            new Prefix("Murderous", 5), // Murderous
            new Prefix("Slow", 5, true), // Slow
            new Prefix("Sluggish", 5),
            new Prefix("Lazy", 5, true),
            new Prefix("Annoying", 5, true), //                 
            new Prefix("Nasty", 5),
            new Prefix("Manic", 3), // new position?
            new Prefix("Hurtful", 0),                           
            new Prefix("Strong", 0),
            new Prefix("Unpleasant", 0),                        //55
            new Prefix("Weak", 0, true), 
            new Prefix("Ruthless", 0),
            new Prefix("Frenzying", 4), // Frenzying is moved here!
            new Prefix("Godly", 0), // Godly                    60
            new Prefix("Demonic", 0), // Demonic                //60
            new Prefix("Zealous", 0), // Zealous
            new Prefix("Hard", 1), // Hard
            new Prefix("Guarding", 1), // Guarding
            new Prefix("Armored", 1), // Armored                65
            new Prefix("Warding", 1), // Warding
            new Prefix("Arcane", 1), // Arcane
            new Prefix("Precise", 1), // Precise
            new Prefix("Lucky", 1), // Lucky
            new Prefix("Jagged", 1), // Jagged                  70
            new Prefix("Spiked", 1), // Spiked
            new Prefix("Angry", 1), // Angry
            new Prefix("Menacing", 1), // Menacing
            new Prefix("Brisk", 1), // Brisk
            new Prefix("Fleeting", 1), // Fleeting              75
            new Prefix("Hasty", 1), // Hasty
            new Prefix("Quick", 1), // Quick
            new Prefix("Wild", 1), // Wild
            new Prefix("Rash", 1), // Rash
            new Prefix("Intrepid", 1), // Intrepid              80
            new Prefix("Violent", 1), // Violent
            new Prefix("Legendary", 2), // Legendary
            new Prefix("Unreal", 4), // Unreal
            new Prefix("Mythical", 3), // Mythical
            #endregion
        };

        #region Overrides

        public override string Name
        { get { return Assembly.GetExecutingAssembly().GetName().Name; } }

        public override Version Version
        { get { return Assembly.GetExecutingAssembly().GetName().Version; } }

        public override string Author
        { get { return "Snirk Immington"; } }

        public override string Description
        { get { return "Punishes people with illegal prefixes..."; } }

        public PluginMain ( Main game ) : base(game) { Order = 1; }
        
        #endregion

        #region Initialize

        public override void Initialize ( )
        {
            NetHooks.GreetPlayer += OnGreet;
            GameHooks.Update += OnUpdate;
            GameHooks.Initialize += OnInit;
            GameHooks.PostInitialize += OnPost;
        }

        protected override void Dispose ( bool disposing )
        {
            if (disposing)
            {
                NetHooks.GreetPlayer -= OnGreet;
                GameHooks.Update -= OnUpdate;
                GameHooks.Initialize -= OnInit;
                GameHooks.PostInitialize -= OnPost;
            }
            base.Dispose(disposing);
        }

        private void OnInit ( )
        {
            Commands.ChatCommands.Add(new Command(Permissions.maintenance, reload, "reloadpp"));
        }

        private void OnPost ( )
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    Config = ConfigFile.Read(ConfigPath);
                }
                Config.Write(ConfigPath);
                if (!Config.DoVanillaStyleAcutalWorkingRedigitIsLazyPrefixChecksBecauseThisWholeDatabaseThingJustIsntworkingOut) switch (Config.PunishType.ToLower())
                {
                    case "kick": case "disable": case "ban": case "kill":
                    Console.WriteLine("Set up PrefixPunisher config."); break;

                    default: Log.Error("Incorrectly set up PrefixPunisher config! Fix value \"PunishType\" - resorting to default, \"kick\"");
                    Console.WriteLine("Incorrectly set up PrefixPunisher config! Fix value \"PunishType\" - resorting to default, \"kick\"");
                    Config.PunishType = "kick";
                    break;
                }
                //Console.WriteLine("Set up PluginPunisher config.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in PluginPunisher config file, writing logs.");
                Log.Error(ex.ToString());
            }

            try
            { setupDB(); }

            catch (Exception ex)
            { 
                Console.WriteLine("Error in PluginPunisher Okay Items database, ignoring it and writing logs.");
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region Hooks

        private void OnGreet ( int who, HandledEventArgs args )
        {
            try
            {
                if (TShock.Players[who].Group.HasPermission("canuseillegalprefix")) return;

                var tsply = TShock.Players[who];
                var inv = tsply.TPlayer.inventory.ToList();
                inv.AddRange(tsply.TPlayer.armor);

                foreach (var item in inv)
                {
                    string chek = "";
                    //var chek = isIllegal(item);
                    if (Config.DoVanillaStyleAcutalWorkingRedigitIsLazyPrefixChecksBecauseThisWholeDatabaseThingJustIsntworkingOut)
                    {
                        if (OldCheck(item)) chek = "";
                        else chek = "illegal prefix on " +  item.AffixName();
                    }
                    if (chek != "")
                    {
                        var message = "illegal" + chek + " - " + item.AffixName();

                        switch (Config.PunishType.ToLower())
                        {
                            case "disable":
                                {
                                    tsply.LastThreat = DateTime.UtcNow;
                                    tsply.SetBuff(33, 530, true);
                                    tsply.SetBuff(32, 530, true);
                                    tsply.SetBuff(23, 330, true);
                                    tsply.SendWarningMessage("This server does not allow " + message + ". Dispose of it at once!");
                                    return;
                                }

                            case "kick":
                                {
                                    TShock.Utils.ForceKick(tsply, message, true); return;
                                }

                            case "ban":
                                {
                                    // We need all plugins that interface with SQL to sanitize.
                                    string sqlName = message.Replace('\'', '`');

                                    // Using the depreciated code because the current ban system doesn't kick people
                                    //TShock.Bans.AddBan(tsply.IP, tsply.Name, "Illegally prefixed " + sqlName);

                                    TShock.Utils.Ban(tsply, sqlName);
                                    try { TShock.Utils.ForceKick(tsply, "Banned: " + message, true); }
                                    catch (Exception) { }
                                    return;
                                }
                            default: return;
                        }
                    }
                }
            }
            catch (Exception ex) { Log.Error(ex.ToString()); } // Errors were reported with config switchup.
        }

        private void OnUpdate ( )
        {
            if (( DateTime.UtcNow - LastCheck ).TotalSeconds >= 2)
            {
                LastCheck = DateTime.Now;
                try
                {
                    foreach (var ply in TShock.Players)
                    {
                        if (!ply.RealPlayer) continue;

                        if (ply.Group.HasPermission("canuseillegalprefix")) continue;

                        var inv = ply.TPlayer.inventory.ToList();
                        inv.AddRange(ply.TPlayer.armor);

                        foreach (var item in inv)
                        {
                            if (item == null || item.type == 0) continue;

                            #region if it is illegal
                            var chek = isIllegal(item);
                            if (chek != "")
                            {
                                var message = "illegal" + chek + " - " + item.AffixName();

                                switch (Config.PunishType.ToLower())
                                {
                                    case "disable":
                                    {
                                        ply.LastThreat = DateTime.UtcNow;
                                        ply.SetBuff(33, 230, true);
                                        ply.SetBuff(32, 230, true);
                                        ply.SetBuff(23, 230, true);
                                        ply.SendWarningMessage("This server does not allow " + message+" - remove it at once!");
                                        break;
                                    }

                                    case "kick":
                                    {
                                        TShock.Utils.ForceKick(ply, message, true);
                                        break;
                                    }

                                    case "kill":
                                    {
                                        if (!ply.Dead) NetMessage.SendData((int)PacketTypes.PlayerDamage, -1, -1,
                                            " was killed for  "  + message + '.', ply.Index, 0, 9999);

                                        ply.SendWarningMessage("This server doesnot allow " + message + ". Dispose of it at once!");
                                        break;
                                    }

                                    case "ban":
                                    {
                                        string sqlName = message.Replace('\'', '`');
                                        TShock.Utils.Ban(ply, sqlName);
                                        try { TShock.Utils.ForceKick(ply, "Banned: " + message, true); }
                                        catch (Exception) { }
                                        break;
                                    }
                                    default: break;
                                }
                                break; // break the foreach item
                            }
                            #endregion
                        }
                    }
                } // try
                catch (Exception) { }
            }
            
        }
        
        #endregion

        private static void reload ( CommandArgs com )
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    Config = ConfigFile.Read(ConfigPath);
                }
                Config.Write(ConfigPath);
                switch (Config.PunishType.ToLower())
                {
                    case "kick":
                    case "disable":
                    case "ban":
                    case "kill":
                    com.Player.SendInfoMessage("Reloaded config file successfully."); break;

                    default: Log.Error("Incorrectly set up PrefixPunisher config! Fix value \"PunishType\" - resorting to default, \"kick\"");
                    com.Player.SendErrorMessage("Invalid \"PunishType\" - restoring to default, \"kick\"");
                    Config.PunishType = "kick";
                    break;
                }
            }
            catch (Exception ex)
            {
                com.Player.SendErrorMessage("Error in PluginPunisher config file, writing in the logs.");
                Log.Error(com.Player.Name + " used /reloadpp, there was an error: " + ex.ToString());
            }

            try { setupDB(); com.Player.SendSuccessMessage("Reloaded Okay Prefixes Database correctly! Check the console to see what was set up."); }
            catch (Exception ex)
            {
                com.Player.SendErrorMessage("Error in PluginPunisher Okay Prefixes database, writing in the logs.");
                Log.Error(com.Player.Name + " used /reloadpp, there was an error: " + ex.ToString());
            }
        }

        #region utils

        private static string isIllegal ( Item it )
        {
            if (it.prefix == 0) return "";

            // I want to marry a lambda expression.
            if (OkayStuff.Any(o => o.Prefix == it.prefix && o.ItemType == it.type)) return "";

            var prefix = Prefixes[it.prefix]; 

            if (prefix.isBad && Config.AllowAllNegativeModifiers) return "";

            #region handle non weapons
            if (it.damage == -1)
            {
                if (it.accessory)
                {
                    if (prefix.ItemType != 1)
                    {
                        if (!Config.AllowItemsWithPrefixesOfOtherTypes) return " prefix type";
                        
                        else return "";
                    }
                    else return "";
                }
                else if (it.defense != 0) // it must be armor
                {
                    if (!Config.AllowPrefixedArmor) return "ly prefixed armor";

                    else return "";
                }
                else
                {
                    if (!Config.AllowPrefixedHarmless) return "ly prefixed harmless item";

                    else return "";
                }
            }
            #endregion

            #region handle weapons
            else // weapon or ammo or tool
            {
                if (it.maxStack != 1)
                {
                    if (it.ammo != 0)
                    {
                        if (!Config.AllowPrefixedAmmo) return "ly prefixed ammo";

                        else return "";
                    }
                    else
                    {
                        if (!Config.AllowPrefixedStackedWeapons) return "ly prefixed stacked weapon";

                        else return "";
                    }
                }
                if (it.melee)
                {
                    if (it.useStyle == 5) // it is a spear, flail, drill, chainsaw, or hamaxe
                    {
                        if (prefix.ItemType != 0)
                        {
                            if (!Config.AllowItemsWithPrefixesOfOtherTypes) return " prefix type";

                            else return "";
                        }
                        else return "";
                    }
                    else // it must be a sword type, 1 or 3, both are ok
                    {
                        if (prefix.ItemType == 0 || prefix.ItemType == 2 || prefix.ItemType == 5)
                        {
                            return "";
                        }
                        else
                        {
                            if (!Config.AllowItemsWithPrefixesOfOtherTypes) return " prefix type";

                            else return "";
                        }
                    }
                }
                else if (it.magic)
                {
                    if (prefix.ItemType != 0 && prefix.ItemType != 5 && prefix.ItemType != 3)
                    {
                        if (!Config.AllowItemsWithPrefixesOfOtherTypes) return " prefix type";

                        else return "";
                    }
                    else return "";
                }
                else if (it.ranged)
                {
                    if (prefix.ItemType != 4 && prefix.ItemType != 5 && prefix.ItemType != 0)
                    {
                        if (!Config.AllowItemsWithPrefixesOfOtherTypes) return " prefix type";

                        else return "";
                    }
                    else return "";
                }
                else return ""; // never happens
            }
            #endregion
        }

        private static void readDB ( )
        {
            foreach (var line in File.ReadAllLines(DataPath).Where(l => l[0] != '#'))
            {
                var split = line.Split(':');
                if (split.Length != 2)
                {
                    Console.WriteLine("There was an error parsing \""+line+"\", skipping that one.");
                    continue;
                }

                var item = TShock.Utils.GetItemByIdOrName(split[0]);
                if (item.Count != 1)
                {
                    Console.WriteLine("Unable to parse the item name in \""+line+"\", skipping that one.");
                    continue;
                }
                byte prefix = 0;
                if (!byte.TryParse(split[1], out prefix))
                {
                    Console.WriteLine("Unable to parse the prefix in \""+line+"\" (it must be a number), skipping that one.");
                    continue;
                }

                if (prefix < 1 || prefix > 84)
                {
                    Console.WriteLine("Prefix number is invalid in \""+line+"\" (must be between 1 and 84), skipping that one.");
                    continue;
                }
                Console.WriteLine("Added combination: Item = {0}, Prefix = {1} ({2})".SFormat(item[0].name, prefix, Prefixes[prefix].Name));
                OkayStuff.Add(new OkayCombo(item[0].type, prefix));
            }
            Console.WriteLine("If there are errors in the item combinations, please tell Snirk.");
        }

        private static void writeDB ( )
        {
            File.WriteAllLines(DataPath, new string[] {
                "# This config file is where you can add specific item/prefix combinations that are allowed by the plugin.",
                "# The format is: ItemName:PrefixNumber and write one per line!",
                "# Example: Neptune's Shell:1",
                "# This will allow Large Neptune Shells on the server even if they are normally banned.",
                "# Also, feel free to add notes, as long as they begin with \"#\"! Make sure not to remove the \"#\" from these lines."});

        }

        private static void setupDB ( )
        {
            OkayStuff.Clear();
            if (File.Exists(DataPath))
            {
                readDB();
            }
            else writeDB();
        }

        private static bool OldCheck(Item it)
        {
            var prefix = it.prefix;

            it.Prefix(prefix);
            if (it.prefix == prefix) return true;

            else return false;
        }

        #endregion
    }

    class ConfigFile
    {
        public string PunishType = "kick";
        public bool DoVanillaStyleAcutalWorkingRedigitIsLazyPrefixChecksBecauseThisWholeDatabaseThingJustIsntworkingOut = true;
        public bool AllowPrefixedArmor = false;
        public bool AllowPrefixedAmmo = false;
        public bool AllowPrefixedHarmless = true;
        public bool AllowPrefixedStackedWeapons = false;
        public bool AllowItemsWithPrefixesOfOtherTypes = false;
        public bool AllowAllNegativeModifiers = false;

        #region serialization

        internal static ConfigFile Read ( string path )
        {
            if (!File.Exists(path))
                return new ConfigFile();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        internal static ConfigFile Read ( Stream stream )
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<ConfigFile>(sr.ReadToEnd());
                if (ConfigRead != null)
                    ConfigRead(cf);
                return cf;
            }
        }

        internal void Write ( string path )
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        internal void Write ( Stream stream )
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }

        internal static Action<ConfigFile> ConfigRead;

        #endregion
    }

    /// <summary>
    /// Stores information on prefixes.
    /// </summary>
    public class Prefix
	{
		/// <summary>
        /// 0 = Universal/tool. 1 = Accessory. 2 = Melee. 3 = Magic. 4 = Weapon. 5 = Common/weapon.
        /// </summary>
        public byte ItemType { get; set; }
        /// <summary>
        /// Should it be blocked by Config.
        /// </summary>
        public bool isBad { get; set; }

        /// <summary>
        /// Returns the name of the prefix. Basically for debug purposes.
        /// </summary>
        public string Name { get; set; }

        public Prefix (string name, byte itemtype, bool isbad = false)
        { ItemType = itemtype; isBad = isbad; Name = name; }
	}

    /// <summary>
    /// Because dictionaries are secretly really bitchy.
    /// </summary>
    struct OkayCombo
    {
        public int ItemType;
        public byte Prefix;

        public OkayCombo ( int type, byte prefix )
        {
            ItemType = type;
            Prefix = prefix;
        }
    }
}
