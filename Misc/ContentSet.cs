﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Origins.Misc {
	public abstract class ContentSet<TContent, TData> : ILoadable where TContent : ILoadable, ContentSetItem<TData>, new() {
		public abstract IEnumerable<TData> GetContentItems();
		public void Load(Mod mod) {
			foreach (var item in GetContentItems()) {
				TContent content = new();
				content.SetFromData(item);
				mod.AddContent(content);
			}
		}
		public void Unload() { }
	}
	public interface ContentSetItem<TData> {
		public void SetFromData(TData data);
	}
}
