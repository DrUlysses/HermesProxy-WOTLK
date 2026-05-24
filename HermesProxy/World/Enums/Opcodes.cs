using System;
using HermesProxy.Enums;

namespace HermesProxy.World.Enums;

public static class Opcodes
{
	public static ClientVersionBuild GetOpcodesDefiningBuild(ClientVersionBuild version)
	{
		switch (version)
		{
		case ClientVersionBuild.V1_12_1_5875:
		case ClientVersionBuild.V1_12_2_6005:
		case ClientVersionBuild.V1_12_3_6141:
			return ClientVersionBuild.V1_12_1_5875;
		case ClientVersionBuild.V2_4_3_8606:
			return ClientVersionBuild.V2_4_3_8606;
		case ClientVersionBuild.V3_3_5a_12340:
			return ClientVersionBuild.V3_3_5a_12340;
		case ClientVersionBuild.V2_5_2_39570:
		case ClientVersionBuild.V2_5_2_39618:
		case ClientVersionBuild.V1_14_0_39802:
		case ClientVersionBuild.V2_5_2_39926:
		case ClientVersionBuild.V1_14_0_39958:
		case ClientVersionBuild.V2_5_2_40011:
		case ClientVersionBuild.V2_5_2_40045:
		case ClientVersionBuild.V1_14_0_40140:
		case ClientVersionBuild.V1_14_0_40179:
		case ClientVersionBuild.V2_5_2_40203:
		case ClientVersionBuild.V1_14_0_40237:
		case ClientVersionBuild.V2_5_2_40260:
		case ClientVersionBuild.V1_14_0_40347:
		case ClientVersionBuild.V2_5_2_40422:
		case ClientVersionBuild.V1_14_0_40441:
		case ClientVersionBuild.V2_5_2_40488:
		case ClientVersionBuild.V2_5_2_40617:
		case ClientVersionBuild.V1_14_0_40618:
		case ClientVersionBuild.V2_5_2_40892:
		case ClientVersionBuild.V2_5_2_41446:
		case ClientVersionBuild.V2_5_2_41510:
			return ClientVersionBuild.V2_5_2_39570;
		case ClientVersionBuild.V1_14_1_40487:
		case ClientVersionBuild.V1_14_1_40594:
		case ClientVersionBuild.V1_14_1_40666:
		case ClientVersionBuild.V1_14_1_40688:
		case ClientVersionBuild.V1_14_1_40800:
		case ClientVersionBuild.V1_14_1_40818:
		case ClientVersionBuild.V1_14_1_40926:
		case ClientVersionBuild.V1_14_1_40962:
		case ClientVersionBuild.V1_14_1_41009:
		case ClientVersionBuild.V1_14_1_41030:
		case ClientVersionBuild.V1_14_1_41077:
		case ClientVersionBuild.V1_14_1_41137:
		case ClientVersionBuild.V1_14_1_41243:
		case ClientVersionBuild.V1_14_1_41511:
		case ClientVersionBuild.V1_14_1_41794:
		case ClientVersionBuild.V1_14_1_42032:
			return ClientVersionBuild.V1_14_1_40688;
		case ClientVersionBuild.V2_5_3_41402:
		case ClientVersionBuild.V2_5_3_41531:
		case ClientVersionBuild.V2_5_3_41750:
		case ClientVersionBuild.V2_5_3_41812:
		case ClientVersionBuild.V1_14_2_41858:
		case ClientVersionBuild.V1_14_2_41959:
		case ClientVersionBuild.V1_14_2_42065:
		case ClientVersionBuild.V1_14_2_42082:
		case ClientVersionBuild.V2_5_3_42083:
		case ClientVersionBuild.V1_14_2_42214:
		case ClientVersionBuild.V2_5_3_42328:
		case ClientVersionBuild.V1_14_2_42597:
		case ClientVersionBuild.V2_5_3_42598:
			return ClientVersionBuild.V2_5_3_41750;
		case ClientVersionBuild.V3_4_3_54261:
			return ClientVersionBuild.V3_4_3_54261;
		default:
			return ClientVersionBuild.Zero;
		}
	}

	public static Type GetOpcodesEnumForVersion(ClientVersionBuild version)
	{
		return GetOpcodesDefiningBuild(version) switch
		{
			ClientVersionBuild.V1_12_1_5875 => typeof(V1_12_1_5875.Opcode), 
			ClientVersionBuild.V2_4_3_8606 => typeof(V2_4_3_8606.Opcode), 
			ClientVersionBuild.V3_3_5a_12340 => typeof(V3_3_5a_12340.Opcode), 
			ClientVersionBuild.V2_5_2_39570 => typeof(V2_5_2_39570.Opcode), 
			ClientVersionBuild.V2_5_3_41750 => typeof(V2_5_3_41750.Opcode), 
			ClientVersionBuild.V1_14_1_40688 => typeof(V1_14_1_40688.Opcode), 
			ClientVersionBuild.V3_4_3_54261 => typeof(V3_4_3_54261.Opcode), 
			_ => null, 
		};
	}

	public static uint GetOpcodeValueForVersion(Opcode opcode, ClientVersionBuild version)
	{
		return GetOpcodeValueForVersion(opcode.ToString(), version);
	}

	public static uint GetOpcodeValueForVersion(string opcodeName, ClientVersionBuild version)
	{
		if (Enum.TryParse(GetOpcodesEnumForVersion(version), opcodeName, out object opcode))
		{
			return (uint)opcode;
		}
		return 0u;
	}

	public static string GetOpcodeNameForVersion(uint opcode, ClientVersionBuild version)
	{
		Type enumType = GetOpcodesEnumForVersion(version);
		return Enum.ToObject(enumType, opcode).ToString();
	}

	public static Opcode GetUniversalOpcode(uint opcode, ClientVersionBuild version)
	{
		string name = GetOpcodeNameForVersion(opcode, version);
		return GetUniversalOpcode(name);
	}

	public static Opcode GetUniversalOpcode(string name)
	{
		if (Enum.TryParse(typeof(Opcode), name, out object opcode))
		{
			return (Opcode)opcode;
		}
		return Opcode.UNKNOWN_SMSG;
	}

	private static uint FindOpcodeValueInEnum<T>(string name) where T : Enum
	{
		foreach (object item in Enum.GetValues(typeof(T)))
		{
			if (Enum.GetName(typeof(T), item) == name)
			{
				return (uint)item;
			}
		}
		return 0u;
	}

	private static string FindOpcodeNameInEnum<T>(uint value) where T : Enum
	{
		foreach (object item in Enum.GetValues(typeof(T)))
		{
			if ((uint)item == value)
			{
				return Enum.GetName(typeof(T), item);
			}
		}
		return "UNKNOWN";
	}
}
