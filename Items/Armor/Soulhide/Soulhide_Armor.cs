using Microsoft.Xna.Framework.Graphics;
using Origins.Dev;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Armor.Soulhide {
    [AutoloadEquip(EquipType.Head)]
	public class Soulhide_Helmet : ModItem, IWikiArmorSet, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 3;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			//player.weaponSize(DamageClass.Melee) += 0.1f; !!!
		}
		public override bool IsArmorSet(Item head, Item body, Item legs) {
			return body.type == ModContent.ItemType<Soulhide_Coat>() && legs.type == ModContent.ItemType<Soulhide_Guards>();
		}
		public override void UpdateArmorSet(Player player) {
			player.setBonus = "Nearby enemies are afflicted 'Shadowflame' and 'Weak'";
			player.GetModPlayer<OriginPlayer>().soulhideSet = true;
		}
        public override void ArmorSetShadows(Player player) {
			if (Main.rand.NextBool(6)) {
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, DustID.Shadowflame);
				dust.velocity *= 0.1f;
			}
		}
        public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.DemoniteOre, 8);
			recipe.AddIngredient(ItemID.RottenChunk, 16);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public string ArmorSetName => "Soulhide_Armor";
		public int HeadItemID => Type;
		public int BodyItemID => ModContent.ItemType<Soulhide_Coat>();
		public int LegsItemID => ModContent.ItemType<Soulhide_Guards>();
	}
	[AutoloadEquip(EquipType.Body)]
	public class Soulhide_Coat : ModItem, INoSeperateWikiPage {
		public override void SetStaticDefaults() {
			if (Main.netMode != NetmodeID.Server) {
				if (Mod.RequestAssetIfExists("Items/Armor/Soulhide/Soulhide_Coat_Cloth_Legs", out Asset<Texture2D> asset)) {
					Origins.TorsoLegLayers.Add(Item.bodySlot, asset);
				}
			}
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults() {
			Item.defense = 4;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.DemoniteOre, 20);
			recipe.AddIngredient(ItemID.RottenChunk, 28);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	[AutoloadEquip(EquipType.Legs)]
	public class Soulhide_Guards : ModItem, INoSeperateWikiPage {
		
		public override void SetDefaults() {
			Item.defense = 3;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
		}
		public override void UpdateEquip(Player player) {
			player.GetKnockback(DamageClass.Generic) += 0.14f;
		}
		public override void AddRecipes() {
			Recipe recipe = Recipe.Create(Type);
			recipe.AddIngredient(ItemID.DemoniteOre, 14);
			recipe.AddIngredient(ItemID.RottenChunk, 22);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
	public class Soulhide_Debuff : ModBuff {
		public override string Texture => "Terraria/Images/Buff_160";
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			ID = Type;
		}
		public override void Update(NPC npc, ref int buffIndex) {
			npc.AddBuff(BuffID.ShadowFlame, 180);
			npc.AddBuff(BuffID.Weak, 60);
		}
	}
}
