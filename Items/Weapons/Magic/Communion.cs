using Origins.Buffs;
using Origins.Dev;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace Origins.Items.Weapons.Magic {
	public class Communion : ModItem, ICustomWikiStat {
        public string[] Categories => [
			"Wand"
		];
		public override void SetStaticDefaults() {
			Item.staff[Type] = true;
		}
		public override void SetDefaults() {
			Item.DefaultToMagicWeapon(ModContent.ProjectileType<Communion_P>(), 26, 10);
			Item.DamageType = DamageClass.Magic;
			Item.damage = 52;
			Item.crit = 8;
			Item.knockBack = 8;
			Item.width = 38;
			Item.height = 38;
			Item.mana = 18;
			Item.ammo = AmmoID.None;
			Item.value = Item.sellPrice(gold: 8);
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = Origins.Sounds.DefiledIdle.WithPitchRange(-0.6f, -0.4f);
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			position += velocity.SafeNormalize(default) * 80;
		}
	}
	public class Communion_P : ModProjectile {
		public override string Texture => "Terraria/Images/Item_1";
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.SnowBallFriendly);
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.extraUpdates = 0;
			Projectile.penetrate = 25;
			Projectile.hide = true;
			Projectile.alpha = 0;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 20;
		}
		public override void OnSpawn(IEntitySource source) {
			Projectile.localAI[2] = Projectile.velocity.Length();
			Projectile.localAI[0] = Projectile.localAI[0] * 0.05f;

			Projectile.ai[0] = -1;
			float maxDist = 256 * 256;
			foreach (NPC currentTarget in Main.ActiveNPCs) {
				if (!currentTarget.CanBeChasedBy()) continue;

				float currentDist = currentTarget.Center.DistanceSQ(Projectile.Center);

				if (currentDist < maxDist) {
					maxDist = currentDist;
					Projectile.ai[0] = currentTarget.whoAmI;
				}
			}
		}
		public override bool? CanHitNPC(NPC target) {
			if (Projectile.localNPCImmunity[target.whoAmI] > 0) return false;
			return null;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.velocity -= target.velocity * Math.Min(target.knockBackResist, 1);
			if (!float.IsNaN(hit.Knockback)) target.velocity -= Projectile.velocity.SafeNormalize(default) * hit.Knockback;

			Projectile.localAI[2] += Projectile.localAI[0];
			float maxDist = 256 * 256;
			Projectile.ai[0] = -1;
			foreach (NPC currentTarget in Main.ActiveNPCs) {
				if (currentTarget.whoAmI == target.whoAmI || Projectile.localNPCImmunity[currentTarget.whoAmI] > 0 || !currentTarget.CanBeChasedBy()) continue;

				float currentDist = currentTarget.Center.DistanceSQ(Projectile.Center);

				if (currentDist < maxDist) {
					maxDist = currentDist;
					Projectile.ai[0] = currentTarget.whoAmI;
				}
			}

            target.AddBuff(Rasterized_Debuff.ID, 35);
		}
		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			modifiers.HitDirectionOverride = 0;
		}
		public override void AI() {
			const float dust_speed_mult = -0.5f;
			Dust.NewDustDirect(
				Projectile.Center,
				0,
				0,
				DustID.AncientLight,
				Projectile.velocity.X * dust_speed_mult,
				Projectile.velocity.Y * dust_speed_mult,
				newColor: Color.White,
				Scale: 1f
			).noGravity = true;

			if (Projectile.ai[0] != -1) {
				NPC currentTarget = Main.npc[(int)Projectile.ai[0]];
				if (!currentTarget.CanBeChasedBy(Projectile)) goto retarget;
				Projectile.velocity = (Projectile.DirectionTo(currentTarget.Center) * 12 + Projectile.velocity).SafeNormalize(default) * Projectile.localAI[2];
				goto end;
			}

			retarget:
			float maxAngle = 0.4f;
			Vector2 direction = Projectile.velocity.SafeNormalize(default);
			foreach (NPC currentTarget in Main.ActiveNPCs) {
				if (Projectile.localNPCImmunity[currentTarget.whoAmI] > 0 || !currentTarget.CanBeChasedBy()) continue;

				float dot = Vector2.Dot(direction, Projectile.Center.DirectionTo(currentTarget.Center));
				if (dot > maxAngle) {
					maxAngle = dot;
					Projectile.ai[0] = currentTarget.whoAmI;
				}
			}
			end:;
		}
	}
}
