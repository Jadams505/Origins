﻿using Microsoft.Xna.Framework;
using Origins.Items.Weapons.Ammo;
using Origins.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Demolitionist {
    public class Bombardment : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Bombardment");
			Tooltip.SetDefault("Releases a barrage of mines");
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ProximityMineLauncher);
			Item.damage = 2;
			Item.useTime = 6;
			Item.useAnimation = 36;
			Item.knockBack = 4f;
			Item.useAmmo = ModContent.ItemType<Resizable_Mine>();
			Item.shoot = ModContent.ProjectileType<Bombardment_P>();
			Item.shootSpeed = 9;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(silver: 49);
			Item.UseSound = null;
			Item.reuseDelay = 60;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			velocity = velocity.RotatedByRandom(0.15f);
			Terraria.Audio.SoundEngine.PlaySound(SoundID.Item61.WithPitch(0.25f), position);
		}
	}
	public class Bombardment_P : ModProjectile, IIsExplodingProjectile {
		public override string Texture => "Origins/Items/Weapons/Ammo/Resizable_Mine_P";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Floating Mine");
			Origins.MagicTripwireRange[Type] = 30;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Grenade);
			Projectile.timeLeft = 420;
			Projectile.scale = 0.3f;
			Projectile.penetrate = 1;
			Projectile.aiStyle = 0;
			//AIType = ProjectileID.ProximityMineI;
		}
		public override void AI() {
			Projectile.velocity *= 0.95f;
			//Projectile.noGravity = true; Not intuitive enough
		}
		public override bool PreKill(int timeLeft) {
			//Projectile.type = ProjectileID.RocketI;
			Projectile.penetrate = -1;
			Projectile.position.X += Projectile.width / 2;
			Projectile.position.Y += Projectile.height / 2;
			Projectile.width = 96;
			Projectile.height = 96;
			Projectile.position.X -= Projectile.width / 2;
			Projectile.position.Y -= Projectile.height / 2;
			Projectile.Damage();
			ExplosiveGlobalProjectile.ExplosionVisual(Projectile, true, sound: SoundID.Item62);
			return false;
		}
		public bool IsExploding() => Projectile.penetrate == -1;
	}
}
