﻿using Terraria.ModLoader;

namespace Origins.Buffs {
    public class Solvent_Debuff : ModBuff {
		public override string Texture => "Terraria/Images/Buff_160";
		public static int ID { get; private set; } = -1;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Solvent");
            ID = Type;
        }
    }
}
