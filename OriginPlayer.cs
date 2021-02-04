﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Origins.Buffs;
using Origins.Items.Armor.Vanity.Terlet.PlagueTexan;
using Origins.Items.Materials;
using Origins.Items.Weapons.Explosives;
using Origins.Items.Weapons.Summon;
using Origins.Projectiles;
using Origins.World;
using Origins.World.BiomeData;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Origins.Items.OriginGlobalItem;
using static Origins.OriginExtensions;

namespace Origins {
    public class OriginPlayer : ModPlayer {
        public const float rivenMaxMult = 0.3f;
        public float rivenMult => (1f-rivenMaxMult)+Math.Max((player.statLife/(float)player.statLifeMax2)*(rivenMaxMult*2), rivenMaxMult);

        public bool fiberglassSet = false;
        public bool cryostenSet = false;
        public bool cryostenHelmet = false;
        public bool felnumSet = false;
        public float felnumShock = 0;
        public float oldFelnumShock = 0;
        public bool celestineSet = false;
        //public const int FelnumMax = 100;
        public bool minerSet = false;
        public bool defiledSet = false;
        public bool rivenSet = false;

        public bool bombHandlingDevice = false;
        public bool dimStarlight = false;
        public byte dimStarlightCooldown = 0;
        public bool madHand = false;

        public float explosiveDamage = 1;
        public int explosiveCrit = 4;
        public float explosiveThrowSpeed = 1;

        public bool ZoneVoid = false;
        public float ZoneVoidProgress = 0;
        public float ZoneVoidProgressSmoothed = 0;

        public bool ZoneDefiled = false;
        public float ZoneDefiledProgress = 0;
        public float ZoneDefiledProgressSmoothed = 0;

        public bool DrawShirt = false;
        public bool DrawPants = false;
        public bool ItemLayerWrench = false;
        public bool PlagueSight = false;

        internal static bool ItemChecking = false;
        public int cryostenLifeRegenCount = 0;
        internal byte oldBonuses = 0;
        public const int minionSubSlotValues = 3;
        public float[] minionSubSlots = new float[minionSubSlotValues];
        public int wormHeadIndex = -1;
        public override void ResetEffects() {
            oldBonuses = 0;
            if(fiberglassSet)oldBonuses|=1;
            if(felnumSet)oldBonuses|=2;
            if(!player.frozen) {
                DrawShirt = false;
                DrawPants = false;
            }
            fiberglassSet = false;
            cryostenSet = false;
            cryostenHelmet = false;
            oldFelnumShock = felnumShock;
            if(!felnumSet) {
                felnumShock = 0;
            } else if(felnumShock>player.statLifeMax2) {
                felnumShock-=(felnumShock-player.statLifeMax2)/player.statLifeMax2*5+1;
            }
            felnumSet = false;
            celestineSet = false;
            minerSet = false;
            defiledSet = false;
            rivenSet = false;
            bombHandlingDevice = false;
            dimStarlight = false;
            madHand = false;
            explosiveDamage = 1f;
            explosiveCrit = 4;
            explosiveThrowSpeed = 1f;
            if(IsExplosive(player.HeldItem)) {
                explosiveCrit += player.HeldItem.crit;
            }
            if(cryostenLifeRegenCount>0)
                cryostenLifeRegenCount--;
            if(dimStarlightCooldown>0)
                dimStarlightCooldown--;
            player.breathMax = 200;
            PlagueSight = false;
            minionSubSlots = new float[minionSubSlotValues];
        }
        public override void PostUpdateMiscEffects() {
            if(cryostenHelmet) {
                if(player.statLife!=player.statLifeMax2&&(int)Main.time%(cryostenLifeRegenCount>0 ? 5 : 15)==0)
                    for(int i = 0; i < 10; i++) {
                        int num6 = Dust.NewDust(player.position, player.width, player.height, 92);
                        Main.dust[num6].noGravity = true;
                        Main.dust[num6].velocity *= 0.75f;
                        int num7 = Main.rand.Next(-40, 41);
                        int num8 = Main.rand.Next(-40, 41);
                        Main.dust[num6].position.X += num7;
                        Main.dust[num6].position.Y += num8;
                        Main.dust[num6].velocity.X = -num7 * 0.075f;
                        Main.dust[num6].velocity.Y = -num8 * 0.075f;
                    }
            }
        }
        public override void UpdateLifeRegen() {
            if(cryostenHelmet)player.lifeRegenCount+=cryostenLifeRegenCount>0 ? 180 : 1;
        }
        public override void ModifyWeaponDamage(Item item, ref float add, ref float mult, ref float flat) {
            if(IsExplosive(item))add+=explosiveDamage-1;
            if(fiberglassSet) {
                flat+=4;
            }
            if(rivenSet&&!ItemChecking) {
                mult*=rivenMult;
            }
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit) {
            if(felnumShock>29) {
                damage+=(int)(felnumShock/15);
                felnumShock = 0;
                Main.PlaySound(SoundID.Item, (int)player.Center.X, (int)player.Center.Y, 122, 2f, 1f);
            }
        }
        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) {
            if(item.useAmmo == 0&&IsExplosive(item)) {
                speedX*=explosiveThrowSpeed;
                speedY*=explosiveThrowSpeed;
            }
            if(item.shoot>ProjectileID.None&&felnumShock>29) {
                Projectile p = new Projectile();
                p.SetDefaults(item.shoot);
                OriginGlobalProj.felnumEffectNext = true;
                if(p.melee)
                    return true;
                damage+=(int)(felnumShock/15);
                felnumShock = 0;
                Main.PlaySound(SoundID.Item, (int)player.Center.X, (int)player.Center.Y, 122, 2f, 1f);
            }
            return true;
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection) {
            if(Origins.ExplosiveModOnHit.Contains(proj.type)) {
                damage = (int)(damage*(player.allDamage+explosiveDamage-1)*0.7f);
            }
            if(Origins.ExplosiveProjectiles[proj.type]) {
                damage+=target.defense/10;
            }
            if(fiberglassSet) {
                damage+=4;
            }
            if(proj.melee&&felnumShock>29) {
                damage+=(int)(felnumShock/15);
                felnumShock = 0;
                Main.PlaySound(SoundID.Item, (int)player.Center.X, (int)player.Center.Y, 122, 2f, 1f);
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit) {
            if(crit) {
                if(celestineSet)
                    Item.NewItem(target.Hitbox, Main.rand.Next(Origins.celestineBoosters));
                if(dimStarlight&&dimStarlightCooldown<1) {
                    Item.NewItem(target.position, target.width, target.height, ItemID.Star);
                    dimStarlightCooldown = 90;
                }
            }
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit) {
            if(crit) {
                if(celestineSet)
                    Item.NewItem(target.Hitbox, Main.rand.Next(Origins.celestineBoosters));
                if(dimStarlight&&dimStarlightCooldown<1) {
                    Item.NewItem(target.position, target.width, target.height, ItemID.Star);
                    dimStarlightCooldown = 90;
                }
            }
        }
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit) {
            if(minerSet)
                if(proj.owner == player.whoAmI && proj.friendly) {
                    damage = (int)(damage/explosiveDamage);
                    damage-=damage/5;
                }
        }
        public override void PostSellItem(NPC vendor, Item[] shopInventory, Item item) {
            if(vendor.type==NPCID.Demolitionist&&item.type==ModContent.ItemType<Peat_Moss>()) {
                OriginWorld originWorld = ModContent.GetInstance<OriginWorld>();
                if(originWorld.peatSold<20 && item.type==ModContent.ItemType<Peat_Moss>()) {
                    if(item.stack>=20-originWorld.peatSold) {
                        item.stack-=20-originWorld.peatSold;
                        originWorld.peatSold = 20;
                        int nextSlot = 0;
                        for(; ++nextSlot<shopInventory.Length&&!shopInventory[nextSlot].IsAir;);
                        if(nextSlot<shopInventory.Length)shopInventory[nextSlot++].SetDefaults(ModContent.ItemType<Impact_Grenade>());
                        if(nextSlot<shopInventory.Length)shopInventory[nextSlot++].SetDefaults(ModContent.ItemType<Impact_Bomb>());
                        if(nextSlot<shopInventory.Length)shopInventory[nextSlot].SetDefaults(ModContent.ItemType<Impact_Dynamite>());
                    } else {
                        originWorld.peatSold+=item.stack;
                        item.TurnToAir();
                    }
                }
            }
        }
        /*public override void PostBuyItem(NPC vendor, Item[] shopInventory, Item item) {
            if(item.type==ModContent.ItemType<Peat_Moss>()) {

            }
        }*/
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource) {
            if(defiledSet) {
                float manaDamage = damage*0.15f;
                float costMult = 3;
                float costMult2 = (1/(player.magicDamage+player.allDamage-1f))/(player.magicDamageMult*player.allDamageMult);
                if(player.statMana < manaDamage*costMult) {
                    manaDamage = player.statMana/costMult;
                }
                if(player.magicCuffs) {
                    if(costMult2>1)
                        costMult2 = 1/costMult2;
                }
                if(manaDamage*costMult*costMult2>=1f)
                    player.ManaEffect((int)-(manaDamage*costMult*costMult2));
                player.CheckMana((int)Math.Floor(manaDamage*costMult*costMult2), true);
                damage = (int)(damage-manaDamage);
                player.magicCuffs = false;
                player.AddBuff(ModContent.BuffType<Defiled_Exhaustion_Buff>(), 10);
            }
            return damage != 0;
        }
        public override void UpdateBiomes() {
            ZoneVoid = OriginWorld.voidTiles > 300;
            ZoneVoidProgress = Math.Min(OriginWorld.voidTiles - 200, 200)/300f;
            ZoneDefiled = OriginWorld.defiledTiles > DefiledWastelands.NeededTiles;
            ZoneDefiledProgress = Math.Min(OriginWorld.defiledTiles - (DefiledWastelands.NeededTiles-DefiledWastelands.ShaderTileCount), DefiledWastelands.ShaderTileCount)/DefiledWastelands.ShaderTileCount;
            LinearSmoothing(ref ZoneVoidProgressSmoothed, ZoneVoidProgress, OriginWorld.biomeShaderSmoothing);
            LinearSmoothing(ref ZoneDefiledProgressSmoothed, ZoneDefiledProgress, OriginWorld.biomeShaderSmoothing);
            /*if(ZoneVoidProgress!=ZoneVoidProgressSmoothed) {
                if(Math.Abs(ZoneVoidProgress-ZoneVoidProgressSmoothed)<OriginWorld.biomeShaderSmoothing) {
                    ZoneVoidProgressSmoothed = ZoneVoidProgress;
                } else {
                    if(ZoneVoidProgress>ZoneVoidProgressSmoothed) {
                        ZoneVoidProgressSmoothed+=OriginWorld.biomeShaderSmoothing;
                    }else if(ZoneVoidProgress<ZoneVoidProgressSmoothed) {
                        ZoneVoidProgressSmoothed-=OriginWorld.biomeShaderSmoothing;
                    }
                }
            }
            if(ZoneDefiledProgress!=ZoneDefiledProgressSmoothed) {
                if(Math.Abs(ZoneDefiledProgress-ZoneDefiledProgressSmoothed)<OriginWorld.biomeShaderSmoothing) {
                    ZoneDefiledProgressSmoothed = ZoneDefiledProgress;
                } else {
                    if(ZoneDefiledProgress>ZoneDefiledProgressSmoothed) {
                        ZoneDefiledProgressSmoothed+=OriginWorld.biomeShaderSmoothing;
                    }else if(ZoneDefiledProgress<ZoneDefiledProgressSmoothed) {
                        ZoneDefiledProgressSmoothed-=OriginWorld.biomeShaderSmoothing;
                    }
                }
            }*/
        }
        public override bool CustomBiomesMatch(Player other) {
            OriginPlayer modOther = other.GetModPlayer<OriginPlayer>();
            return !((ZoneVoid^modOther.ZoneVoid)||(ZoneDefiled^modOther.ZoneDefiled));
        }
        public override void SendCustomBiomes(BinaryWriter writer) {
            byte flags = 0;
            if(ZoneVoid)
                flags |= 1;
            if(ZoneDefiled)
                flags |= 2;
            writer.Write(flags);
            //writer.Write(ZoneVoidTime);
        }

        public override void ReceiveCustomBiomes(BinaryReader reader) {
            byte flags = reader.ReadByte();
            ZoneVoid = ((flags & 1)!=0);
            ZoneDefiled = ((flags & 2)!=0);
            //ZoneVoidTime = reader.ReadInt32();
        }

        public override void CopyCustomBiomesTo(Player other) {
            OriginPlayer modOther = other.GetModPlayer<OriginPlayer>();
            //modOther.ZoneVoidTime = ZoneVoidTime;
            modOther.ZoneVoid = ZoneVoid;
            modOther.ZoneDefiled = ZoneDefiled;
        }
        public override void UpdateBiomeVisuals() {
            player.ManageSpecialBiomeVisuals("Origins:ZoneDusk", ZoneVoidProgressSmoothed>0, player.Center);
            if(ZoneVoidProgressSmoothed>0)
                Filters.Scene["Origins:ZoneDusk"].GetShader().UseProgress(ZoneVoidProgressSmoothed);
            player.ManageSpecialBiomeVisuals("Origins:ZoneDefiled", ZoneDefiledProgressSmoothed>0, player.Center);
            if(ZoneDefiledProgressSmoothed>0)
                Filters.Scene["Origins:ZoneDefiled"].GetShader().UseProgress(ZoneDefiledProgressSmoothed);
            /*if(ZoneVoidProgress>0) {
            }*/
        }
        public override bool PreItemCheck() {
            ItemChecking = true;
            return true;
        }
        public override void PostItemCheck() {
            ItemChecking = false;
        }
        public override void ModifyDrawLayers(List<PlayerLayer> layers) {
            if(DrawShirt) {
                int itemindex = layers.IndexOf(PlayerLayer.HeldItem);
                PlayerLayer itemlayer = layers[itemindex];
                layers.RemoveAt(itemindex);
                layers.Insert(layers.IndexOf(PlayerLayer.MountFront), itemlayer);
                layers.Insert(layers.IndexOf(PlayerLayer.MountFront), PlayerShirt);
                PlayerShirt.visible = true;
            }
            if(DrawPants) {
                layers.Insert(layers.IndexOf(PlayerLayer.Legs), PlayerPants);
                PlayerPants.visible = true;
            }
            if(felnumShock>0) {
                layers.Add(FelnumGlow);
                FelnumGlow.visible = true;
            }
            if(ItemLayerWrench && !player.HeldItem.noUseGraphic) {
                switch(player.HeldItem.useStyle) {
                    case 5:
                    layers[layers.IndexOf(PlayerLayer.HeldItem)] = ShootWrenchLayer;
                    ShootWrenchLayer.visible = true;
                    break;
                    default:
                    layers[layers.IndexOf(PlayerLayer.HeldItem)] = SlashWrenchLayer;
                    SlashWrenchLayer.visible = true;
                    break;
                }
            }
            ItemLayerWrench = false;
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo) {
            if(PlagueSight) drawInfo.eyeColor = new Color(255,215,0);
            if(drawInfo.drawPlayer.body==Origins.PlagueTexanJacketID) drawInfo.drawHands = true;
        }
        public override void FrameEffects() {
            for(int i = 13; i < 18+player.extraAccessorySlots; i++) {
                if(player.armor[i].type==Plague_Texan_Sight.id)Plague_Texan_Sight.ApplyVisuals(player);
            }
        }
        //public static PlayerLayer PlagueEyes = new PlayerLayer("Origins", "PlagueEyes", null, (drawInfo)=> {drawInfo.eyeColor = Color.Goldenrod;});
        public static PlayerLayer PlayerShirt = new PlayerLayer("Origins", "PlayerShirt", null, delegate (PlayerDrawInfo drawInfo2) {
            Player drawPlayer = drawInfo2.drawPlayer;
            Vector2 Position = drawInfo2.position;
            SpriteEffects spriteEffects = drawInfo2.spriteEffects;
            int skinVariant = drawPlayer.skinVariant;
            DrawData drawData;
            drawData = new DrawData(Main.playerTextures[skinVariant, 14], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.legFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.legFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo2.legOrigin, new Rectangle?(drawPlayer.legFrame), drawInfo2.shirtColor, drawPlayer.legRotation, drawInfo2.legOrigin, 1f, spriteEffects, 0);
            Main.playerDrawData.Add(drawData);
            if(!drawPlayer.Male) {
                drawData = new DrawData(Main.playerTextures[skinVariant, 4], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), new Rectangle?(drawPlayer.bodyFrame), drawInfo2.underShirtColor, drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
                drawData = new DrawData(Main.playerTextures[skinVariant, 6], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), new Rectangle?(drawPlayer.bodyFrame), drawInfo2.shirtColor, drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
            } else {
                drawData = new DrawData(Main.playerTextures[skinVariant, 4], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), new Rectangle?(drawPlayer.bodyFrame), drawInfo2.underShirtColor, drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
                drawData = new DrawData(Main.playerTextures[skinVariant, 6], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), new Rectangle?(drawPlayer.bodyFrame), drawInfo2.shirtColor, drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
                Main.playerDrawData.Add(drawData);
            }
            drawData = new DrawData(Main.playerTextures[skinVariant, 5], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2), new Rectangle?(drawPlayer.bodyFrame), drawInfo2.bodyColor, drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
            Main.playerDrawData.Add(drawData);
        });
        public static PlayerLayer PlayerPants = new PlayerLayer("Origins", "PlayerPants", null, delegate (PlayerDrawInfo drawInfo2) {
            Player drawPlayer = drawInfo2.drawPlayer;
            Vector2 Position = drawInfo2.position;
            SpriteEffects spriteEffects = drawInfo2.spriteEffects;
            int skinVariant = drawPlayer.skinVariant;
            DrawData drawData;
            drawData = new DrawData(Main.playerTextures[skinVariant, 11], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.legFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.legFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo2.legOrigin, new Rectangle?(drawPlayer.legFrame), drawInfo2.pantsColor, drawPlayer.legRotation, drawInfo2.legOrigin, 1f, spriteEffects, 0);
            Main.playerDrawData.Add(drawData);
            drawData = new DrawData(Main.playerTextures[skinVariant, 12], new Vector2((int)(Position.X - Main.screenPosition.X - drawPlayer.legFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.legFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo2.legOrigin, new Rectangle?(drawPlayer.legFrame), drawInfo2.shoeColor, drawPlayer.legRotation, drawInfo2.legOrigin, 1f, spriteEffects, 0);
            Main.playerDrawData.Add(drawData);
        });
        public static PlayerLayer ShootWrenchLayer = new PlayerLayer("Origins", "FiberglassBowLayer", null, delegate (PlayerDrawInfo drawInfo2) {
            Player drawPlayer = drawInfo2.drawPlayer;
            float num77 = drawPlayer.itemRotation + MathHelper.PiOver4 * drawPlayer.direction;
            Item item = drawPlayer.inventory[drawPlayer.selectedItem];
            Texture2D itemTexture = Main.itemTexture[item.type];
            IAnimatedItem aItem = (IAnimatedItem)item.modItem;
            int num80 = 10;
            Vector2 vector7 = new Vector2(itemTexture.Width / 2, itemTexture.Height / 2);
            Vector2 vector8 = OriginExtensions.DrawPlayerItemPos(drawPlayer.gravDir, item.type);
            num80 = (int)vector8.X;
            vector7.Y = vector8.Y;
            Vector2 origin4 = new Vector2(-num80, itemTexture.Height / 2);
            if(drawPlayer.direction == -1) {
                origin4 = new Vector2(itemTexture.Width + num80, itemTexture.Height / 2);
            }
            origin4.X-=drawPlayer.width/2;
            Vector4 col = drawInfo2.faceColor.ToVector4()/drawPlayer.skinColor.ToVector4();
            DrawData value = new DrawData(itemTexture, new Vector2((int)(drawInfo2.itemLocation.X - Main.screenPosition.X + vector7.X), (int)(drawInfo2.itemLocation.Y - Main.screenPosition.Y + vector7.Y)), aItem.Animation.GetFrame(itemTexture), item.GetAlpha(new Color(col.X, col.Y, col.Z, col.W)), drawPlayer.itemRotation, origin4, item.scale, drawInfo2.spriteEffects, 0);
            Main.playerDrawData.Add(value);
            if(drawPlayer.inventory[drawPlayer.selectedItem].glowMask != -1) {
                value = new DrawData(Main.glowMaskTexture[item.glowMask], new Vector2((int)(drawInfo2.itemLocation.X - Main.screenPosition.X + vector7.X), (int)(drawInfo2.itemLocation.Y - Main.screenPosition.Y + vector7.Y)), aItem.Animation.GetFrame(itemTexture), item.GetAlpha(aItem.GlowmaskTint??new Color(col.X, col.Y, col.Z, col.W)), drawPlayer.itemRotation, origin4, item.scale, drawInfo2.spriteEffects, 0);
                Main.playerDrawData.Add(value);
            }
        });
        public static PlayerLayer SlashWrenchLayer = new PlayerLayer("Origins", "FelnumBroadswordLayer", null, delegate (PlayerDrawInfo drawInfo2) {
            Player drawPlayer = drawInfo2.drawPlayer;
            float num77 = drawPlayer.itemRotation + MathHelper.PiOver4 * drawPlayer.direction;
            Item item = drawPlayer.inventory[drawPlayer.selectedItem];
            Texture2D itemTexture = Main.itemTexture[item.type];
            IAnimatedItem aItem = (IAnimatedItem)item.modItem;
            Rectangle frame = aItem.Animation.GetFrame(itemTexture);
            Color currentColor = Lighting.GetColor((int)(drawInfo2.position.X + drawPlayer.width * 0.5) / 16, (int)((drawInfo2.position.Y + drawPlayer.height * 0.5) / 16.0));
            SpriteEffects spriteEffects = (drawPlayer.direction==1 ? 0 : SpriteEffects.FlipHorizontally) | (drawPlayer.gravDir==1f ? 0 : SpriteEffects.FlipVertically);
            DrawData value = new DrawData(itemTexture, new Vector2((int)(drawInfo2.itemLocation.X - Main.screenPosition.X), (int)(drawInfo2.itemLocation.Y - Main.screenPosition.Y)), frame, drawPlayer.inventory[drawPlayer.selectedItem].GetAlpha(currentColor), drawPlayer.itemRotation, new Vector2(frame.Width * 0.5f - frame.Width * 0.5f * drawPlayer.direction, frame.Height), drawPlayer.inventory[drawPlayer.selectedItem].scale, spriteEffects, 0);
            Main.playerDrawData.Add(value);
            if(drawPlayer.inventory[drawPlayer.selectedItem].color != default) {
                value = new DrawData(itemTexture, new Vector2((int)(drawInfo2.itemLocation.X - Main.screenPosition.X), (int)(drawInfo2.itemLocation.Y - Main.screenPosition.Y)), frame, drawPlayer.inventory[drawPlayer.selectedItem].GetColor(currentColor), drawPlayer.itemRotation, new Vector2(frame.Width * 0.5f - frame.Width * 0.5f * drawPlayer.direction, frame.Height), drawPlayer.inventory[drawPlayer.selectedItem].scale, spriteEffects, 0);
                Main.playerDrawData.Add(value);
            }
            if(drawPlayer.inventory[drawPlayer.selectedItem].glowMask != -1) {
                value = new DrawData(Main.glowMaskTexture[drawPlayer.inventory[drawPlayer.selectedItem].glowMask], new Vector2((int)(drawInfo2.itemLocation.X - Main.screenPosition.X), (int)(drawInfo2.itemLocation.Y - Main.screenPosition.Y)), frame, aItem.GlowmaskTint??new Color(250, 250, 250, item.alpha), drawPlayer.itemRotation, new Vector2(frame.Width * 0.5f - frame.Width * 0.5f * drawPlayer.direction, frame.Height), drawPlayer.inventory[drawPlayer.selectedItem].scale, spriteEffects, 0);
                Main.playerDrawData.Add(value);
            }
        });
        public static PlayerLayer FelnumGlow = new PlayerLayer("Origins", "FelnumGlow", null, delegate (PlayerDrawInfo drawInfo2) {
            Player drawPlayer = drawInfo2.drawPlayer;
            Vector2 Position;
            Rectangle? Frame;
            Texture2D Texture;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if(drawPlayer.direction == -1) {
                spriteEffects |= SpriteEffects.FlipHorizontally;
            }
            if(drawPlayer.gravDir == -1f) {
                spriteEffects |= SpriteEffects.FlipVertically;
            }
            DrawData item;
            int a = (int)Math.Max(Math.Min((drawPlayer.GetModPlayer<OriginPlayer>().felnumShock*255)/drawPlayer.statLifeMax2, 255), 1);
            if(drawPlayer.head == Origins.FelnumHeadArmorID) {
                Position = new Vector2(drawInfo2.position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f, drawInfo2.position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.headPosition + drawInfo2.headOrigin;
                Frame = new Rectangle?(drawPlayer.bodyFrame);
                Texture = ModContent.GetTexture("Origins/Items/Armor/Felnum/Felnum_Glow_Head");
                item = new DrawData(Texture, Position, Frame, new Color(a, a, a, a), drawPlayer.headRotation, drawInfo2.headOrigin, 1f, spriteEffects, 0);
                item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type);
                Main.playerDrawData.Add(item);
                Texture = ModContent.GetTexture("Origins/Items/Armor/Felnum/Felnum_Glow_Eye");
                item = new DrawData(Texture, Position, Frame, new Color(a, a, a, a), drawPlayer.headRotation, drawInfo2.headOrigin, 1f, spriteEffects, 0);
                item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type);
                Main.playerDrawData.Add(item);
            } else if(drawInfo2.drawHair||drawInfo2.drawAltHair||drawPlayer.head == ArmorIDs.Head.FamiliarWig) {
                Position = new Vector2(drawInfo2.position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f, drawInfo2.position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f) + drawPlayer.headPosition + drawInfo2.headOrigin;
                Frame = new Rectangle?(drawPlayer.bodyFrame);
                Texture = ModContent.GetTexture("Origins/Items/Armor/Felnum/Felnum_Glow_Eye");
                item = new DrawData(Texture, Position, Frame, new Color(a, a, a, a), drawPlayer.headRotation, drawInfo2.headOrigin, 1f, spriteEffects, 0);
                item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[0].type);
                Main.playerDrawData.Add(item);
            }
            if(drawPlayer.body == Origins.FelnumBodyArmorID) {
                Position = new Vector2(((int)(drawInfo2.position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f)), (int)(drawInfo2.position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo2.bodyOrigin;
                Frame = new Rectangle?(drawPlayer.bodyFrame);
                Texture = ModContent.GetTexture("Origins/Items/Armor/Felnum/Felnum_Glow_Arms");
                item = new DrawData(Texture, Position, Frame, new Color(a, a, a, a), drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
                item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type);
                Main.playerDrawData.Add(item);

                Position = new Vector2(((int)(drawInfo2.position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f)), (int)(drawInfo2.position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + drawInfo2.bodyOrigin;
                Frame = new Rectangle?(drawPlayer.bodyFrame);
                Texture = ModContent.GetTexture(drawPlayer.Male ? "Origins/Items/Armor/Felnum/Felnum_Glow_Body" : "Origins/Items/Armor/Felnum/Felnum_Glow_FemaleBody");
                item = new DrawData(Texture, Position, Frame, new Color(a, a, a, a), drawPlayer.bodyRotation, drawInfo2.bodyOrigin, 1f, spriteEffects, 0);
                item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[1].type);
                Main.playerDrawData.Add(item);
            }
            if(drawPlayer.legs == Origins.FelnumLegsArmorID) {
                Position = new Vector2((int)(drawInfo2.position.X - Main.screenPosition.X - drawPlayer.bodyFrame.Width / 2f + drawPlayer.width / 2f), (int)(drawInfo2.position.Y - Main.screenPosition.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.legPosition + drawInfo2.legOrigin;
                Frame = new Rectangle?(drawPlayer.legFrame);
                Texture = ModContent.GetTexture("Origins/Items/Armor/Felnum/Felnum_Glow_Legs");
                item = new DrawData(Texture, Position, Frame, new Color(a, a, a, a), drawPlayer.legRotation, drawInfo2.legOrigin, 1f, spriteEffects, 0);
                item.shader = GameShaders.Armor.GetShaderIdFromItemId(drawPlayer.dye[2].type);
                Main.playerDrawData.Add(item);
            }
        });
    }
}
