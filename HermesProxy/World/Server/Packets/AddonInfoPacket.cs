using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class AddonInfoPacket : ServerPacket
{
    public readonly uint AddonCount;
    public AddonInfoPacket(uint count = 0)
        : base(Opcode.SMSG_ADDON_INFO, ConnectionType.Realm)
    {
        AddonCount = count;
    }

    protected override void Write()
    {
        if (ModernVersion.ExpansionVersion >= 3)
        {
            // SMSG_ADDON_INFO for 3.4.x
            var hasAddons = AddonCount > 0;
            _worldPacket.WriteBit(hasAddons);
            _worldPacket.WriteBit(false); // Unk bit
            _worldPacket.FlushBits();
            
            if (hasAddons)
            {
                _worldPacket.WriteUInt32(AddonCount);
                for (var i = 0; i < AddonCount; i++)
                {
                    _worldPacket.WriteBits(2, 8); // State: Authenticated
                    _worldPacket.WriteBit(false); // Has public key
                    _worldPacket.WriteBit(false); // Has signature
                }
            }
            _worldPacket.FlushBits();
            _worldPacket.WriteBit(false); // Has some list?
            _worldPacket.FlushBits();
        }
        // Legacy 3.3.5 structure is different.
    }
}
