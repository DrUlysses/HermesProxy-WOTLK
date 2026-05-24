using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AddonInfoPacket : ServerPacket
{
    public uint AddonCount;
    public AddonInfoPacket(uint count = 0)
        : base(Opcode.SMSG_ADDON_INFO, ConnectionType.Realm)
    {
        this.AddonCount = count;
    }

    protected override void Write()
    {
        if (ModernVersion.ExpansionVersion >= 3)
        {
            // SMSG_ADDON_INFO for 3.4.x
            bool hasAddons = this.AddonCount > 0;
            this._worldPacket.WriteBit(hasAddons);
            this._worldPacket.WriteBit(false); // Unk bit
            this._worldPacket.FlushBits();
            
            if (hasAddons)
            {
                this._worldPacket.WriteUInt32(this.AddonCount);
                for (int i = 0; i < this.AddonCount; i++)
                {
                    this._worldPacket.WriteBits(2, 8); // State: Authenticated
                    this._worldPacket.WriteBit(false); // Has public key
                    this._worldPacket.WriteBit(false); // Has signature
                }
            }
            this._worldPacket.FlushBits();
            this._worldPacket.WriteBit(false); // Has some list?
            this._worldPacket.FlushBits();
        }
        else
        {
            // Legacy 3.3.5 structure is different.
        }
    }
}
