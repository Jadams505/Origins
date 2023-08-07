using Origins.Tiles.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Amber {
    [AutoloadEquip(EquipType.Head)]
	public class Amber_Helmet : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Amber Exploder Goggles");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 7;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightRed;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Amber_Breastplate>() && legs.type == ModContent.ItemType<Amber_Greaves>();
		}
		public override void UpdateArmorSet(Player player) {
			player.setBonus = "-38% explosive blast radius. All explosives are preserved in amber and release amber shards upon detonation\nAmber shards slow enemies, heal the player of any self-damage received, and decrease the defense of struck enemies by half";
			player.GetModPlayer<OriginPlayer>().explosiveBlastRadius -= 0.38f;
			//player.GetModPlayer<OriginPlayer>().amberSet = true;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.Amber, 4);
			recipe.AddIngredient(ItemID.SoulofNight, 3);
			recipe.AddIngredient(ModContent.ItemType<Carburite_Item>(), 12);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Body)]
	public class Amber_Breastplate : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Amber Exploder Resin");
			// Tooltip.SetDefault("-34% explosive self-damage");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 7;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.GetModPlayer<OriginPlayer>().explosiveSelfDamage -= 0.34f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.Amber, 12);
			recipe.AddIngredient(ItemID.SoulofNight, 3);
			recipe.AddIngredient(ModContent.ItemType<Carburite_Item>(), 36);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Amber_Greaves : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Amber Exploder Guards");
			// Tooltip.SetDefault("Increased movement speed");
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 7;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.1f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.Amber, 8);
			recipe.AddIngredient(ItemID.SoulofNight, 3);
			recipe.AddIngredient(ModContent.ItemType<Carburite_Item>(), 24);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
	}
}
