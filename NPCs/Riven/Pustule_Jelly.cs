﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Origins.NPCs.Riven {
    public class Pustule_Jelly : ModNPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Pustule Jelly");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.BloodJelly);
            NPC.lifeMax = 380;
            NPC.defense = 20;
            NPC.damage = 70;
            NPC.width = 32;
            NPC.height = 42;
            NPC.frame.Height = 40;
        }
        public override void FindFrame(int frameHeight) {
		    NPC.spriteDirection = NPC.direction;
		    NPC.frameCounter += 1.0;
		    if (NPC.frameCounter >= 24.0){
			    NPC.frameCounter = 0.0;
		    }
		    NPC.frame.Y = 42 * (int)(NPC.frameCounter / 6.0);
        }
    }
}
