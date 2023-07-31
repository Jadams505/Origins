﻿using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Other.Consumables {
	public class Nullification_Potion : ModItem {
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Nullification Potion");
			// Tooltip.SetDefault("Removes all current harmful effects");
			Item.ResearchUnlockCount = 20;
		}
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.WrathPotion);
			Item.buffTime = 6;
			Item.value = Item.sellPrice(silver: 2);
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type, 5);
			recipe.AddIngredient(ItemID.StrangeBrew, 5);
			recipe.AddIngredient(ModContent.ItemType<Lunar_Token>());
			recipe.AddTile(TileID.Bottles);
			recipe.Register();
		}
		public override bool? UseItem(Player player) {
			//debuff removal

			for (int i = 0; i < player.buffType.Length; i++) {
				int buffType = player.buffType[i];
				if (buffType == 0) break;
				if (Main.debuff[buffType] && (!BuffID.Sets.NurseCannotRemoveDebuff[buffType] || buffType == BuffID.Suffocation)) {
					player.DelBuff(i--);
				}
			}
			return null;
		}
	}
}
