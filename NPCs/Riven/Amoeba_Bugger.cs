﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.GameContent.ItemDropRules;
using Origins.Items.Materials;
using Terraria.Audio;
using Origins.Tiles.Defiled;
using Terraria.GameContent.Bestiary;

namespace Origins.NPCs.Riven {
    public class Amoeba_Bugger : Glowing_Mod_NPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Amoeba Bugger");
            Main.npcFrameCount[NPC.type] = 2;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Bunny);
            NPC.aiStyle = NPCAIStyleID.Sharkron;
            NPC.lifeMax = 20;
            NPC.defense = 0;
            NPC.damage = 10;
            NPC.width = 28;
            NPC.height = 26;
            NPC.friendly = false;
            NPC.HitSound = Origins.Sounds.DefiledHurt;
            NPC.DeathSound = Origins.Sounds.DefiledKill;
            NPC.noGravity = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement(""),
            });
        }
        public override void AI() {
            if (Main.rand.NextBool(900)) SoundEngine.PlaySound(Origins.Sounds.DefiledIdle.WithPitchRange(1f, 1.2f), NPC.Center);
            NPC.FaceTarget();
            if(!NPC.HasValidTarget)NPC.direction = Math.Sign(NPC.velocity.X);
            NPC.spriteDirection = NPC.direction;
            if(++NPC.frameCounter>5) {
                NPC.frame = new Rectangle(0, (NPC.frame.Y+28)%56, 32, 26);
                NPC.frameCounter = 0;
            }
        }
        public override void HitEffect(int hitDirection, double damage) {
            if(NPC.life<0) {
                for(int i = 0; i < 3; i++) Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/R_Effect_Blood" + Main.rand.Next(1,4)));
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/R_Effect_Meat" + Main.rand.Next(2, 4)));
            } else {
                Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/R_Effect_Blood" + Main.rand.Next(1, 4)));
            }
        }
    }
}
