﻿using Origins.Journal;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Spirit_Shard : ModItem, IJournalEntryItem {
		public string IndicatorKey => "Mods.Origins.Journal.Indicator.Whispers";
		public string EntryName => "Origins/" + typeof(Eccentric_Stone_Entry).Name;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Spirit Shard");
			// Tooltip.SetDefault("Artifact minions turn into ghosts of their former selves upon death");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.DefaultToAccessory(16, 16);
			Item.rare = ItemRarityID.Green;
		}
		public override void UpdateEquip(Player player) {
			player.GetModPlayer<OriginPlayer>().spiritShard = true;
		}
	}
}
