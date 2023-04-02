﻿using Origins.Tiles.Other;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Accessories {
	[AutoloadEquip(EquipType.Shield)]
	public class Resin_Shield : ModItem {
		public static int ShieldID { get; private set; }
		public static int InactiveShieldID { get; private set; }
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Resin Shield");
			Tooltip.SetDefault("Blocks all self-damage on next hit\n10 second cooldown\n'A shield to withstand the test of time'");
			SacrificeTotal = 1;
			ShieldID = Item.shieldSlot;
		}
		public override void Load() {
			if (Main.netMode == NetmodeID.Server) return;
			InactiveShieldID = EquipLoader.AddEquipTexture(
				Mod,
				"Origins/Items/Accessories/Resin_Shield_Shield_Inactive",
				EquipType.Shield,
				name: "Resin_Shield_Shield_Inactive"
			);
		}
		public override void SetDefaults() {
			Item.DefaultToAccessory(36, 38);
			Item.defense = 3;
			Item.value = Item.sellPrice(gold: 4);
			Item.rare = ItemRarityID.Pink;
		}
		public override void UpdateEquip(Player player) {
			player.noKnockback = true;
			player.fireWalk = true;
			player.GetModPlayer<OriginPlayer>().ResinShield = true;
		}
		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Amber, 6);
			recipe.AddIngredient(ItemID.ObsidianShield);
			recipe.AddIngredient(ModContent.ItemType<Carburite_Item>(), 12);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}
