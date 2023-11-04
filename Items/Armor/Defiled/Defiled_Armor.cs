using Microsoft.Xna.Framework;
using Origins.Dev;
using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Defiled {
    [AutoloadEquip(EquipType.Head)]
	public class Defiled_Helmet : ModItem, IWikiArmorSet, INoSeperateWikiPage {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("{$Defiled} Helmet");
			// Tooltip.SetDefault("5% increased critical strike chance");
			if (Main.netMode != NetmodeID.Server) {
				Origins.AddHelmetGlowmask(Item.headSlot, "Items/Armor/Defiled/Defiled_Helmet_Head_Glow");
			}
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 6;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetCritChance(DamageClass.Generic) += 5;
		}
        public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Defiled_Breastplate>() && legs.type == ModContent.ItemType<Defiled_Greaves>();
		}
		public override void UpdateArmorSet(Player player) {
			player.setBonus = "Greatly increased maximum life";
			player.statLifeMax2 += (int)(player.statLifeMax2 * 0.25);
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 15);
			recipe.AddIngredient(ModContent.ItemType<Undead_Chunk>(), 10);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public string ArmorSetName => "Defiled_Armor";
		public int HeadItemID => Type;
		public int BodyItemID => ModContent.ItemType<Defiled_Breastplate>();
		public int LegsItemID => ModContent.ItemType<Defiled_Greaves>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Defiled_Breastplate : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 7;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetDamage(DamageClass.Generic) += 0.03f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 25);
			recipe.AddIngredient(ModContent.ItemType<Undead_Chunk>(), 20);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Defiled_Greaves : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 6;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetDamage(DamageClass.Generic) += 0.03f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 20);
			recipe.AddIngredient(ModContent.ItemType<Undead_Chunk>(), 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Head)]
	public class Defiled2_Helmet : Defiled_Helmet, IWikiArmorSet, INoSeperateWikiPage {
		public override void AddRecipes() {}
		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) { }
		public new string ArmorSetName => "Ancient_Defiled_Armor";
		public new int HeadItemID => Type;
		public new int BodyItemID => ModContent.ItemType<Defiled2_Breastplate>();
		public new int LegsItemID => ModContent.ItemType<Defiled2_Greaves>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Defiled2_Breastplate : Defiled_Breastplate {
		public override void AddRecipes() {}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Defiled2_Greaves : Defiled_Greaves {
		public override void SetStaticDefaults() {}
		public override void AddRecipes() {}
	}
}
