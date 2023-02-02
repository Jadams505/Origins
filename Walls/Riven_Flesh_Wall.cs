﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Walls {
	public class Riven_Flesh_Wall : ModWall {
		public override void SetStaticDefaults() {
			WallID.Sets.Conversion.Stone[Type] = true;
			Main.wallBlend[Type] = WallID.Stone;//what wall type this wall is considered to be when blending
			AddMapEntry(new Color(40, 140, 200));
		}
	}
	public class Riven_Flesh_Wall_Safe : Defiled_Stone_Wall {
		public override string Texture => "Origins/Walls/Riven_Flesh_Wall";
		public override void SetStaticDefaults() {
			ItemDrop = ItemType<Riven_Flesh_Wall_Item>();
			Main.wallHouse[Type] = true;
			base.SetStaticDefaults();
		}
	}
	public class Riven_Flesh_Wall_Item : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spug Flesh Wall");
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.StoneWall);
			Item.createWall = WallType<Riven_Flesh_Wall_Safe>();
		}
	}
}
