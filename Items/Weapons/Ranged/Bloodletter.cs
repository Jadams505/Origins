﻿using Microsoft.Xna.Framework;
using Origins.Items.Weapons.Ammo;
using Origins.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Ranged {
    public class Bloodletter : Harpoon_Gun {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Bloodletter");
			// Tooltip.SetDefault("Uses harpoons as ammo\n80% chance not to consume ammo");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.damage = 21;
			Item.DamageType = DamageClass.Ranged;
			Item.knockBack = 4;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.useAnimation = 3;
			Item.useTime = 3;
			Item.reuseDelay = 2;
			Item.width = 58;
			Item.height = 22;
			Item.useAmmo = Harpoon.ID;
			Item.shoot = Harpoon_P.ID;
			Item.shootSpeed = 15.25f;
			Item.UseSound = SoundID.Item11;
			Item.value = Item.sellPrice(gold: 1, silver: 50);
			Item.rare = ItemRarityID.Blue;
			Item.autoReuse = true;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.CrimtaneBar, 8);
			recipe.AddIngredient(ModContent.ItemType<Harpoon_Gun>());
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public override Vector2? HoldoutOffset() => new Vector2(-8, 0);
		public override void OnConsumeAmmo(Item ammo, Player player) {
			if (!Main.rand.NextBool(5)) {
				ammo.stack++;
			} else {
				consume = true;
			}
		}
		public override void ModifyShotProjectile(Projectile projectile, EntitySource_ItemUse_WithAmmo source) {
			projectile.GetGlobalProjectile<HarpoonGlobalProjectile>().bloodletter = true;
		}
	}
}
