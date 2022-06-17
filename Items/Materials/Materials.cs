﻿using Origins.Tiles;
using Origins.Tiles.Defiled;
using Origins.Tiles.Riven;
using Origins.Tiles.Dusk;
using Origins.Tiles.Brine;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Materials {
    public class Acid_Bottle : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Acid Bottle");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }
        public override void SetDefaults() {
            Item.maxStack = 1;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddIngredient(ItemID.Stinger, 2);
            recipe.AddIngredient(ModContent.ItemType<Brine_Sample>(), 1);
            recipe.AddTile(TileID.AlchemyTable);
            recipe.Register();
        }
    }
    public class Acrid_Bar : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Acrid Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            //Tooltip.SetDefault();
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.TitaniumBar, 1);
            recipe.AddIngredient(ModContent.ItemType<Bleeding_Obsidian_Shard>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Acid_Bottle>(), 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
            recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.AdamantiteBar, 1);
            recipe.AddIngredient(ModContent.ItemType<Bleeding_Obsidian_Shard>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Acid_Bottle>(), 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
        }
    }
    public class Adhesive_Wrap : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Adhesive Wrap");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            //Tooltip.SetDefault();
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Tree_Sap>(), 3);
            recipe.AddIngredient(ModContent.ItemType<Silicon_Wafer>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
	public class Angelium : ModItem {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        //add lore here
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
    }
    public class Bark : ModItem {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(ModContent.ItemType<Rubber>());
            recipe.AddIngredient(this, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Bat_Hide : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Bat Hide");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            //Tooltip.SetDefault();
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(ItemID.Leather);
            recipe.AddIngredient(this, 4);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
    public class Bleeding_Obsidian_Shard : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Bleeding Obsidian Shard");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 48;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.ShadowScale);
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(ModContent.ItemType<Bleeding_Obsidian_Item>());
            recipe.AddIngredient(this, 6);
            recipe.Register();
            recipe = Mod.CreateRecipe(Type, 6);
            recipe.AddIngredient(ModContent.ItemType<Bleeding_Obsidian_Item>());
            recipe.Register();
        }
    }
    public class Brine_Sample : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Brine Sample");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.ShadowScale);
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddCondition(
                Terraria.Localization.NetworkText.FromLiteral("Brine"),
                (_) => Main.LocalPlayer.adjWater && Main.LocalPlayer.GetModPlayer<OriginPlayer>().ZoneBrine
            );
            recipe.Register();
            recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<Sulphur_Stone_Item>()); //Forgot to implement Decaying Mush...
            recipe.AddTile(TileID.AlchemyTable);
            recipe.Register();
        }
    }
    public class Defiled_Bar : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Defiled Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Ore_Item>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
    public class Defiled_Key : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Defiled Key");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
			Item.width = 14;
			Item.height = 20;
			Item.maxStack = 99;
        }
    }
    public class Ember_Onyx : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Ember Onyx");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
    }
    public class Felnum_Bar : ModItem {
        /*
         * brown color in its natural form
         * tinted silver color when hardened
         * exhibits a property named "electrical greed" where it grows hard blue crystals from anywhere it would lose electrons, to effectively "reclaim" them, even if it already has a strong negative charge
         */
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Felnum Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Felnum_Ore_Item>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
    public class Infested_Bar : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Infested Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Infested_Ore_Item>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
    public class Shaping_Matter : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Meta Gel");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            //Tooltip.SetDefault();
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
        }
    }
    public class Peat_Moss : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Peat Moss");
            Tooltip.SetDefault("The Demolitionist might find this interesting...");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.value = 300;//3 silver
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(ItemID.ExplosivePowder);
            recipe.AddIngredient(this, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Rivenform : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Rivenform");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            //Tooltip.SetDefault();
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
        }
    }
    public class Riven_Key : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Riven Key");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.width = 14;
            Item.height = 20;
            Item.maxStack = 99;
        }
    }
    public class Riven_Sample : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Riven Sample");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
        }
    }
    public class Rubber : ModItem {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
    }
    public class Silicon_Wafer : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Silicon Packet");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ItemID.SandBlock, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Strange_String : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Strange String");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        //add lore here
        public override void SetDefaults() {
            Item.maxStack = 99;
        }
    }
    public class Tree_Sap : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Tree Sap");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(ModContent.ItemType<Rubber>());
            recipe.AddIngredient(this, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Undead_Chunk : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Undead Chunk");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(ItemID.ObsidianHelm);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = Mod.CreateRecipe(ItemID.ObsidianShirt);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 10);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = Mod.CreateRecipe(ItemID.ObsidianPants);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
    public class Valkyrum_Bar : ModItem {
        //Alloy of Felnum and Angelium
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Valkyrum Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Felnum_Bar>(), 1);
            //recipe.AddIngredient(ModContent.ItemType<_Bar>(), 1);
            recipe.AddIngredient(ItemID.Ectoplasm, 1);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
    public class Viridium_Bar : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Viridium Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
    }
    public class Wilting_Rose_Item : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Wilting Rose");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
    }
}