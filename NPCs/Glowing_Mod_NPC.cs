﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Origins.NPCs {
	public abstract class Glowing_Mod_NPC : ModNPC {
		public virtual string GlowTexturePath => Texture + "_Glow";
		//public virtual bool DrawOverTiles => false;
		private Asset<Texture2D> _glowTexture;
		public Texture2D GlowTexture => (_glowTexture ??= (ModContent.RequestIfExists<Texture2D>(GlowTexturePath, out var asset) ? asset : null))?.Value;
		public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			DrawGlow(spriteBatch, screenPos, GlowTexture, NPC);
		}
		public static void DrawGlow(SpriteBatch spriteBatch, Vector2 screenPos, Texture2D glowTexture, NPC npc) {
			if (glowTexture is not null) {
				Tile tile = Framing.GetTileSafely(npc.TopLeft.ToTileCoordinates());
				if (!tile.HasTile || !Main.tileBlockLight[tile.TileType]) goto success;

				tile = Framing.GetTileSafely(npc.TopRight.ToTileCoordinates());
				if (!tile.HasTile || !Main.tileBlockLight[tile.TileType]) goto success;

				tile = Framing.GetTileSafely(npc.BottomLeft.ToTileCoordinates());
				if (!tile.HasTile || !Main.tileBlockLight[tile.TileType]) goto success;

				tile = Framing.GetTileSafely(npc.BottomRight.ToTileCoordinates());
				if (!tile.HasTile || !Main.tileBlockLight[tile.TileType]) goto success;
				return;
				success:
				SpriteEffects spriteEffects = SpriteEffects.None;
				if (npc.spriteDirection == 1) {
					spriteEffects = SpriteEffects.FlipHorizontally;
				}
				Vector2 halfSize = new Vector2(glowTexture.Width / 2, glowTexture.Height / Main.npcFrameCount[npc.type] / 2);
				spriteBatch.Draw(
					glowTexture,
					new Vector2(npc.position.X - screenPos.X + (npc.width / 2) - glowTexture.Width * npc.scale / 2f + halfSize.X * npc.scale, npc.position.Y - screenPos.Y + npc.height - glowTexture.Height * npc.scale / Main.npcFrameCount[npc.type] + 4f + halfSize.Y * npc.scale + Main.NPCAddHeight(npc) + npc.gfxOffY),
					npc.frame,
					Color.White,
					npc.rotation,
					halfSize,
					npc.scale,
					spriteEffects,
				0);
			}
		}
	}
}
