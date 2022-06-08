﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Origins.Projectiles;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Origins.Items.Weapons.Other {
    public class Viper_Rifle : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("HNO-3 \"Viper\"");
            Tooltip.SetDefault("Has a chance to inflict \"Solvent\", increasing critical damage\nDeals critical damage on otherwise afflicted enemies");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Gatligator);
            Item.damage = 48;
            Item.crit = 5;
            Item.knockBack = 6.75f;
            Item.useAnimation = Item.useTime = 27;
            Item.width = 114;
            Item.height = 40;
            Item.autoReuse = false;
            Item.scale = 0.75f;
            Item.UseSound = Origins.Sounds.HeavyCannon;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-6, 0);
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
            Vector2 unit = Vector2.Normalize(velocity);
            position += unit * 16;
            float dist = 80 - velocity.Length();
            position += unit * dist;
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            OriginGlobalProj.viperEffectNext = true;
            Vector2 unit = Vector2.Normalize(velocity);
            float dist = 80 - velocity.Length();
            position -= unit * dist;
            Projectile barrelProj = Projectile.NewProjectileDirect(source, position, unit * (dist / 20), type, damage, knockback, player.whoAmI);
            barrelProj.extraUpdates = 19;
            barrelProj.timeLeft = 20;
            OriginGlobalProj.viperEffectNext = true;
            OriginGlobalProj.killLinkNext = barrelProj.whoAmI;
            return true;
        }
    }
}
