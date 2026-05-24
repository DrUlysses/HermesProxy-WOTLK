using System.Collections.Generic;

namespace HermesProxy.World.Server.Packets;

public class ClassFilter
{
	public int ItemClass;

	public readonly List<SubClassFilter> SubClassFilters = new();
}
