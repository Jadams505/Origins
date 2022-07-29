using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Origins.Items.Weapons.Fiberglass {
	public class Fiberglass_Pistol : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Fiberglass Pistol");
			Tooltip.SetDefault("Be careful, it's sharp");
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.damage = 11;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 18;
			Item.height = 36;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.value = 5000;
			Item.shootSpeed = 14;
			Item.autoReuse = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item11;
		}
        public override Vector2? HoldoutOffset() {
            return new Vector2(-0.5f,0);
        }
		public override void UseItemHitbox(Player player, ref Rectangle hitbox, ref bool noHitbox){
			hitbox.Width = player.itemWidth;
			hitbox.Height = player.itemHeight;
            hitbox.X = (int)player.itemLocation.X;
            hitbox.Y = (int)player.itemLocation.Y;
		}
    }
}
