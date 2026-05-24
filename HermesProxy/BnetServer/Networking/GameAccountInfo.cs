using HermesProxy.World;
using HermesProxy.World.Enums;

namespace BNetServer.Networking;

public class GameAccountInfo
{
	public readonly uint Id;

	public readonly string Name;

	public readonly string DisplayName;

	public readonly uint UnbanDate;

	public readonly bool IsBanned;

	public readonly bool IsPermanenetlyBanned;

	public WowGuid128 WoWAccountGuid => WowGuid128.Create(HighGuidType703.WowAccount, Id);

	public GameAccountInfo(string name)
	{
		Id = 1u;
		Name = name;
		UnbanDate = 0u;
		IsPermanenetlyBanned = false;
		IsBanned = IsPermanenetlyBanned || UnbanDate > Time.UnixTime;
		var hashPos = Name.IndexOf('#');
		if (hashPos != -1)
		{
			var name2 = Name;
			var num = hashPos + 1;
			DisplayName = "WoW" + name2.Substring(num, name2.Length - num);
		}
		else
		{
			DisplayName = Name;
		}
	}
}
