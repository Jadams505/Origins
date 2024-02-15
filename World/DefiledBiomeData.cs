﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Origins.Tiles.Defiled;
using Origins.Walls;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static Terraria.WorldGen;
using System;
using Terraria.ID;
using Terraria.Localization;
using Terraria.GameContent.Achievements;
using Origins.Projectiles.Misc;
using Origins.Items.Accessories;
using Origins.Backgrounds;
using Terraria.Graphics.Effects;
using static Origins.OriginExtensions;
using Terraria.GameContent.ItemDropRules;
using Origins.Items.Pets;
using AltLibrary.Common.AltBiomes;
using Origins.NPCs.Defiled;
using AltLibrary.Core.Generation;
using Origins.Items.Materials;
using Origins.Items.Weapons.Melee;
using Origins.Items.Weapons.Magic;
using Origins.Items.Weapons.Ranged;
using Terraria.GameContent.Personalities;
using Terraria.ObjectData;

namespace Origins.World.BiomeData {
	public class Defiled_Wastelands : ModBiome {
		public static IItemDropRule FirstFissureDropRule;
		public static IItemDropRule FissureDropRule;
		public override int Music => Origins.Music.Defiled;
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		public override string BestiaryIcon => "Origins/UI/Defiled_Wastelands_Bestiary_Icon";
		public override string BackgroundPath => "Origins/UI/MapBGs/Defiled_Wastelands_Normal";
		public override string MapBackground => BackgroundPath;
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<Defiled_Surface_Background>();
		public override int BiomeTorchItemType => ModContent.ItemType<Defiled_Torch>();
		public override int BiomeCampfireItemType => ModContent.ItemType<Defiled_Campfire_Item>();
		public override bool IsBiomeActive(Player player) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.ZoneDefiledProgress = (Math.Min(
				OriginSystem.defiledTiles - (Defiled_Wastelands.NeededTiles - Defiled_Wastelands.ShaderTileCount),
				Defiled_Wastelands.ShaderTileCount
			) / Defiled_Wastelands.ShaderTileCount) * 0.9f;
			LinearSmoothing(ref originPlayer.ZoneDefiledProgressSmoothed, originPlayer.ZoneDefiledProgress, OriginSystem.biomeShaderSmoothing);

			return OriginSystem.defiledTiles > Defiled_Wastelands.NeededTiles;
		}
		public override void SpecialVisuals(Player player, bool isActive) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			Filters.Scene["Origins:ZoneDefiled"].GetShader().UseProgress(originPlayer.ZoneDefiledProgressSmoothed);
			player.ManageSpecialBiomeVisuals("Origins:ZoneDefiled", originPlayer.ZoneDefiledProgressSmoothed > 0, player.Center);
		}
		public override float GetWeight(Player player) {
			return player.GetModPlayer<OriginPlayer>().ZoneDefiledProgress * 0.98f;
		}
		public override void Load() {
			FirstFissureDropRule = ItemDropRule.Common(ModContent.ItemType<Kruncher>());
			FirstFissureDropRule.OnSuccess(ItemDropRule.Common(ItemID.MusketBall, 1, 100, 100));

			FissureDropRule = new OneFromRulesRule(1,
				FirstFissureDropRule,
				ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Infusion>()),
				ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Krakram>()),
				ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Suspicious_Looking_Pebble>()),
				ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Dim_Starlight>())
			);
		}
		public override void Unload() {
			FirstFissureDropRule = null;
			FissureDropRule = null;
		}
		public const int NeededTiles = 200;
		public const int ShaderTileCount = 75;
		public const short DefaultTileDust = DustID.Titanium;
		//public static SpawnConditionBestiaryInfoElement BestiaryIcon = new SpawnConditionBestiaryInfoElement("Bestiary_Biomes.Ocean", 28, "Images/MapBG11");
		public static class SpawnRates {
			public const float ChunkSlime = 1;
			public const float Cyclops = 1;
			public const float Mite = 1;
			public const float Brute = 0.6f;
			public const float Flyer = 0.6f;
			public const float Worm = 0.6f;
			public const float Mimic = 0.1f;
			public const float Bident = 0.2f;
			public const float Tripod = 0.3f;
			public const float AncientCyclops = 0.03f;
		}
		public static class Gen {
			public static void StartDefiled(float i, float j) {
				const float strength = 2.8f; //width of tunnels
				const float wallThickness = 3.1f;
				const float distance = 40; //tunnel length
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				ushort stoneWallID = (ushort)ModContent.WallType<Defiled_Stone_Wall>();
				Vector2 startVec = new Vector2(i, j);
				int fisureCount = 0;
				DefiledCave(i, j);
				Queue<(int generation, (Vector2 position, Vector2 velocity))> veins = new Queue<(int generation, (Vector2 position, Vector2 velocity))>();
				int startCount = genRand.Next(4, 9);
				float maxSpread = 3f / startCount;
				Vector2 vel;
				for (int count = startCount; count > 0; count--) {
					vel = Vector2.UnitX.RotatedBy((MathHelper.TwoPi * (count / (float)startCount)) + genRand.NextFloat(-maxSpread, maxSpread));
					veins.Enqueue((0, (startVec + vel * 16, vel)));
				}
				(int generation, (Vector2 position, Vector2 velocity) data) current;
				(int generation, (Vector2 position, Vector2 velocity) data) next;
				List<Vector2> fissureCheckSpots = new List<Vector2>();
				Vector2 airCheckVec;
				while (veins.Count > 0) {
					current = veins.Dequeue();
					int endChance = genRand.Next(1, 5) + genRand.Next(0, 4) + genRand.Next(0, 4);
					int selector = genRand.Next(4);
					if (endChance <= current.generation) {
						if (genRand.Next(veins.Count) < 6 - fissureCheckSpots.Count) {
							selector = 3;
						}
					} else if (selector == 3 && genRand.Next(veins.Count) > 6 - fissureCheckSpots.Count) {
						selector = genRand.Next(3);
					}
					switch (selector) {
						case 0:
						case 1: {
							next = (current.generation + 1,
								DefiledVeinRunner(
									(int)current.data.position.X,
									(int)current.data.position.Y,
									strength * genRand.NextFloat(0.9f, 1.1f), //tunnel width randomization
									current.data.velocity.RotatedBy(genRand.NextBool() ? genRand.NextFloat(-0.6f, -0.1f) : genRand.NextFloat(0.2f, 0.6f)), //random rotation
									genRand.NextFloat(distance * 0.8f, distance * 1.2f), //tunnel length
									stoneID,
									wallThickness,
									wallType: stoneWallID));
							airCheckVec = next.data.position;
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].WallType == WallID.None) {
								break;
							}
							if (endChance > current.generation) {
								veins.Enqueue(next);
							}
							break;
						}//single vein
						case 2: {
							next = (current.generation + 2,
								DefiledVeinRunner(
									(int)current.data.position.X,
									(int)current.data.position.Y,
									strength * genRand.NextFloat(0.9f, 1.1f),
									current.data.velocity.RotatedBy(-0.4f + genRand.NextFloat(-1, 0.2f)),
									genRand.NextFloat(distance * 0.8f, distance * 1.2f),
									stoneID,
									wallThickness,
									wallType: stoneWallID));
							airCheckVec = next.data.position;
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].WallType == WallID.None) {
								break;
							}
							if (endChance > current.generation) {
								veins.Enqueue(next);
							}
							next = (current.generation + 2,
								DefiledVeinRunner(
									(int)current.data.position.X,
									(int)current.data.position.Y,
									strength * genRand.NextFloat(0.9f, 1.1f),
									current.data.velocity.RotatedBy(0.4f + genRand.NextFloat(-0.2f, 1)),
									genRand.NextFloat(distance * 0.8f, distance * 1.2f),
									stoneID,
									wallThickness,
									wallType: stoneWallID));
							airCheckVec = next.data.position;
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].WallType == WallID.None) {
								break;
							}
							if (endChance > current.generation) {
								veins.Enqueue(next);
							}
							break;
						}//split vein
						case 3: {
							next = (current.generation + 2,
								DefiledVeinRunner(
									(int)current.data.position.X,
									(int)current.data.position.Y,
									strength * genRand.NextFloat(0.9f, 1.1f),
									current.data.velocity.RotatedBy(genRand.NextBool() ? genRand.NextFloat(-0.4f, -0.2f) : genRand.NextFloat(0.2f, 0.4f)),
									genRand.NextFloat(distance * 0.8f, distance * 1.2f),
									stoneID,
									wallThickness,
									wallType: stoneWallID));
							airCheckVec = next.data.position;
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].WallType == WallID.None) {
								break;
							}
							if (endChance > next.generation) {
								veins.Enqueue(next);
							}
							float size = genRand.NextFloat(0.3f, 0.4f);
							if (airCheckVec.Y >= Main.worldSurface) {
								DefiledCave(next.data.position.X, next.data.position.Y, size);
							}
							DefiledRib(next.data.position.X, next.data.position.Y, size * 30, 0.75f);
							fissureCheckSpots.Add(next.data.position);
							break;
						}//vein & cave
					}
				}
				ushort fissureID = (ushort)ModContent.TileType<Defiled_Fissure>();
				while (fisureCount < 6 && fissureCheckSpots.Count > 0) {
					int ch = genRand.Next(fissureCheckSpots.Count);
					for (int o = 0; o > -5; o = o > 0 ? -o : -o + 1) {
						Point p = fissureCheckSpots[ch].ToPoint();
						int loop = 0;
						for (; !Main.tile[p.X + o - 1, p.Y + 1].HasTile || !Main.tile[p.X + o, p.Y + 1].HasTile; p.Y++) {
							if (++loop > 10) {
								break;
							}
						}
						WorldGen.KillTile(p.X + o - 1, p.Y - 1);
						WorldGen.KillTile(p.X + o, p.Y - 1);
						WorldGen.KillTile(p.X + o - 1, p.Y);
						WorldGen.KillTile(p.X + o, p.Y);
						WorldGen.PlaceTile(p.X + o - 1, p.Y + 1, stoneID);
						WorldGen.PlaceTile(p.X + o, p.Y + 1, stoneID);
						WorldGen.SlopeTile(p.X + o - 1, p.Y + 1, SlopeID.None);
						WorldGen.SlopeTile(p.X + o, p.Y + 1, SlopeID.None);
						if (TileObject.CanPlace(p.X + o, p.Y, fissureID, 0, 0, out TileObject to)) {
							WorldGen.Place2x2(p.X + o, p.Y, fissureID, 0);
							fisureCount++;
							break;
						}
					}
					fissureCheckSpots.RemoveAt(ch);
				}
				ushort defiledAltar = (ushort)ModContent.TileType<Defiled_Altar>();
				for (int i0 = genRand.Next(10, 15); i0-- > 0;) {
					int tries = 0;
					bool placed = false;
					while (!placed && ++tries < 10000) {
						int x = (int)i + genRand.Next(-100, 101);
						int y = (int)j + genRand.Next(-80, 81);
						if (!Framing.GetTileSafely(x, y).HasTile) {
							for (; !Framing.GetTileSafely(x, y).HasTile; y++) {
								if (y > Main.maxTilesY) break;
							}
							y--;
						} else {
							while (Framing.GetTileSafely(x, y).HasTile && y > Main.worldSurface) {
								y--;
							}
						}
						Place3x2(x, y, defiledAltar);
						placed = Framing.GetTileSafely(x, y).TileIsType(defiledAltar);
					}
				}
				/*ushort defiledPot = (ushort)ModContent.TileType<Defiled_Pot>();
				int placedPots = 0;
				for (int i0 = genRand.Next(100, 150); i0-- > 0;) {
					int x = (int)i + genRand.Next(-100, 101);
					int y = (int)j + genRand.Next(-80, 81);
					if (!Framing.GetTileSafely(x, y).HasTile) {
						for (; !Framing.GetTileSafely(x, y).HasTile; y++) {
							if (y > Main.maxTilesY) break;
						}
						y--;
					} else {
						while (Framing.GetTileSafely(x, y).HasTile && y > Main.worldSurface) {
							y--;
						}
					}
					Place3x2(x, y, defiledPot);
					if (Framing.GetTileSafely(x, y).TileIsType(defiledPot)) placedPots++;
				}
				Origins.instance.Logger.Info($"Placed {placedPots} defiled pots");
				Origins.instance.Logger.Info($"Generated {{$Defiled_Wastelands}} with {fisureCount} fissures");*/
				//Main.NewText($"Generated Defiled Wastelands with {fisureCount} fissures");
			}
			public static void DefiledCave(float i, float j, float sizeMult = 1f) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				ushort stoneWallID = (ushort)ModContent.WallType<Defiled_Stone_Wall>();
				for (int x = (int)Math.Floor(i - (28 * sizeMult + 5)); x < (int)Math.Ceiling(i + (28 * sizeMult + 5)); x++) {
					for (int y = (int)Math.Ceiling(j + (28 * sizeMult + 4)); y >= (int)Math.Floor(j - (28 * sizeMult + 4)); y--) {
						if (Main.tile[x, y].HasTile && !WorldGen.CanKillTile(x, y)) continue;
						float diff = (float)Math.Sqrt((((y - j) * (y - j)) + (x - i) * (x - i)) * (GenRunners.GetWallDistOffset((float)Math.Atan2(y - j, x - i) * 4 + x + y) * 0.0316076058772687986171132238548f + 1));
						if (diff > 35 * sizeMult) {
							continue;
						}
						if (Main.tile[x, y].WallType != stoneWallID) {
							Main.tile[x, y].ResetToType(stoneID);
						}
						Main.tile[x, y].WallType = stoneWallID;
						if (diff < 35 * sizeMult - 5) {
							Tile tile0 = Main.tile[x, y];
							tile0.HasTile = false;
						}
					}
				}
			}
			public static void DefiledRibs(float i, float j, float sizeMult = 1f) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				for (int x = (int)Math.Floor(i - (28 * sizeMult + 5)); x < (int)Math.Ceiling(i + (28 * sizeMult + 5)); x++) {
					for (int y = (int)Math.Ceiling(j + (28 * sizeMult + 4)); y >= (int)Math.Floor(j - (28 * sizeMult + 4)); y--) {
						float diff = (float)Math.Sqrt((((y - j) * (y - j)) + (x - i) * (x - i)) * (GenRunners.GetWallDistOffset((float)Math.Atan2(y - j, x - i) * 4 + x + y) * 0.0316076058772687986171132238548f + 1));
						if (diff > 16 * sizeMult) {
							continue;
						}
						if (Main.tile[x, y].HasTile && !WorldGen.CanKillTile(x, y)) continue;
						if (Math.Cos(diff * 0.7f) <= 0.1f) {
							Main.tile[x, y].ResetToType(stoneID);
						} else {
							Tile tile0 = Main.tile[x, y];
							tile0.HasTile = false;
						}
					}
				}
			}
			public static void DefiledRib(float i, float j, float size = 16f, float thickness = 1) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				for (int x = (int)Math.Floor(i - size); x < (int)Math.Ceiling(i + size); x++) {
					for (int y = (int)Math.Ceiling(j + size); y >= (int)Math.Floor(j - size); y--) {
						if (Main.tile[x, y].HasTile && Main.tileSolid[Main.tile[x, y].TileType]) {
							continue;
						}
						if (Main.tile[x, y].HasTile && !WorldGen.CanKillTile(x, y)) continue;
						float diff = (float)Math.Sqrt((((y - j) * (y - j)) + (x - i) * (x - i)) * (GenRunners.GetWallDistOffset((float)Math.Atan2(y - j, x - i) * 4 + x + y) * 0.0316076058772687986171132238548f + 1));
						if (diff > size + thickness || diff < size - thickness) {
							continue;
						}
						Main.tile[x, y].ResetToType(stoneID);
					}
				}
			}
			public static (Vector2 position, Vector2 velocity) DefiledVeinRunner(int i, int j, double strength, Vector2 speed, double length, ushort wallBlockType, float wallThickness, float twist = 0, bool randomtwist = false, int wallType = -1) {
				Vector2 pos = new Vector2(i, j);
				Tile tile;
				if (randomtwist) twist = Math.Abs(twist);
				int X0 = int.MaxValue;
				int X1 = 0;
				int Y0 = int.MaxValue;
				int Y1 = 0;
				double baseStrength = strength;
				strength = Math.Pow(strength, 2);
				float basewallThickness = wallThickness;
				wallThickness *= wallThickness;
				double decay = speed.Length();
				Vector2 direction = speed / (float)decay;
				bool hasWall = wallType != -1;
				ushort _wallType = hasWall ? (ushort)wallType : (ushort)0;
				while (length > 0) {
					length -= decay;
					int minX = (int)(pos.X - (strength + wallThickness) * 0.5);
					int maxX = (int)(pos.X + (strength + wallThickness) * 0.5);
					int minY = (int)(pos.Y - (strength + wallThickness) * 0.5);
					int maxY = (int)(pos.Y + (strength + wallThickness) * 0.5);
					if (minX < 1) {
						minX = 1;
					}
					if (maxX > Main.maxTilesX - 1) {
						maxX = Main.maxTilesX - 1;
					}
					if (minY < 1) {
						minY = 1;
					}
					if (maxY > Main.maxTilesY - 1) {
						maxY = Main.maxTilesY - 1;
					}
					for (int l = minX; l < maxX; l++) {
						for (int k = minY; k < maxY; k++) {
							float el = l + (GenRunners.GetWallDistOffset((float)length + k) + 0.5f) / 2.5f;
							float ek = k + (GenRunners.GetWallDistOffset((float)length + l) + 0.5f) / 2.5f;
							double dist = Math.Pow(Math.Abs(el - pos.X), 2) + Math.Pow(Math.Abs(ek - pos.Y), 2);
							tile = Main.tile[l, k];
							if (Main.tileDungeon[tile.TileType]) {
								return (pos, speed);
							}
							bool openAir = (k < Main.worldSurface && tile.WallType == WallID.None);
							if (dist > strength) {
								double d = Math.Sqrt(dist);
								if (!openAir && d < baseStrength + basewallThickness && OriginExtensions.IsTileReplacable(l, k) && tile.WallType != _wallType) {

									if (!WorldGen.IsAContainer(tile)) {
										tile.HasTile = true;
										tile.ResetToType(wallBlockType);
									}
									//WorldGen.SquareTileFrame(l, k);
									if (hasWall) {
										if (tile.WallType == WallID.DirtUnsafe || tile.WallType == WallID.MudUnsafe) {
											OriginExtensions.SpreadWall(l, k, _wallType, new Dictionary<ushort, bool>() {
												[WallID.DirtUnsafe] = true,
												[WallID.MudUnsafe] = true
											});
										}
										tile.WallType = _wallType;
									}
								}
								continue;
							}
							if (TileID.Sets.CanBeClearedDuringGeneration[tile.TileType]) {
								if (!Main.tileContainer[tile.TileType] && !Main.tileContainer[Main.tile[l, k - 1].TileType]) {
									Tile tile0 = Main.tile[l, k];
									tile0.HasTile = false;
								}
								//WorldGen.SquareTileFrame(l, k);
								if (hasWall && !openAir) {
									tile.WallType = _wallType;
								}
								if (l > X1) {
									X1 = l;
								} else if (l < X0) {
									X0 = l;
								}
								if (k > Y1) {
									Y1 = k;
								} else if (k < Y0) {
									Y0 = k;
								}
							}
						}
					}
					pos += speed;
					if (randomtwist || twist != 0.0) {
						speed = randomtwist ? speed.RotatedBy(genRand.NextFloat(-twist, twist)) : speed.RotatedBy(twist);
					}
				}
				if (X0 < 1) {
					X0 = 1;
				}
				if (Y0 > Main.maxTilesX - 1) {
					Y0 = Main.maxTilesX - 1;
				}
				if (X1 < 1) {
					X1 = 1;
				}
				if (Y1 > Main.maxTilesY - 1) {
					Y1 = Main.maxTilesY - 1;
				}
				RangeFrame(X0, Y0, X1, Y1);
				NetMessage.SendTileSquare(Main.myPlayer, X0, Y0, X1 - X0, Y1 - Y1);
				return (pos, speed);
			}
		}

		public static void CheckFissure(int i, int j, int type) {
			if (destroyObject) {
				return;
			}
			int x = Main.tile[i, j].TileFrameX != 0 ? i - 1 : i;
			int y = Main.tile[i, j].TileFrameY != 0 && Main.tile[i, j].TileFrameY != 36 ? j - 1 : j;
			for (int k = 0; k < 2; k++) {
				for (int l = 0; l < 2; l++) {
					Tile tile = Main.tile[x + k, y + l];
					if (tile != null && (!tile.HasUnactuatedTile || tile.TileType != type)) {
						destroyObject = true;
						break;
					}
				}
				if (destroyObject) {
					break;
				}
			}
			if (!destroyObject) {
				return;
			}
			for (int m = x; m < x + 2; m++) {
				for (int n = y; n < y + 2; n++) {
					if (Main.tile[m, n].TileType == type) {
						KillTile(m, n);
					}
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient && !WorldGen.noTileActions) {
				if (genRand.NextBool(2)) {
					spawnMeteor = true;
				}
				float fx = x * 16;
				float fy = y * 16;

				float distance = -1f;
				int player = 0;
				for (int playerIndex = 0; playerIndex < 255; playerIndex++) {
					float currentDist = Math.Abs(Main.player[playerIndex].position.X - fx) + Math.Abs(Main.player[playerIndex].position.Y - fy);
					if (currentDist < distance || distance == -1f) {
						player = playerIndex;
						distance = currentDist;
					}
				}

				DropAttemptInfo dropInfo = default(DropAttemptInfo);
				dropInfo.player = Main.player[player];
				dropInfo.IsExpertMode = Main.expertMode;
				dropInfo.IsMasterMode = Main.masterMode;
				dropInfo.IsInSimulation = false;
				dropInfo.rng = Main.rand;
				Origins.ResolveRuleWithHandler(shadowOrbSmashed ? FissureDropRule : FirstFissureDropRule, dropInfo, (DropAttemptInfo info, int item, int stack, bool _) => {
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, item, stack, pfix: -1);
				});
				/*int selection = Main.rand.Next(5);
				if (!shadowOrbSmashed) {
					selection = 0;
				}
				switch (selection) {
					case 0: 
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Defiled_Burst>(), 1, noBroadcast: false, -1);
					int stack2 = WorldGen.genRand.Next(100, 101);
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.MusketBall, stack2);
					break;
				
					case 1:
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Infusion>(), 1, noBroadcast: false, -1);
					break;

					case 2:
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Defiled_Chakram>(), 1, noBroadcast: false, -1);
					break;

					case 3:
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ItemID.ShadowOrb, 1, noBroadcast: false, -1);
					break;

					case 4:
					Item.NewItem(GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<Dim_Starlight>(), 1, noBroadcast: false, -1);
					break;
				}*/
				shadowOrbSmashed = true;

				//this projectile handles the rest
				Projectile.NewProjectile(GetItemSource_FromTileBreak(i, j), new Vector2((i + 1) * 16, (j + 1) * 16), Vector2.Zero, ModContent.ProjectileType<Defiled_Wastelands_Signal>(), 0, 0, Main.myPlayer, ai0: 1, ai1: player);

				AchievementsHelper.NotifyProgressionEvent(7);
			}
			SoundEngine.PlaySound(Origins.Sounds.DefiledKill, new Vector2(i * 16, j * 16));
			destroyObject = false;
		}
	}
	public class Underground_Defiled_Wastelands_Biome : ModBiome {
		public override int Music => Origins.Music.UndergroundDefiled;
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		public override float GetWeight(Player player) {
			return player.GetModPlayer<OriginPlayer>().ZoneDefiledProgress;
		}
		public override bool IsBiomeActive(Player player) {
			return player.ZoneRockLayerHeight && player.InModBiome<Defiled_Wastelands>();
		}
	}
	public class Defiled_Wastelands_Desert : ModBiome {
		public override int Music => Origins.Music.Defiled;
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		public override string BestiaryIcon => "Origins/UI/Defiled_Wastelands_Bestiary_Icon";
		public override string BackgroundPath => "Origins/UI/MapBGs/Defiled_Wastelands_Desert";
		public override string MapBackground => BackgroundPath;
		public override bool IsBiomeActive(Player player) {
			return player.ZoneDesert && player.InModBiome<Defiled_Wastelands>();
		}
		public override float GetWeight(Player player) {
			return player.GetModPlayer<OriginPlayer>().ZoneDefiledProgress * 0.99f;
		}
	}
	public class Defiled_Wastelands_Alt_Biome : AltBiome {
		public override string WorldIcon => "Origins/UI/WorldGen/IconDefiled";
		public override string OuterTexture => "Origins/UI/WorldGen/Outer_Defiled";
		public override string IconSmall => "Origins/UI/WorldGen/IconEvilDefiled";
		public override Color OuterColor => new(170, 170, 170);
		public override IShoppingBiome Biome => ModContent.GetInstance<Defiled_Wastelands>();
		public override void SetStaticDefaults() {
			BiomeType = AltLibrary.BiomeType.Evil;
			//DisplayName.SetDefault(Language.GetTextValue("{$Defiled_Wastelands}"));
			//Description.SetDefault(Language.GetTextValue("A desaturated and bleak environment that is actually a living organism growing its body."));
			//GenPassName.SetDefault(Language.GetTextValue("{$Mods.Origins.Generic.Riven_Hive}"));
			//GenPassName.SetDefault(Language.GetTextValue("{$Defiled_Wastelands}"));
			AddTileConversion(ModContent.TileType<Defiled_Grass>(), TileID.Grass);
			AddTileConversion(ModContent.TileType<Defiled_Jungle_Grass>(), TileID.JungleGrass);
			AddTileConversion(ModContent.TileType<Defiled_Stone>(), TileID.Stone);
			AddTileConversion(ModContent.TileType<Defiled_Sand>(), TileID.Sand);
			AddTileConversion(ModContent.TileType<Defiled_Sandstone>(), TileID.Sandstone);
			AddTileConversion(ModContent.TileType<Hardened_Defiled_Sand>(), TileID.HardenedSand);
			AddTileConversion(ModContent.TileType<Defiled_Ice>(), TileID.IceBlock);


			BiomeFlesh = TileID.SilverBrick;
			BiomeFleshWall = WallID.SilverBrick;

			FleshDoorTile = TileID.ClosedDoor;
			FleshChairTile = TileID.Chairs;
			FleshTableTile = TileID.Tables;
			FleshChestTile = TileID.Containers;
			FleshDoorTileStyle = 7;
			FleshChairTileStyle = 7;
			FleshTableTileStyle = 7;
			FleshChestTileStyle = 7;

			FountainTile = TileID.WaterFountain;
			FountainTileStyle = 1;
			//FountainTile = TileID.WaterFountain;
			//Fountain = TileID.WaterFountain;

			AddTileConversion(ModContent.TileType<Defiled_Large_Foliage>(), TileID.LargePiles);
			AddTileConversion(ModContent.TileType<Defiled_Large_Foliage>(), TileID.LargePiles2);

			SeedType = ModContent.ItemType<Defiled_Grass_Seeds>();
			BiomeOre = ModContent.TileType<Defiled_Ore>();
			BiomeOreItem = ModContent.ItemType<Defiled_Ore_Item>();
			AltarTile = ModContent.TileType<Defiled_Altar>();

			BiomeChestItem = ModContent.ItemType<Defiled_Dungeon_Chest_Placeholder_Item>();
			BiomeChestTile = ModContent.TileType<Defiled_Dungeon_Chest>();
			BiomeChestTileStyle = 1;
			BiomeKeyItem = ModContent.ItemType<Defiled_Key>();

			MimicType = ModContent.NPCType<Defiled_Mimic>();

			AddWallConversions<Defiled_Stone_Wall>(
				WallID.Stone,
				WallID.CaveUnsafe,
				WallID.Cave2Unsafe,
				WallID.Cave3Unsafe,
				WallID.Cave4Unsafe,
				WallID.Cave5Unsafe,
				WallID.Cave6Unsafe,
				WallID.Cave7Unsafe,
				WallID.Cave8Unsafe,
				WallID.EbonstoneUnsafe,
				WallID.CorruptionUnsafe1,
				WallID.CorruptionUnsafe2,
				WallID.CorruptionUnsafe3,
				WallID.CorruptionUnsafe4,
				WallID.CrimstoneUnsafe,
				WallID.CrimsonUnsafe1,
				WallID.CrimsonUnsafe2,
				WallID.CrimsonUnsafe3,
				WallID.CrimsonUnsafe4
			);
				AddWallConversions<Defiled_Sandstone_Wall>(
				WallID.Sandstone,
				WallID.CorruptSandstone,
				WallID.CrimsonSandstone,
				WallID.HallowSandstone
			);
			AddWallConversions<Hardened_Defiled_Sand_Wall>(
				WallID.HardenedSand,
				WallID.CorruptHardenedSand,
				WallID.CrimsonHardenedSand,
				WallID.HallowHardenedSand
			);
			AddWallConversions<Chambersite_Defiled_Stone_Wall>((ushort)ModContent.WallType<Chambersite_Stone_Wall>());
		}
		public override int GetAltBlock(int BaseBlock, int posX, int posY, bool GERunner = false) {
			switch (BaseBlock) {
				case TileID.LargePiles: {
					Tile tile = Main.tile[posX, posY];
					switch (tile.TileFrameX / (18 * 3)) {
						case 7 or 8 or 9 or 10 or 11 or 12:
						return ModContent.TileType<Defiled_Large_Foliage>();
					}
					break;
				}
				case TileID.LargePiles2: {
					Tile tile = Main.tile[posX, posY];
					switch ((tile.TileFrameX / (18 * 3), tile.TileFrameY / (18 * 2))) {
						case (0, 0) or (1, 0) or (2, 0) or (3, 0) or (4, 0) or (5, 0):
						case (15, 0) or (16, 0) or (17, 0):
						case (16, 1) or (17, 1):
						return ModContent.TileType<Defiled_Large_Foliage>();
					}
					break;
				}
				case TileID.SmallPiles: {
					Tile tile = Main.tile[posX, posY];
					switch ((tile.TileFrameX / 18, tile.TileFrameY / 18)) {
						case (28, 0) or (29, 0) or (30, 0) or (31, 0) or (32, 0) or (33, 0) or (34, 0):
						case (31, 1) or (32, 1) or (33, 1) or (34, 1) or (35, 1) or (36, 1):
						break;
					}
					TileObjectData tileObjectData = TileObjectData.GetTileData(tile);
					if (tileObjectData.Width == 2) {

					}
					break;
				}
			}
			return base.GetAltBlock(BaseBlock, posX, posY, GERunner);
		}
		public override AltMaterialContext MaterialContext {
			get {
				AltMaterialContext context = new AltMaterialContext();
				context.SetEvilHerb(ModContent.ItemType<Wilting_Rose_Item>());
				context.SetEvilBar(ModContent.ItemType<Defiled_Bar>());
				context.SetEvilOre(ModContent.ItemType<Defiled_Ore_Item>());
				context.SetVileInnard(ModContent.ItemType<Strange_String>());
				context.SetVileComponent(ModContent.ItemType<Black_Bile>());
				context.SetEvilBossDrop(ModContent.ItemType<Undead_Chunk>());
				context.SetEvilSword(ModContent.ItemType<Spiker_Sword>());
				return context;
			}
		}
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() {
			return new Defiled_Wastelands_Generation_Pass();
		}
		public static List<int> defiledWastelandsWestEdge;
		public static List<int> defiledWastelandsEastEdge;
		public class Defiled_Wastelands_Generation_Pass : EvilBiomeGenerationPass {
			Stack<Point> defiledHearts = new Stack<Point>() { };
			public override void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound) {
				for (int offset = 0; offset < 150; offset = offset > 0 ? (-offset) : ((-offset) + 1)) {
					if (evilBiomePositionWestBound + offset < 0 || evilBiomePositionEastBound + offset > Main.maxTilesX) continue;
					for (int j = (int)OriginSystem.worldSurfaceLow; j < Main.maxTilesY; j++) {
						Tile tile = Framing.GetTileSafely(evilBiomePosition + offset, j);
						if (!tile.HasTile) continue;
						if (tile.TileType == TileID.JungleGrass) break;
						evilBiomePosition += offset;
						evilBiomePositionWestBound += offset;
						evilBiomePositionEastBound += offset;
						goto positioned;
					}
				}
				positioned:
				defiledWastelandsWestEdge ??= new();
				defiledWastelandsEastEdge ??= new();
				defiledWastelandsWestEdge.Add(evilBiomePositionWestBound);
				defiledWastelandsEastEdge.Add(evilBiomePositionEastBound);
				int startY;
				for (startY = (int)GenVars.worldSurfaceLow; !Main.tile[evilBiomePosition, startY].HasTile; startY++) ;
				Point start = new Point(evilBiomePosition, startY + genRand.Next(105, 150));//range of depths

				Defiled_Wastelands.Gen.StartDefiled(start.X, start.Y);
				defiledHearts.Push(start);
				OriginSystem.Instance.hasDefiled = true;
			}

			public override void PostGenerateEvil() {
				Point heart;
				while (defiledHearts.Count > 0) {
					heart = defiledHearts.Pop();
					Defiled_Wastelands.Gen.DefiledRibs(heart.X + genRand.NextFloat(-0.5f, 0.5f), heart.Y + genRand.NextFloat(-0.5f, 0.5f));
					for (int i = heart.X - 1; i < heart.X + 3; i++) {
						for (int j = heart.Y - 2; j < heart.Y + 2; j++) {
							Main.tile[i, j].SetActive(false);
						}
					}
					TileObject.CanPlace(heart.X, heart.Y, (ushort)ModContent.TileType<Defiled_Heart>(), 0, 1, out var data);
					TileObject.Place(data);
					OriginSystem.Instance.Defiled_Hearts.Add(heart);
				}
			}
		}
	}
}
