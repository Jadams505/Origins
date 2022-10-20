﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Origins.Tiles.Defiled;
using Origins.Walls;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Tyfyter.Utils;
using static Terraria.WorldGen;
using System;
using Terraria.ID;
using Origins.Items.Weapons.Defiled;
using Terraria.Localization;
using Terraria.GameContent.Achievements;
using Origins.Projectiles.Misc;
using Origins.Items.Accessories;
using Origins.Backgrounds;
using Terraria.Graphics.Effects;
using static Origins.OriginExtensions;
using Terraria.Chat;
using Terraria.GameContent.ItemDropRules;
using Origins.Items.Pets;
using AltLibrary.Common.AltBiomes;
using Origins.NPCs.Defiled;
using AltLibrary.Core.Generation;

namespace Origins.World.BiomeData {
	public class Defiled_Wastelands : ModBiome {
		public static IItemDropRule FirstFissureDropRule;
		public static IItemDropRule FissureDropRule;
		public override int Music => Origins.Music.Defiled;
		public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
		public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<Defiled_Surface_Background>();
		public override bool IsBiomeActive(Player player) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.ZoneDefiled = OriginSystem.defiledTiles > Defiled_Wastelands.NeededTiles;
			originPlayer.ZoneDefiledProgress = Math.Min(OriginSystem.defiledTiles - (Defiled_Wastelands.NeededTiles - Defiled_Wastelands.ShaderTileCount), Defiled_Wastelands.ShaderTileCount) / Defiled_Wastelands.ShaderTileCount;
			LinearSmoothing(ref originPlayer.ZoneDefiledProgressSmoothed, originPlayer.ZoneDefiledProgress, OriginSystem.biomeShaderSmoothing);

			return originPlayer.ZoneDefiled;
		}
		public override void SpecialVisuals(Player player, bool isActive) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			Filters.Scene["Origins:ZoneDefiled"].GetShader().UseProgress(originPlayer.ZoneDefiledProgressSmoothed);
			player.ManageSpecialBiomeVisuals("Origins:ZoneDefiled", originPlayer.ZoneDefiledProgressSmoothed > 0, player.Center);
		}
		public override void Load() {
			FirstFissureDropRule = ItemDropRule.Common(ModContent.ItemType<Defiled_Burst>());
			FirstFissureDropRule.OnSuccess(ItemDropRule.Common(ItemID.MusketBall, 1, 100, 100));

			FissureDropRule = new OneFromRulesRule(1,
				FirstFissureDropRule,
				ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Infusion>()),
				ItemDropRule.NotScalingWithLuck(ModContent.ItemType<Defiled_Chakram>()),
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
            public const float Cyclops = 1;
            public const float Mite = 1;
            public const float Brute = 0.6f;
            public const float Flyer = 0.6f;
            public const float Worm = 0.6f;
            public const float Hunter = 0.6f;
			public const float Mimic = 0.1f;
			public const float Bident = 0.2f;
			public const float Tripod = 0.3f;
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
				for (int count = startCount; count>0; count--) {
					vel = Vector2.UnitX.RotatedBy((MathHelper.TwoPi * (count / (float)startCount)) + genRand.NextFloat(-maxSpread, maxSpread));
					veins.Enqueue((0, (startVec + vel * 16, vel)));
				}
				(int generation, (Vector2 position, Vector2 velocity) data) current;
				(int generation, (Vector2 position, Vector2 velocity) data) next;
				List<Vector2> fisureCheckSpots = new List<Vector2>();
				Vector2 airCheckVec;
				while (veins.Count > 0) {
					current = veins.Dequeue();
					int endChance = genRand.Next(1, 5) + genRand.Next(0, 4) + genRand.Next(0, 4);
					int selector = genRand.Next(4);
                    if (endChance <= current.generation) {
						if (genRand.Next(veins.Count) < 6 - fisureCheckSpots.Count) {
							selector = 3;
						}
                    }else if (selector == 3 && genRand.Next(veins.Count) > 6 - fisureCheckSpots.Count) {
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
							fisureCheckSpots.Add(next.data.position);
							break;
						}//vein & cave
					}
                }
				ushort fissureID = (ushort)ModContent.TileType<Defiled_Fissure>();
                while (fisureCount < 6 && fisureCheckSpots.Count > 0) {
					int ch = genRand.Next(fisureCheckSpots.Count);
					for (int o = 0; o > -5; o = o > 0 ? -o : -o + 1) {
						Point p = fisureCheckSpots[ch].ToPoint();
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
					fisureCheckSpots.RemoveAt(ch);
				}
				ushort defiledAltar = (ushort)ModContent.TileType<Defiled_Altar>();
				for (int i0 = genRand.Next(10, 15); i0-->0;) {
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
				ushort defiledPot = (ushort)ModContent.TileType<Defiled_Pot>();
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
				Origins.instance.Logger.Info($"Generated {{$Defiled_Wastelands}} with {fisureCount} fissures");
				//Main.NewText($"Generated Defiled Wastelands with {fisureCount} fissures");
			}
			public static void DefiledCave(float i, float j, float sizeMult = 1f) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				ushort stoneWallID = (ushort)ModContent.WallType<Defiled_Stone_Wall>();
				for (int x = (int)Math.Floor(i - (28 * sizeMult + 5)); x < (int)Math.Ceiling(i + (28 * sizeMult + 5)); x++) {
					for (int y = (int)Math.Ceiling(j + (28 * sizeMult + 4)); y >= (int)Math.Floor(j - (28 * sizeMult + 4)); y--) {
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
                        if (Math.Cos(diff*0.7f)<=0.1f) {
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
								if (!openAir && d < baseStrength + basewallThickness && TileID.Sets.CanBeClearedDuringGeneration[tile.TileType] && tile.WallType != _wallType && CanKillTile(l, k)) {
									
                                    if (!Main.tileContainer[tile.TileType]) {
										tile.HasTile = true;
										tile.ResetToType(wallBlockType);
									}
									//WorldGen.SquareTileFrame(l, k);
									if (hasWall) {
										if (tile.WallType == WallID.GrassUnsafe) {
											WorldGen.Spread.Wall2(l, k, _wallType);
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
		public override bool IsBiomeActive(Player player) {
			return base.IsBiomeActive(player);
		}
	}
	public class Defiled_Wastelands_Alt_Biome : AltBiome {
		public override string WorldIcon => "";//TODO: Redo tree icons for AltLib
		public override string OuterTexture => "Origins/UI/WorldGen/Outer_Defiled";
		public override string IconSmall => "Origins/icon_small";
		public override Color OuterColor => new(170, 170, 170);
		public override List<int> SpreadingTiles => new List<int> {
			ModContent.TileType<Defiled_Grass>(),
			ModContent.TileType<Defiled_Stone>(),
			ModContent.TileType<Defiled_Sand>(),
			ModContent.TileType<Defiled_Sandstone>(),
			ModContent.TileType<Hardened_Defiled_Sand>(),
			ModContent.TileType<Defiled_Ice>(),
		};
		public override void SetStaticDefaults() {
			BiomeType = AltLibrary.BiomeType.Evil;
			GenPassName.SetDefault("{$Defiled_Wastelands}");
			BiomeGrass = ModContent.TileType<Defiled_Grass>();
			BiomeStone = ModContent.TileType<Defiled_Stone>();
			BiomeSand = ModContent.TileType<Defiled_Sand>();
			BiomeSandstone = ModContent.TileType<Defiled_Sandstone>();
			BiomeHardenedSand = ModContent.TileType<Hardened_Defiled_Sand>();
			BiomeIce = ModContent.TileType<Defiled_Ice>();
			BiomeOre = ModContent.TileType<Defiled_Ore>();
			BiomeOreItem = ModContent.ItemType<Defiled_Ore_Item>();
			AltarTile = ModContent.TileType<Defiled_Altar>();
			BiomeChestTile = ModContent.TileType<Defiled_Dungeon_Chest>();
			MimicType = ModContent.NPCType<Defiled_Mimic>();
		}
		public override EvilBiomeGenerationPass GetEvilBiomeGenerationPass() {
			return new Defiled_Wastelands_Generation_Pass();
		}
		public class Defiled_Wastelands_Generation_Pass : EvilBiomeGenerationPass {
			Stack<Point> defiledHearts = new Stack<Point>() { };
			public override void GenerateEvil(int evilBiomePosition, int evilBiomePositionWestBound, int evilBiomePositionEastBound) {
				int startY;
				for (startY = (int)WorldGen.worldSurfaceLow; !Main.tile[evilBiomePosition, startY].HasTile; startY++) ;
				Point start = new Point(evilBiomePosition, startY + genRand.Next(105, 150));//range of depths

				Defiled_Wastelands.Gen.StartDefiled(start.X, start.Y);
				defiledHearts.Push(start);
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
					OriginSystem.instance.Defiled_Hearts.Add(heart);
				}
			}
		}
	}
}
