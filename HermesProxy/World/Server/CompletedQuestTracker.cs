using System.Collections.Generic;
using HermesProxy.World.Enums;
using HermesProxy.World.Server.Packets;

namespace HermesProxy.World.Server;

public class CompletedQuestTracker
{
	private Dictionary<int, ulong> _cachedQuestCompleted = new Dictionary<int, ulong>();

	public GlobalSessionData Session { get; }

	public CompletedQuestTracker(GlobalSessionData globalSession)
	{
		Session = globalSession;
	}

	public void MarkQuestAsNotCompleted(uint questQuestId)
	{
		Session.AccountMetaDataMgr.MarkQuestAsNotCompleted(Session.GameState.CurrentPlayerInfo.Realm.Name, Session.GameState.CurrentPlayerInfo.Name, questQuestId);
		uint? questBit = GameData.GetUniqueQuestBit(questQuestId);
		if (questBit.HasValue)
		{
			SendSingleUpdateToClient(questBit.Value, isSet: false);
		}
	}

	public void MarkQuestAsCompleted(uint questQuestId)
	{
		Session.AccountMetaDataMgr.MarkQuestAsCompleted(Session.GameState.CurrentPlayerInfo.Realm.Name, Session.GameState.CurrentPlayerInfo.Name, questQuestId);
		uint? questBit = GameData.GetUniqueQuestBit(questQuestId);
		if (questBit.HasValue)
		{
			SendSingleUpdateToClient(questBit.Value, isSet: true);
		}
	}

	public void Reload()
	{
		List<uint> questIds = Session.AccountMetaDataMgr.GetAllCompletedQuests(Session.GameState.CurrentPlayerInfo.Realm.Name, Session.GameState.CurrentPlayerInfo.Name);
		_cachedQuestCompleted = new Dictionary<int, ulong>();
		foreach (uint questId in questIds)
		{
			uint? questBit = GameData.GetUniqueQuestBit(questId);
			if (questBit.HasValue)
			{
				int idx = (int)(questBit - 1 >> 6).Value;
				int bitIdx = (int)((questBit - 1) & 0x3F).Value;
				_cachedQuestCompleted.TryAdd(idx, 0uL);
				_cachedQuestCompleted[idx] |= (ulong)(1L << bitIdx);
			}
		}
	}

	private void SendSingleUpdateToClient(uint questBit, bool isSet)
	{
		int idx = (int)(questBit - 1 >> 6);
		int bitIdx = (int)((questBit - 1) & 0x3F);
		_cachedQuestCompleted.TryAdd(idx, 0uL);
		if (isSet)
		{
			_cachedQuestCompleted[idx] |= (ulong)(1L << bitIdx);
		}
		else
		{
			_cachedQuestCompleted[idx] &= (ulong)(~(1L << bitIdx));
		}
		ObjectUpdate updateData = new ObjectUpdate(Session.GameState.CurrentPlayerGuid, UpdateTypeModern.Values, Session);
		updateData.ActivePlayerData.QuestCompleted[idx] = _cachedQuestCompleted[idx];
		UpdateObject updatePacket = new UpdateObject(Session.GameState);
		updatePacket.ObjectUpdates.Add(updateData);
		Session.WorldClient.SendPacketToClient(updatePacket);
	}

	public void WriteAllCompletedIntoArray(ulong?[] dest)
	{
		foreach (KeyValuePair<int, ulong> kv in _cachedQuestCompleted)
		{
			dest[kv.Key] = kv.Value;
		}
	}
}
