﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Origins.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Ranged {
	public class Viper_Rifle : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("HNO-3 \"Viper\"");
			Tooltip.SetDefault("Has a chance to inflict \"Toxic Shock\"\nDeals critical damage on otherwise afflicted enemies");
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.Gatligator);
			Item.damage = 60;
			Item.crit = 9;
			Item.knockBack = 6.75f;
			Item.useAnimation = Item.useTime = 27;
			Item.width = 100;
			Item.height = 28;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(silver: 50);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = Origins.Sounds.HeavyCannon;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Eitrite_Bar>(), 26);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		public override Vector2? HoldoutOffset() => new Vector2(-6, 0);
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 unit = Vector2.Normalize(velocity);
			position += unit * 16;
			float dist = 80 - velocity.Length();
			position += unit * dist;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			OriginGlobalProj.viperEffectNext = true;
			Vector2 unit = Vector2.Normalize(velocity);
			float dist = 80 - velocity.Length();
			position -= unit * dist;
			Projectile barrelProj = Projectile.NewProjectileDirect(source, position, unit * (dist / 20), type, damage, knockback, player.whoAmI);
			barrelProj.extraUpdates = 19;
			barrelProj.timeLeft = 20;
			OriginGlobalProj.viperEffectNext = true;
			OriginGlobalProj.killLinkNext = barrelProj.whoAmI;
			return true;
		}
	}
}
