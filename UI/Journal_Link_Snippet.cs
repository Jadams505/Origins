﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Chat;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Origins.UI {
    public class Journal_Link_Handler : ITagHandler {
		public class Journal_Link_Snippet : TextSnippet {
			string key;
			int lastHovered = 0;
			public Journal_Link_Snippet(string key, Color color = default) : base() {
				this.key = key;
				Text = Language.Exists("Mods.Origins.Journal.Name." + key) ? Language.GetTextValue("Mods.Origins.Journal.Name." + key) : Language.GetTextValue("Mods.Origins.ItemName." + key);
				CheckForHover = true;
				this.Color = color;
			}
			public override void Update() {
				base.Update();
				if(lastHovered > 0) lastHovered--;
			}
			public override void OnHover() {
				base.OnHover();
				lastHovered = 4;
				Main.LocalPlayer.mouseInterface = true;
			}
			public override void OnClick() {
				bool canRetry = true;
				retry:
				if (Main.InGameUI.CurrentState is Journal_UI_Open journalUI) {
					journalUI.Switch_Mode(Journal_UI_Mode.Normal_Page, key);
				} else if(canRetry) {
					IngameFancyUI.OpenUIState(new Journal_UI_Open());
					canRetry = false;
					goto retry;
				}
			}
			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1) {
				if (justCheckingString || lastHovered == 0 || lastHovered == 2 || spriteBatch is null || color != Color.Black) {
					size = default;
					return false;
				}
				size = default;
				const float lightness = 0.95f;
				ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, Text, position, new Color(1f * lightness, 0.8f * lightness, lastHovered * 0.15f * lightness, 1f), 0, Vector2.Zero, new Vector2(scale));
				//ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, Text, position, new Color(0f, 0f, 0f, color.A / 255f), 0, Vector2.Zero, new Vector2(scale));
				//size = ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, Text, position, color, 0, Vector2.Zero, new Vector2(scale));
				return true;
			}
		}
		public TextSnippet Parse(string text, Color baseColor = default(Color), string options = null) {
			return new Journal_Link_Snippet(text, baseColor);
		}
	}
}
