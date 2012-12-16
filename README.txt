Prefix Punisher

Description:

Used to prevent people with illegal prefixes (prefixed ammo or armor, items with prefixes they couldn't naturally have, prefixed blocks, etc) from doing things on your server. 

The config file ("PrefixPunsiher Condfig.json") values:

> PunishType: Must be either "kick", "disable", "kill", or "ban". Self-explanitory.
> AllowPrefixedArmor: Whether the plugin checks armor for prefixes. Although prefixed armor is usually a blatant hack, I have heard that there is a bug where it can be prefixed on creation.
> AllowPrefixedAmmo: Bullets, Arrows, Stars, Sand Blocks, etc. with prefixes (unreal bullets, for example, are overpowered).
> AllowPrefixedHarmless: Things like "Massive Cursed Torch" (an item I am fond of) "Spiked Toilet," etc. do not affect a player's power. Note that prefixed Sand Blocks or Stars count as ammo.
> AllowPrefixedStackedWeapons: Light discs, Shurikens, etc. fall into this category. If they are prefixed, they are hacked.
> AllowItemsWithPrefixesOfOtherTypes: Accessories with weapon prefixes, vice versa, etc. These are not naturally found, but also not as bad as warding armor.
> AllowAllNegativeModifiers: Each prefix can be good or bad, and this allows bad ones through. Bad is defined as having no positive traits. This is checked first, so it overrides all the other settings.

The defaults are PunishType = "kick" and everything else is false (highest level of security).
The more values that are set to true, the more hacked items you let in.

Commands: 

No commands (yet).

Permissions:

Any player with "canuseillegalprefix"... can use illegal prefixes.

Version list:

1.0 -> initial release. 