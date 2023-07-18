﻿using Origins.NPCs;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.UI.Chat;
using Terraria.GameContent;
using Microsoft.Xna.Framework;

namespace Origins.Buffs {
	public class Torn_Debuff : ModBuff {
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Torn");
			// Description.SetDefault("Your max life has been reduced!");
			Main.debuff[Type] = true;
			ID = Type;
		}
		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<OriginPlayer>().tornDebuff = true;
		}
		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<OriginGlobalNPC>().tornDebuff = true;
		}
		public override void PostDraw(SpriteBatch spriteBatch, int buffIndex, BuffDrawParams drawParams) {
			OriginPlayer originPlayer = Main.LocalPlayer.GetModPlayer<OriginPlayer>();
			float target = originPlayer.tornTarget;
			float targetTime = originPlayer.tornTargetTime;
			float time = originPlayer.tornTime;
			//if (target == 0.7f) return;

			string text = $"{(1 - target) * (time / (float)targetTime):P0}";
			ChatManager.DrawColorCodedStringWithShadow(
				spriteBatch,
				FontAssets.CombatText[0].Value,
				text,
				drawParams.MouseRectangle.Top(),
				new Color(50, 180, 230),
				0,
				FontAssets.CombatText[0].Value.MeasureString(text) * new Vector2(0.5f, 0.75f),
				new Vector2(0.5f)
			);
		}
	}
}
