﻿using Terraria.GameContent.Creative;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Origins.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using System;

namespace Origins.Items.Accessories {
	public class Amebic_Vial : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Amebic Vial");
			Tooltip.SetDefault("Amebic tentacles will protect you from projectiles");
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.YoYoGlove);
			Item.handOffSlot = -1;
			Item.handOnSlot = -1;
			Item.shoot = ModContent.ProjectileType<Amebic_Vial_Tentacle>();
			Item.rare = ItemRarityID.Expert;
			Item.expert = true;
			Item.canBePlacedInVanityRegardlessOfConditions = true;
		}
		public override void UpdateAccessory(Player player, bool isHidden) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			if (originPlayer.amebicVialCooldown > 0) {
				originPlayer.amebicVialVisible = false;
				return;
			}
			originPlayer.amebicVialVisible = !isHidden;
			const float maxDist = 64 * 64;
			Vector2 target = default;
			float bestWeight = 0;
			Projectile projectile;
			Vector2 currentPos;
			Vector2 diff;
			for (int i = 0; i < Main.maxProjectiles; i++) {
				projectile = Main.projectile[i];
				if (projectile.active && projectile.hostile) {
					currentPos = projectile.Hitbox.ClosestPointInRect(player.MountedCenter);
					diff = player.Hitbox.ClosestPointInRect(projectile.Center) - currentPos;
					float dist = diff.LengthSquared();
					if (dist > maxDist) continue;
					float currentWeight = Vector2.Dot(projectile.velocity, diff.SafeNormalize(default)) * dist;
					if (currentWeight > bestWeight) {
						bestWeight = currentWeight;
						target = currentPos;
					}
				}
			}
			NPC npc;
			for (int i = 0; i < Main.maxNPCs; i++) {
				npc = Main.npc[i];
				if (npc.active && npc.aiStyle == NPCAIStyleID.Spell) {
					currentPos = npc.Hitbox.ClosestPointInRect(player.MountedCenter);
					diff = player.Hitbox.ClosestPointInRect(npc.Center) - currentPos;
					float dist = diff.LengthSquared();
					if (dist > maxDist) continue;
					float currentWeight = Vector2.Dot(npc.velocity, diff.SafeNormalize(default)) * dist;
					if (currentWeight > bestWeight) {
						bestWeight = currentWeight;
						target = currentPos;
					}
				}
			}
			if (bestWeight > 0) {
				float dir = (target.Y > player.MountedCenter.Y == target.X > player.MountedCenter.X) ? -1 : 1;
				Projectile.NewProjectile(player.GetSource_Accessory(Item), player.MountedCenter, (target - player.MountedCenter).SafeNormalize(default).RotatedBy(dir * -1f) * 3.2f, Item.shoot, 1, 0, player.whoAmI, ai1:dir);
				originPlayer.amebicVialCooldown = 120;
			}
		}
		public override void UpdateVanity(Player player) {
			player.GetModPlayer<OriginPlayer>().amebicVialVisible = true;
		}
	}
	public class Amebic_Vial_Tentacle : ModProjectile {
		public override string Texture => "Origins/Items/Weapons/Riven/Flagellash_P";
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Amebic Tentacle");
		}
		public override void SetDefaults() {
			Projectile.CloneDefaults(ItemID.Spear);
			Projectile.timeLeft = 40;
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = 0;
			Projectile.tileCollide = false;
		}
		public float movementFactor {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void AI() {
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);

			Projectile.Center = ownerMountedCenter;

			if (movementFactor == 0f) {
				movementFactor = 5.1f;
				Projectile.netUpdate = true;
			}
			if (Projectile.timeLeft < 20) {
				movementFactor -= 1f;
			} else {
				movementFactor += 1f;
			}

			Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1] * 0.05f);
			Projectile.position += Projectile.velocity * movementFactor;

			Projectile.rotation = Projectile.velocity.ToRotation();
			if (Projectile.spriteDirection == 1) {
				Projectile.rotation -= MathHelper.PiOver2;
			}
			Projectile other;
			for (int i = 0; i < Main.maxProjectiles; i++) {
				other = Main.projectile[i];
				if (other.active && other.hostile && (Colliding(Projectile.Hitbox, other.Hitbox)??false)) {
					other.velocity = Vector2.Lerp(other.velocity, Projectile.velocity, 0.5f);
				}
			}
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (projHitbox.Intersects(targetHitbox) || Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Main.player[Projectile.owner].MountedCenter + Projectile.velocity * 2, Projectile.Center)) {
				return true;
			}
			return false;
		}
		public override bool PreDraw(ref Color lightColor) {
			Player projOwner = Main.player[Projectile.owner];
			Vector2 ownerMountedCenter = projOwner.RotatedRelativePoint(projOwner.MountedCenter, true);
			Vector2 diff = Projectile.Center - ownerMountedCenter;
			int dist = (int)Math.Min(diff.Length(), 180);
			Main.EntitySpriteDraw(
				TextureAssets.Projectile[Type].Value,
				ownerMountedCenter - Main.screenPosition,
				new Rectangle(0, 180 - dist, 6, dist),
				Color.White,
				Projectile.rotation,
				new Vector2(3, 0),
				Projectile.scale,
				SpriteEffects.None,
			0);
			return false;
		}
	}
}
