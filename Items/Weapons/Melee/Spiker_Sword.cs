using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Melee {
    public class Spiker_Sword : ModItem {
		static short glowmask;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Spiker Sword");
			Tooltip.SetDefault("Very pointy");
			glowmask = Origins.AddGlowMask(this);
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
			Item.damage = 30;
			Item.DamageType = DamageClass.Melee;
			Item.width = 42;
			Item.height = 50;
			Item.useTime = 28;
			Item.useAnimation = 28;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 7.5f;
            Item.useTurn = true;
			Item.value = Item.sellPrice(silver: 40);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.glowMask = glowmask;
		}
		public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Defiled_Bar>(), 6);
            recipe.AddIngredient(ModContent.ItemType<Undead_Chunk>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

			recipe = Recipe.Create(ItemID.NightsEdge);
			recipe.AddIngredient(this);
			recipe.AddIngredient(ItemID.Muramasa);
			recipe.AddIngredient(ItemID.BladeofGrass);
			recipe.AddIngredient(ItemID.FieryGreatsword);
			recipe.AddTile(TileID.DemonAltar);
			recipe.Register();
		}
	}
}