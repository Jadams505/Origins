﻿using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Tyfyter.Utils;

namespace Origins.Projectiles {
	//separate global for organization
	public class MeleeGlobalProjectile : GlobalProjectile {
		internal static bool[] applyScaleToProjectile;
		public static bool[] ApplyScaleToProjectile { get => applyScaleToProjectile; }
		public override bool InstancePerEntity => true;
		protected override bool CloneNewInstances => false;
		float scaleModifier = 1f;
		public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.CountsAsClass(DamageClass.Melee);
		public override void OnSpawn(Projectile projectile, IEntitySource source) {
			if (ApplyScaleToProjectile[projectile.type] && source is EntitySource_ItemUse itemUse) {
				SetScaleModifier(projectile, itemUse.Player.GetAdjustedItemScale(itemUse.Item));
			}
		}
		public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter) {
			binaryWriter.Write(scaleModifier);
		}
		public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader) {
			scaleModifier = binaryReader.ReadSingle();
		}
		public void SetScaleModifier(Projectile projectile, float modifier) {
			projectile.scale = (projectile.scale / scaleModifier) * modifier;
			scaleModifier = modifier;
		}
	}
}
