using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class GuildPermissionsQueryResults : ServerPacket
{
    public uint GuildID;
    public uint RankID;
    public uint Flags;
    public uint WithdrawGoldLimit;
    public uint RemainingWithdrawGoldLimit;
    public readonly uint[] TabPermissions = new uint[6];

    public GuildPermissionsQueryResults()
        : base(Opcode.SMSG_GUILD_PERMISSIONS_QUERY_RESULTS, ConnectionType.Instance)
    {
    }

    protected override void Write()
    {
        _worldPacket.WriteUInt32(GuildID);
        _worldPacket.WriteUInt32(RankID);
        _worldPacket.WriteUInt32(Flags);
        _worldPacket.WriteUInt32(WithdrawGoldLimit);
        _worldPacket.WriteUInt32(RemainingWithdrawGoldLimit);
        for (var i = 0; i < 6; i++)
        {
            _worldPacket.WriteUInt32(TabPermissions[i]);
        }
    }
}
