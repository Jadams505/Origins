﻿using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Other {
    [AutoloadEquip(EquipType.Head)]
	public class Ancient_Hallowed_Visage : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Ancient Hallowed Visage");
			Tooltip.SetDefault("+15% explosive velocity\n+8% explosive critical strike chance");
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 26;
			Item.defense = 3;
			Item.value = Item.buyPrice(gold: 5);
			Item.rare = ItemRarityID.Pink;
		}
		public override void UpdateEquip(Player player) {
			player.GetModPlayer<OriginPlayer>().explosiveProjectileSpeed += 0.15f;
			player.GetCritChance(DamageClasses.Explosive) += 8;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ItemID.HallowedPlateMail && legs.type == ItemID.HallowedGreaves;
		}
		public override void UpdateArmorSet(Player player) {
			player.setBonus = Language.GetTextValue("ArmorSetBonus.Hallowed");
			player.onHitDodge = true;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.HallowedBar, 12);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}
