﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Items.Weapons.Ranged {
    public class Marrowick_Bow : ModItem {
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.ShadewoodBow);
			Item.damage = 9;
			Item.width = 26;
			Item.height = 62;
			Item.value = Item.sellPrice(copper: 30);
		}
		public override Vector2? HoldoutOffset() {
			return new Vector2(-4f, 0);
		}
	}
}
