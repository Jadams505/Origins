﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Melee {
	public class Crystal_Cutters : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Crystal Cutters");
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.TerraBlade);
			Item.damage = 44;
			Item.DamageType = DamageClass.Melee;
			Item.width = 34;
			Item.height = 34;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 39;
			Item.useAnimation = 13;
			Item.knockBack = 4f;
			Item.shoot = ModContent.ProjectileType<Crystal_Cutters_P>();
			Item.shootSpeed = 7;
			Item.rare = ItemRarityID.LightRed;
			Item.autoReuse = true;
			Item.value = Item.buyPrice(gold: 1);
		}
	}
	public class Crystal_Cutters_P : ModProjectile {
		public override string Texture => "Origins/Items/Weapons/Melee/Crystal_Cutters_P";
		public static int ID { get; private set; }
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Crystal Ball");
			ID = Type;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.CrystalDart);
			Projectile.aiStyle = 0;
			Projectile.alpha = 0;
		}
		public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 200);
		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) => fallThrough = true; // only works because we happen to want to set fallThrough to the same value we want to return
		public override bool OnTileCollide(Vector2 oldVelocity) {
			if (Projectile.velocity.X != oldVelocity.X) {
				Projectile.velocity.X = 0f - oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) {
				Projectile.velocity.Y = 0f - oldVelocity.Y;
			}
			Projectile.timeLeft -= 60;
			return false;
		}
	}
}