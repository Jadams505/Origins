﻿using Origins.Items.Materials;
using Origins.Items.Weapons.Other;
using Origins.Tiles;
using Origins.Tiles.Defiled;
using Origins.Tiles.Riven;
using Origins.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.NPCs {
    public partial class OriginGlobalNPC : GlobalNPC {
        bool downedSkeletron = false;
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			List<IItemDropRule> dropRules = npcLoot.Get(false);
			IItemDropRule entry;
			int len = dropRules.Count;
			var def = new LootConditions.IsWorldEvil(OriginSystem.evil_wastelands);
			var riv = new LootConditions.IsWorldEvil(OriginSystem.evil_riven);
			var defExp = new LootConditions.IsWorldEvilAndNotExpert(OriginSystem.evil_wastelands);
			var rivExp = new LootConditions.IsWorldEvilAndNotExpert(OriginSystem.evil_riven);
			for (int i = 0; i < len; i++) {
				entry = dropRules[i];
				if (entry is ItemDropWithConditionRule rule) {
					if (rule.condition is Conditions.IsCorruption) {
						rule.condition = new LootConditions.IsWorldEvil(OriginSystem.evil_corruption);
					} else if (rule.condition is Conditions.IsCrimson) {
						rule.condition = new LootConditions.IsWorldEvil(OriginSystem.evil_crimson);
					} else if (rule.condition is Conditions.IsCorruptionAndNotExpert) {
						rule.condition = new LootConditions.IsWorldEvilAndNotExpert(OriginSystem.evil_corruption);
						switch (rule.itemId) {
							case ItemID.DemoniteOre:
							npcLoot.Add(ItemDropRule.ByCondition(
								defExp,
								ModContent.ItemType<Defiled_Ore_Item>(),
								rule.chanceDenominator,
								rule.amountDroppedMinimum,
								rule.amountDroppedMaximum,
								rule.chanceNumerator
							));
							npcLoot.Add(ItemDropRule.ByCondition(
								rivExp,
								ModContent.ItemType<Infested_Ore_Item>(),
								rule.chanceDenominator,
								rule.amountDroppedMinimum,
								rule.amountDroppedMaximum,
								rule.chanceNumerator
							));
							break;
							case ItemID.CorruptSeeds:
							npcLoot.Add(ItemDropRule.ByCondition(
								defExp,
								ModContent.ItemType<Defiled_Grass_Seeds>(),
								rule.chanceDenominator,
								rule.amountDroppedMinimum,
								rule.amountDroppedMaximum,
								rule.chanceNumerator
							));
							break;
						}
					} else if (rule.condition is Conditions.IsCrimsonAndNotExpert) {
						rule.condition = new LootConditions.IsWorldEvilAndNotExpert(OriginSystem.evil_crimson);
					}
				}
			}
            switch (npc.type) {
                case NPCID.CaveBat:
                case NPCID.GiantBat:
                case NPCID.IceBat:
                case NPCID.IlluminantBat:
                case NPCID.JungleBat:
                case NPCID.VampireBat:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bat_Hide>(), 1, 1, 3));
                break;
                case NPCID.ArmoredSkeleton:
                case NPCID.SkeletonArcher:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Tiny_Sniper>(), 50));
                break;
                case NPCID.MossHornet:
                case NPCID.BigMossHornet:
                case NPCID.GiantMossHornet:
                case NPCID.LittleMossHornet:
                case NPCID.TinyMossHornet:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Peat_Moss>(), 1, 1, 4));
                break;
                default:
                break;
            }
        }
		public override void OnKill(NPC npc) {
            switch(npc.type) {
                case NPCID.SkeletronHead:
                if(!NPC.downedBoss3)GenFelnumOre();
                break;
                default:
                break;
            }
        }
		public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Defiled_Key>(), 2500, 1, 1, new Conditions.DesertKeyCondition()));
		}
		void GenFelnumOre() {
            string text = "The clouds have been blessed with Felnum.";
			if (Main.netMode == NetmodeID.SinglePlayer) {
                Main.NewText(text, Colors.RarityPurple);
			}else if (Main.netMode == NetmodeID.Server) {
                ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Colors.RarityPurple);
			}
            if(!Main.gameMenu && Main.netMode != NetmodeID.MultiplayerClient) {
                int x = 0, y = 0;
                int felnumOre = ModContent.TileType<Felnum_Ore>();
                int type;
                Tile tile;
                int fails = 0;
                int success = 0;
                for (int k = 0; k < (int)((Main.maxTilesX * Main.maxTilesY) * (Main.expertMode?6E-06:4E-06)); k++) {
                    int tries = 0;
                    type = TileID.BlueDungeonBrick;
                    while(type!=TileID.Cloud&&type!=TileID.Dirt&&type!=TileID.Grass&&type!=TileID.Stone&&type!=TileID.RainCloud) {
				        x = WorldGen.genRand.Next(0, Main.maxTilesX);
						y = WorldGen.genRand.Next(90, (int)OriginSystem.worldSurfaceLow - 5);
                        tile = Framing.GetTileSafely(x, y);
                        type = tile.HasTile?tile.TileType:TileID.BlueDungeonBrick;
                        if(++tries >= 150) {
                            if(++fails%2==0)k--;
                            success--;
                            type = TileID.Dirt;
                        }
                    }
                    success++;
				    GenRunners.FelnumRunner(x, y, WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 6), felnumOre);
			    }
                //Main.NewText($"generation complete, ran {runCount} times with {fails} fails");
            }
        }
    }
}
