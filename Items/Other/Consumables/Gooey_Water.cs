﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Origins.Items.Materials;
using Origins.Tiles.Defiled;
using Origins.World.BiomeData;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Origins.Items.Other.Consumables {
    public class Gooey_Water : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Gooey Water");
			Tooltip.SetDefault("Spreads the {$Riven_Hive} to some blocks");
			SacrificeTotal = 99;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.BloodWater);
			Item.shoot = ModContent.ProjectileType<Gooey_Water_P>();
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.BottledWater, 10);
			recipe.AddIngredient(ModContent.ItemType<Defiled_Sand_Item>(), 1);
			recipe.AddIngredient(ModContent.ItemType<Defiled_Grass_Seeds>(), 1);
			recipe.Register();
		}
	}
	public class Gooey_Water_P : ModProjectile {
		public override string Texture => base.Texture.Substring(0, base.Texture.Length - 2);
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Gooey Water");
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.BloodWater);
		}
		public override void Kill(int timeLeft) {
			AltLibrary.Core.ALConvert.Convert<Riven_Hive_Alt_Biome>((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16);

			SoundEngine.PlaySound(SoundID.Shatter, Projectile.position);
			for (int i = 0; i < 5; i++) {
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Glass);
			}
			for (int i = 0; i < 30; i++) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.UnholyWater, 0f, -2f, 0, default(Color), 1.1f);
				dust.alpha = 100;
				dust.velocity.X *= 1.5f;
				dust.velocity *= 3f;
			}
		}
	}
}
