using System.Collections.Generic;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ModernInitialSpells : ServerPacket
{
    public readonly List<uint> Spells = new();
    public ModernInitialSpells() : base(Opcode.SMSG_SEND_KNOWN_SPELLS) { }

    protected override void Write()
    {
        if (ModernVersion.ExpansionVersion >= 3)
        {
            // 3.4.3 SMSG_SEND_KNOWN_SPELLS
            var hasSpells = Spells.Count > 0;
            _worldPacket.WriteBit(hasSpells);
            _worldPacket.FlushBits();

            if (hasSpells)
            {
                _worldPacket.WriteUInt32((uint)Spells.Count);
                foreach (var spell in Spells)
                {
                    _worldPacket.WriteUInt32(spell);
                    _worldPacket.WriteBit(false); // isFavorite
                    _worldPacket.WriteBit(false); // isPassive
                }
            }
            _worldPacket.FlushBits();
            _worldPacket.WriteBit(false); // something else
            _worldPacket.FlushBits();
        }
        else
        {
            // Fallback for older modern clients if any
            _worldPacket.WriteBit(Spells.Count > 0);
            _worldPacket.FlushBits();
            if (Spells.Count > 0)
            {
                _worldPacket.WriteUInt32((uint)Spells.Count);
                foreach (var spell in Spells)
                {
                    _worldPacket.WriteUInt32(spell);
                }
            }
        }
    }
}
