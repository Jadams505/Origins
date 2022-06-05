using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Fiberglass {
	public class Fiberglass_Sword : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Fiberglass Sword");
			Tooltip.SetDefault("Be careful, it's sharp");
		}
		public override void SetDefaults() {
			Item.damage = 18;
			Item.melee = true;
			Item.width = 42;
			Item.height = 42;
			Item.useTime = 16;
			Item.useAnimation = 16;
			Item.useStyle = 1;
			Item.knockBack = 2;
			Item.value = 5000;
			Item.autoReuse = true;
            Item.useTurn = true;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item1;
		}
	}
}
