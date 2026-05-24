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
        this.Guid = guid;
        this.IsOutOfRange = isOutOfRange;
    }

    protected override void Write()
    {
        if (ModernVersion.ExpansionVersion >= 3)
        {
            this._worldPacket.WritePackedGuid128(this.Guid);
            this._worldPacket.WriteBit(this.IsOutOfRange);
            this._worldPacket.FlushBits();
        }
        else
        {
            this._worldPacket.WriteGuid(this.Guid.To64());
        }
    }
}
