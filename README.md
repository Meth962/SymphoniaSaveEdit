# SymphoniaSaveEdit
A save viewer and editor for Tales of Symphonia for PC, GC, PS3, Switch and unencrypted PS4 saves.

This is a very old project that I built for myself. Enough people were asking about it though so I was semi-supporting it. I could not keep up with the myriad of requests such as "Make Yuan a playable character" so I lost interest. However, it should support most things you would need to do to your save file.

Anything that is displayed by this program can also be changed. Some things are not in a field that is changeable but can be added at some point as requested. I simply wasn't changing those values and therefore left them as display only.

## What You Can Edit
Generally try to edit in the tabs as it is a better experience.
* Character Techs - in the **Party** tab.
* Character Titles - in the **Party** tab.
* Treasures found for title - in the **Treasures** tab, double-click on an item to "find" it. This marks it found, but does not give you the item.
* Women spoken to by Zelos - in the **Gigolo** tab. The game actually tracks which **cities** you schmoozed all the women in. Double-click to clear a city.
* Dogs for Dog Lover - in the **Dog Lover** tab. Double-click on the location to clear the dog named at that location.
* Items / Inventory - in the **Items** tab, click on an item, then use the slider at the bottom to adjust the quantity.
* Cheats - in the **Cheats** tab
	* Max Gald - set your gald to 999,999.
 	* Max Stats - max your characters' stats to Gamecube maximums. 1999 for Attack, Defense, Strength, etc. 9999 HP, 999 TP, Level 250, Exp 999,999, full overlimit bar, and clear status.
  	* Max Grade - max your grade points to 9999.0
  	* Max Items - sets all items to 20 quantity, except for certain key items that stay at 1.
  	* Max Techs - this sets the usage count for every tech to 999.
  	* All Titles - this enables all titles on all characters. It tries to ignore the blank/bugged ones.
  	* All Techs - unlocks all techs on all characters. This tries to ignore blank/bad ones.
  	* Max Cooking - sets all recipes to max value of 8. This differs by recipe and console version.

Many other parts of the save can be changed in the last tab **Manual Edit** but that is a poor experience. *Only use that if you need to*, as it does have everything I have found in the save so far.

## Using Manual Edit
Any number you see preceeded by "0x" is a hex number. Be sure to leave the "0x" when editing, and 
remember to convert your number to hex.

## Thank Yous
**Noxbur** - not only helped me get the PS4 version working, but you can thank him for this working for all game versions again. I abandoned this project long ago, but when I picked it up again, he was extremely helpful working with me. Testing saves, exporting, importing, encrypting, decrypting, every time I needed to check something on the Switch or PS4 version that I did not own. He also made spreadsheets for us to write down all the addresses for each version of the game, which was a huge help clearing up all the problems between each version.

## History
**v0.9.2** (4/1/2024): Kratos fixed
* Kratos techs fixed up.
* Added ability to double click on missing dog, treasure, or gigolo town to complete it.
* added safety checks before save is loaded
  
**v0.9.1** (3/31/2024): Item fixup
* Fixed items according to PS4 save. Seems the itemIDs mostly line up on all versions.
* Changed default extension on opening a file.

**v0.9.1b** (3/30/2024): Tech fixup
* Rewrote all techs to match PS4. Kratos was a complete guess as I don't have a save with him in it. A couple techs could still be off by one.
* Added Thank You

**v0.9.0** (3/30/2024): PS4 + Rewrite
* Thanks to Noxbur, we found how the PS4 version calculates checksum. This works with raw (decrypted) PS4 saves.
* Have to confirm on other platforms but this fix is focusing on PS4/Switch raw saves.  
* Code needs a rewrite. Started the process of splitting console specific code into other classes and removing methods from the monolithic main class.
* Finally solved the small differences that brought confusion between all versions
  | Platform | Endianness | Offset | Checksum |
  | -------- | ---------- | ------ | -------- |
  | GC | big | 0x1bd | 0x0008-0x29d4 |
  | PS3 | big | 0x4a0 | 0x0004-0x2614 |
  | PC | little | 0x548 | none |
  | Switch | little | 0x4a8 | none |
  | PS4 | little | 0x4a8 | 0x0004-0x25f8 |
* Titles corrected after they were reversed around from PS3 version being backwards (big-endian)
* Bugged titles are now listed as such and unclickable

9/24/2019
* Added support for unencrypted PS3 files since some emulators do not follow the PS3's file format correctly.
  
6/20/2018
* Fixed the offset for PC saves and Affection
	
2/18/2018
* Still haven't tested this at all for PS3 or PC saves (Backup your saves). Please report any issues.
