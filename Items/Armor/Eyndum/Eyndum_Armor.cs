using Origins.Items.Materials;
using Terraria;
using Origins.Buffs;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Eyndum {
    [AutoloadEquip(EquipType.Head)]
    public class Eyndum_Helmet : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Eyndum Helmet");
            if (Main.netMode != NetmodeID.Server) {
                Origins.AddHelmetGlowmask(Item.headSlot, "Items/Armor/Eyndum/Eyndum_Helmet_Head_Glow");
            }
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.defense = 8;
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateEquip(Player player) {
        }
        public override bool IsArmorSet(Item head, Item body, Item legs) {
            return body.type == ModContent.ItemType<Eyndum_Breastplate>() && legs.type == ModContent.ItemType<Eyndum_Greaves>();
        }
        public override void UpdateArmorSet(Player player) {
            player.GetModPlayer<OriginPlayer>().eyndumSet = true;
            Origins.instance.SetEyndumCoreUI();
        }
        public override void AddRecipes() {
            /*Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 15);
            //recipe.AddIngredient(ModContent.ItemType<>(), 10);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
        }
    }
    [AutoloadEquip(EquipType.Body)]
    public class Eyndum_Breastplate : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Eyndum Breastplate");
            if (Main.netMode != NetmodeID.Server) {
                Origins.AddBreastplateGlowmask(Item.bodySlot, "Items/Armor/Eyndum/Eyndum_Breastplate_Body_Glow");
                Origins.AddBreastplateGlowmask(-Item.bodySlot, "Items/Armor/Eyndum/Eyndum_Breastplate_FemaleBody_Glow");
            }
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.defense = 16;
            Item.wornArmor = true;
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateEquip(Player player) {

        }
        public override void AddRecipes() {
            /*Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 25);
            //recipe.AddIngredient(ModContent.ItemType<>(), 20);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
        }
    }
    [AutoloadEquip(EquipType.Legs)]
    public class Eyndum_Greaves : ModItem {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Eyndum Greaves");
            if (Main.netMode != NetmodeID.Server) {
                Origins.AddLeggingGlowMask(Item.legSlot, "Items/Armor/Eyndum/Eyndum_Greaves_Legs_Glow");
            }
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override void SetDefaults() {
            Item.defense = 12;
            Item.rare = ItemRarityID.Pink;
        }
        public override void UpdateEquip(Player player) {

        }
        public override void AddRecipes() {
            /*Recipe recipe = Mod.CreateRecipe(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 20);
            //recipe.AddIngredient(ModContent.ItemType<>(), 15);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
        }
    }
}
