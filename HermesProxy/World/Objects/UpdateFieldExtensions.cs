using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HermesProxy.Enums;
using HermesProxy.World.Client;

namespace HermesProxy.World.Objects;

public static class UpdateFieldExtensions
{
	private static TypeCode GetTypeCodeOfReturnValue<TK>()
	{
		Type type = typeof(TK);
		TypeCode typeCode = Type.GetTypeCode(type);
		TypeCode typeCode2 = typeCode;
		TypeCode typeCode3 = typeCode2;
		if ((uint)(typeCode3 - 9) <= 1u || (uint)(typeCode3 - 13) <= 1u)
		{
			return typeCode;
		}
		typeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(type));
		TypeCode typeCode4 = typeCode;
		TypeCode typeCode5 = typeCode4;
		if ((uint)(typeCode5 - 9) <= 1u || (uint)(typeCode5 - 13) <= 1u)
		{
			return typeCode;
		}
		throw new ArgumentException("Type must be one of int, uint, float or its nullable counterpart but was " + type.Name);
	}

	public static TK GetValue<T, TK>(this Dictionary<int, UpdateField> dict, T updateField)
	{
		if (dict != null && dict.TryGetValue(LegacyVersion.GetUpdateField(updateField), out var uf))
		{
			switch (GetTypeCodeOfReturnValue<TK>())
			{
			case TypeCode.UInt32:
				return (TK)(object)uf.UInt32Value;
			case TypeCode.Int32:
				return (TK)(object)(int)uf.UInt32Value;
			case TypeCode.Single:
				return (TK)(object)uf.FloatValue;
			case TypeCode.Double:
				return (TK)(object)(double)uf.FloatValue;
			}
		}
		return default(TK);
	}

	public static IEnumerable<TK> GetValue<T, TK>(this Dictionary<int, List<UpdateField>> dict, T updateField)
	{
		if (dict != null && dict.TryGetValue(LegacyVersion.GetUpdateField(updateField), out var ufs))
		{
			switch (GetTypeCodeOfReturnValue<TK>())
			{
			case TypeCode.UInt32:
				return ufs.Select(uf => (TK)(object)uf.UInt32Value);
			case TypeCode.Int32:
				return ufs.Select(uf => (TK)(object)(int)uf.UInt32Value);
			case TypeCode.Single:
				return ufs.Select(uf => (TK)(object)uf.FloatValue);
			case TypeCode.Double:
				return ufs.Select(uf => (TK)(object)(double)uf.FloatValue);
			}
		}
		return Enumerable.Empty<TK>();
	}

	public static TK[] GetArray<T, TK>(this Dictionary<int, UpdateField> dict, T firstUpdateField, int count) where T : Enum
	{
		return dict.GetArray<TK>(LegacyVersion.GetUpdateField(firstUpdateField), count);
	}

	public static TK[] GetArray<TK>(this Dictionary<int, UpdateField> dict, int firstUpdateField, int count)
	{
		TK[] result = new TK[count];
		TypeCode type = GetTypeCodeOfReturnValue<TK>();
		for (int i = 0; i < count; i++)
		{
			if (dict != null && dict.TryGetValue(firstUpdateField + i, out var uf))
			{
				switch (type)
				{
				case TypeCode.UInt32:
					result[i] = (TK)(object)uf.UInt32Value;
					break;
				case TypeCode.Int32:
					result[i] = (TK)(object)(int)uf.UInt32Value;
					break;
				case TypeCode.Single:
					result[i] = (TK)(object)uf.FloatValue;
					break;
				case TypeCode.Double:
					result[i] = (TK)(object)(double)uf.FloatValue;
					break;
				}
			}
		}
		return result;
	}

	public static WowGuid GetGuidValue(this Dictionary<int, UpdateField> UpdateFields, int field)
	{
		if (!LegacyVersion.AddedInVersion(ClientVersionBuild.V6_0_2_19033))
		{
			uint[] parts = UpdateFields.GetArray<uint>(field, 2);
			return new WowGuid64(MathFunctions.MakePair64(parts[0], parts[1]));
		}
		uint[] parts2 = UpdateFields.GetArray<uint>(field, 4);
		return new WowGuid128(MathFunctions.MakePair64(parts2[0], parts2[1]), MathFunctions.MakePair64(parts2[2], parts2[3]));
	}

	public static TK GetEnum<T, TK>(this Dictionary<int, UpdateField> dict, T updateField)
	{
		try
		{
			if (dict != null && dict.TryGetValue(LegacyVersion.GetUpdateField(updateField), out var uf))
			{
				return (TK)Enum.Parse(typeof(TK).GetGenericArguments()[0], uf.UInt32Value.ToString(CultureInfo.InvariantCulture));
			}
		}
		catch (OverflowException)
		{
			return default(TK);
		}
		return default(TK);
	}
}
