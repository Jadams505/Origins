﻿using Microsoft.Xna.Framework;
using Origins.Items.Armor.Defiled;
using Origins.Items.Materials;
using Origins.Items.Weapons.Demolitionist;
using Origins.World.BiomeData;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Defiled {
	public class Defiled_Asphyxiator : ModNPC, IDefiledEnemy {
		public const float speedMult = 0.75f;
		//public float SpeedMult => npc.frame.Y==510?1.6f:0.8f;
		//bool attacking = false;
		public override void SetStaticDefaults() {
			Main.npcFrameCount[NPC.type] = 9;
			NPCID.Sets.NPCBestiaryDrawOffset[Type] = NPCExtensions.BestiaryWalkLeft;
		}
		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.Zombie);
			NPC.aiStyle = NPCAIStyleID.None;
			NPC.lifeMax = 160;
			NPC.defense = 9;
			NPC.damage = 49;
			NPC.width = 92;
			NPC.height = 56;
			NPC.friendly = false;
			NPC.HitSound = Origins.Sounds.DefiledHurt.WithPitchRange(0.5f, 0.75f);
			NPC.DeathSound = Origins.Sounds.DefiledKill.WithPitchRange(0.5f, 0.75f);
			NPC.value = 103;
			NPC.noGravity = true;
			SpawnModBiomes = [
				ModContent.GetInstance<Defiled_Wastelands>().Type
			];
			this.CopyBanner<Defiled_Banner_NPC>();
		}
		public int MaxMana => 100;
		public int MaxManaDrain => 100;
		public float Mana { get; set; }
		public void Regenerate(out int lifeRegen) {
			int factor = 37 / ((NPC.life / 40) + 2);
			lifeRegen = factor;
			Mana -= factor / 90f;// 3 mana for every 2 health regenerated
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			if (spawnInfo.SpawnTileY < Main.worldSurface || spawnInfo.DesertCave) return 0;
			return Defiled_Wastelands.SpawnRates.FlyingEnemyRate(spawnInfo, true) * Defiled_Wastelands.SpawnRates.Brute;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.AddTags(
				this.GetBestiaryFlavorText()
			);
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Strange_String>(), 1, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bombardment>(), 48));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Defiled2_Helmet>(), 525));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Defiled2_Breastplate>(), 525));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Defiled2_Greaves>(), 525));
		}
		public int Frame {
			get => NPC.frame.Y / 58;
			set => NPC.frame.Y = value * 58;
		}
		public override void AI() {
			NPC.TargetClosestUpgraded();
			if (NPC.HasValidTarget && NPC.HasPlayerTarget) {
				Player target = Main.player[NPC.target];
				int level = Defiled_Asphyxiator_Debuff.GetLevel(target);
				int projectileType = ProjectileID.None;
				float speed = 32f;
				float inertia = 128f;
				NPC.rotation = NPC.velocity.X * 0.1f;
				Vector2 vectorToTargetPosition = target.Center - NPC.Center;
				float dist = vectorToTargetPosition.Length();
				vectorToTargetPosition /= dist;
				NPC.aiAction = 0;
				switch (level) {
					case 3: {

					}
					break;
					case 0:
					projectileType = ModContent.ProjectileType<Defiled_Asphyxiator_P2>();
					goto default;
					case 1:
					projectileType = ModContent.ProjectileType<Defiled_Asphyxiator_P3>();
					goto default;
					case 2:
					projectileType = ModContent.ProjectileType<Defiled_Asphyxiator_P1>();
					goto default;
					default: {
						const float hover_range = 16 * 13;
						if (dist < hover_range - 32) {
							speed *= -1;
						} else if (dist < hover_range + 32) {
							speed = 0;
							if (NPC.velocity.LengthSquared() < 0.1f) {
								// If there is a case where it's not moving at all, give it a little "poke"
								NPC.velocity += Main.rand.NextVector2Circular(1, 1) * 0.05f;
							}
						}

						if (++NPC.ai[0] > 90 - 9 * 3) {
							NPC.aiAction = 1;
							if (NPC.ai[0] > 90) {
								NPC.ai[0] = 0;
								if (Main.netMode != NetmodeID.MultiplayerClient) {
									Projectile.NewProjectile(
										NPC.GetSource_FromAI(),
										NPC.Center,
										vectorToTargetPosition * 8,
										projectileType,
										20,
										4
									);
								}
							}
						}
					}
					break;
				}
				vectorToTargetPosition *= speed;
				NPC.velocity = (NPC.velocity * (inertia - 1) + vectorToTargetPosition) / inertia;
				Vector2 nextVel = Collision.TileCollision(NPC.position, NPC.velocity, NPC.width, NPC.height, true, true);
				if (nextVel.X != NPC.velocity.X) NPC.velocity.X *= -0.9f;
				if (nextVel.Y != NPC.velocity.Y) NPC.velocity.Y *= -0.9f;
			}
		}
		public override void FindFrame(int frameHeight) {
			if (++NPC.frameCounter > 9) {
				NPC.frameCounter = 0;
				Frame++;
			}
			switch (NPC.aiAction) {
				case 0:
				if (Frame < 0 || Frame >= 4) {
					Frame = 0;
					NPC.frameCounter = 0;
				}
				break;
				case 1:
				if (Frame < 4 || Frame >= 9) {
					Frame = 4;
					NPC.frameCounter = 0;
				}
				break;
			}
		}
		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
			Rectangle spawnbox = projectile.Hitbox.MoveToWithin(NPC.Hitbox);
			for (int i = Main.rand.Next(3); i-- > 0;) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), Main.rand.NextVectorIn(spawnbox), projectile.velocity, "Gores/NPCs/DF_Effect_Small" + Main.rand.Next(1, 4));
		}
		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
			int halfWidth = NPC.width / 2;
			int baseX = player.direction > 0 ? 0 : halfWidth;
			for (int i = Main.rand.Next(3); i-- > 0;) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(baseX + Main.rand.Next(halfWidth), Main.rand.Next(NPC.height)), hit.GetKnockbackFromHit(), "Gores/NPCs/DF_Effect_Small" + Main.rand.Next(1, 4));
		}
		public override void HitEffect(NPC.HitInfo hit) {
			if (NPC.life < 0) {
				for (int i = 0; i < 6; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/DF3_Gore");
				for (int i = 0; i < 10; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/DF_Effect_Medium" + Main.rand.Next(1, 4));
			}
		}
	}
	public class Defiled_Asphyxiator_P1 : ModProjectile {
		public override void SetDefaults() {
			Projectile.width = Projectile.height = 36;
			Projectile.hostile = true;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info) {
			Defiled_Asphyxiator_Debuff.AddBuff(target);
		}
	}
	public class Defiled_Asphyxiator_P2 : Defiled_Asphyxiator_P1 { }
	public class Defiled_Asphyxiator_P3 : Defiled_Asphyxiator_P1 { }
	public class Defiled_Asphyxiator_Debuff : ModBuff {
		public override string Texture => typeof(Defiled_Asphyxiator_P2).GetDefaultTMLName();
		public static int ID { get; private set; }
		public override void SetStaticDefaults() => ID = Type;
		public static int GetLevel(Player player) => GetLevel(player, out _);
		public static int GetLevel(Player player, out int index) {
			index = player.FindBuffIndex(Defiled_Asphyxiator_Debuff_3.ID);
			if (index != -1) return 3;
			index = player.FindBuffIndex(Defiled_Asphyxiator_Debuff_2.ID);
			if (index != -1) return 2;
			index = player.FindBuffIndex(ID);
			if (index != -1) return 1;
			return 0;
		}
		public static void AddBuff(Player player) {
			const int level_1_time = 120;
			const int level_2_time = 120 + level_1_time;// pointlessly adding and subtracting to show that level 2 will decay into level 1
			const int level_3_time = 150;
			int level = GetLevel(player, out int index);
			if (index != -1) {
				if (level == 1) {
					player.buffType[index] = Defiled_Asphyxiator_Debuff_2.ID;
					player.buffTime[index] = level_3_time;
				} else {
					player.buffType[index] = Defiled_Asphyxiator_Debuff_3.ID;
					player.buffTime[index] = level_2_time - level_1_time;
				}
			} else {
				player.AddBuff(ID, level_1_time);
			}
		}
	}
	public class Defiled_Asphyxiator_Debuff_2 : Defiled_Asphyxiator_Debuff {
		public static new int ID { get; private set; }
		public override void SetStaticDefaults() => ID = Type;
		public override void Update(Player player, ref int buffIndex) {
			if (player.buffTime[buffIndex] == 1) {
				player.buffType[buffIndex] = Defiled_Asphyxiator_Debuff.ID;
				player.buffTime[buffIndex] = 120;
			}
			base.Update(player, ref buffIndex);
		}
	}
	public class Defiled_Asphyxiator_Debuff_3 : Defiled_Asphyxiator_Debuff {
		public static new int ID { get; private set; }
		public override void SetStaticDefaults() => ID = Type;
	}
}
