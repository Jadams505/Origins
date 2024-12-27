using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Origins.Dev;
using Microsoft.Xna.Framework;
using Origins.Reflection;
using Origins.Buffs;
using Origins.Items.Materials;

namespace Origins.Items.Weapons.Magic {
    public class Eternanite : ModItem, ICustomWikiStat {
        public string[] Categories => [
            "UsesBookcase",
            "SpellBook"
        ];
        public override void SetDefaults() {
			Item.CloneDefaults(ItemID.Flamethrower);
			Item.damage = 38;
			Item.DamageType = DamageClass.Magic;
			Item.shoot = ModContent.ProjectileType<Eternanite_P>();
			Item.mana = 14;
			Item.useAmmo = AmmoID.None;
			Item.noUseGraphic = false;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.knockBack = 8;
			Item.shootSpeed = 14f;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item82;
		}
		public override void AddRecipes() {
			Recipe.Create(Type)
			.AddIngredient<Eternabrite>()
			.AddIngredient<Black_Bile>(5)
			.AddIngredient(ItemID.SoulofNight, 5)
			.AddTile(TileID.Bookcases)
			.Register();
		}
    }
	public class Eternanite_P : ModProjectile {
		public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.Flames;
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Flames);
			Projectile.DamageType = DamageClass.Magic;
			Projectile.aiStyle = 0;
			//AIType = ProjectileID.Flames;
		}
		public override void AI() {
			Projectile.localAI[0] += 1f;
			const int endTime = 60;
			const int startTime = 12;
			const int totalTime = endTime + startTime;
			if (Projectile.localAI[0] >= totalTime) {
				Projectile.Kill();
			}
			if (Projectile.localAI[0] >= endTime) {
				Projectile.velocity *= 0.95f;
			}
			if (Projectile.localAI[0] < 50 && Main.rand.NextBool(4)) {
				Dust dust = Dust.NewDustDirect(
					Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Projectile.localAI[0], 0f, 72f, 0.5f, 1f),
					4, 4,
					DustID.Asphalt,
					Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f,
					100
				);
				if (Main.rand.NextBool(4)) {
					dust.noGravity = true;
					dust.scale *= 2f;
					dust.velocity.X *= 2f;
					dust.velocity.Y *= 2f;
				}
				dust.scale *= 1.25f;
				dust.velocity *= 1.2f;
				dust.velocity += Projectile.velocity * 1f * Utils.Remap(Projectile.localAI[0], 0f, endTime * 0.75f, 1f, 0.1f) * Utils.Remap(Projectile.localAI[0], 0f, endTime * 0.1f, 0.1f, 1f);
			}
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(Rasterized_Debuff.ID, 24);
		}
		public override bool PreDraw(ref Color lightColor) {
			Projectile.DrawFlamethrower(new(22, 18, 33), new(31, 26, 45), new(43, 37, 61), new(22, 18, 33));
			Color black = Color.Black * 0.75f;
			Projectile.DrawFlamethrower(black, black, black, black, scale: 0.75f);
			return false;
		}
		public override bool OnTileCollide(Vector2 oldVelocity) {
			Projectile.velocity = Vector2.Zero;
			return false;
		}
	}
}