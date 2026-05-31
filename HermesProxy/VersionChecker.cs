using System;
using HermesProxy.Enums;

namespace HermesProxy;

public static class VersionChecker
{
    public static bool IsSupportedLegacyVersion(ClientVersionBuild legacyVersion)
    {
        return legacyVersion switch
        {
            ClientVersionBuild.V1_12_1_5875 or
                ClientVersionBuild.V1_12_2_6005 or
                ClientVersionBuild.V1_12_3_6141 or
                ClientVersionBuild.V2_4_3_8606 or
                ClientVersionBuild.V3_3_5a_12340 => true,
            _ => false
        };
    }

    public static bool IsSupportedModernVersion(ClientVersionBuild modernVersion)
    {
        return modernVersion switch
        {
            ClientVersionBuild.V2_5_2_39570 or
                ClientVersionBuild.V2_5_2_39618 or
                ClientVersionBuild.V1_14_0_39802 or
                ClientVersionBuild.V2_5_2_39926 or
                ClientVersionBuild.V1_14_0_39958 or
                ClientVersionBuild.V2_5_2_40011 or
                ClientVersionBuild.V2_5_2_40045 or
                ClientVersionBuild.V1_14_0_40140 or
                ClientVersionBuild.V1_14_0_40179 or
                ClientVersionBuild.V2_5_2_40203 or
                ClientVersionBuild.V1_14_0_40237 or
                ClientVersionBuild.V2_5_2_40260 or
                ClientVersionBuild.V1_14_0_40347 or
                ClientVersionBuild.V2_5_2_40422 or
                ClientVersionBuild.V1_14_0_40441 or
                ClientVersionBuild.V2_5_2_40488 or
                ClientVersionBuild.V2_5_2_40617 or
                ClientVersionBuild.V1_14_0_40618 or
                ClientVersionBuild.V2_5_2_40892 or
                ClientVersionBuild.V2_5_2_41446 or
                ClientVersionBuild.V2_5_2_41510 or
                ClientVersionBuild.V1_14_1_40487 or
                ClientVersionBuild.V1_14_1_40594 or
                ClientVersionBuild.V1_14_1_40666 or
                ClientVersionBuild.V1_14_1_40688 or
                ClientVersionBuild.V1_14_1_40800 or
                ClientVersionBuild.V1_14_1_40818 or
                ClientVersionBuild.V1_14_1_40926 or
                ClientVersionBuild.V1_14_1_40962 or
                ClientVersionBuild.V1_14_1_41009 or
                ClientVersionBuild.V1_14_1_41030 or
                ClientVersionBuild.V1_14_1_41077 or
                ClientVersionBuild.V1_14_1_41137 or
                ClientVersionBuild.V1_14_1_41243 or
                ClientVersionBuild.V1_14_1_41511 or
                ClientVersionBuild.V1_14_1_41794 or
                ClientVersionBuild.V1_14_1_42032 or
                ClientVersionBuild.V2_5_3_41402 or
                ClientVersionBuild.V2_5_3_41531 or
                ClientVersionBuild.V2_5_3_41750 or
                ClientVersionBuild.V2_5_3_41812 or
                ClientVersionBuild.V1_14_2_41858 or
                ClientVersionBuild.V1_14_2_41959 or
                ClientVersionBuild.V1_14_2_42065 or
                ClientVersionBuild.V1_14_2_42082 or
                ClientVersionBuild.V2_5_3_42083 or
                ClientVersionBuild.V1_14_2_42214 or
                ClientVersionBuild.V2_5_3_42328 or
                ClientVersionBuild.V1_14_2_42597 or
                ClientVersionBuild.V2_5_3_42598 or
                ClientVersionBuild.V3_4_3_54261 => true,
            _ => false
        };
    }

    public static ClientVersionBuild GetBestLegacyVersion(ClientVersionBuild modernVersion)
    {
        var expansionVersion = GetExpansionVersion(modernVersion);
        var result = expansionVersion switch
        {
            1 => ClientVersionBuild.V1_12_1_5875,
            2 => ClientVersionBuild.V2_4_3_8606,
            3 => ClientVersionBuild.V3_3_5a_12340,
            _ => ClientVersionBuild.Zero,
        };
        return result;
    }

    private static byte GetExpansionVersion(ClientVersionBuild version)
    {
        var str = version.ToString();
        str = str.Replace("V", "");
        str = str[..str.IndexOf("_", StringComparison.Ordinal)];
        return (byte)uint.Parse(str);
    }
}