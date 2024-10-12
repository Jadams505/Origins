﻿using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Origins.Items.Accessories {
	[AutoloadEquip(EquipType.Shield)]
	public class Shield_of_Retribution : ModItem, ICustomWikiStat {
		public string[] Categories => [
			"Combat"
		];
		public override void SetDefaults() {
			Item.DefaultToAccessory(36, 38);
			Item.defense = 3;
			Item.shoot = ProjectileID.BulletHighVelocity;
			Item.value = Item.sellPrice(gold: 3);
			Item.rare = ItemRarityID.Pink;
		}
		public override void UpdateEquip(Player player) {
			player.noKnockback = true;
			player.fireWalk = true;
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.retributionShield = true;
			originPlayer.retributionShieldItem = Item;
		}
		public override void AddRecipes() {
			CreateRecipe()
			.AddIngredient(ItemID.ObsidianShield)
			.AddIngredient(ModContent.ItemType<Razorwire>())
			.AddTile(TileID.TinkerersWorkbench)
			.Register();
		}
	}
}
