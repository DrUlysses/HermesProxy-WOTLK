using System;
using System.Collections.Generic;
using Framework.Constants;
using Framework.Logging;
using HermesProxy.Enums;
using HermesProxy.World.Enums;
using HermesProxy.World.Objects.Version.V3_4_3_54261;

namespace HermesProxy.World.Server.Packets;

public class UpdateObject : ServerPacket
{
	private GameSessionData _gameState;

	public uint NumObjUpdates;

	public ushort MapID;

	public byte[] Data;

	public readonly List<WowGuid128> OutOfRangeGuids = new();

	public readonly List<WowGuid128> DestroyedGuids = new();

	public List<ObjectUpdate> ObjectUpdates = new();

	public UpdateObject(GameSessionData gameState)
		: base(Opcode.SMSG_UPDATE_OBJECT, ConnectionType.Instance)
	{
		_gameState = gameState;
	}

	public static void ResetLoginBuffer(GameSessionData gameState)
	{
		gameState.PendingLoginUpdates = new List<ObjectUpdate>();
		gameState.PendingLoginDestroys = new List<WowGuid128>();
		gameState.PlayerObjectSent = false;
	}

	protected override void Write()
	{
		if (ModernVersion.ExpansionVersion >= 3 && !_gameState.PlayerObjectSent)
		{
			Log.Print(LogType.Debug, $"[UpdateObject] _playerObjectSent=false, checking for player in {ObjectUpdates.Count} updates", "");
		}
		if (ModernVersion.ExpansionVersion >= 3 && !_gameState.PlayerObjectSent)
		{
			if (_gameState.PendingLoginUpdates == null)
			{
				ResetLoginBuffer(_gameState);
			}
			var hasPlayer = false;
			foreach (var update in ObjectUpdates)
			{
				if (update.Guid == _gameState.CurrentPlayerGuid)
				{
					hasPlayer = true;
					break;
				}
			}
			if (!hasPlayer && ObjectUpdates.Count > 0)
			{
				_gameState.PendingLoginUpdates.AddRange(ObjectUpdates);
				_gameState.PendingLoginDestroys.AddRange(DestroyedGuids);
				Log.Print(LogType.Debug, $"[UpdateObject] Buffering {ObjectUpdates.Count} updates (total: {_gameState.PendingLoginUpdates.Count})", "Packets\\UpdatePackets.cs");
				SkipSend = true;
				return;
			}
			if (hasPlayer)
			{
				var merged = new List<ObjectUpdate>(_gameState.PendingLoginUpdates);
				merged.AddRange(ObjectUpdates);
				ObjectUpdates = merged;
				DestroyedGuids.AddRange(_gameState.PendingLoginDestroys);
				_gameState.PlayerObjectSent = true;
				Log.Print(LogType.Debug, $"[UpdateObject] Merged {_gameState.PendingLoginUpdates.Count} buffered + sending {ObjectUpdates.Count} total updates", "Packets\\UpdatePackets.cs");
			}
		}
		MapID = (ushort)_gameState.CurrentMapId.Value;
		if (ObjectUpdateBuilder.DEBUG_SKIP_GAMEOBJECTS)
		{
			ObjectUpdates.RemoveAll(u => u.Guid.GetObjectType() == ObjectType.GameObject);
		}
		// Transport debug logging + selective filter to isolate crash
		if (ModernVersion.ExpansionVersion >= 3)
		{
			foreach (var upd in ObjectUpdates)
			{
				if (upd.Guid.GetHighType() == HighGuidType.Transport || upd.Guid.GetHighType() == HighGuidType.MOTransport)
				{
					var go = upd.GameObjectData;
					var mi = upd.CreateData?.MoveInfo;
					var od = upd.ObjectData;
					Log.Print(LogType.Debug, $"[Transport] {upd.Guid} Type={upd.Type} Entry={od?.EntryID} GO: DisplayID={go?.DisplayID} TypeID={go?.TypeID} State={go?.State} Flags={go?.Flags} Level={go?.Level} MoveInfo: Pos={mi?.Position} Rot={mi?.Rotation} PathTimer={mi?.TransportPathTimer}", "");
				}
			}
			// All old-style Transport GUIDs (TypeID=11 elevators and TypeID=15 boats) crash the
			// WotLK Classic 3.4.3 client - the transport system was completely redesigned in that version.
			ObjectUpdates.RemoveAll(u =>
			{
				if (u.Guid.GetHighType() != HighGuidType.Transport && u.Guid.GetHighType() != HighGuidType.MOTransport)
					return false;
				var entry = u.ObjectData?.EntryID.HasValue == true ? (uint)u.ObjectData.EntryID.Value : 0;
				Log.Print(LogType.Debug, $"[Transport] FILTERED old-style transport entry {entry} — not compatible with 3.4.3 client", "");
				return true;
			});
		}
		NumObjUpdates = (uint)ObjectUpdates.Count;
		_worldPacket.WriteUInt32(NumObjUpdates);
		_worldPacket.WriteUInt16(MapID);
		var buffer = new WorldPacket();
		if (buffer.WriteBit(!OutOfRangeGuids.Empty() || !DestroyedGuids.Empty()))
		{
			buffer.WriteUInt16((ushort)DestroyedGuids.Count);
			buffer.WriteInt32(DestroyedGuids.Count + OutOfRangeGuids.Count);
			foreach (var destroyGuid in DestroyedGuids)
			{
				buffer.WritePackedGuid128(destroyGuid);
			}
			foreach (var outOfRangeGuid in OutOfRangeGuids)
			{
				buffer.WritePackedGuid128(outOfRangeGuid);
			}
		}
		var data = new WorldPacket();
		Log.Print(LogType.Debug, $"[UpdateObject] Writing {ObjectUpdates.Count} updates, {DestroyedGuids.Count} destroyed, {OutOfRangeGuids.Count} OOR, map={MapID}", "UpdateObject.cs");
		foreach (var update2 in ObjectUpdates)
		{
			update2.InitializePlaceholders();
			switch (ModernVersion.GetUpdateFieldsDefiningBuild())
			{
			case ClientVersionBuild.V1_14_0_40237:
			{
				var builder5 = new Objects.Version.V1_14_0_40237.ObjectUpdateBuilder(update2, _gameState);
				builder5.WriteToPacket(data);
				break;
			}
			case ClientVersionBuild.V1_14_1_40688:
			{
				var builder4 = new Objects.Version.V1_14_1_40688.ObjectUpdateBuilder(update2, _gameState);
				builder4.WriteToPacket(data);
				break;
			}
			case ClientVersionBuild.V2_5_2_39570:
			{
				var builder3 = new Objects.Version.V2_5_2_39570.ObjectUpdateBuilder(update2, _gameState);
				builder3.WriteToPacket(data);
				break;
			}
			case ClientVersionBuild.V2_5_3_41750:
			{
				var builder2 = new Objects.Version.V2_5_3_41750.ObjectUpdateBuilder(update2, _gameState);
				builder2.WriteToPacket(data);
				break;
			}
			case ClientVersionBuild.V3_4_3_54261:
			{
				var builder = new ObjectUpdateBuilder(update2, _gameState);
				builder.WriteToPacket(data);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("No object update builder defined for current build.");
			}
		}
		data.FlushBits();
		var bytes = data.GetData();
		Log.Print(LogType.Debug, $"[UpdateObject] Data block size={bytes.Length}, first 64 bytes: {BitConverter.ToString(bytes, 0, Math.Min(64, bytes.Length))}", "UpdateObject.cs");
		buffer.WriteInt32(bytes.Length);
		buffer.WriteBytes(bytes);
		Data = buffer.GetData();
		_worldPacket.WriteBytes(Data);
	}
}
