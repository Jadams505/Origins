using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Origins.Items.Weapons.Demolitionist {
	public class Flakjack : ModItem, ICustomDrawItem {
		public static AutoCastingAsset<Texture2D> UseTexture { get; private set; }
		public static AutoCastingAsset<Texture2D> UseGlowTexture { get; private set; }
		public override void Unload() {
			UseTexture = null;
			UseGlowTexture = null;
		}
		static short glowmask;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Flakjack");
			if (!Main.dedServ) {
				UseTexture = Mod.Assets.Request<Texture2D>("Items/Weapons/Demolitionist/Flakjack_Use");
				UseGlowTexture = Mod.Assets.Request<Texture2D>("Items/Weapons/Demolitionist/Flakjack_Use_Glow");
			}
			glowmask = Origins.AddGlowMask(this);
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.SniperRifle);
			Item.DamageType = DamageClasses.ExplosiveVersion[DamageClass.Ranged];
			Item.glowMask = glowmask;
			Item.damage = 96;
			Item.crit = 14;
			Item.useAnimation = 32;
			Item.useTime = 17;
			Item.useAmmo = ModContent.ItemType<Ammo.Resizable_Mine_One>();
			Item.shoot = ModContent.ProjectileType<Flakjack_P_1>();
			Item.shootSpeed = 12;
			Item.reuseDelay = 6;
			Item.autoReuse = true;
			Item.value = Item.sellPrice(gold: 5);
			Item.rare = CrimsonRarity.ID;
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			SoundEngine.PlaySound(SoundID.Item40, position);
			SoundEngine.PlaySound(SoundID.Item36.WithVolume(0.75f), position);
			OriginGlobalProj.extraUpdatesNext = 2;
			Vector2 perp = velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(default);
			if (player.ItemUsesThisAnimation == 1) {
				position += perp * player.direction * 2;
			} else {
				position -= perp * player.direction * 6;
			}
			type += Item.shoot - 1;
		}

		public void DrawInHand(Texture2D itemTexture, ref PlayerDrawSet drawInfo, Vector2 itemCenter, Color lightColor, Vector2 drawOrigin) {
			Player drawPlayer = drawInfo.drawPlayer;
			float itemRotation = drawPlayer.itemRotation;
			float scale = drawPlayer.GetAdjustedItemScale(Item);

			Vector2 pos = new Vector2((int)(drawInfo.ItemLocation.X - Main.screenPosition.X + itemCenter.X), (int)(drawInfo.ItemLocation.Y - Main.screenPosition.Y + itemCenter.Y));

			int frame = 0;
			int useFrame = drawPlayer.itemTimeMax - drawPlayer.itemTime;
			switch (drawPlayer.ItemUsesThisAnimation) {
				case 1:
				if (useFrame < 3) {
					frame += 1;
				} else if (useFrame < 6) {
					frame += 2;
				} else {
					frame += 3;
				}
				break;

				case 2:
				frame = 3;
				goto case 1;
			}
			frame %= 6;

			drawInfo.DrawDataCache.Add(new DrawData(
				UseTexture,
				pos,
				new Rectangle(0, 30 * frame, 72, 28),
				Item.GetAlpha(lightColor),
				itemRotation,
				drawOrigin,
				scale,
				drawInfo.itemEffect,
			0));

			drawInfo.DrawDataCache.Add(new DrawData(
				UseGlowTexture,
				pos,
				new Rectangle(0, 30 * frame, 72, 28),
				Item.GetAlpha(Color.White),
				itemRotation,
				drawOrigin,
				scale,
				drawInfo.itemEffect,
			0));
		}
	}
	public class Flakjack_Explosion_P : ModProjectile, IIsExplodingProjectile {
		public override string Texture => "Terraria/Images/Projectile_16";
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Flakjack");
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.ProximityMineI);
			Projectile.timeLeft = 5;
			Projectile.penetrate = -1;
			Projectile.aiStyle = 0;
			Projectile.width = 96;
			Projectile.height = 96;
			Projectile.hide = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.usesLocalNPCImmunity = true;
		}
		public override void AI() {
			if (Projectile.ai[0] == 0) {
				ExplosiveGlobalProjectile.ExplosionVisual(
					Projectile,
					true,
					sound: SoundID.Item62
				);
				Projectile.ai[0] = 1;
			}
		}
		public void Explode(int delay = 0) { }
		public bool IsExploding() => true;
	}
	public class Flakjack_P_1 : ModProjectile, IIsExplodingProjectile {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Resizable Mine");
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.ProximityMineI);
			Projectile.MaxUpdates = 10;
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.timeLeft = 420;
			Projectile.penetrate = 1;
			Projectile.aiStyle = 0;
			Projectile.alpha = 255;
		}
		public override void OnSpawn(IEntitySource source) {
			Projectile.velocity *= 0.2f;
		}
		public override void AI() {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			const int magicTripwireRange = 40;

			Rectangle magicTripwireHitbox = new Rectangle(
				(int)Projectile.Center.X - magicTripwireRange,
				(int)Projectile.Center.Y - magicTripwireRange,
				magicTripwireRange * 2,
				magicTripwireRange * 2
			);
			bool tripped = false;
			for (int i = 0; i < Main.maxNPCs; i++) {
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy() && !npc.collideY && magicTripwireHitbox.Intersects(npc.Hitbox)) {
					tripped = true;
				}
			}
			ExplosiveGlobalProjectile global = Projectile.GetGlobalProjectile<ExplosiveGlobalProjectile>();
			if (tripped) {
				global.magicTripwireTripped = true;
			} else if (global.magicTripwireTripped) {
				(this as IIsExplodingProjectile).Explode(0);
			}
			if (Projectile.alpha > 0)
				Projectile.alpha -= 15;
			if (Projectile.alpha < 0)
				Projectile.alpha = 0;
		}
		public override Color? GetAlpha(Color lightColor) {
			if (Projectile.alpha < 200) {
				return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, (255 - Projectile.alpha) / 2);
			}
			return Color.Transparent;
		}
		public override void Kill(int timeLeft) {
			if (Projectile.owner == Main.myPlayer) {
				Projectile.NewProjectileDirect(
					Projectile.GetSource_Death(),
					Projectile.Center,
					default,
					ModContent.ProjectileType<Flakjack_Explosion_P>(),
					Projectile.damage,
					Projectile.knockBack,
					Projectile.owner
				);
			}
		}
		public bool IsExploding() => false;
	}
	public class Flakjack_P_2 : Flakjack_P_1 { }
	public class Flakjack_P_3 : Flakjack_P_1 { }
	public class Flakjack_P_4 : Flakjack_P_1 { }
	public class Flakjack_P_5 : Flakjack_P_1 { }
}
