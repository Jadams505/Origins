﻿using Microsoft.Xna.Framework.Graphics;
using Origins.Dev;
using Origins.NPCs;
using Terraria;
using Terraria.Graphics.Shaders;

namespace Origins.Items.Other.Dyes {
    public class Rasterized_Dye : Dye_Item, ICustomWikiStat {
		public static int ID { get; private set; }
		public static int ShaderID { get; private set; }
        public string[] Categories => new string[] {
            "Dye"
        };
        public override void SetStaticDefaults() {
			ID = Type;
			GameShaders.Armor.BindShader(Type, new DelegatedArmorShaderData(
				Mod.Assets.Request<Effect>("Effects/Rasterize"),
				"Rasterize",
				(self, entity, _) => {
					float rasterizedTime = 0;
					if (entity is Player player) {
						rasterizedTime = player.GetModPlayer<OriginPlayer>().rasterizedTime;
					} else if (entity is NPC npc) {
						rasterizedTime = npc.GetGlobalNPC<OriginGlobalNPC>().rasterizedTime / 2f;
					}
					if (rasterizedTime == 0) {
						rasterizedTime = 8;
					}
					self.Shader.Parameters["uOffset"].SetValue(entity.velocity.WithMaxLength(4) * 0.125f * rasterizedTime);
				}
			))
			.UseImage(Origins.cellNoiseTexture.asset);
			ShaderID = GameShaders.Armor.GetShaderIdFromItemId(Type);
			Item.ResearchUnlockCount = 3;
		}
	}
}