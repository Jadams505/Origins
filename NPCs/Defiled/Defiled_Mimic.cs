﻿using Microsoft.Xna.Framework;
using Origins.Items.Accessories;
using Origins.Items.Weapons.Ranged;
using Origins.World.BiomeData;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.NPCs.Defiled {
	public class Defiled_Mimic : ModNPC, IDefiledEnemy {
		public override string Texture => "Terraria/Images/NPC_" + NPCID.BigMimicCorruption;// remove when sprite is complete
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("{$Defiled} Mimic");
			Main.npcFrameCount[NPC.type] = 14;
			SpawnModBiomes = new int[] {
				ModContent.GetInstance<Defiled_Wastelands>().Type
			};
		}
		public override void SetDefaults() {
			NPC.width = 28;
			NPC.height = 44;
			NPC.aiStyle = NPCAIStyleID.Biome_Mimic;
			NPC.damage = 90;
			NPC.defense = 34;
			NPC.lifeMax = 3500;
			NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.value = 30000f;
			NPC.knockBackResist = 0.1f;
			NPC.rarity = 5;
		}
		public bool ForceSyncMana => false;
		public float Mana { get; set; }
		public override void FindFrame(int frameHeight) {
			NPC.CloneFrame(NPCID.BigMimicCorruption, frameHeight);
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.OneFromOptions(1,
				ItemID.SoulDrain,
				ModContent.ItemType<Incision>(),
				ItemID.FetidBaghnakhs,
				ModContent.ItemType<Ravel>(),
				ItemID.TendonHook
			));
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterHealingPotion, 1, 5, 10));
			npcLoot.Add(ItemDropRule.Common(ItemID.GreaterManaPotion, 1, 5, 15));
		}
		public override void HitEffect(NPC.HitInfo hit) {
			//spawn gore if npc is dead after being hit
			if (NPC.life < 0) {
				for (int i = 0; i < 3; i++) Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF3_Gore"));
				for (int i = 0; i < 6; i++) Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Medium" + Main.rand.Next(1, 4)));
			}
		}
	}
}
