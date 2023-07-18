﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Walls {
    public class Sulphur_Stone_Wall : ModWall {
		public override void SetStaticDefaults() {
			WallID.Sets.Conversion.Stone[Type] = true;
			Main.wallBlend[Type] = WallID.Stone;//what wall type this wall is considered to be when blending
			Origins.WallHammerRequirement[Type] = 70;
			AddMapEntry(new Color(6, 26, 19));
		}
		public override void RandomUpdate(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			if (tile.LiquidAmount == 0 || (tile.LiquidAmount < 255 && tile.LiquidType == LiquidID.Water)) {
				tile.LiquidAmount = 255;
				tile.LiquidType = LiquidID.Water;
			}
		}
	}
	public class Sulphur_Stone_Wall_Safe : Defiled_Stone_Wall {
		public override string Texture => "Origins/Walls/Sulphur_Stone_Wall";
		public override void SetStaticDefaults() {
			ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = ItemType<Sulphur_Stone_Wall_Item>();
			Main.wallHouse[Type] = true;
			base.SetStaticDefaults();
		}
	}
	public class Sulphur_Stone_Wall_Item : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Sulphur Stone Wall");
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.StoneWall);
			Item.createWall = WallType<Sulphur_Stone_Wall_Safe>();
		}
	}
}
