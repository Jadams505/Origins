﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Localization;

namespace Origins.Tiles {
    public abstract class ModChest : ModTile {
        public int keyItem { get; protected set; } = -1;
        public override void SetStaticDefaults() {
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
		}

		public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX < 36 ? 1 : 0);

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) => true;

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX >= 36;

		public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual) {
            //frameXAdjustment = 36;
			dustType = this.DustType;
			return true;
		}

		public string MapChestName(string name, int i, int j) {
			int left = i;
			int top = j;
			Tile tile = Main.tile[i, j];
			if (tile.TileFrameX % 36 != 0) {
				left--;
			}
			if (tile.TileFrameY != 0) {
				top--;
			}
			int chest = Chest.FindChest(left, top);
			if (chest < 0) {
				return Language.GetTextValue("LegacyChestType.0");
			}else if (Main.chest[chest].name == "") {
				return name;
			}else {
				return name + ": " + Main.chest[chest].name;
			}
		}

		public override void NumDust(int i, int j, bool fail, ref int num) {
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) {
			Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 32, 32, ChestDrop);
			Chest.DestroyChest(i, j);
		}

		public override bool RightClick(int i, int j) {
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0) {
				left--;
			}
			if (tile.TileFrameY != 0) {
				top--;
			}
			if (player.sign >= 0) {
				SoundEngine.PlaySound(SoundID.MenuClose);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}
			if (Main.editChest) {
				SoundEngine.PlaySound(SoundID.MenuTick);
				Main.editChest = false;
				Main.npcChatText = "";
			}
			if (player.editedChestName) {
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
				player.editedChestName = false;
			}
			bool isLocked = IsLockedChest(left, top);
			if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked) {
				if (left == player.chestX && top == player.chestY && player.chest >= 0) {
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.MenuClose);
				} else {
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, (float)top, 0f, 0f, 0, 0, 0);
					Main.stackSplit = 600;
				}
			} else {
				if (isLocked) {
					if (player.ConsumeItem(keyItem) && Chest.Unlock(left, top)) {
						if (Main.netMode == NetmodeID.MultiplayerClient) {
							NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, (float)left, (float)top);
						}
					}
				} else {
					int chest = Chest.FindChest(left, top);
					if (chest >= 0) {
						player.OpenChest(i, j, chest);
						/*Main.stackSplit = 600;
						if (chest == player.chest) {
							player.chest = -1;
							SoundEngine.PlaySound(SoundID.MenuClose);
						} else {
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = left;
							player.chestY = top;
							SoundEngine.PlaySound(player.chest < 0 ? SoundID.MenuOpen : SoundID.MenuTick);
						}*/
						Recipe.FindRecipes();
					}
				}
			}
			return true;
		}

		public override void MouseOver(int i, int j) {
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0) {
				left--;
			}
			if (tile.TileFrameY != 0) {
				top--;
			}
			int chestIndex = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (chestIndex < 0) {
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			} else {
                player.cursorItemIconText = Main.chest[chestIndex].name.Length > 0 ? Main.chest[chestIndex].name : ContainerName.GetDefault();
				if (player.cursorItemIconText.Equals(ContainerName.GetDefault())) {
					if (IsLockedChest(left,top))player.cursorItemIconID = keyItem;
                    else player.cursorItemIconID = ChestDrop;
					player.cursorItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j) {
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "") {
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
    }
}
