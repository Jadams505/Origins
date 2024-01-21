﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Origins.Items.Weapons.Ammo;
using Origins.NPCs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Origins.OriginExtensions;

namespace Origins.Items.Weapons.Demolitionist {
	public class Ace_Shrapnel : ModItem {

		
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ProximityMineLauncher);
			Item.DamageType = DamageClasses.ExplosiveVersion[DamageClass.Ranged];
			Item.damage = 36;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.useTime = 20;
			Item.useAnimation = 28;
			Item.shootSpeed /= 1;
			Item.shoot = ModContent.ProjectileType<Ace_Shrapnel_P>();
			Item.ammo = ModContent.ItemType<Scrap>();
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<NE8>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Scrap>(), 15);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			type -= ModContent.ProjectileType<Ace_Shrapnel_P>();
			type /= 3;
			Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI, 6 + type, 0 - type);
			return false;
		}
	}
	public class Ace_Shrapnel_P : ModProjectile {
		
		public override string Texture => "Origins/Projectiles/Pixel";
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Bullet);
			Projectile.DamageType = DamageClasses.ExplosiveVersion[DamageClass.Ranged];
			Projectile.aiStyle = 0;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 0;
			Projectile.width = Projectile.height = 10;
			Projectile.light = 0;
			Projectile.timeLeft = 168;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
			Projectile.ignoreWater = true;
		}
		public override void AI() {
			Dust.NewDustDirect(Projectile.Center, 0, 0, DustID.Torch, Scale: 0.4f).noGravity = true;
			if (Projectile.ai[0] > 0 && Projectile.timeLeft % 6 == 0) {
				Projectile.ai[0]--;
				if (Projectile.velocity.Length() < 1) {
					Vector2 v = Main.rand.NextVector2Unit() * 6;
					Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + v * 8, v, ModContent.ProjectileType<Ace_Shrapnel_Shard>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, Projectile.ai[1] + 1);
					return;
				}
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.RotatedByRandom(1) * 1.1f, ModContent.ProjectileType<Ace_Shrapnel_Shard>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI, Projectile.ai[1] + 1);
			}
		}
		public override bool? CanHitNPC(NPC target) {
			return false;//((int)projectile.ai[0]<=0)?null:((bool?)false);
		}
		public override bool PreDraw(ref Color lightColor) {
			return false;
		}
	}
	public class Ace_Shrapnel_Shard : ModProjectile {

		const float cohesion = 0.1f;

		const double chaos = Math.PI;
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.BoneGloveProj;
		int dustStyle;
		
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Bullet);
			Projectile.DamageType = DamageClasses.ExplosiveVersion[DamageClass.Ranged];
			Projectile.aiStyle = 0;
			Projectile.penetrate = 3;
			Projectile.extraUpdates = 0;
			Projectile.width = Projectile.height = 10;
			Projectile.timeLeft = 240;
			Projectile.ignoreWater = true;
			if (Shrapnel_Dust.DustIDs is not null) dustStyle = Main.rand.Next(Shrapnel_Dust.DustIDs);
		}
		public override void AI() {
			Dust.NewDustPerfect(Projectile.Center, dustStyle, Vector2.Zero);
			if (Projectile.ai[0] >= 0) {
				Projectile center = Main.projectile[(int)Projectile.ai[0]];
				if (!center.active) {
					Projectile.ai[0] = -1;
					return;
				}
				Projectile.velocity = Projectile.velocity.RotatedByRandom(chaos);
				//float angle = projectile.velocity.ToRotation();
				float targetAngle = (center.Center - Projectile.Center).ToRotation();
				Projectile.velocity = (Projectile.velocity + new Vector2(cohesion * (Projectile.ai[1] > 1 ? 2 : 1), 0).RotatedBy(targetAngle)).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length();
				//projectile.velocity = projectile.velocity.RotatedBy(Clamp((float)AngleDif(targetAngle,angle), -0.05f, 0.05f));
				//Dust.NewDustDirect(projectile.Center+new Vector2(16,0).RotatedBy(targetAngle), 0, 0, 6, Scale:2).noGravity = true;
			}
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.immune[Projectile.owner] /= 2;
			OriginGlobalNPC.InflictImpedingShrapnel(target, 600);
			/*if (target.life <= 0 && Projectile.ai[1] < 5) {
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<Ace_Shrapnel_P>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 8 - Projectile.ai[1], Projectile.ai[1]);
			}*/
		}
	}
	public class Shrapnel_Dust : ModDust {
		public static List<int> DustIDs { get; private set; }
		public override void SetStaticDefaults() {
			if (DustIDs is null) DustIDs = new(3);
			DustIDs.Add(Type);
		}
		public override void Unload() {
			DustIDs = null;
		}
		public override void OnSpawn(Dust dust) {
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 14, 10);
		}
		public override Color? GetAlpha(Dust dust, Color lightColor) {
			return Color.Lerp(lightColor, new Color(255, 255, 255, 128), 0.5f);
		}
	}
	public class Shrapnel_Dust_2 : Shrapnel_Dust {
		public override void OnSpawn(Dust dust) {
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 8, 8);
		}
	}
	public class Shrapnel_Dust_3 : Shrapnel_Dust {
		public override void OnSpawn(Dust dust) {
			dust.noGravity = true;
			dust.frame = new Rectangle(0, 0, 12, 12);
		}
	}
}
