﻿#define ANIMATED
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Dev;
using Origins.Items.Materials;
using Origins.Items.Weapons.Ammo;
using Origins.Items.Weapons.Ammo.Canisters;
using Origins.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Demolitionist {
	public class Mine_Flayer : ModItem, ICustomWikiStat {
        public string[] Categories => new string[] {
            "HardmodeLauncher",
			"CanistahUser"
        };
        public override void SetDefaults() {
			Item.CloneDefaults(ItemID.TerraBlade);
			Item.shootsEveryUse = false;
			Item.damage = 48;
			Item.DamageType = DamageClasses.ExplosiveVersion[DamageClass.Melee];
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = 4;
			Item.useAnimation = 40;
			Item.knockBack = 4f;
			Item.useAmmo = ModContent.ItemType<Resizable_Mine_One>();
			Item.shoot = ModContent.ProjectileType<Mine_Flayer_P>();
			Item.shootSpeed = 5;
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(gold: 5);
			Item.reuseDelay = 60;
			Item.autoReuse = false;
			Item.UseSound = null;
            Item.ArmorPenetration += 2;
        }
		public override void UseStyle(Player player, Rectangle heldItemFrame) {
			if (player.reuseDelay == 0) {
				Item.useStyle = ItemUseStyleID.RaiseLamp;
			} else {
				Item.useStyle = ItemUseStyleID.Swing;
			}
		}
		public override void UseItemFrame(Player player) {
			if (player.HeldItem.type != Type) return;
			if (player.itemAnimation == player.itemTime) {
				switch ((player.itemAnimation / 4) % 3) {
					case 0:
					player.bodyFrame.Y = player.bodyFrame.Height * 3;
					break;

					case 1:
					player.bodyFrame.Y = player.bodyFrame.Height * 2;
					break;

					case 2:
					player.bodyFrame.Y = player.bodyFrame.Height;
					break;
				}
				return;
			}
		}
		public override bool CanConsumeAmmo(Item ammo, Player player) {
			return !Main.rand.NextBool(3, 5);
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Busted_Servo>(), 30);
			recipe.AddIngredient(ModContent.ItemType<Power_Core>());
			recipe.AddIngredient(ModContent.ItemType<Rotor>(), 8);
			recipe.AddTile(TileID.MythrilAnvil); //Fabricator
			recipe.Register();
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			velocity = OriginExtensions.Vec2FromPolar(player.direction == 1 ? player.itemRotation : player.itemRotation + MathHelper.Pi, velocity.Length());
			type = Item.shoot;
			SoundEngine.PlaySound(SoundID.Item61.WithPitch(0.25f), position);
		}
	}
	public class Mine_Flayer_P : ModProjectile, IIsExplodingProjectile, ICanisterProjectile {
		public override string Texture => "Terraria/Images/Item_1";
		public static AutoLoadingAsset<Texture2D> outerTexture = ICanisterProjectile.base_texture_path + "Resizable_Mine_Outer";
		public static AutoLoadingAsset<Texture2D> innerTexture = ICanisterProjectile.base_texture_path + "Resizable_Mine_Inner";
		public AutoLoadingAsset<Texture2D> OuterTexture => outerTexture;
		public AutoLoadingAsset<Texture2D> InnerTexture => innerTexture;
		public override void SetStaticDefaults() {
			Origins.MagicTripwireRange[Type] = 40;
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.ProximityMineI);
			Projectile.DamageType = DamageClasses.ExplosiveVersion[DamageClass.Melee];
			Projectile.timeLeft = 420;
			Projectile.scale = 0.5f;
			Projectile.penetrate = 1;
		}
		public override bool PreKill(int timeLeft) {
			Projectile.penetrate = -1;
			Projectile.position.X += Projectile.width / 2;
			Projectile.position.Y += Projectile.height / 2;
			Projectile.width = 96;
			Projectile.height = 96;
			Projectile.position.X -= Projectile.width / 2;
			Projectile.position.Y -= Projectile.height / 2;
			Projectile.Damage();
			ExplosiveGlobalProjectile.ExplosionVisual(Projectile, true, sound: SoundID.Item62);
			return true;
		}
		public bool IsExploding() => Projectile.penetrate == -1;
	}
}
