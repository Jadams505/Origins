using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Origins.Dev;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using System;
namespace Origins.Items.Weapons.Magic {
	public class Shimmer_Rod : ModItem, ICustomWikiStat {
		public string[] Categories => [
			"MagicStaff"
		];
		public override string Texture => "Terraria/Images/Item_" + ItemID.NimbusRod;
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.NimbusRod);
			Item.useStyle = ItemUseStyleID.Swing;
			Item.shoot = ModContent.ProjectileType<Shimmer_Cloud_Held>();
			Item.channel = true;
		}
	}
	public class Shimmer_Cloud_Held : ModProjectile {
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainCloudMoving;
		public override void SetStaticDefaults() {
			Main.projFrames[Type] = 4;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.RainCloudMoving);
			Projectile.aiStyle = 0;
			Projectile.timeLeft = Projectile.timeLeft;
		}
		public Vector2 TargetPos {
			get => new(Projectile.ai[0], Projectile.ai[1]);
			set {
				Projectile.ai[0] = value.X;
				Projectile.ai[1] = value.Y;
			}
		}
		public override void OnSpawn(IEntitySource source) {
			TargetPos = Main.MouseWorld;
		}
		public override void AI() {
			Player owner = Main.player[Projectile.owner];
			owner.heldProj = Projectile.whoAmI;
			if (Projectile.ai[2] == 0) {
				owner.itemTime = owner.itemTimeMax;
				owner.itemAnimation = owner.itemAnimationMax;
				Projectile.Center = owner.itemLocation + new Vector2(24 * owner.direction, -24).RotatedBy(owner.itemRotation);
				if (!owner.channel && Main.myPlayer == Projectile.owner) {
					Projectile.ai[2] = 1;
					Vector2 diff = (Main.MouseWorld - TargetPos);
					if (diff == default) {
						Projectile.localAI[0] = 0;
					} else {
						Projectile.localAI[0] = diff.ToRotation() - MathHelper.PiOver2;
					}
					Projectile.netUpdate = true;
				}
			} else {
				Vector2 tan = new Vector2(24 * owner.direction, -24).RotatedBy(owner.itemRotation);
				Projectile.Center = owner.itemLocation + tan;
				if (Main.myPlayer == Projectile.owner) {
					Vector2 diff = (TargetPos - Projectile.Center).SafeNormalize(Projectile.velocity);
					Vector2 normTan = new Vector2(-tan.Y * owner.direction, tan.X * owner.direction).SafeNormalize(Projectile.velocity);
					//Dust.NewDustPerfect(Projectile.Center, 6, diff * 8).noGravity = true;
					//Dust.NewDustPerfect(Projectile.Center, 6, normTan * 8).noGravity = true;
					float dot = 1 - Vector2.Dot(diff, normTan);
					float change = Projectile.localAI[1] - dot;
					if (Projectile.localAI[2] == 1 && (dot == 0 || change < 0)) {
						//Dust.NewDustPerfect(Projectile.Center, 23, normTan).noGravity = true;
						Projectile.NewProjectile(
							owner.GetSource_ItemUse(owner.HeldItem),
							Projectile.Center,
							diff * Projectile.velocity.Length(),
							ModContent.ProjectileType<Shimmer_Cloud_Ball>(),
							Projectile.damage,
							Projectile.knockBack,
							Projectile.owner,
							Projectile.ai[0],
							Projectile.ai[1],
							Projectile.localAI[0]
						);
						Projectile.Kill();
					}
					Projectile.localAI[1] = dot;
					Projectile.localAI[2] = 1;
				}
			}
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4) {
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 3)
					Projectile.frame = 0;
			}
		}
		public override void PostDraw(Color lightColor) {
			Rectangle frame = TextureAssets.Projectile[Type].Value.Frame(verticalFrames: 4);
			Main.spriteBatch.Draw(
				TextureAssets.Projectile[Type].Value,
				TargetPos - Main.screenPosition - frame.Size() * 0.5f,
				frame,
				Color.White * 0.6f
			);
		}
		public override bool ShouldUpdatePosition() => false;
	}
	public class Shimmer_Cloud_Ball : ModProjectile {
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainCloudMoving;
		public override void SetStaticDefaults() {
			Main.projFrames[Type] = 4;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.RainCloudMoving);
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 18000;
		}
		public Vector2 TargetPos {
			get => new(Projectile.ai[0], Projectile.ai[1]);
			set {
				Projectile.ai[0] = value.X;
				Projectile.ai[1] = value.Y;
			}
		}
		public override void AI() {
			if (TargetPos != default) {
				Vector2 combined = Projectile.velocity * (TargetPos - Projectile.Center);
				if (Projectile.owner == Main.myPlayer && combined.X <= 0 && combined.Y <= 0) {
					Projectile.NewProjectile(
						Projectile.GetSource_FromAI(),
						TargetPos,
						default,
						ModContent.ProjectileType<Shimmer_Cloud_P>(),
						Projectile.damage,
						Projectile.knockBack,
						Projectile.owner,
						Projectile.ai[2]
					);
					Projectile.Kill();
					OriginExtensions.FadeOutOldProjectilesAtLimit([ModContent.ProjectileType<Shimmer_Cloud_P>(), ModContent.ProjectileType<Shimmer_Cloud_Ball>()], 3, 52);
				}
			}

			Projectile.rotation += Projectile.velocity.X * 0.02f;
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4) {
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 3)
					Projectile.frame = 0;
			}
		}
	}
	public class Shimmer_Cloud_P : ModProjectile {
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainCloudRaining;
		public override void SetStaticDefaults() {
			Main.projFrames[Type] = 6;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.RainCloudRaining);
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 18000;
			Projectile.width = 28;
			Projectile.height = 28;
		}
		public override void AI() {
			Projectile.rotation = Projectile.ai[0];
			if (++Projectile.ai[1] % 10f < 1) {
				Vector2 unit = new Vector2(0, 1).RotatedBy(Projectile.rotation);
				Vector2 perp = new(unit.Y, -unit.X);
				Projectile.NewProjectile(
					Projectile.GetSource_FromAI(),
					Projectile.Center + unit * 24 + perp * Main.rand.Next(-14, 15),
					unit * 5,
					ModContent.ProjectileType<Shimmer_Cloud_Rain>(),
					Projectile.damage,
					Projectile.knockBack,
					Projectile.owner,
					Projectile.ai[2]
				);
			}
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 4) {
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame > 5)
					Projectile.frame = 0;
			}
		}
		public override bool PreDraw(ref Color lightColor) {
			Rectangle frame = TextureAssets.Projectile[Type].Value.Frame(verticalFrames: 6, frameY: Projectile.frame);
			float timeFactor = Math.Min(Projectile.timeLeft / 52f, 1);
			Main.spriteBatch.Draw(
				TextureAssets.Projectile[Type].Value,
				Projectile.Center - Main.screenPosition,
				frame,
				lightColor * timeFactor,
				Projectile.rotation,
				frame.Size() * 0.5f,
				Projectile.scale,
				0,
			0);
			return false;
		}
	}
	public class Shimmer_Cloud_Rain : ModProjectile {
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.RainFriendly;
		public override void SetStaticDefaults() {
			Origins.HomingEffectivenessMultiplier[Type] = 2;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.RainFriendly);
			Projectile.aiStyle = 0;
			Projectile.width = 4;
			Projectile.height = 4;
		}
		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (projHitbox.Intersects(targetHitbox)) return true;
			const float factor = 1.5f;
			Rectangle hitbox = projHitbox;
			for (int i = 0; i < 7; i++) {
				hitbox.X = projHitbox.X - (int)(Projectile.velocity.X * i * factor);
				hitbox.Y = projHitbox.Y - (int)(Projectile.velocity.Y * i * factor);
				if (hitbox.Intersects(targetHitbox)) return true;
			}
			return false;
		}
	}
}