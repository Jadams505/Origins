﻿using CalamityMod.Items.Weapons.Magic;
using CalamityMod.NPCs.ExoMechs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Items.Accessories;
using Origins.Items.Armor.Defiled;
using Origins.Items.Materials;
using Origins.Items.Other.Consumables.Food;
using Origins.Items.Weapons.Magic;
using Origins.Misc;
using Origins.NPCs.Defiled;
using Origins.World.BiomeData;
using PegasusLib;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Brine {
	public class Carpalfish : Brine_Pool_NPC {
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			Main.npcFrameCount[NPC.type] = 16;
			NPCID.Sets.NPCBestiaryDrawOffset[Type] = new NPCID.Sets.NPCBestiaryDrawModifiers() {
				Velocity = 1f
			};
		}
		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.Vulture);
			NPC.aiStyle = -1;
			NPC.lifeMax = 358;
			NPC.defense = 26;
			NPC.damage = 65;
			NPC.width = 30;
			NPC.height = 30;
			NPC.catchItem = 0;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit19;
			NPC.DeathSound = SoundID.NPCDeath22;
			NPC.knockBackResist = 0.65f;
			NPC.value = 500;
			NPC.noGravity = true;
			SpawnModBiomes = [
				ModContent.GetInstance<Brine_Pool>().Type
			];
		}
		public override float SpawnChance(NPCSpawnInfo spawnInfo) {
			return Brine_Pool.SpawnRates.EnemyRate(spawnInfo, Brine_Pool.SpawnRates.Carpalfish);
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.AddTags([
				this.GetBestiaryFlavorText()
			]);
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ModContent.ItemType<Alkaliphiliac_Tissue>(), 1, 3, 7));
		}
		public override void AI() {
			if (NPC.ai[2] < 0) {
				NPC.ai[2]++;
				return;
			}
			const int charge_start_delay = 60;
			const int charge_startup_time = 45;
			const int charge_duration = 35;
			const int charge_end_duration = 10;
			DoTargeting();
			Vector2 direction;
			bool canCharge = false;
			if (NPC.wet) {
				NPC.noGravity = true;
				if (TargetPos != default) {
					direction = NPC.DirectionTo(TargetPos);
					float dir = Math.Abs(direction.X) * 0.5f;
					direction.Y = MathHelper.Clamp(direction.Y, -dir, dir);
					direction = direction.SafeNormalize(default);
					float turnSpeed = 0.2f;
					if (NPC.ai[2] >= charge_start_delay + charge_startup_time) {
						turnSpeed = 0;
					} else if (NPC.ai[2] >= charge_start_delay) {
						turnSpeed = 0.05f;
					} else if (Math.Sign(Math.Cos(NPC.rotation)) != Math.Sign(direction.X)) {
						turnSpeed = MathHelper.Pi;
					}
					;
					if (GeometryUtils.AngularSmoothing(ref NPC.rotation, direction.ToRotation(), turnSpeed) && Collision.CanHitLine(NPC.position, 30, 30, TargetPos - Vector2.One * 15, 30, 30)) {
						canCharge = true;
					}
				} else {
					if (NPC.collideX) NPC.velocity.X = -NPC.direction;
					NPC.direction = Math.Sign(NPC.velocity.X);
					if (NPC.direction == 0) NPC.direction = 1;
					direction = Vector2.UnitX * NPC.direction;
					GeometryUtils.AngularSmoothing(ref NPC.rotation, MathHelper.PiOver2 - NPC.direction * MathHelper.PiOver2, 0.1f);
				}
				bool friction = true;
				if (TargetPos != default || NPC.ai[2] >= charge_start_delay + charge_startup_time) {
					if (++NPC.ai[2] >= charge_start_delay + charge_startup_time + charge_duration + charge_end_duration) NPC.ai[2] = 0;
					if (NPC.ai[2] >= charge_start_delay + charge_startup_time) {
						if (NPC.ai[2] == charge_start_delay + charge_startup_time) NPC.velocity += direction * 12;
						if (NPC.ai[2] < charge_start_delay + charge_startup_time + charge_duration) NPC.velocity += NPC.velocity.WithMaxLength(0.2f);
						NPC.frameCounter++;
						friction = false;
					} else if (NPC.ai[2] >= charge_start_delay) {
						if (!canCharge) NPC.ai[2] = 0;
						NPC.velocity += direction * 0.05f;
					} else {
						if (!canCharge) NPC.ai[2] = 0;
						NPC.velocity += direction * 0.2f;
					}
				} else {
					NPC.velocity += direction * 0.2f;
					NPC.ai[2] = 0;
				}
				if (friction) NPC.velocity *= 0.96f;
				if (NPC.ai[2] >= charge_start_delay + charge_startup_time) {
					Vector2 velocity = NPC.velocity;
					Vector4 slopeCollision = Collision.SlopeCollision(NPC.position, velocity, NPC.width, NPC.height);
					velocity = slopeCollision.ZW();
					velocity = Collision.TileCollision(NPC.position, velocity, NPC.width, NPC.height);
					if (velocity != NPC.velocity) {
						NPC.ai[2] = charge_start_delay * -0.75f;
						bool sounded = false;
						foreach (Point tile in Collision.GetTilesIn(NPC.position + NPC.velocity, NPC.BottomRight + NPC.velocity)) {
							if (Framing.GetTileSafely(tile).HasFullSolidTile()) {
								WorldGen.KillTile(tile.X, tile.Y, fail: true, effectOnly: true);
								if (!sounded) {
									sounded = true;
									SoundEngine.PlaySound(SoundID.Dig.WithPitchOffset(-0.2f), NPC.Center);
								}
							}
						}
						NPC.velocity = NPC.velocity * -0.5f;
						if (Main.netMode != NetmodeID.MultiplayerClient) {
							if (NPC.ai[3] == 0) {
								NPC.ai[3] = 1;
								int item = Item.NewItem(
									NPC.GetSource_OnHurt(null),
									NPC.Center,
									ModContent.ItemType<Venom_Fang>()
								);
								if (Main.netMode == NetmodeID.MultiplayerClient) {
									NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item, 1f);
								}
							}
							NPC.HitInfo hitInfo = NPC.GetIncomingStrikeModifiers(DamageClass.Default, 0).ToHitInfo(20, true, 0);
							NPC.StrikeNPC(hitInfo, noPlayerInteraction: true);
							if (Main.netMode != NetmodeID.SinglePlayer) NetMessage.SendStrikeNPC(NPC, hitInfo);
						}
					}
				} else if (Collision.WetCollision(NPC.position, 30, 30)) {
					while (NPC.velocity.Y < 0 && !Collision.WetCollision(NPC.position + NPC.velocity, 30, 15)) {
						NPC.velocity.Y += 1;
					}
				}
			} else {
				NPC.noGravity = false;
				NPC.rotation = NPC.velocity.ToRotation();
				//NPC.rotation += 0.01f;
			}
			NPC.spriteDirection = Math.Sign(Math.Cos(NPC.rotation));
		}
		public override bool? CanFallThroughPlatforms() => true;
		public override void FindFrame(int frameHeight) {
			float frame = ((int)++NPC.frameCounter) / 60;
			if (NPC.frameCounter >= 60) {
				frame = 0;
				NPC.frameCounter = 0;
			}
			NPC.frame = new Rectangle(0, 30 * (int)(frame * Main.npcFrameCount[Type]), 80, 30);
		}
		public override void HitEffect(NPC.HitInfo hit) {
			if (NPC.life < 0) {
				for (int i = 0; i < 2; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/DF3_Gore");
				for (int i = 0; i < 6; i++) Origins.instance.SpawnGoreByName(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, "Gores/NPCs/DF_Effect_Medium" + Main.rand.Next(1, 4));
			}
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
			if (NPC.spriteDirection == -1) {
				spriteEffects |= SpriteEffects.FlipVertically;
			}
			Texture2D texture = TextureAssets.Npc[Type].Value;
			Vector2 halfSize = new(texture.Width * 0.5f, texture.Height / Main.npcFrameCount[NPC.type] / 2);
			Vector2 position = new(NPC.position.X - screenPos.X + (NPC.width / 2) - texture.Width * NPC.scale / 2f + halfSize.X * NPC.scale, NPC.position.Y - screenPos.Y + NPC.height - texture.Height * NPC.scale / Main.npcFrameCount[NPC.type] + 4f + halfSize.Y * NPC.scale + NPC.gfxOffY);
			Vector2 origin = new(halfSize.X * 1.6f, halfSize.Y);
			spriteBatch.Draw(
				TextureAssets.Npc[Type].Value,
				position,
				NPC.frame,
				drawColor,
				NPC.rotation,
				origin,
				NPC.scale,
				spriteEffects,
			0);
			return false;
		}
	}
}
