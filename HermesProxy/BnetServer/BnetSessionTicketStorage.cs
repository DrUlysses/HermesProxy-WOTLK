using System.Collections.Generic;
using HermesProxy;

namespace BNetServer;

public static class BnetSessionTicketStorage
{
	public static readonly Dictionary<string, GlobalSessionData> SessionsByName = new();

	public static readonly Dictionary<string, GlobalSessionData> SessionsByTicket = new();

	public static readonly Dictionary<ulong, GlobalSessionData> SessionsByKey = new();

	public static void AddNewSessionByName(string name, GlobalSessionData session)
	{
		if (SessionsByName.ContainsKey(name))
		{
			SessionsByName[name].OnDisconnect();
			SessionsByName[name] = session;
		}
		else
		{
			SessionsByName.Add(name, session);
		}
	}

	public static void AddNewSessionByTicket(string loginTicket, GlobalSessionData session)
	{
		if (SessionsByTicket.ContainsKey(loginTicket))
		{
			SessionsByTicket[loginTicket].OnDisconnect();
			SessionsByTicket[loginTicket] = session;
		}
		else
		{
			SessionsByTicket.Add(loginTicket, session);
		}
	}

	public static void AddNewSessionByKey(ulong connectKey, GlobalSessionData session)
	{
		if (SessionsByKey.ContainsKey(connectKey))
		{
			SessionsByKey[connectKey].OnDisconnect();
			SessionsByKey[connectKey] = session;
		}
		else
		{
			SessionsByKey.Add(connectKey, session);
		}
	}
}
