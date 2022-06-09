﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Origins.Items.Accessories {
    public class Fiberglass_Dagger : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Fiberglass Dagger");
            Tooltip.SetDefault("Increases weapon damage by 8, but reduces defense by 8");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.WormScarf);
            Item.neckSlot = -1;
        }
        public override void UpdateEquip(Player player) {
            player.statDefense -= 8;
            player.GetModPlayer<OriginPlayer>().fiberglassDagger = true;
        }
    }
}
