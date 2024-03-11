﻿using Origins.Dev;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Origins.Items.Accessories {
	[AutoloadEquip(EquipType.HandsOn)]
	public class Destructive_Claws : ModItem, ICustomWikiStat {
		public string[] Categories => new string[] {
			"Combat",
			"Explosive"
		};
        static short glowmask;
        public override void SetStaticDefaults() {
            glowmask = Origins.AddGlowMask(this);
        }
        public override void SetDefaults() {
			Item.DefaultToAccessory(38, 20);
			Item.value = Item.sellPrice(gold: 3);
			Item.rare = ItemRarityID.Orange;
            Item.glowMask = glowmask;
        }
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.FeralClaws);
			recipe.AddIngredient(ModContent.ItemType<Bomb_Handling_Device>());
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
		public override void UpdateEquip(Player player) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			player.GetAttackSpeed(DamageClasses.Explosive) += 0.1f;
			originPlayer.destructiveClaws = true;
			originPlayer.explosiveThrowSpeed += 0.25f;
		}
	}
}
