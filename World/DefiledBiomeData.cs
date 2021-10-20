﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Origins.Tiles.Defiled;
using Origins.Walls;
using Terraria;
using Terraria.ModLoader;
using Tyfyter.Utils;
using static Terraria.WorldGen;
using System;
using Terraria.ID;
using Origins.Items.Weapons.Defiled;
using Terraria.Localization;
using Terraria.GameContent.Achievements;
using Origins.Projectiles.Misc;

namespace Origins.World.BiomeData {
    public static class DefiledWastelands {
        public const int NeededTiles = 200;
        public const int ShaderTileCount = 75;
		public const short DefaultTileDust = DustID.Titanium;
        public static class SpawnRates {
            public const float Cyclops = 1;
            public const float Mite = 1;
            public const float Brute = 0.6f;
            public const float Flyer = 0.6f;
            public const float Worm = 0.6f;
            public const float Hunter = 0.6f;
		}
		public static class Gen {
			static int fisureCount = 0;
			public static void StartDefiled(float i, float j) {
				const float strength = 2.8f; //width of tunnels
				const float wallThickness = 3.1f;
				const float distance = 40; //tunnel length
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				ushort stoneWallID = (ushort)ModContent.WallType<Defiled_Stone_Wall>();
				Vector2 startVec = new Vector2(i, j);
				fisureCount = 0;
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
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].wall == WallID.None) {
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
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].wall == WallID.None) {
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
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].wall == WallID.None) {
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
							if (airCheckVec.Y < Main.worldSurface && Main.tile[(int)airCheckVec.X, (int)airCheckVec.Y].wall == WallID.None) {
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
						Vector2 p = fisureCheckSpots[ch];
						int loop = 0;
                        for (; !Main.tile[(int)p.X, (int)p.Y + 1].active(); p.Y++) {
                            if (++loop > 10) {
								break;
                            }
                        }
                        if (TileObject.CanPlace((int)p.X, (int)p.Y, fissureID, 0, 0, out TileObject to)) {
							WorldGen.Place2x2((int)p.X, (int)p.Y, fissureID, 0);
							fisureCount++;
							break;
                        }
                    }
					fisureCheckSpots.RemoveAt(ch);
                }
				Main.NewText(fisureCount);
			}
			public static void DefiledCave(float i, float j, float sizeMult = 1f) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				ushort stoneWallID = (ushort)ModContent.WallType<Defiled_Stone_Wall>();
				for (int x = (int)Math.Floor(i - (28 * sizeMult + 5)); x < (int)Math.Ceiling(i + (28 * sizeMult + 5)); x++) {
					for (int y = (int)Math.Ceiling(j + (28 * sizeMult + 4)); y >= (int)Math.Floor(j - (28 * sizeMult + 4)); y--) {
						float diff = (float)Math.Sqrt((((y - j) * (y - j)) + (x - i) * (x - i)) * (GetWallDistOffset((float)Math.Atan2(y - j, x - i) * 4 + x + y) * 0.0316076058772687986171132238548f + 1));
						if (diff > 35 * sizeMult) {
							continue;
						}
						if (Main.tile[x, y].wall != stoneWallID) {
							Main.tile[x, y].ResetToType(stoneID);
						}
						Main.tile[x, y].wall = stoneWallID;
						if (diff < 35 * sizeMult - 5) {
							Main.tile[x, y].active(false);
						}
					}
				}
			}
			public static void DefiledRibs(float i, float j, float sizeMult = 1f) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				for (int x = (int)Math.Floor(i - (28 * sizeMult + 5)); x < (int)Math.Ceiling(i + (28 * sizeMult + 5)); x++) {
					for (int y = (int)Math.Ceiling(j + (28 * sizeMult + 4)); y >= (int)Math.Floor(j - (28 * sizeMult + 4)); y--) {
						float diff = (float)Math.Sqrt((((y - j) * (y - j)) + (x - i) * (x - i)) * (GetWallDistOffset((float)Math.Atan2(y - j, x - i) * 4 + x + y) * 0.0316076058772687986171132238548f + 1));
						if (diff > 16 * sizeMult) {
							continue;
						}
                        if (Math.Cos(diff*0.7f)<=0.1f) {
							Main.tile[x, y].ResetToType(stoneID);
                        } else {
							Main.tile[x, y].active(false);
						}
					}
				}
			}
			public static void DefiledRib(float i, float j, float size = 16f, float thickness = 1) {
				ushort stoneID = (ushort)ModContent.TileType<Defiled_Stone>();
				for (int x = (int)Math.Floor(i - size); x < (int)Math.Ceiling(i + size); x++) {
					for (int y = (int)Math.Ceiling(j + size); y >= (int)Math.Floor(j - size); y--) {
                        if (Main.tile[x, y].active() && Main.tileSolid[Main.tile[x, y].type]) {
							continue;
                        }
						float diff = (float)Math.Sqrt((((y - j) * (y - j)) + (x - i) * (x - i)) * (GetWallDistOffset((float)Math.Atan2(y - j, x - i) * 4 + x + y) * 0.0316076058772687986171132238548f + 1));
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
							float el = l + (GetWallDistOffset((float)length + k) + 0.5f) / 2.5f;
							float ek = k + (GetWallDistOffset((float)length + l) + 0.5f) / 2.5f;
							double dist = Math.Pow(Math.Abs(el - pos.X), 2) + Math.Pow(Math.Abs(ek - pos.Y), 2);
							tile = Main.tile[l, k];
							bool openAir = (k < Main.worldSurface && tile.wall == WallID.None);
							if (dist > strength) {
								double d = Math.Sqrt(dist);
								if (!openAir && d < baseStrength + basewallThickness && TileID.Sets.CanBeClearedDuringGeneration[tile.type] && tile.wall != _wallType) {
									
                                    if (!Main.tileContainer[tile.type]) {
										tile.active(active: true);
										tile.ResetToType(wallBlockType);
									}
									//WorldGen.SquareTileFrame(l, k);
									if (hasWall) {
										tile.wall = _wallType;
									}
								}
								continue;
							}
							if (TileID.Sets.CanBeClearedDuringGeneration[tile.type]) {
								if (!Main.tileContainer[tile.type] && !Main.tileContainer[Main.tile[l, k-1].type]) Main.tile[l, k].active(active: false);
								//WorldGen.SquareTileFrame(l, k);
								if (hasWall && !openAir) {
									tile.wall = _wallType;
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
				NetMessage.SendTileRange(Main.myPlayer, X0, Y0, X1 - X0, Y1 - Y1);
				return (pos, speed);
			}
			//range seems to be -3 to ~2
			public static float GetWallDistOffset(float value) {
				float x = value * 0.4f;
				float halfx = x * 0.5f;
				float quarx = x * 0.5f;
				float fx0 = (float)Math.Min(Math.Pow(halfx % 3, halfx % 5), 2);
				halfx += 0.5f;
				float fx1 = (float)Math.Min(Math.Pow(halfx % 3, halfx % 5), 2);
				float fx2 = fx0 * (float)(Math.Min(Math.Pow(quarx % 3, quarx % 5), 2) + 0.5f);
				return fx0 - fx2 + fx1;
			}
		}
		
		public static void CheckFissure(int i, int j, int type) {
			if (destroyObject) {
				return;
			}
            int x = Main.tile[i, j].frameX != 0 ? i - 1 : i;
            int y = Main.tile[i, j].frameY != 0 && Main.tile[i, j].frameY != 36 ? j - 1 : j;
            for (int k = 0; k < 2; k++) {
				for (int l = 0; l < 2; l++) {
					Tile tile = Main.tile[x + k, y + l];
					if (tile != null && (!tile.nactive() || tile.type != type)) {
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
					if (Main.tile[m, n].type == type) {
						KillTile(m, n);
					}
				}
			}
			if (Main.netMode != NetmodeID.MultiplayerClient && !noTileActions) {
				if (genRand.Next(2) == 0) {
					spawnMeteor = true;
				}
				int selection = Main.rand.Next(5);
				if (!shadowOrbSmashed) {
					selection = 0;
				}
				switch (selection) {
					case 0: 
					Item.NewItem(i * 16, j * 16, 32, 32, ModContent.ItemType<Defiled_Burst>(), 1, noBroadcast: false, -1);
					int stack2 = WorldGen.genRand.Next(100, 101);
					Item.NewItem(i * 16, j * 16, 32, 32, ItemID.MusketBall, stack2);
					break;
				
					case 1:
					Item.NewItem(i * 16, j * 16, 32, 32, ItemID.Vilethorn, 1, noBroadcast: false, -1);
					break;

					case 2:
					Item.NewItem(i * 16, j * 16, 32, 32, ItemID.BallOHurt, 1, noBroadcast: false, -1);
					break;

					case 3:
					Item.NewItem(i * 16, j * 16, 32, 32, ItemID.ShadowOrb, 1, noBroadcast: false, -1);
					break;

					case 4:
					Item.NewItem(i * 16, j * 16, 32, 32, ItemID.BandofStarpower, 1, noBroadcast: false, -1);
					break;
				}
				shadowOrbSmashed = true;
				float fx = x * 16;
				float fy = y * 16;
				float dist = -1f;
				int player = 0;
				for (int index = 0; index < 255; index++) {
					float curDist = Math.Abs(Main.player[index].position.X - fx) + Math.Abs(Main.player[index].position.Y - fy);
					if (curDist < dist || dist == -1f) {
						player = index;
						dist = curDist;
					}
				}
				Projectile.NewProjectile(new Vector2((i + 1) * 16, (j + 1) * 16), Vector2.Zero, ModContent.ProjectileType<Defiled_Wastelands_Signal>(), 0, 0, ai0:1, ai1:player);
				/*
				shadowOrbCount++;
				if (shadowOrbCount >= 3) {
					shadowOrbCount = 0;
					float fx = x * 16;
					float fy = y * 16;
					float distance = -1f;
					int plr = 0;
					for (int pindex = 0; pindex < 255; pindex++) {
						float currentDist = Math.Abs(Main.player[pindex].position.X - fx) + Math.Abs(Main.player[pindex].position.Y - fy);
						if (currentDist < distance || distance == -1f) {
							plr = pindex;
							distance = currentDist;
						}
					}
					NPC.SpawnOnPlayer(plr, 1);
				} else {
					LocalizedText localizedText = Lang.misc[10];
					if (shadowOrbCount == 2) {
						localizedText = Lang.misc[11];
					}
					if (Main.netMode == NetmodeID.SinglePlayer) {
						Main.NewText(localizedText.ToString(), 50, byte.MaxValue, 130);
					}else if (Main.netMode == NetmodeID.Server) {
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(localizedText.Key), new Color(50, 255, 130));
					}
					AchievementsHelper.NotifyProgressionEvent(7);
				}
				 */
			}
			Main.PlaySound(SoundID.NPCKilled, i * 16, j * 16);
			destroyObject = false;
		}
	}
}
