﻿using Origins.Items.Accessories;
using Origins.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Origins.Questing {
	public class Discount_1_Quest : Quest {
		//backing field for Stage property
		int stage = 0;

		int worms = 0;
		const int wormTarget = 10;
		//Stage property so changing quest stage also updates its event handlers
		public override int Stage {
			get => stage;
			set {
				stage = value;
				// clear kill event handlers so they only run if the quest is active
				PreUpdateInventoryEvent = null;
				UpdateInventoryEvent = null;
				switch (stage) {
					case 1:
					PreUpdateInventoryEvent = () => {
						worms = 0;
					};
					UpdateInventoryEvent = (item) => {
						if (item.makeNPC > 0) {
							worms += item.stack;
						}
					};
					break;
					case 2:
					worms = wormTarget;
					break;
				}
			}
		}
		public override bool Started => Stage > 0;
		public override bool Completed => Stage > 1;
		public override bool HasStartDialogue(NPC npc) {
			return npc.type == NPCID.BestiaryGirl && Stage == 0;
		}
		public override bool HasDialogue(NPC npc) {
			if (npc.type != NPCID.BestiaryGirl) return false; // NPCs other than the merchant won't have any dialogue related to this quest
			switch (Stage) {
				case 1:
				return worms >= wormTarget;
			}
			return false;
		}
		public override string GetDialogue() {
			switch (Stage) {
				case 1:
				return "Complete Quest";

				default:
				if (Origins.npcChatQuestSelected) {
					return "Accept";
				}
				return Language.GetTextValue(NameKey);
			}
		}
		public override void OnDialogue() {
			switch (stage) {
				case 0: {
					if (Origins.npcChatQuestSelected) {
						Stage = 1;
					} else {
						Main.npcChatText = Language.GetTextValue("Mods.Origins.Quests.Zoologist.Discount_1_Quest.Start");
						Origins.npcChatQuestSelected = true;// (npcChatQuestSelected is reset to false when the player closes the dialogue box)
					}
					break;
				}
				case 1: {
					ConsumeItems(Main.LocalPlayer.inventory, ((i) => i.makeNPC > 0, wormTarget));
					Main.npcChatText = Language.GetTextValue("Mods.Origins.Quests.Zoologist.Discount_1_Quest.Complete");
					Stage = 2;
					ShouldSync = true;
					break;
				}
			}
		}
		public override string GetJournalPage() {
			return Language.GetTextValue(
				"Mods.Origins.Quests.Zoologist.Discount_1_Quest.Journal", //translation key

				worms,
				wormTarget,
				StageTagOption(worms >= wormTarget),
				StageTagOption(Completed),
				NPC.GetFirstNPCNameOrNull(NPCID.BestiaryGirl) ?? Language.GetTextValue("Mods.Origins.Generic.Zoologist")
			);
		}
		public override void SetStaticDefaults() {
			NameKey = "Mods.Origins.Quests.Zoologist.Discount_1_Quest.Name";
		}
		public override void SaveData(TagCompound tag) {
			//save stage and kills
			tag.Add("Stage", Stage);
		}
		public override void LoadData(TagCompound tag) {
			//load stage and kills, note that it uses the Stage property so that it sets the event handlers
			//SafeGet returns the default value (0 for ints) if the tag doesn't have the data
			Stage = tag.SafeGet<int>("Stage");
		}
	}
	public class Discount_2_Quest : Quest {
		//backing field for Stage property
		int stage = 0;

		bool hasSquirrel = false;
		//Stage property so changing quest stage also updates its event handlers
		public override int Stage {
			get => stage;
			set {
				stage = value;
				// clear kill event handlers so they only run if the quest is active
				PreUpdateInventoryEvent = null;
				UpdateInventoryEvent = null;
				switch (stage) {
					case 1:
					PreUpdateInventoryEvent = () => {
						hasSquirrel = false;
					};
					UpdateInventoryEvent = (item) => {
						if (item.type == ItemID.SquirrelGold) {
							hasSquirrel = true;
						}
					};
					break;
					case 2:
					hasSquirrel = true;
					break;
				}
			}
		}
		public override bool Started => Stage > 0;
		public override bool Completed => Stage > 1;
		public override bool HasStartDialogue(NPC npc) {
			return npc.type == NPCID.BestiaryGirl && Stage == 0 && ModContent.GetInstance<Discount_1_Quest>().Completed;
		}
		public override bool HasDialogue(NPC npc) {
			if (npc.type != NPCID.BestiaryGirl) return false; // NPCs other than the merchant won't have any dialogue related to this quest
			switch (Stage) {
				case 1:
				return hasSquirrel = true;
			}
			return false;
		}
		public override string GetDialogue() {
			switch (Stage) {
				case 1:
				return "Complete Quest";

				default:
				if (Origins.npcChatQuestSelected) {
					return "Accept";
				}
				return Language.GetTextValue(NameKey);
			}
		}
		public override void OnDialogue() {
			switch (stage) {
				case 0: {
					if (Origins.npcChatQuestSelected) {
						Stage = 1;
					} else {
						Main.npcChatText = Language.GetTextValue("Mods.Origins.Quests.Zoologist.Discount_2_Quest.Start");
						Origins.npcChatQuestSelected = true;// (npcChatQuestSelected is reset to false when the player closes the dialogue box)
					}
					break;
				}
				case 1: {
					Item[] inventory = Main.LocalPlayer.inventory;
					RecipeGroup ironBarGroup = RecipeGroup.recipeGroups[RecipeGroupID.IronBar];
					ConsumeItems(inventory, ((i) => i.type == ItemID.SquirrelGold, 1));
					Main.npcChatText = Language.GetTextValue("Mods.Origins.Quests.Zoologist.Discount_2_Quest.Complete");
					Stage = 2;
					ShouldSync = true;
					break;
				}
			}
		}
		public override string GetJournalPage() {
			return Language.GetTextValue(
				"Mods.Origins.Quests.Zoologist.Discount_2_Quest.Journal", //translation key

				StageTagOption(hasSquirrel),
				StageTagOption(Completed),
				NPC.GetFirstNPCNameOrNull(NPCID.BestiaryGirl) ?? Language.GetTextValue("Mods.Origins.Generic.Zoologist")
			);
		}
		public override void SetStaticDefaults() {
			NameKey = "Mods.Origins.Quests.Zoologist.Discount_2_Quest.Name";
		}
		public override void SaveData(TagCompound tag) {
			//save stage and kills
			tag.Add("Stage", Stage);
		}
		public override void LoadData(TagCompound tag) {
			//load stage and kills, note that it uses the Stage property so that it sets the event handlers
			//SafeGet returns the default value (0 for ints) if the tag doesn't have the data
			Stage = tag.SafeGet<int>("Stage");
		}
	}
	public class Eccentric_Stone_Quest : Quest {
		//backing field for Stage property
		int stage = 0;

		int worms = 0;
		const int wormTarget = 10;
		//Stage property so changing quest stage also updates its event handlers
		public override int Stage {
			get => stage;
			set {
				stage = value;
				// clear kill event handlers so they only run if the quest is active
				PreUpdateInventoryEvent = null;
				UpdateInventoryEvent = null;
				switch (stage) {
					case 1:
					PreUpdateInventoryEvent = () => {
						worms = 0;
					};
					UpdateInventoryEvent = (item) => {
						if (item.type == ItemID.EnchantedNightcrawler) {
							worms += item.stack;
						}
					};
					break;
					case 2:
					worms = wormTarget;
					break;
				}
			}
		}
		public override bool Started => Stage > 0;
		public override bool Completed => Stage > 1;
		public override bool HasStartDialogue(NPC npc) {
			return npc.type == NPCID.BestiaryGirl && Stage == 0 && ModContent.GetInstance<Discount_2_Quest>().Completed;
		}
		public override bool HasDialogue(NPC npc) {
			if (npc.type != NPCID.BestiaryGirl) return false; // NPCs other than the merchant won't have any dialogue related to this quest
			switch (Stage) {
				case 1:
				return worms >= wormTarget;
			}
			return false;
		}
		public override string GetDialogue() {
			switch (Stage) {
				case 1:
				return "Complete Quest";

				default:
				if (Origins.npcChatQuestSelected) {
					return "Accept";
				}
				return Language.GetTextValue(NameKey);
			}
		}
		public override void OnDialogue() {
			switch (stage) {
				case 0: {
					if (Origins.npcChatQuestSelected) {
						Stage = 1;
					} else {
						Main.npcChatText = Language.GetTextValue("Mods.Origins.Quests.Zoologist.Eccentric_Stone_Quest.Start");
						Origins.npcChatQuestSelected = true;// (npcChatQuestSelected is reset to false when the player closes the dialogue box)
					}
					break;
				}
				case 1: {
					Item[] inventory = Main.LocalPlayer.inventory;
					RecipeGroup ironBarGroup = RecipeGroup.recipeGroups[RecipeGroupID.IronBar];
					ConsumeItems(inventory, ((i) => i.type == ItemID.EnchantedNightcrawler, wormTarget));
					Main.npcChatText = Language.GetTextValue("Mods.Origins.Quests.Zoologist.Eccentric_Stone_Quest.Complete");


					int index = Item.NewItem(
						Main.LocalPlayer.GetSource_GiftOrReward(),
						Main.LocalPlayer.position,
						Main.LocalPlayer.Size,
						ModContent.ItemType<Eccentric_Stone>()
					);
					if (Main.netMode == NetmodeID.MultiplayerClient) {
						NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);
					}

					Stage = 2;
					ShouldSync = true;
					break;
				}
			}
		}
		public override string GetJournalPage() {
			return Language.GetTextValue(
				"Mods.Origins.Quests.Zoologist.Eccentric_Stone_Quest.Journal", //translation key

				worms,
				wormTarget,
				StageTagOption(worms >= wormTarget),
				StageTagOption(Completed),
				NPC.GetFirstNPCNameOrNull(NPCID.BestiaryGirl) ?? Language.GetTextValue("Mods.Origins.Generic.Zoologist")
			);
		}
		public override void SetStaticDefaults() {
			NameKey = "Mods.Origins.Quests.Zoologist.Eccentric_Stone_Quest.Name";
		}
		public override void SaveData(TagCompound tag) {
			//save stage and kills
			tag.Add("Stage", Stage);
		}
		public override void LoadData(TagCompound tag) {
			//load stage and kills, note that it uses the Stage property so that it sets the event handlers
			//SafeGet returns the default value (0 for ints) if the tag doesn't have the data
			Stage = tag.SafeGet<int>("Stage");
		}
	}
}
