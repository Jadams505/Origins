﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using System;
using System.Linq;
using static Tyfyter.Utils.UITools;

namespace Origins.UI {
    public class Eyndum_Core_UI : SingleItemSlotUI {
        public bool hasSetBonus = true;
        public override void OnInitialize() {
            float slotX = Main.screenWidth - 64 - 28;
            float slotY = (174 + (!Main.mapFullscreen && Main.mapStyle == 1 ? 256 : 0)) + (1 * 56) * 0.85f;

            // Ensures that the player's core slot item is not null, then adds the slot
            ref Ref<Item> item = ref Main.LocalPlayer.GetModPlayer<OriginPlayer>().eyndumCore;
            if (item is null) {
                item = new Ref<Item>(new Item());
                item.Value.SetDefaults(0);
            }
            SetItemSlot(item, new Vector2(slotX, slotY), slotColor:new Color(50, 106, 46, 220), extraTextures: (Origins.eyndumCoreUITexture, new Color(50, 106, 46, 160)));
        }
        public override void Update(GameTime gameTime) {
            itemSlot.slotSourceMissing = !hasSetBonus;
            base.Update(gameTime);
            hasSetBonus = true;
        }
    }
}