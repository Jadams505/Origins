﻿using Origins.Items.Materials;
using Origins.Tiles.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Origins.Dev;
namespace Origins.Items.Weapons.Demolitionist {
	public class Omnidirectional_Claymore : ModItem, ICustomWikiStat {
        public string[] Categories => [
            "OtherExplosive",
            "SpendableWeapon"
        ];
        public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 5;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.LandMine);
			Item.damage = 165;
			Item.createTile = ModContent.TileType<Omnidirectional_Claymore_Tile>();
			Item.value = Item.sellPrice(gold: 1, silver: 75);
			Item.rare = ItemRarityID.Pink;
		}
		public override void AddRecipes() {
			return;//TODO: implement
			Recipe recipe = Recipe.Create(Type, 250);
			recipe.AddIngredient(ItemID.ExplosivePowder, 7);
			recipe.AddIngredient(ModContent.ItemType<Busted_Servo>(), 13);
			recipe.AddIngredient(ModContent.ItemType<Rotor>(), 25);
			recipe.AddTile(TileID.MythrilAnvil); //Fabricator
			recipe.Register();
		}
	}
}
