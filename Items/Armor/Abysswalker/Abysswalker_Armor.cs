using Origins.Dev;
using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Abysswalker {
    [AutoloadEquip(EquipType.Head)]
	public class Abysswalker_Hood : ModItem, IWikiArmorSet, INoSeperateWikiPage {
        public string[] Categories => [
            "ArmorSet",
            "ManaShielding",
            "SummmonBoostGear",
            "ExplosiveBoostGear"
        ];
        public override void SetDefaults() {
			Item.defense = 5;
			Item.value = Item.sellPrice(silver: 40);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.statLifeMax2 += 20;
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Abysswalker_Cloak>() && legs.type == ModContent.ItemType<Abysswalker_Greaves>();
		}
		public override void UpdateArmorSet(Player player) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.mimicSet = true;

			float defiledPercentage = 1f; //OriginSystem.totalDefiled / (float)WorldGen.totalSolid;

			player.setBonus = $"Not yet fully implemented\nIncreases max assimilation by 50%\nGain different abilities depending on accrued assimilation amounts from each evil";
			if (player.whoAmI == Main.myPlayer) Origins.SetMimicSetUI();

			int mimicSetLevel = OriginSystem.MimicSetLevel;
			if (mimicSetLevel >= 1) {
				switch (originPlayer.GetMimicSetChoice(0)) {
					case 1:
					//BROADCAST
					break;
					case 2:
					//DREAM
					break;
					case 3:
					//GROW
					player.statLifeMax2 += (int)(200 * defiledPercentage);
					player.statManaMax2 += (int)(40 * defiledPercentage);
					player.moveSpeed += 1 * defiledPercentage;
					player.lifeRegenCount += (int)(7 * defiledPercentage);
					player.jumpSpeedBoost += 5 * defiledPercentage;
					player.AddMaxBreath((int)(157 * defiledPercentage));
					player.statDefense += (int)(10 * defiledPercentage);
					break;
				}
			}
			if (mimicSetLevel >= 2) {
				switch (originPlayer.GetMimicSetChoice(1)) {
					case 1:
					//INJECT
					originPlayer.setActiveAbility = 1;
					break;
					case 2:
					//MANIPULATE
					break;
					case 3:
					//FOCUS
					originPlayer.explosiveThrowSpeed += 0.4f * defiledPercentage;
					originPlayer.explosiveSelfDamage -= defiledPercentage;
					//originPlayer.explosiveBlastRadius += 0.5f * defiledPercentage;
					if (defiledPercentage == 1) {
						player.GetModPlayer<OriginPlayer>().bleedingObsidianSet = true;
					}
					break;
				}
			}
			if (mimicSetLevel >= 3) {
				switch (originPlayer.GetMimicSetChoice(2)) {
					case 1:
					//FLOAT
					if (player.wings == 0) {
						player.wings = 8;
					}
					if (player.wingsLogic == 0 || player.wingTimeMax <= 140) {
						player.wingsLogic = 26;
						player.wingTimeMax = 140;
						player.wingTimeMax += (int)(60 * defiledPercentage);
					}
					player.noFallDmg = true;
					break;
					case 2:
					//COMMAND
					break;
					case 3:
					//EVOLVE
					break;
				}
			}
		}
		public override void AddRecipes() {
			return;
			Recipe.Create(Type)
			.AddIngredient(ItemID.SoulofNight, 4)
			.AddRecipeGroupWithItem(OriginSystem.DeathweedRecipeGroupID, showItem: ItemID.Deathweed, 8)
			.AddRecipeGroupWithItem(OriginSystem.RottenChunkRecipeGroupID, showItem: ItemID.Vertebrae, 13)
			.AddRecipeGroupWithItem(OriginSystem.ShadowScaleRecipeGroupID, showItem: ModContent.ItemType<Undead_Chunk>(), 15)
			.AddRecipeGroupWithItem(OriginSystem.CursedFlameRecipeGroupID, showItem: ModContent.ItemType<Alkahest>(), 10)
			.AddTile(TileID.DemonAltar)
			.Register();
		}
		public string ArmorSetName => "Abysswalker_Armor";
		public int HeadItemID => Type;
		public int BodyItemID => ModContent.ItemType<Abysswalker_Cloak>();
		public int LegsItemID => ModContent.ItemType<Abysswalker_Greaves>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Abysswalker_Cloak : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 11;
			Item.value = Item.sellPrice(silver: 40);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.lifeRegenCount += 2;
		}
		public override void AddRecipes() {
			return;
			Recipe.Create(Type)
			.AddIngredient(ItemID.SoulofNight, 4)
			.AddRecipeGroupWithItem(OriginSystem.DeathweedRecipeGroupID, showItem: ItemID.Deathweed, 24)
			.AddRecipeGroupWithItem(OriginSystem.RottenChunkRecipeGroupID, showItem: ItemID.Vertebrae, 39)
			.AddRecipeGroupWithItem(OriginSystem.ShadowScaleRecipeGroupID, showItem: ModContent.ItemType<Undead_Chunk>(), 45)
			.AddRecipeGroupWithItem(OriginSystem.CursedFlameRecipeGroupID, showItem: ModContent.ItemType<Alkahest>(), 30)
			.AddTile(TileID.DemonAltar)
			.Register();
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Abysswalker_Greaves : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 8;
			Item.value = Item.sellPrice(gold: 2);
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateEquip(Player player) {
			player.moveSpeed += 0.15f;
		}
		public override void AddRecipes() {
			return;
			Recipe.Create(Type)
			.AddIngredient(ItemID.SoulofNight, 4)
			.AddRecipeGroupWithItem(OriginSystem.DeathweedRecipeGroupID, showItem: ItemID.Deathweed, 16)
			.AddRecipeGroupWithItem(OriginSystem.RottenChunkRecipeGroupID, showItem: ItemID.Vertebrae, 26)
			.AddRecipeGroupWithItem(OriginSystem.ShadowScaleRecipeGroupID, showItem: ModContent.ItemType<Undead_Chunk>(), 30)
			.AddRecipeGroupWithItem(OriginSystem.CursedFlameRecipeGroupID, showItem: ModContent.ItemType<Alkahest>(), 20)
			.AddTile(TileID.DemonAltar)
			.Register();
		}
	}
}
