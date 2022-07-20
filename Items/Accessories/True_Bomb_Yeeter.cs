﻿using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
    public class True_Bomb_Yeeter : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Pneumatic Bomb Thrower");
            Tooltip.SetDefault("Also commonly referred to as the 'True Bomb Yeeter'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
        }
        public override void UpdateEquip(Player player) {
            player.GetModPlayer<OriginPlayer>().bombHandlingDevice = true;
            player.GetModPlayer<OriginPlayer>().explosiveThrowSpeed+=0.81f;
        }
    }
}