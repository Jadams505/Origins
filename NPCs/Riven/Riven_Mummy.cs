﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Riven {
    public class Riven_Mummy : Glowing_Mod_NPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Amebic Mummy");
            Main.npcFrameCount[NPC.type] = 16;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.lifeMax = 110;
            NPC.defense = 8;
            NPC.damage = 42;
            NPC.width = 40;
            NPC.height = 54;
            NPC.friendly = false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("It was only a matter of time before the Riven got to the body. It now wanders aimlessly in the Rivenated deserts in search of new hosts."),
            });
        }
        public override void AI() {
            NPC.TargetClosest();
            if (NPC.HasPlayerTarget) {
                NPC.FaceTarget();
                NPC.spriteDirection = NPC.direction;
            }
            //increment frameCounter every frame and run the following code when it exceeds 7 (i.e. run the following code every 8 frames)
			if(++NPC.frameCounter>7) {
				//add frame height (with buffer) to frame y position and modulo by frame height (with buffer) multiplied by walking frame count
				NPC.frame = new Rectangle(0, (NPC.frame.Y + 56) % 896, 36, 56);
                //reset frameCounter so this doesn't trigger every frame after the first time
				NPC.frameCounter = 0;
			}
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            npcLoot.Add(ItemDropRule.Common(ItemID.DarkShard, 10));
            npcLoot.Add(ItemDropRule.StatusImmunityItem(ItemID.Megaphone, 100));
            npcLoot.Add(ItemDropRule.StatusImmunityItem(ItemID.Blindfold, 100));
            npcLoot.Add(ItemDropRule.Common(ItemID.MummyMask, 75));
            npcLoot.Add(ItemDropRule.Common(ItemID.MummyShirt, 75));
            npcLoot.Add(ItemDropRule.Common(ItemID.MummyPants, 75));
        }
        public override void HitEffect(int hitDirection, double damage) {
            //spawn gore if npc is dead after being hit
            if(NPC.life<0) {
                for(int i = 0; i < 3; i++)Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF3_Gore"));
                for(int i = 0; i < 6; i++)Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Medium"+Main.rand.Next(1,4)));
            }
        }
    }
}
