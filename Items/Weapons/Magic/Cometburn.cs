﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Magic {
    public class Cometburn : ModItem {
        protected override bool CloneNewInstances => true;
        float shootSpeed;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Cometburn");
            Tooltip.SetDefault("Prompts comets to turn into meteorites");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.MeteorStaff);
            Item.damage = 99;
            Item.DamageType = DamageClass.Magic;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.width = 58;
            Item.height = 58;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.knockBack = 9.5f;
            Item.value = 500000;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = Cometburn_P.ID;
            Item.shootSpeed = 10f;
            Item.autoReuse = true;
            Item.mana = 15;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Boulder);
            recipe.AddIngredient(ModContent.ItemType<Space_Goo>(), 20);
            recipe.AddIngredient(ModContent.ItemType<Space_Rock>(), 15);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			for (int i = 0; i < 3; i++) {
                Vector2 speed = new Vector2(0, Item.shootSpeed).RotatedByRandom(1);
                Projectile.NewProjectile(source, Main.MouseWorld - new Vector2(0, 72) - (speed * 80), speed, type, damage, knockback, player.whoAmI, ai1:Main.MouseWorld.Y);
            }
            return false;
        }
    }
    public class Cometburn_P : ModProjectile {
        public static int ID { get; private set; }
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Fallen Comet");
            ID = Projectile.type;
        }
        public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.Meteor1);
            Projectile.width = 42;
            Projectile.height = 44;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
        }
		public override void AI() {
            Lighting.AddLight(Projectile.Center, 0, 0.5f, 0);
            if (Main.rand.NextBool(9)) {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0, 0, 100, new Color(0, 255, 0), 0.5f);
                dust.shader = GameShaders.Armor.GetSecondaryShader(18, Main.LocalPlayer);
                dust.fadeIn = Main.rand.NextFloat(0.1f);
                dust.noGravity = false;
                dust.noLight = true;
            }
			if (Projectile.Center.Y > Projectile.ai[1]) {
                Projectile.tileCollide = true;
			}
        }
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            if(Main.rand.NextBool(3)) target.AddBuff(BuffID.VortexDebuff, 720);//placeholder
        }
        public override void OnHitPlayer(Player target, int damage, bool crit) {
            if (Main.rand.NextBool(3)) target.AddBuff(BuffID.VortexDebuff, 720);//placeholder
        }
    }
}