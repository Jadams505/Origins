﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Journal;
using Origins.Questing;
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
	public class Quest_Link_Handler : ITagHandler {
		public class Quest_Link_Snippet : TextSnippet {
			string key;
			int lastHovered = 0;
			bool completed;
			bool inJournal;
			public Quest_Link_Snippet(string key, Color color = default, bool completed = false, bool inJournal = false) : this(Quest_Registry.GetQuestByKey(key), color, completed, inJournal) { }
			public Quest_Link_Snippet(Quest quest, Color color = default, bool completed = false, bool inJournal = false) : base() {
				key = quest.FullName;
				Text = quest.NameValue;
				CheckForHover = true;
				Color = color;
				this.completed = completed;
				this.inJournal = inJournal;
			}
			public override void Update() {
				base.Update();
				if (lastHovered > 0) lastHovered--;
			}
			public override void OnHover() {
				base.OnHover();
				lastHovered = 4;
				Main.LocalPlayer.mouseInterface = true;
			}
			public override void OnClick() {
				Origins.OpenJournalQuest(key);
			}
			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1) {
				if (inJournal) {
					if (completed) {
						StringBuilder builder = new StringBuilder();
						Vector2 dimensions = FontAssets.MouseText.Value.MeasureString(Text);
						var strikethroughFont = OriginExtensions.StrikethroughFont;
						size = dimensions;
						if (justCheckingString) return false;
						const char strike = '–';
						float strikeWidth = strikethroughFont.MeasureString(strike.ToString()).X - 2;
						for (int i = (int)Math.Ceiling(dimensions.X / strikeWidth); i-- > 0;) {
							builder.Append(strike);
						}
						string strikethroughText = builder.ToString();
						color *= 0.666f;
						if (lastHovered > 0) {
							const float lightness = 0.95f;
							for (int i = 0; i < ChatManager.ShadowDirections.Length; i++) {
								Color shadowColor = i == 1 || i == 3 ? color : new Color(1f * lightness, 0.8f * lightness, lastHovered * 0.15f * lightness, 1f);
								ChatManager.DrawColorCodedString(
									spriteBatch,
									FontAssets.MouseText.Value,
									Text,
									position + ChatManager.ShadowDirections[i],
									shadowColor,
									0,
									Vector2.Zero,
									new Vector2(scale)
								);
								ChatManager.DrawColorCodedString(
									spriteBatch,
									strikethroughFont,
									strikethroughText,
									position + dimensions * new Vector2(0.5f, 0.025f) + ChatManager.ShadowDirections[i],
									new Color(shadowColor.R, shadowColor.G, shadowColor.B, 255),
									0,
									new Vector2(strikeWidth * builder.Length * 0.5f, 0),
									new Vector2(scale)
								);
							}
						}
						ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, Text, position, color, 0, Vector2.Zero, new Vector2(scale));
						ChatManager.DrawColorCodedString(spriteBatch, strikethroughFont, strikethroughText, position + dimensions * new Vector2(0.5f, 0.025f), new Color(color.R, color.G, color.B, 255), 0, new Vector2(strikeWidth * builder.Length * 0.5f, 0), new Vector2(scale));
						//ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, builder.ToString(), position + dimensions * new Vector2(0.5f, 0.025f), new Color(color.R, color.G, color.B, 255), 0, new Vector2(strikeWidth * builder.Length * 0.5f, 0), new Vector2(scale));
						return true;
					} else {
						size = FontAssets.MouseText.Value.MeasureString(Text);
						ChatManager.DrawColorCodedString(spriteBatch, FontAssets.MouseText.Value, Text, position, color, 0, Vector2.Zero, new Vector2(scale));
						return true;
					}
				} else {
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
		}
		public TextSnippet Parse(string text, Color baseColor = default(Color), string optionString = null) {
			string[] options = (optionString ?? "").Split(',');
			bool completed = false;
			bool inJournal = false;
			for (int i = 0; i < options.Length; i++) {
				switch (options[i]) {
					case "completed":
					completed = true;
					break;

					case "inJournal":
					inJournal = true;
					break;
				}
			}
			return new Quest_Link_Snippet(text, baseColor, completed, inJournal);
		}
	}
}
