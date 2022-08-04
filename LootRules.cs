﻿using Origins.World.BiomeData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Origins.LootConditions {
	public class OneOfEachRule : IItemDropRule {
		public List<IItemDropRuleChainAttempt> ChainedRules { get; }
		IItemDropRule[] rules;
		public bool CanDrop(DropAttemptInfo info) => true;
		public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) {
			for (int i = 0; i < rules.Length; i++) {
				rules[i].ReportDroprates(drops, ratesInfo);
			}
		}
		public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info) {
			ItemDropAttemptResult result = new ItemDropAttemptResult() {
				State = ItemDropAttemptResultState.DidNotRunCode
			};
			for (int i = 0; i < rules.Length; i++) {
				switch (rules[i].TryDroppingItem(info).State) {
					case ItemDropAttemptResultState.Success:
					result.State = ItemDropAttemptResultState.Success;
					break;
					case ItemDropAttemptResultState.DoesntFillConditions:
					if (result.State is ItemDropAttemptResultState.DidNotRunCode or ItemDropAttemptResultState.FailedRandomRoll) {
						result.State = ItemDropAttemptResultState.DoesntFillConditions;
					}
					break;
					case ItemDropAttemptResultState.FailedRandomRoll:
					if (result.State is ItemDropAttemptResultState.DidNotRunCode) {
						result.State = ItemDropAttemptResultState.FailedRandomRoll;
					}
					break;
				}
			}
			return result;
		}
	}
	public class IsWorldEvil : IItemDropRuleCondition {
		int worldEvil;
		public IsWorldEvil(int worldEvil) {
			this.worldEvil = worldEvil;
		}
		public bool CanDrop(DropAttemptInfo info) {
			return ModContent.GetInstance<OriginSystem>().worldEvil == worldEvil;
		}

		public bool CanShowItemDropInUI() {
			return ModContent.GetInstance<OriginSystem>().worldEvil == worldEvil;
		}

		public string GetConditionDescription() {
			return worldEvil switch {
				OriginSystem.evil_corruption => Language.GetTextValue("Bestiary_ItemDropConditions.IsCorruption"),
				OriginSystem.evil_crimson => Language.GetTextValue("Bestiary_ItemDropConditions.IsCrimson"),
				OriginSystem.evil_wastelands => Language.GetTextValue("Bestiary_ItemDropConditions.IsCorruption"),
				OriginSystem.evil_riven => Language.GetTextValue("Bestiary_ItemDropConditions.IsCrimson"),
				_ => Language.GetTextValue("Bestiary_ItemDropConditions.IsCorruption")
			};
		}
	}
	public class IsWorldEvilAndNotExpert : IItemDropRuleCondition {
		int worldEvil;
		public IsWorldEvilAndNotExpert(int worldEvil) {
			this.worldEvil = worldEvil;
		}
		public bool CanDrop(DropAttemptInfo info) {
			return !Main.expertMode && ModContent.GetInstance<OriginSystem>().worldEvil == worldEvil;
		}

		public bool CanShowItemDropInUI() {
			return !Main.expertMode && ModContent.GetInstance<OriginSystem>().worldEvil == worldEvil;
		}

		public string GetConditionDescription() {
			return worldEvil switch {
				OriginSystem.evil_corruption => Language.GetTextValue("Bestiary_ItemDropConditions.IsCorruptionAndNotExpert"),
				OriginSystem.evil_crimson => Language.GetTextValue("Bestiary_ItemDropConditions.IsCrimsonAndNotExpert"),
				OriginSystem.evil_wastelands => Language.GetTextValue("Bestiary_ItemDropConditions.IsCorruptionAndNotExpert"),
				OriginSystem.evil_riven => Language.GetTextValue("Bestiary_ItemDropConditions.IsCrimsonAndNotExpert"),
				_ => Language.GetTextValue("Bestiary_ItemDropConditions.IsCorruptionAndNotExpert")
			};
		}
	}
	public class DefiledKeyCondition : IItemDropRuleCondition {
		public bool CanDrop(DropAttemptInfo info) {
			return info.npc.value > 0f && Main.hardMode && !info.IsInSimulation && info.player.GetModPlayer<OriginPlayer>().ZoneDefiled;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return Language.GetTextValue("Bestiary_ItemDropConditions.DesertKeyCondition");
		}
	}
	public class SoulOfNight : IItemDropRuleCondition, IProvideItemConditionDescription {
		public bool CanDrop(DropAttemptInfo info) {
			if (Conditions.SoulOfWhateverConditionCanDrop(info)) {
				return info.player.ZoneCorrupt || info.player.ZoneCrimson || info.player.InModBiome<Defiled_Wastelands>() || info.player.InModBiome<Riven_Hive>();
			}
			return false;
		}

		public bool CanShowItemDropInUI() {
			return true;
		}

		public string GetConditionDescription() {
			return Language.GetTextValue("Mods.Origins.ItemDropConditions.SoulOfNight");
		}
	}
}