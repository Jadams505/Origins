﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class CFHES : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("C.F.H.E.S.");
			// Tooltip.SetDefault("All explosives inflict 'On Fire!' and have a reduced fuse time\nGreatly increased explosive blast radius");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.DefaultToAccessory(28, 34);
			Item.value = Item.sellPrice(gold: 4, silver: 20);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.GetModPlayer<OriginPlayer>().explosiveBlastRadius *= 2.2f;
			player.GetModPlayer<OriginPlayer>().magicTripwire = true;

			player.GetModPlayer<OriginPlayer>().dangerBarrel = true;
			player.GetModPlayer<OriginPlayer>().explosiveFuseTime *= 0.666f;
		}
	}
}