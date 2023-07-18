﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Other.Consumables.Food {
	public class Jam_Sandwich : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Jam Sandwich");
			// Tooltip.SetDefault("{$CommonItemTooltip.MediumStats}\n'What kind of jam is this?'");
			Item.ResearchUnlockCount = 5;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ShrimpPoBoy);
			Item.holdStyle = ItemHoldStyleID.None;
			Item.scale = 0.6f;
			Item.buffType = BuffID.WellFed2;
			Item.buffTime = 60 * 60 * 10;
			Item.value = Item.sellPrice(silver: 10);
			Item.rare = ItemRarityID.Orange;
		}
	}
}
