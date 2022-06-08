using Origins.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;

namespace Origins.Items.Tools {
	public class Lightning_Rod : ModItem {

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Lightning Rod");
			//Tooltip.SetDefault("Can fish in lava.");
			//ItemID.Sets.CanFishInLava[item.type] = true;
		}

		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ReinforcedFishingPole);
			Item.damage = 0;
			Item.DamageType = DamageClass.Generic;
			Item.noMelee = true;
			//Sets the poles fishing power
			Item.fishingPole = 37;
			//Wooden Fishing Pole is 9f and Golden Fishing Rod is 17f
			Item.shootSpeed = 13.7f;
			Item.shoot = ModContent.ProjectileType<Lightning_Rod_Bobber>();
		}
		public override void ModifyWeaponDamage(Player player, ref StatModifier damage) {
			damage = new StatModifier(
				(damage.Additive - 1) * 1.5f + 1,
				(damage.Multiplicative - 1) * 1.5f + 1,
				0,
				damage.Flat * 1.5f + damage.Base * 1.5f
			);
		}
		public override void AddRecipes() {
			Recipe recipe = Mod.CreateRecipe(Type);
			recipe.AddIngredient(ModContent.ItemType<Felnum_Bar>(), 8);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	public class Lightning_Rod_Bobber : ModProjectile {

		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Lightning Rod Bobber");
		}

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.BobberReinforced);
			DrawOriginOffsetY = -8;
			Projectile.friendly = true;
		}

		public override bool PreDrawExtras() {
			Color fishingLineColor = new Color(176, 225, 255);
			Lighting.AddLight(Projectile.Center, fishingLineColor.R / 255, fishingLineColor.G / 255, fishingLineColor.B / 255);

			//Change these two values in order to change the origin of where the line is being drawn
			int xPositionAdditive = 45;
			float yPositionAdditive = 35f;

			Player player = Main.player[Projectile.owner];
			if (!Projectile.bobber || player.inventory[player.selectedItem].holdStyle <= 0)
				return false;

			Vector2 lineOrigin = player.MountedCenter;
			lineOrigin.Y += player.gfxOffY;
			int type = player.inventory[player.selectedItem].type;
			//This variable is used to account for Gravitation Potions
			float gravity = player.gravDir;

			if (type == ModContent.ItemType<Lightning_Rod>()) {
				lineOrigin.X += xPositionAdditive * player.direction;
				if (player.direction < 0) {
					lineOrigin.X -= 13f;
				}
				lineOrigin.Y -= yPositionAdditive * gravity;
			}

			if (gravity == -1f) {
				lineOrigin.Y -= 12f;
			}
			// RotatedRelativePoint adjusts lineOrigin to account for player rotation.
			lineOrigin = player.RotatedRelativePoint(lineOrigin + new Vector2(8f), true) - new Vector2(8f);
			Vector2 playerToProjectile = Projectile.Center - lineOrigin;
			bool canDraw = true;
			if (playerToProjectile.X == 0f && playerToProjectile.Y == 0f)
				return false;

			float playerToProjectileMagnitude = playerToProjectile.Length();
			playerToProjectileMagnitude = 12f / playerToProjectileMagnitude;
			playerToProjectile *= playerToProjectileMagnitude;
			lineOrigin -= playerToProjectile;
			playerToProjectile = Projectile.Center - lineOrigin;

			Vector2 lastArcPos = lineOrigin;

			// This math draws the line, while allowing the line to sag.
			int index = 0;
			while (canDraw) {
				index += 1;
				float height = 12f;
				float positionMagnitude = playerToProjectile.Length();
				if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
					break;

				if (positionMagnitude < 20f) {
					height = positionMagnitude - 8f;
					canDraw = false;
				}
				playerToProjectile *= 12f / positionMagnitude;
				lineOrigin += playerToProjectile;
				playerToProjectile.X = Projectile.position.X + Projectile.width * 0.5f - lineOrigin.X;
				playerToProjectile.Y = Projectile.position.Y + Projectile.height * 0.1f - lineOrigin.Y;
				if (positionMagnitude > 12f) {
					float positionInverseMultiplier = 0.3f;
					float absVelocitySum = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
					if (absVelocitySum > 16f) {
						absVelocitySum = 16f;
					}
					absVelocitySum = 1f - absVelocitySum / 16f;
					positionInverseMultiplier *= absVelocitySum;
					absVelocitySum = positionMagnitude / 80f;
					if (absVelocitySum > 1f) {
						absVelocitySum = 1f;
					}
					positionInverseMultiplier *= absVelocitySum;
					if (positionInverseMultiplier < 0f) {
						positionInverseMultiplier = 0f;
					}
					absVelocitySum = 1f - Projectile.localAI[0] / 100f;
					positionInverseMultiplier *= absVelocitySum;
					if (playerToProjectile.Y > 0f) {
						playerToProjectile.Y *= 1f + positionInverseMultiplier;
						playerToProjectile.X *= 1f - positionInverseMultiplier;
					}
					else {
						absVelocitySum = Math.Abs(Projectile.velocity.X) / 3f;
						if (absVelocitySum > 1f) {
							absVelocitySum = 1f;
						}
						absVelocitySum -= 0.5f;
						positionInverseMultiplier *= absVelocitySum;
						if (positionInverseMultiplier > 0f) {
							positionInverseMultiplier *= 2f;
						}
						playerToProjectile.Y *= 1f + positionInverseMultiplier;
						playerToProjectile.X *= 1f - positionInverseMultiplier;
					}
				}
				//This color decides the color of the fishing line. The color is randomized as decided in the AI.
				Color lineColor = Lighting.GetColor((int)lineOrigin.X / 16, (int)(lineOrigin.Y / 16f), fishingLineColor);
				float rotation = playerToProjectile.ToRotation() - MathHelper.PiOver2;
				Texture2D fishingLineTexture = TextureAssets.FishingLine.Value;
				Main.spriteBatch.Draw(fishingLineTexture, new Vector2(lineOrigin.X - Main.screenPosition.X + fishingLineTexture.Width * 0.5f, lineOrigin.Y - Main.screenPosition.Y + fishingLineTexture.Height * 0.5f), new Rectangle(0, 0, fishingLineTexture.Width, (int)height), lineColor, rotation, new Vector2(fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
			}
			return false;
		}
	}
}