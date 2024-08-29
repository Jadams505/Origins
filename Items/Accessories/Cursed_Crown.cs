﻿using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Origins.Items.Accessories {
	[AutoloadEquip(EquipType.Face)]
	public class Cursed_Crown : ModItem, ICustomWikiStat {
		public string[] Categories => [
			"MasterAcc"
		];
		public override void SetDefaults() {
			Item.DefaultToAccessory(22, 32);
			Item.rare = ItemRarityID.Green;
			Item.value = Item.sellPrice(gold: 1);
			Item.master = true;
		}
		public override void UpdateAccessory(Player player, bool hideVisual) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.cursedCrown = true;
			player.canFloatInWater = true;
			if (player.wet) player.velocity.Y -= 0.5f;
			player.noFallDmg = true;
			player.moveSpeed *= 0.9f;
			player.jumpSpeedBoost += 3.75f;
			player.endurance += (1 - player.endurance) * 0.3f;
			if (!hideVisual) originPlayer.cursedCrownVisual = true;
		}
		public override void UpdateVanity(Player player) {
			player.GetModPlayer<OriginPlayer>().cursedCrownVisual = true;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(ItemID.GoldCrown);
			recipe.AddIngredient(Type);
			recipe.AddTile(TileID.BewitchingTable);
			recipe.Register();
		}
	}
}
