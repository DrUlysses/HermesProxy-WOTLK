/*
 * Copyright (C) 2012-2020 CypherCore <http://github.com/CypherCore>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Bgs.Protocol;
using Bgs.Protocol.GameUtilities.V1;
using Framework;
using Framework.Constants;
using Framework.Logging;
using Framework.Realm;
using Framework.Serialization;
using Framework.Util;
using Framework.Web;
using Google.Protobuf;
using HermesProxy;
using HermesProxy.Auth;
using HermesProxy.World;
using Address = Framework.Web.Address;

public class RealmManager
{
    const int placeholderRegion = 1;
    const int placeholderBattlegroup = 1;

    public RealmManager()
    {
        LoadBuildInfo();

        var subRegion = new RealmId(placeholderRegion, placeholderBattlegroup, 0).GetAddressString();
        if (!_subRegions.Contains(subRegion))
            _subRegions.Add(subRegion);
    }

    void LoadBuildInfo()
    {
        var build = new RealmBuildInfo
        {
            MajorVersion = ModernVersion.ExpansionVersion,
            MinorVersion = ModernVersion.MajorVersion,
            BugfixVersion = ModernVersion.MinorVersion
        };

        var hotfixVersion = "";
        if (!hotfixVersion.IsEmpty() && hotfixVersion.Length < build.HotfixVersion.Length)
            build.HotfixVersion = hotfixVersion.ToCharArray();

        build.Build = (uint)Settings.ClientBuild;

        build.FallbackStaticSeed = Settings.ClientSeed;
        build.BuildSeeds = GameData.BuildAuthSeeds.GetValueOrDefault(build.Build, new Dictionary<string, byte[]>());

        _builds.Add(build);
    }

    void UpdateRealm(Realm realm)
    {
        var oldRealm = _realms.LookupByKey(realm.Id);
        if (oldRealm != null && oldRealm == realm)
                return;

        _realms[realm.Id] = realm;
    }

    public void AddRealm(uint id, string name, string externalAddress, ushort port, RealmType type, RealmFlags flags, byte characterCount, byte timezone, float populationLevel)
    {
        var existingRealms = new Dictionary<RealmId, string>();
        foreach (var p in _realms)
            existingRealms[p.Key] = p.Value.Name;

        var realm = new Realm
        {
            Name = name,
            ExternalAddress = externalAddress,
            Port = port
        };

        var realmType = type;
        if (realmType == RealmType.FFAPVP)
            realmType = RealmType.PVP;
        if (realmType >= RealmType.MaxType)
            realmType = RealmType.Normal;

        realm.Type = (byte)realmType;
        realm.Flags = flags;
        realm.CharacterCount = characterCount;
        realm.Timezone = timezone;
        realm.PopulationLevel = populationLevel;
        realm.Build = (uint)Settings.ClientBuild;

        realm.Id = new RealmId(placeholderRegion, placeholderBattlegroup, id);
        UpdateRealm(realm);

        if (!existingRealms.ContainsKey(realm.Id))
            Log.Print(LogType.Server, $"Added realm \"{realm.Name}\" at {realm.ExternalAddress}:{realm.Port}");
        else
            Log.Print(LogType.Server, $"Updating realm \"{realm.Name}\" at { realm.ExternalAddress}:{realm.Port}");

        existingRealms.Remove(realm.Id);
    }

    public void UpdateRealms(List<RealmInfo> authRealmList)
    {
        _realms.Clear();

        // Circle through results and add them to the realm map
        if (authRealmList != null)
        {
            foreach (var authRealmEntry in authRealmList)
            {
                AddRealm(authRealmEntry.ID, authRealmEntry.Name, authRealmEntry.Address, authRealmEntry.Port, authRealmEntry.Type, authRealmEntry.Flags, authRealmEntry.CharacterCount, authRealmEntry.Timezone, authRealmEntry.Population);
            }
        }
    }

    /// <summary>
    /// Prints the <see cref="RealmInfo"/> from the <see cref="Realms"/> instance.
    /// </summary>
    public void PrintRealmList()
    {
        if (_realms.Count == 0)
            return;

        Log.Print(LogType.Debug, "");
        Log.Print(LogType.Debug, $"{"Type",-5} {"Type",-5} {"Locked",-8} {"Flags",-10} {"Name",-15} {"Address",-15} {"Port",-10} {"Build",-10}");

        foreach (var realm in _realms)
            Log.Print(LogType.Debug, realm.ToString());

        Log.Print(LogType.Debug,"");
    }

    public Realm? GetRealm(RealmId id)
    {
        return _realms.LookupByKey(id);
    }

    public RealmBuildInfo GetBuildInfo(uint build)
    {
        foreach (var clientBuild in _builds)
            if (clientBuild.Build == build)
                return clientBuild;

        return null;
    }

    public uint GetMinorMajorBugfixVersionForBuild(uint build)
    {
        var buildInfo = _builds.FirstOrDefault(p => p.Build < build);
        return buildInfo != null ? buildInfo.MajorVersion * 10000 + buildInfo.MinorVersion * 100 + buildInfo.BugfixVersion : 0;
    }

    public void WriteSubRegions(GetAllValuesForAttributeResponse response)
    {
        foreach (var subRegion in GetSubRegions())
        {
            var variant = new Variant
            {
                StringValue = subRegion
            };
            response.AttributeValue.Add(variant);
        }
    }

    public byte[] GetCompressdRealmEntryJSON(Realm realm, uint build)
    {
        var compressed = new byte[0];
        if (realm != null)
        {
            if (!realm.Flags.HasAnyFlag(RealmFlags.Offline) && realm.Build == build)
            {
                var realmEntry = new RealmEntry
                {
                    WowRealmAddress = (int)realm.Id.GetAddress(),
                    CfgTimezonesID = 1,
                    PopulationState = Math.Max((int)realm.PopulationLevel, 1),
                    CfgCategoriesID = realm.Timezone
                };

                var version = new ClientVersion();
                var buildInfo = GetBuildInfo(realm.Build);
                if (buildInfo != null)
                {
                    version.Major = (int)buildInfo.MajorVersion;
                    version.Minor = (int)buildInfo.MinorVersion;
                    version.Revision = (int)buildInfo.BugfixVersion;
                    version.Build = (int)buildInfo.Build;
                }
                else
                {
                    version.Major = 6;
                    version.Minor = 2;
                    version.Revision = 4;
                    version.Build = (int)realm.Build;
                }
                realmEntry.Version = version;

                realmEntry.CfgRealmsID = (int)realm.Id.Index;
                realmEntry.Flags = (int)realm.Flags;
                realmEntry.Name = realm.Name;
                realmEntry.CfgConfigsID = (int)realm.GetConfigId();
                realmEntry.CfgLanguagesID = 1;

                compressed = Json.Deflate("JamJSONRealmEntry", realmEntry);
            }
        }

        return compressed;
    }

    public byte[] GetRealmList(uint build, string subRegion)
    {
        var realmList = new RealmListUpdates();
        foreach (var realm in _realms)
        {
            if (realm.Value.Id.GetSubRegionAddress() != subRegion)
                continue;

            var flag = realm.Value.Flags;
            if (realm.Value.Build != build)
                flag |= RealmFlags.VersionMismatch;

            var realmListUpdate = new RealmListUpdate
            {
                Update =
                {
                    WowRealmAddress = (int)realm.Value.Id.GetAddress(),
                    CfgTimezonesID = 1,
                    PopulationState = realm.Value.Flags.HasAnyFlag(RealmFlags.Offline) ? 0 : Math.Max((int)realm.Value.PopulationLevel, 1),
                    CfgCategoriesID = realm.Value.Timezone
                }
            };

            var buildInfo = GetBuildInfo(realm.Value.Build);
            if (buildInfo != null)
            {
                realmListUpdate.Update.Version.Major = (int)buildInfo.MajorVersion;
                realmListUpdate.Update.Version.Minor = (int)buildInfo.MinorVersion;
                realmListUpdate.Update.Version.Revision = (int)buildInfo.BugfixVersion;
                realmListUpdate.Update.Version.Build = (int)buildInfo.Build;
            }
            else
            {
                realmListUpdate.Update.Version.Major = 7;
                realmListUpdate.Update.Version.Minor = 1;
                realmListUpdate.Update.Version.Revision = 0;
                realmListUpdate.Update.Version.Build = (int)realm.Value.Build;
            }

            realmListUpdate.Update.CfgRealmsID = (int)realm.Value.Id.Index;
            realmListUpdate.Update.Flags = (int)flag;
            realmListUpdate.Update.Name = realm.Value.Name;
            realmListUpdate.Update.CfgConfigsID = (int)realm.Value.GetConfigId();
            realmListUpdate.Update.CfgLanguagesID = 1;

            realmListUpdate.Deleting = false;

            realmList.Updates.Add(realmListUpdate);
        }

        return Json.Deflate("JSONRealmListUpdates", realmList);
    }

    public BattlenetRpcErrorCode JoinRealm(GlobalSessionData globalSession, uint realmAddress, uint build, IPAddress clientAddress, byte[] clientSecret, string accountName, ClientResponse response)
    {
        globalSession.RealmId = new RealmId(realmAddress);
        Log.Print(LogType.Debug, $"JoinRealm: realmAddress=0x{realmAddress:X8}, decoded={globalSession.RealmId}, realms count={_realms.Count}");
        foreach (var r in _realms)
            Log.Print(LogType.Debug, $"  Available realm: {r.Key} -> {r.Value.Name}");
        var realm = GetRealm(globalSession.RealmId);
        if (realm != null)
        {
            if (realm.Flags.HasAnyFlag(RealmFlags.Offline) || realm.Build != build)
                return BattlenetRpcErrorCode.UserServerNotPermittedOnRealm;

            var serverAddresses = new RealmListServerIPAddresses();
            var addressFamily = new AddressFamily
            {
                Id = 1
            };

            var address = new Address
            {
                Ip = realm.GetAddressForClient(clientAddress).Address.ToString(),
                Port = Settings.RealmPort
            };
            addressFamily.Addresses.Add(address);
            serverAddresses.Families.Add(addressFamily);

            var compressed = Json.Deflate("JSONRealmListServerIPAddresses", serverAddresses);

            var serverSecret = new byte[0].GenerateRandomKey(32);
            var keyData = clientSecret.ToArray().Combine(serverSecret);

            globalSession.SessionKey = keyData;

            response.Attribute.AddBlob("Param_RealmJoinTicket", ByteString.CopyFrom(accountName, Encoding.UTF8));
            response.Attribute.AddBlob("Param_ServerAddresses", ByteString.CopyFrom(compressed));
            response.Attribute.AddBlob("Param_JoinSecret", ByteString.CopyFrom(serverSecret));

            return BattlenetRpcErrorCode.Ok;
        }

        return BattlenetRpcErrorCode.UtilServerUnknownRealm;
    }

    public ICollection<Realm> GetRealms() { return _realms.Values; }
    List<string> GetSubRegions() { return _subRegions; }

    List<RealmBuildInfo> _builds = new();
    ConcurrentDictionary<RealmId, Realm> _realms = new();
    List<string> _subRegions = new();
}

public class RealmBuildInfo
{
    public uint Build;
    public uint MajorVersion;
    public uint MinorVersion;
    public uint BugfixVersion;
    public char[] HotfixVersion = new char[4];
    public byte[] FallbackStaticSeed = new byte[16];
    public Dictionary<string/*Platform*/, byte[] /*Seed*/> BuildSeeds = new();
}
