﻿using System;
using Terraria;
using Terraria.ModLoader;

namespace Origins.Buffs {
	public class Purifying_Buff : ModBuff {
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			ID = Type;
		}
		public override void Update(Player player, ref int buffIndex) {
			OriginPlayer originPlayer = player.GetModPlayer<OriginPlayer>();
			originPlayer.CorruptionAssimilation -= Math.Min(0.01f, originPlayer.CorruptionAssimilation);
			originPlayer.CrimsonAssimilation    -= Math.Min(0.01f, originPlayer.CrimsonAssimilation);
			originPlayer.DefiledAssimilation    -= Math.Min(0.01f, originPlayer.DefiledAssimilation);
			originPlayer.RivenAssimilation      -= Math.Min(0.01f, originPlayer.RivenAssimilation);
			originPlayer.corruptionAssimilationDebuffMult = 0;
			originPlayer.crimsonAssimilationDebuffMult    = 0;
			originPlayer.defiledAssimilationDebuffMult    = 0;
			originPlayer.rivenAssimilationDebuffMult      = 0;
			originPlayer.tornSeverityDecayRate *= 5;
		}
	}
}
