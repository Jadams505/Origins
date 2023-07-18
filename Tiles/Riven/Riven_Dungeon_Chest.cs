﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.Tiles.Riven {
	public class Riven_Dungeon_Chest : ModChest, IGlowingModTile {
		public AutoCastingAsset<Texture2D> GlowTexture { get; private set; }
		public Color GlowColor => Color.White;
		public override void SetStaticDefaults() {
			if (!Main.dedServ) {
				GlowTexture = Mod.Assets.Request<Texture2D>("Tiles/Riven/Riven_Dungeon_Chest_Glow");
			}
			base.SetStaticDefaults();
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Riven Chest");
			AddMapEntry(new Color(200, 200, 200), name, MapChestName);
			name = CreateMapEntryName(Name + "_Locked"); // With multiple map entries, you need unique translation keys.
			// name.SetDefault("Locked Riven Chest");
			AddMapEntry(new Color(140, 140, 140), name, MapChestName);
			//disableSmartCursor = true;
			AdjTiles = new int[] { TileID.Containers };
			ContainerName/* tModPorter Note: Removed. Override DefaultContainerName instead */.SetDefault("Riven Chest");
			ItemDrop/* tModPorter Note: Removed. Tiles and walls will drop the item which places them automatically. Use RegisterItemDrop to alter the automatic drop if necessary. */ = ModContent.ItemType<Riven_Dungeon_Chest_Item>();
			keyItem = ModContent.ItemType<Riven_Key>();
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch) {
			this.DrawChestGlow(i, j, spriteBatch);
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
			r = 0.2f;
			g = 0.15f;
			b = 0.06f;
		}
	}
	public class Riven_Dungeon_Chest_Item : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("{$Riven} Chest");
		}
		public override void SetDefaults() {
			Item.width = 32;
			Item.height = 32;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.value = 500;
			Item.createTile = ModContent.TileType<Riven_Dungeon_Chest>();
		}
	}
}
