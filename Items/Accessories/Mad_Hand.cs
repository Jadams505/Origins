﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
    public class Mad_Hand : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("SOTH");
            Tooltip.SetDefault("Double the gunpowder, double the fun.");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.accessory = true;
            Item.width = 21;
            Item.height = 20;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
        }
        public override void UpdateEquip(Player player) {
            player.GetModPlayer<OriginPlayer>().madHand = true;
            player.GetModPlayer<OriginPlayer>().explosiveThrowSpeed+=0.65f;
        }
    }
}
