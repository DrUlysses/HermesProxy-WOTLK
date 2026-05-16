# Packet Structures Reference

This file documents the packet structure (fields read/written) for each handled opcode.

Total handled opcodes: 762
Total ServerPacket classes: 347

## Table of Contents

1. [CMSG Handlers (Client->Legacy Server reads)](#cmsg-handlers)
2. [SMSG Handlers (Legacy Server->Modern Client writes)](#smsg-handlers)
3. [MSG Handlers (Bidirectional)](#msg-handlers)
4. [ServerPacket Write Structures (Modern Client format)](#serverpacket-structures)

## CMSG Handlers

### CMSG_ACCEPT_GUILD_INVITE

- Legacy value: 132 (0x0084)
- Modern value: 13822 (0x35FE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1494

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ACCEPT_GUILD_INVITE);
this.SendPacketToServer(packet);
}
```

---

### CMSG_ACCEPT_TRADE

- Legacy value: 282 (0x011A)
- Modern value: 12634 (0x315A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4385
- Fields:
  - `WriteUInt32(trade.StateIndex)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ACCEPT_TRADE);
packet.WriteUInt32(trade.StateIndex);
this.SendPacketToServer(packet);
}
```

---

### CMSG_ACTIVATE_TAXI

- Legacy value: 429 (0x01AD)
- Modern value: 13483 (0x34AB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4234
- Fields:
  - `WriteGuid(taxi.FlightMaster.To64()`
  - `WriteUInt32(this.GetSession()`
  - `WriteUInt32(taxi.Node)`
  - `WriteGuid(taxi.FlightMaster.To64()`
  - `WriteUInt32(0u)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(itr)`

```csharp
{
if (this.TaxiPathExist(this.GetSession().GameState.CurrentTaxiNode, taxi.Node))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ACTIVATE_TAXI);
packet.WriteGuid(taxi.FlightMaster.To64());
packet.WriteUInt32(this.GetSession().GameState.CurrentTaxiNode);
packet.WriteUInt32(taxi.Node);
this.SendPacketToServer(packet);
}
else
{
HashSet<uint> path = this.GetTaxiPath(this.GetSession().GameState.CurrentTaxiNode, taxi.Node, this.GetSession().GameState.UsableTaxiNodes);
if (path.Count <= 1)
{
return;
}
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_ACTIVATE_TAXI_EXPRESS);
packet2.WriteGuid(taxi.FlightMaster.To64());
packet2.WriteUInt32(0u);
packet2.WriteUInt32((uint)path.Count);
foreach (uint itr in path)
{
packet2.WriteUInt32(itr);
}
this.SendPacketToServer(packet2);
}
this.GetSession().GameState.IsWaitingForTaxiStart = true;
}
```

---

### CMSG_ADD_FRIEND

- Legacy value: 105 (0x0069)
- Modern value: 14040 (0x36D8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3697
- Fields:
  - `WriteCString(friend.Name)`
  - `WriteCString(friend.Note)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ADD_FRIEND);
packet.WriteCString(friend.Name);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteCString(friend.Note);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_ADD_IGNORE

- Legacy value: 108 (0x006C)
- Modern value: 14044 (0x36DC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3709
- Fields:
  - `WriteCString(ignore.Name)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ADD_IGNORE);
packet.WriteCString(ignore.Name);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AREA_SPIRIT_HEALER_QUERY

- Legacy value: 738 (0x02E2)
- Modern value: 13488 (0x34B0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3057
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_AREA_SPIRIT_HEALER_QUEUE

- Legacy value: 739 (0x02E3)
- Modern value: 13489 (0x34B1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3058
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_AREA_TRIGGER

- Legacy value: 180 (0x00B4)
- Modern value: 12758 (0x31D6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2406
- Fields:
  - `WriteUInt32(at.AreaTriggerID)`

```csharp
{
if (at.Entered)
{
this.GetSession().GameState.LastEnteredAreaTrigger = at.AreaTriggerID;
WorldPacket packet = new WorldPacket(Opcode.CMSG_AREA_TRIGGER);
packet.WriteUInt32(at.AreaTriggerID);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_ARENA_TEAM_ACCEPT

- Legacy value: 849 (0x0351)
- Modern value: 14009 (0x36B9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:247

```csharp
{
WorldPacket packet = new WorldPacket(arena.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_ARENA_TEAM_DECLINE

- Legacy value: 850 (0x0352)
- Modern value: 14010 (0x36BA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:248

```csharp
{
WorldPacket packet = new WorldPacket(arena.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_ARENA_TEAM_DISBAND

- Legacy value: 853 (0x0355)
- Modern value: 14013 (0x36BD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:238
- Fields:
  - `WriteUInt32(arena.TeamId)`

```csharp
{
WorldPacket packet = new WorldPacket(arena.GetUniversalOpcode());
packet.WriteUInt32(arena.TeamId);
this.SendPacketToServer(packet);
}
```

---

### CMSG_ARENA_TEAM_LEADER

- Legacy value: 854 (0x0356)
- Modern value: 14014 (0x36BE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:229
- Fields:
  - `WriteUInt32(arena.TeamId)`
  - `WriteCString(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(arena.GetUniversalOpcode());
packet.WriteUInt32(arena.TeamId);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(arena.PlayerGuid));
this.SendPacketToServer(packet);
}
```

---

### CMSG_ARENA_TEAM_LEAVE

- Legacy value: 851 (0x0353)
- Modern value: 14011 (0x36BB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:239
- Fields:
  - `WriteUInt32(arena.TeamId)`

```csharp
{
WorldPacket packet = new WorldPacket(arena.GetUniversalOpcode());
packet.WriteUInt32(arena.TeamId);
this.SendPacketToServer(packet);
}
```

---

### CMSG_ARENA_TEAM_QUERY

- Legacy value: 843 (0x034B)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Server/WorldSocket.cs:186

```csharp
{
if (this.GetSession().GameState.ArenaTeams.TryGetValue(arena.TeamId, out var team))
{
ArenaTeamQueryResponse response = new ArenaTeamQueryResponse();
response.TeamId = arena.TeamId;
response.Emblem = new ArenaTeamEmblem();
response.Emblem.TeamId = arena.TeamId;
response.Emblem.TeamSize = team.TeamSize;
response.Emblem.BackgroundColor = team.BackgroundColor;
response.Emblem.EmblemStyle = team.EmblemStyle;
response.Emblem.EmblemColor = team.EmblemColor;
response.Emblem.BorderStyle = team.BorderStyle;
response.Emblem.BorderColor = team.BorderColor;
response.Emblem.TeamName = team.Name;
this.SendPacket(response);
}
}
```

---

### CMSG_ARENA_TEAM_REMOVE

- Legacy value: 852 (0x0354)
- Modern value: 14012 (0x36BC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:228
- Fields:
  - `WriteUInt32(arena.TeamId)`
  - `WriteCString(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(arena.GetUniversalOpcode());
packet.WriteUInt32(arena.TeamId);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(arena.PlayerGuid));
this.SendPacketToServer(packet);
}
```

---

### CMSG_ARENA_TEAM_ROSTER

- Legacy value: 845 (0x034D)
- Modern value: 14008 (0x36B8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:168
- Fields:
  - `WriteUInt32(this.GetSession()`
  - `WriteUInt32(this.GetSession()`

```csharp
{
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180) || this.GetSession().GameState.CurrentArenaTeamIds[arena.TeamIndex] == 0)
{
ArenaTeamRosterResponse response = new ArenaTeamRosterResponse();
response.TeamSize = ModernVersion.GetArenaTeamSizeFromIndex(arena.TeamIndex);
this.SendPacket(response);
return;
}
WorldPacket packet = new WorldPacket(Opcode.CMSG_ARENA_TEAM_QUERY);
packet.WriteUInt32(this.GetSession().GameState.CurrentArenaTeamIds[arena.TeamIndex]);
this.SendPacketToServer(packet);
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_ARENA_TEAM_ROSTER);
packet2.WriteUInt32(this.GetSession().GameState.CurrentArenaTeamIds[arena.TeamIndex]);
this.SendPacketToServer(packet2);
}
```

---

### CMSG_ATTACK_STOP

- Legacy value: 322 (0x0142)
- Modern value: 12886 (0x3256)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1106

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ATTACK_STOP);
this.SendPacketToServer(packet);
}
```

---

### CMSG_ATTACK_SWING

- Legacy value: 321 (0x0141)
- Modern value: 12885 (0x3255)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1098
- Fields:
  - `WriteGuid(attack.Victim.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ATTACK_SWING);
packet.WriteGuid(attack.Victim.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUCTION_HELLO_REQUEST

- Legacy value: N/A
- Modern value: 13514 (0x34CA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:255
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_AUCTION_HELLO);
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUCTION_LIST_BIDDED_ITEMS

- Legacy value: 612 (0x0264)
- Modern value: 13520 (0x34D0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:263
- Fields:
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteUInt32(auction.Offset)`
  - `WriteInt32(auction.AuctionItemIDs.Count)`
  - `WriteUInt32(itemId)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUCTION_LIST_BIDDED_ITEMS);
packet.WriteGuid(auction.Auctioneer.To64());
packet.WriteUInt32(auction.Offset);
packet.WriteInt32(auction.AuctionItemIDs.Count);
foreach (uint itemId in auction.AuctionItemIDs)
{
packet.WriteUInt32(itemId);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUCTION_LIST_ITEMS

- Legacy value: 600 (0x0258)
- Modern value: 13517 (0x34CD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:286
- Fields:
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteUInt32(auction.Offset)`
  - `WriteCString(auction.Name)`
  - `WriteUInt8(auction.MinLevel)`
  - `WriteUInt8(auction.MaxLevel)`
  - `WriteInt32(ModernToLegacyInventorySlotType(auction.ClassFilters[0].SubC)`
  - `WriteInt32(auction.ClassFilters[0].ItemClass)`
  - `WriteInt32(auction.ClassFilters[0].SubClassFilters[0].ItemSubclass)`
  - `WriteInt32(-1)`
  - `WriteInt32(auction.ClassFilters[0].ItemClass)`
  - `WriteInt32(-1)`
  - `WriteInt32(-1)`
  - `WriteInt32(-1)`
  - `WriteInt32(-1)`
  - `WriteInt32(auction.Quality)`
  - `WriteBool(auction.OnlyUsable)`
  - `WriteBool(auction.ExactMatch)`
  - `WriteUInt8((byte)`
  - `WriteUInt8(sort.Type)`
  - `WriteUInt8(sort.Direction)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUCTION_LIST_ITEMS);
packet.WriteGuid(auction.Auctioneer.To64());
packet.WriteUInt32(auction.Offset);
packet.WriteCString(auction.Name);
packet.WriteUInt8(auction.MinLevel);
packet.WriteUInt8(auction.MaxLevel);
if (auction.ClassFilters.Count > 0)
{
if (auction.ClassFilters[0].SubClassFilters.Count == 1)
{
packet.WriteInt32(ModernToLegacyInventorySlotType(auction.ClassFilters[0].SubClassFilters[0].InvTypeMask));
packet.WriteInt32(auction.ClassFilters[0].ItemClass);
packet.WriteInt32(auction.ClassFilters[0].SubClassFilters[0].ItemSubclass);
}
else
{
packet.WriteInt32(-1);
packet.WriteInt32(auction.ClassFilters[0].ItemClass);
packet.WriteInt32(-1);
}
}
else
{
packet.WriteInt32(-1);
packet.WriteInt32(-1);
packet.WriteInt32(-1);
}
packet.WriteInt32(auction.Quality);
packet.WriteBool(auction.OnlyUsable);
```

---

### CMSG_AUCTION_LIST_OWNED_ITEMS

- Legacy value: 601 (0x0259)
- Modern value: 13519 (0x34CF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:277
- Fields:
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteUInt32(auction.Offset)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUCTION_LIST_OWNED_ITEMS);
packet.WriteGuid(auction.Auctioneer.To64());
packet.WriteUInt32(auction.Offset);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUCTION_PLACE_BID

- Legacy value: 602 (0x025A)
- Modern value: 13521 (0x34D1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:433
- Fields:
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteUInt32(auction.AuctionID)`
  - `WriteUInt32((uint)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUCTION_PLACE_BID);
packet.WriteGuid(auction.Auctioneer.To64());
packet.WriteUInt32(auction.AuctionID);
packet.WriteUInt32((uint)auction.BidAmount);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUCTION_REMOVE_ITEM

- Legacy value: 599 (0x0257)
- Modern value: 13516 (0x34CC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:424
- Fields:
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteUInt32(auction.AuctionID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUCTION_REMOVE_ITEM);
packet.WriteGuid(auction.Auctioneer.To64());
packet.WriteUInt32(auction.AuctionID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUCTION_SELL_ITEM

- Legacy value: 598 (0x0256)
- Modern value: 13515 (0x34CB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:362
- Fields:
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteGuid(item.Guid.To64()`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(expireTime)`
  - `WriteGuid(auction.Auctioneer.To64()`
  - `WriteInt32(auction.Items.Count)`
  - `WriteGuid(item2.Guid.To64()`
  - `WriteUInt32(item2.UseCount)`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(expireTime)`

```csharp
{
uint expireTime = auction.ExpireTime;
if (LegacyVersion.ExpansionVersion <= 1 && ModernVersion.ExpansionVersion > 1)
{
switch (expireTime)
{
case 720u:
expireTime = 120u;
break;
case 1440u:
expireTime = 480u;
break;
case 2880u:
expireTime = 1440u;
break;
}
}
else if (LegacyVersion.ExpansionVersion > 1 && ModernVersion.ExpansionVersion <= 1)
{
switch (expireTime)
{
case 120u:
expireTime = 720u;
break;
case 480u:
expireTime = 1440u;
break;
case 1440u:
expireTime = 2880u;
break;
```

---

### CMSG_AUTOBANK_ITEM

- Legacy value: 643 (0x0283)
- Modern value: 14743 (0x3997)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2058
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(item.GetUniversalOpcode());
byte containerSlot = ((item.PackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot);
byte slot = ((item.PackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUTOSTORE_BANK_ITEM

- Legacy value: 642 (0x0282)
- Modern value: 14742 (0x3996)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2057
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(item.GetUniversalOpcode());
byte containerSlot = ((item.PackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot);
byte slot = ((item.PackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUTOSTORE_LOOT_ITEM

- Legacy value: 264 (0x0108)
- Modern value: 12817 (0x3211)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2182
- Fields:
  - `WriteUInt8(item.LootListID)`

```csharp
{
foreach (LootRequest item in loot.Loot)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUTOSTORE_LOOT_ITEM);
packet.WriteUInt8(item.LootListID);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_AUTO_EQUIP_ITEM

- Legacy value: 266 (0x010A)
- Modern value: 14744 (0x3998)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2056
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(item.GetUniversalOpcode());
byte containerSlot = ((item.PackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot);
byte slot = ((item.PackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUTO_EQUIP_ITEM_SLOT

- Legacy value: 271 (0x010F)
- Modern value: 14749 (0x399D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2069
- Fields:
  - `WriteGuid(item.Item.To64()`
  - `WriteUInt8(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUTO_EQUIP_ITEM_SLOT);
packet.WriteGuid(item.Item.To64());
byte slot = ModernVersion.AdjustInventorySlot(item.ItemDstSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUTO_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1629
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value)`
  - `WriteUInt8(item.ContainerItemSlot)`
  - `WriteUInt8(byte.MaxValue)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot)`
  - `WriteBool(data: false)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (item.ContainerSlot.HasValue)
{
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
packet.WriteUInt8(item.ContainerItemSlot);
}
else
{
packet.WriteUInt8(byte.MaxValue);
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
}
packet.WriteBool(data: false);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
else
{
packet.WriteUInt8(0);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_AUTO_STORE_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1694
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: true)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`
  - `WriteBool(data: true)`
  - `WriteUInt8(0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: true);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
else
{
packet.WriteUInt8(0);
}
packet.WriteBool(data: true);
packet.WriteUInt8(0);
this.SendPacketToServer(packet);
}
```

---

### CMSG_BANKER_ACTIVATE

- Legacy value: 439 (0x01B7)
- Modern value: 13491 (0x34B3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3050
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEFIELD_LEAVE

- Legacy value: 737 (0x02E1)
- Modern value: 12661 (0x3175)
- Handler: HermesProxy/World/Server/WorldSocket.cs:505
- Fields:
  - `WriteUInt8(2)`
  - `WriteUInt8(0)`
  - `WriteUInt32(this.GetSession()`
  - `WriteUInt16(8080)`
  - `WriteUInt32(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_LEAVE);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt8(2);
packet.WriteUInt8(0);
packet.WriteUInt32(this.GetSession().GameState.GetBattleFieldQueueType(1u));
packet.WriteUInt16(8080);
}
else
{
packet.WriteUInt32(this.GetSession().GameState.CurrentMapId.Value);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEFIELD_LIST

- Legacy value: 572 (0x023C)
- Modern value: 12673 (0x3181)
- Handler: HermesProxy/World/Server/WorldSocket.cs:495
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt8(0)`
  - `WriteUInt8(1)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_LIST);
packet.WriteUInt32((uint)request.ListID);
packet.WriteUInt8(0); // fromWhere: 0=battlemaster, 1=UI
packet.WriteUInt8(1); // canGainXP
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEFIELD_PORT

- Legacy value: N/A
- Modern value: 13605 (0x3525)
- Handler: HermesProxy/World/Server/WorldSocket.cs:461
- Fields:
  - `WriteUInt8(2)`
  - `WriteUInt8(0)`
  - `WriteUInt32(this.GetSession()`
  - `WriteUInt16(8080)`
  - `WriteBool(port.AcceptedInvite)`
  - `WriteUInt32(this.GetSession()`
  - `WriteBool(port.AcceptedInvite)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_PORT);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt8(2);
packet.WriteUInt8(0);
packet.WriteUInt32(this.GetSession().GameState.GetBattleFieldQueueType(port.Ticket.Id));
packet.WriteUInt16(8080);
packet.WriteBool(port.AcceptedInvite);
}
else
{
packet.WriteUInt32(this.GetSession().GameState.GetBattleFieldQueueType(port.Ticket.Id));
packet.WriteBool(port.AcceptedInvite);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEMASTER_HELLO

- Legacy value: 727 (0x02D7)
- Modern value: 12977 (0x32B1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3056
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEMASTER_JOIN

- Legacy value: 750 (0x02EE)
- Modern value: 13600 (0x3520)
- Handler: HermesProxy/World/Server/WorldSocket.cs:443
- Fields:
  - `WriteGuid(join.BattlemasterGuid.To64()`
  - `WriteUInt32(GameData.GetMapIdFromBattlegroundId(join.BattlefieldListId)`
  - `WriteUInt32(join.BattlefieldListId)`
  - `WriteInt32(join.BattlefieldInstanceID)`
  - `WriteBool(join.JoinAsGroup)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEMASTER_JOIN);
packet.WriteGuid(join.BattlemasterGuid.To64());
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt32(GameData.GetMapIdFromBattlegroundId(join.BattlefieldListId));
}
else
{
packet.WriteUInt32(join.BattlefieldListId);
}
packet.WriteInt32(join.BattlefieldInstanceID);
packet.WriteBool(join.JoinAsGroup);
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEMASTER_JOIN_ARENA

- Legacy value: 856 (0x0358)
- Modern value: 13601 (0x3521)
- Handler: HermesProxy/World/Server/WorldSocket.cs:206
- Fields:
  - `WriteGuid(join.Guid.To64()`
  - `WriteUInt8(join.TeamIndex)`
  - `WriteBool(data: true)`
  - `WriteBool(data: true)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEMASTER_JOIN_ARENA);
packet.WriteGuid(join.Guid.To64());
packet.WriteUInt8(join.TeamIndex);
packet.WriteBool(data: true);
packet.WriteBool(data: true);
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLEMASTER_JOIN_SKIRMISH

- Legacy value: N/A
- Modern value: 13602 (0x3522)
- Handler: HermesProxy/World/Server/WorldSocket.cs:217
- Fields:
  - `WriteGuid(join.Guid.To64()`
  - `WriteUInt8(join.TeamSize)`
  - `WriteBool(join.AsGroup)`
  - `WriteBool(data: false)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEMASTER_JOIN_ARENA);
packet.WriteGuid(join.Guid.To64());
packet.WriteUInt8(join.TeamSize);
packet.WriteBool(join.AsGroup);
packet.WriteBool(data: false);
this.SendPacketToServer(packet);
}
```

---

### CMSG_BATTLENET_REQUEST

- Legacy value: N/A
- Modern value: 14077 (0x36FD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3668

```csharp
{
if (this._bnetRpc == null)
{
Log.Print(LogType.Error, $"Client tried {108} without authentication", "HandleBattlenetRequest", "SessionHandler.cs");
}
else
{
this._bnetRpc.Invoke(0u, (OriginalHash)request.Method.GetServiceHash(), request.Method.GetMethodId(), request.Method.Token, new CodedInputStream(request.Data));
}
}
```

---

### CMSG_BATTLE_PAY_GET_PRODUCT_LIST

- Legacy value: N/A
- Modern value: 14020 (0x36C4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2670

```csharp
{
}
```

---

### CMSG_BATTLE_PAY_GET_PURCHASE_LIST

- Legacy value: N/A
- Modern value: 14021 (0x36C5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2675

```csharp
{
}
```

---

### CMSG_BATTLE_PET_REQUEST_JOURNAL

- Legacy value: N/A
- Modern value: 13861 (0x3625)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2786

```csharp
{
}
```

---

### CMSG_BEGIN_TRADE

- Legacy value: 279 (0x0117)
- Modern value: 12631 (0x3157)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4393

```csharp
{
WorldPacket packet = new WorldPacket(trade.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_BINDER_ACTIVATE

- Legacy value: 437 (0x01B5)
- Modern value: 13490 (0x34B2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3051
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_BUSY_TRADE

- Legacy value: 280 (0x0118)
- Modern value: 12632 (0x3158)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4394

```csharp
{
WorldPacket packet = new WorldPacket(trade.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_BUY_BACK_ITEM

- Legacy value: 656 (0x0290)
- Modern value: 13476 (0x34A4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2090
- Fields:
  - `WriteGuid(item.VendorGUID.To64()`
  - `WriteUInt32(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BUY_BACK_ITEM);
packet.WriteGuid(item.VendorGUID.To64());
byte slot = ModernVersion.AdjustInventorySlot((byte)item.Slot);
packet.WriteUInt32(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_BUY_BANK_SLOT

- Legacy value: 441 (0x01B9)
- Modern value: 13492 (0x34B4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3083
- Fields:
  - `WriteGuid(bank.Guid.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BUY_BANK_SLOT);
packet.WriteGuid(bank.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_BUY_ITEM

- Legacy value: 418 (0x01A2)
- Modern value: 13475 (0x34A3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1953
- Fields:
  - `WriteGuid(item.VendorGUID.To64()`
  - `WriteUInt32(item.Item.ItemID)`
  - `WriteUInt32((ModernVersion.ExpansionVersion >= 3)`
  - `WriteUInt32(quantity)`
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BUY_ITEM);
packet.WriteGuid(item.VendorGUID.To64());
packet.WriteUInt32(item.Item.ItemID);
uint quantity = item.Quantity / this.GetSession().GameState.GetItemBuyCount(item.Item.ItemID);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
packet.WriteUInt32((ModernVersion.ExpansionVersion >= 3) ? item.Muid : item.Slot);
packet.WriteUInt32(quantity);
}
else
{
packet.WriteUInt8((byte)quantity);
}
packet.WriteUInt8((byte)item.BagSlot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_BUY_STABLE_SLOT

- Legacy value: 626 (0x0272)
- Modern value: 12651 (0x316B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3185
- Fields:
  - `WriteGuid(stable.StableMaster.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BUY_STABLE_SLOT);
packet.WriteGuid(stable.StableMaster.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_CALENDAR_GET_NUM_PENDING

- Legacy value: 1095 (0x0447)
- Modern value: 13948 (0x367C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2761

```csharp
{
}
```

---

### CMSG_CANCEL_AURA

- Legacy value: 310 (0x0136)
- Modern value: 12719 (0x31AF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4097
- Fields:
  - `WriteUInt32(aura.SpellID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CANCEL_AURA);
packet.WriteUInt32(aura.SpellID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CANCEL_AUTO_REPEAT_SPELL

- Legacy value: 621 (0x026D)
- Modern value: 13543 (0x34E7)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4086

```csharp
{
if (Settings.ServerSpellDelay > 0)
{
Thread.Sleep(Settings.ServerSpellDelay);
}
WorldPacket packet = new WorldPacket(Opcode.CMSG_CANCEL_AUTO_REPEAT_SPELL);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CANCEL_CAST

- Legacy value: 303 (0x012F)
- Modern value: 12959 (0x329F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4058
- Fields:
  - `WriteUInt8(0)`
  - `WriteUInt32(cast.SpellID)`

```csharp
{
if (Settings.ServerSpellDelay > 0)
{
Thread.Sleep(Settings.ServerSpellDelay);
}
WorldPacket packet = new WorldPacket(Opcode.CMSG_CANCEL_CAST);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt8(0);
}
packet.WriteUInt32(cast.SpellID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CANCEL_CHANNELLING

- Legacy value: 315 (0x013B)
- Modern value: 12906 (0x326A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4074
- Fields:
  - `WriteInt32(cast.SpellID)`

```csharp
{
if (Settings.ServerSpellDelay > 0)
{
Thread.Sleep(Settings.ServerSpellDelay);
}
WorldPacket packet = new WorldPacket(Opcode.CMSG_CANCEL_CHANNELLING);
packet.WriteInt32(cast.SpellID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CANCEL_MOUNT_AURA

- Legacy value: 885 (0x0375)
- Modern value: 12927 (0x327F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4105
- Fields:
  - `WriteUInt32(aura.SpellID)`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CANCEL_MOUNT_AURA);
this.SendPacketToServer(packet);
return;
}
WowGuid128 guid = this.GetSession().GameState.CurrentPlayerGuid;
Dictionary<int, UpdateField> updateFields = this.GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
if (updateFields == null)
{
return;
}
for (byte i = 0; i < 32; i++)
{
AuraDataInfo aura = this.GetSession().WorldClient.ReadAuraSlot(i, guid, updateFields);
if (aura != null && GameData.MountAuras.Contains(aura.SpellID))
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_CANCEL_AURA);
packet2.WriteUInt32(aura.SpellID);
this.SendPacketToServer(packet2);
}
}
}
```

---

### CMSG_CANCEL_TEMP_ENCHANTMENT

- Legacy value: 889 (0x0379)
- Modern value: 13554 (0x34F2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2147
- Fields:
  - `WriteUInt32(cancel.EnchantmentSlot)`

```csharp
{
if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CANCEL_TEMP_ENCHANTMENT);
packet.WriteUInt32(cancel.EnchantmentSlot);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_CANCEL_TRADE

- Legacy value: 284 (0x011C)
- Modern value: 12636 (0x315C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4395

```csharp
{
WorldPacket packet = new WorldPacket(trade.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_CAN_DUEL

- Legacy value: N/A
- Modern value: 13924 (0x3664)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1121

```csharp
{
CanDuelResult result = new CanDuelResult();
result.TargetGUID = request.TargetGUID;
result.Result = true;
this.SendPacket(result);
}
```

---

### CMSG_CAST_SPELL

- Legacy value: 302 (0x012E)
- Modern value: 12956 (0x329C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3862
- Fields:
  - `WriteUInt32(cast.Cast.SpellID)`
  - `WriteUInt32(cast.Cast.SpellID)`
  - `WriteUInt8(0)`
  - `WriteUInt8(0)`
  - `WriteUInt32(cast.Cast.SpellID)`
  - `WriteUInt8((byte)`

```csharp
{
if (Settings.ServerSpellDelay > 0)
{
Thread.Sleep(Settings.ServerSpellDelay);
}
if (GameData.NextMeleeSpells.Contains(cast.Cast.SpellID) || GameData.AutoRepeatSpells.Contains(cast.Cast.SpellID))
{
ClientCastRequest castRequest = new ClientCastRequest();
castRequest.Timestamp = Environment.TickCount;
castRequest.SpellId = cast.Cast.SpellID;
castRequest.SpellXSpellVisualId = cast.Cast.SpellXSpellVisualID;
castRequest.ClientGUID = cast.Cast.CastID;
if (this.GetSession().GameState.CurrentClientSpecialCast != null)
{
castRequest.ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, this.GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, 10000 + cast.Cast.CastID.GetCounter());
this.SendCastRequestFailed(castRequest, isPet: false);
return;
}
castRequest.ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, this.GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, cast.Cast.SpellID + this.GetSession().GameState.CurrentPlayerGuid.GetCounter());
SpellPrepare prepare = new SpellPrepare();
prepare.ClientCastID = cast.Cast.CastID;
prepare.ServerCastID = castRequest.ServerGUID;
this.SendPacket(prepare);
this.GetSession().GameState.CurrentClientSpecialCast = castRequest;
}
else
{
ClientCastRequest castRequest2 = new ClientCastRequest();
castRequest2.Timestamp = Environment.TickCount;
castRequest2.SpellId = cast.Cast.SpellID;
```

---

### CMSG_CHANGE_REALM_TICKET

- Legacy value: N/A
- Modern value: 14081 (0x3701)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3648

```csharp
{
ChangeRealmTicketResponse response = new ChangeRealmTicketResponse();
response.Token = request.Token;
if (!this.GetSession().AuthClient.IsConnected() && this.GetSession().AuthClient.Reconnect() != HermesProxy.Auth.AuthResult.SUCCESS)
{
Log.Print(LogType.Error, "Failed to reconnect to auth server.", "HandleChangeRealmTicket", "SessionHandler.cs");
response.Allow = false;
this.SendPacket(response);
}
else
{
this._bnetRpc.SetClientSecret(request.Secret);
response.Allow = true;
response.Ticket = new ByteBuffer(new byte[1]);
this.SendPacket(response);
}
}
```

---

### CMSG_CHARACTER_RENAME_REQUEST

- Legacy value: 711 (0x02C7)
- Modern value: 14025 (0x36C9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:764
- Fields:
  - `WriteGuid(rename.Guid.To64()`
  - `WriteCString(rename.NewName)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CHARACTER_RENAME_REQUEST);
packet.WriteGuid(rename.Guid.To64());
packet.WriteCString(rename.NewName);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CHAR_DELETE

- Legacy value: 56 (0x0038)
- Modern value: 13981 (0x369D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:580
- Fields:
  - `WriteGuid(charDelete.Guid.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CHAR_DELETE);
packet.WriteGuid(charDelete.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_CHAT_ADDON_MESSAGE

- Legacy value: N/A
- Modern value: 14318 (0x37EE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:981

```csharp
{
uint language = uint.MaxValue;
string text = packet.Params.Prefix + "\t" + packet.Params.Text;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
ChatMessageTypeWotLK chatMsg = (ChatMessageTypeWotLK)Enum.Parse(typeof(ChatMessageTypeWotLK), packet.Params.Type.ToString());
this.GetSession().WorldClient.SendMessageChatWotLK(chatMsg, language, text, "", "");
}
else
{
ChatMessageTypeVanilla chatMsg2 = (ChatMessageTypeVanilla)Enum.Parse(typeof(ChatMessageTypeVanilla), packet.Params.Type.ToString());
this.GetSession().WorldClient.SendMessageChatVanilla(chatMsg2, language, text, "", "");
}
}
```

---

### CMSG_CHAT_ADDON_MESSAGE_TARGETED

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:998

```csharp
{
uint language = uint.MaxValue;
string text = packet.Params.Prefix + "\t" + packet.Params.Text;
string channelName = (packet.ChannelGuid.IsEmpty() ? "" : this.GetSession().GameState.GetChannelName((int)packet.ChannelGuid.GetCounter()));
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
ChatMessageTypeWotLK chatMsg = (ChatMessageTypeWotLK)Enum.Parse(typeof(ChatMessageTypeWotLK), packet.Params.Type.ToString());
this.GetSession().WorldClient.SendMessageChatWotLK(chatMsg, language, text, channelName, packet.Target);
}
else
{
ChatMessageTypeVanilla chatMsg2 = (ChatMessageTypeVanilla)Enum.Parse(typeof(ChatMessageTypeVanilla), packet.Params.Type.ToString());
this.GetSession().WorldClient.SendMessageChatVanilla(chatMsg2, language, text, channelName, packet.Target);
}
}
```

---

### CMSG_CHAT_CHANNEL_ANNOUNCEMENTS

- Legacy value: 167 (0x00A7)
- Modern value: 14307 (0x37E3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:793
- Fields:
  - `WriteCString(command.ChannelName)`

```csharp
{
WorldPacket packet = new WorldPacket(command.GetUniversalOpcode());
packet.WriteCString(command.ChannelName);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CHAT_CHANNEL_DECLINE_INVITE

- Legacy value: 1040 (0x0410)
- Modern value: 14310 (0x37E6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:828
- Fields:
  - `WriteCString(command.ChannelName)`

```csharp
{
if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_DECLINE_INVITE);
packet.WriteCString(command.ChannelName);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_CHAT_CHANNEL_DISPLAY_LIST

- Legacy value: 978 (0x03D2)
- Modern value: 14294 (0x37D6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:810
- Fields:
  - `WriteCString(command.ChannelName)`
  - `WriteCString(command.ChannelName)`

```csharp
{
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_LIST);
packet.WriteCString(command.ChannelName);
this.SendPacketToServer(packet);
}
else
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_DISPLAY_LIST);
packet2.WriteCString(command.ChannelName);
this.SendPacketToServer(packet2);
}
this.GetSession().GameState.ChannelDisplayList = true;
}
```

---

### CMSG_CHAT_CHANNEL_LIST

- Legacy value: 154 (0x009A)
- Modern value: 14293 (0x37D5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:801
- Fields:
  - `WriteCString(command.ChannelName)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CHAT_CHANNEL_LIST);
packet.WriteCString(command.ChannelName);
this.SendPacketToServer(packet);
this.GetSession().GameState.ChannelDisplayList = false;
}
```

---

### CMSG_CHAT_CHANNEL_OWNER

- Legacy value: 158 (0x009E)
- Modern value: 14297 (0x37D9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:792
- Fields:
  - `WriteCString(command.ChannelName)`

```csharp
{
WorldPacket packet = new WorldPacket(command.GetUniversalOpcode());
packet.WriteCString(command.ChannelName);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CHAT_JOIN_CHANNEL

- Legacy value: 151 (0x0097)
- Modern value: 14280 (0x37C8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:773

```csharp
{
if (this.GetSession().WorldClient != null)
{
this.GetSession().WorldClient.SendChatJoinChannel(join.ChatChannelId, join.ChannelName, join.Password);
}
}
```

---

### CMSG_CHAT_LEAVE_CHANNEL

- Legacy value: 152 (0x0098)
- Modern value: 14281 (0x37C9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:782

```csharp
{
if (this.GetSession().WorldClient != null)
{
this.GetSession().GameState.LeftChannelName = leave.ChannelName;
this.GetSession().WorldClient.SendChatLeaveChannel(leave.ZoneChannelID, leave.ChannelName);
}
}
```

---

### CMSG_CHAT_MESSAGE_AFK

- Legacy value: N/A
- Modern value: 14291 (0x37D3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:839

```csharp
{
List<string> toBeSentTextParts = WorldSocket.ConvertTextMessageIntoMaxLengthParts(afk.Text);
if (toBeSentTextParts.Count >= 1)
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Afk, 0u, toBeSentTextParts[0], "", "");
}
else
{
this.GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Afk, 0u, toBeSentTextParts[0], "", "");
}
}
}
```

---

### CMSG_CHAT_MESSAGE_CHANNEL

- Legacy value: N/A
- Modern value: 14287 (0x37CF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:873

```csharp
{
List<string> toBeSentTextParts = WorldSocket.ConvertTextMessageIntoMaxLengthParts(channel.Text);
foreach (string text in toBeSentTextParts)
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Channel, channel.Language, text, channel.Target, "");
}
else
{
this.GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Channel, channel.Language, text, channel.Target, "");
}
}
}
```

---

### CMSG_CHAT_MESSAGE_DND

- Legacy value: N/A
- Modern value: 14292 (0x37D4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:856

```csharp
{
List<string> toBeSentTextParts = WorldSocket.ConvertTextMessageIntoMaxLengthParts(dnd.Text);
if (toBeSentTextParts.Count >= 1)
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Dnd, 0u, toBeSentTextParts[0], "", "");
}
else
{
this.GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Dnd, 0u, toBeSentTextParts[0], "", "");
}
}
}
```

---

### CMSG_CHAT_MESSAGE_EMOTE

- Legacy value: N/A
- Modern value: 14312 (0x37E8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:907

```csharp
{
List<string> toBeSentTextParts = WorldSocket.ConvertTextMessageIntoMaxLengthParts(emote.Text);
if (toBeSentTextParts.Count >= 1)
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Emote, 0u, toBeSentTextParts[0], "", "");
}
else
{
this.GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Emote, 0u, toBeSentTextParts[0], "", "");
}
}
}
```

---

### CMSG_CHAT_MESSAGE_GUILD

- Legacy value: N/A
- Modern value: 14289 (0x37D1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:924

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_INSTANCE_CHAT

- Legacy value: N/A
- Modern value: 14316 (0x37EC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:931

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_OFFICER

- Legacy value: N/A
- Modern value: 14290 (0x37D2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:925

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_PARTY

- Legacy value: N/A
- Modern value: 14314 (0x37EA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:926

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_RAID

- Legacy value: N/A
- Modern value: 14315 (0x37EB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:927

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_RAID_WARNING

- Legacy value: N/A
- Modern value: 14317 (0x37ED)
- Handler: HermesProxy/World/Server/WorldSocket.cs:928

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_SAY

- Legacy value: N/A
- Modern value: 14311 (0x37E7)
- Handler: HermesProxy/World/Server/WorldSocket.cs:929

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_MESSAGE_WHISPER

- Legacy value: N/A
- Modern value: 14288 (0x37D0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:890

```csharp
{
List<string> toBeSentTextParts = WorldSocket.ConvertTextMessageIntoMaxLengthParts(whisper.Text);
foreach (string text in toBeSentTextParts)
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().WorldClient.SendMessageChatWotLK(ChatMessageTypeWotLK.Whisper, whisper.Language, text, "", whisper.Target);
}
else
{
this.GetSession().WorldClient.SendMessageChatVanilla(ChatMessageTypeVanilla.Whisper, whisper.Language, text, "", whisper.Target);
}
}
}
```

---

### CMSG_CHAT_MESSAGE_YELL

- Legacy value: N/A
- Modern value: 14313 (0x37E9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:930

```csharp
{
ChatMessageTypeModern type;
switch (packet.GetUniversalOpcode())
{
case Opcode.CMSG_CHAT_MESSAGE_SAY:
type = ChatMessageTypeModern.Say;
break;
case Opcode.CMSG_CHAT_MESSAGE_YELL:
type = ChatMessageTypeModern.Yell;
break;
case Opcode.CMSG_CHAT_MESSAGE_GUILD:
type = ChatMessageTypeModern.Guild;
break;
case Opcode.CMSG_CHAT_MESSAGE_OFFICER:
type = ChatMessageTypeModern.Officer;
break;
case Opcode.CMSG_CHAT_MESSAGE_PARTY:
type = ChatMessageTypeModern.Party;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID:
type = ChatMessageTypeModern.Raid;
break;
case Opcode.CMSG_CHAT_MESSAGE_RAID_WARNING:
type = ChatMessageTypeModern.RaidWarning;
break;
case Opcode.CMSG_CHAT_MESSAGE_INSTANCE_CHAT:
type = ((!this.GetSession().GameState.IsInBattleground()) ? ChatMessageTypeModern.Party : ChatMessageTypeModern.Battleground);
break;
default:
Log.Print(LogType.Error, $"HandleMessagechatOpcode : Unknown chat opcode ({packet.GetOpcode()})", "HandleChatMessage", "ChatHandler.cs");
```

---

### CMSG_CHAT_REGISTER_ADDON_PREFIXES

- Legacy value: N/A
- Modern value: 14285 (0x37CD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1026

```csharp
{
foreach (string prefix in addons.Prefixes)
{
this.GetSession().GameState.AddonPrefixes.Add(prefix);
}
}
```

---

### CMSG_CHAT_UNREGISTER_ALL_ADDON_PREFIXES

- Legacy value: N/A
- Modern value: 14286 (0x37CE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1035

```csharp
{
this.GetSession().GameState.AddonPrefixes.Clear();
}
```

---

### CMSG_CLEAR_TRADE_ITEM

- Legacy value: 286 (0x011E)
- Modern value: 12638 (0x315E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4404
- Fields:
  - `WriteUInt8(trade.TradeSlot)`

```csharp
{
TradeSession tradeSession = this.GetSession().GameState.CurrentTrade;
if (tradeSession == null)
{
Log.Print(LogType.Error, $"Got {trade.GetUniversalOpcode()} without trade session", "HandleClearTradeItem", "TradeHandler.cs");
}
else
{
tradeSession.ClientStateIndex++;
WorldPacket packet = new WorldPacket(Opcode.CMSG_CLEAR_TRADE_ITEM);
packet.WriteUInt8(trade.TradeSlot);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_CLOSE_INTERACTION

- Legacy value: N/A
- Modern value: 13459 (0x3493)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2806

```csharp
{
}
```

---

### CMSG_COMPLETE_CINEMATIC

- Legacy value: 252 (0x00FC)
- Modern value: 13637 (0x3545)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2462

```csharp
{
WorldPacket packet = new WorldPacket(cinematic.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_CONFIRM_RESPEC_WIPE

- Legacy value: N/A
- Modern value: 12813 (0x320D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3104
- Fields:
  - `WriteGuid(respec.TrainerGUID.To64()`
  - `WriteGuid(respec.TrainerGUID.To64()`

```csharp
{
switch (respec.RespecType)
{
case SpecResetType.Talents:
{
WorldPacket packet2 = new WorldPacket(Opcode.MSG_TALENT_WIPE_CONFIRM);
packet2.WriteGuid(respec.TrainerGUID.To64());
this.SendPacketToServer(packet2);
break;
}
case SpecResetType.PetTalents:
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_UNLEARN);
packet.WriteGuid(respec.TrainerGUID.To64());
this.SendPacketToServer(packet);
break;
}
default:
Log.Print(LogType.Error, $"Unhandled respec type {respec.RespecType}.", "HandleConfirmRespecWipe", "NPCHandler.cs");
break;
}
}
```

---

### CMSG_CONTACT_LIST

- Legacy value: 102 (0x0066)
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:3681
- Fields:
  - `WriteUInt32((uint)`

```csharp
{
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_FRIEND_LIST);
this.SendPacketToServer(packet);
}
else
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_CONTACT_LIST);
packet2.WriteUInt32((uint)contacts.Flags);
this.SendPacketToServer(packet2);
}
}
```

---

### CMSG_CONVERT_RAID

- Legacy value: 654 (0x028E)
- Modern value: 13905 (0x3651)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1250

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CONVERT_RAID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_CREATE_CHARACTER

- Legacy value: 54 (0x0036)
- Modern value: 13893 (0x3645)
- Handler: HermesProxy/World/Server/WorldSocket.cs:562
- Fields:
  - `WriteCString(charCreate.CreateInfo.Name)`
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`
  - `WriteUInt8(skin)`
  - `WriteUInt8(face)`
  - `WriteUInt8(hairStyle)`
  - `WriteUInt8(hairColor)`
  - `WriteUInt8(facialhair)`
  - `WriteUInt8(0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_CREATE_CHARACTER);
packet.WriteCString(charCreate.CreateInfo.Name);
packet.WriteUInt8((byte)charCreate.CreateInfo.RaceId);
packet.WriteUInt8((byte)charCreate.CreateInfo.ClassId);
packet.WriteUInt8((byte)charCreate.CreateInfo.Sex);
CharacterCustomizations.ConvertModernCustomizationsToLegacy(charCreate.CreateInfo.Customizations, out var skin, out var face, out var hairStyle, out var hairColor, out var facialhair);
packet.WriteUInt8(skin);
packet.WriteUInt8(face);
packet.WriteUInt8(hairStyle);
packet.WriteUInt8(hairColor);
packet.WriteUInt8(facialhair);
packet.WriteUInt8(0);
this.SendPacketToServer(packet);
}
```

---

### CMSG_DB_QUERY_BULK

- Legacy value: N/A
- Modern value: 13797 (0x35E5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1831
- Fields:
  - `WriteCString(bct.MaleText)`
  - `WriteCString(bct.FemaleText)`
  - `WriteUInt32(bct.Entry)`
  - `WriteUInt32(bct.Language)`
  - `WriteUInt32(0u)`
  - `WriteUInt16(0)`
  - `WriteUInt8(0)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt16(bct.Emotes[j])`
  - `WriteUInt16(bct.EmoteDelays[k])`
  - `WriteUInt32(id)`
  - `WriteGuid(WowGuid64.Empty)`
  - `WriteUInt32(id)`
  - `WriteGuid(WowGuid64.Empty)`

```csharp
{
foreach (uint id in query.Queries)
{
DBReply reply = new DBReply();
reply.RecordID = id;
reply.TableHash = query.TableHash;
reply.Status = HotfixStatus.Invalid;
reply.Timestamp = (uint)Time.UnixTime;
Log.PrintNet(LogType.Debug, LogNetDir.C2P, $"DB_QUERY_BULK requested ({query.TableHash}) #{id}", "HandleDbQueryBulk", "HotfixHandler.cs");
if (query.TableHash == DB2Hash.BroadcastText)
{
BroadcastText bct = GameData.GetBroadcastText(id);
if (bct == null)
{
bct = new BroadcastText();
bct.Entry = id;
bct.MaleText = "Clear your cache!";
bct.FemaleText = "Clear your cache!";
}
reply.Status = HotfixStatus.Valid;
reply.Data.WriteCString(bct.MaleText);
reply.Data.WriteCString(bct.FemaleText);
reply.Data.WriteUInt32(bct.Entry);
reply.Data.WriteUInt32(bct.Language);
reply.Data.WriteUInt32(0u);
reply.Data.WriteUInt16(0);
reply.Data.WriteUInt8(0);
reply.Data.WriteUInt32(0u);
if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
{
```

---

### CMSG_DECLINE_GUILD_INVITES

- Legacy value: N/A
- Modern value: 13597 (0x351D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1528

```csharp
{
this.GetSession().GameState.CurrentPlayerStorage.Settings.SetAutoBlockGuildInvites(packet.GuildInvitesShouldGetBlocked);
ObjectUpdate updateData = new ObjectUpdate(this.GetSession().GameState.CurrentPlayerGuid, UpdateTypeModern.Values, this.GetSession());
PlayerFlags flags = this.GetSession().GameState.CurrentPlayerStorage.Settings.CreateNewFlags();
updateData.PlayerData.PlayerFlags = (uint)flags;
UpdateObject updatePacket = new UpdateObject(this.GetSession().GameState);
updatePacket.ObjectUpdates.Add(updateData);
this.GetSession().WorldClient.SendPacketToClient(updatePacket);
}
```

---

### CMSG_DECLINE_PETITION

- Legacy value: N/A
- Modern value: 13620 (0x3534)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3325
- Fields:
  - `WriteGuid(petition.PetitionGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_PETITION_DECLINE);
packet.WriteGuid(petition.PetitionGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_DEL_FRIEND

- Legacy value: 106 (0x006A)
- Modern value: 14041 (0x36D9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3717
- Fields:
  - `WriteGuid(friend.Guid.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(friend.GetUniversalOpcode());
packet.WriteGuid(friend.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_DEL_IGNORE

- Legacy value: 109 (0x006D)
- Modern value: 14045 (0x36DD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3718
- Fields:
  - `WriteGuid(friend.Guid.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(friend.GetUniversalOpcode());
packet.WriteGuid(friend.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_DESTROY_ITEM

- Legacy value: 273 (0x0111)
- Modern value: 12947 (0x3293)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2044
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`
  - `WriteUInt32(item.Count)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_DESTROY_ITEM);
byte containerSlot = ((item.ContainerId != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ContainerId) : item.ContainerId);
byte slot = ((item.ContainerId == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.SlotNum) : item.SlotNum);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
packet.WriteUInt32(item.Count);
this.SendPacketToServer(packet);
}
```

---

### CMSG_DF_GET_JOIN_STATUS

- Legacy value: 662 (0x0296)
- Modern value: 13846 (0x3616)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2756

```csharp
{
}
```

---

### CMSG_DF_GET_SYSTEM_INFO

- Legacy value: N/A
- Modern value: 13845 (0x3615)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2690

```csharp
{
if (packet.Player)
{
WorldPacket legacyPacket = new WorldPacket(Opcode.CMSG_LFG_PLAYER_LOCK_INFO_REQUEST);
this.SendPacketToServer(legacyPacket);
}
else
{
WorldPacket legacyPacket = new WorldPacket(Opcode.CMSG_LFG_PARTY_LOCK_INFO_REQUEST);
this.SendPacketToServer(legacyPacket);
}
}
```

---

### CMSG_DF_JOIN

- Legacy value: N/A
- Modern value: 13835 (0x360B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2705
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt8(0)`
  - `WriteUInt8(0)`
  - `WriteUInt8((byte)`
  - `WriteUInt32(packet.Slots[i])`
  - `WriteUInt8(3)`
  - `WriteUInt8(0)`
  - `WriteUInt8(0)`
  - `WriteUInt8(0)`
  - `WriteCString("")`

```csharp
{
// Legacy 3.3.5a format: uint32 Roles, bool NoPartialClear, bool Achievements, uint8 slotCount, uint32[] Slots, uint8 needsCount(3), uint8[3] Needs, string Comment
WorldPacket legacyPacket = new WorldPacket(Opcode.CMSG_LFG_JOIN);
legacyPacket.WriteUInt32((uint)packet.Roles);
legacyPacket.WriteUInt8(0); // NoPartialClear
legacyPacket.WriteUInt8(0); // Achievements
legacyPacket.WriteUInt8((byte)packet.Slots.Length);
for (int i = 0; i < packet.Slots.Length; i++)
legacyPacket.WriteUInt32(packet.Slots[i]);
legacyPacket.WriteUInt8(3); // Needs count
legacyPacket.WriteUInt8(0); // Need 1
legacyPacket.WriteUInt8(0); // Need 2
legacyPacket.WriteUInt8(0); // Need 3
legacyPacket.WriteCString(""); // Comment
this.SendPacketToServer(legacyPacket);
}
```

---

### CMSG_DF_LEAVE

- Legacy value: N/A
- Modern value: 13844 (0x3614)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2749

```csharp
{
WorldPacket legacyPacket = new WorldPacket(Opcode.CMSG_LFG_LEAVE);
this.SendPacketToServer(legacyPacket);
}
```

---

### CMSG_DF_PROPOSAL_RESPONSE

- Legacy value: N/A
- Modern value: 13833 (0x3609)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2724, HermesProxy/World/Server/WorldSocket.cs:2740
- Fields:
  - `ReadUInt64 -> instanceID`
  - `ReadUInt32 -> proposalID`
  - `WriteUInt32(proposalID)`
  - `WriteUInt8((byte)`
  - `WriteUInt32(packet.ProposalID)`
  - `WriteUInt8((byte)`

```csharp
{
// Modern: RideTicket + uint64 InstanceID + uint32 ProposalID + bit Accepted
RideTicket ticket = new RideTicket();
ticket.Read(packet);
ulong instanceID = packet.ReadUInt64();
uint proposalID = packet.ReadUInt32();
bool accepted = packet.HasBit();
// Legacy: uint32 ProposalID + uint8 Accept
WorldPacket legacyPacket = new WorldPacket(Opcode.CMSG_LFG_PROPOSAL_RESULT);
legacyPacket.WriteUInt32(proposalID);
legacyPacket.WriteUInt8((byte)(accepted ? 1 : 0));
this.SendPacketToServer(legacyPacket);
}
```

---

### CMSG_DISCARDED_TIME_SYNC_ACKS

- Legacy value: N/A
- Modern value: 14913 (0x3A41)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2826

```csharp
{
}
```

---

### CMSG_DO_READY_CHECK

- Legacy value: N/A
- Modern value: 13877 (0x3635)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1257

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_RAID_READY_CHECK);
this.SendPacketToServer(packet);
}
```

---

### CMSG_DUEL_RESPONSE

- Legacy value: N/A
- Modern value: 13538 (0x34E2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1130
- Fields:
  - `WriteGuid(response.ArbiterGUID.To64()`
  - `WriteGuid(response.ArbiterGUID.To64()`

```csharp
{
if (response.Accepted)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_DUEL_ACCEPTED);
packet.WriteGuid(response.ArbiterGUID.To64());
this.SendPacketToServer(packet);
}
else
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_DUEL_CANCELLED);
packet2.WriteGuid(response.ArbiterGUID.To64());
this.SendPacketToServer(packet2);
}
}
```

---

### CMSG_ENABLE_TAXI_NODE

- Legacy value: 1171 (0x0493)
- Modern value: 13481 (0x34A9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4226
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TALK_TO_GOSSIP);
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_ENUM_CHARACTERS

- Legacy value: 55 (0x0037)
- Modern value: 13801 (0x35E9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:523

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_ENUM_CHARACTERS);
this.SendPacketToServer(packet);
}
```

---

### CMSG_FAR_SIGHT

- Legacy value: 634 (0x027A)
- Modern value: 13544 (0x34E8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2469
- Fields:
  - `WriteBool(sight.Enable)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_FAR_SIGHT);
packet.WriteBool(sight.Enable);
this.SendPacketToServer(packet);
this.GetSession().GameState.IsInFarSight = sight.Enable;
}
```

---

### CMSG_GAME_OBJ_REPORT_USE

- Legacy value: 1153 (0x0481)
- Modern value: 13551 (0x34EF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1157
- Fields:
  - `WriteGuid(use.Guid.To64()`

```csharp
{
this.GetSession().GameState.CurrentInteractedWithGO = use.Guid;
WorldPacket packet = new WorldPacket(Opcode.CMSG_GAME_OBJ_REPORT_USE);
packet.WriteGuid(use.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_GAME_OBJ_USE

- Legacy value: 177 (0x00B1)
- Modern value: 13550 (0x34EE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1147
- Fields:
  - `WriteGuid(guid64)`

```csharp
{
WowGuid64 guid64 = use.Guid.To64();
Framework.Logging.Log.Print(Framework.Logging.LogType.Debug, $"[GameObjUse] Modern GUID={use.Guid} -> Legacy GUID={guid64} raw=0x{guid64.GetLowValue():X16}");
WorldPacket packet = new WorldPacket(Opcode.CMSG_GAME_OBJ_USE);
packet.WriteGuid(guid64);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GENERATE_RANDOM_CHARACTER_NAME

- Legacy value: N/A
- Modern value: 13800 (0x35E8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:554

```csharp
{
GenerateRandomCharacterNameResult result = new GenerateRandomCharacterNameResult();
result.Success = false;
this.SendPacket(result);
}
```

---

### CMSG_GET_ACCOUNT_CHARACTER_LIST

- Legacy value: N/A
- Modern value: 14015 (0x36BF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:530

```csharp
{
GetAccountCharacterListResult response = new GetAccountCharacterListResult();
response.Token = request.Token;
foreach (OwnCharacterInfo ownCharacter in this.GetSession().GameState.OwnCharacters)
{
response.CharacterList.Add(new AccountCharacterListEntry
{
AccountId = WowGuid128.Create(HighGuidType703.WowAccount, this.GetSession().GameAccountInfo.Id),
CharacterGuid = ownCharacter.CharacterGuid,
RealmVirtualAddress = this.GetSession().RealmId.GetAddress(),
RealmName = "",
LastLoginUnixSec = ownCharacter.LastLoginUnixSec,
Name = ownCharacter.Name,
Race = ownCharacter.RaceId,
Class = ownCharacter.ClassId,
Sex = ownCharacter.SexId,
Level = ownCharacter.Level
});
}
this.SendPacket(response);
}
```

---

### CMSG_GET_UNDELETE_CHARACTER_COOLDOWN_STATUS

- Legacy value: N/A
- Modern value: 14055 (0x36E7)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2680

```csharp
{
}
```

---

### CMSG_GM_TICKET_GET_CASE_STATUS

- Legacy value: N/A
- Modern value: 13967 (0x368F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2791

```csharp
{
}
```

---

### CMSG_GOSSIP_SELECT_OPTION

- Legacy value: 380 (0x017C)
- Modern value: 13460 (0x3494)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3066
- Fields:
  - `WriteGuid(gossip.GossipUnit.To64()`
  - `WriteUInt32(gossip.GossipID)`
  - `WriteUInt32(gossip.GossipIndex)`
  - `WriteCString(gossip.PromotionCode)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GOSSIP_SELECT_OPTION);
packet.WriteGuid(gossip.GossipUnit.To64());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt32(gossip.GossipID);
}
packet.WriteUInt32(gossip.GossipIndex);
if (!string.IsNullOrEmpty(gossip.PromotionCode))
{
packet.WriteCString(gossip.PromotionCode);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_GROUP_CHANGE_SUB_GROUP

- Legacy value: 638 (0x027E)
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1323
- Fields:
  - `WriteCString(this.GetSession()`
  - `WriteUInt8(group.NewSubGroup)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GROUP_CHANGE_SUB_GROUP);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(group.TargetGUID));
packet.WriteUInt8(group.NewSubGroup);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GROUP_SWAP_SUB_GROUP

- Legacy value: 640 (0x0280)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1332
- Fields:
  - `WriteCString(this.GetSession()`
  - `WriteCString(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GROUP_SWAP_SUB_GROUP);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(group.FirstTarget));
packet.WriteCString(this.GetSession().GameState.GetPlayerName(group.SecondTarget));
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_ADD_RANK

- Legacy value: 562 (0x0232)
- Modern value: 12388 (0x3064)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1464
- Fields:
  - `WriteCString(rank.Name)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_ADD_RANK);
packet.WriteCString(rank.Name);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_AUTO_DECLINE_INVITATION

- Legacy value: N/A
- Modern value: 12385 (0x3061)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1540

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_DECLINE_INVITATION);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_ACTIVATE

- Legacy value: 998 (0x03E6)
- Modern value: 13493 (0x34B5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1547
- Fields:
  - `WriteGuid(activate.BankGuid.To64()`
  - `WriteBool(activate.FullUpdate)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_ACTIVATE);
packet.WriteGuid(activate.BankGuid.To64());
packet.WriteBool(activate.FullUpdate);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_BUY_TAB

- Legacy value: 1002 (0x03EA)
- Modern value: 13507 (0x34C3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1611
- Fields:
  - `WriteGuid(buy.BankGuid.To64()`
  - `WriteUInt8(buy.BankTab)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_BUY_TAB);
packet.WriteGuid(buy.BankGuid.To64());
packet.WriteUInt8(buy.BankTab);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_DEPOSIT_MONEY

- Legacy value: 1004 (0x03EC)
- Modern value: 13509 (0x34C5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1566
- Fields:
  - `WriteGuid(deposit.BankGuid.To64()`
  - `WriteUInt32((uint)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_DEPOSIT_MONEY);
packet.WriteGuid(deposit.BankGuid.To64());
packet.WriteUInt32((uint)deposit.Money);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_LOG_QUERY

- Legacy value: N/A
- Modern value: 12418 (0x3082)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1594
- Fields:
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_GUILD_BANK_LOG_QUERY);
packet.WriteUInt8((byte)query.Tab);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_QUERY_TAB

- Legacy value: 999 (0x03E7)
- Modern value: 13506 (0x34C2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1556
- Fields:
  - `WriteGuid(query.BankGuid.To64()`
  - `WriteUInt8(query.Tab)`
  - `WriteBool(query.FullUpdate)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_QUERY_TAB);
packet.WriteGuid(query.BankGuid.To64());
packet.WriteUInt8(query.Tab);
packet.WriteBool(query.FullUpdate);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY_QUERY

- Legacy value: N/A
- Modern value: 12419 (0x3083)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1359

```csharp
{
if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.MSG_GUILD_BANK_MONEY_WITHDRAWN);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_GUILD_BANK_SET_TAB_TEXT

- Legacy value: 1035 (0x040B)
- Modern value: 12422 (0x3086)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1602
- Fields:
  - `WriteUInt8((byte)`
  - `WriteCString(query.TabText)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SET_TAB_TEXT);
packet.WriteUInt8((byte)query.Tab);
packet.WriteCString(query.TabText);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_TEXT_QUERY

- Legacy value: N/A
- Modern value: 12423 (0x3087)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1575
- Fields:
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_QUERY_GUILD_BANK_TEXT);
packet.WriteUInt8((byte)query.Tab);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_UPDATE_TAB

- Legacy value: 1003 (0x03EB)
- Modern value: 13508 (0x34C4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1583
- Fields:
  - `WriteGuid(update.BankGuid.To64()`
  - `WriteUInt8(update.BankTab)`
  - `WriteCString(update.Name)`
  - `WriteCString(update.Icon)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_UPDATE_TAB);
packet.WriteGuid(update.BankGuid.To64());
packet.WriteUInt8(update.BankTab);
packet.WriteCString(update.Name);
packet.WriteCString(update.Icon);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_BANK_WITHDRAW_MONEY

- Legacy value: 1005 (0x03ED)
- Modern value: 13510 (0x34C6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1620
- Fields:
  - `WriteGuid(withdraw.BankGuid.To64()`
  - `WriteUInt32((uint)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_WITHDRAW_MONEY);
packet.WriteGuid(withdraw.BankGuid.To64());
packet.WriteUInt32((uint)withdraw.Money);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_DECLINE_INVITATION

- Legacy value: 133 (0x0085)
- Modern value: 12384 (0x3060)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1501

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_DECLINE_INVITATION);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_DELETE

- Legacy value: 143 (0x008F)
- Modern value: 12392 (0x3068)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1508

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_DELETE);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_DELETE_RANK

- Legacy value: 563 (0x0233)
- Modern value: 12389 (0x3065)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1472

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_DELETE_RANK);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_DEMOTE_MEMBER

- Legacy value: 140 (0x008C)
- Modern value: 12382 (0x305E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1411
- Fields:
  - `WriteCString(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_DEMOTE_MEMBER);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(demote.Demotee));
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_GET_ROSTER

- Legacy value: 137 (0x0089)
- Modern value: 12403 (0x3073)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1369

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_INFO);
this.SendPacketToServer(packet);
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_GUILD_GET_ROSTER);
this.SendPacketToServer(packet2);
}
```

---

### CMSG_GUILD_INVITE_BY_NAME

- Legacy value: 130 (0x0082)
- Modern value: 13832 (0x3608)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1427
- Fields:
  - `WriteCString(invite.Name)`
  - `WriteUInt32(invite.ArenaTeamId)`
  - `WriteCString(invite.Name)`

```csharp
{
if (invite.ArenaTeamId == 0)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_INVITE_BY_NAME);
packet.WriteCString(invite.Name);
this.SendPacketToServer(packet);
}
else
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_ARENA_TEAM_INVITE);
packet2.WriteUInt32(invite.ArenaTeamId);
packet2.WriteCString(invite.Name);
this.SendPacketToServer(packet2);
}
}
```

---

### CMSG_GUILD_LEAVE

- Legacy value: 141 (0x008D)
- Modern value: 12386 (0x3062)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1487

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_LEAVE);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_OFFICER_REMOVE_MEMBER

- Legacy value: 142 (0x008E)
- Modern value: 12387 (0x3063)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1419
- Fields:
  - `WriteCString(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_OFFICER_REMOVE_MEMBER);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(remove.Removee));
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_PERMISSIONS_QUERY

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1349

```csharp
{
if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.MSG_GUILD_PERMISSIONS);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_GUILD_PROMOTE_MEMBER

- Legacy value: 139 (0x008B)
- Modern value: 12381 (0x305D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1403
- Fields:
  - `WriteCString(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_PROMOTE_MEMBER);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(promote.Promotee));
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_SET_ACHIEVEMENT_TRACKING

- Legacy value: N/A
- Modern value: 12399 (0x306F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2766

```csharp
{
}
```

---

### CMSG_GUILD_SET_GUILD_MASTER

- Legacy value: 144 (0x0090)
- Modern value: 14032 (0x36D0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1479
- Fields:
  - `WriteCString(master.NewMasterName)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_SET_GUILD_MASTER);
packet.WriteCString(master.NewMasterName);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_SET_MEMBER_NOTE

- Legacy value: N/A
- Modern value: 12402 (0x3072)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1394
- Fields:
  - `WriteCString(this.GetSession()`
  - `WriteCString(note.Note)`

```csharp
{
WorldPacket packet = new WorldPacket(note.IsPublic ? Opcode.CMSG_GUILD_SET_PUBLIC_NOTE : Opcode.CMSG_GUILD_SET_OFFICER_NOTE);
packet.WriteCString(this.GetSession().GameState.GetPlayerName(note.NoteeGUID));
packet.WriteCString(note.Note);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_SET_RANK_PERMISSIONS

- Legacy value: 561 (0x0231)
- Modern value: 12391 (0x3067)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1445
- Fields:
  - `WriteUInt32(rank.RankID)`
  - `WriteUInt32(rank.Flags)`
  - `WriteCString(rank.RankName)`
  - `WriteInt32(rank.WithdrawGoldLimit)`
  - `WriteUInt32(rank.TabFlags[i])`
  - `WriteUInt32(rank.TabWithdrawItemLimit[i])`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_SET_RANK_PERMISSIONS);
packet.WriteUInt32(rank.RankID);
packet.WriteUInt32(rank.Flags);
packet.WriteCString(rank.RankName);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteInt32(rank.WithdrawGoldLimit);
for (int i = 0; i < 6; i++)
{
packet.WriteUInt32(rank.TabFlags[i]);
packet.WriteUInt32(rank.TabWithdrawItemLimit[i]);
}
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_UPDATE_INFO_TEXT

- Legacy value: 764 (0x02FC)
- Modern value: 12405 (0x3075)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1386
- Fields:
  - `WriteCString(text.InfoText)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_UPDATE_INFO_TEXT);
packet.WriteCString(text.InfoText);
this.SendPacketToServer(packet);
}
```

---

### CMSG_GUILD_UPDATE_MOTD_TEXT

- Legacy value: 145 (0x0091)
- Modern value: 12404 (0x3074)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1378
- Fields:
  - `WriteCString(text.MotdText)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_UPDATE_MOTD_TEXT);
packet.WriteCString(text.MotdText);
this.SendPacketToServer(packet);
}
```

---

### CMSG_HOTFIX_REQUEST

- Legacy value: N/A
- Modern value: 13798 (0x35E6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1924

```csharp
{
HotfixConnect connect = new HotfixConnect();
foreach (uint id in request.Hotfixes)
{
if (GameData.Hotfixes.TryGetValue(id, out var record))
{
Log.Print(LogType.Debug, $"Hotfix record {record.RecordId} from {record.TableHash}.", "HandleHotfixRequest", "HotfixHandler.cs");
connect.Hotfixes.Add(record);
}
}
this.SendPacket(connect);
}
```

---

### CMSG_IGNORE_TRADE

- Legacy value: 281 (0x0119)
- Modern value: 12633 (0x3159)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4397

```csharp
{
WorldPacket packet = new WorldPacket(trade.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_INITIATE_TRADE

- Legacy value: 278 (0x0116)
- Modern value: 12630 (0x3156)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4360
- Fields:
  - `WriteGuid(trade.Guid.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_INITIATE_TRADE);
packet.WriteGuid(trade.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_INSPECT

- Legacy value: 276 (0x0114)
- Modern value: 13609 (0x3529)
- Handler: HermesProxy/World/Server/WorldSocket.cs:730
- Fields:
  - `WriteGuid(inspect.Target.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_INSPECT);
packet.WriteGuid(inspect.Target.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_INSPECT_HONOR_STATS

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:738
- Fields:
  - `WriteGuid(inspect.Target.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_INSPECT_HONOR_STATS);
packet.WriteGuid(inspect.Target.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_INSPECT_PVP

- Legacy value: N/A
- Modern value: 13987 (0x36A3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:746
- Fields:
  - `WriteGuid(inspect.Target.To64()`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.MSG_INSPECT_ARENA_TEAMS);
packet.WriteGuid(inspect.Target.To64());
this.SendPacketToServer(packet);
return;
}
InspectPvP pvp = new InspectPvP();
pvp.PlayerGUID = inspect.Target;
pvp.ArenaTeams.Add(new ArenaTeamInspectData());
pvp.ArenaTeams.Add(new ArenaTeamInspectData());
pvp.ArenaTeams.Add(new ArenaTeamInspectData());
this.SendPacket(pvp);
}
```

---

### CMSG_LEARN_TALENT

- Legacy value: 593 (0x0251)
- Modern value: 13650 (0x3552)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4132
- Fields:
  - `WriteUInt32(talent.TalentID)`
  - `WriteUInt32(talent.Rank)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LEARN_TALENT);
packet.WriteUInt32(talent.TalentID);
packet.WriteUInt32(talent.Rank);
this.SendPacketToServer(packet);
}
```

---

### CMSG_LEAVE_GROUP

- Legacy value: N/A
- Modern value: 13900 (0x364C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1197

```csharp
{
this.GetSession().GameState.WeWantToLeaveGroup = true;
WorldPacket packet = new WorldPacket(Opcode.CMSG_GROUP_DISBAND);
this.SendPacketToServer(packet);
}
```

---

### CMSG_LFG_LIST_GET_STATUS

- Legacy value: N/A
- Modern value: 13837 (0x360D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2781

```csharp
{
}
```

---

### CMSG_LIST_INVENTORY

- Legacy value: 414 (0x019E)
- Modern value: 13473 (0x34A1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3052
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_LOADING_SCREEN_NOTIFY

- Legacy value: N/A
- Modern value: 13817 (0x35F9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:588

```csharp
{
if (loadingScreenNotify.MapID >= 0)
{
this.GetSession().GameState.CurrentMapId = loadingScreenNotify.MapID;
}
}
```

---

### CMSG_LOGOUT_CANCEL

- Legacy value: 78 (0x004E)
- Modern value: 13528 (0x34D8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:653

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOGOUT_CANCEL);
this.SendPacketToServer(packet);
}
```

---

### CMSG_LOGOUT_REQUEST

- Legacy value: 75 (0x004B)
- Modern value: 13526 (0x34D6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:646

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOGOUT_REQUEST);
this.SendPacketToServer(packet);
}
```

---

### CMSG_LOOT_ITEM

- Legacy value: N/A
- Modern value: 12817 (0x3211)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2181
- Fields:
  - `WriteUInt8(item.LootListID)`

```csharp
{
foreach (LootRequest item in loot.Loot)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_AUTOSTORE_LOOT_ITEM);
packet.WriteUInt8(item.LootListID);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_LOOT_MASTER_GIVE

- Legacy value: 675 (0x02A3)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2244
- Fields:
  - `WriteGuid(item.LootObj.To64()`
  - `WriteUInt8(item.LootListID)`
  - `WriteGuid(loot.TargetGUID.To64()`

```csharp
{
foreach (LootRequest item in loot.Loot)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOOT_MASTER_GIVE);
packet.WriteGuid(item.LootObj.To64());
packet.WriteUInt8(item.LootListID);
packet.WriteGuid(loot.TargetGUID.To64());
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_LOOT_MONEY

- Legacy value: 350 (0x015E)
- Modern value: 12816 (0x3210)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2202

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOOT_MONEY);
this.SendPacketToServer(packet);
}
```

---

### CMSG_LOOT_RELEASE

- Legacy value: 351 (0x015F)
- Modern value: 12819 (0x3213)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2173
- Fields:
  - `WriteGuid(loot.Owner.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOOT_RELEASE);
packet.WriteGuid(loot.Owner.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_LOOT_ROLL

- Legacy value: 672 (0x02A0)
- Modern value: 12820 (0x3214)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2234
- Fields:
  - `WriteGuid(loot.LootObj.To64()`
  - `WriteUInt32(loot.LootListID)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOOT_ROLL);
packet.WriteGuid(loot.LootObj.To64());
packet.WriteUInt32(loot.LootListID);
packet.WriteUInt8((byte)loot.RollType);
this.SendPacketToServer(packet);
}
```

---

### CMSG_LOOT_UNIT

- Legacy value: 349 (0x015D)
- Modern value: 12815 (0x320F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2193
- Fields:
  - `WriteGuid(loot.Unit.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_LOOT_UNIT);
packet.WriteGuid(loot.Unit.To64());
this.SendPacketToServer(packet);
this.GetSession().GameState.LastLootTargetGuid = loot.Unit.To64();
}
```

---

### CMSG_MAIL_CREATE_TEXT_ITEM

- Legacy value: 586 (0x024A)
- Modern value: 13627 (0x353B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2272
- Fields:
  - `WriteGuid(mail.Mailbox.To64()`
  - `WriteUInt32(mail.MailID)`
  - `WriteUInt32(0u)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_CREATE_TEXT_ITEM);
packet.WriteGuid(mail.Mailbox.To64());
packet.WriteUInt32(mail.MailID);
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MAIL_DELETE

- Legacy value: 585 (0x0249)
- Modern value: 12837 (0x3225)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2285
- Fields:
  - `WriteGuid(this.GetSession()`
  - `WriteUInt32(mail.MailID)`
  - `WriteUInt32(0u)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_DELETE);
packet.WriteGuid(this.GetSession().GameState.CurrentInteractedWithGO.To64());
packet.WriteUInt32(mail.MailID);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt32(0u);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MAIL_GET_LIST

- Legacy value: 570 (0x023A)
- Modern value: 13622 (0x3536)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2264
- Fields:
  - `WriteGuid(mail.Mailbox.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_GET_LIST);
packet.WriteGuid(mail.Mailbox.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_MAIL_MARK_AS_READ

- Legacy value: 583 (0x0247)
- Modern value: 13626 (0x353A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2298
- Fields:
  - `WriteGuid(mail.Mailbox.To64()`
  - `WriteUInt32(mail.MailID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_MARK_AS_READ);
packet.WriteGuid(mail.Mailbox.To64());
packet.WriteUInt32(mail.MailID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MAIL_RETURN_TO_SENDER

- Legacy value: 584 (0x0248)
- Modern value: 13912 (0x3658)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2307
- Fields:
  - `WriteGuid(this.GetSession()`
  - `WriteUInt32(mail.MailID)`
  - `WriteGuid(mail.SenderGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_RETURN_TO_SENDER);
packet.WriteGuid(this.GetSession().GameState.CurrentInteractedWithGO.To64());
packet.WriteUInt32(mail.MailID);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteGuid(mail.SenderGUID.To64());
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MAIL_TAKE_ITEM

- Legacy value: 582 (0x0246)
- Modern value: 13624 (0x3538)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2320
- Fields:
  - `WriteGuid(mail.Mailbox.To64()`
  - `WriteUInt32(mail.MailID)`
  - `WriteUInt32(mail.AttachID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_TAKE_ITEM);
packet.WriteGuid(mail.Mailbox.To64());
packet.WriteUInt32(mail.MailID);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt32(mail.AttachID);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MAIL_TAKE_MONEY

- Legacy value: 581 (0x0245)
- Modern value: 13623 (0x3537)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2333
- Fields:
  - `WriteGuid(mail.Mailbox.To64()`
  - `WriteUInt32(mail.MailID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MAIL_TAKE_MONEY);
packet.WriteGuid(mail.Mailbox.To64());
packet.WriteUInt32(mail.MailID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MERGE_GUILD_BANK_ITEM_WITH_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1807
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: true)`
  - `WriteUInt8(item.BankTab2)`
  - `WriteUInt8(item.BankSlot2)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(item.BankTab1)`
  - `WriteUInt8(item.BankSlot1)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt32(item.StackCount)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: true);
packet.WriteUInt8(item.BankTab2);
packet.WriteUInt8(item.BankSlot2);
packet.WriteUInt32(0u);
packet.WriteUInt8(item.BankTab1);
packet.WriteUInt8(item.BankSlot1);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(item.StackCount);
}
else
{
packet.WriteUInt8((byte)item.StackCount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MERGE_GUILD_BANK_ITEM_WITH_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1749
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value)`
  - `WriteUInt8(item.ContainerItemSlot)`
  - `WriteUInt8(byte.MaxValue)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot)`
  - `WriteBool(data: true)`
  - `WriteUInt32(item.StackCount)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (item.ContainerSlot.HasValue)
{
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
packet.WriteUInt8(item.ContainerItemSlot);
}
else
{
packet.WriteUInt8(byte.MaxValue);
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
}
packet.WriteBool(data: true);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(item.StackCount);
}
else
{
packet.WriteUInt8((byte)item.StackCount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MERGE_ITEM_WITH_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1662
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value)`
  - `WriteUInt8(item.ContainerItemSlot)`
  - `WriteUInt8(byte.MaxValue)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot)`
  - `WriteBool(data: false)`
  - `WriteUInt32(item.StackCount)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (item.ContainerSlot.HasValue)
{
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
packet.WriteUInt8(item.ContainerItemSlot);
}
else
{
packet.WriteUInt8(byte.MaxValue);
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
}
packet.WriteBool(data: false);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(item.StackCount);
}
else
{
packet.WriteUInt8((byte)item.StackCount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MINIMAP_PING

- Legacy value: N/A
- Modern value: 13902 (0x364E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1298

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_MINIMAP_PING);
packet.WriteVector2(ping.Position);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOUNT_SPECIAL_ANIM

- Legacy value: 369 (0x0171)
- Modern value: 12928 (0x3280)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2478

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MOUNT_SPECIAL_ANIM);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_CHANGE_TRANSPORT

- Legacy value: 909 (0x038D)
- Modern value: 14895 (0x3A2F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2836
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_DISMISS_VEHICLE

- Legacy value: N/A
- Modern value: 14899 (0x3A33)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2837
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_DOUBLE_JUMP

- Legacy value: N/A
- Modern value: 14827 (0x39EB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2866
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_FALL_LAND

- Legacy value: N/A
- Modern value: 14843 (0x39FB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2838
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_FALL_RESET

- Legacy value: 714 (0x02CA)
- Modern value: 14873 (0x3A19)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2839
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_FEATHER_FALL_ACK

- Legacy value: 719 (0x02CF)
- Modern value: 14876 (0x3A1C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2959
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`
  - `WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAck)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAckOpcode(movementAck.GetUniversalOpcode())) ? 1 : 0);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_FORCE_FLIGHT_BACK_SPEED_CHANGE_ACK

- Legacy value: 900 (0x0384)
- Modern value: 14894 (0x3A2E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2910
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_FLIGHT_SPEED_CHANGE_ACK

- Legacy value: 898 (0x0382)
- Modern value: 14893 (0x3A2D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2911
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_PITCH_RATE_CHANGE_ACK

- Legacy value: 1117 (0x045D)
- Modern value: 14898 (0x3A32)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2912
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_ROOT_ACK

- Legacy value: 233 (0x00E9)
- Modern value: 14862 (0x3A0E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2980
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_FORCE_RUN_BACK_SPEED_CHANGE_ACK

- Legacy value: 229 (0x00E5)
- Modern value: 14860 (0x3A0C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2913
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_RUN_SPEED_CHANGE_ACK

- Legacy value: 227 (0x00E3)
- Modern value: 14859 (0x3A0B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2914
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_SWIM_BACK_SPEED_CHANGE_ACK

- Legacy value: 733 (0x02DD)
- Modern value: 14882 (0x3A22)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2915
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_SWIM_SPEED_CHANGE_ACK

- Legacy value: 231 (0x00E7)
- Modern value: 14861 (0x3A0D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2916
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_TURN_RATE_CHANGE_ACK

- Legacy value: 735 (0x02DF)
- Modern value: 14883 (0x3A23)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2917
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_FORCE_UNROOT_ACK

- Legacy value: 235 (0x00EB)
- Modern value: 14863 (0x3A0F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2981
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_FORCE_WALK_SPEED_CHANGE_ACK

- Legacy value: 731 (0x02DB)
- Modern value: 14881 (0x3A21)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2918
- Fields:
  - `WritePackedGuid(speed.MoverGUID.To64()`
  - `WriteGuid(speed.MoverGUID.To64()`
  - `WriteUInt32(speed.Ack.MoveCounter)`
  - `WriteFloat(speed.Speed)`

```csharp
{
Opcode opcode = speed.GetUniversalOpcode();
bool flag = LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180);
bool flag2 = flag;
if (flag2)
{
bool flag3 = opcode - 743 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
flag2 = flag3;
}
if (!flag2)
{
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(speed.MoverGUID.To64());
}
else
{
packet.WriteGuid(speed.MoverGUID.To64());
}
packet.WriteUInt32(speed.Ack.MoveCounter);
speed.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteFloat(speed.Speed);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_MOVE_GRAVITY_DISABLE_ACK

- Legacy value: 1231 (0x04CF)
- Modern value: 14901 (0x3A35)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2983
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_GRAVITY_ENABLE_ACK

- Legacy value: 1233 (0x04D1)
- Modern value: 14902 (0x3A36)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2984
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1782
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: true)`
  - `WriteUInt8(item.BankTab2)`
  - `WriteUInt8(item.BankSlot2)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(item.BankTab1)`
  - `WriteUInt8(item.BankSlot1)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: true);
packet.WriteUInt8(item.BankTab2);
packet.WriteUInt8(item.BankSlot2);
packet.WriteUInt32(0u);
packet.WriteUInt8(item.BankTab1);
packet.WriteUInt8(item.BankSlot1);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
else
{
packet.WriteUInt8(0);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_HEARTBEAT

- Legacy value: N/A
- Modern value: 14864 (0x3A10)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2840
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_HOVER_ACK

- Legacy value: 246 (0x00F6)
- Modern value: 14867 (0x3A13)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2960
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`
  - `WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAck)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAckOpcode(movementAck.GetUniversalOpcode())) ? 1 : 0);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_INIT_ACTIVE_MOVER_COMPLETE

- Legacy value: N/A
- Modern value: 14918 (0x3A46)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3009
- Fields:
  - `WriteGuid(this.GetSession()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_ACTIVE_MOVER);
packet.WriteGuid(this.GetSession().GameState.CurrentPlayerGuid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_JUMP

- Legacy value: N/A
- Modern value: 14826 (0x39EA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2841
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_KNOCK_BACK_ACK

- Legacy value: 240 (0x00F0)
- Modern value: 14866 (0x3A12)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2982
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_REMOVE_MOVEMENT_FORCES

- Legacy value: N/A
- Modern value: 14871 (0x3A17)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2842
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_CAN_FLY_ACK

- Legacy value: 837 (0x0345)
- Modern value: 14887 (0x3A27)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2961
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`
  - `WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAck)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAckOpcode(movementAck.GetUniversalOpcode())) ? 1 : 0);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_FACING

- Legacy value: N/A
- Modern value: 14857 (0x3A09)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2843
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_FACING_HEARTBEAT

- Legacy value: N/A
- Modern value: 14943 (0x3A5F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2844
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_FLY

- Legacy value: 838 (0x0346)
- Modern value: 14888 (0x3A28)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2845
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_PITCH

- Legacy value: N/A
- Modern value: 14858 (0x3A0A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2846
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_RUN_MODE

- Legacy value: N/A
- Modern value: 14834 (0x39F2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2847
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SET_WALK_MODE

- Legacy value: N/A
- Modern value: 14835 (0x39F3)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2848
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_SPLINE_DONE

- Legacy value: 713 (0x02C9)
- Modern value: 14872 (0x3A18)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3017
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`
  - `WriteInt32(movement.SplineID)`
  - `WriteFloat(0f)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MOVE_SPLINE_DONE);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteInt32(movement.SplineID);
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteFloat(0f);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_ASCEND

- Legacy value: N/A
- Modern value: 14889 (0x3A29)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2849
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_BACKWARD

- Legacy value: N/A
- Modern value: 14821 (0x39E5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2850
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_DESCEND

- Legacy value: N/A
- Modern value: 14896 (0x3A30)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2851
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_FORWARD

- Legacy value: N/A
- Modern value: 14820 (0x39E4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2852
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_PITCH_DOWN

- Legacy value: N/A
- Modern value: 14832 (0x39F0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2853
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_PITCH_UP

- Legacy value: N/A
- Modern value: 14831 (0x39EF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2854
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_STRAFE_LEFT

- Legacy value: N/A
- Modern value: 14823 (0x39E7)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2858
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_STRAFE_RIGHT

- Legacy value: N/A
- Modern value: 14824 (0x39E8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2859
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_SWIM

- Legacy value: N/A
- Modern value: 14844 (0x39FC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2855
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_TURN_LEFT

- Legacy value: N/A
- Modern value: 14828 (0x39EC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2856
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_START_TURN_RIGHT

- Legacy value: N/A
- Modern value: 14829 (0x39ED)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2857
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_STOP

- Legacy value: N/A
- Modern value: 14822 (0x39E6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2860
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_STOP_ASCEND

- Legacy value: N/A
- Modern value: 14890 (0x3A2A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2861
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_STOP_PITCH

- Legacy value: N/A
- Modern value: 14833 (0x39F1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2862
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_STOP_STRAFE

- Legacy value: N/A
- Modern value: 14825 (0x39E9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2863
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_STOP_SWIM

- Legacy value: N/A
- Modern value: 14845 (0x39FD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2864
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_STOP_TURN

- Legacy value: N/A
- Modern value: 14830 (0x39EE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2865
- Fields:
  - `WritePackedGuid(movement.Guid.To64()`

```csharp
{
string opcodeName = movement.GetUniversalOpcode().ToString();
opcodeName = opcodeName.Replace("CMSG", "MSG");
uint opcode = Opcodes.GetOpcodeValueForVersion(opcodeName, Settings.ServerBuild);
if (opcode == 0)
{
opcode = Opcodes.GetOpcodeValueForVersion("MSG_MOVE_SET_FACING", Settings.ServerBuild);
}
WorldPacket packet = new WorldPacket(opcode);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.Guid.To64());
}
movement.MoveInfo.WriteMovementInfoLegacy(packet);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_TELEPORT_ACK

- Legacy value: N/A
- Modern value: 14842 (0x39FA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2885
- Fields:
  - `WritePackedGuid(teleport.MoverGUID.To64()`
  - `WriteGuid(teleport.MoverGUID.To64()`
  - `WriteUInt32(teleport.MoveCounter)`
  - `WriteUInt32(teleport.MoveTime)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_MOVE_TELEPORT_ACK);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(teleport.MoverGUID.To64());
}
else
{
packet.WriteGuid(teleport.MoverGUID.To64());
}
packet.WriteUInt32(teleport.MoveCounter);
packet.WriteUInt32(teleport.MoveTime);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_TIME_SKIPPED

- Legacy value: 718 (0x02CE)
- Modern value: 14875 (0x3A1B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3034
- Fields:
  - `WritePackedGuid(movement.MoverGUID.To64()`
  - `WriteGuid(movement.MoverGUID.To64()`
  - `WriteUInt32(movement.TimeSkipped)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_MOVE_TIME_SKIPPED);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movement.MoverGUID.To64());
}
else
{
packet.WriteGuid(movement.MoverGUID.To64());
}
packet.WriteUInt32(movement.TimeSkipped);
this.SendPacketToServer(packet);
}
```

---

### CMSG_MOVE_WATER_WALK_ACK

- Legacy value: 720 (0x02D0)
- Modern value: 14877 (0x3A1D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2962
- Fields:
  - `WritePackedGuid(movementAck.MoverGUID.To64()`
  - `WriteGuid(movementAck.MoverGUID.To64()`
  - `WriteUInt32(movementAck.Ack.MoveCounter)`
  - `WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAck)`

```csharp
{
WorldPacket packet = new WorldPacket(movementAck.GetUniversalOpcode());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WritePackedGuid(movementAck.MoverGUID.To64());
}
else
{
packet.WriteGuid(movementAck.MoverGUID.To64());
}
packet.WriteUInt32(movementAck.Ack.MoveCounter);
movementAck.Ack.MoveInfo.WriteMovementInfoLegacy(packet);
packet.WriteInt32(movementAck.Ack.MoveInfo.Flags.HasAnyFlag(this.GetFlagForAckOpcode(movementAck.GetUniversalOpcode())) ? 1 : 0);
this.SendPacketToServer(packet);
}
```

---

### CMSG_NEXT_CINEMATIC_CAMERA

- Legacy value: 251 (0x00FB)
- Modern value: 13636 (0x3544)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2461

```csharp
{
WorldPacket packet = new WorldPacket(cinematic.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_OBJECT_UPDATE_FAILED

- Legacy value: N/A
- Modern value: 12675 (0x3183)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2627

```csharp
{
Log.Print(LogType.Error, $"Object update failed for {fail.ObjectGuid}.", "HandleObjectUpdateFailed", "MiscHandler.cs");
}
```

---

### CMSG_OFFER_PETITION

- Legacy value: 451 (0x01C3)
- Modern value: 13053 (0x32FD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3312
- Fields:
  - `WriteUInt32(petition.UnkInt)`
  - `WriteGuid(petition.ItemGUID.To64()`
  - `WriteGuid(petition.TargetPlayer.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_OFFER_PETITION);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt32(petition.UnkInt);
}
packet.WriteGuid(petition.ItemGUID.To64());
packet.WriteGuid(petition.TargetPlayer.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_OPENING_CINEMATIC

- Legacy value: 249 (0x00F9)
- Modern value: 13635 (0x3543)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2460

```csharp
{
WorldPacket packet = new WorldPacket(cinematic.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_OPEN_ITEM

- Legacy value: 172 (0x00AC)
- Modern value: 12998 (0x32C6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2128
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_OPEN_ITEM);
byte containerSlot = ((item.PackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot);
byte slot = ((item.PackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_OPT_OUT_OF_LOOT

- Legacy value: 1033 (0x0409)
- Modern value: 13558 (0x34F6)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2219
- Fields:
  - `WriteInt32(loot.PassOnLoot ? 1 : 0)`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_OPT_OUT_OF_LOOT);
packet.WriteInt32(loot.PassOnLoot ? 1 : 0);
this.SendPacketToServer(packet);
}
else
{
this.GetSession().GameState.IsPassingOnLoot = loot.PassOnLoot;
}
}
```

---

### CMSG_OVERRIDE_SCREEN_FLASH

- Legacy value: N/A
- Modern value: 13598 (0x351E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2660

```csharp
{
}
```

---

### CMSG_PARTY_INVITE

- Legacy value: 110 (0x006E)
- Modern value: 13828 (0x3604)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1166
- Fields:
  - `WriteCString(invite.TargetName)`
  - `WriteUInt32(0u)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PARTY_INVITE);
packet.WriteCString(invite.TargetName);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_PARTY_INVITE_RESPONSE

- Legacy value: N/A
- Modern value: 13830 (0x3606)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1178
- Fields:
  - `WriteUInt32(0u)`

```csharp
{
if (invite.Accept)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GROUP_ACCEPT);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
this.SendPacketToServer(packet);
}
else
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_GROUP_DECLINE);
this.SendPacketToServer(packet2);
}
}
```

---

### CMSG_PARTY_UNINVITE

- Legacy value: N/A
- Modern value: 13898 (0x364A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1205
- Fields:
  - `WriteGuid(kick.TargetGUID.To64()`
  - `WriteCString(kick.Reason)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GROUP_UNINVITE_GUID);
packet.WriteGuid(kick.TargetGUID.To64());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteCString(kick.Reason);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_PETITION_BUY

- Legacy value: 445 (0x01BD)
- Modern value: 13512 (0x34C8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3243
- Fields:
  - `WriteGuid(petition.Unit.To64()`
  - `WriteUInt32(0u)`
  - `WriteUInt64(0uL)`
  - `WriteCString(petition.Title)`
  - `WriteCString("")`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt16(0)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteCString("")`
  - `WriteUInt16(0)`
  - `WriteUInt8(0)`
  - `WriteUInt32(petition.Index)`
  - `WriteUInt32(0u)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PETITION_BUY);
packet.WriteGuid(petition.Unit.To64());
packet.WriteUInt32(0u);
packet.WriteUInt64(0uL);
packet.WriteCString(petition.Title);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteCString("");
}
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt16(0);
}
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
packet.WriteUInt32(0u);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
for (int i = 0; i < 10; i++)
{
packet.WriteCString("");
}
```

---

### CMSG_PETITION_RENAME_GUILD

- Legacy value: N/A
- Modern value: 14033 (0x36D1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3303
- Fields:
  - `WriteGuid(petition.PetitionGuid.To64()`
  - `WriteCString(petition.NewGuildName)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_PETITION_RENAME);
packet.WriteGuid(petition.PetitionGuid.To64());
packet.WriteCString(petition.NewGuildName);
this.SendPacketToServer(packet);
}
```

---

### CMSG_PETITION_SHOW_SIGNATURES

- Legacy value: 446 (0x01BE)
- Modern value: 13513 (0x34C9)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3286
- Fields:
  - `WriteGuid(petition.Item.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PETITION_SHOW_SIGNATURES);
packet.WriteGuid(petition.Item.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_PET_ABANDON

- Legacy value: 374 (0x0176)
- Modern value: 13453 (0x348D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3193
- Fields:
  - `WriteGuid(pet.PetGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_ABANDON);
packet.WriteGuid(pet.PetGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_PET_ACTION

- Legacy value: 373 (0x0175)
- Modern value: 13451 (0x348B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3129
- Fields:
  - `WriteGuid(act.PetGUID.To64()`
  - `WriteUInt32(act.Action)`
  - `WriteGuid(act.TargetGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_ACTION);
packet.WriteGuid(act.PetGUID.To64());
packet.WriteUInt32(act.Action);
packet.WriteGuid(act.TargetGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_PET_CANCEL_AURA

- Legacy value: 619 (0x026B)
- Modern value: 13454 (0x348E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3227
- Fields:
  - `WriteGuid(cancel.PetGUID.To64()`
  - `WriteUInt32(cancel.SpellID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_CANCEL_AURA);
packet.WriteGuid(cancel.PetGUID.To64());
packet.WriteUInt32(cancel.SpellID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_PET_CAST_SPELL

- Legacy value: 496 (0x01F0)
- Modern value: 12955 (0x329B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3945
- Fields:
  - `WriteGuid(cast.PetGUID.To64()`
  - `WriteUInt8(0)`
  - `WriteUInt32(cast.Cast.SpellID)`
  - `WriteUInt8((byte)`

```csharp
{
if (Settings.ServerSpellDelay > 0)
{
Thread.Sleep(Settings.ServerSpellDelay);
}
ClientCastRequest castRequest = new ClientCastRequest();
castRequest.Timestamp = Environment.TickCount;
castRequest.SpellId = cast.Cast.SpellID;
castRequest.SpellXSpellVisualId = cast.Cast.SpellXSpellVisualID;
castRequest.ClientGUID = cast.Cast.CastID;
castRequest.ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, this.GetSession().GameState.CurrentMapId.Value, cast.Cast.SpellID, 10000 + cast.Cast.CastID.GetCounter());
if (this.GetSession().GameState.CurrentClientPetCast != null)
{
if (this.GetSession().GameState.CurrentClientPetCast.HasStarted)
{
this.SendCastRequestFailed(castRequest, isPet: true);
}
else if (this.GetSession().GameState.CurrentClientPetCast.Timestamp + 10000 < castRequest.Timestamp)
{
Log.Print(LogType.Warn, $"Clearing CurrentClientPetCast because of 10 sec timeout! (oldSpell:{this.GetSession().GameState.CurrentClientPetCast.SpellId} newSpell:{castRequest.SpellId})", "HandlePetCastSpell", "SpellHandler.cs");
this.SendCastRequestFailed(this.GetSession().GameState.CurrentClientPetCast, isPet: true);
this.GetSession().GameState.CurrentClientPetCast = null;
foreach (ClientCastRequest pending in this.GetSession().GameState.PendingClientPetCasts)
{
this.SendCastRequestFailed(pending, isPet: true);
}
this.GetSession().GameState.PendingClientPetCasts.Clear();
this.SendCastRequestFailed(castRequest, isPet: true);
}
else
```

---

### CMSG_PET_RENAME

- Legacy value: 375 (0x0177)
- Modern value: 13958 (0x3686)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3157
- Fields:
  - `WriteGuid(pet.RenameData.PetGUID.To64()`
  - `WriteCString(pet.RenameData.NewName)`
  - `WriteBool(pet.RenameData.HasDeclinedNames)`
  - `WriteCString(pet.RenameData.DeclinedNames.name[i])`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_RENAME);
packet.WriteGuid(pet.RenameData.PetGUID.To64());
packet.WriteCString(pet.RenameData.NewName);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteBool(pet.RenameData.HasDeclinedNames);
if (pet.RenameData.HasDeclinedNames)
{
for (int i = 0; i < 5; i++)
{
packet.WriteCString(pet.RenameData.DeclinedNames.name[i]);
}
}
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_PET_SET_ACTION

- Legacy value: 372 (0x0174)
- Modern value: 13450 (0x348A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3147
- Fields:
  - `WriteGuid(action.PetGUID.To64()`
  - `WriteUInt32(action.Index)`
  - `WriteUInt32(action.Action)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_SET_ACTION);
packet.WriteGuid(action.PetGUID.To64());
packet.WriteUInt32(action.Index);
packet.WriteUInt32(action.Action);
this.SendPacketToServer(packet);
}
```

---

### CMSG_PET_STOP_ATTACK

- Legacy value: 746 (0x02EA)
- Modern value: 13452 (0x348C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3139
- Fields:
  - `WriteGuid(stop.PetGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PET_STOP_ATTACK);
packet.WriteGuid(stop.PetGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_PLAYER_LOGIN

- Legacy value: 61 (0x003D)
- Modern value: 13803 (0x35EB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:616
- Fields:
  - `WriteGuid(playerLogin.Guid.To64()`

```csharp
{
if (!this.GetSession().GameState.CachedPlayers.TryGetValue(playerLogin.Guid, out var selectedChar))
{
Log.Print(LogType.Error, $"Player tried to log in with unknown char id: {playerLogin.Guid}", "HandlePlayerLogin", "CharacterHandler.cs");
return;
}
Realm realm = this.GetSession().RealmManager.GetRealm(this.GetSession().RealmId);
if (realm == null)
{
Log.Print(LogType.Error, $"Player tried to log in to unknown realm id: {this.GetSession().RealmId}", "HandlePlayerLogin", "CharacterHandler.cs");
return;
}
this.GetSession().AccountMetaDataMgr.SaveLastSelectedCharacter(realm.Name, selectedChar.Name, playerLogin.Guid.Low, Time.UnixTime);
if (this.GetSession().AuthClient != null)
{
this.GetSession().AuthClient.Disconnect();
}
this.SendConnectToInstance(ConnectToSerial.WorldAttempt1);
this.GetSession().GameState.IsConnectedToInstance = true;
this.GetSession().GameState.IsFirstEnterWorld = true;
this.GetSession().GameState.CurrentPlayerGuid = playerLogin.Guid;
this.GetSession().GameState.CurrentPlayerInfo = this.GetSession().GameState.OwnCharacters.Single((OwnCharacterInfo x) => x.CharacterGuid == playerLogin.Guid);
this.GetSession().GameState.CurrentPlayerStorage.LoadCurrentPlayer();
WorldPacket packet = new WorldPacket(Opcode.CMSG_PLAYER_LOGIN);
packet.WriteGuid(playerLogin.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_PLAYER_SHOWING_CLOAK

- Legacy value: 698 (0x02BA)
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:721
- Fields:
  - `WriteBool(show.Showing)`

```csharp
{
WorldPacket packet = new WorldPacket(show.GetUniversalOpcode());
packet.WriteBool(show.Showing);
this.SendPacketToServer(packet);
}
```

---

### CMSG_PLAYER_SHOWING_HELM

- Legacy value: 697 (0x02B9)
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:722
- Fields:
  - `WriteBool(show.Showing)`

```csharp
{
WorldPacket packet = new WorldPacket(show.GetUniversalOpcode());
packet.WriteBool(show.Showing);
this.SendPacketToServer(packet);
}
```

---

### CMSG_PUSH_QUEST_TO_PARTY

- Legacy value: 413 (0x019D)
- Modern value: 13471 (0x349F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3596
- Fields:
  - `WriteUInt32(quest.QuestID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_PUSH_QUEST_TO_PARTY);
packet.WriteUInt32(quest.QuestID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_PVP_LOG_DATA

- Legacy value: N/A
- Modern value: 12671 (0x317F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:488

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_PVP_LOG_DATA);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_CORPSE_LOCATION_FROM_CLIENT

- Legacy value: N/A
- Modern value: 13922 (0x3662)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2437

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_CORPSE_QUERY);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_COUNTDOWN_TIMER

- Legacy value: N/A
- Modern value: 12714 (0x31AA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2771

```csharp
{
}
```

---

### CMSG_QUERY_CREATURE

- Legacy value: 96 (0x0060)
- Modern value: 12912 (0x3270)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3373
- Fields:
  - `WriteUInt32(queryCreature.CreatureID)`
  - `WriteGuid(new WowGuid64(HighGuidTypeLegacy.Creature, queryCreature.Cre)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_CREATURE);
packet.WriteUInt32(queryCreature.CreatureID);
packet.WriteGuid(new WowGuid64(HighGuidTypeLegacy.Creature, queryCreature.CreatureID, 1u));
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_GAME_OBJECT

- Legacy value: 94 (0x005E)
- Modern value: 12913 (0x3271)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3382
- Fields:
  - `WriteUInt32(queryGo.GameObjectID)`
  - `WriteGuid(queryGo.Guid.To64()`

```csharp
{
// Respond from cache immediately if available (avoids round-trip for transports)
if (GetSession().GameState.GameObjectQueryCache.TryGetValue(queryGo.GameObjectID, out var cached))
{
var response = new HermesProxy.World.Server.Packets.QueryGameObjectResponse();
response.GameObjectID = cached.GameObjectID;
response.Guid = WowGuid128.Empty;
response.Allow = cached.Allow;
response.Stats = cached.Stats;
SendPacket(response);
return;
}
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_GAME_OBJECT);
packet.WriteUInt32(queryGo.GameObjectID);
packet.WriteGuid(queryGo.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_GUILD_INFO

- Legacy value: 84 (0x0054)
- Modern value: 13963 (0x368B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1341
- Fields:
  - `WriteUInt32((uint)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_GUILD_INFO);
packet.WriteUInt32((uint)query.GuildGuid.GetCounter());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_NEXT_MAIL_TIME

- Legacy value: N/A
- Modern value: 13625 (0x3539)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2257

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_QUERY_NEXT_MAIL_TIME);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_NPC_TEXT

- Legacy value: 383 (0x017F)
- Modern value: 12914 (0x3272)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3411
- Fields:
  - `WriteUInt32(queryText.TextID)`
  - `WriteGuid(queryText.Guid.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_NPC_TEXT);
packet.WriteUInt32(queryText.TextID);
packet.WriteGuid(queryText.Guid.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_PAGE_TEXT

- Legacy value: 90 (0x005A)
- Modern value: 12916 (0x3274)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3402
- Fields:
  - `WriteUInt32(queryText.PageTextID)`
  - `WriteGuid(queryText.ItemGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_PAGE_TEXT);
packet.WriteUInt32(queryText.PageTextID);
packet.WriteGuid(queryText.ItemGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_PETITION

- Legacy value: 454 (0x01C6)
- Modern value: 12919 (0x3277)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3294
- Fields:
  - `WriteUInt32(petition.PetitionID)`
  - `WriteGuid(petition.ItemGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_PETITION);
packet.WriteUInt32(petition.PetitionID);
packet.WriteGuid(petition.ItemGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_PET_NAME

- Legacy value: 82 (0x0052)
- Modern value: 12917 (0x3275)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3420
- Fields:
  - `WriteUInt32(queryName.UnitGUID.GetEntry()`
  - `WriteGuid(queryName.UnitGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_PET_NAME);
packet.WriteUInt32(queryName.UnitGUID.GetEntry());
packet.WriteGuid(queryName.UnitGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_PLAYER_NAME

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:597
- Fields:
  - `WriteGuid(queryPlayerName.Player.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_NAME_QUERY);
packet.WriteGuid(queryPlayerName.Player.To64());
this.SendPacketToServer(packet, (!this.GetSession().GameState.IsInWorld) ? Opcode.SMSG_LOGIN_VERIFY_WORLD : Opcode.MSG_NULL_ACTION);
}
```

---

### CMSG_QUERY_PLAYER_NAMES

- Legacy value: N/A
- Modern value: 14194 (0x3772)
- Handler: HermesProxy/World/Server/WorldSocket.cs:605
- Fields:
  - `WriteGuid(guid.To64()`

```csharp
{
foreach (WowGuid128 guid in queryPlayerNames.Players)
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_NAME_QUERY);
packet.WriteGuid(guid.To64());
this.SendPacketToServer(packet, (!this.GetSession().GameState.IsInWorld) ? Opcode.SMSG_LOGIN_VERIFY_WORLD : Opcode.MSG_NULL_ACTION);
}
}
```

---

### CMSG_QUERY_QUEST_INFO

- Legacy value: 92 (0x005C)
- Modern value: 12915 (0x3273)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3365
- Fields:
  - `WriteUInt32(queryQuest.QuestID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_QUEST_INFO);
packet.WriteUInt32(queryQuest.QuestID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUERY_TIME

- Legacy value: 462 (0x01CE)
- Modern value: 13525 (0x34D5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3358

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUERY_TIME);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_CONFIRM_ACCEPT

- Legacy value: 411 (0x019B)
- Modern value: 13470 (0x349E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3588
- Fields:
  - `WriteUInt32(quest.QuestID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_CONFIRM_ACCEPT);
packet.WriteUInt32(quest.QuestID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_GIVER_ACCEPT_QUEST

- Legacy value: 393 (0x0189)
- Modern value: 13464 (0x3498)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3466
- Fields:
  - `WriteGuid(quest.QuestGiverGUID.To64()`
  - `WriteUInt32(quest.QuestID)`
  - `WriteInt32(quest.StartCheat ? 1 : 0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_ACCEPT_QUEST);
packet.WriteGuid(quest.QuestGiverGUID.To64());
packet.WriteUInt32(quest.QuestID);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901))
{
packet.WriteInt32(quest.StartCheat ? 1 : 0);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_GIVER_CHOOSE_REWARD

- Legacy value: 398 (0x018E)
- Modern value: 13466 (0x349A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3544
- Fields:
  - `WriteUInt32(quest.QuestID)`
  - `WriteGuid(quest.QuestGiverGUID.To64()`
  - `WriteUInt32(quest.QuestID)`
  - `WriteInt32(choiceIndex)`

```csharp
{
int choiceIndex = 0;
if (quest.Choice.Item.ItemID != 0)
{
QuestTemplate questTemplate = GameData.GetQuestTemplate(quest.QuestID);
if (questTemplate == null)
{
Log.Print(LogType.Error, "Unable to select quest reward because quest template is missing. Try again.", "HandleQuestGiverChooseReward", "QuestHandler.cs");
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_QUERY_QUEST_INFO);
packet2.WriteUInt32(quest.QuestID);
this.SendPacketToServer(packet2);
QuestGiverQuestFailed fail = new QuestGiverQuestFailed();
fail.QuestID = quest.QuestID;
fail.Reason = InventoryResult.ItemNotFound;
this.SendPacket(fail);
return;
}
for (int i = 0; i < questTemplate.UnfilteredChoiceItems.Length; i++)
{
if (questTemplate.UnfilteredChoiceItems[i].ItemID == quest.Choice.Item.ItemID)
{
choiceIndex = i;
break;
}
}
}
WorldPacket packet3 = new WorldPacket(Opcode.CMSG_QUEST_GIVER_CHOOSE_REWARD);
packet3.WriteGuid(quest.QuestGiverGUID.To64());
packet3.WriteUInt32(quest.QuestID);
packet3.WriteInt32(choiceIndex);
```

---

### CMSG_QUEST_GIVER_CLOSE_QUEST

- Legacy value: N/A
- Modern value: 13641 (0x3549)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2831

```csharp
{
}
```

---

### CMSG_QUEST_GIVER_COMPLETE_QUEST

- Legacy value: 394 (0x018A)
- Modern value: 13465 (0x3499)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3579
- Fields:
  - `WriteGuid(quest.QuestGiverGUID.To64()`
  - `WriteUInt32(quest.QuestID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_COMPLETE_QUEST);
packet.WriteGuid(quest.QuestGiverGUID.To64());
packet.WriteUInt32(quest.QuestID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_GIVER_HELLO

- Legacy value: 388 (0x0184)
- Modern value: 13462 (0x3496)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3527
- Fields:
  - `WriteGuid(hello.QuestGiverGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_HELLO);
packet.WriteGuid(hello.QuestGiverGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_GIVER_QUERY_QUEST

- Legacy value: 390 (0x0186)
- Modern value: 13463 (0x3497)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3453
- Fields:
  - `WriteGuid(quest.QuestGiverGUID.To64()`
  - `WriteUInt32(quest.QuestID)`
  - `WriteBool(quest.RespondToGiver)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_QUERY_QUEST);
packet.WriteGuid(quest.QuestGiverGUID.To64());
packet.WriteUInt32(quest.QuestID);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteBool(quest.RespondToGiver);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_GIVER_REQUEST_REWARD

- Legacy value: 396 (0x018C)
- Modern value: 13467 (0x349B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3535
- Fields:
  - `WriteGuid(quest.QuestGiverGUID.To64()`
  - `WriteUInt32(quest.QuestID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_REQUEST_REWARD);
packet.WriteGuid(quest.QuestGiverGUID.To64());
packet.WriteUInt32(quest.QuestID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_GIVER_STATUS_MULTIPLE_QUERY

- Legacy value: 1047 (0x0417)
- Modern value: 13469 (0x349D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3495
- Fields:
  - `WriteGuid(guid.To64()`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_STATUS_MULTIPLE_QUERY);
this.SendPacketToServer(packet);
return;
}
int UNIT_NPC_FLAGS = ModernVersion.GetUpdateField(UnitField.UNIT_NPC_FLAGS);
if (UNIT_NPC_FLAGS < 0)
{
return;
}
List<WowGuid128> npcGuids = new List<WowGuid128>();
this.GetSession().GameState.ObjectCacheMutex.WaitOne();
foreach (KeyValuePair<WowGuid128, UpdateFieldsArray> obj in this.GetSession().GameState.ObjectCacheModern)
{
if (obj.Key.GetObjectType() == ObjectType.Unit && obj.Value.GetUpdateField<uint>(UNIT_NPC_FLAGS, 0).HasAnyFlag(NPCFlags.QuestGiver))
{
npcGuids.Add(obj.Key);
}
}
this.GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
foreach (WowGuid128 guid in npcGuids)
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_QUEST_GIVER_STATUS_QUERY);
packet2.WriteGuid(guid.To64());
this.SendPacketToServer(packet2);
}
}
```

---

### CMSG_QUEST_GIVER_STATUS_QUERY

- Legacy value: 386 (0x0182)
- Modern value: 13468 (0x349C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3487
- Fields:
  - `WriteGuid(query.QuestGiverGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_GIVER_STATUS_QUERY);
packet.WriteGuid(query.QuestGiverGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_LOG_REMOVE_QUEST

- Legacy value: 404 (0x0194)
- Modern value: 13614 (0x352E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3479
- Fields:
  - `WriteUInt8(quest.Slot)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_QUEST_LOG_REMOVE_QUEST);
packet.WriteUInt8(quest.Slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEST_PUSH_RESULT

- Legacy value: N/A
- Modern value: 13472 (0x34A0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3604
- Fields:
  - `WriteGuid(quest.SenderGUID.To64()`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_QUEST_PUSH_RESULT);
packet.WriteGuid(quest.SenderGUID.To64());
packet.WriteUInt8((byte)quest.Result);
this.SendPacketToServer(packet);
}
```

---

### CMSG_QUEUED_MESSAGES_END

- Legacy value: N/A
- Modern value: 14188 (0x376C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2665

```csharp
{
}
```

---

### CMSG_RANDOM_ROLL

- Legacy value: N/A
- Modern value: 13911 (0x3657)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1306
- Fields:
  - `WriteInt32(roll.Min)`
  - `WriteInt32(roll.Max)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_RANDOM_ROLL);
packet.WriteInt32(roll.Min);
packet.WriteInt32(roll.Max);
this.SendPacketToServer(packet);
}
```

---

### CMSG_READY_CHECK_RESPONSE

- Legacy value: N/A
- Modern value: 13878 (0x3636)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1264
- Fields:
  - `WriteBool(raid.IsReady)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_RAID_READY_CHECK);
packet.WriteBool(raid.IsReady);
this.SendPacketToServer(packet);
ReadyCheckResponse ready = new ReadyCheckResponse();
ready.Player = this.GetSession().GameState.CurrentPlayerGuid;
ready.IsReady = raid.IsReady;
ready.PartyGUID = WowGuid128.Create(HighGuidType703.Party, 1000uL);
this.SendPacket(ready);
}
```

---

### CMSG_READ_ITEM

- Legacy value: 173 (0x00AD)
- Modern value: 12999 (0x32C7)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2079
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_READ_ITEM);
byte containerSlot = ((item.PackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.PackSlot) : item.PackSlot);
byte slot = ((item.PackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.Slot) : item.Slot);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_RECLAIM_CORPSE

- Legacy value: 466 (0x01D2)
- Modern value: 13531 (0x34DB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2444
- Fields:
  - `WriteGuid(corpse.CorpseGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_RECLAIM_CORPSE);
packet.WriteGuid(corpse.CorpseGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_REPAIR_ITEM

- Legacy value: 680 (0x02A8)
- Modern value: 13548 (0x34EC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2100
- Fields:
  - `WriteGuid(item.VendorGUID.To64()`
  - `WriteGuid(item.ItemGUID.To64()`
  - `WriteBool(item.UseGuildBank)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_REPAIR_ITEM);
packet.WriteGuid(item.VendorGUID.To64());
packet.WriteGuid(item.ItemGUID.To64());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteBool(item.UseGuildBank);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_REPOP_REQUEST

- Legacy value: 346 (0x015A)
- Modern value: 13606 (0x3526)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2426
- Fields:
  - `WriteBool(repop.CheckInstance)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_REPOP_REQUEST);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteBool(repop.CheckInstance);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_REPORT_CLIENT_VARIABLES

- Legacy value: N/A
- Modern value: 14087 (0x3707)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2811

```csharp
{
}
```

---

### CMSG_REPORT_ENABLED_ADDONS

- Legacy value: N/A
- Modern value: 14086 (0x3706)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2816

```csharp
{
}
```

---

### CMSG_REPORT_KEYBINDING_EXECUTION_COUNTS

- Legacy value: N/A
- Modern value: 14088 (0x3708)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2821

```csharp
{
}
```

---

### CMSG_REQUEST_ACCOUNT_DATA

- Legacy value: 522 (0x020A)
- Modern value: 13972 (0x3694)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1075

```csharp
{
if (this.GetSession().AccountDataMgr.Data[data.DataType] == null)
{
Log.Print(LogType.Error, $"Client requested missing account data {data.DataType}.", "HandleRequestAccountData", "ClientConfigHandler.cs");
this.GetSession().AccountDataMgr.Data[data.DataType] = new AccountData();
this.GetSession().AccountDataMgr.Data[data.DataType].Type = data.DataType;
this.GetSession().AccountDataMgr.Data[data.DataType].Timestamp = Time.UnixTime;
this.GetSession().AccountDataMgr.Data[data.DataType].UncompressedSize = 0u;
this.GetSession().AccountDataMgr.Data[data.DataType].CompressedData = new byte[0];
}
this.GetSession().AccountDataMgr.Data[data.DataType].Guid = data.PlayerGuid;
UpdateAccountData update = new UpdateAccountData(this.GetSession().AccountDataMgr.Data[data.DataType]);
this.SendPacket(update);
}
```

---

### CMSG_REQUEST_BATTLEFIELD_STATUS

- Legacy value: N/A
- Modern value: 13789 (0x35DD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:481

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_BATTLEFIELD_STATUS);
this.SendPacketToServer(packet);
}
```

---

### CMSG_REQUEST_CEMETERY_LIST

- Legacy value: N/A
- Modern value: 12665 (0x3179)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2801

```csharp
{
}
```

---

### CMSG_REQUEST_CONQUEST_FORMULA_CONSTANTS

- Legacy value: N/A
- Modern value: 12980 (0x32B4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2615

```csharp
{
ConquestFormulaConstants response = new ConquestFormulaConstants();
response.PvpMinCPPerWeek = 1500;
response.PvpMaxCPPerWeek = 3000;
response.PvpCPBaseCoefficient = 1511.26f;
response.PvpCPExpCoefficient = 1639.28f;
response.PvpCPNumerator = 0.00412f;
this.SendPacket(response);
}
```

---

### CMSG_REQUEST_FORCED_REACTIONS

- Legacy value: N/A
- Modern value: 12805 (0x3205)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2776

```csharp
{
}
```

---

### CMSG_REQUEST_LFG_LIST_BLACKLIST

- Legacy value: N/A
- Modern value: 12964 (0x32A4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2512

```csharp
{
LFGListUpdateBlacklist blacklist = new LFGListUpdateBlacklist();
if (ModernVersion.ExpansionVersion > 1)
{
blacklist.AddBlacklist(796, 3);
blacklist.AddBlacklist(797, 3);
blacklist.AddBlacklist(798, 3);
blacklist.AddBlacklist(799, 3);
blacklist.AddBlacklist(800, 3);
blacklist.AddBlacklist(801, 3);
blacklist.AddBlacklist(802, 3);
blacklist.AddBlacklist(803, 3);
blacklist.AddBlacklist(804, 3);
blacklist.AddBlacklist(805, 3);
blacklist.AddBlacklist(806, 3);
blacklist.AddBlacklist(807, 3);
blacklist.AddBlacklist(808, 3);
blacklist.AddBlacklist(809, 3);
blacklist.AddBlacklist(810, 3);
blacklist.AddBlacklist(811, 3);
blacklist.AddBlacklist(812, 3);
blacklist.AddBlacklist(813, 3);
blacklist.AddBlacklist(814, 3);
blacklist.AddBlacklist(815, 3);
blacklist.AddBlacklist(816, 3);
blacklist.AddBlacklist(817, 3);
blacklist.AddBlacklist(818, 3);
blacklist.AddBlacklist(820, 3);
blacklist.AddBlacklist(827, 3);
blacklist.AddBlacklist(828, 3);
```

---

### CMSG_REQUEST_PARTY_MEMBER_STATS

- Legacy value: 639 (0x027F)
- Modern value: 13910 (0x3656)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1315
- Fields:
  - `WriteGuid(request.TargetGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_REQUEST_PARTY_MEMBER_STATS);
packet.WriteGuid(request.TargetGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_REQUEST_PET_INFO

- Legacy value: 633 (0x0279)
- Modern value: 13456 (0x3490)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3236

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_REQUEST_PET_INFO);
this.SendPacketToServer(packet);
}
```

---

### CMSG_REQUEST_PLAYED_TIME

- Legacy value: 460 (0x01CC)
- Modern value: 12922 (0x327A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:660
- Fields:
  - `WriteBool(played.TriggerScriptEvent)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_REQUEST_PLAYED_TIME);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteBool(played.TriggerScriptEvent);
}
this.SendPacketToServer(packet);
this.GetSession().GameState.ShowPlayedTime = played.TriggerScriptEvent;
}
```

---

### CMSG_REQUEST_PVP_REWARDS

- Legacy value: N/A
- Modern value: 12694 (0x3196)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2650

```csharp
{
}
```

---

### CMSG_REQUEST_RAID_INFO

- Legacy value: 717 (0x02CD)
- Modern value: 14034 (0x36D2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1946

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_REQUEST_RAID_INFO);
this.SendPacketToServer(packet);
}
```

---

### CMSG_REQUEST_RATED_PVP_INFO

- Legacy value: N/A
- Modern value: 13796 (0x35E4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2655

```csharp
{
}
```

---

### CMSG_REQUEST_STABLED_PETS

- Legacy value: N/A
- Modern value: 13457 (0x3491)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3177
- Fields:
  - `WriteGuid(stable.StableMaster.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_LIST_STABLED_PETS);
packet.WriteGuid(stable.StableMaster.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_RESET_INSTANCES

- Legacy value: 797 (0x031D)
- Modern value: 13930 (0x366A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1939

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_RESET_INSTANCES);
this.SendPacketToServer(packet);
}
```

---

### CMSG_RESURRECT_RESPONSE

- Legacy value: 348 (0x015C)
- Modern value: 13957 (0x3685)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4141
- Fields:
  - `WriteGuid(revive.CasterGUID.To64()`
  - `WriteUInt8((revive.Response == 0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_RESURRECT_RESPONSE);
packet.WriteGuid(revive.CasterGUID.To64());
packet.WriteUInt8((revive.Response == 0) ? ((byte)1) : ((byte)0));
this.SendPacketToServer(packet);
}
```

---

### CMSG_SAVE_CUF_PROFILES

- Legacy value: N/A
- Modern value: 12686 (0x318E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1092

```csharp
{
this.GetSession().AccountDataMgr.SaveCUFProfiles(cuf.Data);
}
```

---

### CMSG_SAVE_GUILD_EMBLEM

- Legacy value: N/A
- Modern value: 12968 (0x32A8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1515
- Fields:
  - `WriteGuid(emblem.DesignerGUID.To64()`
  - `WriteUInt32(emblem.EmblemStyle)`
  - `WriteUInt32(emblem.EmblemColor)`
  - `WriteUInt32(emblem.BorderStyle)`
  - `WriteUInt32(emblem.BorderColor)`
  - `WriteUInt32(emblem.BackgroundColor)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_SAVE_GUILD_EMBLEM);
packet.WriteGuid(emblem.DesignerGUID.To64());
packet.WriteUInt32(emblem.EmblemStyle);
packet.WriteUInt32(emblem.EmblemColor);
packet.WriteUInt32(emblem.BorderStyle);
packet.WriteUInt32(emblem.BorderColor);
packet.WriteUInt32(emblem.BackgroundColor);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SELF_RES

- Legacy value: 691 (0x02B3)
- Modern value: 13617 (0x3531)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4150

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SELF_RES);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SELL_ITEM

- Legacy value: 416 (0x01A0)
- Modern value: 13474 (0x34A2)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1973
- Fields:
  - `WriteGuid(vendorGuid64)`
  - `WriteGuid(itemGuid64)`
  - `WriteUInt32(item.Amount)`
  - `WriteUInt8((byte)`

```csharp
{
WowGuid64 vendorGuid64 = item.VendorGUID.To64();
WowGuid64 itemGuid64 = item.ItemGUID.To64();
Log.Print(LogType.Debug, $"[SellItem] Item128={item.ItemGUID} → Item64={itemGuid64} Vendor128={item.VendorGUID} → Vendor64={vendorGuid64}", "HandleSellItem", "");
WorldPacket packet = new WorldPacket(Opcode.CMSG_SELL_ITEM);
packet.WriteGuid(vendorGuid64);
packet.WriteGuid(itemGuid64);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WriteUInt32(item.Amount);
}
else
{
packet.WriteUInt8((byte)item.Amount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_SEND_MAIL

- Legacy value: 568 (0x0238)
- Modern value: 13819 (0x35FB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2375

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) || mail.Attachments.Count <= 1)
{
this.BuildSendMail(mail, mail.Attachments);
return;
}
mail.SendMoney /= mail.Attachments.Count;
mail.Cod /= mail.Attachments.Count;
foreach (SendMail.MailAttachment item in mail.Attachments)
{
List<SendMail.MailAttachment> attachments = new List<SendMail.MailAttachment>();
attachments.Add(item);
this.BuildSendMail(mail, attachments);
Thread.Sleep(500);
}
}
```

---

### CMSG_SEND_TEXT_EMOTE

- Legacy value: 260 (0x0104)
- Modern value: 13448 (0x3488)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1016
- Fields:
  - `WriteInt32(emote.EmoteID)`
  - `WriteInt32(emote.SoundIndex)`
  - `WriteGuid(emote.Target.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SEND_TEXT_EMOTE);
packet.WriteInt32(emote.EmoteID);
packet.WriteInt32(emote.SoundIndex);
packet.WriteGuid(emote.Target.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_ACTION_BAR_TOGGLES

- Legacy value: 703 (0x02BF)
- Modern value: 13618 (0x3532)
- Handler: HermesProxy/World/Server/WorldSocket.cs:705
- Fields:
  - `WriteUInt8(bars.Mask)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_ACTION_BAR_TOGGLES);
packet.WriteUInt8(bars.Mask);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_ACTION_BUTTON

- Legacy value: 296 (0x0128)
- Modern value: 13661 (0x355D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:695
- Fields:
  - `WriteUInt8(button.Index)`
  - `WriteUInt16(button.Action)`
  - `WriteUInt16(button.Type)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_ACTION_BUTTON);
packet.WriteUInt8(button.Index);
packet.WriteUInt16(button.Action);
packet.WriteUInt16(button.Type);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_ACTIVE_MOVER

- Legacy value: 618 (0x026A)
- Modern value: 14908 (0x3A3C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3001
- Fields:
  - `WriteGuid(move.MoverGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_ACTIVE_MOVER);
packet.WriteGuid(move.MoverGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_AMMO

- Legacy value: 616 (0x0268)
- Modern value: 13662 (0x355E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2139
- Fields:
  - `WriteUInt32(ammo.ItemId)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_AMMO);
packet.WriteUInt32(ammo.ItemId);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_ASSISTANT_LEADER

- Legacy value: 655 (0x028F)
- Modern value: 13906 (0x3652)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1217
- Fields:
  - `WriteGuid(assist.TargetGUID.To64()`
  - `WriteBool(assist.Apply)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_ASSISTANT_LEADER);
packet.WriteGuid(assist.TargetGUID.To64());
packet.WriteBool(assist.Apply);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_CONTACT_NOTES

- Legacy value: 107 (0x006B)
- Modern value: 14042 (0x36DA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3726
- Fields:
  - `WriteGuid(friend.Guid.To64()`
  - `WriteCString(friend.Notes)`

```csharp
{
if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_CONTACT_NOTES);
packet.WriteGuid(friend.Guid.To64());
packet.WriteCString(friend.Notes);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_SET_DUNGEON_DIFFICULTY

- Legacy value: N/A
- Modern value: 13956 (0x3684)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2633
- Fields:
  - `WriteUInt32(dificultyId)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_SET_DUNGEON_DIFFICULTY);
uint dificultyId = (byte)Enum.Parse(typeof(DifficultyLegacy), ((DifficultyModern)difficulty.DifficultyID/*cast due to .constrained prefix*/).ToString());
packet.WriteUInt32(dificultyId);
this.SendPacketToServer(packet);
DungeonDifficultySet difficultySet = new DungeonDifficultySet();
difficultySet.DifficultyID = (int)difficulty.DifficultyID;
this.SendPacket(difficultySet);
}
```

---

### CMSG_SET_EVERYONE_IS_ASSISTANT

- Legacy value: N/A
- Modern value: 13850 (0x361A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1226
- Fields:
  - `WriteGuid(member.GUID.To64()`
  - `WriteBool(assist.Apply)`

```csharp
{
List<PartyPlayerInfo> groupMembers = this.GetSession().GameState.GetCurrentGroup().PlayerList;
foreach (PartyPlayerInfo member in groupMembers)
{
if (!(member.GUID == this.GetSession().GameState.CurrentPlayerGuid))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_ASSISTANT_LEADER);
packet.WriteGuid(member.GUID.To64());
packet.WriteBool(assist.Apply);
this.SendPacketToServer(packet);
}
}
}
```

---

### CMSG_SET_FACTION_AT_WAR

- Legacy value: 293 (0x0125)
- Modern value: 13534 (0x34DE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3613
- Fields:
  - `WriteUInt32(faction.FactionIndex)`
  - `WriteBool(data: true)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_FACTION_AT_WAR);
packet.WriteUInt32(faction.FactionIndex);
packet.WriteBool(data: true);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_FACTION_INACTIVE

- Legacy value: 791 (0x0317)
- Modern value: 13536 (0x34E0)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3631
- Fields:
  - `WriteUInt32(faction.FactionIndex)`
  - `WriteBool(faction.State)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_FACTION_INACTIVE);
packet.WriteUInt32(faction.FactionIndex);
packet.WriteBool(faction.State);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_FACTION_NOT_AT_WAR

- Legacy value: N/A
- Modern value: 13535 (0x34DF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3622
- Fields:
  - `WriteUInt32(faction.FactionIndex)`
  - `WriteBool(data: false)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_FACTION_AT_WAR);
packet.WriteUInt32(faction.FactionIndex);
packet.WriteBool(data: false);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_LOOT_METHOD

- Legacy value: 122 (0x007A)
- Modern value: 13899 (0x364B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2209
- Fields:
  - `WriteUInt32((uint)`
  - `WriteGuid(loot.LootMasterGUID.To64()`
  - `WriteUInt32(loot.LootThreshold)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_LOOT_METHOD);
packet.WriteUInt32((uint)loot.LootMethod);
packet.WriteGuid(loot.LootMasterGUID.To64());
packet.WriteUInt32(loot.LootThreshold);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_PARTY_LEADER

- Legacy value: 120 (0x0078)
- Modern value: 13901 (0x364D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1242
- Fields:
  - `WriteGuid(leader.TargetGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_PARTY_LEADER);
packet.WriteGuid(leader.TargetGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_PVP

- Legacy value: N/A
- Modern value: 12972 (0x32AC)
- Handler: HermesProxy/World/Server/WorldSocket.cs:687
- Fields:
  - `WriteBool(pvp.Enable)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TOGGLE_PVP);
packet.WriteBool(pvp.Enable);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_SELECTION

- Legacy value: 317 (0x013D)
- Modern value: 13608 (0x3528)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2418
- Fields:
  - `WriteGuid(selection.TargetGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_SELECTION);
packet.WriteGuid(selection.TargetGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_SHEATHED

- Legacy value: 480 (0x01E0)
- Modern value: 13449 (0x3489)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1113
- Fields:
  - `WriteInt32(sheath.SheathState)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_SHEATHED);
packet.WriteInt32(sheath.SheathState);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_TITLE

- Legacy value: 884 (0x0374)
- Modern value: 12926 (0x327E)
- Handler: HermesProxy/World/Server/WorldSocket.cs:672
- Fields:
  - `WriteInt32(title.TitleID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_TITLE);
packet.WriteInt32(title.TitleID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_TRADE_GOLD

- Legacy value: 287 (0x011F)
- Modern value: 12639 (0x315F)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4368
- Fields:
  - `WriteInt32((int)`

```csharp
{
TradeSession tradeSession = this.GetSession().GameState.CurrentTrade;
if (tradeSession == null)
{
Log.Print(LogType.Error, $"Got {trade.GetUniversalOpcode()} without trade session", "HandleSetTradeGold", "TradeHandler.cs");
}
else
{
tradeSession.ClientStateIndex++;
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_TRADE_GOLD);
packet.WriteInt32((int)trade.Coinage);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_SET_TRADE_ITEM

- Legacy value: 285 (0x011D)
- Modern value: 12637 (0x315D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4421
- Fields:
  - `WriteUInt8(trade.TradeSlot)`
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`

```csharp
{
TradeSession tradeSession = this.GetSession().GameState.CurrentTrade;
if (tradeSession == null)
{
Log.Print(LogType.Error, $"Got {trade.GetUniversalOpcode()} without trade session", "HandleSetTradeItem", "TradeHandler.cs");
return;
}
tradeSession.ClientStateIndex++;
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_TRADE_ITEM);
packet.WriteUInt8(trade.TradeSlot);
byte containerSlot = ((trade.PackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(trade.PackSlot) : trade.PackSlot);
byte slot = ((trade.PackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(trade.ItemSlotInPack) : trade.ItemSlotInPack);
packet.WriteUInt8(containerSlot);
packet.WriteUInt8(slot);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SET_WATCHED_FACTION

- Legacy value: 792 (0x0318)
- Modern value: 13537 (0x34E1)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3640
- Fields:
  - `WriteUInt32(faction.FactionIndex)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SET_WATCHED_FACTION);
packet.WriteUInt32(faction.FactionIndex);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SIGN_PETITION

- Legacy value: 448 (0x01C0)
- Modern value: 13619 (0x3533)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3333
- Fields:
  - `WriteGuid(petition.PetitionGUID.To64()`
  - `WriteUInt8(petition.Choice)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SIGN_PETITION);
packet.WriteGuid(petition.PetitionGUID.To64());
packet.WriteUInt8(petition.Choice);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SOCKET_GEMS

- Legacy value: 839 (0x0347)
- Modern value: 13547 (0x34EB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2113
- Fields:
  - `WriteGuid(gems.ItemGuid.To64()`
  - `WriteGuid(gems.Gems[i].To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SOCKET_GEMS);
packet.WriteGuid(gems.ItemGuid.To64());
for (int i = 0; i < 3; i++)
{
packet.WriteGuid(gems.Gems[i].To64());
}
this.SendPacketToServer(packet);
SocketGemsSuccess success = new SocketGemsSuccess();
success.ItemGuid = gems.ItemGuid;
this.SendPacket(success);
}
```

---

### CMSG_SPIRIT_HEALER_ACTIVATE

- Legacy value: 540 (0x021C)
- Modern value: 13487 (0x34AF)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3053
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_SPLIT_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1806
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: true)`
  - `WriteUInt8(item.BankTab2)`
  - `WriteUInt8(item.BankSlot2)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(item.BankTab1)`
  - `WriteUInt8(item.BankSlot1)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt32(item.StackCount)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: true);
packet.WriteUInt8(item.BankTab2);
packet.WriteUInt8(item.BankSlot2);
packet.WriteUInt32(0u);
packet.WriteUInt8(item.BankTab1);
packet.WriteUInt8(item.BankSlot1);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(item.StackCount);
}
else
{
packet.WriteUInt8((byte)item.StackCount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_SPLIT_GUILD_BANK_ITEM_TO_INVENTORY

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1750
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value)`
  - `WriteUInt8(item.ContainerItemSlot)`
  - `WriteUInt8(byte.MaxValue)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot)`
  - `WriteBool(data: true)`
  - `WriteUInt32(item.StackCount)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (item.ContainerSlot.HasValue)
{
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
packet.WriteUInt8(item.ContainerItemSlot);
}
else
{
packet.WriteUInt8(byte.MaxValue);
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
}
packet.WriteBool(data: true);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(item.StackCount);
}
else
{
packet.WriteUInt8((byte)item.StackCount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_SPLIT_ITEM

- Legacy value: 270 (0x010E)
- Modern value: 14748 (0x399C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1993
- Fields:
  - `WriteUInt8(containerSlot1)`
  - `WriteUInt8(slot1)`
  - `WriteUInt8(containerSlot2)`
  - `WriteUInt8(slot2)`
  - `WriteInt32(item.Quantity)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SPLIT_ITEM);
byte containerSlot1 = ((item.FromPackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.FromPackSlot) : item.FromPackSlot);
byte slot1 = ((item.FromPackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.FromSlot) : item.FromSlot);
byte containerSlot2 = ((item.ToPackSlot != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ToPackSlot) : item.ToPackSlot);
byte slot2 = ((item.ToPackSlot == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ToSlot) : item.ToSlot);
packet.WriteUInt8(containerSlot1);
packet.WriteUInt8(slot1);
packet.WriteUInt8(containerSlot2);
packet.WriteUInt8(slot2);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
packet.WriteInt32(item.Quantity);
}
else
{
packet.WriteUInt8((byte)item.Quantity);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_SPLIT_ITEM_TO_GUILD_BANK

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1661
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value)`
  - `WriteUInt8(item.ContainerItemSlot)`
  - `WriteUInt8(byte.MaxValue)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot)`
  - `WriteBool(data: false)`
  - `WriteUInt32(item.StackCount)`
  - `WriteUInt8((byte)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (item.ContainerSlot.HasValue)
{
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
packet.WriteUInt8(item.ContainerItemSlot);
}
else
{
packet.WriteUInt8(byte.MaxValue);
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
}
packet.WriteBool(data: false);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(item.StackCount);
}
else
{
packet.WriteUInt8((byte)item.StackCount);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_STABLE_PET

- Legacy value: 624 (0x0270)
- Modern value: 12648 (0x3168)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3201
- Fields:
  - `WriteGuid(pet.StableMaster.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_STABLE_PET);
packet.WriteGuid(pet.StableMaster.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_STABLE_SWAP_PET

- Legacy value: 629 (0x0275)
- Modern value: 12650 (0x316A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3218
- Fields:
  - `WriteGuid(pet.StableMaster.To64()`
  - `WriteUInt32(pet.PetNumber)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_STABLE_SWAP_PET);
packet.WriteGuid(pet.StableMaster.To64());
packet.WriteUInt32(pet.PetNumber);
this.SendPacketToServer(packet);
}
```

---

### CMSG_STAND_STATE_CHANGE

- Legacy value: 257 (0x0101)
- Modern value: 12684 (0x318C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2452
- Fields:
  - `WriteUInt32(state.StandState)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_STAND_STATE_CHANGE);
packet.WriteUInt32(state.StandState);
this.SendPacketToServer(packet);
}
```

---

### CMSG_STORE_GUILD_BANK_ITEM

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:1717
- Fields:
  - `WriteGuid(item.BankGuid.To64()`
  - `WriteBool(data: false)`
  - `WriteUInt8(item.BankTab)`
  - `WriteUInt8(item.BankSlot)`
  - `WriteUInt32(0u)`
  - `WriteBool(data: false)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value)`
  - `WriteUInt8(item.ContainerItemSlot)`
  - `WriteUInt8(byte.MaxValue)`
  - `WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot)`
  - `WriteBool(data: true)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_GUILD_BANK_SWAP_ITEMS);
packet.WriteGuid(item.BankGuid.To64());
packet.WriteBool(data: false);
packet.WriteUInt8(item.BankTab);
packet.WriteUInt8(item.BankSlot);
packet.WriteUInt32(0u);
packet.WriteBool(data: false);
if (item.ContainerSlot.HasValue)
{
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerSlot.Value));
packet.WriteUInt8(item.ContainerItemSlot);
}
else
{
packet.WriteUInt8(byte.MaxValue);
packet.WriteUInt8(ModernVersion.AdjustInventorySlot(item.ContainerItemSlot));
}
packet.WriteBool(data: true);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.WriteUInt32(0u);
}
else
{
packet.WriteUInt8(0);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_SUMMON_RESPONSE

- Legacy value: 684 (0x02AC)
- Modern value: 13932 (0x366C)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1286
- Fields:
  - `WriteGuid(update.SummonerGUID.To64()`
  - `WriteBool(update.Accept)`

```csharp
{
if (update.Accept || LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SUMMON_RESPONSE);
packet.WriteGuid(update.SummonerGUID.To64());
packet.WriteBool(update.Accept);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_SUPPORT_TICKET_SUBMIT_COMPLAINT

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Server/WorldSocket.cs:4168
- Fields:
  - `WriteUInt8(2)`
  - `WriteUInt32(complaint.Header.SelfPlayerMapId)`
  - `WriteVector3`
  - `WriteCString(ticketText)`
  - `WriteCString("")`
  - `WriteUInt32(complaint.Header.SelfPlayerMapId)`
  - `WriteVector3`
  - `WriteCString(ticketText)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteBytes(Array.Empty<byte>()`

```csharp
{
string targetPlayerName = this.Session.GameState.GetPlayerName(complaint.TargetCharacterGuid);
if (string.IsNullOrWhiteSpace(targetPlayerName))
{
this.Session.SendHermesTextMessage("Unable to report player because CharacterName was not resolved (can be fixed by restarting the client)", isError: true);
return;
}
string ticketText = "[REPORTED VIA QUICKMENU]\r\nI would like to report player '" + targetPlayerName + "'";
if (!WowGuid128.IsUnknownPlayerGuid(complaint.TargetCharacterGuid))
{
ticketText += $"  (id: {complaint.TargetCharacterGuid.GetCounter()})";
}
if (complaint.ComplaintType != GmTicketComplaintType.Unknown)
{
ticketText += $" for {complaint.ComplaintType}";
}
if (complaint.SelectedMailInfo != null)
{
ticketText = ticketText + "\r\n" + $"Mail in question (id: {complaint.SelectedMailInfo.MailId}) with subject '{complaint.SelectedMailInfo.MailSubject}'";
}
if (!complaint.TextNote.IsEmpty())
{
ticketText += "\r\n-------------";
ticketText = ticketText + "\r\n" + complaint.TextNote;
}
WorldPacket packet = new WorldPacket(Opcode.CMSG_GM_TICKET_CREATE);
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt8(2);
packet.WriteUInt32(complaint.Header.SelfPlayerMapId);
```

---

### CMSG_SWAP_INV_ITEM

- Legacy value: 269 (0x010D)
- Modern value: 14747 (0x399B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2016
- Fields:
  - `WriteUInt8(slot2)`
  - `WriteUInt8(slot1)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SWAP_INV_ITEM);
byte slot1 = ModernVersion.AdjustInventorySlot(item.Slot1);
byte slot2 = ModernVersion.AdjustInventorySlot(item.Slot2);
// Modern client: Slot2=source, Slot1=destination (reversed from field names)
// Legacy server expects: srcSlot first, dstSlot second
packet.WriteUInt8(slot2);
packet.WriteUInt8(slot1);
this.SendPacketToServer(packet);
}
```

---

### CMSG_SWAP_ITEM

- Legacy value: 268 (0x010C)
- Modern value: 14746 (0x399A)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2029
- Fields:
  - `WriteUInt8(containerSlotB)`
  - `WriteUInt8(slotB)`
  - `WriteUInt8(containerSlotA)`
  - `WriteUInt8(slotA)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_SWAP_ITEM);
byte containerSlotB = ((item.ContainerSlotB != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ContainerSlotB) : item.ContainerSlotB);
byte slotB = ((item.ContainerSlotB == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.SlotB) : item.SlotB);
byte containerSlotA = ((item.ContainerSlotA != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ContainerSlotA) : item.ContainerSlotA);
byte slotA = ((item.ContainerSlotA == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.SlotA) : item.SlotA);
packet.WriteUInt8(containerSlotB);
packet.WriteUInt8(slotB);
packet.WriteUInt8(containerSlotA);
packet.WriteUInt8(slotA);
this.SendPacketToServer(packet);
}
```

---

### CMSG_TALK_TO_GOSSIP

- Legacy value: 379 (0x017B)
- Modern value: 13458 (0x3492)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3054
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_TAXI_NODE_STATUS_QUERY

- Legacy value: 426 (0x01AA)
- Modern value: 13480 (0x34A8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4217
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_TAXI_QUERY_AVAILABLE_NODES

- Legacy value: 428 (0x01AC)
- Modern value: 13482 (0x34AA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4218
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_TIME_SYNC_RESPONSE

- Legacy value: 913 (0x0391)
- Modern value: 14909 (0x3A3D)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2394
- Fields:
  - `WriteUInt32(response.SequenceIndex)`
  - `WriteUInt32(response.ClientTime)`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TIME_SYNC_RESPONSE);
packet.WriteUInt32(response.SequenceIndex);
packet.WriteUInt32(response.ClientTime);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_TOGGLE_PVP

- Legacy value: 595 (0x0253)
- Modern value: 12971 (0x32AB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:680

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TOGGLE_PVP);
this.SendPacketToServer(packet);
}
```

---

### CMSG_TOTEM_DESTROYED

- Legacy value: 1044 (0x0414)
- Modern value: 13560 (0x34F8)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4157
- Fields:
  - `WriteUInt8(totem.Slot)`

```csharp
{
if (!LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TOTEM_DESTROYED);
packet.WriteUInt8(totem.Slot);
this.SendPacketToServer(packet);
}
}
```

---

### CMSG_TRAINER_BUY_SPELL

- Legacy value: 434 (0x01B2)
- Modern value: 13486 (0x34AE)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3091
- Fields:
  - `WriteGuid(buy.TrainerGUID.To64()`
  - `WriteUInt32(buy.SpellID)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TRAINER_BUY_SPELL);
packet.WriteGuid(buy.TrainerGUID.To64());
if (ModernVersion.ExpansionVersion > 1 && LegacyVersion.ExpansionVersion <= 1)
{
buy.SpellID = this.GetSession().GameState.GetLearnSpellFromRealSpell(buy.SpellID);
}
packet.WriteUInt32(buy.SpellID);
this.SendPacketToServer(packet);
}
```

---

### CMSG_TRAINER_LIST

- Legacy value: 432 (0x01B0)
- Modern value: 13485 (0x34AD)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3055
- Fields:
  - `WriteGuid(interact.CreatureGUID.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(interact.GetUniversalOpcode());
packet.WriteGuid(interact.CreatureGUID.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_TURN_IN_PETITION

- Legacy value: 452 (0x01C4)
- Modern value: 13621 (0x3535)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3342
- Fields:
  - `WriteGuid(petition.Item.To64()`
  - `WriteUInt32(petition.BackgroundColor)`
  - `WriteUInt32(petition.EmblemStyle)`
  - `WriteUInt32(petition.EmblemColor)`
  - `WriteUInt32(petition.BorderStyle)`
  - `WriteUInt32(petition.BorderColor)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TURN_IN_PETITION);
packet.WriteGuid(petition.Item.To64());
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.WriteUInt32(petition.BackgroundColor);
packet.WriteUInt32(petition.EmblemStyle);
packet.WriteUInt32(petition.EmblemColor);
packet.WriteUInt32(petition.BorderStyle);
packet.WriteUInt32(petition.BorderColor);
}
this.SendPacketToServer(packet);
}
```

---

### CMSG_TUTORIAL_FLAG

- Legacy value: 254 (0x00FE)
- Modern value: 14052 (0x36E4)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2485, HermesProxy/World/Server/WorldSocket.cs:2796
- Fields:
  - `WriteUInt32(tutorial.TutorialBit)`

```csharp
{
switch (tutorial.Action)
{
case TutorialAction.Clear:
{
WorldPacket packet3 = new WorldPacket(Opcode.CMSG_TUTORIAL_CLEAR);
this.SendPacketToServer(packet3);
break;
}
case TutorialAction.Reset:
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_TUTORIAL_RESET);
this.SendPacketToServer(packet2);
break;
}
case TutorialAction.Update:
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_TUTORIAL_FLAG);
packet.WriteUInt32(tutorial.TutorialBit);
this.SendPacketToServer(packet);
break;
}
}
}
```

---

### CMSG_UNACCEPT_TRADE

- Legacy value: 283 (0x011B)
- Modern value: 12635 (0x315B)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4396

```csharp
{
WorldPacket packet = new WorldPacket(trade.GetUniversalOpcode());
this.SendPacketToServer(packet);
}
```

---

### CMSG_UNLEARN_SKILL

- Legacy value: 514 (0x0202)
- Modern value: 13541 (0x34E5)
- Handler: HermesProxy/World/Server/WorldSocket.cs:713
- Fields:
  - `WriteUInt32(skill.SkillLine)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_UNLEARN_SKILL);
packet.WriteUInt32(skill.SkillLine);
this.SendPacketToServer(packet);
}
```

---

### CMSG_UNSTABLE_PET

- Legacy value: 625 (0x0271)
- Modern value: 12649 (0x3169)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3209
- Fields:
  - `WriteGuid(pet.StableMaster.To64()`
  - `WriteUInt32(pet.PetNumber)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_UNSTABLE_PET);
packet.WriteGuid(pet.StableMaster.To64());
packet.WriteUInt32(pet.PetNumber);
this.SendPacketToServer(packet);
}
```

---

### CMSG_UPDATE_ACCOUNT_DATA

- Legacy value: 523 (0x020B)
- Modern value: 13973 (0x3695)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1069

```csharp
{
this.GetSession().AccountDataMgr.SaveData(data.PlayerGuid, data.Time, data.DataType, data.Size, data.CompressedData);
}
```

---

### CMSG_UPDATE_RAID_TARGET

- Legacy value: N/A
- Modern value: 13907 (0x3653)
- Handler: HermesProxy/World/Server/WorldSocket.cs:1277
- Fields:
  - `WriteGuid(update.Target.To64()`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.MSG_RAID_TARGET_UPDATE);
packet.WriteInt8(update.Symbol);
packet.WriteGuid(update.Target.To64());
this.SendPacketToServer(packet);
}
```

---

### CMSG_UPDATE_VAS_PURCHASE_STATES

- Legacy value: N/A
- Modern value: 14075 (0x36FB)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2685

```csharp
{
}
```

---

### CMSG_USE_ITEM

- Legacy value: 171 (0x00AB)
- Modern value: 12952 (0x3298)
- Handler: HermesProxy/World/Server/WorldSocket.cs:4001
- Fields:
  - `WriteUInt8(containerSlot)`
  - `WriteUInt8(slot)`
  - `WriteUInt8(this.GetSession()`
  - `WriteUInt32(use.Cast.SpellID)`
  - `WriteGuid(use.CastItem.To64()`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`

```csharp
{
if (Settings.ServerSpellDelay > 0)
{
Thread.Sleep(Settings.ServerSpellDelay);
}
ClientCastRequest castRequest = new ClientCastRequest();
castRequest.Timestamp = Environment.TickCount;
castRequest.SpellId = use.Cast.SpellID;
castRequest.SpellXSpellVisualId = use.Cast.SpellXSpellVisualID;
castRequest.ClientGUID = use.Cast.CastID;
castRequest.ServerGUID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, this.GetSession().GameState.CurrentMapId.Value, use.Cast.SpellID, 10000 + use.Cast.CastID.GetCounter());
castRequest.ItemGUID = use.CastItem;
Log.Print(LogType.Debug, $"[UseItem] SpellID={use.Cast.SpellID} PackSlot={use.PackSlot} Slot={use.Slot} ItemGUID={use.CastItem} PendingCast={this.GetSession().GameState.CurrentClientNormalCast != null}", "HandleUseItem", "");
if (this.GetSession().GameState.CurrentClientNormalCast != null)
{
if (this.GetSession().GameState.CurrentClientNormalCast.HasStarted)
{
this.SendCastRequestFailed(castRequest, isPet: false);
}
else if (this.GetSession().GameState.CurrentClientNormalCast.Timestamp + 10000 < castRequest.Timestamp)
{
Log.Print(LogType.Warn, $"Clearing CurrentClientNormalCast because of 10 sec timeout! (oldSpell:{this.GetSession().GameState.CurrentClientNormalCast.SpellId} newSpell:{castRequest.SpellId})", "HandleUseItem", "SpellHandler.cs");
this.SendCastRequestFailed(this.GetSession().GameState.CurrentClientNormalCast, isPet: false);
this.GetSession().GameState.CurrentClientNormalCast = null;
foreach (ClientCastRequest pending in this.GetSession().GameState.PendingClientCasts)
{
this.SendCastRequestFailed(pending, isPet: false);
}
this.GetSession().GameState.PendingClientCasts.Clear();
this.SendCastRequestFailed(castRequest, isPet: false);
```

---

### CMSG_VIOLENCE_LEVEL

- Legacy value: N/A
- Modern value: 12679 (0x3187)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2645

```csharp
{
}
```

---

### CMSG_WHO

- Legacy value: 98 (0x0062)
- Modern value: 13955 (0x3683)
- Handler: HermesProxy/World/Server/WorldSocket.cs:3429
- Fields:
  - `WriteInt32(who.Request.MinLevel)`
  - `WriteInt32(who.Request.MaxLevel)`
  - `WriteCString(who.Request.Name)`
  - `WriteCString(who.Request.Guild)`
  - `WriteInt32((int)`
  - `WriteInt32(who.Request.ClassFilter)`
  - `WriteInt32(who.Areas.Count)`
  - `WriteInt32(area)`
  - `WriteInt32(who.Request.Words.Count)`
  - `WriteCString(word)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_WHO);
packet.WriteInt32(who.Request.MinLevel);
packet.WriteInt32(who.Request.MaxLevel);
packet.WriteCString(who.Request.Name);
packet.WriteCString(who.Request.Guild);
packet.WriteInt32((int)who.Request.RaceFilter);
packet.WriteInt32(who.Request.ClassFilter);
packet.WriteInt32(who.Areas.Count);
foreach (int area in who.Areas)
{
packet.WriteInt32(area);
}
packet.WriteInt32(who.Request.Words.Count);
foreach (string word in who.Request.Words)
{
packet.WriteCString(word);
}
this.SendPacketToServer(packet);
this.GetSession().GameState.LastWhoRequestId = who.RequestID;
}
```

---

### CMSG_WORLD_PORT_RESPONSE

- Legacy value: N/A
- Modern value: 13818 (0x35FA)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2902

```csharp
{
this.GetSession().GameState.IsWaitingForWorldPortAck = false;
WorldPacket packet = new WorldPacket(Opcode.MSG_MOVE_WORLDPORT_ACK);
this.SendPacketToServer(packet);
}
```

---

### CMSG_WRAP_ITEM

- Legacy value: 467 (0x01D3)
- Modern value: 14740 (0x3994)
- Handler: HermesProxy/World/Server/WorldSocket.cs:2158
- Fields:
  - `WriteUInt8(giftBag)`
  - `WriteUInt8(giftSlot)`
  - `WriteUInt8(itemBag)`
  - `WriteUInt8(itemSlot)`

```csharp
{
WorldPacket packet = new WorldPacket(Opcode.CMSG_WRAP_ITEM);
byte giftBag = ((item.GiftBag != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.GiftBag) : item.GiftBag);
byte giftSlot = ((item.GiftBag == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.GiftSlot) : item.GiftSlot);
byte itemBag = ((item.ItemBag != byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ItemBag) : item.ItemBag);
byte itemSlot = ((item.ItemBag == byte.MaxValue) ? ModernVersion.AdjustInventorySlot(item.ItemSlot) : item.ItemSlot);
packet.WriteUInt8(giftBag);
packet.WriteUInt8(giftSlot);
packet.WriteUInt8(itemBag);
packet.WriteUInt8(itemSlot);
this.SendPacketToServer(packet);
}
```

---

## SMSG Handlers

### SMSG_ACCOUNT_DATA_TIMES

- Legacy value: 521 (0x0209)
- Modern value: 9994 (0x270A)
- Handler: HermesProxy/World/Client/WorldClient.cs:4815

```csharp
{
this.GetSession().RealmSocket.SendAccountDataTimes();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().RealmSocket.SendFeatureSystemStatus();
this.GetSession().RealmSocket.SendMotd();
this.GetSession().RealmSocket.SendSetTimeZoneInformation();
this.GetSession().RealmSocket.SendSeasonInfo();
}
}
```

---

### SMSG_ACHIEVEMENT_EARNED

- Legacy value: 1128 (0x0468)
- Modern value: 9795 (0x2643)
- Handler: HermesProxy/World/Client/WorldClient.cs:5178
- Fields:
  - `ReadPackedGuid -> playerGuid64`
  - `ReadUInt32 -> AchievementID`
  - `ReadUInt32 -> datePackedTime`
  - `ReadUInt32`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
AchievementEarnedPkt earned = new AchievementEarnedPkt();
WowGuid64 playerGuid64 = packet.ReadPackedGuid();
earned.AchievementID = packet.ReadUInt32();
uint datePackedTime = packet.ReadUInt32();
earned.Time = (long)Time.GetUnixTimeFromPackedTime(datePackedTime);
packet.ReadUInt32(); // unknown/reserved (0)

uint realmAddress = this.GetSession().RealmId.GetAddress();
earned.Sender = this.GetSession().GameState.CurrentPlayerGuid;
earned.Earner = playerGuid64.To128(this.GetSession().GameState);
earned.EarnerNativeRealm = realmAddress;
earned.EarnerVirtualRealm = realmAddress;
earned.Initial = false;
this.SendPacketToClient(earned);
}
}
```

---

### SMSG_ACTIVATE_TAXI_REPLY

- Legacy value: 430 (0x01AE)
- Modern value: 9853 (0x267D)
- Handler: HermesProxy/World/Client/WorldClient.cs:8926
- Fields:
  - `ReadUInt32 -> reply`

```csharp
{
ActivateTaxiReply reply = (ActivateTaxiReply)packet.ReadUInt32();
if (reply != ActivateTaxiReply.Ok)
{
ActivateTaxiReplyPkt taxi = new ActivateTaxiReplyPkt();
taxi.Reply = reply;
this.SendPacketToClient(taxi);
this.GetSession().GameState.IsWaitingForTaxiStart = false;
}
}
```

---

### SMSG_AI_REACTION

- Legacy value: 316 (0x013C)
- Modern value: 9909 (0x26B5)
- Handler: HermesProxy/World/Client/WorldClient.cs:2185
- Fields:
  - `ReadGuid -> UnitGUID`
  - `ReadUInt32 -> Reaction`

```csharp
{
AIReaction reaction = new AIReaction();
reaction.UnitGUID = packet.ReadGuid().To128(this.GetSession().GameState);
reaction.Reaction = packet.ReadUInt32();
this.SendPacketToClient(reaction);
}
```

---

### SMSG_ALL_ACHIEVEMENT_DATA

- Legacy value: 1149 (0x047D)
- Modern value: 9584 (0x2570)
- Handler: HermesProxy/World/Client/WorldClient.cs:5102
- Fields:
  - `ReadUInt32 -> achievementId`
  - `ReadUInt32 -> datePackedTime`
  - `ReadUInt32 -> criteriaId`
  - `ReadPackedGuid -> counter`
  - `ReadPackedGuid -> playerGuid64`
  - `ReadUInt32 -> timedFlag`
  - `ReadUInt32 -> datePackedTime`
  - `ReadUInt32 -> timeFromStart`
  - `ReadUInt32 -> timeFromCreate`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
AllAchievementData data = new AllAchievementData();
uint realmAddress = this.GetSession().RealmId.GetAddress();
WowGuid128 playerGuid = this.GetSession().GameState.CurrentPlayerGuid;

// 3.3.5a format: earned achievements (terminated by -1), then criteria progress (terminated by -1)
// Earned achievements
while (true)
{
uint achievementId = packet.ReadUInt32();
if (achievementId == 0xFFFFFFFF) // -1 terminator
break;
uint datePackedTime = packet.ReadUInt32();
long dateUnix = (long)Time.GetUnixTimeFromPackedTime(datePackedTime);

EarnedAchievement earned = new EarnedAchievement();
earned.Id = achievementId;
earned.Date = dateUnix;
earned.Owner = playerGuid;
earned.VirtualRealmAddress = realmAddress;
earned.NativeRealmAddress = realmAddress;
data.Earned.Add(earned);
}

// Criteria progress
while (true)
{
uint criteriaId = packet.ReadUInt32();
```

---

### SMSG_AREA_SPIRIT_HEALER_TIME

- Legacy value: 740 (0x02E4)
- Modern value: 10048 (0x2740)
- Handler: HermesProxy/World/Client/WorldClient.cs:815
- Fields:
  - `ReadGuid -> HealerGuid`
  - `ReadUInt32 -> TimeLeft`

```csharp
{
AreaSpiritHealerTime healer = new AreaSpiritHealerTime();
healer.HealerGuid = packet.ReadGuid().To128(this.GetSession().GameState);
healer.TimeLeft = packet.ReadUInt32();
this.SendPacketToClient(healer);
}
```

---

### SMSG_AREA_TRIGGER_MESSAGE

- Legacy value: 696 (0x02B8)
- Modern value: 10368 (0x2880)
- Handler: HermesProxy/World/Client/WorldClient.cs:4915
- Fields:
  - `ReadUInt32 -> length`
  - `ReadString -> message`

```csharp
{
uint length = packet.ReadUInt32();
string message = packet.ReadString(length);
if (this.GetSession().GameState.LastEnteredAreaTrigger != 0)
{
AreaTriggerMessage denied = new AreaTriggerMessage();
denied.AreaTriggerID = this.GetSession().GameState.LastEnteredAreaTrigger;
this.SendPacketToClient(denied);
}
else
{
ChatPkt chat = new ChatPkt(this.GetSession(), ChatMessageTypeModern.System, message);
this.SendPacketToClient(chat);
}
}
```

---

### SMSG_ARENA_TEAM_COMMAND_RESULT

- Legacy value: 841 (0x0349)
- Modern value: 10083 (0x2763)
- Handler: HermesProxy/World/Client/WorldClient.cs:173
- Fields:
  - `ReadUInt32 -> Action`
  - `ReadCString -> TeamName`
  - `ReadCString -> PlayerName`
  - `ReadUInt32 -> errorType`

```csharp
{
ArenaTeamCommandResult arena = new ArenaTeamCommandResult();
arena.Action = (ArenaTeamCommandType)packet.ReadUInt32();
arena.TeamName = packet.ReadCString();
arena.PlayerName = packet.ReadCString();
ArenaTeamCommandErrorLegacy errorType = (ArenaTeamCommandErrorLegacy)packet.ReadUInt32();
arena.Error = (ArenaTeamCommandErrorModern)Enum.Parse(typeof(ArenaTeamCommandErrorModern), errorType.ToString());
this.SendPacketToClient(arena);
}
```

---

### SMSG_ARENA_TEAM_EVENT

- Legacy value: 855 (0x0357)
- Modern value: 10082 (0x2762)
- Handler: HermesProxy/World/Client/WorldClient.cs:143
- Fields:
  - `ReadUInt8 -> eventType`
  - `ReadUInt8 -> count`
  - `ReadCString -> str`
  - `ReadGuid`

```csharp
{
ArenaTeamEvent arena = new ArenaTeamEvent();
ArenaTeamEventLegacy eventType = (ArenaTeamEventLegacy)packet.ReadUInt8();
arena.Event = (ArenaTeamEventModern)Enum.Parse(typeof(ArenaTeamEventModern), eventType.ToString());
byte count = packet.ReadUInt8();
for (byte i = 0; i < count; i++)
{
string str = packet.ReadCString();
switch (i)
{
case 0:
arena.Param1 = str;
break;
case 1:
arena.Param2 = str;
break;
case 2:
arena.Param3 = str;
break;
}
}
if (packet.CanRead())
{
packet.ReadGuid();
}
this.SendPacketToClient(arena);
}
```

---

### SMSG_ARENA_TEAM_INVITE

- Legacy value: 848 (0x0350)
- Modern value: 10081 (0x2761)
- Handler: HermesProxy/World/Client/WorldClient.cs:185
- Fields:
  - `ReadCString -> PlayerName`
  - `ReadCString -> TeamName`

```csharp
{
ArenaTeamInvite arena = new ArenaTeamInvite();
arena.PlayerName = packet.ReadCString();
arena.TeamName = packet.ReadCString();
arena.PlayerGuid = this.GetSession().GameState.GetPlayerGuidByName(arena.PlayerName);
if (arena.PlayerGuid == null)
{
arena.PlayerGuid = WowGuid128.Empty;
}
arena.PlayerVirtualAddress = this.GetSession().RealmId.GetAddress();
arena.TeamGuid = WowGuid128.Create(HighGuidType703.ArenaTeam, 1uL);
this.SendPacketToClient(arena);
}
```

---

### SMSG_ARENA_TEAM_QUERY_RESPONSE

- Legacy value: 844 (0x034C)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:61
- Fields:
  - `ReadUInt32 -> teamId`
  - `ReadCString -> Name`
  - `ReadUInt32 -> TeamSize`
  - `ReadUInt32 -> BackgroundColor`
  - `ReadUInt32 -> EmblemStyle`
  - `ReadUInt32 -> EmblemColor`
  - `ReadUInt32 -> BorderStyle`
  - `ReadUInt32 -> BorderColor`

```csharp
{
uint teamId = packet.ReadUInt32();
if (!this.GetSession().GameState.ArenaTeams.TryGetValue(teamId, out var team))
{
team = new ArenaTeamData();
this.GetSession().GameState.ArenaTeams.Add(teamId, team);
}
team.Name = packet.ReadCString();
team.TeamSize = packet.ReadUInt32();
team.BackgroundColor = packet.ReadUInt32();
team.EmblemStyle = packet.ReadUInt32();
team.EmblemColor = packet.ReadUInt32();
team.BorderStyle = packet.ReadUInt32();
team.BorderColor = packet.ReadUInt32();
}
```

---

### SMSG_ARENA_TEAM_ROSTER

- Legacy value: 846 (0x034E)
- Modern value: 10080 (0x2760)
- Handler: HermesProxy/World/Client/WorldClient.cs:96
- Fields:
  - `ReadUInt32 -> TeamId`
  - `ReadBool`
  - `ReadUInt32 -> count`
  - `ReadUInt32 -> TeamSize`
  - `ReadGuid -> MemberGUID`
  - `ReadBool -> Online`
  - `ReadCString -> Name`
  - `ReadInt32 -> Captain`
  - `ReadUInt8 -> Level`
  - `ReadUInt8 -> ClassId`
  - `ReadUInt32 -> WeekGamesPlayed`
  - `ReadUInt32 -> WeekGamesWon`
  - `ReadUInt32 -> SeasonGamesPlayed`
  - `ReadUInt32 -> SeasonGamesWon`
  - `ReadUInt32 -> PersonalRating`
  - `ReadFloat -> dword60`
  - `ReadFloat -> dword68`

```csharp
{
ArenaTeamRosterResponse arena = new ArenaTeamRosterResponse();
arena.TeamId = packet.ReadUInt32();
bool hiddenRating = false;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_8_9464))
{
packet.ReadBool();
}
uint count = packet.ReadUInt32();
arena.TeamSize = packet.ReadUInt32();
for (int i = 0; i < count; i++)
{
ArenaTeamMember member = default(ArenaTeamMember);
PlayerCache cache = new PlayerCache();
member.MemberGUID = packet.ReadGuid().To128(this.GetSession().GameState);
member.Online = packet.ReadBool();
member.Name = (cache.Name = packet.ReadCString());
member.Captain = packet.ReadInt32();
member.Level = (cache.Level = packet.ReadUInt8());
member.ClassId = (cache.ClassId = (Class)packet.ReadUInt8());
this.GetSession().GameState.UpdatePlayerCache(member.MemberGUID, cache);
member.WeekGamesPlayed = packet.ReadUInt32();
member.WeekGamesWon = packet.ReadUInt32();
member.SeasonGamesPlayed = packet.ReadUInt32();
member.SeasonGamesWon = packet.ReadUInt32();
member.PersonalRating = packet.ReadUInt32();
if (hiddenRating)
{
member.dword60 = packet.ReadFloat();
member.dword68 = packet.ReadFloat();
```

---

### SMSG_ARENA_TEAM_STATS

- Legacy value: 859 (0x035B)
- Modern value: 10084 (0x2764)
- Handler: HermesProxy/World/Client/WorldClient.cs:79
- Fields:
  - `ReadUInt32 -> teamId`
  - `ReadUInt32 -> Rating`
  - `ReadUInt32 -> WeekPlayed`
  - `ReadUInt32 -> WeekWins`
  - `ReadUInt32 -> SeasonPlayed`
  - `ReadUInt32 -> SeasonWins`
  - `ReadUInt32 -> Rank`

```csharp
{
uint teamId = packet.ReadUInt32();
if (!this.GetSession().GameState.ArenaTeams.TryGetValue(teamId, out var team))
{
team = new ArenaTeamData();
this.GetSession().GameState.ArenaTeams.Add(teamId, team);
}
team.Rating = packet.ReadUInt32();
team.WeekPlayed = packet.ReadUInt32();
team.WeekWins = packet.ReadUInt32();
team.SeasonPlayed = packet.ReadUInt32();
team.SeasonWins = packet.ReadUInt32();
team.Rank = packet.ReadUInt32();
}
```

---

### SMSG_ATTACKER_STATE_UPDATE

- Legacy value: 330 (0x014A)
- Modern value: 10578 (0x2952)
- Handler: HermesProxy/World/Client/WorldClient.cs:2067
- Fields:
  - `ReadUInt32 -> hitInfo`
  - `ReadPackedGuid -> AttackerGUID`
  - `ReadPackedGuid -> VictimGUID`
  - `ReadInt32 -> Damage`
  - `ReadInt32 -> OverDamage`
  - `ReadUInt8 -> subDamageCount`
  - `ReadUInt32 -> school`
  - `ReadFloat -> FloatDamage`
  - `ReadInt32 -> IntDamage`
  - `ReadInt32 -> Absorbed`
  - `ReadInt32 -> Resisted`
  - `ReadUInt8 -> VictimState`
  - `ReadUInt32 -> VictimState`
  - `ReadInt32 -> AttackerState`
  - `ReadUInt32 -> MeleeSpellID`
  - `ReadInt32 -> BlockAmount`
  - `ReadInt32 -> RageGained`
  - `ReadUInt32 -> State1`
  - `ReadFloat -> State2`
  - `ReadFloat -> State3`
  - `ReadFloat -> State4`
  - `ReadFloat -> State5`
  - `ReadFloat -> State6`
  - `ReadFloat -> State7`
  - `ReadFloat -> State8`
  - `ReadFloat -> State9`
  - `ReadFloat -> State10`
  - `ReadFloat -> State11`
  - `ReadUInt32 -> State12`
  - `ReadUInt32`
  - `ReadUInt32`

```csharp
{
AttackerStateUpdate attack = new AttackerStateUpdate();
uint hitInfo = packet.ReadUInt32();
attack.HitInfo = LegacyVersion.ConvertHitInfoFlags(hitInfo);
attack.AttackerGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
attack.VictimGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
attack.Damage = packet.ReadInt32();
attack.OriginalDamage = attack.Damage;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
{
attack.OverDamage = packet.ReadInt32();
}
else
{
attack.OverDamage = -1;
}
byte subDamageCount = packet.ReadUInt8();
for (int i = 0; i < subDamageCount; i++)
{
SubDamage subDmg = new SubDamage();
uint school = packet.ReadUInt32();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
school = (uint)(1 << (int)(byte)school);
}
subDmg.SchoolMask = school;
subDmg.FloatDamage = packet.ReadFloat();
subDmg.IntDamage = packet.ReadInt32();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_3_9183) || hitInfo.HasAnyFlag(HitInfo.FullAbsorb | HitInfo.PartialAbsorb))
{
```

---

### SMSG_ATTACKSWING_BADFACING

- Legacy value: 326 (0x0146)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2154

```csharp
{
AttackSwingError attack = new AttackSwingError();
attack.Reason = AttackSwingErr.BadFacing;
this.SendPacketToClient(attack);
}
```

---

### SMSG_ATTACKSWING_CANT_ATTACK

- Legacy value: 329 (0x0149)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2170

```csharp
{
AttackSwingError attack = new AttackSwingError();
attack.Reason = AttackSwingErr.CantAttack;
this.SendPacketToClient(attack);
}
```

---

### SMSG_ATTACKSWING_DEADTARGET

- Legacy value: 328 (0x0148)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2162

```csharp
{
AttackSwingError attack = new AttackSwingError();
attack.Reason = AttackSwingErr.DeadTarget;
this.SendPacketToClient(attack);
}
```

---

### SMSG_ATTACKSWING_NOTINRANGE

- Legacy value: 325 (0x0145)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2146

```csharp
{
AttackSwingError attack = new AttackSwingError();
attack.Reason = AttackSwingErr.NotInRange;
this.SendPacketToClient(attack);
}
```

---

### SMSG_ATTACK_START

- Legacy value: 323 (0x0143)
- Modern value: 10557 (0x293D)
- Handler: HermesProxy/World/Client/WorldClient.cs:2015
- Fields:
  - `ReadGuid -> Attacker`
  - `ReadGuid -> Victim`

```csharp
{
SAttackStart attack = new SAttackStart();
attack.Attacker = packet.ReadGuid().To128(this.GetSession().GameState);
attack.Victim = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(attack);
}
```

---

### SMSG_ATTACK_STOP

- Legacy value: 324 (0x0144)
- Modern value: 10558 (0x293E)
- Handler: HermesProxy/World/Client/WorldClient.cs:2024
- Fields:
  - `ReadPackedGuid -> Attacker`
  - `ReadPackedGuid -> Victim`
  - `ReadUInt32 -> NowDead`

```csharp
{
SAttackStop attack = new SAttackStop();
if (packet.CanRead())
{
attack.Attacker = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
if (packet.CanRead())
{
attack.Victim = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
if (packet.CanRead())
{
attack.NowDead = packet.ReadUInt32() != 0;
}
this.SendPacketToClient(attack);
}
```

---

### SMSG_AUCTION_BIDDER_NOTIFICATION

- Legacy value: 606 (0x025E)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:358
- Fields:
  - `ReadUInt32 -> auctionHouseId`
  - `ReadUInt32 -> AuctionID`
  - `ReadGuid -> Bidder`
  - `ReadUInt32 -> bidAmount`
  - `ReadUInt32 -> minIncrement`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesID`

```csharp
{
AuctionBidderNotification info = new AuctionBidderNotification();
uint auctionHouseId = packet.ReadUInt32();
info.AuctionID = packet.ReadUInt32();
info.Bidder = packet.ReadGuid().To128(this.GetSession().GameState);
uint bidAmount = packet.ReadUInt32();
uint minIncrement = packet.ReadUInt32();
info.Item.ItemID = packet.ReadUInt32();
info.Item.RandomPropertiesID = packet.ReadUInt32();
if (bidAmount == 0)
{
AuctionWonNotification auction = new AuctionWonNotification();
auction.Info = info;
this.SendPacketToClient(auction);
}
else
{
AuctionOutbidNotification auction2 = new AuctionOutbidNotification();
auction2.Info = info;
auction2.BidAmount = bidAmount;
auction2.MinIncrement = minIncrement;
this.SendPacketToClient(auction2);
}
}
```

---

### SMSG_AUCTION_COMMAND_RESULT

- Legacy value: 603 (0x025B)
- Modern value: 9968 (0x26F0)
- Handler: HermesProxy/World/Client/WorldClient.cs:302
- Fields:
  - `ReadUInt32 -> AuctionID`
  - `ReadUInt32 -> Command`
  - `ReadUInt32 -> ErrorCode`
  - `ReadUInt32 -> MinIncrement`
  - `ReadUInt32 -> BagResult`
  - `ReadGuid -> Guid`
  - `ReadUInt32 -> Money`
  - `ReadUInt32 -> MinIncrement`

```csharp
{
AuctionCommandResult auction = new AuctionCommandResult();
auction.AuctionID = packet.ReadUInt32();
auction.Command = (AuctionHouseAction)packet.ReadUInt32();
auction.ErrorCode = (AuctionHouseError)packet.ReadUInt32();
switch (auction.ErrorCode)
{
case AuctionHouseError.Ok:
if (auction.Command == AuctionHouseAction.Bid)
{
auction.MinIncrement = packet.ReadUInt32();
}
break;
case AuctionHouseError.Inventory:
auction.BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32());
break;
case AuctionHouseError.HigherBid:
auction.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
auction.Money = packet.ReadUInt32();
auction.MinIncrement = packet.ReadUInt32();
break;
}
this.SendPacketToClient(auction);
}
```

---

### SMSG_AUCTION_LIST_BIDDED_ITEMS_RESULT

- Legacy value: 613 (0x0265)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:264
- Fields:
  - `ReadUInt32 -> count`
  - `ReadInt32 -> TotalItemsCount`
  - `ReadUInt32 -> DesiredDelay`

```csharp
{
AuctionListMyItemsResult auction = new AuctionListMyItemsResult(packet.GetUniversalOpcode(isModern: false));
uint count = packet.ReadUInt32();
for (uint i = 0u; i < count; i++)
{
AuctionItem item = this.ReadAuctionItem(packet);
auction.Items.Add(item);
}
auction.TotalItemsCount = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
{
auction.DesiredDelay = packet.ReadUInt32();
}
this.SendPacketToClient(auction);
}
```

---

### SMSG_AUCTION_LIST_ITEMS_RESULT

- Legacy value: 604 (0x025C)
- Modern value: 10339 (0x2863)
- Handler: HermesProxy/World/Client/WorldClient.cs:283
- Fields:
  - `ReadUInt32 -> count`
  - `ReadInt32 -> TotalItemsCount`
  - `ReadUInt32 -> DesiredDelay`

```csharp
{
AuctionListItemsResult auction = new AuctionListItemsResult();
uint count = packet.ReadUInt32();
for (uint i = 0u; i < count; i++)
{
AuctionItem item = this.ReadAuctionItem(packet);
item.CensorServerSideInfo = true;
auction.Items.Add(item);
}
auction.TotalItemsCount = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
{
auction.DesiredDelay = packet.ReadUInt32();
}
this.SendPacketToClient(auction);
}
```

---

### SMSG_AUCTION_LIST_OWNED_ITEMS_RESULT

- Legacy value: 605 (0x025D)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:265
- Fields:
  - `ReadUInt32 -> count`
  - `ReadInt32 -> TotalItemsCount`
  - `ReadUInt32 -> DesiredDelay`

```csharp
{
AuctionListMyItemsResult auction = new AuctionListMyItemsResult(packet.GetUniversalOpcode(isModern: false));
uint count = packet.ReadUInt32();
for (uint i = 0u; i < count; i++)
{
AuctionItem item = this.ReadAuctionItem(packet);
auction.Items.Add(item);
}
auction.TotalItemsCount = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
{
auction.DesiredDelay = packet.ReadUInt32();
}
this.SendPacketToClient(auction);
}
```

---

### SMSG_AUCTION_OWNER_NOTIFICATION

- Legacy value: 607 (0x025F)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:329
- Fields:
  - `ReadUInt32 -> AuctionID`
  - `ReadUInt32 -> BidAmount`
  - `ReadUInt32 -> minIncrement`
  - `ReadGuid -> buyer`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadFloat -> mailDelay`

```csharp
{
AuctionOwnerNotification info = new AuctionOwnerNotification();
info.AuctionID = packet.ReadUInt32();
info.BidAmount = packet.ReadUInt32();
uint minIncrement = packet.ReadUInt32();
WowGuid buyer = packet.ReadGuid();
info.Item.ItemID = packet.ReadUInt32();
info.Item.RandomPropertiesID = packet.ReadUInt32();
float mailDelay = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056)) ? 3600f : packet.ReadFloat());
if (buyer.IsEmpty())
{
AuctionClosedNotification auction = new AuctionClosedNotification();
auction.Info = info;
auction.Sold = info.BidAmount != 0;
auction.ProceedsMailDelay = mailDelay;
this.SendPacketToClient(auction);
}
else
{
AuctionOwnerBidNotification auction2 = new AuctionOwnerBidNotification();
auction2.Info = info;
auction2.MinIncrement = minIncrement;
auction2.Bidder = buyer.To128(this.GetSession().GameState);
this.SendPacketToClient(auction2);
}
}
```

---

### SMSG_AURA_UPDATE

- Legacy value: 1174 (0x0496)
- Modern value: 11295 (0x2C1F)
- Handler: HermesProxy/World/Client/WorldClient.cs:8687
- Fields:
  - `ReadPackedGuid -> guid`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
AuraUpdate update = new AuraUpdate(guid, all: false);
this.ReadSingleAura(packet, guid, update);
if (update.Auras.Count > 0)
{
this.SendPacketToClient(update);
}
}
}
```

---

### SMSG_AURA_UPDATE_ALL

- Legacy value: 1173 (0x0495)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:8702
- Fields:
  - `ReadPackedGuid -> guid`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
AuraUpdate update = new AuraUpdate(guid, all: true);
while (packet.CanRead())
{
this.ReadSingleAura(packet, guid, update);
}
if (update.Auras.Count > 0)
{
this.SendPacketToClient(update);
}
}
}
```

---

### SMSG_BATTLEFIELD_LIST

- Legacy value: 573 (0x023D)
- Modern value: 10535 (0x2927)
- Handler: HermesProxy/World/Client/WorldClient.cs:385, HermesProxy/World/Client/WorldClient.cs:402, HermesProxy/World/Client/WorldClient.cs:419
- Fields:
  - `ReadGuid -> BattlemasterGuid`
  - `ReadUInt32 -> BattlemasterListID`
  - `ReadUInt8`
  - `ReadUInt32 -> instancesCount`
  - `ReadInt32 -> instanceId`
  - `ReadGuid -> BattlemasterGuid`
  - `ReadUInt32 -> BattlemasterListID`
  - `ReadUInt8`
  - `ReadUInt32 -> instancesCount`
  - `ReadInt32 -> instanceId`
  - `ReadGuid -> BattlemasterGuid`
  - `ReadBool -> PvpAnywhere`
  - `ReadUInt32 -> BattlemasterListID`
  - `ReadUInt8 -> MinLevel`
  - `ReadUInt8 -> MaxLevel`
  - `ReadBool`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadBool`
  - `ReadBool -> HasRandomWinToday`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadUInt32 -> instancesCount`
  - `ReadInt32 -> instanceId`

```csharp
{
BattlefieldList bglist = new BattlefieldList();
bglist.BattlemasterGuid = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = bglist.BattlemasterGuid;
bglist.BattlemasterListID = GameData.GetBattlegroundIdFromMapId(packet.ReadUInt32());
packet.ReadUInt8();
uint instancesCount = packet.ReadUInt32();
for (int i = 0; i < instancesCount; i++)
{
int instanceId = packet.ReadInt32();
bglist.BattlefieldInstances.Add(instanceId);
}
this.SendPacketToClient(bglist);
}
```

---

### SMSG_BATTLEFIELD_STATUS

- Legacy value: 724 (0x02D4)
- Modern value: 10533 (0x2925)
- Handler: HermesProxy/World/Client/WorldClient.cs:452, HermesProxy/World/Client/WorldClient.cs:538
- Fields:
  - `ReadUInt32 -> Id`
  - `ReadUInt32 -> mapId`
  - `ReadUInt8`
  - `ReadUInt32 -> InstanceID`
  - `ReadUInt32 -> status`
  - `ReadUInt32 -> AverageWaitTime`
  - `ReadUInt32 -> WaitTime`
  - `ReadUInt32 -> Timeout`
  - `ReadUInt32 -> ShutdownTimer`
  - `ReadUInt32 -> StartTimer`
  - `ReadUInt32 -> Id`
  - `ReadUInt8 -> ArenaTeamSize`
  - `ReadUInt8`
  - `ReadUInt32 -> battlefieldListId`
  - `ReadUInt16`
  - `ReadUInt8 -> RangeMin`
  - `ReadUInt8 -> RangeMax`
  - `ReadUInt32 -> InstanceID`
  - `ReadBool -> IsArena`
  - `ReadUInt32 -> status`
  - `ReadUInt32 -> AverageWaitTime`
  - `ReadUInt32 -> WaitTime`
  - `ReadUInt32 -> Mapid`
  - `ReadUInt64`
  - `ReadUInt32 -> Timeout`
  - `ReadUInt32 -> Mapid`
  - `ReadUInt64`
  - `ReadUInt32 -> ShutdownTimer`
  - `ReadUInt32 -> StartTimer`
  - `ReadUInt8 -> ArenaFaction`

```csharp
{
BattlefieldStatusHeader hdr = new BattlefieldStatusHeader();
hdr.Ticket.Id = 1 + packet.ReadUInt32();
hdr.Ticket.RequesterGuid = this.GetSession().GameState.CurrentPlayerGuid;
hdr.Ticket.Time = this.GetSession().GameState.GetBattleFieldQueueTime(hdr.Ticket.Id);
hdr.Ticket.Type = RideType.Battlegrounds;
uint mapId = packet.ReadUInt32();
if (mapId != 0)
{
uint battlefieldListId = GameData.GetBattlegroundIdFromMapId(mapId);
hdr.BattlefieldListIDs.Add(battlefieldListId);
packet.ReadUInt8();
hdr.InstanceID = packet.ReadUInt32();
BattleGroundStatus status = (BattleGroundStatus)packet.ReadUInt32();
switch (status)
{
case BattleGroundStatus.WaitQueue:
{
BattlefieldStatusQueued queue = new BattlefieldStatusQueued();
queue.Hdr = hdr;
queue.AverageWaitTime = packet.ReadUInt32();
queue.WaitTime = packet.ReadUInt32();
this.SendPacketToClient(queue);
break;
}
case BattleGroundStatus.WaitJoin:
{
BattlefieldStatusNeedConfirmation confirm = new BattlefieldStatusNeedConfirmation();
confirm.Hdr = hdr;
confirm.Mapid = mapId;
```

---

### SMSG_BATTLEGROUND_PLAYER_JOINED

- Legacy value: 748 (0x02EC)
- Modern value: 10539 (0x292B)
- Handler: HermesProxy/World/Client/WorldClient.cs:806
- Fields:
  - `ReadGuid -> Guid`

```csharp
{
BattlegroundPlayerLeftOrJoined player = new BattlegroundPlayerLeftOrJoined(packet.GetUniversalOpcode(isModern: false));
player.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(player);
}
```

---

### SMSG_BATTLEGROUND_PLAYER_LEFT

- Legacy value: 749 (0x02ED)
- Modern value: 10540 (0x292C)
- Handler: HermesProxy/World/Client/WorldClient.cs:807
- Fields:
  - `ReadGuid -> Guid`

```csharp
{
BattlegroundPlayerLeftOrJoined player = new BattlegroundPlayerLeftOrJoined(packet.GetUniversalOpcode(isModern: false));
player.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(player);
}
```

---

### SMSG_BINDER_CONFIRM

- Legacy value: 747 (0x02EB)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:5980
- Fields:
  - `ReadGuid -> Guid`

```csharp
{
BinderConfirm confirm = new BinderConfirm();
confirm.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = confirm.Guid;
this.SendPacketToClient(confirm);
}
```

---

### SMSG_BIND_POINT_UPDATE

- Legacy value: 341 (0x0155)
- Modern value: 9597 (0x257D)
- Handler: HermesProxy/World/Client/WorldClient.cs:4828
- Fields:
  - `ReadVector3`
  - `ReadUInt32 -> BindMapID`
  - `ReadUInt32 -> BindAreaID`

```csharp
{
BindPointUpdate point = new BindPointUpdate();
point.BindPosition = packet.ReadVector3();
point.BindMapID = packet.ReadUInt32();
point.BindAreaID = packet.ReadUInt32();
this.SendPacketToClient(point);
}
```

---

### SMSG_BUY_FAILED

- Legacy value: 421 (0x01A5)
- Modern value: 9927 (0x26C7)
- Handler: HermesProxy/World/Client/WorldClient.cs:4161
- Fields:
  - `ReadGuid -> VendorGUID`
  - `ReadUInt32 -> Muid`
  - `ReadUInt8 -> Reason`

```csharp
{
BuyFailed fail = new BuyFailed();
fail.VendorGUID = packet.ReadGuid().To128(this.GetSession().GameState);
fail.Muid = packet.ReadUInt32();
fail.Reason = (BuyResult)packet.ReadUInt8();
this.SendPacketToClient(fail);
}
```

---

### SMSG_BUY_SUCCEEDED

- Legacy value: 420 (0x01A4)
- Modern value: 9926 (0x26C6)
- Handler: HermesProxy/World/Client/WorldClient.cs:4071
- Fields:
  - `ReadGuid -> VendorGUID`
  - `ReadUInt32 -> Muid`
  - `ReadInt32 -> NewQuantity`
  - `ReadUInt32 -> QuantityBought`

```csharp
{
BuySucceeded buy = new BuySucceeded();
buy.VendorGUID = packet.ReadGuid().To128(this.GetSession().GameState);
buy.Muid = packet.ReadUInt32();
buy.NewQuantity = packet.ReadInt32();
buy.QuantityBought = packet.ReadUInt32();
this.SendPacketToClient(buy);
}
```

---

### SMSG_CACHE_VERSION

- Legacy value: 1195 (0x04AB)
- Modern value: 10524 (0x291C)
- Handler: HermesProxy/World/Client/WorldClient.cs:5385

```csharp
{
}
```

---

### SMSG_CANCEL_AUTO_REPEAT

- Legacy value: 668 (0x029C)
- Modern value: 9950 (0x26DE)
- Handler: HermesProxy/World/Client/WorldClient.cs:8193
- Fields:
  - `ReadPackedGuid -> Guid`

```csharp
{
if (Settings.ClientSpellDelay > 0)
{
Thread.Sleep(Settings.ClientSpellDelay);
}
if (this.GetSession().GameState.CurrentClientSpecialCast != null && GameData.AutoRepeatSpells.Contains(this.GetSession().GameState.CurrentClientSpecialCast.SpellId))
{
this.GetSession().GameState.CurrentClientSpecialCast = null;
}
CancelAutoRepeat cancel = new CancelAutoRepeat();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
cancel.Guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
else
{
cancel.Guid = this.GetSession().GameState.CurrentPlayerGuid;
}
this.SendPacketToClient(cancel);
}
```

---

### SMSG_CANCEL_COMBAT

- Legacy value: 334 (0x014E)
- Modern value: 10571 (0x294B)
- Handler: HermesProxy/World/Client/WorldClient.cs:2178

```csharp
{
CancelCombat combat = new CancelCombat();
this.SendPacketToClient(combat);
}
```

---

### SMSG_CAST_FAILED

- Legacy value: 304 (0x0130)
- Modern value: 11348 (0x2C54)
- Handler: HermesProxy/World/Client/WorldClient.cs:7724
- Fields:
  - `ReadUInt8`
  - `ReadUInt32 -> spellId`
  - `ReadUInt8 -> status`
  - `ReadUInt8 -> reason`
  - `ReadUInt8`
  - `ReadInt32 -> arg1`
  - `ReadInt32 -> arg2`

```csharp
{
if (Settings.ClientSpellDelay > 0)
{
Thread.Sleep(Settings.ClientSpellDelay);
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.ReadUInt8();
}
uint spellId = packet.ReadUInt32();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
byte status = packet.ReadUInt8();
if (status != 2)
{
return;
}
}
uint reason = packet.ReadUInt8();
if (LegacyVersion.InVersion(ClientVersionBuild.V2_0_1_6180, ClientVersionBuild.V3_0_2_9056))
{
packet.ReadUInt8();
}
int arg1 = 0;
int arg2 = 0;
if (packet.CanRead())
{
arg1 = packet.ReadInt32();
}
if (packet.CanRead())
```

---

### SMSG_CHANNEL_LIST

- Legacy value: 155 (0x009B)
- Modern value: 11204 (0x2BC4)
- Handler: HermesProxy/World/Client/WorldClient.cs:1598
- Fields:
  - `ReadBool -> Display`
  - `ReadCString -> ChannelName`
  - `ReadUInt8 -> ChannelFlags`
  - `ReadInt32 -> count`
  - `ReadGuid -> Guid`
  - `ReadUInt8 -> Flags`

```csharp
{
ChannelListResponse list = new ChannelListResponse();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
list.Display = packet.ReadBool();
}
else
{
list.Display = this.GetSession().GameState.ChannelDisplayList;
}
list.ChannelName = packet.ReadCString();
list.ChannelFlags = (ChannelFlags)packet.ReadUInt8();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
ChannelListResponse.ChannelPlayer member = new ChannelListResponse.ChannelPlayer
{
Guid = packet.ReadGuid().To128(this.GetSession().GameState),
VirtualRealmAddress = this.GetSession().RealmId.GetAddress(),
Flags = packet.ReadUInt8()
};
list.Members.Add(member);
}
this.SendPacketToClient(list);
}
```

---

### SMSG_CHANNEL_NOTIFY

- Legacy value: 153 (0x0099)
- Modern value: 11201 (0x2BC1)
- Handler: HermesProxy/World/Client/WorldClient.cs:1494
- Fields:
  - `ReadUInt8 -> type`
  - `ReadBytes`
  - `ReadCString -> channelName`
  - `ReadGuid`
  - `ReadUInt32 -> flags`
  - `ReadUInt8 -> flags`
  - `ReadInt32 -> channelId`
  - `ReadInt32`
  - `ReadInt32 -> ChatChannelID`
  - `ReadBool -> Suspended`
  - `ReadCString`
  - `ReadGuid`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadGuid`
  - `ReadGuid`
  - `ReadGuid`

```csharp
{
ChatNotify type = (ChatNotify)packet.ReadUInt8();
if (type == ChatNotify.InvalidName)
{
packet.ReadBytes(3u);
}
string channelName = packet.ReadCString();
switch (type)
{
case ChatNotify.Joined:
case ChatNotify.Left:
case ChatNotify.PasswordChanged:
case ChatNotify.OwnerChanged:
case ChatNotify.AnnouncementsOn:
case ChatNotify.AnnouncementsOff:
case ChatNotify.ModerationOn:
case ChatNotify.ModerationOff:
case ChatNotify.PlayerAlreadyMember:
case ChatNotify.Invite:
case ChatNotify.VoiceOn:
case ChatNotify.VoiceOff:
packet.ReadGuid();
break;
case ChatNotify.YouJoined:
{
ChannelFlags flags = (ChannelFlags)((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? packet.ReadUInt32() : packet.ReadUInt8());
int channelId = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.ReadInt32();
```

---

### SMSG_CHARACTER_LOGIN_FAILED

- Legacy value: 65 (0x0041)
- Modern value: 9989 (0x2705)
- Handler: HermesProxy/World/Client/WorldClient.cs:1142
- Fields:
  - `ReadUInt8 -> Code`

```csharp
{
CharacterLoginFailed failed = new CharacterLoginFailed();
failed.Code = (LoginFailureReason)packet.ReadUInt8();
this.SendPacketToClient(failed);
this.GetSession().GameState.IsInWorld = false;
}
```

---

### SMSG_CHARACTER_RENAME_RESULT

- Legacy value: 712 (0x02C8)
- Modern value: 10087 (0x2767)
- Handler: HermesProxy/World/Client/WorldClient.cs:1480
- Fields:
  - `ReadUInt8 -> result`
  - `ReadGuid -> Guid`
  - `ReadCString -> Name`

```csharp
{
byte result = packet.ReadUInt8();
CharacterRenameResult rename = new CharacterRenameResult();
rename.Result = ModernVersion.ConvertResponseCodesValue(result);
if (rename.Result == 0)
{
rename.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
rename.Name = packet.ReadCString();
}
this.SendPacketToClient(rename);
}
```

---

### SMSG_CHAT

- Legacy value: 150 (0x0096)
- Modern value: 11181 (0x2BAD)
- Handler: HermesProxy/World/Client/WorldClient.cs:1626, HermesProxy/World/Client/WorldClient.cs:1695
- Fields:
  - `ReadUInt8 -> chatType`
  - `ReadUInt32 -> language`
  - `ReadUInt32`
  - `ReadCString -> senderName`
  - `ReadGuid -> receiver`
  - `ReadGuid -> sender`
  - `ReadGuid`
  - `ReadGuid -> sender`
  - `ReadUInt32`
  - `ReadCString -> senderName`
  - `ReadGuid -> receiver`
  - `ReadCString -> channelName`
  - `ReadUInt32`
  - `ReadGuid -> sender`
  - `ReadGuid -> sender`
  - `ReadUInt32 -> textLength`
  - `ReadString -> text`
  - `ReadUInt8 -> chatTag`
  - `WriteGuid(sender.To64()`
  - `ReadUInt8 -> chatType`
  - `ReadUInt32 -> language`
  - `ReadGuid -> sender`
  - `ReadInt32`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> senderNameLength3`
  - `ReadString -> senderName`
  - `ReadGuid -> receiver`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> senderNameLength2`
  - `ReadString -> senderName`
  - `ReadUInt32 -> senderNameLength`
  - `ReadString -> senderName`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> receiverNameLength`
  - `ReadString -> receiverName`
  - `ReadUInt32 -> gmNameLength`
  - `ReadString`
  - `ReadCString -> channelName`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> textLength`
  - `ReadString -> text`
  - `ReadUInt8 -> chatFlags`
  - `ReadUInt32 -> gmNameLength2`
  - `ReadString`
  - `ReadUInt32 -> achievementId`
  - `WriteGuid(sender.To64()`
  - `WriteUInt8(0)`

```csharp
{
ChatMessageTypeVanilla chatType = (ChatMessageTypeVanilla)packet.ReadUInt8();
uint language = packet.ReadUInt32();
string senderName = "";
WowGuid128 sender = null;
WowGuid128 receiver = null;
string channelName = "";
switch (chatType)
{
case ChatMessageTypeVanilla.MonsterEmote:
case ChatMessageTypeVanilla.MonsterWhisper:
case ChatMessageTypeVanilla.RaidBossEmote:
packet.ReadUInt32();
senderName = packet.ReadCString();
receiver = packet.ReadGuid().To128(this.GetSession().GameState);
break;
case ChatMessageTypeVanilla.Say:
case ChatMessageTypeVanilla.Party:
case ChatMessageTypeVanilla.Yell:
sender = packet.ReadGuid().To128(this.GetSession().GameState);
packet.ReadGuid();
break;
case ChatMessageTypeVanilla.MonsterSay:
case ChatMessageTypeVanilla.MonsterYell:
sender = packet.ReadGuid().To128(this.GetSession().GameState);
packet.ReadUInt32();
senderName = packet.ReadCString();
receiver = packet.ReadGuid().To128(this.GetSession().GameState);
break;
case ChatMessageTypeVanilla.Channel:
```

---

### SMSG_CHAT_PLAYER_NOTFOUND

- Legacy value: 681 (0x02A9)
- Modern value: 11191 (0x2BB7)
- Handler: HermesProxy/World/Client/WorldClient.cs:1951
- Fields:
  - `ReadCString -> Name`

```csharp
{
ChatPlayerNotfound error = new ChatPlayerNotfound();
error.Name = packet.ReadCString();
this.SendPacketToClient(error);
}
```

---

### SMSG_CHAT_SERVER_MESSAGE

- Legacy value: 657 (0x0291)
- Modern value: 11205 (0x2BC5)
- Handler: HermesProxy/World/Client/WorldClient.cs:1969
- Fields:
  - `ReadInt32 -> MessageID`
  - `ReadCString -> StringParam`

```csharp
{
ChatServerMessage message = new ChatServerMessage();
message.MessageID = packet.ReadInt32();
message.StringParam = packet.ReadCString();
this.SendPacketToClient(message);
}
```

---

### SMSG_CLEAR_COOLDOWN

- Legacy value: 478 (0x01DE)
- Modern value: 9914 (0x26BA)
- Handler: HermesProxy/World/Client/WorldClient.cs:8257
- Fields:
  - `ReadUInt32 -> SpellID`
  - `ReadGuid -> guid`

```csharp
{
ClearCooldown cooldown = new ClearCooldown();
cooldown.SpellID = packet.ReadUInt32();
WowGuid guid = packet.ReadGuid();
cooldown.IsPet = guid.GetHighType() == HighGuidType.Pet;
this.SendPacketToClient(cooldown);
}
```

---

### SMSG_COMPRESSED_MOVES

- Legacy value: 763 (0x02FB)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5724
- Fields:
  - `ReadInt32 -> uncompressedSize`
  - `ReadUInt8 -> size`
  - `ReadUInt16 -> opc`
  - `ReadBytes -> data`

```csharp
{
int uncompressedSize = packet.ReadInt32();
WorldPacket pkt = packet.Inflate(uncompressedSize);
while (pkt.CanRead())
{
byte size = pkt.ReadUInt8();
ushort opc = pkt.ReadUInt16();
byte[] data = pkt.ReadBytes((uint)(size - 2));
WorldPacket pkt2 = new WorldPacket(opc, data);
pkt2.SetReceiveTime(pkt.GetReceivedTime());
this.HandlePacket(pkt2);
}
}
```

---

### SMSG_COMPRESSED_UPDATE_OBJECT

- Legacy value: 502 (0x01F6)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:9083
- Fields:
  - `ReadInt32 -> packet2`

```csharp
{
using WorldPacket packet2 = packet.Inflate(packet.ReadInt32());
this.HandleUpdateObject(packet2);
}
```

---

### SMSG_CONTACT_LIST

- Legacy value: 103 (0x0067)
- Modern value: 10124 (0x278C)
- Handler: HermesProxy/World/Client/WorldClient.cs:7556
- Fields:
  - `ReadUInt32 -> Flags`
  - `ReadUInt32 -> count`
  - `ReadGuid -> Guid`
  - `ReadUInt32 -> TypeFlags`
  - `ReadCString -> Note`
  - `ReadUInt8 -> Status`
  - `ReadUInt32 -> AreaID`
  - `ReadUInt32 -> Level`
  - `ReadUInt32 -> ClassID`

```csharp
{
ContactList contacts = new ContactList();
contacts.Flags = (SocialFlag)packet.ReadUInt32();
uint count = packet.ReadUInt32();
for (int i = 0; i < count; i++)
{
ContactInfo contact = new ContactInfo();
contact.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
contact.WowAccountGuid = this.GetSession().GetGameAccountGuidForPlayer(contact.Guid);
contact.NativeRealmAddr = this.GetSession().RealmId.GetAddress();
contact.VirtualRealmAddr = this.GetSession().RealmId.GetAddress();
contact.TypeFlags = (SocialFlag)packet.ReadUInt32();
contact.Note = packet.ReadCString();
if (contact.TypeFlags.HasAnyFlag(SocialFlag.Friend))
{
contact.Status = (FriendStatus)packet.ReadUInt8();
if (contact.Status != FriendStatus.Offline)
{
contact.AreaID = packet.ReadUInt32();
contact.Level = packet.ReadUInt32();
contact.ClassID = (Class)packet.ReadUInt32();
}
}
contacts.Contacts.Add(contact);
}
this.SendPacketToClient(contacts);
}
```

---

### SMSG_CONTROL_UPDATE

- Legacy value: 345 (0x0159)
- Modern value: 9799 (0x2647)
- Handler: HermesProxy/World/Client/WorldClient.cs:5465
- Fields:
  - `ReadPackedGuid -> Guid`
  - `ReadBool -> HasControl`

```csharp
{
ControlUpdate control = new ControlUpdate();
control.Guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
control.HasControl = packet.ReadBool();
this.SendPacketToClient(control);
}
```

---

### SMSG_COOLDOWN_CHEAT

- Legacy value: 481 (0x01E1)
- Modern value: 10041 (0x2739)
- Handler: HermesProxy/World/Client/WorldClient.cs:8267
- Fields:
  - `ReadGuid -> Guid`

```csharp
{
CooldownCheat cooldown = new CooldownCheat();
cooldown.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(cooldown);
}
```

---

### SMSG_COOLDOWN_EVENT

- Legacy value: 309 (0x0135)
- Modern value: 9913 (0x26B9)
- Handler: HermesProxy/World/Client/WorldClient.cs:8247
- Fields:
  - `ReadUInt32 -> SpellID`
  - `ReadGuid -> guid`

```csharp
{
CooldownEvent cooldown = new CooldownEvent();
cooldown.SpellID = packet.ReadUInt32();
WowGuid guid = packet.ReadGuid();
cooldown.IsPet = guid.GetHighType() == HighGuidType.Pet;
this.SendPacketToClient(cooldown);
}
```

---

### SMSG_CORPSE_RECLAIM_DELAY

- Legacy value: 617 (0x0269)
- Modern value: 10058 (0x274A)
- Handler: HermesProxy/World/Client/WorldClient.cs:4856
- Fields:
  - `ReadUInt32 -> Remaining`

```csharp
{
CorpseReclaimDelay delay = new CorpseReclaimDelay();
delay.Remaining = packet.ReadUInt32();
this.SendPacketToClient(delay);
}
```

---

### SMSG_CREATE_CHAR

- Legacy value: 58 (0x003A)
- Modern value: 9985 (0x2701)
- Handler: HermesProxy/World/Client/WorldClient.cs:957
- Fields:
  - `ReadUInt8 -> result`

```csharp
{
byte result = packet.ReadUInt8();
CreateChar createChar = new CreateChar();
createChar.Guid = new WowGuid128();
createChar.Code = ModernVersion.ConvertResponseCodesValue(result);
this.SendPacketToClient(createChar);
}
```

---

### SMSG_CRITERIA_UPDATE

- Legacy value: 1130 (0x046A)
- Modern value: 9953 (0x26E1)
- Handler: HermesProxy/World/Client/WorldClient.cs:5159
- Fields:
  - `ReadUInt32 -> CriteriaID`
  - `ReadPackedGuid -> Quantity`
  - `ReadPackedGuid -> playerGuid64`
  - `ReadUInt32 -> Flags`
  - `ReadUInt32 -> datePackedTime`
  - `ReadUInt32 -> ElapsedTime`
  - `ReadUInt32 -> CreationTime`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
CriteriaUpdatePkt update = new CriteriaUpdatePkt();
update.CriteriaID = packet.ReadUInt32();
update.Quantity = packet.ReadPackedGuid().Low; // counter packed as guid
WowGuid64 playerGuid64 = packet.ReadPackedGuid();
update.Flags = packet.ReadUInt32(); // timed flag
uint datePackedTime = packet.ReadUInt32();
update.CurrentTime = (long)Time.GetUnixTimeFromPackedTime(datePackedTime);
update.ElapsedTime = packet.ReadUInt32();
update.CreationTime = packet.ReadUInt32();
update.PlayerGUID = this.GetSession().GameState.CurrentPlayerGuid ?? WowGuid128.Empty;
this.SendPacketToClient(update);
}
}
```

---

### SMSG_DEATH_RELEASE_LOC

- Legacy value: 888 (0x0378)
- Modern value: 9939 (0x26D3)
- Handler: HermesProxy/World/Client/WorldClient.cs:4847
- Fields:
  - `ReadInt32 -> MapID`
  - `ReadVector3`

```csharp
{
DeathReleaseLoc death = new DeathReleaseLoc();
death.MapID = packet.ReadInt32();
death.Location = packet.ReadVector3();
this.SendPacketToClient(death);
}
```

---

### SMSG_DEFENSE_MESSAGE

- Legacy value: 826 (0x033A)
- Modern value: 11190 (0x2BB6)
- Handler: HermesProxy/World/Client/WorldClient.cs:1959
- Fields:
  - `ReadUInt32 -> ZoneID`
  - `ReadUInt32`
  - `ReadCString -> MessageText`

```csharp
{
DefenseMessage message = new DefenseMessage();
message.ZoneID = packet.ReadUInt32();
packet.ReadUInt32();
message.MessageText = packet.ReadCString();
this.SendPacketToClient(message);
}
```

---

### SMSG_DELETE_CHAR

- Legacy value: 60 (0x003C)
- Modern value: 9986 (0x2702)
- Handler: HermesProxy/World/Client/WorldClient.cs:967
- Fields:
  - `ReadUInt8 -> result`

```csharp
{
byte result = packet.ReadUInt8();
DeleteChar deleteChar = new DeleteChar();
deleteChar.Code = ModernVersion.ConvertResponseCodesValue(result);
this.SendPacketToClient(deleteChar);
}
```

---

### SMSG_DESTROY_OBJECT

- Legacy value: 170 (0x00AA)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:9068
- Fields:
  - `ReadGuid -> guid`

```csharp
{
WowGuid128 guid = packet.ReadGuid().To128(this.GetSession().GameState);
Log.Print(LogType.Debug, $"[DestroyObject] Destroying {guid} type={guid.GetHighType()}", "HandleDestroyObject", "");
this.GetSession().GameState.ObjectCacheMutex.WaitOne();
this.GetSession().GameState.ObjectCacheLegacy.Remove(guid);
this.GetSession().GameState.ObjectCacheModern.Remove(guid);
this.GetSession().GameState.ObjectCacheMutex.ReleaseMutex();
this.GetSession().GameState.LastAuraCasterOnTarget.Remove(guid);
UpdateObject updateObject = new UpdateObject(this.GetSession().GameState);
updateObject.DestroyedGuids.Add(guid);
this.SendPacketToClient(updateObject);
}
```

---

### SMSG_DUEL_COMPLETE

- Legacy value: 362 (0x016A)
- Modern value: 10565 (0x2945)
- Handler: HermesProxy/World/Client/WorldClient.cs:2221
- Fields:
  - `ReadBool -> Started`

```csharp
{
DuelComplete duel = new DuelComplete();
duel.Started = packet.ReadBool();
this.SendPacketToClient(duel);
}
```

---

### SMSG_DUEL_COUNTDOWN

- Legacy value: 695 (0x02B7)
- Modern value: 10564 (0x2944)
- Handler: HermesProxy/World/Client/WorldClient.cs:2213
- Fields:
  - `ReadUInt32 -> Countdown`

```csharp
{
DuelCountdown duel = new DuelCountdown();
duel.Countdown = packet.ReadUInt32();
this.SendPacketToClient(duel);
}
```

---

### SMSG_DUEL_IN_BOUNDS

- Legacy value: 361 (0x0169)
- Modern value: 10563 (0x2943)
- Handler: HermesProxy/World/Client/WorldClient.cs:2241

```csharp
{
DuelInBounds duel = new DuelInBounds();
this.SendPacketToClient(duel);
}
```

---

### SMSG_DUEL_OUT_OF_BOUNDS

- Legacy value: 360 (0x0168)
- Modern value: 10562 (0x2942)
- Handler: HermesProxy/World/Client/WorldClient.cs:2248

```csharp
{
DuelOutOfBounds duel = new DuelOutOfBounds();
this.SendPacketToClient(duel);
}
```

---

### SMSG_DUEL_REQUESTED

- Legacy value: 359 (0x0167)
- Modern value: 10560 (0x2940)
- Handler: HermesProxy/World/Client/WorldClient.cs:2203
- Fields:
  - `ReadGuid -> ArbiterGUID`
  - `ReadGuid -> RequestedByGUID`

```csharp
{
DuelRequested duel = new DuelRequested();
duel.ArbiterGUID = packet.ReadGuid().To128(this.GetSession().GameState);
duel.RequestedByGUID = packet.ReadGuid().To128(this.GetSession().GameState);
duel.RequestedByWowAccount = this.GetSession().GetGameAccountGuidForPlayer(duel.RequestedByGUID);
this.SendPacketToClient(duel);
}
```

---

### SMSG_DUEL_WINNER

- Legacy value: 363 (0x016B)
- Modern value: 10566 (0x2946)
- Handler: HermesProxy/World/Client/WorldClient.cs:2229
- Fields:
  - `ReadBool -> Fled`
  - `ReadCString -> BeatenName`
  - `ReadCString -> WinnerName`

```csharp
{
DuelWinner duel = new DuelWinner();
duel.Fled = packet.ReadBool();
duel.BeatenName = packet.ReadCString();
duel.WinnerName = packet.ReadCString();
duel.BeatenVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
duel.WinnerVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
this.SendPacketToClient(duel);
}
```

---

### SMSG_DURABILITY_DAMAGE_DEATH

- Legacy value: 701 (0x02BD)
- Modern value: 10053 (0x2745)
- Handler: HermesProxy/World/Client/WorldClient.cs:4232

```csharp
{
DurabilityDamageDeath death = new DurabilityDamageDeath();
death.Percent = 10u;
this.SendPacketToClient(death);
}
```

---

### SMSG_EMOTE

- Legacy value: 259 (0x0103)
- Modern value: 10185 (0x27C9)
- Handler: HermesProxy/World/Client/WorldClient.cs:1919
- Fields:
  - `ReadUInt32 -> EmoteID`
  - `ReadGuid -> Guid`

```csharp
{
EmoteMessage emote = new EmoteMessage();
emote.EmoteID = packet.ReadUInt32();
emote.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(emote);
}
```

---

### SMSG_ENCHANTMENT_LOG

- Legacy value: 471 (0x01D7)
- Modern value: 10003 (0x2713)
- Handler: HermesProxy/World/Client/WorldClient.cs:4272
- Fields:
  - `ReadPackedGuid -> Owner`
  - `ReadPackedGuid -> Caster`
  - `ReadGuid -> Owner`
  - `ReadGuid -> Caster`
  - `ReadInt32 -> ItemID`
  - `ReadInt32 -> Enchantment`

```csharp
{
EnchantmentLog enchantment = new EnchantmentLog();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
enchantment.Owner = packet.ReadPackedGuid().To128(this.GetSession().GameState);
enchantment.Caster = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
else
{
enchantment.Owner = packet.ReadGuid().To128(this.GetSession().GameState);
enchantment.Caster = packet.ReadGuid().To128(this.GetSession().GameState);
}
enchantment.ItemID = packet.ReadInt32();
GameSessionData session = this.GetSession().GameState;
for (int i = 0; i < 23; i++)
{
if (session.GetItemId(session.GetInventorySlotItem(i).To128(session)).Equals((uint)enchantment.ItemID))
{
enchantment.ItemGUID = session.GetInventorySlotItem(i).To128(session);
break;
}
}
if (!(enchantment.ItemGUID == null))
{
enchantment.Enchantment = packet.ReadInt32();
this.SendPacketToClient(enchantment);
}
}
```

---

### SMSG_ENUM_CHARACTERS_RESULT

- Legacy value: 59 (0x003B)
- Modern value: 9603 (0x2583)
- Handler: HermesProxy/World/Client/WorldClient.cs:845
- Fields:
  - `ReadUInt8 -> count`
  - `ReadGuid -> Guid`
  - `ReadCString -> Name`
  - `ReadUInt8 -> RaceId`
  - `ReadUInt8 -> ClassId`
  - `ReadUInt8 -> SexId`
  - `ReadUInt8 -> skin`
  - `ReadUInt8 -> face`
  - `ReadUInt8 -> hairStyle`
  - `ReadUInt8 -> hairColor`
  - `ReadUInt8 -> facialHair`
  - `ReadUInt8 -> ExperienceLevel`
  - `ReadUInt32 -> ZoneId`
  - `ReadUInt32 -> MapId`
  - `ReadVector3`
  - `ReadUInt32 -> guildId`
  - `ReadUInt32 -> Flags`
  - `ReadUInt32 -> Flags2`
  - `ReadUInt8 -> FirstLogin`
  - `ReadUInt32 -> PetCreatureDisplayId`
  - `ReadUInt32 -> PetExperienceLevel`
  - `ReadUInt32 -> PetCreatureFamilyId`
  - `ReadUInt32 -> DisplayId`
  - `ReadUInt8 -> InvType`
  - `ReadUInt32 -> DisplayEnchantId`
  - `ReadUInt32 -> DisplayId`
  - `ReadUInt8 -> InvType`
  - `ReadUInt32 -> DisplayEnchantId`

```csharp
{
EnumCharactersResult charEnum = new EnumCharactersResult();
charEnum.Success = true;
charEnum.IsDeletedCharacters = false;
charEnum.IsNewPlayerRestrictionSkipped = false;
charEnum.IsNewPlayerRestricted = false;
charEnum.IsNewPlayer = false;
charEnum.IsAlliedRacesCreationAllowed = false;
charEnum.DisabledClassesMask = null;
this.GetSession().GameState.OwnCharacters.Clear();
byte count = packet.ReadUInt8();
for (byte i = 0; i < count; i++)
{
EnumCharactersResult.CharacterInfo char1 = new EnumCharactersResult.CharacterInfo();
char1.ListPosition = i;
PlayerCache cache = new PlayerCache();
char1.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
char1.Name = (cache.Name = packet.ReadCString());
char1.RaceId = (cache.RaceId = (Race)packet.ReadUInt8());
char1.ClassId = (cache.ClassId = (Class)packet.ReadUInt8());
char1.SexId = (cache.SexId = (Gender)packet.ReadUInt8());
byte skin = packet.ReadUInt8();
byte face = packet.ReadUInt8();
byte hairStyle = packet.ReadUInt8();
byte hairColor = packet.ReadUInt8();
byte facialHair = packet.ReadUInt8();
char1.Customizations = CharacterCustomizations.ConvertLegacyCustomizationsToModern(char1.RaceId, char1.SexId, skin, face, hairStyle, hairColor, facialHair);
char1.ExperienceLevel = (cache.Level = packet.ReadUInt8());
if (char1.ExperienceLevel > charEnum.MaxCharacterLevel)
{
```

---

### SMSG_ENVIRONMENTAL_DAMAGE_LOG

- Legacy value: 508 (0x01FC)
- Modern value: 11294 (0x2C1E)
- Handler: HermesProxy/World/Client/WorldClient.cs:8534
- Fields:
  - `ReadGuid -> Victim`
  - `ReadUInt8 -> Type`
  - `ReadInt32 -> Amount`
  - `ReadInt32 -> Absorbed`
  - `ReadInt32 -> Resisted`

```csharp
{
EnvironmentalDamageLog damage = new EnvironmentalDamageLog();
damage.Victim = packet.ReadGuid().To128(this.GetSession().GameState);
damage.Type = (EnvironmentalDamage)packet.ReadUInt8();
damage.Amount = packet.ReadInt32();
damage.Absorbed = packet.ReadInt32();
damage.Resisted = packet.ReadInt32();
this.SendPacketToClient(damage);
}
```

---

### SMSG_EXPLORATION_EXPERIENCE

- Legacy value: 504 (0x01F8)
- Modern value: 10079 (0x275F)
- Handler: HermesProxy/World/Client/WorldClient.cs:4967
- Fields:
  - `ReadUInt32 -> AreaID`
  - `ReadUInt32 -> Experience`

```csharp
{
ExplorationExperience explore = new ExplorationExperience();
explore.AreaID = packet.ReadUInt32();
explore.Experience = packet.ReadUInt32();
this.SendPacketToClient(explore);
}
```

---

### SMSG_FEATURE_SYSTEM_STATUS

- Legacy value: 969 (0x03C9)
- Modern value: 9663 (0x25BF)
- Handler: HermesProxy/World/Client/WorldClient.cs:8859

```csharp
{
this.GetSession().RealmSocket.SendFeatureSystemStatus();
}
```

---

### SMSG_FISH_ESCAPED

- Legacy value: 457 (0x01C9)
- Modern value: 9936 (0x26D0)
- Handler: HermesProxy/World/Client/WorldClient.cs:2289

```csharp
{
FishEscaped fish = new FishEscaped();
this.SendPacketToClient(fish);
}
```

---

### SMSG_FISH_NOT_HOOKED

- Legacy value: 456 (0x01C8)
- Modern value: 9935 (0x26CF)
- Handler: HermesProxy/World/Client/WorldClient.cs:2282

```csharp
{
FishNotHooked fish = new FishNotHooked();
this.SendPacketToClient(fish);
}
```

---

### SMSG_FORCE_FLIGHT_BACK_SPEED_CHANGE

- Legacy value: 899 (0x0383)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5617
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_FLIGHT_SPEED_CHANGE

- Legacy value: 897 (0x0381)
- Modern value: 11764 (0x2DF4)
- Handler: HermesProxy/World/Client/WorldClient.cs:5616
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_PITCH_RATE_CHANGE

- Legacy value: 1116 (0x045C)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5618
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_RUN_BACK_SPEED_CHANGE

- Legacy value: 228 (0x00E4)
- Modern value: 11761 (0x2DF1)
- Handler: HermesProxy/World/Client/WorldClient.cs:5612
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_RUN_SPEED_CHANGE

- Legacy value: 226 (0x00E2)
- Modern value: 11760 (0x2DF0)
- Handler: HermesProxy/World/Client/WorldClient.cs:5611
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_SWIM_BACK_SPEED_CHANGE

- Legacy value: 732 (0x02DC)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5614
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_SWIM_SPEED_CHANGE

- Legacy value: 230 (0x00E6)
- Modern value: 11762 (0x2DF2)
- Handler: HermesProxy/World/Client/WorldClient.cs:5613
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_TURN_RATE_CHANGE

- Legacy value: 734 (0x02DE)
- Modern value: 11767 (0x2DF7)
- Handler: HermesProxy/World/Client/WorldClient.cs:5615
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FORCE_WALK_SPEED_CHANGE

- Legacy value: 730 (0x02DA)
- Modern value: 11766 (0x2DF6)
- Handler: HermesProxy/World/Client/WorldClient.cs:5610
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadUInt8`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("SMSG_FORCE_", "SMSG_MOVE_SET_")
.Replace("_CHANGE", "");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveSetSpeed speed = new MoveSetSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveCounter = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_FORCE_RUN_SPEED_CHANGE)
{
packet.ReadUInt8();
}
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2420 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveSetSpeed flySpeed = new MoveSetSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveCounter = speed.MoveCounter;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### SMSG_FRIEND_LIST

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:7508
- Fields:
  - `ReadUInt8 -> count`
  - `ReadGuid -> Guid`
  - `ReadUInt8 -> Status`
  - `ReadUInt32 -> AreaID`
  - `ReadUInt32 -> Level`
  - `ReadUInt32 -> ClassID`

```csharp
{
ContactList contacts = new ContactList();
contacts.Flags = SocialFlag.Friend;
byte count = packet.ReadUInt8();
for (int i = 0; i < count; i++)
{
ContactInfo contact = new ContactInfo();
contact.TypeFlags = SocialFlag.Friend;
contact.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
contact.WowAccountGuid = this.GetSession().GetGameAccountGuidForPlayer(contact.Guid);
contact.NativeRealmAddr = this.GetSession().RealmId.GetAddress();
contact.VirtualRealmAddr = this.GetSession().RealmId.GetAddress();
contact.Status = (FriendStatus)packet.ReadUInt8();
if (contact.Status != FriendStatus.Offline)
{
contact.AreaID = packet.ReadUInt32();
contact.Level = packet.ReadUInt32();
contact.ClassID = (Class)packet.ReadUInt32();
}
contacts.Contacts.Add(contact);
}
this.SendPacketToClient(contacts);
}
```

---

### SMSG_FRIEND_STATUS

- Legacy value: 104 (0x0068)
- Modern value: 10125 (0x278D)
- Handler: HermesProxy/World/Client/WorldClient.cs:7586
- Fields:
  - `ReadUInt8 -> FriendResult`
  - `ReadGuid -> Guid`
  - `ReadCString -> Notes`
  - `ReadCString -> Notes`
  - `ReadUInt8 -> Status`
  - `ReadUInt32 -> AreaID`
  - `ReadUInt32 -> Level`
  - `ReadUInt32 -> ClassID`
  - `ReadUInt8 -> Status`
  - `ReadUInt32 -> AreaID`
  - `ReadUInt32 -> Level`
  - `ReadUInt32 -> ClassID`

```csharp
{
FriendStatusPkt friend = new FriendStatusPkt();
friend.FriendResult = (FriendsResult)packet.ReadUInt8();
friend.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
friend.WowAccountGuid = this.GetSession().GetGameAccountGuidForPlayer(friend.Guid);
friend.VirtualRealmAddress = this.GetSession().RealmId.GetAddress();
switch (friend.FriendResult)
{
case FriendsResult.AddedOffline:
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
friend.Notes = packet.ReadCString();
}
break;
case FriendsResult.AddedOnline:
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
friend.Notes = packet.ReadCString();
}
friend.Status = (FriendStatus)packet.ReadUInt8();
friend.AreaID = packet.ReadUInt32();
friend.Level = packet.ReadUInt32();
friend.ClassID = (Class)packet.ReadUInt32();
break;
case FriendsResult.Online:
friend.Status = (FriendStatus)packet.ReadUInt8();
friend.AreaID = packet.ReadUInt32();
friend.Level = packet.ReadUInt32();
friend.ClassID = (Class)packet.ReadUInt32();
break;
```

---

### SMSG_GAME_OBJECT_CUSTOM_ANIM

- Legacy value: 179 (0x00B3)
- Modern value: 9668 (0x25C4)
- Handler: HermesProxy/World/Client/WorldClient.cs:2273
- Fields:
  - `ReadGuid -> ObjectGUID`
  - `ReadUInt32 -> CustomAnim`

```csharp
{
GameObjectCustomAnim anim = new GameObjectCustomAnim();
anim.ObjectGUID = packet.ReadGuid().To128(this.GetSession().GameState);
anim.CustomAnim = packet.ReadUInt32();
this.SendPacketToClient(anim);
}
```

---

### SMSG_GAME_OBJECT_DESPAWN

- Legacy value: 533 (0x0215)
- Modern value: 9669 (0x25C5)
- Handler: HermesProxy/World/Client/WorldClient.cs:2255
- Fields:
  - `ReadGuid -> guid`

```csharp
{
WowGuid64 guid = packet.ReadGuid();
GameObjectDespawn despawn = new GameObjectDespawn();
despawn.ObjectGUID = guid.To128(this.GetSession().GameState);
this.SendPacketToClient(despawn);
this.GetSession().GameState.DespawnedGameObjects.Add(guid);
}
```

---

### SMSG_GAME_OBJECT_RESET_STATE

- Legacy value: 679 (0x02A7)
- Modern value: 10014 (0x271E)
- Handler: HermesProxy/World/Client/WorldClient.cs:2265
- Fields:
  - `ReadGuid -> ObjectGUID`

```csharp
{
GameObjectResetState reset = new GameObjectResetState();
reset.ObjectGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(reset);
}
```

---

### SMSG_GM_MESSAGECHAT

- Legacy value: 947 (0x03B3)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:1696
- Fields:
  - `ReadUInt8 -> chatType`
  - `ReadUInt32 -> language`
  - `ReadGuid -> sender`
  - `ReadInt32`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> senderNameLength3`
  - `ReadString -> senderName`
  - `ReadGuid -> receiver`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> senderNameLength2`
  - `ReadString -> senderName`
  - `ReadUInt32 -> senderNameLength`
  - `ReadString -> senderName`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> receiverNameLength`
  - `ReadString -> receiverName`
  - `ReadUInt32 -> gmNameLength`
  - `ReadString`
  - `ReadCString -> channelName`
  - `ReadGuid -> receiver`
  - `ReadUInt32 -> textLength`
  - `ReadString -> text`
  - `ReadUInt8 -> chatFlags`
  - `ReadUInt32 -> gmNameLength2`
  - `ReadString`
  - `ReadUInt32 -> achievementId`
  - `WriteGuid(sender.To64()`
  - `WriteUInt8(0)`

```csharp
{
ChatMessageTypeWotLK chatType = (ChatMessageTypeWotLK)packet.ReadUInt8();
uint language = packet.ReadUInt32();
WowGuid128 sender = packet.ReadGuid().To128(this.GetSession().GameState);
string senderName = "";
string receiverName = "";
string channelName = "";
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_1_0_6692))
{
packet.ReadInt32();
}
WowGuid128 receiver;
switch (chatType)
{
case ChatMessageTypeWotLK.Achievement:
case ChatMessageTypeWotLK.GuildAchievement:
receiver = packet.ReadGuid().To128(this.GetSession().GameState);
break;
case ChatMessageTypeWotLK.WhisperForeign:
{
uint senderNameLength3 = packet.ReadUInt32();
senderName = packet.ReadString(senderNameLength3);
receiver = packet.ReadGuid().To128(this.GetSession().GameState);
break;
}
case ChatMessageTypeWotLK.BattlegroundNeutral:
case ChatMessageTypeWotLK.BattlegroundAlliance:
case ChatMessageTypeWotLK.BattlegroundHorde:
{
receiver = packet.ReadGuid().To128(this.GetSession().GameState);
```

---

### SMSG_GM_TICKET_CREATE

- Legacy value: 518 (0x0206)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:8850
- Fields:
  - `ReadUInt32 -> response`

```csharp
{
LegacyGmTicketResponse response = (LegacyGmTicketResponse)packet.ReadUInt32();
bool flag = ((response == LegacyGmTicketResponse.CreateSuccess || response == LegacyGmTicketResponse.UpdateSuccess) ? true : false);
bool isError = !flag;
this.Session.SendHermesTextMessage($"GM Ticket Status: {response}", isError);
}
```

---

### SMSG_GOSSIP_COMPLETE

- Legacy value: 382 (0x017E)
- Modern value: 10903 (0x2A97)
- Handler: HermesProxy/World/Client/WorldClient.cs:5961

```csharp
{
GossipComplete gossip = new GossipComplete();
this.SendPacketToClient(gossip);
}
```

---

### SMSG_GOSSIP_MESSAGE

- Legacy value: 381 (0x017D)
- Modern value: 10904 (0x2A98)
- Handler: HermesProxy/World/Client/WorldClient.cs:5919
- Fields:
  - `ReadGuid -> GossipGUID`
  - `ReadInt32 -> GossipID`
  - `ReadInt32 -> TextID`
  - `ReadUInt32 -> optionsCount`
  - `ReadInt32 -> OptionIndex`
  - `ReadUInt8 -> OptionIcon`
  - `ReadBool -> OptionFlags`
  - `ReadInt32 -> OptionCost`
  - `ReadCString -> Text`
  - `ReadCString -> Confirm`
  - `ReadUInt32 -> questsCount`

```csharp
{
GossipMessagePkt gossip = new GossipMessagePkt();
gossip.GossipGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = gossip.GossipGUID;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
{
gossip.GossipID = packet.ReadInt32();
}
else
{
gossip.GossipID = (int)gossip.GossipGUID.GetEntry();
}
gossip.TextID = packet.ReadInt32();
uint optionsCount = packet.ReadUInt32();
for (uint i = 0u; i < optionsCount; i++)
{
ClientGossipOption option = new ClientGossipOption();
option.OptionIndex = packet.ReadInt32();
option.OptionIcon = packet.ReadUInt8();
option.OptionFlags = (byte)(packet.ReadBool() ? 1u : 0u);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
option.OptionCost = packet.ReadInt32();
}
option.Text = packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
option.Confirm = packet.ReadCString();
}
gossip.GossipOptions.Add(option);
```

---

### SMSG_GOSSIP_POI

- Legacy value: 548 (0x0224)
- Modern value: 10136 (0x2798)
- Handler: HermesProxy/World/Client/WorldClient.cs:5968
- Fields:
  - `ReadUInt32 -> Flags`
  - `ReadUInt32 -> Icon`
  - `ReadUInt32 -> Importance`
  - `ReadCString -> Name`

```csharp
{
GossipPOI poi = new GossipPOI();
poi.Flags = packet.ReadUInt32();
poi.Pos = new Framework.GameMath.Vector3(packet.ReadVector2());
poi.Icon = packet.ReadUInt32();
poi.Importance = packet.ReadUInt32();
poi.Name = packet.ReadCString();
this.SendPacketToClient(poi);
}
```

---

### SMSG_GROUP_DECLINE

- Legacy value: 116 (0x0074)
- Modern value: 10129 (0x2791)
- Handler: HermesProxy/World/Client/WorldClient.cs:2320
- Fields:
  - `ReadCString -> Name`

```csharp
{
GroupDecline party = new GroupDecline();
party.Name = packet.ReadCString();
this.SendPacketToClient(party);
}
```

---

### SMSG_GROUP_LIST

- Legacy value: 125 (0x007D)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2362, HermesProxy/World/Client/WorldClient.cs:2471
- Fields:
  - `ReadBool -> isRaid`
  - `ReadUInt8 -> ownSubGroupAndFlags`
  - `ReadUInt32 -> membersCount`
  - `ReadCString -> Name`
  - `ReadGuid -> GUID`
  - `ReadUInt8 -> Status`
  - `ReadUInt8 -> subGroupAndFlags`
  - `ReadGuid -> LeaderGUID`
  - `ReadUInt8 -> Method`
  - `ReadGuid -> LootMaster`
  - `ReadUInt8 -> Threshold`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadUInt8 -> groupType`
  - `ReadUInt8 -> ownSubGroup`
  - `ReadUInt8 -> ownGroupFlags`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadUInt32`
  - `ReadGuid -> PartyGUID`
  - `ReadUInt32`
  - `ReadUInt32 -> membersCount`
  - `ReadCString -> Name`
  - `ReadGuid -> GUID`
  - `ReadUInt8 -> Status`
  - `ReadUInt8 -> Subgroup`
  - `ReadUInt8 -> Flags`
  - `ReadUInt8`
  - `ReadGuid -> LeaderGUID`
  - `ReadUInt8 -> Method`
  - `ReadGuid -> LootMaster`
  - `ReadUInt8 -> Threshold`
  - `ReadUInt8 -> difficultyId`

```csharp
{
PartyUpdate party = new PartyUpdate();
party.SequenceNum = this.GetSession().GameState.GroupUpdateCounter++;
bool isRaid = packet.ReadBool();
byte ownSubGroupAndFlags = packet.ReadUInt8();
party.PartyIndex = (byte)((isRaid && this.GetSession().GameState.IsInBattleground()) ? 1u : 0u);
party.PartyGUID = WowGuid128.Create(HighGuidType703.Party, (ulong)(1000 + party.PartyIndex));
if (party.PartyIndex != 0)
{
party.PartyFlags |= GroupFlags.FakeRaid;
}
HashSet<WowGuid128> uniqueMembers = new HashSet<WowGuid128>();
uint membersCount = packet.ReadUInt32();
if (membersCount != 0)
{
if (isRaid)
{
party.PartyFlags |= GroupFlags.Raid;
}
party.DifficultySettings = new PartyDifficultySettings();
party.DifficultySettings.DungeonDifficultyID = DifficultyModern.Normal;
if (ModernVersion.ExpansionVersion > 1)
{
party.DifficultySettings.RaidDifficultyID = DifficultyModern.Raid25N;
}
else
{
party.DifficultySettings.RaidDifficultyID = DifficultyModern.Raid40;
}
if (party.PartyIndex != 0)
```

---

### SMSG_GROUP_NEW_LEADER

- Legacy value: 121 (0x0079)
- Modern value: 9773 (0x262D)
- Handler: HermesProxy/World/Client/WorldClient.cs:2601
- Fields:
  - `ReadCString -> Name`

```csharp
{
GroupNewLeader party = new GroupNewLeader();
party.Name = packet.ReadCString();
party.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
this.SendPacketToClient(party);
}
```

---

### SMSG_GROUP_UNINVITE

- Legacy value: 119 (0x0077)
- Modern value: 10131 (0x2793)
- Handler: HermesProxy/World/Client/WorldClient.cs:2594

```csharp
{
GroupUninvite party = new GroupUninvite();
this.SendPacketToClient(party);
}
```

---

### SMSG_GUILD_BANK_QUERY_RESULTS

- Legacy value: 1000 (0x03E8)
- Modern value: 10719 (0x29DF)
- Handler: HermesProxy/World/Client/WorldClient.cs:3821
- Fields:
  - `ReadUInt64 -> Money`
  - `ReadUInt8 -> Tab`
  - `ReadInt32 -> WithdrawalsRemaining`
  - `ReadBool`
  - `ReadUInt8 -> size`
  - `ReadCString -> Name`
  - `ReadCString -> Icon`
  - `ReadUInt8 -> slots`
  - `ReadUInt8 -> Slot`
  - `ReadInt32 -> entry`
  - `ReadUInt32 -> Flags`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadInt32 -> Count`
  - `ReadUInt8 -> Count`
  - `ReadInt32 -> EnchantmentID`
  - `ReadUInt8 -> Charges`
  - `ReadUInt8 -> enchantments`
  - `ReadUInt8 -> slot`
  - `ReadUInt32 -> enchantId`

```csharp
{
GuildBankQueryResults result = new GuildBankQueryResults();
result.Money = packet.ReadUInt64();
result.Tab = packet.ReadUInt8();
result.WithdrawalsRemaining = packet.ReadInt32();
bool hasTabs = false;
if (packet.ReadBool() && result.Tab == 0)
{
hasTabs = true;
byte size = packet.ReadUInt8();
for (int i = 0; i < size; i++)
{
GuildBankTabInfo tabInfo = new GuildBankTabInfo
{
TabIndex = i,
Name = packet.ReadCString(),
Icon = packet.ReadCString()
};
result.TabInfo.Add(tabInfo);
}
}
byte slots = packet.ReadUInt8();
for (int j = 0; j < slots; j++)
{
GuildBankItemInfo itemInfo = new GuildBankItemInfo();
itemInfo.Slot = packet.ReadUInt8();
int entry = packet.ReadInt32();
if (entry > 0)
{
itemInfo.Item.ItemID = (uint)entry;
```

---

### SMSG_GUILD_COMMAND_RESULT

- Legacy value: 147 (0x0093)
- Modern value: 10682 (0x29BA)
- Handler: HermesProxy/World/Client/WorldClient.cs:3477
- Fields:
  - `ReadUInt32 -> Command`
  - `ReadCString -> Name`
  - `ReadUInt32 -> Result`

```csharp
{
GuildCommandResult result = new GuildCommandResult();
result.Command = (GuildCommandType)packet.ReadUInt32();
result.Name = packet.ReadCString();
result.Result = (GuildCommandError)packet.ReadUInt32();
this.SendPacketToClient(result);
}
```

---

### SMSG_GUILD_EVENT

- Legacy value: 146 (0x0092)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3487
- Fields:
  - `ReadUInt8 -> eventType`
  - `ReadUInt8 -> size`
  - `ReadCString`
  - `ReadGuid -> guid`

```csharp
{
GuildEventType eventType = (GuildEventType)packet.ReadUInt8();
byte size = packet.ReadUInt8();
string[] strings = new string[size];
for (int i = 0; i < size; i++)
{
strings[i] = packet.ReadCString();
}
WowGuid128 guid = WowGuid128.Empty;
if (packet.CanRead())
{
guid = packet.ReadGuid().To128(this.GetSession().GameState);
}
switch (eventType)
{
case GuildEventType.Promotion:
case GuildEventType.Demotion:
{
WowGuid128 officer = this.GetSession().GameState.GetPlayerGuidByName(strings[0]);
WowGuid128 player = this.GetSession().GameState.GetPlayerGuidByName(strings[1]);
uint rankId = this.GetSession().GetGuildRankIdByName(this.GetSession().GameState.GetPlayerGuildId(this.GetSession().GameState.CurrentPlayerGuid), strings[2]);
if (officer != null && player != null)
{
GuildSendRankChange promote = new GuildSendRankChange();
promote.Officer = officer;
promote.Other = player;
promote.Promote = eventType == GuildEventType.Promotion;
promote.RankID = rankId;
this.SendPacketToClient(promote);
}
```

---

### SMSG_GUILD_INFO

- Legacy value: 136 (0x0088)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3678
- Fields:
  - `ReadCString`
  - `ReadInt32 -> day`
  - `ReadInt32 -> month`
  - `ReadInt32 -> year`
  - `ReadUInt32`
  - `ReadUInt32 -> CurrentGuildNumAccounts`

```csharp
{
packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
this.GetSession().GameState.CurrentGuildCreateTime = packet.ReadPackedTime();
}
else
{
int day = packet.ReadInt32();
int month = packet.ReadInt32();
int year = packet.ReadInt32();
try
{
DateTime date = new DateTime(year, month, day);
this.GetSession().GameState.CurrentGuildCreateTime = (uint)Time.DateTimeToUnixTime(date);
}
catch
{
Log.Print(LogType.Error, $"Invalid guild create date: {day}-{month}-{year}", "HandleGuildInfo", "World\\Client\\PacketHandlers\\GuildHandler.cs");
}
}
packet.ReadUInt32();
this.GetSession().GameState.CurrentGuildNumAccounts = packet.ReadUInt32();
}
```

---

### SMSG_GUILD_INVITE

- Legacy value: 131 (0x0083)
- Modern value: 10699 (0x29CB)
- Handler: HermesProxy/World/Client/WorldClient.cs:3784
- Fields:
  - `ReadCString -> InviterName`
  - `ReadCString -> GuildName`

```csharp
{
GuildInvite invite = new GuildInvite();
invite.InviterName = packet.ReadCString();
invite.InviterVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
invite.GuildName = packet.ReadCString();
invite.GuildVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
invite.GuildGUID = this.GetSession().GetGuildGuid(invite.GuildName);
this.SendPacketToClient(invite);
}
```

---

### SMSG_GUILD_INVITE_DECLINED

- Legacy value: 134 (0x0086)
- Modern value: 10729 (0x29E9)
- Handler: HermesProxy/World/Client/WorldClient.cs:3812
- Fields:
  - `ReadCString -> InviterName`

```csharp
{
GuildInviteDeclined invite = new GuildInviteDeclined();
invite.InviterName = packet.ReadCString();
invite.InviterVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
this.SendPacketToClient(invite);
}
```

---

### SMSG_GUILD_ROSTER

- Legacy value: 138 (0x008A)
- Modern value: 10683 (0x29BB)
- Handler: HermesProxy/World/Client/WorldClient.cs:3705
- Fields:
  - `ReadUInt32 -> membersCount`
  - `ReadCString -> WelcomeText`
  - `ReadCString -> InfoText`
  - `ReadInt32 -> ranksCount`
  - `ReadUInt32 -> Flags`
  - `ReadInt32 -> WithdrawGoldLimit`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadGuid -> Guid`
  - `ReadUInt8 -> Status`
  - `ReadCString -> Name`
  - `ReadInt32 -> RankID`
  - `ReadUInt8 -> Level`
  - `ReadUInt8 -> ClassID`
  - `ReadUInt8 -> SexID`
  - `ReadInt32 -> AreaID`
  - `ReadFloat -> LastSave`
  - `ReadCString -> Note`
  - `ReadCString -> OfficerNote`

```csharp
{
GuildRoster guild = new GuildRoster();
uint membersCount = packet.ReadUInt32();
if (this.GetSession().GameState.CurrentGuildNumAccounts != 0)
{
guild.NumAccounts = this.GetSession().GameState.CurrentGuildNumAccounts;
}
else
{
guild.NumAccounts = membersCount;
}
guild.WelcomeText = packet.ReadCString();
guild.InfoText = packet.ReadCString();
if (this.GetSession().GameState.CurrentGuildCreateTime != 0)
{
guild.CreateDate = this.GetSession().GameState.CurrentGuildCreateTime;
}
else
{
guild.CreateDate = (uint)Time.UnixTime;
}
int ranksCount = packet.ReadInt32();
if (ranksCount > 0)
{
GuildRanks ranks = new GuildRanks();
for (byte i = 0; i < ranksCount; i++)
{
GuildRankData rank = new GuildRankData();
rank.RankID = i;
rank.RankOrder = i;
```

---

### SMSG_HIGHEST_THREAT_UPDATE

- Legacy value: 1154 (0x0482)
- Modern value: 9945 (0x26D9)
- Handler: HermesProxy/World/Client/WorldClient.cs:2043
- Fields:
  - `ReadPackedGuid -> unitGuid`

```csharp
{
// Consume packet to prevent "No handler" warning — client doesn't need this
WowGuid128 unitGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
Log.Print(LogType.Debug, $"[Combat] HIGHEST_THREAT_UPDATE unit={unitGuid} (consumed, not forwarded)", "HandleHighestThreatUpdate", "");
}
```

---

### SMSG_IGNORE_LIST

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:7534
- Fields:
  - `ReadUInt8 -> count`
  - `ReadGuid -> Guid`

```csharp
{
ContactList contacts = new ContactList();
contacts.Flags = SocialFlag.Ignored;
byte count = packet.ReadUInt8();
HashSet<WowGuid128> ignoredPlayers = new HashSet<WowGuid128>();
for (int i = 0; i < count; i++)
{
ContactInfo contact = new ContactInfo();
contact.TypeFlags = SocialFlag.Ignored;
contact.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
contact.WowAccountGuid = this.GetSession().GetGameAccountGuidForPlayer(contact.Guid);
contact.NativeRealmAddr = this.GetSession().RealmId.GetAddress();
contact.VirtualRealmAddr = this.GetSession().RealmId.GetAddress();
contacts.Contacts.Add(contact);
ignoredPlayers.Add(contact.Guid);
}
this.Session.GameState.IgnoredPlayers = ignoredPlayers;
this.SendPacketToClient(contacts);
}
```

---

### SMSG_INITIALIZE_FACTIONS

- Legacy value: 290 (0x0122)
- Modern value: 10020 (0x2724)
- Handler: HermesProxy/World/Client/WorldClient.cs:7436
- Fields:
  - `ReadUInt32 -> count`
  - `ReadUInt8`
  - `ReadInt32`

```csharp
{
if (this.GetSession().GameState.IsFirstEnterWorld)
{
InitializeFactions factions = new InitializeFactions();
uint count = packet.ReadUInt32();
for (uint i = 0u; i < count; i++)
{
factions.FactionFlags[i] = (ReputationFlags)packet.ReadUInt8();
factions.FactionStandings[i] = packet.ReadInt32();
}
this.SendPacketToClient(factions);
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.SendPacketToClient(new TimeSyncRequest());
}
}
}
```

---

### SMSG_INIT_WORLD_STATES

- Legacy value: 706 (0x02C2)
- Modern value: 10054 (0x2746)
- Handler: HermesProxy/World/Client/WorldClient.cs:12186
- Fields:
  - `ReadUInt32 -> MapID`
  - `ReadUInt32 -> ZoneID`
  - `ReadUInt32 -> AreaID`
  - `ReadUInt16 -> count`
  - `ReadUInt32 -> variable`
  - `ReadInt32 -> value`

```csharp
{
InitWorldStates states = new InitWorldStates();
states.MapID = packet.ReadUInt32();
this.GetSession().GameState.CurrentMapId = states.MapID;
states.ZoneID = packet.ReadUInt32();
states.AreaID = (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_1_0_6692) ? packet.ReadUInt32() : states.ZoneID);
this.GetSession().GameState.HasWsgAllyFlagCarrier = false;
this.GetSession().GameState.HasWsgHordeFlagCarrier = false;
ushort count = packet.ReadUInt16();
for (ushort i = 0; i < count; i++)
{
uint variable = packet.ReadUInt32();
int value = packet.ReadInt32();
if (variable != 0 || value != 0)
{
states.AddState(variable, value);
}
switch (variable)
{
case 2339u:
this.GetSession().GameState.HasWsgAllyFlagCarrier = value == 2;
break;
case 2338u:
this.GetSession().GameState.HasWsgHordeFlagCarrier = value == 2;
break;
}
}
states.AddClassicStates();
this.SendPacketToClient(states);
if (LegacyVersion.ExpansionVersion <= 1 || ModernVersion.ExpansionVersion <= 1)
```

---

### SMSG_INSPECT_RESULT

- Legacy value: 277 (0x0115)
- Modern value: 9777 (0x2631)
- Handler: HermesProxy/World/Client/WorldClient.cs:1283
- Fields:
  - `ReadGuid -> GUID`
  - `ReadPackedGuid -> GUID`
  - `ReadUInt32 -> talentsCount`
  - `ReadUInt8 -> talent`

```csharp
{
InspectResult inspect = new InspectResult();
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_INSPECT_RESULT)
{
inspect.DisplayInfo.GUID = packet.ReadGuid().To128(this.GetSession().GameState);
}
else
{
inspect.DisplayInfo.GUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
if (!this.GetSession().GameState.CachedPlayers.TryGetValue(inspect.DisplayInfo.GUID, out var cache))
{
return;
}
inspect.DisplayInfo.Name = cache.Name;
inspect.DisplayInfo.ClassId = cache.ClassId;
inspect.DisplayInfo.RaceId = cache.RaceId;
inspect.DisplayInfo.SexId = cache.SexId;
Dictionary<int, UpdateField> updates = this.GetSession().GameState.GetCachedObjectFieldsLegacy(inspect.DisplayInfo.GUID);
if (updates != null)
{
int PLAYER_VISIBLE_ITEM_1_0 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_0);
if (PLAYER_VISIBLE_ITEM_1_0 >= 0)
{
byte offset = (byte)(LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 16u : 12u);
for (byte i = 0; i < 19; i++)
{
if (updates.ContainsKey(PLAYER_VISIBLE_ITEM_1_0 + i * offset))
{
uint itemId = updates[PLAYER_VISIBLE_ITEM_1_0 + i * offset].UInt32Value;
```

---

### SMSG_INSPECT_TALENT

- Legacy value: 1012 (0x03F4)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:1284
- Fields:
  - `ReadGuid -> GUID`
  - `ReadPackedGuid -> GUID`
  - `ReadUInt32 -> talentsCount`
  - `ReadUInt8 -> talent`

```csharp
{
InspectResult inspect = new InspectResult();
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_INSPECT_RESULT)
{
inspect.DisplayInfo.GUID = packet.ReadGuid().To128(this.GetSession().GameState);
}
else
{
inspect.DisplayInfo.GUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
if (!this.GetSession().GameState.CachedPlayers.TryGetValue(inspect.DisplayInfo.GUID, out var cache))
{
return;
}
inspect.DisplayInfo.Name = cache.Name;
inspect.DisplayInfo.ClassId = cache.ClassId;
inspect.DisplayInfo.RaceId = cache.RaceId;
inspect.DisplayInfo.SexId = cache.SexId;
Dictionary<int, UpdateField> updates = this.GetSession().GameState.GetCachedObjectFieldsLegacy(inspect.DisplayInfo.GUID);
if (updates != null)
{
int PLAYER_VISIBLE_ITEM_1_0 = LegacyVersion.GetUpdateField(PlayerField.PLAYER_VISIBLE_ITEM_1_0);
if (PLAYER_VISIBLE_ITEM_1_0 >= 0)
{
byte offset = (byte)(LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) ? 16u : 12u);
for (byte i = 0; i < 19; i++)
{
if (updates.ContainsKey(PLAYER_VISIBLE_ITEM_1_0 + i * offset))
{
uint itemId = updates[PLAYER_VISIBLE_ITEM_1_0 + i * offset].UInt32Value;
```

---

### SMSG_INSTANCE_DIFFICULTY

- Legacy value: 827 (0x033B)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5205

```csharp
{
}
```

---

### SMSG_INSTANCE_RESET

- Legacy value: 798 (0x031E)
- Modern value: 9862 (0x2686)
- Handler: HermesProxy/World/Client/WorldClient.cs:3951
- Fields:
  - `ReadUInt32 -> MapID`

```csharp
{
InstanceReset reset = new InstanceReset();
reset.MapID = packet.ReadUInt32();
this.SendPacketToClient(reset);
}
```

---

### SMSG_INSTANCE_RESET_FAILED

- Legacy value: 799 (0x031F)
- Modern value: 9863 (0x2687)
- Handler: HermesProxy/World/Client/WorldClient.cs:3959
- Fields:
  - `ReadUInt32 -> ResetFailedReason`
  - `ReadUInt32 -> MapID`

```csharp
{
InstanceResetFailed reset = new InstanceResetFailed();
reset.ResetFailedReason = (ResetFailedReason)packet.ReadUInt32();
reset.MapID = packet.ReadUInt32();
this.SendPacketToClient(reset);
}
```

---

### SMSG_INSTANCE_SAVE_CREATED

- Legacy value: 715 (0x02CB)
- Modern value: 10112 (0x2780)
- Handler: HermesProxy/World/Client/WorldClient.cs:4018
- Fields:
  - `ReadUInt32 -> Gm`

```csharp
{
InstanceSaveCreated save = new InstanceSaveCreated();
save.Gm = packet.ReadUInt32() != 0;
this.SendPacketToClient(save);
}
```

---

### SMSG_INVALIDATE_PLAYER

- Legacy value: 796 (0x031C)
- Modern value: 12287 (0x2FFF)
- Handler: HermesProxy/World/Client/WorldClient.cs:5049
- Fields:
  - `ReadGuid -> Guid`

```csharp
{
InvalidatePlayer invalidate = new InvalidatePlayer();
invalidate.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(invalidate);
if (this.GetSession().GameState.CachedPlayers.ContainsKey(invalidate.Guid))
{
this.GetSession().GameState.CachedPlayers.Remove(invalidate.Guid);
}
}
```

---

### SMSG_INVENTORY_CHANGE_FAILURE

- Legacy value: 274 (0x0112)
- Modern value: 11685 (0x2DA5)
- Handler: HermesProxy/World/Client/WorldClient.cs:4171, HermesProxy/World/Client/WorldClient.cs:4196
- Fields:
  - `ReadUInt8 -> BagResult`
  - `ReadInt32 -> Level`
  - `ReadGuid`
  - `ReadGuid`
  - `ReadUInt8 -> ContainerBSlot`
  - `ReadUInt8 -> BagResult`
  - `ReadGuid`
  - `ReadGuid`
  - `ReadUInt8 -> ContainerBSlot`
  - `ReadInt32 -> Level`
  - `ReadGuid -> SrcContainer`
  - `ReadInt32 -> SrcSlot`
  - `ReadGuid -> DstContainer`
  - `ReadInt32 -> LimitCategory`

```csharp
{
InventoryChangeFailure failure = new InventoryChangeFailure();
failure.BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt8());
if (failure.BagResult != InventoryResult.Ok)
{
InventoryResult bagResult = failure.BagResult;
InventoryResult inventoryResult = bagResult;
if (inventoryResult == InventoryResult.CantEquipLevel)
{
failure.Level = packet.ReadInt32();
}
failure.Item[0] = packet.ReadGuid().To128(this.GetSession().GameState);
failure.Item[1] = packet.ReadGuid().To128(this.GetSession().GameState);
failure.ContainerBSlot = packet.ReadUInt8();
this.SendPacketToClient(failure);
if (this.GetSession().GameState.CurrentClientNormalCast != null && !this.GetSession().GameState.CurrentClientNormalCast.HasStarted && this.GetSession().GameState.CurrentClientNormalCast.ItemGUID == failure.Item[0])
{
this.GetSession().InstanceSocket.SendCastRequestFailed(this.GetSession().GameState.CurrentClientNormalCast, isPet: false);
this.GetSession().GameState.CurrentClientNormalCast = null;
}
}
}
```

---

### SMSG_ITEM_COOLDOWN

- Legacy value: 176 (0x00B0)
- Modern value: 10184 (0x27C8)
- Handler: HermesProxy/World/Client/WorldClient.cs:4240
- Fields:
  - `ReadGuid -> ItemGuid`
  - `ReadUInt32 -> SpellID`

```csharp
{
ItemCooldown item = new ItemCooldown();
item.ItemGuid = packet.ReadGuid().To128(this.GetSession().GameState);
item.SpellID = packet.ReadUInt32();
item.Cooldown = 30000u;
this.SendPacketToClient(item);
}
```

---

### SMSG_ITEM_ENCHANT_TIME_UPDATE

- Legacy value: 491 (0x01EB)
- Modern value: 10069 (0x2755)
- Handler: HermesProxy/World/Client/WorldClient.cs:4261
- Fields:
  - `ReadGuid -> ItemGuid`
  - `ReadUInt32 -> Slot`
  - `ReadUInt32 -> DurationLeft`
  - `ReadGuid -> OwnerGuid`

```csharp
{
ItemEnchantTimeUpdate enchant = new ItemEnchantTimeUpdate();
enchant.ItemGuid = packet.ReadGuid().To128(this.GetSession().GameState);
enchant.Slot = packet.ReadUInt32();
enchant.DurationLeft = packet.ReadUInt32();
enchant.OwnerGuid = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(enchant);
}
```

---

### SMSG_ITEM_NAME_QUERY_RESPONSE

- Legacy value: 709 (0x02C5)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:6934
- Fields:
  - `ReadUInt32 -> entry`
  - `ReadCString -> name`
  - `ReadUInt32`

```csharp
{
uint entry = packet.ReadUInt32();
string name = packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.ReadUInt32();
}
GameData.StoreItemName(entry, name);
}
```

---

### SMSG_ITEM_PUSH_RESULT

- Legacy value: 358 (0x0166)
- Modern value: 9763 (0x2623)
- Handler: HermesProxy/World/Client/WorldClient.cs:4082
- Fields:
  - `ReadGuid -> PlayerGUID`
  - `ReadUInt32 -> fromNPC`
  - `ReadUInt32 -> Created`
  - `ReadUInt32 -> showInChat`
  - `ReadUInt8 -> Slot`
  - `ReadInt32 -> SlotInBag`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadUInt32 -> Quantity`
  - `ReadUInt32 -> QuantityInInventory`

```csharp
{
ItemPushResult item = new ItemPushResult();
item.PlayerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
bool fromNPC = packet.ReadUInt32() == 1;
item.Created = packet.ReadUInt32() == 1;
bool showInChat = packet.ReadUInt32() == 1;
if (fromNPC && !item.Created)
{
item.DisplayText = ItemPushResult.DisplayType.Received;
item.Pushed = true;
}
else if (!showInChat)
{
item.DisplayText = ItemPushResult.DisplayType.Hidden;
}
else
{
item.DisplayText = ItemPushResult.DisplayType.Loot;
}
item.Slot = packet.ReadUInt8();
item.SlotInBag = packet.ReadInt32();
item.Item.ItemID = packet.ReadUInt32();
item.Item.RandomPropertiesSeed = packet.ReadUInt32();
item.Item.RandomPropertiesID = packet.ReadUInt32();
item.Quantity = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
item.QuantityInInventory = packet.ReadUInt32();
}
else
```

---

### SMSG_ITEM_QUERY_SINGLE_RESPONSE

- Legacy value: 88 (0x0058)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:6827

```csharp
{
KeyValuePair<int, bool> entry = packet.ReadEntry();
if (entry.Value)
{
if (this.GetSession().GameState.RequestedItemHotfixes.Contains((uint)entry.Key))
{
DBReply reply = new DBReply();
reply.RecordID = (uint)entry.Key;
reply.TableHash = DB2Hash.Item;
reply.Status = HotfixStatus.Invalid;
reply.Timestamp = (uint)Time.UnixTime;
this.SendPacketToClient(reply);
}
if (this.GetSession().GameState.RequestedItemSparseHotfixes.Contains((uint)entry.Key))
{
DBReply reply2 = new DBReply();
reply2.RecordID = (uint)entry.Key;
reply2.TableHash = DB2Hash.ItemSparse;
reply2.Status = HotfixStatus.Invalid;
reply2.Timestamp = (uint)Time.UnixTime;
this.SendPacketToClient(reply2);
}
}
else
{
ItemTemplate item = new ItemTemplate();
item.ReadFromLegacyPacket((uint)entry.Key, packet);
this.SendItemUpdatesIfNeeded(item);
GameData.StoreItemTemplate((uint)entry.Key, item);
}
```

---

### SMSG_LEARNED_DANCE_MOVES

- Legacy value: 1109 (0x0455)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5380

```csharp
{
}
```

---

### SMSG_LEARNED_SPELL

- Legacy value: 299 (0x012B)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:7693
- Fields:
  - `ReadUInt32 -> spellId`

```csharp
{
LearnedSpells spells = new LearnedSpells();
uint spellId = packet.ReadUInt32();
spells.Spells.Add(spellId);
this.SendPacketToClient(spells);
}
```

---

### SMSG_LEVEL_UP_INFO

- Legacy value: 468 (0x01D4)
- Modern value: 9961 (0x26E9)
- Handler: HermesProxy/World/Client/WorldClient.cs:1250
- Fields:
  - `ReadInt32 -> Level`
  - `ReadInt32 -> HealthDelta`
  - `ReadInt32`
  - `ReadInt32`

```csharp
{
LevelUpInfo info = new LevelUpInfo();
info.Level = packet.ReadInt32();
info.HealthDelta = packet.ReadInt32();
for (int i = 0; i < LegacyVersion.GetPowersCount(); i++)
{
info.PowerDelta[i] = packet.ReadInt32();
}
for (int j = 0; j < 5; j++)
{
info.StatDelta[j] = packet.ReadInt32();
}
this.SendPacketToClient(info);
}
```

---

### SMSG_LFG_JOIN_RESULT

- Legacy value: 868 (0x0364)
- Modern value: 10780 (0x2A1C)
- Handler: HermesProxy/World/Client/WorldClient.cs:5267
- Fields:
  - `ReadUInt32 -> Result`
  - `ReadUInt32 -> ResultDetail`
  - `ReadUInt8 -> partySize`
  - `ReadGuid -> PlayerGuid`
  - `ReadUInt32 -> dungeonCount`
  - `ReadUInt32 -> Slot`
  - `ReadUInt32 -> Reason`

```csharp
{
DfJoinResult result = new DfJoinResult();
result.Ticket.RequesterGuid = this.GetSession().GameState.CurrentPlayerGuid;
result.Ticket.Id = 1;
result.Ticket.Type = RideType.Lfg;
result.Ticket.Time = (long)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
result.Result = (byte)packet.ReadUInt32(); // joinData.result
result.ResultDetail = (byte)packet.ReadUInt32(); // joinData.state
if (packet.CanRead())
{
byte partySize = packet.ReadUInt8();
for (int p = 0; p < partySize; p++)
{
DfJoinBlackList blackList = new DfJoinBlackList();
blackList.PlayerGuid = packet.ReadGuid().To128(this.GetSession().GameState);
uint dungeonCount = packet.ReadUInt32();
for (uint d = 0; d < dungeonCount; d++)
{
DfJoinBlackListSlot slot = new DfJoinBlackListSlot();
slot.Slot = packet.ReadUInt32();
slot.Reason = packet.ReadUInt32();
blackList.Slots.Add(slot);
}
result.BlackList.Add(blackList);
}
}
this.SendPacketToClient(result);
}
```

---

### SMSG_LFG_PLAYER_INFO

- Legacy value: 879 (0x036F)
- Modern value: 10807 (0x2A37)
- Handler: HermesProxy/World/Client/WorldClient.cs:5210
- Fields:
  - `ReadUInt8 -> randomCount`
  - `ReadUInt32 -> Slot`
  - `ReadUInt8 -> isDone`
  - `ReadUInt32 -> RewardMoney`
  - `ReadUInt32 -> RewardXP`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt8 -> itemCount`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32`
  - `ReadUInt32 -> Quantity`
  - `ReadUInt32 -> lockCount`
  - `ReadUInt32 -> Slot`
  - `ReadUInt32 -> Reason`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
{
LfgPlayerInfoPkt info = new LfgPlayerInfoPkt();

// 3.3.5a format: random dungeons, then locked dungeons
// Random dungeons (available)
byte randomCount = packet.ReadUInt8();
for (int i = 0; i < randomCount; i++)
{
LfgPlayerDungeonInfo dungeon = new LfgPlayerDungeonInfo();
dungeon.Slot = packet.ReadUInt32(); // dungeon entry (id + type)
bool isDone = packet.ReadUInt8() != 0;
dungeon.FirstReward = !isDone;
dungeon.CompletionQuantity = isDone ? 1 : 0;
dungeon.CompletionLimit = 1;

LfgPlayerQuestReward rewards = new LfgPlayerQuestReward();
rewards.Items = new List<LfgPlayerQuestRewardItem>();
rewards.Currency = new List<LfgPlayerQuestRewardCurrency>();
rewards.BonusCurrency = new List<LfgPlayerQuestRewardCurrency>();
rewards.RewardMoney = (int)packet.ReadUInt32();
rewards.RewardXP = (int)packet.ReadUInt32();
packet.ReadUInt32(); // unknown
packet.ReadUInt32(); // unknown
byte itemCount = packet.ReadUInt8();
for (int j = 0; j < itemCount; j++)
{
LfgPlayerQuestRewardItem item = new LfgPlayerQuestRewardItem();
item.ItemID = (int)packet.ReadUInt32();
```

---

### SMSG_LFG_PROPOSAL_UPDATE

- Legacy value: 865 (0x0361)
- Modern value: 10797 (0x2A2D)
- Handler: HermesProxy/World/Client/WorldClient.cs:5348
- Fields:
  - `ReadUInt32 -> dungeonEntry`
  - `ReadUInt8 -> State`
  - `ReadUInt32 -> ProposalID`
  - `ReadUInt32 -> CompletedMask`
  - `ReadUInt8 -> silent`
  - `ReadUInt8 -> playerCount`
  - `ReadUInt32 -> Roles`
  - `ReadUInt8 -> Me`
  - `ReadUInt8 -> inDungeon`
  - `ReadUInt8 -> sameGroup`
  - `ReadUInt8 -> Responded`
  - `ReadUInt8 -> Accepted`

```csharp
{
DfProposalUpdate proposal = new DfProposalUpdate();
proposal.Ticket.RequesterGuid = this.GetSession().GameState.CurrentPlayerGuid;
proposal.Ticket.Id = 1;
proposal.Ticket.Type = RideType.Lfg;
proposal.Ticket.Time = (long)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
uint dungeonEntry = packet.ReadUInt32();
proposal.Slot = dungeonEntry;
proposal.State = (sbyte)packet.ReadUInt8();
proposal.ProposalID = packet.ReadUInt32();
proposal.CompletedMask = packet.ReadUInt32();
bool silent = packet.ReadUInt8() != 0;
proposal.ProposalSilent = silent;
byte playerCount = packet.ReadUInt8();
for (int i = 0; i < playerCount; i++)
{
DfProposalPlayer player = new DfProposalPlayer();
player.Roles = (byte)packet.ReadUInt32();
player.Me = packet.ReadUInt8() != 0;
bool inDungeon = packet.ReadUInt8() != 0;
bool sameGroup = packet.ReadUInt8() != 0;
player.SameParty = sameGroup;
player.MyParty = inDungeon;
player.Responded = packet.ReadUInt8() != 0;
player.Accepted = packet.ReadUInt8() != 0;
proposal.Players.Add(player);
}
this.SendPacketToClient(proposal);
}
```

---

### SMSG_LFG_QUEUE_STATUS

- Legacy value: 869 (0x0365)
- Modern value: 10784 (0x2A20)
- Handler: HermesProxy/World/Client/WorldClient.cs:5327
- Fields:
  - `ReadUInt32 -> Slot`
  - `ReadInt32 -> AvgWaitTime`
  - `ReadInt32 -> AvgWaitTimeMe`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadUInt32 -> QueuedTime`

```csharp
{
DfQueueStatus status = new DfQueueStatus();
status.Ticket.RequesterGuid = this.GetSession().GameState.CurrentPlayerGuid;
status.Ticket.Id = 1;
status.Ticket.Type = RideType.Lfg;
status.Ticket.Time = (long)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
status.Slot = packet.ReadUInt32();
status.AvgWaitTime = (uint)packet.ReadInt32();
status.AvgWaitTimeMe = (uint)packet.ReadInt32();
status.AvgWaitTimeByRole[0] = (uint)packet.ReadInt32(); // Tank
status.AvgWaitTimeByRole[1] = (uint)packet.ReadInt32(); // Healer
status.AvgWaitTimeByRole[2] = (uint)packet.ReadInt32(); // DPS
status.LastNeeded[0] = packet.ReadUInt8(); // Tanks needed
status.LastNeeded[1] = packet.ReadUInt8(); // Healers needed
status.LastNeeded[2] = packet.ReadUInt8(); // DPS needed
status.QueuedTime = packet.ReadUInt32();
this.SendPacketToClient(status);
}
```

---

### SMSG_LFG_UPDATE_PLAYER

- Legacy value: 871 (0x0367)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:5298
- Fields:
  - `ReadUInt8 -> updateType`
  - `ReadUInt8 -> hasExtraInfo`
  - `ReadUInt8 -> Queued`
  - `ReadUInt8`
  - `ReadUInt8`
  - `ReadUInt8 -> dungeonCount`
  - `ReadUInt32`
  - `ReadCString`

```csharp
{
DfUpdateStatus status = new DfUpdateStatus();
status.Ticket.RequesterGuid = this.GetSession().GameState.CurrentPlayerGuid;
status.Ticket.Id = 1;
status.Ticket.Type = RideType.Lfg;
status.Ticket.Time = (long)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
byte updateType = packet.ReadUInt8();
status.SubType = updateType;
bool hasExtraInfo = packet.ReadUInt8() != 0;
if (hasExtraInfo)
{
status.Queued = packet.ReadUInt8() != 0;
packet.ReadUInt8(); // unk
packet.ReadUInt8(); // unk
byte dungeonCount = packet.ReadUInt8();
for (int i = 0; i < dungeonCount; i++)
{
status.Slots.Add(packet.ReadUInt32());
}
status.Joined = true;
status.LfgJoined = true;
status.NotifyUI = true;
packet.ReadCString(); // comment - not used in modern
}
this.SendPacketToClient(status);
}
```

---

### SMSG_LOAD_EQUIPMENT_SET

- Legacy value: 1212 (0x04BC)
- Modern value: 9998 (0x270E)
- Handler: HermesProxy/World/Client/WorldClient.cs:5200

```csharp
{
}
```

---

### SMSG_LOGIN_SET_TIME_SPEED

- Legacy value: 66 (0x0042)
- Modern value: 9997 (0x270D)
- Handler: HermesProxy/World/Client/WorldClient.cs:4897
- Fields:
  - `ReadUInt32 -> ServerTime`
  - `ReadFloat -> NewSpeed`
  - `ReadInt32 -> ServerTimeHolidayOffset`

```csharp
{
if (this.GetSession().GameState.IsFirstEnterWorld)
{
LoginSetTimeSpeed login = new LoginSetTimeSpeed();
login.ServerTime = packet.ReadUInt32();
login.GameTime = login.ServerTime;
login.NewSpeed = packet.ReadFloat();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_2_9901))
{
login.ServerTimeHolidayOffset = packet.ReadInt32();
login.GameTimeHolidayOffset = login.ServerTimeHolidayOffset;
}
this.SendPacketToClient(login);
}
}
```

---

### SMSG_LOGIN_VERIFY_WORLD

- Legacy value: 566 (0x0236)
- Modern value: 9623 (0x2597)
- Handler: HermesProxy/World/Client/WorldClient.cs:1077
- Fields:
  - `ReadUInt32 -> MapID`
  - `ReadFloat -> X`
  - `ReadFloat -> Y`
  - `ReadFloat -> Z`
  - `ReadFloat -> Orientation`

```csharp
{
// Only reset buffer on first login, not on teleports
// Teleports don't send a new player CreateObject so _playerObjectSent would never become true
if (!this.GetSession().GameState.IsInWorld)
{
UpdateObject.ResetLoginBuffer(this.GetSession().GameState);
}
LoginVerifyWorld verify = new LoginVerifyWorld();
verify.MapID = packet.ReadUInt32();
this.GetSession().GameState.CurrentMapId = verify.MapID;
verify.Pos.X = packet.ReadFloat();
verify.Pos.Y = packet.ReadFloat();
verify.Pos.Z = packet.ReadFloat();
verify.Pos.Orientation = packet.ReadFloat();
Log.Print(LogType.Server, $"[LoginVerifyWorld] Map={verify.MapID} Pos=({verify.Pos.X}, {verify.Pos.Y}, {verify.Pos.Z}) Orient={verify.Pos.Orientation}", "HandleLoginVerifyWorld", "World\\Client\\PacketHandlers\\CharacterHandler.cs");
this.SendPacketToClient(verify);
this.GetSession().GameState.IsInWorld = true;
if (ModernVersion.ExpansionVersion >= 3)
{
EmptyInitWorldStates worldStates = new EmptyInitWorldStates();
worldStates.MapId = verify.MapID;
worldStates.ZoneId = 0;
worldStates.AreaId = 0;
this.SendPacketToClient(worldStates);
}
WorldServerInfo info = new WorldServerInfo();
if (verify.MapID > 1)
{
info.DifficultyID = 1u;
info.InstanceGroupSize = 5u;
```

---

### SMSG_LOGOUT_CANCEL_ACK

- Legacy value: 79 (0x004F)
- Modern value: 9861 (0x2685)
- Handler: HermesProxy/World/Client/WorldClient.cs:1207

```csharp
{
LogoutCancelAck logout = new LogoutCancelAck();
this.SendPacketToClient(logout);
}
```

---

### SMSG_LOGOUT_COMPLETE

- Legacy value: 77 (0x004D)
- Modern value: 9860 (0x2684)
- Handler: HermesProxy/World/Client/WorldClient.cs:1197

```csharp
{
LogoutComplete logout = new LogoutComplete();
this.SendPacketToClient(logout);
this.GetSession().GameState = GameSessionData.CreateNewGameSessionData(this.GetSession());
this.GetSession().InstanceSocket.CloseSocket();
this.GetSession().InstanceSocket = null;
}
```

---

### SMSG_LOGOUT_RESPONSE

- Legacy value: 76 (0x004C)
- Modern value: 9859 (0x2683)
- Handler: HermesProxy/World/Client/WorldClient.cs:1188
- Fields:
  - `ReadInt32 -> LogoutResult`
  - `ReadBool -> Instant`

```csharp
{
LogoutResponse logout = new LogoutResponse();
logout.LogoutResult = packet.ReadInt32();
logout.Instant = packet.ReadBool();
this.SendPacketToClient(logout);
}
```

---

### SMSG_LOG_XP_GAIN

- Legacy value: 464 (0x01D0)
- Modern value: 9957 (0x26E5)
- Handler: HermesProxy/World/Client/WorldClient.cs:1214
- Fields:
  - `ReadGuid -> Victim`
  - `ReadInt32 -> Original`
  - `ReadUInt8 -> Reason`
  - `ReadInt32 -> Amount`
  - `ReadFloat -> GroupBonus`
  - `ReadUInt8 -> RAFBonus`

```csharp
{
LogXPGain log = new LogXPGain();
log.Victim = packet.ReadGuid().To128(this.GetSession().GameState);
log.Original = packet.ReadInt32();
log.Reason = (PlayerLogXPReason)packet.ReadUInt8();
if (log.Reason == PlayerLogXPReason.Kill)
{
log.Amount = packet.ReadInt32();
log.GroupBonus = packet.ReadFloat();
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089) && packet.CanRead())
{
log.RAFBonus = packet.ReadUInt8();
}
this.SendPacketToClient(log);
}
```

---

### SMSG_LOOT_ALL_PASSED

- Legacy value: 670 (0x029E)
- Modern value: 9761 (0x2621)
- Handler: HermesProxy/World/Client/WorldClient.cs:4502
- Fields:
  - `ReadGuid -> LootObj`
  - `ReadUInt32 -> LootListID`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`

```csharp
{
LootAllPassed loot = new LootAllPassed();
loot.LootObj = packet.ReadGuid().ToLootGuid();
loot.Item.LootListID = (byte)packet.ReadUInt32();
loot.Item.Loot.ItemID = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesSeed = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesID = packet.ReadUInt32();
loot.Item.Quantity = 1u;
this.SendPacketToClient(loot);
LootRollsComplete complete = new LootRollsComplete();
complete.LootObj = loot.LootObj;
complete.LootListID = loot.Item.LootListID;
this.SendPacketToClient(complete);
}
```

---

### SMSG_LOOT_CLEAR_MONEY

- Legacy value: 357 (0x0165)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:4387

```csharp
{
CoinRemoved loot = new CoinRemoved();
loot.LootObj = this.GetSession().GameState.LastLootTargetGuid.ToLootGuid();
this.SendPacketToClient(loot);
}
```

---

### SMSG_LOOT_LIST

- Legacy value: 1017 (0x03F9)
- Modern value: 10049 (0x2741)
- Handler: HermesProxy/World/Client/WorldClient.cs:4303
- Fields:
  - `ReadGuid -> creatureGuid`
  - `ReadPackedGuid -> masterLooter`
  - `ReadPackedGuid -> roundRobinWinner`

```csharp
{
LootList list = new LootList();
WowGuid64 creatureGuid = packet.ReadGuid();
list.Owner = creatureGuid.To128(this.GetSession().GameState);
list.LootObj = creatureGuid.ToLootGuid();

WowGuid64 masterLooter = packet.ReadPackedGuid();
if (!masterLooter.IsEmpty())
list.Master = masterLooter.To128(this.GetSession().GameState);

WowGuid64 roundRobinWinner = packet.ReadPackedGuid();
if (!roundRobinWinner.IsEmpty())
list.RoundRobinWinner = roundRobinWinner.To128(this.GetSession().GameState);

this.SendPacketToClient(list);
}
```

---

### SMSG_LOOT_MASTER_LIST

- Legacy value: 676 (0x02A4)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:4519
- Fields:
  - `ReadUInt8 -> count`
  - `ReadGuid -> guid`

```csharp
{
if (!(this.GetSession().GameState.LastLootTargetGuid == null))
{
LootList list = new LootList();
list.Owner = this.GetSession().GameState.LastLootTargetGuid.To128(this.GetSession().GameState);
list.LootObj = this.GetSession().GameState.LastLootTargetGuid.ToLootGuid();
list.Master = this.GetSession().GameState.CurrentPlayerGuid;
this.SendPacketToClient(list);
MasterLootCandidateList loot = new MasterLootCandidateList();
loot.LootObj = this.GetSession().GameState.LastLootTargetGuid.ToLootGuid();
byte count = packet.ReadUInt8();
for (byte i = 0; i < count; i++)
{
WowGuid128 guid = packet.ReadGuid().To128(this.GetSession().GameState);
loot.Players.Add(guid);
}
this.SendPacketToClient(loot);
}
}
```

---

### SMSG_LOOT_MONEY_NOTIFY

- Legacy value: 355 (0x0163)
- Modern value: 9756 (0x261C)
- Handler: HermesProxy/World/Client/WorldClient.cs:4375
- Fields:
  - `ReadUInt32 -> Money`
  - `ReadBool -> SoleLooter`

```csharp
{
LootMoneyNotify loot = new LootMoneyNotify();
loot.Money = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
loot.SoleLooter = packet.ReadBool();
}
this.SendPacketToClient(loot);
}
```

---

### SMSG_LOOT_RELEASE

- Legacy value: 353 (0x0161)
- Modern value: 9755 (0x261B)
- Handler: HermesProxy/World/Client/WorldClient.cs:4354
- Fields:
  - `ReadGuid -> owner`
  - `ReadBool`

```csharp
{
LootReleaseResponse loot = new LootReleaseResponse();
WowGuid64 owner = packet.ReadGuid();
loot.Owner = owner.To128(this.GetSession().GameState);
loot.LootObj = owner.ToLootGuid();
packet.ReadBool();
this.SendPacketToClient(loot);
}
```

---

### SMSG_LOOT_REMOVED

- Legacy value: 354 (0x0162)
- Modern value: 9749 (0x2615)
- Handler: HermesProxy/World/Client/WorldClient.cs:4365
- Fields:
  - `ReadUInt8 -> LootListID`

```csharp
{
LootRemoved loot = new LootRemoved();
loot.Owner = this.GetSession().GameState.LastLootTargetGuid.To128(this.GetSession().GameState);
loot.LootObj = this.GetSession().GameState.LastLootTargetGuid.ToLootGuid();
loot.LootListID = packet.ReadUInt8();
this.SendPacketToClient(loot);
}
```

---

### SMSG_LOOT_RESPONSE

- Legacy value: 352 (0x0160)
- Modern value: 9748 (0x2614)
- Handler: HermesProxy/World/Client/WorldClient.cs:4322
- Fields:
  - `ReadGuid -> LastLootTargetGuid`
  - `ReadUInt8 -> AcquireReason`
  - `ReadUInt8 -> FailureReason`
  - `ReadUInt32 -> Coins`
  - `ReadUInt8 -> itemsCount`
  - `ReadUInt8 -> LootListID`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> Quantity`
  - `ReadUInt32`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadUInt8 -> uiType`

```csharp
{
LootResponse loot = new LootResponse();
this.GetSession().GameState.LastLootTargetGuid = packet.ReadGuid();
loot.Owner = this.GetSession().GameState.LastLootTargetGuid.To128(this.GetSession().GameState);
loot.LootObj = this.GetSession().GameState.LastLootTargetGuid.ToLootGuid();
loot.AcquireReason = (LootType)packet.ReadUInt8();
if (loot.AcquireReason == LootType.None)
{
loot.FailureReason = (LootError)packet.ReadUInt8();
return;
}
loot.LootMethod = this.GetSession().GameState.GetCurrentLootMethod();
loot.Coins = packet.ReadUInt32();
byte itemsCount = packet.ReadUInt8();
for (int i = 0; i < itemsCount; i++)
{
LootItemData lootItem = new LootItemData();
lootItem.LootListID = packet.ReadUInt8();
lootItem.Loot.ItemID = packet.ReadUInt32();
lootItem.Quantity = packet.ReadUInt32();
packet.ReadUInt32();
lootItem.Loot.RandomPropertiesSeed = packet.ReadUInt32();
lootItem.Loot.RandomPropertiesID = packet.ReadUInt32();
LootSlotTypeLegacy uiType = (LootSlotTypeLegacy)packet.ReadUInt8();
lootItem.UIType = (LootSlotTypeModern)Enum.Parse(typeof(LootSlotTypeModern), uiType.ToString());
loot.Items.Add(lootItem);
}
this.SendPacketToClient(loot);
}
```

---

### SMSG_LOOT_ROLL

- Legacy value: 674 (0x02A2)
- Modern value: 9758 (0x261E)
- Handler: HermesProxy/World/Client/WorldClient.cs:4441
- Fields:
  - `ReadGuid -> owner`
  - `ReadUInt32 -> LootListID`
  - `ReadGuid -> Player`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadUInt8 -> Roll`
  - `ReadUInt8 -> rollType`
  - `ReadBool -> Autopassed`

```csharp
{
LootRollBroadcast loot = new LootRollBroadcast();
WowGuid64 owner = packet.ReadGuid();
loot.LootObj = owner.ToLootGuid();
loot.Item.LootListID = (byte)packet.ReadUInt32();
loot.Player = packet.ReadGuid().To128(this.GetSession().GameState);
loot.Item.Loot.ItemID = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesSeed = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesID = packet.ReadUInt32();
loot.Item.Quantity = 1u;
loot.Roll = packet.ReadUInt8();
byte rollType = packet.ReadUInt8();
if (loot.Roll == 128 && rollType == 128)
{
loot.RollType = RollType.Pass;
}
else if (loot.Roll == 0 && rollType == 0)
{
loot.RollType = RollType.Need;
}
else
{
loot.RollType = (RollType)rollType;
}
if (loot.Roll == 128)
{
loot.Roll = 0;
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
```

---

### SMSG_LOOT_ROLL_WON

- Legacy value: 671 (0x029F)
- Modern value: 9762 (0x2622)
- Handler: HermesProxy/World/Client/WorldClient.cs:4478
- Fields:
  - `ReadGuid -> LootObj`
  - `ReadUInt32 -> LootListID`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadGuid -> Winner`
  - `ReadUInt8 -> Roll`
  - `ReadUInt8 -> RollType`

```csharp
{
LootRollWon loot = new LootRollWon();
loot.LootObj = packet.ReadGuid().ToLootGuid();
loot.Item.LootListID = (byte)packet.ReadUInt32();
loot.Item.Loot.ItemID = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesSeed = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesID = packet.ReadUInt32();
loot.Item.Quantity = 1u;
loot.Winner = packet.ReadGuid().To128(this.GetSession().GameState);
loot.Roll = packet.ReadUInt8();
loot.RollType = (RollType)packet.ReadUInt8();
if (loot.RollType == RollType.Need)
{
loot.MainSpec = 128;
}
this.SendPacketToClient(loot);
LootRollsComplete complete = new LootRollsComplete();
complete.LootObj = loot.LootObj;
complete.LootListID = loot.Item.LootListID;
this.SendPacketToClient(complete);
}
```

---

### SMSG_LOOT_START_ROLL

- Legacy value: 673 (0x02A1)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:4395
- Fields:
  - `ReadGuid -> owner`
  - `ReadUInt32 -> MapID`
  - `ReadUInt32 -> LootListID`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadUInt32 -> Quantity`
  - `ReadUInt32 -> RollTime`
  - `ReadUInt8 -> ValidRolls`
  - `WriteGuid(owner)`
  - `WriteUInt32(loot.Item.LootListID)`
  - `WriteUInt8(0)`

```csharp
{
StartLootRoll loot = new StartLootRoll();
WowGuid64 owner = packet.ReadGuid();
loot.LootObj = owner.ToLootGuid();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
loot.MapID = packet.ReadUInt32();
}
else
{
loot.MapID = this.GetSession().GameState.CurrentMapId.Value;
}
loot.Item.LootListID = (byte)packet.ReadUInt32();
loot.Item.Loot.ItemID = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesSeed = packet.ReadUInt32();
loot.Item.Loot.RandomPropertiesID = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
loot.Item.Quantity = packet.ReadUInt32();
}
else
{
loot.Item.Quantity = 1u;
}
loot.RollTime = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
loot.ValidRolls = (RollMask)packet.ReadUInt8();
}
else
```

---

### SMSG_MAIL_COMMAND_RESULT

- Legacy value: 569 (0x0239)
- Modern value: 9787 (0x263B)
- Handler: HermesProxy/World/Client/WorldClient.cs:4774
- Fields:
  - `ReadUInt32 -> MailID`
  - `ReadUInt32 -> Command`
  - `ReadUInt32 -> ErrorCode`
  - `ReadUInt32 -> BagResult`
  - `ReadUInt32 -> AttachID`
  - `ReadUInt32 -> QtyInInventory`

```csharp
{
MailCommandResult mail = new MailCommandResult();
mail.MailID = packet.ReadUInt32();
mail.Command = (MailActionType)packet.ReadUInt32();
mail.ErrorCode = (MailErrorType)packet.ReadUInt32();
if (mail.ErrorCode == MailErrorType.Equip)
{
mail.BagResult = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32());
}
else if (mail.Command == MailActionType.AttachmentExpired)
{
mail.AttachID = packet.ReadUInt32();
mail.QtyInInventory = packet.ReadUInt32();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
mail.AttachID = 1u;
}
}
this.SendPacketToClient(mail);
}
```

---

### SMSG_MAIL_LIST_RESULT

- Legacy value: 571 (0x023B)
- Modern value: 10070 (0x2756)
- Handler: HermesProxy/World/Client/WorldClient.cs:4584
- Fields:
  - `ReadInt32 -> TotalNumRecords`
  - `ReadUInt8 -> count`
  - `ReadUInt16`
  - `ReadInt32 -> MailID`
  - `ReadUInt8 -> SenderType`
  - `ReadGuid -> SenderCharacter`
  - `ReadUInt32 -> AltSenderID`
  - `ReadUInt32 -> AltSenderID`
  - `ReadUInt32 -> Cod`
  - `ReadCString -> Subject`
  - `ReadUInt32 -> ItemTextId`
  - `WriteUInt32(mail.ItemTextId)`
  - `WriteInt32(mail.MailID)`
  - `WriteUInt32(0u)`
  - `ReadUInt32`
  - `ReadInt32 -> StationeryID`
  - `ReadUInt32 -> SentMoney`
  - `ReadUInt32 -> Cod`
  - `ReadUInt32 -> Flags`
  - `ReadFloat -> DaysLeft`
  - `ReadInt32 -> MailTemplateID`
  - `ReadCString -> Subject`
  - `ReadCString -> Body`
  - `ReadUInt8 -> itemsCount`

```csharp
{
MailListResult result = new MailListResult();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
result.TotalNumRecords = packet.ReadInt32();
}
byte count = packet.ReadUInt8();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_2_0_10192))
{
result.TotalNumRecords = count;
}
for (int i = 0; i < count; i++)
{
MailListEntry mail = new MailListEntry();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.ReadUInt16();
}
mail.MailID = packet.ReadInt32();
mail.SenderType = (MailType)packet.ReadUInt8();
switch (mail.SenderType)
{
case MailType.Normal:
mail.SenderCharacter = packet.ReadGuid().To128(this.GetSession().GameState);
break;
case MailType.Item:
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
mail.AltSenderID = packet.ReadUInt32();
}
```

---

### SMSG_MONSTER_MOVE_TRANSPORT

- Legacy value: 686 (0x02AE)
- Modern value: 11732 (0x2DD4)
- Handler: HermesProxy/World/Client/WorldClient.cs:5741
- Fields:
  - `ReadPackedGuid -> guid`
  - `ReadPackedGuid -> TransportGuid`
  - `ReadBool`
  - `ReadVector3`
  - `ReadUInt32 -> SplineId`
  - `ReadUInt8 -> type`
  - `ReadVector3`
  - `ReadGuid -> FinalFacingGuid`
  - `ReadFloat -> FinalOrientation`
  - `ReadUInt32 -> splineFlags`
  - `ReadUInt32 -> splineFlags2`
  - `ReadUInt32 -> splineFlags3`
  - `ReadUInt8`
  - `ReadInt32`
  - `ReadUInt32 -> SplineTimeFull`
  - `ReadFloat`
  - `ReadInt32`
  - `ReadUInt32 -> SplineCount`
  - `ReadVector3`
  - `ReadVector3`

```csharp
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
ServerSideMovement moveSpline = new ServerSideMovement();
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_MONSTER_MOVE_TRANSPORT)
{
moveSpline.TransportGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
moveSpline.TransportSeat = packet.ReadInt8();
}
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
packet.ReadBool();
}
moveSpline.StartPosition = packet.ReadVector3();
moveSpline.SplineId = packet.ReadUInt32();
SplineTypeLegacy type = (SplineTypeLegacy)packet.ReadUInt8();
switch (type)
{
case SplineTypeLegacy.FacingSpot:
moveSpline.SplineType = SplineTypeModern.FacingSpot;
moveSpline.FinalFacingSpot = packet.ReadVector3();
break;
case SplineTypeLegacy.FacingTarget:
moveSpline.SplineType = SplineTypeModern.FacingTarget;
moveSpline.FinalFacingGuid = packet.ReadGuid().To128(this.GetSession().GameState);
break;
case SplineTypeLegacy.FacingAngle:
moveSpline.SplineType = SplineTypeModern.FacingAngle;
```

---

### SMSG_MOTD

- Legacy value: 829 (0x033D)
- Modern value: 11183 (0x2BAF)
- Handler: HermesProxy/World/Client/WorldClient.cs:8865
- Fields:
  - `ReadUInt32 -> count`
  - `ReadCString`

```csharp
{
MOTD motd = new MOTD();
uint count = packet.ReadUInt32();
for (uint i = 0u; i < count; i++)
{
motd.Text.Add(packet.ReadCString());
}
this.SendPacketToClient(motd);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.GetSession().RealmSocket.SendSetTimeZoneInformation();
this.GetSession().RealmSocket.SendSeasonInfo();
}
}
```

---

### SMSG_MOVE_DISABLE_GRAVITY

- Legacy value: 1230 (0x04CE)
- Modern value: 11789 (0x2E0D)
- Handler: HermesProxy/World/Client/WorldClient.cs:5712
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_DISABLE_TRANSITION_BETWEEN_SWIM_AND_FLY

- Legacy value: 831 (0x033F)
- Modern value: 11788 (0x2E0C)
- Handler: HermesProxy/World/Client/WorldClient.cs:5711
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_ENABLE_GRAVITY

- Legacy value: 1232 (0x04D0)
- Modern value: 11790 (0x2E0E)
- Handler: HermesProxy/World/Client/WorldClient.cs:5713
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_ENABLE_TRANSITION_BETWEEN_SWIM_AND_FLY

- Legacy value: 830 (0x033E)
- Modern value: 11787 (0x2E0B)
- Handler: HermesProxy/World/Client/WorldClient.cs:5710
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_KNOCK_BACK

- Legacy value: 239 (0x00EF)
- Modern value: 11779 (0x2E03)
- Handler: HermesProxy/World/Client/WorldClient.cs:5453
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`
  - `ReadFloat -> HorizontalSpeed`
  - `ReadFloat -> VerticalSpeed`

```csharp
{
MoveKnockBack knockback = new MoveKnockBack();
knockback.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
knockback.MoveCounter = packet.ReadUInt32();
knockback.Direction = packet.ReadVector2();
knockback.HorizontalSpeed = packet.ReadFloat();
knockback.VerticalSpeed = packet.ReadFloat();
this.SendPacketToClient(knockback);
}
```

---

### SMSG_MOVE_ROOT

- Legacy value: 232 (0x00E8)
- Modern value: 11769 (0x2DF9)
- Handler: HermesProxy/World/Client/WorldClient.cs:5702
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SET_CAN_FLY

- Legacy value: 835 (0x0343)
- Modern value: 11781 (0x2E05)
- Handler: HermesProxy/World/Client/WorldClient.cs:5708
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SET_FEATHER_FALL

- Legacy value: 242 (0x00F2)
- Modern value: 11775 (0x2DFF)
- Handler: HermesProxy/World/Client/WorldClient.cs:5714
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SET_HOVERING

- Legacy value: 244 (0x00F4)
- Modern value: 11777 (0x2E01)
- Handler: HermesProxy/World/Client/WorldClient.cs:5706
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SET_LAND_WALK

- Legacy value: 223 (0x00DF)
- Modern value: 11774 (0x2DFE)
- Handler: HermesProxy/World/Client/WorldClient.cs:5705
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SET_NORMAL_FALL

- Legacy value: 243 (0x00F3)
- Modern value: 11776 (0x2E00)
- Handler: HermesProxy/World/Client/WorldClient.cs:5715
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SET_WATER_WALK

- Legacy value: 222 (0x00DE)
- Modern value: 11771 (0x2DFB)
- Handler: HermesProxy/World/Client/WorldClient.cs:5704
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_SPLINE_DISABLE_GRAVITY

- Legacy value: 1235 (0x04D3)
- Modern value: 11803 (0x2E1B)
- Handler: HermesProxy/World/Client/WorldClient.cs:5682
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_ENABLE_GRAVITY

- Legacy value: 1236 (0x04D4)
- Modern value: 11804 (0x2E1C)
- Handler: HermesProxy/World/Client/WorldClient.cs:5681
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_ROOT

- Legacy value: 794 (0x031A)
- Modern value: 11801 (0x2E19)
- Handler: HermesProxy/World/Client/WorldClient.cs:5679
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_FEATHER_FALL

- Legacy value: 773 (0x0305)
- Modern value: 11807 (0x2E1F)
- Handler: HermesProxy/World/Client/WorldClient.cs:5683
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_FLIGHT_BACK_SPEED

- Legacy value: 902 (0x0386)
- Modern value: 11756 (0x2DEC)
- Handler: HermesProxy/World/Client/WorldClient.cs:5592
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_FLIGHT_SPEED

- Legacy value: 901 (0x0385)
- Modern value: 11755 (0x2DEB)
- Handler: HermesProxy/World/Client/WorldClient.cs:5593
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_FLYING

- Legacy value: 1058 (0x0422)
- Modern value: 11817 (0x2E29)
- Handler: HermesProxy/World/Client/WorldClient.cs:5693
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_HOVER

- Legacy value: 775 (0x0307)
- Modern value: 11809 (0x2E21)
- Handler: HermesProxy/World/Client/WorldClient.cs:5685
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_LAND_WALK

- Legacy value: 778 (0x030A)
- Modern value: 11812 (0x2E24)
- Handler: HermesProxy/World/Client/WorldClient.cs:5688
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_NORMAL_FALL

- Legacy value: 774 (0x0306)
- Modern value: 11808 (0x2E20)
- Handler: HermesProxy/World/Client/WorldClient.cs:5684
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_PITCH_RATE

- Legacy value: 1118 (0x045E)
- Modern value: 11759 (0x2DEF)
- Handler: HermesProxy/World/Client/WorldClient.cs:5594
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_RUN_BACK_SPEED

- Legacy value: 767 (0x02FF)
- Modern value: 11752 (0x2DE8)
- Handler: HermesProxy/World/Client/WorldClient.cs:5595
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_RUN_MODE

- Legacy value: 781 (0x030D)
- Modern value: 11815 (0x2E27)
- Handler: HermesProxy/World/Client/WorldClient.cs:5691
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_RUN_SPEED

- Legacy value: 766 (0x02FE)
- Modern value: 11751 (0x2DE7)
- Handler: HermesProxy/World/Client/WorldClient.cs:5596
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_SWIM_BACK_SPEED

- Legacy value: 770 (0x0302)
- Modern value: 11754 (0x2DEA)
- Handler: HermesProxy/World/Client/WorldClient.cs:5597
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_SWIM_SPEED

- Legacy value: 768 (0x0300)
- Modern value: 11753 (0x2DE9)
- Handler: HermesProxy/World/Client/WorldClient.cs:5598
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_TURN_RATE

- Legacy value: 771 (0x0303)
- Modern value: 11758 (0x2DEE)
- Handler: HermesProxy/World/Client/WorldClient.cs:5599
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_WALK_BACK_SPEED

- Legacy value: 769 (0x0301)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5600
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_WALK_MODE

- Legacy value: 782 (0x030E)
- Modern value: 11816 (0x2E28)
- Handler: HermesProxy/World/Client/WorldClient.cs:5692
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_SET_WALK_SPEED

- Legacy value: N/A
- Modern value: 11757 (0x2DED)
- Handler: HermesProxy/World/Client/WorldClient.cs:5601
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
MoveSplineSetSpeed speed = new MoveSplineSetSpeed(packet.GetUniversalOpcode(isModern: false));
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
}
```

---

### SMSG_MOVE_SPLINE_SET_WATER_WALK

- Legacy value: 777 (0x0309)
- Modern value: 11811 (0x2E23)
- Handler: HermesProxy/World/Client/WorldClient.cs:5687
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_START_SWIM

- Legacy value: 779 (0x030B)
- Modern value: 11813 (0x2E25)
- Handler: HermesProxy/World/Client/WorldClient.cs:5689
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_STOP_SWIM

- Legacy value: 780 (0x030C)
- Modern value: 11814 (0x2E26)
- Handler: HermesProxy/World/Client/WorldClient.cs:5690
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_UNROOT

- Legacy value: 772 (0x0304)
- Modern value: 11802 (0x2E1A)
- Handler: HermesProxy/World/Client/WorldClient.cs:5680
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_UNSET_FLYING

- Legacy value: 1059 (0x0423)
- Modern value: 11818 (0x2E2A)
- Handler: HermesProxy/World/Client/WorldClient.cs:5694
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_SPLINE_UNSET_HOVER

- Legacy value: 776 (0x0308)
- Modern value: 11810 (0x2E22)
- Handler: HermesProxy/World/Client/WorldClient.cs:5686
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveSplineSetFlag spline = new MoveSplineSetFlag(packet.GetUniversalOpcode(isModern: false));
spline.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(spline);
}
```

---

### SMSG_MOVE_UNROOT

- Legacy value: 234 (0x00EA)
- Modern value: 11770 (0x2DFA)
- Handler: HermesProxy/World/Client/WorldClient.cs:5703
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_UNSET_CAN_FLY

- Legacy value: 836 (0x0344)
- Modern value: 11782 (0x2E06)
- Handler: HermesProxy/World/Client/WorldClient.cs:5709
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_MOVE_UNSET_HOVERING

- Legacy value: 245 (0x00F5)
- Modern value: 11778 (0x2E02)
- Handler: HermesProxy/World/Client/WorldClient.cs:5707
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
MoveSetFlag flag = new MoveSetFlag(packet.GetUniversalOpcode(isModern: false));
flag.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
flag.MoveCounter = packet.ReadUInt32();
this.SendPacketToClient(flag);
}
```

---

### SMSG_NEW_TAXI_PATH

- Legacy value: 431 (0x01AF)
- Modern value: 9854 (0x267E)
- Handler: HermesProxy/World/Client/WorldClient.cs:8919

```csharp
{
NewTaxiPath taxi = new NewTaxiPath();
this.SendPacketToClient(taxi);
}
```

---

### SMSG_NEW_WORLD

- Legacy value: 62 (0x003E)
- Modern value: 9620 (0x2594)
- Handler: HermesProxy/World/Client/WorldClient.cs:5553
- Fields:
  - `ReadUInt32 -> CurrentMapId`
  - `ReadVector3`
  - `ReadFloat -> Orientation`

```csharp
{
NewWorld teleport = new NewWorld();
this.GetSession().GameState.CurrentMapId = (teleport.MapID = packet.ReadUInt32());
teleport.Position = packet.ReadVector3();
teleport.Orientation = packet.ReadFloat();
teleport.Reason = 4u;
this.GetSession().GameState.IsFirstEnterWorld = false;
if (!this.GetSession().GameState.IsWaitingForNewWorld)
{
return;
}
this.GetSession().GameState.IsWaitingForNewWorld = false;
this.GetSession().GameState.IsWaitingForWorldPortAck = true;
this.SendPacketToClient(teleport);
if (teleport.MapID > 1)
{
UpdateLastInstance instance = new UpdateLastInstance();
instance.MapID = teleport.MapID;
this.SendPacketToClient(instance);
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
this.SendPacketToClient(new TimeSyncRequest());
}
ResumeToken resume = new ResumeToken();
resume.SequenceIndex = 3u;
resume.Reason = 1u;
this.SendPacketToClient(resume);
}
WorldServerInfo info = new WorldServerInfo();
if (teleport.MapID > 1)
```

---

### SMSG_NOTIFY_RECEIVED_MAIL

- Legacy value: 645 (0x0285)
- Modern value: 9788 (0x263C)
- Handler: HermesProxy/World/Client/WorldClient.cs:4541
- Fields:
  - `ReadFloat -> Delay`

```csharp
{
NotifyReceivedMail mail = new NotifyReceivedMail();
mail.Delay = packet.ReadFloat();
this.SendPacketToClient(mail);
}
```

---

### SMSG_ON_MONSTER_MOVE

- Legacy value: 221 (0x00DD)
- Modern value: 11732 (0x2DD4)
- Handler: HermesProxy/World/Client/WorldClient.cs:5740
- Fields:
  - `ReadPackedGuid -> guid`
  - `ReadPackedGuid -> TransportGuid`
  - `ReadBool`
  - `ReadVector3`
  - `ReadUInt32 -> SplineId`
  - `ReadUInt8 -> type`
  - `ReadVector3`
  - `ReadGuid -> FinalFacingGuid`
  - `ReadFloat -> FinalOrientation`
  - `ReadUInt32 -> splineFlags`
  - `ReadUInt32 -> splineFlags2`
  - `ReadUInt32 -> splineFlags3`
  - `ReadUInt8`
  - `ReadInt32`
  - `ReadUInt32 -> SplineTimeFull`
  - `ReadFloat`
  - `ReadInt32`
  - `ReadUInt32 -> SplineCount`
  - `ReadVector3`
  - `ReadVector3`

```csharp
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
ServerSideMovement moveSpline = new ServerSideMovement();
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_MONSTER_MOVE_TRANSPORT)
{
moveSpline.TransportGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
moveSpline.TransportSeat = packet.ReadInt8();
}
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
packet.ReadBool();
}
moveSpline.StartPosition = packet.ReadVector3();
moveSpline.SplineId = packet.ReadUInt32();
SplineTypeLegacy type = (SplineTypeLegacy)packet.ReadUInt8();
switch (type)
{
case SplineTypeLegacy.FacingSpot:
moveSpline.SplineType = SplineTypeModern.FacingSpot;
moveSpline.FinalFacingSpot = packet.ReadVector3();
break;
case SplineTypeLegacy.FacingTarget:
moveSpline.SplineType = SplineTypeModern.FacingTarget;
moveSpline.FinalFacingGuid = packet.ReadGuid().To128(this.GetSession().GameState);
break;
case SplineTypeLegacy.FacingAngle:
moveSpline.SplineType = SplineTypeModern.FacingAngle;
```

---

### SMSG_PARTY_COMMAND_RESULT

- Legacy value: 127 (0x007F)
- Modern value: 10134 (0x2796)
- Handler: HermesProxy/World/Client/WorldClient.cs:2296
- Fields:
  - `ReadUInt32 -> Command`
  - `ReadCString -> Name`
  - `ReadUInt32 -> partyResult`
  - `ReadUInt32 -> ResultData`

```csharp
{
PartyCommandResult party = new PartyCommandResult();
party.Command = (byte)packet.ReadUInt32();
party.Name = packet.ReadCString();
uint partyResult = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
party.Result = (byte)partyResult;
}
else
{
Type typeFromHandle = typeof(PartyResultModern);
PartyResultVanilla partyResultVanilla = (PartyResultVanilla)partyResult;
party.Result = (byte)Enum.Parse(typeFromHandle, partyResultVanilla.ToString());
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
party.ResultData = packet.ReadUInt32();
}
this.SendPacketToClient(party);
}
```

---

### SMSG_PARTY_INVITE

- Legacy value: 111 (0x006F)
- Modern value: 9661 (0x25BD)
- Handler: HermesProxy/World/Client/WorldClient.cs:2328
- Fields:
  - `ReadBool -> CanAccept`
  - `ReadCString -> InviterName`
  - `ReadUInt32 -> ProposedRoles`
  - `ReadUInt8 -> lfgSlotsCount`
  - `ReadInt32`
  - `ReadInt32 -> LfgCompletedMask`

```csharp
{
PartyInvite party = new PartyInvite();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
party.CanAccept = packet.ReadBool();
}
Realm realm = this.GetSession().RealmManager.GetRealm(this.GetSession().RealmId);
party.InviterRealm = new VirtualRealmInfo(realm.Id.GetAddress(), isHomeRealm: true, isInternalRealm: false, realm.Name, realm.NormalizedName);
party.InviterName = packet.ReadCString();
party.InviterGUID = this.GetSession().GameState.GetPlayerGuidByName(party.InviterName);
if (party.InviterGUID == null)
{
party.InviterGUID = WowGuid128.Empty;
party.InviterBNetAccountId = WowGuid128.Empty;
}
else
{
party.InviterBNetAccountId = this.GetSession().GetBnetAccountGuidForPlayer(party.InviterGUID);
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
party.ProposedRoles = packet.ReadUInt32();
byte lfgSlotsCount = packet.ReadUInt8();
for (int i = 0; i < lfgSlotsCount; i++)
{
party.LfgSlots.Add(packet.ReadInt32());
}
party.LfgCompletedMask = packet.ReadInt32();
}
this.SendPacketToClient(party);
```

---

### SMSG_PARTY_KILL_LOG

- Legacy value: 501 (0x01F5)
- Modern value: 10074 (0x275A)
- Handler: HermesProxy/World/Client/WorldClient.cs:2194
- Fields:
  - `ReadGuid -> Player`
  - `ReadGuid -> Victim`

```csharp
{
PartyKillLog log = new PartyKillLog();
log.Player = packet.ReadGuid().To128(this.GetSession().GameState);
log.Victim = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(log);
}
```

---

### SMSG_PARTY_MEMBER_FULL_STATE

- Legacy value: 754 (0x02F2)
- Modern value: 10073 (0x2759)
- Handler: HermesProxy/World/Client/WorldClient.cs:3078, HermesProxy/World/Client/WorldClient.cs:3292
- Fields:
  - `ReadPackedGuid -> MemberGuid`
  - `ReadUInt32 -> updateFlags`
  - `ReadUInt8 -> StatusFlags`
  - `ReadUInt16 -> CurrentHealth`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8 -> PowerType`
  - `ReadUInt16 -> CurrentPower`
  - `ReadUInt16 -> MaxPower`
  - `ReadUInt16 -> Level`
  - `ReadUInt16 -> ZoneID`
  - `ReadInt16 -> PositionX`
  - `ReadInt16 -> PositionY`
  - `ReadUInt32 -> auraMask`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt16 -> auraMask2`
  - `ReadUInt16 -> SpellId`
  - `ReadGuid -> NewPetGuid`
  - `ReadCString -> NewPetName`
  - `ReadUInt16 -> DisplayID`
  - `ReadUInt16 -> Health`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8`
  - `ReadInt16`
  - `ReadInt16`
  - `ReadUInt32 -> auraMask3`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt16 -> auraMask4`
  - `ReadUInt16 -> SpellId`
  - `ReadPackedGuid -> MemberGuid`
  - `ReadUInt32 -> updateFlags`
  - `ReadUInt16 -> StatusFlags`
  - `ReadUInt16 -> CurrentHealth`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8 -> PowerType`
  - `ReadUInt16 -> CurrentPower`
  - `ReadUInt16 -> MaxPower`
  - `ReadUInt16 -> Level`
  - `ReadUInt16 -> ZoneID`
  - `ReadInt16 -> PositionX`
  - `ReadInt16 -> PositionY`
  - `ReadUInt64 -> auraMask`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt8`
  - `ReadGuid -> NewPetGuid`
  - `ReadCString -> NewPetName`
  - `ReadUInt16 -> DisplayID`
  - `ReadUInt16 -> Health`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8`
  - `ReadInt16`
  - `ReadInt16`
  - `ReadUInt64 -> auraMask2`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt8`

```csharp
{
if (this.GetSession().GameState.CurrentMapId == 489 && (this.GetSession().GameState.HasWsgAllyFlagCarrier || this.GetSession().GameState.HasWsgHordeFlagCarrier) && this._requestBgPlayerPosCounter++ > 10)
{
WorldPacket packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
this.SendPacket(packet2);
this._requestBgPlayerPosCounter = 0u;
}
PartyMemberFullState state = new PartyMemberFullState();
if (this.GetSession().GameState.IsInBattleground())
{
state.PartyType[0] = 0;
state.PartyType[1] = 2;
}
else
{
state.PartyType[0] = 1;
state.PartyType[1] = 0;
}
state.MemberGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
GroupUpdateFlagVanilla updateFlags = (GroupUpdateFlagVanilla)packet.ReadUInt32();
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Status))
{
state.StatusFlags = (GroupMemberOnlineStatus)packet.ReadUInt8();
}
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentHealth))
{
state.CurrentHealth = packet.ReadUInt16();
}
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.MaxHealth))
{
```

---

### SMSG_PARTY_MEMBER_PARTIAL_STATE

- Legacy value: 126 (0x007E)
- Modern value: 10072 (0x2758)
- Handler: HermesProxy/World/Client/WorldClient.cs:2718, HermesProxy/World/Client/WorldClient.cs:2923
- Fields:
  - `ReadPackedGuid -> AffectedGUID`
  - `ReadUInt32 -> updateFlags`
  - `ReadUInt8 -> StatusFlags`
  - `ReadUInt16 -> CurrentHealth`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8 -> PowerType`
  - `ReadUInt16 -> CurrentPower`
  - `ReadUInt16 -> MaxPower`
  - `ReadUInt16 -> Level`
  - `ReadUInt16 -> ZoneID`
  - `ReadInt16 -> X`
  - `ReadInt16 -> Y`
  - `ReadUInt32 -> auraMask`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt16 -> auraMask2`
  - `ReadUInt16 -> SpellId`
  - `ReadGuid -> NewPetGuid`
  - `ReadCString -> NewPetName`
  - `ReadUInt16 -> DisplayID`
  - `ReadUInt16 -> Health`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8`
  - `ReadInt16`
  - `ReadInt16`
  - `ReadUInt32 -> auraMask3`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt16 -> auraMask4`
  - `ReadUInt16 -> SpellId`
  - `ReadPackedGuid -> AffectedGUID`
  - `ReadUInt32 -> updateFlags`
  - `ReadUInt16 -> StatusFlags`
  - `ReadUInt16 -> CurrentHealth`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8 -> PowerType`
  - `ReadUInt16 -> CurrentPower`
  - `ReadUInt16 -> MaxPower`
  - `ReadUInt16 -> Level`
  - `ReadUInt16 -> ZoneID`
  - `ReadInt16 -> X`
  - `ReadInt16 -> Y`
  - `ReadUInt64 -> auraMask`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt8`
  - `ReadGuid -> NewPetGuid`
  - `ReadCString -> NewPetName`
  - `ReadUInt16 -> DisplayID`
  - `ReadUInt16 -> Health`
  - `ReadUInt16 -> MaxHealth`
  - `ReadUInt8`
  - `ReadInt16`
  - `ReadInt16`
  - `ReadUInt64 -> auraMask2`
  - `ReadUInt16 -> SpellId`
  - `ReadUInt8`

```csharp
{
if (this.GetSession().GameState.CurrentMapId == 489 && (this.GetSession().GameState.HasWsgAllyFlagCarrier || this.GetSession().GameState.HasWsgHordeFlagCarrier) && this._requestBgPlayerPosCounter++ > 10)
{
WorldPacket packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
this.SendPacket(packet2);
this._requestBgPlayerPosCounter = 0u;
}
PartyMemberPartialState state = new PartyMemberPartialState();
state.AffectedGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
GroupUpdateFlagVanilla updateFlags = (GroupUpdateFlagVanilla)packet.ReadUInt32();
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.Status))
{
state.StatusFlags = packet.ReadUInt8();
}
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentHealth))
{
state.CurrentHealth = packet.ReadUInt16();
}
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.MaxHealth))
{
state.MaxHealth = packet.ReadUInt16();
}
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.PowerType))
{
state.PowerType = packet.ReadUInt8();
}
if (updateFlags.HasFlag(GroupUpdateFlagVanilla.CurrentPower))
{
state.CurrentPower = packet.ReadUInt16();
}
```

---

### SMSG_PAUSE_MIRROR_TIMER

- Legacy value: 474 (0x01DA)
- Modern value: 10000 (0x2710)
- Handler: HermesProxy/World/Client/WorldClient.cs:5032
- Fields:
  - `ReadUInt32 -> Timer`
  - `ReadBool -> Paused`

```csharp
{
PauseMirrorTimer timer = new PauseMirrorTimer();
timer.Timer = (MirrorTimerType)packet.ReadUInt32();
timer.Paused = packet.ReadBool();
this.SendPacketToClient(timer);
}
```

---

### SMSG_PETITION_SHOW_LIST

- Legacy value: 444 (0x01BC)
- Modern value: 9919 (0x26BF)
- Handler: HermesProxy/World/Client/WorldClient.cs:6244
- Fields:
  - `ReadGuid -> Unit`
  - `ReadUInt8 -> count`
  - `ReadUInt32 -> Index`
  - `ReadUInt32 -> CharterEntry`
  - `ReadUInt32`
  - `ReadUInt32 -> CharterCost`
  - `ReadUInt32`
  - `ReadUInt32 -> RequiredSignatures`

```csharp
{
ServerPetitionShowList petitions = new ServerPetitionShowList();
petitions.Unit = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = petitions.Unit;
byte count = packet.ReadUInt8();
for (int i = 0; i < count; i++)
{
PetitionEntry petition = default(PetitionEntry);
petition.Index = packet.ReadUInt32();
petition.CharterEntry = packet.ReadUInt32();
petition.IsArena = ((petition.CharterEntry != 5863) ? 1u : 0u);
packet.ReadUInt32();
petition.CharterCost = packet.ReadUInt32();
packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
petition.RequiredSignatures = packet.ReadUInt32();
}
else
{
petition.RequiredSignatures = 9u;
}
petitions.Petitions.Add(petition);
}
this.SendPacketToClient(petitions);
}
```

---

### SMSG_PETITION_SHOW_SIGNATURES

- Legacy value: 447 (0x01BF)
- Modern value: 9920 (0x26C0)
- Handler: HermesProxy/World/Client/WorldClient.cs:6273
- Fields:
  - `ReadGuid -> Item`
  - `ReadGuid -> Owner`
  - `ReadInt32 -> PetitionID`
  - `ReadUInt8 -> counter`
  - `ReadGuid -> Signer`
  - `ReadInt32 -> Choice`

```csharp
{
ServerPetitionShowSignatures petition = new ServerPetitionShowSignatures();
petition.Item = packet.ReadGuid().To128(this.GetSession().GameState);
petition.Owner = packet.ReadGuid().To128(this.GetSession().GameState);
petition.OwnerAccountID = this.GetSession().GetGameAccountGuidForPlayer(petition.Owner);
petition.PetitionID = packet.ReadInt32();
byte counter = packet.ReadUInt8();
for (int i = 0; i < counter; i++)
{
ServerPetitionShowSignatures.PetitionSignature signature = new ServerPetitionShowSignatures.PetitionSignature
{
Signer = packet.ReadGuid().To128(this.GetSession().GameState),
Choice = packet.ReadInt32()
};
petition.Signatures.Add(signature);
}
this.SendPacketToClient(petition);
}
```

---

### SMSG_PETITION_SIGN_RESULTS

- Legacy value: 449 (0x01C1)
- Modern value: 10060 (0x274C)
- Handler: HermesProxy/World/Client/WorldClient.cs:6356
- Fields:
  - `ReadGuid -> Item`
  - `ReadGuid -> Player`
  - `ReadUInt32 -> Error`

```csharp
{
PetitionSignResults petition = new PetitionSignResults();
petition.Item = packet.ReadGuid().To128(this.GetSession().GameState);
petition.Player = packet.ReadGuid().To128(this.GetSession().GameState);
petition.Error = (PetitionSignResult)packet.ReadUInt32();
this.SendPacketToClient(petition);
}
```

---

### SMSG_PET_ACTION_SOUND

- Legacy value: 804 (0x0324)
- Modern value: 9888 (0x26A0)
- Handler: HermesProxy/World/Client/WorldClient.cs:6157
- Fields:
  - `ReadGuid -> UnitGUID`
  - `ReadUInt32 -> Action`

```csharp
{
PetActionSound sound = new PetActionSound();
sound.UnitGUID = packet.ReadGuid().To128(this.GetSession().GameState);
sound.Action = packet.ReadUInt32();
this.SendPacketToClient(sound);
}
```

---

### SMSG_PET_BROKEN

- Legacy value: 687 (0x02AF)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:6166

```csharp
{
PrintNotification notify = new PrintNotification();
notify.NotifyText = "Your pet has run away";
this.SendPacketToClient(notify);
}
```

---

### SMSG_PET_CAST_FAILED

- Legacy value: 312 (0x0138)
- Modern value: 11349 (0x2C55)
- Handler: HermesProxy/World/Client/WorldClient.cs:7801, HermesProxy/World/Client/WorldClient.cs:7834
- Fields:
  - `ReadUInt32 -> spellId`
  - `ReadUInt8 -> status`
  - `ReadUInt8 -> reason`
  - `ReadUInt8`
  - `ReadUInt32 -> spellId`
  - `ReadUInt8 -> reason`
  - `ReadInt32 -> FailedArg1`
  - `ReadInt32 -> FailedArg2`

```csharp
{
if (Settings.ClientSpellDelay > 0)
{
Thread.Sleep(Settings.ClientSpellDelay);
}
uint spellId = packet.ReadUInt32();
byte status = packet.ReadUInt8();
if (status != 2 || this.GetSession().GameState.CurrentClientPetCast == null || this.GetSession().GameState.CurrentClientPetCast.SpellId != spellId)
{
return;
}
if (!this.GetSession().GameState.CurrentClientPetCast.HasStarted)
{
SpellPrepare prepare2 = new SpellPrepare();
prepare2.ClientCastID = this.GetSession().GameState.CurrentClientPetCast.ClientGUID;
prepare2.ServerCastID = this.GetSession().GameState.CurrentClientPetCast.ServerGUID;
this.SendPacketToClient(prepare2);
}
PetCastFailed spell = new PetCastFailed();
spell.SpellID = spellId;
uint reason = packet.ReadUInt8();
spell.Reason = LegacyVersion.ConvertSpellCastResult(reason);
spell.CastID = this.GetSession().GameState.CurrentClientPetCast.ServerGUID;
this.SendPacketToClient(spell);
foreach (ClientCastRequest pending in this.GetSession().GameState.PendingClientPetCasts)
{
this.GetSession().InstanceSocket.SendCastRequestFailed(pending, isPet: true);
}
this.GetSession().GameState.PendingClientPetCasts.Clear();
}
```

---

### SMSG_PET_SPELLS_MESSAGE

- Legacy value: 377 (0x0179)
- Modern value: 11298 (0x2C22)
- Handler: HermesProxy/World/Client/WorldClient.cs:6105
- Fields:
  - `ReadGuid -> guid`
  - `ReadUInt16 -> CreatureFamily`
  - `ReadUInt32 -> TimeLimit`
  - `ReadUInt8 -> ReactState`
  - `ReadUInt8 -> CommandState`
  - `ReadUInt8`
  - `ReadUInt8 -> Flag`
  - `ReadUInt32`
  - `ReadUInt8 -> spellCount`
  - `ReadUInt32`
  - `ReadUInt8 -> cdCount`
  - `ReadUInt32 -> SpellID`
  - `ReadUInt16 -> SpellID`
  - `ReadUInt16 -> Category`
  - `ReadUInt32 -> Duration`
  - `ReadUInt32 -> CategoryDuration`

```csharp
{
WowGuid guid = packet.ReadGuid();
this.GetSession().GameState.CurrentPetGuid = guid.To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentClientPetCast = null;
if (guid.IsEmpty())
{
PetClearSpells clear = new PetClearSpells();
this.SendPacketToClient(clear);
return;
}
PetSpells spells = new PetSpells();
spells.PetGUID = guid.To128(this.GetSession().GameState);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
spells.CreatureFamily = packet.ReadUInt16();
}
spells.TimeLimit = packet.ReadUInt32();
spells.ReactState = (ReactStates)packet.ReadUInt8();
spells.CommandState = (CommandStates)packet.ReadUInt8();
packet.ReadUInt8();
spells.Flag = packet.ReadUInt8();
for (int i = 0; i < 10; i++)
{
spells.ActionButtons[i] = packet.ReadUInt32();
}
byte spellCount = packet.ReadUInt8();
for (int j = 0; j < spellCount; j++)
{
spells.Actions.Add(packet.ReadUInt32());
}
```

---

### SMSG_PET_STABLE_RESULT

- Legacy value: 627 (0x0273)
- Modern value: 9619 (0x2593)
- Handler: HermesProxy/World/Client/WorldClient.cs:6236
- Fields:
  - `ReadUInt8 -> Result`

```csharp
{
PetStableResult stable = new PetStableResult();
stable.Result = packet.ReadUInt8();
this.SendPacketToClient(stable);
}
```

---

### SMSG_PET_UNLEARN_CONFIRM

- Legacy value: 753 (0x02F1)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:6174
- Fields:
  - `ReadGuid -> TrainerGUID`
  - `ReadUInt32 -> Cost`

```csharp
{
RespecWipeConfirm respec = new RespecWipeConfirm();
respec.TrainerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
respec.Cost = packet.ReadUInt32();
respec.RespecType = SpecResetType.PetTalents;
this.SendPacketToClient(respec);
}
```

---

### SMSG_PLAYED_TIME

- Legacy value: 461 (0x01CD)
- Modern value: 9941 (0x26D5)
- Handler: HermesProxy/World/Client/WorldClient.cs:1233
- Fields:
  - `ReadUInt32 -> TotalTime`
  - `ReadUInt32 -> LevelTime`
  - `ReadBool -> TriggerEvent`

```csharp
{
PlayedTime played = new PlayedTime();
played.TotalTime = packet.ReadUInt32();
played.LevelTime = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
played.TriggerEvent = packet.ReadBool();
}
else
{
played.TriggerEvent = this.GetSession().GameState.ShowPlayedTime;
}
this.SendPacketToClient(played);
}
```

---

### SMSG_PLAYER_BOUND

- Legacy value: 344 (0x0158)
- Modern value: 12280 (0x2FF8)
- Handler: HermesProxy/World/Client/WorldClient.cs:4838
- Fields:
  - `ReadGuid -> BinderGUID`
  - `ReadUInt32 -> AreaID`

```csharp
{
PlayerBound bound = new PlayerBound();
bound.BinderGUID = packet.ReadGuid().To128(this.GetSession().GameState);
bound.AreaID = packet.ReadUInt32();
this.SendPacketToClient(bound);
}
```

---

### SMSG_PLAYER_SKINNED

- Legacy value: 700 (0x02BC)
- Modern value: 12294 (0x3006)
- Handler: HermesProxy/World/Client/WorldClient.cs:834
- Fields:
  - `ReadBool -> FreeRepop`

```csharp
{
PlayerSkinned skinned = new PlayerSkinned();
if (packet.CanRead())
{
skinned.FreeRepop = packet.ReadBool();
}
this.SendPacketToClient(skinned);
}
```

---

### SMSG_PLAY_MUSIC

- Legacy value: 631 (0x0277)
- Modern value: 10093 (0x276D)
- Handler: HermesProxy/World/Client/WorldClient.cs:4976
- Fields:
  - `ReadUInt32 -> SoundEntryID`

```csharp
{
PlayMusic music = new PlayMusic();
music.SoundEntryID = packet.ReadUInt32();
this.SendPacketToClient(music);
}
```

---

### SMSG_PLAY_OBJECT_SOUND

- Legacy value: 632 (0x0278)
- Modern value: 10094 (0x276E)
- Handler: HermesProxy/World/Client/WorldClient.cs:4993
- Fields:
  - `ReadUInt32 -> SoundEntryID`
  - `ReadGuid -> SourceObjectGUID`

```csharp
{
PlayObjectSound sound = new PlayObjectSound();
sound.SoundEntryID = packet.ReadUInt32();
sound.SourceObjectGUID = packet.ReadGuid().To128(this.GetSession().GameState);
sound.TargetObjectGUID = sound.SourceObjectGUID;
this.SendPacketToClient(sound);
}
```

---

### SMSG_PLAY_SOUND

- Legacy value: 722 (0x02D2)
- Modern value: 10092 (0x276C)
- Handler: HermesProxy/World/Client/WorldClient.cs:4984
- Fields:
  - `ReadUInt32 -> SoundEntryID`

```csharp
{
PlaySound sound = new PlaySound();
sound.SoundEntryID = packet.ReadUInt32();
sound.SourceObjectGuid = this.GetSession().GameState.CurrentPlayerGuid;
this.SendPacketToClient(sound);
}
```

---

### SMSG_PLAY_SPELL_VISUAL

- Legacy value: 499 (0x01F3)
- Modern value: 11330 (0x2C42)
- Handler: HermesProxy/World/Client/WorldClient.cs:8599
- Fields:
  - `ReadGuid -> Unit`
  - `ReadUInt32 -> KitRecID`

```csharp
{
PlaySpellVisualKit spell = new PlaySpellVisualKit();
spell.Unit = packet.ReadGuid().To128(this.GetSession().GameState);
spell.KitRecID = packet.ReadUInt32();
this.SendPacketToClient(spell);
}
```

---

### SMSG_PONG

- Legacy value: 477 (0x01DD)
- Modern value: 12366 (0x304E)
- Handler: HermesProxy/World/Client/WorldClient.cs:4797
- Fields:
  - `ReadUInt32 -> serial`

```csharp
{
uint serial = packet.ReadUInt32();
this.SendPacketToClient(new Pong(serial));
}
```

---

### SMSG_POWER_UPDATE

- Legacy value: 1152 (0x0480)
- Modern value: 9938 (0x26D2)
- Handler: HermesProxy/World/Client/WorldClient.cs:5080
- Fields:
  - `ReadPackedGuid -> guid`
  - `ReadUInt8 -> powerType`
  - `ReadInt32 -> powerValue`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
byte powerType = packet.ReadUInt8();
int powerValue = packet.ReadInt32();
PowerUpdate update = new PowerUpdate(guid);
update.Powers.Add(new PowerUpdatePower(powerValue, powerType));
this.SendPacketToClient(update);
}
}
```

---

### SMSG_PRINT_NOTIFICATION

- Legacy value: 459 (0x01CB)
- Modern value: 9674 (0x25CA)
- Handler: HermesProxy/World/Client/WorldClient.cs:1943
- Fields:
  - `ReadCString -> NotifyText`

```csharp
{
PrintNotification notify = new PrintNotification();
notify.NotifyText = packet.ReadCString();
this.SendPacketToClient(notify);
}
```

---

### SMSG_PVP_CREDIT

- Legacy value: 652 (0x028C)
- Modern value: 10570 (0x294A)
- Handler: HermesProxy/World/Client/WorldClient.cs:824
- Fields:
  - `ReadInt32 -> OriginalHonor`
  - `ReadGuid -> Target`
  - `ReadUInt32 -> Rank`

```csharp
{
PvPCredit credit = new PvPCredit();
credit.OriginalHonor = packet.ReadInt32();
credit.Target = packet.ReadGuid().To128(this.GetSession().GameState);
credit.Rank = packet.ReadUInt32();
this.SendPacketToClient(credit);
}
```

---

### SMSG_QUERY_CREATURE_RESPONSE

- Legacy value: 97 (0x0061)
- Modern value: 10516 (0x2914)
- Handler: HermesProxy/World/Client/WorldClient.cs:6631
- Fields:
  - `ReadCString`
  - `ReadCString -> Title`
  - `ReadCString -> CursorName`
  - `ReadUInt32`
  - `ReadInt32 -> Type`
  - `ReadInt32 -> Family`
  - `ReadInt32 -> Classification`
  - `ReadUInt32`
  - `ReadInt32`
  - `ReadUInt32 -> PetSpellDataId`
  - `ReadUInt32 -> displayId`
  - `ReadFloat -> HpMulti`
  - `ReadFloat -> EnergyMulti`
  - `ReadBool -> Civilian`
  - `ReadBool -> Leader`
  - `ReadUInt32 -> itemId`
  - `ReadUInt32`

```csharp
{
QueryCreatureResponse response = new QueryCreatureResponse();
KeyValuePair<int, bool> id = packet.ReadEntry();
response.CreatureID = (uint)id.Key;
if (id.Value)
{
response.Allow = false;
this.SendPacketToClient(response);
return;
}
response.Allow = true;
response.Stats = new CreatureTemplate();
CreatureTemplate creature = response.Stats;
for (int i = 0; i < 4; i++)
{
creature.Name[i] = packet.ReadCString();
}
creature.Title = packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
creature.CursorName = packet.ReadCString();
}
creature.Flags[0] = packet.ReadUInt32();
creature.Type = packet.ReadInt32();
creature.Family = packet.ReadInt32();
creature.Classification = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
for (int j = 0; j < 2; j++)
{
```

---

### SMSG_QUERY_GAME_OBJECT_RESPONSE

- Legacy value: 95 (0x005F)
- Modern value: 10517 (0x2915)
- Handler: HermesProxy/World/Client/WorldClient.cs:6718
- Fields:
  - `ReadUInt32 -> Type`
  - `ReadUInt32 -> DisplayID`
  - `ReadCString`
  - `ReadCString -> IconName`
  - `ReadCString -> CastBarCaption`
  - `ReadCString -> UnkString`
  - `ReadInt32`
  - `ReadFloat -> Size`
  - `ReadUInt32 -> itemId`

```csharp
{
QueryGameObjectResponse response = new QueryGameObjectResponse();
KeyValuePair<int, bool> id = packet.ReadEntry();
response.GameObjectID = (uint)id.Key;
response.Guid = WowGuid128.Empty;
if (id.Value)
{
response.Allow = false;
this.SendPacketToClient(response);
return;
}
response.Allow = true;
response.Stats = new GameObjectStats();
GameObjectStats gameObject = response.Stats;
gameObject.Type = packet.ReadUInt32();
gameObject.DisplayID = packet.ReadUInt32();
for (int i = 0; i < 4; i++)
{
gameObject.Name[i] = packet.ReadCString();
}
gameObject.IconName = packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
gameObject.CastBarCaption = packet.ReadCString();
gameObject.UnkString = packet.ReadCString();
}
for (int j = 0; j < 24; j++)
{
gameObject.Data[j] = packet.ReadInt32();
}
```

---

### SMSG_QUERY_GUILD_INFO_RESPONSE

- Legacy value: 85 (0x0055)
- Modern value: 10726 (0x29E6)
- Handler: HermesProxy/World/Client/WorldClient.cs:3640
- Fields:
  - `ReadUInt32 -> guildId`
  - `ReadCString -> GuildName`
  - `ReadCString -> rankName`
  - `ReadUInt32 -> EmblemStyle`
  - `ReadUInt32 -> EmblemColor`
  - `ReadUInt32 -> BorderStyle`
  - `ReadUInt32 -> BorderColor`
  - `ReadUInt32 -> BackgroundColor`

```csharp
{
QueryGuildInfoResponse guild = new QueryGuildInfoResponse();
uint guildId = packet.ReadUInt32();
guild.GuildGUID = WowGuid128.Create(HighGuidType703.Guild, guildId);
guild.PlayerGuid = this.GetSession().GameState.CurrentPlayerGuid;
guild.HasGuildInfo = true;
guild.Info = new QueryGuildInfoResponse.GuildInfo();
guild.Info.GuildGuid = guild.GuildGUID;
guild.Info.VirtualRealmAddress = this.GetSession().RealmId.GetAddress();
guild.Info.GuildName = packet.ReadCString();
this.GetSession().StoreGuildGuidAndName(guild.GuildGUID, guild.Info.GuildName);
List<string> ranks = new List<string>();
for (uint i = 0u; i < 10; i++)
{
string rankName = packet.ReadCString();
if (!string.IsNullOrEmpty(rankName))
{
QueryGuildInfoResponse.GuildInfo.RankInfo rank = new QueryGuildInfoResponse.GuildInfo.RankInfo
{
RankID = i,
RankOrder = i,
RankName = rankName
};
ranks.Add(rankName);
guild.Info.Ranks.Add(rank);
}
}
this.GetSession().StoreGuildRankNames(guildId, ranks);
guild.Info.EmblemStyle = packet.ReadUInt32();
guild.Info.EmblemColor = packet.ReadUInt32();
```

---

### SMSG_QUERY_ITEM_TEXT_RESPONSE

- Legacy value: 580 (0x0244)
- Modern value: 10526 (0x291E)
- Handler: HermesProxy/World/Client/WorldClient.cs:4697
- Fields:
  - `ReadUInt32 -> itemTextId`
  - `ReadCString -> text`

```csharp
{
uint itemTextId = packet.ReadUInt32();
string text = packet.ReadCString();
if (this.GetSession().GameState.ItemTexts.ContainsKey(itemTextId))
{
this.GetSession().GameState.ItemTexts[itemTextId] = text;
}
else
{
this.GetSession().GameState.ItemTexts.Add(itemTextId, text);
}
if (this.GetSession().GameState.RequestedItemTextIds.Contains(itemTextId))
{
this.GetSession().GameState.RequestedItemTextIds.Remove(itemTextId);
}
if (this.GetSession().GameState.PendingMailListPacket == null || this.GetSession().GameState.RequestedItemTextIds.Count != 0)
{
return;
}
MailListResult result = this.GetSession().GameState.PendingMailListPacket;
foreach (MailListEntry mail in result.Mails)
{
if (mail.ItemTextId != 0)
{
mail.Body = this.GetSession().GameState.ItemTexts[mail.ItemTextId];
}
}
this.SendPacketToClient(result);
}
```

---

### SMSG_QUERY_NPC_TEXT_RESPONSE

- Legacy value: 384 (0x0180)
- Modern value: 10518 (0x2916)
- Handler: HermesProxy/World/Client/WorldClient.cs:6789
- Fields:
  - `ReadFloat`
  - `ReadCString -> maleText`
  - `ReadCString -> femaleText`
  - `ReadUInt32 -> language`
  - `ReadUInt32`
  - `ReadUInt32`

```csharp
{
QueryNPCTextResponse response = new QueryNPCTextResponse();
KeyValuePair<int, bool> id = packet.ReadEntry();
response.TextID = (uint)id.Key;
if (id.Value)
{
response.Allow = false;
this.SendPacketToClient(response);
return;
}
response.Allow = true;
for (int i = 0; i < 8; i++)
{
response.Probabilities[i] = packet.ReadFloat();
string maleText = packet.ReadCString().TrimEnd().Replace("\0", "");
string femaleText = packet.ReadCString().TrimEnd().Replace("\0", "");
uint language = packet.ReadUInt32();
ushort[] emoteDelays = new ushort[3];
ushort[] emotes = new ushort[3];
for (int j = 0; j < 3; j++)
{
emoteDelays[j] = (ushort)packet.ReadUInt32();
emotes[j] = (ushort)packet.ReadUInt32();
}
if ((string.IsNullOrEmpty(maleText) && string.IsNullOrEmpty(femaleText)) || (maleText.Equals("Greetings $N") && femaleText.Equals("Greetings $N") && i != 0))
{
response.BroadcastTextID[i] = 0u;
}
else
{
```

---

### SMSG_QUERY_PAGE_TEXT_RESPONSE

- Legacy value: 91 (0x005B)
- Modern value: 10519 (0x2917)
- Handler: HermesProxy/World/Client/WorldClient.cs:6773
- Fields:
  - `ReadUInt32 -> PageTextID`
  - `ReadCString -> Text`
  - `ReadUInt32 -> NextPageID`

```csharp
{
QueryPageTextResponse response = new QueryPageTextResponse();
response.PageTextID = packet.ReadUInt32();
response.Allow = true;
QueryPageTextResponse.PageTextInfo page = new QueryPageTextResponse.PageTextInfo
{
Id = response.PageTextID,
Text = packet.ReadCString(),
NextPageID = packet.ReadUInt32()
};
response.Pages.Add(page);
this.SendPacketToClient(response);
}
```

---

### SMSG_QUERY_PETITION_RESPONSE

- Legacy value: 455 (0x01C7)
- Modern value: 10523 (0x291B)
- Handler: HermesProxy/World/Client/WorldClient.cs:6294
- Fields:
  - `ReadUInt32 -> PetitionID`
  - `ReadGuid -> Petitioner`
  - `ReadCString -> Title`
  - `ReadCString -> BodyText`
  - `ReadUInt32`
  - `ReadUInt32 -> MinSignatures`
  - `ReadUInt32 -> MaxSignatures`
  - `ReadInt32 -> DeadLine`
  - `ReadInt32 -> IssueDate`
  - `ReadInt32 -> AllowedGuildID`
  - `ReadInt32 -> AllowedClasses`
  - `ReadInt32 -> AllowedRaces`
  - `ReadInt16 -> AllowedGender`
  - `ReadInt32 -> AllowedMinLevel`
  - `ReadInt32 -> AllowedMaxLevel`
  - `ReadInt32 -> NumChoices`
  - `ReadCString`
  - `ReadUInt32 -> Muid`
  - `ReadInt32 -> StaticType`

```csharp
{
QueryPetitionResponse petition = new QueryPetitionResponse();
petition.PetitionID = packet.ReadUInt32();
petition.Allow = true;
petition.Info = new PetitionInfo();
petition.Info.PetitionID = petition.PetitionID;
petition.Info.Petitioner = packet.ReadGuid().To128(this.GetSession().GameState);
petition.Info.Title = packet.ReadCString();
petition.Info.BodyText = packet.ReadCString();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
packet.ReadUInt32();
}
petition.Info.MinSignatures = packet.ReadUInt32();
petition.Info.MaxSignatures = packet.ReadUInt32();
petition.Info.DeadLine = packet.ReadInt32();
petition.Info.IssueDate = packet.ReadInt32();
petition.Info.AllowedGuildID = packet.ReadInt32();
petition.Info.AllowedClasses = packet.ReadInt32();
petition.Info.AllowedRaces = packet.ReadInt32();
petition.Info.AllowedGender = packet.ReadInt16();
petition.Info.AllowedMinLevel = packet.ReadInt32();
petition.Info.AllowedMaxLevel = packet.ReadInt32();
petition.Info.NumChoices = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
for (int i = 0; i < 10; i++)
{
petition.Info.Choicetext[i] = packet.ReadCString();
}
```

---

### SMSG_QUERY_PET_NAME_RESPONSE

- Legacy value: 83 (0x0053)
- Modern value: 10521 (0x2919)
- Handler: HermesProxy/World/Client/WorldClient.cs:6903
- Fields:
  - `ReadUInt32 -> petNumber`
  - `ReadCString -> Name`
  - `ReadBytes`
  - `ReadUInt32 -> Timestamp`
  - `ReadBool`
  - `ReadCString`

```csharp
{
uint petNumber = packet.ReadUInt32();
WowGuid128 guid = this.GetSession().GameState.GetPetGuidByNumber(petNumber);
if (guid == null)
{
Log.Print(LogType.Error, $"Pet name query response for unknown pet {petNumber}!", "HandleQueryPetNameResponse", "World\\Client\\PacketHandlers\\QueryHandler.cs");
return;
}
QueryPetNameResponse response = new QueryPetNameResponse();
response.UnitGUID = guid;
response.Name = packet.ReadCString();
if (response.Name.Length == 0)
{
response.Allow = false;
packet.ReadBytes(7u);
return;
}
response.Allow = true;
response.Timestamp = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool())
{
for (int i = 0; i < 5; i++)
{
response.DeclinedNames.name[i] = packet.ReadCString();
}
}
this.SendPacketToClient(response);
}
```

---

### SMSG_QUERY_PLAYER_NAME_RESPONSE

- Legacy value: 81 (0x0051)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:976
- Fields:
  - `ReadPackedGuid -> playerGuid`
  - `ReadBool`
  - `ReadGuid -> playerGuid`
  - `ReadCString -> Name`
  - `ReadCString`
  - `ReadUInt8 -> RaceID`
  - `ReadUInt8 -> Sex`
  - `ReadUInt8 -> ClassID`
  - `ReadUInt32 -> RaceID`
  - `ReadUInt32 -> Sex`
  - `ReadInt32 -> ClassID`
  - `ReadBool`
  - `ReadCString`

```csharp
{
WowGuid128 playerGuid;
byte result = 0;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
playerGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (packet.ReadBool())
result = 1;
}
else
{
playerGuid = packet.ReadGuid().To128(this.GetSession().GameState);
}

PlayerGuidLookupData data = new PlayerGuidLookupData();
data.GuidActual = playerGuid;

if (result != 0)
{
// Player not found - send error response
if (ModernVersion.GetCurrentOpcode(Opcode.SMSG_QUERY_PLAYER_NAME_RESPONSE) != 0)
{
QueryPlayerNameResponse response = new QueryPlayerNameResponse();
response.Player = playerGuid;
response.Result = 1;
this.SendPacketToClient(response);
}
else
{
QueryPlayerNamesResponse response = new QueryPlayerNamesResponse();
```

---

### SMSG_QUERY_QUEST_INFO_RESPONSE

- Legacy value: 93 (0x005D)
- Modern value: 10902 (0x2A96)
- Handler: HermesProxy/World/Client/WorldClient.cs:6386
- Fields:
  - `ReadInt32 -> QuestType`
  - `ReadInt32 -> QuestLevel`
  - `ReadInt32 -> MinLevel`
  - `ReadInt32 -> QuestSortID`
  - `ReadUInt32 -> QuestInfoID`
  - `ReadUInt32 -> SuggestedGroupNum`
  - `ReadInt32 -> factionId`
  - `ReadInt32 -> factionValue`
  - `ReadUInt32 -> RewardNextQuest`
  - `ReadUInt32 -> RewardXPDifficulty`
  - `ReadInt32 -> rewOrReqMoney`
  - `ReadUInt32 -> RewardBonusMoney`
  - `ReadUInt32`
  - `ReadUInt32 -> RewardSpell`
  - `ReadUInt32 -> RewardHonor`
  - `ReadFloat -> RewardKillHonor`
  - `ReadUInt32 -> StartItem`
  - `ReadUInt32 -> Flags`
  - `ReadUInt32 -> RewardTitle`
  - `ReadInt32 -> requiredPlayerKills`
  - `ReadUInt32`
  - `ReadInt32 -> RewardArenaPoints`
  - `ReadInt32`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32 -> Quantity`
  - `ReadUInt32`
  - `ReadInt32`
  - `ReadUInt32`
  - `ReadUInt32 -> POIContinent`
  - `ReadFloat -> POIx`
  - `ReadFloat -> POIy`
  - `ReadUInt32 -> POIPriority`
  - `ReadCString -> LogTitle`
  - `ReadCString -> LogDescription`
  - `ReadCString -> QuestDescription`
  - `ReadCString -> AreaDescription`
  - `ReadCString -> QuestCompletionLog`
  - `ReadInt32 -> creatureOrGoAmount`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadCString -> objectiveText`

```csharp
{
QueryQuestInfoResponse response = new QueryQuestInfoResponse();
KeyValuePair<int, bool> id = packet.ReadEntry();
response.QuestID = (uint)id.Key;
if (id.Value)
{
response.Allow = false;
this.SendPacketToClient(response);
return;
}
response.Allow = true;
response.Info = new QuestTemplate();
QuestTemplate quest = response.Info;
quest.QuestID = response.QuestID;
quest.QuestType = packet.ReadInt32();
quest.QuestLevel = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
{
quest.MinLevel = packet.ReadInt32();
}
else
{
quest.MinLevel = 1;
}
quest.QuestSortID = packet.ReadInt32();
quest.QuestInfoID = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
quest.SuggestedGroupNum = packet.ReadUInt32();
}
```

---

### SMSG_QUERY_TIME_RESPONSE

- Legacy value: 463 (0x01CF)
- Modern value: 9956 (0x26E4)
- Handler: HermesProxy/World/Client/WorldClient.cs:6374
- Fields:
  - `ReadInt32 -> CurrentTime`
  - `ReadInt32`

```csharp
{
QueryTimeResponse response = new QueryTimeResponse();
response.CurrentTime = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.CanRead())
{
packet.ReadInt32();
}
this.SendPacketToClient(response);
}
```

---

### SMSG_QUEST_CONFIRM_ACCEPT

- Legacy value: 412 (0x019C)
- Modern value: 10895 (0x2A8F)
- Handler: HermesProxy/World/Client/WorldClient.cs:7417
- Fields:
  - `ReadUInt32 -> QuestID`
  - `ReadCString -> QuestTitle`
  - `ReadGuid -> InitiatedBy`

```csharp
{
QuestConfirmAccept quest = new QuestConfirmAccept();
quest.QuestID = packet.ReadUInt32();
quest.QuestTitle = packet.ReadCString();
quest.InitiatedBy = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(quest);
}
```

---

### SMSG_QUEST_GIVER_INVALID_QUEST

- Legacy value: 399 (0x018F)
- Modern value: 10885 (0x2A85)
- Handler: HermesProxy/World/Client/WorldClient.cs:7361
- Fields:
  - `ReadUInt32 -> Reason`

```csharp
{
QuestGiverInvalidQuest quest = new QuestGiverInvalidQuest();
quest.Reason = (QuestFailedReasons)packet.ReadUInt32();
this.SendPacketToClient(quest);
}
```

---

### SMSG_QUEST_GIVER_OFFER_REWARD_MESSAGE

- Legacy value: 397 (0x018D)
- Modern value: 10900 (0x2A94)
- Handler: HermesProxy/World/Client/WorldClient.cs:7244
- Fields:
  - `ReadGuid -> QuestGiverGUID`
  - `ReadUInt32 -> QuestID`
  - `ReadCString -> QuestTitle`
  - `ReadCString -> RewardText`
  - `ReadBool -> AutoLaunched`
  - `ReadUInt32 -> AutoLaunched`
  - `ReadUInt32`
  - `ReadUInt32 -> SuggestedPartyMembers`
  - `ReadUInt32 -> emotesCount`
  - `ReadUInt32 -> Delay`
  - `ReadUInt32 -> Type`

```csharp
{
QuestGiverOfferRewardMessage quest = new QuestGiverOfferRewardMessage();
quest.QuestData.QuestGiverGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = quest.QuestData.QuestGiverGUID;
quest.QuestData.QuestGiverCreatureID = quest.QuestData.QuestGiverGUID.GetEntry();
quest.QuestGiverCreatureID = (int)quest.QuestData.QuestGiverGUID.GetEntry();
quest.QuestData.QuestID = packet.ReadUInt32();
quest.QuestTitle = packet.ReadCString();
quest.RewardText = packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
{
quest.QuestData.AutoLaunched = packet.ReadBool();
}
else
{
quest.QuestData.AutoLaunched = packet.ReadUInt32() != 0;
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
{
quest.QuestData.QuestFlags[0] = packet.ReadUInt32();
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
quest.QuestData.SuggestedPartyMembers = packet.ReadUInt32();
}
uint emotesCount = packet.ReadUInt32();
for (int i = 0; i < emotesCount; i++)
{
QuestDescEmote emote = new QuestDescEmote
{
```

---

### SMSG_QUEST_GIVER_QUEST_COMPLETE

- Legacy value: 401 (0x0191)
- Modern value: 10883 (0x2A83)
- Handler: HermesProxy/World/Client/WorldClient.cs:7284
- Fields:
  - `ReadUInt32 -> QuestID`
  - `ReadUInt32`
  - `ReadUInt32 -> XPReward`
  - `ReadInt32 -> MoneyReward`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadUInt32 -> itemsCount`
  - `ReadUInt32 -> itemId2`
  - `ReadUInt32 -> itemCount2`

```csharp
{
QuestGiverQuestComplete quest = new QuestGiverQuestComplete();
quest.QuestID = packet.ReadUInt32();
this.GetSession().GameState.CurrentPlayerStorage.CompletedQuests.MarkQuestAsCompleted(quest.QuestID);
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.ReadUInt32();
}
quest.XPReward = packet.ReadUInt32();
quest.MoneyReward = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_3_0_7561))
{
packet.ReadInt32();
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.ReadInt32();
packet.ReadInt32();
}
uint itemId = 0u;
uint itemCount = 0u;
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
{
uint itemsCount = packet.ReadUInt32();
for (uint i = 0u; i < itemsCount; i++)
{
uint itemId2 = packet.ReadUInt32();
uint itemCount2 = packet.ReadUInt32();
if (itemId2 != 0 && itemCount2 != 0)
{
```

---

### SMSG_QUEST_GIVER_QUEST_DETAILS

- Legacy value: 392 (0x0188)
- Modern value: 10898 (0x2A92)
- Handler: HermesProxy/World/Client/WorldClient.cs:6992
- Fields:
  - `ReadGuid -> QuestGiverGUID`
  - `ReadGuid -> InformUnit`
  - `ReadUInt32 -> QuestID`
  - `ReadCString -> QuestTitle`
  - `ReadCString -> DescriptionText`
  - `ReadCString -> LogDescription`
  - `ReadBool -> AutoLaunched`
  - `ReadUInt32 -> AutoLaunched`
  - `ReadUInt32`
  - `ReadUInt32 -> SuggestedPartyMembers`
  - `ReadUInt8`
  - `ReadBool -> StartCheat`
  - `ReadBool -> DisplayPopup`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32 -> Money`
  - `ReadUInt32 -> XP`
  - `ReadUInt32 -> emoteCount`
  - `ReadUInt32 -> Type`
  - `ReadUInt32 -> Delay`

```csharp
{
QuestGiverQuestDetails quest = new QuestGiverQuestDetails();
quest.QuestGiverGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = quest.QuestGiverGUID;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
quest.InformUnit = packet.ReadGuid().To128(this.GetSession().GameState);
}
else
{
quest.InformUnit = quest.QuestGiverGUID;
}
quest.QuestID = packet.ReadUInt32();
quest.QuestTitle = packet.ReadCString();
quest.DescriptionText = packet.ReadCString();
quest.LogDescription = packet.ReadCString();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
{
quest.AutoLaunched = packet.ReadBool();
}
else
{
quest.AutoLaunched = packet.ReadUInt32() != 0;
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
{
quest.QuestFlags[0] = packet.ReadUInt32();
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
```

---

### SMSG_QUEST_GIVER_QUEST_FAILED

- Legacy value: 402 (0x0192)
- Modern value: 10886 (0x2A86)
- Handler: HermesProxy/World/Client/WorldClient.cs:7352
- Fields:
  - `ReadUInt32 -> QuestID`
  - `ReadUInt32 -> Reason`

```csharp
{
QuestGiverQuestFailed quest = new QuestGiverQuestFailed();
quest.QuestID = packet.ReadUInt32();
quest.Reason = LegacyVersion.ConvertInventoryResult(packet.ReadUInt32());
this.SendPacketToClient(quest);
}
```

---

### SMSG_QUEST_GIVER_QUEST_LIST_MESSAGE

- Legacy value: 389 (0x0185)
- Modern value: 10906 (0x2A9A)
- Handler: HermesProxy/World/Client/WorldClient.cs:7150
- Fields:
  - `ReadGuid -> QuestGiverGUID`
  - `ReadCString -> Greeting`
  - `ReadUInt32 -> GreetEmoteDelay`
  - `ReadUInt32 -> GreetEmoteType`
  - `ReadUInt8 -> count`

```csharp
{
QuestGiverQuestListMessage quests = new QuestGiverQuestListMessage();
quests.QuestGiverGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = quests.QuestGiverGUID;
quests.Greeting = packet.ReadCString();
quests.GreetEmoteDelay = packet.ReadUInt32();
quests.GreetEmoteType = packet.ReadUInt32();
byte count = packet.ReadUInt8();
for (int i = 0; i < count; i++)
{
ClientGossipQuest quest = this.ReadGossipQuestOption(packet);
quests.QuestOptions.Add(quest);
}
this.SendPacketToClient(quests);
}
```

---

### SMSG_QUEST_GIVER_REQUEST_ITEMS

- Legacy value: 395 (0x018B)
- Modern value: 10899 (0x2A93)
- Handler: HermesProxy/World/Client/WorldClient.cs:7189
- Fields:
  - `ReadGuid -> QuestGiverGUID`
  - `ReadUInt32 -> QuestID`
  - `ReadCString -> QuestTitle`
  - `ReadCString -> CompletionText`
  - `ReadUInt32 -> CompEmoteDelay`
  - `ReadUInt32 -> CompEmoteType`
  - `ReadUInt32 -> AutoLaunched`
  - `ReadUInt32`
  - `ReadUInt32 -> SuggestPartyMembers`
  - `ReadInt32 -> MoneyToGet`
  - `ReadUInt32 -> itemsCount`
  - `ReadUInt32 -> ObjectID`
  - `ReadUInt32 -> Amount`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32 -> statusFlags`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32`

```csharp
{
QuestGiverRequestItems quest = new QuestGiverRequestItems();
quest.QuestGiverGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = quest.QuestGiverGUID;
quest.QuestGiverCreatureID = quest.QuestGiverGUID.GetEntry();
quest.QuestID = packet.ReadUInt32();
quest.QuestTitle = packet.ReadCString();
quest.CompletionText = packet.ReadCString();
quest.CompEmoteDelay = packet.ReadUInt32();
quest.CompEmoteType = packet.ReadUInt32();
quest.AutoLaunched = packet.ReadUInt32() != 0;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_3_11685))
{
quest.QuestFlags[0] = packet.ReadUInt32();
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
quest.SuggestPartyMembers = packet.ReadUInt32();
}
quest.MoneyToGet = packet.ReadInt32();
uint itemsCount = packet.ReadUInt32();
for (int i = 0; i < itemsCount; i++)
{
QuestObjectiveCollect item = new QuestObjectiveCollect
{
ObjectID = packet.ReadUInt32(),
Amount = packet.ReadUInt32()
};
packet.ReadUInt32();
quest.Collect.Add(item);
```

---

### SMSG_QUEST_GIVER_STATUS

- Legacy value: 387 (0x0183)
- Modern value: 10907 (0x2A9B)
- Handler: HermesProxy/World/Client/WorldClient.cs:7126
- Fields:
  - `ReadGuid -> Guid`
  - `ReadUInt8 -> Status`

```csharp
{
QuestGiverStatusPkt response = new QuestGiverStatusPkt();
response.QuestGiver.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
response.QuestGiver.Status = LegacyVersion.ConvertQuestGiverStatus(packet.ReadUInt8());
this.SendPacketToClient(response);
}
```

---

### SMSG_QUEST_GIVER_STATUS_MULTIPLE

- Legacy value: 1048 (0x0418)
- Modern value: 10897 (0x2A91)
- Handler: HermesProxy/World/Client/WorldClient.cs:7135
- Fields:
  - `ReadInt32 -> count`
  - `ReadGuid -> Guid`
  - `ReadUInt8 -> Status`

```csharp
{
QuestGiverStatusMultiple response = new QuestGiverStatusMultiple();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
QuestGiverInfo info = new QuestGiverInfo();
info.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
info.Status = LegacyVersion.ConvertQuestGiverStatus(packet.ReadUInt8());
response.QuestGivers.Add(info);
}
this.SendPacketToClient(response);
}
```

---

### SMSG_QUEST_UPDATE_ADD_ITEM

- Legacy value: 410 (0x019A)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:7379
- Fields:
  - `ReadUInt32 -> itemId`
  - `ReadUInt32 -> count`
  - `WriteUInt32((uint)`

```csharp
{
uint itemId = packet.ReadUInt32();
uint count = packet.ReadUInt32();
QuestObjective objective = GameData.GetQuestObjectiveForItem(itemId);
if (objective != null)
{
return;
}
Dictionary<int, UpdateField> updateFields = this.GetSession().GameState.GetCachedObjectFieldsLegacy(this.GetSession().GameState.CurrentPlayerGuid);
int questsCount = LegacyVersion.GetQuestLogSize();
for (int i = 0; i < questsCount; i++)
{
QuestLog logEntry = this.ReadQuestLogEntry(i, null, updateFields);
if (logEntry != null && logEntry.QuestID.HasValue && GameData.GetQuestTemplate((uint)logEntry.QuestID.Value) == null)
{
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_QUERY_QUEST_INFO);
packet2.WriteUInt32((uint)logEntry.QuestID.Value);
this.SendPacketToServer(packet2);
}
}
}
```

---

### SMSG_QUEST_UPDATE_ADD_KILL

- Legacy value: 409 (0x0199)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:7403
- Fields:
  - `ReadUInt32 -> QuestID`
  - `ReadUInt32 -> Count`
  - `ReadUInt32 -> Required`
  - `ReadGuid -> VictimGUID`

```csharp
{
QuestUpdateAddCredit credit = new QuestUpdateAddCredit();
credit.QuestID = packet.ReadUInt32();
KeyValuePair<int, bool> entry = packet.ReadEntry();
credit.ObjectID = entry.Key;
credit.ObjectiveType = (entry.Value ? QuestObjectiveType.GameObject : QuestObjectiveType.Monster);
credit.Count = (ushort)packet.ReadUInt32();
credit.Required = (ushort)packet.ReadUInt32();
credit.VictimGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(credit);
}
```

---

### SMSG_QUEST_UPDATE_COMPLETE

- Legacy value: 408 (0x0198)
- Modern value: 10889 (0x2A89)
- Handler: HermesProxy/World/Client/WorldClient.cs:7369
- Fields:
  - `ReadUInt32 -> QuestID`

```csharp
{
QuestUpdateStatus quest = new QuestUpdateStatus(packet.GetUniversalOpcode(isModern: false));
quest.QuestID = packet.ReadUInt32();
this.SendPacketToClient(quest);
}
```

---

### SMSG_QUEST_UPDATE_FAILED

- Legacy value: 406 (0x0196)
- Modern value: 10890 (0x2A8A)
- Handler: HermesProxy/World/Client/WorldClient.cs:7370
- Fields:
  - `ReadUInt32 -> QuestID`

```csharp
{
QuestUpdateStatus quest = new QuestUpdateStatus(packet.GetUniversalOpcode(isModern: false));
quest.QuestID = packet.ReadUInt32();
this.SendPacketToClient(quest);
}
```

---

### SMSG_QUEST_UPDATE_FAILED_TIMER

- Legacy value: 407 (0x0197)
- Modern value: 10891 (0x2A8B)
- Handler: HermesProxy/World/Client/WorldClient.cs:7371
- Fields:
  - `ReadUInt32 -> QuestID`

```csharp
{
QuestUpdateStatus quest = new QuestUpdateStatus(packet.GetUniversalOpcode(isModern: false));
quest.QuestID = packet.ReadUInt32();
this.SendPacketToClient(quest);
}
```

---

### SMSG_RAID_GROUP_ONLY

- Legacy value: 646 (0x0286)
- Modern value: 10159 (0x27AF)
- Handler: HermesProxy/World/Client/WorldClient.cs:4026
- Fields:
  - `ReadInt32 -> Delay`
  - `ReadUInt32 -> Reason`

```csharp
{
RaidGroupOnly save = new RaidGroupOnly();
save.Delay = packet.ReadInt32();
save.Reason = (RaidGroupReason)packet.ReadUInt32();
this.SendPacketToClient(save);
}
```

---

### SMSG_RAID_INSTANCE_INFO

- Legacy value: 716 (0x02CC)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3976
- Fields:
  - `ReadInt32 -> count`
  - `ReadUInt32 -> MapID`
  - `ReadUInt32 -> DifficultyID`
  - `ReadUInt64 -> InstanceID`
  - `ReadBool -> Locked`
  - `ReadBool -> Extended`
  - `ReadInt32 -> TimeRemaining`
  - `ReadInt32 -> TimeRemaining`
  - `ReadUInt32 -> InstanceID`
  - `ReadUInt32`

```csharp
{
RaidInstanceInfo infos = new RaidInstanceInfo();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
InstanceLock instance = new InstanceLock();
instance.MapID = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
instance.DifficultyID = (DifficultyModern)packet.ReadUInt32();
}
else if (ModernVersion.ExpansionVersion == 1)
{
instance.DifficultyID = DifficultyModern.Raid40;
}
else
{
instance.DifficultyID = DifficultyModern.Raid25N;
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
instance.InstanceID = packet.ReadUInt64();
instance.Locked = packet.ReadBool();
instance.Extended = packet.ReadBool();
instance.TimeRemaining = packet.ReadInt32();
}
else
{
instance.TimeRemaining = packet.ReadInt32();
instance.InstanceID = packet.ReadUInt32();
```

---

### SMSG_RAID_INSTANCE_MESSAGE

- Legacy value: 762 (0x02FA)
- Modern value: 11188 (0x2BB4)
- Handler: HermesProxy/World/Client/WorldClient.cs:4035
- Fields:
  - `ReadUInt32 -> Type`
  - `ReadUInt32 -> MapID`
  - `ReadUInt32 -> DifficultyID`
  - `ReadUInt32`
  - `ReadBool -> Locked`
  - `ReadBool -> Extended`

```csharp
{
RaidInstanceMessage instance = new RaidInstanceMessage();
instance.Type = (InstanceResetWarningType)packet.ReadUInt32();
instance.MapID = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
instance.DifficultyID = (DifficultyModern)packet.ReadUInt32();
}
else if (ModernVersion.ExpansionVersion == 1)
{
instance.DifficultyID = DifficultyModern.Raid40;
}
else
{
instance.DifficultyID = DifficultyModern.Raid25N;
}
packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056) && instance.Type == InstanceResetWarningType.Welcome)
{
instance.Locked = packet.ReadBool();
instance.Extended = packet.ReadBool();
}
this.SendPacketToClient(instance);
}
```

---

### SMSG_READ_ITEM_RESULT_FAILED

- Legacy value: 175 (0x00AF)
- Modern value: 10153 (0x27A9)
- Handler: HermesProxy/World/Client/WorldClient.cs:4152
- Fields:
  - `ReadGuid -> ItemGUID`

```csharp
{
ReadItemResultFailed read = new ReadItemResultFailed();
read.ItemGUID = packet.ReadGuid().To128(this.GetSession().GameState);
read.Subcode = 2;
this.SendPacketToClient(read);
}
```

---

### SMSG_READ_ITEM_RESULT_OK

- Legacy value: 174 (0x00AE)
- Modern value: 10145 (0x27A1)
- Handler: HermesProxy/World/Client/WorldClient.cs:4144
- Fields:
  - `ReadGuid -> ItemGUID`

```csharp
{
ReadItemResultOK read = new ReadItemResultOK();
read.ItemGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(read);
}
```

---

### SMSG_RESET_FAILED_NOTIFY

- Legacy value: 918 (0x0396)
- Modern value: 9911 (0x26B7)
- Handler: HermesProxy/World/Client/WorldClient.cs:3968
- Fields:
  - `ReadUInt32`

```csharp
{
ResetFailedNotify reset = new ResetFailedNotify();
packet.ReadUInt32();
this.SendPacketToClient(reset);
}
```

---

### SMSG_RESURRECT_REQUEST

- Legacy value: 347 (0x015B)
- Modern value: 9598 (0x257E)
- Handler: HermesProxy/World/Client/WorldClient.cs:8795
- Fields:
  - `ReadGuid -> CasterGUID`
  - `ReadUInt32`
  - `ReadCString -> Name`
  - `ReadBool -> Sickness`
  - `ReadBool -> UseTimer`

```csharp
{
ResurrectRequest revive = new ResurrectRequest();
revive.CasterGUID = packet.ReadGuid().To128(this.GetSession().GameState);
revive.CasterVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
packet.ReadUInt32();
revive.Name = packet.ReadCString();
revive.Sickness = packet.ReadBool();
revive.UseTimer = packet.ReadBool();
this.SendPacketToClient(revive);
}
```

---

### SMSG_SELL_RESPONSE

- Legacy value: 417 (0x01A1)
- Modern value: 9925 (0x26C5)
- Handler: HermesProxy/World/Client/WorldClient.cs:4250
- Fields:
  - `ReadGuid -> VendorGUID`
  - `ReadGuid -> ItemGUID`
  - `ReadUInt8 -> Reason`

```csharp
{
SellResponse sell = new SellResponse();
sell.VendorGUID = packet.ReadGuid().To128(this.GetSession().GameState);
sell.ItemGUID = packet.ReadGuid().To128(this.GetSession().GameState);
sell.Reason = packet.ReadUInt8();
Log.Print(LogType.Debug, $"[SellResponse] Item={sell.ItemGUID} Vendor={sell.VendorGUID} Reason={sell.Reason}", "HandleSellResponse", "");
this.SendPacketToClient(sell);
}
```

---

### SMSG_SEND_KNOWN_SPELLS

- Legacy value: 298 (0x012A)
- Modern value: 11303 (0x2C27)
- Handler: HermesProxy/World/Client/WorldClient.cs:7630
- Fields:
  - `ReadBool -> InitialLogin`
  - `ReadUInt16 -> spellCount`
  - `ReadUInt16 -> spellId`
  - `ReadUInt32 -> spellId`
  - `ReadInt16`
  - `ReadUInt16 -> cooldownCount`
  - `ReadUInt16 -> spellId2`
  - `ReadUInt32 -> spellId2`
  - `ReadUInt16 -> itemId`
  - `ReadUInt32 -> itemId`
  - `ReadUInt16 -> Category`
  - `ReadInt32 -> RecoveryTime`
  - `ReadInt32 -> CategoryRecoveryTime`

```csharp
{
SendKnownSpells spells = new SendKnownSpells();
spells.InitialLogin = packet.ReadBool();
ushort spellCount = packet.ReadUInt16();
for (ushort i = 0; i < spellCount; i++)
{
if (!packet.CanRead()) break;
uint spellId = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt16() : packet.ReadUInt32());
spells.KnownSpells.Add(spellId);
if (!packet.CanRead()) break;
packet.ReadInt16();
}
this.SendPacketToClient(spells);
if (!packet.CanRead())
return;
ushort cooldownCount = packet.ReadUInt16();
if (cooldownCount != 0)
{
SendSpellHistory histories = new SendSpellHistory();
for (ushort i2 = 0; i2 < cooldownCount; i2++)
{
SpellHistoryEntry history = new SpellHistoryEntry();
uint spellId2 = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt16() : packet.ReadUInt32());
history.SpellID = spellId2;
uint itemId = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V4_2_2_14545)) ? packet.ReadUInt16() : packet.ReadUInt32());
history.ItemID = itemId;
history.Category = packet.ReadUInt16();
history.RecoveryTime = packet.ReadInt32();
history.CategoryRecoveryTime = packet.ReadInt32();
histories.Entries.Add(history);
```

---

### SMSG_SEND_UNLEARN_SPELLS

- Legacy value: 1054 (0x041E)
- Modern value: 11307 (0x2C2B)
- Handler: HermesProxy/World/Client/WorldClient.cs:7702
- Fields:
  - `ReadUInt32 -> spellCount`
  - `ReadUInt32 -> spellId`

```csharp
{
SendUnlearnSpells spells = new SendUnlearnSpells();
uint spellCount = packet.ReadUInt32();
for (uint i = 0u; i < spellCount; i++)
{
uint spellId = packet.ReadUInt32();
spells.Spells.Add(spellId);
}
this.SendPacketToClient(spells);
}
```

---

### SMSG_SET_EXTRA_AURA_INFO

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:8643
- Fields:
  - `ReadPackedGuid -> guid`
  - `ReadUInt8 -> slot`
  - `ReadUInt32 -> spellId`
  - `ReadInt32 -> durationFull`
  - `ReadInt32 -> durationLeft`

```csharp
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (!packet.CanRead())
{
return;
}
byte slot = packet.ReadUInt8();
uint spellId = packet.ReadUInt32();
int durationFull = packet.ReadInt32();
int durationLeft = packet.ReadInt32();
this.GetSession().GameState.StoreAuraDurationFull(guid, slot, durationFull);
this.GetSession().GameState.StoreAuraDurationLeft(guid, slot, durationLeft, (int)packet.GetReceivedTime());
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_SET_EXTRA_AURA_INFO_NEED_UPDATE)
{
this.GetSession().GameState.StoreAuraCaster(guid, slot, this.GetSession().GameState.CurrentPlayerGuid);
}
if (durationFull <= 0 && durationLeft <= 0)
{
return;
}
Dictionary<int, UpdateField> updateFields = this.GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
if (updateFields != null)
{
AuraInfo aura = new AuraInfo
{
Slot = slot,
AuraData = this.ReadAuraSlot(slot, guid, updateFields)
};
if (aura.AuraData != null && aura.AuraData.SpellID == spellId)
{
```

---

### SMSG_SET_EXTRA_AURA_INFO_NEED_UPDATE

- Legacy value: N/A
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:8644
- Fields:
  - `ReadPackedGuid -> guid`
  - `ReadUInt8 -> slot`
  - `ReadUInt32 -> spellId`
  - `ReadInt32 -> durationFull`
  - `ReadInt32 -> durationLeft`

```csharp
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (!packet.CanRead())
{
return;
}
byte slot = packet.ReadUInt8();
uint spellId = packet.ReadUInt32();
int durationFull = packet.ReadInt32();
int durationLeft = packet.ReadInt32();
this.GetSession().GameState.StoreAuraDurationFull(guid, slot, durationFull);
this.GetSession().GameState.StoreAuraDurationLeft(guid, slot, durationLeft, (int)packet.GetReceivedTime());
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_SET_EXTRA_AURA_INFO_NEED_UPDATE)
{
this.GetSession().GameState.StoreAuraCaster(guid, slot, this.GetSession().GameState.CurrentPlayerGuid);
}
if (durationFull <= 0 && durationLeft <= 0)
{
return;
}
Dictionary<int, UpdateField> updateFields = this.GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
if (updateFields != null)
{
AuraInfo aura = new AuraInfo
{
Slot = slot,
AuraData = this.ReadAuraSlot(slot, guid, updateFields)
};
if (aura.AuraData != null && aura.AuraData.SpellID == spellId)
{
```

---

### SMSG_SET_FACTION_STANDING

- Legacy value: 292 (0x0124)
- Modern value: 10028 (0x272C)
- Handler: HermesProxy/World/Client/WorldClient.cs:7456
- Fields:
  - `ReadFloat`
  - `ReadBool -> showVisual`
  - `ReadInt32 -> count`
  - `ReadInt32 -> Index`
  - `ReadInt32 -> Standing`

```csharp
{
SetFactionStanding standing = new SetFactionStanding();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_4_0_8089))
{
packet.ReadFloat();
}
bool showVisual = true;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
showVisual = packet.ReadBool();
}
standing.ShowVisual = showVisual;
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
FactionStandingData faction = new FactionStandingData
{
Index = packet.ReadInt32(),
Standing = packet.ReadInt32()
};
standing.Factions.Add(faction);
}
this.SendPacketToClient(standing);
}
```

---

### SMSG_SET_FACTION_VISIBLE

- Legacy value: 291 (0x0123)
- Modern value: 10026 (0x272A)
- Handler: HermesProxy/World/Client/WorldClient.cs:7500
- Fields:
  - `ReadUInt32 -> FactionIndex`

```csharp
{
SetFactionVisible faction = new SetFactionVisible(visible: true);
faction.FactionIndex = packet.ReadUInt32();
this.SendPacketToClient(faction);
}
```

---

### SMSG_SET_FLAT_SPELL_MODIFIER

- Legacy value: 614 (0x0266)
- Modern value: 11315 (0x2C33)
- Handler: HermesProxy/World/Client/WorldClient.cs:8819
- Fields:
  - `ReadUInt8 -> classIndex`
  - `ReadUInt8 -> modIndex`
  - `ReadInt32 -> modValue`

```csharp
{
byte classIndex = packet.ReadUInt8();
byte modIndex = packet.ReadUInt8();
int modValue = packet.ReadInt32();
if (this.GetSession().GameState.CurrentPlayerCreateTime != 0)
{
SetSpellModifier spell = new SetSpellModifier(packet.GetUniversalOpcode(isModern: false));
SpellModifierInfo mod = new SpellModifierInfo();
SpellModifierData data = new SpellModifierData
{
ClassIndex = classIndex
};
mod.ModIndex = modIndex;
data.ModifierValue = modValue;
mod.ModifierData.Add(data);
spell.Modifiers.Add(mod);
this.SendPacketToClient(spell);
}
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_SET_FLAT_SPELL_MODIFIER)
{
this.GetSession().GameState.SetFlatSpellMod(modIndex, classIndex, modValue);
}
else
{
this.GetSession().GameState.SetPctSpellMod(modIndex, classIndex, modValue);
}
}
```

---

### SMSG_SET_FORCED_REACTIONS

- Legacy value: 677 (0x02A5)
- Modern value: 10013 (0x271D)
- Handler: HermesProxy/World/Client/WorldClient.cs:7483
- Fields:
  - `ReadInt32 -> count`
  - `ReadInt32 -> Faction`
  - `ReadInt32 -> Reaction`

```csharp
{
SetForcedReactions reactions = new SetForcedReactions();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
ForcedReaction reaction = new ForcedReaction
{
Faction = packet.ReadInt32(),
Reaction = packet.ReadInt32()
};
reactions.Reactions.Add(reaction);
}
this.SendPacketToClient(reactions);
}
```

---

### SMSG_SET_PCT_SPELL_MODIFIER

- Legacy value: 615 (0x0267)
- Modern value: 11316 (0x2C34)
- Handler: HermesProxy/World/Client/WorldClient.cs:8820
- Fields:
  - `ReadUInt8 -> classIndex`
  - `ReadUInt8 -> modIndex`
  - `ReadInt32 -> modValue`

```csharp
{
byte classIndex = packet.ReadUInt8();
byte modIndex = packet.ReadUInt8();
int modValue = packet.ReadInt32();
if (this.GetSession().GameState.CurrentPlayerCreateTime != 0)
{
SetSpellModifier spell = new SetSpellModifier(packet.GetUniversalOpcode(isModern: false));
SpellModifierInfo mod = new SpellModifierInfo();
SpellModifierData data = new SpellModifierData
{
ClassIndex = classIndex
};
mod.ModIndex = modIndex;
data.ModifierValue = modValue;
mod.ModifierData.Add(data);
spell.Modifiers.Add(mod);
this.SendPacketToClient(spell);
}
if (packet.GetUniversalOpcode(isModern: false) == Opcode.SMSG_SET_FLAT_SPELL_MODIFIER)
{
this.GetSession().GameState.SetFlatSpellMod(modIndex, classIndex, modValue);
}
else
{
this.GetSession().GameState.SetPctSpellMod(modIndex, classIndex, modValue);
}
}
```

---

### SMSG_SET_PROFICIENCY

- Legacy value: 295 (0x0127)
- Modern value: 10037 (0x2735)
- Handler: HermesProxy/World/Client/WorldClient.cs:4062
- Fields:
  - `ReadUInt8 -> ProficiencyClass`
  - `ReadUInt32 -> ProficiencyMask`

```csharp
{
SetProficiency proficiency = new SetProficiency();
proficiency.ProficiencyClass = packet.ReadUInt8();
proficiency.ProficiencyMask = packet.ReadUInt32();
this.SendPacketToClient(proficiency);
}
```

---

### SMSG_SHOW_BANK

- Legacy value: 440 (0x01B8)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:6023
- Fields:
  - `ReadGuid -> Guid`

```csharp
{
ShowBank bank = new ShowBank();
bank.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = bank.Guid;
this.SendPacketToClient(bank);
}
```

---

### SMSG_SHOW_TAXI_NODES

- Legacy value: 425 (0x01A9)
- Modern value: 9933 (0x26CD)
- Handler: HermesProxy/World/Client/WorldClient.cs:8892
- Fields:
  - `ReadUInt32`
  - `ReadGuid -> UnitGUID`
  - `ReadUInt32 -> CurrentNode`
  - `ReadUInt8 -> nodesMask`

```csharp
{
uint playerFlags = this.GetSession().GameState.GetLegacyFieldValueUInt32(this.GetSession().GameState.CurrentPlayerGuid, PlayerField.PLAYER_FLAGS);
if (playerFlags.HasAnyFlag(PlayerFlags.GM))
{
ChatPkt chat = new ChatPkt(this.GetSession(), ChatMessageTypeModern.System, "Disable GM mode before talking to taxi master or your game will freeze.");
this.SendPacketToClient(chat);
return;
}
ShowTaxiNodes taxi = new ShowTaxiNodes();
if (packet.ReadUInt32() != 0)
{
taxi.WindowInfo = new ShowTaxiNodesWindowInfo();
taxi.WindowInfo.UnitGUID = packet.ReadGuid().To128(this.GetSession().GameState);
taxi.WindowInfo.CurrentNode = (this.GetSession().GameState.CurrentTaxiNode = packet.ReadUInt32());
}
while (packet.CanRead())
{
byte nodesMask = packet.ReadUInt8();
taxi.CanLandNodes.Add(nodesMask);
taxi.CanUseNodes.Add(nodesMask);
}
this.GetSession().GameState.UsableTaxiNodes = taxi.CanUseNodes;
this.SendPacketToClient(taxi);
}
```

---

### SMSG_SPECIAL_MOUNT_ANIM

- Legacy value: 370 (0x0172)
- Modern value: 9887 (0x269F)
- Handler: HermesProxy/World/Client/WorldClient.cs:5011
- Fields:
  - `ReadGuid -> UnitGUID`

```csharp
{
SpecialMountAnim mount = new SpecialMountAnim();
mount.UnitGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(mount);
}
```

---

### SMSG_SPELL_COOLDOWN

- Legacy value: 308 (0x0134)
- Modern value: 11285 (0x2C15)
- Handler: HermesProxy/World/Client/WorldClient.cs:8216
- Fields:
  - `ReadGuid -> Caster`
  - `ReadUInt8 -> Flags`
  - `ReadUInt32 -> SpellID`
  - `ReadUInt32 -> ForcedCooldown`
  - `ReadUInt32 -> SpellID`
  - `ReadPackedGuid -> Caster`
  - `ReadUInt32 -> ForcedCooldown`

```csharp
{
SpellCooldownPkt cooldown = new SpellCooldownPkt();
try
{
cooldown.Caster = packet.ReadGuid().To128(this.GetSession().GameState);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
cooldown.Flags = packet.ReadUInt8();
}
while (packet.CanRead())
{
SpellCooldownStruct cd = new SpellCooldownStruct();
cd.SpellID = packet.ReadUInt32();
cd.ForcedCooldown = packet.ReadUInt32();
cooldown.SpellCooldowns.Add(cd);
}
}
catch (ArgumentOutOfRangeException)
{
packet.ResetReadPos();
SpellCooldownStruct cd2 = new SpellCooldownStruct();
cd2.SpellID = packet.ReadUInt32();
cooldown.Caster = packet.ReadPackedGuid().To128(this.GetSession().GameState);
cd2.ForcedCooldown = packet.ReadUInt32();
cooldown.SpellCooldowns.Add(cd2);
}
this.SendPacketToClient(cooldown);
}
```

---

### SMSG_SPELL_DAMAGE_SHIELD

- Legacy value: 591 (0x024F)
- Modern value: 11310 (0x2C2E)
- Handler: HermesProxy/World/Client/WorldClient.cs:8505
- Fields:
  - `ReadGuid -> VictimGUID`
  - `ReadGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadInt32 -> Damage`
  - `ReadUInt32 -> OverKill`
  - `ReadUInt32 -> school`

```csharp
{
SpellDamageShield spell = new SpellDamageShield();
spell.VictimGUID = packet.ReadGuid().To128(this.GetSession().GameState);
spell.CasterGUID = packet.ReadGuid().To128(this.GetSession().GameState);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
spell.SpellID = packet.ReadUInt32();
}
else
{
spell.SpellID = 7294u;
}
spell.Damage = packet.ReadInt32();
spell.OriginalDamage = spell.Damage;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
spell.OverKill = packet.ReadUInt32();
}
uint school = packet.ReadUInt32();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
school = (uint)(1 << (int)(byte)school);
}
spell.SchoolMask = school;
this.SendPacketToClient(spell);
}
```

---

### SMSG_SPELL_DELAYED

- Legacy value: 482 (0x01E2)
- Modern value: 11324 (0x2C3C)
- Handler: HermesProxy/World/Client/WorldClient.cs:8455
- Fields:
  - `ReadPackedGuid -> CasterGUID`
  - `ReadGuid -> CasterGUID`
  - `ReadInt32 -> Delay`

```csharp
{
SpellDelayed delay = new SpellDelayed();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
delay.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
else
{
delay.CasterGUID = packet.ReadGuid().To128(this.GetSession().GameState);
}
delay.Delay = packet.ReadInt32();
this.SendPacketToClient(delay);
}
```

---

### SMSG_SPELL_DISPELL_LOG

- Legacy value: 635 (0x027B)
- Modern value: 11287 (0x2C17)
- Handler: HermesProxy/World/Client/WorldClient.cs:8563
- Fields:
  - `ReadPackedGuid -> TargetGUID`
  - `ReadPackedGuid -> CasterGUID`
  - `ReadUInt32 -> DispelledBySpellID`
  - `ReadBool -> hasDebug`
  - `ReadInt32 -> count`
  - `ReadUInt32 -> SpellID`
  - `ReadBool -> Harmful`
  - `ReadInt32`
  - `ReadInt32`

```csharp
{
SpellDispellLog spell = new SpellDispellLog();
spell.TargetGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
spell.DispelledBySpellID = packet.ReadUInt32();
}
else
{
spell.DispelledBySpellID = this.GetSession().GameState.LastDispellSpellId;
}
bool hasDebug = LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
SpellDispellData dispel = new SpellDispellData
{
SpellID = packet.ReadUInt32()
};
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
dispel.Harmful = packet.ReadBool();
}
spell.DispellData.Add(dispel);
}
if (hasDebug)
{
packet.ReadInt32();
packet.ReadInt32();
```

---

### SMSG_SPELL_ENERGIZE_LOG

- Legacy value: 337 (0x0151)
- Modern value: 11289 (0x2C19)
- Handler: HermesProxy/World/Client/WorldClient.cs:8443
- Fields:
  - `ReadPackedGuid -> TargetGUID`
  - `ReadPackedGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadUInt32 -> Type`
  - `ReadInt32 -> Amount`

```csharp
{
SpellEnergizeLog spell = new SpellEnergizeLog();
spell.TargetGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.SpellID = packet.ReadUInt32();
spell.Type = (PowerType)packet.ReadUInt32();
spell.Amount = packet.ReadInt32();
this.SendPacketToClient(spell);
}
```

---

### SMSG_SPELL_FAILED_OTHER

- Legacy value: 678 (0x02A6)
- Modern value: 11346 (0x2C52)
- Handler: HermesProxy/World/Client/WorldClient.cs:7884
- Fields:
  - `ReadPackedGuid -> casterUnit`
  - `ReadUInt8`
  - `ReadUInt32 -> spellId`
  - `ReadUInt8 -> reason`

```csharp
{
WowGuid128 casterUnit = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180)) ? packet.ReadGuid().To128(this.GetSession().GameState) : packet.ReadPackedGuid().To128(this.GetSession().GameState));
if (casterUnit == this.GetSession().GameState.CurrentPlayerGuid && Settings.ClientSpellDelay > 0)
{
Thread.Sleep(Settings.ClientSpellDelay);
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.ReadUInt8();
}
uint spellId = packet.ReadUInt32();
byte reason = 61;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
reason = (byte)LegacyVersion.ConvertSpellCastResult(packet.ReadUInt8());
}
WowGuid128 castId;
uint spellVisual;
if (this.GetSession().GameState.CurrentPlayerGuid == casterUnit && this.GetSession().GameState.CurrentClientNormalCast != null && this.GetSession().GameState.CurrentClientNormalCast.SpellId == spellId)
{
castId = this.GetSession().GameState.CurrentClientNormalCast.ServerGUID;
spellVisual = this.GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
}
else if (this.GetSession().GameState.CurrentPetGuid == casterUnit && this.GetSession().GameState.CurrentClientPetCast != null && this.GetSession().GameState.CurrentClientPetCast.SpellId == spellId)
{
castId = this.GetSession().GameState.CurrentClientPetCast.ServerGUID;
spellVisual = this.GetSession().GameState.CurrentClientPetCast.SpellXSpellVisualId;
}
else
{
```

---

### SMSG_SPELL_FAILURE

- Legacy value: 307 (0x0133)
- Modern value: 11344 (0x2C50)
- Handler: HermesProxy/World/Client/WorldClient.cs:7878

```csharp
{
// Consumed — SpellFailure is generated from SMSG_SPELL_FAILED_OTHER handler
}
```

---

### SMSG_SPELL_GO

- Legacy value: 306 (0x0132)
- Modern value: 11318 (0x2C36)
- Handler: HermesProxy/World/Client/WorldClient.cs:7991

```csharp
{
if (!this.GetSession().GameState.CurrentMapId.HasValue)
{
return;
}
SpellGo spell = new SpellGo();
spell.Cast = this.HandleSpellStartOrGo(packet, isSpellGo: true);
// 3.3.5a SpellGo doesn't set CAST_FLAG_HAS_TRAJECTORY but 3.4.3 always expects it
spell.Cast.CastFlags |= (uint)CastFlag.HasTrajectory;
if (this.GetSession().GameState.CurrentPlayerGuid == spell.Cast.CasterUnit && this.GetSession().GameState.CurrentClientNormalCast != null && this.GetSession().GameState.CurrentClientNormalCast.SpellId == spell.Cast.SpellID)
{
spell.Cast.CastID = this.GetSession().GameState.CurrentClientNormalCast.ServerGUID;
spell.Cast.SpellXSpellVisualID = this.GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
this.GetSession().GameState.CurrentClientNormalCast = null;
}
else if (this.GetSession().GameState.CurrentPlayerGuid == spell.Cast.CasterUnit && this.GetSession().GameState.CurrentClientSpecialCast != null && this.GetSession().GameState.CurrentClientSpecialCast.SpellId == spell.Cast.SpellID)
{
spell.Cast.CastID = this.GetSession().GameState.CurrentClientSpecialCast.ServerGUID;
spell.Cast.SpellXSpellVisualID = this.GetSession().GameState.CurrentClientSpecialCast.SpellXSpellVisualId;
this.GetSession().GameState.CurrentClientSpecialCast = null;
}
else if (this.GetSession().GameState.CurrentPetGuid == spell.Cast.CasterUnit && this.GetSession().GameState.CurrentClientPetCast != null && this.GetSession().GameState.CurrentClientPetCast.SpellId == spell.Cast.SpellID)
{
spell.Cast.CastID = this.GetSession().GameState.CurrentClientPetCast.ServerGUID;
spell.Cast.SpellXSpellVisualID = this.GetSession().GameState.CurrentClientPetCast.SpellXSpellVisualId;
this.GetSession().GameState.CurrentClientPetCast = null;
}
if (!spell.Cast.CasterUnit.IsEmpty() && GameData.AuraSpells.Contains((uint)spell.Cast.SpellID))
{
foreach (WowGuid128 target in spell.Cast.HitTargets)
```

---

### SMSG_SPELL_HEAL_LOG

- Legacy value: 336 (0x0150)
- Modern value: 11290 (0x2C1A)
- Handler: HermesProxy/World/Client/WorldClient.cs:8331
- Fields:
  - `ReadPackedGuid -> TargetGUID`
  - `ReadPackedGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadInt32 -> HealAmount`
  - `ReadUInt32 -> OverHeal`
  - `ReadUInt32 -> Absorbed`
  - `ReadBool -> Crit`
  - `ReadBool`
  - `ReadFloat -> CritRollMade`
  - `ReadFloat -> CritRollNeeded`

```csharp
{
SpellHealLog spell = new SpellHealLog();
spell.TargetGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.SpellID = packet.ReadUInt32();
spell.HealAmount = packet.ReadInt32();
spell.OriginalHealAmount = spell.HealAmount;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
{
spell.OverHeal = packet.ReadUInt32();
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
spell.Absorbed = packet.ReadUInt32();
}
spell.Crit = packet.ReadBool();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180) && packet.ReadBool())
{
spell.CritRollMade = packet.ReadFloat();
spell.CritRollNeeded = packet.ReadFloat();
}
this.SendPacketToClient(spell);
}
```

---

### SMSG_SPELL_INSTAKILL_LOG

- Legacy value: 815 (0x032F)
- Modern value: 11312 (0x2C30)
- Handler: HermesProxy/World/Client/WorldClient.cs:8546
- Fields:
  - `ReadGuid -> CasterGUID`
  - `ReadGuid -> TargetGUID`
  - `ReadGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`

```csharp
{
SpellInstakillLog spell = new SpellInstakillLog();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
spell.CasterGUID = packet.ReadGuid().To128(this.GetSession().GameState);
spell.TargetGUID = packet.ReadGuid().To128(this.GetSession().GameState);
}
else
{
spell.CasterGUID = (spell.TargetGUID = packet.ReadGuid().To128(this.GetSession().GameState));
}
spell.SpellID = packet.ReadUInt32();
this.SendPacketToClient(spell);
}
```

---

### SMSG_SPELL_NON_MELEE_DAMAGE_LOG

- Legacy value: 592 (0x0250)
- Modern value: 11311 (0x2C2F)
- Handler: HermesProxy/World/Client/WorldClient.cs:8275
- Fields:
  - `ReadPackedGuid -> TargetGUID`
  - `ReadPackedGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadInt32 -> Damage`
  - `ReadInt32 -> Overkill`
  - `ReadUInt8 -> school`
  - `ReadInt32 -> Absorbed`
  - `ReadInt32 -> Resisted`
  - `ReadBool -> Periodic`
  - `ReadUInt8`
  - `ReadInt32 -> ShieldBlock`
  - `ReadUInt32 -> Flags`
  - `ReadBool`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`
  - `ReadFloat`

```csharp
{
SpellNonMeleeDamageLog spell = new SpellNonMeleeDamageLog();
spell.TargetGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.SpellID = packet.ReadUInt32();
spell.SpellXSpellVisualID = GameData.GetSpellVisual(spell.SpellID);
spell.CastID = WowGuid128.Create(HighGuidType703.Cast, SpellCastSource.Normal, this.GetSession().GameState.CurrentMapId.Value, spell.SpellID, spell.SpellID + spell.CasterGUID.GetCounter());
spell.Damage = packet.ReadInt32();
spell.OriginalDamage = spell.Damage;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_3_9183))
{
spell.Overkill = packet.ReadInt32();
}
else
{
spell.Overkill = -1;
}
byte school = packet.ReadUInt8();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
school = (byte)(1 << (int)school);
}
spell.SchoolMask = school;
spell.Absorbed = packet.ReadInt32();
spell.Resisted = packet.ReadInt32();
spell.Periodic = packet.ReadBool();
packet.ReadUInt8();
spell.ShieldBlock = packet.ReadInt32();
spell.Flags = (SpellHitType)packet.ReadUInt32();
if (packet.ReadBool() && !spell.Flags.HasAnyFlag(SpellHitType.Split))
```

---

### SMSG_SPELL_PERIODIC_AURA_LOG

- Legacy value: 590 (0x024E)
- Modern value: 11288 (0x2C18)
- Handler: HermesProxy/World/Client/WorldClient.cs:8357
- Fields:
  - `ReadPackedGuid -> TargetGUID`
  - `ReadPackedGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadInt32 -> count`
  - `ReadUInt32 -> aura`
  - `ReadInt32 -> Amount`
  - `ReadUInt32 -> OverHealOrKill`
  - `ReadUInt32 -> school`
  - `ReadUInt32 -> AbsorbedOrAmplitude`
  - `ReadUInt32 -> Resisted`
  - `ReadBool -> Crit`
  - `ReadInt32 -> Amount`
  - `ReadUInt32 -> OverHealOrKill`
  - `ReadUInt32 -> AbsorbedOrAmplitude`
  - `ReadBool -> Crit`
  - `ReadUInt32 -> SchoolMaskOrPower`
  - `ReadInt32 -> Amount`
  - `ReadUInt32 -> SchoolMaskOrPower`
  - `ReadInt32 -> Amount`
  - `ReadFloat`

```csharp
{
SpellPeriodicAuraLog spell = new SpellPeriodicAuraLog();
spell.TargetGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
spell.SpellID = packet.ReadUInt32();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
AuraType aura = (AuraType)packet.ReadUInt32();
switch (aura)
{
case AuraType.PeriodicDamage:
case AuraType.PeriodicDamagePercent:
{
SpellPeriodicAuraLog.SpellLogEffect effect4 = new SpellPeriodicAuraLog.SpellLogEffect();
effect4.Effect = (uint)aura;
effect4.Amount = packet.ReadInt32();
effect4.OriginalDamage = effect4.Amount;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
effect4.OverHealOrKill = packet.ReadUInt32();
}
uint school = packet.ReadUInt32();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
school = (uint)(1 << (int)(byte)school);
}
effect4.SchoolMaskOrPower = school;
effect4.AbsorbedOrAmplitude = packet.ReadUInt32();
effect4.Resisted = packet.ReadUInt32();
```

---

### SMSG_SPELL_START

- Legacy value: 305 (0x0131)
- Modern value: 11319 (0x2C37)
- Handler: HermesProxy/World/Client/WorldClient.cs:7935

```csharp
{
if (!this.GetSession().GameState.CurrentMapId.HasValue)
{
return;
}
SpellStart spell = new SpellStart();
spell.Cast = this.HandleSpellStartOrGo(packet, isSpellGo: false);
byte failPending = 0;
if (this.GetSession().GameState.CurrentPlayerGuid == spell.Cast.CasterUnit && this.GetSession().GameState.CurrentClientNormalCast != null && this.GetSession().GameState.CurrentClientNormalCast.SpellId == spell.Cast.SpellID)
{
spell.Cast.CastID = this.GetSession().GameState.CurrentClientNormalCast.ServerGUID;
spell.Cast.SpellXSpellVisualID = this.GetSession().GameState.CurrentClientNormalCast.SpellXSpellVisualId;
this.GetSession().GameState.CurrentClientNormalCast.HasStarted = true;
SpellPrepare prepare = new SpellPrepare();
prepare.ClientCastID = this.GetSession().GameState.CurrentClientNormalCast.ClientGUID;
prepare.ServerCastID = spell.Cast.CastID;
this.SendPacketToClient(prepare);
failPending = 1;
}
else if (this.GetSession().GameState.CurrentPetGuid == spell.Cast.CasterUnit && this.GetSession().GameState.CurrentClientPetCast != null && this.GetSession().GameState.CurrentClientPetCast.SpellId == spell.Cast.SpellID)
{
spell.Cast.CastID = this.GetSession().GameState.CurrentClientPetCast.ServerGUID;
spell.Cast.SpellXSpellVisualID = this.GetSession().GameState.CurrentClientPetCast.SpellXSpellVisualId;
this.GetSession().GameState.CurrentClientPetCast.HasStarted = true;
SpellPrepare prepare2 = new SpellPrepare();
prepare2.ClientCastID = this.GetSession().GameState.CurrentClientPetCast.ClientGUID;
prepare2.ServerCastID = spell.Cast.CastID;
this.SendPacketToClient(prepare2);
failPending = 2;
}
```

---

### SMSG_SPIRIT_HEALER_CONFIRM

- Legacy value: 546 (0x0222)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:6094
- Fields:
  - `ReadGuid -> guid`
  - `WriteGuid(guid)`

```csharp
{
// 3.4.3 client has no SMSG_SPIRIT_HEALER_CONFIRM opcode — spirit healer works directly.
// Auto-accept by sending CMSG_SPIRIT_HEALER_ACTIVATE back to the legacy server.
WowGuid64 guid = packet.ReadGuid();
WorldPacket activate = new WorldPacket(Opcode.CMSG_SPIRIT_HEALER_ACTIVATE);
activate.WriteGuid(guid);
this.SendPacket(activate);
}
```

---

### SMSG_STAND_STATE_UPDATE

- Legacy value: 669 (0x029D)
- Modern value: 10012 (0x271C)
- Handler: HermesProxy/World/Client/WorldClient.cs:4959
- Fields:
  - `ReadUInt8 -> StandState`

```csharp
{
StandStateUpdate state = new StandStateUpdate();
state.StandState = packet.ReadUInt8();
this.SendPacketToClient(state);
}
```

---

### SMSG_START_MIRROR_TIMER

- Legacy value: 473 (0x01D9)
- Modern value: 9999 (0x270F)
- Handler: HermesProxy/World/Client/WorldClient.cs:5019
- Fields:
  - `ReadUInt32 -> Timer`
  - `ReadInt32 -> Value`
  - `ReadInt32 -> MaxValue`
  - `ReadInt32 -> Scale`
  - `ReadBool -> Paused`
  - `ReadInt32 -> SpellID`

```csharp
{
StartMirrorTimer timer = new StartMirrorTimer();
timer.Timer = (MirrorTimerType)packet.ReadUInt32();
timer.Value = packet.ReadInt32();
timer.MaxValue = packet.ReadInt32();
timer.Scale = packet.ReadInt32();
timer.Paused = packet.ReadBool();
timer.SpellID = packet.ReadInt32();
this.SendPacketToClient(timer);
}
```

---

### SMSG_STOP_MIRROR_TIMER

- Legacy value: 475 (0x01DB)
- Modern value: 10001 (0x2711)
- Handler: HermesProxy/World/Client/WorldClient.cs:5041
- Fields:
  - `ReadUInt32 -> Timer`

```csharp
{
StopMirrorTimer timer = new StopMirrorTimer();
timer.Timer = (MirrorTimerType)packet.ReadUInt32();
this.SendPacketToClient(timer);
}
```

---

### SMSG_SUMMON_REQUEST

- Legacy value: 683 (0x02AB)
- Modern value: 10017 (0x2721)
- Handler: HermesProxy/World/Client/WorldClient.cs:2707
- Fields:
  - `ReadGuid -> SummonerGUID`
  - `ReadInt32 -> AreaID`
  - `ReadUInt32`

```csharp
{
SummonRequest summon = new SummonRequest();
summon.SummonerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
summon.SummonerVirtualRealmAddress = this.GetSession().RealmId.GetAddress();
summon.AreaID = packet.ReadInt32();
packet.ReadUInt32();
this.SendPacketToClient(summon);
}
```

---

### SMSG_SUPERCEDED_SPELLS

- Legacy value: 300 (0x012C)
- Modern value: 11337 (0x2C49)
- Handler: HermesProxy/World/Client/WorldClient.cs:7672
- Fields:
  - `ReadUInt32 -> supercededId`
  - `ReadUInt32 -> spellId`
  - `ReadUInt16 -> supercededId`
  - `ReadUInt16 -> spellId`

```csharp
{
SupercededSpells spells = new SupercededSpells();
uint supercededId;
uint spellId;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
supercededId = packet.ReadUInt32();
spellId = packet.ReadUInt32();
}
else
{
supercededId = packet.ReadUInt16();
spellId = packet.ReadUInt16();
}
spells.SpellID.Add(spellId);
spells.Superceded.Add(supercededId);
this.SendPacketToClient(spells);
}
```

---

### SMSG_TAXI_NODE_STATUS

- Legacy value: 427 (0x01AB)
- Modern value: 9852 (0x267C)
- Handler: HermesProxy/World/Client/WorldClient.cs:8882
- Fields:
  - `ReadGuid -> FlightMaster`
  - `ReadBool -> learned`

```csharp
{
TaxiNodeStatusPkt taxi = new TaxiNodeStatusPkt();
taxi.FlightMaster = packet.ReadGuid().To128(this.GetSession().GameState);
bool learned = packet.ReadBool();
taxi.Status = (learned ? TaxiNodeStatus.Learned : TaxiNodeStatus.Unlearned);
this.SendPacketToClient(taxi);
}
```

---

### SMSG_TEXT_EMOTE

- Legacy value: 261 (0x0105)
- Modern value: 9850 (0x267A)
- Handler: HermesProxy/World/Client/WorldClient.cs:1928
- Fields:
  - `ReadGuid -> SourceGUID`
  - `ReadInt32 -> EmoteID`
  - `ReadInt32 -> SoundIndex`
  - `ReadUInt32 -> nameLength`
  - `ReadString -> targetName`

```csharp
{
STextEmote emote = new STextEmote();
emote.SourceGUID = packet.ReadGuid().To128(this.GetSession().GameState);
emote.SourceAccountGUID = this.GetSession().GetGameAccountGuidForPlayer(emote.SourceGUID);
emote.EmoteID = packet.ReadInt32();
emote.SoundIndex = packet.ReadInt32();
uint nameLength = packet.ReadUInt32();
string targetName = packet.ReadString(nameLength);
WowGuid128 targetGuid = this.GetSession().GameState.GetPlayerGuidByName(targetName);
emote.TargetGUID = ((targetGuid != null) ? targetGuid : WowGuid128.Empty);
this.SendPacketToClient(emote);
}
```

---

### SMSG_THREAT_CLEAR

- Legacy value: 1157 (0x0485)
- Modern value: 9948 (0x26DC)
- Handler: HermesProxy/World/Client/WorldClient.cs:2051
- Fields:
  - `ReadPackedGuid -> unitGuid`

```csharp
{
// Consume packet to prevent "No handler" warning — client doesn't need this
WowGuid128 unitGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
Log.Print(LogType.Debug, $"[Combat] THREAT_CLEAR unit={unitGuid} (consumed, not forwarded)", "HandleThreatClear", "");
}
```

---

### SMSG_THREAT_UPDATE

- Legacy value: 1155 (0x0483)
- Modern value: 9946 (0x26DA)
- Handler: HermesProxy/World/Client/WorldClient.cs:2059
- Fields:
  - `ReadPackedGuid -> unitGuid`

```csharp
{
// Consume packet to prevent "No handler" warning — client doesn't need this
WowGuid128 unitGuid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
Log.Print(LogType.Debug, $"[Combat] THREAT_UPDATE unit={unitGuid} (consumed, not forwarded)", "HandleThreatUpdate", "");
}
```

---

### SMSG_TIME_SYNC_REQUEST

- Legacy value: 912 (0x0390)
- Modern value: 11730 (0x2DD2)
- Handler: HermesProxy/World/Client/WorldClient.cs:4864
- Fields:
  - `ReadUInt32 -> SequenceIndex`

```csharp
{
TimeSyncRequest sync = new TimeSyncRequest();
sync.SequenceIndex = packet.ReadUInt32();
this.SendPacketToClient(sync);
}
```

---

### SMSG_TOTEM_CREATED

- Legacy value: 1043 (0x0413)
- Modern value: 9928 (0x26C8)
- Handler: HermesProxy/World/Client/WorldClient.cs:8808
- Fields:
  - `ReadUInt8 -> Slot`
  - `ReadGuid -> Totem`
  - `ReadUInt32 -> Duration`
  - `ReadUInt32 -> SpellId`

```csharp
{
TotemCreated totem = new TotemCreated();
totem.Slot = packet.ReadUInt8();
totem.Totem = packet.ReadGuid().To128(this.GetSession().GameState);
totem.Duration = packet.ReadUInt32();
totem.SpellId = packet.ReadUInt32();
this.SendPacketToClient(totem);
}
```

---

### SMSG_TRADE_STATUS

- Legacy value: 288 (0x0120)
- Modern value: 9602 (0x2582)
- Handler: HermesProxy/World/Client/WorldClient.cs:8939
- Fields:
  - `ReadUInt32 -> Status`
  - `ReadGuid -> Partner`
  - `ReadUInt32 -> Id`
  - `ReadUInt32 -> BagResult`
  - `ReadBool -> FailureForYou`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt8 -> TradeSlot`

```csharp
{
TradeStatusPkt trade = new TradeStatusPkt();
trade.Status = (TradeStatus)packet.ReadUInt32();
TradeSession tradeSession = this.GetSession().GameState.CurrentTrade;
if (tradeSession == null)
{
TradeStatus status = trade.Status;
TradeStatus tradeStatus = status;
if ((uint)(tradeStatus - 1) > 1u)
{
Log.Print(LogType.Error, $"Got SMSG_TRADE_STATUS without trade session (status: {trade.Status})", "HandleTradeStatus", "World\\Client\\PacketHandlers\\TradeHandler.cs");
this.SendPacketToClient(new TradeStatusPkt
{
Status = TradeStatus.Cancelled
});
return;
}
tradeSession = new TradeSession();
this.GetSession().GameState.CurrentTrade = tradeSession;
}
switch (trade.Status)
{
case TradeStatus.Proposed:
trade.Partner = (tradeSession.Partner = packet.ReadGuid().To128(this.GetSession().GameState));
trade.PartnerAccount = (tradeSession.PartnerAccount = this.GetSession().GetGameAccountGuidForPlayer(trade.Partner));
break;
case TradeStatus.Initiated:
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
trade.Id = packet.ReadUInt32();
```

---

### SMSG_TRADE_STATUS_EXTENDED

- Legacy value: 289 (0x0121)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:9010
- Fields:
  - `ReadUInt8 -> WhichPlayer`
  - `ReadUInt32 -> actualTradeId`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32 -> Gold`
  - `ReadInt32 -> ProposedEnchantment`
  - `ReadUInt8 -> Slot`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32`
  - `ReadInt32 -> StackCount`
  - `ReadUInt32`
  - `ReadGuid -> GiftCreator`
  - `ReadInt32 -> EnchantID`
  - `ReadUInt32`
  - `ReadGuid -> Creator`
  - `ReadInt32 -> Charges`
  - `ReadUInt32 -> RandomPropertiesSeed`
  - `ReadUInt32 -> RandomPropertiesID`
  - `ReadUInt32 -> Lock`
  - `ReadUInt32 -> MaxDurability`
  - `ReadUInt32 -> Durability`

```csharp
{
TradeSession tradeSession = this.GetSession().GameState.CurrentTrade;
if (tradeSession == null)
{
Log.Print(LogType.Error, "Got SMSG_TRADE_STATUS_EXTENDED without trade session", "HandleTradeStatusExtended", "World\\Client\\PacketHandlers\\TradeHandler.cs");
return;
}
tradeSession.ServerStateIndex++;
TradeUpdated trade = new TradeUpdated();
trade.WhichPlayer = packet.ReadUInt8();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
uint actualTradeId = packet.ReadUInt32();
if (actualTradeId != trade.Id)
{
Log.Print(LogType.Error, $"Got SMSG_TRADE_STATUS_EXTENDED with wrong tradeId (expected {trade.Id} but got {actualTradeId})", "HandleTradeStatusExtended", "World\\Client\\PacketHandlers\\TradeHandler.cs");
return;
}
}
trade.Id = tradeSession.TradeId;
packet.ReadUInt32();
packet.ReadUInt32();
trade.ClientStateIndex = tradeSession.ClientStateIndex;
trade.CurrentStateIndex = tradeSession.ServerStateIndex;
trade.Gold = packet.ReadUInt32();
trade.ProposedEnchantment = packet.ReadInt32();
while (packet.CanRead())
{
TradeUpdated.TradeItem item = new TradeUpdated.TradeItem();
item.Unwrapped = new TradeUpdated.UnwrappedTradeItem();
```

---

### SMSG_TRAINER_BUY_FAILED

- Legacy value: 436 (0x01B4)
- Modern value: 9952 (0x26E0)
- Handler: HermesProxy/World/Client/WorldClient.cs:6073
- Fields:
  - `ReadGuid -> TrainerGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadUInt32 -> TrainerFailedReason`

```csharp
{
TrainerBuyFailed buy = new TrainerBuyFailed();
buy.TrainerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
buy.SpellID = packet.ReadUInt32();
buy.TrainerFailedReason = packet.ReadUInt32();
this.SendPacketToClient(buy);
ChatPkt chat = new ChatPkt(this.GetSession(), ChatMessageTypeModern.System, $"Failed to learn Spell {buy.SpellID} (Reason {buy.TrainerFailedReason}).");
this.SendPacketToClient(chat);
}
```

---

### SMSG_TRAINER_LIST

- Legacy value: 433 (0x01B1)
- Modern value: 9951 (0x26DF)
- Handler: HermesProxy/World/Client/WorldClient.cs:6032
- Fields:
  - `ReadGuid -> TrainerGUID`
  - `ReadInt32 -> TrainerType`
  - `ReadInt32 -> count`
  - `ReadUInt32 -> spellId`
  - `ReadUInt8 -> stateOld`
  - `ReadUInt32 -> MoneyCost`
  - `ReadInt32`
  - `ReadInt32`
  - `ReadUInt8 -> ReqLevel`
  - `ReadUInt32 -> ReqSkillLine`
  - `ReadUInt32 -> ReqSkillRank`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadCString -> Greeting`

```csharp
{
TrainerList trainer = new TrainerList();
trainer.TrainerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = trainer.TrainerGUID;
trainer.TrainerID = trainer.TrainerGUID.GetEntry();
trainer.TrainerType = packet.ReadInt32();
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
TrainerListSpell spell = new TrainerListSpell();
uint spellId = packet.ReadUInt32();
if (ModernVersion.ExpansionVersion > 1 && LegacyVersion.ExpansionVersion <= 1)
{
uint realSpellId = GameData.GetRealSpell(spellId);
if (realSpellId != spellId)
{
this.GetSession().GameState.StoreRealSpell(realSpellId, spellId);
spellId = realSpellId;
}
}
spell.SpellID = spellId;
TrainerSpellStateLegacy stateOld = (TrainerSpellStateLegacy)packet.ReadUInt8();
TrainerSpellStateModern stateNew = (TrainerSpellStateModern)Enum.Parse(typeof(TrainerSpellStateModern), stateOld.ToString());
spell.Usable = stateNew;
spell.MoneyCost = packet.ReadUInt32();
packet.ReadInt32();
packet.ReadInt32();
spell.ReqLevel = packet.ReadUInt8();
spell.ReqSkillLine = packet.ReadUInt32();
spell.ReqSkillRank = packet.ReadUInt32();
```

---

### SMSG_TRANSFER_ABORTED

- Legacy value: 64 (0x0040)
- Modern value: 9987 (0x2703)
- Handler: HermesProxy/World/Client/WorldClient.cs:5524
- Fields:
  - `ReadUInt32 -> MapID`
  - `ReadUInt8 -> Reason`
  - `ReadUInt8 -> legacyReason`
  - `ReadUInt8 -> Arg`

```csharp
{
TransferAborted transfer = new TransferAborted();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
transfer.MapID = packet.ReadUInt32();
}
else
{
transfer.MapID = this.GetSession().GameState.PendingTransferMapId;
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
transfer.Reason = (TransferAbortReasonModern)packet.ReadUInt8();
}
else
{
TransferAbortReasonLegacy legacyReason = (TransferAbortReasonLegacy)packet.ReadUInt8();
transfer.Reason = (TransferAbortReasonModern)Enum.Parse(typeof(TransferAbortReasonModern), legacyReason.ToString());
}
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
transfer.Arg = packet.ReadUInt8();
}
this.SendPacketToClient(transfer);
this.GetSession().GameState.IsWaitingForNewWorld = false;
}
```

---

### SMSG_TRANSFER_PENDING

- Legacy value: 63 (0x003F)
- Modern value: 9677 (0x25CD)
- Handler: HermesProxy/World/Client/WorldClient.cs:5504
- Fields:
  - `ReadUInt32 -> MapID`

```csharp
{
if (this.GetSession().GameState.IsWaitingForWorldPortAck)
{
Log.Print(LogType.Error, "Skipping SMSG_TRANSFER_PENDING, client is already being teleported.", "HandleTransferPending", "World\\Client\\PacketHandlers\\MovementHandler.cs");
return;
}
TransferPending transfer = new TransferPending();
transfer.MapID = (this.GetSession().GameState.PendingTransferMapId = packet.ReadUInt32());
transfer.OldMapPosition = Framework.GameMath.Vector3.Zero;
this.SendPacketToClient(transfer);
this.GetSession().GameState.IsFirstEnterWorld = false;
this.GetSession().GameState.IsWaitingForNewWorld = true;
SuspendToken suspend = new SuspendToken();
suspend.SequenceIndex = 3u;
suspend.Reason = 1u;
this.SendPacketToClient(suspend);
}
```

---

### SMSG_TRIGGER_CINEMATIC

- Legacy value: 250 (0x00FA)
- Modern value: 10186 (0x27CA)
- Handler: HermesProxy/World/Client/WorldClient.cs:5003
- Fields:
  - `ReadUInt32 -> CinematicID`

```csharp
{
TriggerCinematic cinematic = new TriggerCinematic();
cinematic.CinematicID = packet.ReadUInt32();
this.SendPacketToClient(cinematic);
}
```

---

### SMSG_TURN_IN_PETITION_RESULT

- Legacy value: 453 (0x01C5)
- Modern value: 10062 (0x274E)
- Handler: HermesProxy/World/Client/WorldClient.cs:6366
- Fields:
  - `ReadUInt32 -> Result`

```csharp
{
TurnInPetitionResult petition = new TurnInPetitionResult();
petition.Result = (PetitionTurnResult)packet.ReadUInt32();
this.SendPacketToClient(petition);
}
```

---

### SMSG_TUTORIAL_FLAGS

- Legacy value: 253 (0x00FD)
- Modern value: 10174 (0x27BE)
- Handler: HermesProxy/World/Client/WorldClient.cs:4804
- Fields:
  - `ReadUInt32`

```csharp
{
TutorialFlags tutorials = new TutorialFlags();
for (byte i = 0; i < 8; i++)
{
tutorials.TutorialData[i] = packet.ReadUInt32();
}
this.SendPacketToClient(tutorials);
}
```

---

### SMSG_UNLEARNED_SPELLS

- Legacy value: 515 (0x0203)
- Modern value: 11339 (0x2C4B)
- Handler: HermesProxy/World/Client/WorldClient.cs:7715
- Fields:
  - `ReadUInt16 -> spellId`
  - `ReadUInt32 -> spellId`

```csharp
{
UnlearnedSpells spells = new UnlearnedSpells();
uint spellId = ((!LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767)) ? packet.ReadUInt16() : packet.ReadUInt32());
spells.Spells.Add(spellId);
this.SendPacketToClient(spells);
}
```

---

### SMSG_UPDATE_ACTION_BUTTONS

- Legacy value: 297 (0x0129)
- Modern value: 9696 (0x25E0)
- Handler: HermesProxy/World/Client/WorldClient.cs:1151
- Fields:
  - `ReadUInt8 -> type`
  - `ReadInt32 -> packed`

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_1_0_9767))
{
byte type = packet.ReadUInt8();
if (type == 2)
{
return;
}
}
List<int> buttons = new List<int>();
int buttonCount = 120;
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_0_10192))
{
buttonCount = 144;
}
else if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
buttonCount = 132;
}
for (int i = 0; i < buttonCount; i++)
{
int packed = packet.ReadInt32();
buttons.Add(packed);
}
while (buttons.Count < 180)
{
buttons.Add(0);
}
this.GetSession().GameState.ActionButtons = buttons;
UpdateActionButtons updateButtons = new UpdateActionButtons();
```

---

### SMSG_UPDATE_AURA_DURATION

- Legacy value: N/A
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:8608
- Fields:
  - `ReadUInt8 -> slot`
  - `ReadInt32 -> duration`

```csharp
{
byte slot = packet.ReadUInt8();
int duration = packet.ReadInt32();
WowGuid128 guid = this.GetSession().GameState.CurrentPlayerGuid;
if (guid == null)
{
return;
}
this.GetSession().GameState.StoreAuraDurationLeft(guid, slot, duration, (int)packet.GetReceivedTime());
if (duration <= 0)
{
return;
}
Dictionary<int, UpdateField> updateFields = this.GetSession().GameState.GetCachedObjectFieldsLegacy(guid);
if (updateFields != null)
{
AuraInfo aura = new AuraInfo
{
Slot = slot,
AuraData = this.ReadAuraSlot(slot, guid, updateFields)
};
if (aura.AuraData != null)
{
aura.AuraData.Flags |= AuraFlagsModern.Duration;
aura.AuraData.Duration = duration;
aura.AuraData.Remaining = duration;
AuraUpdate update = new AuraUpdate(guid, all: false);
update.Auras.Add(aura);
this.SendPacketToClient(update);
}
```

---

### SMSG_UPDATE_COMBO_POINTS

- Legacy value: 925 (0x039D)
- Modern value: 0 (0x0000)
- Handler: HermesProxy/World/Client/WorldClient.cs:1267
- Fields:
  - `ReadPackedGuid -> ComboTarget`
  - `ReadUInt8 -> comboPoints`

```csharp
{
ObjectUpdate updateData = new ObjectUpdate(this.GetSession().GameState.CurrentPlayerGuid, UpdateTypeModern.Values, this.GetSession());
updateData.ActivePlayerData.ComboTarget = packet.ReadPackedGuid().To128(this.GetSession().GameState);
byte comboPoints = packet.ReadUInt8();
sbyte powerSlot = ClassPowerTypes.GetPowerSlotForClass(this.GetSession().GameState.GetUnitClass(this.GetSession().GameState.CurrentPlayerGuid), PowerType.ComboPoints);
if (powerSlot >= 0)
{
updateData.UnitData.Power[powerSlot] = comboPoints;
}
UpdateObject updatePacket = new UpdateObject(this.GetSession().GameState);
updatePacket.ObjectUpdates.Add(updateData);
this.SendPacketToClient(updatePacket);
}
```

---

### SMSG_UPDATE_INSTANCE_OWNERSHIP

- Legacy value: 811 (0x032B)
- Modern value: 9897 (0x26A9)
- Handler: HermesProxy/World/Client/WorldClient.cs:3943
- Fields:
  - `ReadUInt32 -> IOwnInstance`

```csharp
{
UpdateInstanceOwnership instance = new UpdateInstanceOwnership();
instance.IOwnInstance = packet.ReadUInt32();
this.SendPacketToClient(instance);
}
```

---

### SMSG_UPDATE_OBJECT

- Legacy value: 169 (0x00A9)
- Modern value: 10187 (0x27CB)
- Handler: HermesProxy/World/Client/WorldClient.cs:9090
- Fields:
  - `ReadUInt32 -> count`
  - `ReadBool`
  - `ReadUInt8 -> type`
  - `ReadPackedGuid -> guid3`
  - `ReadPackedGuid -> guid2`
  - `ReadPackedGuid -> oldGuid2`
  - `ReadPackedGuid -> oldGuid`
  - `WriteUInt32(itemId)`
  - `WriteGuid(WowGuid64.Empty)`

```csharp
{
uint count = packet.ReadUInt32();
this.PrintString($"Updates Count = {count}");
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
{
packet.ReadBool();
}
HashSet<uint> missingItemTemplates = new HashSet<uint>();
List<AuraUpdate> auraUpdates = new List<AuraUpdate>();
UpdateObject updateObject = new UpdateObject(this.GetSession().GameState);
for (int i = 0; i < count; i++)
{
UpdateTypeLegacy type = (UpdateTypeLegacy)packet.ReadUInt8();
this.PrintString($"Update Type = {type}", i);
switch (type)
{
case UpdateTypeLegacy.Values:
{
WowGuid128 guid3 = packet.ReadPackedGuid().To128(this.GetSession().GameState);
this.PrintString("Guid = " + guid3.ToString(), i);
ObjectUpdate updateData2 = new ObjectUpdate(guid3, UpdateTypeModern.Values, this.GetSession());
AuraUpdate auraUpdate2 = new AuraUpdate(guid3, all: false);
PowerUpdate powerUpdate = new PowerUpdate(guid3);
this.ReadValuesUpdateBlock(packet, guid3, updateData2, auraUpdate2, powerUpdate, i);
if (powerUpdate.Powers.Count != 0)
{
this.SendPacketToClient(powerUpdate);
}
if (guid3 == this.GetSession().GameState.CurrentPlayerGuid)
{
```

---

### SMSG_UPDATE_TALENT_DATA

- Legacy value: 1216 (0x04C0)
- Modern value: 9687 (0x25D7)
- Handler: HermesProxy/World/Client/WorldClient.cs:5094

```csharp
{
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
}
}
```

---

### SMSG_UPDATE_WORLD_STATE

- Legacy value: 707 (0x02C3)
- Modern value: 10056 (0x2748)
- Handler: HermesProxy/World/Client/WorldClient.cs:12245
- Fields:
  - `ReadUInt32 -> VariableID`
  - `ReadInt32 -> Value`

```csharp
{
UpdateWorldState update = new UpdateWorldState();
update.VariableID = packet.ReadUInt32();
update.Value = packet.ReadInt32();
this.SendPacketToClient(update);
if (update.VariableID == 2339)
{
WorldPacket packet2 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
this.SendPacket(packet2);
this.GetSession().GameState.HasWsgAllyFlagCarrier = update.Value == 2;
}
else if (update.VariableID == 2338)
{
WorldPacket packet3 = new WorldPacket(Opcode.MSG_BATTLEGROUND_PLAYER_POSITIONS);
this.SendPacket(packet3);
this.GetSession().GameState.HasWsgHordeFlagCarrier = update.Value == 2;
}
}
```

---

### SMSG_VENDOR_INVENTORY

- Legacy value: 415 (0x019F)
- Modern value: 9656 (0x25B8)
- Handler: HermesProxy/World/Client/WorldClient.cs:5989
- Fields:
  - `ReadGuid -> VendorGUID`
  - `ReadUInt8 -> itemsCount`
  - `ReadUInt8 -> Reason`
  - `ReadInt32 -> Slot`
  - `ReadUInt32 -> ItemID`
  - `ReadUInt32`
  - `ReadInt32 -> Quantity`
  - `ReadUInt32 -> Price`
  - `ReadInt32 -> Durability`
  - `ReadUInt32 -> StackCount`
  - `ReadInt32 -> ExtendedCostID`

```csharp
{
VendorInventory vendor = new VendorInventory();
vendor.VendorGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = vendor.VendorGUID;
byte itemsCount = packet.ReadUInt8();
if (itemsCount == 0)
{
vendor.Reason = packet.ReadUInt8();
this.SendPacketToClient(vendor);
return;
}
for (byte i = 0; i < itemsCount; i++)
{
VendorItem vendorItem = new VendorItem();
vendorItem.Slot = packet.ReadInt32();
vendorItem.MuID = (uint)(i + 1);
vendorItem.Item.ItemID = packet.ReadUInt32();
packet.ReadUInt32();
vendorItem.Quantity = packet.ReadInt32();
vendorItem.Price = packet.ReadUInt32();
vendorItem.Durability = packet.ReadInt32();
vendorItem.StackCount = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
vendorItem.ExtendedCostID = packet.ReadInt32();
}
this.GetSession().GameState.SetItemBuyCount(vendorItem.Item.ItemID, vendorItem.StackCount);
vendor.Items.Add(vendorItem);
}
this.SendPacketToClient(vendor);
```

---

### SMSG_WEATHER

- Legacy value: 756 (0x02F4)
- Modern value: 9894 (0x26A6)
- Handler: HermesProxy/World/Client/WorldClient.cs:4872
- Fields:
  - `ReadUInt32 -> type`
  - `ReadFloat -> Intensity`
  - `ReadUInt32`
  - `ReadBool -> Abrupt`
  - `ReadUInt32 -> WeatherID`
  - `ReadFloat -> Intensity`
  - `ReadBool -> Abrupt`

```csharp
{
WeatherPkt weather = new WeatherPkt();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
WeatherType type = (WeatherType)packet.ReadUInt32();
weather.Intensity = packet.ReadFloat();
weather.WeatherID = Weather.ConvertWeatherTypeToWeatherState(type, weather.Intensity);
packet.ReadUInt32();
if (packet.CanRead())
{
weather.Abrupt = packet.ReadBool();
}
}
else
{
weather.WeatherID = (WeatherState)packet.ReadUInt32();
weather.Intensity = packet.ReadFloat();
weather.Abrupt = packet.ReadBool();
}
this.SendPacketToClient(weather);
this.SendPacketToClient(new StartLightningStorm());
}
```

---

### SMSG_WHO

- Legacy value: 99 (0x0063)
- Modern value: 11182 (0x2BAE)
- Handler: HermesProxy/World/Client/WorldClient.cs:6946
- Fields:
  - `ReadUInt32 -> count`
  - `ReadUInt32`
  - `ReadCString -> Name`
  - `ReadCString -> GuildName`
  - `ReadUInt32 -> Level`
  - `ReadUInt32 -> ClassID`
  - `ReadUInt32 -> RaceID`
  - `ReadUInt8 -> Sex`
  - `ReadInt32 -> AreaID`

```csharp
{
WhoResponsePkt response = new WhoResponsePkt();
response.RequestID = this.GetSession().GameState.LastWhoRequestId;
uint count = packet.ReadUInt32();
packet.ReadUInt32();
for (int i = 0; i < count; i++)
{
WhoEntry player = new WhoEntry();
player.PlayerData.Name = packet.ReadCString();
player.GuildName = packet.ReadCString();
player.PlayerData.Level = (byte)packet.ReadUInt32();
player.PlayerData.ClassID = (Class)packet.ReadUInt32();
player.PlayerData.RaceID = (Race)packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
player.PlayerData.Sex = (Gender)packet.ReadUInt8();
}
player.AreaID = packet.ReadInt32();
player.PlayerData.GuidActual = this.GetSession().GameState.GetPlayerGuidByName(player.PlayerData.Name);
if (player.PlayerData.GuidActual == null)
{
player.PlayerData.GuidActual = WowGuid128.CreateUnknownPlayerGuid();
}
player.PlayerData.AccountID = this.GetSession().GetGameAccountGuidForPlayer(player.PlayerData.GuidActual);
player.PlayerData.BnetAccountID = this.GetSession().GetBnetAccountGuidForPlayer(player.PlayerData.GuidActual);
player.PlayerData.VirtualRealmAddress = this.GetSession().RealmId.GetAddress();
if (!string.IsNullOrEmpty(player.GuildName))
{
player.GuildGUID = this.GetSession().GetGuildGuid(player.GuildName);
player.GuildVirtualRealmAddress = player.PlayerData.VirtualRealmAddress;
```

---

### SMSG_ZONE_UNDER_ATTACK

- Legacy value: 596 (0x0254)
- Modern value: 11189 (0x2BB5)
- Handler: HermesProxy/World/Client/WorldClient.cs:5061
- Fields:
  - `ReadInt32 -> AreaID`

```csharp
{
ZoneUnderAttack zone = new ZoneUnderAttack();
zone.AreaID = packet.ReadInt32();
this.SendPacketToClient(zone);
}
```

---

## MSG Handlers

### MSG_AUCTION_HELLO

- Legacy value: 597 (0x0255)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:201
- Fields:
  - `ReadGuid -> Guid`
  - `ReadUInt32 -> AuctionHouseID`
  - `ReadBool -> OpenForBusiness`
  - `WriteGuid(auction.Guid.To64()`
  - `WriteUInt32(0u)`

```csharp
{
AuctionHelloResponse auction = new AuctionHelloResponse();
auction.Guid = packet.ReadGuid().To128(this.GetSession().GameState);
this.GetSession().GameState.CurrentInteractedWithNPC = auction.Guid;
auction.AuctionHouseID = packet.ReadUInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_3_0_10958))
{
auction.OpenForBusiness = packet.ReadBool();
}
this.SendPacketToClient(auction);
WorldPacket packet2 = new WorldPacket(Opcode.CMSG_AUCTION_LIST_OWNED_ITEMS);
packet2.WriteGuid(auction.Guid.To64());
packet2.WriteUInt32(0u);
this.SendPacketToServer(packet2);
}
```

---

### MSG_BATTLEGROUND_PLAYER_POSITIONS

- Legacy value: 745 (0x02E9)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:747, HermesProxy/World/Client/WorldClient.cs:776
- Fields:
  - `ReadUInt32 -> teamMembersCount`
  - `ReadBool`
  - `ReadUInt32 -> teamMembersCount`
  - `ReadUInt32 -> flagCarriersCount`

```csharp
{
this.GetSession().GameState.FlagCarrierGuids.Clear();
BattlegroundPlayerPositions bglist = new BattlegroundPlayerPositions();
uint teamMembersCount = packet.ReadUInt32();
for (uint i = 0u; i < teamMembersCount; i++)
{
this.ReadBattlegroundPlayerPosition(packet);
}
if (packet.ReadBool())
{
BattlegroundPlayerPosition position = this.ReadBattlegroundPlayerPosition(packet);
if (this.GetSession().GameState.IsAlliancePlayer(position.Guid))
{
position.IconID = 1;
position.ArenaSlot = 3;
}
else
{
position.IconID = 2;
position.ArenaSlot = 2;
}
bglist.FlagCarriers.Add(position);
this.GetSession().GameState.FlagCarrierGuids.Add(position.Guid);
}
this.SendPacketToClient(bglist);
}
```

---

### MSG_CHANNEL_START

- Legacy value: 313 (0x0139)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:8471
- Fields:
  - `ReadPackedGuid -> CasterGUID`
  - `ReadUInt32 -> SpellID`
  - `ReadUInt32 -> Duration`

```csharp
{
SpellChannelStart channel = new SpellChannelStart();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
channel.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
else
{
channel.CasterGUID = this.GetSession().GameState.CurrentPlayerGuid;
}
channel.SpellID = packet.ReadUInt32();
channel.SpellXSpellVisualID = GameData.GetSpellVisual(channel.SpellID);
channel.Duration = packet.ReadUInt32();
this.SendPacketToClient(channel);
}
```

---

### MSG_CHANNEL_UPDATE

- Legacy value: 314 (0x013A)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:8489
- Fields:
  - `ReadPackedGuid -> CasterGUID`
  - `ReadInt32 -> TimeRemaining`

```csharp
{
SpellChannelUpdate channel = new SpellChannelUpdate();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V2_0_1_6180))
{
channel.CasterGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
}
else
{
channel.CasterGUID = this.GetSession().GameState.CurrentPlayerGuid;
}
channel.TimeRemaining = packet.ReadInt32();
this.SendPacketToClient(channel);
}
```

---

### MSG_CORPSE_QUERY

- Legacy value: 534 (0x0216)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:4933
- Fields:
  - `ReadBool -> Valid`
  - `ReadInt32 -> ActualMapID`
  - `ReadVector3`
  - `ReadInt32 -> MapID`
  - `ReadInt32`

```csharp
{
CorpseLocation corpse = new CorpseLocation
{
Player = this.GetSession().GameState.CurrentPlayerGuid,
Transport = WowGuid128.Empty
};
corpse.Valid = packet.ReadBool();
if (corpse.Valid)
{
corpse.ActualMapID = packet.ReadInt32();
corpse.Position = packet.ReadVector3();
corpse.MapID = packet.ReadInt32();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_2_2_10482))
{
packet.ReadInt32();
}
}
else
{
corpse.MapID = (corpse.ActualMapID = (int)this.GetSession().GameState.CurrentMapId.Value);
}
this.SendPacketToClient(corpse);
}
```

---

### MSG_GUILD_BANK_LOG_QUERY

- Legacy value: 1006 (0x03EE)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3905
- Fields:
  - `ReadUInt8 -> Tab`
  - `ReadUInt8 -> logSize`
  - `ReadGuid -> PlayerGUID`
  - `ReadInt32 -> ItemID`
  - `ReadUInt8 -> Count`
  - `ReadUInt32 -> Money`
  - `ReadUInt32 -> TimeOffset`

```csharp
{
GuildBankLogQueryResults result = new GuildBankLogQueryResults();
result.Tab = packet.ReadUInt8();
byte logSize = packet.ReadUInt8();
for (byte i = 0; i < logSize; i++)
{
GuildBankLogEntry logEntry = new GuildBankLogEntry();
logEntry.EntryType = packet.ReadInt8();
logEntry.PlayerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
if (result.Tab != 6)
{
logEntry.ItemID = packet.ReadInt32();
logEntry.Count = packet.ReadUInt8();
if (logEntry.EntryType == 3 || logEntry.EntryType == 7)
{
logEntry.OtherTab = packet.ReadInt8();
}
}
else
{
logEntry.Money = packet.ReadUInt32();
}
logEntry.TimeOffset = packet.ReadUInt32();
result.Entry.Add(logEntry);
}
this.SendPacketToClient(result);
}
```

---

### MSG_GUILD_BANK_MONEY_WITHDRAWN

- Legacy value: 1022 (0x03FE)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3935
- Fields:
  - `ReadUInt32 -> RemainingWithdrawMoney`

```csharp
{
GuildBankRemainingWithdrawMoney result = new GuildBankRemainingWithdrawMoney();
result.RemainingWithdrawMoney = packet.ReadUInt32();
this.SendPacketToClient(result);
}
```

---

### MSG_INSPECT_ARENA_TEAMS

- Legacy value: 887 (0x0377)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:1458
- Fields:
  - `ReadGuid -> PlayerGUID`
  - `ReadUInt8 -> slot`
  - `ReadUInt32 -> teamId`
  - `ReadInt32 -> TeamRating`
  - `ReadInt32 -> TeamGamesPlayed`
  - `ReadInt32 -> TeamGamesWon`
  - `ReadInt32 -> PersonalGamesPlayed`
  - `ReadInt32 -> PersonalRating`

```csharp
{
InspectPvP inspect = new InspectPvP();
inspect.PlayerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
ArenaTeamInspectData team = new ArenaTeamInspectData();
byte slot = packet.ReadUInt8();
uint teamId = packet.ReadUInt32();
team.TeamGuid = WowGuid128.Create(HighGuidType703.ArenaTeam, teamId);
team.TeamRating = packet.ReadInt32();
team.TeamGamesPlayed = packet.ReadInt32();
team.TeamGamesWon = packet.ReadInt32();
team.PersonalGamesPlayed = packet.ReadInt32();
team.PersonalRating = packet.ReadInt32();
this.GetSession().GameState.StoreArenaTeamDataForPlayer(inspect.PlayerGUID, slot, team);
for (byte i = 0; i < 3; i++)
{
inspect.ArenaTeams.Add(this.GetSession().GameState.GetArenaTeamDataForPlayer(inspect.PlayerGUID, slot));
}
this.SendPacketToClient(inspect);
}
```

---

### MSG_INSPECT_HONOR_STATS

- Legacy value: 726 (0x02D6)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:1372, HermesProxy/World/Client/WorldClient.cs:1425
- Fields:
  - `ReadGuid -> playerGuid`
  - `ReadUInt8 -> lifetimeHighestRank`
  - `ReadUInt16 -> todayHonorableKills`
  - `ReadUInt16 -> todayDishonorableKills`
  - `ReadUInt16 -> yesterdayHonorableKills`
  - `ReadUInt16 -> yesterdayDishonorableKills`
  - `ReadUInt16 -> lastWeekHonorableKills`
  - `ReadUInt16 -> lastWeekDishonorableKills`
  - `ReadUInt16 -> thisWeekHonorableKills`
  - `ReadUInt16 -> thisWeekDishonorableKills`
  - `ReadUInt32 -> lifetimeHonorableKills`
  - `ReadUInt32 -> lifetimeDishonorableKills`
  - `ReadUInt32 -> yesterdayHonor`
  - `ReadUInt32 -> lastWeekHonor`
  - `ReadUInt32 -> thisWeekHonor`
  - `ReadUInt32 -> standing`
  - `ReadUInt8 -> rankProgress`
  - `ReadGuid -> playerGuid`
  - `ReadUInt8 -> lifetimeHighestRank`
  - `ReadUInt16 -> todayHonorableKills`
  - `ReadUInt16 -> yesterdayHonorableKills`
  - `ReadUInt32 -> todayHonor`
  - `ReadUInt32 -> yesterdayHonor`
  - `ReadUInt32 -> lifetimeHonorableKills`

```csharp
{
WowGuid128 playerGuid = packet.ReadGuid().To128(this.GetSession().GameState);
byte lifetimeHighestRank = packet.ReadUInt8();
ushort todayHonorableKills = packet.ReadUInt16();
ushort todayDishonorableKills = packet.ReadUInt16();
ushort yesterdayHonorableKills = packet.ReadUInt16();
ushort yesterdayDishonorableKills = packet.ReadUInt16();
ushort lastWeekHonorableKills = packet.ReadUInt16();
ushort lastWeekDishonorableKills = packet.ReadUInt16();
ushort thisWeekHonorableKills = packet.ReadUInt16();
ushort thisWeekDishonorableKills = packet.ReadUInt16();
uint lifetimeHonorableKills = packet.ReadUInt32();
uint lifetimeDishonorableKills = packet.ReadUInt32();
uint yesterdayHonor = packet.ReadUInt32();
uint lastWeekHonor = packet.ReadUInt32();
uint thisWeekHonor = packet.ReadUInt32();
uint standing = packet.ReadUInt32();
byte rankProgress = packet.ReadUInt8();
if (ModernVersion.ExpansionVersion == 1)
{
InspectHonorStatsResultClassic inspect = new InspectHonorStatsResultClassic();
inspect.PlayerGUID = playerGuid;
inspect.LifetimeHighestRank = lifetimeHighestRank;
inspect.TodayHonorableKills = todayHonorableKills;
inspect.TodayDishonorableKills = todayDishonorableKills;
inspect.YesterdayHonorableKills = yesterdayHonorableKills;
inspect.YesterdayDishonorableKills = yesterdayDishonorableKills;
inspect.LastWeekHonorableKills = lastWeekHonorableKills;
inspect.LastWeekDishonorableKills = lastWeekDishonorableKills;
inspect.ThisWeekHonorableKills = thisWeekHonorableKills;
```

---

### MSG_LIST_STABLED_PETS

- Legacy value: 623 (0x026F)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:6184
- Fields:
  - `ReadGuid -> StableMaster`
  - `ReadUInt8 -> count`
  - `ReadUInt8 -> NumStableSlots`
  - `ReadUInt32 -> PetNumber`
  - `ReadUInt32 -> CreatureID`
  - `ReadUInt32 -> ExperienceLevel`
  - `ReadCString -> PetName`
  - `ReadUInt32 -> LoyaltyLevel`
  - `ReadUInt8 -> PetFlags`
  - `WriteUInt32(pet.CreatureID)`
  - `WriteGuid(WowGuid64.Empty)`

```csharp
{
PetGuids pets = new PetGuids();
Dictionary<int, UpdateField> updateFields = this.GetSession().GameState.GetCachedObjectFieldsLegacy(this.GetSession().GameState.CurrentPlayerGuid);
int UNIT_FIELD_SUMMON = LegacyVersion.GetUpdateField(UnitField.UNIT_FIELD_SUMMON);
if (UNIT_FIELD_SUMMON >= 0 && updateFields.ContainsKey(UNIT_FIELD_SUMMON))
{
WowGuid128 guid = WorldClient.GetGuidValue(updateFields, UnitField.UNIT_FIELD_SUMMON).To128(this.GetSession().GameState);
if (!guid.IsEmpty())
{
pets.Guids.Add(guid);
}
}
this.SendPacketToClient(pets);
PetStableList stable = new PetStableList();
stable.StableMaster = packet.ReadGuid().To128(this.GetSession().GameState);
byte count = packet.ReadUInt8();
stable.NumStableSlots = packet.ReadUInt8();
for (byte i = 0; i < count; i++)
{
PetStableInfo pet = new PetStableInfo();
pet.PetNumber = packet.ReadUInt32();
pet.CreatureID = packet.ReadUInt32();
pet.ExperienceLevel = packet.ReadUInt32();
pet.PetName = packet.ReadCString();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V3_0_2_9056))
{
pet.LoyaltyLevel = (byte)packet.ReadUInt32();
}
pet.PetFlags = packet.ReadUInt8();
if (pet.PetFlags != 1)
```

---

### MSG_MINIMAP_PING

- Legacy value: 469 (0x01D5)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3456
- Fields:
  - `ReadGuid -> SenderGUID`

```csharp
{
MinimapPing ping = new MinimapPing();
ping.SenderGUID = packet.ReadGuid().To128(this.GetSession().GameState);
ping.Position = packet.ReadVector2();
this.SendPacketToClient(ping);
}
```

---

### MSG_MOVE_FALL_LAND

- Legacy value: 201 (0x00C9)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5420
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_FEATHER_FALL

- Legacy value: 688 (0x02B0)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5424
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_GRAVITY_CHNG

- Legacy value: 1234 (0x04D2)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5412
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_HEARTBEAT

- Legacy value: 238 (0x00EE)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5419
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_HOVER

- Legacy value: 247 (0x00F7)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5423
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_JUMP

- Legacy value: 187 (0x00BB)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5399
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_KNOCK_BACK

- Legacy value: 241 (0x00F1)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5437
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> JumpSinAngle`
  - `ReadFloat -> JumpCosAngle`
  - `ReadFloat -> JumpHorizontalSpeed`
  - `ReadFloat -> JumpVerticalSpeed`

```csharp
{
MoveUpdateKnockBack knockback = new MoveUpdateKnockBack();
knockback.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
knockback.MoveInfo = new MovementInfo();
knockback.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
knockback.MoveInfo.Flags = (uint)((MovementFlagWotLK)knockback.MoveInfo.Flags).CastFlags<MovementFlagModern>();
knockback.MoveInfo.JumpSinAngle = packet.ReadFloat();
knockback.MoveInfo.JumpCosAngle = packet.ReadFloat();
knockback.MoveInfo.JumpHorizontalSpeed = packet.ReadFloat();
knockback.MoveInfo.JumpVerticalSpeed = packet.ReadFloat();
knockback.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(knockback);
}
```

---

### MSG_MOVE_ROOT

- Legacy value: 236 (0x00EC)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5413
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_SET_FACING

- Legacy value: 218 (0x00DA)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5409
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_SET_FLIGHT_BACK_SPEED

- Legacy value: 896 (0x0380)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5645
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_FLIGHT_SPEED

- Legacy value: 894 (0x037E)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5646
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_PITCH

- Legacy value: 219 (0x00DB)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5410
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_SET_PITCH_RATE

- Legacy value: 1115 (0x045B)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5647
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_RUN_BACK_SPEED

- Legacy value: 207 (0x00CF)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5648
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_RUN_MODE

- Legacy value: 194 (0x00C2)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5406
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_SET_RUN_SPEED

- Legacy value: 205 (0x00CD)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5649
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_SWIM_BACK_SPEED

- Legacy value: 213 (0x00D5)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5650
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_SWIM_SPEED

- Legacy value: 211 (0x00D3)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5651
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_TURN_RATE

- Legacy value: 216 (0x00D8)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5652
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_SET_WALK_MODE

- Legacy value: 195 (0x00C3)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5407
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_SET_WALK_SPEED

- Legacy value: 209 (0x00D1)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5653
- Fields:
  - `ReadPackedGuid -> MoverGUID`
  - `ReadFloat -> Speed`

```csharp
{
string opcodeName = packet.GetUniversalOpcode(isModern: false).ToString().Replace("MSG_MOVE_SET", "SMSG_MOVE_UPDATE");
Opcode universalOpcode = Opcodes.GetUniversalOpcode(opcodeName);
MoveUpdateSpeed speed = new MoveUpdateSpeed(universalOpcode);
speed.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
speed.MoveInfo = new MovementInfo();
speed.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
MovementFlagModern newFlags = ((MovementFlagWotLK)speed.MoveInfo.Flags).CastFlags<MovementFlagModern>();
speed.MoveInfo.Flags = (uint)newFlags;
speed.MoveInfo.ValidateMovementInfo();
speed.Speed = packet.ReadFloat();
this.SendPacketToClient(speed);
bool flag = universalOpcode - 2477 <= Opcode.CMSG_ABANDON_NPE_RESPONSE;
if (flag && LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_0_1_6180))
{
Opcode flyOpcode = (Opcode)Enum.Parse(typeof(Opcode), universalOpcode.ToString().Replace("SWIM", "FLIGHT"));
MoveUpdateSpeed flySpeed = new MoveUpdateSpeed(flyOpcode);
flySpeed.MoverGUID = speed.MoverGUID;
flySpeed.MoveInfo = speed.MoveInfo;
flySpeed.Speed = speed.Speed;
this.SendPacketToClient(flySpeed);
}
}
```

---

### MSG_MOVE_START_ASCEND

- Legacy value: 857 (0x0359)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5396
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_BACKWARD

- Legacy value: 182 (0x00B6)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5391
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_DESCEND

- Legacy value: 935 (0x03A7)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5397
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_FORWARD

- Legacy value: 181 (0x00B5)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5390
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_PITCH_DOWN

- Legacy value: 192 (0x00C0)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5404
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_PITCH_UP

- Legacy value: 191 (0x00BF)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5403
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_STRAFE_LEFT

- Legacy value: 184 (0x00B8)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5393
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_STRAFE_RIGHT

- Legacy value: 185 (0x00B9)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5394
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_SWIM

- Legacy value: 202 (0x00CA)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5415
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_SWIM_CHEAT

- Legacy value: 833 (0x0341)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5417
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_TURN_LEFT

- Legacy value: 188 (0x00BC)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5400
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_START_TURN_RIGHT

- Legacy value: 189 (0x00BD)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5401
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP

- Legacy value: 183 (0x00B7)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5392
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP_ASCEND

- Legacy value: 858 (0x035A)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5398
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP_PITCH

- Legacy value: 193 (0x00C1)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5405
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP_STRAFE

- Legacy value: 186 (0x00BA)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5395
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP_SWIM

- Legacy value: 203 (0x00CB)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5416
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP_SWIM_CHEAT

- Legacy value: 834 (0x0342)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5418
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_STOP_TURN

- Legacy value: 190 (0x00BE)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5402
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_TELEPORT

- Legacy value: 197 (0x00C5)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5408
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_TELEPORT_ACK

- Legacy value: 199 (0x00C7)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5474
- Fields:
  - `ReadPackedGuid -> guid`
  - `ReadUInt32 -> MoveCounter`

```csharp
{
WowGuid128 guid = packet.ReadPackedGuid().To128(this.GetSession().GameState);
if (this.GetSession().GameState.IsInTaxiFlight && this.GetSession().GameState.CurrentPlayerGuid == guid)
{
ControlUpdate control = new ControlUpdate();
control.Guid = guid;
control.HasControl = true;
this.SendPacketToClient(control);
this.GetSession().GameState.IsInTaxiFlight = false;
}
MoveTeleport teleport = new MoveTeleport();
teleport.MoverGUID = guid;
teleport.MoveCounter = packet.ReadUInt32();
MovementInfo moveInfo = new MovementInfo();
moveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveInfo.Flags = (uint)((MovementFlagWotLK)moveInfo.Flags).CastFlags<MovementFlagModern>();
moveInfo.ValidateMovementInfo();
teleport.Position = moveInfo.Position;
teleport.Orientation = moveInfo.Orientation;
teleport.TransportGUID = moveInfo.TransportGuid;
if (moveInfo.TransportSeat > 0)
{
teleport.Vehicle = new VehicleTeleport();
teleport.Vehicle.VehicleSeatIndex = moveInfo.TransportSeat;
}
this.SendPacketToClient(teleport);
}
```

---

### MSG_MOVE_TOGGLE_COLLISION_CHEAT

- Legacy value: 217 (0x00D9)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5411
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_UNROOT

- Legacy value: 237 (0x00ED)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5414
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_UPDATE_CAN_FLY

- Legacy value: 941 (0x03AD)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5421
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_UPDATE_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY

- Legacy value: 842 (0x034A)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5422
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_MOVE_WATER_WALK

- Legacy value: 689 (0x02B1)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5425
- Fields:
  - `ReadPackedGuid -> MoverGUID`

```csharp
{
MoveUpdate moveUpdate = new MoveUpdate();
moveUpdate.MoverGUID = packet.ReadPackedGuid().To128(this.GetSession().GameState);
moveUpdate.MoveInfo = new MovementInfo();
moveUpdate.MoveInfo.ReadMovementInfoLegacy(packet, this.GetSession().GameState);
moveUpdate.MoveInfo.Flags = (uint)((MovementFlagWotLK)moveUpdate.MoveInfo.Flags).CastFlags<MovementFlagModern>();
moveUpdate.MoveInfo.ValidateMovementInfo();
this.SendPacketToClient(moveUpdate);
}
```

---

### MSG_PETITION_DECLINE

- Legacy value: 450 (0x01C2)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:6344
- Fields:
  - `ReadGuid -> guid`

```csharp
{
WowGuid128 guid = packet.ReadGuid().To128(this.GetSession().GameState);
string name = this.GetSession().GameState.GetPlayerName(guid);
if (!string.IsNullOrEmpty(name))
{
ChatPkt chat = new ChatPkt(this.GetSession(), ChatMessageTypeModern.System, name + " has declined your guild invitation.");
this.SendPacketToClient(chat);
}
}
```

---

### MSG_PETITION_RENAME

- Legacy value: 705 (0x02C1)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:6335
- Fields:
  - `ReadGuid -> PetitionGuid`
  - `ReadCString -> NewGuildName`

```csharp
{
PetitionRenameGuildResponse petition = new PetitionRenameGuildResponse();
petition.PetitionGuid = packet.ReadGuid().To128(this.GetSession().GameState);
petition.NewGuildName = packet.ReadCString();
this.SendPacketToClient(petition);
}
```

---

### MSG_PVP_LOG_DATA

- Legacy value: 736 (0x02E0)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:623, HermesProxy/World/Client/WorldClient.cs:665
- Fields:
  - `ReadBool`
  - `ReadUInt8 -> Winner`
  - `ReadInt32 -> count`
  - `ReadGuid -> PlayerGUID`
  - `ReadInt32 -> Rank`
  - `ReadUInt32 -> Kills`
  - `ReadUInt32 -> HonorKills`
  - `ReadUInt32 -> Deaths`
  - `ReadUInt32 -> ContributionPoints`
  - `ReadInt32 -> statsCount`
  - `ReadUInt32`
  - `ReadBool`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadUInt32`
  - `ReadCString`
  - `ReadBool`
  - `ReadUInt8 -> Winner`
  - `ReadInt32 -> count`
  - `ReadGuid -> PlayerGUID`
  - `ReadUInt32 -> Kills`
  - `ReadUInt32 -> HonorKills`
  - `ReadUInt32 -> Deaths`
  - `ReadUInt32 -> ContributionPoints`
  - `ReadBool -> Faction`
  - `ReadUInt32 -> DamageDone`
  - `ReadUInt32 -> HealingDone`
  - `ReadInt32 -> statsCount`
  - `ReadUInt32`

```csharp
{
PVPMatchStatisticsMessage pvp = new PVPMatchStatisticsMessage();
if (packet.ReadBool())
{
pvp.Winner = packet.ReadUInt8();
}
int count = packet.ReadInt32();
for (int i = 0; i < count; i++)
{
PVPMatchStatisticsMessage.PVPMatchPlayerStatistics player = new PVPMatchStatisticsMessage.PVPMatchPlayerStatistics();
player.PlayerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
player.Rank = packet.ReadInt32();
player.Kills = packet.ReadUInt32();
player.Honor = new PVPMatchStatisticsMessage.HonorData();
player.Honor.HonorKills = packet.ReadUInt32();
player.Honor.Deaths = packet.ReadUInt32();
player.Honor.ContributionPoints = packet.ReadUInt32();
int statsCount = packet.ReadInt32();
for (int j = 0; j < statsCount; j++)
{
player.Stats.Add(packet.ReadUInt32());
}
if (this.GetSession().GameState.CachedPlayers.TryGetValue(player.PlayerGUID, out var cache))
{
player.Sex = cache.SexId;
player.PlayerRace = cache.RaceId;
player.PlayerClass = cache.ClassId;
player.Faction = GameData.IsAllianceRace(cache.RaceId);
}
else
```

---

### MSG_QUERY_GUILD_BANK_TEXT

- Legacy value: 1034 (0x040A)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3896
- Fields:
  - `ReadUInt8 -> Tab`
  - `ReadCString -> Text`

```csharp
{
GuildBankTextQueryResult result = new GuildBankTextQueryResult();
result.Tab = packet.ReadUInt8();
result.Text = packet.ReadCString();
this.SendPacketToClient(result);
}
```

---

### MSG_QUERY_NEXT_MAIL_TIME

- Legacy value: 644 (0x0284)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:4549
- Fields:
  - `ReadFloat -> NextMailTime`
  - `ReadUInt32 -> count`
  - `ReadGuid -> SenderGuid`
  - `ReadInt32 -> AltSenderID`
  - `ReadInt32 -> AltSenderType`
  - `ReadInt32 -> StationeryID`
  - `ReadFloat -> TimeLeft`

```csharp
{
MailQueryNextTimeResult result = new MailQueryNextTimeResult();
result.NextMailTime = packet.ReadFloat();
if (LegacyVersion.RemovedInVersion(ClientVersionBuild.V2_3_0_7561))
{
if (result.NextMailTime == 0f)
{
MailQueryNextTimeResult.MailNextTimeEntry mail = new MailQueryNextTimeResult.MailNextTimeEntry();
mail.SenderGuid = this.GetSession().GameState.CurrentPlayerGuid;
mail.AltSenderID = 0;
mail.AltSenderType = 0;
mail.StationeryID = 41;
mail.TimeLeft = 3600f;
result.Mails.Add(mail);
}
}
else
{
uint count = packet.ReadUInt32();
for (int i = 0; i < count; i++)
{
MailQueryNextTimeResult.MailNextTimeEntry mail2 = new MailQueryNextTimeResult.MailNextTimeEntry();
mail2.SenderGuid = packet.ReadGuid().To128(this.GetSession().GameState);
mail2.AltSenderID = packet.ReadInt32();
mail2.AltSenderType = (sbyte)packet.ReadInt32();
mail2.StationeryID = packet.ReadInt32();
mail2.TimeLeft = packet.ReadFloat();
result.Mails.Add(mail2);
}
}
```

---

### MSG_QUEST_PUSH_RESULT

- Legacy value: 630 (0x0276)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:7427
- Fields:
  - `ReadGuid -> SenderGUID`
  - `ReadUInt8 -> Result`

```csharp
{
QuestPushResult quest = new QuestPushResult();
quest.SenderGUID = packet.ReadGuid().To128(this.GetSession().GameState);
quest.Result = (QuestPushReason)packet.ReadUInt8();
this.SendPacketToClient(quest);
}
```

---

### MSG_RAID_READY_CHECK

- Legacy value: 802 (0x0322)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2610, HermesProxy/World/Client/WorldClient.cs:2638
- Fields:
  - `ReadGuid -> Player`
  - `ReadBool -> IsReady`
  - `ReadGuid -> InitiatorGUID`

```csharp
{
if (!packet.CanRead())
{
ReadyCheckStarted ready = new ReadyCheckStarted();
ready.InitiatorGUID = this.GetSession().GameState.GetCurrentGroupLeader();
ready.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
ready.PartyGUID = this.GetSession().GameState.GetCurrentGroupGuid();
this.SendPacketToClient(ready);
return;
}
ReadyCheckResponse ready2 = new ReadyCheckResponse();
ready2.Player = packet.ReadGuid().To128(this.GetSession().GameState);
ready2.IsReady = packet.ReadBool();
ready2.PartyGUID = this.GetSession().GameState.GetCurrentGroupGuid();
this.SendPacketToClient(ready2);
this.GetSession().GameState.GroupReadyCheckResponses++;
if (this.GetSession().GameState.GroupReadyCheckResponses >= this.GetSession().GameState.GetCurrentGroupSize())
{
this.GetSession().GameState.GroupReadyCheckResponses = 0u;
ReadyCheckCompleted completed = new ReadyCheckCompleted();
completed.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
completed.PartyGUID = this.GetSession().GameState.GetCurrentGroupGuid();
this.SendPacketToClient(completed);
}
}
```

---

### MSG_RAID_READY_CHECK_CONFIRM

- Legacy value: 942 (0x03AE)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2648
- Fields:
  - `ReadGuid -> Player`
  - `ReadBool -> IsReady`

```csharp
{
ReadyCheckResponse ready = new ReadyCheckResponse();
ready.Player = packet.ReadGuid().To128(this.GetSession().GameState);
ready.IsReady = packet.ReadBool();
ready.PartyGUID = this.GetSession().GameState.GetCurrentGroupGuid();
this.SendPacketToClient(ready);
this.GetSession().GameState.GroupReadyCheckResponses++;
if (this.GetSession().GameState.GroupReadyCheckResponses >= this.GetSession().GameState.GetCurrentGroupSize())
{
this.GetSession().GameState.GroupReadyCheckResponses = 0u;
ReadyCheckCompleted completed = new ReadyCheckCompleted();
completed.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
completed.PartyGUID = this.GetSession().GameState.GetCurrentGroupGuid();
this.SendPacketToClient(completed);
}
}
```

---

### MSG_RAID_READY_CHECK_FINISHED

- Legacy value: 966 (0x03C6)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2667

```csharp
{
ReadyCheckCompleted ready = new ReadyCheckCompleted();
ready.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
ready.PartyGUID = this.GetSession().GameState.GetCurrentGroupGuid();
this.SendPacketToClient(ready);
}
```

---

### MSG_RAID_TARGET_UPDATE

- Legacy value: 801 (0x0321)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:2676
- Fields:
  - `ReadBool`
  - `ReadGuid -> guid`
  - `ReadGuid -> ChangedBy`
  - `ReadGuid -> Target`

```csharp
{
if (packet.ReadBool())
{
SendRaidTargetUpdateAll update = new SendRaidTargetUpdateAll();
update.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
while (packet.CanRead())
{
sbyte symbol = packet.ReadInt8();
WowGuid128 guid = packet.ReadGuid().To128(this.GetSession().GameState);
update.TargetIcons.Add(new Tuple<sbyte, WowGuid128>(symbol, guid));
}
this.SendPacketToClient(update);
return;
}
SendRaidTargetUpdateSingle update2 = new SendRaidTargetUpdateSingle();
update2.PartyIndex = this.GetSession().GameState.GetCurrentPartyIndex();
if (LegacyVersion.AddedInVersion(ClientVersionBuild.V3_0_2_9056))
{
update2.ChangedBy = packet.ReadGuid().To128(this.GetSession().GameState);
}
else
{
update2.ChangedBy = this.GetSession().GameState.CurrentPlayerGuid;
}
update2.Symbol = packet.ReadInt8();
update2.Target = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(update2);
}
```

---

### MSG_RANDOM_ROLL

- Legacy value: 507 (0x01FB)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3465
- Fields:
  - `ReadInt32 -> Min`
  - `ReadInt32 -> Max`
  - `ReadInt32 -> Result`
  - `ReadGuid -> Roller`

```csharp
{
RandomRoll roll = new RandomRoll();
roll.Min = packet.ReadInt32();
roll.Max = packet.ReadInt32();
roll.Result = packet.ReadInt32();
roll.Roller = packet.ReadGuid().To128(this.GetSession().GameState);
roll.RollerWowAccount = this.GetSession().GetGameAccountGuidForPlayer(roll.Roller);
this.SendPacketToClient(roll);
}
```

---

### MSG_SAVE_GUILD_EMBLEM

- Legacy value: 497 (0x01F1)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3804
- Fields:
  - `ReadUInt32 -> Error`

```csharp
{
PlayerSaveGuildEmblem emblem = new PlayerSaveGuildEmblem();
emblem.Error = (GuildEmblemError)packet.ReadUInt32();
this.SendPacketToClient(emblem);
}
```

---

### MSG_SET_DUNGEON_DIFFICULTY

- Legacy value: 809 (0x0329)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:5069
- Fields:
  - `ReadInt32 -> difficultyId`
  - `ReadInt32`
  - `ReadInt32`

```csharp
{
DungeonDifficultySet difficulty = new DungeonDifficultySet();
int difficultyId = packet.ReadInt32();
difficulty.DifficultyID = (byte)Enum.Parse(typeof(DifficultyModern), ((DifficultyLegacy)difficultyId/*cast due to .constrained prefix*/).ToString());
packet.ReadInt32();
packet.ReadInt32();
this.SendPacketToClient(difficulty);
}
```

---

### MSG_TABARDVENDOR_ACTIVATE

- Legacy value: 498 (0x01F2)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:3796
- Fields:
  - `ReadGuid -> DesignerGUID`

```csharp
{
PlayerTabardVendorActivate activate = new PlayerTabardVendorActivate();
activate.DesignerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
this.SendPacketToClient(activate);
}
```

---

### MSG_TALENT_WIPE_CONFIRM

- Legacy value: 682 (0x02AA)
- Modern value: N/A
- Handler: HermesProxy/World/Client/WorldClient.cs:6085
- Fields:
  - `ReadGuid -> TrainerGUID`
  - `ReadUInt32 -> Cost`

```csharp
{
RespecWipeConfirm respec = new RespecWipeConfirm();
respec.TrainerGUID = packet.ReadGuid().To128(this.GetSession().GameState);
respec.Cost = packet.ReadUInt32();
this.SendPacketToClient(respec);
}
```

---

## ServerPacket Structures

These are the Write() methods that serialize data for the modern client.

### AIReaction

- File: HermesProxy/World/Server/Packets/AIReaction.cs
- Fields:
  - `WriteUInt32(this.Reaction)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.UnitGUID);
base._worldPacket.WriteUInt32(this.Reaction);
}
```

---

### AccountDataTimes

- File: HermesProxy/World/Server/Packets/AccountDataTimes.cs
- Fields:
  - `WriteInt64(this.ServerTime)`
  - `WriteInt64(accounttime)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PlayerGuid);
base._worldPacket.WriteInt64(this.ServerTime);
long[] accountTimes = this.AccountTimes;
foreach (long accounttime in accountTimes)
{
base._worldPacket.WriteInt64(accounttime);
}
}
```

---

### AchievementEarnedPkt

- File: HermesProxy/World/Server/Packets/AchievementEarnedPkt.cs
- Fields:
  - `WriteUInt32(this.AchievementID)`
  - `WriteUInt32(this.EarnerNativeRealm)`
  - `WriteUInt32(this.EarnerVirtualRealm)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Sender);
base._worldPacket.WritePackedGuid128(this.Earner);
base._worldPacket.WriteUInt32(this.AchievementID);
base._worldPacket.WritePackedTime(this.Time);
base._worldPacket.WriteUInt32(this.EarnerNativeRealm);
base._worldPacket.WriteUInt32(this.EarnerVirtualRealm);
base._worldPacket.WriteBit(this.Initial);
base._worldPacket.FlushBits();
}
```

---

### ActivateTaxiReplyPkt

- File: HermesProxy/World/Server/Packets/ActivateTaxiReplyPkt.cs

```csharp
{
base._worldPacket.WriteBits(this.Reply, 4);
base._worldPacket.FlushBits();
}
```

---

### AllAccountCriteria

- File: HermesProxy/World/Server/Packets/AllAccountCriteria.cs
- Fields:
  - `WriteInt32(this.Progress.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Progress.Count);
foreach (CriteriaProgressPkt item in this.Progress)
{
item.Write(base._worldPacket);
}
}
```

---

### AllAchievementData

- File: HermesProxy/World/Server/Packets/AllAchievementData.cs
- Fields:
  - `WriteInt32(this.Earned.Count)`
  - `WriteInt32(this.Progress.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Earned.Count);
base._worldPacket.WriteInt32(this.Progress.Count);
foreach (EarnedAchievement earned in this.Earned)
{
earned.Write(base._worldPacket);
}
foreach (CriteriaProgressPkt progress in this.Progress)
{
progress.Write(base._worldPacket);
}
}
```

---

### AreaSpiritHealerTime

- File: HermesProxy/World/Server/Packets/AreaSpiritHealerTime.cs
- Fields:
  - `WriteUInt32(this.TimeLeft)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.HealerGuid);
base._worldPacket.WriteUInt32(this.TimeLeft);
}
```

---

### AreaTriggerMessage

- File: HermesProxy/World/Server/Packets/AreaTriggerMessage.cs
- Fields:
  - `WriteUInt32(this.AreaTriggerID)`

```csharp
{
base._worldPacket.WriteUInt32(this.AreaTriggerID);
}
```

---

### ArenaTeamCommandResult

- File: HermesProxy/World/Server/Packets/ArenaTeamCommandResult.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`
  - `WriteString(this.TeamName)`
  - `WriteString(this.PlayerName)`

```csharp
{
base._worldPacket.WriteUInt8((byte)this.Action);
base._worldPacket.WriteUInt8((byte)this.Error);
base._worldPacket.WriteBits(this.TeamName.GetByteCount(), 7);
base._worldPacket.WriteBits(this.PlayerName.GetByteCount(), 6);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.TeamName);
base._worldPacket.WriteString(this.PlayerName);
}
```

---

### ArenaTeamEvent

- File: HermesProxy/World/Server/Packets/ArenaTeamEvent.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteString(this.Param1)`
  - `WriteString(this.Param2)`
  - `WriteString(this.Param3)`

```csharp
{
base._worldPacket.WriteUInt8((byte)this.Event);
base._worldPacket.WriteBits(this.Param1.GetByteCount(), 9);
base._worldPacket.WriteBits(this.Param2.GetByteCount(), 9);
base._worldPacket.WriteBits(this.Param3.GetByteCount(), 9);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Param1);
base._worldPacket.WriteString(this.Param2);
base._worldPacket.WriteString(this.Param3);
}
```

---

### ArenaTeamInvite

- File: HermesProxy/World/Server/Packets/ArenaTeamInvite.cs
- Fields:
  - `WriteUInt32(this.PlayerVirtualAddress)`
  - `WriteString(this.PlayerName)`
  - `WriteString(this.TeamName)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PlayerGuid);
base._worldPacket.WriteUInt32(this.PlayerVirtualAddress);
base._worldPacket.WritePackedGuid128(this.TeamGuid);
base._worldPacket.WriteBits(this.PlayerName.GetByteCount(), 6);
base._worldPacket.WriteBits(this.TeamName.GetByteCount(), 7);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.PlayerName);
base._worldPacket.WriteString(this.TeamName);
}
```

---

### ArenaTeamQueryResponse

- File: HermesProxy/World/Server/Packets/ArenaTeamQueryResponse.cs
- Fields:
  - `WriteUInt32(this.TeamId)`

```csharp
{
base._worldPacket.WriteUInt32(this.TeamId);
base._worldPacket.WriteBit(this.Emblem != null);
base._worldPacket.FlushBits();
if (this.Emblem != null)
{
this.Emblem.Write(base._worldPacket);
}
}
```

---

### ArenaTeamRosterResponse

- File: HermesProxy/World/Server/Packets/ArenaTeamRosterResponse.cs
- Fields:
  - `WriteUInt32(this.TeamId)`
  - `WriteUInt32(this.TeamSize)`
  - `WriteUInt32(this.TeamPlayed)`
  - `WriteUInt32(this.TeamWins)`
  - `WriteUInt32(this.SeasonPlayed)`
  - `WriteUInt32(this.SeasonWins)`
  - `WriteUInt32(this.TeamRating)`
  - `WriteUInt32(this.PlayerRating)`
  - `WriteInt32(this.Members.Count)`

```csharp
{
base._worldPacket.WriteUInt32(this.TeamId);
base._worldPacket.WriteUInt32(this.TeamSize);
base._worldPacket.WriteUInt32(this.TeamPlayed);
base._worldPacket.WriteUInt32(this.TeamWins);
base._worldPacket.WriteUInt32(this.SeasonPlayed);
base._worldPacket.WriteUInt32(this.SeasonWins);
base._worldPacket.WriteUInt32(this.TeamRating);
base._worldPacket.WriteUInt32(this.PlayerRating);
base._worldPacket.WriteInt32(this.Members.Count);
if (ModernVersion.AddedInClassicVersion(1, 14, 2, 2, 5, 3))
{
base._worldPacket.WriteBit(this.UnkBit);
base._worldPacket.FlushBits();
}
foreach (ArenaTeamMember member2 in this.Members)
{
member2.Write(base._worldPacket);
}
}
```

---

### AttackSwingError

- File: HermesProxy/World/Server/Packets/AttackSwingError.cs

```csharp
{
base._worldPacket.WriteBits((uint)this.Reason, 3);
base._worldPacket.FlushBits();
}
```

---

### AttackerStateUpdate

- File: HermesProxy/World/Server/Packets/AttackerStateUpdate.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteInt32(this.Damage)`
  - `WriteInt32(this.OriginalDamage)`
  - `WriteInt32(this.OverDamage)`
  - `WriteUInt8((byte)`
  - `WriteUInt32(subDmg.SchoolMask)`
  - `WriteFloat(subDmg.FloatDamage)`
  - `WriteInt32(subDmg.IntDamage)`
  - `WriteInt32(subDmg.Absorbed)`
  - `WriteInt32(subDmg.Resisted)`
  - `WriteUInt8(this.VictimState)`
  - `WriteInt32(this.AttackerState)`
  - `WriteUInt32(this.MeleeSpellID)`
  - `WriteInt32(this.BlockAmount)`
  - `WriteInt32(this.RageGained)`
  - `WriteUInt32(this.UnkState.State1)`
  - `WriteFloat(this.UnkState.State2)`
  - `WriteFloat(this.UnkState.State3)`
  - `WriteFloat(this.UnkState.State4)`
  - `WriteFloat(this.UnkState.State5)`
  - `WriteFloat(this.UnkState.State6)`
  - `WriteFloat(this.UnkState.State7)`
  - `WriteFloat(this.UnkState.State8)`
  - `WriteFloat(this.UnkState.State9)`
  - `WriteFloat(this.UnkState.State10)`
  - `WriteFloat(this.UnkState.State11)`
  - `WriteUInt32(this.UnkState.State12)`
  - `WriteFloat(this.Unk)`
  - `WriteUInt8((byte)`
  - `WriteUInt8(this.ContentTuning.TargetLevel)`
  - `WriteUInt8(this.ContentTuning.Expansion)`
  - `WriteInt16(this.ContentTuning.PlayerLevelDelta)`
  - `WriteFloat(this.ContentTuning.PlayerItemLevel)`
  - `WriteFloat(this.ContentTuning.TargetItemLevel)`
  - `WriteUInt32(this.ContentTuning.ScalingHealthItemLevelCurveID)`
  - `WriteUInt32((uint)`
  - `WriteInt32(0)`
  - `WriteInt32(0)`
  - `WriteUInt32(attackRoundInfo.GetSize()`
  - `WriteBytes(attackRoundInfo)`

```csharp
{
WorldPacket attackRoundInfo = new WorldPacket();
attackRoundInfo.WriteUInt32((uint)this.HitInfo);
attackRoundInfo.WritePackedGuid128(this.AttackerGUID);
attackRoundInfo.WritePackedGuid128(this.VictimGUID);
attackRoundInfo.WriteInt32(this.Damage);
attackRoundInfo.WriteInt32(this.OriginalDamage);
attackRoundInfo.WriteInt32(this.OverDamage);
attackRoundInfo.WriteUInt8((byte)this.SubDmg.Count);
foreach (SubDamage subDmg in this.SubDmg)
{
attackRoundInfo.WriteUInt32(subDmg.SchoolMask);
attackRoundInfo.WriteFloat(subDmg.FloatDamage);
attackRoundInfo.WriteInt32(subDmg.IntDamage);
if (this.HitInfo.HasAnyFlag(HitInfo.FullAbsorb | HitInfo.PartialAbsorb))
{
attackRoundInfo.WriteInt32(subDmg.Absorbed);
}
if (this.HitInfo.HasAnyFlag(HitInfo.FullResist | HitInfo.PartialResist))
{
attackRoundInfo.WriteInt32(subDmg.Resisted);
}
}
attackRoundInfo.WriteUInt8(this.VictimState);
attackRoundInfo.WriteInt32(this.AttackerState);
```

---

### AuctionClosedNotification

- File: HermesProxy/World/Server/Packets/AuctionClosedNotification.cs
- Fields:
  - `WriteFloat(this.ProceedsMailDelay)`

```csharp
{
this.Info.Write(base._worldPacket);
base._worldPacket.WriteFloat(this.ProceedsMailDelay);
base._worldPacket.WriteBit(this.Sold);
base._worldPacket.FlushBits();
}
```

---

### AuctionCommandResult

- File: HermesProxy/World/Server/Packets/AuctionCommandResult.cs
- Fields:
  - `WriteUInt32(this.AuctionID)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteUInt64(this.MinIncrement)`
  - `WriteUInt64(this.Money)`
  - `WriteUInt32(this.DesiredDelay)`

```csharp
{
base._worldPacket.WriteUInt32(this.AuctionID);
base._worldPacket.WriteInt32((int)this.Command);
base._worldPacket.WriteInt32((int)this.ErrorCode);
base._worldPacket.WriteInt32((int)this.BagResult);
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteUInt64(this.MinIncrement);
base._worldPacket.WriteUInt64(this.Money);
base._worldPacket.WriteUInt32(this.DesiredDelay);
}
```

---

### AuctionHelloResponse

- File: HermesProxy/World/Server/Packets/AuctionHelloResponse.cs
- Fields:
  - `WriteUInt32(this.AuctionHouseID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteUInt32(this.AuctionHouseID);
base._worldPacket.WriteBit(this.OpenForBusiness);
base._worldPacket.FlushBits();
}
```

---

### AuctionListItemsResult

- File: HermesProxy/World/Server/Packets/AuctionListItemsResult.cs
- Fields:
  - `WriteInt32(this.Items.Count)`
  - `WriteInt32(this.TotalItemsCount)`
  - `WriteUInt32(this.DesiredDelay)`
  - `WriteBool(this.OnlyUsable)`

```csharp
{
base._worldPacket.WriteInt32(this.Items.Count);
base._worldPacket.WriteInt32(this.TotalItemsCount);
base._worldPacket.WriteUInt32(this.DesiredDelay);
if (this.Items.Count > 0)
{
base._worldPacket.WriteBool(this.OnlyUsable);
}
foreach (AuctionItem item in this.Items)
{
item.Write(base._worldPacket);
}
}
```

---

### AuctionListMyItemsResult

- File: HermesProxy/World/Server/Packets/AuctionListMyItemsResult.cs
- Fields:
  - `WriteInt32(this.Items.Count)`
  - `WriteInt32(this.TotalItemsCount)`
  - `WriteUInt32(this.DesiredDelay)`

```csharp
{
base._worldPacket.WriteInt32(this.Items.Count);
base._worldPacket.WriteInt32(this.TotalItemsCount);
base._worldPacket.WriteUInt32(this.DesiredDelay);
foreach (AuctionItem item in this.Items)
{
item.Write(base._worldPacket);
}
}
```

---

### AuctionOutbidNotification

- File: HermesProxy/World/Server/Packets/AuctionOutbidNotification.cs
- Fields:
  - `WriteUInt64(this.BidAmount)`
  - `WriteUInt64(this.MinIncrement)`

```csharp
{
this.Info.Write(base._worldPacket);
base._worldPacket.WriteUInt64(this.BidAmount);
base._worldPacket.WriteUInt64(this.MinIncrement);
}
```

---

### AuctionOwnerBidNotification

- File: HermesProxy/World/Server/Packets/AuctionOwnerBidNotification.cs
- Fields:
  - `WriteUInt64(this.MinIncrement)`

```csharp
{
this.Info.Write(base._worldPacket);
base._worldPacket.WriteUInt64(this.MinIncrement);
base._worldPacket.WritePackedGuid128(this.Bidder);
}
```

---

### AuctionWonNotification

- File: HermesProxy/World/Server/Packets/AuctionWonNotification.cs

```csharp
{
this.Info.Write(base._worldPacket);
}
```

---

### AuraUpdate

- File: HermesProxy/World/Server/Packets/AuraUpdate.cs

```csharp
{
base._worldPacket.WriteBit(this.UpdateAll);
base._worldPacket.WriteBits(this.Auras.Count, 9);
foreach (AuraInfo aura2 in this.Auras)
{
aura2.Write(base._worldPacket);
}
base._worldPacket.WritePackedGuid128(this.UnitGUID);
}
```

---

### AuthChallenge

- File: HermesProxy/World/Server/Packets/AuthChallenge.cs
- Fields:
  - `WriteBytes(this.DosChallenge)`
  - `WriteBytes(this.Challenge)`
  - `WriteUInt8(this.DosZeroBits)`

```csharp
{
base._worldPacket.WriteBytes(this.DosChallenge);
base._worldPacket.WriteBytes(this.Challenge);
base._worldPacket.WriteUInt8(this.DosZeroBits);
}
```

---

### AuthResponse

- File: HermesProxy/World/Server/Packets/AuthResponse.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt32(this.SuccessInfo.VirtualRealmAddress)`
  - `WriteInt32(this.SuccessInfo.VirtualRealms.Count)`
  - `WriteUInt32(this.SuccessInfo.TimeRested)`
  - `WriteUInt8(this.SuccessInfo.ActiveExpansionLevel)`
  - `WriteUInt8(this.SuccessInfo.AccountExpansionLevel)`
  - `WriteUInt32(this.SuccessInfo.TimeSecondsUntilPCKick)`
  - `WriteInt32(this.SuccessInfo.AvailableClasses.Count)`
  - `WriteInt32(this.SuccessInfo.Templates.Count)`
  - `WriteUInt32(this.SuccessInfo.CurrencyID)`
  - `WriteInt64(this.SuccessInfo.Time)`
  - `WriteUInt8(raceClassAvailability.RaceID)`
  - `WriteInt32(raceClassAvailability.Classes.Count)`
  - `WriteUInt8(classAvailability.ClassID)`
  - `WriteUInt8(classAvailability.ActiveExpansionLevel)`
  - `WriteUInt8(classAvailability.AccountExpansionLevel)`
  - `WriteUInt8(0)`
  - `WriteUInt32(this.SuccessInfo.GameTimeInfo.BillingPlan)`
  - `WriteUInt32(this.SuccessInfo.GameTimeInfo.TimeRemain)`
  - `WriteUInt32(this.SuccessInfo.GameTimeInfo.Unknown735)`
  - `WriteUInt16(this.SuccessInfo.NumPlayersHorde.Value)`
  - `WriteUInt16(this.SuccessInfo.NumPlayersAlliance.Value)`
  - `WriteInt32(this.SuccessInfo.ExpansionTrialExpiration.Value)`
  - `WriteUInt32(templat.TemplateSetId)`
  - `WriteInt32(templat.Classes.Count)`
  - `WriteUInt8(templateClass.ClassID)`
  - `WriteUInt8((byte)`
  - `WriteString(templat.Name)`
  - `WriteString(templat.Description)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Result);
base._worldPacket.WriteBit(this.SuccessInfo != null);
base._worldPacket.WriteBit(this.WaitInfo != null);
base._worldPacket.FlushBits();
if (this.SuccessInfo != null)
{
base._worldPacket.WriteUInt32(this.SuccessInfo.VirtualRealmAddress);
base._worldPacket.WriteInt32(this.SuccessInfo.VirtualRealms.Count);
base._worldPacket.WriteUInt32(this.SuccessInfo.TimeRested);
base._worldPacket.WriteUInt8(this.SuccessInfo.ActiveExpansionLevel);
base._worldPacket.WriteUInt8(this.SuccessInfo.AccountExpansionLevel);
base._worldPacket.WriteUInt32(this.SuccessInfo.TimeSecondsUntilPCKick);
base._worldPacket.WriteInt32(this.SuccessInfo.AvailableClasses.Count);
base._worldPacket.WriteInt32(this.SuccessInfo.Templates.Count);
base._worldPacket.WriteUInt32(this.SuccessInfo.CurrencyID);
base._worldPacket.WriteInt64(this.SuccessInfo.Time);
foreach (RaceClassAvailability raceClassAvailability in this.SuccessInfo.AvailableClasses)
{
base._worldPacket.WriteUInt8(raceClassAvailability.RaceID);
base._worldPacket.WriteInt32(raceClassAvailability.Classes.Count);
foreach (ClassAvailability classAvailability in raceClassAvailability.Classes)
{
base._worldPacket.WriteUInt8(classAvailability.ClassID);
base._worldPacket.WriteUInt8(classAvailability.ActiveExpansionLevel);
```

---

### AvailableHotfixes

- File: HermesProxy/World/Server/Packets/AvailableHotfixes.cs
- Fields:
  - `WriteUInt32(this.VirtualRealmAddress)`
  - `WriteInt32(GameData.Hotfixes.Count)`

```csharp
{
base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
base._worldPacket.WriteInt32(GameData.Hotfixes.Count);
foreach (KeyValuePair<uint, HotfixRecord> hotfix2 in GameData.Hotfixes)
{
hotfix2.Value.WriteAvailable(base._worldPacket);
}
}
```

---

### BattlePetJournalLockAcquired

- File: HermesProxy/World/Server/Packets/BattlePetJournalLockAcquired.cs

```csharp
{
}
```

---

### BattlefieldList

- File: HermesProxy/World/Server/Packets/BattlefieldList.cs
- Fields:
  - `WriteInt32(this.Verification)`
  - `WriteUInt32(this.BattlemasterListID)`
  - `WriteUInt8(this.MinLevel)`
  - `WriteUInt8(this.MaxLevel)`
  - `WriteInt32(this.BattlefieldInstances.Count)`
  - `WriteInt32(field)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.BattlemasterGuid);
base._worldPacket.WriteInt32(this.Verification);
base._worldPacket.WriteUInt32(this.BattlemasterListID);
base._worldPacket.WriteUInt8(this.MinLevel);
base._worldPacket.WriteUInt8(this.MaxLevel);
base._worldPacket.WriteInt32(this.BattlefieldInstances.Count);
foreach (int field in this.BattlefieldInstances)
{
base._worldPacket.WriteInt32(field);
}
base._worldPacket.WriteBit(this.PvpAnywhere);
base._worldPacket.WriteBit(this.HasRandomWinToday);
base._worldPacket.FlushBits();
}
```

---

### BattlefieldStatusActive

- File: HermesProxy/World/Server/Packets/BattlefieldStatusActive.cs
- Fields:
  - `WriteUInt32(this.Mapid)`
  - `WriteUInt32(this.ShutdownTimer)`
  - `WriteUInt32(this.StartTimer)`

```csharp
{
this.Hdr.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.Mapid);
base._worldPacket.WriteUInt32(this.ShutdownTimer);
base._worldPacket.WriteUInt32(this.StartTimer);
base._worldPacket.WriteBit(this.ArenaFaction != 0);
base._worldPacket.WriteBit(this.LeftEarly);
base._worldPacket.FlushBits();
}
```

---

### BattlefieldStatusFailed

- File: HermesProxy/World/Server/Packets/BattlefieldStatusFailed.cs
- Fields:
  - `WriteUInt64(queueID)`
  - `WriteInt32(this.Reason)`

```csharp
{
this.Ticket.Write(base._worldPacket);
ulong queueID = this.BattlefieldListId | 0x1F10000000000000L;
base._worldPacket.WriteUInt64(queueID);
base._worldPacket.WriteInt32(this.Reason);
base._worldPacket.WritePackedGuid128(this.ClientID);
}
```

---

### BattlefieldStatusNeedConfirmation

- File: HermesProxy/World/Server/Packets/BattlefieldStatusNeedConfirmation.cs
- Fields:
  - `WriteUInt32(this.Mapid)`
  - `WriteUInt32(this.Timeout)`
  - `WriteUInt8(this.Role)`

```csharp
{
this.Hdr.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.Mapid);
base._worldPacket.WriteUInt32(this.Timeout);
base._worldPacket.WriteUInt8(this.Role);
}
```

---

### BattlefieldStatusQueued

- File: HermesProxy/World/Server/Packets/BattlefieldStatusQueued.cs
- Fields:
  - `WriteUInt32(this.AverageWaitTime)`
  - `WriteUInt32(this.WaitTime)`
  - `WriteInt32(this.Unk254)`

```csharp
{
this.Hdr.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.AverageWaitTime);
base._worldPacket.WriteUInt32(this.WaitTime);
if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 3, 2, 5, 4))
{
base._worldPacket.WriteInt32(this.Unk254);
}
base._worldPacket.WriteBit(this.AsGroup);
base._worldPacket.WriteBit(this.EligibleForMatchmaking);
base._worldPacket.WriteBit(this.SuspendedQueue);
base._worldPacket.FlushBits();
}
```

---

### BattlegroundInit

- File: HermesProxy/World/Server/Packets/BattlegroundInit.cs
- Fields:
  - `WriteUInt32(this.Milliseconds)`
  - `WriteUInt16(this.BattlegroundPoints)`

```csharp
{
base._worldPacket.WriteUInt32(this.Milliseconds);
base._worldPacket.WriteUInt16(this.BattlegroundPoints);
}
```

---

### BattlegroundPlayerLeftOrJoined

- File: HermesProxy/World/Server/Packets/BattlegroundPlayerLeftOrJoined.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### BattlegroundPlayerPositions

- File: HermesProxy/World/Server/Packets/BattlegroundPlayerPositions.cs
- Fields:
  - `WriteInt32(this.FlagCarriers.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.FlagCarriers.Count);
foreach (BattlegroundPlayerPosition flagCarrier in this.FlagCarriers)
{
flagCarrier.Write(base._worldPacket);
}
}
```

---

### BattlenetNotification

- File: HermesProxy/World/Server/Packets/BattlenetNotification.cs
- Fields:
  - `WriteUInt32(this.Data.GetSize()`
  - `WriteBytes(this.Data)`

```csharp
{
this.Method.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.Data.GetSize());
base._worldPacket.WriteBytes(this.Data);
}
```

---

### BattlenetResponse

- File: HermesProxy/World/Server/Packets/BattlenetResponse.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt32(this.Data.GetSize()`
  - `WriteBytes(this.Data)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Status);
this.Method.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.Data.GetSize());
base._worldPacket.WriteBytes(this.Data);
}
```

---

### BindPointUpdate

- File: HermesProxy/World/Server/Packets/BindPointUpdate.cs
- Fields:
  - `WriteVector3`
  - `WriteUInt32(this.BindMapID)`
  - `WriteUInt32(this.BindAreaID)`

```csharp
{
base._worldPacket.WriteVector3(this.BindPosition);
base._worldPacket.WriteUInt32(this.BindMapID);
base._worldPacket.WriteUInt32(this.BindAreaID);
}
```

---

### BinderConfirm

- File: HermesProxy/World/Server/Packets/BinderConfirm.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### BuyFailed

- File: HermesProxy/World/Server/Packets/BuyFailed.cs
- Fields:
  - `WriteUInt32(this.Muid)`
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.VendorGUID);
base._worldPacket.WriteUInt32(this.Muid);
base._worldPacket.WriteUInt8((byte)this.Reason);
}
```

---

### BuySucceeded

- File: HermesProxy/World/Server/Packets/BuySucceeded.cs
- Fields:
  - `WriteUInt32(this.Muid)`
  - `WriteInt32(this.NewQuantity)`
  - `WriteUInt32(this.QuantityBought)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.VendorGUID);
base._worldPacket.WriteUInt32(this.Muid);
base._worldPacket.WriteInt32(this.NewQuantity);
base._worldPacket.WriteUInt32(this.QuantityBought);
}
```

---

### CanDuelResult

- File: HermesProxy/World/Server/Packets/CanDuelResult.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WriteBit(this.Result);
base._worldPacket.FlushBits();
}
```

---

### CancelAutoRepeat

- File: HermesProxy/World/Server/Packets/CancelAutoRepeat.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### CancelCombat

- File: HermesProxy/World/Server/Packets/CancelCombat.cs

```csharp
{
}
```

---

### CastFailed

- File: HermesProxy/World/Server/Packets/CastFailed.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.SpellXSpellVisualID)`
  - `WriteUInt32(this.Reason)`
  - `WriteInt32(this.FailedArg1)`
  - `WriteInt32(this.FailedArg2)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CastID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
base._worldPacket.WriteUInt32(this.Reason);
base._worldPacket.WriteInt32(this.FailedArg1);
base._worldPacket.WriteInt32(this.FailedArg2);
}
```

---

### ChangeRealmTicketResponse

- File: HermesProxy/World/Server/Packets/ChangeRealmTicketResponse.cs
- Fields:
  - `WriteUInt32(this.Token)`
  - `WriteUInt32(this.Ticket.GetSize()`
  - `WriteBytes(this.Ticket)`

```csharp
{
base._worldPacket.WriteUInt32(this.Token);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.WriteUInt32(this.Ticket.GetSize());
base._worldPacket.WriteBytes(this.Ticket);
}
```

---

### ChannelListResponse

- File: HermesProxy/World/Server/Packets/ChannelListResponse.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteInt32(this.Members.Count)`
  - `WriteString(this.ChannelName)`
  - `WriteUInt32(player.VirtualRealmAddress)`
  - `WriteUInt8(player.Flags)`

```csharp
{
base._worldPacket.WriteBit(this.Display);
base._worldPacket.WriteBits(this.ChannelName.GetByteCount(), 7);
base._worldPacket.WriteUInt32((uint)this.ChannelFlags);
base._worldPacket.WriteInt32(this.Members.Count);
base._worldPacket.WriteString(this.ChannelName);
foreach (ChannelPlayer player in this.Members)
{
base._worldPacket.WritePackedGuid128(player.Guid);
base._worldPacket.WriteUInt32(player.VirtualRealmAddress);
base._worldPacket.WriteUInt8(player.Flags);
}
}
```

---

### ChannelNotifyJoined

- File: HermesProxy/World/Server/Packets/ChannelNotifyJoined.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteInt32(this.ChatChannelID)`
  - `WriteUInt64(this.InstanceID)`
  - `WriteString(this.Channel)`
  - `WriteString(this.ChannelWelcomeMsg)`

```csharp
{
base._worldPacket.WriteBits(this.Channel.GetByteCount(), 7);
base._worldPacket.WriteBits(this.ChannelWelcomeMsg.GetByteCount(), 11);
base._worldPacket.WriteUInt32((uint)this.ChannelFlags);
base._worldPacket.WriteInt32(this.ChatChannelID);
base._worldPacket.WriteUInt64(this.InstanceID);
base._worldPacket.WritePackedGuid128(this.ChannelGUID);
base._worldPacket.WriteString(this.Channel);
base._worldPacket.WriteString(this.ChannelWelcomeMsg);
}
```

---

### ChannelNotifyLeft

- File: HermesProxy/World/Server/Packets/ChannelNotifyLeft.cs
- Fields:
  - `WriteInt32(this.ChatChannelID)`
  - `WriteString(this.Channel)`

```csharp
{
base._worldPacket.WriteBits(this.Channel.GetByteCount(), 7);
base._worldPacket.WriteBit(this.Suspended);
base._worldPacket.WriteInt32(this.ChatChannelID);
base._worldPacket.WriteString(this.Channel);
}
```

---

### CharacterLoginFailed

- File: HermesProxy/World/Server/Packets/CharacterLoginFailed.cs
- Fields:
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WriteUInt8((byte)this.Code);
}
```

---

### CharacterRenameResult

- File: HermesProxy/World/Server/Packets/CharacterRenameResult.cs
- Fields:
  - `WriteUInt8(this.Result)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteUInt8(this.Result);
base._worldPacket.WriteBit(this.Guid != null);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
base._worldPacket.FlushBits();
if (this.Guid != null)
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
base._worldPacket.WriteString(this.Name);
}
```

---

### ChatPkt

- File: HermesProxy/World/Server/Packets/ChatPkt.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteUInt32(this._Language)`
  - `WriteUInt32(this.TargetVirtualAddress)`
  - `WriteUInt32(this.SenderVirtualAddress)`
  - `WriteInt32((int)`
  - `WriteFloat(this.DisplayTime)`
  - `WriteInt32(this.SpellID)`
  - `WriteString(this.SenderName)`
  - `WriteString(this.TargetName)`
  - `WriteString(this.Prefix)`
  - `WriteString(this.Channel)`
  - `WriteString(this.ChatText)`
  - `WriteUInt32(this.Unused_801.Value)`

```csharp
{
base._worldPacket.WriteUInt8((byte)this.SlashCmd);
base._worldPacket.WriteUInt32(this._Language);
base._worldPacket.WritePackedGuid128(this.SenderGUID);
base._worldPacket.WritePackedGuid128(this.SenderGuildGUID);
base._worldPacket.WritePackedGuid128(this.SenderAccountGUID);
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WriteUInt32(this.TargetVirtualAddress);
base._worldPacket.WriteUInt32(this.SenderVirtualAddress);
base._worldPacket.WriteInt32((int)this.AchievementID);
base._worldPacket.WriteFloat(this.DisplayTime);
base._worldPacket.WriteInt32(this.SpellID);
base._worldPacket.WriteBits(this.SenderName.GetByteCount(), 11);
base._worldPacket.WriteBits(this.TargetName.GetByteCount(), 11);
base._worldPacket.WriteBits(this.Prefix.GetByteCount(), 5);
base._worldPacket.WriteBits(this.Channel.GetByteCount(), 7);
base._worldPacket.WriteBits(this.ChatText.GetByteCount(), 12);
base._worldPacket.WriteBits((uint)this._ChatFlags, 15);
base._worldPacket.WriteBit(this.HideChatLog);
base._worldPacket.WriteBit(this.FakeSenderName);
base._worldPacket.WriteBit(this.Unused_801.HasValue);
base._worldPacket.WriteBit(this.ChannelGUID != null);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.SenderName);
base._worldPacket.WriteString(this.TargetName);
```

---

### ChatPlayerNotfound

- File: HermesProxy/World/Server/Packets/ChatPlayerNotfound.cs
- Fields:
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
base._worldPacket.WriteString(this.Name);
}
```

---

### ChatServerMessage

- File: HermesProxy/World/Server/Packets/ChatServerMessage.cs
- Fields:
  - `WriteInt32(this.MessageID)`
  - `WriteString(this.StringParam)`

```csharp
{
base._worldPacket.WriteInt32(this.MessageID);
base._worldPacket.WriteBits(this.StringParam.GetByteCount(), 11);
base._worldPacket.WriteString(this.StringParam);
}
```

---

### ClearCooldown

- File: HermesProxy/World/Server/Packets/ClearCooldown.cs
- Fields:
  - `WriteUInt32(this.SpellID)`

```csharp
{
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteBit(this.ClearOnHold);
base._worldPacket.WriteBit(this.IsPet);
base._worldPacket.FlushBits();
}
```

---

### ClientCacheVersion

- File: HermesProxy/World/Server/Packets/ClientCacheVersion.cs
- Fields:
  - `WriteUInt32(this.CacheVersion)`

```csharp
{
base._worldPacket.WriteUInt32(this.CacheVersion);
}
```

---

### CoinRemoved

- File: HermesProxy/World/Server/Packets/CoinRemoved.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
}
```

---

### ConnectTo

- File: HermesProxy/World/Server/Packets/ConnectTo.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteBytes(this.Payload.Where.IPv4)`
  - `WriteBytes(this.Payload.Where.IPv6)`
  - `WriteString(this.Payload.Where.NameSocket)`
  - `WriteBytes(this.Payload.Signature, (uint)`
  - `WriteBytes(whereBuffer)`
  - `WriteUInt16(this.Payload.Port)`
  - `WriteUInt32((uint)`
  - `WriteUInt8(this.Con)`
  - `WriteUInt64(this.Key)`

```csharp
{
ByteBuffer whereBuffer = new ByteBuffer();
whereBuffer.WriteUInt8((byte)this.Payload.Where.Type);
switch (this.Payload.Where.Type)
{
case AddressType.IPv4:
whereBuffer.WriteBytes(this.Payload.Where.IPv4);
break;
case AddressType.IPv6:
whereBuffer.WriteBytes(this.Payload.Where.IPv6);
break;
case AddressType.NamedSocket:
whereBuffer.WriteString(this.Payload.Where.NameSocket);
break;
}
Sha256 hash = new Sha256();
hash.Process(whereBuffer.GetData(), (int)whereBuffer.GetSize());
hash.Process((uint)this.Payload.Where.Type);
hash.Finish(BitConverter.GetBytes(this.Payload.Port));
this.Payload.Signature = RsaCrypt.RSA.SignHash(hash.Digest, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1).Reverse().ToArray();
base._worldPacket.WriteBytes(this.Payload.Signature, (uint)this.Payload.Signature.Length);
base._worldPacket.WriteBytes(whereBuffer);
base._worldPacket.WriteUInt16(this.Payload.Port);
base._worldPacket.WriteUInt32((uint)this.Serial);
base._worldPacket.WriteUInt8(this.Con);
```

---

### ConnectionStatus

- File: HermesProxy/World/Server/Packets/ConnectionStatus.cs

```csharp
{
base._worldPacket.WriteBits(this.State, 2);
base._worldPacket.WriteBit(this.SuppressNotification);
base._worldPacket.FlushBits();
}
```

---

### ConquestFormulaConstants

- File: HermesProxy/World/Server/Packets/ConquestFormulaConstants.cs
- Fields:
  - `WriteInt32(this.PvpMinCPPerWeek)`
  - `WriteInt32(this.PvpMaxCPPerWeek)`
  - `WriteFloat(this.PvpCPBaseCoefficient)`
  - `WriteFloat(this.PvpCPExpCoefficient)`
  - `WriteFloat(this.PvpCPNumerator)`

```csharp
{
base._worldPacket.WriteInt32(this.PvpMinCPPerWeek);
base._worldPacket.WriteInt32(this.PvpMaxCPPerWeek);
base._worldPacket.WriteFloat(this.PvpCPBaseCoefficient);
base._worldPacket.WriteFloat(this.PvpCPExpCoefficient);
base._worldPacket.WriteFloat(this.PvpCPNumerator);
}
```

---

### ContactList

- File: HermesProxy/World/Server/Packets/ContactList.cs
- Fields:
  - `WriteUInt32((uint)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Flags);
base._worldPacket.WriteBits(this.Contacts.Count, 8);
base._worldPacket.FlushBits();
foreach (ContactInfo contact in this.Contacts)
{
contact.Write(base._worldPacket);
}
}
```

---

### ControlUpdate

- File: HermesProxy/World/Server/Packets/ControlUpdate.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteBit(this.HasControl);
base._worldPacket.FlushBits();
}
```

---

### CooldownCheat

- File: HermesProxy/World/Server/Packets/CooldownCheat.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### CooldownEvent

- File: HermesProxy/World/Server/Packets/CooldownEvent.cs
- Fields:
  - `WriteUInt32(this.SpellID)`

```csharp
{
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteBit(this.IsPet);
base._worldPacket.FlushBits();
}
```

---

### CorpseLocation

- File: HermesProxy/World/Server/Packets/CorpseLocation.cs
- Fields:
  - `WriteInt32(this.ActualMapID)`
  - `WriteVector3`
  - `WriteInt32(this.MapID)`

```csharp
{
base._worldPacket.WriteBit(this.Valid);
base._worldPacket.FlushBits();
base._worldPacket.WritePackedGuid128(this.Player);
base._worldPacket.WriteInt32(this.ActualMapID);
base._worldPacket.WriteVector3(this.Position);
base._worldPacket.WriteInt32(this.MapID);
base._worldPacket.WritePackedGuid128(this.Transport);
}
```

---

### CorpseReclaimDelay

- File: HermesProxy/World/Server/Packets/CorpseReclaimDelay.cs
- Fields:
  - `WriteUInt32(this.Remaining)`

```csharp
{
base._worldPacket.WriteUInt32(this.Remaining);
}
```

---

### CreateChar

- File: HermesProxy/World/Server/Packets/CreateChar.cs
- Fields:
  - `WriteUInt8(this.Code)`

```csharp
{
base._worldPacket.WriteUInt8(this.Code);
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### CriteriaUpdatePkt

- File: HermesProxy/World/Server/Packets/CriteriaUpdatePkt.cs
- Fields:
  - `WriteUInt32(this.CriteriaID)`
  - `WriteUInt64(this.Quantity)`
  - `WriteUInt32(0)`
  - `WriteUInt32(this.Flags)`
  - `WriteInt64(this.ElapsedTime)`
  - `WriteUInt32(this.CreationTime)`

```csharp
{
base._worldPacket.WriteUInt32(this.CriteriaID);
base._worldPacket.WriteUInt64(this.Quantity);
base._worldPacket.WritePackedGuid128(this.PlayerGUID);
base._worldPacket.WriteUInt32(0); // Unused_10_1_5
base._worldPacket.WriteUInt32(this.Flags);
base._worldPacket.WritePackedTime(this.CurrentTime);
base._worldPacket.WriteInt64(this.ElapsedTime); // Duration<Seconds> = int64
base._worldPacket.WriteUInt32(this.CreationTime); // Timestamp<> = uint32
base._worldPacket.WriteBit(false); // RafAcceptanceID
base._worldPacket.FlushBits();
}
```

---

### DBReply

- File: HermesProxy/World/Server/Packets/DBReply.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt32(this.RecordID)`
  - `WriteUInt32(this.Timestamp)`
  - `WriteUInt32(this.Data.GetSize()`
  - `WriteBytes(this.Data.GetData()`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.TableHash);
base._worldPacket.WriteUInt32(this.RecordID);
base._worldPacket.WriteUInt32(this.Timestamp);
base._worldPacket.WriteBits((byte)this.Status, 3);
base._worldPacket.WriteUInt32(this.Data.GetSize());
base._worldPacket.WriteBytes(this.Data.GetData());
}
```

---

### DeathReleaseLoc

- File: HermesProxy/World/Server/Packets/DeathReleaseLoc.cs
- Fields:
  - `WriteInt32(this.MapID)`
  - `WriteVector3`

```csharp
{
base._worldPacket.WriteInt32(this.MapID);
base._worldPacket.WriteVector3(this.Location);
}
```

---

### DefenseMessage

- File: HermesProxy/World/Server/Packets/DefenseMessage.cs
- Fields:
  - `WriteUInt32(this.ZoneID)`
  - `WriteString(this.MessageText)`

```csharp
{
base._worldPacket.WriteUInt32(this.ZoneID);
base._worldPacket.WriteBits(this.MessageText.GetByteCount(), 12);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.MessageText);
}
```

---

### DeleteChar

- File: HermesProxy/World/Server/Packets/DeleteChar.cs
- Fields:
  - `WriteUInt8(this.Code)`

```csharp
{
base._worldPacket.WriteUInt8(this.Code);
}
```

---

### DfJoinResult

- File: HermesProxy/World/Server/Packets/LfgSmsgPackets.cs
- Fields:
  - `WriteUInt8(this.Result)`
  - `WriteUInt8(this.ResultDetail)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(0u)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(slot.Slot)`
  - `WriteUInt32(slot.Reason)`
  - `WriteInt32(slot.SubReason1)`
  - `WriteInt32(slot.SubReason2)`
  - `WriteUInt32(slot.SoftLock)`

```csharp
{
this.Ticket.Write(base._worldPacket);
base._worldPacket.WriteUInt8(this.Result);
base._worldPacket.WriteUInt8(this.ResultDetail);
base._worldPacket.WriteUInt32((uint)this.BlackList.Count);
base._worldPacket.WriteUInt32(0u); // BlackListNames count
foreach (DfJoinBlackList entry in this.BlackList)
{
base._worldPacket.WriteBit(entry.PlayerGuid != null);
base._worldPacket.WriteUInt32((uint)entry.Slots.Count);
if (entry.PlayerGuid != null)
{
base._worldPacket.WritePackedGuid128(entry.PlayerGuid);
}
foreach (DfJoinBlackListSlot slot in entry.Slots)
{
base._worldPacket.WriteUInt32(slot.Slot);
base._worldPacket.WriteUInt32(slot.Reason);
base._worldPacket.WriteInt32(slot.SubReason1);
base._worldPacket.WriteInt32(slot.SubReason2);
base._worldPacket.WriteUInt32(slot.SoftLock);
}
}
}
```

---

### DfProposalUpdate

- File: HermesProxy/World/Server/Packets/LfgSmsgPackets.cs
- Fields:
  - `WriteUInt64(this.InstanceID)`
  - `WriteUInt32(this.ProposalID)`
  - `WriteUInt32(this.Slot)`
  - `WriteUInt32(this.CompletedMask)`
  - `WriteUInt32(this.EncounterMask)`
  - `WriteUInt32((uint)`
  - `WriteUInt8(0)`
  - `WriteUInt8(player.Roles)`

```csharp
{
this.Ticket.Write(base._worldPacket);
base._worldPacket.WriteUInt64(this.InstanceID);
base._worldPacket.WriteUInt32(this.ProposalID);
base._worldPacket.WriteUInt32(this.Slot);
base._worldPacket.WriteInt8(this.State);
base._worldPacket.WriteUInt32(this.CompletedMask);
base._worldPacket.WriteUInt32(this.EncounterMask);
base._worldPacket.WriteUInt32((uint)this.Players.Count);
base._worldPacket.WriteUInt8(0); // Unused
base._worldPacket.WriteBit(this.ValidCompletedMask);
base._worldPacket.WriteBit(this.ProposalSilent);
base._worldPacket.WriteBit(this.IsRequeue);
base._worldPacket.FlushBits();
foreach (DfProposalPlayer player in this.Players)
{
base._worldPacket.WriteUInt8(player.Roles);
base._worldPacket.WriteBit(player.Me);
base._worldPacket.WriteBit(player.SameParty);
base._worldPacket.WriteBit(player.MyParty);
base._worldPacket.WriteBit(player.Responded);
base._worldPacket.WriteBit(player.Accepted);
base._worldPacket.FlushBits();
}
}
```

---

### DfQueueStatus

- File: HermesProxy/World/Server/Packets/LfgSmsgPackets.cs
- Fields:
  - `WriteUInt32(this.Slot)`
  - `WriteUInt32(this.AvgWaitTimeMe)`
  - `WriteUInt32(this.AvgWaitTime)`
  - `WriteUInt32(this.AvgWaitTimeByRole[i])`
  - `WriteUInt8(this.LastNeeded[i])`
  - `WriteUInt32(this.QueuedTime)`

```csharp
{
this.Ticket.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.Slot);
base._worldPacket.WriteUInt32(this.AvgWaitTimeMe);
base._worldPacket.WriteUInt32(this.AvgWaitTime);
for (int i = 0; i < 3; i++)
{
base._worldPacket.WriteUInt32(this.AvgWaitTimeByRole[i]);
base._worldPacket.WriteUInt8(this.LastNeeded[i]);
}
base._worldPacket.WriteUInt32(this.QueuedTime);
}
```

---

### DfUpdateStatus

- File: HermesProxy/World/Server/Packets/LfgSmsgPackets.cs
- Fields:
  - `WriteUInt8(this.SubType)`
  - `WriteUInt8(this.Reason)`
  - `WriteUInt32((uint)`
  - `WriteUInt8(this.RequestedRoles)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(this.QueueMapID)`
  - `WriteUInt32(slot)`

```csharp
{
this.Ticket.Write(base._worldPacket);
base._worldPacket.WriteUInt8(this.SubType);
base._worldPacket.WriteUInt8(this.Reason);
base._worldPacket.WriteUInt32((uint)this.Slots.Count);
base._worldPacket.WriteUInt8(this.RequestedRoles);
base._worldPacket.WriteUInt32((uint)this.SuspendedPlayers.Count);
base._worldPacket.WriteUInt32(this.QueueMapID);
foreach (uint slot in this.Slots)
{
base._worldPacket.WriteUInt32(slot);
}
foreach (WowGuid128 guid in this.SuspendedPlayers)
{
base._worldPacket.WritePackedGuid128(guid);
}
base._worldPacket.WriteBit(this.IsParty);
base._worldPacket.WriteBit(this.NotifyUI);
base._worldPacket.WriteBit(this.Joined);
base._worldPacket.WriteBit(this.LfgJoined);
base._worldPacket.WriteBit(this.Queued);
base._worldPacket.WriteBit(false); // Unused
base._worldPacket.FlushBits();
}
```

---

### DisplayToast

- File: HermesProxy/World/Server/Packets/DisplayToast.cs
- Fields:
  - `WriteUInt64(this.Quantity)`
  - `WriteUInt8(this.DisplayToastMethod)`
  - `WriteUInt32(this.QuestID)`
  - `WriteUInt32(this.SpecializationID)`
  - `WriteUInt32(this.ItemQuantity)`
  - `WriteUInt32(this.CurrencyID)`

```csharp
{
base._worldPacket.WriteUInt64(this.Quantity);
base._worldPacket.WriteUInt8(this.DisplayToastMethod);
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WriteBit(this.Mailed);
base._worldPacket.WriteBits(this.Type, 2);
if (this.Type == 0)
{
base._worldPacket.WriteBit(this.BonusRoll);
base._worldPacket.FlushBits();
this.ItemReward.Write(base._worldPacket);
base._worldPacket.WriteUInt32(this.SpecializationID);
base._worldPacket.WriteUInt32(this.ItemQuantity);
}
else
{
base._worldPacket.FlushBits();
}
if (this.Type == 1)
{
base._worldPacket.WriteUInt32(this.CurrencyID);
}
}
```

---

### DuelComplete

- File: HermesProxy/World/Server/Packets/DuelComplete.cs

```csharp
{
base._worldPacket.WriteBit(this.Started);
base._worldPacket.FlushBits();
}
```

---

### DuelCountdown

- File: HermesProxy/World/Server/Packets/DuelCountdown.cs
- Fields:
  - `WriteUInt32(this.Countdown)`

```csharp
{
base._worldPacket.WriteUInt32(this.Countdown);
}
```

---

### DuelInBounds

- File: HermesProxy/World/Server/Packets/DuelInBounds.cs

```csharp
{
}
```

---

### DuelOutOfBounds

- File: HermesProxy/World/Server/Packets/DuelOutOfBounds.cs

```csharp
{
}
```

---

### DuelRequested

- File: HermesProxy/World/Server/Packets/DuelRequested.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.ArbiterGUID);
base._worldPacket.WritePackedGuid128(this.RequestedByGUID);
base._worldPacket.WritePackedGuid128(this.RequestedByWowAccount);
}
```

---

### DuelWinner

- File: HermesProxy/World/Server/Packets/DuelWinner.cs
- Fields:
  - `WriteUInt32(this.BeatenVirtualRealmAddress)`
  - `WriteUInt32(this.WinnerVirtualRealmAddress)`
  - `WriteString(this.BeatenName)`
  - `WriteString(this.WinnerName)`

```csharp
{
base._worldPacket.WriteBits(this.BeatenName.GetByteCount(), 6);
base._worldPacket.WriteBits(this.WinnerName.GetByteCount(), 6);
base._worldPacket.WriteBit(this.Fled);
base._worldPacket.WriteUInt32(this.BeatenVirtualRealmAddress);
base._worldPacket.WriteUInt32(this.WinnerVirtualRealmAddress);
base._worldPacket.WriteString(this.BeatenName);
base._worldPacket.WriteString(this.WinnerName);
}
```

---

### DungeonDifficultySet

- File: HermesProxy/World/Server/Packets/DungeonDifficultySet.cs
- Fields:
  - `WriteInt32(this.DifficultyID)`

```csharp
{
base._worldPacket.WriteInt32(this.DifficultyID);
}
```

---

### DurabilityDamageDeath

- File: HermesProxy/World/Server/Packets/DurabilityDamageDeath.cs
- Fields:
  - `WriteUInt32(this.Percent)`

```csharp
{
base._worldPacket.WriteUInt32(this.Percent);
}
```

---

### EmoteMessage

- File: HermesProxy/World/Server/Packets/EmoteMessage.cs
- Fields:
  - `WriteUInt32(this.EmoteID)`
  - `WriteInt32(this.SpellVisualKitIDs.Count)`
  - `WriteInt32(this.SequenceVariation)`
  - `WriteUInt32(id)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteUInt32(this.EmoteID);
base._worldPacket.WriteInt32(this.SpellVisualKitIDs.Count);
base._worldPacket.WriteInt32(this.SequenceVariation);
foreach (uint id in this.SpellVisualKitIDs)
{
base._worldPacket.WriteUInt32(id);
}
}
```

---

### EmptyAccountHeirloomUpdate

- File: HermesProxy/World/Server/Packets/EmptyAccountHeirloomUpdate.cs
- Fields:
  - `WriteInt32(0)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`

```csharp
{
base._worldPacket.WriteBit(bit: true);
base._worldPacket.FlushBits();
base._worldPacket.WriteInt32(0);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt32(0u);
}
```

---

### EmptyAccountMountUpdate

- File: HermesProxy/World/Server/Packets/EmptyAccountMountUpdate.cs
- Fields:
  - `WriteUInt32(0u)`

```csharp
{
base._worldPacket.WriteBit(bit: true);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.FlushBits();
}
```

---

### EmptyAccountToyUpdate

- File: HermesProxy/World/Server/Packets/EmptyAccountToyUpdate.cs
- Fields:
  - `WriteInt32(0)`
  - `WriteInt32(0)`
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteBit(bit: true);
base._worldPacket.FlushBits();
base._worldPacket.WriteInt32(0);
base._worldPacket.WriteInt32(0);
base._worldPacket.WriteInt32(0);
}
```

---

### EmptyActiveGlyphs

- File: HermesProxy/World/Server/Packets/EmptyActiveGlyphs.cs
- Fields:
  - `WriteUInt32(0u)`

```csharp
{
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteBit(bit: true);
base._worldPacket.FlushBits();
}
```

---

### EmptyAllAccountCriteria

- File: HermesProxy/World/Server/Packets/EmptyAllAccountCriteria.cs
- Fields:
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteInt32(0);
}
```

---

### EmptyAllAchievementData

- File: HermesProxy/World/Server/Packets/EmptyAllAchievementData.cs
- Fields:
  - `WriteInt32(0)`
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteInt32(0);
base._worldPacket.WriteInt32(0);
}
```

---

### EmptyEquipmentSetList

- File: HermesProxy/World/Server/Packets/EmptyEquipmentSetList.cs
- Fields:
  - `WriteUInt32(0u)`

```csharp
{
base._worldPacket.WriteUInt32(0u);
}
```

---

### EmptyInitWorldStates

- File: HermesProxy/World/Server/Packets/EmptyInitWorldStates.cs
- Fields:
  - `WriteUInt32(this.MapId)`
  - `WriteInt32(this.ZoneId)`
  - `WriteInt32(this.AreaId)`
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapId);
base._worldPacket.WriteInt32(this.ZoneId);
base._worldPacket.WriteInt32(this.AreaId);
base._worldPacket.WriteInt32(0);
}
```

---

### EmptySetupCurrency

- File: HermesProxy/World/Server/Packets/EmptySetupCurrency.cs
- Fields:
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteInt32(0);
}
```

---

### EmptySpellCharges

- File: HermesProxy/World/Server/Packets/EmptySpellCharges.cs
- Fields:
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteInt32(0);
}
```

---

### EmptySpellHistory

- File: HermesProxy/World/Server/Packets/EmptySpellHistory.cs
- Fields:
  - `WriteInt32(0)`

```csharp
{
base._worldPacket.WriteInt32(0);
}
```

---

### EmptyTalentData

- File: HermesProxy/World/Server/Packets/EmptyTalentData.cs
- Fields:
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`
  - `WriteUInt32(1u)`
  - `WriteUInt8(0)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(0)`

```csharp
{
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt8(0);
base._worldPacket.WriteUInt32(1u);
base._worldPacket.WriteUInt8(0);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt8(0);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt8(0);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.FlushBits();
}
```

---

### EnchantmentLog

- File: HermesProxy/World/Server/Packets/EnchantmentLog.cs
- Fields:
  - `WriteInt32(this.ItemID)`
  - `WriteInt32(this.Enchantment)`
  - `WriteInt32(this.EnchantSlot)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Owner);
base._worldPacket.WritePackedGuid128(this.Caster);
base._worldPacket.WritePackedGuid128(this.ItemGUID);
base._worldPacket.WriteInt32(this.ItemID);
base._worldPacket.WriteInt32(this.Enchantment);
base._worldPacket.WriteInt32(this.EnchantSlot);
}
```

---

### EnterEncryptedMode

- File: HermesProxy/World/Server/Packets/EnterEncryptedMode.cs

```csharp
{
if (ModernVersion.ExpansionVersion >= 3)
{
this.WriteEd25519();
}
else
{
this.WriteRSA();
}
}
```

---

### EnumCharactersResult

- File: HermesProxy/World/Server/Packets/EnumCharactersResult.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteInt32(this.MaxCharacterLevel)`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(this.DisabledClassesMask.Value)`
  - `WriteInt32(this.Characters.Count)`
  - `WriteInt32(this.MaxCharacterLevel)`
  - `WriteInt32(this.RaceUnlockData.Count)`
  - `WriteInt32(this.UnlockedConditionalAppearances.Count)`
  - `WriteUInt32(this.DisabledClassesMask.Value)`

```csharp
{
base._worldPacket.WriteBit(this.Success);
base._worldPacket.WriteBit(this.IsDeletedCharacters);
base._worldPacket.WriteBit(this.IsNewPlayerRestrictionSkipped);
base._worldPacket.WriteBit(this.IsNewPlayerRestricted);
base._worldPacket.WriteBit(this.IsNewPlayer);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBit(this.DisabledClassesMask.HasValue);
base._worldPacket.WriteUInt32((uint)this.Characters.Count);
base._worldPacket.WriteInt32(this.MaxCharacterLevel);
base._worldPacket.WriteUInt32((uint)this.RaceUnlockData.Count);
base._worldPacket.WriteUInt32((uint)this.UnlockedConditionalAppearances.Count);
base._worldPacket.WriteUInt32(0u);
if (this.DisabledClassesMask.HasValue)
{
base._worldPacket.WriteUInt32(this.DisabledClassesMask.Value);
}
foreach (UnlockedConditionalAppearance unlockedConditionalAppearance2 in this.UnlockedConditionalAppearances)
{
unlockedConditionalAppearance2.Write(base._worldPacket);
}
foreach (CharacterInfo charInfo in this.Characters)
{
```

---

### EnvironmentalDamageLog

- File: HermesProxy/World/Server/Packets/EnvironmentalDamageLog.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteInt32(this.Amount)`
  - `WriteInt32(this.Resisted)`
  - `WriteInt32(this.Absorbed)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Victim);
base._worldPacket.WriteUInt8((byte)this.Type);
base._worldPacket.WriteInt32(this.Amount);
base._worldPacket.WriteInt32(this.Resisted);
base._worldPacket.WriteInt32(this.Absorbed);
base._worldPacket.WriteBit(this.LogData != null);
base._worldPacket.FlushBits();
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
}
```

---

### ExplorationExperience

- File: HermesProxy/World/Server/Packets/ExplorationExperience.cs
- Fields:
  - `WriteUInt32(this.AreaID)`
  - `WriteUInt32(this.Experience)`

```csharp
{
base._worldPacket.WriteUInt32(this.AreaID);
base._worldPacket.WriteUInt32(this.Experience);
}
```

---

### FeatureSystemStatus

- File: HermesProxy/World/Server/Packets/FeatureSystemStatus.cs
- Fields:
  - `WriteUInt8(this.ComplaintStatus)`
  - `WriteUInt32(this.ScrollOfResurrectionRequestsRemaining)`
  - `WriteUInt32(this.ScrollOfResurrectionMaxRequestsPerDay)`
  - `WriteUInt32(this.CfgRealmID)`
  - `WriteInt32(this.CfgRealmRecID)`
  - `WriteUInt32(this.RAFSystem.MaxRecruits)`
  - `WriteUInt32(this.RAFSystem.MaxRecruitMonths)`
  - `WriteUInt32(this.RAFSystem.MaxRecruitmentUses)`
  - `WriteUInt32(this.RAFSystem.DaysInCycle)`
  - `WriteUInt32(this.TwitterPostThrottleLimit)`
  - `WriteUInt32(this.TwitterPostThrottleCooldown)`
  - `WriteUInt32(this.TokenPollTimeSeconds)`
  - `WriteUInt32(this.KioskSessionMinutes)`
  - `WriteInt64(this.TokenBalanceAmount)`
  - `WriteUInt32(this.BpayStoreProductDeliveryDelay)`
  - `WriteUInt32(this.ClubsPresenceUpdateTimer)`
  - `WriteUInt32(this.HiddenUIClubsPresenceUpdateTimer)`
  - `WriteInt32(this.ActiveSeason)`
  - `WriteInt32(this.GameRuleValues.Count)`
  - `WriteInt16(this.MaxPlayerNameQueriesPerPacket)`
  - `WriteInt16(this.PlayerNameQueryTelemetryInterval)`
  - `WriteFloat(this.QuickJoinConfig.ToastDuration)`
  - `WriteFloat(this.QuickJoinConfig.DelayDuration)`
  - `WriteFloat(this.QuickJoinConfig.QueueMultiplier)`
  - `WriteFloat(this.QuickJoinConfig.PlayerMultiplier)`
  - `WriteFloat(this.QuickJoinConfig.PlayerFriendValue)`
  - `WriteFloat(this.QuickJoinConfig.PlayerGuildValue)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleInitialThreshold)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleDecayTime)`
  - `WriteFloat(this.QuickJoinConfig.ThrottlePrioritySpike)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleMinThreshold)`
  - `WriteFloat(this.QuickJoinConfig.ThrottlePvPPriorityNormal)`
  - `WriteFloat(this.QuickJoinConfig.ThrottlePvPPriorityLow)`
  - `WriteFloat(this.QuickJoinConfig.ThrottlePvPHonorThreshold)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityDefault)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityAbove)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleLfgListPriorityBelow)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleLfgListIlvlScalingAbove)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleLfgListIlvlScalingBelow)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleRfPriorityAbove)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleRfIlvlScalingAbove)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleDfMaxItemLevel)`
  - `WriteFloat(this.QuickJoinConfig.ThrottleDfBestPriority)`
  - `WriteInt32(this.SessionAlert.Delay)`
  - `WriteInt32(this.SessionAlert.Period)`
  - `WriteInt32(this.SessionAlert.DisplayTime)`
  - `WriteInt32(this.RaceClassExpansionLevels.Count)`
  - `WriteUInt8(this.RaceClassExpansionLevels[i])`

```csharp
{
if (ModernVersion.ExpansionVersion >= 3)
{
this.WriteWotLK();
return;
}
base._worldPacket.WriteUInt8(this.ComplaintStatus);
base._worldPacket.WriteUInt32(this.ScrollOfResurrectionRequestsRemaining);
base._worldPacket.WriteUInt32(this.ScrollOfResurrectionMaxRequestsPerDay);
base._worldPacket.WriteUInt32(this.CfgRealmID);
base._worldPacket.WriteInt32(this.CfgRealmRecID);
base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruits);
base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruitMonths);
base._worldPacket.WriteUInt32(this.RAFSystem.MaxRecruitmentUses);
base._worldPacket.WriteUInt32(this.RAFSystem.DaysInCycle);
base._worldPacket.WriteUInt32(this.TwitterPostThrottleLimit);
base._worldPacket.WriteUInt32(this.TwitterPostThrottleCooldown);
base._worldPacket.WriteUInt32(this.TokenPollTimeSeconds);
base._worldPacket.WriteUInt32(this.KioskSessionMinutes);
base._worldPacket.WriteInt64(this.TokenBalanceAmount);
base._worldPacket.WriteUInt32(this.BpayStoreProductDeliveryDelay);
base._worldPacket.WriteUInt32(this.ClubsPresenceUpdateTimer);
base._worldPacket.WriteUInt32(this.HiddenUIClubsPresenceUpdateTimer);
if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
{
```

---

### FeatureSystemStatusGlueScreen

- File: HermesProxy/World/Server/Packets/FeatureSystemStatusGlueScreen.cs
- Fields:
  - `WriteUInt32(this.TokenPollTimeSeconds)`
  - `WriteUInt32(this.KioskSessionMinutes)`
  - `WriteInt64(this.TokenBalanceAmount)`
  - `WriteInt32(this.MaxCharactersPerRealm)`
  - `WriteInt32(this.LiveRegionCharacterCopySourceRegions.Count)`
  - `WriteUInt32(this.BpayStoreProductDeliveryDelay)`
  - `WriteInt32(this.ActiveCharacterUpgradeBoostType)`
  - `WriteInt32(this.ActiveClassTrialBoostType)`
  - `WriteInt32(this.MinimumExpansionLevel)`
  - `WriteInt32(this.MaximumExpansionLevel)`
  - `WriteInt32(this.ActiveSeason)`
  - `WriteInt32(this.GameRuleValues.Count)`
  - `WriteInt16(this.MaxPlayerNameQueriesPerPacket)`
  - `WriteInt16(this.PlayerNameQueryTelemetryInterval)`
  - `WriteInt32(sourceRegion)`

```csharp
{
if (ModernVersion.ExpansionVersion >= 3)
{
this.WriteWotlk343();
return;
}
base._worldPacket.WriteBit(this.BpayStoreEnabled);
base._worldPacket.WriteBit(this.BpayStoreAvailable);
base._worldPacket.WriteBit(this.BpayStoreDisabledByParentalControls);
base._worldPacket.WriteBit(this.CharUndeleteEnabled);
base._worldPacket.WriteBit(this.CommerceSystemEnabled);
base._worldPacket.WriteBit(this.Unk14);
base._worldPacket.WriteBit(this.WillKickFromWorld);
base._worldPacket.WriteBit(this.IsExpansionPreorderInStore);
base._worldPacket.WriteBit(this.KioskModeEnabled);
base._worldPacket.WriteBit(this.CompetitiveModeEnabled);
base._worldPacket.WriteBit(this.TrialBoostEnabled);
base._worldPacket.WriteBit(this.TokenBalanceEnabled);
base._worldPacket.WriteBit(this.LiveRegionCharacterListEnabled);
base._worldPacket.WriteBit(this.LiveRegionCharacterCopyEnabled);
base._worldPacket.WriteBit(this.LiveRegionAccountCopyEnabled);
base._worldPacket.WriteBit(this.LiveRegionKeyBindingsCopyEnabled);
base._worldPacket.WriteBit(this.Unknown901CheckoutRelated);
base._worldPacket.WriteBit(this.EuropaTicketSystemStatus != null);
base._worldPacket.FlushBits();
```

---

### FishEscaped

- File: HermesProxy/World/Server/Packets/FishEscaped.cs

```csharp
{
}
```

---

### FishNotHooked

- File: HermesProxy/World/Server/Packets/FishNotHooked.cs

```csharp
{
}
```

---

### FriendStatusPkt

- File: HermesProxy/World/Server/Packets/FriendStatusPkt.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteUInt32(this.VirtualRealmAddress)`
  - `WriteUInt8((byte)`
  - `WriteUInt32(this.AreaID)`
  - `WriteUInt32(this.Level)`
  - `WriteUInt32((uint)`
  - `WriteString(this.Notes)`

```csharp
{
base._worldPacket.WriteUInt8((byte)this.FriendResult);
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WritePackedGuid128(this.WowAccountGuid);
base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
base._worldPacket.WriteUInt8((byte)this.Status);
base._worldPacket.WriteUInt32(this.AreaID);
base._worldPacket.WriteUInt32(this.Level);
base._worldPacket.WriteUInt32((uint)this.ClassID);
base._worldPacket.WriteBits(this.Notes.GetByteCount(), 10);
base._worldPacket.WriteBit(this.Mobile);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Notes);
}
```

---

### GameObjectCustomAnim

- File: HermesProxy/World/Server/Packets/GameObjectCustomAnim.cs
- Fields:
  - `WriteUInt32(this.CustomAnim)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.ObjectGUID);
base._worldPacket.WriteUInt32(this.CustomAnim);
base._worldPacket.WriteBit(this.PlayAsDespawn);
base._worldPacket.FlushBits();
}
```

---

### GameObjectDespawn

- File: HermesProxy/World/Server/Packets/GameObjectDespawn.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.ObjectGUID);
}
```

---

### GameObjectResetState

- File: HermesProxy/World/Server/Packets/GameObjectResetState.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.ObjectGUID);
}
```

---

### GenerateRandomCharacterNameResult

- File: HermesProxy/World/Server/Packets/GenerateRandomCharacterNameResult.cs
- Fields:
  - `WriteBool(this.Success)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteBool(this.Success);
base._worldPacket.WriteBits(this.Name.Length, 6);
base._worldPacket.WriteString(this.Name);
}
```

---

### GetAccountCharacterListResult

- File: HermesProxy/World/Server/Packets/GetAccountCharacterListResult.cs
- Fields:
  - `WriteUInt32(this.Token)`
  - `WriteUInt32((uint)`

```csharp
{
base._worldPacket.WriteUInt32(this.Token);
base._worldPacket.WriteUInt32((uint)this.CharacterList.Count);
base._worldPacket.ResetBitPos();
base._worldPacket.WriteBit(bit: false);
foreach (AccountCharacterListEntry entry in this.CharacterList)
{
entry.Write(base._worldPacket);
}
}
```

---

### GossipComplete

- File: HermesProxy/World/Server/Packets/GossipComplete.cs

```csharp
{
if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 2, 2, 5, 3))
{
base._worldPacket.WriteBit(this.SuppressSound);
base._worldPacket.FlushBits();
}
}
```

---

### GossipMessagePkt

- File: HermesProxy/World/Server/Packets/GossipMessagePkt.cs
- Fields:
  - `WriteInt32(this.GossipID)`
  - `WriteInt32(this.FriendshipFactionID)`
  - `WriteInt32(this.TextID)`
  - `WriteInt32(this.GossipOptions.Count)`
  - `WriteInt32(this.GossipQuests.Count)`
  - `WriteInt32(options.OptionIndex)`
  - `WriteUInt8(options.OptionIcon)`
  - `WriteUInt8(options.OptionFlags)`
  - `WriteInt32(options.OptionCost)`
  - `WriteUInt32(options.Language)`
  - `WriteString(options.Text)`
  - `WriteString(options.Confirm)`
  - `WriteInt32(options.SpellID.Value)`

```csharp
{
if (ModernVersion.ExpansionVersion >= 3)
{
this.WriteWotLK();
return;
}
base._worldPacket.WritePackedGuid128(this.GossipGUID);
base._worldPacket.WriteInt32(this.GossipID);
base._worldPacket.WriteInt32(this.FriendshipFactionID);
base._worldPacket.WriteInt32(this.TextID);
base._worldPacket.WriteInt32(this.GossipOptions.Count);
base._worldPacket.WriteInt32(this.GossipQuests.Count);
foreach (ClientGossipOption options in this.GossipOptions)
{
base._worldPacket.WriteInt32(options.OptionIndex);
base._worldPacket.WriteUInt8(options.OptionIcon);
base._worldPacket.WriteUInt8(options.OptionFlags);
base._worldPacket.WriteInt32(options.OptionCost);
if (ModernVersion.AddedInVersion(9, 2, 0, 1, 14, 1, 2, 5, 3))
{
base._worldPacket.WriteUInt32(options.Language);
}
base._worldPacket.WriteBits(options.Text.GetByteCount(), 12);
base._worldPacket.WriteBits(options.Confirm.GetByteCount(), 12);
base._worldPacket.WriteBits((byte)options.Status, 2);
```

---

### GossipPOI

- File: HermesProxy/World/Server/Packets/GossipPOI.cs
- Fields:
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteFloat(this.Pos.X)`
  - `WriteFloat(this.Pos.Y)`
  - `WriteFloat(this.Pos.Z)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteInt32((int)this.Id);
base._worldPacket.WriteInt32((int)this.Flags);
base._worldPacket.WriteFloat(this.Pos.X);
base._worldPacket.WriteFloat(this.Pos.Y);
base._worldPacket.WriteFloat(this.Pos.Z);
base._worldPacket.WriteInt32((int)this.Icon);
base._worldPacket.WriteInt32((int)this.Importance);
base._worldPacket.WriteInt32((int)this.Unknown905);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Name);
}
```

---

### GroupDecline

- File: HermesProxy/World/Server/Packets/GroupDecline.cs
- Fields:
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Name);
}
```

---

### GroupNewLeader

- File: HermesProxy/World/Server/Packets/GroupNewLeader.cs
- Fields:
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteInt8(this.PartyIndex);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
base._worldPacket.WriteString(this.Name);
}
```

---

### GroupUninvite

- File: HermesProxy/World/Server/Packets/GroupUninvite.cs

```csharp
{
}
```

---

### GuildBankLogQueryResults

- File: HermesProxy/World/Server/Packets/GuildBankLogQueryResults.cs
- Fields:
  - `WriteInt32(this.Tab)`
  - `WriteInt32(this.Entry.Count)`
  - `WriteUInt32(logEntry.TimeOffset)`
  - `WriteUInt64(logEntry.Money.Value)`
  - `WriteInt32(logEntry.ItemID.Value)`
  - `WriteInt32(logEntry.Count.Value)`
  - `WriteUInt64(this.WeeklyBonusMoney.Value)`

```csharp
{
base._worldPacket.WriteInt32(this.Tab);
base._worldPacket.WriteInt32(this.Entry.Count);
base._worldPacket.WriteBit(this.WeeklyBonusMoney.HasValue);
base._worldPacket.FlushBits();
foreach (GuildBankLogEntry logEntry in this.Entry)
{
base._worldPacket.WritePackedGuid128(logEntry.PlayerGUID);
base._worldPacket.WriteUInt32(logEntry.TimeOffset);
base._worldPacket.WriteInt8(logEntry.EntryType);
base._worldPacket.WriteBit(logEntry.Money.HasValue);
base._worldPacket.WriteBit(logEntry.ItemID.HasValue);
base._worldPacket.WriteBit(logEntry.Count.HasValue);
base._worldPacket.WriteBit(logEntry.OtherTab.HasValue);
base._worldPacket.FlushBits();
if (logEntry.Money.HasValue)
{
base._worldPacket.WriteUInt64(logEntry.Money.Value);
}
if (logEntry.ItemID.HasValue)
{
base._worldPacket.WriteInt32(logEntry.ItemID.Value);
}
if (logEntry.Count.HasValue)
{
```

---

### GuildBankQueryResults

- File: HermesProxy/World/Server/Packets/GuildBankQueryResults.cs
- Fields:
  - `WriteUInt64(this.Money)`
  - `WriteInt32(this.Tab)`
  - `WriteInt32(this.WithdrawalsRemaining)`
  - `WriteInt32(this.TabInfo.Count)`
  - `WriteInt32(this.ItemInfo.Count)`
  - `WriteInt32(tab.TabIndex)`
  - `WriteString(tab.Name)`
  - `WriteString(tab.Icon)`
  - `WriteInt32(item.Slot)`
  - `WriteInt32(item.Count)`
  - `WriteInt32(item.EnchantmentID)`
  - `WriteInt32(item.Charges)`
  - `WriteInt32(item.OnUseEnchantmentID)`
  - `WriteUInt32(item.Flags)`

```csharp
{
base._worldPacket.WriteUInt64(this.Money);
base._worldPacket.WriteInt32(this.Tab);
base._worldPacket.WriteInt32(this.WithdrawalsRemaining);
base._worldPacket.WriteInt32(this.TabInfo.Count);
base._worldPacket.WriteInt32(this.ItemInfo.Count);
base._worldPacket.WriteBit(this.FullUpdate);
base._worldPacket.FlushBits();
foreach (GuildBankTabInfo tab in this.TabInfo)
{
base._worldPacket.WriteInt32(tab.TabIndex);
base._worldPacket.WriteBits(tab.Name.GetByteCount(), 7);
base._worldPacket.WriteBits(tab.Icon.GetByteCount(), 9);
base._worldPacket.WriteString(tab.Name);
base._worldPacket.WriteString(tab.Icon);
}
foreach (GuildBankItemInfo item in this.ItemInfo)
{
base._worldPacket.WriteInt32(item.Slot);
base._worldPacket.WriteInt32(item.Count);
base._worldPacket.WriteInt32(item.EnchantmentID);
base._worldPacket.WriteInt32(item.Charges);
base._worldPacket.WriteInt32(item.OnUseEnchantmentID);
base._worldPacket.WriteUInt32(item.Flags);
item.Item.Write(base._worldPacket);
```

---

### GuildBankRemainingWithdrawMoney

- File: HermesProxy/World/Server/Packets/GuildBankRemainingWithdrawMoney.cs
- Fields:
  - `WriteInt64(this.RemainingWithdrawMoney)`

```csharp
{
base._worldPacket.WriteInt64(this.RemainingWithdrawMoney);
}
```

---

### GuildBankTextQueryResult

- File: HermesProxy/World/Server/Packets/GuildBankTextQueryResult.cs
- Fields:
  - `WriteInt32(this.Tab)`
  - `WriteString(this.Text)`

```csharp
{
base._worldPacket.WriteInt32(this.Tab);
base._worldPacket.WriteBits(this.Text.GetByteCount(), 14);
base._worldPacket.WriteString(this.Text);
}
```

---

### GuildCommandResult

- File: HermesProxy/World/Server/Packets/GuildCommandResult.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Result);
base._worldPacket.WriteUInt32((uint)this.Command);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 8);
base._worldPacket.WriteString(this.Name);
}
```

---

### GuildEventBankMoneyChanged

- File: HermesProxy/World/Server/Packets/GuildEventBankMoneyChanged.cs
- Fields:
  - `WriteUInt64(this.Money)`

```csharp
{
base._worldPacket.WriteUInt64(this.Money);
}
```

---

### GuildEventDisbanded

- File: HermesProxy/World/Server/Packets/GuildEventDisbanded.cs

```csharp
{
}
```

---

### GuildEventMotd

- File: HermesProxy/World/Server/Packets/GuildEventMotd.cs
- Fields:
  - `WriteString(this.MotdText)`

```csharp
{
base._worldPacket.WriteBits(this.MotdText.GetByteCount(), 11);
base._worldPacket.WriteString(this.MotdText);
}
```

---

### GuildEventNewLeader

- File: HermesProxy/World/Server/Packets/GuildEventNewLeader.cs
- Fields:
  - `WriteUInt32(this.OldLeaderVirtualRealmAddress)`
  - `WriteUInt32(this.NewLeaderVirtualRealmAddress)`
  - `WriteString(this.OldLeaderName)`
  - `WriteString(this.NewLeaderName)`

```csharp
{
base._worldPacket.WriteBit(this.SelfPromoted);
base._worldPacket.WriteBits(this.OldLeaderName.GetByteCount(), 6);
base._worldPacket.WriteBits(this.NewLeaderName.GetByteCount(), 6);
base._worldPacket.WritePackedGuid128(this.OldLeaderGUID);
base._worldPacket.WriteUInt32(this.OldLeaderVirtualRealmAddress);
base._worldPacket.WritePackedGuid128(this.NewLeaderGUID);
base._worldPacket.WriteUInt32(this.NewLeaderVirtualRealmAddress);
base._worldPacket.WriteString(this.OldLeaderName);
base._worldPacket.WriteString(this.NewLeaderName);
}
```

---

### GuildEventPlayerJoined

- File: HermesProxy/World/Server/Packets/GuildEventPlayerJoined.cs
- Fields:
  - `WriteUInt32(this.VirtualRealmAddress)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
base._worldPacket.WriteString(this.Name);
}
```

---

### GuildEventPlayerLeft

- File: HermesProxy/World/Server/Packets/GuildEventPlayerLeft.cs
- Fields:
  - `WriteUInt32(this.RemoverVirtualRealmAddress)`
  - `WriteString(this.RemoverName)`
  - `WriteUInt32(this.LeaverVirtualRealmAddress)`
  - `WriteString(this.LeaverName)`

```csharp
{
base._worldPacket.WriteBit(this.Removed);
base._worldPacket.WriteBits(this.LeaverName.GetByteCount(), 6);
if (this.Removed)
{
base._worldPacket.WriteBits(this.RemoverName.GetByteCount(), 6);
base._worldPacket.WritePackedGuid128(this.RemoverGUID);
base._worldPacket.WriteUInt32(this.RemoverVirtualRealmAddress);
base._worldPacket.WriteString(this.RemoverName);
}
base._worldPacket.WritePackedGuid128(this.LeaverGUID);
base._worldPacket.WriteUInt32(this.LeaverVirtualRealmAddress);
base._worldPacket.WriteString(this.LeaverName);
}
```

---

### GuildEventPresenceChange

- File: HermesProxy/World/Server/Packets/GuildEventPresenceChange.cs
- Fields:
  - `WriteUInt32(this.VirtualRealmAddress)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteUInt32(this.VirtualRealmAddress);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 6);
base._worldPacket.WriteBit(this.LoggedOn);
base._worldPacket.WriteBit(this.Mobile);
base._worldPacket.WriteString(this.Name);
}
```

---

### GuildEventRanksUpdated

- File: HermesProxy/World/Server/Packets/GuildEventRanksUpdated.cs

```csharp
{
}
```

---

### GuildEventTabAdded

- File: HermesProxy/World/Server/Packets/GuildEventTabAdded.cs

```csharp
{
}
```

---

### GuildEventTabModified

- File: HermesProxy/World/Server/Packets/GuildEventTabModified.cs
- Fields:
  - `WriteInt32(this.Tab)`
  - `WriteString(this.Name)`
  - `WriteString(this.Icon)`

```csharp
{
base._worldPacket.WriteInt32(this.Tab);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 7);
base._worldPacket.WriteBits(this.Icon.GetByteCount(), 9);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Name);
base._worldPacket.WriteString(this.Icon);
}
```

---

### GuildEventTabTextChanged

- File: HermesProxy/World/Server/Packets/GuildEventTabTextChanged.cs
- Fields:
  - `WriteInt32(this.Tab)`

```csharp
{
base._worldPacket.WriteInt32(this.Tab);
}
```

---

### GuildInvite

- File: HermesProxy/World/Server/Packets/GuildInvite.cs
- Fields:
  - `WriteUInt32(this.InviterVirtualRealmAddress)`
  - `WriteUInt32(this.GuildVirtualRealmAddress)`
  - `WriteUInt32(this.OldGuildVirtualRealmAddress)`
  - `WriteUInt32(this.EmblemStyle)`
  - `WriteUInt32(this.EmblemColor)`
  - `WriteUInt32(this.BorderStyle)`
  - `WriteUInt32(this.BorderColor)`
  - `WriteUInt32(this.BackgroundColor)`
  - `WriteInt32(this.AchievementPoints)`
  - `WriteString(this.InviterName)`
  - `WriteString(this.GuildName)`
  - `WriteString(this.OldGuildName)`

```csharp
{
base._worldPacket.WriteBits(this.InviterName.GetByteCount(), 6);
base._worldPacket.WriteBits(this.GuildName.GetByteCount(), 7);
base._worldPacket.WriteBits(this.OldGuildName.GetByteCount(), 7);
base._worldPacket.WriteUInt32(this.InviterVirtualRealmAddress);
base._worldPacket.WriteUInt32(this.GuildVirtualRealmAddress);
base._worldPacket.WritePackedGuid128(this.GuildGUID);
base._worldPacket.WriteUInt32(this.OldGuildVirtualRealmAddress);
base._worldPacket.WritePackedGuid128(this.OldGuildGUID);
base._worldPacket.WriteUInt32(this.EmblemStyle);
base._worldPacket.WriteUInt32(this.EmblemColor);
base._worldPacket.WriteUInt32(this.BorderStyle);
base._worldPacket.WriteUInt32(this.BorderColor);
base._worldPacket.WriteUInt32(this.BackgroundColor);
base._worldPacket.WriteInt32(this.AchievementPoints);
base._worldPacket.WriteString(this.InviterName);
base._worldPacket.WriteString(this.GuildName);
base._worldPacket.WriteString(this.OldGuildName);
}
```

---

### GuildInviteDeclined

- File: HermesProxy/World/Server/Packets/GuildInviteDeclined.cs
- Fields:
  - `WriteUInt32(this.InviterVirtualRealmAddress)`
  - `WriteString(this.InviterName)`

```csharp
{
base._worldPacket.WriteBits(this.InviterName.GetByteCount(), 6);
base._worldPacket.WriteBit(this.AutoDecline);
base._worldPacket.FlushBits();
base._worldPacket.WriteUInt32(this.InviterVirtualRealmAddress);
base._worldPacket.WriteString(this.InviterName);
}
```

---

### GuildRanks

- File: HermesProxy/World/Server/Packets/GuildRanks.cs
- Fields:
  - `WriteInt32(this.Ranks.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Ranks.Count);
this.Ranks.ForEach(delegate(GuildRankData p)
{
p.Write(base._worldPacket);
});
}
```

---

### GuildRoster

- File: HermesProxy/World/Server/Packets/GuildRoster.cs
- Fields:
  - `WriteUInt32(this.NumAccounts)`
  - `WriteInt32(this.GuildFlags)`
  - `WriteInt32(this.MemberData.Count)`
  - `WriteString(this.WelcomeText)`
  - `WriteString(this.InfoText)`

```csharp
{
base._worldPacket.WriteUInt32(this.NumAccounts);
base._worldPacket.WritePackedTime(this.CreateDate);
base._worldPacket.WriteInt32(this.GuildFlags);
base._worldPacket.WriteInt32(this.MemberData.Count);
base._worldPacket.WriteBits(this.WelcomeText.GetByteCount(), 11);
base._worldPacket.WriteBits(this.InfoText.GetByteCount(), 11);
base._worldPacket.FlushBits();
this.MemberData.ForEach(delegate(GuildRosterMemberData p)
{
p.Write(base._worldPacket);
});
base._worldPacket.WriteString(this.WelcomeText);
base._worldPacket.WriteString(this.InfoText);
}
```

---

### GuildSendRankChange

- File: HermesProxy/World/Server/Packets/GuildSendRankChange.cs
- Fields:
  - `WriteUInt32(this.RankID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Officer);
base._worldPacket.WritePackedGuid128(this.Other);
base._worldPacket.WriteUInt32(this.RankID);
base._worldPacket.WriteBit(this.Promote);
base._worldPacket.FlushBits();
}
```

---

### HotFixMessage

- File: HermesProxy/World/Server/Packets/HotFixMessage.cs
- Fields:
  - `WriteInt32(this.Hotfixes.Count)`
  - `WriteUInt32(totalDataSize)`
  - `WriteBytes(hotfix2.HotfixContent)`

```csharp
{
base._worldPacket.WriteInt32(this.Hotfixes.Count);
uint totalDataSize = 0u;
foreach (HotfixRecord hotfix in this.Hotfixes)
{
totalDataSize += hotfix.HotfixContent.GetSize();
hotfix.WriteHotFixMessageContent(base._worldPacket);
}
base._worldPacket.WriteUInt32(totalDataSize);
foreach (HotfixRecord hotfix2 in this.Hotfixes)
{
base._worldPacket.WriteBytes(hotfix2.HotfixContent);
}
}
```

---

### HotfixConnect

- File: HermesProxy/World/Server/Packets/HotfixConnect.cs
- Fields:
  - `WriteInt32(this.Hotfixes.Count)`
  - `WriteUInt32(totalDataSize)`
  - `WriteBytes(hotfix2.HotfixContent)`

```csharp
{
base._worldPacket.WriteInt32(this.Hotfixes.Count);
uint totalDataSize = 0u;
foreach (HotfixRecord hotfix in this.Hotfixes)
{
totalDataSize += hotfix.HotfixContent.GetSize();
hotfix.WriteHotFixMessageContent(base._worldPacket);
}
base._worldPacket.WriteUInt32(totalDataSize);
foreach (HotfixRecord hotfix2 in this.Hotfixes)
{
base._worldPacket.WriteBytes(hotfix2.HotfixContent);
}
}
```

---

### InitWorldStates

- File: HermesProxy/World/Server/Packets/InitWorldStates.cs
- Fields:
  - `WriteUInt32(this.MapID)`
  - `WriteUInt32(this.ZoneID)`
  - `WriteUInt32(this.AreaID)`
  - `WriteInt32(this.Worldstates.Count)`
  - `WriteUInt32(wsi.VariableID)`
  - `WriteInt32(wsi.Value)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteUInt32(this.ZoneID);
base._worldPacket.WriteUInt32(this.AreaID);
base._worldPacket.WriteInt32(this.Worldstates.Count);
foreach (WorldStateInfo wsi in this.Worldstates)
{
base._worldPacket.WriteUInt32(wsi.VariableID);
base._worldPacket.WriteInt32(wsi.Value);
}
}
```

---

### InitialSetup

- File: HermesProxy/World/Server/Packets/InitialSetup.cs
- Fields:
  - `WriteUInt8(this.ServerExpansionLevel)`
  - `WriteUInt8(this.ServerExpansionTier)`

```csharp
{
base._worldPacket.WriteUInt8(this.ServerExpansionLevel);
base._worldPacket.WriteUInt8(this.ServerExpansionTier);
}
```

---

### InitializeFactions

- File: HermesProxy/World/Server/Packets/InitializeFactions.cs
- Fields:
  - `WriteUInt16((ushort)`
  - `WriteUInt8((byte)`
  - `WriteInt32(this.FactionStandings[i])`

```csharp
{
ushort count = InitializeFactions.GetFactionCount();
for (ushort i = 0; i < count; i++)
{
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteUInt16((ushort)this.FactionFlags[i]);
}
else
{
base._worldPacket.WriteUInt8((byte)(this.FactionFlags[i] & (ReputationFlags.Visible | ReputationFlags.AtWar | ReputationFlags.Hidden | ReputationFlags.Header | ReputationFlags.Peaceful | ReputationFlags.Inactive | ReputationFlags.ShowPropagated | ReputationFlags.HeaderShowsBar)));
}
base._worldPacket.WriteInt32(this.FactionStandings[i]);
}
for (ushort i2 = 0; i2 < count; i2++)
{
base._worldPacket.WriteBit(this.FactionHasBonus[i2]);
}
base._worldPacket.FlushBits();
}
```

---

### InspectHonorStatsResultClassic

- File: HermesProxy/World/Server/Packets/InspectHonorStatsResultClassic.cs
- Fields:
  - `WriteUInt8(this.LifetimeHighestRank)`
  - `WriteUInt16(this.TodayHonorableKills)`
  - `WriteUInt16(this.TodayDishonorableKills)`
  - `WriteUInt16(this.YesterdayHonorableKills)`
  - `WriteUInt16(this.YesterdayDishonorableKills)`
  - `WriteUInt16(this.LastWeekHonorableKills)`
  - `WriteUInt16(this.LastWeekDishonorableKills)`
  - `WriteUInt16(this.ThisWeekHonorableKills)`
  - `WriteUInt16(this.ThisWeekDishonorableKills)`
  - `WriteUInt32(this.LifetimeHonorableKills)`
  - `WriteUInt32(this.LifetimeDishonorableKills)`
  - `WriteUInt32(this.YesterdayHonor)`
  - `WriteUInt32(this.LastWeekHonor)`
  - `WriteUInt32(this.ThisWeekHonor)`
  - `WriteUInt32(this.Standing)`
  - `WriteUInt8(this.RankProgress)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PlayerGUID);
base._worldPacket.WriteUInt8(this.LifetimeHighestRank);
base._worldPacket.WriteUInt16(this.TodayHonorableKills);
base._worldPacket.WriteUInt16(this.TodayDishonorableKills);
base._worldPacket.WriteUInt16(this.YesterdayHonorableKills);
base._worldPacket.WriteUInt16(this.YesterdayDishonorableKills);
base._worldPacket.WriteUInt16(this.LastWeekHonorableKills);
base._worldPacket.WriteUInt16(this.LastWeekDishonorableKills);
base._worldPacket.WriteUInt16(this.ThisWeekHonorableKills);
base._worldPacket.WriteUInt16(this.ThisWeekDishonorableKills);
base._worldPacket.WriteUInt32(this.LifetimeHonorableKills);
base._worldPacket.WriteUInt32(this.LifetimeDishonorableKills);
base._worldPacket.WriteUInt32(this.YesterdayHonor);
base._worldPacket.WriteUInt32(this.LastWeekHonor);
base._worldPacket.WriteUInt32(this.ThisWeekHonor);
base._worldPacket.WriteUInt32(this.Standing);
base._worldPacket.WriteUInt8(this.RankProgress);
}
```

---

### InspectHonorStatsResultTBC

- File: HermesProxy/World/Server/Packets/InspectHonorStatsResultTBC.cs
- Fields:
  - `WriteUInt8(this.LifetimeHighestRank)`
  - `WriteUInt16(this.Unused1)`
  - `WriteUInt16(this.YesterdayHonorableKills)`
  - `WriteUInt16(this.Unused3)`
  - `WriteUInt16(this.LifetimeHonorableKills)`
  - `WriteUInt32(this.Unused4)`
  - `WriteUInt32(this.Unused5)`
  - `WriteUInt32(this.Unused6)`
  - `WriteUInt32(this.Unused7)`
  - `WriteUInt32(this.Unused8)`
  - `WriteUInt8(this.Unused9)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PlayerGUID);
base._worldPacket.WriteUInt8(this.LifetimeHighestRank);
base._worldPacket.WriteUInt16(this.Unused1);
base._worldPacket.WriteUInt16(this.YesterdayHonorableKills);
base._worldPacket.WriteUInt16(this.Unused3);
base._worldPacket.WriteUInt16(this.LifetimeHonorableKills);
base._worldPacket.WriteUInt32(this.Unused4);
base._worldPacket.WriteUInt32(this.Unused5);
base._worldPacket.WriteUInt32(this.Unused6);
base._worldPacket.WriteUInt32(this.Unused7);
base._worldPacket.WriteUInt32(this.Unused8);
base._worldPacket.WriteUInt8(this.Unused9);
}
```

---

### InspectPvP

- File: HermesProxy/World/Server/Packets/InspectPvP.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.PlayerGUID);
base._worldPacket.WriteBits(this.Brackets.Count, 3);
base._worldPacket.WriteBits(this.ArenaTeams.Count, 2);
base._worldPacket.FlushBits();
foreach (PvPBracketInspectData bracket in this.Brackets)
{
bracket.Write(base._worldPacket);
}
foreach (ArenaTeamInspectData team in this.ArenaTeams)
{
team.Write(base._worldPacket);
}
}
```

---

### InspectResult

- File: HermesProxy/World/Server/Packets/InspectResult.cs
- Fields:
  - `WriteInt32(this.Glyphs.Count)`
  - `WriteInt32(this.Talents.Count)`
  - `WriteInt32(this.ItemLevel)`
  - `WriteUInt8(this.LifetimeMaxRank)`
  - `WriteUInt16(this.TodayHK)`
  - `WriteUInt16(this.YesterdayHK)`
  - `WriteUInt32(this.LifetimeHK)`
  - `WriteUInt32(this.HonorLevel)`
  - `WriteUInt16(this.Glyphs[i])`
  - `WriteUInt8(this.Talents[j])`
  - `WriteUInt32(this.AzeriteLevel.Value)`

```csharp
{
this.DisplayInfo.Write(base._worldPacket);
base._worldPacket.WriteInt32(this.Glyphs.Count);
base._worldPacket.WriteInt32(this.Talents.Count);
base._worldPacket.WriteInt32(this.ItemLevel);
base._worldPacket.WriteUInt8(this.LifetimeMaxRank);
base._worldPacket.WriteUInt16(this.TodayHK);
base._worldPacket.WriteUInt16(this.YesterdayHK);
base._worldPacket.WriteUInt32(this.LifetimeHK);
base._worldPacket.WriteUInt32(this.HonorLevel);
for (int i = 0; i < this.Glyphs.Count; i++)
{
base._worldPacket.WriteUInt16(this.Glyphs[i]);
}
for (int j = 0; j < this.Talents.Count; j++)
{
base._worldPacket.WriteUInt8(this.Talents[j]);
}
base._worldPacket.WriteBit(this.GuildData != null);
base._worldPacket.WriteBit(this.AzeriteLevel.HasValue);
base._worldPacket.FlushBits();
foreach (PVPBracketData item in this.Bracket)
{
item.Write(base._worldPacket);
}
```

---

### InstanceReset

- File: HermesProxy/World/Server/Packets/InstanceReset.cs
- Fields:
  - `WriteUInt32(this.MapID)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
}
```

---

### InstanceResetFailed

- File: HermesProxy/World/Server/Packets/InstanceResetFailed.cs
- Fields:
  - `WriteUInt32(this.MapID)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteBits(this.ResetFailedReason, 2);
base._worldPacket.FlushBits();
}
```

---

### InstanceSaveCreated

- File: HermesProxy/World/Server/Packets/InstanceSaveCreated.cs

```csharp
{
base._worldPacket.WriteBit(this.Gm);
base._worldPacket.FlushBits();
}
```

---

### InvalidatePlayer

- File: HermesProxy/World/Server/Packets/InvalidatePlayer.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### InventoryChangeFailure

- File: HermesProxy/World/Server/Packets/InventoryChangeFailure.cs
- Fields:
  - `WriteInt32((int)`
  - `WriteUInt8(this.ContainerBSlot)`
  - `WriteInt32(this.Level)`
  - `WriteInt32(this.SrcSlot)`
  - `WriteInt32(this.LimitCategory)`

```csharp
{
base._worldPacket.WriteInt32((int)this.BagResult);
base._worldPacket.WritePackedGuid128(this.Item[0]);
base._worldPacket.WritePackedGuid128(this.Item[1]);
base._worldPacket.WriteUInt8(this.ContainerBSlot);
switch (this.BagResult)
{
case InventoryResult.CantEquipLevel:
case InventoryResult.PurchaseLevelTooLow:
base._worldPacket.WriteInt32(this.Level);
break;
case InventoryResult.EventAutoEquipBindConfirm:
base._worldPacket.WritePackedGuid128(this.SrcContainer);
base._worldPacket.WriteInt32(this.SrcSlot);
base._worldPacket.WritePackedGuid128(this.DstContainer);
break;
case InventoryResult.ItemMaxLimitCategoryCountExceeded:
case InventoryResult.ItemMaxLimitCategorySocketedExceeded:
case InventoryResult.ItemMaxLimitCategoryEquippedExceeded:
base._worldPacket.WriteInt32(this.LimitCategory);
break;
}
}
```

---

### ItemCooldown

- File: HermesProxy/World/Server/Packets/ItemCooldown.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.Cooldown)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.ItemGuid);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.Cooldown);
}
```

---

### ItemEnchantTimeUpdate

- File: HermesProxy/World/Server/Packets/ItemEnchantTimeUpdate.cs
- Fields:
  - `WriteUInt32(this.DurationLeft)`
  - `WriteUInt32(this.Slot)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.ItemGuid);
base._worldPacket.WriteUInt32(this.DurationLeft);
base._worldPacket.WriteUInt32(this.Slot);
base._worldPacket.WritePackedGuid128(this.OwnerGuid);
}
```

---

### ItemPushResult

- File: HermesProxy/World/Server/Packets/ItemPushResult.cs
- Fields:
  - `WriteUInt8(this.Slot)`
  - `WriteInt32(this.SlotInBag)`
  - `WriteInt32(this.QuestLogItemID)`
  - `WriteUInt32(this.Quantity)`
  - `WriteUInt32(this.QuantityInInventory)`
  - `WriteInt32(this.DungeonEncounterID)`
  - `WriteInt32(this.BattlePetSpeciesID)`
  - `WriteInt32(this.BattlePetBreedID)`
  - `WriteUInt32(this.BattlePetBreedQuality)`
  - `WriteInt32(this.BattlePetLevel)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PlayerGUID);
base._worldPacket.WriteUInt8(this.Slot);
base._worldPacket.WriteInt32(this.SlotInBag);
base._worldPacket.WriteInt32(this.QuestLogItemID);
base._worldPacket.WriteUInt32(this.Quantity);
base._worldPacket.WriteUInt32(this.QuantityInInventory);
base._worldPacket.WriteInt32(this.DungeonEncounterID);
base._worldPacket.WriteInt32(this.BattlePetSpeciesID);
base._worldPacket.WriteInt32(this.BattlePetBreedID);
base._worldPacket.WriteUInt32(this.BattlePetBreedQuality);
base._worldPacket.WriteInt32(this.BattlePetLevel);
base._worldPacket.WritePackedGuid128(this.ItemGUID);
base._worldPacket.WriteBit(this.Pushed);
base._worldPacket.WriteBit(this.Created);
base._worldPacket.WriteBits((uint)this.DisplayText, 3);
base._worldPacket.WriteBit(this.IsBonusRoll);
base._worldPacket.WriteBit(this.IsEncounterLoot);
base._worldPacket.FlushBits();
this.Item.Write(base._worldPacket);
}
```

---

### LFGListUpdateBlacklist

- File: HermesProxy/World/Server/Packets/LFGListUpdateBlacklist.cs
- Fields:
  - `WriteInt32(this.Blacklist.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Blacklist.Count);
foreach (LFGListBlacklistEntry item in this.Blacklist)
{
item.Write(base._worldPacket);
}
}
```

---

### LearnedSpells

- File: HermesProxy/World/Server/Packets/LearnedSpells.cs
- Fields:
  - `WriteInt32(this.Spells.Count)`
  - `WriteInt32(this.FavoriteSpellID.Count)`
  - `WriteUInt32(this.SpecializationID)`
  - `WriteUInt32(spell)`
  - `WriteInt32(spell2)`

```csharp
{
base._worldPacket.WriteInt32(this.Spells.Count);
base._worldPacket.WriteInt32(this.FavoriteSpellID.Count);
base._worldPacket.WriteUInt32(this.SpecializationID);
foreach (uint spell in this.Spells)
{
base._worldPacket.WriteUInt32(spell);
}
foreach (int spell2 in this.FavoriteSpellID)
{
base._worldPacket.WriteInt32(spell2);
}
base._worldPacket.WriteBit(this.SuppressMessaging);
base._worldPacket.FlushBits();
}
```

---

### LevelUpInfo

- File: HermesProxy/World/Server/Packets/LevelUpInfo.cs
- Fields:
  - `WriteInt32(this.Level)`
  - `WriteInt32(this.HealthDelta)`
  - `WriteInt32((i < this.PowerDelta.Length)`
  - `WriteInt32(stat)`
  - `WriteInt32(this.NumNewTalents)`
  - `WriteInt32(this.NumNewPvpTalentSlots)`

```csharp
{
base._worldPacket.WriteInt32(this.Level);
base._worldPacket.WriteInt32(this.HealthDelta);
int powerCount = ((ModernVersion.ExpansionVersion >= 3) ? 10 : ModernVersion.GetPowerCountForClientVersion());
for (int i = 0; i < powerCount; i++)
{
base._worldPacket.WriteInt32((i < this.PowerDelta.Length) ? this.PowerDelta[i] : 0);
}
int[] statDelta = this.StatDelta;
foreach (int stat in statDelta)
{
base._worldPacket.WriteInt32(stat);
}
base._worldPacket.WriteInt32(this.NumNewTalents);
if (ModernVersion.ExpansionVersion < 3)
{
base._worldPacket.WriteInt32(this.NumNewPvpTalentSlots);
}
}
```

---

### LfgPlayerInfoPkt

- File: HermesProxy/World/Server/Packets/LfgPlayerInfoPkt.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(slot.Slot)`
  - `WriteUInt32(slot.Reason)`
  - `WriteInt32(slot.SubReason1)`
  - `WriteInt32(slot.SubReason2)`
  - `WriteUInt32(slot.SoftLock)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Dungeons.Count);
// Write BlackList
bool hasGuid = this.BlackList.PlayerGuid != null;
base._worldPacket.WriteBit(hasGuid);
base._worldPacket.WriteUInt32((uint)(this.BlackList.Slots?.Count ?? 0));
if (hasGuid)
base._worldPacket.WritePackedGuid128(this.BlackList.PlayerGuid);
if (this.BlackList.Slots != null)
foreach (var slot in this.BlackList.Slots)
{
base._worldPacket.WriteUInt32(slot.Slot);
base._worldPacket.WriteUInt32(slot.Reason);
base._worldPacket.WriteInt32(slot.SubReason1);
base._worldPacket.WriteInt32(slot.SubReason2);
base._worldPacket.WriteUInt32(slot.SoftLock);
}
// Write Dungeons
foreach (var dungeon in this.Dungeons)
{
dungeon.Write(base._worldPacket);
}
}
```

---

### LoadCUFProfiles

- File: HermesProxy/World/Server/Packets/LoadCUFProfiles.cs
- Fields:
  - `WriteBytes(this.Data)`

```csharp
{
base._worldPacket.WriteBytes(this.Data);
}
```

---

### LogXPGain

- File: HermesProxy/World/Server/Packets/LogXPGain.cs
- Fields:
  - `WriteInt32(this.Original)`
  - `WriteUInt8((byte)`
  - `WriteInt32(this.Amount)`
  - `WriteFloat(this.GroupBonus)`
  - `WriteUInt8(this.RAFBonus)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Victim);
base._worldPacket.WriteInt32(this.Original);
base._worldPacket.WriteUInt8((byte)this.Reason);
base._worldPacket.WriteInt32(this.Amount);
base._worldPacket.WriteFloat(this.GroupBonus);
base._worldPacket.WriteUInt8(this.RAFBonus);
}
```

---

### LoginSetTimeSpeed

- File: HermesProxy/World/Server/Packets/LoginSetTimeSpeed.cs
- Fields:
  - `WriteUInt32(this.ServerTime)`
  - `WriteUInt32(this.GameTime)`
  - `WriteFloat(this.NewSpeed)`
  - `WriteInt32(this.ServerTimeHolidayOffset)`
  - `WriteInt32(this.GameTimeHolidayOffset)`

```csharp
{
base._worldPacket.WriteUInt32(this.ServerTime);
base._worldPacket.WriteUInt32(this.GameTime);
base._worldPacket.WriteFloat(this.NewSpeed);
base._worldPacket.WriteInt32(this.ServerTimeHolidayOffset);
base._worldPacket.WriteInt32(this.GameTimeHolidayOffset);
}
```

---

### LoginVerifyWorld

- File: HermesProxy/World/Server/Packets/LoginVerifyWorld.cs
- Fields:
  - `WriteUInt32(this.MapID)`
  - `WriteFloat(this.Pos.X)`
  - `WriteFloat(this.Pos.Y)`
  - `WriteFloat(this.Pos.Z)`
  - `WriteFloat(this.Pos.Orientation)`
  - `WriteUInt32(this.Reason)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteFloat(this.Pos.X);
base._worldPacket.WriteFloat(this.Pos.Y);
base._worldPacket.WriteFloat(this.Pos.Z);
base._worldPacket.WriteFloat(this.Pos.Orientation);
base._worldPacket.WriteUInt32(this.Reason);
}
```

---

### LogoutCancelAck

- File: HermesProxy/World/Server/Packets/LogoutCancelAck.cs

```csharp
{
}
```

---

### LogoutComplete

- File: HermesProxy/World/Server/Packets/LogoutComplete.cs

```csharp
{
}
```

---

### LogoutResponse

- File: HermesProxy/World/Server/Packets/LogoutResponse.cs
- Fields:
  - `WriteInt32(this.LogoutResult)`

```csharp
{
base._worldPacket.WriteInt32(this.LogoutResult);
base._worldPacket.WriteBit(this.Instant);
base._worldPacket.FlushBits();
}
```

---

### LootAllPassed

- File: HermesProxy/World/Server/Packets/LootAllPassed.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
this.Item.Write(base._worldPacket);
}
```

---

### LootList

- File: HermesProxy/World/Server/Packets/LootList.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Owner);
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WriteBit(this.Master != null);
base._worldPacket.WriteBit(this.RoundRobinWinner != null);
base._worldPacket.FlushBits();
if (this.Master != null)
{
base._worldPacket.WritePackedGuid128(this.Master);
}
if (this.RoundRobinWinner != null)
{
base._worldPacket.WritePackedGuid128(this.RoundRobinWinner);
}
}
```

---

### LootMoneyNotify

- File: HermesProxy/World/Server/Packets/LootMoneyNotify.cs
- Fields:
  - `WriteUInt64(this.Money)`
  - `WriteUInt64(this.MoneyMod)`

```csharp
{
base._worldPacket.WriteUInt64(this.Money);
base._worldPacket.WriteUInt64(this.MoneyMod);
base._worldPacket.WriteBit(this.SoleLooter);
base._worldPacket.FlushBits();
}
```

---

### LootReleaseResponse

- File: HermesProxy/World/Server/Packets/LootReleaseResponse.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WritePackedGuid128(this.Owner);
}
```

---

### LootRemoved

- File: HermesProxy/World/Server/Packets/LootRemoved.cs
- Fields:
  - `WriteUInt8(this.LootListID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Owner);
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WriteUInt8(this.LootListID);
}
```

---

### LootResponse

- File: HermesProxy/World/Server/Packets/LootResponse.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`
  - `WriteUInt8(this.Threshold)`
  - `WriteUInt32(this.Coins)`
  - `WriteInt32(this.Items.Count)`
  - `WriteInt32(this.Currencies.Count)`
  - `WriteUInt32(currency.CurrencyID)`
  - `WriteUInt32(currency.Quantity)`
  - `WriteUInt8(currency.LootListID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Owner);
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WriteUInt8((byte)this.FailureReason);
base._worldPacket.WriteUInt8((byte)this.AcquireReason);
base._worldPacket.WriteUInt8((byte)this.LootMethod);
base._worldPacket.WriteUInt8(this.Threshold);
base._worldPacket.WriteUInt32(this.Coins);
base._worldPacket.WriteInt32(this.Items.Count);
base._worldPacket.WriteInt32(this.Currencies.Count);
base._worldPacket.WriteBit(this.Acquired);
base._worldPacket.WriteBit(this.AELooting);
base._worldPacket.FlushBits();
foreach (LootItemData item in this.Items)
{
item.Write(base._worldPacket);
}
foreach (LootCurrency currency in this.Currencies)
{
base._worldPacket.WriteUInt32(currency.CurrencyID);
base._worldPacket.WriteUInt32(currency.Quantity);
base._worldPacket.WriteUInt8(currency.LootListID);
base._worldPacket.WriteBits(currency.UIType, 3);
base._worldPacket.FlushBits();
}
```

---

### LootRollBroadcast

- File: HermesProxy/World/Server/Packets/LootRollBroadcast.cs
- Fields:
  - `WriteInt32(this.Roll)`
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WritePackedGuid128(this.Player);
base._worldPacket.WriteInt32(this.Roll);
base._worldPacket.WriteUInt8((byte)this.RollType);
this.Item.Write(base._worldPacket);
base._worldPacket.WriteBit(this.Autopassed);
base._worldPacket.FlushBits();
}
```

---

### LootRollWon

- File: HermesProxy/World/Server/Packets/LootRollWon.cs
- Fields:
  - `WriteInt32(this.Roll)`
  - `WriteUInt8((byte)`
  - `WriteUInt8(this.MainSpec)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WritePackedGuid128(this.Winner);
base._worldPacket.WriteInt32(this.Roll);
base._worldPacket.WriteUInt8((byte)this.RollType);
this.Item.Write(base._worldPacket);
base._worldPacket.WriteUInt8(this.MainSpec);
}
```

---

### LootRollsComplete

- File: HermesProxy/World/Server/Packets/LootRollsComplete.cs
- Fields:
  - `WriteUInt8(this.LootListID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WriteUInt8(this.LootListID);
}
```

---

### MOTD

- File: HermesProxy/World/Server/Packets/MOTD.cs
- Fields:
  - `WriteString(line)`

```csharp
{
base._worldPacket.WriteBits(this.Text.Count, 4);
base._worldPacket.FlushBits();
foreach (string line in this.Text)
{
base._worldPacket.WriteBits(line.GetByteCount(), 7);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(line);
}
}
```

---

### MailCommandResult

- File: HermesProxy/World/Server/Packets/MailCommandResult.cs
- Fields:
  - `WriteUInt32(this.MailID)`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(this.AttachID)`
  - `WriteUInt32(this.QtyInInventory)`

```csharp
{
base._worldPacket.WriteUInt32(this.MailID);
base._worldPacket.WriteUInt32((uint)this.Command);
base._worldPacket.WriteUInt32((uint)this.ErrorCode);
base._worldPacket.WriteUInt32((uint)this.BagResult);
base._worldPacket.WriteUInt32(this.AttachID);
base._worldPacket.WriteUInt32(this.QtyInInventory);
}
```

---

### MailListResult

- File: HermesProxy/World/Server/Packets/MailListResult.cs
- Fields:
  - `WriteInt32(this.Mails.Count)`
  - `WriteInt32(this.TotalNumRecords)`

```csharp
{
base._worldPacket.WriteInt32(this.Mails.Count);
base._worldPacket.WriteInt32(this.TotalNumRecords);
this.Mails.ForEach(delegate(MailListEntry p)
{
p.Write(base._worldPacket);
});
}
```

---

### MailQueryNextTimeResult

- File: HermesProxy/World/Server/Packets/MailQueryNextTimeResult.cs
- Fields:
  - `WriteFloat(this.NextMailTime)`
  - `WriteInt32(this.Mails.Count)`
  - `WriteFloat(entry.TimeLeft)`
  - `WriteInt32(entry.AltSenderID)`
  - `WriteInt32(entry.StationeryID)`

```csharp
{
base._worldPacket.WriteFloat(this.NextMailTime);
base._worldPacket.WriteInt32(this.Mails.Count);
foreach (MailNextTimeEntry entry in this.Mails)
{
base._worldPacket.WritePackedGuid128(entry.SenderGuid);
base._worldPacket.WriteFloat(entry.TimeLeft);
base._worldPacket.WriteInt32(entry.AltSenderID);
base._worldPacket.WriteInt8(entry.AltSenderType);
base._worldPacket.WriteInt32(entry.StationeryID);
}
}
```

---

### MasterLootCandidateList

- File: HermesProxy/World/Server/Packets/MasterLootCandidateList.cs
- Fields:
  - `WriteInt32(this.Players.Count)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WriteInt32(this.Players.Count);
foreach (WowGuid128 guid in this.Players)
{
base._worldPacket.WritePackedGuid128(guid);
}
}
```

---

### MinimapPing

- File: HermesProxy/World/Server/Packets/MinimapPing.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.SenderGUID);
base._worldPacket.WriteVector2(this.Position);
}
```

---

### MonsterMove

- File: HermesProxy/World/Server/Packets/MonsterMove.cs
- Fields:
  - `WriteVector3`
  - `WriteUInt32(this.MoveSpline.SplineId)`
  - `WriteVector3`
  - `WriteUInt32((uint)`
  - `WriteInt32(0)`
  - `WriteUInt32(this.MoveSpline.SplineTimeFull)`
  - `WriteUInt32(0u)`
  - `WriteUInt8(this.MoveSpline.SplineMode)`
  - `WriteVector3`
  - `WriteFloat(this.MoveSpline.FinalOrientation)`
  - `WriteFloat(this.MoveSpline.FinalOrientation)`
  - `WriteVector3`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteVector3(this.MoveSpline.StartPosition);
base._worldPacket.WriteUInt32(this.MoveSpline.SplineId);
base._worldPacket.WriteVector3(Vector3.Zero);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBits((this.Points.Count == 0) ? 2 : 0, 3);
base._worldPacket.WriteUInt32((uint)this.MoveSpline.SplineFlags);
base._worldPacket.WriteInt32(0);
base._worldPacket.WriteUInt32(this.MoveSpline.SplineTimeFull);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt8(this.MoveSpline.SplineMode);
base._worldPacket.WritePackedGuid128((this.MoveSpline.TransportGuid != null) ? this.MoveSpline.TransportGuid : WowGuid128.Empty);
base._worldPacket.WriteInt8(this.MoveSpline.TransportSeat);
base._worldPacket.WriteBits((byte)this.MoveSpline.SplineType, 2);
base._worldPacket.WriteBits(this.Points.Count, 16);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBits(this.PackedDeltas.Count, 16);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.FlushBits();
switch (this.MoveSpline.SplineType)
{
```

---

### MoveKnockBack

- File: HermesProxy/World/Server/Packets/MoveKnockBack.cs
- Fields:
  - `WriteUInt32(this.MoveCounter)`
  - `WriteFloat(this.HorizontalSpeed)`
  - `WriteFloat(this.VerticalSpeed)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteUInt32(this.MoveCounter);
base._worldPacket.WriteVector2(this.Direction);
base._worldPacket.WriteFloat(this.HorizontalSpeed);
base._worldPacket.WriteFloat(this.VerticalSpeed);
}
```

---

### MoveSetCollisionHeight

- File: HermesProxy/World/Server/Packets/MoveSetCollisionHeight.cs
- Fields:
  - `WriteUInt32(this.SequenceIndex)`
  - `WriteFloat(this.Height)`
  - `WriteFloat(this.Scale)`
  - `WriteUInt32(this.MountDisplayID)`
  - `WriteInt32(this.ScaleDuration)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteUInt32(this.SequenceIndex);
base._worldPacket.WriteFloat(this.Height);
base._worldPacket.WriteFloat(this.Scale);
base._worldPacket.WriteByteEnum(this.Reason);
base._worldPacket.WriteUInt32(this.MountDisplayID);
base._worldPacket.WriteInt32(this.ScaleDuration);
}
```

---

### MoveSetFlag

- File: HermesProxy/World/Server/Packets/MoveSetFlag.cs
- Fields:
  - `WriteUInt32(this.MoveCounter)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteUInt32(this.MoveCounter);
}
```

---

### MoveSetSpeed

- File: HermesProxy/World/Server/Packets/MoveSetSpeed.cs
- Fields:
  - `WriteUInt32(this.MoveCounter)`
  - `WriteFloat(this.Speed)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteUInt32(this.MoveCounter);
base._worldPacket.WriteFloat(this.Speed);
}
```

---

### MoveSplineSetFlag

- File: HermesProxy/World/Server/Packets/MoveSplineSetFlag.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
}
```

---

### MoveSplineSetSpeed

- File: HermesProxy/World/Server/Packets/MoveSplineSetSpeed.cs
- Fields:
  - `WriteFloat(this.Speed)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteFloat(this.Speed);
}
```

---

### MoveTeleport

- File: HermesProxy/World/Server/Packets/MoveTeleport.cs
- Fields:
  - `WriteUInt32(this.MoveCounter)`
  - `WriteVector3`
  - `WriteFloat(this.Orientation)`
  - `WriteUInt8(this.PreloadWorld)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.MoverGUID);
base._worldPacket.WriteUInt32(this.MoveCounter);
base._worldPacket.WriteVector3(this.Position);
base._worldPacket.WriteFloat(this.Orientation);
base._worldPacket.WriteUInt8(this.PreloadWorld);
base._worldPacket.WriteBit(this.TransportGUID != null);
base._worldPacket.WriteBit(this.Vehicle != null);
base._worldPacket.FlushBits();
if (this.Vehicle != null)
{
base._worldPacket.WriteInt8(this.Vehicle.VehicleSeatIndex);
base._worldPacket.WriteBit(this.Vehicle.VehicleExitVoluntary);
base._worldPacket.WriteBit(this.Vehicle.VehicleExitTeleport);
base._worldPacket.FlushBits();
}
if (this.TransportGUID != null)
{
base._worldPacket.WritePackedGuid128(this.TransportGUID);
}
}
```

---

### MoveUpdate

- File: HermesProxy/World/Server/Packets/MoveUpdate.cs

```csharp
{
this.MoveInfo.WriteMovementInfoModern(base._worldPacket, this.MoverGUID);
}
```

---

### MoveUpdateKnockBack

- File: HermesProxy/World/Server/Packets/MoveUpdateKnockBack.cs

```csharp
{
this.MoveInfo.WriteMovementInfoModern(base._worldPacket, this.MoverGUID);
}
```

---

### MoveUpdateSpeed

- File: HermesProxy/World/Server/Packets/MoveUpdateSpeed.cs
- Fields:
  - `WriteFloat(this.Speed)`

```csharp
{
this.MoveInfo.WriteMovementInfoModern(base._worldPacket, this.MoverGUID);
base._worldPacket.WriteFloat(this.Speed);
}
```

---

### NewTaxiPath

- File: HermesProxy/World/Server/Packets/NewTaxiPath.cs

```csharp
{
}
```

---

### NewWorld

- File: HermesProxy/World/Server/Packets/NewWorld.cs
- Fields:
  - `WriteUInt32(this.MapID)`
  - `WriteVector3`
  - `WriteFloat(this.Orientation)`
  - `WriteUInt32(this.Reason)`
  - `WriteVector3`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteVector3(this.Position);
base._worldPacket.WriteFloat(this.Orientation);
base._worldPacket.WriteUInt32(this.Reason);
base._worldPacket.WriteVector3(this.MovementOffset);
}
```

---

### NotifyReceivedMail

- File: HermesProxy/World/Server/Packets/NotifyReceivedMail.cs
- Fields:
  - `WriteFloat(this.Delay)`

```csharp
{
base._worldPacket.WriteFloat(this.Delay);
}
```

---

### PVPMatchStatisticsMessage

- File: HermesProxy/World/Server/Packets/PVPMatchStatisticsMessage.cs
- Fields:
  - `WriteInt32(this.Statistics.Count)`
  - `WriteUInt8(this.Winner.Value)`

```csharp
{
base._worldPacket.WriteBit(this.Ratings != null);
base._worldPacket.WriteBit(this.ArenaTeams != null);
base._worldPacket.WriteBit(this.Winner.HasValue);
if (this.ArenaTeams != null)
{
this.ArenaTeams.Write(base._worldPacket);
}
base._worldPacket.WriteInt32(this.Statistics.Count);
sbyte[] playerCount = this.PlayerCount;
foreach (sbyte count in playerCount)
{
base._worldPacket.WriteInt8(count);
}
if (this.Ratings != null)
{
this.Ratings.Write(base._worldPacket);
}
if (this.Winner.HasValue)
{
base._worldPacket.WriteUInt8(this.Winner.Value);
}
foreach (PVPMatchPlayerStatistics player in this.Statistics)
{
player.Write(base._worldPacket);
```

---

### PartyCommandResult

- File: HermesProxy/World/Server/Packets/PartyCommandResult.cs
- Fields:
  - `WriteUInt32(this.ResultData)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WriteBits(this.Name.GetByteCount(), 9);
base._worldPacket.WriteBits(this.Command, 4);
base._worldPacket.WriteBits(this.Result, 6);
base._worldPacket.WriteUInt32(this.ResultData);
base._worldPacket.WritePackedGuid128(this.ResultGUID);
base._worldPacket.WriteString(this.Name);
}
```

---

### PartyInvite

- File: HermesProxy/World/Server/Packets/PartyInvite.cs
- Fields:
  - `WriteUInt16(this.Unk1)`
  - `WriteUInt32(this.ProposedRoles)`
  - `WriteInt32(this.LfgSlots.Count)`
  - `WriteInt32(this.LfgCompletedMask)`
  - `WriteString(this.InviterName)`
  - `WriteInt32(LfgSlot)`

```csharp
{
base._worldPacket.WriteBit(this.CanAccept);
base._worldPacket.WriteBit(this.MightCRZYou);
base._worldPacket.WriteBit(this.IsXRealm);
base._worldPacket.WriteBit(this.MustBeBNetFriend);
base._worldPacket.WriteBit(this.AllowMultipleRoles);
base._worldPacket.WriteBit(this.QuestSessionActive);
base._worldPacket.WriteBits(this.InviterName.GetByteCount(), 6);
this.InviterRealm.Write(base._worldPacket);
base._worldPacket.WritePackedGuid128(this.InviterGUID);
base._worldPacket.WritePackedGuid128(this.InviterBNetAccountId);
base._worldPacket.WriteUInt16(this.Unk1);
base._worldPacket.WriteUInt32(this.ProposedRoles);
base._worldPacket.WriteInt32(this.LfgSlots.Count);
base._worldPacket.WriteInt32(this.LfgCompletedMask);
base._worldPacket.WriteString(this.InviterName);
foreach (int LfgSlot in this.LfgSlots)
{
base._worldPacket.WriteInt32(LfgSlot);
}
}
```

---

### PartyKillLog

- File: HermesProxy/World/Server/Packets/PartyKillLog.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Player);
base._worldPacket.WritePackedGuid128(this.Victim);
}
```

---

### PartyMemberFullState

- File: HermesProxy/World/Server/Packets/PartyMemberFullState.cs
- Fields:
  - `WriteInt16((short)`
  - `WriteUInt8(this.PowerType)`
  - `WriteInt16((short)`
  - `WriteInt32(this.CurrentHealth)`
  - `WriteInt32(this.MaxHealth)`
  - `WriteUInt16(this.CurrentPower)`
  - `WriteUInt16(this.MaxPower)`
  - `WriteUInt16(this.Level)`
  - `WriteUInt16(this.SpecID)`
  - `WriteUInt16(this.ZoneID)`
  - `WriteUInt16(this.WmoGroupID)`
  - `WriteInt32(this.WmoDoodadPlacementID)`
  - `WriteInt16(this.PositionX)`
  - `WriteInt16(this.PositionY)`
  - `WriteInt16(this.PositionZ)`
  - `WriteInt32(this.VehicleSeat)`
  - `WriteInt32(this.Auras.Count)`

```csharp
{
base._worldPacket.WriteBit(this.ForEnemy);
base._worldPacket.FlushBits();
for (byte i = 0; i < 2; i++)
{
base._worldPacket.WriteInt8(this.PartyType[i]);
}
base._worldPacket.WriteInt16((short)this.StatusFlags);
base._worldPacket.WriteUInt8(this.PowerType);
base._worldPacket.WriteInt16((short)this.PowerDisplayID);
base._worldPacket.WriteInt32(this.CurrentHealth);
base._worldPacket.WriteInt32(this.MaxHealth);
base._worldPacket.WriteUInt16(this.CurrentPower);
base._worldPacket.WriteUInt16(this.MaxPower);
base._worldPacket.WriteUInt16(this.Level);
base._worldPacket.WriteUInt16(this.SpecID);
base._worldPacket.WriteUInt16(this.ZoneID);
base._worldPacket.WriteUInt16(this.WmoGroupID);
base._worldPacket.WriteInt32(this.WmoDoodadPlacementID);
base._worldPacket.WriteInt16(this.PositionX);
base._worldPacket.WriteInt16(this.PositionY);
base._worldPacket.WriteInt16(this.PositionZ);
base._worldPacket.WriteInt32(this.VehicleSeat);
base._worldPacket.WriteInt32(this.Auras.Count);
this.Phases.Write(base._worldPacket);
```

---

### PartyMemberPartialState

- File: HermesProxy/World/Server/Packets/PartyMemberPartialState.cs
- Fields:
  - `WriteUInt8(this.PartyType.PartyType1)`
  - `WriteUInt8(this.PartyType.PartyType2)`
  - `WriteUInt16(this.StatusFlags.Value)`
  - `WriteUInt8(this.PowerType.Value)`
  - `WriteUInt16(this.OverrideDisplayPower.Value)`
  - `WriteUInt32(this.CurrentHealth.Value)`
  - `WriteUInt32(this.MaxHealth.Value)`
  - `WriteUInt16(this.CurrentPower.Value)`
  - `WriteUInt16(this.MaxPower.Value)`
  - `WriteUInt16(this.Level.Value)`
  - `WriteUInt16(this.Spec.Value)`
  - `WriteUInt16(this.ZoneID.Value)`
  - `WriteUInt16(this.WmoGroupID.Value)`
  - `WriteUInt32(this.WmoDoodadPlacementID.Value)`
  - `WriteInt16(this.Position.X)`
  - `WriteInt16(this.Position.Y)`
  - `WriteInt16(this.Position.Z)`
  - `WriteUInt32(this.VehicleSeatRecID.Value)`
  - `WriteInt32(this.Auras.Count)`

```csharp
{
base._worldPacket.WriteBit(this.ForEnemyChanged);
base._worldPacket.WriteBit(this.SetPvPInactive);
base._worldPacket.WriteBit(this.Unk901_1);
base._worldPacket.WriteBit(this.PartyType != null);
base._worldPacket.WriteBit(this.StatusFlags.HasValue);
base._worldPacket.WriteBit(this.PowerType.HasValue);
base._worldPacket.WriteBit(this.OverrideDisplayPower.HasValue);
base._worldPacket.WriteBit(this.CurrentHealth.HasValue);
base._worldPacket.WriteBit(this.MaxHealth.HasValue);
base._worldPacket.WriteBit(this.CurrentPower.HasValue);
base._worldPacket.WriteBit(this.MaxPower.HasValue);
base._worldPacket.WriteBit(this.Level.HasValue);
base._worldPacket.WriteBit(this.Spec.HasValue);
base._worldPacket.WriteBit(this.ZoneID.HasValue);
base._worldPacket.WriteBit(this.WmoGroupID.HasValue);
base._worldPacket.WriteBit(this.WmoDoodadPlacementID.HasValue);
base._worldPacket.WriteBit(this.Position != null);
base._worldPacket.WriteBit(this.VehicleSeatRecID.HasValue);
base._worldPacket.WriteBit(this.Auras != null);
base._worldPacket.WriteBit(this.Pet != null);
base._worldPacket.WriteBit(this.Phase != null);
base._worldPacket.WriteBit(this.Unk901_2 != null);
base._worldPacket.FlushBits();
if (this.Pet != null)
```

---

### PartyUpdate

- File: HermesProxy/World/Server/Packets/PartyUpdate.cs
- Fields:
  - `WriteUInt16((ushort)`
  - `WriteUInt8(this.PartyIndex)`
  - `WriteUInt8((byte)`
  - `WriteInt32(this.MyIndex)`
  - `WriteInt32(this.SequenceNum)`
  - `WriteInt32(this.PlayerList.Count)`

```csharp
{
base._worldPacket.WriteUInt16((ushort)this.PartyFlags);
base._worldPacket.WriteUInt8(this.PartyIndex);
base._worldPacket.WriteUInt8((byte)this.PartyType);
base._worldPacket.WriteInt32(this.MyIndex);
base._worldPacket.WritePackedGuid128(this.PartyGUID);
base._worldPacket.WriteInt32(this.SequenceNum);
base._worldPacket.WritePackedGuid128(this.LeaderGUID);
base._worldPacket.WriteInt32(this.PlayerList.Count);
base._worldPacket.WriteBit(this.LfgInfos != null);
base._worldPacket.WriteBit(this.LootSettings != null);
base._worldPacket.WriteBit(this.DifficultySettings != null);
base._worldPacket.FlushBits();
foreach (PartyPlayerInfo player in this.PlayerList)
{
player.Write(base._worldPacket);
}
if (this.LootSettings != null)
{
this.LootSettings.Write(base._worldPacket);
}
if (this.DifficultySettings != null)
{
this.DifficultySettings.Write(base._worldPacket);
}
```

---

### PauseMirrorTimer

- File: HermesProxy/World/Server/Packets/PauseMirrorTimer.cs
- Fields:
  - `WriteInt32((int)`

```csharp
{
base._worldPacket.WriteInt32((int)this.Timer);
base._worldPacket.WriteBit(this.Paused);
base._worldPacket.FlushBits();
}
```

---

### PetActionSound

- File: HermesProxy/World/Server/Packets/PetActionSound.cs
- Fields:
  - `WriteUInt32(this.Action)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.UnitGUID);
base._worldPacket.WriteUInt32(this.Action);
}
```

---

### PetCastFailed

- File: HermesProxy/World/Server/Packets/PetCastFailed.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.Reason)`
  - `WriteInt32(this.FailedArg1)`
  - `WriteInt32(this.FailedArg2)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CastID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.Reason);
base._worldPacket.WriteInt32(this.FailedArg1);
base._worldPacket.WriteInt32(this.FailedArg2);
}
```

---

### PetClearSpells

- File: HermesProxy/World/Server/Packets/PetClearSpells.cs

```csharp
{
}
```

---

### PetGuids

- File: HermesProxy/World/Server/Packets/PetGuids.cs
- Fields:
  - `WriteInt32(this.Guids.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Guids.Count);
foreach (WowGuid128 guid in this.Guids)
{
base._worldPacket.WritePackedGuid128(guid);
}
}
```

---

### PetSpells

- File: HermesProxy/World/Server/Packets/PetSpells.cs
- Fields:
  - `WriteUInt16(this.CreatureFamily)`
  - `WriteInt16(this.Specialization)`
  - `WriteUInt32(this.TimeLimit)`
  - `WriteUInt16((ushort)`
  - `WriteUInt8((byte)`
  - `WriteUInt32(actionButton)`
  - `WriteInt32(this.Actions.Count)`
  - `WriteInt32(this.Cooldowns.Count)`
  - `WriteInt32(this.SpellHistory.Count)`
  - `WriteUInt32(action)`
  - `WriteUInt32(cooldown.SpellID)`
  - `WriteUInt32(cooldown.Duration)`
  - `WriteUInt32(cooldown.CategoryDuration)`
  - `WriteFloat(cooldown.ModRate)`
  - `WriteUInt16(cooldown.Category)`
  - `WriteUInt32(history.CategoryID)`
  - `WriteUInt32(history.RecoveryTime)`
  - `WriteFloat(history.ChargeModRate)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PetGUID);
base._worldPacket.WriteUInt16(this.CreatureFamily);
base._worldPacket.WriteInt16(this.Specialization);
base._worldPacket.WriteUInt32(this.TimeLimit);
base._worldPacket.WriteUInt16((ushort)((byte)this.CommandState | (this.Flag << 16)));
base._worldPacket.WriteUInt8((byte)this.ReactState);
uint[] actionButtons = this.ActionButtons;
foreach (uint actionButton in actionButtons)
{
base._worldPacket.WriteUInt32(actionButton);
}
base._worldPacket.WriteInt32(this.Actions.Count);
base._worldPacket.WriteInt32(this.Cooldowns.Count);
base._worldPacket.WriteInt32(this.SpellHistory.Count);
foreach (uint action in this.Actions)
{
base._worldPacket.WriteUInt32(action);
}
foreach (PetSpellCooldown cooldown in this.Cooldowns)
{
base._worldPacket.WriteUInt32(cooldown.SpellID);
base._worldPacket.WriteUInt32(cooldown.Duration);
base._worldPacket.WriteUInt32(cooldown.CategoryDuration);
base._worldPacket.WriteFloat(cooldown.ModRate);
```

---

### PetStableList

- File: HermesProxy/World/Server/Packets/PetStableList.cs
- Fields:
  - `WriteInt32(this.Pets.Count)`
  - `WriteUInt8(this.NumStableSlots)`
  - `WriteUInt32(pet.PetNumber)`
  - `WriteUInt32(pet.CreatureID)`
  - `WriteUInt32(pet.DisplayID)`
  - `WriteUInt32(pet.ExperienceLevel)`
  - `WriteUInt8(pet.LoyaltyLevel)`
  - `WriteUInt8(pet.PetFlags)`
  - `WriteString(pet.PetName)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.StableMaster);
base._worldPacket.WriteInt32(this.Pets.Count);
base._worldPacket.WriteUInt8(this.NumStableSlots);
foreach (PetStableInfo pet in this.Pets)
{
base._worldPacket.WriteUInt32(pet.PetNumber);
base._worldPacket.WriteUInt32(pet.CreatureID);
base._worldPacket.WriteUInt32(pet.DisplayID);
base._worldPacket.WriteUInt32(pet.ExperienceLevel);
base._worldPacket.WriteUInt8(pet.LoyaltyLevel);
base._worldPacket.WriteUInt8(pet.PetFlags);
base._worldPacket.WriteBits(pet.PetName.GetByteCount(), 8);
base._worldPacket.WriteString(pet.PetName);
}
}
```

---

### PetStableResult

- File: HermesProxy/World/Server/Packets/PetStableResult.cs
- Fields:
  - `WriteUInt8(this.Result)`

```csharp
{
base._worldPacket.WriteUInt8(this.Result);
}
```

---

### PetitionRenameGuildResponse

- File: HermesProxy/World/Server/Packets/PetitionRenameGuildResponse.cs
- Fields:
  - `WriteString(this.NewGuildName)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.PetitionGuid);
base._worldPacket.WriteBits(this.NewGuildName.GetByteCount(), 7);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.NewGuildName);
}
```

---

### PetitionSignResults

- File: HermesProxy/World/Server/Packets/PetitionSignResults.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Item);
base._worldPacket.WritePackedGuid128(this.Player);
base._worldPacket.WriteBits(this.Error, 4);
base._worldPacket.FlushBits();
}
```

---

### PhaseShiftChange

- File: HermesProxy/World/Server/Packets/PhaseShiftChange.cs
- Fields:
  - `WriteUInt32(this.PhaseShiftFlags)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`
  - `WriteUInt32(0u)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Client);
base._worldPacket.WriteUInt32(this.PhaseShiftFlags);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WritePackedGuid128(WowGuid128.Empty);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt32(0u);
base._worldPacket.WriteUInt32(0u);
}
```

---

### PlayMusic

- File: HermesProxy/World/Server/Packets/PlayMusic.cs
- Fields:
  - `WriteUInt32(this.SoundEntryID)`

```csharp
{
base._worldPacket.WriteUInt32(this.SoundEntryID);
}
```

---

### PlayObjectSound

- File: HermesProxy/World/Server/Packets/PlayObjectSound.cs
- Fields:
  - `WriteUInt32(this.SoundEntryID)`
  - `WriteVector3`
  - `WriteInt32(this.BroadcastTextID)`

```csharp
{
base._worldPacket.WriteUInt32(this.SoundEntryID);
base._worldPacket.WritePackedGuid128(this.SourceObjectGUID);
base._worldPacket.WritePackedGuid128(this.TargetObjectGUID);
base._worldPacket.WriteVector3(this.Position);
base._worldPacket.WriteInt32(this.BroadcastTextID);
}
```

---

### PlaySound

- File: HermesProxy/World/Server/Packets/PlaySound.cs
- Fields:
  - `WriteUInt32(this.SoundEntryID)`
  - `WriteInt32(this.BroadcastTextId)`

```csharp
{
base._worldPacket.WriteUInt32(this.SoundEntryID);
base._worldPacket.WritePackedGuid128(this.SourceObjectGuid);
base._worldPacket.WriteInt32(this.BroadcastTextId);
}
```

---

### PlaySpellVisualKit

- File: HermesProxy/World/Server/Packets/PlaySpellVisualKit.cs
- Fields:
  - `WriteUInt32(this.KitRecID)`
  - `WriteUInt32(this.KitType)`
  - `WriteUInt32(this.Duration)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Unit);
base._worldPacket.WriteUInt32(this.KitRecID);
base._worldPacket.WriteUInt32(this.KitType);
base._worldPacket.WriteUInt32(this.Duration);
base._worldPacket.WriteBit(this.MountedVisual);
base._worldPacket.FlushBits();
}
```

---

### PlayedTime

- File: HermesProxy/World/Server/Packets/PlayedTime.cs
- Fields:
  - `WriteUInt32(this.TotalTime)`
  - `WriteUInt32(this.LevelTime)`

```csharp
{
base._worldPacket.WriteUInt32(this.TotalTime);
base._worldPacket.WriteUInt32(this.LevelTime);
base._worldPacket.WriteBit(this.TriggerEvent);
base._worldPacket.FlushBits();
}
```

---

### PlayerBound

- File: HermesProxy/World/Server/Packets/PlayerBound.cs
- Fields:
  - `WriteUInt32(this.AreaID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.BinderGUID);
base._worldPacket.WriteUInt32(this.AreaID);
}
```

---

### PlayerSaveGuildEmblem

- File: HermesProxy/World/Server/Packets/PlayerSaveGuildEmblem.cs
- Fields:
  - `WriteUInt32((uint)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Error);
}
```

---

### PlayerSkinned

- File: HermesProxy/World/Server/Packets/PlayerSkinned.cs

```csharp
{
base._worldPacket.WriteBit(this.FreeRepop);
base._worldPacket.FlushBits();
}
```

---

### PlayerTabardVendorActivate

- File: HermesProxy/World/Server/Packets/PlayerTabardVendorActivate.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.DesignerGUID);
}
```

---

### Pong

- File: HermesProxy/World/Server/Packets/Pong.cs
- Fields:
  - `WriteUInt32(this.Serial)`

```csharp
{
base._worldPacket.WriteUInt32(this.Serial);
}
```

---

### PowerUpdate

- File: HermesProxy/World/Server/Packets/PowerUpdate.cs
- Fields:
  - `WriteInt32(this.Powers.Count)`
  - `WriteInt32(power.Power)`
  - `WriteUInt8(power.PowerType)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteInt32(this.Powers.Count);
foreach (PowerUpdatePower power in this.Powers)
{
base._worldPacket.WriteInt32(power.Power);
base._worldPacket.WriteUInt8(power.PowerType);
}
}
```

---

### PrintNotification

- File: HermesProxy/World/Server/Packets/PrintNotification.cs
- Fields:
  - `WriteString(this.NotifyText)`

```csharp
{
base._worldPacket.WriteBits(this.NotifyText.GetByteCount(), 12);
base._worldPacket.WriteString(this.NotifyText);
}
```

---

### PvPCredit

- File: HermesProxy/World/Server/Packets/PvPCredit.cs
- Fields:
  - `WriteInt32(this.OriginalHonor)`
  - `WriteInt32(this.Honor)`
  - `WriteUInt32(this.Rank)`

```csharp
{
base._worldPacket.WriteInt32(this.OriginalHonor);
base._worldPacket.WriteInt32(this.Honor);
base._worldPacket.WritePackedGuid128(this.Target);
base._worldPacket.WriteUInt32(this.Rank);
}
```

---

### QueryCreatureResponse

- File: HermesProxy/World/Server/Packets/QueryCreatureResponse.cs
- Fields:
  - `WriteUInt32(this.CreatureID)`
  - `WriteCString(this.Stats.Name[j])`
  - `WriteCString(this.Stats.NameAlt[j])`
  - `WriteUInt32(this.Stats.Flags[k])`
  - `WriteInt32(this.Stats.Type)`
  - `WriteInt32(this.Stats.Family)`
  - `WriteInt32(this.Stats.Classification)`
  - `WriteUInt32(this.Stats.PetSpellDataId)`
  - `WriteUInt32(this.Stats.ProxyCreatureID[l])`
  - `WriteInt32(this.Stats.Display.CreatureDisplay.Count)`
  - `WriteFloat(this.Stats.Display.TotalProbability)`
  - `WriteUInt32(display.CreatureDisplayID)`
  - `WriteFloat(display.Scale)`
  - `WriteFloat(display.Probability)`
  - `WriteFloat(this.Stats.HpMulti)`
  - `WriteFloat(this.Stats.EnergyMulti)`
  - `WriteInt32(this.Stats.QuestItems.Count)`
  - `WriteUInt32(this.Stats.MovementInfoID)`
  - `WriteInt32(this.Stats.HealthScalingExpansion)`
  - `WriteUInt32(this.Stats.RequiredExpansion)`
  - `WriteUInt32(this.Stats.VignetteID)`
  - `WriteInt32(this.Stats.Class)`
  - `WriteInt32(this.Stats.DifficultyID)`
  - `WriteInt32(this.Stats.WidgetSetID)`
  - `WriteInt32(this.Stats.WidgetSetUnitConditionID)`
  - `WriteCString(this.Stats.Title)`
  - `WriteCString(this.Stats.TitleAlt)`
  - `WriteCString(this.Stats.CursorName)`
  - `WriteUInt32(questItem)`

```csharp
{
base._worldPacket.WriteUInt32(this.CreatureID);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.FlushBits();
if (!this.Allow)
{
return;
}
base._worldPacket.WriteBits((!this.Stats.Title.IsEmpty()) ? (this.Stats.Title.GetByteCount() + 1) : 0, 11);
base._worldPacket.WriteBits((!this.Stats.TitleAlt.IsEmpty()) ? (this.Stats.TitleAlt.GetByteCount() + 1) : 0, 11);
base._worldPacket.WriteBits((!this.Stats.CursorName.IsEmpty()) ? (this.Stats.CursorName.GetByteCount() + 1) : 0, 6);
base._worldPacket.WriteBit(this.Stats.Civilian);
base._worldPacket.WriteBit(this.Stats.Leader);
for (int i = 0; i < 4; i++)
{
base._worldPacket.WriteBits(this.Stats.Name[i].GetByteCount() + 1, 11);
base._worldPacket.WriteBits(this.Stats.NameAlt[i].GetByteCount() + 1, 11);
}
for (int j = 0; j < 4; j++)
{
if (!string.IsNullOrEmpty(this.Stats.Name[j]))
{
base._worldPacket.WriteCString(this.Stats.Name[j]);
}
if (!string.IsNullOrEmpty(this.Stats.NameAlt[j]))
```

---

### QueryGameObjectResponse

- File: HermesProxy/World/Server/Packets/QueryGameObjectResponse.cs
- Fields:
  - `WriteUInt32(this.GameObjectID)`
  - `WriteUInt32(this.Stats.Type)`
  - `WriteUInt32(this.Stats.DisplayID)`
  - `WriteCString(this.Stats.Name[i])`
  - `WriteCString(this.Stats.IconName)`
  - `WriteCString(this.Stats.CastBarCaption)`
  - `WriteCString(this.Stats.UnkString)`
  - `WriteInt32(this.Stats.Data[j])`
  - `WriteFloat(this.Stats.Size)`
  - `WriteUInt8((byte)`
  - `WriteUInt32(questItem)`
  - `WriteUInt32(this.Stats.ContentTuningId)`
  - `WriteUInt32(statsData.GetSize()`
  - `WriteBytes(statsData)`

```csharp
{
base._worldPacket.WriteUInt32(this.GameObjectID);
base._worldPacket.WritePackedGuid128(this.Guid);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.FlushBits();
ByteBuffer statsData = new ByteBuffer();
if (this.Allow)
{
statsData.WriteUInt32(this.Stats.Type);
statsData.WriteUInt32(this.Stats.DisplayID);
for (int i = 0; i < 4; i++)
{
statsData.WriteCString(this.Stats.Name[i]);
}
statsData.WriteCString(this.Stats.IconName);
statsData.WriteCString(this.Stats.CastBarCaption);
statsData.WriteCString(this.Stats.UnkString);
int dataFieldsCount = (ModernVersion.AddedInClassicVersion(1, 14, 1, 2, 5, 3) ? 35 : 34);
for (int j = 0; j < dataFieldsCount; j++)
{
statsData.WriteInt32(this.Stats.Data[j]);
}
statsData.WriteFloat(this.Stats.Size);
statsData.WriteUInt8((byte)this.Stats.QuestItems.Count);
foreach (uint questItem in this.Stats.QuestItems)
```

---

### QueryGuildInfoResponse

- File: HermesProxy/World/Server/Packets/QueryGuildInfoResponse.cs
- Fields:
  - `WriteUInt32(this.Info.VirtualRealmAddress)`
  - `WriteInt32(this.Info.Ranks.Count)`
  - `WriteUInt32(this.Info.EmblemStyle)`
  - `WriteUInt32(this.Info.EmblemColor)`
  - `WriteUInt32(this.Info.BorderStyle)`
  - `WriteUInt32(this.Info.BorderColor)`
  - `WriteUInt32(this.Info.BackgroundColor)`
  - `WriteUInt32(rank.RankID)`
  - `WriteUInt32(rank.RankOrder)`
  - `WriteString(rank.RankName)`
  - `WriteString(this.Info.GuildName)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.GuildGUID);
if (ModernVersion.RemovedInVersion(9, 2, 0, 1, 14, 2, 2, 5, 3))
{
base._worldPacket.WritePackedGuid128(this.PlayerGuid);
}
base._worldPacket.WriteBit(this.HasGuildInfo);
base._worldPacket.FlushBits();
if (!this.HasGuildInfo)
{
return;
}
base._worldPacket.WritePackedGuid128(this.Info.GuildGuid);
base._worldPacket.WriteUInt32(this.Info.VirtualRealmAddress);
base._worldPacket.WriteInt32(this.Info.Ranks.Count);
base._worldPacket.WriteUInt32(this.Info.EmblemStyle);
base._worldPacket.WriteUInt32(this.Info.EmblemColor);
base._worldPacket.WriteUInt32(this.Info.BorderStyle);
base._worldPacket.WriteUInt32(this.Info.BorderColor);
base._worldPacket.WriteUInt32(this.Info.BackgroundColor);
base._worldPacket.WriteBits(this.Info.GuildName.GetByteCount(), 7);
base._worldPacket.FlushBits();
foreach (GuildInfo.RankInfo rank in this.Info.Ranks)
{
base._worldPacket.WriteUInt32(rank.RankID);
```

---

### QueryNPCTextResponse

- File: HermesProxy/World/Server/Packets/QueryNPCTextResponse.cs
- Fields:
  - `WriteUInt32(this.TextID)`
  - `WriteInt32(this.Allow ? 64 : 0)`
  - `WriteFloat(this.Probabilities[i])`
  - `WriteUInt32(this.BroadcastTextID[i2])`

```csharp
{
base._worldPacket.WriteUInt32(this.TextID);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.FlushBits();
base._worldPacket.WriteInt32(this.Allow ? 64 : 0);
if (this.Allow)
{
for (uint i = 0u; i < 8; i++)
{
base._worldPacket.WriteFloat(this.Probabilities[i]);
}
for (uint i2 = 0u; i2 < 8; i2++)
{
base._worldPacket.WriteUInt32(this.BroadcastTextID[i2]);
}
}
}
```

---

### QueryPageTextResponse

- File: HermesProxy/World/Server/Packets/QueryPageTextResponse.cs
- Fields:
  - `WriteUInt32(this.PageTextID)`
  - `WriteInt32(this.Pages.Count)`

```csharp
{
base._worldPacket.WriteUInt32(this.PageTextID);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.FlushBits();
if (!this.Allow)
{
return;
}
base._worldPacket.WriteInt32(this.Pages.Count);
foreach (PageTextInfo page in this.Pages)
{
page.Write(base._worldPacket);
}
}
```

---

### QueryPetNameResponse

- File: HermesProxy/World/Server/Packets/QueryPetNameResponse.cs
- Fields:
  - `WriteString(this.DeclinedNames.name[i2])`
  - `WriteInt64(this.Timestamp)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.UnitGUID);
base._worldPacket.WriteBit(this.Allow);
if (this.Allow)
{
base._worldPacket.WriteBits(this.Name.GetByteCount(), 8);
base._worldPacket.WriteBit(this.HasDeclined);
for (byte i = 0; i < 5; i++)
{
base._worldPacket.WriteBits(this.DeclinedNames.name[i].GetByteCount(), 7);
}
for (byte i2 = 0; i2 < 5; i2++)
{
base._worldPacket.WriteString(this.DeclinedNames.name[i2]);
}
base._worldPacket.WriteInt64(this.Timestamp);
base._worldPacket.WriteString(this.Name);
}
base._worldPacket.FlushBits();
}
```

---

### QueryPetitionResponse

- File: HermesProxy/World/Server/Packets/QueryPetitionResponse.cs
- Fields:
  - `WriteUInt32(this.PetitionID)`

```csharp
{
base._worldPacket.WriteUInt32(this.PetitionID);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.FlushBits();
if (this.Allow)
{
this.Info.Write(base._worldPacket);
}
}
```

---

### QueryPlayerNameResponse

- File: HermesProxy/World/Server/Packets/QueryPlayerNameResponse.cs

```csharp
{
base._worldPacket.WriteInt8((sbyte)this.Result);
base._worldPacket.WritePackedGuid128(this.Player);
if (this.Result == 0)
{
this.Data.Write(base._worldPacket);
}
}
```

---

### QueryPlayerNamesResponse

- File: HermesProxy/World/Server/Packets/QueryPlayerNamesResponse.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteUInt8(result.Result)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Players.Count);
foreach (NameCacheLookupResult result in this.Players)
{
base._worldPacket.WriteUInt8(result.Result);
base._worldPacket.WritePackedGuid128(result.Player);
base._worldPacket.WriteBit(result.Result == 0 && result.Data != null); // hasData
base._worldPacket.WriteBit(false); // hasUnused920
base._worldPacket.FlushBits();
if (result.Result == 0 && result.Data != null)
{
result.Data.Write(base._worldPacket);
}
}
}
```

---

### QueryQuestInfoResponse

- File: HermesProxy/World/Server/Packets/QueryQuestInfoResponse.cs
- Fields:
  - `WriteUInt32(this.QuestID)`
  - `WriteUInt32(this.Info.QuestID)`
  - `WriteInt32(this.Info.QuestType)`
  - `WriteInt32(this.Info.QuestLevel)`
  - `WriteInt32(this.Info.QuestScalingFactionGroup)`
  - `WriteInt32(this.Info.QuestMaxScalingLevel)`
  - `WriteUInt32(this.Info.QuestPackageID)`
  - `WriteInt32(this.Info.MinLevel)`
  - `WriteInt32(this.Info.QuestSortID)`
  - `WriteUInt32(this.Info.QuestInfoID)`
  - `WriteUInt32(this.Info.SuggestedGroupNum)`
  - `WriteUInt32(this.Info.RewardNextQuest)`
  - `WriteUInt32(this.Info.RewardXPDifficulty)`
  - `WriteFloat(this.Info.RewardXPMultiplier)`
  - `WriteInt32(this.Info.RewardMoney)`
  - `WriteUInt32(this.Info.RewardMoneyDifficulty)`
  - `WriteFloat(this.Info.RewardMoneyMultiplier)`
  - `WriteUInt32(this.Info.RewardBonusMoney)`
  - `WriteUInt32(this.Info.RewardDisplaySpell[i])`
  - `WriteUInt32(this.Info.RewardSpell)`
  - `WriteUInt32(this.Info.RewardHonor)`
  - `WriteFloat(this.Info.RewardKillHonor)`
  - `WriteInt32(this.Info.RewardArtifactXPDifficulty)`
  - `WriteFloat(this.Info.RewardArtifactXPMultiplier)`
  - `WriteInt32(this.Info.RewardArtifactCategoryID)`
  - `WriteUInt32(this.Info.StartItem)`
  - `WriteUInt32(this.Info.Flags)`
  - `WriteUInt32(this.Info.FlagsEx)`
  - `WriteUInt32(this.Info.FlagsEx2)`
  - `WriteUInt32(this.Info.RewardItems[i2])`
  - `WriteUInt32(this.Info.RewardAmount[i2])`
  - `WriteInt32(this.Info.ItemDrop[i2])`
  - `WriteInt32(this.Info.ItemDropQuantity[i2])`
  - `WriteUInt32(this.Info.UnfilteredChoiceItems[i3].ItemID)`
  - `WriteUInt32(this.Info.UnfilteredChoiceItems[i3].Quantity)`
  - `WriteUInt32(this.Info.UnfilteredChoiceItems[i3].DisplayID)`
  - `WriteUInt32(this.Info.POIContinent)`
  - `WriteFloat(this.Info.POIx)`
  - `WriteFloat(this.Info.POIy)`
  - `WriteUInt32(this.Info.POIPriority)`
  - `WriteUInt32(this.Info.RewardTitle)`
  - `WriteInt32(this.Info.RewardArenaPoints)`
  - `WriteUInt32(this.Info.RewardSkillLineID)`
  - `WriteUInt32(this.Info.RewardNumSkillUps)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteUInt32(this.Info.RewardFactionID[i4])`
  - `WriteInt32(this.Info.RewardFactionValue[i4])`
  - `WriteInt32(this.Info.RewardFactionOverride[i4])`
  - `WriteInt32(this.Info.RewardFactionCapIn[i4])`
  - `WriteUInt32(this.Info.RewardFactionFlags)`
  - `WriteUInt32(this.Info.RewardCurrencyID[i5])`
  - `WriteUInt32(this.Info.RewardCurrencyQty[i5])`
  - `WriteUInt32(this.Info.AcceptedSoundKitID)`
  - `WriteUInt32(this.Info.CompleteSoundKitID)`
  - `WriteInt32((int)`
  - `WriteInt64(this.Info.TimeAllowed)`
  - `WriteInt32(this.Info.Objectives.Count)`
  - `WriteUInt64((ulong)`
  - `WriteInt32(this.Info.TreasurePickerID)`
  - `WriteInt32(this.Info.Expansion)`
  - `WriteInt32(this.Info.ManagedWorldStateID)`
  - `WriteInt32(this.Info.QuestSessionBonus)`
  - `WriteInt32(this.Info.QuestGiverCreatureID)`
  - `WriteUInt32(questObjective.Id)`
  - `WriteUInt8((byte)`
  - `WriteInt32(questObjective.ObjectID)`
  - `WriteInt32(questObjective.Amount)`
  - `WriteUInt32((uint)`
  - `WriteUInt32(questObjective.Flags2)`
  - `WriteFloat(questObjective.ProgressBarWeight)`
  - `WriteInt32(questObjective.VisualEffects.Length)`
  - `WriteInt32(visualEffect)`
  - `WriteString(questObjective.Description)`
  - `WriteString(this.Info.LogTitle)`
  - `WriteString(this.Info.LogDescription)`
  - `WriteString(this.Info.QuestDescription)`
  - `WriteString(this.Info.AreaDescription)`
  - `WriteString(this.Info.PortraitGiverText)`
  - `WriteString(this.Info.PortraitGiverName)`
  - `WriteString(this.Info.PortraitTurnInText)`
  - `WriteString(this.Info.PortraitTurnInName)`
  - `WriteString(this.Info.QuestCompletionLog)`

```csharp
{
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WriteBit(this.Allow);
base._worldPacket.FlushBits();
if (!this.Allow)
{
return;
}
base._worldPacket.WriteUInt32(this.Info.QuestID);
base._worldPacket.WriteInt32(this.Info.QuestType);
base._worldPacket.WriteInt32(this.Info.QuestLevel);
base._worldPacket.WriteInt32(this.Info.QuestScalingFactionGroup);
base._worldPacket.WriteInt32(this.Info.QuestMaxScalingLevel);
base._worldPacket.WriteUInt32(this.Info.QuestPackageID);
base._worldPacket.WriteInt32(this.Info.MinLevel);
base._worldPacket.WriteInt32(this.Info.QuestSortID);
base._worldPacket.WriteUInt32(this.Info.QuestInfoID);
base._worldPacket.WriteUInt32(this.Info.SuggestedGroupNum);
base._worldPacket.WriteUInt32(this.Info.RewardNextQuest);
base._worldPacket.WriteUInt32(this.Info.RewardXPDifficulty);
base._worldPacket.WriteFloat(this.Info.RewardXPMultiplier);
base._worldPacket.WriteInt32(this.Info.RewardMoney);
base._worldPacket.WriteUInt32(this.Info.RewardMoneyDifficulty);
base._worldPacket.WriteFloat(this.Info.RewardMoneyMultiplier);
base._worldPacket.WriteUInt32(this.Info.RewardBonusMoney);
```

---

### QueryTimeResponse

- File: HermesProxy/World/Server/Packets/QueryTimeResponse.cs
- Fields:
  - `WriteInt64(this.CurrentTime)`

```csharp
{
base._worldPacket.WriteInt64(this.CurrentTime);
}
```

---

### QuestConfirmAccept

- File: HermesProxy/World/Server/Packets/QuestConfirmAccept.cs
- Fields:
  - `WriteUInt32(this.QuestID)`
  - `WriteString(this.QuestTitle)`

```csharp
{
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WritePackedGuid128(this.InitiatedBy);
base._worldPacket.WriteBits(this.QuestTitle.GetByteCount(), 10);
base._worldPacket.WriteString(this.QuestTitle);
}
```

---

### QuestForceRemoved

- File: HermesProxy/World/Server/Packets/QuestForceRemoved.cs
- Fields:
  - `WriteInt32((int)`

```csharp
{
base._worldPacket.WriteInt32((int)this.QuestID);
}
```

---

### QuestGiverInvalidQuest

- File: HermesProxy/World/Server/Packets/QuestGiverInvalidQuest.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteInt32(this.ContributionRewardID)`
  - `WriteString(this.ReasonText)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.Reason);
base._worldPacket.WriteInt32(this.ContributionRewardID);
base._worldPacket.WriteBit(this.SendErrorMessage);
base._worldPacket.WriteBits(this.ReasonText.GetByteCount(), 9);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.ReasonText);
}
```

---

### QuestGiverOfferRewardMessage

- File: HermesProxy/World/Server/Packets/QuestGiverOfferRewardMessage.cs
- Fields:
  - `WriteInt32(this.QuestPackageID)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32(this.QuestGiverCreatureID)`
  - `WriteUInt32(0u)`
  - `WriteString(this.QuestTitle)`
  - `WriteString(this.RewardText)`
  - `WriteString(this.PortraitGiverText)`
  - `WriteString(this.PortraitGiverName)`
  - `WriteString(this.PortraitTurnInText)`
  - `WriteString(this.PortraitTurnInName)`

```csharp
{
this.QuestData.Write(base._worldPacket);
base._worldPacket.WriteInt32(this.QuestPackageID);
base._worldPacket.WriteInt32((int)this.PortraitGiver);
base._worldPacket.WriteInt32((int)this.PortraitGiverMount);
base._worldPacket.WriteInt32((int)this.PortraitGiverModelSceneID);
base._worldPacket.WriteInt32((int)this.PortraitTurnIn);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteInt32(this.QuestGiverCreatureID);
base._worldPacket.WriteUInt32(0u);
}
base._worldPacket.WriteBits(this.QuestTitle.GetByteCount(), 9);
base._worldPacket.WriteBits(this.RewardText.GetByteCount(), 12);
base._worldPacket.WriteBits(this.PortraitGiverText.GetByteCount(), 10);
base._worldPacket.WriteBits(this.PortraitGiverName.GetByteCount(), 8);
base._worldPacket.WriteBits(this.PortraitTurnInText.GetByteCount(), 10);
base._worldPacket.WriteBits(this.PortraitTurnInName.GetByteCount(), 8);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.QuestTitle);
base._worldPacket.WriteString(this.RewardText);
base._worldPacket.WriteString(this.PortraitGiverText);
base._worldPacket.WriteString(this.PortraitGiverName);
base._worldPacket.WriteString(this.PortraitTurnInText);
base._worldPacket.WriteString(this.PortraitTurnInName);
```

---

### QuestGiverQuestComplete

- File: HermesProxy/World/Server/Packets/QuestGiverQuestComplete.cs
- Fields:
  - `WriteUInt32(this.QuestID)`
  - `WriteUInt32(this.XPReward)`
  - `WriteInt64(this.MoneyReward)`
  - `WriteUInt32(this.SkillLineIDReward)`
  - `WriteUInt32(this.NumSkillUpsReward)`

```csharp
{
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WriteUInt32(this.XPReward);
base._worldPacket.WriteInt64(this.MoneyReward);
base._worldPacket.WriteUInt32(this.SkillLineIDReward);
base._worldPacket.WriteUInt32(this.NumSkillUpsReward);
base._worldPacket.WriteBit(this.UseQuestReward);
base._worldPacket.WriteBit(this.LaunchGossip);
base._worldPacket.WriteBit(this.LaunchQuest);
base._worldPacket.WriteBit(this.HideChatMessage);
this.ItemReward.Write(base._worldPacket);
}
```

---

### QuestGiverQuestDetails

- File: HermesProxy/World/Server/Packets/QuestGiverQuestDetails.cs
- Fields:
  - `WriteInt32((int)`
  - `WriteInt32(this.QuestPackageID)`
  - `WriteInt32((int)`
  - `WriteUInt32(this.PortraitGiverMount)`
  - `WriteUInt32(this.PortraitGiverModelSceneID)`
  - `WriteInt32((int)`
  - `WriteUInt32(this.QuestFlags[0])`
  - `WriteUInt32(this.QuestFlags[1])`
  - `WriteUInt32((this.QuestFlags.Length > 2)`
  - `WriteInt32((int)`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteUInt32((uint)`
  - `WriteInt32(this.QuestStartItemID)`
  - `WriteInt32(this.QuestSessionBonus)`
  - `WriteInt32(this.QuestGiverCreatureID)`
  - `WriteUInt32(0u)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteUInt32(emote.Delay)`
  - `WriteInt32((int)`
  - `WriteInt32(obj.ObjectID)`
  - `WriteInt32(obj.Amount)`
  - `WriteUInt8(obj.Type)`
  - `WriteString(this.QuestTitle)`
  - `WriteString(this.DescriptionText)`
  - `WriteString(this.LogDescription)`
  - `WriteString(this.PortraitGiverText)`
  - `WriteString(this.PortraitGiverName)`
  - `WriteString(this.PortraitTurnInText)`
  - `WriteString(this.PortraitTurnInName)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.QuestGiverGUID);
base._worldPacket.WritePackedGuid128(this.InformUnit);
base._worldPacket.WriteInt32((int)this.QuestID);
base._worldPacket.WriteInt32(this.QuestPackageID);
base._worldPacket.WriteInt32((int)this.PortraitGiver);
base._worldPacket.WriteUInt32(this.PortraitGiverMount);
base._worldPacket.WriteUInt32(this.PortraitGiverModelSceneID);
base._worldPacket.WriteInt32((int)this.PortraitTurnIn);
base._worldPacket.WriteUInt32(this.QuestFlags[0]);
base._worldPacket.WriteUInt32(this.QuestFlags[1]);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteUInt32((this.QuestFlags.Length > 2) ? this.QuestFlags[2] : 0u);
}
base._worldPacket.WriteInt32((int)this.SuggestedPartyMembers);
base._worldPacket.WriteUInt32((uint)this.LearnSpells.Count);
base._worldPacket.WriteUInt32((uint)this.DescEmotes.Length);
base._worldPacket.WriteUInt32((uint)this.Objectives.Count);
base._worldPacket.WriteInt32(this.QuestStartItemID);
base._worldPacket.WriteInt32(this.QuestSessionBonus);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteInt32(this.QuestGiverCreatureID);
base._worldPacket.WriteUInt32(0u);
```

---

### QuestGiverQuestFailed

- File: HermesProxy/World/Server/Packets/QuestGiverQuestFailed.cs
- Fields:
  - `WriteUInt32(this.QuestID)`
  - `WriteUInt32((uint)`

```csharp
{
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WriteUInt32((uint)this.Reason);
}
```

---

### QuestGiverQuestListMessage

- File: HermesProxy/World/Server/Packets/QuestGiverQuestListMessage.cs
- Fields:
  - `WriteUInt32(this.GreetEmoteDelay)`
  - `WriteUInt32(this.GreetEmoteType)`
  - `WriteInt32(this.QuestOptions.Count)`
  - `WriteString(this.Greeting)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.QuestGiverGUID);
base._worldPacket.WriteUInt32(this.GreetEmoteDelay);
base._worldPacket.WriteUInt32(this.GreetEmoteType);
base._worldPacket.WriteInt32(this.QuestOptions.Count);
base._worldPacket.WriteBits(this.Greeting.GetByteCount(), 11);
base._worldPacket.FlushBits();
foreach (ClientGossipQuest quest in this.QuestOptions)
{
quest.Write(base._worldPacket);
}
base._worldPacket.WriteString(this.Greeting);
}
```

---

### QuestGiverRequestItems

- File: HermesProxy/World/Server/Packets/QuestGiverRequestItems.cs
- Fields:
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteUInt32(this.QuestFlags[0])`
  - `WriteUInt32(this.QuestFlags[1])`
  - `WriteUInt32((this.QuestFlags.Length > 2)`
  - `WriteInt32((int)`
  - `WriteInt32(this.MoneyToGet)`
  - `WriteInt32(this.Collect.Count)`
  - `WriteInt32(this.Currency.Count)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteInt32((int)`
  - `WriteUInt32(obj.Flags)`
  - `WriteInt32((int)`
  - `WriteInt32(cur.Amount)`
  - `WriteInt32((int)`
  - `WriteUInt32(0u)`
  - `WriteString(this.QuestTitle)`
  - `WriteString(this.CompletionText)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.QuestGiverGUID);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteInt32((int)this.QuestGiverCreatureID);
}
base._worldPacket.WriteInt32((int)this.QuestID);
base._worldPacket.WriteInt32((int)this.CompEmoteDelay);
base._worldPacket.WriteInt32((int)this.CompEmoteType);
base._worldPacket.WriteUInt32(this.QuestFlags[0]);
base._worldPacket.WriteUInt32(this.QuestFlags[1]);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteUInt32((this.QuestFlags.Length > 2) ? this.QuestFlags[2] : 0u);
}
base._worldPacket.WriteInt32((int)this.SuggestPartyMembers);
base._worldPacket.WriteInt32(this.MoneyToGet);
base._worldPacket.WriteInt32(this.Collect.Count);
base._worldPacket.WriteInt32(this.Currency.Count);
base._worldPacket.WriteInt32((int)this.StatusFlags);
foreach (QuestObjectiveCollect obj in this.Collect)
{
base._worldPacket.WriteInt32((int)obj.ObjectID);
base._worldPacket.WriteInt32((int)obj.Amount);
base._worldPacket.WriteUInt32(obj.Flags);
```

---

### QuestGiverStatusMultiple

- File: HermesProxy/World/Server/Packets/QuestGiverStatusMultiple.cs
- Fields:
  - `WriteInt32(this.QuestGivers.Count)`
  - `WriteUInt64((ulong)`

```csharp
{
base._worldPacket.WriteInt32(this.QuestGivers.Count);
foreach (QuestGiverInfo questGiver in this.QuestGivers)
{
base._worldPacket.WritePackedGuid128(questGiver.Guid);
base._worldPacket.WriteUInt64((ulong)questGiver.Status);
}
}
```

---

### QuestGiverStatusPkt

- File: HermesProxy/World/Server/Packets/QuestGiverStatusPkt.cs
- Fields:
  - `WriteUInt64((ulong)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.QuestGiver.Guid);
base._worldPacket.WriteUInt64((ulong)this.QuestGiver.Status);
}
```

---

### QuestPushResult

- File: HermesProxy/World/Server/Packets/QuestPushResult.cs
- Fields:
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.SenderGUID);
base._worldPacket.WriteUInt8((byte)this.Result);
}
```

---

### QuestUpdateAddCredit

- File: HermesProxy/World/Server/Packets/QuestUpdateAddCredit.cs
- Fields:
  - `WriteUInt32(this.QuestID)`
  - `WriteInt32(this.ObjectID)`
  - `WriteUInt16(this.Count)`
  - `WriteUInt16(this.Required)`
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.VictimGUID);
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WriteInt32(this.ObjectID);
base._worldPacket.WriteUInt16(this.Count);
base._worldPacket.WriteUInt16(this.Required);
base._worldPacket.WriteUInt8((byte)this.ObjectiveType);
}
```

---

### QuestUpdateAddCreditSimple

- File: HermesProxy/World/Server/Packets/QuestUpdateAddCreditSimple.cs
- Fields:
  - `WriteUInt32(this.QuestID)`
  - `WriteInt32(this.ObjectID)`
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WriteUInt32(this.QuestID);
base._worldPacket.WriteInt32(this.ObjectID);
base._worldPacket.WriteUInt8((byte)this.ObjectiveType);
}
```

---

### QuestUpdateStatus

- File: HermesProxy/World/Server/Packets/QuestUpdateStatus.cs
- Fields:
  - `WriteUInt32(this.QuestID)`

```csharp
{
base._worldPacket.WriteUInt32(this.QuestID);
}
```

---

### RaidGroupOnly

- File: HermesProxy/World/Server/Packets/RaidGroupOnly.cs
- Fields:
  - `WriteInt32(this.Delay)`
  - `WriteUInt32((uint)`

```csharp
{
base._worldPacket.WriteInt32(this.Delay);
base._worldPacket.WriteUInt32((uint)this.Reason);
}
```

---

### RaidInstanceInfo

- File: HermesProxy/World/Server/Packets/RaidInstanceInfo.cs
- Fields:
  - `WriteInt32(this.LockList.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.LockList.Count);
foreach (InstanceLock lockInfos in this.LockList)
{
lockInfos.Write(base._worldPacket);
}
}
```

---

### RaidInstanceMessage

- File: HermesProxy/World/Server/Packets/RaidInstanceMessage.cs
- Fields:
  - `WriteUInt8((byte)`
  - `WriteUInt32(this.MapID)`
  - `WriteUInt32((uint)`

```csharp
{
base._worldPacket.WriteUInt8((byte)this.Type);
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteUInt32((uint)this.DifficultyID);
base._worldPacket.WriteBit(this.Locked);
base._worldPacket.WriteBit(this.Extended);
base._worldPacket.FlushBits();
}
```

---

### RandomRoll

- File: HermesProxy/World/Server/Packets/RandomRoll.cs
- Fields:
  - `WriteInt32(this.Min)`
  - `WriteInt32(this.Max)`
  - `WriteInt32(this.Result)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Roller);
base._worldPacket.WritePackedGuid128(this.RollerWowAccount);
base._worldPacket.WriteInt32(this.Min);
base._worldPacket.WriteInt32(this.Max);
base._worldPacket.WriteInt32(this.Result);
}
```

---

### ReadItemResultFailed

- File: HermesProxy/World/Server/Packets/ReadItemResultFailed.cs
- Fields:
  - `WriteUInt32(this.Delay)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.ItemGUID);
base._worldPacket.WriteUInt32(this.Delay);
base._worldPacket.WriteBits(this.Subcode, 2);
base._worldPacket.FlushBits();
}
```

---

### ReadItemResultOK

- File: HermesProxy/World/Server/Packets/ReadItemResultOK.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.ItemGUID);
}
```

---

### ReadyCheckCompleted

- File: HermesProxy/World/Server/Packets/ReadyCheckCompleted.cs

```csharp
{
base._worldPacket.WriteInt8(this.PartyIndex);
base._worldPacket.WritePackedGuid128(this.PartyGUID);
}
```

---

### ReadyCheckResponse

- File: HermesProxy/World/Server/Packets/ReadyCheckResponse.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.PartyGUID);
base._worldPacket.WritePackedGuid128(this.Player);
base._worldPacket.WriteBit(this.IsReady);
base._worldPacket.FlushBits();
}
```

---

### ReadyCheckStarted

- File: HermesProxy/World/Server/Packets/ReadyCheckStarted.cs
- Fields:
  - `WriteUInt64(this.Duration)`

```csharp
{
base._worldPacket.WriteInt8(this.PartyIndex);
base._worldPacket.WritePackedGuid128(this.PartyGUID);
base._worldPacket.WritePackedGuid128(this.InitiatorGUID);
base._worldPacket.WriteUInt64(this.Duration);
}
```

---

### ResetFailedNotify

- File: HermesProxy/World/Server/Packets/ResetFailedNotify.cs

```csharp
{
}
```

---

### RespecWipeConfirm

- File: HermesProxy/World/Server/Packets/RespecWipeConfirm.cs
- Fields:
  - `WriteUInt32(this.Cost)`

```csharp
{
base._worldPacket.WriteInt8((sbyte)this.RespecType);
base._worldPacket.WriteUInt32(this.Cost);
base._worldPacket.WritePackedGuid128(this.TrainerGUID);
}
```

---

### ResumeComms

- File: HermesProxy/World/Server/Packets/ResumeComms.cs

```csharp
{
}
```

---

### ResumeToken

- File: HermesProxy/World/Server/Packets/ResumeToken.cs
- Fields:
  - `WriteUInt32(this.SequenceIndex)`

```csharp
{
base._worldPacket.WriteUInt32(this.SequenceIndex);
base._worldPacket.WriteBits(this.Reason, 2);
base._worldPacket.FlushBits();
}
```

---

### ResurrectRequest

- File: HermesProxy/World/Server/Packets/ResurrectRequest.cs
- Fields:
  - `WriteUInt32(this.CasterVirtualRealmAddress)`
  - `WriteUInt32(this.PetNumber)`
  - `WriteUInt32(this.SpellID)`
  - `WriteString(this.Name)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.CasterVirtualRealmAddress);
base._worldPacket.WriteUInt32(this.PetNumber);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteBits(this.Name.GetByteCount(), 11);
base._worldPacket.WriteBit(this.UseTimer);
base._worldPacket.WriteBit(this.Sickness);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Name);
}
```

---

### SAttackStart

- File: HermesProxy/World/Server/Packets/SAttackStart.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Attacker);
base._worldPacket.WritePackedGuid128(this.Victim);
}
```

---

### SAttackStop

- File: HermesProxy/World/Server/Packets/SAttackStop.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Attacker ?? WowGuid128.Empty);
base._worldPacket.WritePackedGuid128(this.Victim ?? WowGuid128.Empty);
base._worldPacket.WriteBit(this.NowDead);
base._worldPacket.FlushBits();
}
```

---

### STextEmote

- File: HermesProxy/World/Server/Packets/STextEmote.cs
- Fields:
  - `WriteInt32(this.EmoteID)`
  - `WriteInt32(this.SoundIndex)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.SourceGUID);
base._worldPacket.WritePackedGuid128(this.SourceAccountGUID);
base._worldPacket.WriteInt32(this.EmoteID);
base._worldPacket.WriteInt32(this.SoundIndex);
base._worldPacket.WritePackedGuid128(this.TargetGUID);
}
```

---

### SeasonInfo

- File: HermesProxy/World/Server/Packets/SeasonInfo.cs
- Fields:
  - `WriteInt32(this.MythicPlusSeasonID)`
  - `WriteInt32(this.MythicPlusMilestoneSeasonID)`
  - `WriteInt32(this.CurrentSeason)`
  - `WriteInt32(this.PreviousSeason)`
  - `WriteInt32(this.ConquestWeeklyProgressCurrencyID)`
  - `WriteInt32(this.PvpSeasonID)`

```csharp
{
base._worldPacket.WriteInt32(this.MythicPlusSeasonID);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteInt32(this.MythicPlusMilestoneSeasonID);
}
base._worldPacket.WriteInt32(this.CurrentSeason);
base._worldPacket.WriteInt32(this.PreviousSeason);
base._worldPacket.WriteInt32(this.ConquestWeeklyProgressCurrencyID);
base._worldPacket.WriteInt32(this.PvpSeasonID);
base._worldPacket.WriteBit(this.WeeklyRewardChestsEnabled);
base._worldPacket.FlushBits();
}
```

---

### SellResponse

- File: HermesProxy/World/Server/Packets/SellResponse.cs
- Fields:
  - `WriteUInt32(1)`
  - `WriteInt32((int)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.VendorGUID);
base._worldPacket.WriteUInt32(1); // ItemGUIDs count
base._worldPacket.WriteInt32((int)this.Reason);
base._worldPacket.WritePackedGuid128(this.ItemGUID);
}
```

---

### SendKnownSpells

- File: HermesProxy/World/Server/Packets/SendKnownSpells.cs
- Fields:
  - `WriteInt32(this.KnownSpells.Count)`
  - `WriteInt32(this.FavoriteSpells.Count)`
  - `WriteUInt32(spellId)`
  - `WriteUInt32(spellId2)`

```csharp
{
base._worldPacket.WriteBit(this.InitialLogin);
base._worldPacket.WriteInt32(this.KnownSpells.Count);
base._worldPacket.WriteInt32(this.FavoriteSpells.Count);
foreach (uint spellId in this.KnownSpells)
{
base._worldPacket.WriteUInt32(spellId);
}
foreach (uint spellId2 in this.FavoriteSpells)
{
base._worldPacket.WriteUInt32(spellId2);
}
}
```

---

### SendRaidTargetUpdateAll

- File: HermesProxy/World/Server/Packets/SendRaidTargetUpdateAll.cs
- Fields:
  - `WriteInt32(this.TargetIcons.Count)`

```csharp
{
base._worldPacket.WriteInt8(this.PartyIndex);
base._worldPacket.WriteInt32(this.TargetIcons.Count);
foreach (Tuple<sbyte, WowGuid128> pair in this.TargetIcons)
{
base._worldPacket.WritePackedGuid128(pair.Item2);
base._worldPacket.WriteInt8(pair.Item1);
}
}
```

---

### SendRaidTargetUpdateSingle

- File: HermesProxy/World/Server/Packets/SendRaidTargetUpdateSingle.cs

```csharp
{
base._worldPacket.WriteInt8(this.PartyIndex);
base._worldPacket.WriteInt8(this.Symbol);
base._worldPacket.WritePackedGuid128(this.Target);
base._worldPacket.WritePackedGuid128(this.ChangedBy);
}
```

---

### SendSpellCharges

- File: HermesProxy/World/Server/Packets/SendSpellCharges.cs
- Fields:
  - `WriteInt32(this.Entries.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Entries.Count);
this.Entries.ForEach(delegate(SpellChargeEntry p)
{
p.Write(base._worldPacket);
});
}
```

---

### SendSpellHistory

- File: HermesProxy/World/Server/Packets/SendSpellHistory.cs
- Fields:
  - `WriteInt32(this.Entries.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Entries.Count);
this.Entries.ForEach(delegate(SpellHistoryEntry p)
{
p.Write(base._worldPacket);
});
}
```

---

### SendUnlearnSpells

- File: HermesProxy/World/Server/Packets/SendUnlearnSpells.cs
- Fields:
  - `WriteInt32(this.Spells.Count)`
  - `WriteUInt32(spell)`

```csharp
{
base._worldPacket.WriteInt32(this.Spells.Count);
foreach (uint spell in this.Spells)
{
base._worldPacket.WriteUInt32(spell);
}
}
```

---

### ServerPetitionShowList

- File: HermesProxy/World/Server/Packets/ServerPetitionShowList.cs
- Fields:
  - `WriteInt32(this.Petitions.Count)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Unit);
base._worldPacket.WriteInt32(this.Petitions.Count);
foreach (PetitionEntry petition2 in this.Petitions)
{
petition2.Write(base._worldPacket);
}
}
```

---

### ServerPetitionShowSignatures

- File: HermesProxy/World/Server/Packets/ServerPetitionShowSignatures.cs
- Fields:
  - `WriteInt32(this.PetitionID)`
  - `WriteInt32(this.Signatures.Count)`
  - `WriteInt32(signature.Choice)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Item);
base._worldPacket.WritePackedGuid128(this.Owner);
base._worldPacket.WritePackedGuid128(this.OwnerAccountID);
base._worldPacket.WriteInt32(this.PetitionID);
base._worldPacket.WriteInt32(this.Signatures.Count);
foreach (PetitionSignature signature in this.Signatures)
{
base._worldPacket.WritePackedGuid128(signature.Signer);
base._worldPacket.WriteInt32(signature.Choice);
}
}
```

---

### ServerTimeOffset

- File: HermesProxy/World/Server/Packets/ServerTimeOffset.cs
- Fields:
  - `WriteInt64(this.Time)`

```csharp
{
base._worldPacket.WriteInt64(this.Time);
}
```

---

### SetAllTaskProgress

- File: HermesProxy/World/Server/Packets/SetAllTaskProgress.cs
- Fields:
  - `WriteInt32(this.Tasks.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Tasks.Count);
foreach (TaskProgress task in this.Tasks)
{
task.Write(base._worldPacket);
}
}
```

---

### SetFactionStanding

- File: HermesProxy/World/Server/Packets/SetFactionStanding.cs
- Fields:
  - `WriteFloat(this.ReferAFriendBonus)`
  - `WriteFloat(this.BonusFromAchievementSystem)`
  - `WriteInt32(this.Factions.Count)`

```csharp
{
base._worldPacket.WriteFloat(this.ReferAFriendBonus);
base._worldPacket.WriteFloat(this.BonusFromAchievementSystem);
base._worldPacket.WriteInt32(this.Factions.Count);
foreach (FactionStandingData faction in this.Factions)
{
faction.Write(base._worldPacket);
}
base._worldPacket.WriteBit(this.ShowVisual);
base._worldPacket.FlushBits();
}
```

---

### SetFactionVisible

- File: HermesProxy/World/Server/Packets/SetFactionVisible.cs
- Fields:
  - `WriteUInt32(this.FactionIndex)`

```csharp
{
base._worldPacket.WriteUInt32(this.FactionIndex);
}
```

---

### SetForcedReactions

- File: HermesProxy/World/Server/Packets/SetForcedReactions.cs
- Fields:
  - `WriteInt32(this.Reactions.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Reactions.Count);
foreach (ForcedReaction reaction2 in this.Reactions)
{
reaction2.Write(base._worldPacket);
}
}
```

---

### SetProficiency

- File: HermesProxy/World/Server/Packets/SetProficiency.cs
- Fields:
  - `WriteUInt32(this.ProficiencyMask)`
  - `WriteUInt8(this.ProficiencyClass)`

```csharp
{
base._worldPacket.WriteUInt32(this.ProficiencyMask);
base._worldPacket.WriteUInt8(this.ProficiencyClass);
}
```

---

### SetSpellModifier

- File: HermesProxy/World/Server/Packets/SetSpellModifier.cs
- Fields:
  - `WriteInt32(this.Modifiers.Count)`

```csharp
{
base._worldPacket.WriteInt32(this.Modifiers.Count);
foreach (SpellModifierInfo spellMod in this.Modifiers)
{
spellMod.Write(base._worldPacket);
}
}
```

---

### SetTimeZoneInformation

- File: HermesProxy/World/Server/Packets/SetTimeZoneInformation.cs
- Fields:
  - `WriteString(this.ServerTimeTZ)`
  - `WriteString(this.GameTimeTZ)`
  - `WriteString(this.ServerRegionalTZ ?? "US/Eastern")`

```csharp
{
base._worldPacket.WriteBits(this.ServerTimeTZ.GetByteCount(), 7);
base._worldPacket.WriteBits(this.GameTimeTZ.GetByteCount(), 7);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteBits((this.ServerRegionalTZ ?? "US/Eastern").GetByteCount(), 7);
}
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.ServerTimeTZ);
base._worldPacket.WriteString(this.GameTimeTZ);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteString(this.ServerRegionalTZ ?? "US/Eastern");
}
}
```

---

### SetupCurrency

- File: HermesProxy/World/Server/Packets/SetupCurrency.cs
- Fields:
  - `WriteInt32(this.Data.Count)`
  - `WriteUInt32(data.Type)`
  - `WriteUInt32(data.Quantity)`
  - `WriteUInt32(data.WeeklyQuantity.Value)`
  - `WriteUInt32(data.MaxWeeklyQuantity.Value)`
  - `WriteUInt32(data.TrackedQuantity.Value)`
  - `WriteInt32(data.MaxQuantity.Value)`
  - `WriteInt32(data.Unused901.Value)`

```csharp
{
base._worldPacket.WriteInt32(this.Data.Count);
foreach (Record data in this.Data)
{
base._worldPacket.WriteUInt32(data.Type);
base._worldPacket.WriteUInt32(data.Quantity);
base._worldPacket.WriteBit(data.WeeklyQuantity.HasValue);
base._worldPacket.WriteBit(data.MaxWeeklyQuantity.HasValue);
base._worldPacket.WriteBit(data.TrackedQuantity.HasValue);
base._worldPacket.WriteBit(data.MaxQuantity.HasValue);
base._worldPacket.WriteBit(data.Unused901.HasValue);
base._worldPacket.WriteBits(data.Flags, 5);
base._worldPacket.FlushBits();
if (data.WeeklyQuantity.HasValue)
{
base._worldPacket.WriteUInt32(data.WeeklyQuantity.Value);
}
if (data.MaxWeeklyQuantity.HasValue)
{
base._worldPacket.WriteUInt32(data.MaxWeeklyQuantity.Value);
}
if (data.TrackedQuantity.HasValue)
{
base._worldPacket.WriteUInt32(data.TrackedQuantity.Value);
}
```

---

### ShowBank

- File: HermesProxy/World/Server/Packets/ShowBank.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### ShowTaxiNodes

- File: HermesProxy/World/Server/Packets/ShowTaxiNodes.cs
- Fields:
  - `WriteInt32(canLandNodes.Count / 8)`
  - `WriteInt32(canUseNodes.Count / 8)`
  - `WriteUInt32(this.WindowInfo.CurrentNode)`
  - `WriteUInt8(node)`
  - `WriteUInt8(node2)`

```csharp
{
base._worldPacket.WriteBit(this.WindowInfo != null);
base._worldPacket.FlushBits();
List<byte> canLandNodes = new List<byte>(this.CanLandNodes);
this.PadToUInt64Alignment(canLandNodes);
base._worldPacket.WriteInt32(canLandNodes.Count / 8);
List<byte> canUseNodes = new List<byte>(this.CanUseNodes);
this.PadToUInt64Alignment(canUseNodes);
base._worldPacket.WriteInt32(canUseNodes.Count / 8);
if (this.WindowInfo != null)
{
base._worldPacket.WritePackedGuid128(this.WindowInfo.UnitGUID);
base._worldPacket.WriteUInt32(this.WindowInfo.CurrentNode);
}
foreach (byte node in canLandNodes)
{
base._worldPacket.WriteUInt8(node);
}
foreach (byte node2 in canUseNodes)
{
base._worldPacket.WriteUInt8(node2);
}
}
```

---

### SocialContractRequestResponse

- File: HermesProxy/World/Server/Packets/SocialContractRequestResponse.cs
- Fields:
  - `WriteBool(data: false)`

```csharp
{
base._worldPacket.WriteBool(data: false);
}
```

---

### SocketGemsSuccess

- File: HermesProxy/World/Server/Packets/SocketGemsSuccess.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.ItemGuid);
}
```

---

### SpecialMountAnim

- File: HermesProxy/World/Server/Packets/SpecialMountAnim.cs
- Fields:
  - `WriteInt32(this.SpellVisualKitIDs.Count)`
  - `WriteInt32(this.SequenceVariation)`
  - `WriteInt32(id)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.UnitGUID);
base._worldPacket.WriteInt32(this.SpellVisualKitIDs.Count);
base._worldPacket.WriteInt32(this.SequenceVariation);
foreach (int id in this.SpellVisualKitIDs)
{
base._worldPacket.WriteInt32(id);
}
}
```

---

### SpellChannelStart

- File: HermesProxy/World/Server/Packets/SpellChannelStart.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.SpellXSpellVisualID)`
  - `WriteUInt32(this.Duration)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
base._worldPacket.WriteUInt32(this.Duration);
base._worldPacket.WriteBit(this.InterruptImmunities != null);
base._worldPacket.WriteBit(this.HealPrediction != null);
base._worldPacket.FlushBits();
if (this.InterruptImmunities != null)
{
this.InterruptImmunities.Write(base._worldPacket);
}
if (this.HealPrediction != null)
{
this.HealPrediction.Write(base._worldPacket);
}
}
```

---

### SpellChannelUpdate

- File: HermesProxy/World/Server/Packets/SpellChannelUpdate.cs
- Fields:
  - `WriteInt32(this.TimeRemaining)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteInt32(this.TimeRemaining);
}
```

---

### SpellCooldownPkt

- File: HermesProxy/World/Server/Packets/SpellCooldownPkt.cs
- Fields:
  - `WriteUInt8(this.Flags)`
  - `WriteInt32(this.SpellCooldowns.Count)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Caster);
base._worldPacket.WriteUInt8(this.Flags);
base._worldPacket.WriteInt32(this.SpellCooldowns.Count);
foreach (SpellCooldownStruct cd in this.SpellCooldowns)
{
cd.Write(base._worldPacket);
}
}
```

---

### SpellDamageShield

- File: HermesProxy/World/Server/Packets/SpellDamageShield.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteInt32(this.Damage)`
  - `WriteInt32(this.OriginalDamage)`
  - `WriteUInt32(this.OverKill)`
  - `WriteUInt32(this.SchoolMask)`
  - `WriteUInt32(this.LogAbsorbed)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.VictimGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteInt32(this.Damage);
base._worldPacket.WriteInt32(this.OriginalDamage);
base._worldPacket.WriteUInt32(this.OverKill);
base._worldPacket.WriteUInt32(this.SchoolMask);
base._worldPacket.WriteUInt32(this.LogAbsorbed);
base._worldPacket.WriteBit(this.LogData != null);
base._worldPacket.FlushBits();
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
}
```

---

### SpellDelayed

- File: HermesProxy/World/Server/Packets/SpellDelayed.cs
- Fields:
  - `WriteInt32(this.Delay)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteInt32(this.Delay);
}
```

---

### SpellDispellLog

- File: HermesProxy/World/Server/Packets/SpellDispellLog.cs
- Fields:
  - `WriteUInt32(this.DispelledBySpellID)`
  - `WriteInt32(this.DispellData.Count)`
  - `WriteUInt32(data.SpellID)`
  - `WriteInt32(data.Rolled.Value)`
  - `WriteInt32(data.Needed.Value)`

```csharp
{
base._worldPacket.WriteBit(this.IsSteal);
base._worldPacket.WriteBit(this.IsBreak);
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.DispelledBySpellID);
base._worldPacket.WriteInt32(this.DispellData.Count);
foreach (SpellDispellData data in this.DispellData)
{
base._worldPacket.WriteUInt32(data.SpellID);
base._worldPacket.WriteBit(data.Harmful);
base._worldPacket.WriteBit(data.Rolled.HasValue);
base._worldPacket.WriteBit(data.Needed.HasValue);
if (data.Rolled.HasValue)
{
base._worldPacket.WriteInt32(data.Rolled.Value);
}
if (data.Needed.HasValue)
{
base._worldPacket.WriteInt32(data.Needed.Value);
}
base._worldPacket.FlushBits();
}
}
```

---

### SpellEnergizeLog

- File: HermesProxy/World/Server/Packets/SpellEnergizeLog.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32((uint)`
  - `WriteInt32(this.Amount)`
  - `WriteInt32(this.OverEnergize)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32((uint)this.Type);
base._worldPacket.WriteInt32(this.Amount);
base._worldPacket.WriteInt32(this.OverEnergize);
base._worldPacket.WriteBit(this.LogData != null);
base._worldPacket.FlushBits();
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
}
```

---

### SpellFailedOther

- File: HermesProxy/World/Server/Packets/SpellFailedOther.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.SpellXSpellVisualID)`
  - `WriteUInt8(this.Reason)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CasterUnit);
base._worldPacket.WritePackedGuid128(this.CastID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
base._worldPacket.WriteUInt8(this.Reason);
}
```

---

### SpellFailure

- File: HermesProxy/World/Server/Packets/SpellFailure.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.SpellXSpellVisualID)`
  - `WriteUInt16(this.Reason)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.CasterUnit);
base._worldPacket.WritePackedGuid128(this.CastID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
base._worldPacket.WriteUInt16(this.Reason);
}
```

---

### SpellGo

- File: HermesProxy/World/Server/Packets/SpellGo.cs

```csharp
{
this.Cast.Write(base._worldPacket);
base._worldPacket.WriteBit(this.LogData != null);
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
base._worldPacket.FlushBits();
}
```

---

### SpellHealLog

- File: HermesProxy/World/Server/Packets/SpellHealLog.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteInt32(this.HealAmount)`
  - `WriteInt32(this.OriginalHealAmount)`
  - `WriteUInt32(this.OverHeal)`
  - `WriteUInt32(this.Absorbed)`
  - `WriteUInt32(0)`
  - `WriteFloat(this.CritRollMade.Value)`
  - `WriteFloat(this.CritRollNeeded.Value)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteInt32(this.HealAmount);
base._worldPacket.WriteInt32(this.OriginalHealAmount);
base._worldPacket.WriteUInt32(this.OverHeal);
base._worldPacket.WriteUInt32(this.Absorbed);
base._worldPacket.WriteUInt32(0); // Supporters count
base._worldPacket.WriteBit(this.Crit);
base._worldPacket.WriteBit(this.CritRollMade.HasValue);
base._worldPacket.WriteBit(this.CritRollNeeded.HasValue);
base._worldPacket.WriteBit(this.LogData != null);
base._worldPacket.WriteBit(this.ContentTuning != null);
base._worldPacket.FlushBits();
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
if (this.CritRollMade.HasValue)
{
base._worldPacket.WriteFloat(this.CritRollMade.Value);
}
if (this.CritRollNeeded.HasValue)
{
```

---

### SpellInstakillLog

- File: HermesProxy/World/Server/Packets/SpellInstakillLog.cs
- Fields:
  - `WriteUInt32(this.SpellID)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.SpellID);
}
```

---

### SpellNonMeleeDamageLog

- File: HermesProxy/World/Server/Packets/SpellNonMeleeDamageLog.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.SpellXSpellVisualID)`
  - `WriteInt32(this.Damage)`
  - `WriteInt32(this.OriginalDamage)`
  - `WriteInt32(this.Overkill)`
  - `WriteUInt8(this.SchoolMask)`
  - `WriteInt32(this.Absorbed)`
  - `WriteInt32(this.Resisted)`
  - `WriteInt32(this.ShieldBlock)`
  - `WriteUInt32(0)`
  - `WriteUInt32(0)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WritePackedGuid128(this.CastID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.SpellXSpellVisualID);
base._worldPacket.WriteInt32(this.Damage);
base._worldPacket.WriteInt32(this.OriginalDamage);
base._worldPacket.WriteInt32(this.Overkill);
base._worldPacket.WriteUInt8(this.SchoolMask);
base._worldPacket.WriteInt32(this.Absorbed);
base._worldPacket.WriteInt32(this.Resisted);
base._worldPacket.WriteInt32(this.ShieldBlock);
base._worldPacket.WriteUInt32(0); // WorldTextViewers count
base._worldPacket.WriteUInt32(0); // Supporters count
base._worldPacket.WriteBit(this.Periodic);
base._worldPacket.WriteBits((uint)this.Flags, 7);
base._worldPacket.WriteBit(bit: false);
base._worldPacket.WriteBit(this.LogData != null);
base._worldPacket.WriteBit(this.ContentTuning != null);
base._worldPacket.FlushBits();
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
```

---

### SpellPeriodicAuraLog

- File: HermesProxy/World/Server/Packets/SpellPeriodicAuraLog.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteInt32(this.Effects.Count)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TargetGUID);
base._worldPacket.WritePackedGuid128(this.CasterGUID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteInt32(this.Effects.Count);
base._worldPacket.WriteBit(this.LogData != null);
base._worldPacket.FlushBits();
foreach (SpellLogEffect effect in this.Effects)
{
effect.Write(base._worldPacket);
}
if (this.LogData != null)
{
this.LogData.Write(base._worldPacket);
}
}
```

---

### SpellPrepare

- File: HermesProxy/World/Server/Packets/SpellPrepare.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.ClientCastID);
base._worldPacket.WritePackedGuid128(this.ServerCastID);
}
```

---

### SpellStart

- File: HermesProxy/World/Server/Packets/SpellStart.cs

```csharp
{
this.Cast.Write(base._worldPacket);
}
```

---

### SpiritHealerConfirm

- File: HermesProxy/World/Server/Packets/SpiritHealerConfirm.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.Guid);
}
```

---

### StandStateUpdate

- File: HermesProxy/World/Server/Packets/StandStateUpdate.cs
- Fields:
  - `WriteUInt32(this.AnimKitID)`
  - `WriteUInt8(this.StandState)`

```csharp
{
base._worldPacket.WriteUInt32(this.AnimKitID);
base._worldPacket.WriteUInt8(this.StandState);
}
```

---

### StartLightningStorm

- File: HermesProxy/World/Server/Packets/StartLightningStorm.cs
- Fields:
  - `WriteUInt32(this.LightningStormId)`

```csharp
{
base._worldPacket.WriteUInt32(this.LightningStormId);
}
```

---

### StartLootRoll

- File: HermesProxy/World/Server/Packets/StartLootRoll.cs
- Fields:
  - `WriteUInt32(this.MapID)`
  - `WriteUInt32(this.RollTime)`
  - `WriteUInt8((byte)`
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.LootObj);
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteUInt32(this.RollTime);
base._worldPacket.WriteUInt8((byte)this.ValidRolls);
base._worldPacket.WriteUInt8((byte)this.Method);
this.Item.Write(base._worldPacket);
}
```

---

### StartMirrorTimer

- File: HermesProxy/World/Server/Packets/StartMirrorTimer.cs
- Fields:
  - `WriteInt32((int)`
  - `WriteInt32(this.Value)`
  - `WriteInt32(this.MaxValue)`
  - `WriteInt32(this.Scale)`
  - `WriteInt32(this.SpellID)`

```csharp
{
base._worldPacket.WriteInt32((int)this.Timer);
base._worldPacket.WriteInt32(this.Value);
base._worldPacket.WriteInt32(this.MaxValue);
base._worldPacket.WriteInt32(this.Scale);
base._worldPacket.WriteInt32(this.SpellID);
base._worldPacket.WriteBit(this.Paused);
base._worldPacket.FlushBits();
}
```

---

### StopMirrorTimer

- File: HermesProxy/World/Server/Packets/StopMirrorTimer.cs
- Fields:
  - `WriteInt32((int)`

```csharp
{
base._worldPacket.WriteInt32((int)this.Timer);
}
```

---

### SummonRequest

- File: HermesProxy/World/Server/Packets/SummonRequest.cs
- Fields:
  - `WriteUInt32(this.SummonerVirtualRealmAddress)`
  - `WriteInt32(this.AreaID)`
  - `WriteUInt8((byte)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.SummonerGUID);
base._worldPacket.WriteUInt32(this.SummonerVirtualRealmAddress);
base._worldPacket.WriteInt32(this.AreaID);
base._worldPacket.WriteUInt8((byte)this.Reason);
base._worldPacket.WriteBit(this.SkipStartingArea);
base._worldPacket.FlushBits();
}
```

---

### SupercededSpells

- File: HermesProxy/World/Server/Packets/SupercededSpells.cs
- Fields:
  - `WriteInt32(this.SpellID.Count)`
  - `WriteInt32(this.Superceded.Count)`
  - `WriteInt32(this.FavoriteSpellID.Count)`
  - `WriteUInt32(spellId)`
  - `WriteUInt32(spellId2)`
  - `WriteInt32(spellId3)`

```csharp
{
base._worldPacket.WriteInt32(this.SpellID.Count);
base._worldPacket.WriteInt32(this.Superceded.Count);
base._worldPacket.WriteInt32(this.FavoriteSpellID.Count);
foreach (uint spellId in this.SpellID)
{
base._worldPacket.WriteUInt32(spellId);
}
foreach (uint spellId2 in this.Superceded)
{
base._worldPacket.WriteUInt32(spellId2);
}
foreach (int spellId3 in this.FavoriteSpellID)
{
base._worldPacket.WriteInt32(spellId3);
}
}
```

---

### SuspendToken

- File: HermesProxy/World/Server/Packets/SuspendToken.cs
- Fields:
  - `WriteUInt32(this.SequenceIndex)`

```csharp
{
base._worldPacket.WriteUInt32(this.SequenceIndex);
base._worldPacket.WriteBits(this.Reason, 2);
base._worldPacket.FlushBits();
}
```

---

### TaxiNodeStatusPkt

- File: HermesProxy/World/Server/Packets/TaxiNodeStatusPkt.cs

```csharp
{
base._worldPacket.WritePackedGuid128(this.FlightMaster);
base._worldPacket.WriteBits(this.Status, 2);
base._worldPacket.FlushBits();
}
```

---

### TimeSyncRequest

- File: HermesProxy/World/Server/Packets/TimeSyncRequest.cs
- Fields:
  - `WriteUInt32(this.SequenceIndex)`

```csharp
{
base._worldPacket.WriteUInt32(this.SequenceIndex);
}
```

---

### TotemCreated

- File: HermesProxy/World/Server/Packets/TotemCreated.cs
- Fields:
  - `WriteUInt8(this.Slot)`
  - `WriteUInt32(this.Duration)`
  - `WriteUInt32(this.SpellId)`
  - `WriteFloat(this.TimeMod)`

```csharp
{
base._worldPacket.WriteUInt8(this.Slot);
base._worldPacket.WritePackedGuid128(this.Totem);
base._worldPacket.WriteUInt32(this.Duration);
base._worldPacket.WriteUInt32(this.SpellId);
base._worldPacket.WriteFloat(this.TimeMod);
base._worldPacket.WriteBit(this.CannotDismiss);
base._worldPacket.FlushBits();
}
```

---

### TradeStatusPkt

- File: HermesProxy/World/Server/Packets/TradeStatusPkt.cs
- Fields:
  - `WriteInt32((int)`
  - `WriteUInt32(this.ItemID)`
  - `WriteUInt32(this.Id)`
  - `WriteUInt8(this.TradeSlot)`
  - `WriteInt32(this.CurrencyType)`
  - `WriteInt32(this.CurrencyQuantity)`

```csharp
{
base._worldPacket.WriteBit(this.PartnerIsSameBnetAccount);
base._worldPacket.WriteBits(this.Status, 5);
switch (this.Status)
{
case TradeStatus.Failed:
base._worldPacket.WriteBit(this.FailureForYou);
base._worldPacket.WriteInt32((int)this.BagResult);
base._worldPacket.WriteUInt32(this.ItemID);
break;
case TradeStatus.Initiated:
base._worldPacket.WriteUInt32(this.Id);
break;
case TradeStatus.Proposed:
base._worldPacket.WritePackedGuid128(this.Partner);
base._worldPacket.WritePackedGuid128(this.PartnerAccount);
break;
case TradeStatus.WrongRealm:
case TradeStatus.NotOnTaplist:
base._worldPacket.WriteUInt8(this.TradeSlot);
break;
case TradeStatus.CurrencyNotTradable:
case TradeStatus.NotEnoughCurrency:
base._worldPacket.WriteInt32(this.CurrencyType);
base._worldPacket.WriteInt32(this.CurrencyQuantity);
```

---

### TradeUpdated

- File: HermesProxy/World/Server/Packets/TradeUpdated.cs
- Fields:
  - `WriteUInt8(this.WhichPlayer)`
  - `WriteUInt32(this.Id)`
  - `WriteUInt32(this.ClientStateIndex)`
  - `WriteUInt32(this.CurrentStateIndex)`
  - `WriteUInt64(this.Gold)`
  - `WriteInt32(this.CurrencyType)`
  - `WriteInt32(this.CurrencyQuantity)`
  - `WriteInt32(this.ProposedEnchantment)`
  - `WriteInt32(this.Items.Count)`

```csharp
{
base._worldPacket.WriteUInt8(this.WhichPlayer);
base._worldPacket.WriteUInt32(this.Id);
base._worldPacket.WriteUInt32(this.ClientStateIndex);
base._worldPacket.WriteUInt32(this.CurrentStateIndex);
base._worldPacket.WriteUInt64(this.Gold);
base._worldPacket.WriteInt32(this.CurrencyType);
base._worldPacket.WriteInt32(this.CurrencyQuantity);
base._worldPacket.WriteInt32(this.ProposedEnchantment);
base._worldPacket.WriteInt32(this.Items.Count);
this.Items.ForEach(delegate(TradeItem item)
{
item.Write(base._worldPacket);
});
}
```

---

### TrainerBuyFailed

- File: HermesProxy/World/Server/Packets/TrainerBuyFailed.cs
- Fields:
  - `WriteUInt32(this.SpellID)`
  - `WriteUInt32(this.TrainerFailedReason)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TrainerGUID);
base._worldPacket.WriteUInt32(this.SpellID);
base._worldPacket.WriteUInt32(this.TrainerFailedReason);
}
```

---

### TrainerList

- File: HermesProxy/World/Server/Packets/TrainerList.cs
- Fields:
  - `WriteInt32(this.TrainerType)`
  - `WriteUInt32(this.TrainerID)`
  - `WriteInt32(this.Spells.Count)`
  - `WriteUInt32(spell.SpellID)`
  - `WriteUInt32(spell.MoneyCost)`
  - `WriteUInt32(spell.ReqSkillLine)`
  - `WriteUInt32(spell.ReqSkillRank)`
  - `WriteUInt32(spell.ReqAbility[i])`
  - `WriteUInt8((byte)`
  - `WriteUInt8(spell.ReqLevel)`
  - `WriteString(this.Greeting)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.TrainerGUID);
base._worldPacket.WriteInt32(this.TrainerType);
base._worldPacket.WriteUInt32(this.TrainerID);
base._worldPacket.WriteInt32(this.Spells.Count);
foreach (TrainerListSpell spell in this.Spells)
{
base._worldPacket.WriteUInt32(spell.SpellID);
base._worldPacket.WriteUInt32(spell.MoneyCost);
base._worldPacket.WriteUInt32(spell.ReqSkillLine);
base._worldPacket.WriteUInt32(spell.ReqSkillRank);
for (uint i = 0u; i < 3; i++)
{
base._worldPacket.WriteUInt32(spell.ReqAbility[i]);
}
base._worldPacket.WriteUInt8((byte)spell.Usable);
base._worldPacket.WriteUInt8(spell.ReqLevel);
}
base._worldPacket.WriteBits(this.Greeting.GetByteCount(), 11);
base._worldPacket.FlushBits();
base._worldPacket.WriteString(this.Greeting);
}
```

---

### TransferAborted

- File: HermesProxy/World/Server/Packets/TransferAborted.cs
- Fields:
  - `WriteUInt32(this.MapID)`
  - `WriteUInt8(this.Arg)`
  - `WriteInt32(this.MapDifficultyXConditionID)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteUInt8(this.Arg);
base._worldPacket.WriteInt32(this.MapDifficultyXConditionID);
base._worldPacket.WriteBits(this.Reason, 6);
base._worldPacket.FlushBits();
}
```

---

### TransferPending

- File: HermesProxy/World/Server/Packets/TransferPending.cs
- Fields:
  - `WriteUInt32(this.MapID)`
  - `WriteVector3`
  - `WriteUInt32(this.Ship.Id)`
  - `WriteInt32(this.Ship.OriginMapID)`
  - `WriteInt32(this.TransferSpellID.Value)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
base._worldPacket.WriteVector3(this.OldMapPosition);
base._worldPacket.WriteBit(this.Ship != null);
base._worldPacket.WriteBit(this.TransferSpellID.HasValue);
if (this.Ship != null)
{
base._worldPacket.WriteUInt32(this.Ship.Id);
base._worldPacket.WriteInt32(this.Ship.OriginMapID);
}
if (this.TransferSpellID.HasValue)
{
base._worldPacket.WriteInt32(this.TransferSpellID.Value);
}
base._worldPacket.FlushBits();
}
```

---

### TriggerCinematic

- File: HermesProxy/World/Server/Packets/TriggerCinematic.cs
- Fields:
  - `WriteUInt32(this.CinematicID)`

```csharp
{
base._worldPacket.WriteUInt32(this.CinematicID);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WritePackedGuid128(this.ConversationGuid);
}
}
```

---

### TurnInPetitionResult

- File: HermesProxy/World/Server/Packets/TurnInPetitionResult.cs

```csharp
{
base._worldPacket.WriteBits(this.Result, 4);
base._worldPacket.FlushBits();
}
```

---

### TutorialFlags

- File: HermesProxy/World/Server/Packets/TutorialFlags.cs
- Fields:
  - `WriteUInt32(this.TutorialData[i])`

```csharp
{
for (byte i = 0; i < 8; i++)
{
base._worldPacket.WriteUInt32(this.TutorialData[i]);
}
}
```

---

### UnlearnedSpells

- File: HermesProxy/World/Server/Packets/UnlearnedSpells.cs
- Fields:
  - `WriteInt32(this.Spells.Count)`
  - `WriteUInt32(spellId)`

```csharp
{
base._worldPacket.WriteInt32(this.Spells.Count);
foreach (uint spellId in this.Spells)
{
base._worldPacket.WriteUInt32(spellId);
}
base._worldPacket.WriteBit(this.SuppressMessaging);
base._worldPacket.FlushBits();
}
```

---

### UpdateAccountData

- File: HermesProxy/World/Server/Packets/UpdateAccountData.cs
- Fields:
  - `WriteInt64(this.Time)`
  - `WriteUInt32(this.Size)`
  - `WriteUInt32(0u)`
  - `WriteInt32(this.CompressedData.Length)`
  - `WriteBytes(this.CompressedData)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.Player);
base._worldPacket.WriteInt64(this.Time);
base._worldPacket.WriteUInt32(this.Size);
if (ModernVersion.GetAccountDataCount() <= 8)
{
base._worldPacket.WriteBits(this.DataType, 3);
}
else
{
base._worldPacket.WriteBits(this.DataType, 4);
}
if (this.CompressedData == null)
{
base._worldPacket.WriteUInt32(0u);
return;
}
base._worldPacket.WriteInt32(this.CompressedData.Length);
base._worldPacket.WriteBytes(this.CompressedData);
}
```

---

### UpdateActionButtons

- File: HermesProxy/World/Server/Packets/UpdateActionButtons.cs
- Fields:
  - `WriteInt64((i < this.ActionButtons.Count)`
  - `WriteUInt8(this.Reason)`

```csharp
{
for (int i = 0; i < 180; i++)
{
base._worldPacket.WriteInt64((i < this.ActionButtons.Count) ? this.ActionButtons[i] : 0);
}
base._worldPacket.WriteUInt8(this.Reason);
}
```

---

### UpdateInstanceOwnership

- File: HermesProxy/World/Server/Packets/UpdateInstanceOwnership.cs
- Fields:
  - `WriteUInt32(this.IOwnInstance)`

```csharp
{
base._worldPacket.WriteUInt32(this.IOwnInstance);
}
```

---

### UpdateLastInstance

- File: HermesProxy/World/Server/Packets/UpdateLastInstance.cs
- Fields:
  - `WriteUInt32(this.MapID)`

```csharp
{
base._worldPacket.WriteUInt32(this.MapID);
}
```

---

### UpdateObject

- File: HermesProxy/World/Server/Packets/UpdateObject.cs
- Fields:
  - `WriteUInt32(this.NumObjUpdates)`
  - `WriteUInt16(this.MapID)`
  - `WriteUInt16((ushort)`
  - `WriteInt32(this.DestroyedGuids.Count + this.OutOfRangeGuids.Count)`
  - `WriteByte(0)`
  - `WriteInt32(bytes.Length)`
  - `WriteBytes(bytes)`
  - `WriteBytes(this.Data)`

```csharp
{
if (ModernVersion.ExpansionVersion >= 3 && !this._gameState.PlayerObjectSent)
{
Log.Print(LogType.Debug, $"[UpdateObject] _playerObjectSent=false, checking for player in {this.ObjectUpdates.Count} updates", "Write", "");
}
if (ModernVersion.ExpansionVersion >= 3 && !this._gameState.PlayerObjectSent)
{
if (this._gameState.PendingLoginUpdates == null)
{
ResetLoginBuffer(this._gameState);
}
bool hasPlayer = false;
foreach (ObjectUpdate update in this.ObjectUpdates)
{
if (update.Guid == this._gameState.CurrentPlayerGuid)
{
hasPlayer = true;
break;
}
}
if (!hasPlayer && this.ObjectUpdates.Count > 0)
{
this._gameState.PendingLoginUpdates.AddRange(this.ObjectUpdates);
this._gameState.PendingLoginDestroys.AddRange(this.DestroyedGuids);
Log.Print(LogType.Debug, $"[UpdateObject] Buffering {this.ObjectUpdates.Count} updates (total: {this._gameState.PendingLoginUpdates.Count})", "Write", "Packets\\UpdatePackets.cs");
```

---

### UpdateWorldState

- File: HermesProxy/World/Server/Packets/UpdateWorldState.cs
- Fields:
  - `WriteUInt32(this.VariableID)`
  - `WriteInt32(this.Value)`

```csharp
{
base._worldPacket.WriteUInt32(this.VariableID);
base._worldPacket.WriteInt32(this.Value);
base._worldPacket.WriteBit(this.Hidden);
base._worldPacket.FlushBits();
}
```

---

### VendorInventory

- File: HermesProxy/World/Server/Packets/VendorInventory.cs
- Fields:
  - `WriteUInt8(this.Reason)`
  - `WriteInt32(this.Items.Count)`

```csharp
{
base._worldPacket.WritePackedGuid128(this.VendorGUID);
base._worldPacket.WriteUInt8(this.Reason);
base._worldPacket.WriteInt32(this.Items.Count);
foreach (VendorItem item in this.Items)
{
item.Write(base._worldPacket);
}
}
```

---

### WaitQueueFinish

- File: HermesProxy/World/Server/Packets/WaitQueueFinish.cs

```csharp
{
}
```

---

### WaitQueueUpdate

- File: HermesProxy/World/Server/Packets/WaitQueueUpdate.cs

```csharp
{
this.WaitInfo.Write(base._worldPacket);
}
```

---

### WeatherPkt

- File: HermesProxy/World/Server/Packets/WeatherPkt.cs
- Fields:
  - `WriteUInt32((uint)`
  - `WriteFloat(this.Intensity)`

```csharp
{
base._worldPacket.WriteUInt32((uint)this.WeatherID);
base._worldPacket.WriteFloat(this.Intensity);
base._worldPacket.WriteBit(this.Abrupt);
base._worldPacket.FlushBits();
}
```

---

### WhoResponsePkt

- File: HermesProxy/World/Server/Packets/WhoResponsePkt.cs
- Fields:
  - `WriteUInt32(this.RequestID)`

```csharp
{
base._worldPacket.WriteUInt32(this.RequestID);
base._worldPacket.WriteBits(this.Players.Count, 6);
base._worldPacket.FlushBits();
this.Players.ForEach(delegate(WhoEntry p)
{
p.Write(base._worldPacket);
});
}
```

---

### WorldServerInfo

- File: HermesProxy/World/Server/Packets/WorldServerInfo.cs
- Fields:
  - `WriteUInt32(this.DifficultyID)`
  - `WriteUInt8(this.IsTournamentRealm)`
  - `WriteUInt32(this.RestrictedAccountMaxLevel.Value)`
  - `WriteUInt64(this.RestrictedAccountMaxMoney.Value)`
  - `WriteUInt32(this.InstanceGroupSize.Value)`

```csharp
{
base._worldPacket.WriteUInt32(this.DifficultyID);
if (ModernVersion.ExpansionVersion >= 3)
{
base._worldPacket.WriteBit(this.IsTournamentRealm != 0);
}
else
{
base._worldPacket.WriteUInt8(this.IsTournamentRealm);
}
base._worldPacket.WriteBit(this.XRealmPvpAlert);
base._worldPacket.WriteBit(this.RestrictedAccountMaxLevel.HasValue);
base._worldPacket.WriteBit(this.RestrictedAccountMaxMoney.HasValue);
base._worldPacket.WriteBit(this.InstanceGroupSize.HasValue);
if (this.RestrictedAccountMaxLevel.HasValue)
{
base._worldPacket.WriteUInt32(this.RestrictedAccountMaxLevel.Value);
}
if (this.RestrictedAccountMaxMoney.HasValue)
{
base._worldPacket.WriteUInt64(this.RestrictedAccountMaxMoney.Value);
}
if (this.InstanceGroupSize.HasValue)
{
base._worldPacket.WriteUInt32(this.InstanceGroupSize.Value);
```

---

### ZoneUnderAttack

- File: HermesProxy/World/Server/Packets/ZoneUnderAttack.cs
- Fields:
  - `WriteInt32(this.AreaID)`

```csharp
{
base._worldPacket.WriteInt32(this.AreaID);
}
```

---

