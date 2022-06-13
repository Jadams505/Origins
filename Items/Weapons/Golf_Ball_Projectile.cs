﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons {
	public abstract class Golf_Ball_Projectile : ModProjectile {
		public override void SetStaticDefaults() {
			base.SetStaticDefaults();
			ProjectileID.Sets.IsAGolfBall[Type] = true;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.DirtGolfBall);
		}
		public override bool? CanDamage() {
			if (Projectile.velocity.LengthSquared() > 4 * 4) {
				return null;
			}
			return false;
		}
		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
			damage += (int)(damage * Projectile.velocity.Length() * 0.0833f);
		}
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			Vector2 hitbox = Projectile.Hitbox.Center.ToVector2();
			Vector2 intersect = Rectangle.Intersect(Projectile.Hitbox, target.Hitbox).Center.ToVector2();
			bool bounced = false;
			if (hitbox.X != intersect.X) {
				Projectile.velocity.X = -Projectile.velocity.X;
				bounced = true;
			}
			if (hitbox.Y != intersect.Y) {
				Projectile.velocity.Y = -Projectile.velocity.Y;
				bounced = true;
			}
			if (!bounced) {
				if (Math.Abs(Projectile.velocity.X) > Math.Abs(Projectile.velocity.Y)) {
					Projectile.velocity.X = -Projectile.velocity.X;
				} else if (Math.Abs(Projectile.velocity.Y) > Math.Abs(Projectile.velocity.X)) {
					Projectile.velocity.Y = -Projectile.velocity.Y;
				}
			}
		}
	}
}
