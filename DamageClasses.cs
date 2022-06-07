﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Origins {
	public class DamageClasses : ILoadable {
		private static DamageClass explosive;
		private static DamageClass ranged_Magic;
		public static DamageClass Explosive => explosive ??= ModContent.GetInstance<Explosive>();
		public static Dictionary<DamageClass, DamageClass> ExplosiveVersion { get; private set; }
		public static DamageClass Ranged_Magic => ranged_Magic ??= ModContent.GetInstance<Ranged_Magic>();
		public void Load(Mod mod) {
			List<DamageClass> damageClasses = (List<DamageClass>)(typeof(DamageClassLoader).GetField("DamageClasses", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null));
			int len = damageClasses.Count;
			ExplosiveVersion = new Dictionary<DamageClass, DamageClass>(new DamageClass_Equality_Comparer());
			for (int i = 0; i < len; i++) {
				DamageClass other = damageClasses[i];
				ExplosiveVersion.Add(other, ExplosivePlus.CreateAndRegister(other));
			}
		}

		public void Unload() {
			explosive = null;
			ExplosiveVersion = null;
			ranged_Magic = null;
		}
	}
	public class DamageClass_Equality_Comparer : IEqualityComparer<DamageClass> {
		public bool Equals(DamageClass x, DamageClass y) => x.Type == y.Type;

		public int GetHashCode([DisallowNull] DamageClass obj) => obj.Type;
	}
	public class Explosive : DamageClass {
		public override void SetStaticDefaults() {
			ClassName.SetDefault("explosive damage");
		}
		public override void SetDefaultStats(Player player) {
			player.GetCritChance(this) += 4;
		}
	}
	[Autoload(false)]
	public class ExplosivePlus : DamageClass {
		DamageClass other;
		public ExplosivePlus(DamageClass other) {
			this.other = other;
		}
		internal static ExplosivePlus CreateAndRegister(DamageClass other) {
			ExplosivePlus newClass = new(other);
			newClass.Register();
			return newClass;
		}
		public override void SetStaticDefaults() {
			ClassName.SetDefault("explosive "+other.ClassName.GetDefault());
		}
		public override bool GetEffectInheritance(DamageClass damageClass) {
			return damageClass == DamageClasses.Explosive || damageClass == other || other.GetEffectInheritance(damageClass);
		}
		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
			if(damageClass == Generic || damageClass == DamageClasses.Explosive || damageClass == other) {
				return StatInheritanceData.Full;
			}
			return other.GetModifierInheritance(damageClass);
		}
		public override void SetDefaultStats(Player player) {
			//player.GetCritChance(this) += 4;
		}
	}
	public class Ranged_Magic : DamageClass {
		public override void SetStaticDefaults() {
			ClassName.SetDefault("ranged/magic damage");
		}
		public override StatInheritanceData GetModifierInheritance(DamageClass damageClass) {
			if (damageClass == Generic || damageClass == Ranged || damageClass == Magic) {
				return StatInheritanceData.Full;
			}
			return StatInheritanceData.None;
		}
		public override bool GetEffectInheritance(DamageClass damageClass) {
			return damageClass == Ranged || damageClass == Magic;
		}
		public override void SetDefaultStats(Player player) {
			player.GetCritChance(this) += 4;
		}
	}
}
