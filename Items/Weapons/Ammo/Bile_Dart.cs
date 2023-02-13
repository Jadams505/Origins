using Microsoft.Xna.Framework;
using Origins.Buffs;
using Origins.Items.Materials;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Ammo {
	public class Bile_Dart : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Bile Dart");
			Tooltip.SetDefault("Stuns the target");
			SacrificeTotal = 99;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.CursedDart);
			Item.maxStack = 999;
			Item.damage = 9;
			Item.shoot = ModContent.ProjectileType<Bile_Dart_P>();
			Item.shootSpeed = 3f;
			Item.knockBack = 2.2f;
			Item.value = Item.sellPrice(copper: 6);
			Item.rare = ItemRarityID.Orange;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type, 100);
			recipe.AddIngredient(ModContent.ItemType<Black_Bile>());
			recipe.Register();
		}
	}
	public class Bile_Dart_P : ModProjectile {
		public override string Texture => "Origins/Items/Weapons/Ammo/Bile_Dart";
		public override void SetDefaults() {
            Projectile.CloneDefaults(ProjectileID.CursedDart);
        }
		public override void AI() {
			Projectile.localAI[0] += 1f;
			if (Projectile.localAI[0] > 3f)
				Projectile.alpha = 0;

			if (Projectile.ai[0] >= 20f) {
				Projectile.ai[0] = 20f;
				Projectile.velocity.Y += 0.075f;
			}
		}
		public override Color? GetAlpha(Color lightColor) {
			return Projectile.alpha == 0 ? new Color(255, 255, 255, 200) : Color.Transparent;
		}
		public override void Kill(int timeLeft) {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            SoundEngine.PlaySound(SoundID.NPCHit22.WithVolume(0.5f), Projectile.position);
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
            target.AddBuff(ModContent.BuffType<Rasterized_Debuff>(), 20);
        }
    }
}
