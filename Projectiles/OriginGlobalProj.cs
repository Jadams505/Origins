﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Origins.Projectiles {
    public class OriginGlobalProj : GlobalProjectile {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        //bool init = true;
        bool felnumEffect = false;
        public static bool felnumEffectNext = false;
        public static bool hostileNext = false;
        public override void SetDefaults(Projectile projectile) {
            if(hostileNext) {
                projectile.hostile = true;
                hostileNext = false;
            }
            felnumEffect = felnumEffectNext;
            felnumEffectNext = false;
        }
        public override void AI(Projectile projectile) {
            if(felnumEffect) {
                if(projectile.melee) {
                    if(Main.player[projectile.owner].GetModPlayer<OriginPlayer>().felnumShock>19)Dust.NewDustPerfect(projectile.Center, 226, projectile.velocity.RotatedByRandom(0.1)*0.5f, Scale:0.5f);
                }else Dust.NewDustPerfect(projectile.Center, 226, projectile.velocity.RotatedByRandom(0.1)*0.5f, Scale:0.5f);
            }
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
            //this is actually how vanilla does projectile crits, which might explain why there are no vanilla multiclass weapons, since a 4% crit chance with a 4-class weapon would crit ~15% of the time
            if (Origins.ExplosiveProjectiles[projectile.type] && Main.rand.Next(1, 101) <= Main.player[projectile.owner].GetModPlayer<OriginPlayer>().explosiveCrit){
				crit = true;
			}
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit) {
            if(Origins.ExplosiveProjectiles[projectile.type]) {
                OriginPlayer originPlayer = Main.player[projectile.owner].GetModPlayer<OriginPlayer>();
                if(originPlayer.madHand) {
                    target.AddBuff(BuffID.Oiled, 600);
                    target.AddBuff(BuffID.OnFire, 600);
                }
            }
        }
        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox) {
            if(Origins.ExplosiveProjectiles[projectile.type]) {
                OriginPlayer originPlayer = Main.player[projectile.owner].GetModPlayer<OriginPlayer>();
                if(originPlayer.madHand&&(projectile.timeLeft<=3||projectile.penetrate==0)){
                    hitbox.Inflate(hitbox.Width/4,hitbox.Height/4);
                    //Main.NewText($"{hitbox.Width},{hitbox.Height}");
                }
            }
        }
        /*
            try {
                 GlobalProjectile[] globals = (GlobalProjectile[])typeof(Projectile).GetField("globalProjectiles", BindingFlags.NonPublic|BindingFlags.Instance).GetValue(projectile);
                foreach(GlobalProjectile g in globals) {
                    Main.NewText("found "+g.GetType());
                    mod.Logger.Info(g.GetType());
                }
            } catch(Exception e) {
                Main.NewText(e.GetType()+", see logs for more details");
                mod.Logger.Info(e);
            }
        */
    }
}
