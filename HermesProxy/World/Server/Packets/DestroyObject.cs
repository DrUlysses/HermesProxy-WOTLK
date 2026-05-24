using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class DestroyObject : ServerPacket
{
    public WowGuid128 Guid;
    public bool IsOutOfRange;

    public DestroyObject(WowGuid128 guid, bool isOutOfRange = false)
        : base(Opcode.SMSG_DESTROY_OBJECT, ConnectionType.Instance)
    {
        Guid = guid;
        IsOutOfRange = isOutOfRange;
    }

    protected override void Write()
    {
        if (ModernVersion.ExpansionVersion >= 3)
        {
            _worldPacket.WritePackedGuid128(Guid);
            _worldPacket.WriteBit(IsOutOfRange);
            _worldPacket.FlushBits();
        }
        else
        {
            _worldPacket.WriteGuid(Guid.To64());
        }
    }
}
