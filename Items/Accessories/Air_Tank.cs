﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	public class Air_Tank : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Air Tank");
            Tooltip.SetDefault("Greatly extends underwater breathing\nImmunity to ‘Suffocation’");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.YoYoGlove);
            Item.handOffSlot = -1;
            Item.handOnSlot = -1;
        }
        public override void UpdateEquip(Player player) {
            player.buffImmune[BuffID.Suffocation] = true;
            player.breathMax+=215;
        }
    }
}
