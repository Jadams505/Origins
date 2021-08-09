﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Origins.Projectiles;
using Microsoft.Xna.Framework.Graphics;

namespace Origins.Items.Weapons.Other {
    public class Void_Spectral : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Void Spectral");
            Tooltip.SetDefault("");
        }
        public override void SetDefaults() {
            item.CloneDefaults(ItemID.ToxicFlask);
            item.damage = 85;
            item.crit = 5;
            item.knockBack *= 0.5f;
            item.useAnimation = item.useTime = 27;
            item.width = 56;
            item.height = 46;
            item.shoot = ModContent.ProjectileType<Void_Spectral_P>();
            item.shootSpeed *= 1.5f;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
            Vector2 velocity = new Vector2(speedX, speedY);
            Projectile.NewProjectile(position, velocity, type, damage, knockBack, player.whoAmI, ai1:velocity.Length()*0.3f);
            return false;
        }
    }
    public class Void_Spectral_P : ModProjectile {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Void Spectral");
        }
        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.ToxicFlask);
            projectile.width = 32;
            projectile.height = 32;
        }
        public override void AI() {
			projectile.rotation -= (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.03f * projectile.direction;
			projectile.rotation += Math.Abs(projectile.velocity.X) * 0.04f * projectile.direction;
        }
        public override void Kill(int timeLeft) {
			Main.PlaySound(SoundID.Item107, projectile.position);
			Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 704);
			Gore.NewGore(projectile.Center, -projectile.oldVelocity * 0.2f, 705);
			if (projectile.owner == Main.myPlayer) {
				int count = Main.rand.Next(9, 12);
                int type = ModContent.ProjectileType<Void_Spectral_Wisp>();
				for (int i = 0; i < count; i++) {
                    Vector2 velocity = Main.rand.NextVector2Circular(1, 1);
					Projectile.NewProjectile(projectile.Center+Main.rand.NextVector2Circular(64, 64), velocity, type, projectile.damage, 1f, projectile.owner, ai1:projectile.ai[1]*Main.rand.NextFloat(0.85f, 1.15f));
                }
			}
        }
    }
    public class Void_Spectral_Wisp : ModProjectile {
        public override string Texture => "Origins/Items/Weapons/Other/Void_Spectral";
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Void Spectral");
        }
        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.SpiritFlame);
            projectile.aiStyle = 0;
            projectile.penetrate = 2;
			projectile.timeLeft = 120-Main.rand.Next(30);
            projectile.localAI[1] = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
        }
        public override void AI() {
	        if (projectile.localAI[0] > 0f) {
                projectile.localAI[0]--;
	        }
	        if (projectile.localAI[0] == 0f && projectile.owner == Main.myPlayer) {
		        projectile.localAI[0] = 5f;
                float currentTargetDist = projectile.ai[0]>0?Main.npc[(int)projectile.ai[0] - 1].Distance(projectile.Center):0;
		        for (int i = 0; i < Main.maxNPCs; i++) {
			        NPC targetOption = Main.npc[i];
			        if (targetOption.CanBeChasedBy()) {
                        float newTargetDist = targetOption.Distance(projectile.Center);
				        bool selectNew = projectile.ai[0] <= 0f || currentTargetDist > newTargetDist;
				        if (selectNew && (newTargetDist < 240f)) {
					        projectile.ai[0] = i+1;
                            currentTargetDist = newTargetDist;
				        }
			        }
		        }
		        if (projectile.ai[0] > 0f) {
			        projectile.timeLeft = 300-Main.rand.Next(120);
			        projectile.netUpdate = true;
		        }
	        }

			Dust dust = Dust.NewDustDirect(projectile.Center, 0, 0, 27, 0f, -2f);
			dust.noGravity = true;
            dust.velocity = projectile.oldVelocity * 0.5f;
			dust.scale = 0.85f+(float)Math.Sin(projectile.timeLeft)*0.15f;
			dust.fadeIn = 0.5f;
			dust.alpha = 200;

			int target = (int)projectile.ai[0]-1;
			if (target >= 0) {
                if(Main.npc[target].active) {
                    if(projectile.Distance(Main.npc[target].Center) > 1f) {
                        Vector2 dir = projectile.DirectionTo(Main.npc[target].Center);
                        if(dir.HasNaNs()) {
                            dir = Vector2.UnitY;
                        }
                        float angle = dir.ToRotation();
                        PolarVec2 velocity = (PolarVec2)projectile.velocity;
                        float targetVel = projectile.ai[1];
                        bool changed = false;
                        if(velocity.R != targetVel) {
                            OriginExtensions.LinearSmoothing(ref velocity.R, targetVel, (targetVel-0.5f)*0.1f);
                            changed = true;
                        }
                        if(velocity.Theta != angle) {
                            OriginExtensions.AngularSmoothing(ref velocity.Theta, angle, 0.1f, false);
                            changed = true;
                        }
                        if(changed) {
                            projectile.velocity = (Vector2)velocity;
                        }
                    }
                    return;
                }
				projectile.ai[0] = 0f;
				projectile.netUpdate = true;
			} else {
                PolarVec2 velocity = (PolarVec2)projectile.velocity;
                float targetVel = projectile.ai[1];
                bool changed = false;
                if(velocity.R != targetVel) {
                    OriginExtensions.LinearSmoothing(ref velocity.R, targetVel/3f, (targetVel-0.5f)*0.1f);
                    changed = true;
                }

                if(velocity.Theta != projectile.localAI[1]) {
                    OriginExtensions.AngularSmoothing(ref velocity.Theta, projectile.localAI[1], (targetVel-0.5f)*0.03f, false);
                    changed = true;
                } else {
                    projectile.localAI[1] = Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi);
                }

                if(changed) {
                    projectile.velocity = (Vector2)velocity;
                }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor) {
            return false;
        }
    }
}
