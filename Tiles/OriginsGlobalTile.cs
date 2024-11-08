﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Items.Other.Consumables;
using Origins.Tiles.Defiled;
using Origins.Tiles.Riven;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Tiles {
	public class OriginsGlobalTile : GlobalTile {
		static Dictionary<int, AutoLoadingAsset<Texture2D>> stalactiteTextures;
		public override void SetStaticDefaults() {
			stalactiteTextures = new() {
				[ModContent.TileType<Defiled_Ice>()] = "Origins/Tiles/Defiled/Defiled_Icicle"
			};
		}
		public override void Unload() {
			stalactiteTextures = null;
		}
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged) {
			//if (Main.tile[i, j - 1].TileType == Defiled_Altar.ID && type != Defiled_Altar.ID) return false;
			//if (Main.tile[i, j - 1].TileType == Riven_Altar.ID && type != Riven_Altar.ID) return false;
			return true;
		}
		public override void MouseOver(int i, int j, int type) {
			Point targetPos = new(i, j);
			Tile tile = Framing.GetTileSafely(targetPos);
			if (tile.HasTile && Main.tileContainer[tile.TileType]) {
				if (tile.TileFrameX % 36 != 0) {
					targetPos.X--;
				}
				if (tile.TileFrameY != 0) {
					targetPos.Y--;
				}
				OriginSystem originSystem = ModContent.GetInstance<OriginSystem>();
				if (originSystem.VoidLocks.TryGetValue(targetPos, out Guid owner)) {
					if (owner == Main.LocalPlayer.GetModPlayer<OriginPlayer>().guid) {
						Main.LocalPlayer.cursorItemIconID = ItemID.ShadowKey;
						if (Main.LocalPlayer.tileInteractAttempted) {
							if (Main.LocalPlayer.HasItemInInventoryOrOpenVoidBag(ItemID.ShadowKey)) {
								originSystem.VoidLocks.Remove(targetPos);
							} else {
								Main.LocalPlayer.tileInteractAttempted = false;
							}
						}
					} else {
						Main.LocalPlayer.cursorItemIconID = ModContent.ItemType<Void_Lock>();
						Main.LocalPlayer.tileInteractAttempted = false;
					}
				}
			}
		}
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem) {
			/*if () {

			}*/
		}
		public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak) {
			switch (type) {
				case TileID.Plants:
				case TileID.CorruptPlants:
				case TileID.CrimsonPlants:
				case TileID.HallowedPlants:
				ConvertPlantsByAnchor(ref Main.tile[i, j].TileType, Main.tile[i, j + 1].TileType);
				return true;
			}
			if (type == ModContent.TileType<Defiled_Foliage>()) {
				ConvertPlantsByAnchor(ref Main.tile[i, j].TileType, Main.tile[i, j + 1].TileType);
			}
			return true;
		}
		public static void ConvertPlantsByAnchor(ref ushort plant, ushort anchor) {
			switch (anchor) {
				case TileID.Grass:
				plant = TileID.Plants;
				return;
				case TileID.CorruptGrass:
				plant = TileID.CorruptPlants;
				return;
				case TileID.CrimsonGrass:
				plant = TileID.CrimsonPlants;
				return;
				case TileID.HallowedGrass:
				plant = TileID.HallowedPlants;
				return;
			}
			if (anchor == ModContent.TileType<Defiled_Grass>()) {
				plant = (ushort)ModContent.TileType<Defiled_Foliage>();
			} else if (anchor == ModContent.TileType<Riven_Grass>()) {
				plant = (ushort)ModContent.TileType<Riven_Foliage>();
			}
		}
		public static bool GetStalactiteTexture(int i, int j, int frameY, out AutoLoadingAsset<Texture2D> texture) {
			int direction = -1;
			if (frameY is 36 or 54 or 90) {
				direction = 1;
			}
			int baseY = j;
			while (Main.tile[i, baseY].TileIsType(TileID.Stalactite)) {
				baseY += direction;
			}
			return stalactiteTextures.TryGetValue(Main.tile[i, baseY].TileType, out texture);
		}
		public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData) {
			if (type == TileID.Stalactite) {
				if (GetStalactiteTexture(i, j, drawData.tileFrameY, out AutoLoadingAsset<Texture2D> texture)) {
					if (texture.IsLoaded) {
						drawData.tileFrameY %= 54;
						drawData.drawTexture = texture;
					} else {
						texture.LoadAsset();
					}
				}
			}
		}
		public override bool? IsTileBiomeSightable(int i, int j, int type, ref Color sightColor) {
			if (TileLoader.GetTile(type) is ModTile modTile) {
				if (modTile is IDefiledTile) {
					sightColor = Color.White;
					return true;
				}
				if (modTile is IRivenTile) {
					sightColor = Color.Cyan;
					return true;
				}
				if (modTile is IAshenTile) {
					sightColor = Color.OrangeRed;
					return true;
				}
			}
			return null;
		}
	}
}
