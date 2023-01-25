﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
    public class Rebreather : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Rebreather");
            Tooltip.SetDefault("Gain more breath as you move in water");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.YoYoGlove);
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 1);
        }
        public override void UpdateEquip(Player player) {
            //player.GetModPlayer<OriginPlayer>().rebreather = true;
        }
    }
}
