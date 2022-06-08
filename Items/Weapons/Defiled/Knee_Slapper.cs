using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.NPCs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Tyfyter.Utils;

namespace Origins.Items.Weapons.Defiled {
	public class Knee_Slapper : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Knee Slapper");
			Tooltip.SetDefault("'How does the fish feel about this?'");
		}
		public override void SetDefaults() {
			Item.damage = 45;
			Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
			Item.width = 30;
			Item.height = 36;
			Item.useTime = 17;
			Item.useAnimation = 17;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5;
            Item.shoot = ModContent.ProjectileType<Knee_Slapper_P>();
			Item.shootSpeed = 16f;
			Item.value = 5000;
            Item.useTurn = true;
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
		}
		public override int ChoosePrefix(UnifiedRandom rand) {
			if (Item.noUseGraphic) {
				Item.noUseGraphic = false;
				Item.Prefix(-2);
				Item.noUseGraphic = true;
				return Item.prefix;
			}
			return -1;
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			Projectile proj = Projectile.NewProjectileDirect(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, player.itemAnimationMax, velocity.ToRotation());
			proj.scale = Item.scale;
			return false;
		}
	}
    public class Knee_Slapper_P : ModProjectile {
		static bool lastSlapDir = false;
        public override string Texture => "Origins/Items/Weapons/Defiled/Infusion_P";
		public List<PolarVec2> nodes;
		PolarVec2 GetSwingStartOffset => new PolarVec2(0, Projectile.ai[1] - Projectile.direction * 0.35f);//-MathHelper.PiOver2 + projectile.direction * 0.35f
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Knee Slapper");
		}
		public override void SetDefaults() {
			Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.timeLeft = 40;
			Projectile.localNPCHitCooldown = 15;
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.extraUpdates = 3;
			//projectile.hide = true;
			Projectile.ownerHitCheck = false;
			Projectile.tileCollide = false;
		}
		public override void AI() {
			Player owner = Main.player[Projectile.owner];
			if (nodes is null) {
				Projectile.timeLeft = (int)(Projectile.ai[0] * 4);
				Projectile.localNPCHitCooldown = Projectile.timeLeft;
				Projectile.localAI[1] = 16f / Projectile.ai[0];
				Projectile.localAI[0] = owner.direction;
				Projectile.ai[0] = Projectile.direction = (lastSlapDir = !lastSlapDir) ? 1 : -1;
				//projectile.ai[1] = (Main.MouseWorld - owner.MountedCenter).ToRotation();
				float offset = Projectile.direction * 0.1f * Projectile.localAI[1];
				nodes = new List<PolarVec2>() {
					new PolarVec2(24 * Projectile.scale, -offset),
					new PolarVec2(24 * Projectile.scale, -offset * 2),
					new PolarVec2(24 * Projectile.scale, -offset * 3),
					new PolarVec2(24 * Projectile.scale, -offset * 4),
					new PolarVec2(36 * Projectile.scale, -offset * 5)
				};
			}
			owner.direction = (int)Projectile.localAI[0];
			Projectile.direction = (int)Projectile.ai[0];
			Vector2 basePosition = owner.MountedCenter;
			//Vector2 position = default;
			PolarVec2 position = GetSwingStartOffset;
			for (int i = 0; i < nodes.Count; i++) {
				PolarVec2 vec = nodes[i];
				position.R += vec.R;
				position.Theta += vec.Theta;
				//position += (Vector2)vec;
				//Dust.NewDustPerfect(basePosition + (Vector2)position, 6, Vector2.Zero).noGravity = true;
				vec.Theta += Projectile.direction * 0.015f * Projectile.localAI[1];// / (i + 1);
				nodes[i] = vec;
			}
			owner.heldProj = Projectile.whoAmI;
			/*
			Vector2 basePosition = owner.MountedCenter;
			Vector2 position = default;
			//PolarVec2 position = GetSwingStartOffset;
			for (int i = 0; i < nodes.Count; i++) {
				PolarVec2 vec = nodes[i];
				//position.R += vec.R;
				//position.Theta += vec.Theta;
				position += (Vector2)vec;
				Dust.NewDustPerfect(basePosition + (Vector2)position, 6, Vector2.Zero).noGravity = true;
				vec.Theta += projectile.direction * 0.015f * projectile.localAI[1] * i;// / (i + 1);
				nodes[i] = vec;
			}*/
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			Projectile.direction = (int)Projectile.ai[0];
			Player owner = Main.player[Projectile.owner];
			Vector2 basePosition = owner.MountedCenter;
			PolarVec2 position = GetSwingStartOffset;
			Vector2 lastPosition = basePosition;
			for (int i = 0; i < nodes.Count; i++) {
				PolarVec2 vec = nodes[i];
				position.R += vec.R;
				position.Theta += vec.Theta;
				float collisionPoint = 0;
				if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), lastPosition, basePosition + (Vector2)position, 8, ref collisionPoint)) {
					return true;
				}
				lastPosition = basePosition + (Vector2)position;
			}
			return false;
		}
		public override bool PreDraw(ref Color lightColor) {
			Projectile.direction = (int)Projectile.ai[0];
			Player owner = Main.player[Projectile.owner];
			Vector2 basePosition = owner.MountedCenter;
			PolarVec2 position = GetSwingStartOffset;
			Vector2 lastPosition = basePosition;
			Texture2D texture = Mod.Assets.Request<Texture2D>("Items/Weapons/Defiled/Knee_Slapper_Body").Value;
			PolarVec2 diff = default;
			for (int i = 0; i < nodes.Count - 1; i++) {
				PolarVec2 vec = nodes[i];
				position.R += vec.R;
				position.Theta += vec.Theta;
				//Dust.NewDustPerfect(lastPosition, 6, Vector2.Zero).noGravity = true;
				diff = (PolarVec2)(basePosition + (Vector2)position - lastPosition);
				if (i == 0) {
					Main.EntitySpriteDraw(
						Mod.Assets.Request<Texture2D>("Items/Weapons/Defiled/Knee_Slapper_Tail").Value,
						lastPosition - Main.screenPosition,
						null,
						new Color(Lighting.GetSubLight(lastPosition)),
						diff.Theta,
						new Vector2(-8, 21),
						new Vector2(diff.R / 30f, 0.9f) * Projectile.scale,
						SpriteEffects.None,
						0);
				} else {
					Main.EntitySpriteDraw(
						texture,
						lastPosition - Main.screenPosition,
						null,
						new Color(Lighting.GetSubLight(lastPosition)),
						diff.Theta,
						new Vector2(2, 21),
						new Vector2(diff.R / 30f, 0.9f) * Projectile.scale,
						(i % 2 == 0) ? SpriteEffects.None : SpriteEffects.FlipVertically,
						0);
				}
				lastPosition = basePosition + (Vector2)position;
			}

			Main.EntitySpriteDraw(
				Mod.Assets.Request<Texture2D>("Items/Weapons/Defiled/Knee_Slapper_Head").Value,
				lastPosition - Main.screenPosition,
				null,
				new Color(Lighting.GetSubLight(lastPosition)),
				diff.Theta,
				new Vector2(2, 21),
				new Vector2(1, 0.9f) * Projectile.scale,
				SpriteEffects.None,
				0);
			return false;
		}
	}
}
