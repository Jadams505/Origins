﻿using Microsoft.Xna.Framework.Graphics;
using Origins.Dev;
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
			GameShaders.Armor.BindShader(Type, new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/Rasterize"), "Rasterize"))
			.UseImage(Origins.cellNoiseTexture.asset);
			ShaderID = GameShaders.Armor.GetShaderIdFromItemId(Type);
			Item.ResearchUnlockCount = 3;
		}
	}
}
