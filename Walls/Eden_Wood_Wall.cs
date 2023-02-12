﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Walls {
    public class Eden_Wood_Wall : ModWall {
		public override void SetStaticDefaults() {
			Main.wallBlend[Type] = WallID.Wood;
			AddMapEntry(new Color(75, 20, 20));
		}
	}
	public class Eden_Wood_Wall_Item : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Eden Wood Wall");
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.WoodWall);
			Item.createWall = WallType<Eden_Wood_Wall>();
		}
	}
}
