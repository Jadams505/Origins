﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Ammo {
	public class Flammable_Harpoon : ModItem {
		public override string Texture => "Origins/Items/Weapons/Ammo/Flammable_Harpoon";
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
			ID = Type;
		}
		public override void SetDefaults() {
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.consumable = true;
			Item.maxStack = 99;
			Item.shoot = Flammable_Harpoon_P.ID;
			Item.ammo = Harpoon.ID;
			Item.value = Item.sellPrice(silver: 3);
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type, 8);
            recipe.AddIngredient(ItemID.Gel);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();

			recipe = Recipe.Create(Type, 8);
            recipe.AddIngredient(ItemID.Gel);
            recipe.AddIngredient(ModContent.ItemType<Harpoon>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	public class Flammable_Harpoon_P : Harpoon_P {
		public static new int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			ID = Type;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.OnFire, Main.rand.Next(360, 480));
		}
	}
}
