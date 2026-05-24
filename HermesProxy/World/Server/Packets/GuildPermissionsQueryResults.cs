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
    public uint[] TabPermissions = new uint[6];

    public GuildPermissionsQueryResults()
        : base(Opcode.SMSG_GUILD_PERMISSIONS_QUERY_RESULTS, ConnectionType.Instance)
    {
    }

    protected override void Write()
    {
        base._worldPacket.WriteUInt32(this.GuildID);
        base._worldPacket.WriteUInt32(this.RankID);
        base._worldPacket.WriteUInt32(this.Flags);
        base._worldPacket.WriteUInt32(this.WithdrawGoldLimit);
        base._worldPacket.WriteUInt32(this.RemainingWithdrawGoldLimit);
        for (int i = 0; i < 6; i++)
        {
            base._worldPacket.WriteUInt32(this.TabPermissions[i]);
        }
    }
}
