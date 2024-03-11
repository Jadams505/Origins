using Microsoft.Xna.Framework;
using Origins.Buffs;
using Origins.Items.Weapons.Ammo;
using Origins.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

using Origins.Dev;
namespace Origins.Items.Weapons.Demolitionist {
	public class Shrapnel_Bomb : ModItem, ICustomWikiStat {
        public string[] Categories => new string[] {
            "ThrownExplosive",
			"IsBomb"
        };
        public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.Bomb);
			Item.damage = 50;
			Item.useTime = (int)(Item.useTime * 1.15);
			Item.useAnimation = (int)(Item.useAnimation * 1.15);
			Item.shoot = ModContent.ProjectileType<Shrapnel_Bomb_P>();
			Item.shootSpeed *= 0.95f;
			Item.knockBack = 13f;
			Item.value = Item.sellPrice(silver: 1, copper: 80);
			Item.rare = ItemRarityID.Blue;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type, 4);
			recipe.AddIngredient(ItemID.Bomb, 4);
			recipe.AddIngredient(ModContent.ItemType<Scrap>());
			recipe.Register();
		}
	}
	public class Shrapnel_Bomb_P : ModProjectile {
		public override string Texture => "Origins/Items/Weapons/Demolitionist/Shrapnel_Bomb";
		public override void SetStaticDefaults() {
			Origins.MagicTripwireRange[Type] = 32;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Bomb);
			Projectile.penetrate = 1;
			Projectile.timeLeft = 135;
		}
		public override bool PreKill(int timeLeft) {
			Projectile.type = ProjectileID.Bomb;
			return true;
		}
		public override void OnKill(int timeLeft) {
			Projectile.position.X += Projectile.width / 2;
			Projectile.position.Y += Projectile.height / 2;
			Projectile.width = 128;
			Projectile.height = 128;
			Projectile.position.X -= Projectile.width / 2;
			Projectile.position.Y -= Projectile.height / 2;
			Projectile.Damage();
            ExplosiveGlobalProjectile.DealSelfDamage(Projectile);
            SoundEngine.PlaySound(Origins.Sounds.ShrapnelFest, Projectile.Center);
            Vector2 v;
			for (int i = 4; i-- > 0;) {
				v = Main.rand.NextVector2Unit() * 4;
				Projectile.NewProjectile(
					Projectile.GetSource_FromThis(),
					Projectile.Center + v * 8,
					v,
					ModContent.ProjectileType<Impeding_Shrapnel_Shard>(),
					Projectile.damage / 2,
					Projectile.knockBack / 4,
					Projectile.owner,
					ai2: 0.5f
				);
			}
		}
	}
}
