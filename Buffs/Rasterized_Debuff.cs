﻿using Terraria.ModLoader;

namespace Origins.Buffs {
	public class Rasterized_Debuff : ModBuff {
		public const int duration = 24;
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			ID = Type;
		}
	}
}
