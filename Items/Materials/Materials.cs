﻿using Origins.Tiles;
using Origins.Tiles.Defiled;
using Origins.Tiles.Riven;
using Origins.Tiles.Dusk;
using Origins.Tiles.Brine;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

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
            Recipe recipe = Recipe.Create(Type);
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
        }
        public override void SetDefaults() {
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.TitaniumBar, 1);
            recipe.AddIngredient(ModContent.ItemType<Bleeding_Obsidian_Shard>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Acid_Bottle>(), 3);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();
            recipe = Recipe.Create(Type);
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
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Tree_Sap>(), 3);
            recipe.AddIngredient(ModContent.ItemType<Silicon_Wafer>(), 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
    public class Amebic_Gel : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            glowmask = Origins.AddGlowMask(this, "");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
            Item.glowMask = glowmask;
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
            Recipe recipe = Recipe.Create(ModContent.ItemType<Rubber>());
            recipe.AddIngredient(this, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Bat_Hide : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Bat Hide");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(ItemID.Leather);
            recipe.AddIngredient(this, 4);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
    }
    public class Bleeding_Obsidian_Shard : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Bleeding Obsidian Shard");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 48;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Pink;
            Item.glowMask = glowmask;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(ModContent.ItemType<Bleeding_Obsidian_Item>());
            recipe.AddIngredient(this, 6);
            recipe.Register();
            recipe = Recipe.Create(Type, 6);
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
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddCondition(
                Terraria.Localization.NetworkText.FromLiteral("Brine"),
                (_) => Main.LocalPlayer.adjWater && Main.LocalPlayer.GetModPlayer<OriginPlayer>().ZoneBrine
            );
            recipe.Register();
            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ModContent.ItemType<Sulphur_Stone_Item>()); //Forgot to implement Decaying Mush...
            recipe.AddTile(TileID.AlchemyTable);
            recipe.Register();
        }
    }
    public class Bud_Barnacle : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Bud Barnacle");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 30;
        }
        public override void SetDefaults() {
            Item.maxStack = 1;
        }
    }
    public class Busted_Servo : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Busted Servo");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Pink;
        }
    }
    public class Conductor_Rod : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Conductor Rod");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.CopperBar, 3);
            recipe.AddTile(TileID.Anvils); //Fabricator not implemented yet
            recipe.Register();
        }
    }
    public class Chromtain_Bar : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Chromtain Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.FragmentSolar, 4);
            recipe.AddIngredient(ModContent.ItemType<Formium_Bar>(), 4);
            recipe.AddTile(TileID.Anvils); //Omni-Printer also not implemented
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
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
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
            Item.rare = ItemRarityID.Yellow;
        }
    }
    public class Element36_Bundle : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Element-36 Bundle");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.FragmentNebula, 2);
            recipe.AddIngredient(ItemID.FragmentStardust, 2);
            recipe.AddIngredient(ModContent.ItemType<Formium_Bar>(), 4);
            recipe.AddIngredient(ModContent.ItemType<Fibron_Plating>(), 4);
            recipe.AddTile(TileID.Anvils); //Omni-Printer also not implemented
            recipe.Register();
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
    public class Eyndum_Bar : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Eyndum Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.GoldBar, 2);
            recipe.AddIngredient(ItemID.FragmentVortex, 4);
            recipe.AddIngredient(ModContent.ItemType<Void_Spark>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Formium_Bar>(), 4);
            recipe.AddTile(TileID.Anvils); //Omni-Printer also not implemented, still maybe a unique forge and dimension
            recipe.Register();
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
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Felnum_Ore_Item>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
    public class Fibron_Plating : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Fibron Plating");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
        }
    }
    public class Formium_Bar : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Formium Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Formium_Scrap>(), 6);
            recipe.Register();
        }
    }
    public class Formium_Scrap : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Formium Scrap");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Red;
        }
    }
    public class Infested_Bar : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Encrusted Bar");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Infested_Ore_Item>(), 3);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
    public class Lunar_Token : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Lunar Token");
            Tooltip.SetDefault("Valuable to the demented.");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Cyan;
            //Item.IsCurrency = true;
            Item.glowMask = glowmask;
        }
    }
    public class Shaping_Matter : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Meta Gel");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Orange;
        }
    }
    public class Modular_Plating : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Modular Plating");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.HallowedBar, 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
    public class Nova_Fragment : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Nova Fragment");
            Tooltip.SetDefault("The essence of a dying star in its last moment...");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FragmentSolar); //I thought this would make it float like souls and other fragments
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Cyan;
            Item.glowMask = glowmask;
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
            Recipe recipe = Recipe.Create(ItemID.ExplosivePowder);
            recipe.AddIngredient(this, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Power_Core : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Power Core");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Pink;
            Item.glowMask = glowmask;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.HallowedBar, 4);
            recipe.AddIngredient(ModContent.ItemType<Acrid_Bar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<Conductor_Rod>(), 2);
            recipe.AddTile(TileID.Anvils); //Fabricator not implemented yet
            recipe.Register();
        }
    }
    public class Quantium : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Qube");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
            Item.glowMask = glowmask;
        }
    }
    public class Riven_Key : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Riven Key");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.width = 18;
            Item.height = 30;
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Yellow;
            Item.glowMask = glowmask;
        }
    }
    public class Riven_Sample : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Riven Carapace");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
            Item.glowMask= glowmask;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(ItemID.ObsidianHelm);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = Recipe.Create(ItemID.ObsidianShirt);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 10);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = Recipe.Create(ItemID.ObsidianPants);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
        public class Rotor : ModItem {
        //add lore here
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type, 5);
            recipe.AddIngredient(ModContent.ItemType<Silicon_Wafer>(), 2);
            recipe.AddIngredient(ModContent.ItemType<Conductor_Rod>(), 2);
            recipe.AddIngredient(ItemID.HallowedBar, 2);
            recipe.AddTile(TileID.Anvils); //Fabricator not implemented yet
            recipe.Register();
        }
    }
    public class Rubber : ModItem {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Bark>(), 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
            recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Tree_Sap>(), 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
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
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.SandBlock, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Space_Goo : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Space Goo");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Yellow;
            Item.glowMask = glowmask;
        }
    }
    public class Space_Rock : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Space Rock");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
        }
    }
    public class Stellar_Spark : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Stellar Spark");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
            Item.glowMask = glowmask;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.FallenStar, 12);
            recipe.AddIngredient(ItemID.FragmentSolar, 2);
            recipe.AddIngredient(ItemID.FragmentStardust, 2);
            recipe.AddIngredient(ModContent.ItemType<Lunar_Token>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
    public class Strange_String : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Strange String");
            Tooltip.SetDefault("'Involuntary neurectomy'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(ItemID.BattlePotion);
            recipe.AddIngredient(ItemID.BottledWater, 1);
            recipe.AddIngredient(ItemID.Deathweed, 1);
            recipe.AddIngredient(Type, 1);
            recipe.AddTile(TileID.Bottles);
            recipe.Register();

            recipe = Recipe.Create(ItemID.MechanicalWorm);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 6);
            recipe.AddIngredient(Type, 6);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
    public class Superconductor : ModItem {
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 20;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type, 3);
            recipe.AddIngredient(ItemID.GoldBar, 7);
            recipe.AddIngredient(ModContent.ItemType<Formium_Bar>(), 3);
            recipe.AddIngredient(ModContent.ItemType<Conductor_Rod>(), 5);
            recipe.AddIngredient(ModContent.ItemType<Rubber>(), 20);
            recipe.AddTile(TileID.Anvils); //No Omni-Printer
            recipe.Register();
        }
    }
    public class Thruster_Component : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Thruster Component");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type, 2);
            recipe.AddIngredient(ModContent.ItemType<Fibron_Plating>(), 8);
            recipe.AddIngredient(ModContent.ItemType<Space_Goo>(), 4);
            recipe.AddTile(TileID.Anvils); //No Omni-Printer
            recipe.Register();
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
            Recipe recipe = Recipe.Create(ModContent.ItemType<Rubber>());
            recipe.AddIngredient(this, 3);
            recipe.AddTile(TileID.GlassKiln);
            recipe.Register();
        }
    }
    public class Undead_Chunk : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Undead Chunk");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
        public override void SetDefaults() {
            Item.maxStack = 99;
            Item.rare = ItemRarityID.Blue;
            Item.glowMask = glowmask;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(ItemID.ObsidianHelm);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = Recipe.Create(ItemID.ObsidianShirt);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 10);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
            recipe = Recipe.Create(ItemID.ObsidianPants);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(this, 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
    public class Unpowered_Eyndum_Core : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Unpowered Eyndum Core");
            Tooltip.SetDefault("'Limitless potential'");
            Item.rare = ItemRarityID.Red;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 2;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 8;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Superconductor>(), 10);
            recipe.AddIngredient(ModContent.ItemType<Eyndum_Bar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<Formium_Bar>(), 4);
            recipe.AddTile(TileID.Anvils); //No Omni-Printer
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
            Item.rare = ItemRarityID.Yellow;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
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
            Item.rare = ItemRarityID.Lime;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.ChlorophyteOre, 3); //Need Taranum
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
    public class Void_Spark : ModItem {
        static short glowmask;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Void Spark");
            glowmask = Origins.AddGlowMask(this);
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
            
        }
        public override void SetDefaults() {
            Item.maxStack = 999;
            Item.rare = ItemRarityID.Purple;
            Item.glowMask = glowmask;
        }
        public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.FragmentVortex, 4);
            recipe.AddIngredient(ModContent.ItemType<Bleeding_Obsidian_Item>(), 12);
            recipe.AddIngredient(ModContent.ItemType<Lunar_Token>());
            recipe.AddTile(TileID.Anvils); //You guessed it, no Omni-Printer
            recipe.Register();
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