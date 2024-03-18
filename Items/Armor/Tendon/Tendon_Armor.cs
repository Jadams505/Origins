using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Tendon {
    [AutoloadEquip(EquipType.Head)]
	public class Tendon_Helmet : ModItem, IWikiArmorSet, INoSeperateWikiPage {
        public string[] Categories => new string[] {
            "ArmorSet",
            "RangedBoostGear",
			"ExplosiveBoostGear",
			"GenericBoostGear"
        };
        public override void SetDefaults() {
			Item.defense = 3;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetDamage(DamageClass.Ranged) += 0.1f;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Tendon_Shirt>() && legs.type == ModContent.ItemType<Tendon_Pants>();
		}
		public override void UpdateArmorSet(Player player) {
			player.setBonus = "Mobility is based on available lifeforce";
			player.GetModPlayer<OriginPlayer>().tendonSet = true;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.CrimtaneOre, 8);
			recipe.AddIngredient(ItemID.Vertebrae, 16);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public string ArmorSetName => "Tendon_Armor";
		public int HeadItemID => Type;
		public int BodyItemID => ModContent.ItemType<Tendon_Shirt>();
		public int LegsItemID => ModContent.ItemType<Tendon_Pants>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Tendon_Shirt : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 4;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetCritChance(DamageClass.Generic) += 0.06f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.CrimtaneOre, 20);
			recipe.AddIngredient(ItemID.Vertebrae, 28);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Tendon_Pants : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 3;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.ammoBox = true;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.CrimtaneOre, 14);
			recipe.AddIngredient(ItemID.Vertebrae, 22);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}
