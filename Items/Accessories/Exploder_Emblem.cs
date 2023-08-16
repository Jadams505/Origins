﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Exploder_Emblem : ModItem {
		public override void SetStaticDefaults() {
			OriginExtensions.InsertIntoShimmerCycle(Type, ItemID.SummonerEmblem);
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.WarriorEmblem);
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.GetDamage(DamageClasses.Explosive) += 0.05f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(ItemID.AvengerEmblem);
			recipe.AddIngredient(Type);
			recipe.AddIngredient(ItemID.SoulofMight, 5);
			recipe.AddIngredient(ItemID.SoulofSight, 5);
			recipe.AddIngredient(ItemID.SoulofFright, 5);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}
