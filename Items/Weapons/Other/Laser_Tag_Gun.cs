﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Other {
	public class Laser_Tag_Gun : AnimatedModItem, IElementalItem {
        public ushort Element => Elements.Earth;
        static DrawAnimationManual animation;
        public override DrawAnimation Animation => animation;
        public override Color? GlowmaskTint => Main.teamColor[Main.player[Item.playerIndexTheItemIsReservedFor].team];
        static short glowmask;
        public override void SetStaticDefaults() {
			DisplayName.SetDefault("Laser Tag Gun");
			Tooltip.SetDefault("‘Once you're tagged follow through the safety guidelines and walk out of the chamber.’");
            animation = new DrawAnimationManual(1);
			Main.RegisterItemAnimation(Item.type, animation);
            glowmask = Origins.AddGlowMask("Weapons/Other/Laser_Tag_Gun_Glow");
		}
		public override void SetDefaults() {
            Item.CloneDefaults(ItemID.SpaceGun);
			Item.damage = 1;
			Item.magic = true;
			Item.ranged = true;
			Item.noMelee = true;
            Item.crit = 46;
			Item.width = 42;
			Item.height = 14;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.mana = 10;
			Item.value = 70000;
            Item.shoot = ModContent.ProjectileType<Laser_Tag_Laser>();
			Item.rare = ItemRarityID.Green;
            Item.glowMask = glowmask;
            Item.scale = 1;
		}
        public override void UpdateInventory(Player player) {
        }
        int GetCritMod(Player player) {
            OriginPlayer modPlayer = player.GetModPlayer<OriginPlayer>();
            int critMod = 0;
            if((modPlayer.oldBonuses&1)!=0||modPlayer.fiberglassSet||modPlayer.fiberglassDagger) {
                critMod = -50;
            }
            if((modPlayer.oldBonuses&2)!=0||modPlayer.felnumSet) {
                critMod = -64;
            }
            return critMod;
        }
        public override void ModifyWeaponCrit(Player player, ref float crit) {
            if(player.HeldItem.type != Item.type)crit+=GetCritMod(player);
        }
        public override Vector2? HoldoutOffset() {
            return new Vector2(3-(11*Main.player[Item.playerIndexTheItemIsReservedFor].direction),0);
        }
        public override void HoldItem(Player player) {
            if(player.itemAnimation!=0) {
                player.GetModPlayer<OriginPlayer>().itemLayerWrench = true;
            }
            int critMod = GetCritMod(player);
            player.rangedCrit+=critMod;
            player.magicCrit+=critMod;
        }
    }
    public class Laser_Tag_Laser : ModProjectile {
        public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.GreenLaser);
            Projectile.light = 0;
            Projectile.aiStyle = 0;
            Projectile.extraUpdates++;
			Projectile.magic = true;
			Projectile.ranged = true;
        }
        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();
            try {
                Color color = Main.teamColor[Main.player[Projectile.owner].team];
                Lighting.AddLight(Projectile.Center, Vector3.Normalize(color.ToVector3())*3);
            } catch(Exception) { }
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
            if(crit)damage*=199;
        }
        public override void ModifyHitPvp(Player target, ref int damage, ref bool crit) {
            if(crit)damage*=199;
        }
        public override void OnHitPvp(Player target, int damage, bool crit) {
            target.AddBuff(BuffID.Cursed, 600);
        }
        public override bool PreDraw(ref Color lightColor) {
            Color color = Main.teamColor[Main.player[Projectile.owner].team];
            spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center-Main.screenPosition, null, color, Projectile.rotation, new Vector2(42,1), Projectile.scale, SpriteEffects.None, 1);
            return false;
        }
    }
}
