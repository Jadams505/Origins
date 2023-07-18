﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Origins.Tiles.Other {
    public class Laser_Tag_Console : ModTile {
		public override void SetStaticDefaults() {
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(81, 81, 81), Language.GetText("Laser Tag Console"));
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Laser_Tag_Console_Item>());
		}
	}
	public class Laser_Tag_Console_Item : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Laser Tag Console");
			// Tooltip.SetDefault("Used to set up laser tag matches");
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.LampPost);
			Item.value = Item.buyPrice(gold: 10);
			Item.createTile = ModContent.TileType<Laser_Tag_Console>();
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.CopperBar, 2);
			recipe.AddIngredient(ModContent.ItemType<Busted_Servo>(), 5);
			recipe.AddIngredient(ModContent.ItemType<Power_Core>());
			recipe.AddIngredient(ModContent.ItemType<Silicon>(), 8);
			recipe.AddTile(TileID.MythrilAnvil); //Fabricator
			recipe.Register();
		}
	}
}
