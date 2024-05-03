﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Origins.World.BiomeData;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Riven {
    public class Spider_Amoeba : Glowing_Mod_NPC, IRivenEnemy {
		public override void SetStaticDefaults() {
			Main.npcFrameCount[NPC.type] = 5;
		}
		public override void SetDefaults() {// could not add stats because 
			NPC.CloneDefaults(NPCID.Zombie);
			NPC.aiStyle = NPCAIStyleID.Fighter;
			NPC.width = 68;
			NPC.height = 30;
			SetSharedDefaults();
		}
		public void SetSharedDefaults() {
			NPC.lifeMax = 81;
			NPC.defense = 10;
			NPC.damage = 33;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit13;
			NPC.DeathSound = SoundID.NPCDeath24.WithPitch(0.6f);
			NPC.value = 90;
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return spawnInfo.SpawnTileY < Main.worldSurface ? 0 : Riven_Hive.SpawnRates.LandEnemyRate(spawnInfo) * Riven_Hive.SpawnRates.Spighter;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				this.GetBestiaryFlavorText(),
			});
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bud_Barnacle>(), 1, 1, 3));
		}
		public override void AI() {
			NPC.TargetClosest();
			if (NPC.HasPlayerTarget) {
				NPC.FaceTarget();
				NPC.spriteDirection = NPC.direction;
			}
			//increment frameCounter every frame and run the following code when it exceeds 7 (i.e. run the following code every 8 frames)
			
			if (NPC.collideY) {
				NPC.DoFrames(7);
				if (NPC.collideX) NPC.velocity.Y = -6f;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			if (NPC.velocity.Y == 0f && NPC.NPCCanStickToWalls()) {
				NPC.Transform(ModContent.NPCType<Spider_Amoeba_Wall>());
			}
		}
		public override void HitEffect(NPC.HitInfo hit) {
			//spawn gore if npc is dead after being hit
			if (NPC.life < 0) {
				for (int i = 0; i < 3; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/R_Effect_Blood" + Main.rand.Next(1, 4));
				Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/R_Effect_Meat" + Main.rand.Next(1, 4));
			}
		}
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
            OriginPlayer.InflictTorn(target, 180, targetSeverity: 1f - 0.85f);
        }
    }public class Spider_Amoeba_Wall : Spider_Amoeba {
		public override void SetStaticDefaults() {
			Main.npcFrameCount[NPC.type] = 4;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
				Hide = true
			});
		}
		public override void SetDefaults() {// could not add stats because 
			NPC.CloneDefaults(NPCID.WallCreeperWall);
			NPC.aiStyle = NPCAIStyleID.Spider;
			NPC.width = 68;
			NPC.height = 68;
			SetSharedDefaults();
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo) => 0;
		public override void AI() {
			NPC.DoFrames(7);
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			if (!NPC.NPCCanStickToWalls()) {
				NPC.Transform(ModContent.NPCType<Spider_Amoeba>());
			}
		}
	}
}
