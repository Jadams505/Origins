﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Other {
	public class Batholith : OriginTile {
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			TileID.Sets.Stone[Type] = true;
			ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = ItemType<Batholith_Item>();
			AddMapEntry(new Color(35, 10, 10));
			mergeID = TileID.Stone;
			MinPick = 250;
			MineResist = 2;
		}
		public override bool CanExplode(int i, int j) {
			return false;
		}
	}
	public class Batholith_Item : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Batholith");
			// Tooltip.SetDefault("'Respect your elders'");
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = TileType<Batholith>();
		}
	}
}
