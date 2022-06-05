using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Tools {
	public class Acrid_Pickaxe : ModItem {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Acrid Pickaxe");
			Tooltip.SetDefault("");
		}
		public override void SetDefaults() {
            Item.CloneDefaults(ItemID.TitaniumPickaxe);
			Item.damage = 28;
			Item.melee = true;
            Item.pick = 195;
			Item.width = 34;
			Item.height = 32;
			Item.useTime = 7;
			Item.useAnimation = 22;
			Item.knockBack = 4f;
			Item.value = 3600;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
		}
        public override float UseTimeMultiplier(Player player) {
            return player.wet?1.5f:1;
        }
	}
}
