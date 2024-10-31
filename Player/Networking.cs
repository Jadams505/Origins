﻿using Origins.Buffs;
using Origins.Questing;
using Origins.Tiles;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Origins.PlayerSyncDatas;
using static Origins.PlayerVisualSyncDatas;

namespace Origins {
	public partial class OriginPlayer : ModPlayer {
		bool dummyInitialize = false;
		bool netInitialized = false;
		void NetInit() {
			if (guid == Guid.Empty) guid = Guid.NewGuid();
			if (Main.netMode == NetmodeID.Server) {
				if (!dummyInitialize) {
					Mod.Logger.Info($"FakeInit {netInitialized}, {Player.name}");
					dummyInitialize = true;
					return;
				}
				if (!netInitialized) {
					Mod.Logger.Info($"NetInit {netInitialized}, {Player.name}");
					netInitialized = true;
					ModPacket packet = Mod.GetPacket();
					packet.Write(Origins.NetMessageType.sync_peat);
					packet.Write((short)OriginSystem.Instance.peatSold);
					packet.Send(Player.whoAmI);
					TESystem.SyncAllToPlayer(Player.whoAmI);
					
					packet = Mod.GetPacket();
					packet.Write(Origins.NetMessageType.add_void_lock);
					packet.Write((ushort)OriginSystem.Instance.VoidLocks.Count);
					foreach (var @lock in OriginSystem.Instance.VoidLocks) {
						packet.Write(@lock.Key.X);
						packet.Write(@lock.Key.Y);
						packet.Write(@lock.Value.ToByteArray());
					}
					packet.Send(Player.whoAmI);
					foreach (var quest in Quest_Registry.NetQuests) {
						quest.Sync(Player.whoAmI);
					}
				}
			} else {
				if (!dummyInitialize) {
					Mod.Logger.Info($"Client FakeInit {netInitialized}, {Player.name}");
					dummyInitialize = true;
					ModPacket packet = Origins.instance.GetPacket();
					packet.Write(Origins.NetMessageType.sync_guid);
					packet.Write((byte)Player.whoAmI);
					packet.Write(guid.ToByteArray());
					packet.Send(-1, Main.myPlayer);
					return;
				}
				if (!netInitialized) {
					Mod.Logger.Info($"Client NetInit {netInitialized}, {Player.name}");
					netInitialized = true;
					ModPacket packet = Origins.instance.GetPacket();
					packet.Write(Origins.NetMessageType.sync_guid);
					packet.Write((byte)Player.whoAmI);
					packet.Write(guid.ToByteArray());
					packet.Send(-1, Main.myPlayer);
				}
			}
		}
		public override void CopyClientState(ModPlayer targetCopy) {
			OriginPlayer clone = (OriginPlayer)targetCopy;// shoot this one
			clone.quantumInjectors = quantumInjectors;
			clone.defiledWill = defiledWill;
			if (!Player.HasBuff(Purifying_Buff.ID)) {
				clone.corruptionAssimilation = corruptionAssimilation;
				clone.crimsonAssimilation = crimsonAssimilation;
				clone.defiledAssimilation = defiledAssimilation;
				clone.rivenAssimilation = rivenAssimilation;
			}
			clone.blastSetActive = blastSetActive;
		}
		public override void SendClientChanges(ModPlayer clientPlayer) {
			OriginPlayer clone = (OriginPlayer)clientPlayer;// shoot this one
			PlayerSyncDatas syncDatas = 0;
			PlayerVisualSyncDatas visualSyncDatas = 0;
			if (clone.mojoInjection != mojoInjection) syncDatas |= MojoInjection;
			if (clone.quantumInjectors != quantumInjectors) syncDatas |= QuantumInjectors;
			if (clone.defiledWill != defiledWill) syncDatas |= DefiledWills;

			if ((clone.corruptionAssimilation != corruptionAssimilation ||
				clone.crimsonAssimilation != crimsonAssimilation ||
				clone.defiledAssimilation != defiledAssimilation ||
				clone.rivenAssimilation != rivenAssimilation)
				&& !Player.HasBuff(Purifying_Buff.ID)
				) syncDatas |= Assimilation;

			if (clone.blastSetActive != blastSetActive) visualSyncDatas |= BlastSetActive;

			SyncPlayer(-1, Main.myPlayer, false, syncDatas, visualSyncDatas);
		}
		public override void SyncPlayer(int toWho, int fromWho, bool newPlayer) {
			//return;
			if (Main.netMode == NetmodeID.SinglePlayer) return;
			NetInit();
			SyncPlayer(toWho, fromWho, newPlayer, (PlayerSyncDatas)ushort.MaxValue, (PlayerVisualSyncDatas)ushort.MaxValue);
			if (newPlayer) {
				ModPacket packet = Origins.instance.GetPacket();
				packet.Write(Origins.NetMessageType.sync_guid);
				packet.Write((byte)Player.whoAmI);
				packet.Write(guid.ToByteArray());
				packet.Send(toWho, fromWho);
			}
		}
		public void SyncPlayer(int toWho, int fromWho, bool newPlayer, PlayerSyncDatas syncDatas, PlayerVisualSyncDatas visualSyncDatas) {
			//return;
			if (Main.netMode == NetmodeID.SinglePlayer || (syncDatas == 0 && visualSyncDatas == 0)) return;
			ModPacket packet = Mod.GetPacket();
			packet.Write(Origins.NetMessageType.sync_player);
			packet.Write((byte)Player.whoAmI);
			packet.Write((ushort)syncDatas);
			if (syncDatas.HasFlag(MojoInjection)) packet.Write(mojoInjection);
			if (syncDatas.HasFlag(QuantumInjectors)) packet.Write((byte)quantumInjectors);
			if (syncDatas.HasFlag(DefiledWills)) packet.Write((byte)defiledWill);
			if (syncDatas.HasFlag(Assimilation)) { // by sending it with a precision of 1% we can put all of the assimilations in the 4 bytes one of them would take with full precision with very little inaccuracy
				packet.Write((byte)(corruptionAssimilation * 100));
				packet.Write((byte)(crimsonAssimilation * 100));
				packet.Write((byte)(defiledAssimilation * 100));
				packet.Write((byte)(rivenAssimilation * 100));
			}

			packet.Write((ushort)visualSyncDatas);

			if (visualSyncDatas.HasFlag(BlastSetActive)) packet.Write(blastSetActive);

			packet.Send(toWho, fromWho);
		}
		public void ReceivePlayerSync(BinaryReader reader) {
			PlayerSyncDatas syncDatas = (PlayerSyncDatas)reader.ReadUInt16();
			if (syncDatas.HasFlag(MojoInjection)) mojoInjection = reader.ReadBoolean();
			if (syncDatas.HasFlag(QuantumInjectors)) quantumInjectors = reader.ReadByte();
			if (syncDatas.HasFlag(DefiledWills)) defiledWill = reader.ReadByte();
			if (syncDatas.HasFlag(Assimilation)) {
				corruptionAssimilation = reader.ReadByte() / 100f;
				crimsonAssimilation = reader.ReadByte() / 100f;
				defiledAssimilation = reader.ReadByte() / 100f;
				rivenAssimilation = reader.ReadByte() / 100f;
			}

			PlayerVisualSyncDatas visualSyncDatas = (PlayerVisualSyncDatas)reader.ReadUInt16();
			if (visualSyncDatas.HasFlag(BlastSetActive)) blastSetActive = reader.ReadBoolean();
		}
	}
	[Flags]
	public enum PlayerSyncDatas : ushort {
		Assimilation     = 0b00000001,
		MojoInjection    = 0b00100000,
		DefiledWills     = 0b01000000,
		QuantumInjectors = 0b10000000,
	}
	[Flags]
	public enum PlayerVisualSyncDatas : ushort {
		BlastSetActive   = 0b00000001,
	}
}
