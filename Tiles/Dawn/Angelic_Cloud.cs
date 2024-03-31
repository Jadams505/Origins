﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Dawn {
	public class Angelic_Cloud : OriginTile {
        public string[] Categories => new string[] {
            "OtherBlock"
        };
        public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(150, 150, 20));
			mergeID = TileID.Cloud;
		}
	}
	public class Angelic_Cloud_Item : ModItem {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 100;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.Cloud);
			Item.createTile = TileType<Angelic_Cloud>();
		}
	}
}
