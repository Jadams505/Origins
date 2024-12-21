﻿using PegasusLib;
using PegasusLib.Reflection;
using System.Reflection;
using NPC = Terraria.NPC;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Origins.Reflection {
	public class ShopMethods : ILoadable {
		private delegate void AddHappinessReportText_Del(string textKeyInCategory, object substitutes = null, int otherNPCType = 0);
		private static AddHappinessReportText_Del _AddHappinessReportText;
		public static FastFieldInfo<ShopHelper, NPC> _currentNPCBeingTalkedTo { get; private set; }
		public static FastFieldInfo<ShopHelper, float> _currentPriceAdjustment { get; private set; }
		public void Load(Mod mod) {
			_AddHappinessReportText = typeof(ShopHelper).GetMethod("AddHappinessReportText", BindingFlags.NonPublic | BindingFlags.Instance).CreateDelegate<AddHappinessReportText_Del>(new ShopHelper());
			FieldInfo _currentNPCBeingTalkedTo = typeof(ShopHelper).GetField("_currentNPCBeingTalkedTo", BindingFlags.NonPublic | BindingFlags.Public);
			if (_currentNPCBeingTalkedTo is not null) ShopMethods._currentNPCBeingTalkedTo = new(_currentNPCBeingTalkedTo);
			else mod.Logger.Error("could not find ShopHelper._currentNPCBeingTalkedTo");
			_currentPriceAdjustment = new("_currentPriceAdjustment", BindingFlags.Public | BindingFlags.NonPublic);
		}
		public void Unload() {
			_AddHappinessReportText = null;
			_currentNPCBeingTalkedTo = null;
			_currentPriceAdjustment = null;
		}
		public static void AddHappinessReportText(ShopHelper instance, string textKeyInCategory, object substitutes = null, int otherNPCType = 0) {
			DelegateMethods._target.SetValue(_AddHappinessReportText, instance);
			_AddHappinessReportText(textKeyInCategory, substitutes, otherNPCType);
		}
	}
}