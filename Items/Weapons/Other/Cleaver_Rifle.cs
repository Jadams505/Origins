﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Other {
    //implemented in 10 minutes, so it might have an issue or two
    public class Cleaver_Rifle : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Cleaver Rifle");
            Tooltip.SetDefault("Crude and dangerous");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Gatligator);
            Item.damage = 39;
            Item.useAnimation = Item.useTime = 10;
            Item.shootSpeed*=2;
            Item.width = 106;
            Item.height = 32;
            Item.scale = 0.7f;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-18, 0);
    }
    //Undecided obtain method...
}
