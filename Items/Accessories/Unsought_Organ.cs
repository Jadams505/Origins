﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
    public class Unsought_Organ : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Unsought Organ");
            Tooltip.SetDefault("Half of damage recieved is split across 3 enemies whilst inflicting 'Toxic Shock'\n5% increased damage and critical strike chance\nEnemies are less likely to target you");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.YoYoGlove);
            Item.rare = ItemRarityID.LightPurple;
            Item.value = Item.sellPrice(gold: 5);
            Item.shoot = ProjectileID.BulletHighVelocity;
        }
        public override void UpdateEquip(Player player) {
            player.aggro -= 275;
            player.GetDamage(DamageClass.Generic) *= 1.05f;
            player.GetCritChance(DamageClass.Generic) *= 1.05f;
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
}
