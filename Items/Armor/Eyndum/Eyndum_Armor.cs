using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Eyndum {
    [AutoloadEquip(EquipType.Head)]
	public class Eyndum_Helmet : ModItem, IWikiArmorSet, INoSeperateWikiPage {
        public string[] Categories => new string[] {
            "PostMLArmorSet",
            "MeleeBoostGear",
            "RangedBoostGear",
            "MagicBoostGear",
            "SummmonBoostGear",
            "ExplosiveBoostGear"
        };
        public override void SetStaticDefaults() {
			if (Main.netMode != NetmodeID.Server) {
				Origins.AddHelmetGlowmask(Item.headSlot, "Items/Armor/Eyndum/Eyndum_Helmet_Head_Glow");
			}
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 16;
			Item.value = Item.sellPrice(gold: 20);
			Item.rare = CrimsonRarity.ID;
		}
		public override void UpdateEquip(Player player) {
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Eyndum_Breastplate>() && legs.type == ModContent.ItemType<Eyndum_Greaves>();
		}
		public override void UpdateArmorSet(Player player) {
			player.GetModPlayer<OriginPlayer>().eyndumSet = true;
			if (player.whoAmI == Main.myPlayer && !Main.gameMenu) Origins.SetEyndumCoreUI();
		}
		public override void AddRecipes() {
			/*Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 15);
            //recipe.AddIngredient(ModContent.ItemType<>(), 10);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
		}
		public string ArmorSetName => "Eyndum_Armor";
		public int HeadItemID => Type;
		public int BodyItemID => ModContent.ItemType<Eyndum_Breastplate>();
		public int LegsItemID => ModContent.ItemType<Eyndum_Greaves>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Eyndum_Breastplate : ModItem, INoSeperateWikiPage {
		public override void SetStaticDefaults() {
			if (Main.netMode != NetmodeID.Server) {
				Origins.AddBreastplateGlowmask(Item.bodySlot, "Items/Armor/Eyndum/Eyndum_Breastplate_Body_Glow");
				Origins.AddBreastplateGlowmask(-Item.bodySlot, "Items/Armor/Eyndum/Eyndum_Breastplate_FemaleBody_Glow");
			}
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 16;
			Item.value = Item.sellPrice(gold: 20);
			Item.rare = CrimsonRarity.ID;
		}
		public override void UpdateEquip(Player player) {
			player.statLifeMax2 += 20;
        }
		public override void AddRecipes() {
			/*Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 25);
            //recipe.AddIngredient(ModContent.ItemType<>(), 20);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Eyndum_Greaves : ModItem, INoSeperateWikiPage {
		public override void SetStaticDefaults() {
			if (Main.netMode != NetmodeID.Server) {
				Origins.AddLeggingGlowMask(Item.legSlot, "Items/Armor/Eyndum/Eyndum_Greaves_Legs_Glow");
			}
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 12;
			Item.value = Item.sellPrice(gold: 20);
			Item.rare = CrimsonRarity.ID;
		}
		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.2f;
		}
		public override void AddRecipes() {
			/*Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 20);
            //recipe.AddIngredient(ModContent.ItemType<>(), 15);
            recipe.SetResult(this);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();*/
		}
	}
}
