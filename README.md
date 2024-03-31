# SymphoniaSaveEdit
A save viewer and editor for Tales of Symphonia for PC, GC, PS3, Switch and unencrypted PS4 saves.

This is a very old project that I built for myself. Enough people were asking about it though so I was semi-supporting it. I could not keep up with the myriad of requests such as "Make Yuan a playable character" so I lost interest. However, it should support most things you would need to do to your save file.

Anything that is displayed by this program can also be changed. Some things are not in a field that is changeable but can be added at some point as requested. I simply wasn't changing those values and therefore left them as display only.

## History
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

## Using Manual Edit
Any number you see preceeded by "0x" is a hex number. Be sure to leave the "0x" when editing, and 
remember to convert your number to hex.
