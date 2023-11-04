﻿using Origins.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Unsought_Organ : ModItem {
		
		public override void SetDefaults() {
			Item.DefaultToAccessory(28, 26);
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.sellPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<Unsought_Organ_P>();
		}
		public override void UpdateEquip(Player player) {
			player.aggro -= 275;
			player.GetDamage(DamageClass.Generic) *= 1.05f;
			player.GetCritChance(DamageClass.Generic) += 5f;
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.unsoughtOrgan = true;
			originPlayer.unsoughtOrganItem = Item;
		}
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<Olid_Organ>());
			recipe.AddIngredient(ModContent.ItemType<Razorwire>());
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
	public class Unsought_Organ_P : ModProjectile {
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CursedBullet;
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.CursedBullet);
			Projectile.aiStyle = 0;
		}
		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.alpha > 0)
				Projectile.alpha -= 15;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(Toxic_Shock_Debuff.ID, Toxic_Shock_Debuff.default_duration);
		}
	}
}
