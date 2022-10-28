﻿using Microsoft.Xna.Framework;
using Origins.Items.Materials;
using Origins.Tiles.Defiled;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.NPCs.Defiled {
    public class Defiled_Swarmer : ModNPC {
        public override void SetStaticDefaults() {
            DisplayName.SetDefault("{$Defiled} Swarmer");
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.DontDoHardmodeScaling[Type] = true;
        }
        public override void SetDefaults() {
            NPC.CloneDefaults(NPCID.Bunny);
            NPC.aiStyle = NPCAIStyleID.Demon_Eye;
            NPC.lifeMax = NPC.life = 20;
            NPC.defense = 8;
            NPC.damage = 10;
            NPC.width = 28;
            NPC.height = 26;
            NPC.friendly = false;
            NPC.catchItem = 0;
            NPC.HitSound = Origins.Sounds.DefiledHurt;
            NPC.DeathSound = Origins.Sounds.DefiledKill;
        }
        public override void UpdateLifeRegen(ref int damage) {
            if (NPC.life > 20) {
                NPC.lifeRegen += 75 / (NPC.life / 20);
            }
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                new FlavorTextBestiaryInfoElement("Not commonly found in the Wastelands. They appear when a massive {$Defiled} being is in distress."),
            });
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot) {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Undead_Chunk>(), 2, 2, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Defiled_Ore_Item>(), 2, 2, 4));
            npcLoot.Add(ItemDropRule.ByCondition(new Conditions.PlayerNeedsHealing(), ItemID.Heart, 2));
        }
        public override void AI() {
            NPCAimedTarget target = NPC.GetTargetData();
            NPC.rotation = NPC.AngleTo(target.Center) + MathHelper.PiOver2;
            if (Main.rand.NextBool(900)) SoundEngine.PlaySound(Origins.Sounds.DefiledIdle.WithPitchRange(1f, 1.2f), NPC.Center);
            NPC.FaceTarget();
            if(!NPC.HasValidTarget)NPC.direction = Math.Sign(NPC.velocity.X);
            NPC.spriteDirection = NPC.direction;
            if(++NPC.frameCounter>5) {
                NPC.frame = new Rectangle(0, (NPC.frame.Y+28)%84, 28, 26);
                NPC.frameCounter = 0;
            }
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit) {
            Rectangle spawnbox = projectile.Hitbox.MoveToWithin(NPC.Hitbox);
            Gore.NewGore(NPC.GetSource_Death(), Main.rand.NextVectorIn(spawnbox), projectile.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
        }
        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit) {
            int halfWidth = NPC.width / 2;
            int baseX = player.direction > 0 ? 0 : halfWidth;
            Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(baseX + Main.rand.Next(halfWidth),Main.rand.Next(NPC.height)), new Vector2(knockback*player.direction, -0.1f*knockback), Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Small"+Main.rand.Next(1,4)));
        }
        public override void HitEffect(int hitDirection, double damage) {
            if(NPC.life<0) {
                for(int i = 0; i < 3; i++)Gore.NewGore(NPC.GetSource_Death(), NPC.position+new Vector2(Main.rand.Next(NPC.width),Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/DF_Effect_Medium"+Main.rand.Next(1,4)));
            }
        }
    }
}
