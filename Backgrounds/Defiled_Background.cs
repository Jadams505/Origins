﻿using Origins;
using Terraria;
using Terraria.ModLoader;

namespace ExampleMod.Backgrounds {
	public class Defiled_Background : ModSurfaceBgStyle {
        public override bool Autoload(ref string name) {
            return true;
        }

        public override bool ChooseBgStyle() {
			return !Main.gameMenu && Main.LocalPlayer.GetModPlayer<OriginPlayer>().ZoneDefiled;
		}

		public override int ChooseFarTexture() {
			return mod.GetBackgroundSlot("Backgrounds/Defiled_Background3");
		}

		public override int ChooseMiddleTexture() {
			return mod.GetBackgroundSlot("Backgrounds/Defiled_Background2");
		}

		public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b) {
			return mod.GetBackgroundSlot("Backgrounds/Defiled_Background3");
		}

		public override void ModifyFarFades(float[] fades, float transitionSpeed) {
			for (int i = 0; i < fades.Length; i++) {
				if (i == Slot) {
					fades[i] += transitionSpeed;
					if (fades[i] > 1f) {
						fades[i] = 1f;
					}
				}else {
					fades[i] -= transitionSpeed;
					if (fades[i] < 0f) {
						fades[i] = 0f;
					}
				}
			}
		}

        /*public override void FillTextureArray(int[] textureSlots) {
			textureSlots[0] = mod.GetBackgroundSlot("Backgrounds/Defiled_Background");
			textureSlots[1] = mod.GetBackgroundSlot("Backgrounds/Defiled_Background2");
			textureSlots[2] = mod.GetBackgroundSlot("Backgrounds/Defiled_Background3");
			//textureSlots[3] = mod.GetBackgroundSlot("Backgrounds/Void_Background");
		}*/
    }
}