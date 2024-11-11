﻿using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Other {
    public class Carburite : OriginTile {
        public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
			Main.tileMerge[Type][TileID.Dirt] = true;
			Main.tileMerge[TileID.Dirt][Type] = true;
			AddMapEntry(new Color(110, 57, 33));
			MinPick = 55;
			MineResist = 3;
		}
	}
	public class Carburite_Item : ModItem, ICustomWikiStat {
		public string[] Categories => [
			"Ore"
		];
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.value = Item.sellPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;// to match the vanilla ores which require 55 pickaxe power to mine
			Item.createTile = TileType<Carburite>();
		}
		public void ModifyWikiStats(JObject data) {
			string base_key = $"WikiGenerator.Stats.{Mod?.Name}.{Name}.";
			string key = base_key + "Crafting";
			data.AppendStat("Crafting", Language.GetTextValue(key), key);
			data.Add("Tier", 5);
		}
	}
}
