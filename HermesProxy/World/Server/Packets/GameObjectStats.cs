using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class GameObjectStats
{
	public readonly string[] Name = new string[4];

	public string IconName = "";

	public string CastBarCaption = "";

	public string UnkString = "";

	public uint Type;

	public uint DisplayID;

	public readonly int[] Data = new int[35];

	public float Size = 1f;

	public readonly List<uint> QuestItems = new();

	public uint ContentTuningId;

	public uint RequiredLevel;
}
