﻿using Origins.Items.Accessories;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Origins.Dev;
namespace Origins.Items.Weapons.Melee {
	public class The_Bird : ModItem, ICustomWikiStat {
        public string[] Categories => [
            "Sword",
			"DeveloperItem",
			"ReworkExpected"
        ];
        public override void SetDefaults() {
			Item.CloneDefaults(ItemID.WoodenSword);
			Item.damage = 99;
			Item.useAnimation = Item.useTime = 20;
			Item.rare = ItemRarityID.Cyan;
			Item.knockBack = (int)99999996;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Baseball_Bat>());
			recipe.AddIngredient(ModContent.ItemType<Razorwire>());
			//recipe.AddCondition(player.name == "Pandora");
			recipe.DisableRecipe();
		}
	}
}
