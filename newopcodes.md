# Modern Opcodes (3.4.3 Build 54261)

Total: 895 opcodes

## Legend
- **HANDLED**: Has a packet handler in HermesProxy
- **UNHANDLED**: No handler exists
- **LEGACY MATCH**: Also exists in 3.3.5a legacy server
- **MODERN ONLY**: Does NOT exist in legacy server
- **IN UNIVERSAL**: Exists in the universal opcode enum (required for translation)

## CMSG (Client -> Server)

| Opcode                                         | Value (Dec) | Value (Hex) | Handled | Legacy Match | In Universal | Handler Location                                                                           |
|------------------------------------------------|-------------|-------------|---------|--------------|--------------|--------------------------------------------------------------------------------------------|
| CMSG_ACCEPT_GUILD_INVITE                       | 13822       | 0x35FE      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1494                                               |
| CMSG_ACCEPT_TRADE                              | 12634       | 0x315A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4385                                               |
| CMSG_ACTIVATE_TAXI                             | 13483       | 0x34AB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4234                                               |
| CMSG_ADDON_LIST                                | 13784       | 0x35D8      | NO      | NO           | YES          |                                                                                            |
| CMSG_ADD_FRIEND                                | 14040       | 0x36D8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3697                                               |
| CMSG_ADD_IGNORE                                | 14044       | 0x36DC      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3709                                               |
| CMSG_ALTER_APPEARANCE                          | 13557       | 0x34F5      | NO      | YES          | YES          |                                                                                            |
| CMSG_AREA_SPIRIT_HEALER_QUERY                  | 13488       | 0x34B0      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3057                                               |
| CMSG_AREA_SPIRIT_HEALER_QUEUE                  | 13489       | 0x34B1      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3058                                               |
| CMSG_AREA_TRIGGER                              | 12758       | 0x31D6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2406                                               |
| CMSG_ARENA_TEAM_ACCEPT                         | 14009       | 0x36B9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:247                                                |
| CMSG_ARENA_TEAM_DECLINE                        | 14010       | 0x36BA      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:248                                                |
| CMSG_ARENA_TEAM_DISBAND                        | 14013       | 0x36BD      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:238                                                |
| CMSG_ARENA_TEAM_INVITE                         | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_ARENA_TEAM_LEADER                         | 14014       | 0x36BE      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:229                                                |
| CMSG_ARENA_TEAM_LEAVE                          | 14011       | 0x36BB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:239                                                |
| CMSG_ARENA_TEAM_QUERY                          | 0           | 0x0000      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:186                                                |
| CMSG_ARENA_TEAM_REMOVE                         | 14012       | 0x36BC      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:228                                                |
| CMSG_ARENA_TEAM_ROSTER                         | 14008       | 0x36B8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:168                                                |
| CMSG_ATTACK_STOP                               | 12886       | 0x3256      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1106                                               |
| CMSG_ATTACK_SWING                              | 12885       | 0x3255      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1098                                               |
| CMSG_AUCTION_HELLO_REQUEST                     | 13514       | 0x34CA      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:255                                                |
| CMSG_AUCTION_LIST_BIDDED_ITEMS                 | 13520       | 0x34D0      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:263                                                |
| CMSG_AUCTION_LIST_ITEMS                        | 13517       | 0x34CD      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:286                                                |
| CMSG_AUCTION_LIST_OWNED_ITEMS                  | 13519       | 0x34CF      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:277                                                |
| CMSG_AUCTION_PLACE_BID                         | 13521       | 0x34D1      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:433                                                |
| CMSG_AUCTION_REMOVE_ITEM                       | 13516       | 0x34CC      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:424                                                |
| CMSG_AUCTION_SELL_ITEM                         | 13515       | 0x34CB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:362                                                |
| CMSG_AUTH_CONTINUED_SESSION                    | 14182       | 0x3766      | NO      | YES          | YES          |                                                                                            |
| CMSG_AUTH_SESSION                              | 14181       | 0x3765      | NO      | YES          | YES          |                                                                                            |
| CMSG_AUTOBANK_ITEM                             | 14743       | 0x3997      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2058                                               |
| CMSG_AUTOSTORE_BANK_ITEM                       | 14742       | 0x3996      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2057                                               |
| CMSG_AUTOSTORE_LOOT_ITEM                       | 12817       | 0x3211      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2182                                               |
| CMSG_AUTO_EQUIP_ITEM                           | 14744       | 0x3998      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2056                                               |
| CMSG_AUTO_EQUIP_ITEM_SLOT                      | 14749       | 0x399D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2069                                               |
| CMSG_AUTO_STORE_BAG_ITEM                       | 14745       | 0x3999      | NO      | YES          | YES          |                                                                                            |
| CMSG_BANKER_ACTIVATE                           | 13491       | 0x34B3      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3050                                               |
| CMSG_BATTLEFIELD_LEAVE                         | 12661       | 0x3175      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:505                                                |
| CMSG_BATTLEFIELD_LIST                          | 12673       | 0x3181      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:495                                                |
| CMSG_BATTLEFIELD_PORT                          | 13605       | 0x3525      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:461                                                |
| CMSG_BATTLEMASTER_HELLO                        | 12977       | 0x32B1      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3056                                               |
| CMSG_BATTLEMASTER_JOIN                         | 13600       | 0x3520      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:443                                                |
| CMSG_BATTLEMASTER_JOIN_ARENA                   | 13601       | 0x3521      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:206                                                |
| CMSG_BATTLEMASTER_JOIN_SKIRMISH                | 13602       | 0x3522      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:217                                                |
| CMSG_BATTLENET_REQUEST                         | 14077       | 0x36FD      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3668                                               |
| CMSG_BATTLE_PAY_GET_PRODUCT_LIST               | 14020       | 0x36C4      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2670                                               |
| CMSG_BATTLE_PAY_GET_PURCHASE_LIST              | 14021       | 0x36C5      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2675                                               |
| CMSG_BATTLE_PET_REQUEST_JOURNAL                | 13861       | 0x3625      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2786                                               |
| CMSG_BEGIN_TRADE                               | 12631       | 0x3157      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4393                                               |
| CMSG_BINDER_ACTIVATE                           | 13490       | 0x34B2      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3051                                               |
| CMSG_BUG                                       | 13959       | 0x3687      | NO      | YES          | YES          |                                                                                            |
| CMSG_BUSY_TRADE                                | 12632       | 0x3158      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4394                                               |
| CMSG_BUY_BACK_ITEM                             | 13476       | 0x34A4      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2090                                               |
| CMSG_BUY_BANK_SLOT                             | 13492       | 0x34B4      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3083                                               |
| CMSG_BUY_ITEM                                  | 13475       | 0x34A3      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1953                                               |
| CMSG_BUY_STABLE_SLOT                           | 12651       | 0x316B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3185                                               |
| CMSG_CALENDAR_GET_NUM_PENDING                  | 13948       | 0x367C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2761                                               |
| CMSG_CANCEL_AURA                               | 12719       | 0x31AF      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4097                                               |
| CMSG_CANCEL_AUTO_REPEAT_SPELL                  | 13543       | 0x34E7      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4086                                               |
| CMSG_CANCEL_CAST                               | 12959       | 0x329F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4058                                               |
| CMSG_CANCEL_CHANNELLING                        | 12906       | 0x326A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4074                                               |
| CMSG_CANCEL_GROWTH_AURA                        | 12911       | 0x326F      | NO      | YES          | YES          |                                                                                            |
| CMSG_CANCEL_MOUNT_AURA                         | 12927       | 0x327F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4105                                               |
| CMSG_CANCEL_QUEUED_SPELL                       | 12674       | 0x3182      | NO      | NO           | YES          |                                                                                            |
| CMSG_CANCEL_TEMP_ENCHANTMENT                   | 13554       | 0x34F2      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2147                                               |
| CMSG_CANCEL_TRADE                              | 12636       | 0x315C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4395                                               |
| CMSG_CAN_DUEL                                  | 13924       | 0x3664      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1121                                               |
| CMSG_CAST_SPELL                                | 12956       | 0x329C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3862                                               |
| CMSG_CHANGE_REALM_TICKET                       | 14081       | 0x3701      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3648                                               |
| CMSG_CHARACTER_RENAME_REQUEST                  | 14025       | 0x36C9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:764                                                |
| CMSG_CHAR_DELETE                               | 13981       | 0x369D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:580                                                |
| CMSG_CHAT_ADDON_MESSAGE                        | 14318       | 0x37EE      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:981                                                |
| CMSG_CHAT_CHANNEL_ANNOUNCEMENTS                | 14307       | 0x37E3      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:793                                                |
| CMSG_CHAT_CHANNEL_BAN                          | 14305       | 0x37E1      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_DECLINE_INVITE               | 14310       | 0x37E6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:828                                                |
| CMSG_CHAT_CHANNEL_DISPLAY_LIST                 | 14294       | 0x37D6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:810                                                |
| CMSG_CHAT_CHANNEL_INVITE                       | 14303       | 0x37DF      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_KICK                         | 14304       | 0x37E0      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_LIST                         | 14293       | 0x37D5      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:801                                                |
| CMSG_CHAT_CHANNEL_MODERATOR                    | 14299       | 0x37DB      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_OWNER                        | 14297       | 0x37D9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:792                                                |
| CMSG_CHAT_CHANNEL_PASSWORD                     | 14295       | 0x37D7      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_SET_OWNER                    | 14296       | 0x37D8      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_SILENCE_ALL                  | 14308       | 0x37E4      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_UNBAN                        | 14306       | 0x37E2      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_UNMODERATOR                  | 14300       | 0x37DC      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_CHANNEL_UNSILENCE_ALL                | 14309       | 0x37E5      | NO      | YES          | YES          |                                                                                            |
| CMSG_CHAT_IGNORED                              | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_CHAT_JOIN_CHANNEL                         | 14280       | 0x37C8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:773                                                |
| CMSG_CHAT_LEAVE_CHANNEL                        | 14281       | 0x37C9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:782                                                |
| CMSG_CHAT_MESSAGE_AFK                          | 14291       | 0x37D3      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:839                                                |
| CMSG_CHAT_MESSAGE_CHANNEL                      | 14287       | 0x37CF      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:873                                                |
| CMSG_CHAT_MESSAGE_DND                          | 14292       | 0x37D4      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:856                                                |
| CMSG_CHAT_MESSAGE_EMOTE                        | 14312       | 0x37E8      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:907                                                |
| CMSG_CHAT_MESSAGE_GUILD                        | 14289       | 0x37D1      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:924                                                |
| CMSG_CHAT_MESSAGE_INSTANCE_CHAT                | 14316       | 0x37EC      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:931                                                |
| CMSG_CHAT_MESSAGE_OFFICER                      | 14290       | 0x37D2      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:925                                                |
| CMSG_CHAT_MESSAGE_PARTY                        | 14314       | 0x37EA      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:926                                                |
| CMSG_CHAT_MESSAGE_RAID                         | 14315       | 0x37EB      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:927                                                |
| CMSG_CHAT_MESSAGE_RAID_WARNING                 | 14317       | 0x37ED      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:928                                                |
| CMSG_CHAT_MESSAGE_SAY                          | 14311       | 0x37E7      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:929                                                |
| CMSG_CHAT_MESSAGE_WHISPER                      | 14288       | 0x37D0      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:890                                                |
| CMSG_CHAT_MESSAGE_YELL                         | 14313       | 0x37E9      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:930                                                |
| CMSG_CHAT_REGISTER_ADDON_PREFIXES              | 14285       | 0x37CD      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1026                                               |
| CMSG_CHAT_UNREGISTER_ALL_ADDON_PREFIXES        | 14286       | 0x37CE      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1035                                               |
| CMSG_CLEAR_TRADE_ITEM                          | 12638       | 0x315E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4404                                               |
| CMSG_CLOSE_INTERACTION                         | 13459       | 0x3493      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2806                                               |
| CMSG_COMPLETE_CINEMATIC                        | 13637       | 0x3545      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2462                                               |
| CMSG_CONFIRM_RESPEC_WIPE                       | 12813       | 0x320D      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3104                                               |
| CMSG_CONVERT_RAID                              | 13905       | 0x3651      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1250                                               |
| CMSG_CREATE_CHARACTER                          | 13893       | 0x3645      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:562                                                |
| CMSG_DB_QUERY_BULK                             | 13797       | 0x35E5      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1831                                               |
| CMSG_DECLINE_GUILD_INVITES                     | 13597       | 0x351D      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1528                                               |
| CMSG_DECLINE_PETITION                          | 13620       | 0x3534      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3325                                               |
| CMSG_DEL_FRIEND                                | 14041       | 0x36D9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3717                                               |
| CMSG_DEL_IGNORE                                | 14045       | 0x36DD      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3718                                               |
| CMSG_DESTROY_ITEM                              | 12947       | 0x3293      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2044                                               |
| CMSG_DF_GET_JOIN_STATUS                        | 13846       | 0x3616      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2756                                               |
| CMSG_DF_GET_SYSTEM_INFO                        | 13845       | 0x3615      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2690                                               |
| CMSG_DF_JOIN                                   | 13835       | 0x360B      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2705                                               |
| CMSG_DF_LEAVE                                  | 13844       | 0x3614      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2749                                               |
| CMSG_DF_PROPOSAL_RESPONSE                      | 13833       | 0x3609      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2724; HermesProxy/World/Server/WorldSocket.cs:2740 |
| CMSG_DF_SET_ROLES                              | 13847       | 0x3617      | NO      | NO           | YES          |                                                                                            |
| CMSG_DISCARDED_TIME_SYNC_ACKS                  | 14913       | 0x3A41      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2826                                               |
| CMSG_DO_READY_CHECK                            | 13877       | 0x3635      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1257                                               |
| CMSG_DUEL_RESPONSE                             | 13538       | 0x34E2      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1130                                               |
| CMSG_ENABLE_NAGLE                              | 14187       | 0x376B      | NO      | NO           | YES          |                                                                                            |
| CMSG_ENABLE_TAXI_NODE                          | 13481       | 0x34A9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4226                                               |
| CMSG_ENTER_ENCRYPTED_MODE_ACK                  | 14183       | 0x3767      | NO      | NO           | YES          |                                                                                            |
| CMSG_ENUM_CHARACTERS                           | 13801       | 0x35E9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:523                                                |
| CMSG_FAR_SIGHT                                 | 13544       | 0x34E8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2469                                               |
| CMSG_GAME_OBJ_REPORT_USE                       | 13551       | 0x34EF      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1157                                               |
| CMSG_GAME_OBJ_USE                              | 13550       | 0x34EE      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1147                                               |
| CMSG_GENERATE_RANDOM_CHARACTER_NAME            | 13800       | 0x35E8      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:554                                                |
| CMSG_GET_ACCOUNT_CHARACTER_LIST                | 14015       | 0x36BF      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:530                                                |
| CMSG_GET_UNDELETE_CHARACTER_COOLDOWN_STATUS    | 14055       | 0x36E7      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2680                                               |
| CMSG_GM_TICKET_CREATE                          | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_GM_TICKET_DELETE_TICKET                   | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_GM_TICKET_GET_CASE_STATUS                 | 13967       | 0x368F      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2791                                               |
| CMSG_GM_TICKET_GET_TICKET                      | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_GM_TICKET_UPDATE_TEXT                     | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_GOSSIP_SELECT_OPTION                      | 13460       | 0x3494      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3066                                               |
| CMSG_GROUP_DECLINE                             | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_GROUP_REMOVE_LEADER                       | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_GROUP_SWAP_SUB_GROUP                      | 0           | 0x0000      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1332                                               |
| CMSG_GUILD_ADD_RANK                            | 12388       | 0x3064      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1464                                               |
| CMSG_GUILD_AUTO_DECLINE_INVITATION             | 12385       | 0x3061      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1540                                               |
| CMSG_GUILD_BANK_ACTIVATE                       | 13493       | 0x34B5      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1547                                               |
| CMSG_GUILD_BANK_BUY_TAB                        | 13507       | 0x34C3      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1611                                               |
| CMSG_GUILD_BANK_DEPOSIT_MONEY                  | 13509       | 0x34C5      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1566                                               |
| CMSG_GUILD_BANK_LOG_QUERY                      | 12418       | 0x3082      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1594                                               |
| CMSG_GUILD_BANK_QUERY_TAB                      | 13506       | 0x34C2      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1556                                               |
| CMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY_QUERY | 12419       | 0x3083      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1359                                               |
| CMSG_GUILD_BANK_SET_TAB_TEXT                   | 12422       | 0x3086      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1602                                               |
| CMSG_GUILD_BANK_SWAP_ITEMS                     | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_GUILD_BANK_TEXT_QUERY                     | 12423       | 0x3087      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1575                                               |
| CMSG_GUILD_BANK_UPDATE_TAB                     | 13508       | 0x34C4      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1583                                               |
| CMSG_GUILD_BANK_WITHDRAW_MONEY                 | 13510       | 0x34C6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1620                                               |
| CMSG_GUILD_DECLINE_INVITATION                  | 12384       | 0x3060      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1501                                               |
| CMSG_GUILD_DELETE                              | 12392       | 0x3068      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1508                                               |
| CMSG_GUILD_DELETE_RANK                         | 12389       | 0x3065      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1472                                               |
| CMSG_GUILD_DEMOTE_MEMBER                       | 12382       | 0x305E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1411                                               |
| CMSG_GUILD_EVENT_LOG_QUERY                     | 12421       | 0x3085      | NO      | NO           | YES          |                                                                                            |
| CMSG_GUILD_GET_ROSTER                          | 12403       | 0x3073      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1369                                               |
| CMSG_GUILD_INVITE_BY_NAME                      | 13832       | 0x3608      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1427                                               |
| CMSG_GUILD_LEAVE                               | 12386       | 0x3062      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1487                                               |
| CMSG_GUILD_OFFICER_LIST                        | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_GUILD_OFFICER_REMOVE_MEMBER               | 12387       | 0x3063      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1419                                               |
| CMSG_GUILD_PROMOTE_MEMBER                      | 12381       | 0x305D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1403                                               |
| CMSG_GUILD_SET_ACHIEVEMENT_TRACKING            | 12399       | 0x306F      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2766                                               |
| CMSG_GUILD_SET_GUILD_MASTER                    | 14032       | 0x36D0      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1479                                               |
| CMSG_GUILD_SET_MEMBER_NOTE                     | 12402       | 0x3072      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1394                                               |
| CMSG_GUILD_SET_RANK_PERMISSIONS                | 12391       | 0x3067      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1445                                               |
| CMSG_GUILD_UPDATE_INFO_TEXT                    | 12405       | 0x3075      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1386                                               |
| CMSG_GUILD_UPDATE_MOTD_TEXT                    | 12404       | 0x3074      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1378                                               |
| CMSG_HEARTH_AND_RESURRECT                      | 13574       | 0x3506      | NO      | YES          | YES          |                                                                                            |
| CMSG_HOTFIX_REQUEST                            | 13798       | 0x35E6      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1924                                               |
| CMSG_IGNORE_TRADE                              | 12633       | 0x3159      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4397                                               |
| CMSG_INITIATE_TRADE                            | 12630       | 0x3156      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4360                                               |
| CMSG_INSPECT                                   | 13609       | 0x3529      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:730                                                |
| CMSG_INSPECT_PVP                               | 13987       | 0x36A3      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:746                                                |
| CMSG_INSTANCE_LOCK_RESPONSE                    | 13579       | 0x350B      | NO      | YES          | YES          |                                                                                            |
| CMSG_ITEM_QUERY_SINGLE                         | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_ITEM_TEXT_QUERY                           | 12997       | 0x32C5      | NO      | YES          | YES          |                                                                                            |
| CMSG_KEEP_ALIVE                                | 13953       | 0x3681      | NO      | YES          | YES          |                                                                                            |
| CMSG_LEARN_TALENT                              | 13650       | 0x3552      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4132                                               |
| CMSG_LEARN_TALENT_GROUP                        | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_LEAVE_GROUP                               | 13900       | 0x364C      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1197                                               |
| CMSG_LFG_LIST_GET_STATUS                       | 13837       | 0x360D      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2781                                               |
| CMSG_LIST_INVENTORY                            | 13473       | 0x34A1      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3052                                               |
| CMSG_LOADING_SCREEN_NOTIFY                     | 13817       | 0x35F9      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:588                                                |
| CMSG_LOGOUT_CANCEL                             | 13528       | 0x34D8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:653                                                |
| CMSG_LOGOUT_REQUEST                            | 13526       | 0x34D6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:646                                                |
| CMSG_LOG_DISCONNECT                            | 14185       | 0x3769      | NO      | NO           | YES          |                                                                                            |
| CMSG_LOOT_CURRENCY                             | 0           | 0x0000      | NO      | NO           | YES          |                                                                                            |
| CMSG_LOOT_ITEM                                 | 12817       | 0x3211      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2181                                               |
| CMSG_LOOT_MASTER_GIVE                          | 0           | 0x0000      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2244                                               |
| CMSG_LOOT_MONEY                                | 12816       | 0x3210      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2202                                               |
| CMSG_LOOT_RELEASE                              | 12819       | 0x3213      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2173                                               |
| CMSG_LOOT_ROLL                                 | 12820       | 0x3214      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2234                                               |
| CMSG_LOOT_UNIT                                 | 12815       | 0x320F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2193                                               |
| CMSG_MAIL_CREATE_TEXT_ITEM                     | 13627       | 0x353B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2272                                               |
| CMSG_MAIL_DELETE                               | 12837       | 0x3225      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2285                                               |
| CMSG_MAIL_GET_LIST                             | 13622       | 0x3536      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2264                                               |
| CMSG_MAIL_MARK_AS_READ                         | 13626       | 0x353A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2298                                               |
| CMSG_MAIL_RETURN_TO_SENDER                     | 13912       | 0x3658      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2307                                               |
| CMSG_MAIL_TAKE_ITEM                            | 13624       | 0x3538      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2320                                               |
| CMSG_MAIL_TAKE_MONEY                           | 13623       | 0x3537      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2333                                               |
| CMSG_MESSAGECHAT                               | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_MINIMAP_PING                              | 13902       | 0x364E      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1298                                               |
| CMSG_MOUNT_SPECIAL_ANIM                        | 12928       | 0x3280      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2478                                               |
| CMSG_MOVE_CHANGE_TRANSPORT                     | 14895       | 0x3A2F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2836                                               |
| CMSG_MOVE_DISMISS_VEHICLE                      | 14899       | 0x3A33      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2837                                               |
| CMSG_MOVE_DOUBLE_JUMP                          | 14827       | 0x39EB      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2866                                               |
| CMSG_MOVE_FALL_LAND                            | 14843       | 0x39FB      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2838                                               |
| CMSG_MOVE_FALL_RESET                           | 14873       | 0x3A19      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2839                                               |
| CMSG_MOVE_FEATHER_FALL_ACK                     | 14876       | 0x3A1C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2959                                               |
| CMSG_MOVE_FORCE_FLIGHT_BACK_SPEED_CHANGE_ACK   | 14894       | 0x3A2E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2910                                               |
| CMSG_MOVE_FORCE_FLIGHT_SPEED_CHANGE_ACK        | 14893       | 0x3A2D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2911                                               |
| CMSG_MOVE_FORCE_PITCH_RATE_CHANGE_ACK          | 14898       | 0x3A32      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2912                                               |
| CMSG_MOVE_FORCE_ROOT_ACK                       | 14862       | 0x3A0E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2980                                               |
| CMSG_MOVE_FORCE_RUN_BACK_SPEED_CHANGE_ACK      | 14860       | 0x3A0C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2913                                               |
| CMSG_MOVE_FORCE_RUN_SPEED_CHANGE_ACK           | 14859       | 0x3A0B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2914                                               |
| CMSG_MOVE_FORCE_SWIM_BACK_SPEED_CHANGE_ACK     | 14882       | 0x3A22      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2915                                               |
| CMSG_MOVE_FORCE_SWIM_SPEED_CHANGE_ACK          | 14861       | 0x3A0D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2916                                               |
| CMSG_MOVE_FORCE_TURN_RATE_CHANGE_ACK           | 14883       | 0x3A23      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2917                                               |
| CMSG_MOVE_FORCE_UNROOT_ACK                     | 14863       | 0x3A0F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2981                                               |
| CMSG_MOVE_FORCE_WALK_SPEED_CHANGE_ACK          | 14881       | 0x3A21      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2918                                               |
| CMSG_MOVE_GRAVITY_DISABLE_ACK                  | 14901       | 0x3A35      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2983                                               |
| CMSG_MOVE_GRAVITY_ENABLE_ACK                   | 14902       | 0x3A36      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2984                                               |
| CMSG_MOVE_HEARTBEAT                            | 14864       | 0x3A10      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2840                                               |
| CMSG_MOVE_HOVER_ACK                            | 14867       | 0x3A13      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2960                                               |
| CMSG_MOVE_INIT_ACTIVE_MOVER_COMPLETE           | 14918       | 0x3A46      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3009                                               |
| CMSG_MOVE_JUMP                                 | 14826       | 0x39EA      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2841                                               |
| CMSG_MOVE_KNOCK_BACK_ACK                       | 14866       | 0x3A12      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2982                                               |
| CMSG_MOVE_REMOVE_MOVEMENT_FORCES               | 14871       | 0x3A17      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2842                                               |
| CMSG_MOVE_SET_CAN_FLY_ACK                      | 14887       | 0x3A27      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2961                                               |
| CMSG_MOVE_SET_FACING                           | 14857       | 0x3A09      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2843                                               |
| CMSG_MOVE_SET_FACING_HEARTBEAT                 | 14943       | 0x3A5F      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2844                                               |
| CMSG_MOVE_SET_FLIGHT_SPEED_CHEAT               | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_MOVE_SET_FLY                              | 14888       | 0x3A28      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2845                                               |
| CMSG_MOVE_SET_PITCH                            | 14858       | 0x3A0A      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2846                                               |
| CMSG_MOVE_SET_RUN_MODE                         | 14834       | 0x39F2      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2847                                               |
| CMSG_MOVE_SET_WALK_MODE                        | 14835       | 0x39F3      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2848                                               |
| CMSG_MOVE_SPLINE_DONE                          | 14872       | 0x3A18      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3017                                               |
| CMSG_MOVE_START_ASCEND                         | 14889       | 0x3A29      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2849                                               |
| CMSG_MOVE_START_BACKWARD                       | 14821       | 0x39E5      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2850                                               |
| CMSG_MOVE_START_DESCEND                        | 14896       | 0x3A30      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2851                                               |
| CMSG_MOVE_START_FORWARD                        | 14820       | 0x39E4      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2852                                               |
| CMSG_MOVE_START_PITCH_DOWN                     | 14832       | 0x39F0      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2853                                               |
| CMSG_MOVE_START_PITCH_UP                       | 14831       | 0x39EF      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2854                                               |
| CMSG_MOVE_START_STRAFE_LEFT                    | 14823       | 0x39E7      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2858                                               |
| CMSG_MOVE_START_STRAFE_RIGHT                   | 14824       | 0x39E8      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2859                                               |
| CMSG_MOVE_START_SWIM                           | 14844       | 0x39FC      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2855                                               |
| CMSG_MOVE_START_TURN_LEFT                      | 14828       | 0x39EC      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2856                                               |
| CMSG_MOVE_START_TURN_RIGHT                     | 14829       | 0x39ED      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2857                                               |
| CMSG_MOVE_STOP                                 | 14822       | 0x39E6      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2860                                               |
| CMSG_MOVE_STOP_ASCEND                          | 14890       | 0x3A2A      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2861                                               |
| CMSG_MOVE_STOP_BACKWARD                        | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_MOVE_STOP_PITCH                           | 14833       | 0x39F1      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2862                                               |
| CMSG_MOVE_STOP_STRAFE                          | 14825       | 0x39E9      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2863                                               |
| CMSG_MOVE_STOP_SWIM                            | 14845       | 0x39FD      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2864                                               |
| CMSG_MOVE_STOP_TURN                            | 14830       | 0x39EE      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2865                                               |
| CMSG_MOVE_TELEPORT_ACK                         | 14842       | 0x39FA      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2885                                               |
| CMSG_MOVE_TIME_SKIPPED                         | 14875       | 0x3A1B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3034                                               |
| CMSG_MOVE_WATER_WALK_ACK                       | 14877       | 0x3A1D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2962                                               |
| CMSG_NEXT_CINEMATIC_CAMERA                     | 13636       | 0x3544      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2461                                               |
| CMSG_OBJECT_UPDATE_FAILED                      | 12675       | 0x3183      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2627                                               |
| CMSG_OFFER_PETITION                            | 13053       | 0x32FD      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3312                                               |
| CMSG_OPENING_CINEMATIC                         | 13635       | 0x3543      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2460                                               |
| CMSG_OPEN_ITEM                                 | 12998       | 0x32C6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2128                                               |
| CMSG_OPT_OUT_OF_LOOT                           | 13558       | 0x34F6      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2219                                               |
| CMSG_OVERRIDE_SCREEN_FLASH                     | 13598       | 0x351E      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2660                                               |
| CMSG_PARTY_INVITE                              | 13828       | 0x3604      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1166                                               |
| CMSG_PARTY_INVITE_RESPONSE                     | 13830       | 0x3606      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1178                                               |
| CMSG_PARTY_UNINVITE                            | 13898       | 0x364A      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1205                                               |
| CMSG_PETITION_BUY                              | 13512       | 0x34C8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3243                                               |
| CMSG_PETITION_RENAME_GUILD                     | 14033       | 0x36D1      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3303                                               |
| CMSG_PETITION_SHOW_SIGNATURES                  | 13513       | 0x34C9      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3286                                               |
| CMSG_PET_ABANDON                               | 13453       | 0x348D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3193                                               |
| CMSG_PET_ACTION                                | 13451       | 0x348B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3129                                               |
| CMSG_PET_CANCEL_AURA                           | 13454       | 0x348E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3227                                               |
| CMSG_PET_CAST_SPELL                            | 12955       | 0x329B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3945                                               |
| CMSG_PET_RENAME                                | 13958       | 0x3686      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3157                                               |
| CMSG_PET_SET_ACTION                            | 13450       | 0x348A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3147                                               |
| CMSG_PET_STOP_ATTACK                           | 13452       | 0x348C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3139                                               |
| CMSG_PING                                      | 14184       | 0x3768      | NO      | YES          | YES          |                                                                                            |
| CMSG_PLAYER_AI_CHEAT                           | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_PLAYER_DIFFICULTY_CHANGE                  | 0           | 0x0000      | NO      | NO           | YES          |                                                                                            |
| CMSG_PLAYER_LOGIN                              | 13803       | 0x35EB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:616                                                |
| CMSG_PUSH_QUEST_TO_PARTY                       | 13471       | 0x349F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3596                                               |
| CMSG_PUSH_SPELL_TO_ACTION_BAR                  | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_PVP_LOG_DATA                              | 12671       | 0x317F      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:488                                                |
| CMSG_QUERY_CORPSE_LOCATION_FROM_CLIENT         | 13922       | 0x3662      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2437                                               |
| CMSG_QUERY_COUNTDOWN_TIMER                     | 12714       | 0x31AA      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2771                                               |
| CMSG_QUERY_CREATURE                            | 12912       | 0x3270      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3373                                               |
| CMSG_QUERY_GAME_OBJECT                         | 12913       | 0x3271      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3382                                               |
| CMSG_QUERY_GUILD_INFO                          | 13963       | 0x368B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1341                                               |
| CMSG_QUERY_NEXT_MAIL_TIME                      | 13625       | 0x3539      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2257                                               |
| CMSG_QUERY_NPC_TEXT                            | 12914       | 0x3272      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3411                                               |
| CMSG_QUERY_PAGE_TEXT                           | 12916       | 0x3274      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3402                                               |
| CMSG_QUERY_PETITION                            | 12919       | 0x3277      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3294                                               |
| CMSG_QUERY_PET_NAME                            | 12917       | 0x3275      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3420                                               |
| CMSG_QUERY_PLAYER_NAMES                        | 14194       | 0x3772      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:605                                                |
| CMSG_QUERY_QUESTS_COMPLETED                    | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_QUERY_QUEST_INFO                          | 12915       | 0x3273      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3365                                               |
| CMSG_QUERY_TIME                                | 13525       | 0x34D5      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3358                                               |
| CMSG_QUEST_CONFIRM_ACCEPT                      | 13470       | 0x349E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3588                                               |
| CMSG_QUEST_GIVER_ACCEPT_QUEST                  | 13464       | 0x3498      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3466                                               |
| CMSG_QUEST_GIVER_CHOOSE_REWARD                 | 13466       | 0x349A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3544                                               |
| CMSG_QUEST_GIVER_CLOSE_QUEST                   | 13641       | 0x3549      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2831                                               |
| CMSG_QUEST_GIVER_COMPLETE_QUEST                | 13465       | 0x3499      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3579                                               |
| CMSG_QUEST_GIVER_HELLO                         | 13462       | 0x3496      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3527                                               |
| CMSG_QUEST_GIVER_QUERY_QUEST                   | 13463       | 0x3497      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3453                                               |
| CMSG_QUEST_GIVER_REQUEST_REWARD                | 13467       | 0x349B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3535                                               |
| CMSG_QUEST_GIVER_STATUS_MULTIPLE_QUERY         | 13469       | 0x349D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3495                                               |
| CMSG_QUEST_GIVER_STATUS_QUERY                  | 13468       | 0x349C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3487                                               |
| CMSG_QUEST_LOG_REMOVE_QUEST                    | 13614       | 0x352E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3479                                               |
| CMSG_QUEST_POI_QUERY                           | 14002       | 0x36B2      | NO      | YES          | YES          |                                                                                            |
| CMSG_QUEST_PUSH_RESULT                         | 13472       | 0x34A0      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3604                                               |
| CMSG_QUEUED_MESSAGES_END                       | 14188       | 0x376C      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2665                                               |
| CMSG_RAID_READY_CHECK                          | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_RAID_READY_CHECK_CONFIRM                  | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_RAID_READY_CHECK_FINISHED                 | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_RANDOM_ROLL                               | 13911       | 0x3657      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1306                                               |
| CMSG_READY_CHECK_RESPONSE                      | 13878       | 0x3636      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1264                                               |
| CMSG_READ_ITEM                                 | 12999       | 0x32C7      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2079                                               |
| CMSG_REAL_GROUP_UPDATE                         | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_RECLAIM_CORPSE                            | 13531       | 0x34DB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2444                                               |
| CMSG_REMOVE_GLYPH                              | 13056       | 0x3300      | NO      | YES          | YES          |                                                                                            |
| CMSG_REPAIR_ITEM                               | 13548       | 0x34EC      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2100                                               |
| CMSG_REPOP_REQUEST                             | 13606       | 0x3526      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2426                                               |
| CMSG_REPORT_CLIENT_VARIABLES                   | 14087       | 0x3707      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2811                                               |
| CMSG_REPORT_ENABLED_ADDONS                     | 14086       | 0x3706      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2816                                               |
| CMSG_REPORT_KEYBINDING_EXECUTION_COUNTS        | 14088       | 0x3708      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2821                                               |
| CMSG_REQUEST_ACCOUNT_DATA                      | 13972       | 0x3694      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1075                                               |
| CMSG_REQUEST_BATTLEFIELD_STATUS                | 13789       | 0x35DD      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:481                                                |
| CMSG_REQUEST_CEMETERY_LIST                     | 12665       | 0x3179      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2801                                               |
| CMSG_REQUEST_CONQUEST_FORMULA_CONSTANTS        | 12980       | 0x32B4      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2615                                               |
| CMSG_REQUEST_FORCED_REACTIONS                  | 12805       | 0x3205      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2776                                               |
| CMSG_REQUEST_LFG_LIST_BLACKLIST                | 12964       | 0x32A4      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2512                                               |
| CMSG_REQUEST_PARTY_MEMBER_STATS                | 13910       | 0x3656      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1315                                               |
| CMSG_REQUEST_PET_INFO                          | 13456       | 0x3490      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3236                                               |
| CMSG_REQUEST_PLAYED_TIME                       | 12922       | 0x327A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:660                                                |
| CMSG_REQUEST_PVP_REWARDS                       | 12694       | 0x3196      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2650                                               |
| CMSG_REQUEST_RAID_INFO                         | 14034       | 0x36D2      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1946                                               |
| CMSG_REQUEST_RATED_PVP_INFO                    | 13796       | 0x35E4      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2655                                               |
| CMSG_REQUEST_STABLED_PETS                      | 13457       | 0x3491      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3177                                               |
| CMSG_REQUEST_VEHICLE_EXIT                      | 12855       | 0x3237      | NO      | YES          | YES          |                                                                                            |
| CMSG_REQUEST_VEHICLE_NEXT_SEAT                 | 12857       | 0x3239      | NO      | YES          | YES          |                                                                                            |
| CMSG_REQUEST_VEHICLE_PREV_SEAT                 | 12856       | 0x3238      | NO      | YES          | YES          |                                                                                            |
| CMSG_REQUEST_VEHICLE_SWITCH_SEAT               | 12858       | 0x323A      | NO      | YES          | YES          |                                                                                            |
| CMSG_RESET_FACTION_CHEAT                       | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_RESET_INSTANCES                           | 13930       | 0x366A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1939                                               |
| CMSG_RESURRECT_RESPONSE                        | 13957       | 0x3685      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4141                                               |
| CMSG_SAVE_CUF_PROFILES                         | 12686       | 0x318E      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1092                                               |
| CMSG_SAVE_GUILD_EMBLEM                         | 12968       | 0x32A8      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1515                                               |
| CMSG_SELF_RES                                  | 13617       | 0x3531      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4150                                               |
| CMSG_SELL_ITEM                                 | 13474       | 0x34A2      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1973                                               |
| CMSG_SEND_CONTACT_LIST                         | 14039       | 0x36D7      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_SEND_MAIL                                 | 13819       | 0x35FB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2375                                               |
| CMSG_SEND_TEXT_EMOTE                           | 13448       | 0x3488      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1016                                               |
| CMSG_SERVER_TIME_OFFSET_REQUEST                | 13980       | 0x369C      | NO      | NO           | YES          |                                                                                            |
| CMSG_SET_ACTIONBAR_TOGGLES                     | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_SET_ACTION_BAR_TOGGLES                    | 13618       | 0x3532      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:705                                                |
| CMSG_SET_ACTION_BUTTON                         | 13661       | 0x355D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:695                                                |
| CMSG_SET_ACTIVE_MOVER                          | 14908       | 0x3A3C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3001                                               |
| CMSG_SET_AMMO                                  | 13662       | 0x355E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2139                                               |
| CMSG_SET_ASSISTANT_LEADER                      | 13906       | 0x3652      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1217                                               |
| CMSG_SET_CONTACT_NOTES                         | 14042       | 0x36DA      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3726                                               |
| CMSG_SET_DIFFICULTY_ID                         | 12834       | 0x3222      | NO      | NO           | YES          |                                                                                            |
| CMSG_SET_DUNGEON_DIFFICULTY                    | 13956       | 0x3684      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2633                                               |
| CMSG_SET_EVERYONE_IS_ASSISTANT                 | 13850       | 0x361A      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1226                                               |
| CMSG_SET_FACTION_ATWAR                         | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_SET_FACTION_AT_WAR                        | 13534       | 0x34DE      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3613                                               |
| CMSG_SET_FACTION_INACTIVE                      | 13536       | 0x34E0      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3631                                               |
| CMSG_SET_FACTION_NOT_AT_WAR                    | 13535       | 0x34DF      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:3622                                               |
| CMSG_SET_LOOT_METHOD                           | 13899       | 0x364B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2209                                               |
| CMSG_SET_PARTY_LEADER                          | 13901       | 0x364D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1242                                               |
| CMSG_SET_PVP                                   | 12972       | 0x32AC      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:687                                                |
| CMSG_SET_SELECTION                             | 13608       | 0x3528      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2418                                               |
| CMSG_SET_SHEATHED                              | 13449       | 0x3489      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1113                                               |
| CMSG_SET_TITLE                                 | 12926       | 0x327E      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:672                                                |
| CMSG_SET_TRADE_GOLD                            | 12639       | 0x315F      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4368                                               |
| CMSG_SET_TRADE_ITEM                            | 12637       | 0x315D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4421                                               |
| CMSG_SET_WATCHED_FACTION                       | 13537       | 0x34E1      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3640                                               |
| CMSG_SIGN_PETITION                             | 13619       | 0x3533      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3333                                               |
| CMSG_SOCIAL_CONTRACT_REQUEST                   | 14156       | 0x374C      | NO      | NO           | YES          |                                                                                            |
| CMSG_SOCKETSPELLS                              | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_SOCKET_GEMS                               | 13547       | 0x34EB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2113                                               |
| CMSG_SORT_BAGS                                 | 0           | 0x0000      | NO      | NO           | YES          |                                                                                            |
| CMSG_SPELL_CLICK                               | 13461       | 0x3495      | NO      | YES          | YES          |                                                                                            |
| CMSG_SPIRIT_HEALER_ACTIVATE                    | 13487       | 0x34AF      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3053                                               |
| CMSG_SPLIT_ITEM                                | 14748       | 0x399C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1993                                               |
| CMSG_STABLE_PET                                | 12648       | 0x3168      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3201                                               |
| CMSG_STABLE_REVIVE_PET                         | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_STABLE_SWAP_PET                           | 12650       | 0x316A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3218                                               |
| CMSG_STAND_STATE_CHANGE                        | 12684       | 0x318C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2452                                               |
| CMSG_SUMMON_RESPONSE                           | 13932       | 0x366C      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1286                                               |
| CMSG_SWAP_INV_ITEM                             | 14747       | 0x399B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2016                                               |
| CMSG_SWAP_ITEM                                 | 14746       | 0x399A      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2029                                               |
| CMSG_TABARD_VENDOR_ACTIVATE                    | 12969       | 0x32A9      | NO      | NO           | YES          |                                                                                            |
| CMSG_TALK_TO_GOSSIP                            | 13458       | 0x3492      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3054                                               |
| CMSG_TAXI_NODE_STATUS_QUERY                    | 13480       | 0x34A8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4217                                               |
| CMSG_TAXI_QUERY_AVAILABLE_NODES                | 13482       | 0x34AA      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4218                                               |
| CMSG_TIME_SYNC_RESPONSE                        | 14909       | 0x3A3D      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2394                                               |
| CMSG_TOGGLE_PVP                                | 12971       | 0x32AB      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:680                                                |
| CMSG_TOTEM_DESTROYED                           | 13560       | 0x34F8      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4157                                               |
| CMSG_TRAINER_BUY_SPELL                         | 13486       | 0x34AE      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3091                                               |
| CMSG_TRAINER_LIST                              | 13485       | 0x34AD      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3055                                               |
| CMSG_TURN_IN_PETITION                          | 13621       | 0x3535      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3342                                               |
| CMSG_TUTORIAL_FLAG                             | 14052       | 0x36E4      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2485; HermesProxy/World/Server/WorldSocket.cs:2796 |
| CMSG_UNACCEPT_TRADE                            | 12635       | 0x315B      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4396                                               |
| CMSG_UNLEARN_SKILL                             | 13541       | 0x34E5      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:713                                                |
| CMSG_UNSTABLE_PET                              | 12649       | 0x3169      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3209                                               |
| CMSG_UPDATE_ACCOUNT_DATA                       | 13973       | 0x3695      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:1069                                               |
| CMSG_UPDATE_MISSILE_TRAJECTORY                 | 14915       | 0x3A43      | NO      | YES          | YES          |                                                                                            |
| CMSG_UPDATE_RAID_TARGET                        | 13907       | 0x3653      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:1277                                               |
| CMSG_UPDATE_VAS_PURCHASE_STATES                | 14075       | 0x36FB      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2685                                               |
| CMSG_USE_ITEM                                  | 12952       | 0x3298      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:4001                                               |
| CMSG_VIOLENCE_LEVEL                            | 12679       | 0x3187      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2645                                               |
| CMSG_WARDEN_DATA                               | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |
| CMSG_WHO                                       | 13955       | 0x3683      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:3429                                               |
| CMSG_WHOIS                                     | 0           | 0x0000      | NO      | NO           | **NO**       |                                                                                            |
| CMSG_WORLD_PORT_RESPONSE                       | 13818       | 0x35FA      | YES     | NO           | YES          | HermesProxy/World/Server/WorldSocket.cs:2902                                               |
| CMSG_WRAP_ITEM                                 | 14740       | 0x3994      | YES     | YES          | YES          | HermesProxy/World/Server/WorldSocket.cs:2158                                               |
| CMSG_ZONEUPDATE                                | 0           | 0x0000      | NO      | YES          | YES          |                                                                                            |

## SMSG (Server -> Client)

| Opcode | Value (Dec) | Value (Hex) | Handled | Legacy Match | In Universal | Handler Location |
|--------|-------------|-------------|---------|--------------|--------------|------------------|
| SMSG_ACCOUNT_DATA_TIMES | 9994 | 0x270A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4815 |
| SMSG_ACCOUNT_HEIRLOOM_UPDATE | 9649 | 0x25B1 | NO | NO | YES |  |
| SMSG_ACCOUNT_MOUNT_UPDATE | 9646 | 0x25AE | NO | NO | YES |  |
| SMSG_ACCOUNT_TOY_UPDATE | 9648 | 0x25B0 | NO | NO | YES |  |
| SMSG_ACHIEVEMENT_DELETED | 9960 | 0x26E8 | NO | YES | YES |  |
| SMSG_ACHIEVEMENT_EARNED | 9795 | 0x2643 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5178 |
| SMSG_ACTIVATE_TAXI_REPLY | 9853 | 0x267D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8926 |
| SMSG_ACTIVE_GLYPHS | 11345 | 0x2C51 | NO | NO | YES |  |
| SMSG_ADDON_INFO | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_AI_REACTION | 9909 | 0x26B5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2185 |
| SMSG_ALL_ACCOUNT_CRITERIA | 9585 | 0x2571 | NO | NO | YES |  |
| SMSG_ALL_ACHIEVEMENT_DATA | 9584 | 0x2570 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5102 |
| SMSG_AREA_SPIRIT_HEALER_TIME | 10048 | 0x2740 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:815 |
| SMSG_AREA_TRIGGER_MESSAGE | 10368 | 0x2880 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4915 |
| SMSG_ARENA_TEAM_COMMAND_RESULT | 10083 | 0x2763 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:173 |
| SMSG_ARENA_TEAM_EVENT | 10082 | 0x2762 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:143 |
| SMSG_ARENA_TEAM_INVITE | 10081 | 0x2761 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:185 |
| SMSG_ARENA_TEAM_ROSTER | 10080 | 0x2760 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:96 |
| SMSG_ARENA_TEAM_STATS | 10084 | 0x2764 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:79 |
| SMSG_ATTACKER_STATE_UPDATE | 10578 | 0x2952 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2067 |
| SMSG_ATTACK_START | 10557 | 0x293D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2015 |
| SMSG_ATTACK_STOP | 10558 | 0x293E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2024 |
| SMSG_ATTACK_SWING_ERROR | 10572 | 0x294C | NO | NO | YES |  |
| SMSG_AUCTION_CLOSED_NOTIFICATION | 9971 | 0x26F3 | NO | NO | YES |  |
| SMSG_AUCTION_COMMAND_RESULT | 9968 | 0x26F0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:302 |
| SMSG_AUCTION_HELLO_RESPONSE | 9966 | 0x26EE | NO | NO | YES |  |
| SMSG_AUCTION_LIST_BIDDER_ITEMS_RESULT | 10365 | 0x287D | NO | NO | **NO** |  |
| SMSG_AUCTION_LIST_ITEMS_RESULT | 10339 | 0x2863 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:283 |
| SMSG_AUCTION_OUTBID_NOTIFICATION | 9970 | 0x26F2 | NO | NO | YES |  |
| SMSG_AUCTION_OWNER_BID_NOTIFICATION | 9972 | 0x26F4 | NO | NO | YES |  |
| SMSG_AUCTION_OWNER_LIST_RESULT | 10364 | 0x287C | NO | NO | **NO** |  |
| SMSG_AUCTION_REMOVED_NOTIFICATION | 9973 | 0x26F5 | NO | YES | YES |  |
| SMSG_AUCTION_WON_NOTIFICATION | 9969 | 0x26F1 | NO | NO | YES |  |
| SMSG_AURA_UPDATE | 11295 | 0x2C1F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8687 |
| SMSG_AURA_UPDATE_ALL | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8702 |
| SMSG_AUTH_CHALLENGE | 12360 | 0x3048 | NO | YES | YES |  |
| SMSG_AUTH_RESPONSE | 9581 | 0x256D | NO | YES | YES |  |
| SMSG_AVAILABLE_HOTFIXES | 10511 | 0x290F | NO | NO | YES |  |
| SMSG_BATTLEFIELD_LIST | 10535 | 0x2927 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:385; HermesProxy/World/Client/WorldClient.cs:402 |
| SMSG_BATTLEFIELD_STATUS | 10533 | 0x2925 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:452; HermesProxy/World/Client/WorldClient.cs:538 |
| SMSG_BATTLEFIELD_STATUS_ACTIVE | 10531 | 0x2923 | NO | NO | YES |  |
| SMSG_BATTLEFIELD_STATUS_FAILED | 10534 | 0x2926 | NO | NO | YES |  |
| SMSG_BATTLEFIELD_STATUS_NEED_CONFIRMATION | 10530 | 0x2922 | NO | NO | YES |  |
| SMSG_BATTLEFIELD_STATUS_QUEUED | 10532 | 0x2924 | NO | YES | YES |  |
| SMSG_BATTLEGROUND_INIT | 10575 | 0x294F | NO | NO | YES |  |
| SMSG_BATTLEGROUND_PLAYER_JOINED | 10539 | 0x292B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:806 |
| SMSG_BATTLEGROUND_PLAYER_LEFT | 10540 | 0x292C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:807 |
| SMSG_BATTLEGROUND_PLAYER_POSITIONS | 10536 | 0x2928 | NO | NO | YES |  |
| SMSG_BATTLENET_NOTIFICATION | 10248 | 0x2808 | NO | NO | YES |  |
| SMSG_BATTLENET_RESPONSE | 10247 | 0x2807 | NO | NO | YES |  |
| SMSG_BATTLE_NET_CONNECTION_STATUS | 10249 | 0x2809 | NO | NO | YES |  |
| SMSG_BATTLE_PET_JOURNAL_LOCK_ACQUIRED | 9709 | 0x25ED | NO | NO | YES |  |
| SMSG_BINDER_CONFIRM | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5980 |
| SMSG_BIND_POINT_UPDATE | 9597 | 0x257D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4828 |
| SMSG_BUY_FAILED | 9927 | 0x26C7 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4161 |
| SMSG_BUY_SUCCEEDED | 9926 | 0x26C6 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4071 |
| SMSG_CACHE_VERSION | 10524 | 0x291C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5385 |
| SMSG_CANCEL_AUTO_REPEAT | 9950 | 0x26DE | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8193 |
| SMSG_CANCEL_COMBAT | 10571 | 0x294B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2178 |
| SMSG_CAN_DUEL_RESULT | 10567 | 0x2947 | NO | NO | YES |  |
| SMSG_CAST_FAILED | 11348 | 0x2C54 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7724 |
| SMSG_CHANGE_REALM_TICKET_RESPONSE | 10250 | 0x280A | NO | NO | YES |  |
| SMSG_CHANNEL_LIST | 11204 | 0x2BC4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1598 |
| SMSG_CHANNEL_MEMBER_COUNT | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_CHANNEL_NOTIFY | 11201 | 0x2BC1 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1494 |
| SMSG_CHANNEL_NOTIFY_JOINED | 11202 | 0x2BC2 | NO | NO | YES |  |
| SMSG_CHANNEL_NOTIFY_LEFT | 11203 | 0x2BC3 | NO | NO | YES |  |
| SMSG_CHARACTER_LOGIN_FAILED | 9989 | 0x2705 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1142 |
| SMSG_CHARACTER_RENAME_RESULT | 10087 | 0x2767 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1480 |
| SMSG_CHAT | 11181 | 0x2BAD | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1626; HermesProxy/World/Client/WorldClient.cs:1695 |
| SMSG_CHAT_PLAYER_NOTFOUND | 11191 | 0x2BB7 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1951 |
| SMSG_CHAT_PLAYER_NOT_FOUND | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_CHAT_SERVER_MESSAGE | 11205 | 0x2BC5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1969 |
| SMSG_CLEAR_COOLDOWN | 9914 | 0x26BA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8257 |
| SMSG_COIN_REMOVED | 9751 | 0x2617 | NO | NO | YES |  |
| SMSG_COMPRESSED_UPDATE_OBJECT | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:9083 |
| SMSG_CONNECT_TO | 12365 | 0x304D | NO | YES | YES |  |
| SMSG_CONQUEST_FORMULA_CONSTANTS | 10121 | 0x2789 | NO | NO | YES |  |
| SMSG_CONTACT_LIST | 10124 | 0x278C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7556 |
| SMSG_CONTROL_UPDATE | 9799 | 0x2647 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5465 |
| SMSG_COOLDOWN_CHEAT | 10041 | 0x2739 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8267 |
| SMSG_COOLDOWN_EVENT | 9913 | 0x26B9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8247 |
| SMSG_CORPSE_LOCATION | 9807 | 0x264F | NO | NO | YES |  |
| SMSG_CORPSE_RECLAIM_DELAY | 10058 | 0x274A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4856 |
| SMSG_CREATE_CHAR | 9985 | 0x2701 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:957 |
| SMSG_CRITERIA_DELETED | 9959 | 0x26E7 | NO | YES | YES |  |
| SMSG_CRITERIA_UPDATE | 9953 | 0x26E1 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5159 |
| SMSG_DB_REPLY | 10510 | 0x290E | NO | NO | YES |  |
| SMSG_DEATH_RELEASE_LOC | 9939 | 0x26D3 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4847 |
| SMSG_DEFENSE_MESSAGE | 11190 | 0x2BB6 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1959 |
| SMSG_DELETE_CHAR | 9986 | 0x2702 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:967 |
| SMSG_DESTROY_OBJECT | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:9068 |
| SMSG_DISPLAY_TOAST | 9764 | 0x2624 | NO | NO | YES |  |
| SMSG_DUEL_COMPLETE | 10565 | 0x2945 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2221 |
| SMSG_DUEL_COUNTDOWN | 10564 | 0x2944 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2213 |
| SMSG_DUEL_IN_BOUNDS | 10563 | 0x2943 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2241 |
| SMSG_DUEL_OUT_OF_BOUNDS | 10562 | 0x2942 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2248 |
| SMSG_DUEL_REQUESTED | 10560 | 0x2940 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2203 |
| SMSG_DUEL_WINNER | 10566 | 0x2946 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2229 |
| SMSG_DURABILITY_DAMAGE_DEATH | 10053 | 0x2745 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4232 |
| SMSG_EMOTE | 10185 | 0x27C9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1919 |
| SMSG_ENCHANTMENT_LOG | 10003 | 0x2713 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4272 |
| SMSG_ENTER_ENCRYPTED_MODE | 12361 | 0x3049 | NO | NO | YES |  |
| SMSG_ENTITY_LOOK_ROTATION | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_ENUM_CHARACTERS_RESULT | 9603 | 0x2583 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:845 |
| SMSG_ENVIRONMENTAL_DAMAGE_LOG | 11294 | 0x2C1E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8534 |
| SMSG_EXPECTED_SPAM_RECORDS | 11185 | 0x2BB1 | NO | YES | YES |  |
| SMSG_EXPLORATION_EXPERIENCE | 10079 | 0x275F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4967 |
| SMSG_FEATURE_SYSTEM_STATUS | 9663 | 0x25BF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8859 |
| SMSG_FEATURE_SYSTEM_STATUS_GLUE_SCREEN | 9664 | 0x25C0 | NO | NO | YES |  |
| SMSG_FISH_ESCAPED | 9936 | 0x26D0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2289 |
| SMSG_FISH_NOT_HOOKED | 9935 | 0x26CF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2282 |
| SMSG_FORCE_FLIGHT_SPEED_CHANGE | 11764 | 0x2DF4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5616 |
| SMSG_FORCE_RUN_BACK_SPEED_CHANGE | 11761 | 0x2DF1 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5612 |
| SMSG_FORCE_RUN_SPEED_CHANGE | 11760 | 0x2DF0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5611 |
| SMSG_FORCE_SWIM_SPEED_CHANGE | 11762 | 0x2DF2 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5613 |
| SMSG_FORCE_TURN_RATE_CHANGE | 11767 | 0x2DF7 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5615 |
| SMSG_FORCE_WALK_SPEED_CHANGE | 11766 | 0x2DF6 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5610 |
| SMSG_FRIEND_STATUS | 10125 | 0x278D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7586 |
| SMSG_GAMESPEED_SET | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_GAME_OBJECT_CUSTOM_ANIM | 9668 | 0x25C4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2273 |
| SMSG_GAME_OBJECT_DESPAWN | 9669 | 0x25C5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2255 |
| SMSG_GAME_OBJECT_RESET_STATE | 10014 | 0x271E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2265 |
| SMSG_GENERATE_RANDOM_CHARACTER_NAME_RESULT | 9605 | 0x2585 | NO | NO | YES |  |
| SMSG_GET_ACCOUNT_CHARACTER_LIST_RESULT | 10085 | 0x2765 | NO | NO | YES |  |
| SMSG_GM_MESSAGECHAT | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1696 |
| SMSG_GM_TICKET_STATUS_UPDATE | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_GOSSIP_COMPLETE | 10903 | 0x2A97 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5961 |
| SMSG_GOSSIP_MESSAGE | 10904 | 0x2A98 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5919 |
| SMSG_GOSSIP_POI | 10136 | 0x2798 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5968 |
| SMSG_GROUP_DECLINE | 10129 | 0x2791 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2320 |
| SMSG_GROUP_DESTROYED | 10132 | 0x2794 | NO | YES | YES |  |
| SMSG_GROUP_NEW_LEADER | 9773 | 0x262D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2601 |
| SMSG_GROUP_UNINVITE | 10131 | 0x2793 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2594 |
| SMSG_GUILD_BANK_LOG_QUERY_RESULTS | 10720 | 0x29E0 | NO | NO | YES |  |
| SMSG_GUILD_BANK_QUERY_RESULTS | 10719 | 0x29DF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3821 |
| SMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY | 10721 | 0x29E1 | NO | NO | YES |  |
| SMSG_GUILD_BANK_TEXT_QUERY_RESULT | 10724 | 0x29E4 | NO | NO | YES |  |
| SMSG_GUILD_COMMAND_RESULT | 10682 | 0x29BA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3477 |
| SMSG_GUILD_EVENT_BANK_MONEY_CHANGED | 10743 | 0x29F7 | NO | NO | YES |  |
| SMSG_GUILD_EVENT_DISBANDED | 10734 | 0x29EE | NO | NO | YES |  |
| SMSG_GUILD_EVENT_LOG_QUERY_RESULTS | 10723 | 0x29E3 | NO | NO | YES |  |
| SMSG_GUILD_EVENT_MOTD | 10735 | 0x29EF | NO | NO | YES |  |
| SMSG_GUILD_EVENT_NEW_LEADER | 10733 | 0x29ED | NO | NO | YES |  |
| SMSG_GUILD_EVENT_PLAYER_JOINED | 10731 | 0x29EB | NO | NO | YES |  |
| SMSG_GUILD_EVENT_PLAYER_LEFT | 10732 | 0x29EC | NO | NO | YES |  |
| SMSG_GUILD_EVENT_PRESENCE_CHANGE | 10736 | 0x29F0 | NO | NO | YES |  |
| SMSG_GUILD_EVENT_RANKS_UPDATED | 10737 | 0x29F1 | NO | NO | YES |  |
| SMSG_GUILD_EVENT_TAB_ADDED | 10739 | 0x29F3 | NO | NO | YES |  |
| SMSG_GUILD_EVENT_TAB_MODIFIED | 10741 | 0x29F5 | NO | NO | YES |  |
| SMSG_GUILD_EVENT_TAB_TEXT_CHANGED | 10742 | 0x29F6 | NO | NO | YES |  |
| SMSG_GUILD_INVITE | 10699 | 0x29CB | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3784 |
| SMSG_GUILD_INVITE_DECLINED | 10729 | 0x29E9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3812 |
| SMSG_GUILD_RANKS | 10697 | 0x29C9 | NO | NO | YES |  |
| SMSG_GUILD_ROSTER | 10683 | 0x29BB | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3705 |
| SMSG_GUILD_SEND_RANK_CHANGE | 10681 | 0x29B9 | NO | NO | YES |  |
| SMSG_HEALTH_UPDATE | 9937 | 0x26D1 | NO | YES | YES |  |
| SMSG_HIGHEST_THREAT_UPDATE | 9945 | 0x26D9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2043 |
| SMSG_HOTFIX_CONNECT | 10513 | 0x2911 | NO | NO | YES |  |
| SMSG_HOTFIX_MESSAGE | 10512 | 0x2910 | NO | NO | YES |  |
| SMSG_INITIALIZE_FACTIONS | 10020 | 0x2724 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7436 |
| SMSG_INITIAL_SETUP | 9600 | 0x2580 | NO | NO | YES |  |
| SMSG_INIT_WORLD_STATES | 10054 | 0x2746 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:12186 |
| SMSG_INSPECT_HONOR_STATS | 10547 | 0x2933 | NO | NO | YES |  |
| SMSG_INSPECT_PVP | 10018 | 0x2722 | NO | NO | YES |  |
| SMSG_INSPECT_RESULT | 9777 | 0x2631 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1283 |
| SMSG_INSTANCE_LOCK_WARNING_QUERY | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_INSTANCE_RESET | 9862 | 0x2686 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3951 |
| SMSG_INSTANCE_RESET_FAILED | 9863 | 0x2687 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3959 |
| SMSG_INSTANCE_SAVE_CREATED | 10112 | 0x2780 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4018 |
| SMSG_INVALIDATE_PLAYER | 12287 | 0x2FFF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5049 |
| SMSG_INVENTORY_CHANGE_FAILURE | 11685 | 0x2DA5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4171; HermesProxy/World/Client/WorldClient.cs:4196 |
| SMSG_ITEM_COOLDOWN | 10184 | 0x27C8 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4240 |
| SMSG_ITEM_ENCHANT_TIME_UPDATE | 10069 | 0x2755 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4261 |
| SMSG_ITEM_NAME_QUERY_RESPONSE | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6934 |
| SMSG_ITEM_PUSH_RESULT | 9763 | 0x2623 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4082 |
| SMSG_ITEM_QUERY_SINGLE_RESPONSE | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6827 |
| SMSG_LEARNED_SPELLS | 11338 | 0x2C4A | NO | NO | YES |  |
| SMSG_LEVEL_UP_INFO | 9961 | 0x26E9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1250 |
| SMSG_LFG_DISABLED | 10803 | 0x2A33 | NO | YES | YES |  |
| SMSG_LFG_JOIN_RESULT | 10780 | 0x2A1C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5267 |
| SMSG_LFG_LIST_UPDATE_BLACKLIST | 10794 | 0x2A2A | NO | NO | YES |  |
| SMSG_LFG_OFFER_CONTINUE | 10804 | 0x2A34 | NO | YES | YES |  |
| SMSG_LFG_PARTY_INFO | 10806 | 0x2A36 | NO | YES | YES |  |
| SMSG_LFG_PLAYER_INFO | 10807 | 0x2A37 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5210 |
| SMSG_LFG_PLAYER_REWARD | 10808 | 0x2A38 | NO | YES | YES |  |
| SMSG_LFG_PROPOSAL_UPDATE | 10797 | 0x2A2D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5348 |
| SMSG_LFG_QUEUE_STATUS | 10784 | 0x2A20 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5327 |
| SMSG_LFG_ROLE_CHECK_UPDATE | 10785 | 0x2A21 | NO | YES | YES |  |
| SMSG_LFG_TELEPORT | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_LFG_UPDATE_PLAYER | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5298 |
| SMSG_LFG_UPDATE_SEARCH | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_LFG_UPDATE_STATUS | 10788 | 0x2A24 | NO | NO | YES |  |
| SMSG_LOAD_CUF_PROFILES | 9660 | 0x25BC | NO | NO | YES |  |
| SMSG_LOAD_EQUIPMENT_SET | 9998 | 0x270E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5200 |
| SMSG_LOGIN_SET_TIME_SPEED | 9997 | 0x270D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4897 |
| SMSG_LOGIN_VERIFY_WORLD | 9623 | 0x2597 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1077 |
| SMSG_LOGOUT_CANCEL_ACK | 9861 | 0x2685 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1207 |
| SMSG_LOGOUT_COMPLETE | 9860 | 0x2684 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1197 |
| SMSG_LOGOUT_RESPONSE | 9859 | 0x2683 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1188 |
| SMSG_LOG_XP_GAIN | 9957 | 0x26E5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1214 |
| SMSG_LOOT_ALL_PASSED | 9761 | 0x2621 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4502 |
| SMSG_LOOT_CLEAR_MONEY | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4387 |
| SMSG_LOOT_CONTENTS | 0 | 0x0000 | NO | NO | YES |  |
| SMSG_LOOT_LIST | 10049 | 0x2741 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4303 |
| SMSG_LOOT_MASTER_LIST | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4519 |
| SMSG_LOOT_MONEY_NOTIFY | 9756 | 0x261C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4375 |
| SMSG_LOOT_RELEASE | 9755 | 0x261B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4354 |
| SMSG_LOOT_REMOVED | 9749 | 0x2615 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4365 |
| SMSG_LOOT_RESPONSE | 9748 | 0x2614 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4322 |
| SMSG_LOOT_ROLL | 9758 | 0x261E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4441 |
| SMSG_LOOT_ROLLS_COMPLETE | 9760 | 0x2620 | NO | NO | YES |  |
| SMSG_LOOT_ROLL_WON | 9762 | 0x2622 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4478 |
| SMSG_LOOT_START_ROLL | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4395 |
| SMSG_MAIL_COMMAND_RESULT | 9787 | 0x263B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4774 |
| SMSG_MAIL_LIST_RESULT | 10070 | 0x2756 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4584 |
| SMSG_MAIL_QUERY_NEXT_TIME_RESULT | 10071 | 0x2757 | NO | NO | YES |  |
| SMSG_MINIMAP_PING | 9934 | 0x26CE | NO | NO | YES |  |
| SMSG_MONEY_BALANCE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_MONSTER_MOVE_TRANSPORT | 11732 | 0x2DD4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5741 |
| SMSG_MOTD | 11183 | 0x2BAF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8865 |
| SMSG_MOUNT_RESULT | 9595 | 0x257B | NO | YES | YES |  |
| SMSG_MOVE_DISABLE_GRAVITY | 11789 | 0x2E0D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5712 |
| SMSG_MOVE_DISABLE_TRANSITION_BETWEEN_SWIM_AND_FLY | 11788 | 0x2E0C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5711 |
| SMSG_MOVE_ENABLE_GRAVITY | 11790 | 0x2E0E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5713 |
| SMSG_MOVE_ENABLE_TRANSITION_BETWEEN_SWIM_AND_FLY | 11787 | 0x2E0B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5710 |
| SMSG_MOVE_KNOCK_BACK | 11779 | 0x2E03 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5453 |
| SMSG_MOVE_ROOT | 11769 | 0x2DF9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5702 |
| SMSG_MOVE_SET_CAN_FLY | 11781 | 0x2E05 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5708 |
| SMSG_MOVE_SET_COLLISION_HEIGHT | 11795 | 0x2E13 | NO | NO | YES |  |
| SMSG_MOVE_SET_FEATHER_FALL | 11775 | 0x2DFF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5714 |
| SMSG_MOVE_SET_FLIGHT_BACK_SPEED | 11765 | 0x2DF5 | NO | NO | YES |  |
| SMSG_MOVE_SET_FLIGHT_SPEED | 11764 | 0x2DF4 | NO | NO | YES |  |
| SMSG_MOVE_SET_HOVERING | 11777 | 0x2E01 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5706 |
| SMSG_MOVE_SET_LAND_WALK | 11774 | 0x2DFE | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5705 |
| SMSG_MOVE_SET_NORMAL_FALL | 11776 | 0x2E00 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5715 |
| SMSG_MOVE_SET_PITCH_RATE | 11768 | 0x2DF8 | NO | NO | YES |  |
| SMSG_MOVE_SET_RUN_BACK_SPEED | 11761 | 0x2DF1 | NO | NO | YES |  |
| SMSG_MOVE_SET_RUN_SPEED | 11760 | 0x2DF0 | NO | NO | YES |  |
| SMSG_MOVE_SET_SWIM_BACK_SPEED | 11763 | 0x2DF3 | NO | NO | YES |  |
| SMSG_MOVE_SET_SWIM_SPEED | 11762 | 0x2DF2 | NO | NO | YES |  |
| SMSG_MOVE_SET_TURN_RATE | 11767 | 0x2DF7 | NO | NO | YES |  |
| SMSG_MOVE_SET_WALK_SPEED | 11766 | 0x2DF6 | NO | NO | YES |  |
| SMSG_MOVE_SET_WATER_WALK | 11771 | 0x2DFB | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5704 |
| SMSG_MOVE_SPLINE_DISABLE_GRAVITY | 11803 | 0x2E1B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5682 |
| SMSG_MOVE_SPLINE_ENABLE_GRAVITY | 11804 | 0x2E1C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5681 |
| SMSG_MOVE_SPLINE_ROOT | 11801 | 0x2E19 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5679 |
| SMSG_MOVE_SPLINE_SET_FEATHER_FALL | 11807 | 0x2E1F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5683 |
| SMSG_MOVE_SPLINE_SET_FLIGHT_BACK_SPEED | 11756 | 0x2DEC | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5592 |
| SMSG_MOVE_SPLINE_SET_FLIGHT_SPEED | 11755 | 0x2DEB | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5593 |
| SMSG_MOVE_SPLINE_SET_FLYING | 11817 | 0x2E29 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5693 |
| SMSG_MOVE_SPLINE_SET_HOVER | 11809 | 0x2E21 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5685 |
| SMSG_MOVE_SPLINE_SET_LAND_WALK | 11812 | 0x2E24 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5688 |
| SMSG_MOVE_SPLINE_SET_NORMAL_FALL | 11808 | 0x2E20 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5684 |
| SMSG_MOVE_SPLINE_SET_PITCH_RATE | 11759 | 0x2DEF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5594 |
| SMSG_MOVE_SPLINE_SET_RUN_BACK_SPEED | 11752 | 0x2DE8 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5595 |
| SMSG_MOVE_SPLINE_SET_RUN_MODE | 11815 | 0x2E27 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5691 |
| SMSG_MOVE_SPLINE_SET_RUN_SPEED | 11751 | 0x2DE7 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5596 |
| SMSG_MOVE_SPLINE_SET_SWIM_BACK_SPEED | 11754 | 0x2DEA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5597 |
| SMSG_MOVE_SPLINE_SET_SWIM_SPEED | 11753 | 0x2DE9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5598 |
| SMSG_MOVE_SPLINE_SET_TURN_RATE | 11758 | 0x2DEE | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5599 |
| SMSG_MOVE_SPLINE_SET_WALK_MODE | 11816 | 0x2E28 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5692 |
| SMSG_MOVE_SPLINE_SET_WALK_SPEED | 11757 | 0x2DED | YES | NO | YES | HermesProxy/World/Client/WorldClient.cs:5601 |
| SMSG_MOVE_SPLINE_SET_WATER_WALK | 11811 | 0x2E23 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5687 |
| SMSG_MOVE_SPLINE_START_SWIM | 11813 | 0x2E25 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5689 |
| SMSG_MOVE_SPLINE_STOP_SWIM | 11814 | 0x2E26 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5690 |
| SMSG_MOVE_SPLINE_UNROOT | 11802 | 0x2E1A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5680 |
| SMSG_MOVE_SPLINE_UNSET_FLYING | 11818 | 0x2E2A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5694 |
| SMSG_MOVE_SPLINE_UNSET_HOVER | 11810 | 0x2E22 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5686 |
| SMSG_MOVE_TELEPORT | 11780 | 0x2E04 | NO | NO | YES |  |
| SMSG_MOVE_UNROOT | 11770 | 0x2DFA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5703 |
| SMSG_MOVE_UNSET_CAN_FLY | 11782 | 0x2E06 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5709 |
| SMSG_MOVE_UNSET_HOVERING | 11778 | 0x2E02 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5707 |
| SMSG_MOVE_UPDATE | 11744 | 0x2DE0 | NO | NO | YES |  |
| SMSG_MOVE_UPDATE_KNOCK_BACK | 11746 | 0x2DE2 | NO | NO | YES |  |
| SMSG_NEW_TAXI_PATH | 9854 | 0x267E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8919 |
| SMSG_NEW_WORLD | 9620 | 0x2594 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5553 |
| SMSG_NOTIFY_RECEIVED_MAIL | 9788 | 0x263C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4541 |
| SMSG_ON_CANCEL_EXPECTED_RIDE_VEHICLE_AURA | 9958 | 0x26E6 | NO | YES | YES |  |
| SMSG_ON_MONSTER_MOVE | 11732 | 0x2DD4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5740 |
| SMSG_OVERRIDE_LIGHT | 9915 | 0x26BB | NO | YES | YES |  |
| SMSG_PARTY_COMMAND_RESULT | 10134 | 0x2796 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2296 |
| SMSG_PARTY_INVITE | 9661 | 0x25BD | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2328 |
| SMSG_PARTY_KILL_LOG | 10074 | 0x275A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2194 |
| SMSG_PARTY_MEMBER_FULL_STATE | 10073 | 0x2759 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3078; HermesProxy/World/Client/WorldClient.cs:3292 |
| SMSG_PARTY_MEMBER_PARTIAL_STATE | 10072 | 0x2758 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2718; HermesProxy/World/Client/WorldClient.cs:2923 |
| SMSG_PARTY_MEMBER_STATS_FULL | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_PARTY_UPDATE | 9716 | 0x25F4 | NO | NO | YES |  |
| SMSG_PAUSE_MIRROR_TIMER | 10000 | 0x2710 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5032 |
| SMSG_PETITION_RENAME_GUILD_RESPONSE | 10746 | 0x29FA | NO | NO | YES |  |
| SMSG_PETITION_SHOW_LIST | 9919 | 0x26BF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6244 |
| SMSG_PETITION_SHOW_SIGNATURES | 9920 | 0x26C0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6273 |
| SMSG_PETITION_SIGN_RESULTS | 10060 | 0x274C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6356 |
| SMSG_PET_ACTION_FEEDBACK | 10057 | 0x2749 | NO | YES | YES |  |
| SMSG_PET_ACTION_SOUND | 9888 | 0x26A0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6157 |
| SMSG_PET_BROKEN | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6166 |
| SMSG_PET_CAST_FAILED | 11349 | 0x2C55 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7801; HermesProxy/World/Client/WorldClient.cs:7834 |
| SMSG_PET_CLEAR_SPELLS | 11297 | 0x2C21 | NO | NO | YES |  |
| SMSG_PET_GUIDS | 9988 | 0x2704 | NO | YES | YES |  |
| SMSG_PET_LEARNED_SPELL | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_PET_MODE | 9608 | 0x2588 | NO | YES | YES |  |
| SMSG_PET_REMOVED_SPELL | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_PET_SPELLS_MESSAGE | 11298 | 0x2C22 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6105 |
| SMSG_PET_STABLE_RESULT | 9619 | 0x2593 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6236 |
| SMSG_PET_TAME_FAILURE | 9907 | 0x26B3 | NO | YES | YES |  |
| SMSG_PET_UNLEARN_CONFIRM | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6174 |
| SMSG_PHASE_SHIFT_CHANGE | 9592 | 0x2578 | NO | YES | YES |  |
| SMSG_PLAYED_TIME | 9941 | 0x26D5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1233 |
| SMSG_PLAYER_BOUND | 12280 | 0x2FF8 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4838 |
| SMSG_PLAYER_SAVE_GUILD_EMBLEM | 10745 | 0x29F9 | NO | NO | YES |  |
| SMSG_PLAYER_SKINNED | 12294 | 0x3006 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:834 |
| SMSG_PLAY_MUSIC | 10093 | 0x276D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4976 |
| SMSG_PLAY_OBJECT_SOUND | 10094 | 0x276E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4993 |
| SMSG_PLAY_SOUND | 10092 | 0x276C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4984 |
| SMSG_PLAY_SPELL_VISUAL | 11330 | 0x2C42 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8599 |
| SMSG_PLAY_SPELL_VISUAL_KIT | 11334 | 0x2C46 | NO | NO | YES |  |
| SMSG_PLAY_TIME_WARNING | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_PONG | 12366 | 0x304E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4797 |
| SMSG_POWER_UPDATE | 9938 | 0x26D2 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5080 |
| SMSG_PRE_RESURRECT | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_PRINT_NOTIFICATION | 9674 | 0x25CA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1943 |
| SMSG_PROCRESIST | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_PROPOSED_INVITE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_PVP_CREDIT | 10570 | 0x294A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:824 |
| SMSG_QUERY_ARENA_TEAM_RESPONSE | 10528 | 0x2920 | NO | NO | YES |  |
| SMSG_QUERY_CREATURE_RESPONSE | 10516 | 0x2914 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6631 |
| SMSG_QUERY_GAME_OBJECT_RESPONSE | 10517 | 0x2915 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6718 |
| SMSG_QUERY_GUILD_INFO_RESPONSE | 10726 | 0x29E6 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3640 |
| SMSG_QUERY_ITEM_TEXT_RESPONSE | 10526 | 0x291E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4697 |
| SMSG_QUERY_NPC_TEXT_RESPONSE | 10518 | 0x2916 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6789 |
| SMSG_QUERY_PAGE_TEXT_RESPONSE | 10519 | 0x2917 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6773 |
| SMSG_QUERY_PETITION_RESPONSE | 10523 | 0x291B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6294 |
| SMSG_QUERY_PET_NAME_RESPONSE | 10521 | 0x2919 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6903 |
| SMSG_QUERY_PLAYER_NAMES_RESPONSE | 12315 | 0x301B | NO | NO | YES |  |
| SMSG_QUERY_QUEST_INFO_RESPONSE | 10902 | 0x2A96 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6386 |
| SMSG_QUERY_TIME_RESPONSE | 9956 | 0x26E4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6374 |
| SMSG_QUESTLOG_FULL | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_QUEST_CONFIRM_ACCEPT | 10895 | 0x2A8F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7417 |
| SMSG_QUEST_FORCE_REMOVE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_QUEST_GIVER_INVALID_QUEST | 10885 | 0x2A85 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7361 |
| SMSG_QUEST_GIVER_OFFER_REWARD_MESSAGE | 10900 | 0x2A94 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7244 |
| SMSG_QUEST_GIVER_QUEST_COMPLETE | 10883 | 0x2A83 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7284 |
| SMSG_QUEST_GIVER_QUEST_DETAILS | 10898 | 0x2A92 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6992 |
| SMSG_QUEST_GIVER_QUEST_FAILED | 10886 | 0x2A86 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7352 |
| SMSG_QUEST_GIVER_QUEST_LIST_MESSAGE | 10906 | 0x2A9A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7150 |
| SMSG_QUEST_GIVER_REQUEST_ITEMS | 10899 | 0x2A93 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7189 |
| SMSG_QUEST_GIVER_STATUS | 10907 | 0x2A9B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7126 |
| SMSG_QUEST_GIVER_STATUS_MULTIPLE | 10897 | 0x2A91 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7135 |
| SMSG_QUEST_POI_QUERY_RESPONSE | 10909 | 0x2A9D | NO | YES | YES |  |
| SMSG_QUEST_PUSH_RESULT | 10896 | 0x2A90 | NO | NO | YES |  |
| SMSG_QUEST_UPDATE_ADD_CREDIT | 10892 | 0x2A8C | NO | NO | YES |  |
| SMSG_QUEST_UPDATE_ADD_CREDIT_SIMPLE | 10893 | 0x2A8D | NO | NO | YES |  |
| SMSG_QUEST_UPDATE_ADD_ITEM | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7379 |
| SMSG_QUEST_UPDATE_COMPLETE | 10889 | 0x2A89 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7369 |
| SMSG_QUEST_UPDATE_FAILED | 10890 | 0x2A8A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7370 |
| SMSG_QUEST_UPDATE_FAILED_TIMER | 10891 | 0x2A8B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7371 |
| SMSG_RAID_GROUP_ONLY | 10159 | 0x27AF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4026 |
| SMSG_RAID_INSTANCE_MESSAGE | 11188 | 0x2BB4 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4035 |
| SMSG_RAID_READY_CHECK | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_RAID_READY_CHECK_CONFIRM | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_RAID_READY_CHECK_ERROR | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_RAID_READY_CHECK_RESULT | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_RAID_ROLL_VOTE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_RANDOM_ROLL | 9776 | 0x2630 | NO | NO | YES |  |
| SMSG_READY_CHECK_COMPLETED | 9720 | 0x25F8 | NO | NO | YES |  |
| SMSG_READY_CHECK_RESPONSE | 9719 | 0x25F7 | NO | NO | YES |  |
| SMSG_READY_CHECK_STARTED | 9718 | 0x25F6 | NO | NO | YES |  |
| SMSG_READ_ITEM_RESULT_FAILED | 10153 | 0x27A9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4152 |
| SMSG_READ_ITEM_RESULT_OK | 10145 | 0x27A1 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4144 |
| SMSG_REAL_GROUP_UPDATE | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_RESET_FAILED_NOTIFY | 9911 | 0x26B7 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3968 |
| SMSG_RESPEC_WIPE_CONFIRM | 9746 | 0x2612 | NO | NO | YES |  |
| SMSG_RESUME_COMMS | 12363 | 0x304B | NO | YES | YES |  |
| SMSG_RESUME_TOKEN | 9641 | 0x25A9 | NO | NO | YES |  |
| SMSG_RESURRECT_REQUEST | 9598 | 0x257E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8795 |
| SMSG_SEASON_INFO | 9665 | 0x25C1 | NO | NO | YES |  |
| SMSG_SELL_RESPONSE | 9925 | 0x26C5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4250 |
| SMSG_SEND_KNOWN_SPELLS | 11303 | 0x2C27 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7630 |
| SMSG_SEND_SPELL_CHARGES | 11306 | 0x2C2A | NO | NO | YES |  |
| SMSG_SEND_SPELL_HISTORY | 11304 | 0x2C28 | NO | NO | YES |  |
| SMSG_SEND_UNLEARN_SPELLS | 11307 | 0x2C2B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7702 |
| SMSG_SERVER_MESSAGE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_SERVER_PERF | 0 | 0x0000 | NO | NO | YES |  |
| SMSG_SERVER_TIME_OFFSET | 10004 | 0x2714 | NO | NO | YES |  |
| SMSG_SETUP_CURRENCY | 9587 | 0x2573 | NO | NO | YES |  |
| SMSG_SET_DUNGEON_DIFFICULTY | 9892 | 0x26A4 | NO | NO | YES |  |
| SMSG_SET_FACTION_STANDING | 10028 | 0x272C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7456 |
| SMSG_SET_FACTION_VISIBLE | 10026 | 0x272A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7500 |
| SMSG_SET_FLAT_SPELL_MODIFIER | 11315 | 0x2C33 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8819 |
| SMSG_SET_FORCED_REACTIONS | 10013 | 0x271D | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7483 |
| SMSG_SET_PCT_SPELL_MODIFIER | 11316 | 0x2C34 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8820 |
| SMSG_SET_PLAYER_DECLINED_NAMES_RESULT | 12291 | 0x3003 | NO | YES | YES |  |
| SMSG_SET_PROFICIENCY | 10037 | 0x2735 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4062 |
| SMSG_SET_TIME_ZONE_INFORMATION | 9847 | 0x2677 | NO | NO | YES |  |
| SMSG_SHOW_RATINGS | 0 | 0x0000 | NO | NO | YES |  |
| SMSG_SHOW_TAXI_NODES | 9933 | 0x26CD | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8892 |
| SMSG_SOCIAL_CONTRACT_REQUEST_RESPONSE | 10386 | 0x2892 | NO | NO | YES |  |
| SMSG_SOCKET_GEMS_SUCCESS | 10023 | 0x2727 | NO | NO | YES |  |
| SMSG_SPECIAL_MOUNT_ANIM | 9887 | 0x269F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5011 |
| SMSG_SPELLLOGEXECUTE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_SPELL_CHANNEL_START | 11313 | 0x2C31 | NO | NO | YES |  |
| SMSG_SPELL_CHANNEL_UPDATE | 11314 | 0x2C32 | NO | NO | YES |  |
| SMSG_SPELL_COOLDOWN | 11285 | 0x2C15 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8216 |
| SMSG_SPELL_DAMAGE_SHIELD | 11310 | 0x2C2E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8505 |
| SMSG_SPELL_DELAYED | 11324 | 0x2C3C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8455 |
| SMSG_SPELL_DISPELL_LOG | 11287 | 0x2C17 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8563 |
| SMSG_SPELL_ENERGIZE_LOG | 11289 | 0x2C19 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8443 |
| SMSG_SPELL_EXTRA_ATTACKS | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_SPELL_FAILED_OTHER | 11346 | 0x2C52 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7884 |
| SMSG_SPELL_FAILURE | 11344 | 0x2C50 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7878 |
| SMSG_SPELL_GO | 11318 | 0x2C36 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7991 |
| SMSG_SPELL_HEAL_LOG | 11290 | 0x2C1A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8331 |
| SMSG_SPELL_INSTAKILL_LOG | 11312 | 0x2C30 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8546 |
| SMSG_SPELL_NON_MELEE_DAMAGE_LOG | 11311 | 0x2C2F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8275 |
| SMSG_SPELL_PERIODIC_AURA_LOG | 11288 | 0x2C18 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8357 |
| SMSG_SPELL_PREPARE | 11317 | 0x2C35 | NO | NO | YES |  |
| SMSG_SPELL_START | 11319 | 0x2C37 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7935 |
| SMSG_SPELL_UPDATE_CHAIN_TARGETS | 0 | 0x0000 | NO | YES | YES |  |
| SMSG_STAND_STATE_UPDATE | 10012 | 0x271C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4959 |
| SMSG_START_LIGHTNING_STORM | 9895 | 0x26A7 | NO | NO | YES |  |
| SMSG_START_MIRROR_TIMER | 9999 | 0x270F | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5019 |
| SMSG_STOP_MIRROR_TIMER | 10001 | 0x2711 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5041 |
| SMSG_SUMMON_RAID_MEMBER_VALIDATE_FAILED | 9613 | 0x258D | NO | NO | YES |  |
| SMSG_SUMMON_REQUEST | 10017 | 0x2721 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2707 |
| SMSG_SUPERCEDED_SPELLS | 11337 | 0x2C49 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7672 |
| SMSG_SUSPEND_COMMS | 12362 | 0x304A | NO | YES | YES |  |
| SMSG_SUSPEND_TOKEN | 9640 | 0x25A8 | NO | NO | YES |  |
| SMSG_TAXI_NODE_STATUS | 9852 | 0x267C | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8882 |
| SMSG_TAXI_PATH_ACTIVATED | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_TEXT_EMOTE | 9850 | 0x267A | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1928 |
| SMSG_THREAT_CLEAR | 9948 | 0x26DC | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2051 |
| SMSG_THREAT_REMOVE | 9947 | 0x26DB | NO | YES | YES |  |
| SMSG_THREAT_UPDATE | 9946 | 0x26DA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:2059 |
| SMSG_TIME_SYNC_REQUEST | 11730 | 0x2DD2 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4864 |
| SMSG_TITLE_EARNED | 9943 | 0x26D7 | NO | YES | YES |  |
| SMSG_TOTEM_CREATED | 9928 | 0x26C8 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8808 |
| SMSG_TRADE_STATUS | 9602 | 0x2582 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:8939 |
| SMSG_TRADE_UPDATED | 9601 | 0x2581 | NO | NO | YES |  |
| SMSG_TRAINER_BUY_FAILED | 9952 | 0x26E0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6073 |
| SMSG_TRAINER_LIST | 9951 | 0x26DF | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6032 |
| SMSG_TRANSFER_ABORTED | 9987 | 0x2703 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5524 |
| SMSG_TRANSFER_PENDING | 9677 | 0x25CD | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5504 |
| SMSG_TRIGGER_CINEMATIC | 10186 | 0x27CA | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5003 |
| SMSG_TURN_IN_PETITION_RESULT | 10062 | 0x274E | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6366 |
| SMSG_TUTORIAL_FLAGS | 10174 | 0x27BE | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4804 |
| SMSG_UNLEARNED_SPELLS | 11339 | 0x2C4B | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:7715 |
| SMSG_UPDATE_ACCOUNT_DATA | 9993 | 0x2709 | NO | YES | YES |  |
| SMSG_UPDATE_ACTION_BUTTONS | 9696 | 0x25E0 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1151 |
| SMSG_UPDATE_AURA_DURATION | 0 | 0x0000 | YES | NO | YES | HermesProxy/World/Client/WorldClient.cs:8608 |
| SMSG_UPDATE_COMBO_POINTS | 0 | 0x0000 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:1267 |
| SMSG_UPDATE_INSTANCE_OWNERSHIP | 9897 | 0x26A9 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:3943 |
| SMSG_UPDATE_LAST_INSTANCE | 9864 | 0x2688 | NO | YES | YES |  |
| SMSG_UPDATE_OBJECT | 10187 | 0x27CB | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:9090 |
| SMSG_UPDATE_TALENT_DATA | 9687 | 0x25D7 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5094 |
| SMSG_UPDATE_WORLD_STATE | 10056 | 0x2748 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:12245 |
| SMSG_VEHICLE_RIDE_ALLOWED_QUERY_RESPONSE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_VENDOR_INVENTORY | 9656 | 0x25B8 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5989 |
| SMSG_WAIT_QUEUE_FINISH | 9583 | 0x256F | NO | NO | YES |  |
| SMSG_WAIT_QUEUE_UPDATE | 9582 | 0x256E | NO | NO | YES |  |
| SMSG_WEATHER | 9894 | 0x26A6 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:4872 |
| SMSG_WHO | 11182 | 0x2BAE | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:6946 |
| SMSG_WHOIS | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_WIPE_ALL_CRITERIA_FROM_CLIENT | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_WORLD_SERVER_INFO | 9645 | 0x25AD | NO | NO | YES |  |
| SMSG_WORLD_STATE_UI_TIMER_UPDATE | 0 | 0x0000 | NO | NO | **NO** |  |
| SMSG_XP_GAIN_ABORTED | 9673 | 0x25C9 | NO | NO | YES |  |
| SMSG_ZONE_UNDER_ATTACK | 11189 | 0x2BB5 | YES | YES | YES | HermesProxy/World/Client/WorldClient.cs:5061 |

## MSG (Bidirectional)

| Opcode | Value (Dec) | Value (Hex) | Handled | Legacy Match | In Universal | Handler Location |
|--------|-------------|-------------|---------|--------------|--------------|------------------|
| MSG_NULL_ACTION | 0 | 0x0000 | NO | YES | YES |  |

## Other

| Opcode | Value (Dec) | Value (Hex) | Handled | Legacy Match | In Universal | Handler Location |
|--------|-------------|-------------|---------|--------------|--------------|------------------|

## CRITICAL: Modern Opcodes Missing from Universal Enum

These opcodes exist in the modern client but have NO entry in the universal opcode enum.
They CANNOT be translated and will cause errors.

- **CMSG_CHAT_IGNORED** = 0 (0x0000) - Handled: NO
- **CMSG_GROUP_REMOVE_LEADER** = 0 (0x0000) - Handled: NO
- **CMSG_GUILD_OFFICER_LIST** = 0 (0x0000) - Handled: NO
- **CMSG_LEARN_TALENT_GROUP** = 0 (0x0000) - Handled: NO
- **CMSG_MOVE_SET_FLIGHT_SPEED_CHEAT** = 0 (0x0000) - Handled: NO
- **CMSG_MOVE_STOP_BACKWARD** = 0 (0x0000) - Handled: NO
- **CMSG_PUSH_SPELL_TO_ACTION_BAR** = 0 (0x0000) - Handled: NO
- **CMSG_RAID_READY_CHECK** = 0 (0x0000) - Handled: NO
- **CMSG_RAID_READY_CHECK_CONFIRM** = 0 (0x0000) - Handled: NO
- **CMSG_RAID_READY_CHECK_FINISHED** = 0 (0x0000) - Handled: NO
- **CMSG_REAL_GROUP_UPDATE** = 0 (0x0000) - Handled: NO
- **CMSG_SEND_CONTACT_LIST** = 14039 (0x36D7) - Handled: NO
- **CMSG_SET_ACTIONBAR_TOGGLES** = 0 (0x0000) - Handled: NO
- **CMSG_SET_FACTION_ATWAR** = 0 (0x0000) - Handled: NO
- **CMSG_SOCKETSPELLS** = 0 (0x0000) - Handled: NO
- **CMSG_WHOIS** = 0 (0x0000) - Handled: NO
- **SMSG_AUCTION_LIST_BIDDER_ITEMS_RESULT** = 10365 (0x287D) - Handled: NO
- **SMSG_AUCTION_OWNER_LIST_RESULT** = 10364 (0x287C) - Handled: NO
- **SMSG_CHAT_PLAYER_NOT_FOUND** = 0 (0x0000) - Handled: NO
- **SMSG_ENTITY_LOOK_ROTATION** = 0 (0x0000) - Handled: NO
- **SMSG_GAMESPEED_SET** = 0 (0x0000) - Handled: NO
- **SMSG_LFG_TELEPORT** = 0 (0x0000) - Handled: NO
- **SMSG_MONEY_BALANCE** = 0 (0x0000) - Handled: NO
- **SMSG_PARTY_MEMBER_STATS_FULL** = 0 (0x0000) - Handled: NO
- **SMSG_PET_LEARNED_SPELL** = 0 (0x0000) - Handled: NO
- **SMSG_PET_REMOVED_SPELL** = 0 (0x0000) - Handled: NO
- **SMSG_PRE_RESURRECT** = 0 (0x0000) - Handled: NO
- **SMSG_PROCRESIST** = 0 (0x0000) - Handled: NO
- **SMSG_PROPOSED_INVITE** = 0 (0x0000) - Handled: NO
- **SMSG_QUESTLOG_FULL** = 0 (0x0000) - Handled: NO
- **SMSG_QUEST_FORCE_REMOVE** = 0 (0x0000) - Handled: NO
- **SMSG_RAID_READY_CHECK** = 0 (0x0000) - Handled: NO
- **SMSG_RAID_READY_CHECK_CONFIRM** = 0 (0x0000) - Handled: NO
- **SMSG_RAID_READY_CHECK_ERROR** = 0 (0x0000) - Handled: NO
- **SMSG_RAID_READY_CHECK_RESULT** = 0 (0x0000) - Handled: NO
- **SMSG_RAID_ROLL_VOTE** = 0 (0x0000) - Handled: NO
- **SMSG_SERVER_MESSAGE** = 0 (0x0000) - Handled: NO
- **SMSG_SPELLLOGEXECUTE** = 0 (0x0000) - Handled: NO
- **SMSG_SPELL_EXTRA_ATTACKS** = 0 (0x0000) - Handled: NO
- **SMSG_TAXI_PATH_ACTIVATED** = 0 (0x0000) - Handled: NO
- **SMSG_VEHICLE_RIDE_ALLOWED_QUERY_RESPONSE** = 0 (0x0000) - Handled: NO
- **SMSG_WHOIS** = 0 (0x0000) - Handled: NO
- **SMSG_WIPE_ALL_CRITERIA_FROM_CLIENT** = 0 (0x0000) - Handled: NO
- **SMSG_WORLD_STATE_UI_TIMER_UPDATE** = 0 (0x0000) - Handled: NO

## Summary

- Total modern opcodes: 895
- CMSG: 427
- SMSG: 467
- MSG: 1
- Other: 0
- Handled: 638
- Unhandled: 257
- Also in legacy: 600
- Modern only (no legacy equivalent): 295
- Missing from universal enum: 44
