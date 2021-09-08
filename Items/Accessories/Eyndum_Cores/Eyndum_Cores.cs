﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories.Eyndum_Cores {
    public abstract class Eyndum_Core : ModItem {
        public abstract Color CoreGlowColor { get; }
        public override bool CanRightClick() {
            Item equippedItem = Main.LocalPlayer.GetModPlayer<OriginPlayer>().eyndumCore.Value;
            if (equippedItem.type!=item.type) {
                int e = equippedItem.type;
                int t = item.type;
                equippedItem.SetDefaults(t);
                item.SetDefaults(e);
                Main.PlaySound(SoundID.Grab);
            }
            return false;
        }
    }
    public class Agility_Core : Eyndum_Core {
        public override Color CoreGlowColor => new Color(255, 220, 0, 160);
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Agility Core");
        }
        public override void UpdateEquip(Player player) {
            player.wingTimeMax *= 2;
            player.moveSpeed *= 4f;
            player.runAcceleration *= 3f;
            player.maxRunSpeed *= 3f;
            player.jumpSpeedBoost += 5;
        }
    }
    public class Combat_Core : Eyndum_Core {
        public override Color CoreGlowColor => new Color(160, 0, 255, 160);
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Combat Core");
        }
        public override void UpdateEquip(Player player) {
            player.allDamageMult *= 1.48f;
        }
    }
    public class Construction_Core : Eyndum_Core {
        public override Color CoreGlowColor => new Color(255, 160, 0, 160);
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Construction Core");
        }
        public override void UpdateEquip(Player player) {
            player.tileSpeed *= 2f;
            player.wallSpeed *= 2.5f;
            player.pickSpeed *= 3f;
            player.blockRange += 40;
        }
    }
    public class Lifeforce_Core : Eyndum_Core {
        public override Color CoreGlowColor => new Color(255, 0, 75, 160);
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Lifeforce Core");
        }
        public override void UpdateEquip(Player player) {
            player.statLifeMax2 += player.statLifeMax2 / 2;
            player.lifeRegenCount += player.statLifeMax2 / 22;
        }
    }
}
