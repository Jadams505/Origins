﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Super_Turbo_Reel : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Super Turbo Reel");
			// Tooltip.SetDefault("Greatly increased movement speed and return speed for harpoon guns");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.DefaultToAccessory(28, 22);
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
		}
		public override void UpdateEquip(Player player) {
			/*player.moveSpeed += 0.15f;
			player.runAcceleration += 0.75f;*/
			player.hasMagiluminescence = true;
			player.GetModPlayer<OriginPlayer>().turboReel2 = true;
		}
	}
}
