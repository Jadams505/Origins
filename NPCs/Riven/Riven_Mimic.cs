﻿using Microsoft.Xna.Framework;
using Origins.World.BiomeData;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Riven {
    public class Riven_Mimic : Glowing_Mod_NPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Riven Mimic");
            Main.npcFrameCount[NPC.type] = 14;
            SpawnModBiomes = new int[] {
                ModContent.GetInstance<Riven_Hive>().Type
            };
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.BigMimicCrimson);
        }
        public override void FindFrame(int frameHeight) {
            NPC.CloneFrame(NPCID.BigMimicCrimson, frameHeight);
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
