using Microsoft.Xna.Framework;
using Origins.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Origins.Dev;
using Origins.Projectiles;
using Terraria.Audio;

namespace Origins.Items.Weapons.Demolitionist {
    public class Hellfire_Bomb : ModItem, ICustomWikiStat {
		static short glowmask;
        public string[] Categories => new string[] {
            "ThrownExplosive",
			"IsBomb",
            "SpendableWeapon"
        };
        public override void SetStaticDefaults() {
			glowmask = Origins.AddGlowMask(this);
			Item.ResearchUnlockCount = 99;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.Bomb);
			Item.damage = 65;
			Item.shoot = ModContent.ProjectileType<Hellfire_Bomb_P>();
			Item.value *= 9;
			Item.rare = ItemRarityID.Orange;
			Item.glowMask = glowmask;
            Item.ArmorPenetration += 1;
        }
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type, 15);
			recipe.AddIngredient(ItemID.Bomb, 15);
			recipe.AddIngredient(ItemID.Fireblossom);
			recipe.AddIngredient(ItemID.Hellstone, 5);
			recipe.Register();
		}
	}
	public class Hellfire_Bomb_P : ModProjectile {
		public override string Texture => "Origins/Items/Weapons/Demolitionist/Hellfire_Bomb";
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
			Projectile.NewProjectile(
				Projectile.GetSource_Death(),
				Projectile.Center,
				default,
				ModContent.ProjectileType<Hellfire_Bomb_Fire>(),
				Projectile.damage,
				Projectile.knockBack,
				Projectile.owner
			);
		}
	}
	public class Hellfire_Bomb_Fire : ExplosionProjectile {
		public override DamageClass DamageType => DamageClasses.ThrownExplosive;
		public override int Size => 128;
		public override SoundStyle? Sound => null;
		public override int FireDustAmount => 2;
		public override int SmokeDustAmount => 1;
		public override int SmokeGoreAmount => 0;
		public override void SetDefaults() {
			base.SetDefaults();
			Projectile.timeLeft = 60;
			Projectile.usesLocalNPCImmunity = false;
			Projectile.usesIDStaticNPCImmunity = false;
			Projectile.idStaticNPCHitCooldown = 6;
		}
		public override void AI() {
			base.AI();
			Projectile.ai[0] = 0;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(BuffID.OnFire3, 180);
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo info) {
			target.AddBuff(BuffID.OnFire3, 180);
		}
	}
}
