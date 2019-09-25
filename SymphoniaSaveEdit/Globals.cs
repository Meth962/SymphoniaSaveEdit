﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace SymphoniaSaveEdit
{
    static class Globals
    {
        public static SolidColorBrush WhiteBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));

        public static List<string> TreasureNames = new List<string>()
        {
          "Iselia Forest: Apple Gel #1",
          "Iselia Forest: Life Bottle",
          "Iselia Forest: Apple Gel #2",
          "Iselia Forest: Orange Gel #2",
          "Iselia Forest: Apple Gel #3",
          "Temple of Martel: Apple Gel",
          "Temple of Martel: Life Bottle #2",
          "Temple of Martel: 250 Gald",
          "Temple of Martel: Life Bottle #1",
          "Temple of Martel: Panacea Bottle",
          "Iselia Forest: Leather Glove",
          "Triet Ruins: Apple Gel",
          "Triet Ruins: Lemon Gel",
          "Triet Ruins: Life Bottle",
          "Triet Ruins: Bracelet",
          "Triet Ruins: Savory",
          "Triet Ruins: Circlet",
          "Triet Ruins: Stiletto",
          "Triet Ruins: Mumei",
          "Triet Ruins: 1000 Gald",
          "Ossa Trail: Battle Staff",
          "Ossa Trail: Melange Gel",
          "Ossa Trail: Apple Gel",
          "Ossa Trail: Orange Gel",
          "Ossa Trail: Fine Guard",
          "Ossa Trail: Black Silver",
          "Ossa Trail: Beast Fang",
          "Ossa Trail: Ex Gem Lv1",
          "Thoda Geyser: Mermaid Tear",
          "Thoda Geyser: Circlet #1",
          "Thoda Geyser: Life Bottle",
          "Thoda Geyser: Stun Bracelet",
          "Thoda Geyser: Circlet #2",
          "Thoda Geyser: White Silver",
          "Thoda Geyser: Orange Gel",
          "Thoda Geyser: Ex Gem Lv1",
          "Sylverant Base: Beast Hide #1",
          "Sylverant Base: Magical Cloth",
          "Sylverant Base: Beast Hide #2",
          "Sylverant Base: 1500 Gald",
          "Balacruf Mausoleum: 1800 Gald",
          "Balacruf Mausoleum: Beast Fang",
          "Balacruf Mausoleum: Beast Hide",
          "Balacruf Mausoleum: Iron Guard",
          "Balacruf Mausoleum: Blue Ribbon #1",
          "Balacruf Mausoleum: Blue Ribbon #2",
          "Balacruf Mausoleum: Ex Gem Lv2",
          "Palmacosta Ranch: White Silver",
          "Palmacosta Ranch: Omega Shield",
          "Palmacosta Ranch: Mage Cloak",
          "Palmacosta Ranch: Red Card",
          "Palmacosta Ranch: Orange Gel #3",
          "Palmacosta Ranch: Ex Gem Lv2",
          "Asgard Human Ranch: Beast Hide",
          "Asgard Human Ranch: White Robe",
          "Asgard Human Ranch: Iron Bracelet",
          "Asgard Human Ranch: Card of Earth",
          "Asgard Human Ranch: Stun Charm",
          "Asgard Human Ranch: Lamellar Leather",
          "Asgard Human Ranch: Cleric's Hat",
          "Asgard Human Ranch: Pellets",
          "Asgard Human Ranch: Ex Gem Lv2",
          "Tower of Mana: Iron Mail",
          "Tower of Mana: Ex Gem Lv2 #2",
          "Tower of Mana: Ex Gem Lv2 #1",
          "Tower of Mana: Lunar Guard",
          "Tower of Mana: Moon Robe",
          "Tower of Mana: Arnet Helm",
          "Tower of Mana: Stinger Ring",
          "Sylvarant Base 2: Ex Gem Lv2 #3 ",
          "Sylvarant Base 2: Straw Hat",
          "Sylvarant Base 2: Protect Ring",
          "Fooji Mountains: Card of Fire",
          "Fooji Mountains: Misty Robe",
          "Fooji Mountains: Black Onyx",
          "Fooji Mountains: Cool Orbit",
          "Fooji Mountains: Ex Gem Lv2",
          "Meltokio Sewers: Ex Gem Lv1",
          "Meltokio Sewers: 2500 Gald",
          "Meltokio Sewers: Ex Gem Lv2",
          "Meltokio Sewers: Ex Gem Lv3",
          "Meltokio Sewers: Thunderbolt",
          "Meltokio Sewers: Card of Lightning",
          "Meltokio Sewers: Great Ax",
          "Meltokio Sewers: Breastplate",
          "Meltokio Sewers: Spirit Ring",
          "Gaoracchia Forest: Phoenix Rod",
          "Gaoracchia Forest: Witch's Robe",
          "Gaoracchia Forest: Pretty Ribbon",
          "Gaoracchia Forest: Angel Bracelet",
          "Gaoracchia Forest: Drain Charm",
          "Toize Valley Mine: Saint Rapier",
          "Toize Valley Mine: Sand Saber",
          "Toize Valley Mine: Crescent Ax",
          "Toize Valley Mine: Iron Greaves",
          "Toize Valley Mine: Silk Robe",
          "Toize Valley Mine: Thunder Cape",
          "Toize Valley Mine: Super Pellets",
          "Toize Valley Mine: Ex Gem Lv3",
          "Toize Valley Mine: Sage",
          "Toize Valley Mine: Ex Gem Lv1",
          "Temple of Earth: Bellabane",
          "Temple of Earth: Ex Gem Lv1",
          "Temple of Earth: Ex Gem Lv2",
          "Temple of Earth: Ex Gem Lv3",
          "Temple of Earth: Ancient Rod",
          "Temple of Earth: Bardiche",
          "Temple of Earth: Ghost Shell",
          "Temple of Earth: Mythril Guard",
          "Temple of Earth: Mythril Bracelet",
          "Temple of Earth: Mythril Circlet",
          "Temple of Ice: Defenser",
          "Temple of Ice: Ancient Robe",
          "Temple of Ice: Mythril Shield",
          "Temple of Ice: Ice Coffin",
          "Temple of Ice: Mythril Armor",
          "Temple of Ice: Mythril Gauntlet",
          "Temple of Ice: Rosemary",
          "Temple of Ice: Ex Gem Lv2",
          "Temple of Ice: Ex Gem Lv3",
          "Remote Island Ranch: Saffron",
          "Remote Island Ranch: Ex Gem Lv1",
          "Remote Island Ranch: Ex Gem Lv2 #2",
          "Remote Island Ranch: Ex Gem Lv2 #1",
          "Remote Island Ranch: Ex Gem Lv3 #1",
          "Remote Island Ranch: Ex Gem Lv3 #2",
          "Remote Island Ranch: Ex Gem Lv4",
          "Remote Island Ranch: Mythril Ax",
          "Remote Island Ranch: Mythril Greaves",
          "Remote Island Ranch: Holy Cloak",
          "Remote Island Ranch: Minazuki",
          "Remote Island Ranch: Revive Ring",
          "Remote Island Ranch: Stone Charm",
          "Iselia Human Ranch: Muramasa",
          "Iselia Human Ranch: Solar Spinner",
          "Iselia Human Ranch: Rune Staff",
          "Iselia Human Ranch: Ether Sword",
          "Iselia Human Ranch: War Hammer",
          "Iselia Human Ranch: Aqua Greaves",
          "Iselia Human Ranch: Rune Mail",
          "Iselia Human Ranch: Rune Guard",
          "Iselia Human Ranch: Hairpin",
          "Iselia Human Ranch: Rune Gauntlet",
          "Iselia Human Ranch: Rune Cloak",
          "Iselia Human Ranch: Rune Circlet",
          "Iselia Human Ranch: Rune Robe",
          "Iselia Human Ranch: Rune Shield",
          "Iselia Human Ranch: Lovely Mittens",
          "Temple of Darkness: Ex Gem Lv 2 #1",
          "Temple of Darkness: Ex Gem Lv 2 #2",
          "Temple of Darkness: Ex Gem Lv 3",
          "Temple of Darkness: Ex Gem Lv 4",
          "Temple of Darkness: Shadow Dancer",
          "Temple of Darkness: Headband",
          "Ymir Forest: Resist Ring",
          "Ymir Forest: Crystal Shell",
          "Ymir Forest: Maid's Hairband",
          "Ymir Forest: Solar Guard",
          "Ymir Forest: Gladius",
          "Latheon Gorge: Ex Gem Lv4 #1",
          "Latheon Gorge: Rare Pellets",
          "Latheon Gorge: Ex Gem Lv3 #1",
          "Latheon Gorge: Ex Gem Lv4 #3",
          "Latheon Gorge: Ex Gem Lv4 #2",
          "Latheon Gorge: Toroid",
          "Latheon Gorge: Ex Gem Lv3 #2",
          "Latheon Gorge: Battle Pick",
          "Latheon Gorge: Flare Greaves",
          "Latheon Gorge: Draupnir",
          "Latheon Gorge: Star Cap",
          "Latheon Gorge: Rare Shield",
          "Welgaia: Laser Blade",
          "Welgaia: Nagazuki",
          "Welgaia: Dragon Fang",
          "Welgaia: Rare Guard",
          "Welgaia: Holy Robe",
          "Welgaia: Holy Circlet",
          "Welgaia: Energy Tablets",
          "Welgaia: Ex Gem Lv2",
          "Welgaia: Ex Gem Lv3 #1",
          "Welgaia: Ex Gem Lv3 #2",
          "Welgaia: Ex Gem Lv4",
          "Temple of Lightning: Ex Gem Lv2",
          "Temple of Lightning: Ex Gem Lv3 #2",
          "Temple of Lightning: Shining Star",
          "Temple of Lightning: Thunder Scepter",
          "Temple of Lightning: Power Greaves",
          "Temple of Lightning: Silver Guard",
          "Temple of Lightning: Battle Cloak",
          "Temple of Lightning: Duel Helm",
          "Tethe'alla Base: Lavender",
          "Tethe'alla Base: Ex Gem Lv2",
          "Tethe'alla Base: Ex Gem Lv3 #2",
          "Tethe'alla Base: Dragon Tooth",
          "Tethe'alla Base: Lightning Sword",
          "Tethe'alla Base: Card of Ice",
          "Tethe'alla Base: Tomahawk Lance",
          "Tethe'alla Base: Silver Mail",
          "Tethe'alla Base: Silver Circlet",
          "Tethe'alla Base: Beam Shield",
          "Tethe'alla Base: Aqua Cape",
          "Tower of Salvation: Diamond Shell",
          "Tower of Salvation: Ogre Ax",
          "Tower of Salvation: Hanuman's Staff",
          "Tower of Salvation: Southern Cross",
          "Tower of Salvation: Phoenix Cloak",
          "Tower of Salvation: Heavenly Robe",
          "Tower of Salvation: Star Mail",
          "Tower of Salvation: Ex Gem Lv4 #1",
          "Tower of Salvation: Energy Tablets",
          "Tower of Salvation: Star Guard",
          "Tower of Salvation: Shaman Dress",
          "Tower of Salvation: Ex Gem Lv3 #1",
          "Tower of Salvation: Star Helm",
          "Tower of Salvation: Star Shield",
          "Tower of Salvation: Star Circlet",
          "Tower of Salvation: Star Gauntlet",
          "Tower of Salvation: Ex Gem Lv3 #2",
          "Tower of Salvation: Ex Gem Lv2",
          "Tower of Salvation: Star Bracelet",
          "Tower of Salvation: Ex Gem Lv4 #2",
          "Tower of Salvation: Spirit Bottle",
          "Palmacosta Ranch: Life Bottle #2",
          "Palmacosta Ranch: Panacea Bottle",
          "Palmacosta Ranch: Apple Gel #1",
          "Palmacosta Ranch: Orange Gel #1",
          "Palmacosta Ranch: Life Bottle #1",
          "Palmacosta Ranch: Organce Gel #2",
          "Iselia Human Ranch: Rune Helm",
          "Iselia Forest: Orange Gel #1",
          "Iselia Forest: 500 Gald",
          "Palmacosta Ranch: Life Bottle #3",
          "Palmacosta Ranch: Apple Gel #2",
          "Palmacosta Ranch: Orange Gel #4",
          "Palmacosta Ranch: Melange Gel",
          "Iselia Human Ranch: Cor Leonis",
          "Remote Island Ranch: Holy Staff",
          "Remote Island Ranch: Vajra",
          "Toize Valley Mine: Battlesuit",
          "None",
          "Torent Forest: Stardust",
          "Torent Forest: Crystal Dagger",
          "Torent Forest: Acalanatha",
          "Torent Forest: Mana Protector",
          "Torent Forest: Warlock Garb",
          "Torent Forest: Shield Ring",
          "Torent Forest: Ex Gem Lv4 #2",
          "Torent Forest: Ex Gem Lv3",
          "Torent Forest: Ex Gem Lv4 #1",
          "Torent Forest: Angel's Tear",
          "Derris Kharlan: Ex Gem Lv2",
          "Derris Kharlan: Ex Gem Lv3 #1",
          "Vinheim: Ninja Sword",
          "Derris Kharlan: Ex Gem Lv4 #2",
          "Derris Kharlan: Ex Gem Lv3 #2",
          "Derris Kharlan: Golden Helm",
          "Derris Kharlan: Ex Gem Lv4 #1",
          "Derris Kharlan: Magical Ribbon",
          "Vinheim: Ex Gem Lv4",
          "Vinheim: Blue Shield",
          "Vinheim: Shield Ring",
          "Vinheim: Demon Seal",
          "Vinheim: Elixir",
          "Vinheim: Spirit Bottle",
          "Vinheim: Energy Tablets",
          "Tethe'alla Base: Ex Gem Lv3 #1",
          "Temple of Lightning: Ex Gem Lv3 #1",
          "Temple of Lightning: Spirit Bottle",
          "Vinheim: Elemental Guard",
          "Vinheim: Prism Guard",
          "Vinheim: Mortality Cloak"
        };
        
        public static List<string> DogNames = new List<string>()
        {
          "None",
          "None",
          "None",
          "None",
          "None",
          "None",
          "Thoda Geyser: Bob",
          "Altimara: Lulu",
          "Asgard: Hal",
          "Asgard: Murry",
          "Palmacosta: Teddy",
          "Palmacosta: Pepe",
          "Altimira: Kenny",
          "Iselia House of Salvation: Kitty",
          "Thoda Dock: Binky",
          "Palmacosta House of Salvation: Caramel",
          "Asgard House of Salvation: Monmon",
          "Exire: Simon",
          "Hemidall: Coco",
          "Hemidall: Cookie",
          "Hima: Boo",
          "Hima: Rocky",
          "Flanoir: Poochi",
          "Flanoir: Penny",
          "Mizuho: Tiggy",
          "Iselia: Bunz",
          "Izoold: Pookie",
          "Meltokio: Chibi",
          "Meltokio: Pudding",
          "Ozette: Sammy",
          "Ozette: Kalcy",
          "Sybak: Chappy",
          "Sybak: Turbie",
          "Luin: Lucky",
          "Exire: Chuchu",
          "Triet: Cammy",
          "None",
          "None",
          "None",
          "None"
        };

        public static List<string> WomenNames = new List<string>()
        { //E0 E3 FF FE 01
          "None",// = 2fc4 - 01",
          "None",//2fc4 - 02",
          "None",//2fc4 - 04 flag for after Origin",
          "None",//2fc4 - 08",
          "None",//2fc4 - 10 flag for all women or Exire?, but doesn't matter",
          "Iselia",//20
          "Triet",//40
          "Izoold",//80
          //
          "Palmacosta",//01
          "Asgard",//02
          "None",
          "None",
          "None",//10
          "Luin",//20
          "Hima",//40
          "Meltokio",//80
          //
          "Sybak",//1
          "Ozette",//2
          "Altimira",//4
          "Flanoir",//8
          "Heimdall",//10
          "Exire",//20
          "Iselia House of Salvation",//40
          "Thoda Dock",//80
          //
          "None",//1
          "Asgard House of Salvation",//2
          "Hot Springs",//4
          "Meltokio House of Guidance",//8
          "SE Abbey",//10
          "Heimdall House of Guidance",//20
          "Thoda Geyser",//40
          "Balacruf Mausoleum",//80
          //
          "Mizuho",//01
          "None",//2fc8 - 02",
          "None",//2fc8 - 04",
          "None",//2fc8 - 08",
          "None",//2fc8 - 10",
          "None",//2fc8 - 20",
          "None",//2fc8 - 40",
          "None",//2fc8 - 80"
        };

        public static List<string> ItemNames = new List<string>()
        {
          "Apple Gel",
          "Lemon Gel",
          "Orange Gel",
          "Pineapple Gel",
          "Melange Gel",
          "Miracle Gel",
          "Elixir",
          "Energy Tablets",
          "Spirit Bottle",
          "Panacea Bottle",
          "Life Bottle",
          "Miracle Bottle",
          "Anti-Magic Bottle",
          "Flare Bottle",
          "Flanoir Potion",
          "Guard Bottle",
          "Acuity Bottle",
          "Syrup Bottle",
          "Palma Potion",
          "Shell Bottle",
          "Mizuho Potion",
          "Rune Bottle",
          "Holy Bottle",
          "Dark Bottle",
          "Savory",
          "Sage",
          "Lavender",
          "Bellabane",
          "Rosemary",
          "Saffron",
          "Red Savory",
          "Red Sage",
          "Red Lavender",
          "Red Bellebane",
          "Red Rosemary",
          "Red Saffron",
          "Magic Lens",
          "All-Divide",
          "Hourglass",
          "Ex Gem Lv1",
          "Ex Gem Lv2",
          "Ex Gem Lv3",
          "Ex Gem Lv4",
          "Aqua Quartz",
          "Green Quartz",
          "Red Quartz",
          "Yellow Quartz",
          "Blue Quartz",
          "Purple Quartz",
          "Black Quartz",
          "White Quartz",
          "Unicorn Horn",
          "Boltzman's Book",
          "Map of Balacruf",
          "Sorcerer's Ring",
          "Tower Key",
          "Card Key",
          "Wing Pack",
          "Inhibitor Ore",
          "Pass",
          "Colette's Necklace",
          "Lyla's Letter",
          "Spiritua Statue",
          "Desian Orb",
          "Mana Fragment",
          "Mana Leaf Herb",
          "Zircon",
          "Tethe'alla Map",
          "Sylvarant Map",
          "Collector's Book",
          "Monster List",
          "Figurine Book",
          "Training Manual",
          "Eternal Ring",
          "Metal Sphere",
          "Magical Cloth",
          "Mythril",
          "Beast Fang",
          "Brass",
          "Beast Hide",
          "White Silver",
          "Black Silver",
          "Mystic Herb",
          "Mermaid's Tear",
          "Pork",
          "Beef",
          "Chicken",
          "Juicy Meat",
          "Beef Strips",
          "Snapper",
          "Tuna",
          "Cod",
          "Squid",
          "Shrimp",
          "Octopus",
          "Tomato",
          "Bell Pepper",
          "Cucumber",
          "Cabbage",
          "Lettuce",
          "Mushroom",
          "Potato",
          "Onion",
          "Radish",
          "Carrot",
          "Strawberry",
          "Banana",
          "Grapes",
          "Apple",
          "Lemon",
          "Peach",
          "Pear",
          "Melon",
          "Pineapple",
          "Kirima",
          "Amango",
          "Rice",
          "Barley Rice",
          "Pasta",
          "Panyan",
          "Bread",
          "Roll",
          "Purple Satay",
          "White Satay",
          "Red Satay",
          "Black Satay",
          "Egg",
          "Cheese",
          "Milk",
          "Seaweed",
          "Kelp",
          "Tofu",
          "Konjac",
          "Miso",
          "Wooden Blade",
          "Rapier",
          "Mumei",
          "Knight's Saber",
          "Masamune",
          "Osafune",
          "Sinclaire",
          "Nimble Rapier",
          "Ogre Sword",
          "Kotetsu",
          "Shiden",
          "Saint Rapier",
          "Dragon Tooth",
          "Defenser",
          "Elemental Brand",
          "Muramasa",
          "Wasier Rapier",
          "Angel's Tear",
          "Ninja Sword",
          "Material Blade",
          "Kusanagi Blade",
          "Valkyrie Saber",
          "Paper Fan",
          "Nebilim",
          "Chakram",
          "Flying Disk",
          "Duel Ring",
          "Slicer Ring",
          "Mystic Ring",
          "Stinger Ring",
          "Ray Thrust",
          "Mythril Ring",
          "Shuriken",
          "Solar Spinner",
          "Lunar Ring",
          "Toroid",
          "Stardust",
          "Angel's Halo",
          "Tambourine",
          "Evil Eye",
          "Nova",
          "Fine Star",
          "Duel Star",
          "Falling Star",
          "Cool Orbit",
          "Thunderbolt",
          "Shining Star",
          "Shadow Dancer",
          "Cor Leonis",
          "Northern Lights",
          "Southern Cross",
          "Final Player",
          "One World",
          "Pantasmagoria",
          "Disaster",
          "Rod",
          "Battle Staff",
          "Earth Rod",
          "Ruby Wand",
          "Rune Staff",
          "Gale Staff",
          "Phoenix Rod",
          "Holy Staff",
          "Thunder Scepter",
          "Ancient Rod",
          "Hanuman's Staff",
          "Crystal Rod",
          "Deck Brush",
          "Heart of Chaos",
          "Spell Card",
          "Card of Water",
          "Card of Earth",
          "Card of Fire",
          "Card of Lightning",
          "Card of Wind",
          "Card of Ice",
          "Vajra",
          "Yaksa",
          "Asura",
          "Acalanatha",
          "Divine Judgement",
          "Money Bag",
          "Gates of Hell",
          "Stiletto",
          "Earth Dagger",
          "Hydra Dagger",
          "Assault Dagger",
          "Flame Dagger",
          "Gladius",
          "Crystal Dagger",
          "Toy Dagger",
          "Fafnir",
          "Long Sword",
          "Steel Sword",
          "Silver Sword",
          "Aqua Brand",
          "Sand Saber",
          "Lightning Sword",
          "Ice Coffin",
          "Ether Sword",
          "Flamberge",
          "Laser Blade",
          "Excalibur",
          "Last Fencer",
          "Baseball Bat",
          "Soul Eater",
          "Francesca",
          "Battle Ax",
          "Great Ax",
          "Crescent Ax",
          "Tomahawk Lance",
          "Halberd",
          "Bardiche",
          "Mythril Ax",
          "War Hammer",
          "Battle Pick",
          "Strike Ax",
          "Ogre Ax",
          "Bahamut's Tear",
          "Gaia Cleaver",
          "Pow Hammer DX",
          "Diablos",
          "Leather Greaves",
          "Iron Greaves",
          "Power Greaves",
          "Venom",
          "Bear Claw",
          "Ghost Shell",
          "Mythril Greaves",
          "Aqua Greaves",
          "Crystal Shell",
          "Flare Greaves",
          "Dragon Fang",
          "Diamond Shell",
          "Kaiser Greaves",
          "Dynast",
          "Glory Arts",
          "Apocalypse",
          "Soft Leather",
          "Chain Mail",
          "Ring Mail",
          "Iron Mail",
          "Splint Mail",
          "Breastplate",
          "Battlesuit",
          "Silver Mail",
          "Mythril Armor",
          "Rune Mail",
          "Brunnhilde",
          "Reflect",
          "Rare Plate",
          "Dragon Mail",
          "Golden Armor",
          "Star Mail",
          "Mumbane",
          "Leather Guard",
          "Fine Guard",
          "Iron Guard",
          "Elven Protector",
          "Lunar Guard",
          "Rune Guard",
          "Star Guard",
          "Prism Guard",
          "Mana Protector",
          "Elemental Guard",
          "Cloak",
          "White Cloak",
          "Amber Cloak",
          "Silk Cloak",
          "Holy Cloak",
          "Mythril Mesh",
          "Star Cloak",
          "Phoenix Cloak",
          "Mortality Cloak",
          "Robe",
          "Feather Robe",
          "Misty Robe",
          "Witch's Robe",
          "Silk Robe",
          "Rune Robe",
          "Holy Robe",
          "Spirit Robe",
          "Shaman Dress",
          "Kannazuki",
          "Leather Helm",
          "Iron Helm",
          "Armet Helm",
          "Cross Helm",
          "Duel Helm",
          "Rune Helm",
          "Sigurd",
          "Rare Helm",
          "Star Helm",
          "Golden Helm",
          "NOTHING",
          "Ribbon",
          "Blue Ribbon",
          "Striped Ribbon",
          "Tartan Ribbon",
          "Pretty Ribbon",
          "Hairpin",
          "Maid's Hairband",
          "Magical Ribbon",
          "Beret",
          "Cleric's Hat",
          "Straw Hat",
          "Pointed Hat",
          "Rune Hat",
          "Headband",
          "Star Cap",
          "Aifread's Hat",
          "Circlet",
          "Silver Circlet",
          "Gold Circlet",
          "Mythril Circlet",
          "Rune Circlet",
          "Holy Circlet",
          "Star Circlet",
          "Elemental Circlet",
          "Lid Shield",
          "Wooden Shield",
          "Omega Shield",
          "Mythril Shield",
          "Rune Shield",
          "Red Shield",
          "Rare Shield",
          "Arredoval",
          "Star Shield",
          "Beam Shield",
          "Blue Shield",
          "Leather Glove",
          "Iron Gauntlet",
          "Claw Gauntlet",
          "Mythril Gauntlet",
          "Rune Gauntlet",
          "Penguinist Gloves",
          "Rare Gauntlet",
          "Star Gauntlet",
          "Hyper Gauntlet",
          "Bracelet",
          "Iron Bracelet",
          "Mythril Bracelet",
          "Lapis Bracelet",
          "Star Bracelet",
          "Angel Bracelet",
          "Drapnir",
          "Shield Ring",
          "Gloves",
          "Kitchen Mittens",
          "Pretty Mittens",
          "Bridal Gloves",
          "Silk Gloves",
          "Cute Mittens",
          "Lovely Mittens",
          "Katz Mittens",
          "Poison Charm",
          "Drain Charm",
          "Stone Charm",
          "Paralysis Charm",
          "Stun Charm",
          "Amulet",
          "Talisman",
          "Blue Talisman",
          "Manji Seal",
          "Stun Bracelet",
          "Heal Bracelet",
          "Spirit Bangle",
          "Yasakani Jewel",
          "Yata Mirror",
          "Emerald Ring",
          "Faerie Ring",
          "Protect Ring",
          "Force Ring",
          "Resist Ring",
          "Reflect Ring",
          "Holy Ring",
          "Spirit Ring",
          "Revive Ring",
          "Attack Ring",
          "Defense Ring",
          "Magic Ring",
          "Warrior Symbol",
          "Guardian Symbol",
          "Rabbit's Foot",
          "Holy Symbol",
          "Spirit Symbol",
          "Dark Seal",
          "Demon's Seal",
          "Extreme Symbol",
          "Mystic Symbol",
          "Krona Symbol",
          "Attack Symbol",
          "Cape",
          "Leather Cape",
          "Thief's Cape",
          "Elven Cape",
          "Aqua Cape",
          "Flare Cape",
          "Thunder Cape",
          "Rune Cape",
          "Boots",
          "Leather Boots",
          "Elven Boots",
          "Water Spider",
          "Heavy Boots",
          "Rune Boots",
          "Persian Boots",
          "Jet Boots",
          "Aquamarine",
          "Amethyst",
          "Opal",
          "Garnet",
          "Sapphire",
          "Diamond",
          "Topaz",
          "Ruby",
          "Sardonyx",
          "Black Onyx",
          "Moonstone",
          "Magic Mist",
          "Reverse Doll",
          "Sephira",
          "Blue Sephira",
          "Mage Cloak",
          "Druid Cloak",
          "Warlock Garb",
          "Battle Cloak",
          "White Robe",
          "Yayoi",
          "Minazuki",
          "Nagazuki",
          "Mirror Shard",
          "Spider Figurine",
          "Chipped Dagger",
          "Derris Emblem",
          "Strike Ring",
          "Technical Ring",
          "Elevator Key",
          "Mithos' Panpipe",
          "Linkite Ocarina",
          "Snow Hare",
          "The Chosen's Orb",
          "Kratos' Locket",
          "Elf Elder's Staff",
          "Mythril Guard",
          "Solar Guard",
          "Elder Cloak",
          "Moon Robe",
          "Ancient Robe",
          "Heavenly Robe",
          "Memory Gem",
          "Employee ID",
          "Past Stone",
          "Future Stone",
          "Ymir Fruit",
          "Ex Gem Max",
          "King's Letter",
          "Corrine's Bell",
          "None",
          "Linkite Nut",
          "Exsphere Shard",
          "Blue Candle",
          "Celsius' Tear",
          "Kuchinawa's Charm",
          "Purple Card",
          "Red Card",
          "Blue Card",
          "Blue Seed",
          "White Seed",
          "None",
          "Sacred Stone",
          "None",
          "Penguinist Quill",
          "Sheena's Letter",
          "Pellets",
          "Fine Pellets",
          "Super Pellets",
          "Rare Pellets",
          "Assassin's Ring",
          "Turquoise",
          "Virginia's Diary",
          "Aifread's Letter",
          "Nebilim's Key",
          "Secret Notebook",
          "Pink Pearl Ring",
          "Spiritua's Ring",
          "Vinheim Key"
        };

        public static string[] CharacterNames = { "Lloyd", "Colette", "Genis", "Raine", "Sheena", "Zelos", "Presea", "Regal", "Kratos" };

        public static string[,] Titles = new string[9, 32]{
            { "Unknown 1","Unknown 2","Unknown 3","Berserker","Eternal Apprentice","Gung Ho","Boorish General","Lone General",
                "Brave Soul","Tetra Slash","Combo Master","Combo Expert","Comboist","Combo Newbie","Holy Sword","Master Swordsman",
                "Grand Swordsman","Tactical Leader","Sword of Swords","Unknown 4","Unknown 5","Midlife Crisis","Peeping Tom","Gentle Idealist",
                "Beach Boy","Arrgh, Me Hearties","Nobleman","Gourmet King","Eternal Swordsman","Drifting Swordsman","Swordsman","Unknown 6"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Friendship First","Don't Run!","Self-Control","Single-minded","Oblivious","Little Pickpocket",
                "Angelic Maiden","Tiny Angel","Chosen","Super Girl","Turbo Waitress","Ironing Board","Dog Lover","Ill-fated Girl",
                "Mermaid","Maid","Fair Lady","Charismatic Chef","Klutz","Spiritua Reborn","Fledgling Chosen","Unknown 11"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","I Hate Gels!","Magic Cycle","Dependent","Study Harder!","Experimental",
                "Warlock","Sorcerer","Mana Master","Ultimate Kid","Strategist","Figuring Collector","Item Collector","Brotherly Love",
                "Beach Comber","Katz Katz Katz","Easter Sunday","Little Chef","Friend","Honor Roll","Magic User","Unknown 12"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","Unknown 12","Unknown 13","Unknown 15","Survivor","Never Say Never",
                "Crimson Rose","Item Keeper","Wisewoman","Professor","Researcher","Gladiator Queen","Monster Collector","Sisterly Love",
                "No, Not the Sun!","Maiden","Glamorous Beauty","Passable Chef?","Grand Healer","Archeological Mania","Teacher","Unknown 16"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","Unknown 12","Unknown 13","Unknown 15","Combo Conductor","Party Comboist",
                "Indecisive","Chicken","Ultimate Summoner","Acrobat","Rose of Battle","WOW!","Treasure Hunter","Master Cook",
                "Queen of the Beach","Successor","You Look Great!","Master Summoner","Elemental Summoner","Summoner","Mysterious Assassin","Unknown 16"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","Unknown 12","Unknown 13","Unknown 15","Unknown 16","Loudmouth",
                "Commander in Chief","Gilgamesh","Casanova","Tetra Slash","Elegant Swordsman","Gleaming Knight","Grand Champion","Idiot Chosen",
                "Pickup Artist","Masked Swordsman","Narcissist","Gourmet Prince","Gigolo","Princess Guard","Magic Swordsman","Unknown 17"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","Unknown 12","Unknown 13","Unknown 15","Unknown 16","Unknown 17",
                "Hunter","Associate","Lone Girl","Fragile Shield","Bursting Girl","Axman","Deadly Flower","Paw Mania",
                "First-timer at Sea","Dream Traveler","Little Madam","Master Chef","Empty Soul","Mature Kid","Taciturn Girl","Unknown 18"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","Unknown 12","Unknown 13","Unknown 15","Unknown 16","Unknown 17",
                "Pratfall King","Way of the Jungle","Potion King","Testosterone","Perfect Battler","Battle Artist","King of the Coliseum","Paw Dandy",
                "Swimmer","God of the Kitchen","Dandy","True Chef","Eternal Sinner","El Presidente","Convict","Unknown 18"},
            { "Unknown 1","Unknown 2","Unknown 3","Unknown 4","Unknown 5","Unknown 6","Unknown 7","Unknown 8",
                "Unknown 9","Unknown 10","Unknown 11","Unknown 12","Unknown 13","Unknown 15","Unknown 16","Unknown 17",
                "Unknown 18","Unknown 19","Unknown 20","Unknown 21","Unknown 22","Tetra Slash","War God","Battle God",
                "Magic Swordsman","Conqueror","Judgment","Gourmet Master","Dad","Traitor","Mercenary","Unknown 23"}
        };

        public static string[,] Techs = new string[9, 40]{
            { "","","","","","Falcon's Crest*","Holy Guardian*","Guardian",
                "Beast Sword Rain","Raining Tiger Blade","Tempest Beast","Tempest Thrust","Demonic Tiger Blade","Demonic Thrust","Aurora Slash","Sonic Burst",
                "Final Justice","Rising Falcon","Hunting Beast","Raging Beast","Beast","Psi Tempest","Omega Tempest","Tempest",
                "Sword Rain: Beta","Sonic Sword Rain","Sword Rain: Alpha","Sword Rain","Super Sonic Thrust","Hurricane Thrust","Sonic Thrust","Twin Tiger Blade",
                "Heavy Tiger Blade","Tiger Rage","Tiger Blade","Demonic Circle","Demonic Chaos","Fierce Demon Fang","Double Demon Fang","Demon Fang"},
            { "","","","","","","","",
                "","-????-*","Sephiroth*","Grand Cross*","Judgment","Sacrifice","Holy Song","Angel Feathers",
                "Damage Guard","Final Fury*","Dancing Sickles*","Mirage Saber*","Stardust Cross","Grand Fall","Grand Chariot","Item Rover",
                "Item Thief","Listra*","Whirlwind Rush","Listra*","Ring Cyclone","Ring Whirlwind","Torrential Para Ball","Para Ball",
                "Hammer Rain","Pow Pow Hammer","Pow Hammer","Triple Ray Satellite","Ray Satellite","Triple Ray Thrust","Dual Ray Thrust","Ray Thrust"},
            { "","","","Indignation Judg.*","Force Field","Tetraspell*","Divine Power*","Meteor Storm",
                "Prism Sword","Earth Bite","Absolute","Atlas","Dreaded Wave","Thunder Arrow","Spiral Flare","Gravity Well",
                "Raging Mist","Freeze Lancer","Ice Tornado","Icicle","Indignation","Spark Wave","Thunder Blade","Lightning",
                "Ground Dasher","Grave","Stalagmite","Stone Blast","Cyclone","Air Blade","Air Thrust","Wind Blade",
                "Explosion","Flame Lance","Eruption","Fire Ball","Tidal Wave","Aqua Laser","Spread","Aqua Edge"},
            {"","","","","","","","",
                "","","","Bloody Lance*","Dark Sphere*","Force Field","Sacred Light*","Magic Shell*",
                "Holy Lance","Ray","Photon","Charge","Permaguard","Field Barrier","Barrier","Keenness",
                "Acuteness","Sharpness","Anti-Magic","Nullify","Dispel","Restore","Purify","Recover",
                "Revive","Resurrection","Revitalize","Healing Circle","Nurse","Cure","Heal","First Aid"},
            {"","","","Summon: Heart*","Summon: Birth","Summon: Origin","Summon: Darkness","Summon: Lightning",
                "Summon: Ice","Summon: Earth","Summon: Light","Summon: Wind","Summon: Water","Summon: Fire","Summon: Corrine","T. Seal: Darkness",
                "S. Seal: Lightning","S. Seal: Ice","S. Seal: Earth","S. Seal: Light","S. Seal: Wind","S. Seal: Water","S. Seal: Fire","Guardian Seal",
                "Cyclone Seal","Purgatory Seal","Demon Seal","Force Seal","Spirit Seal","Life Seal","M. Seal Absolute","Mirage Seal Pinion",
                "Mirage Seal","S. Seal Absolute","Serpent Seal Pinion","Serpent Seal","Power Seal Absolute","Power Seal Pinion","Power Seal","Pyre Seal"},
            {"","","","","","","","",
                "","","","Judgment","Gungnir*","Healing Wind","Healing Stream","First Aid",
                "Thunder Blade","Lightning","Grave","Stone Blast","Air Thrust","Wind Blade","Eruption","Fire Ball",
                "Guardian","Eternal Chaos*","Spirit of the Earth*","Demon Spear","S. Lightning Blade","Lightning Blade","Hell Pyre","Light Spear Cannon",
                "Victory Light Spear","Light spear","Super Sonic Thrust","Hurricane Thrust","Sonic Thrust","Fierce Demon Fange","Double Demon Fang","Demon Fang"},
            {"","","","","","","","",
                "","","","","","","","",
                "","Earthly Protection","Eternal Wind*","Beast","Eternal Damnation","Fiery Infliction","Eternal Devastation","Mass Devastation",
                "Finite Devastation","Devastation","Finality Infliction*","Resolute Infliction","Endless Infliction","Dual Infliction","Infliction","Finality Punishment",
                "Rising Punishment","Dual Punishment","Punishment","Ultimate*","Fiery Destruction","Infinite Destruction","Deadly Destruction","Destruction"},
            {"","","","","","","","",
                "","","","","","Bastion","Mirage","Rage*",
                "Dragon's Talon*","Heaven Rising*","Crescent Dark Moon","Dragon Strike*","Triple Rage Kick","Heaven's Charge","Dragon Rage","Dragon Fury",
                "Rising Dragon","Eagle Fall","Eagle Rage","Eagle Dive","Dragon Dance","Swallow Dance","Swallow Kick","Dragon Fang*",
                "Heel Kick*","Wolverine","Triple Kick","Spin Kick","Crescent Moon","Grand Healer","Chi Healer","Healer"},
            {"","","","","","","","",
                "","","","Judgment","Gungnir*","Healing Wind","Healing Stream","First Aid",
                "Thunder Blade","Lightning","Grave","Stone Blast","Air Thrust","Wind Blade","Eruption","Fire Ball",
                "Guardian","Eternal Chaos*","Spirit of the Earth*","Demon Spear","S. Lightning Blade","Lightning Blade","Hell Pyre","Light Spear Cannon",
                "Victory Light Spear","Light spear","Super Sonic Thrust","Hurricane Thrust","Sonic Thrust","Fierce Demon Fange","Double Demon Fang","Demon Fang"}
        };
    }
}
