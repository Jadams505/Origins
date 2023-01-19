using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Origins.Items.Materials;

namespace Origins.Items.Tools {
	public class Pullinsmash : ModItem {
		static short glowmask;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Stabsmash");
			Tooltip.SetDefault("'Hammer and nail in one package'");
			glowmask = Origins.AddGlowMask(this);
			SacrificeTotal = 1;
		}
		public override void SetDefaults() {
            Item.CloneDefaults(ItemID.TitaniumWaraxe);
			Item.damage = 16;
			Item.DamageType = DamageClass.Melee;
            Item.hammer = 55;
			Item.width = 46;
			Item.height = 38;
			Item.useTime = 17;
			Item.useAnimation = 27;
			Item.knockBack = 4f;
			Item.value = Item.sellPrice(silver: 30);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item1;
			Item.glowMask = glowmask;
		}
		public override void AddRecipes() {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<Encrusted_Bar>(), 10);
			recipe.AddIngredient(ModContent.ItemType<Riven_Sample>(), 5);
			recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
	}
}
