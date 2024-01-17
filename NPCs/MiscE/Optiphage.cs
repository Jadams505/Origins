﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Items.Weapons.Summoner;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.MiscE {
    public class Optiphage : ModNPC {
		public static new AutoCastingAsset<Texture2D> HeadTexture { get; private set; }
		public override void SetStaticDefaults() {
			// DisplayName.SetDefault("Optiphage");
			Main.npcFrameCount[Type] = 2;
			if (!Main.dedServ) {
				HeadTexture = Mod.Assets.Request<Texture2D>("NPCs/MiscE/Optiphage_Head");
			}
			CorruptGlobalNPC.NPCTypes.Add(Type);
		}
		public override void Unload() {
			HeadTexture = null;
		}
		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.DemonEye);
			NPC.aiStyle = 85;
			NPC.lifeMax = 14;
			NPC.defense = 2;
			NPC.damage = 14;
			NPC.width = 16;
			NPC.height = 16;
			NPC.friendly = false;
			NPC.noGravity = true;
			NPC.aiStyle = NPCAIStyleID.Demon_Eye;
			NPC.value = 34;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				this.GetBestiaryFlavorText("This Eater is in its first stage of the Eater life cycle. It goes straight for the eyes of its victims in hopes that it can prompt its own eye to grow in."),
			});
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rotting_Worm_Staff>(), 5));
		}
		public override void AI() {
			if (NPC.aiStyle == NPCAIStyleID.Star_Cell) {
				NPC.hide = true;
				if (NPC.ai[0] == 0f) {
					NPC.aiStyle = NPCAIStyleID.Demon_Eye;
				}else if (NPC.ai[0] == 1f) {
					Player targetPlayer = Main.player[NPC.target];
					NPC.BottomRight = targetPlayer.Center;
					targetPlayer.AddBuff(Buffs.Optiphage_Debuff.ID, 5);
				}
			} else {
				NPCAimedTarget target = NPC.GetTargetData();
				NPC.rotation = NPC.AngleTo(target.Center) + MathHelper.PiOver2;
				NPC.FaceTarget();
				if (!NPC.HasValidTarget) NPC.direction = Math.Sign(NPC.velocity.X);
				NPC.spriteDirection = NPC.direction;
				NPC.hide = false;
				if (++NPC.frameCounter > 5) {
					NPC.frame = new Rectangle(0, (NPC.frame.Y + 30) % 60, 16, 30);
					NPC.frameCounter = 0;
				}
			}
		}
		public override bool CanHitNPC(NPC target)/* tModPorter Suggestion: Return true instead of null */ {
			return NPC.aiStyle != NPCAIStyleID.Star_Cell;
		}
		public override bool CanHitPlayer(Player target, ref int cooldownSlot) {
			return NPC.aiStyle != NPCAIStyleID.Star_Cell;
		}
		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo) {
			NPC.aiStyle = NPCAIStyleID.Star_Cell;
			NPC.ai[0] = 1;
		}
		public override void DrawBehind(int index) {
			if (NPC.aiStyle == NPCAIStyleID.Star_Cell) Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
			if (NPC.aiStyle == NPCAIStyleID.Star_Cell) {
				Player targetPlayer = Main.player[NPC.target];
				Main.EntitySpriteDraw(
					HeadTexture,
					(targetPlayer.Top) - screenPos,
					null,
					drawColor,
					targetPlayer.headRotation,
					new Vector2(11 - targetPlayer.direction * 4, 2),
					NPC.scale,
					targetPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				0);
				return false;
			}
			return true;
		}
	}
}
