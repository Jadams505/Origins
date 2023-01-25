﻿using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
    public class Air_Tank : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Air Tank");
            Tooltip.SetDefault("Extends underwater breathing\nImmunity to ‘Suffocation’");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.YoYoGlove);
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
        }
        public override void UpdateEquip(Player player) {
            player.buffImmune[BuffID.Suffocation] = true;
            player.breathMax += 257;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.TitaniumBar, 20);
            recipe.AddIngredient(ModContent.ItemType<Rubber>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();

            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.AdamantiteBar, 20);
            recipe.AddIngredient(ModContent.ItemType<Rubber>(), 12);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
