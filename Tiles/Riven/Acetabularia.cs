﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Riven {
	public class Acetabularia : OriginTile, DefiledTile {
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileObsidianKill[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			TileID.Sets.ReplaceTileBreakUp[Type] = true;
			TileID.Sets.IgnoredInHouseScore[Type] = true;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Acetabularia");
			AddMapEntry(new Color(0, 175, 200), name);

			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newTile.AnchorValidTiles = new int[] {
				TileType<Riven_Grass>()
			};
			TileObjectData.addTile(Type);

			HitSound = SoundID.Grass;
			DustType = DustID.Marble;
			ItemDrop = ItemType<Acetabularia_Item>();
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) {
			if (i % 2 == 0) {
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak) {
			Main.tile[i, j].TileFrameX = (short)(18 * (i % 3));
			return true;
		}
	}
	public class Acetabularia_Item : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Acetabularia");
			SacrificeTotal = 25;
		}
		public override void SetDefaults() {
			Item.maxStack = 999;
		}
	}
}
