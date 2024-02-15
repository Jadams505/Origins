﻿using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Control_Locus : ModItem, ICustomWikiStat {
		public string[] Categories => new string[] {
			"Combat",
			"Resource",
			"Ranged"
		};
		public override void SetDefaults() {
			Item.DefaultToAccessory(14, 28);
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.master = true;
		}
		public override void UpdateEquip(Player player) {
			player.GetModPlayer<OriginPlayer>().controlLocus = true;
		}
	}
}
