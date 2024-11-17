﻿using Origins.NPCs.Defiled.Boss;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Origins.NPCs.Defiled {
	public class DefiledGlobalNPC : GlobalNPC, IAssimilationProvider {
		public string AssimilationName => "DefiledAssimilation";
		public string AssimilationTexture => "Origins/UI/WorldGen/IconEvilDefiled";
		public static Dictionary<int, AssimilationAmount> AssimilationAmounts { get; private set; } = [];
		public static Dictionary<int, int> NPCTransformations { get; private set; } = [];
		public override void Load() {
			hasErroredAboutWrongNPC = [];
			BiomeNPCGlobals.assimilationProviders.Add(this);
		}
		public override void Unload() {
			AssimilationAmounts = null;
			NPCTransformations = null;
			hasErroredAboutWrongNPC = null;
		}
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation) {
			if (entity.ModNPC is IDefiledEnemy defiledEnemy) {
				if (defiledEnemy.Assimilation is AssimilationAmount amount) {
					if (AssimilationAmounts.TryGetValue(entity.type, out AssimilationAmount assimilationAmount)) {
						if (assimilationAmount != amount) Origins.LogError($"Tried to give entity type {entity.type} ({entity.TypeName}) two different assimilation amounts: {assimilationAmount}, {amount}");
					} else {
						AssimilationAmounts.Add(entity.type, amount);
					}
				}
				return true;
			}
			return false;
		}
		static HashSet<int> hasErroredAboutWrongNPC;
		public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (npc.ModNPC is IDefiledEnemy defiledEnemy) {
				if (npc.life < npc.lifeMax && defiledEnemy.Mana > 0) {
					defiledEnemy.Regenerate(out int lifeRegen);
					if (!npc.HasBuff(BuffID.Bleeding)) npc.lifeRegen += lifeRegen;
				}
			} else {
				if (!hasErroredAboutWrongNPC.Add(npc.type)) {
					Origins.LogError($"something has gone extremely wrong and a non-defiled enemy ({npc.ModNPC}) has a DefiledGlobalNPC");
				}
			}
			if (npc.poisoned) {
				npc.lifeRegen += 6;
			}
			if (npc.onFire) {
				npc.lifeRegen += 4;
			}
			if (npc.onFire2) {// cursed inferno
				npc.lifeRegen += 24;
			}
			if (npc.onFire3) {// hellfire
				npc.lifeRegen += 15;
			}
			if (npc.onFrostBurn) {
				npc.lifeRegen -= 4;
				damage += 1;
			}
			if (npc.onFrostBurn2) {
				npc.lifeRegen -= 16;
				damage += 4;
			}
			/*if (npc.electrified) {
				npc.lifeRegen -= 20;
				damage += 20;
			}
			if (npc.electrified && npc.wet) {
				npc.lifeRegen -= 40;
				damage += 20;
			}*/
			if (npc.shadowFlame) {
				npc.lifeRegen += 15;
			}
			if (npc.oiled && (npc.onFire || npc.onFire2 || npc.onFire3 || npc.shadowFlame)) {
				npc.lifeRegen += 10;
			}
			if (npc.daybreak) {
				npc.lifeRegen += 25 * 2;
				damage -= 5;
			}
			if (npc.javelined) {
				npc.lifeRegen -= 6;
				damage += 3;
			}
			if (npc.dryadBane) {
				const float baseDPS = 2;
				int totalDPS = (int)(baseDPS * BiomeNPCGlobals.CalcDryadDPSMult());
				npc.lifeRegen -= 2 * totalDPS;
				damage += totalDPS / 3;
			}
		}
		public override void OnHitPlayer(NPC npc, Player target, Player.HurtInfo hurtInfo) {
			AssimilationAmount amount = GetAssimilationAmount(npc);
			if (amount != default) {
				target.GetModPlayer<OriginPlayer>().DefiledAssimilation += amount.GetValue(npc, target);
			}
			if (npc.ModNPC is IDefiledEnemy defiledEnemy) {
				defiledEnemy.DrainMana(target);
			}
		}
		public AssimilationAmount GetAssimilationAmount(NPC npc) {
			if (AssimilationAmounts.TryGetValue(npc.type, out AssimilationAmount amount)) {
				return amount;
			} else if (AssimilationAmounts.TryGetValue(0, out amount)) {
				return amount;
			}
			return default;
		}
		public override void OnKill(NPC npc) {
			if (npc.ModNPC is IDefiledEnemy defiledEnemy) {
				defiledEnemy.SpawnWisp(npc);
			}
		}
		public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter) {
			if (npc.ModNPC is IDefiledEnemy defiledEnemy && defiledEnemy.ForceSyncMana) {
				binaryWriter.Write(defiledEnemy.Mana);
			}
		}
		public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader) {
			if (npc.ModNPC is IDefiledEnemy defiledEnemy && defiledEnemy.ForceSyncMana) {
				defiledEnemy.Mana = binaryReader.ReadSingle();
			}
		}
	}
	public interface IDefiledEnemy {
		int MaxManaDrain => 0;
		int MaxMana => 0;
		AssimilationAmount? Assimilation => null;
		float Mana { get; set; }
		bool ForceSyncMana => true;
		void Regenerate(out int lifeRegen) { lifeRegen = 0; }
		void SpawnWisp(NPC npc) {
			if (Main.expertMode) NPC.NewNPC(npc.GetSource_Death(), (int)npc.position.X, (int)npc.position.Y, ModContent.NPCType<Defiled_Wisp>());
		}
		public void DrainMana(Player target) {
			int maxDrain = (int)Math.Min(MaxMana - Mana, MaxManaDrain);
			int manaDrain = Math.Min(maxDrain, target.statMana);
			target.statMana -= manaDrain;
			Mana += manaDrain;
			if (target.manaRegenDelay < 10) target.manaRegenDelay = 10;
		}
	}
}
