﻿using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Origins.Items.Accessories {
	public class Speed_Booster : ModItem, ICustomWikiStat {
		public string[] Categories => [
			"Combat",
			"Movement",
			"RangedBoostAcc",
			"GenericBoostAcc"
		];
		public override void SetDefaults() {
			Item.DefaultToAccessory(16, 24);
			Item.damage = 30;
			Item.knockBack = 2;
			Item.value = Item.sellPrice(gold: 4);
			Item.rare = ItemRarityID.LightRed;
			Item.accessory = true;
		}
		public override void AddRecipes() {
			Recipe.Create(Type)
			.AddIngredient<Automated_Returns_Handler>()
			.AddIngredient<Lovers_Leap>()
			.AddTile(TileID.TinkerersWorkbench)
			.Register();
		}
		public override void UpdateAccessory(Player player, bool hideVisual) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			player.hasMagiluminescence = true;
			if (player.accRunSpeed < 6f) player.accRunSpeed = 6f;
			if (originPlayer.shineSparkCharge > 0) {
				player.accRunSpeed += 3f;
				player.armorEffectDrawShadowBasilisk = true;
			}
			player.rocketBoots = 2;
			originPlayer.guardedHeart = true;
			originPlayer.loversLeap = true;
			originPlayer.loversLeapItem = Item;
			originPlayer.shineSpark = true;
			originPlayer.shineSparkItem = Item;
			originPlayer.shineSparkVisible = !hideVisual;
			originPlayer.turboReel2 = true;
			originPlayer.automatedReturnsHandler = true;

			player.blackBelt = true;
			player.dashType = 1;
			player.spikedBoots += 2;
		}
	}
}
