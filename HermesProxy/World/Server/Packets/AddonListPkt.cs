using System.IO;
using Ionic.Zlib;

namespace HermesProxy.World.Server.Packets;

public class AddonListPkt : ClientPacket
{
    public uint AddonCount;
    public AddonListPkt(WorldPacket packet) : base(packet) { }
    public override void Read()
    {
        try {
            uint compressedSize = _worldPacket.ReadUInt32();
            if (compressedSize > 0)
            {
                byte[] compressed = _worldPacket.ReadBytes(compressedSize);
                using (var ms = new MemoryStream(compressed))
                using (var zlib = new ZlibStream(ms, CompressionMode.Decompress))
                using (var outMs = new MemoryStream())
                {
                    zlib.CopyTo(outMs);
                    byte[] decompressed = outMs.ToArray();
                    using (var reader = new WorldPacket(decompressed))
                    {
                        AddonCount = reader.ReadUInt32();
                    }
                }
            }
        } catch { }
    }
}
