﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using static Origins.OriginExtensions;
using Terraria.GameContent.ItemDropRules;

namespace Origins.NPCs.Defiled {
    public class Defiled_Tripod : ModNPC {
        public const float horizontalSpeed = 3.2f;
        public const float horizontalAirSpeed = 2f;
        public const float verticalSpeed = 4f;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Defiled Tripod");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.aiStyle = NPCAIStyleID.None;//NPCAIStyleID.Fighter;
            NPC.lifeMax = 475;
            NPC.defense = 28;
            NPC.damage = 52;
            NPC.width = 98;
            NPC.height = 98;
            NPC.scale = 0.85f;
            NPC.friendly = false;
        }
        public override void AI() {
            NPC.TargetClosest();
            if (NPC.HasPlayerTarget) {
                NPC.FaceTarget();
                NPC.spriteDirection = NPC.direction;
            }

            NPC.velocity = NPC.oldVelocity * new Vector2(NPC.collideX?0.5f:1, NPC.collideY?0.75f:1);
            int targetVMinDirection = Math.Sign(NPC.targetRect.Bottom - NPC.Bottom.Y);
            int targetVMaxDirection = Math.Sign(NPC.targetRect.Top - NPC.Bottom.Y);
            if(NPC.collideY&&targetVMaxDirection==1) {
                NPC.position.Y++;
            }
            float moveDir = Math.Sign(NPC.velocity.X);
            float absX = moveDir==0?0:NPC.velocity.X / moveDir;

            if(NPC.collideY) {
                NPC.ai[1] = 0;
                //npc.rotation = 0;
                LinearSmoothing(ref NPC.rotation, 0, 0.15f);
                if(moveDir != -NPC.direction) {
                    if(absX < horizontalSpeed) {
                        NPC.velocity.X += NPC.direction * 0.5f;
                    } else {
                        LinearSmoothing(ref NPC.velocity.X, NPC.direction*horizontalSpeed, 0.1f);
                    }
                } else {
                    NPC.velocity.X += NPC.direction * 0.15f;
                }
                if(NPC.ai[0] > 0) {
                    NPC.ai[0]--;
                } else {
                    if(NPC.collideX) {
                        if(NPC.targetRect.Bottom > NPC.Top.Y) {
                            //npc.velocity.X *= 2;
                            NPC.position.X += NPC.direction;
                        } else {
                            if(NPC.velocity.Y > -4) {
                                NPC.velocity.Y -= 1;
                            }
                            if(moveDir != -NPC.direction) {
                                LinearSmoothing(ref NPC.rotation, -MathHelper.PiOver2 * moveDir, 0.15f);
                                //npc.rotation = -MathHelper.PiOver2 * moveDir;
                                if(targetVMinDirection==-1) {
                                    NPC.position.Y--;
                                }
                            }
                        }
                    } else if(moveDir == NPC.direction && absX > 3) {
                        float dist = NPC.targetRect.Distance(NPC.Center);
                        if(dist > 96 && dist < 240) {
                            NPC.velocity.X += moveDir * 4;
                            NPC.velocity.Y -= (NPC.Center.Y - NPC.targetRect.Center.Y > 80) ? 8 : 4f;
                            NPC.ai[0] = 35;
                        }
                    }
                }
            } else if(NPC.collideX) {
                NPC.ai[1] = 0;
                if(targetVMinDirection==-1) {
                    if(NPC.velocity.Y > -verticalSpeed)NPC.velocity.Y -= 1;
                    if(moveDir != -NPC.direction) {
                        LinearSmoothing(ref NPC.rotation, -MathHelper.PiOver2 * moveDir, 0.15f);
                        //npc.rotation = -MathHelper.PiOver2 * moveDir;
                    }
                } else {
                    //npc.velocity.X *= 2;
                    NPC.position.X += NPC.direction;
                }
            } else {
                if(++NPC.ai[1]>1)LinearSmoothing(ref NPC.rotation, 0, 0.15f);
                if(moveDir != -NPC.direction) {
                    if(absX<horizontalAirSpeed)NPC.velocity.X += NPC.direction*0.2f;
                }
            }

			if(NPC.velocity.RotatedBy(-NPC.rotation).X*NPC.direction>0.5f&&++NPC.frameCounter>6) {
				//add frame height to frame y position and modulo by frame height multiplied by walking frame count
				NPC.frame = new Rectangle(0, (NPC.frame.Y+100)%400, 98, 98);
				NPC.frameCounter = 0;
				if (NPC.collideY) {
                    Vector2 stepPos = new Vector2(NPC.spriteDirection * -45, 50).RotatedBy(NPC.rotation) + NPC.Center;
                    SoundEngine.PlaySound(SoundID.MenuTick, (int)stepPos.X, (int)stepPos.Y, volumeScale: Main.rand.NextFloat(0.7f, 0.95f), pitchOffset: Main.rand.NextFloat(-0.2f, 0.2f));
				}
			}
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit) {
            Rectangle spawnbox = projectile.Hitbox.MoveToWithin(NPC.Hitbox);
            for(int i = Main.rand.Next(3); i-->0;)Gore.NewGore(Main.rand.NextVectorIn(spawnbox), projectile.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit) {
            int halfWidth = NPC.width / 2;
            int baseX = player.direction > 0 ? halfWidth : 0;
            for(int i = Main.rand.Next(3); i-->0;)Gore.NewGore(NPC.position+new Vector2(baseX + Main.rand.Next(halfWidth),Main.rand.Next(NPC.height)), new Vector2(knockback*player.direction, -0.1f*knockback), Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
        }
        public override void HitEffect(int hitDirection, double damage) {
            if(NPC.life<0) {
                for(int i = 0; i < 3; i++)Gore.NewGore(NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF3_Gore"));
                for(int i = 0; i < 6; i++)Gore.NewGore(NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Medium"+Main.rand.Next(1,4)));
            }
        }
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
            npcLoot.Add(ItemDropRule.StatusImmunityItem(ItemID.Vitamins, 100));
		}
	}
}
