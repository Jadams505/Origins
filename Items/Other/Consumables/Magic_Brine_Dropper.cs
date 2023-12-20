﻿using Origins.World.BiomeData;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.Items.Other.Consumables {
	public class Magic_Brine_Dropper : ModItem {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;
		}
		public override void SetDefaults() {
			Item.value = Item.sellPrice(copper: 40);
			Item.rare = ItemRarityID.Blue;
			Item.maxStack = 999;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddCondition(
			   Language.GetOrRegister("Mods.Origins.Conditions.Brine"),
			   () => Main.LocalPlayer.adjWater && Main.LocalPlayer.InModBiome<Brine_Pool>()
			);
			recipe.AddIngredient(ItemID.EmptyDropper);
			recipe.AddTile(TileID.CrystalBall);
			recipe.Register();
		}
	}
}