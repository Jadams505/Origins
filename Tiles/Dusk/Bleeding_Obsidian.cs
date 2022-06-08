﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Origins.Tiles.Dusk {
	public class Bleeding_Obsidian : OriginTile {
		public override void SetStaticDefaults() {
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.CanBeClearedDuringGeneration[Type] = true;
            ItemDrop = ItemType<Bleeding_Obsidian_Shard>();
			AddMapEntry(new Color(57, 10, 75));
		}
		public override bool Drop(int i, int j) {
            Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, ItemDrop, Main.rand.Next(4, 7));
            return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) {
            float m = 0.1f;
            r = 37.2f*m;
            g = 6.7f*m;
            b = 49.2f*m;
        }
    }
    public class Bleeding_Obsidian_Item : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Bleeding Obsidian");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.StoneBlock);
            Item.createTile = TileType<Bleeding_Obsidian>();
		}
    }
}
