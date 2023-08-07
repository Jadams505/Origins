﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Boomerang_Magnet : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Boomerang Magnet");
			// Tooltip.SetDefault("Increased return speed for boomerangs");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.DefaultToAccessory(28, 22);
			Item.value = Item.sellPrice(gold: 4);
			Item.rare = ItemRarityID.Green;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.TreasureMagnet);
			recipe.AddIngredient(ModContent.ItemType<Turbo_Reel>());
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
		public override void UpdateEquip(Player player) {
			//player.GetModPlayer<OriginPlayer>().boomerangMagnet = true;
		}
	}
}
