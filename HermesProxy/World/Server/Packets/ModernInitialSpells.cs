using System.Collections.Generic;
using HermesProxy.World;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class ModernInitialSpells : ServerPacket
{
    public List<uint> Spells = new List<uint>();
    public ModernInitialSpells() : base(Opcode.SMSG_SEND_KNOWN_SPELLS) { }

    protected override void Write()
    {
        if (ModernVersion.ExpansionVersion >= 3)
        {
            // 3.4.3 SMSG_SEND_KNOWN_SPELLS
            bool hasSpells = this.Spells.Count > 0;
            base._worldPacket.WriteBit(hasSpells);
            base._worldPacket.FlushBits();

            if (hasSpells)
            {
                base._worldPacket.WriteUInt32((uint)this.Spells.Count);
                foreach (uint spell in this.Spells)
                {
                    base._worldPacket.WriteUInt32(spell);
                    base._worldPacket.WriteBit(false); // isFavorite
                    base._worldPacket.WriteBit(false); // isPassive
                }
            }
            base._worldPacket.FlushBits();
            base._worldPacket.WriteBit(false); // something else
            base._worldPacket.FlushBits();
        }
        else
        {
            // Fallback for older modern clients if any
            base._worldPacket.WriteBit(this.Spells.Count > 0);
            base._worldPacket.FlushBits();
            if (this.Spells.Count > 0)
            {
                base._worldPacket.WriteUInt32((uint)this.Spells.Count);
                foreach (uint spell in this.Spells)
                {
                    base._worldPacket.WriteUInt32(spell);
                }
            }
        }
    }
}
