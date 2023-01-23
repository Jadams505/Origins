using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Ranged {
    public class Ancient_Kruncher : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Ancient Kruncher");
            Tooltip.SetDefault("Wh3re did you f!nd this?");
            SacrificeTotal = 1;
        }
        public override void SetDefaults() {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.noMelee = true;
            Item.crit = -1;
            Item.width = 56;
            Item.height = 18;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.shootSpeed = 15f;
            Item.shoot = ProjectileID.Bullet;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTurn = false;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = Origins.Sounds.Krunch;
        }
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
		    for(int i = 0; i<5; i++)Projectile.NewProjectile(source, position, velocity.RotatedByRandom(i/10f), type, damage, knockback, player.whoAmI);
            return false;
        }
	}
}