using System.Collections.Generic;
using Framework.Constants;
using HermesProxy.World.Enums;

namespace HermesProxy.World.Server.Packets;

public class InitWorldStates : ServerPacket
{
	private struct WorldStateInfo
	{
		public uint VariableID;

		public int Value;

		public WorldStateInfo(uint variableID, int value)
		{
			this.VariableID = variableID;
			this.Value = value;
		}
	}

	public uint ZoneID;

	public uint AreaID;

	public uint MapID;

	private List<WorldStateInfo> Worldstates = new List<WorldStateInfo>();

	public InitWorldStates()
		: base(Opcode.SMSG_INIT_WORLD_STATES, ConnectionType.Instance)
	{
	}

	protected override void Write()
	{
		base._worldPacket.WriteUInt32(this.MapID);
		base._worldPacket.WriteUInt32(this.ZoneID);
		base._worldPacket.WriteUInt32(this.AreaID);
		base._worldPacket.WriteInt32(this.Worldstates.Count);
		foreach (WorldStateInfo wsi in this.Worldstates)
		{
			base._worldPacket.WriteUInt32(wsi.VariableID);
			base._worldPacket.WriteInt32(wsi.Value);
		}
	}

	public void AddState(uint variableID, int value)
	{
		this.Worldstates.Add(new WorldStateInfo(variableID, value));
	}

	public void AddState(uint variableID, bool value)
	{
		this.Worldstates.Add(new WorldStateInfo(variableID, value ? 1 : 0));
	}

	public void AddMissingState(uint variableID, int value)
	{
		foreach (WorldStateInfo worldstate in this.Worldstates)
		{
			if (worldstate.VariableID == variableID)
			{
				return;
			}
		}
		this.Worldstates.Add(new WorldStateInfo(variableID, value));
	}

	public void AddClassicStates()
	{
		if (ModernVersion.ExpansionVersion == 1)
		{
			this.AddMissingState(17101u, 1);
			this.AddMissingState(17222u, 1);
			this.AddMissingState(17223u, 1);
			this.AddMissingState(17224u, 1);
			this.AddMissingState(17225u, 1);
			this.AddMissingState(17226u, 1);
			this.AddMissingState(17227u, 1);
			this.AddMissingState(17228u, 1);
			this.AddMissingState(17229u, 1);
			this.AddMissingState(17230u, 1);
			this.AddMissingState(17231u, 1);
			this.AddMissingState(17232u, 1);
			this.AddMissingState(17233u, 1);
			this.AddMissingState(17234u, 1);
			this.AddMissingState(17424u, 1);
			this.AddMissingState(17430u, 1);
			this.AddMissingState(17478u, 1);
			this.AddMissingState(17560u, 1);
			this.AddMissingState(17640u, 1);
			this.AddMissingState(17641u, 1);
			this.AddMissingState(17642u, 1);
			this.AddMissingState(17643u, 1);
			this.AddMissingState(17647u, 1);
			this.AddMissingState(17648u, 1);
			this.AddMissingState(17687u, 1);
			this.AddMissingState(17697u, 1);
			this.AddMissingState(17698u, 1);
			this.AddMissingState(17704u, 1);
			this.AddMissingState(17705u, 1);
			this.AddMissingState(17706u, 1);
			this.AddMissingState(17707u, 1);
			this.AddMissingState(18261u, 1);
			this.AddMissingState(19361u, 1);
			this.AddMissingState(20281u, 1);
			this.AddMissingState(20470u, 1);
			this.AddMissingState(21260u, 1);
		}
		else
		{
			this.AddMissingState(17223u, 1);
			this.AddMissingState(17647u, 1);
			this.AddMissingState(17648u, 1);
			this.AddMissingState(20445u, 0);
			this.AddMissingState(20446u, 0);
			this.AddMissingState(20447u, 1);
			this.AddMissingState(20487u, 1);
			this.AddMissingState(20488u, 1);
			this.AddMissingState(20489u, 1);
			this.AddMissingState(20491u, 1);
			this.AddMissingState(20492u, 1);
			this.AddMissingState(20493u, 1);
			this.AddMissingState(20494u, 0);
			this.AddMissingState(20495u, 0);
			this.AddMissingState(20496u, 0);
			this.AddMissingState(20497u, 0);
			this.AddMissingState(20518u, 0);
			this.AddMissingState(20560u, 0);
			this.AddMissingState(20562u, 1);
			this.AddMissingState(20563u, 1);
			this.AddMissingState(20567u, 0);
			this.AddMissingState(20738u, 0);
			this.AddMissingState(20882u, 0);
			this.AddMissingState(21125u, 1);
			this.AddMissingState(21126u, 1);
			this.AddMissingState(21195u, 2725);
			this.AddMissingState(21196u, 2542);
			this.AddMissingState(21197u, 2203);
			this.AddMissingState(21198u, 1898);
			this.AddMissingState(21199u, 1453);
			this.AddMissingState(21200u, 2548);
			this.AddMissingState(21201u, 2391);
			this.AddMissingState(21202u, 2086);
			this.AddMissingState(21203u, 1777);
			this.AddMissingState(21204u, 1431);
			this.AddMissingState(21205u, 2354);
			this.AddMissingState(21206u, 2181);
			this.AddMissingState(21207u, 1922);
			this.AddMissingState(21208u, 1686);
			this.AddMissingState(21209u, 1408);
			this.AddMissingState(21238u, 2);
		}
	}
}
