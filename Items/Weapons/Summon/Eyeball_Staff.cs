﻿using Microsoft.Xna.Framework;
using Origins.Buffs;
using Origins.Items.Weapons.Summon;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using static Origins.OriginExtensions;

namespace Origins.Items.Weapons.Summon {
    public class Eyeball_Staff : ModItem {
        internal static int projectileID = 0;
        internal static int buffID = 0;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Eyeball Staff");
            Tooltip.SetDefault("Summons a mini Eye of Cthulhu to fight for you\nCan summon 2 minions per slot");
            ItemID.Sets.StaffMinionSlotsRequired[Item.type] = 1;
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.damage = 8;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item44;
            Item.buffType = buffID;
            Item.shoot = projectileID;
            Item.noMelee = true;
        }
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
		    if(buffID==0)buffID = ModContent.BuffType<Mini_EOC_Buff>();
            player.AddBuff(Item.buffType, 2);
            position = Main.MouseWorld;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (buffID == 0) buffID = ModContent.BuffType<Wormy_Buff>();
            player.AddBuff(buffID, 2);
            position = Main.MouseWorld;
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
            return false;
        }
    }
}
namespace Origins.Buffs {
    public class Mini_EOC_Buff : ModBuff {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Mini Eye of Cthulhu");
            Description.SetDefault("The Eye of Cthulhu will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Eyeball_Staff.buffID = Type;
        }

        public override void Update(Player player, ref int buffIndex) {
            if(player.ownedProjectileCounts[Eyeball_Staff.projectileID] > 0) {
                player.buffTime[buffIndex] = 18000;
            } else {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}

namespace Origins.Items.Weapons.Summon.Minions {
    public class Mini_EOC : ModProjectile {
		public const int frameSpeed = 5;
		public override void SetStaticDefaults() {
            Eyeball_Staff.projectileID = Projectile.type;
			DisplayName.SetDefault("Mini Eye of Cthulhu");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[Projectile.type] = 4;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[Projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults() {
			Projectile.width = 28;
			Projectile.height = 28;
			Projectile.tileCollide = true;
			Projectile.friendly = true;
			Projectile.minion = true;
			Projectile.minionSlots = 0f;
			Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active) {
				player.ClearBuff(Eyeball_Staff.buffID);
			}
			if (player.HasBuff(Eyeball_Staff.buffID)) {
				Projectile.timeLeft = 2;
			}
            OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
            originPlayer.minionSubSlots[0]+=0.5f;
            int eyeCount = player.ownedProjectileCounts[Eyeball_Staff.projectileID]/2;
            if(originPlayer.minionSubSlots[0]<=eyeCount) {
                Projectile.minionSlots = 0.5f;
            } else {
                Projectile.minionSlots = 0;
            }
            #endregion

            #region General behavior
            Vector2 idlePosition = player.Top+new Vector2(player.direction*-player.width/2, 0);
            idlePosition.X -= 48f*player.direction;

			// Teleport to player if distance is too big
			Vector2 vectorToIdlePosition = idlePosition - Projectile.Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();
			if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f) {
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;
			for (int i = 0; i < Main.maxProjectiles; i++) {
				// Fix overlap with other minions
				Projectile other = Main.projectile[i];
				if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) Projectile.velocity.X -= overlapVelocity;
					else Projectile.velocity.X += overlapVelocity;

					if (Projectile.position.Y < other.position.Y) Projectile.velocity.Y -= overlapVelocity;
					else Projectile.velocity.Y += overlapVelocity;
				}
			}
            #endregion

            #region Find target
			// Starting search distance
			float targetDist = 700f;
			float targetAngle = -2;
			Vector2 targetCenter = Projectile.Center;
            int target = -1;
			bool foundTarget = false;

			if (player.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float dist = Vector2.Distance(npc.Center, Projectile.Center);
				if (dist < 2000f) {
					targetDist = dist;
					targetCenter = npc.Center;
                    target = player.MinionAttackTargetNPC;
					foundTarget = true;
				}
			}
            if(Projectile.ai[1]<0) goto movement;
			if (!foundTarget) {
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy()) {
                        Vector2 diff = Projectile.Center-Projectile.Center;
                        float dist = diff.Length();
						if(dist>targetDist)continue;
						float dot = NormDot(diff,Projectile.velocity);
						bool inRange = dist <= targetDist;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        if (((dot>=targetAngle && inRange) || !foundTarget) && lineOfSight) {
                            targetDist = dist;
                            targetAngle = dot;
							targetCenter = npc.Center;
                            target = npc.whoAmI;
							foundTarget = true;
						}
					}
				}
			}

			Projectile.friendly = foundTarget;
            #endregion

            #region Movement
            movement:
            // Default movement parameters (here for attacking)
            float speed = 6f+Projectile.localAI[0]/15;
            float turnSpeed = 1f+Math.Max((Projectile.localAI[0]-15)/30,0);
			float currentSpeed = Projectile.velocity.Length();
            Projectile.tileCollide = true;
            if(foundTarget) {
                Projectile.tileCollide = true;
                if(Projectile.ai[0] != target) {
                    Projectile.ai[0] = target;
                    Projectile.ai[1] = 0;
                } else {
                    if(++Projectile.ai[1]>180) {
                        Projectile.ai[1] = -30;
                    }
                }
                if((int)Math.Ceiling(targetAngle)==-1) {
                    targetCenter.Y-=16;
                }
            } else {
				if (distanceToIdlePosition > 640f) {
                    Projectile.tileCollide = false;
					speed = 16f;
				} else if (distanceToIdlePosition < 64f) {
					speed = 4f;
                    turnSpeed = 0;
				} else {
					speed = 6f;
				}
                if(!Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, idlePosition, 1, 1)) {
                    Projectile.tileCollide = false;
                }
            }
            LinearSmoothing(ref currentSpeed, speed, currentSpeed<1?1:0.1f+Projectile.localAI[0]/60f);
            Vector2 direction = foundTarget?targetCenter - Projectile.Center:vectorToIdlePosition;
			direction.Normalize();
            Projectile.velocity = Vector2.Normalize(Projectile.velocity+direction*turnSpeed)*currentSpeed;
            #endregion

            #region Animation and visuals
            // So it will lean slightly towards the direction it's moving
            Projectile.rotation = (float)Math.Atan(Projectile.velocity.Y/Projectile.velocity.X);
            Projectile.spriteDirection = Math.Sign(Projectile.velocity.X);

			// This is a simple "loop through all frames from top to bottom" animation
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= frameSpeed) {
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					Projectile.frame = 0;
				}
			}
            #endregion
            if(Projectile.localAI[0]>0)Projectile.localAI[0]--;
		}
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
            damage+=(int)(Projectile.localAI[0]/6);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            Vector2 intersect = Rectangle.Intersect(Projectile.Hitbox,target.Hitbox).Center.ToVector2()-Projectile.Hitbox.Center.ToVector2();
            if(intersect.X!=0&&(Math.Sign(intersect.X)==Math.Sign(Projectile.velocity.X))) {
                Projectile.velocity.X = -Projectile.velocity.X;
            }
            if(intersect.Y!=0&&(Math.Sign(intersect.Y)==Math.Sign(Projectile.velocity.Y))) {
                Projectile.velocity.Y = -Projectile.velocity.Y;
            }
            Projectile.localAI[0]+=20-Projectile.localAI[0]/6;
            Projectile.ai[1] = 0;
        }
    }
}
