﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Origins.NPCs.Defiled {
    public class Defiled_Mite : ModNPC {
        internal const int spawnCheckDistance = 15;
        public const int aggroRange = 128;
        byte frame = 0;
        byte anger = 0;
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("Defiled Mite");
            Main.npcFrameCount[NPC.type] = 4;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Bunny);
            NPC.aiStyle = NPCAIStyleID.None;
            NPC.lifeMax = 22;
            NPC.defense = 6;
            NPC.damage = 34;
            NPC.width = 34;
            NPC.height = 26;
            NPC.friendly = false;
        }
        public override bool PreAI() {
            NPC.TargetClosest();
            NPC.aiStyle = NPC.HasPlayerTarget ? NPCAIStyleID.Fighter : NPCAIStyleID.None;
            if(((NPC.Center-NPC.targetRect.Center.ToVector2())*new Vector2(1,2)).Length()>aggroRange) {
                if(NPC.life<NPC.lifeMax) {
                    NPC.aiStyle = NPCAIStyleID.Tortoise;
                } else {
                    NPC.target = -1;
                    NPC.aiStyle = NPCAIStyleID.None;
                }
            }
            if(NPC.HasPlayerTarget) {
                NPC.FaceTarget();
                NPC.spriteDirection = NPC.direction;
            }
            if(NPC.collideY) {
                NPC.rotation = 0;
                if(anger!=0) {
                    if(anger>1)anger--;
                    NPC.aiStyle = NPCAIStyleID.Tortoise;
                }else if(NPC.aiStyle==NPCAIStyleID.None) {
                    NPC.velocity.X*=0.85f;
                }
                if(Math.Sign(NPC.velocity.X) == NPC.direction){
                    frame = (byte)((frame+1)&15);
                } else if(Math.Sign(NPC.velocity.X) == -NPC.direction){
                    frame = (byte)((frame-1)&15);
                }
            }else {
                if(anger == 1) anger = 0;
            }
            return NPC.aiStyle!=NPCAIStyleID.None;
        }
        public override void FindFrame(int frameHeight) {
            NPC.frame = new Rectangle(0, 28*(frame&12)/4, 32, 26);
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit) {
            anger = 6;
            return true;
        }
        public override int SpawnNPC(int tileX, int tileY) {
            Tile tile;
            for(int i = 0; i < spawnCheckDistance; i++) {
                tile = Main.tile[tileX, ++tileY];
                if(tile.HasTile) {
                    tileY--;
                    break;
                }
            }
            return base.SpawnNPC(tileX, tileY);
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit) {
            Rectangle spawnbox = projectile.Hitbox.MoveToWithin(NPC.Hitbox);
            for(int i = Main.rand.Next(3); i-->0;)Gore.NewGore(Main.rand.NextVectorIn(spawnbox), projectile.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit) {
            int halfWidth = NPC.width / 2;
            int baseX = player.direction > 0 ? 0 : halfWidth;
            for(int i = Main.rand.Next(3); i-->0;)Gore.NewGore(NPC.position+new Vector2(baseX + Main.rand.Next(halfWidth),Main.rand.Next(NPC.height)), new Vector2(knockback*player.direction, -0.1f*knockback), Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
        }
        public override void HitEffect(int hitDirection, double damage) {
            if(NPC.life<0) {
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF1_Gore"));
                Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Medium"+Main.rand.Next(1,4)));
                for(int i = 0; i < 3; i++)Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
            }
        }
    }
}
