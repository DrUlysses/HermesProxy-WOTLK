using Framework.Constants;
using Framework.Logging;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

internal class CastFailed : ServerPacket
{
	public WowGuid128? CastID;

	public uint? SpellID;

	public uint Reason;

	public int FailedArg1 = -1;

	public int FailedArg2 = -1;

	public uint? SpellXSpellVisualID;

	public CastFailed()
		: base(Opcode.SMSG_CAST_FAILED, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		if (CastID == null)
		{
			Log.Print(LogType.Debug, $"CastID is null for CastFailed packet");
			return;
		}
		if (SpellID == null)
		{
			Log.Print(LogType.Debug, $"SpellID is null for CastFailed packet with CastID: {CastID}");
			return;
		}
		if (SpellXSpellVisualID == null)
		{
			Log.Print(LogType.Debug, $"SpellXSpellVisualID is null for CastFailed packet with CastID: {CastID}");
			return;
		}
		_worldPacket.WritePackedGuid128(CastID);
		_worldPacket.WriteUInt32(SpellID.Value);
		_worldPacket.WriteUInt32(SpellXSpellVisualID.Value);
		_worldPacket.WriteUInt32(Reason);
		_worldPacket.WriteInt32(FailedArg1);
		_worldPacket.WriteInt32(FailedArg2);
	}
}
