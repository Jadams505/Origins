﻿using Origins.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Origins.Tiles {
    public abstract class OriginTile : ModTile {
        public static List<OriginTile> IDs { get; internal set; }
        public ushort mergeID;
        public override bool Autoload(ref string name, ref string texture) {
            if(IDs!=null) {
                IDs.Add(this);
            } else {
                IDs = new List<OriginTile>() {this};
            }
            mergeID = Type;
            return true;
        }
    }
    /// <summary>
    /// contains defiled wastelands spread code
    /// </summary>
    public abstract class DefiledTile : OriginTile {
        public override void RandomUpdate(int i, int j) {
            if(!Main.hardMode)return;
            WeightedRandom<(int, int)> rand = new WeightedRandom<(int, int)>();
            Tile current;
            for(int y = -3; y < 4; y++) {
                for(int x = -3; x < 4; x++) {
                    current = Main.tile[i+x, j+y];
                    if(OriginWorld.ConvertTileWeak(ref current.type, OriginWorld.evil_wastelands, false)) {
                        if(Main.tile[i+x, j+y-1].type!=TileID.Sunflower)rand.Add((i+x,j+y));
                    }
                }
            }
            if(rand.elements.Count>0) {
                (int x, int y) pos = rand.Get();
                OriginWorld.ConvertTileWeak(ref Main.tile[pos.x, pos.y].type, OriginWorld.evil_wastelands);
				WorldGen.SquareTileFrame(pos.x, pos.y);
				NetMessage.SendTileSquare(-1, pos.x, pos.y, 1);
            }
        }
    }
    /// <summary>
    /// contains riven hive spread code
    /// </summary>
    public abstract class RivenTile : OriginTile {
        public override void RandomUpdate(int i, int j) {
            if(!Main.hardMode)return;
            WeightedRandom<(int, int)> rand = new WeightedRandom<(int, int)>();
            Tile current;
            for(int y = -3; y < 4; y++) {
                for(int x = -3; x < 4; x++) {
                    current = Main.tile[i+x, j+y];
                    if(OriginWorld.ConvertTileWeak(ref current.type, OriginWorld.evil_riven, false)) {
                        if(Main.tile[i+x, j+y-1].type!=TileID.Sunflower)rand.Add((i+x,j+y));
                    }
                }
            }
            if(rand.elements.Count>0) {
                (int x, int y) pos = rand.Get();
                OriginWorld.ConvertTileWeak(ref Main.tile[pos.x, pos.y].type, OriginWorld.evil_riven);
				WorldGen.SquareTileFrame(pos.x, pos.y);
				NetMessage.SendTileSquare(-1, pos.x, pos.y, 1);
            }
        }
    }
}
