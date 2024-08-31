﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Origins.Items.Materials;
using Microsoft.Xna.Framework;

namespace Origins.Items.Other.Consumables.Food {
	public class Soul_Spaghetti : ModItem {
        public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 5;
			ItemID.Sets.FoodParticleColors[Type] = [
				new Color(129, 129, 129),
				new Color(88, 88, 88),
				new Color(66, 66, 66)
			];
			ItemID.Sets.IsFood[Type] = true;
		}
		public override void SetDefaults() {
			Item.DefaultToFood(
				32, 24,
				BuffID.WellFed2,
				60 * 60 * 8
			);
			Item.holdStyle = ItemHoldStyleID.None;
			Item.scale = 0.6f;
			Item.value = Item.sellPrice(silver: 10);
			Item.rare = ItemRarityID.Green;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.BowlofSoup);
			recipe.AddIngredient(ModContent.ItemType<Strange_String>(), 10);
			recipe.AddTile(TileID.CookingPots);
			recipe.Register();
		}
	}
}
