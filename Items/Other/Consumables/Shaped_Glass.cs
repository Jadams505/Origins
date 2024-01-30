﻿using Origins.NPCs.Fiberglass;
using Origins.World.BiomeData;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Other.Consumables {
    public class Shaped_Glass : ModItem {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 20;
			ItemID.Sets.SortingPriorityBossSpawns[Type] = 1;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.SuspiciousLookingEye);
		}
		public override bool? UseItem(Player player) {
			if (player.InModBiome<Fiberglass_Undergrowth>()) {
                SoundEngine.PlaySound(SoundID.Roar);
                NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<Fiberglass_Weaver>());
				return true;
			}
			return false;
		}
	}
}
