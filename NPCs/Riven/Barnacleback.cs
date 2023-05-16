﻿using Microsoft.Xna.Framework;
using Origins.Items.Armor.Encrusted;
using Origins.Items.Materials;
using Origins.World.BiomeData;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Origins.Items.Armor.Encrusted.Encrusted2_Mask;

namespace Origins.NPCs.Riven {
	public class Barnacleback : ModNPC {
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Barnacle Back");
			Main.npcFrameCount[NPC.type] = 3;
			SpawnModBiomes = new int[] {
				ModContent.GetInstance<Riven_Hive>().Type
			};
		}
		public override void SetDefaults() {
			NPC.CloneDefaults(NPCID.Drippler);
			NPC.lifeMax = 40;
			NPC.defense = 0;
			NPC.damage = 14;
			NPC.width = 36;
			NPC.height = 50;
			NPC.friendly = false;
			NPC.HitSound = SoundID.NPCHit13;
			NPC.DeathSound = SoundID.NPCDeath15;
			NPC.knockBackResist = 0.75f;
			NPC.value = 76;
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) {
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				new FlavorTextBestiaryInfoElement("Barnaclebacks are a keystone species to the Riven Hive, working hard to maintain it. Their presence is a sure sign that any progress cleansing the Hive is futile."),
			});
		}
		public override void ModifyNPCLoot(NPCLoot npcLoot) {
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bud_Barnacle>(), 1, 2, 5));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Encrusted2_Mask>(), 525));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Encrusted2_Coat>(), 525));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Encrusted2_Pants>(), 525));
		}
		public override void AI() {
			const float maxDistTiles = 45f * 16;
			const float maxDistTiles2 = 60f * 16;
			for (int i = 0; i < Main.maxNPCs; i++) {
				if (i == NPC.whoAmI) continue;
				NPC currentTarget = Main.npc[i];
				if (currentTarget.CanBeChasedBy()) {
					float distSquared = (currentTarget.Center - NPC.Center).LengthSquared();
					if (distSquared < maxDistTiles * maxDistTiles) {
						currentTarget.AddBuff(Barnacled_Buff.ID, 5);
					}
					if (distSquared < maxDistTiles2 * maxDistTiles2) {
						currentTarget.AddBuff(Barnacled_Buff.ID, 5);
						if (Main.rand.NextBool(6)) {
							Vector2 pos = Main.rand.NextVector2FromRectangle(currentTarget.Hitbox);
							Vector2 dir = (NPC.Center - pos).SafeNormalize(default) * 4;
							Dust dust = Dust.NewDustPerfect(
								pos,
								DustID.Clentaminator_Cyan,
								dir,
								120,
								Color.LightCyan,
								1f
							);
							dust.noGravity = true;
							if (Main.rand.NextBool(2)) {
								dust.noGravity = false;
								dust.scale *= 0.7f;
							}
						}
					}
				}
			}
			if (++NPC.frameCounter > 7) {
				NPC.frame = new Rectangle(0, (NPC.frame.Y + 50) % 150, 36, 50);
				NPC.frameCounter = 0;
			}
		}
		public override void HitEffect(int hitDirection, double damage) {
			if (NPC.life < 0) {
				for (int i = 0; i < 3; i++) Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/R_Effect_Blood" + Main.rand.Next(1, 4)));
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), NPC.velocity, Mod.GetGoreSlot("Gores/NPCs/R_Effect_Meat" + Main.rand.Next(1, 4)));
			}
			NPC.frameCounter = 0;
		}
	}
	public class Barnacled_Buff : ModBuff {
		public override string Texture => "Terraria/Images/Buff_32";
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			DisplayName.SetDefault("Barnacled");
			Description.SetDefault("You shouldn't have this buff, but if you do, something is horribly wrong");
			ID = Type;
		}
		public override void Update(NPC npc, ref int buffIndex) {
			npc.GetGlobalNPC<OriginGlobalNPC>().barnacleBuff = true;
		}
	}
}
