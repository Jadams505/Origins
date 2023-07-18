﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	[LegacyName("Mad_Hand")]
	public class SOTH : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("SOTH");
			// Tooltip.SetDefault("Double the gunpowder, double the fun.");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.DefaultToAccessory(32, 26);
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
		}
		public override void UpdateEquip(Player player) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.madHand = true;
			originPlayer.explosiveBlastRadius += 0.25f;
			originPlayer.explosiveThrowSpeed += 0.65f;
		}
	}
}
