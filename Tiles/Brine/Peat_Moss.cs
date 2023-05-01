﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Brine {
	public class Peat_Moss : OriginTile {
		public override void SetStaticDefaults() {
			Main.tileMergeDirt[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
			ItemDrop = ItemType<Peat_Moss_Item>();
			AddMapEntry(new Color(18, 160, 56));
			HitSound = SoundID.Dig;
		}
	}
}
