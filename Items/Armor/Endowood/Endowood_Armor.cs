using Origins.Dev;
using Origins.Tiles.Defiled;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Endowood {
	[AutoloadEquip(EquipType.Head)]
	public class Endowood_Helmet : ModItem, IWikiArmorSet, INoSeperateWikiPage {
        public string[] Categories => new string[] {
            "ArmorSet"
        };
        public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 1;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Endowood_Breastplate>() && legs.type == ModContent.ItemType<Endowood_Greaves>();
		}
		public override void UpdateArmorSet(Player player) {
			player.statDefense += 1;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Endowood_Item>(), 20);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
		public string ArmorSetName => "Endowood_Armor";
		public int HeadItemID => Type;
		public int BodyItemID => ModContent.ItemType<Endowood_Breastplate>();
		public int LegsItemID => ModContent.ItemType<Endowood_Greaves>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Endowood_Breastplate : ModItem, INoSeperateWikiPage {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 2;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Endowood_Item>(), 30);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Endowood_Greaves : ModItem, INoSeperateWikiPage {
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 1;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Endowood_Item>(), 25);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}