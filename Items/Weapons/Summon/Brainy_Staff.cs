﻿using Microsoft.Xna.Framework;
using Origins.Buffs;
using Origins.Items.Weapons.Summon;
using Origins.Items.Weapons.Summon.Minions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Origins.OriginExtensions;

namespace Origins.Items.Weapons.Summon {
    public class Brainy_Staff : ModItem {
        internal static int projectileID = 0;
        internal static int buffID = 0;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Brainy Staff");
            Tooltip.SetDefault("Summons a mini Brain of Cthulhu to fight for you");
        }
        public override void SetDefaults() {
            item.damage = 12;
            item.mana = 10;
            item.width = 32;
            item.height = 32;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = ItemRarityID.Blue;
            item.UseSound = SoundID.Item44;
            item.buffType = buffID;
            item.shoot = projectileID;
            item.noMelee = true;
            item.summon = true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
            if(buffID==0)buffID = ModContent.BuffType<Brainy_Buff>();
            player.AddBuff(item.buffType, 2);
            position = Main.MouseWorld;
            return true;
        }
    }
}
namespace Origins.Buffs {
    public class Brainy_Buff : ModBuff {
        public override void SetDefaults() {
            DisplayName.SetDefault("Mini Brain of Cthulhu");
            Description.SetDefault("The Brain of Cthulhu will fight for you");
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Brainy_Staff.buffID = Type;
        }

        public override void Update(Player player, ref int buffIndex) {
            if(player.ownedProjectileCounts[Brainy_Staff.projectileID] > 0) {
                player.buffTime[buffIndex] = 18000;
            } else {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
namespace Origins.Items.Weapons.Summon.Minions {
    public class Mini_BOC : ModProjectile {
		public const int frameSpeed = 5;
		public override void SetStaticDefaults() {
            Brainy_Staff.projectileID = projectile.type;
			DisplayName.SetDefault("Brainy");
			// Sets the amount of frames this minion has on its spritesheet
			Main.projFrames[projectile.type] = 4;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;

			// These below are needed for a minion
			// Denotes that this projectile is a pet or minion
			Main.projPet[projectile.type] = true;
			// This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.Homing[projectile.type] = true;
		}

		public sealed override void SetDefaults() {
			projectile.width = 46;
			projectile.height = 30;
			projectile.tileCollide = true;
			projectile.friendly = true;
			projectile.minion = true;
			projectile.minionSlots = 1f;
			projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
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
			Player player = Main.player[projectile.owner];

			#region Active check
			// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
			if (player.dead || !player.active) {
				player.ClearBuff(Brainy_Staff.buffID);
			}
			if (player.HasBuff(Brainy_Staff.buffID)) {
				projectile.timeLeft = 2;
			}
			#endregion

			#region General behavior
			Vector2 idlePosition = player.Top;
            idlePosition.X -= 48f*player.direction;


			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			Vector2 vectorToIdlePosition = idlePosition - projectile.Center;
			float distanceToIdlePosition = vectorToIdlePosition.Length();
			if (Main.myPlayer == player.whoAmI && distanceToIdlePosition > 2000f) {
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				projectile.position = idlePosition;
				projectile.velocity *= 0.1f;
				projectile.netUpdate = true;
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;
			for (int i = 0; i < Main.maxProjectiles; i++) {
				// Fix overlap with other minions
				Projectile other = Main.projectile[i];
				if (i != projectile.whoAmI && other.active && other.owner == projectile.owner && Math.Abs(projectile.position.X - other.position.X) + Math.Abs(projectile.position.Y - other.position.Y) < projectile.width) {
					if (projectile.position.X < other.position.X) projectile.velocity.X -= overlapVelocity;
					else projectile.velocity.X += overlapVelocity;

					if (projectile.position.Y < other.position.Y) projectile.velocity.Y -= overlapVelocity;
					else projectile.velocity.Y += overlapVelocity;
				}
			}
			#endregion

			#region Find target
			// Starting search distance
			float distanceFromTarget = 700f;
			Vector2 targetCenter = projectile.position;
            int target = -1;
			bool foundTarget = false;

			if (player.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, projectile.Center);
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
                    target = player.MinionAttackTargetNPC;
					foundTarget = true;
				}
			}
			if (!foundTarget) {
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, projectile.Center);
						bool closest = Vector2.Distance(projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;
						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.height/(float)npc.width>1?npc.Top+new Vector2(0,8):npc.Center;
                            target = npc.whoAmI;
							foundTarget = true;
						}
					}
				}
			}

			projectile.friendly = foundTarget;
			#endregion

			#region Movement

			// Default movement parameters (here for attacking)
			float speed = 16f;
			float inertia = 12f;

			if (foundTarget) {
                projectile.tileCollide = true;
				// Minion has a target: attack (here, fly towards the enemy)
				//if (distanceFromTarget > 40f || !projectile.Hitbox.Intersects(Main.npc[target].Hitbox)) {
					// The immediate range around the target (so it doesn't latch onto it when close)
					Vector2 direction = targetCenter - projectile.Center;
					direction.Normalize();
					direction *= speed;
					projectile.velocity = (projectile.velocity * (inertia - 1) + direction) / inertia;
				//}
			} else {
                projectile.tileCollide = false;
				if (distanceToIdlePosition > 600f) {
					speed = 24f;
					inertia = 36f;
				} else {
					speed = 6f;
					inertia = 48f;
				}
				if (distanceToIdlePosition > 12f) {
					// The immediate range around the player (when it passively floats about)

					// This is a simple movement formula using the two parameters and its desired direction to create a "homing" movement
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					projectile.velocity = (projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				} else if (projectile.velocity == Vector2.Zero) {
					// If there is a case where it's not moving at all, give it a little "poke"
					projectile.velocity.X = -0.15f;
					projectile.velocity.Y = -0.05f;
				}
			}
			#endregion

			#region Animation and visuals
			// So it will lean slightly towards the direction it's moving
			projectile.rotation = projectile.velocity.X * 0.05f;

			// This is a simple "loop through all frames from top to bottom" animation
			int frameSpeed = 5;
			projectile.frameCounter++;
			if (projectile.frameCounter >= frameSpeed) {
				projectile.frameCounter = 0;
				projectile.frame++;
				if (projectile.frame >= Main.projFrames[projectile.type]) {
					projectile.frame = 0;
				}
			}
			#endregion
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            if(Main.rand.Next(10)<3&&(target.Center-Main.player[projectile.owner].Center).Length()<480) {
                target.AddBuff(BuffID.Confused, 60);
            }
        }
    }
}
