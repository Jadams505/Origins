﻿using Origins.Buffs;
using Origins.Items.Accessories;
using Origins.Items.Materials;
using Origins.Items.Other.Consumables.Food;
using Origins.Items.Pets;
using Origins.Items.Weapons;
using Origins.Items.Weapons.Ammo;
using Origins.Items.Weapons.Demolitionist;
using Origins.Items.Weapons.Magic;
using Origins.Items.Weapons.Ranged;
using Origins.Items.Weapons.Summoner;
using Origins.Tiles.Brine;
using Origins.Tiles.Other;
using Origins.World;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.NPCs {
	public partial class OriginGlobalNPC : GlobalNPC {
		internal static int woFEmblemsCount = 4;
		static OneFromOptionsDropRule _eaterOfWorldsWeaponDrops;
		public static OneFromOptionsDropRule EaterOfWorldsWeaponDrops => _eaterOfWorldsWeaponDrops ??=  new(1, 1, ModContent.ItemType<Rotting_Worm_Staff>(), ModContent.ItemType<Eaterboros>());
		static OneFromOptionsDropRule _brainOfCthulhuWeaponDrops;
		public static OneFromOptionsDropRule BrainOfCthulhuWeaponDrops => _brainOfCthulhuWeaponDrops ??=  new(1, 1, ModContent.ItemType<Hemoptysis>());
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			static LocalizedText GetWarningText(string key) => Language.GetText("Mods.Origins.Warnings." + key);
			List<IItemDropRule> dropRules = npcLoot.Get(false);
			switch (npc.netID) {
				case NPCID.BrainofCthulhu:
				npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Weakpoint_Analyzer>(), 4));
				npcLoot.Add(new LeadingConditionRule(new Conditions.NotExpert()).WithOnSuccess(BrainOfCthulhuWeaponDrops));
				break;
				case NPCID.EaterofWorldsHead or NPCID.EaterofWorldsBody or NPCID.EaterofWorldsTail:
				npcLoot.Add(new LeadingConditionRule(new Conditions.LegacyHack_IsABoss())).WithOnSuccess(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Forbidden_Voice>(), 4));
				npcLoot.Add(new LeadingConditionRule(new Conditions.LegacyHack_IsBossAndNotExpert()).WithOnSuccess(EaterOfWorldsWeaponDrops));
				break;
				case NPCID.KingSlime:
				npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Cursed_Crown>(), 4));
				break;
				case NPCID.EyeofCthulhu:
				npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Strange_Tooth>(), 4));
				break;
				case NPCID.SkeletronHead:
				npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Terrarian_Voodoo_Doll>(), 4));
				break;
				case NPCID.QueenBee: {
					npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Emergency_Bee_Canister>(), 4));
					bool foundWeapon = false;
					OneFromOptionsNotScaledWithLuckDropRule rule = dropRules.FindDropRule<OneFromOptionsNotScaledWithLuckDropRule>(r => r.dropIds.Contains(ItemID.BeeGun));
					if (rule is not null) {
						Array.Resize(ref rule.dropIds, rule.dropIds.Length + 1);
						rule.dropIds[^1] = ModContent.ItemType<Bee_Afraid_Incantation>();
						foundWeapon = true;
					}
					if (!foundWeapon) Origins.LogLoadingWarning(GetWarningText("MissingDropRule").WithFormatArgs(GetWarningText("DropRuleType.Weapon"), Lang.GetNPCName(npc.netID)));
					break;
				}
				case NPCID.Deerclops:
				npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Blizzardwalkers_Jacket>(), 4));
				break;
				case NPCID.SkeletronPrime:
				case NPCID.TheDestroyer:
				case NPCID.TheDestroyerBody:
				case NPCID.TheDestroyerTail:
				case NPCID.Retinazer:
				case NPCID.Spazmatism:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Busted_Servo>(), 1, 8, 37));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Power_Core>(), 1, 1, 3));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rotor>(), 1, 5, 22));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Strange_Power_Up>(), 50));
				break;
				case NPCID.MoonLordCore:
				npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Third_Eye>(), 4));
				break;
				case NPCID.WallofFlesh: {
					npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<Scribe_of_the_Meat_God>(), 4));
					bool foundEmblem = false;
					bool foundWeapon = false;
					IEnumerable<IItemDropRule> rules = dropRules.Where((r) =>
					r is LeadingConditionRule conditionRule &&
					conditionRule.ChainedRules.Count != 0 &&
					conditionRule.ChainedRules[0].RuleToChain is OneFromOptionsNotScaledWithLuckDropRule dropRule &&
					dropRule.dropIds.Contains(ItemID.WarriorEmblem));
					if (rules.Any()) {
						OneFromOptionsNotScaledWithLuckDropRule rule = rules.First().ChainedRules[0].RuleToChain as OneFromOptionsNotScaledWithLuckDropRule;
						if (rule is not null) {
							Array.Resize(ref rule.dropIds, rule.dropIds.Length + 1);
							rule.dropIds[^1] = ModContent.ItemType<Exploder_Emblem>();
							woFEmblemsCount = rule.dropIds.Length;
							foundEmblem = true;
						}
					}
					rules = dropRules.Where((r) =>
					r is LeadingConditionRule conditionRule &&
					conditionRule.ChainedRules.Count != 0 &&
					conditionRule.ChainedRules[0].RuleToChain is OneFromOptionsNotScaledWithLuckDropRule dropRule &&
					dropRule.dropIds.Contains(ItemID.BreakerBlade));
					if (rules.Any()) {
						OneFromOptionsNotScaledWithLuckDropRule rule = rules.First().ChainedRules[0].RuleToChain as OneFromOptionsNotScaledWithLuckDropRule;
						if (rule is not null) {
							Array.Resize(ref rule.dropIds, rule.dropIds.Length + 1);
							rule.dropIds[^1] = ModContent.ItemType<Thermite_Launcher>();
							foundWeapon = true;
						}
					}
					if (!foundEmblem) Origins.LogLoadingWarning(GetWarningText("MissingDropRule").WithFormatArgs(GetWarningText("DropRuleType.Emblem"), Lang.GetNPCName(npc.netID)));
					if (!foundWeapon) Origins.LogLoadingWarning(GetWarningText("MissingDropRule").WithFormatArgs(GetWarningText("DropRuleType.Weapon"), Lang.GetNPCName(npc.netID)));
					break;
				}

				case NPCID.CaveBat:
				case NPCID.GiantBat:
				case NPCID.IceBat:
				case NPCID.IlluminantBat:
				case NPCID.JungleBat:
				case NPCID.VampireBat:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bat_Hide>(), 1, 2, 4));
				break;
				case NPCID.SkeletonSniper: //Tiny skeleton sniper
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Tiny_Sniper>(), 24));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10));
				break;
				case NPCID.Snatcher:
				case NPCID.JungleSlime:
				case NPCID.SpikedJungleSlime:
				case NPCID.MossHornet:
				case NPCID.BigMossHornet:
				case NPCID.GiantMossHornet:
				case NPCID.LittleMossHornet:
				case NPCID.TinyMossHornet:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Peat_Moss_Item>(), 1, 3, 7));
				break;
				case NPCID.AngryBones:
				case NPCID.AngryBonesBig:
				case NPCID.AngryBonesBigMuscle:
				case NPCID.AngryBonesBigHelmet:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bolt_Gun>(), 50));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Longbone>(), 50));
				break;
				case NPCID.Zombie:
				case NPCID.Harpy:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Potato>(), 13));
				break;
				case NPCID.Nymph:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Potato>()));
				break;
				case NPCID.Wolf:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Vanilla_Shake>(), 21));
				break;
				case NPCID.WyvernHead:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Startillery>(), 12));
				break;
				case NPCID.Clown:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Happy_Bomb>(), 1, 69, 69));
				break;
				case NPCID.PurpleSlime:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Plasma_Phial>(), 10));
				break;
				case NPCID.AnglerFish:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Rebreather>(), 20));
				break;
				case NPCID.BloodCrawler:
				case NPCID.BloodCrawlerWall:
				case NPCID.FaceMonster:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Explosive_Artery>(), 87));
				break;
				case NPCID.UndeadMiner:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IWTPA_Standard>(), 4));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10));
				break;
				case NPCID.SporeSkeleton:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Irish_Cheddar>(), 6));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10));
				break;
				case NPCID.GiantTortoise:
				npcLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Rocodile>(), 17, 1, 1, new LootConditions.DownedPlantera()));
				break;
				case NPCID.Skeleton:
				case NPCID.SkeletonAlien:
				case NPCID.SkeletonArcher:
				case NPCID.SkeletonAstonaut:
				case NPCID.SkeletonCommando:
				case NPCID.SkeletonMerchant:
				case NPCID.SkeletonTopHat:
				case NPCID.ArmoredSkeleton:
				case NPCID.BigHeadacheSkeleton:
				case NPCID.BigMisassembledSkeleton:
				case NPCID.BigPantlessSkeleton:
				case NPCID.BigSkeleton:
				case NPCID.BoneThrowingSkeleton:
				case NPCID.BoneThrowingSkeleton2:
				case NPCID.BoneThrowingSkeleton3:
				case NPCID.BoneThrowingSkeleton4:
				case NPCID.GreekSkeleton:
				case NPCID.HeadacheSkeleton:
				case NPCID.HeavySkeleton:
				case NPCID.MisassembledSkeleton:
				case NPCID.PantlessSkeleton:
				case NPCID.SmallHeadacheSkeleton:
				case NPCID.SmallMisassembledSkeleton:
				case NPCID.SmallPantlessSkeleton:
				case NPCID.SmallSkeleton:
				case NPCID.TacticalSkeleton:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Bread>(), 10));
				break;
				case NPCID.TheGroom:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Comb>()));
				break;
				case NPCID.ZombieSuperman:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Superjump_Cape>(), 3));
				//npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Well_Gelled_Heroes_Hair>(), 3));
				break;
				case NPCID.MaggotZombie:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Grave_Danger>(), 20));
				break;
				default:
				break;
			}
			switch (npc.type) {
				case NPCID.DemonEye:
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Eyeball_Staff>(), 63));
				break;
			}
			CommonDrop harpoonRule = null;
			foreach (var rule in npcLoot.Get(includeGlobalDrops: false)) {
				List<DropRateInfo> drops = [];
				DropRateInfoChainFeed ratesInfo = new();
				rule.ReportDroprates(drops, ratesInfo);
				if (drops.Count != 0 && drops[0].itemId == ItemID.Harpoon && rule is CommonDrop harp) {
					harpoonRule = harp;
				}
				if (harpoonRule is not null) break;//add any further replacements with &&
			}
			if (harpoonRule is not null) {
				harpoonRule.itemId = ModContent.ItemType<Harpoon_Gun>();
				harpoonRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Harpoon>(), 1, 15, 99));
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Potato>(), 34));
			}
		}
		public override void OnKill(NPC npc) {
			switch (npc.type) {
				case NPCID.SkeletronHead:
				if (!NPC.downedBoss3) {
					GenFelnumOre();
					OriginSystem.Instance.forceThunderstormDelay = Main.rand.Next(600, (int)(Main.dayLength / 2));
				}
				break;
				default:
				break;
			}
			int shrapnelIndex = npc.FindBuffIndex(Impeding_Shrapnel_Debuff.ID);
			if (shrapnelIndex > -1) {
				Impeding_Shrapnel_Debuff.SpawnShrapnel(npc, npc.buffTime[shrapnelIndex]);
			}
			int outbreakIndex = npc.FindBuffIndex(Outbreak_Bomb_Owner_Buff.ID);
			if (outbreakIndex > -1) {
				Projectile.NewProjectile(npc.GetSource_Death(), npc.Center, default, ModContent.ProjectileType<Outbreak_Bomb_Cloud>(), 60, 1, npc.buffTime[outbreakIndex] - 1);
			}
		}
		public override void ModifyGlobalLoot(GlobalLoot globalLoot) {
			foreach (var rule in globalLoot.Get()) {
				if ((rule is ItemDropWithConditionRule conditionalRule) && conditionalRule.condition is Conditions.SoulOfNight) {
					conditionalRule.condition = new LootConditions.SoulOfNight();
				}
			}
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Dawn_Key>(), 2500, 1, 1, new LootConditions.Dawn_Key_Condition()));
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Defiled_Key>(), 2500, 1, 1, new LootConditions.Defiled_Key_Condition()));
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Dusk_Key>(), 2500, 1, 1, new LootConditions.Dusk_Key_Condition()));
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Hell_Key>(), 2500, 1, 1, new LootConditions.Hell_Key_Condition()));
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Mushroom_Key>(), 2500, 1, 1, new LootConditions.Mushroom_Key_Condition()));
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Ocean_Key>(), 2500, 1, 1, new LootConditions.Ocean_Key_Condition()));
			globalLoot.Add(new ItemDropWithConditionRule(ModContent.ItemType<Riven_Key>(), 2500, 1, 1, new LootConditions.Riven_Key_Condition()));
			globalLoot.Add(ItemDropRule.Common(ModContent.ItemType<Generic_Weapon>(), 50000));
		}

		static void GenFelnumOre() {
			string text = Language.GetTextValue("Mods.Origins.Status_Messages.Felnum_Spawn");
			if (Main.netMode == NetmodeID.SinglePlayer) {
				Main.NewText(text, Colors.RarityGreen);
			} else if (Main.netMode == NetmodeID.Server) {
				ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(text), Colors.RarityGreen);
			}
			if (!Main.gameMenu && Main.netMode != NetmodeID.MultiplayerClient) {
				int x = 0, y = 0;
				int felnumOre = ModContent.TileType<Felnum_Ore>();
				int type;
				Tile tile;
				int fails = 0;
				int success = 0;
				int maxFeln = (int)((Main.maxTilesX * Main.maxTilesY) * (Main.expertMode ? 6E-06 : 4E-06));
				for (int k = 0; k < maxFeln; k++) {
					int tries = 0;
					type = TileID.BlueDungeonBrick;
					while (type != TileID.Cloud && type != TileID.Dirt && type != TileID.Grass && type != TileID.Stone && type != TileID.RainCloud) {
						x = WorldGen.genRand.Next(0, Main.maxTilesX);
						y = WorldGen.genRand.Next(90, (int)OriginSystem.worldSurfaceLow - 5);
						tile = Framing.GetTileSafely(x, y);
						type = tile.HasTile ? tile.TileType : TileID.BlueDungeonBrick;
						if (++tries >= 150) {
							if (++fails % 2 == 0) k--;
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
