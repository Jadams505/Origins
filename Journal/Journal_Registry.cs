﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Origins.Journal {
	public static class Journal_Registry  {
		public static Dictionary<string, JournalEntry> Entries { get; internal set; }
		public static JournalEntry GetJournalEntryByTextKey(string key) {
			return Entries.Values.Where((e) => e.TextKey == key).FirstOrDefault();
		}
	}
}
