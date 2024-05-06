﻿using Microsoft.Xna.Framework;
using Origins.Buffs;
using Origins.Items.Materials;
using Origins.Tiles.Defiled;
using Origins.World.BiomeData;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Defiled {
	public class Shattered_Ghoul : ModNPC, IDefiledEnemy {
		public override void SetStaticDefaults() {
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, new NPCID.Sets.NPCBestiaryDrawModifiers() { // Influences how the NPC looks in the Bestiary
				Velocity = 1
			});
			Main.npcFrameCount[NPC.type] = 8;
		}
		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DesertGhoulCorruption);
			NPC.aiStyle = NPCAIStyleID.Fighter;
			NPC.lifeMax = 280;
			NPC.defense = 30;
			NPC.knockBackResist = 0.5f;
			NPC.damage = 60;
			NPC.width = 20;
			NPC.height = 44;
			NPC.value = 700;
			NPC.friendly = false;
			NPC.HitSound = Origins.Sounds.DefiledHurt;
			NPC.DeathSound = Origins.Sounds.DefiledKill;
			NPC.value = Item.buyPrice(silver: 6, copper: 50);
			Banner = NPCID.DesertGhoul;
			AnimationType = NPCID.DesertGhoulCorruption;
			SpawnModBiomes = [
				ModContent.GetInstance<Defiled_Wastelands_Underground_Desert>().Type
			];
		}
		public int MaxMana => 100;
		public int MaxManaDrain => 20;
		public float Mana { get; set; }
		public bool ForceSyncMana => true;
		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
			this.DrainMana(target);
			target.AddBuff(Rasterized_Debuff.ID, 36);
		}
		public void Regenerate(out int lifeRegen) {
			int factor = Main.rand.RandomRound((180f / NPC.life) * 8);
			lifeRegen = factor;
			Mana -= factor / 180f;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			if (!spawnInfo.DesertCave) return 0;
			if (!spawnInfo.Player.InModBiome<Defiled_Wastelands>()) return 0;
			return Defiled_Wastelands.SpawnRates.Ghoul;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				this.GetBestiaryFlavorText(),
			});
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ItemID.AncientCloth, 10));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Black_Bile>(), 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.DarkShard, 15));
		}
		public override void AI() {
			if (Main.rand.NextBool(800)) SoundEngine.PlaySound(Origins.Sounds.DefiledIdle, NPC.Center);
			NPC.TargetClosest();
			if (NPC.HasPlayerTarget) {
				NPC.FaceTarget();
				NPC.spriteDirection = NPC.direction;
			}
		}
		public override void HitEffect(NPC.HitInfo hit) {
			//spawn gore if npc is dead after being hit
			if (NPC.life < 0) {
				for (int i = 0; i < 3; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/DF3_Gore");
				for (int i = 0; i < 6; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/DF_Effect_Medium" + Main.rand.Next(1, 4));
			}
		}
	}
}