﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Items;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;
using Tyfyter.Utils;

namespace Origins.Projectiles {
	//separate global for organization, might also make non-artifact projectiles less laggy than the alternative
	public class ArtifactMinionGlobalProjectile : GlobalProjectile {
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => false;
		bool isRespawned = false;
		public StatModifier maxHealthModifier = StatModifier.Default;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
			if (entity.ModProjectile is IArtifactMinion) Origins.ArtifactMinion[entity.type] = true;
			return Origins.ArtifactMinion[entity.type];
		}
		public override void SetDefaults(Projectile projectile) {
			isRespawned = false;
		}
		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			ModPrefix prefix = null;
			if (source is EntitySource_ItemUse itemUseSource) {
				prefix = PrefixLoader.GetPrefix(itemUseSource.Item.prefix);
			} else if (source is EntitySource_Parent source_Parent) {
				if (source_Parent.Entity is Projectile parentProjectile) {
					prefix = parentProjectile.TryGetGlobalProjectile(out OriginGlobalProj parent) ? parent.prefix : null;
				}
			}
			if (projectile.ModProjectile is IArtifactMinion artifact) {
				if (prefix is ArtifactMinionPrefix artifactPrefix) artifact.MaxLife = (int)artifactPrefix.MaxLifeModifier.ApplyTo(artifact.MaxLife);
				artifact.Life = artifact.MaxLife;
			}
		}
		public override void PostAI(Projectile projectile) {
			if (projectile.ModProjectile is IArtifactMinion artifact && artifact.Life <= 0) {
				artifact.Life = 0;
				if (artifact.CanDie) {
					projectile.Kill();
				}
			}
		}
		public bool CanRespawn(Projectile projectile) {
			return !isRespawned && Main.player[projectile.owner].GetModPlayer<OriginPlayer>().spiritShard;
		}
		public override void OnKill(Projectile projectile, int timeLeft) {
			if (projectile.ModProjectile is ICustomRespawnArtifact customRespawnArtifact) {
				customRespawnArtifact.Respawn();
			} else {
				if (CanRespawn(projectile)) {
					//basically just stops the old one from counting for one frame
					//done this way since maxMinions is the only thing that's always read for minion slot code
					Main.player[projectile.owner].maxMinions += (int)projectile.minionSlots + 1;
					Projectile proj = Projectile.NewProjectileDirect(
						projectile.GetSource_Death(),
						projectile.Center,
						projectile.velocity,
						projectile.type,
						projectile.originalDamage,
						projectile.knockBack,
						projectile.owner
					);
					proj.originalDamage = projectile.originalDamage;
					proj.GetGlobalProjectile<ArtifactMinionGlobalProjectile>().isRespawned = true;
				}
			}
			if (projectile.TryGetGlobalProjectile(out OriginGlobalProj self) && self.prefix is ArtifactMinionPrefix artifactPrefix) {
				artifactPrefix.OnKill(projectile);
			}
		}
		public override Color? GetAlpha(Projectile projectile, Color lightColor) {
			if (isRespawned) return new Color(175, 225, 255, 128);
			return null;
		}
		public override void PostDraw(Projectile projectile, Color lightColor) {
			if (projectile.owner == Main.myPlayer && projectile.ModProjectile is IArtifactMinion) {
				ArtifactMinionSystem.artifactMinions.Add(projectile.whoAmI);
			}
		}
		public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
			if (projectile.ModProjectile is IArtifactMinion artifact) {
				binaryWriter.Write(artifact.Life);
			}
		}
		public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
			if (projectile.ModProjectile is IArtifactMinion artifact) {
				artifact.Life = binaryReader.ReadSingle();
			}
		}
	}
	public class ArtifactMinionSystem : ModSystem {
		public static List<int> artifactMinions = [];
		public override void Unload() => artifactMinions = null;
		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Entity Health Bars"));
			if (inventoryIndex != -1) {//error prevention & null check
				layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
					"Origins: Artifact Minion Health Bars",
					delegate {
						for (int i = 0; i < artifactMinions.Count; i++) {
							Projectile projectile = Main.projectile[artifactMinions[i]];
							if (projectile.ModProjectile is IArtifactMinion artifact) {
								Vector2 position = default;
								if (Main.HealthBarDrawSettings != 0 && artifact.Life < artifact.MaxLife) {
									if (Main.HealthBarDrawSettings == 1) {
										position = projectile.Bottom + new Vector2(0, projectile.gfxOffY + 10);
									} else {
										position = projectile.Top + new Vector2(0, projectile.gfxOffY - 24);
									}
									float light = Lighting.Brightness((int)position.X / 16, (int)(projectile.Center.Y + projectile.gfxOffY) / 16) * 0.5f + 0.5f;
									if (artifact.Life > 0) {
										Main.instance.DrawHealthBar(
											position.X, position.Y,
											(int)artifact.Life,
											artifact.MaxLife,
											light,
											0.85f
										);
									} else {
										artifact.DrawDeadHealthBar(position, light);
									}
								}
							}
						}
						artifactMinions.Clear();
						return true;
					},
					InterfaceScaleType.Game) { Active = artifactMinions.Count > 0 }
				);
			}
		}
	}
	public interface IArtifactMinion {
		int MaxLife { get; set; }
		float Life { get; set; }
		void OnHurt(int damage, bool fromDoT) { }
		bool CanDie => true;
		void DrawDeadHealthBar(Vector2 position, float light) {
			Main.spriteBatch.Draw(
				TextureAssets.Hb2.Value,
				position - Main.screenPosition,
				null,
				Color.Gray * light,
				0f,
				new Vector2(18f * 0.85f, 0),
				0.85f,
				SpriteEffects.None,
			0);
		}
	}
	public static class ArtifactMinionExtensions {
		public static void DamageArtifactMinion(this IArtifactMinion minion, int damage, bool fromDoT = false, bool noCombatText = false) {
			ModProjectile proj = minion as ModProjectile;
			float healthModifiedDamage = damage * (minion.MaxLife / proj.Projectile.GetGlobalProjectile<ArtifactMinionGlobalProjectile>().maxHealthModifier.ApplyTo(minion.MaxLife));
			minion.Life -= healthModifiedDamage;
			minion.OnHurt(damage, fromDoT);
			if (minion.Life <= 0 && minion.CanDie) proj.Projectile.Kill();
			if (!noCombatText) CombatText.NewText(proj.Projectile.Hitbox, CombatText.DamagedFriendly, damage, !fromDoT, dot: true);
		}
		public static void DamageArtifactMinion(this Projectile minion, int damage, bool fromDoT = false, bool noCombatText = false) {
			if (minion.ModProjectile is IArtifactMinion artifact) artifact.DamageArtifactMinion(damage, fromDoT, noCombatText);
		}
	}
}
