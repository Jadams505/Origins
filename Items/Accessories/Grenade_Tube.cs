﻿using Origins.Tiles.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
    public class Grenade_Tube : ModItem {
        static short glowmask;
        public string[] Categories => new string[] {
            "Combat"
        };
        public override void SetStaticDefaults() {
            glowmask = Origins.AddGlowMask(this);
        }
        public override void SetDefaults() {
			Item.DefaultToAccessory(20, 34);
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 2);
            Item.glowMask = glowmask;
        }
		public override void UpdateEquip(Player player) {
			//player.GetModPlayer<OriginPlayer>().noobTube = true;
		}
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.IllegalGunParts);
			recipe.AddIngredient(ModContent.ItemType<Missile_Armcannon>());
			recipe.AddIngredient(ModContent.ItemType<Silicon_Item>(), 15);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}
