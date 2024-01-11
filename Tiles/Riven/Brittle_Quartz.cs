﻿using Microsoft.Xna.Framework;
using Origins.Items.Accessories;
using Origins.Walls;
using Origins.World.BiomeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Riven {
	public class Brittle_Quartz : OriginTile, RivenTile {
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileMergeDirt[Type] = Main.tileMergeDirt[TileID.Sandstone];
			TileID.Sets.Conversion.Sandstone[Type] = true;
			TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
			//Main.tileMerge[TileID.Sandstone][Type] = true;
			//Main.tileMerge[Type] = Main.tileMerge[TileID.Sandstone];
			//Main.tileMerge[Type][TileID.Sandstone] = true;
			/*for(int i = 0; i < TileLoader.TileCount; i++) {
                Main.tileMerge[i][Type] = Main.tileMerge[i][TileID.Sandstone];
            }*/
			AddMapEntry(new Color(150, 150, 150));
			mergeID = TileID.Sandstone;
			AddDefiledTile();
			DustType = Riven_Hive.DefaultTileDust;
		}
	}
	public class Brittle_Quartz_Item : ModItem {
		public override void SetStaticDefaults() {
            ItemID.Sets.ShimmerTransformToItem[ModContent.ItemType<Silica_Item>()] = Type;
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<Silica_Item>();
            Item.ResearchUnlockCount = 100;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.StoneBlock);
			Item.createTile = TileType<Brittle_Quartz>();
		}
        public override void AddRecipes() {
			//TODO: add the chlorophyte extractor recipes properly
            CreateRecipe()
            .AddIngredient(ItemID.Sandstone)
            .AddTile(TileID.ChlorophyteExtractinator)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.CorruptSandstone)
            .AddTile(TileID.ChlorophyteExtractinator)
            .Register();

            CreateRecipe()
            .AddIngredient(ItemID.CrimsonSandstone)
			.AddTile(TileID.ChlorophyteExtractinator)
            .Register();

            //CreateRecipe()
            //.AddIngredient(ModContent.ItemType<Brittle_Quartz>())
            //.AddTile(TileID.ChlorophyteExtractinator)
            //.Register();

            //CreateRecipe()
            //.AddIngredient(ModContent.ItemType<Ashen_Sandstone>())
            //.AddTile(TileID.ChlorophyteExtractinator)
            //.Register();

            CreateRecipe()
            .AddIngredient(ModContent.ItemType<Brittle_Quartz_Wall_Item>(), 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}
