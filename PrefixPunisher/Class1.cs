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
        private static ConfigFile Config { get; set; } private static DateTime LastCheck = DateTime.Now;

        private static string ConfigPath { get { return Path.Combine(TShock.SavePath, "PrefixPunisher Config.json"); } }

        // I have made this and class Prefix public so anyone referecing my code can make use of so many hours of typing.
        public Prefix[] Prefixes = new Prefix[] 
        {
            #region Prefixes!
            new Prefix(2), // Large                 
            new Prefix(2), // Massive
            new Prefix(2), // Dangerous
            new Prefix(2), // Savage
            new Prefix(2), // Sharp
            new Prefix(2), // Pointy                
            new Prefix(2, true), // Tiny
            new Prefix(2, true), // Terrible
            new Prefix(2, true), // Small
            new Prefix(2, true), // Dull
            new Prefix(2, true), // Unhappy        
            new Prefix(2), // Bulky
            new Prefix(2, true), // Shameful
            new Prefix(2), // Heavy
            new Prefix(2), // Light                 
            new Prefix(4), // Sighted              
            new Prefix(4), // Rapid
            new Prefix(4), // Hasty
            new Prefix(4), // Indimidating
            new Prefix(4), // Deadly                
            new Prefix(4), // Staunch               
            new Prefix(4, true), // Awful
            new Prefix(4, true), // Lethargic
            new Prefix(4, true), // Awkward
            new Prefix(4), // Powerful               
            new Prefix(4), // Frenzying            
            new Prefix(3), // Mystic
            new Prefix(3), // Adept
            new Prefix(3), // Masterful
            new Prefix(3, true), // Inept            
            new Prefix(3, true), // Ignorant       
            new Prefix(3, true), // Deranged
            new Prefix(3), // Intense
            new Prefix(3), // Taboo
            new Prefix(3), // Celestial
            new Prefix(3), // Furious
            new Prefix(3), // Manic
            new Prefix(0), // Keen
            new Prefix(0), // Superior
            new Prefix(0), // Forceful
            new Prefix(0), // Hurtful
            new Prefix(0), // Strong
            new Prefix(0), // Unpleasant
            new Prefix(0, true), // Broken
            new Prefix(0, true), // Damaged
            new Prefix(0, true), // Weak
            new Prefix(0, true), // Shoddy
            new Prefix(0), // Ruthless
            new Prefix(5), // Quick
            new Prefix(5), // Deadly
            new Prefix(5), // Agile
            new Prefix(5), // Nimble
            new Prefix(5), // Murderous
            new Prefix(5, true), // Slow
            new Prefix(5, true), // $luggish
            new Prefix(5, true), // Lazy
            new Prefix(5, true), // Annoying
            new Prefix(5), // Nasty
            new Prefix(0), // Godly
            new Prefix(0), // Demonic
            new Prefix(0), // Zealous
            new Prefix(1), // Hard
            new Prefix(1), // Guarding
            new Prefix(1), // Armored
            new Prefix(1), // Warding
            new Prefix(1), // Arcane
            new Prefix(1), // Precise
            new Prefix(1), // Lucky
            new Prefix(1), // Jagged
            new Prefix(1), // Spiked
            new Prefix(1), // Angry
            new Prefix(1), // Menacing
            new Prefix(1), // Brisk
            new Prefix(1), // Fleeting
            new Prefix(1), // Hasty
            new Prefix(1), // Quick
            new Prefix(1), // Wild
            new Prefix(1), // Rash
            new Prefix(1), // Intrepid
            new Prefix(1), // Violent
            new Prefix(2), // Legendary
            new Prefix(4), // Unreal
            new Prefix(3), // Mythical
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
            GameHooks.PostInitialize += OnPost;
        }

        protected override void Dispose ( bool disposing )
        {
            if (disposing)
            {
                NetHooks.GreetPlayer -= OnGreet;
                GameHooks.Update -= OnUpdate;
                GameHooks.PostInitialize -= OnPost;
            }
            base.Dispose(disposing);
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
                switch (Config.PunishType.ToLower())
                {
                    case "kick": case "disable": case "ban":
                    Console.WriteLine("Set up PrefixPunisher config."); break;

                    default: Log.Error("Incorrectly set up PrefixPunisher config! Fix value \"PunishType\"!");
                    Console.WriteLine("Incorrectly set up PrefixPunisher config! Fix value \"PunishType\"!");
                    break;
                }
                Console.WriteLine("Set up PluginPunisher config.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in PluginPunisher config file, writing logs.");
                Log.Error(ex.ToString());
            }
        }

        #endregion

        #region Hooks

        private void OnGreet ( int who, HandledEventArgs args )
        {
            if (TShock.Players[who].Group.HasPermission("canuseillegalprefix")) return;

            var tsply = TShock.Players[who];
            var inv = tsply.TPlayer.inventory.ToList();
            inv.AddRange(tsply.TPlayer.armor);

            foreach (var item in inv)
            {
                if (isIllegal(item))
                {
                    var name = tsply.Name;

                    switch (Config.PunishType.ToLower())
                    {
                        case "disable":
                        {
                            tsply.Disable("Illegally prefixed " + item.AffixName() + "!"); 
                            return;
                        }

                        case "kick":
                        {
                            TShock.Utils.ForceKick(tsply, "Illegally prefixed " + item.AffixName(), Config.AnnouncePunishes);
                            return;
                        }

                        case "ban":
                        {
                            // We need all plugins that interface with SQL to sanitize.
                            string sqlName = item.AffixName().Replace('\'', '`');

                            // Using the depreciated code because the current ban system doesn't kick people
                            TShock.Bans.AddBan(tsply.IP, tsply.Name, "Illegally prefixed " + sqlName);
                            TShock.Utils.ForceKick(tsply, "Banned: Illegally prefixed " + item.AffixName(), Config.AnnouncePunishes);
                            return;
                        }
                        default: return;
                    }
                }
            }
        }

        private void OnUpdate ( )
        {
            while (true)
            {
                if (( DateTime.Now - LastCheck ).TotalSeconds >= 5)
                {
                    LastCheck = DateTime.Now;
                    foreach (var ply in TShock.Players.Where(p => p.RealPlayer))
                    {
                        if (ply.Group.HasPermission("canuseillegalprefix")) continue;

                        var inv = ply.TPlayer.inventory.ToList();
                        inv.AddRange(ply.TPlayer.armor);

                        foreach (var item in inv)
                        {
                            if (isIllegal(item))
                            {
                                //var name = ply.Name;

                                switch (Config.PunishType.ToLower())
                                {
                                    case "disable":
                                    {
                                        ply.Disable("Illegally prefixed " + item.AffixName() + "!");
                                        return;
                                    }

                                    case "kick":
                                    {
                                        TShock.Utils.ForceKick(ply, "Illegally prefixed " + item.AffixName(), Config.AnnouncePunishes);
                                        return;
                                    }

                                    case "ban":
                                    {
                                        string sqlName = item.AffixName().Replace('\'', '`');
                                        TShock.Bans.AddBan(ply.IP, ply.Name, "Illegally prefixed " + sqlName);
                                        TShock.Utils.ForceKick(ply, "Banned: Illegally prefixed " + item.AffixName(), Config.AnnouncePunishes);
                                        return;
                                    }
                                    default: return;
                                }
                            }
                        }
                    }
                }
            }
        }
        
        #endregion

        #region utils

        private bool isIllegal ( Item it )
        {
            if (it.prefix == 0) return false;

            var prefix = Prefixes[it.prefix - 1];

            if (prefix.isBad && Config.AllowAllNegativeModifiers) return false;

            #region handle non weapons
            if (it.damage == -1)
            {
                if (it.accessory)
                {
                    if (prefix.ItemType != 1)
                    {
                        if (!Config.AllowItemsWithPrefixesOfOtherTypes) return true;
                        
                        else return false;
                    }
                    else return false;
                }
                else if (it.defense != 0) // it must be armor
                {
                    if (!Config.AllowPrefixedArmor) return true;

                    else return false;
                }
                else
                {
                    if (!Config.AllowPrefixedHarmless) return true;

                    else return false;
                }
            }
            #endregion

            #region handle weapons
            else // weapon
            {
                if (it.maxStack != 1)
                {
                    if (!Config.AllowPrefixedStackedWeapons) return true;

                    else return false;
                }
                if (it.melee)
                {
                    if (it.useStyle == 5) // it is a spear, flail, drill, chainsaw, or hamaxe
                    {
                        if (prefix.ItemType != 0)
                        {
                            if (!Config.AllowItemsWithPrefixesOfOtherTypes) return true;

                            else return false;
                        }
                        else return false;
                    }
                    else // it must be a sword type, 1 or 3, both are ok
                    {
                        if (prefix.ItemType == 0 || prefix.ItemType == 2 || prefix.ItemType == 5)
                        {
                            return false;
                        }
                        else
                        {
                            if (!Config.AllowItemsWithPrefixesOfOtherTypes) return true;

                            else return false;
                        }
                    }
                }
                else if (it.magic)
                {
                    if (prefix.ItemType != 0 && prefix.ItemType != 5 && prefix.ItemType != 3)
                    {
                        if (!Config.AllowItemsWithPrefixesOfOtherTypes) return true;

                        else return false;
                    }
                    else return false;
                }
                else if (it.ranged)
                {
                    if (prefix.ItemType != 4 && prefix.ItemType != 5 && prefix.ItemType != 0)
                    {
                        if (!Config.AllowItemsWithPrefixesOfOtherTypes) return true;

                        else return false;
                    }
                    else return false;
                }
                else return false; // never happens
            }
            #endregion
        }

        #endregion
    }

    class ConfigFile
    {
        public string PunishType = "kick";
        public bool AnnouncePunishes = true;
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

        public Prefix ( byte itemtype, bool isbad = false)
        { ItemType = itemtype; isBad = isbad; }
	}
}
