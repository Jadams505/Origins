﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.World.BiomeData;
using System;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Riven {
	public class Savage_Whip : ModNPC, IRivenEnemy {
		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.CursedHammer);
			NPC.aiStyle = NPCAIStyleID.None;
			NPC.lifeMax = 200;
			NPC.defense = 18;
			NPC.damage = 85;
			NPC.width = 32;
			NPC.height = 32;
			NPC.knockBackResist = 0.6f;
			NPC.value = 1000;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			SpawnModBiomes = [
				ModContent.GetInstance<Underground_Riven_Hive_Biome>().Type
			];
		}
		public int AIState {
			get => (int)NPC.ai[0];
			set => NPC.ai[0] = value;
		}
		public override void AI() {
			const float charge_time = 15f;
			const float swing_time = 25f;
			const float spin_range = 240f;
			DoGlow(NPC.Center);

			if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead) NPC.TargetClosest();
			if (NPC.ai[3] <= 0) {
				SpawnSegment(NPC, 5);
				NPC.netUpdate = true;
			}

			switch (AIState) {
				case 0: {
					Vector2 diff = Main.player[NPC.target].MountedCenter - NPC.Center;
					if (diff.LengthSquared() <= spin_range * spin_range) {
						AIState = 1;
						NPC.ai[1] = 0;
					} else {
						NPC.velocity = Vector2.Lerp(NPC.velocity, diff.SafeNormalize(default) * 8, 0.05f);
					}
					OriginExtensions.AngularSmoothing(ref NPC.rotation, 0, 0.1f);
					NPC.localAI[0] = 0;
					break;
				}
				case 1: {
					Vector2 diff = Main.player[NPC.target].MountedCenter - NPC.Center;
					if (diff.LengthSquared() > spin_range * spin_range) {
						if (NPC.ai[1] > 0) {
							NPC.ai[1]--;
						} else {
							AIState = 0;
							NPC.ai[1] = 0;
						}
					} else {
						if (NPC.justHit) {
							AIState = 1;
							NPC.ai[1] = 0;
						}

						//NPC.velocity *= 0.97f;
						if (NPC.ai[1] < charge_time) NPC.ai[1]++;
						else NPC.ai[1] = charge_time;
						float spinSpeed = NPC.ai[1] / charge_time;
						spinSpeed = 0.05f + spinSpeed * 0.50f;
						NPC.localAI[0] = spinSpeed;
						if (NPC.ai[1] >= charge_time) {
							if (NPC.ai[2] != 0 && Vector2.Dot(diff.SafeNormalize(default), new Vector2(-1, 0).RotatedBy(NPC.rotation + 3f)) > 0.9f) {
								NPC.netUpdate = true;
								AIState = 2;
								NPC.ai[1] = 0;
							}
						}
					}
					break;
				}
				case 2: {
					if (NPC.justHit) {
						AIState = 3;
					}

					NPC.velocity *= 0.96f;
					NPC.ai[1] += 1f;
					float spinSpeed = 0.4f;
					NPC.localAI[0] = spinSpeed;
					if (NPC.ai[1] >= swing_time) {
						NPC.netUpdate = true;
						AIState = 3;
					}
					break;
				}
				case 3: {
					NPC.velocity *= 0.96f;
					float spinSpeed = NPC.ai[1] / swing_time;
					spinSpeed = 0.1f + spinSpeed * 0.35f;
					NPC.localAI[0] = spinSpeed;
					NPC.ai[1] -= 1f;
					if (NPC.ai[1] <= 0) {
						NPC.netUpdate = true;
						AIState = 1;
						NPC.ai[1] = charge_time / 3;
					}
					break;
				}
			}
			NPC.rotation += NPC.localAI[0];
			NPC nextNPC = NPC;
			NPC prevNPC;
			nextNPC.rotation = nextNPC.rotation % MathHelper.TwoPi;
			NPC.ai[2] = 0;
			while (nextNPC.ai[3] >= 0) {
				prevNPC = nextNPC;
				nextNPC = Main.npc[(int)nextNPC.ai[3]];
				if (nextNPC.type != Savage_Whip_Segment.ID) break;
				nextNPC.realLife = NPC.whoAmI;
				nextNPC.Center = prevNPC.Center + new Vector2(0, -32).RotatedBy(prevNPC.rotation);
				switch (AIState) {
					case 0:
					nextNPC.localAI[0] = 0;
					OriginExtensions.AngularSmoothing(ref nextNPC.rotation, prevNPC.rotation - 0.8f, 0.1f);
					break;
					case 1:
					nextNPC.rotation += prevNPC.localAI[0];
					nextNPC.localAI[0] = prevNPC.localAI[0] * 0.9f;
					break;
					case 2:
					nextNPC.rotation += prevNPC.localAI[0];
					nextNPC.localAI[0] = prevNPC.localAI[0] * 1.1f;
					break;
					case 3:
					nextNPC.rotation += prevNPC.localAI[0];
					nextNPC.localAI[0] = prevNPC.localAI[0] * 0.5f;
					break;
				}
				nextNPC.rotation = nextNPC.rotation % MathHelper.TwoPi;
				float diff = Tyfyter.Utils.GeometryUtils.AngleDif(nextNPC.rotation, prevNPC.rotation, out int dir);
				if (diff * dir > 0.8f) {
					nextNPC.rotation += diff - dir * 0.8f;
					NPC.ai[2] = 1;
				}
			}
			NPCID.Sets.TrailingMode[Type] = 0;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				this.GetBestiaryFlavorText(),
			});
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.StatusImmunityItem(ItemID.Nazar, 100));
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			spriteBatch.Draw(
				Terraria.GameContent.TextureAssets.Npc[Type].Value,
				NPC.Center - screenPos,
				new Rectangle(12, 0, 24, 36),
				drawColor,
				NPC.rotation,
				new Vector2(12, 18),
				1,
				SpriteEffects.None,
			0);
			return false;
		}
		public static void SpawnSegment(NPC npc, int depthLeft) {
			NPC nextNPC = NPC.NewNPCDirect(
				npc.GetSource_FromThis(),
				npc.Center,
				Savage_Whip_Segment.ID,
				npc.whoAmI
			);
			nextNPC.realLife = npc.whoAmI;
			npc.ai[3] = nextNPC.whoAmI;
			nextNPC.ai[2] = depthLeft == 0 ? 3 : Main.rand.Next(3);
			if (depthLeft > 0) SpawnSegment(nextNPC, depthLeft - 1);
		}
		public static void DoGlow(Vector2 position) {
			Lighting.AddLight(position, new Vector3(0.394f, 0.879f, 0.912f) * Riven_Hive.NormalGlowValue.GetValue());
		}
	}
	public class Savage_Whip_Segment : Savage_Whip {
		public override string Texture => "Origins/NPCs/Riven/Savage_Whip";
		public NPC ParentNPC => Main.npc[NPC.realLife];
		public static int ID { get; private set; }
		public override void SetStaticDefaults() {
			ID = Type;
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new() {
				Hide = true
			});
		}
		public override void ResetEffects() {
			NPC.dontTakeDamage = true;
		}
		public override bool PreAI() {
			NPC.dontTakeDamage = false;
			DoGlow(NPC.Center);
			NPC parentNPC = ParentNPC;
			if (!parentNPC.active) {
				NPC.active = false;
				return false;
			}
			NPC.life = NPC.lifeMax;
			for (int i = 0; i < NPC.maxBuffs; i++) {
				if (parentNPC.buffTime[i] > 0) {
					NPC.buffType[i] = parentNPC.buffType[i];
					NPC.buffTime[i] = parentNPC.buffTime[i];
				}
			}
			return false;
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) { }
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) { }
		public override bool? CanBeHitByProjectile(Projectile projectile) {
			if (projectile.usesLocalNPCImmunity) {
				if (projectile.localNPCImmunity[NPC.realLife] == 0) return null;
				return false;
			}
			if (projectile.usesIDStaticNPCImmunity) {
				if (Projectile.perIDStaticNPCImmunity[projectile.type][NPC.realLife] < Main.GameUpdateCount) return null;
				return false;
			}
			if (projectile.penetrate != 1) {
				if (ParentNPC.immune[projectile.owner] <= 0) return null;
				return false;
			}
			return null;
		}
		public override bool? CanBeHitByItem(Player player, Item item) {
			if (ParentNPC.immune[player.whoAmI] <= 0) return null;
			return false;
		}
		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
			if (projectile.usesLocalNPCImmunity) {
				projectile.localNPCImmunity[NPC.realLife] = projectile.localNPCImmunity[NPC.whoAmI];
			} else if (projectile.usesIDStaticNPCImmunity) {
				Projectile.perIDStaticNPCImmunity[projectile.type][NPC.realLife] = Projectile.perIDStaticNPCImmunity[projectile.type][NPC.whoAmI];
			} else if (projectile.penetrate != 1) {
				ParentNPC.immune[projectile.owner] = NPC.immune[projectile.owner];
			}
		}
		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
			ParentNPC.immune[player.whoAmI] = NPC.immune[player.whoAmI];
		}
		public override void HitEffect(NPC.HitInfo hit) {
			//NPC.life = NPC.lifeMax;
			NPC parentNPC = ParentNPC;
			for (int i = 0; i < NPC.maxBuffs; i++) {
				if (NPC.buffTime[i] > 0) {
					parentNPC.buffType[i] = NPC.buffType[i];
					parentNPC.buffTime[i] = NPC.buffTime[i];
				}
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			Rectangle frame = new Rectangle(14, 38, 26, 28);
			switch ((int)NPC.ai[2]) {
				case 1:
				frame = new Rectangle(14, 68, 26, 24);
				break;
				case 2:
				frame = new Rectangle(14, 94, 26, 26);
				break;
				case 3:
				frame = new Rectangle(0, 122, 48, 26);
				break;
			}
			spriteBatch.Draw(
				Terraria.GameContent.TextureAssets.Npc[Type].Value,
				NPC.Center - screenPos,
				frame,
				drawColor,
				NPC.rotation,
				frame.Size() * 0.5f,
				1,
				SpriteEffects.None,
			0);
			return false;
		}
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
	}
}
