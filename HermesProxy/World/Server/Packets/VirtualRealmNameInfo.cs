using System;

namespace HermesProxy.World.Server.Packets;

internal struct VirtualRealmNameInfo
{
	public readonly bool IsLocal;

	public readonly bool IsInternalRealm;

	public readonly string RealmNameActual;

	public readonly string RealmNameNormalized;

	public VirtualRealmNameInfo(bool isHomeRealm, bool isInternalRealm, string realmNameActual, string realmNameNormalized)
	{
		IsLocal = isHomeRealm;
		IsInternalRealm = isInternalRealm;
		RealmNameActual = realmNameActual;
		RealmNameNormalized = realmNameNormalized;
	}

	public void Write(WorldPacket data)
	{
		data.WriteBit(IsLocal);
		data.WriteBit(IsInternalRealm);
		data.WriteBits(RealmNameActual.GetByteCount(), 8);
		data.WriteBits(RealmNameNormalized.GetByteCount(), 8);
		data.FlushBits();
		data.WriteString(RealmNameActual);
		data.WriteString(RealmNameNormalized);
	}
}
