﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Walls {
	[LegacyName("Sulphur_Stone_Wall", "Dolomite_Wall")]
	public class Baryte_Wall : ModWall {
		public override void SetStaticDefaults() {
			Main.wallBlend[Type] = WallID.Stone;//what wall type this wall is considered to be when blending
			Origins.WallHammerRequirement[Type] = 70;
			AddMapEntry(new Color(6, 26, 19));
		}
		public override void RandomUpdate(int i, int j) {
			Tile tile = Framing.GetTileSafely(i, j);
			if (j >= Main.worldSurface - 50 && (tile.LiquidAmount == 0 || (tile.LiquidAmount < 255 && tile.LiquidType == LiquidID.Water))) {
				tile.LiquidAmount = 255;
				tile.LiquidType = LiquidID.Water;
				WorldGen.SquareTileFrame(i, j);
			}
		}
	}
	[LegacyName("Sulphur_Stone_Wall_Safe", "Dolomite_Wall_Safe")]
	public class Baryte_Wall_Safe : Defiled_Stone_Wall {
		public override string Texture => "Origins/Walls/Baryte_Wall";
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			Main.wallHouse[Type] = true;
		}
	}
	[LegacyName("Sulphur_Stone_Wall_Item", "Dolomite_Wall_Item")]
	public class Baryte_Wall_Item : ModItem {
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.StoneWall);
			Item.createWall = WallType<Baryte_Wall_Safe>();
		}
	}
}
