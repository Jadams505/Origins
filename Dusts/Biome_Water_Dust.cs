using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Origins.Dusts {
    public class White_Water_Dust : ModDust {
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			ID = Type;
		}
		public override void OnSpawn(Dust dust) {
			dust.alpha = 170;
			dust.velocity *= 0.5f;
			dust.velocity.Y += 1f;
		}
		public override bool Update(Dust dust) {
			if (dust.velocity.X == 0f) {
				if (Collision.SolidCollision(dust.position, 2, 2)) {
					dust.scale = 0f;
				}
				dust.rotation += 0.5f;
				dust.scale -= 0.01f;
			}
			if (Collision.WetCollision(new Vector2(dust.position.X, dust.position.Y), 4, 4)) {
				dust.alpha += 20;
				dust.scale -= 0.1f;
			}
			dust.alpha += 2;
			dust.scale -= 0.005f;
			if (dust.alpha > 255) {
				dust.scale = 0f;
			}
			if (dust.velocity.Y > 4f) {
				dust.velocity.Y = 4f;
			}
			if (dust.noGravity) {
				if (dust.velocity.X < 0f) {
					dust.rotation -= 0.2f;
				} else {
					dust.rotation += 0.2f;
				}
				dust.scale += 0.03f;
				dust.velocity.X *= 1.05f;
				dust.velocity.Y += 0.15f;
			}
			return true;
        }
	}
	public class Gooey_Water_Dust : ModDust {
		public static int ID { get; private set; } = -1;
		public override void SetStaticDefaults() {
			ID = Type;
		}
		public override void OnSpawn(Dust dust) {
			dust.alpha = 170;
			dust.velocity *= 0.5f;
			dust.velocity.Y += 1f;
		}
		public override bool Update(Dust dust) {
			if (dust.velocity.X == 0f) {
				if (Collision.SolidCollision(dust.position, 2, 2)) {
					dust.scale = 0f;
				}
				dust.rotation += 0.5f;
				dust.scale -= 0.01f;
			}
			if (Collision.WetCollision(new Vector2(dust.position.X, dust.position.Y), 4, 4)) {
				dust.alpha += 20;
				dust.scale -= 0.1f;
			}
			dust.alpha += 2;
			dust.scale -= 0.005f;
			if (dust.alpha > 255) {
				dust.scale = 0f;
			}
			if (dust.velocity.Y > 4f) {
				dust.velocity.Y = 4f;
			}
			if (dust.noGravity) {
				if (dust.velocity.X < 0f) {
					dust.rotation -= 0.2f;
				} else {
					dust.rotation += 0.2f;
				}
				dust.scale += 0.03f;
				dust.velocity.X *= 1.05f;
				dust.velocity.Y += 0.15f;
			}
			return true;
		}
		public override Color? GetAlpha(Dust dust, Color lightColor) {
			return Color.Lerp(lightColor, new Color(1f, 1f, 1f, 0f), 0.75f);
		}
	}
}