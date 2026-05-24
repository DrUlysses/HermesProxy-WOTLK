using System;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public struct PartyPlayerInfo
{
	public WowGuid128 GUID;

	public string Name;

	public string VoiceStateID;

	public Class ClassId;

	public GroupMemberOnlineStatus Status;

	public byte Subgroup;

	public GroupMemberFlags Flags;

	public byte RolesAssigned;

    public byte FactionGroup; // Unhandled, not sure what it's for

    public bool FromSocialQueue;

	public bool VoiceChatSilenced;

    public bool Connected;

    public void Write(WorldPacket data)
    {
        data.WriteBits(Name.GetByteCount(), 6);
        data.WriteBits(VoiceStateID.GetByteCount() + 1, 6);
        if (ModernVersion.ExpansionVersion == 3)
        {
            bool isConnected = Connected || Status != GroupMemberOnlineStatus.Offline;
            data.WriteBit(isConnected);
            data.WriteBit(VoiceChatSilenced);
            data.WriteBit(FromSocialQueue);
        }
        else
        {
            data.WriteBit(FromSocialQueue);
            data.WriteBit(VoiceChatSilenced);
        }
        data.WritePackedGuid128(GUID);
        if (ModernVersion.ExpansionVersion < 3)
        {
            data.WriteUInt8((byte)Status);
        }
        data.WriteUInt8(Subgroup);
        data.WriteUInt8((byte)Flags);
        data.WriteUInt8(RolesAssigned);
        data.WriteUInt8((byte)ClassId);
        if (ModernVersion.ExpansionVersion == 3)
        {
            data.WriteUInt8(FactionGroup);
        }
        data.WriteString(Name);
        if (!VoiceStateID.IsEmpty())
        {
            data.WriteString(VoiceStateID);
        }
    }
}
