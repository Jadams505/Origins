﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Riven {
    public class Riven_Fighter : ModNPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Riven Acolyte");
            Main.npcFrameCount[NPC.type] = 5;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Zombie);
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.lifeMax = 81;
            NPC.defense = 10;
            NPC.damage = 33;
            NPC.width = 36;
            NPC.height = 40;
            NPC.friendly = false;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("The first creature born in the abominable Riven Hive. It is a very agile protector of its home."),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bud_Barnacle>(), 1, 1, 3));
        }
        public override void AI() {
            NPC.TargetClosest();
            if (NPC.HasPlayerTarget) {
                NPC.FaceTarget();
                NPC.spriteDirection = NPC.direction;
            }
            //increment frameCounter every frame and run the following code when it exceeds 7 (i.e. run the following code every 8 frames)
			if(NPC.collideY && ++NPC.frameCounter>7) {
				//add frame height (with buffer) to frame y position and modulo by frame height (with buffer) multiplied by walking frame count
				NPC.frame = new Rectangle(0, (NPC.frame.Y+40)%160, 36, 40);
                //reset frameCounter so this doesn't trigger every frame after the first time
				NPC.frameCounter = 0;
			}
        }
        public override void HitEffect(int hitDirection, double damage) {
            //spawn gore if npc is dead after being hit
            if(NPC.life<0) {
                for(int i = 0; i < 3; i++)Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF3_Gore"));
                for(int i = 0; i < 6; i++)Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Medium"+Main.rand.Next(1,4)));
            }
		    NPC.frame = new Rectangle(0, 160, 36, 40);
		    NPC.frameCounter = 0;
        }
    }
}
