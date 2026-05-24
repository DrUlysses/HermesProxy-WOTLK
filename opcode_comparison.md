# Opcode Comparison: Legacy (3.3.5a) vs Modern (3.4.3)

## 1. Opcodes Present in BOTH Legacy and Modern

Count: 600

| Opcode                                            | Legacy Value  | Modern Value   | Handled | Handler Location                                                                           |
|---------------------------------------------------|---------------|----------------|---------|--------------------------------------------------------------------------------------------|
| CMSG_ACCEPT_GUILD_INVITE                          | 132 (0x0084)  | 13822 (0x35FE) | YES     | HermesProxy/World/Server/WorldSocket.cs:1494                                               |
| CMSG_ACCEPT_TRADE                                 | 282 (0x011A)  | 12634 (0x315A) | YES     | HermesProxy/World/Server/WorldSocket.cs:4385                                               |
| CMSG_ACTIVATE_TAXI                                | 429 (0x01AD)  | 13483 (0x34AB) | YES     | HermesProxy/World/Server/WorldSocket.cs:4234                                               |
| CMSG_ADD_FRIEND                                   | 105 (0x0069)  | 14040 (0x36D8) | YES     | HermesProxy/World/Server/WorldSocket.cs:3697                                               |
| CMSG_ADD_IGNORE                                   | 108 (0x006C)  | 14044 (0x36DC) | YES     | HermesProxy/World/Server/WorldSocket.cs:3709                                               |
| CMSG_ALTER_APPEARANCE                             | 1062 (0x0426) | 13557 (0x34F5) | NO      |                                                                                            |
| CMSG_AREA_SPIRIT_HEALER_QUERY                     | 738 (0x02E2)  | 13488 (0x34B0) | YES     | HermesProxy/World/Server/WorldSocket.cs:3057                                               |
| CMSG_AREA_SPIRIT_HEALER_QUEUE                     | 739 (0x02E3)  | 13489 (0x34B1) | YES     | HermesProxy/World/Server/WorldSocket.cs:3058                                               |
| CMSG_AREA_TRIGGER                                 | 180 (0x00B4)  | 12758 (0x31D6) | YES     | HermesProxy/World/Server/WorldSocket.cs:2406                                               |
| CMSG_ARENA_TEAM_ACCEPT                            | 849 (0x0351)  | 14009 (0x36B9) | YES     | HermesProxy/World/Server/WorldSocket.cs:247                                                |
| CMSG_ARENA_TEAM_DECLINE                           | 850 (0x0352)  | 14010 (0x36BA) | YES     | HermesProxy/World/Server/WorldSocket.cs:248                                                |
| CMSG_ARENA_TEAM_DISBAND                           | 853 (0x0355)  | 14013 (0x36BD) | YES     | HermesProxy/World/Server/WorldSocket.cs:238                                                |
| CMSG_ARENA_TEAM_INVITE                            | 847 (0x034F)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_ARENA_TEAM_LEADER                            | 854 (0x0356)  | 14014 (0x36BE) | YES     | HermesProxy/World/Server/WorldSocket.cs:229                                                |
| CMSG_ARENA_TEAM_LEAVE                             | 851 (0x0353)  | 14011 (0x36BB) | YES     | HermesProxy/World/Server/WorldSocket.cs:239                                                |
| CMSG_ARENA_TEAM_QUERY                             | 843 (0x034B)  | 0 (0x0000)     | YES     | HermesProxy/World/Server/WorldSocket.cs:186                                                |
| CMSG_ARENA_TEAM_REMOVE                            | 852 (0x0354)  | 14012 (0x36BC) | YES     | HermesProxy/World/Server/WorldSocket.cs:228                                                |
| CMSG_ARENA_TEAM_ROSTER                            | 845 (0x034D)  | 14008 (0x36B8) | YES     | HermesProxy/World/Server/WorldSocket.cs:168                                                |
| CMSG_ATTACK_STOP                                  | 322 (0x0142)  | 12886 (0x3256) | YES     | HermesProxy/World/Server/WorldSocket.cs:1106                                               |
| CMSG_ATTACK_SWING                                 | 321 (0x0141)  | 12885 (0x3255) | YES     | HermesProxy/World/Server/WorldSocket.cs:1098                                               |
| CMSG_AUCTION_LIST_BIDDED_ITEMS                    | 612 (0x0264)  | 13520 (0x34D0) | YES     | HermesProxy/World/Server/WorldSocket.cs:263                                                |
| CMSG_AUCTION_LIST_ITEMS                           | 600 (0x0258)  | 13517 (0x34CD) | YES     | HermesProxy/World/Server/WorldSocket.cs:286                                                |
| CMSG_AUCTION_LIST_OWNED_ITEMS                     | 601 (0x0259)  | 13519 (0x34CF) | YES     | HermesProxy/World/Server/WorldSocket.cs:277                                                |
| CMSG_AUCTION_PLACE_BID                            | 602 (0x025A)  | 13521 (0x34D1) | YES     | HermesProxy/World/Server/WorldSocket.cs:433                                                |
| CMSG_AUCTION_REMOVE_ITEM                          | 599 (0x0257)  | 13516 (0x34CC) | YES     | HermesProxy/World/Server/WorldSocket.cs:424                                                |
| CMSG_AUCTION_SELL_ITEM                            | 598 (0x0256)  | 13515 (0x34CB) | YES     | HermesProxy/World/Server/WorldSocket.cs:362                                                |
| CMSG_AUTH_CONTINUED_SESSION                       | 1298 (0x0512) | 14182 (0x3766) | NO      |                                                                                            |
| CMSG_AUTH_SESSION                                 | 493 (0x01ED)  | 14181 (0x3765) | NO      |                                                                                            |
| CMSG_AUTOBANK_ITEM                                | 643 (0x0283)  | 14743 (0x3997) | YES     | HermesProxy/World/Server/WorldSocket.cs:2058                                               |
| CMSG_AUTOSTORE_BANK_ITEM                          | 642 (0x0282)  | 14742 (0x3996) | YES     | HermesProxy/World/Server/WorldSocket.cs:2057                                               |
| CMSG_AUTOSTORE_LOOT_ITEM                          | 264 (0x0108)  | 12817 (0x3211) | YES     | HermesProxy/World/Server/WorldSocket.cs:2182                                               |
| CMSG_AUTO_EQUIP_ITEM                              | 266 (0x010A)  | 14744 (0x3998) | YES     | HermesProxy/World/Server/WorldSocket.cs:2056                                               |
| CMSG_AUTO_EQUIP_ITEM_SLOT                         | 271 (0x010F)  | 14749 (0x399D) | YES     | HermesProxy/World/Server/WorldSocket.cs:2069                                               |
| CMSG_AUTO_STORE_BAG_ITEM                          | 267 (0x010B)  | 14745 (0x3999) | NO      |                                                                                            |
| CMSG_BANKER_ACTIVATE                              | 439 (0x01B7)  | 13491 (0x34B3) | YES     | HermesProxy/World/Server/WorldSocket.cs:3050                                               |
| CMSG_BATTLEFIELD_LEAVE                            | 737 (0x02E1)  | 12661 (0x3175) | YES     | HermesProxy/World/Server/WorldSocket.cs:505                                                |
| CMSG_BATTLEFIELD_LIST                             | 572 (0x023C)  | 12673 (0x3181) | YES     | HermesProxy/World/Server/WorldSocket.cs:495                                                |
| CMSG_BATTLEMASTER_HELLO                           | 727 (0x02D7)  | 12977 (0x32B1) | YES     | HermesProxy/World/Server/WorldSocket.cs:3056                                               |
| CMSG_BATTLEMASTER_JOIN                            | 750 (0x02EE)  | 13600 (0x3520) | YES     | HermesProxy/World/Server/WorldSocket.cs:443                                                |
| CMSG_BATTLEMASTER_JOIN_ARENA                      | 856 (0x0358)  | 13601 (0x3521) | YES     | HermesProxy/World/Server/WorldSocket.cs:206                                                |
| CMSG_BEGIN_TRADE                                  | 279 (0x0117)  | 12631 (0x3157) | YES     | HermesProxy/World/Server/WorldSocket.cs:4393                                               |
| CMSG_BINDER_ACTIVATE                              | 437 (0x01B5)  | 13490 (0x34B2) | YES     | HermesProxy/World/Server/WorldSocket.cs:3051                                               |
| CMSG_BUG                                          | 458 (0x01CA)  | 13959 (0x3687) | NO      |                                                                                            |
| CMSG_BUSY_TRADE                                   | 280 (0x0118)  | 12632 (0x3158) | YES     | HermesProxy/World/Server/WorldSocket.cs:4394                                               |
| CMSG_BUY_BACK_ITEM                                | 656 (0x0290)  | 13476 (0x34A4) | YES     | HermesProxy/World/Server/WorldSocket.cs:2090                                               |
| CMSG_BUY_BANK_SLOT                                | 441 (0x01B9)  | 13492 (0x34B4) | YES     | HermesProxy/World/Server/WorldSocket.cs:3083                                               |
| CMSG_BUY_ITEM                                     | 418 (0x01A2)  | 13475 (0x34A3) | YES     | HermesProxy/World/Server/WorldSocket.cs:1953                                               |
| CMSG_BUY_STABLE_SLOT                              | 626 (0x0272)  | 12651 (0x316B) | YES     | HermesProxy/World/Server/WorldSocket.cs:3185                                               |
| CMSG_CALENDAR_GET_NUM_PENDING                     | 1095 (0x0447) | 13948 (0x367C) | YES     | HermesProxy/World/Server/WorldSocket.cs:2761                                               |
| CMSG_CANCEL_AURA                                  | 310 (0x0136)  | 12719 (0x31AF) | YES     | HermesProxy/World/Server/WorldSocket.cs:4097                                               |
| CMSG_CANCEL_AUTO_REPEAT_SPELL                     | 621 (0x026D)  | 13543 (0x34E7) | YES     | HermesProxy/World/Server/WorldSocket.cs:4086                                               |
| CMSG_CANCEL_CAST                                  | 303 (0x012F)  | 12959 (0x329F) | YES     | HermesProxy/World/Server/WorldSocket.cs:4058                                               |
| CMSG_CANCEL_CHANNELLING                           | 315 (0x013B)  | 12906 (0x326A) | YES     | HermesProxy/World/Server/WorldSocket.cs:4074                                               |
| CMSG_CANCEL_GROWTH_AURA                           | 667 (0x029B)  | 12911 (0x326F) | NO      |                                                                                            |
| CMSG_CANCEL_MOUNT_AURA                            | 885 (0x0375)  | 12927 (0x327F) | YES     | HermesProxy/World/Server/WorldSocket.cs:4105                                               |
| CMSG_CANCEL_TEMP_ENCHANTMENT                      | 889 (0x0379)  | 13554 (0x34F2) | YES     | HermesProxy/World/Server/WorldSocket.cs:2147                                               |
| CMSG_CANCEL_TRADE                                 | 284 (0x011C)  | 12636 (0x315C) | YES     | HermesProxy/World/Server/WorldSocket.cs:4395                                               |
| CMSG_CAST_SPELL                                   | 302 (0x012E)  | 12956 (0x329C) | YES     | HermesProxy/World/Server/WorldSocket.cs:3862                                               |
| CMSG_CHARACTER_RENAME_REQUEST                     | 711 (0x02C7)  | 14025 (0x36C9) | YES     | HermesProxy/World/Server/WorldSocket.cs:764                                                |
| CMSG_CHAR_DELETE                                  | 56 (0x0038)   | 13981 (0x369D) | YES     | HermesProxy/World/Server/WorldSocket.cs:580                                                |
| CMSG_CHAT_CHANNEL_ANNOUNCEMENTS                   | 167 (0x00A7)  | 14307 (0x37E3) | YES     | HermesProxy/World/Server/WorldSocket.cs:793                                                |
| CMSG_CHAT_CHANNEL_BAN                             | 165 (0x00A5)  | 14305 (0x37E1) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_DECLINE_INVITE                  | 1040 (0x0410) | 14310 (0x37E6) | YES     | HermesProxy/World/Server/WorldSocket.cs:828                                                |
| CMSG_CHAT_CHANNEL_DISPLAY_LIST                    | 978 (0x03D2)  | 14294 (0x37D6) | YES     | HermesProxy/World/Server/WorldSocket.cs:810                                                |
| CMSG_CHAT_CHANNEL_INVITE                          | 163 (0x00A3)  | 14303 (0x37DF) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_KICK                            | 164 (0x00A4)  | 14304 (0x37E0) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_LIST                            | 154 (0x009A)  | 14293 (0x37D5) | YES     | HermesProxy/World/Server/WorldSocket.cs:801                                                |
| CMSG_CHAT_CHANNEL_MODERATOR                       | 159 (0x009F)  | 14299 (0x37DB) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_OWNER                           | 158 (0x009E)  | 14297 (0x37D9) | YES     | HermesProxy/World/Server/WorldSocket.cs:792                                                |
| CMSG_CHAT_CHANNEL_PASSWORD                        | 156 (0x009C)  | 14295 (0x37D7) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_SET_OWNER                       | 157 (0x009D)  | 14296 (0x37D8) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_SILENCE_ALL                     | 973 (0x03CD)  | 14308 (0x37E4) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_UNBAN                           | 166 (0x00A6)  | 14306 (0x37E2) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_UNMODERATOR                     | 160 (0x00A0)  | 14300 (0x37DC) | NO      |                                                                                            |
| CMSG_CHAT_CHANNEL_UNSILENCE_ALL                   | 975 (0x03CF)  | 14309 (0x37E5) | NO      |                                                                                            |
| CMSG_CHAT_JOIN_CHANNEL                            | 151 (0x0097)  | 14280 (0x37C8) | YES     | HermesProxy/World/Server/WorldSocket.cs:773                                                |
| CMSG_CHAT_LEAVE_CHANNEL                           | 152 (0x0098)  | 14281 (0x37C9) | YES     | HermesProxy/World/Server/WorldSocket.cs:782                                                |
| CMSG_CLEAR_TRADE_ITEM                             | 286 (0x011E)  | 12638 (0x315E) | YES     | HermesProxy/World/Server/WorldSocket.cs:4404                                               |
| CMSG_COMPLETE_CINEMATIC                           | 252 (0x00FC)  | 13637 (0x3545) | YES     | HermesProxy/World/Server/WorldSocket.cs:2462                                               |
| CMSG_CONVERT_RAID                                 | 654 (0x028E)  | 13905 (0x3651) | YES     | HermesProxy/World/Server/WorldSocket.cs:1250                                               |
| CMSG_CREATE_CHARACTER                             | 54 (0x0036)   | 13893 (0x3645) | YES     | HermesProxy/World/Server/WorldSocket.cs:562                                                |
| CMSG_DEL_FRIEND                                   | 106 (0x006A)  | 14041 (0x36D9) | YES     | HermesProxy/World/Server/WorldSocket.cs:3717                                               |
| CMSG_DEL_IGNORE                                   | 109 (0x006D)  | 14045 (0x36DD) | YES     | HermesProxy/World/Server/WorldSocket.cs:3718                                               |
| CMSG_DESTROY_ITEM                                 | 273 (0x0111)  | 12947 (0x3293) | YES     | HermesProxy/World/Server/WorldSocket.cs:2044                                               |
| CMSG_DF_GET_JOIN_STATUS                           | 662 (0x0296)  | 13846 (0x3616) | YES     | HermesProxy/World/Server/WorldSocket.cs:2756                                               |
| CMSG_ENABLE_TAXI_NODE                             | 1171 (0x0493) | 13481 (0x34A9) | YES     | HermesProxy/World/Server/WorldSocket.cs:4226                                               |
| CMSG_ENUM_CHARACTERS                              | 55 (0x0037)   | 13801 (0x35E9) | YES     | HermesProxy/World/Server/WorldSocket.cs:523                                                |
| CMSG_FAR_SIGHT                                    | 634 (0x027A)  | 13544 (0x34E8) | YES     | HermesProxy/World/Server/WorldSocket.cs:2469                                               |
| CMSG_GAME_OBJ_REPORT_USE                          | 1153 (0x0481) | 13551 (0x34EF) | YES     | HermesProxy/World/Server/WorldSocket.cs:1157                                               |
| CMSG_GAME_OBJ_USE                                 | 177 (0x00B1)  | 13550 (0x34EE) | YES     | HermesProxy/World/Server/WorldSocket.cs:1147                                               |
| CMSG_GM_TICKET_CREATE                             | 517 (0x0205)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_GM_TICKET_DELETE_TICKET                      | 535 (0x0217)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_GM_TICKET_GET_TICKET                         | 529 (0x0211)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_GM_TICKET_UPDATE_TEXT                        | 519 (0x0207)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_GOSSIP_SELECT_OPTION                         | 380 (0x017C)  | 13460 (0x3494) | YES     | HermesProxy/World/Server/WorldSocket.cs:3066                                               |
| CMSG_GROUP_DECLINE                                | 115 (0x0073)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_GROUP_SWAP_SUB_GROUP                         | 640 (0x0280)  | 0 (0x0000)     | YES     | HermesProxy/World/Server/WorldSocket.cs:1332                                               |
| CMSG_GUILD_ADD_RANK                               | 562 (0x0232)  | 12388 (0x3064) | YES     | HermesProxy/World/Server/WorldSocket.cs:1464                                               |
| CMSG_GUILD_BANK_ACTIVATE                          | 998 (0x03E6)  | 13493 (0x34B5) | YES     | HermesProxy/World/Server/WorldSocket.cs:1547                                               |
| CMSG_GUILD_BANK_BUY_TAB                           | 1002 (0x03EA) | 13507 (0x34C3) | YES     | HermesProxy/World/Server/WorldSocket.cs:1611                                               |
| CMSG_GUILD_BANK_DEPOSIT_MONEY                     | 1004 (0x03EC) | 13509 (0x34C5) | YES     | HermesProxy/World/Server/WorldSocket.cs:1566                                               |
| CMSG_GUILD_BANK_QUERY_TAB                         | 999 (0x03E7)  | 13506 (0x34C2) | YES     | HermesProxy/World/Server/WorldSocket.cs:1556                                               |
| CMSG_GUILD_BANK_SET_TAB_TEXT                      | 1035 (0x040B) | 12422 (0x3086) | YES     | HermesProxy/World/Server/WorldSocket.cs:1602                                               |
| CMSG_GUILD_BANK_SWAP_ITEMS                        | 1001 (0x03E9) | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_GUILD_BANK_UPDATE_TAB                        | 1003 (0x03EB) | 13508 (0x34C4) | YES     | HermesProxy/World/Server/WorldSocket.cs:1583                                               |
| CMSG_GUILD_BANK_WITHDRAW_MONEY                    | 1005 (0x03ED) | 13510 (0x34C6) | YES     | HermesProxy/World/Server/WorldSocket.cs:1620                                               |
| CMSG_GUILD_DECLINE_INVITATION                     | 133 (0x0085)  | 12384 (0x3060) | YES     | HermesProxy/World/Server/WorldSocket.cs:1501                                               |
| CMSG_GUILD_DELETE                                 | 143 (0x008F)  | 12392 (0x3068) | YES     | HermesProxy/World/Server/WorldSocket.cs:1508                                               |
| CMSG_GUILD_DELETE_RANK                            | 563 (0x0233)  | 12389 (0x3065) | YES     | HermesProxy/World/Server/WorldSocket.cs:1472                                               |
| CMSG_GUILD_DEMOTE_MEMBER                          | 140 (0x008C)  | 12382 (0x305E) | YES     | HermesProxy/World/Server/WorldSocket.cs:1411                                               |
| CMSG_GUILD_GET_ROSTER                             | 137 (0x0089)  | 12403 (0x3073) | YES     | HermesProxy/World/Server/WorldSocket.cs:1369                                               |
| CMSG_GUILD_INVITE_BY_NAME                         | 130 (0x0082)  | 13832 (0x3608) | YES     | HermesProxy/World/Server/WorldSocket.cs:1427                                               |
| CMSG_GUILD_LEAVE                                  | 141 (0x008D)  | 12386 (0x3062) | YES     | HermesProxy/World/Server/WorldSocket.cs:1487                                               |
| CMSG_GUILD_OFFICER_REMOVE_MEMBER                  | 142 (0x008E)  | 12387 (0x3063) | YES     | HermesProxy/World/Server/WorldSocket.cs:1419                                               |
| CMSG_GUILD_PROMOTE_MEMBER                         | 139 (0x008B)  | 12381 (0x305D) | YES     | HermesProxy/World/Server/WorldSocket.cs:1403                                               |
| CMSG_GUILD_SET_GUILD_MASTER                       | 144 (0x0090)  | 14032 (0x36D0) | YES     | HermesProxy/World/Server/WorldSocket.cs:1479                                               |
| CMSG_GUILD_SET_RANK_PERMISSIONS                   | 561 (0x0231)  | 12391 (0x3067) | YES     | HermesProxy/World/Server/WorldSocket.cs:1445                                               |
| CMSG_GUILD_UPDATE_INFO_TEXT                       | 764 (0x02FC)  | 12405 (0x3075) | YES     | HermesProxy/World/Server/WorldSocket.cs:1386                                               |
| CMSG_GUILD_UPDATE_MOTD_TEXT                       | 145 (0x0091)  | 12404 (0x3074) | YES     | HermesProxy/World/Server/WorldSocket.cs:1378                                               |
| CMSG_HEARTH_AND_RESURRECT                         | 1180 (0x049C) | 13574 (0x3506) | NO      |                                                                                            |
| CMSG_IGNORE_TRADE                                 | 281 (0x0119)  | 12633 (0x3159) | YES     | HermesProxy/World/Server/WorldSocket.cs:4397                                               |
| CMSG_INITIATE_TRADE                               | 278 (0x0116)  | 12630 (0x3156) | YES     | HermesProxy/World/Server/WorldSocket.cs:4360                                               |
| CMSG_INSPECT                                      | 276 (0x0114)  | 13609 (0x3529) | YES     | HermesProxy/World/Server/WorldSocket.cs:730                                                |
| CMSG_INSTANCE_LOCK_RESPONSE                       | 319 (0x013F)  | 13579 (0x350B) | NO      |                                                                                            |
| CMSG_ITEM_QUERY_SINGLE                            | 86 (0x0056)   | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_ITEM_TEXT_QUERY                              | 579 (0x0243)  | 12997 (0x32C5) | NO      |                                                                                            |
| CMSG_KEEP_ALIVE                                   | 1031 (0x0407) | 13953 (0x3681) | NO      |                                                                                            |
| CMSG_LEARN_TALENT                                 | 593 (0x0251)  | 13650 (0x3552) | YES     | HermesProxy/World/Server/WorldSocket.cs:4132                                               |
| CMSG_LIST_INVENTORY                               | 414 (0x019E)  | 13473 (0x34A1) | YES     | HermesProxy/World/Server/WorldSocket.cs:3052                                               |
| CMSG_LOGOUT_CANCEL                                | 78 (0x004E)   | 13528 (0x34D8) | YES     | HermesProxy/World/Server/WorldSocket.cs:653                                                |
| CMSG_LOGOUT_REQUEST                               | 75 (0x004B)   | 13526 (0x34D6) | YES     | HermesProxy/World/Server/WorldSocket.cs:646                                                |
| CMSG_LOOT_MASTER_GIVE                             | 675 (0x02A3)  | 0 (0x0000)     | YES     | HermesProxy/World/Server/WorldSocket.cs:2244                                               |
| CMSG_LOOT_MONEY                                   | 350 (0x015E)  | 12816 (0x3210) | YES     | HermesProxy/World/Server/WorldSocket.cs:2202                                               |
| CMSG_LOOT_RELEASE                                 | 351 (0x015F)  | 12819 (0x3213) | YES     | HermesProxy/World/Server/WorldSocket.cs:2173                                               |
| CMSG_LOOT_ROLL                                    | 672 (0x02A0)  | 12820 (0x3214) | YES     | HermesProxy/World/Server/WorldSocket.cs:2234                                               |
| CMSG_LOOT_UNIT                                    | 349 (0x015D)  | 12815 (0x320F) | YES     | HermesProxy/World/Server/WorldSocket.cs:2193                                               |
| CMSG_MAIL_CREATE_TEXT_ITEM                        | 586 (0x024A)  | 13627 (0x353B) | YES     | HermesProxy/World/Server/WorldSocket.cs:2272                                               |
| CMSG_MAIL_DELETE                                  | 585 (0x0249)  | 12837 (0x3225) | YES     | HermesProxy/World/Server/WorldSocket.cs:2285                                               |
| CMSG_MAIL_GET_LIST                                | 570 (0x023A)  | 13622 (0x3536) | YES     | HermesProxy/World/Server/WorldSocket.cs:2264                                               |
| CMSG_MAIL_MARK_AS_READ                            | 583 (0x0247)  | 13626 (0x353A) | YES     | HermesProxy/World/Server/WorldSocket.cs:2298                                               |
| CMSG_MAIL_RETURN_TO_SENDER                        | 584 (0x0248)  | 13912 (0x3658) | YES     | HermesProxy/World/Server/WorldSocket.cs:2307                                               |
| CMSG_MAIL_TAKE_ITEM                               | 582 (0x0246)  | 13624 (0x3538) | YES     | HermesProxy/World/Server/WorldSocket.cs:2320                                               |
| CMSG_MAIL_TAKE_MONEY                              | 581 (0x0245)  | 13623 (0x3537) | YES     | HermesProxy/World/Server/WorldSocket.cs:2333                                               |
| CMSG_MESSAGECHAT                                  | 149 (0x0095)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_MOUNT_SPECIAL_ANIM                           | 369 (0x0171)  | 12928 (0x3280) | YES     | HermesProxy/World/Server/WorldSocket.cs:2478                                               |
| CMSG_MOVE_CHANGE_TRANSPORT                        | 909 (0x038D)  | 14895 (0x3A2F) | YES     | HermesProxy/World/Server/WorldSocket.cs:2836                                               |
| CMSG_MOVE_FALL_RESET                              | 714 (0x02CA)  | 14873 (0x3A19) | YES     | HermesProxy/World/Server/WorldSocket.cs:2839                                               |
| CMSG_MOVE_FEATHER_FALL_ACK                        | 719 (0x02CF)  | 14876 (0x3A1C) | YES     | HermesProxy/World/Server/WorldSocket.cs:2959                                               |
| CMSG_MOVE_FORCE_FLIGHT_BACK_SPEED_CHANGE_ACK      | 900 (0x0384)  | 14894 (0x3A2E) | YES     | HermesProxy/World/Server/WorldSocket.cs:2910                                               |
| CMSG_MOVE_FORCE_FLIGHT_SPEED_CHANGE_ACK           | 898 (0x0382)  | 14893 (0x3A2D) | YES     | HermesProxy/World/Server/WorldSocket.cs:2911                                               |
| CMSG_MOVE_FORCE_PITCH_RATE_CHANGE_ACK             | 1117 (0x045D) | 14898 (0x3A32) | YES     | HermesProxy/World/Server/WorldSocket.cs:2912                                               |
| CMSG_MOVE_FORCE_ROOT_ACK                          | 233 (0x00E9)  | 14862 (0x3A0E) | YES     | HermesProxy/World/Server/WorldSocket.cs:2980                                               |
| CMSG_MOVE_FORCE_RUN_BACK_SPEED_CHANGE_ACK         | 229 (0x00E5)  | 14860 (0x3A0C) | YES     | HermesProxy/World/Server/WorldSocket.cs:2913                                               |
| CMSG_MOVE_FORCE_RUN_SPEED_CHANGE_ACK              | 227 (0x00E3)  | 14859 (0x3A0B) | YES     | HermesProxy/World/Server/WorldSocket.cs:2914                                               |
| CMSG_MOVE_FORCE_SWIM_BACK_SPEED_CHANGE_ACK        | 733 (0x02DD)  | 14882 (0x3A22) | YES     | HermesProxy/World/Server/WorldSocket.cs:2915                                               |
| CMSG_MOVE_FORCE_SWIM_SPEED_CHANGE_ACK             | 231 (0x00E7)  | 14861 (0x3A0D) | YES     | HermesProxy/World/Server/WorldSocket.cs:2916                                               |
| CMSG_MOVE_FORCE_TURN_RATE_CHANGE_ACK              | 735 (0x02DF)  | 14883 (0x3A23) | YES     | HermesProxy/World/Server/WorldSocket.cs:2917                                               |
| CMSG_MOVE_FORCE_UNROOT_ACK                        | 235 (0x00EB)  | 14863 (0x3A0F) | YES     | HermesProxy/World/Server/WorldSocket.cs:2981                                               |
| CMSG_MOVE_FORCE_WALK_SPEED_CHANGE_ACK             | 731 (0x02DB)  | 14881 (0x3A21) | YES     | HermesProxy/World/Server/WorldSocket.cs:2918                                               |
| CMSG_MOVE_GRAVITY_DISABLE_ACK                     | 1231 (0x04CF) | 14901 (0x3A35) | YES     | HermesProxy/World/Server/WorldSocket.cs:2983                                               |
| CMSG_MOVE_GRAVITY_ENABLE_ACK                      | 1233 (0x04D1) | 14902 (0x3A36) | YES     | HermesProxy/World/Server/WorldSocket.cs:2984                                               |
| CMSG_MOVE_HOVER_ACK                               | 246 (0x00F6)  | 14867 (0x3A13) | YES     | HermesProxy/World/Server/WorldSocket.cs:2960                                               |
| CMSG_MOVE_KNOCK_BACK_ACK                          | 240 (0x00F0)  | 14866 (0x3A12) | YES     | HermesProxy/World/Server/WorldSocket.cs:2982                                               |
| CMSG_MOVE_SET_CAN_FLY_ACK                         | 837 (0x0345)  | 14887 (0x3A27) | YES     | HermesProxy/World/Server/WorldSocket.cs:2961                                               |
| CMSG_MOVE_SET_FLY                                 | 838 (0x0346)  | 14888 (0x3A28) | YES     | HermesProxy/World/Server/WorldSocket.cs:2845                                               |
| CMSG_MOVE_SPLINE_DONE                             | 713 (0x02C9)  | 14872 (0x3A18) | YES     | HermesProxy/World/Server/WorldSocket.cs:3017                                               |
| CMSG_MOVE_TIME_SKIPPED                            | 718 (0x02CE)  | 14875 (0x3A1B) | YES     | HermesProxy/World/Server/WorldSocket.cs:3034                                               |
| CMSG_MOVE_WATER_WALK_ACK                          | 720 (0x02D0)  | 14877 (0x3A1D) | YES     | HermesProxy/World/Server/WorldSocket.cs:2962                                               |
| CMSG_NEXT_CINEMATIC_CAMERA                        | 251 (0x00FB)  | 13636 (0x3544) | YES     | HermesProxy/World/Server/WorldSocket.cs:2461                                               |
| CMSG_OFFER_PETITION                               | 451 (0x01C3)  | 13053 (0x32FD) | YES     | HermesProxy/World/Server/WorldSocket.cs:3312                                               |
| CMSG_OPENING_CINEMATIC                            | 249 (0x00F9)  | 13635 (0x3543) | YES     | HermesProxy/World/Server/WorldSocket.cs:2460                                               |
| CMSG_OPEN_ITEM                                    | 172 (0x00AC)  | 12998 (0x32C6) | YES     | HermesProxy/World/Server/WorldSocket.cs:2128                                               |
| CMSG_OPT_OUT_OF_LOOT                              | 1033 (0x0409) | 13558 (0x34F6) | YES     | HermesProxy/World/Server/WorldSocket.cs:2219                                               |
| CMSG_PARTY_INVITE                                 | 110 (0x006E)  | 13828 (0x3604) | YES     | HermesProxy/World/Server/WorldSocket.cs:1166                                               |
| CMSG_PETITION_BUY                                 | 445 (0x01BD)  | 13512 (0x34C8) | YES     | HermesProxy/World/Server/WorldSocket.cs:3243                                               |
| CMSG_PETITION_SHOW_SIGNATURES                     | 446 (0x01BE)  | 13513 (0x34C9) | YES     | HermesProxy/World/Server/WorldSocket.cs:3286                                               |
| CMSG_PET_ABANDON                                  | 374 (0x0176)  | 13453 (0x348D) | YES     | HermesProxy/World/Server/WorldSocket.cs:3193                                               |
| CMSG_PET_ACTION                                   | 373 (0x0175)  | 13451 (0x348B) | YES     | HermesProxy/World/Server/WorldSocket.cs:3129                                               |
| CMSG_PET_CANCEL_AURA                              | 619 (0x026B)  | 13454 (0x348E) | YES     | HermesProxy/World/Server/WorldSocket.cs:3227                                               |
| CMSG_PET_CAST_SPELL                               | 496 (0x01F0)  | 12955 (0x329B) | YES     | HermesProxy/World/Server/WorldSocket.cs:3945                                               |
| CMSG_PET_RENAME                                   | 375 (0x0177)  | 13958 (0x3686) | YES     | HermesProxy/World/Server/WorldSocket.cs:3157                                               |
| CMSG_PET_SET_ACTION                               | 372 (0x0174)  | 13450 (0x348A) | YES     | HermesProxy/World/Server/WorldSocket.cs:3147                                               |
| CMSG_PET_STOP_ATTACK                              | 746 (0x02EA)  | 13452 (0x348C) | YES     | HermesProxy/World/Server/WorldSocket.cs:3139                                               |
| CMSG_PING                                         | 476 (0x01DC)  | 14184 (0x3768) | NO      |                                                                                            |
| CMSG_PLAYER_AI_CHEAT                              | 620 (0x026C)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_PLAYER_LOGIN                                 | 61 (0x003D)   | 13803 (0x35EB) | YES     | HermesProxy/World/Server/WorldSocket.cs:616                                                |
| CMSG_PUSH_QUEST_TO_PARTY                          | 413 (0x019D)  | 13471 (0x349F) | YES     | HermesProxy/World/Server/WorldSocket.cs:3596                                               |
| CMSG_QUERY_CREATURE                               | 96 (0x0060)   | 12912 (0x3270) | YES     | HermesProxy/World/Server/WorldSocket.cs:3373                                               |
| CMSG_QUERY_GAME_OBJECT                            | 94 (0x005E)   | 12913 (0x3271) | YES     | HermesProxy/World/Server/WorldSocket.cs:3382                                               |
| CMSG_QUERY_GUILD_INFO                             | 84 (0x0054)   | 13963 (0x368B) | YES     | HermesProxy/World/Server/WorldSocket.cs:1341                                               |
| CMSG_QUERY_NPC_TEXT                               | 383 (0x017F)  | 12914 (0x3272) | YES     | HermesProxy/World/Server/WorldSocket.cs:3411                                               |
| CMSG_QUERY_PAGE_TEXT                              | 90 (0x005A)   | 12916 (0x3274) | YES     | HermesProxy/World/Server/WorldSocket.cs:3402                                               |
| CMSG_QUERY_PETITION                               | 454 (0x01C6)  | 12919 (0x3277) | YES     | HermesProxy/World/Server/WorldSocket.cs:3294                                               |
| CMSG_QUERY_PET_NAME                               | 82 (0x0052)   | 12917 (0x3275) | YES     | HermesProxy/World/Server/WorldSocket.cs:3420                                               |
| CMSG_QUERY_QUESTS_COMPLETED                       | 1280 (0x0500) | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_QUERY_QUEST_INFO                             | 92 (0x005C)   | 12915 (0x3273) | YES     | HermesProxy/World/Server/WorldSocket.cs:3365                                               |
| CMSG_QUERY_TIME                                   | 462 (0x01CE)  | 13525 (0x34D5) | YES     | HermesProxy/World/Server/WorldSocket.cs:3358                                               |
| CMSG_QUEST_CONFIRM_ACCEPT                         | 411 (0x019B)  | 13470 (0x349E) | YES     | HermesProxy/World/Server/WorldSocket.cs:3588                                               |
| CMSG_QUEST_GIVER_ACCEPT_QUEST                     | 393 (0x0189)  | 13464 (0x3498) | YES     | HermesProxy/World/Server/WorldSocket.cs:3466                                               |
| CMSG_QUEST_GIVER_CHOOSE_REWARD                    | 398 (0x018E)  | 13466 (0x349A) | YES     | HermesProxy/World/Server/WorldSocket.cs:3544                                               |
| CMSG_QUEST_GIVER_COMPLETE_QUEST                   | 394 (0x018A)  | 13465 (0x3499) | YES     | HermesProxy/World/Server/WorldSocket.cs:3579                                               |
| CMSG_QUEST_GIVER_HELLO                            | 388 (0x0184)  | 13462 (0x3496) | YES     | HermesProxy/World/Server/WorldSocket.cs:3527                                               |
| CMSG_QUEST_GIVER_QUERY_QUEST                      | 390 (0x0186)  | 13463 (0x3497) | YES     | HermesProxy/World/Server/WorldSocket.cs:3453                                               |
| CMSG_QUEST_GIVER_REQUEST_REWARD                   | 396 (0x018C)  | 13467 (0x349B) | YES     | HermesProxy/World/Server/WorldSocket.cs:3535                                               |
| CMSG_QUEST_GIVER_STATUS_MULTIPLE_QUERY            | 1047 (0x0417) | 13469 (0x349D) | YES     | HermesProxy/World/Server/WorldSocket.cs:3495                                               |
| CMSG_QUEST_GIVER_STATUS_QUERY                     | 386 (0x0182)  | 13468 (0x349C) | YES     | HermesProxy/World/Server/WorldSocket.cs:3487                                               |
| CMSG_QUEST_LOG_REMOVE_QUEST                       | 404 (0x0194)  | 13614 (0x352E) | YES     | HermesProxy/World/Server/WorldSocket.cs:3479                                               |
| CMSG_QUEST_POI_QUERY                              | 483 (0x01E3)  | 14002 (0x36B2) | NO      |                                                                                            |
| CMSG_READ_ITEM                                    | 173 (0x00AD)  | 12999 (0x32C7) | YES     | HermesProxy/World/Server/WorldSocket.cs:2079                                               |
| CMSG_RECLAIM_CORPSE                               | 466 (0x01D2)  | 13531 (0x34DB) | YES     | HermesProxy/World/Server/WorldSocket.cs:2444                                               |
| CMSG_REMOVE_GLYPH                                 | 1162 (0x048A) | 13056 (0x3300) | NO      |                                                                                            |
| CMSG_REPAIR_ITEM                                  | 680 (0x02A8)  | 13548 (0x34EC) | YES     | HermesProxy/World/Server/WorldSocket.cs:2100                                               |
| CMSG_REPOP_REQUEST                                | 346 (0x015A)  | 13606 (0x3526) | YES     | HermesProxy/World/Server/WorldSocket.cs:2426                                               |
| CMSG_REQUEST_ACCOUNT_DATA                         | 522 (0x020A)  | 13972 (0x3694) | YES     | HermesProxy/World/Server/WorldSocket.cs:1075                                               |
| CMSG_REQUEST_PARTY_MEMBER_STATS                   | 639 (0x027F)  | 13910 (0x3656) | YES     | HermesProxy/World/Server/WorldSocket.cs:1315                                               |
| CMSG_REQUEST_PET_INFO                             | 633 (0x0279)  | 13456 (0x3490) | YES     | HermesProxy/World/Server/WorldSocket.cs:3236                                               |
| CMSG_REQUEST_PLAYED_TIME                          | 460 (0x01CC)  | 12922 (0x327A) | YES     | HermesProxy/World/Server/WorldSocket.cs:660                                                |
| CMSG_REQUEST_RAID_INFO                            | 717 (0x02CD)  | 14034 (0x36D2) | YES     | HermesProxy/World/Server/WorldSocket.cs:1946                                               |
| CMSG_REQUEST_VEHICLE_EXIT                         | 1142 (0x0476) | 12855 (0x3237) | NO      |                                                                                            |
| CMSG_REQUEST_VEHICLE_NEXT_SEAT                    | 1144 (0x0478) | 12857 (0x3239) | NO      |                                                                                            |
| CMSG_REQUEST_VEHICLE_PREV_SEAT                    | 1143 (0x0477) | 12856 (0x3238) | NO      |                                                                                            |
| CMSG_REQUEST_VEHICLE_SWITCH_SEAT                  | 1145 (0x0479) | 12858 (0x323A) | NO      |                                                                                            |
| CMSG_RESET_FACTION_CHEAT                          | 641 (0x0281)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_RESET_INSTANCES                              | 797 (0x031D)  | 13930 (0x366A) | YES     | HermesProxy/World/Server/WorldSocket.cs:1939                                               |
| CMSG_RESURRECT_RESPONSE                           | 348 (0x015C)  | 13957 (0x3685) | YES     | HermesProxy/World/Server/WorldSocket.cs:4141                                               |
| CMSG_SELF_RES                                     | 691 (0x02B3)  | 13617 (0x3531) | YES     | HermesProxy/World/Server/WorldSocket.cs:4150                                               |
| CMSG_SELL_ITEM                                    | 416 (0x01A0)  | 13474 (0x34A2) | YES     | HermesProxy/World/Server/WorldSocket.cs:1973                                               |
| CMSG_SEND_MAIL                                    | 568 (0x0238)  | 13819 (0x35FB) | YES     | HermesProxy/World/Server/WorldSocket.cs:2375                                               |
| CMSG_SEND_TEXT_EMOTE                              | 260 (0x0104)  | 13448 (0x3488) | YES     | HermesProxy/World/Server/WorldSocket.cs:1016                                               |
| CMSG_SET_ACTION_BAR_TOGGLES                       | 703 (0x02BF)  | 13618 (0x3532) | YES     | HermesProxy/World/Server/WorldSocket.cs:705                                                |
| CMSG_SET_ACTION_BUTTON                            | 296 (0x0128)  | 13661 (0x355D) | YES     | HermesProxy/World/Server/WorldSocket.cs:695                                                |
| CMSG_SET_ACTIVE_MOVER                             | 618 (0x026A)  | 14908 (0x3A3C) | YES     | HermesProxy/World/Server/WorldSocket.cs:3001                                               |
| CMSG_SET_AMMO                                     | 616 (0x0268)  | 13662 (0x355E) | YES     | HermesProxy/World/Server/WorldSocket.cs:2139                                               |
| CMSG_SET_ASSISTANT_LEADER                         | 655 (0x028F)  | 13906 (0x3652) | YES     | HermesProxy/World/Server/WorldSocket.cs:1217                                               |
| CMSG_SET_CONTACT_NOTES                            | 107 (0x006B)  | 14042 (0x36DA) | YES     | HermesProxy/World/Server/WorldSocket.cs:3726                                               |
| CMSG_SET_FACTION_AT_WAR                           | 293 (0x0125)  | 13534 (0x34DE) | YES     | HermesProxy/World/Server/WorldSocket.cs:3613                                               |
| CMSG_SET_FACTION_INACTIVE                         | 791 (0x0317)  | 13536 (0x34E0) | YES     | HermesProxy/World/Server/WorldSocket.cs:3631                                               |
| CMSG_SET_LOOT_METHOD                              | 122 (0x007A)  | 13899 (0x364B) | YES     | HermesProxy/World/Server/WorldSocket.cs:2209                                               |
| CMSG_SET_PARTY_LEADER                             | 120 (0x0078)  | 13901 (0x364D) | YES     | HermesProxy/World/Server/WorldSocket.cs:1242                                               |
| CMSG_SET_SELECTION                                | 317 (0x013D)  | 13608 (0x3528) | YES     | HermesProxy/World/Server/WorldSocket.cs:2418                                               |
| CMSG_SET_SHEATHED                                 | 480 (0x01E0)  | 13449 (0x3489) | YES     | HermesProxy/World/Server/WorldSocket.cs:1113                                               |
| CMSG_SET_TITLE                                    | 884 (0x0374)  | 12926 (0x327E) | YES     | HermesProxy/World/Server/WorldSocket.cs:672                                                |
| CMSG_SET_TRADE_GOLD                               | 287 (0x011F)  | 12639 (0x315F) | YES     | HermesProxy/World/Server/WorldSocket.cs:4368                                               |
| CMSG_SET_TRADE_ITEM                               | 285 (0x011D)  | 12637 (0x315D) | YES     | HermesProxy/World/Server/WorldSocket.cs:4421                                               |
| CMSG_SET_WATCHED_FACTION                          | 792 (0x0318)  | 13537 (0x34E1) | YES     | HermesProxy/World/Server/WorldSocket.cs:3640                                               |
| CMSG_SIGN_PETITION                                | 448 (0x01C0)  | 13619 (0x3533) | YES     | HermesProxy/World/Server/WorldSocket.cs:3333                                               |
| CMSG_SOCKET_GEMS                                  | 839 (0x0347)  | 13547 (0x34EB) | YES     | HermesProxy/World/Server/WorldSocket.cs:2113                                               |
| CMSG_SPELL_CLICK                                  | 1016 (0x03F8) | 13461 (0x3495) | NO      |                                                                                            |
| CMSG_SPIRIT_HEALER_ACTIVATE                       | 540 (0x021C)  | 13487 (0x34AF) | YES     | HermesProxy/World/Server/WorldSocket.cs:3053                                               |
| CMSG_SPLIT_ITEM                                   | 270 (0x010E)  | 14748 (0x399C) | YES     | HermesProxy/World/Server/WorldSocket.cs:1993                                               |
| CMSG_STABLE_PET                                   | 624 (0x0270)  | 12648 (0x3168) | YES     | HermesProxy/World/Server/WorldSocket.cs:3201                                               |
| CMSG_STABLE_REVIVE_PET                            | 628 (0x0274)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_STABLE_SWAP_PET                              | 629 (0x0275)  | 12650 (0x316A) | YES     | HermesProxy/World/Server/WorldSocket.cs:3218                                               |
| CMSG_STAND_STATE_CHANGE                           | 257 (0x0101)  | 12684 (0x318C) | YES     | HermesProxy/World/Server/WorldSocket.cs:2452                                               |
| CMSG_SUMMON_RESPONSE                              | 684 (0x02AC)  | 13932 (0x366C) | YES     | HermesProxy/World/Server/WorldSocket.cs:1286                                               |
| CMSG_SWAP_INV_ITEM                                | 269 (0x010D)  | 14747 (0x399B) | YES     | HermesProxy/World/Server/WorldSocket.cs:2016                                               |
| CMSG_SWAP_ITEM                                    | 268 (0x010C)  | 14746 (0x399A) | YES     | HermesProxy/World/Server/WorldSocket.cs:2029                                               |
| CMSG_TALK_TO_GOSSIP                               | 379 (0x017B)  | 13458 (0x3492) | YES     | HermesProxy/World/Server/WorldSocket.cs:3054                                               |
| CMSG_TAXI_NODE_STATUS_QUERY                       | 426 (0x01AA)  | 13480 (0x34A8) | YES     | HermesProxy/World/Server/WorldSocket.cs:4217                                               |
| CMSG_TAXI_QUERY_AVAILABLE_NODES                   | 428 (0x01AC)  | 13482 (0x34AA) | YES     | HermesProxy/World/Server/WorldSocket.cs:4218                                               |
| CMSG_TIME_SYNC_RESPONSE                           | 913 (0x0391)  | 14909 (0x3A3D) | YES     | HermesProxy/World/Server/WorldSocket.cs:2394                                               |
| CMSG_TOGGLE_PVP                                   | 595 (0x0253)  | 12971 (0x32AB) | YES     | HermesProxy/World/Server/WorldSocket.cs:680                                                |
| CMSG_TOTEM_DESTROYED                              | 1044 (0x0414) | 13560 (0x34F8) | YES     | HermesProxy/World/Server/WorldSocket.cs:4157                                               |
| CMSG_TRAINER_BUY_SPELL                            | 434 (0x01B2)  | 13486 (0x34AE) | YES     | HermesProxy/World/Server/WorldSocket.cs:3091                                               |
| CMSG_TRAINER_LIST                                 | 432 (0x01B0)  | 13485 (0x34AD) | YES     | HermesProxy/World/Server/WorldSocket.cs:3055                                               |
| CMSG_TURN_IN_PETITION                             | 452 (0x01C4)  | 13621 (0x3535) | YES     | HermesProxy/World/Server/WorldSocket.cs:3342                                               |
| CMSG_TUTORIAL_FLAG                                | 254 (0x00FE)  | 14052 (0x36E4) | YES     | HermesProxy/World/Server/WorldSocket.cs:2485; HermesProxy/World/Server/WorldSocket.cs:2796 |
| CMSG_UNACCEPT_TRADE                               | 283 (0x011B)  | 12635 (0x315B) | YES     | HermesProxy/World/Server/WorldSocket.cs:4396                                               |
| CMSG_UNLEARN_SKILL                                | 514 (0x0202)  | 13541 (0x34E5) | YES     | HermesProxy/World/Server/WorldSocket.cs:713                                                |
| CMSG_UNSTABLE_PET                                 | 625 (0x0271)  | 12649 (0x3169) | YES     | HermesProxy/World/Server/WorldSocket.cs:3209                                               |
| CMSG_UPDATE_ACCOUNT_DATA                          | 523 (0x020B)  | 13973 (0x3695) | YES     | HermesProxy/World/Server/WorldSocket.cs:1069                                               |
| CMSG_UPDATE_MISSILE_TRAJECTORY                    | 1122 (0x0462) | 14915 (0x3A43) | NO      |                                                                                            |
| CMSG_USE_ITEM                                     | 171 (0x00AB)  | 12952 (0x3298) | YES     | HermesProxy/World/Server/WorldSocket.cs:4001                                               |
| CMSG_WARDEN_DATA                                  | 743 (0x02E7)  | 0 (0x0000)     | NO      |                                                                                            |
| CMSG_WHO                                          | 98 (0x0062)   | 13955 (0x3683) | YES     | HermesProxy/World/Server/WorldSocket.cs:3429                                               |
| CMSG_WRAP_ITEM                                    | 467 (0x01D3)  | 14740 (0x3994) | YES     | HermesProxy/World/Server/WorldSocket.cs:2158                                               |
| CMSG_ZONEUPDATE                                   | 500 (0x01F4)  | 0 (0x0000)     | NO      |                                                                                            |
| MSG_NULL_ACTION                                   | 0 (0x0000)    | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_ACCOUNT_DATA_TIMES                           | 521 (0x0209)  | 9994 (0x270A)  | YES     | HermesProxy/World/Client/WorldClient.cs:4815                                               |
| SMSG_ACHIEVEMENT_DELETED                          | 1183 (0x049F) | 9960 (0x26E8)  | NO      |                                                                                            |
| SMSG_ACHIEVEMENT_EARNED                           | 1128 (0x0468) | 9795 (0x2643)  | YES     | HermesProxy/World/Client/WorldClient.cs:5178                                               |
| SMSG_ACTIVATE_TAXI_REPLY                          | 430 (0x01AE)  | 9853 (0x267D)  | YES     | HermesProxy/World/Client/WorldClient.cs:8926                                               |
| SMSG_ADDON_INFO                                   | 751 (0x02EF)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_AI_REACTION                                  | 316 (0x013C)  | 9909 (0x26B5)  | YES     | HermesProxy/World/Client/WorldClient.cs:2185                                               |
| SMSG_ALL_ACHIEVEMENT_DATA                         | 1149 (0x047D) | 9584 (0x2570)  | YES     | HermesProxy/World/Client/WorldClient.cs:5102                                               |
| SMSG_AREA_SPIRIT_HEALER_TIME                      | 740 (0x02E4)  | 10048 (0x2740) | YES     | HermesProxy/World/Client/WorldClient.cs:815                                                |
| SMSG_AREA_TRIGGER_MESSAGE                         | 696 (0x02B8)  | 10368 (0x2880) | YES     | HermesProxy/World/Client/WorldClient.cs:4915                                               |
| SMSG_ARENA_TEAM_COMMAND_RESULT                    | 841 (0x0349)  | 10083 (0x2763) | YES     | HermesProxy/World/Client/WorldClient.cs:173                                                |
| SMSG_ARENA_TEAM_EVENT                             | 855 (0x0357)  | 10082 (0x2762) | YES     | HermesProxy/World/Client/WorldClient.cs:143                                                |
| SMSG_ARENA_TEAM_INVITE                            | 848 (0x0350)  | 10081 (0x2761) | YES     | HermesProxy/World/Client/WorldClient.cs:185                                                |
| SMSG_ARENA_TEAM_ROSTER                            | 846 (0x034E)  | 10080 (0x2760) | YES     | HermesProxy/World/Client/WorldClient.cs:96                                                 |
| SMSG_ARENA_TEAM_STATS                             | 859 (0x035B)  | 10084 (0x2764) | YES     | HermesProxy/World/Client/WorldClient.cs:79                                                 |
| SMSG_ATTACKER_STATE_UPDATE                        | 330 (0x014A)  | 10578 (0x2952) | YES     | HermesProxy/World/Client/WorldClient.cs:2067                                               |
| SMSG_ATTACK_START                                 | 323 (0x0143)  | 10557 (0x293D) | YES     | HermesProxy/World/Client/WorldClient.cs:2015                                               |
| SMSG_ATTACK_STOP                                  | 324 (0x0144)  | 10558 (0x293E) | YES     | HermesProxy/World/Client/WorldClient.cs:2024                                               |
| SMSG_AUCTION_COMMAND_RESULT                       | 603 (0x025B)  | 9968 (0x26F0)  | YES     | HermesProxy/World/Client/WorldClient.cs:302                                                |
| SMSG_AUCTION_LIST_ITEMS_RESULT                    | 604 (0x025C)  | 10339 (0x2863) | YES     | HermesProxy/World/Client/WorldClient.cs:283                                                |
| SMSG_AUCTION_REMOVED_NOTIFICATION                 | 653 (0x028D)  | 9973 (0x26F5)  | NO      |                                                                                            |
| SMSG_AURA_UPDATE                                  | 1174 (0x0496) | 11295 (0x2C1F) | YES     | HermesProxy/World/Client/WorldClient.cs:8687                                               |
| SMSG_AURA_UPDATE_ALL                              | 1173 (0x0495) | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:8702                                               |
| SMSG_AUTH_CHALLENGE                               | 492 (0x01EC)  | 12360 (0x3048) | NO      |                                                                                            |
| SMSG_AUTH_RESPONSE                                | 494 (0x01EE)  | 9581 (0x256D)  | NO      |                                                                                            |
| SMSG_BATTLEFIELD_LIST                             | 573 (0x023D)  | 10535 (0x2927) | YES     | HermesProxy/World/Client/WorldClient.cs:385; HermesProxy/World/Client/WorldClient.cs:402   |
| SMSG_BATTLEFIELD_STATUS                           | 724 (0x02D4)  | 10533 (0x2925) | YES     | HermesProxy/World/Client/WorldClient.cs:452; HermesProxy/World/Client/WorldClient.cs:538   |
| SMSG_BATTLEFIELD_STATUS_QUEUED                    | 744 (0x02E8)  | 10532 (0x2924) | NO      |                                                                                            |
| SMSG_BATTLEGROUND_PLAYER_JOINED                   | 748 (0x02EC)  | 10539 (0x292B) | YES     | HermesProxy/World/Client/WorldClient.cs:806                                                |
| SMSG_BATTLEGROUND_PLAYER_LEFT                     | 749 (0x02ED)  | 10540 (0x292C) | YES     | HermesProxy/World/Client/WorldClient.cs:807                                                |
| SMSG_BINDER_CONFIRM                               | 747 (0x02EB)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:5980                                               |
| SMSG_BIND_POINT_UPDATE                            | 341 (0x0155)  | 9597 (0x257D)  | YES     | HermesProxy/World/Client/WorldClient.cs:4828                                               |
| SMSG_BUY_FAILED                                   | 421 (0x01A5)  | 9927 (0x26C7)  | YES     | HermesProxy/World/Client/WorldClient.cs:4161                                               |
| SMSG_BUY_SUCCEEDED                                | 420 (0x01A4)  | 9926 (0x26C6)  | YES     | HermesProxy/World/Client/WorldClient.cs:4071                                               |
| SMSG_CACHE_VERSION                                | 1195 (0x04AB) | 10524 (0x291C) | YES     | HermesProxy/World/Client/WorldClient.cs:5385                                               |
| SMSG_CANCEL_AUTO_REPEAT                           | 668 (0x029C)  | 9950 (0x26DE)  | YES     | HermesProxy/World/Client/WorldClient.cs:8193                                               |
| SMSG_CANCEL_COMBAT                                | 334 (0x014E)  | 10571 (0x294B) | YES     | HermesProxy/World/Client/WorldClient.cs:2178                                               |
| SMSG_CAST_FAILED                                  | 304 (0x0130)  | 11348 (0x2C54) | YES     | HermesProxy/World/Client/WorldClient.cs:7724                                               |
| SMSG_CHANNEL_LIST                                 | 155 (0x009B)  | 11204 (0x2BC4) | YES     | HermesProxy/World/Client/WorldClient.cs:1598                                               |
| SMSG_CHANNEL_MEMBER_COUNT                         | 981 (0x03D5)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_CHANNEL_NOTIFY                               | 153 (0x0099)  | 11201 (0x2BC1) | YES     | HermesProxy/World/Client/WorldClient.cs:1494                                               |
| SMSG_CHARACTER_LOGIN_FAILED                       | 65 (0x0041)   | 9989 (0x2705)  | YES     | HermesProxy/World/Client/WorldClient.cs:1142                                               |
| SMSG_CHARACTER_RENAME_RESULT                      | 712 (0x02C8)  | 10087 (0x2767) | YES     | HermesProxy/World/Client/WorldClient.cs:1480                                               |
| SMSG_CHAT                                         | 150 (0x0096)  | 11181 (0x2BAD) | YES     | HermesProxy/World/Client/WorldClient.cs:1626; HermesProxy/World/Client/WorldClient.cs:1695 |
| SMSG_CHAT_PLAYER_NOTFOUND                         | 681 (0x02A9)  | 11191 (0x2BB7) | YES     | HermesProxy/World/Client/WorldClient.cs:1951                                               |
| SMSG_CHAT_SERVER_MESSAGE                          | 657 (0x0291)  | 11205 (0x2BC5) | YES     | HermesProxy/World/Client/WorldClient.cs:1969                                               |
| SMSG_CLEAR_COOLDOWN                               | 478 (0x01DE)  | 9914 (0x26BA)  | YES     | HermesProxy/World/Client/WorldClient.cs:8257                                               |
| SMSG_COMPRESSED_UPDATE_OBJECT                     | 502 (0x01F6)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:9083                                               |
| SMSG_CONNECT_TO                                   | 1293 (0x050D) | 12365 (0x304D) | NO      |                                                                                            |
| SMSG_CONTACT_LIST                                 | 103 (0x0067)  | 10124 (0x278C) | YES     | HermesProxy/World/Client/WorldClient.cs:7556                                               |
| SMSG_CONTROL_UPDATE                               | 345 (0x0159)  | 9799 (0x2647)  | YES     | HermesProxy/World/Client/WorldClient.cs:5465                                               |
| SMSG_COOLDOWN_CHEAT                               | 481 (0x01E1)  | 10041 (0x2739) | YES     | HermesProxy/World/Client/WorldClient.cs:8267                                               |
| SMSG_COOLDOWN_EVENT                               | 309 (0x0135)  | 9913 (0x26B9)  | YES     | HermesProxy/World/Client/WorldClient.cs:8247                                               |
| SMSG_CORPSE_RECLAIM_DELAY                         | 617 (0x0269)  | 10058 (0x274A) | YES     | HermesProxy/World/Client/WorldClient.cs:4856                                               |
| SMSG_CREATE_CHAR                                  | 58 (0x003A)   | 9985 (0x2701)  | YES     | HermesProxy/World/Client/WorldClient.cs:957                                                |
| SMSG_CRITERIA_DELETED                             | 1182 (0x049E) | 9959 (0x26E7)  | NO      |                                                                                            |
| SMSG_CRITERIA_UPDATE                              | 1130 (0x046A) | 9953 (0x26E1)  | YES     | HermesProxy/World/Client/WorldClient.cs:5159                                               |
| SMSG_DEATH_RELEASE_LOC                            | 888 (0x0378)  | 9939 (0x26D3)  | YES     | HermesProxy/World/Client/WorldClient.cs:4847                                               |
| SMSG_DEFENSE_MESSAGE                              | 826 (0x033A)  | 11190 (0x2BB6) | YES     | HermesProxy/World/Client/WorldClient.cs:1959                                               |
| SMSG_DELETE_CHAR                                  | 60 (0x003C)   | 9986 (0x2702)  | YES     | HermesProxy/World/Client/WorldClient.cs:967                                                |
| SMSG_DESTROY_OBJECT                               | 170 (0x00AA)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:9068                                               |
| SMSG_DUEL_COMPLETE                                | 362 (0x016A)  | 10565 (0x2945) | YES     | HermesProxy/World/Client/WorldClient.cs:2221                                               |
| SMSG_DUEL_COUNTDOWN                               | 695 (0x02B7)  | 10564 (0x2944) | YES     | HermesProxy/World/Client/WorldClient.cs:2213                                               |
| SMSG_DUEL_IN_BOUNDS                               | 361 (0x0169)  | 10563 (0x2943) | YES     | HermesProxy/World/Client/WorldClient.cs:2241                                               |
| SMSG_DUEL_OUT_OF_BOUNDS                           | 360 (0x0168)  | 10562 (0x2942) | YES     | HermesProxy/World/Client/WorldClient.cs:2248                                               |
| SMSG_DUEL_REQUESTED                               | 359 (0x0167)  | 10560 (0x2940) | YES     | HermesProxy/World/Client/WorldClient.cs:2203                                               |
| SMSG_DUEL_WINNER                                  | 363 (0x016B)  | 10566 (0x2946) | YES     | HermesProxy/World/Client/WorldClient.cs:2229                                               |
| SMSG_DURABILITY_DAMAGE_DEATH                      | 701 (0x02BD)  | 10053 (0x2745) | YES     | HermesProxy/World/Client/WorldClient.cs:4232                                               |
| SMSG_EMOTE                                        | 259 (0x0103)  | 10185 (0x27C9) | YES     | HermesProxy/World/Client/WorldClient.cs:1919                                               |
| SMSG_ENCHANTMENT_LOG                              | 471 (0x01D7)  | 10003 (0x2713) | YES     | HermesProxy/World/Client/WorldClient.cs:4272                                               |
| SMSG_ENUM_CHARACTERS_RESULT                       | 59 (0x003B)   | 9603 (0x2583)  | YES     | HermesProxy/World/Client/WorldClient.cs:845                                                |
| SMSG_ENVIRONMENTAL_DAMAGE_LOG                     | 508 (0x01FC)  | 11294 (0x2C1E) | YES     | HermesProxy/World/Client/WorldClient.cs:8534                                               |
| SMSG_EXPECTED_SPAM_RECORDS                        | 818 (0x0332)  | 11185 (0x2BB1) | NO      |                                                                                            |
| SMSG_EXPLORATION_EXPERIENCE                       | 504 (0x01F8)  | 10079 (0x275F) | YES     | HermesProxy/World/Client/WorldClient.cs:4967                                               |
| SMSG_FEATURE_SYSTEM_STATUS                        | 969 (0x03C9)  | 9663 (0x25BF)  | YES     | HermesProxy/World/Client/WorldClient.cs:8859                                               |
| SMSG_FISH_ESCAPED                                 | 457 (0x01C9)  | 9936 (0x26D0)  | YES     | HermesProxy/World/Client/WorldClient.cs:2289                                               |
| SMSG_FISH_NOT_HOOKED                              | 456 (0x01C8)  | 9935 (0x26CF)  | YES     | HermesProxy/World/Client/WorldClient.cs:2282                                               |
| SMSG_FORCE_FLIGHT_SPEED_CHANGE                    | 897 (0x0381)  | 11764 (0x2DF4) | YES     | HermesProxy/World/Client/WorldClient.cs:5616                                               |
| SMSG_FORCE_RUN_BACK_SPEED_CHANGE                  | 228 (0x00E4)  | 11761 (0x2DF1) | YES     | HermesProxy/World/Client/WorldClient.cs:5612                                               |
| SMSG_FORCE_RUN_SPEED_CHANGE                       | 226 (0x00E2)  | 11760 (0x2DF0) | YES     | HermesProxy/World/Client/WorldClient.cs:5611                                               |
| SMSG_FORCE_SWIM_SPEED_CHANGE                      | 230 (0x00E6)  | 11762 (0x2DF2) | YES     | HermesProxy/World/Client/WorldClient.cs:5613                                               |
| SMSG_FORCE_TURN_RATE_CHANGE                       | 734 (0x02DE)  | 11767 (0x2DF7) | YES     | HermesProxy/World/Client/WorldClient.cs:5615                                               |
| SMSG_FORCE_WALK_SPEED_CHANGE                      | 730 (0x02DA)  | 11766 (0x2DF6) | YES     | HermesProxy/World/Client/WorldClient.cs:5610                                               |
| SMSG_FRIEND_STATUS                                | 104 (0x0068)  | 10125 (0x278D) | YES     | HermesProxy/World/Client/WorldClient.cs:7586                                               |
| SMSG_GAME_OBJECT_CUSTOM_ANIM                      | 179 (0x00B3)  | 9668 (0x25C4)  | YES     | HermesProxy/World/Client/WorldClient.cs:2273                                               |
| SMSG_GAME_OBJECT_DESPAWN                          | 533 (0x0215)  | 9669 (0x25C5)  | YES     | HermesProxy/World/Client/WorldClient.cs:2255                                               |
| SMSG_GAME_OBJECT_RESET_STATE                      | 679 (0x02A7)  | 10014 (0x271E) | YES     | HermesProxy/World/Client/WorldClient.cs:2265                                               |
| SMSG_GM_MESSAGECHAT                               | 947 (0x03B3)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:1696                                               |
| SMSG_GM_TICKET_STATUS_UPDATE                      | 808 (0x0328)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_GOSSIP_COMPLETE                              | 382 (0x017E)  | 10903 (0x2A97) | YES     | HermesProxy/World/Client/WorldClient.cs:5961                                               |
| SMSG_GOSSIP_MESSAGE                               | 381 (0x017D)  | 10904 (0x2A98) | YES     | HermesProxy/World/Client/WorldClient.cs:5919                                               |
| SMSG_GOSSIP_POI                                   | 548 (0x0224)  | 10136 (0x2798) | YES     | HermesProxy/World/Client/WorldClient.cs:5968                                               |
| SMSG_GROUP_DECLINE                                | 116 (0x0074)  | 10129 (0x2791) | YES     | HermesProxy/World/Client/WorldClient.cs:2320                                               |
| SMSG_GROUP_DESTROYED                              | 124 (0x007C)  | 10132 (0x2794) | NO      |                                                                                            |
| SMSG_GROUP_NEW_LEADER                             | 121 (0x0079)  | 9773 (0x262D)  | YES     | HermesProxy/World/Client/WorldClient.cs:2601                                               |
| SMSG_GROUP_UNINVITE                               | 119 (0x0077)  | 10131 (0x2793) | YES     | HermesProxy/World/Client/WorldClient.cs:2594                                               |
| SMSG_GUILD_BANK_QUERY_RESULTS                     | 1000 (0x03E8) | 10719 (0x29DF) | YES     | HermesProxy/World/Client/WorldClient.cs:3821                                               |
| SMSG_GUILD_COMMAND_RESULT                         | 147 (0x0093)  | 10682 (0x29BA) | YES     | HermesProxy/World/Client/WorldClient.cs:3477                                               |
| SMSG_GUILD_INVITE                                 | 131 (0x0083)  | 10699 (0x29CB) | YES     | HermesProxy/World/Client/WorldClient.cs:3784                                               |
| SMSG_GUILD_INVITE_DECLINED                        | 134 (0x0086)  | 10729 (0x29E9) | YES     | HermesProxy/World/Client/WorldClient.cs:3812                                               |
| SMSG_GUILD_ROSTER                                 | 138 (0x008A)  | 10683 (0x29BB) | YES     | HermesProxy/World/Client/WorldClient.cs:3705                                               |
| SMSG_HEALTH_UPDATE                                | 1151 (0x047F) | 9937 (0x26D1)  | NO      |                                                                                            |
| SMSG_HIGHEST_THREAT_UPDATE                        | 1154 (0x0482) | 9945 (0x26D9)  | YES     | HermesProxy/World/Client/WorldClient.cs:2043                                               |
| SMSG_INITIALIZE_FACTIONS                          | 290 (0x0122)  | 10020 (0x2724) | YES     | HermesProxy/World/Client/WorldClient.cs:7436                                               |
| SMSG_INIT_WORLD_STATES                            | 706 (0x02C2)  | 10054 (0x2746) | YES     | HermesProxy/World/Client/WorldClient.cs:12186                                              |
| SMSG_INSPECT_RESULT                               | 277 (0x0115)  | 9777 (0x2631)  | YES     | HermesProxy/World/Client/WorldClient.cs:1283                                               |
| SMSG_INSTANCE_LOCK_WARNING_QUERY                  | 327 (0x0147)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_INSTANCE_RESET                               | 798 (0x031E)  | 9862 (0x2686)  | YES     | HermesProxy/World/Client/WorldClient.cs:3951                                               |
| SMSG_INSTANCE_RESET_FAILED                        | 799 (0x031F)  | 9863 (0x2687)  | YES     | HermesProxy/World/Client/WorldClient.cs:3959                                               |
| SMSG_INSTANCE_SAVE_CREATED                        | 715 (0x02CB)  | 10112 (0x2780) | YES     | HermesProxy/World/Client/WorldClient.cs:4018                                               |
| SMSG_INVALIDATE_PLAYER                            | 796 (0x031C)  | 12287 (0x2FFF) | YES     | HermesProxy/World/Client/WorldClient.cs:5049                                               |
| SMSG_INVENTORY_CHANGE_FAILURE                     | 274 (0x0112)  | 11685 (0x2DA5) | YES     | HermesProxy/World/Client/WorldClient.cs:4171; HermesProxy/World/Client/WorldClient.cs:4196 |
| SMSG_ITEM_COOLDOWN                                | 176 (0x00B0)  | 10184 (0x27C8) | YES     | HermesProxy/World/Client/WorldClient.cs:4240                                               |
| SMSG_ITEM_ENCHANT_TIME_UPDATE                     | 491 (0x01EB)  | 10069 (0x2755) | YES     | HermesProxy/World/Client/WorldClient.cs:4261                                               |
| SMSG_ITEM_NAME_QUERY_RESPONSE                     | 709 (0x02C5)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:6934                                               |
| SMSG_ITEM_PUSH_RESULT                             | 358 (0x0166)  | 9763 (0x2623)  | YES     | HermesProxy/World/Client/WorldClient.cs:4082                                               |
| SMSG_ITEM_QUERY_SINGLE_RESPONSE                   | 88 (0x0058)   | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:6827                                               |
| SMSG_LEVEL_UP_INFO                                | 468 (0x01D4)  | 9961 (0x26E9)  | YES     | HermesProxy/World/Client/WorldClient.cs:1250                                               |
| SMSG_LFG_DISABLED                                 | 920 (0x0398)  | 10803 (0x2A33) | NO      |                                                                                            |
| SMSG_LFG_JOIN_RESULT                              | 868 (0x0364)  | 10780 (0x2A1C) | YES     | HermesProxy/World/Client/WorldClient.cs:5267                                               |
| SMSG_LFG_OFFER_CONTINUE                           | 659 (0x0293)  | 10804 (0x2A34) | NO      |                                                                                            |
| SMSG_LFG_PARTY_INFO                               | 882 (0x0372)  | 10806 (0x2A36) | NO      |                                                                                            |
| SMSG_LFG_PLAYER_INFO                              | 879 (0x036F)  | 10807 (0x2A37) | YES     | HermesProxy/World/Client/WorldClient.cs:5210                                               |
| SMSG_LFG_PLAYER_REWARD                            | 511 (0x01FF)  | 10808 (0x2A38) | NO      |                                                                                            |
| SMSG_LFG_PROPOSAL_UPDATE                          | 865 (0x0361)  | 10797 (0x2A2D) | YES     | HermesProxy/World/Client/WorldClient.cs:5348                                               |
| SMSG_LFG_QUEUE_STATUS                             | 869 (0x0365)  | 10784 (0x2A20) | YES     | HermesProxy/World/Client/WorldClient.cs:5327                                               |
| SMSG_LFG_ROLE_CHECK_UPDATE                        | 867 (0x0363)  | 10785 (0x2A21) | NO      |                                                                                            |
| SMSG_LFG_UPDATE_PLAYER                            | 871 (0x0367)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:5298                                               |
| SMSG_LFG_UPDATE_SEARCH                            | 873 (0x0369)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_LOAD_EQUIPMENT_SET                           | 1212 (0x04BC) | 9998 (0x270E)  | YES     | HermesProxy/World/Client/WorldClient.cs:5200                                               |
| SMSG_LOGIN_SET_TIME_SPEED                         | 66 (0x0042)   | 9997 (0x270D)  | YES     | HermesProxy/World/Client/WorldClient.cs:4897                                               |
| SMSG_LOGIN_VERIFY_WORLD                           | 566 (0x0236)  | 9623 (0x2597)  | YES     | HermesProxy/World/Client/WorldClient.cs:1077                                               |
| SMSG_LOGOUT_CANCEL_ACK                            | 79 (0x004F)   | 9861 (0x2685)  | YES     | HermesProxy/World/Client/WorldClient.cs:1207                                               |
| SMSG_LOGOUT_COMPLETE                              | 77 (0x004D)   | 9860 (0x2684)  | YES     | HermesProxy/World/Client/WorldClient.cs:1197                                               |
| SMSG_LOGOUT_RESPONSE                              | 76 (0x004C)   | 9859 (0x2683)  | YES     | HermesProxy/World/Client/WorldClient.cs:1188                                               |
| SMSG_LOG_XP_GAIN                                  | 464 (0x01D0)  | 9957 (0x26E5)  | YES     | HermesProxy/World/Client/WorldClient.cs:1214                                               |
| SMSG_LOOT_ALL_PASSED                              | 670 (0x029E)  | 9761 (0x2621)  | YES     | HermesProxy/World/Client/WorldClient.cs:4502                                               |
| SMSG_LOOT_CLEAR_MONEY                             | 357 (0x0165)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:4387                                               |
| SMSG_LOOT_LIST                                    | 1017 (0x03F9) | 10049 (0x2741) | YES     | HermesProxy/World/Client/WorldClient.cs:4303                                               |
| SMSG_LOOT_MASTER_LIST                             | 676 (0x02A4)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:4519                                               |
| SMSG_LOOT_MONEY_NOTIFY                            | 355 (0x0163)  | 9756 (0x261C)  | YES     | HermesProxy/World/Client/WorldClient.cs:4375                                               |
| SMSG_LOOT_RELEASE                                 | 353 (0x0161)  | 9755 (0x261B)  | YES     | HermesProxy/World/Client/WorldClient.cs:4354                                               |
| SMSG_LOOT_REMOVED                                 | 354 (0x0162)  | 9749 (0x2615)  | YES     | HermesProxy/World/Client/WorldClient.cs:4365                                               |
| SMSG_LOOT_RESPONSE                                | 352 (0x0160)  | 9748 (0x2614)  | YES     | HermesProxy/World/Client/WorldClient.cs:4322                                               |
| SMSG_LOOT_ROLL                                    | 674 (0x02A2)  | 9758 (0x261E)  | YES     | HermesProxy/World/Client/WorldClient.cs:4441                                               |
| SMSG_LOOT_ROLL_WON                                | 671 (0x029F)  | 9762 (0x2622)  | YES     | HermesProxy/World/Client/WorldClient.cs:4478                                               |
| SMSG_LOOT_START_ROLL                              | 673 (0x02A1)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:4395                                               |
| SMSG_MAIL_COMMAND_RESULT                          | 569 (0x0239)  | 9787 (0x263B)  | YES     | HermesProxy/World/Client/WorldClient.cs:4774                                               |
| SMSG_MAIL_LIST_RESULT                             | 571 (0x023B)  | 10070 (0x2756) | YES     | HermesProxy/World/Client/WorldClient.cs:4584                                               |
| SMSG_MONSTER_MOVE_TRANSPORT                       | 686 (0x02AE)  | 11732 (0x2DD4) | YES     | HermesProxy/World/Client/WorldClient.cs:5741                                               |
| SMSG_MOTD                                         | 829 (0x033D)  | 11183 (0x2BAF) | YES     | HermesProxy/World/Client/WorldClient.cs:8865                                               |
| SMSG_MOUNT_RESULT                                 | 366 (0x016E)  | 9595 (0x257B)  | NO      |                                                                                            |
| SMSG_MOVE_DISABLE_GRAVITY                         | 1230 (0x04CE) | 11789 (0x2E0D) | YES     | HermesProxy/World/Client/WorldClient.cs:5712                                               |
| SMSG_MOVE_DISABLE_TRANSITION_BETWEEN_SWIM_AND_FLY | 831 (0x033F)  | 11788 (0x2E0C) | YES     | HermesProxy/World/Client/WorldClient.cs:5711                                               |
| SMSG_MOVE_ENABLE_GRAVITY                          | 1232 (0x04D0) | 11790 (0x2E0E) | YES     | HermesProxy/World/Client/WorldClient.cs:5713                                               |
| SMSG_MOVE_ENABLE_TRANSITION_BETWEEN_SWIM_AND_FLY  | 830 (0x033E)  | 11787 (0x2E0B) | YES     | HermesProxy/World/Client/WorldClient.cs:5710                                               |
| SMSG_MOVE_KNOCK_BACK                              | 239 (0x00EF)  | 11779 (0x2E03) | YES     | HermesProxy/World/Client/WorldClient.cs:5453                                               |
| SMSG_MOVE_ROOT                                    | 232 (0x00E8)  | 11769 (0x2DF9) | YES     | HermesProxy/World/Client/WorldClient.cs:5702                                               |
| SMSG_MOVE_SET_CAN_FLY                             | 835 (0x0343)  | 11781 (0x2E05) | YES     | HermesProxy/World/Client/WorldClient.cs:5708                                               |
| SMSG_MOVE_SET_FEATHER_FALL                        | 242 (0x00F2)  | 11775 (0x2DFF) | YES     | HermesProxy/World/Client/WorldClient.cs:5714                                               |
| SMSG_MOVE_SET_HOVERING                            | 244 (0x00F4)  | 11777 (0x2E01) | YES     | HermesProxy/World/Client/WorldClient.cs:5706                                               |
| SMSG_MOVE_SET_LAND_WALK                           | 223 (0x00DF)  | 11774 (0x2DFE) | YES     | HermesProxy/World/Client/WorldClient.cs:5705                                               |
| SMSG_MOVE_SET_NORMAL_FALL                         | 243 (0x00F3)  | 11776 (0x2E00) | YES     | HermesProxy/World/Client/WorldClient.cs:5715                                               |
| SMSG_MOVE_SET_WATER_WALK                          | 222 (0x00DE)  | 11771 (0x2DFB) | YES     | HermesProxy/World/Client/WorldClient.cs:5704                                               |
| SMSG_MOVE_SPLINE_DISABLE_GRAVITY                  | 1235 (0x04D3) | 11803 (0x2E1B) | YES     | HermesProxy/World/Client/WorldClient.cs:5682                                               |
| SMSG_MOVE_SPLINE_ENABLE_GRAVITY                   | 1236 (0x04D4) | 11804 (0x2E1C) | YES     | HermesProxy/World/Client/WorldClient.cs:5681                                               |
| SMSG_MOVE_SPLINE_ROOT                             | 794 (0x031A)  | 11801 (0x2E19) | YES     | HermesProxy/World/Client/WorldClient.cs:5679                                               |
| SMSG_MOVE_SPLINE_SET_FEATHER_FALL                 | 773 (0x0305)  | 11807 (0x2E1F) | YES     | HermesProxy/World/Client/WorldClient.cs:5683                                               |
| SMSG_MOVE_SPLINE_SET_FLIGHT_BACK_SPEED            | 902 (0x0386)  | 11756 (0x2DEC) | YES     | HermesProxy/World/Client/WorldClient.cs:5592                                               |
| SMSG_MOVE_SPLINE_SET_FLIGHT_SPEED                 | 901 (0x0385)  | 11755 (0x2DEB) | YES     | HermesProxy/World/Client/WorldClient.cs:5593                                               |
| SMSG_MOVE_SPLINE_SET_FLYING                       | 1058 (0x0422) | 11817 (0x2E29) | YES     | HermesProxy/World/Client/WorldClient.cs:5693                                               |
| SMSG_MOVE_SPLINE_SET_HOVER                        | 775 (0x0307)  | 11809 (0x2E21) | YES     | HermesProxy/World/Client/WorldClient.cs:5685                                               |
| SMSG_MOVE_SPLINE_SET_LAND_WALK                    | 778 (0x030A)  | 11812 (0x2E24) | YES     | HermesProxy/World/Client/WorldClient.cs:5688                                               |
| SMSG_MOVE_SPLINE_SET_NORMAL_FALL                  | 774 (0x0306)  | 11808 (0x2E20) | YES     | HermesProxy/World/Client/WorldClient.cs:5684                                               |
| SMSG_MOVE_SPLINE_SET_PITCH_RATE                   | 1118 (0x045E) | 11759 (0x2DEF) | YES     | HermesProxy/World/Client/WorldClient.cs:5594                                               |
| SMSG_MOVE_SPLINE_SET_RUN_BACK_SPEED               | 767 (0x02FF)  | 11752 (0x2DE8) | YES     | HermesProxy/World/Client/WorldClient.cs:5595                                               |
| SMSG_MOVE_SPLINE_SET_RUN_MODE                     | 781 (0x030D)  | 11815 (0x2E27) | YES     | HermesProxy/World/Client/WorldClient.cs:5691                                               |
| SMSG_MOVE_SPLINE_SET_RUN_SPEED                    | 766 (0x02FE)  | 11751 (0x2DE7) | YES     | HermesProxy/World/Client/WorldClient.cs:5596                                               |
| SMSG_MOVE_SPLINE_SET_SWIM_BACK_SPEED              | 770 (0x0302)  | 11754 (0x2DEA) | YES     | HermesProxy/World/Client/WorldClient.cs:5597                                               |
| SMSG_MOVE_SPLINE_SET_SWIM_SPEED                   | 768 (0x0300)  | 11753 (0x2DE9) | YES     | HermesProxy/World/Client/WorldClient.cs:5598                                               |
| SMSG_MOVE_SPLINE_SET_TURN_RATE                    | 771 (0x0303)  | 11758 (0x2DEE) | YES     | HermesProxy/World/Client/WorldClient.cs:5599                                               |
| SMSG_MOVE_SPLINE_SET_WALK_MODE                    | 782 (0x030E)  | 11816 (0x2E28) | YES     | HermesProxy/World/Client/WorldClient.cs:5692                                               |
| SMSG_MOVE_SPLINE_SET_WATER_WALK                   | 777 (0x0309)  | 11811 (0x2E23) | YES     | HermesProxy/World/Client/WorldClient.cs:5687                                               |
| SMSG_MOVE_SPLINE_START_SWIM                       | 779 (0x030B)  | 11813 (0x2E25) | YES     | HermesProxy/World/Client/WorldClient.cs:5689                                               |
| SMSG_MOVE_SPLINE_STOP_SWIM                        | 780 (0x030C)  | 11814 (0x2E26) | YES     | HermesProxy/World/Client/WorldClient.cs:5690                                               |
| SMSG_MOVE_SPLINE_UNROOT                           | 772 (0x0304)  | 11802 (0x2E1A) | YES     | HermesProxy/World/Client/WorldClient.cs:5680                                               |
| SMSG_MOVE_SPLINE_UNSET_FLYING                     | 1059 (0x0423) | 11818 (0x2E2A) | YES     | HermesProxy/World/Client/WorldClient.cs:5694                                               |
| SMSG_MOVE_SPLINE_UNSET_HOVER                      | 776 (0x0308)  | 11810 (0x2E22) | YES     | HermesProxy/World/Client/WorldClient.cs:5686                                               |
| SMSG_MOVE_UNROOT                                  | 234 (0x00EA)  | 11770 (0x2DFA) | YES     | HermesProxy/World/Client/WorldClient.cs:5703                                               |
| SMSG_MOVE_UNSET_CAN_FLY                           | 836 (0x0344)  | 11782 (0x2E06) | YES     | HermesProxy/World/Client/WorldClient.cs:5709                                               |
| SMSG_MOVE_UNSET_HOVERING                          | 245 (0x00F5)  | 11778 (0x2E02) | YES     | HermesProxy/World/Client/WorldClient.cs:5707                                               |
| SMSG_NEW_TAXI_PATH                                | 431 (0x01AF)  | 9854 (0x267E)  | YES     | HermesProxy/World/Client/WorldClient.cs:8919                                               |
| SMSG_NEW_WORLD                                    | 62 (0x003E)   | 9620 (0x2594)  | YES     | HermesProxy/World/Client/WorldClient.cs:5553                                               |
| SMSG_NOTIFY_RECEIVED_MAIL                         | 645 (0x0285)  | 9788 (0x263C)  | YES     | HermesProxy/World/Client/WorldClient.cs:4541                                               |
| SMSG_ON_CANCEL_EXPECTED_RIDE_VEHICLE_AURA         | 1181 (0x049D) | 9958 (0x26E6)  | NO      |                                                                                            |
| SMSG_ON_MONSTER_MOVE                              | 221 (0x00DD)  | 11732 (0x2DD4) | YES     | HermesProxy/World/Client/WorldClient.cs:5740                                               |
| SMSG_OVERRIDE_LIGHT                               | 1042 (0x0412) | 9915 (0x26BB)  | NO      |                                                                                            |
| SMSG_PARTY_COMMAND_RESULT                         | 127 (0x007F)  | 10134 (0x2796) | YES     | HermesProxy/World/Client/WorldClient.cs:2296                                               |
| SMSG_PARTY_INVITE                                 | 111 (0x006F)  | 9661 (0x25BD)  | YES     | HermesProxy/World/Client/WorldClient.cs:2328                                               |
| SMSG_PARTY_KILL_LOG                               | 501 (0x01F5)  | 10074 (0x275A) | YES     | HermesProxy/World/Client/WorldClient.cs:2194                                               |
| SMSG_PARTY_MEMBER_FULL_STATE                      | 754 (0x02F2)  | 10073 (0x2759) | YES     | HermesProxy/World/Client/WorldClient.cs:3078; HermesProxy/World/Client/WorldClient.cs:3292 |
| SMSG_PARTY_MEMBER_PARTIAL_STATE                   | 126 (0x007E)  | 10072 (0x2758) | YES     | HermesProxy/World/Client/WorldClient.cs:2718; HermesProxy/World/Client/WorldClient.cs:2923 |
| SMSG_PAUSE_MIRROR_TIMER                           | 474 (0x01DA)  | 10000 (0x2710) | YES     | HermesProxy/World/Client/WorldClient.cs:5032                                               |
| SMSG_PETITION_SHOW_LIST                           | 444 (0x01BC)  | 9919 (0x26BF)  | YES     | HermesProxy/World/Client/WorldClient.cs:6244                                               |
| SMSG_PETITION_SHOW_SIGNATURES                     | 447 (0x01BF)  | 9920 (0x26C0)  | YES     | HermesProxy/World/Client/WorldClient.cs:6273                                               |
| SMSG_PETITION_SIGN_RESULTS                        | 449 (0x01C1)  | 10060 (0x274C) | YES     | HermesProxy/World/Client/WorldClient.cs:6356                                               |
| SMSG_PET_ACTION_FEEDBACK                          | 710 (0x02C6)  | 10057 (0x2749) | NO      |                                                                                            |
| SMSG_PET_ACTION_SOUND                             | 804 (0x0324)  | 9888 (0x26A0)  | YES     | HermesProxy/World/Client/WorldClient.cs:6157                                               |
| SMSG_PET_BROKEN                                   | 687 (0x02AF)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:6166                                               |
| SMSG_PET_CAST_FAILED                              | 312 (0x0138)  | 11349 (0x2C55) | YES     | HermesProxy/World/Client/WorldClient.cs:7801; HermesProxy/World/Client/WorldClient.cs:7834 |
| SMSG_PET_GUIDS                                    | 1194 (0x04AA) | 9988 (0x2704)  | NO      |                                                                                            |
| SMSG_PET_MODE                                     | 378 (0x017A)  | 9608 (0x2588)  | NO      |                                                                                            |
| SMSG_PET_SPELLS_MESSAGE                           | 377 (0x0179)  | 11298 (0x2C22) | YES     | HermesProxy/World/Client/WorldClient.cs:6105                                               |
| SMSG_PET_STABLE_RESULT                            | 627 (0x0273)  | 9619 (0x2593)  | YES     | HermesProxy/World/Client/WorldClient.cs:6236                                               |
| SMSG_PET_TAME_FAILURE                             | 371 (0x0173)  | 9907 (0x26B3)  | NO      |                                                                                            |
| SMSG_PET_UNLEARN_CONFIRM                          | 753 (0x02F1)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:6174                                               |
| SMSG_PHASE_SHIFT_CHANGE                           | 1148 (0x047C) | 9592 (0x2578)  | NO      |                                                                                            |
| SMSG_PLAYED_TIME                                  | 461 (0x01CD)  | 9941 (0x26D5)  | YES     | HermesProxy/World/Client/WorldClient.cs:1233                                               |
| SMSG_PLAYER_BOUND                                 | 344 (0x0158)  | 12280 (0x2FF8) | YES     | HermesProxy/World/Client/WorldClient.cs:4838                                               |
| SMSG_PLAYER_SKINNED                               | 700 (0x02BC)  | 12294 (0x3006) | YES     | HermesProxy/World/Client/WorldClient.cs:834                                                |
| SMSG_PLAY_MUSIC                                   | 631 (0x0277)  | 10093 (0x276D) | YES     | HermesProxy/World/Client/WorldClient.cs:4976                                               |
| SMSG_PLAY_OBJECT_SOUND                            | 632 (0x0278)  | 10094 (0x276E) | YES     | HermesProxy/World/Client/WorldClient.cs:4993                                               |
| SMSG_PLAY_SOUND                                   | 722 (0x02D2)  | 10092 (0x276C) | YES     | HermesProxy/World/Client/WorldClient.cs:4984                                               |
| SMSG_PLAY_SPELL_VISUAL                            | 499 (0x01F3)  | 11330 (0x2C42) | YES     | HermesProxy/World/Client/WorldClient.cs:8599                                               |
| SMSG_PLAY_TIME_WARNING                            | 757 (0x02F5)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_PONG                                         | 477 (0x01DD)  | 12366 (0x304E) | YES     | HermesProxy/World/Client/WorldClient.cs:4797                                               |
| SMSG_POWER_UPDATE                                 | 1152 (0x0480) | 9938 (0x26D2)  | YES     | HermesProxy/World/Client/WorldClient.cs:5080                                               |
| SMSG_PRINT_NOTIFICATION                           | 459 (0x01CB)  | 9674 (0x25CA)  | YES     | HermesProxy/World/Client/WorldClient.cs:1943                                               |
| SMSG_PVP_CREDIT                                   | 652 (0x028C)  | 10570 (0x294A) | YES     | HermesProxy/World/Client/WorldClient.cs:824                                                |
| SMSG_QUERY_CREATURE_RESPONSE                      | 97 (0x0061)   | 10516 (0x2914) | YES     | HermesProxy/World/Client/WorldClient.cs:6631                                               |
| SMSG_QUERY_GAME_OBJECT_RESPONSE                   | 95 (0x005F)   | 10517 (0x2915) | YES     | HermesProxy/World/Client/WorldClient.cs:6718                                               |
| SMSG_QUERY_GUILD_INFO_RESPONSE                    | 85 (0x0055)   | 10726 (0x29E6) | YES     | HermesProxy/World/Client/WorldClient.cs:3640                                               |
| SMSG_QUERY_ITEM_TEXT_RESPONSE                     | 580 (0x0244)  | 10526 (0x291E) | YES     | HermesProxy/World/Client/WorldClient.cs:4697                                               |
| SMSG_QUERY_NPC_TEXT_RESPONSE                      | 384 (0x0180)  | 10518 (0x2916) | YES     | HermesProxy/World/Client/WorldClient.cs:6789                                               |
| SMSG_QUERY_PAGE_TEXT_RESPONSE                     | 91 (0x005B)   | 10519 (0x2917) | YES     | HermesProxy/World/Client/WorldClient.cs:6773                                               |
| SMSG_QUERY_PETITION_RESPONSE                      | 455 (0x01C7)  | 10523 (0x291B) | YES     | HermesProxy/World/Client/WorldClient.cs:6294                                               |
| SMSG_QUERY_PET_NAME_RESPONSE                      | 83 (0x0053)   | 10521 (0x2919) | YES     | HermesProxy/World/Client/WorldClient.cs:6903                                               |
| SMSG_QUERY_QUEST_INFO_RESPONSE                    | 93 (0x005D)   | 10902 (0x2A96) | YES     | HermesProxy/World/Client/WorldClient.cs:6386                                               |
| SMSG_QUERY_TIME_RESPONSE                          | 463 (0x01CF)  | 9956 (0x26E4)  | YES     | HermesProxy/World/Client/WorldClient.cs:6374                                               |
| SMSG_QUEST_CONFIRM_ACCEPT                         | 412 (0x019C)  | 10895 (0x2A8F) | YES     | HermesProxy/World/Client/WorldClient.cs:7417                                               |
| SMSG_QUEST_GIVER_INVALID_QUEST                    | 399 (0x018F)  | 10885 (0x2A85) | YES     | HermesProxy/World/Client/WorldClient.cs:7361                                               |
| SMSG_QUEST_GIVER_OFFER_REWARD_MESSAGE             | 397 (0x018D)  | 10900 (0x2A94) | YES     | HermesProxy/World/Client/WorldClient.cs:7244                                               |
| SMSG_QUEST_GIVER_QUEST_COMPLETE                   | 401 (0x0191)  | 10883 (0x2A83) | YES     | HermesProxy/World/Client/WorldClient.cs:7284                                               |
| SMSG_QUEST_GIVER_QUEST_DETAILS                    | 392 (0x0188)  | 10898 (0x2A92) | YES     | HermesProxy/World/Client/WorldClient.cs:6992                                               |
| SMSG_QUEST_GIVER_QUEST_FAILED                     | 402 (0x0192)  | 10886 (0x2A86) | YES     | HermesProxy/World/Client/WorldClient.cs:7352                                               |
| SMSG_QUEST_GIVER_QUEST_LIST_MESSAGE               | 389 (0x0185)  | 10906 (0x2A9A) | YES     | HermesProxy/World/Client/WorldClient.cs:7150                                               |
| SMSG_QUEST_GIVER_REQUEST_ITEMS                    | 395 (0x018B)  | 10899 (0x2A93) | YES     | HermesProxy/World/Client/WorldClient.cs:7189                                               |
| SMSG_QUEST_GIVER_STATUS                           | 387 (0x0183)  | 10907 (0x2A9B) | YES     | HermesProxy/World/Client/WorldClient.cs:7126                                               |
| SMSG_QUEST_GIVER_STATUS_MULTIPLE                  | 1048 (0x0418) | 10897 (0x2A91) | YES     | HermesProxy/World/Client/WorldClient.cs:7135                                               |
| SMSG_QUEST_POI_QUERY_RESPONSE                     | 484 (0x01E4)  | 10909 (0x2A9D) | NO      |                                                                                            |
| SMSG_QUEST_UPDATE_ADD_ITEM                        | 410 (0x019A)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:7379                                               |
| SMSG_QUEST_UPDATE_COMPLETE                        | 408 (0x0198)  | 10889 (0x2A89) | YES     | HermesProxy/World/Client/WorldClient.cs:7369                                               |
| SMSG_QUEST_UPDATE_FAILED                          | 406 (0x0196)  | 10890 (0x2A8A) | YES     | HermesProxy/World/Client/WorldClient.cs:7370                                               |
| SMSG_QUEST_UPDATE_FAILED_TIMER                    | 407 (0x0197)  | 10891 (0x2A8B) | YES     | HermesProxy/World/Client/WorldClient.cs:7371                                               |
| SMSG_RAID_GROUP_ONLY                              | 646 (0x0286)  | 10159 (0x27AF) | YES     | HermesProxy/World/Client/WorldClient.cs:4026                                               |
| SMSG_RAID_INSTANCE_MESSAGE                        | 762 (0x02FA)  | 11188 (0x2BB4) | YES     | HermesProxy/World/Client/WorldClient.cs:4035                                               |
| SMSG_READ_ITEM_RESULT_FAILED                      | 175 (0x00AF)  | 10153 (0x27A9) | YES     | HermesProxy/World/Client/WorldClient.cs:4152                                               |
| SMSG_READ_ITEM_RESULT_OK                          | 174 (0x00AE)  | 10145 (0x27A1) | YES     | HermesProxy/World/Client/WorldClient.cs:4144                                               |
| SMSG_REAL_GROUP_UPDATE                            | 919 (0x0397)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_RESET_FAILED_NOTIFY                          | 918 (0x0396)  | 9911 (0x26B7)  | YES     | HermesProxy/World/Client/WorldClient.cs:3968                                               |
| SMSG_RESUME_COMMS                                 | 1297 (0x0511) | 12363 (0x304B) | NO      |                                                                                            |
| SMSG_RESURRECT_REQUEST                            | 347 (0x015B)  | 9598 (0x257E)  | YES     | HermesProxy/World/Client/WorldClient.cs:8795                                               |
| SMSG_SELL_RESPONSE                                | 417 (0x01A1)  | 9925 (0x26C5)  | YES     | HermesProxy/World/Client/WorldClient.cs:4250                                               |
| SMSG_SEND_KNOWN_SPELLS                            | 298 (0x012A)  | 11303 (0x2C27) | YES     | HermesProxy/World/Client/WorldClient.cs:7630                                               |
| SMSG_SEND_UNLEARN_SPELLS                          | 1054 (0x041E) | 11307 (0x2C2B) | YES     | HermesProxy/World/Client/WorldClient.cs:7702                                               |
| SMSG_SET_FACTION_STANDING                         | 292 (0x0124)  | 10028 (0x272C) | YES     | HermesProxy/World/Client/WorldClient.cs:7456                                               |
| SMSG_SET_FACTION_VISIBLE                          | 291 (0x0123)  | 10026 (0x272A) | YES     | HermesProxy/World/Client/WorldClient.cs:7500                                               |
| SMSG_SET_FLAT_SPELL_MODIFIER                      | 614 (0x0266)  | 11315 (0x2C33) | YES     | HermesProxy/World/Client/WorldClient.cs:8819                                               |
| SMSG_SET_FORCED_REACTIONS                         | 677 (0x02A5)  | 10013 (0x271D) | YES     | HermesProxy/World/Client/WorldClient.cs:7483                                               |
| SMSG_SET_PCT_SPELL_MODIFIER                       | 615 (0x0267)  | 11316 (0x2C34) | YES     | HermesProxy/World/Client/WorldClient.cs:8820                                               |
| SMSG_SET_PLAYER_DECLINED_NAMES_RESULT             | 1050 (0x041A) | 12291 (0x3003) | NO      |                                                                                            |
| SMSG_SET_PROFICIENCY                              | 295 (0x0127)  | 10037 (0x2735) | YES     | HermesProxy/World/Client/WorldClient.cs:4062                                               |
| SMSG_SHOW_TAXI_NODES                              | 425 (0x01A9)  | 9933 (0x26CD)  | YES     | HermesProxy/World/Client/WorldClient.cs:8892                                               |
| SMSG_SPECIAL_MOUNT_ANIM                           | 370 (0x0172)  | 9887 (0x269F)  | YES     | HermesProxy/World/Client/WorldClient.cs:5011                                               |
| SMSG_SPELL_COOLDOWN                               | 308 (0x0134)  | 11285 (0x2C15) | YES     | HermesProxy/World/Client/WorldClient.cs:8216                                               |
| SMSG_SPELL_DAMAGE_SHIELD                          | 591 (0x024F)  | 11310 (0x2C2E) | YES     | HermesProxy/World/Client/WorldClient.cs:8505                                               |
| SMSG_SPELL_DELAYED                                | 482 (0x01E2)  | 11324 (0x2C3C) | YES     | HermesProxy/World/Client/WorldClient.cs:8455                                               |
| SMSG_SPELL_DISPELL_LOG                            | 635 (0x027B)  | 11287 (0x2C17) | YES     | HermesProxy/World/Client/WorldClient.cs:8563                                               |
| SMSG_SPELL_ENERGIZE_LOG                           | 337 (0x0151)  | 11289 (0x2C19) | YES     | HermesProxy/World/Client/WorldClient.cs:8443                                               |
| SMSG_SPELL_FAILED_OTHER                           | 678 (0x02A6)  | 11346 (0x2C52) | YES     | HermesProxy/World/Client/WorldClient.cs:7884                                               |
| SMSG_SPELL_FAILURE                                | 307 (0x0133)  | 11344 (0x2C50) | YES     | HermesProxy/World/Client/WorldClient.cs:7878                                               |
| SMSG_SPELL_GO                                     | 306 (0x0132)  | 11318 (0x2C36) | YES     | HermesProxy/World/Client/WorldClient.cs:7991                                               |
| SMSG_SPELL_HEAL_LOG                               | 336 (0x0150)  | 11290 (0x2C1A) | YES     | HermesProxy/World/Client/WorldClient.cs:8331                                               |
| SMSG_SPELL_INSTAKILL_LOG                          | 815 (0x032F)  | 11312 (0x2C30) | YES     | HermesProxy/World/Client/WorldClient.cs:8546                                               |
| SMSG_SPELL_NON_MELEE_DAMAGE_LOG                   | 592 (0x0250)  | 11311 (0x2C2F) | YES     | HermesProxy/World/Client/WorldClient.cs:8275                                               |
| SMSG_SPELL_PERIODIC_AURA_LOG                      | 590 (0x024E)  | 11288 (0x2C18) | YES     | HermesProxy/World/Client/WorldClient.cs:8357                                               |
| SMSG_SPELL_START                                  | 305 (0x0131)  | 11319 (0x2C37) | YES     | HermesProxy/World/Client/WorldClient.cs:7935                                               |
| SMSG_SPELL_UPDATE_CHAIN_TARGETS                   | 816 (0x0330)  | 0 (0x0000)     | NO      |                                                                                            |
| SMSG_STAND_STATE_UPDATE                           | 669 (0x029D)  | 10012 (0x271C) | YES     | HermesProxy/World/Client/WorldClient.cs:4959                                               |
| SMSG_START_MIRROR_TIMER                           | 473 (0x01D9)  | 9999 (0x270F)  | YES     | HermesProxy/World/Client/WorldClient.cs:5019                                               |
| SMSG_STOP_MIRROR_TIMER                            | 475 (0x01DB)  | 10001 (0x2711) | YES     | HermesProxy/World/Client/WorldClient.cs:5041                                               |
| SMSG_SUMMON_REQUEST                               | 683 (0x02AB)  | 10017 (0x2721) | YES     | HermesProxy/World/Client/WorldClient.cs:2707                                               |
| SMSG_SUPERCEDED_SPELLS                            | 300 (0x012C)  | 11337 (0x2C49) | YES     | HermesProxy/World/Client/WorldClient.cs:7672                                               |
| SMSG_SUSPEND_COMMS                                | 1295 (0x050F) | 12362 (0x304A) | NO      |                                                                                            |
| SMSG_TAXI_NODE_STATUS                             | 427 (0x01AB)  | 9852 (0x267C)  | YES     | HermesProxy/World/Client/WorldClient.cs:8882                                               |
| SMSG_TEXT_EMOTE                                   | 261 (0x0105)  | 9850 (0x267A)  | YES     | HermesProxy/World/Client/WorldClient.cs:1928                                               |
| SMSG_THREAT_CLEAR                                 | 1157 (0x0485) | 9948 (0x26DC)  | YES     | HermesProxy/World/Client/WorldClient.cs:2051                                               |
| SMSG_THREAT_REMOVE                                | 1156 (0x0484) | 9947 (0x26DB)  | NO      |                                                                                            |
| SMSG_THREAT_UPDATE                                | 1155 (0x0483) | 9946 (0x26DA)  | YES     | HermesProxy/World/Client/WorldClient.cs:2059                                               |
| SMSG_TIME_SYNC_REQUEST                            | 912 (0x0390)  | 11730 (0x2DD2) | YES     | HermesProxy/World/Client/WorldClient.cs:4864                                               |
| SMSG_TITLE_EARNED                                 | 883 (0x0373)  | 9943 (0x26D7)  | NO      |                                                                                            |
| SMSG_TOTEM_CREATED                                | 1043 (0x0413) | 9928 (0x26C8)  | YES     | HermesProxy/World/Client/WorldClient.cs:8808                                               |
| SMSG_TRADE_STATUS                                 | 288 (0x0120)  | 9602 (0x2582)  | YES     | HermesProxy/World/Client/WorldClient.cs:8939                                               |
| SMSG_TRAINER_BUY_FAILED                           | 436 (0x01B4)  | 9952 (0x26E0)  | YES     | HermesProxy/World/Client/WorldClient.cs:6073                                               |
| SMSG_TRAINER_LIST                                 | 433 (0x01B1)  | 9951 (0x26DF)  | YES     | HermesProxy/World/Client/WorldClient.cs:6032                                               |
| SMSG_TRANSFER_ABORTED                             | 64 (0x0040)   | 9987 (0x2703)  | YES     | HermesProxy/World/Client/WorldClient.cs:5524                                               |
| SMSG_TRANSFER_PENDING                             | 63 (0x003F)   | 9677 (0x25CD)  | YES     | HermesProxy/World/Client/WorldClient.cs:5504                                               |
| SMSG_TRIGGER_CINEMATIC                            | 250 (0x00FA)  | 10186 (0x27CA) | YES     | HermesProxy/World/Client/WorldClient.cs:5003                                               |
| SMSG_TURN_IN_PETITION_RESULT                      | 453 (0x01C5)  | 10062 (0x274E) | YES     | HermesProxy/World/Client/WorldClient.cs:6366                                               |
| SMSG_TUTORIAL_FLAGS                               | 253 (0x00FD)  | 10174 (0x27BE) | YES     | HermesProxy/World/Client/WorldClient.cs:4804                                               |
| SMSG_UNLEARNED_SPELLS                             | 515 (0x0203)  | 11339 (0x2C4B) | YES     | HermesProxy/World/Client/WorldClient.cs:7715                                               |
| SMSG_UPDATE_ACCOUNT_DATA                          | 524 (0x020C)  | 9993 (0x2709)  | NO      |                                                                                            |
| SMSG_UPDATE_ACTION_BUTTONS                        | 297 (0x0129)  | 9696 (0x25E0)  | YES     | HermesProxy/World/Client/WorldClient.cs:1151                                               |
| SMSG_UPDATE_COMBO_POINTS                          | 925 (0x039D)  | 0 (0x0000)     | YES     | HermesProxy/World/Client/WorldClient.cs:1267                                               |
| SMSG_UPDATE_INSTANCE_OWNERSHIP                    | 811 (0x032B)  | 9897 (0x26A9)  | YES     | HermesProxy/World/Client/WorldClient.cs:3943                                               |
| SMSG_UPDATE_LAST_INSTANCE                         | 800 (0x0320)  | 9864 (0x2688)  | NO      |                                                                                            |
| SMSG_UPDATE_OBJECT                                | 169 (0x00A9)  | 10187 (0x27CB) | YES     | HermesProxy/World/Client/WorldClient.cs:9090                                               |
| SMSG_UPDATE_TALENT_DATA                           | 1216 (0x04C0) | 9687 (0x25D7)  | YES     | HermesProxy/World/Client/WorldClient.cs:5094                                               |
| SMSG_UPDATE_WORLD_STATE                           | 707 (0x02C3)  | 10056 (0x2748) | YES     | HermesProxy/World/Client/WorldClient.cs:12245                                              |
| SMSG_VENDOR_INVENTORY                             | 415 (0x019F)  | 9656 (0x25B8)  | YES     | HermesProxy/World/Client/WorldClient.cs:5989                                               |
| SMSG_WEATHER                                      | 756 (0x02F4)  | 9894 (0x26A6)  | YES     | HermesProxy/World/Client/WorldClient.cs:4872                                               |
| SMSG_WHO                                          | 99 (0x0063)   | 11182 (0x2BAE) | YES     | HermesProxy/World/Client/WorldClient.cs:6946                                               |
| SMSG_ZONE_UNDER_ATTACK                            | 596 (0x0254)  | 11189 (0x2BB5) | YES     | HermesProxy/World/Client/WorldClient.cs:5061                                               |

## 2. Opcodes ONLY in Legacy (3.3.5a) - Not in Modern

Count: 711

These opcodes exist in the old server but have no modern client equivalent.

| Opcode                                               | Legacy Value  | Handled |
|------------------------------------------------------|---------------|---------|
| CMSG_ACCEPT_LEVEL_GRANT                              | 1056 (0x0420) | NO      |
| CMSG_ACTIVATE_TAXI_EXPRESS                           | 786 (0x0312)  | NO      |
| CMSG_ACTIVE_PVP_CHEAT                                | 921 (0x0399)  | NO      |
| CMSG_ADD_PVP_MEDAL_CHEAT                             | 649 (0x0289)  | NO      |
| CMSG_ADVANCE_SPAWN_TIME                              | 49 (0x0031)   | NO      |
| CMSG_AFK_MONITOR_INFO_CLEAR                          | 1285 (0x0505) | NO      |
| CMSG_AFK_MONITOR_INFO_REQUEST                        | 1283 (0x0503) | NO      |
| CMSG_ARENA_TEAM_CREATE                               | 840 (0x0348)  | NO      |
| CMSG_AUCTION_LIST_PENDING_SALES                      | 1167 (0x048F) | NO      |
| CMSG_AUTH_SRP6_BEGIN                                 | 51 (0x0033)   | NO      |
| CMSG_AUTH_SRP6_PROOF                                 | 52 (0x0034)   | NO      |
| CMSG_AUTH_SRP6_RECODE                                | 53 (0x0035)   | NO      |
| CMSG_AUTOSTORE_GROUND_ITEM                           | 263 (0x0107)  | NO      |
| CMSG_AUTO_EQUIP_GROUND_ITEM                          | 262 (0x0106)  | NO      |
| CMSG_BATTLEFIELD_JOIN                                | 574 (0x023E)  | NO      |
| CMSG_BATTLEFIELD_MANAGER_ADVANCE_STATE               | 1257 (0x04E9) | NO      |
| CMSG_BATTLEFIELD_MANAGER_SET_NEXT_TRANSITION_TIME    | 1258 (0x04EA) | NO      |
| CMSG_BATTLEFIELD_STATUS                              | 723 (0x02D3)  | NO      |
| CMSG_BATTLEGROUND_PORT_AND_LEAVE                     | 725 (0x02D5)  | NO      |
| CMSG_BEASTMASTER                                     | 33 (0x0021)   | NO      |
| CMSG_BF_MGR_ENTRY_INVITE_RESPONSE                    | 1247 (0x04DF) | NO      |
| CMSG_BF_MGR_QUEUE_EXIT_REQUEST                       | 1255 (0x04E7) | NO      |
| CMSG_BF_MGR_QUEUE_INVITE_RESPONSE                    | 1250 (0x04E2) | NO      |
| CMSG_BF_MGR_QUEUE_REQUEST                            | 1251 (0x04E3) | NO      |
| CMSG_BOOTME                                          | 1 (0x0001)    | NO      |
| CMSG_BOT_DETECTED                                    | 960 (0x03C0)  | NO      |
| CMSG_BOT_DETECTED2                                   | 23 (0x0017)   | NO      |
| CMSG_BUY_ITEM_IN_SLOT                                | 419 (0x01A3)  | NO      |
| CMSG_BUY_LOTTERY_TICKET_OBSOLETE                     | 822 (0x0336)  | NO      |
| CMSG_CALENDAR_ADD_EVENT                              | 1069 (0x042D) | NO      |
| CMSG_CALENDAR_ARENA_TEAM                             | 1068 (0x042C) | NO      |
| CMSG_CALENDAR_COMPLAIN                               | 1094 (0x0446) | NO      |
| CMSG_CALENDAR_COPY_EVENT                             | 1072 (0x0430) | NO      |
| CMSG_CALENDAR_EVENT_INVITE                           | 1073 (0x0431) | NO      |
| CMSG_CALENDAR_EVENT_INVITE_NOTES                     | 1119 (0x045F) | NO      |
| CMSG_CALENDAR_EVENT_MODERATOR_STATUS                 | 1077 (0x0435) | NO      |
| CMSG_CALENDAR_EVENT_REMOVE_INVITE                    | 1075 (0x0433) | NO      |
| CMSG_CALENDAR_EVENT_RSVP                             | 1074 (0x0432) | NO      |
| CMSG_CALENDAR_EVENT_SIGN_UP                          | 1210 (0x04BA) | NO      |
| CMSG_CALENDAR_EVENT_STATUS                           | 1076 (0x0434) | NO      |
| CMSG_CALENDAR_GET_CALENDAR                           | 1065 (0x0429) | NO      |
| CMSG_CALENDAR_GET_EVENT                              | 1066 (0x042A) | NO      |
| CMSG_CALENDAR_GUILD_FILTER                           | 1067 (0x042B) | NO      |
| CMSG_CALENDAR_REMOVE_EVENT                           | 1071 (0x042F) | NO      |
| CMSG_CALENDAR_UPDATE_EVENT                           | 1070 (0x042E) | NO      |
| CMSG_CHANGEPLAYER_DIFFICULTY                         | 509 (0x01FD)  | NO      |
| CMSG_CHANGE_GDF_ARENA_RATING                         | 1196 (0x04AC) | NO      |
| CMSG_CHANGE_PERSONAL_ARENA_RATING                    | 1061 (0x0425) | NO      |
| CMSG_CHANGE_SEATS_ON_CONTROLLED_VEHICLE              | 1179 (0x049B) | NO      |
| CMSG_CHARACTER_POINT_CHEAT                           | 547 (0x0223)  | NO      |
| CMSG_CHAR_CUSTOMIZE                                  | 1139 (0x0473) | NO      |
| CMSG_CHAR_FACTION_CHANGE                             | 1241 (0x04D9) | NO      |
| CMSG_CHAR_RACE_CHANGE                                | 1272 (0x04F8) | NO      |
| CMSG_CHAT_CHANNEL_MODERATE                           | 168 (0x00A8)  | NO      |
| CMSG_CHAT_CHANNEL_MUTE                               | 161 (0x00A1)  | NO      |
| CMSG_CHAT_CHANNEL_SILENCE_VOICE                      | 972 (0x03CC)  | NO      |
| CMSG_CHAT_CHANNEL_UNMUTE                             | 162 (0x00A2)  | NO      |
| CMSG_CHAT_CHANNEL_UNSILENCE_VOICE                    | 974 (0x03CE)  | NO      |
| CMSG_CHAT_CHANNEL_VOICE_OFF                          | 983 (0x03D7)  | NO      |
| CMSG_CHAT_CHANNEL_VOICE_ON                           | 982 (0x03D6)  | NO      |
| CMSG_CHAT_REPORT_FILTERED                            | 817 (0x0331)  | NO      |
| CMSG_CHAT_REPORT_IGNORED                             | 549 (0x0225)  | NO      |
| CMSG_CHEAT_DUMP_ITEMS_DEBUG_ONLY                     | 922 (0x039A)  | NO      |
| CMSG_CHEAT_PLAYER_LOGIN                              | 962 (0x03C2)  | NO      |
| CMSG_CHEAT_PLAYER_LOOKUP                             | 963 (0x03C3)  | NO      |
| CMSG_CHEAT_SETMONEY                                  | 36 (0x0024)   | NO      |
| CMSG_CHEAT_SET_ARENA_CURRENCY                        | 892 (0x037C)  | NO      |
| CMSG_CHEAT_SET_HONOR_CURRENCY                        | 891 (0x037B)  | NO      |
| CMSG_CHECK_LOGIN_CRITERIA                            | 1186 (0x04A2) | NO      |
| CMSG_CLEAR_CHANNEL_WATCH                             | 1011 (0x03F3) | NO      |
| CMSG_CLEAR_EXPLORATION                               | 567 (0x0237)  | NO      |
| CMSG_CLEAR_HOLIDAY_BG_WIN_TIME                       | 1306 (0x051A) | NO      |
| CMSG_CLEAR_QUEST                                     | 44 (0x002C)   | NO      |
| CMSG_CLEAR_RANDOM_BG_WIN_TIME                        | 1305 (0x0519) | NO      |
| CMSG_CLEAR_SERVER_BUCK_DATA                          | 1052 (0x041C) | NO      |
| CMSG_COMMENTATOR_ENABLE                              | 949 (0x03B5)  | NO      |
| CMSG_COMMENTATOR_ENTER_INSTANCE                      | 956 (0x03BC)  | NO      |
| CMSG_COMMENTATOR_EXIT_INSTANCE                       | 957 (0x03BD)  | NO      |
| CMSG_COMMENTATOR_GET_MAP_INFO                        | 951 (0x03B7)  | NO      |
| CMSG_COMMENTATOR_GET_PLAYER_INFO                     | 953 (0x03B9)  | NO      |
| CMSG_COMMENTATOR_INSTANCE_COMMAND                    | 958 (0x03BE)  | NO      |
| CMSG_COMMENTATOR_SKIRMISH_QUEUE_COMMAND              | 1307 (0x051B) | NO      |
| CMSG_COMPLAINT                                       | 967 (0x03C7)  | NO      |
| CMSG_COMPLETE_ACHIEVEMENT_CHEAT                      | 1134 (0x046E) | NO      |
| CMSG_COMPLETE_MOVIE                                  | 1125 (0x0465) | NO      |
| CMSG_CONNECT_TO_FAILED                               | 1294 (0x050E) | NO      |
| CMSG_CONTACT_LIST                                    | 102 (0x0066)  | YES     |
| CMSG_COOLDOWN_CHEAT                                  | 40 (0x0028)   | NO      |
| CMSG_CORPSE_MAP_POSITION_QUERY                       | 1206 (0x04B6) | NO      |
| CMSG_CREATEGAMEOBJECT                                | 20 (0x0014)   | NO      |
| CMSG_CREATEITEM                                      | 19 (0x0013)   | NO      |
| CMSG_CREATEMONSTER                                   | 17 (0x0011)   | NO      |
| CMSG_DANCE_QUERY                                     | 1105 (0x0451) | NO      |
| CMSG_DBLOOKUP                                        | 2 (0x0002)    | NO      |
| CMSG_DEBUG_ACTIONS_START                             | 789 (0x0315)  | NO      |
| CMSG_DEBUG_ACTIONS_STOP                              | 790 (0x0316)  | NO      |
| CMSG_DEBUG_AISTATE                                   | 46 (0x002E)   | NO      |
| CMSG_DEBUG_CHANGECELLZONE                            | 12 (0x000C)   | NO      |
| CMSG_DEBUG_LIST_TARGETS                              | 984 (0x03D8)  | NO      |
| CMSG_DEBUG_PASSIVE_AURA                              | 320 (0x0140)  | NO      |
| CMSG_DEBUG_SERVER_GEO                                | 1275 (0x04FB) | NO      |
| CMSG_DECHARGE                                        | 516 (0x0204)  | NO      |
| CMSG_DELETE_DANCE                                    | 1108 (0x0454) | NO      |
| CMSG_DEL_PVP_MEDAL_CHEAT                             | 650 (0x028A)  | NO      |
| CMSG_DESTROYMONSTER                                  | 18 (0x0012)   | NO      |
| CMSG_DESTROY_ITEMS                                   | 178 (0x00B2)  | NO      |
| CMSG_DISABLE_PVP_CHEAT                               | 48 (0x0030)   | NO      |
| CMSG_DISMISS_CONTROLLED_VEHICLE                      | 1133 (0x046D) | NO      |
| CMSG_DISMISS_CRITTER                                 | 1165 (0x048D) | NO      |
| CMSG_DROP_NEW_CONNECTION                             | 1299 (0x0513) | NO      |
| CMSG_DUEL_ACCEPTED                                   | 364 (0x016C)  | NO      |
| CMSG_DUEL_CANCELLED                                  | 365 (0x016D)  | NO      |
| CMSG_DUMP_OBJECTS                                    | 1163 (0x048B) | NO      |
| CMSG_EJECT_PASSENGER                                 | 1193 (0x04A9) | NO      |
| CMSG_EMOTE                                           | 258 (0x0102)  | NO      |
| CMSG_ENABLE_DAMAGE_LOG                               | 637 (0x027D)  | NO      |
| CMSG_END_BATTLEFIELD_CHEAT                           | 1228 (0x04CC) | NO      |
| CMSG_EQUIPMENT_SET_DELETE                            | 318 (0x013E)  | NO      |
| CMSG_EQUIPMENT_SET_USE                               | 1237 (0x04D5) | NO      |
| CMSG_EXPIRE_RAID_INSTANCE                            | 1045 (0x0415) | NO      |
| CMSG_FLAG_QUEST                                      | 42 (0x002A)   | NO      |
| CMSG_FLAG_QUEST_FINISH                               | 43 (0x002B)   | NO      |
| CMSG_FLOOD_GRACE_CHEAT                               | 1175 (0x0497) | NO      |
| CMSG_FORCEACTION                                     | 24 (0x0018)   | NO      |
| CMSG_FORCEACTIONONOTHER                              | 25 (0x0019)   | NO      |
| CMSG_FORCEACTIONSHOW                                 | 26 (0x001A)   | NO      |
| CMSG_FORCE_ANIM                                      | 1239 (0x04D7) | NO      |
| CMSG_FORCE_SAY_CHEAT                                 | 1150 (0x047E) | NO      |
| CMSG_GAMESPEED_SET                                   | 70 (0x0046)   | NO      |
| CMSG_GAMETIME_SET                                    | 68 (0x0044)   | NO      |
| CMSG_GETDEATHBINDZONE                                | 342 (0x0156)  | NO      |
| CMSG_GET_CHANNEL_MEMBER_COUNT                        | 980 (0x03D4)  | NO      |
| CMSG_GET_ITEM_PURCHASE_DATA                          | 1203 (0x04B3) | NO      |
| CMSG_GET_MIRROR_IMAGE_DATA                           | 1025 (0x0401) | NO      |
| CMSG_GHOST                                           | 485 (0x01E5)  | NO      |
| CMSG_GMRESPONSE_CREATE_TICKET                        | 1267 (0x04F3) | NO      |
| CMSG_GMTICKETSYSTEM_TOGGLE                           | 666 (0x029A)  | NO      |
| CMSG_GM_CHARACTER_RESTORE                            | 1018 (0x03FA) | NO      |
| CMSG_GM_CHARACTER_SAVE                               | 1019 (0x03FB) | NO      |
| CMSG_GM_CREATE_ITEM_TARGET                           | 528 (0x0210)  | NO      |
| CMSG_GM_DESTROY_ONLINE_CORPSE                        | 785 (0x0311)  | NO      |
| CMSG_GM_FREEZE                                       | 557 (0x022D)  | NO      |
| CMSG_GM_GRANT_ACHIEVEMENT                            | 1220 (0x04C4) | NO      |
| CMSG_GM_INVIS                                        | 486 (0x01E6)  | NO      |
| CMSG_GM_LAG_REPORT                                   | 1282 (0x0502) | NO      |
| CMSG_GM_MOVECORPSE                                   | 556 (0x022C)  | NO      |
| CMSG_GM_NUKE                                         | 506 (0x01FA)  | NO      |
| CMSG_GM_NUKE_ACCOUNT                                 | 783 (0x030F)  | NO      |
| CMSG_GM_NUKE_CHARACTER                               | 1287 (0x0507) | NO      |
| CMSG_GM_REMOVE_ACHIEVEMENT                           | 1221 (0x04C5) | NO      |
| CMSG_GM_REQUEST_PLAYER_INFO                          | 559 (0x022F)  | NO      |
| CMSG_GM_RESURRECT                                    | 554 (0x022A)  | NO      |
| CMSG_GM_REVEALTO                                     | 553 (0x0229)  | NO      |
| CMSG_GM_SET_CRITERIA_FOR_PLAYER                      | 1222 (0x04C6) | NO      |
| CMSG_GM_SET_SECURITY_GROUP                           | 505 (0x01F9)  | NO      |
| CMSG_GM_SHOW_COMPLAINTS                              | 970 (0x03CA)  | NO      |
| CMSG_GM_SILENCE                                      | 552 (0x0228)  | NO      |
| CMSG_GM_SUMMONMOB                                    | 555 (0x022B)  | NO      |
| CMSG_GM_SURVEY_SUBMIT                                | 810 (0x032A)  | NO      |
| CMSG_GM_TEACH                                        | 527 (0x020F)  | NO      |
| CMSG_GM_TICKET_GET_SYSTEM_STATUS                     | 538 (0x021A)  | NO      |
| CMSG_GM_TICKET_RESPONSE_RESOLVE                      | 1264 (0x04F0) | NO      |
| CMSG_GM_UBERINVIS                                    | 558 (0x022E)  | NO      |
| CMSG_GM_UNSQUELCH                                    | 971 (0x03CB)  | NO      |
| CMSG_GM_UNTEACH                                      | 741 (0x02E5)  | NO      |
| CMSG_GM_UPDATE_TICKET_STATUS                         | 807 (0x0327)  | NO      |
| CMSG_GM_VISION                                       | 550 (0x0226)  | NO      |
| CMSG_GM_WHISPER                                      | 946 (0x03B2)  | NO      |
| CMSG_GODMODE                                         | 34 (0x0022)   | NO      |
| CMSG_GRANT_LEVEL                                     | 1037 (0x040D) | NO      |
| CMSG_GROUP_ACCEPT                                    | 114 (0x0072)  | NO      |
| CMSG_GROUP_CANCEL                                    | 112 (0x0070)  | NO      |
| CMSG_GROUP_CHANGE_SUB_GROUP                          | 638 (0x027E)  | YES     |
| CMSG_GROUP_DISBAND                                   | 123 (0x007B)  | NO      |
| CMSG_GROUP_UNINVITE                                  | 117 (0x0075)  | NO      |
| CMSG_GROUP_UNINVITE_GUID                             | 118 (0x0076)  | NO      |
| CMSG_GUILD_CREATE                                    | 129 (0x0081)  | NO      |
| CMSG_GUILD_INFO                                      | 135 (0x0087)  | NO      |
| CMSG_GUILD_SET_OFFICER_NOTE                          | 565 (0x0235)  | NO      |
| CMSG_GUILD_SET_PUBLIC_NOTE                           | 564 (0x0234)  | NO      |
| CMSG_IGNORE_DIMINISHING_RETURNS_CHEAT                | 1029 (0x0405) | NO      |
| CMSG_IGNORE_KNOCKBACK_CHEAT                          | 812 (0x032C)  | NO      |
| CMSG_IGNORE_REQUIREMENTS_CHEAT                       | 936 (0x03A8)  | NO      |
| CMSG_ITEM_NAME_QUERY                                 | 708 (0x02C4)  | NO      |
| CMSG_ITEM_PURCHASE_REFUND                            | 1204 (0x04B4) | NO      |
| CMSG_ITEM_QUERY_MULTIPLE                             | 87 (0x0057)   | NO      |
| CMSG_LEARN_DANCE_MOVE                                | 1110 (0x0456) | NO      |
| CMSG_LEARN_PREVIEW_TALENTS                           | 1217 (0x04C1) | NO      |
| CMSG_LEARN_PREVIEW_TALENTS_PET                       | 1218 (0x04C2) | NO      |
| CMSG_LEARN_SPELL                                     | 16 (0x0010)   | NO      |
| CMSG_LEVEL_CHEAT                                     | 37 (0x0025)   | NO      |
| CMSG_LFG_JOIN                                        | 860 (0x035C)  | NO      |
| CMSG_LFG_LEAVE                                       | 861 (0x035D)  | NO      |
| CMSG_LFG_LFR_JOIN                                    | 862 (0x035E)  | NO      |
| CMSG_LFG_LFR_LEAVE                                   | 863 (0x035F)  | NO      |
| CMSG_LFG_PARTY_LOCK_INFO_REQUEST                     | 881 (0x0371)  | NO      |
| CMSG_LFG_PLAYER_LOCK_INFO_REQUEST                    | 878 (0x036E)  | NO      |
| CMSG_LFG_PROPOSAL_RESULT                             | 866 (0x0362)  | NO      |
| CMSG_LFG_SET_BOOT_VOTE                               | 876 (0x036C)  | NO      |
| CMSG_LFG_SET_COMMENT                                 | 870 (0x0366)  | NO      |
| CMSG_LFG_SET_NEEDS                                   | 875 (0x036B)  | NO      |
| CMSG_LFG_SET_ROLES                                   | 874 (0x036A)  | NO      |
| CMSG_LFG_TELEPORT                                    | 880 (0x0370)  | NO      |
| CMSG_LOAD_DANCES                                     | 1101 (0x044D) | NO      |
| CMSG_LOTTERY_QUERY_OBSOLETE                          | 820 (0x0334)  | NO      |
| CMSG_LOW_LEVEL_RAID1                                 | 1288 (0x0508) | NO      |
| CMSG_LOW_LEVEL_RAID2                                 | 1289 (0x0509) | NO      |
| CMSG_LUA_USAGE                                       | 803 (0x0323)  | NO      |
| CMSG_MAELSTROM_GM_SENT_MAIL                          | 917 (0x0395)  | NO      |
| CMSG_MAELSTROM_INVALIDATE_CACHE                      | 903 (0x0387)  | NO      |
| CMSG_MAELSTROM_RENAME_GUILD                          | 1024 (0x0400) | NO      |
| CMSG_MAKEMONSTERATTACKGUID                           | 22 (0x0016)   | NO      |
| CMSG_MINIGAME_MOVE                                   | 760 (0x02F8)  | NO      |
| CMSG_MOVE_CHARACTER_CHEAT                            | 13 (0x000D)   | NO      |
| CMSG_MOVE_CHARM_PORT_CHEAT                           | 224 (0x00E0)  | NO      |
| CMSG_MOVE_ENABLE_SWIM_TO_FLY_TRANS_ACK               | 832 (0x0340)  | NO      |
| CMSG_MOVE_NOT_ACTIVE_MOVER                           | 721 (0x02D1)  | NO      |
| CMSG_MOVE_SET_COLLISION_HGT_ACK                      | 1303 (0x0517) | NO      |
| CMSG_MOVE_SET_RAW_POSITION                           | 225 (0x00E1)  | NO      |
| CMSG_MOVE_SET_RUN_SPEED                              | 939 (0x03AB)  | NO      |
| CMSG_MOVE_START_SWIM_CHEAT                           | 728 (0x02D8)  | NO      |
| CMSG_MOVE_STOP_SWIM_CHEAT                            | 729 (0x02D9)  | NO      |
| CMSG_NAME_QUERY                                      | 80 (0x0050)   | NO      |
| CMSG_NEW_SPELL_SLOT                                  | 301 (0x012D)  | NO      |
| CMSG_NO_SPELL_VARIANCE                               | 1046 (0x0416) | NO      |
| CMSG_PARTY_SILENCE                                   | 989 (0x03DD)  | NO      |
| CMSG_PARTY_UNSILENCE                                 | 990 (0x03DE)  | NO      |
| CMSG_PERFORM_ACTION_SET                              | 332 (0x014C)  | NO      |
| CMSG_PETGODMODE                                      | 28 (0x001C)   | NO      |
| CMSG_PETITION_SHOW_LIST                              | 443 (0x01BB)  | NO      |
| CMSG_PET_LEARN_TALENT                                | 1146 (0x047A) | NO      |
| CMSG_PET_LEVEL_CHEAT                                 | 38 (0x0026)   | NO      |
| CMSG_PET_SPELL_AUTOCAST                              | 755 (0x02F3)  | NO      |
| CMSG_PET_UNLEARN                                     | 752 (0x02F0)  | NO      |
| CMSG_PET_UNLEARN_TALENTS                             | 1147 (0x047B) | NO      |
| CMSG_PLAYER_LOGOUT                                   | 74 (0x004A)   | NO      |
| CMSG_PLAYER_SHOWING_CLOAK                            | 698 (0x02BA)  | YES     |
| CMSG_PLAYER_SHOWING_HELM                             | 697 (0x02B9)  | YES     |
| CMSG_PLAYER_VEHICLE_ENTER                            | 1192 (0x04A8) | NO      |
| CMSG_PLAY_DANCE                                      | 1099 (0x044B) | NO      |
| CMSG_PROFILEDATA_REQUEST                             | 1225 (0x04C9) | NO      |
| CMSG_PVP_QUEUE_STATS_REQUEST                         | 1243 (0x04DB) | NO      |
| CMSG_QUERY_INSPECT_ACHIEVEMENTS                      | 1131 (0x046B) | NO      |
| CMSG_QUERY_OBJECT_POSITION                           | 4 (0x0004)    | NO      |
| CMSG_QUERY_OBJECT_ROTATION                           | 6 (0x0006)    | NO      |
| CMSG_QUERY_QUEST_COMPLETION_NPCS                     | 1161 (0x0489) | NO      |
| CMSG_QUERY_SERVER_BUCK_DATA                          | 1051 (0x041B) | NO      |
| CMSG_QUERY_VEHICLE_STATUS                            | 1189 (0x04A5) | NO      |
| CMSG_QUEST_GIVER_CANCEL                              | 400 (0x0190)  | NO      |
| CMSG_QUEST_GIVER_QUEST_AUTOLAUNCH                    | 391 (0x0187)  | NO      |
| CMSG_QUEST_LOG_SWAP_QUEST                            | 403 (0x0193)  | NO      |
| CMSG_READY_FOR_ACCOUNT_DATA_TIMES                    | 1279 (0x04FF) | NO      |
| CMSG_REALM_SPLIT                                     | 908 (0x038C)  | NO      |
| CMSG_RECHARGE                                        | 15 (0x000F)   | NO      |
| CMSG_REFER_A_FRIEND                                  | 1038 (0x040E) | NO      |
| CMSG_REPORT_PVP_PLAYER_AFK                           | 996 (0x03E4)  | NO      |
| CMSG_RUN_SCRIPT                                      | 693 (0x02B5)  | NO      |
| CMSG_SAVE_DANCE                                      | 1097 (0x0449) | NO      |
| CMSG_SAVE_EQUIPMENT_SET                              | 1213 (0x04BD) | NO      |
| CMSG_SAVE_PLAYER                                     | 339 (0x0153)  | NO      |
| CMSG_SEND_COMBAT_TRIGGER                             | 916 (0x0394)  | NO      |
| CMSG_SEND_EVENT                                      | 45 (0x002D)   | NO      |
| CMSG_SEND_GENERAL_TRIGGER                            | 915 (0x0393)  | NO      |
| CMSG_SEND_LOCAL_EVENT                                | 914 (0x0392)  | NO      |
| CMSG_SERVERINFO                                      | 1268 (0x04F4) | NO      |
| CMSG_SERVERTIME                                      | 72 (0x0048)   | NO      |
| CMSG_SERVER_BROADCAST                                | 690 (0x02B2)  | NO      |
| CMSG_SERVER_COMMAND                                  | 551 (0x0227)  | NO      |
| CMSG_SERVER_INFO_QUERY                               | 1184 (0x04A0) | NO      |
| CMSG_SETDEATHBINDPOINT                               | 340 (0x0154)  | NO      |
| CMSG_SET_ACTIVE_TALENT_GROUP_OBSOLETE                | 1219 (0x04C3) | NO      |
| CMSG_SET_ACTIVE_VOICE_CHANNEL                        | 979 (0x03D3)  | NO      |
| CMSG_SET_ARENA_MEMBER_SEASON_GAMES                   | 1201 (0x04B1) | NO      |
| CMSG_SET_ARENA_MEMBER_WEEKLY_GAMES                   | 1200 (0x04B0) | NO      |
| CMSG_SET_ARENA_TEAM_RATING_BY_INDEX                  | 1197 (0x04AD) | NO      |
| CMSG_SET_ARENA_TEAM_SEASON_GAMES                     | 1199 (0x04AF) | NO      |
| CMSG_SET_ARENA_TEAM_WEEKLY_GAMES                     | 1198 (0x04AE) | NO      |
| CMSG_SET_BREATH                                      | 1188 (0x04A4) | NO      |
| CMSG_SET_CHANNEL_WATCH                               | 1007 (0x03EF) | NO      |
| CMSG_SET_CHARACTER_MODEL                             | 1292 (0x050C) | NO      |
| CMSG_SET_CRITERIA_CHEAT                              | 1136 (0x0470) | NO      |
| CMSG_SET_DURABILITY_CHEAT                            | 647 (0x0287)  | NO      |
| CMSG_SET_EXPLORATION                                 | 702 (0x02BE)  | NO      |
| CMSG_SET_EXPLORATION_ALL                             | 795 (0x031B)  | NO      |
| CMSG_SET_FACTION_CHEAT                               | 294 (0x0126)  | NO      |
| CMSG_SET_GLYPH                                       | 1127 (0x0467) | NO      |
| CMSG_SET_GLYPH_SLOT                                  | 1126 (0x0466) | NO      |
| CMSG_SET_GRANTABLE_LEVELS                            | 1036 (0x040C) | NO      |
| CMSG_SET_PAID_SERVICE_CHEAT                          | 1245 (0x04DD) | NO      |
| CMSG_SET_PLAYER_DECLINED_NAMES                       | 1049 (0x0419) | NO      |
| CMSG_SET_PVP_RANK_CHEAT                              | 648 (0x0288)  | NO      |
| CMSG_SET_PVP_TITLE                                   | 651 (0x028B)  | NO      |
| CMSG_SET_RUNE_COOLDOWN                               | 1113 (0x0459) | NO      |
| CMSG_SET_RUNE_COUNT                                  | 1112 (0x0458) | NO      |
| CMSG_SET_SAVED_INSTANCE_EXTEND                       | 658 (0x0292)  | NO      |
| CMSG_SET_SKILL_CHEAT                                 | 472 (0x01D8)  | NO      |
| CMSG_SET_STAT_CHEAT                                  | 541 (0x021D)  | NO      |
| CMSG_SET_TAXI_BENCHMARK_MODE                         | 905 (0x0389)  | NO      |
| CMSG_SET_TITLE_SUFFIX                                | 1015 (0x03F7) | NO      |
| CMSG_SET_VEHICLE_REC_ID_ACK                          | 576 (0x0240)  | NO      |
| CMSG_SET_WORLDSTATE                                  | 39 (0x0027)   | NO      |
| CMSG_SKILL_BUY_RANK                                  | 544 (0x0220)  | NO      |
| CMSG_SKILL_BUY_STEP                                  | 543 (0x021F)  | NO      |
| CMSG_START_BATTLEFIELD_CHEAT                         | 1227 (0x04CB) | NO      |
| CMSG_STOP_DANCE                                      | 1102 (0x044E) | NO      |
| CMSG_STORE_LOOT_IN_SLOT                              | 265 (0x0109)  | NO      |
| CMSG_SUSPEND_COMMS_ACK                               | 1296 (0x0510) | NO      |
| CMSG_SYNC_DANCE                                      | 1104 (0x0450) | NO      |
| CMSG_TARGET_CAST                                     | 976 (0x03D0)  | NO      |
| CMSG_TARGET_SCRIPT_CAST                              | 977 (0x03D1)  | NO      |
| CMSG_TAXICLEARALLNODES                               | 422 (0x01A6)  | NO      |
| CMSG_TAXICLEARNODE                                   | 577 (0x0241)  | NO      |
| CMSG_TAXIENABLEALLNODES                              | 423 (0x01A7)  | NO      |
| CMSG_TAXIENABLENODE                                  | 578 (0x0242)  | NO      |
| CMSG_TAXISHOWNODES                                   | 424 (0x01A8)  | NO      |
| CMSG_TELEPORT_TO_UNIT                                | 9 (0x0009)    | NO      |
| CMSG_TEST_DROP_RATE                                  | 660 (0x0294)  | NO      |
| CMSG_TOGGLE_XP_GAIN                                  | 1260 (0x04EC) | NO      |
| CMSG_TRIGGER_CINEMATIC_CHEAT                         | 248 (0x00F8)  | NO      |
| CMSG_TUTORIAL_CLEAR                                  | 255 (0x00FF)  | NO      |
| CMSG_TUTORIAL_RESET                                  | 256 (0x0100)  | NO      |
| CMSG_UI_TIME_REQUEST                                 | 1270 (0x04F6) | NO      |
| CMSG_UNCLAIM_LICENSE                                 | 272 (0x0110)  | NO      |
| CMSG_UNDRESSPLAYER                                   | 32 (0x0020)   | NO      |
| CMSG_UNITANIMTIER_CHEAT                              | 1138 (0x0472) | NO      |
| CMSG_UNLEARN_DANCE_MOVE                              | 1111 (0x0457) | NO      |
| CMSG_UNLEARN_SPELL                                   | 513 (0x0201)  | NO      |
| CMSG_UNLEARN_TALENTS                                 | 531 (0x0213)  | NO      |
| CMSG_UNUSED5                                         | 1208 (0x04B8) | NO      |
| CMSG_UNUSED6                                         | 1209 (0x04B9) | NO      |
| CMSG_UPDATE_PROJECTILE_POSITION                      | 1214 (0x04BE) | NO      |
| CMSG_USE_SKILL_CHEAT                                 | 41 (0x0029)   | NO      |
| CMSG_VOICE_ADD_IGNORE                                | 987 (0x03DB)  | NO      |
| CMSG_VOICE_DEL_IGNORE                                | 988 (0x03DC)  | NO      |
| CMSG_VOICE_SESSION_ENABLE                            | 943 (0x03AF)  | NO      |
| CMSG_VOICE_SET_TALKER_MUTED_REQUEST                  | 929 (0x03A1)  | NO      |
| CMSG_WEATHER_SPEED_CHEAT                             | 31 (0x001F)   | NO      |
| CMSG_WHO_IS                                          | 100 (0x0064)  | NO      |
| CMSG_WORLD_TELEPORT                                  | 8 (0x0008)    | NO      |
| CMSG_XP_CHEAT                                        | 545 (0x0221)  | NO      |
| CMSG_ZONE_MAP                                        | 10 (0x000A)   | NO      |
| MSG_AUCTION_HELLO                                    | 597 (0x0255)  | YES     |
| MSG_BATTLEGROUND_PLAYER_POSITIONS                    | 745 (0x02E9)  | YES     |
| MSG_CHANNEL_START                                    | 313 (0x0139)  | YES     |
| MSG_CHANNEL_UPDATE                                   | 314 (0x013A)  | YES     |
| MSG_CORPSE_QUERY                                     | 534 (0x0216)  | YES     |
| MSG_DELAY_GHOST_TELEPORT                             | 814 (0x032E)  | NO      |
| MSG_DEV_SHOWLABEL                                    | 685 (0x02AD)  | NO      |
| MSG_GM_ACCOUNT_ONLINE                                | 622 (0x026E)  | NO      |
| MSG_GM_BIND_OTHER                                    | 488 (0x01E8)  | NO      |
| MSG_GM_CHANGE_ARENA_RATING                           | 1039 (0x040F) | NO      |
| MSG_GM_DESTROY_CORPSE                                | 784 (0x0310)  | NO      |
| MSG_GM_GEARRATING                                    | 948 (0x03B4)  | NO      |
| MSG_GM_RESETINSTANCELIMIT                            | 828 (0x033C)  | NO      |
| MSG_GM_SHOWLABEL                                     | 495 (0x01EF)  | NO      |
| MSG_GM_SUMMON                                        | 489 (0x01E9)  | NO      |
| MSG_GUILD_BANK_LOG_QUERY                             | 1006 (0x03EE) | YES     |
| MSG_GUILD_BANK_MONEY_WITHDRAWN                       | 1022 (0x03FE) | YES     |
| MSG_GUILD_EVENT_LOG_QUERY                            | 1023 (0x03FF) | NO      |
| MSG_GUILD_PERMISSIONS                                | 1021 (0x03FD) | NO      |
| MSG_INSPECT_ARENA_TEAMS                              | 887 (0x0377)  | YES     |
| MSG_INSPECT_HONOR_STATS                              | 726 (0x02D6)  | YES     |
| MSG_LIST_STABLED_PETS                                | 623 (0x026F)  | YES     |
| MSG_MINIMAP_PING                                     | 469 (0x01D5)  | YES     |
| MSG_MOVE_FALL_LAND                                   | 201 (0x00C9)  | YES     |
| MSG_MOVE_FEATHER_FALL                                | 688 (0x02B0)  | YES     |
| MSG_MOVE_GRAVITY_CHNG                                | 1234 (0x04D2) | YES     |
| MSG_MOVE_HEARTBEAT                                   | 238 (0x00EE)  | YES     |
| MSG_MOVE_HOVER                                       | 247 (0x00F7)  | YES     |
| MSG_MOVE_JUMP                                        | 187 (0x00BB)  | YES     |
| MSG_MOVE_KNOCK_BACK                                  | 241 (0x00F1)  | YES     |
| MSG_MOVE_ROOT                                        | 236 (0x00EC)  | YES     |
| MSG_MOVE_SET_ALL_SPEED_CHEAT                         | 214 (0x00D6)  | NO      |
| MSG_MOVE_SET_COLLISION_HGT                           | 1304 (0x0518) | NO      |
| MSG_MOVE_SET_FACING                                  | 218 (0x00DA)  | YES     |
| MSG_MOVE_SET_FLIGHT_BACK_SPEED                       | 896 (0x0380)  | YES     |
| MSG_MOVE_SET_FLIGHT_BACK_SPEED_CHEAT                 | 895 (0x037F)  | NO      |
| MSG_MOVE_SET_FLIGHT_SPEED                            | 894 (0x037E)  | YES     |
| MSG_MOVE_SET_FLIGHT_SPEED_CHEAT                      | 893 (0x037D)  | NO      |
| MSG_MOVE_SET_PITCH                                   | 219 (0x00DB)  | YES     |
| MSG_MOVE_SET_PITCH_RATE                              | 1115 (0x045B) | YES     |
| MSG_MOVE_SET_PITCH_RATE_CHEAT                        | 1114 (0x045A) | NO      |
| MSG_MOVE_SET_RUN_BACK_SPEED                          | 207 (0x00CF)  | YES     |
| MSG_MOVE_SET_RUN_BACK_SPEED_CHEAT                    | 206 (0x00CE)  | NO      |
| MSG_MOVE_SET_RUN_MODE                                | 194 (0x00C2)  | YES     |
| MSG_MOVE_SET_RUN_SPEED                               | 205 (0x00CD)  | YES     |
| MSG_MOVE_SET_RUN_SPEED_CHEAT                         | 204 (0x00CC)  | NO      |
| MSG_MOVE_SET_SWIM_BACK_SPEED                         | 213 (0x00D5)  | YES     |
| MSG_MOVE_SET_SWIM_BACK_SPEED_CHEAT                   | 212 (0x00D4)  | NO      |
| MSG_MOVE_SET_SWIM_SPEED                              | 211 (0x00D3)  | YES     |
| MSG_MOVE_SET_SWIM_SPEED_CHEAT                        | 210 (0x00D2)  | NO      |
| MSG_MOVE_SET_TURN_RATE                               | 216 (0x00D8)  | YES     |
| MSG_MOVE_SET_TURN_RATE_CHEAT                         | 215 (0x00D7)  | NO      |
| MSG_MOVE_SET_WALK_MODE                               | 195 (0x00C3)  | YES     |
| MSG_MOVE_SET_WALK_SPEED                              | 209 (0x00D1)  | YES     |
| MSG_MOVE_SET_WALK_SPEED_CHEAT                        | 208 (0x00D0)  | NO      |
| MSG_MOVE_START_ASCEND                                | 857 (0x0359)  | YES     |
| MSG_MOVE_START_BACKWARD                              | 182 (0x00B6)  | YES     |
| MSG_MOVE_START_DESCEND                               | 935 (0x03A7)  | YES     |
| MSG_MOVE_START_FORWARD                               | 181 (0x00B5)  | YES     |
| MSG_MOVE_START_PITCH_DOWN                            | 192 (0x00C0)  | YES     |
| MSG_MOVE_START_PITCH_UP                              | 191 (0x00BF)  | YES     |
| MSG_MOVE_START_STRAFE_LEFT                           | 184 (0x00B8)  | YES     |
| MSG_MOVE_START_STRAFE_RIGHT                          | 185 (0x00B9)  | YES     |
| MSG_MOVE_START_SWIM                                  | 202 (0x00CA)  | YES     |
| MSG_MOVE_START_SWIM_CHEAT                            | 833 (0x0341)  | YES     |
| MSG_MOVE_START_TURN_LEFT                             | 188 (0x00BC)  | YES     |
| MSG_MOVE_START_TURN_RIGHT                            | 189 (0x00BD)  | YES     |
| MSG_MOVE_STOP                                        | 183 (0x00B7)  | YES     |
| MSG_MOVE_STOP_ASCEND                                 | 858 (0x035A)  | YES     |
| MSG_MOVE_STOP_PITCH                                  | 193 (0x00C1)  | YES     |
| MSG_MOVE_STOP_STRAFE                                 | 186 (0x00BA)  | YES     |
| MSG_MOVE_STOP_SWIM                                   | 203 (0x00CB)  | YES     |
| MSG_MOVE_STOP_SWIM_CHEAT                             | 834 (0x0342)  | YES     |
| MSG_MOVE_STOP_TURN                                   | 190 (0x00BE)  | YES     |
| MSG_MOVE_TELEPORT                                    | 197 (0x00C5)  | YES     |
| MSG_MOVE_TELEPORT_ACK                                | 199 (0x00C7)  | YES     |
| MSG_MOVE_TELEPORT_CHEAT                              | 198 (0x00C6)  | NO      |
| MSG_MOVE_TIME_SKIPPED                                | 793 (0x0319)  | NO      |
| MSG_MOVE_TOGGLE_COLLISION_CHEAT                      | 217 (0x00D9)  | YES     |
| MSG_MOVE_TOGGLE_FALL_LOGGING                         | 200 (0x00C8)  | NO      |
| MSG_MOVE_TOGGLE_LOGGING                              | 196 (0x00C4)  | NO      |
| MSG_MOVE_UNROOT                                      | 237 (0x00ED)  | YES     |
| MSG_MOVE_UPDATE_CAN_FLY                              | 941 (0x03AD)  | YES     |
| MSG_MOVE_UPDATE_CAN_TRANSITION_BETWEEN_SWIM_AND_FLY  | 842 (0x034A)  | YES     |
| MSG_MOVE_WATER_WALK                                  | 689 (0x02B1)  | YES     |
| MSG_MOVE_WORLDPORT_ACK                               | 220 (0x00DC)  | NO      |
| MSG_NOTIFY_PARTY_SQUELCH                             | 991 (0x03DF)  | NO      |
| MSG_PARTY_ASSIGNMENT                                 | 910 (0x038E)  | NO      |
| MSG_PETITION_DECLINE                                 | 450 (0x01C2)  | YES     |
| MSG_PETITION_RENAME                                  | 705 (0x02C1)  | YES     |
| MSG_PVP_LOG_DATA                                     | 736 (0x02E0)  | YES     |
| MSG_QUERY_GUILD_BANK_TEXT                            | 1034 (0x040A) | YES     |
| MSG_QUERY_NEXT_MAIL_TIME                             | 644 (0x0284)  | YES     |
| MSG_QUEST_PUSH_RESULT                                | 630 (0x0276)  | YES     |
| MSG_RAID_READY_CHECK                                 | 802 (0x0322)  | YES     |
| MSG_RAID_READY_CHECK_CONFIRM                         | 942 (0x03AE)  | YES     |
| MSG_RAID_READY_CHECK_FINISHED                        | 966 (0x03C6)  | YES     |
| MSG_RAID_TARGET_UPDATE                               | 801 (0x0321)  | YES     |
| MSG_RANDOM_ROLL                                      | 507 (0x01FB)  | YES     |
| MSG_SAVE_GUILD_EMBLEM                                | 497 (0x01F1)  | YES     |
| MSG_SET_DUNGEON_DIFFICULTY                           | 809 (0x0329)  | YES     |
| MSG_SET_RAID_DIFFICULTY                              | 1259 (0x04EB) | NO      |
| MSG_TABARDVENDOR_ACTIVATE                            | 498 (0x01F2)  | YES     |
| MSG_TALENT_WIPE_CONFIRM                              | 682 (0x02AA)  | YES     |
| MSG_VIEW_PHASE_SHIFT                                 | 1273 (0x04F9) | NO      |
| SMSG_ADD_RUNE_POWER                                  | 1160 (0x0488) | NO      |
| SMSG_AFK_MONITOR_INFO_RESPONSE                       | 1284 (0x0504) | NO      |
| SMSG_AREA_TRIGGER_NO_CORPSE                          | 1286 (0x0506) | NO      |
| SMSG_ARENA_ERROR                                     | 886 (0x0376)  | NO      |
| SMSG_ARENA_OPPONENT_UPDATE                           | 1223 (0x04C7) | NO      |
| SMSG_ARENA_TEAM_CHANGE_FAILED_QUEUED                 | 1224 (0x04C8) | NO      |
| SMSG_ARENA_TEAM_QUERY_RESPONSE                       | 844 (0x034C)  | YES     |
| SMSG_ATTACKSWING_BADFACING                           | 326 (0x0146)  | YES     |
| SMSG_ATTACKSWING_CANT_ATTACK                         | 329 (0x0149)  | YES     |
| SMSG_ATTACKSWING_DEADTARGET                          | 328 (0x0148)  | YES     |
| SMSG_ATTACKSWING_NOTINRANGE                          | 325 (0x0145)  | YES     |
| SMSG_AUCTION_BIDDER_NOTIFICATION                     | 606 (0x025E)  | YES     |
| SMSG_AUCTION_LIST_BIDDED_ITEMS_RESULT                | 613 (0x0265)  | YES     |
| SMSG_AUCTION_LIST_OWNED_ITEMS_RESULT                 | 605 (0x025D)  | YES     |
| SMSG_AUCTION_LIST_PENDING_SALES                      | 1168 (0x0490) | NO      |
| SMSG_AUCTION_OWNER_NOTIFICATION                      | 607 (0x025F)  | YES     |
| SMSG_AURACASTLOG                                     | 465 (0x01D1)  | NO      |
| SMSG_AUTH_SRP6_RESPONSE                              | 57 (0x0039)   | NO      |
| SMSG_AVAILABLE_VOICE_CHANNEL                         | 986 (0x03DA)  | NO      |
| SMSG_BARBER_SHOP_RESULT                              | 1064 (0x0428) | NO      |
| SMSG_BATTLEFIELD_MGR_EJECTED                         | 1254 (0x04E6) | NO      |
| SMSG_BATTLEFIELD_MGR_EJECT_PENDING                   | 1253 (0x04E5) | NO      |
| SMSG_BATTLEFIELD_MGR_ENTERING                        | 1248 (0x04E0) | NO      |
| SMSG_BATTLEFIELD_MGR_ENTRY_INVITE                    | 1246 (0x04DE) | NO      |
| SMSG_BATTLEFIELD_MGR_QUEUE_INVITE                    | 1249 (0x04E1) | NO      |
| SMSG_BATTLEFIELD_MGR_QUEUE_REQUEST_RESPONSE          | 1252 (0x04E4) | NO      |
| SMSG_BATTLEFIELD_MGR_STATE_CHANGE                    | 1256 (0x04E8) | NO      |
| SMSG_BATTLEFIELD_PORT_DENIED                         | 331 (0x014B)  | NO      |
| SMSG_BATTLEGROUND_INFO_THROTTLED                     | 1190 (0x04A6) | NO      |
| SMSG_BINDZONEREPLY                                   | 343 (0x0157)  | NO      |
| SMSG_BREAK_TARGET                                    | 338 (0x0152)  | NO      |
| SMSG_BUY_BANK_SLOT_RESULT                            | 442 (0x01BA)  | NO      |
| SMSG_CALENDAR_ARENA_TEAM                             | 1081 (0x0439) | NO      |
| SMSG_CALENDAR_CLEAR_PENDING_ACTION                   | 1211 (0x04BB) | NO      |
| SMSG_CALENDAR_COMMAND_RESULT                         | 1085 (0x043D) | NO      |
| SMSG_CALENDAR_EVENT_INVITE                           | 1082 (0x043A) | NO      |
| SMSG_CALENDAR_EVENT_INVITE_ALERT                     | 1088 (0x0440) | NO      |
| SMSG_CALENDAR_EVENT_INVITE_NOTES                     | 1120 (0x0460) | NO      |
| SMSG_CALENDAR_EVENT_INVITE_NOTES_ALERT               | 1121 (0x0461) | NO      |
| SMSG_CALENDAR_EVENT_INVITE_REMOVED                   | 1083 (0x043B) | NO      |
| SMSG_CALENDAR_EVENT_INVITE_REMOVED_ALERT             | 1089 (0x0441) | NO      |
| SMSG_CALENDAR_EVENT_INVITE_STATUS_ALERT              | 1090 (0x0442) | NO      |
| SMSG_CALENDAR_EVENT_MODERATOR_STATUS_ALERT           | 1093 (0x0445) | NO      |
| SMSG_CALENDAR_EVENT_REMOVED_ALERT                    | 1091 (0x0443) | NO      |
| SMSG_CALENDAR_EVENT_STATUS                           | 1084 (0x043C) | NO      |
| SMSG_CALENDAR_EVENT_UPDATED_ALERT                    | 1092 (0x0444) | NO      |
| SMSG_CALENDAR_FILTER_GUILD                           | 1080 (0x0438) | NO      |
| SMSG_CALENDAR_RAID_LOCKOUT_ADDED                     | 1086 (0x043E) | NO      |
| SMSG_CALENDAR_RAID_LOCKOUT_REMOVED                   | 1087 (0x043F) | NO      |
| SMSG_CALENDAR_RAID_LOCKOUT_UPDATED                   | 1137 (0x0471) | NO      |
| SMSG_CALENDAR_SEND_CALENDAR                          | 1078 (0x0436) | NO      |
| SMSG_CALENDAR_SEND_EVENT                             | 1079 (0x0437) | NO      |
| SMSG_CALENDAR_SEND_NUM_PENDING                       | 1096 (0x0448) | NO      |
| SMSG_CAMERA_SHAKE                                    | 1290 (0x050A) | NO      |
| SMSG_CHANGE_PLAYER_DIFFICULTY_RESULT                 | 526 (0x020E)  | NO      |
| SMSG_CHARACTER_PROFILE                               | 824 (0x0338)  | NO      |
| SMSG_CHARACTER_PROFILE_REALM_CONNECTED               | 825 (0x0339)  | NO      |
| SMSG_CHAR_CUSTOMIZE                                  | 1140 (0x0474) | NO      |
| SMSG_CHAR_FACTION_CHANGE_RESULT                      | 1242 (0x04DA) | NO      |
| SMSG_CHAT_NOT_IN_PARTY                               | 665 (0x0299)  | NO      |
| SMSG_CHAT_PLAYER_AMBIGUOUS                           | 813 (0x032D)  | NO      |
| SMSG_CHAT_RESTRICTED                                 | 765 (0x02FD)  | NO      |
| SMSG_CHAT_WRONG_FACTION                              | 537 (0x0219)  | NO      |
| SMSG_CHEAT_DUMP_ITEMS_DEBUG_ONLY_RESPONSE            | 923 (0x039B)  | NO      |
| SMSG_CHEAT_DUMP_ITEMS_DEBUG_ONLY_RESPONSE_WRITE_FILE | 924 (0x039C)  | NO      |
| SMSG_CHEAT_PLAYER_LOOKUP                             | 964 (0x03C4)  | NO      |
| SMSG_CHECK_FOR_BOTS                                  | 21 (0x0015)   | NO      |
| SMSG_CLEAR_EXTRA_AURA_INFO_OBSOLETE                  | 934 (0x03A6)  | NO      |
| SMSG_CLEAR_FAR_SIGHT_IMMEDIATE                       | 525 (0x020D)  | NO      |
| SMSG_CLEAR_TARGET                                    | 959 (0x03BF)  | NO      |
| SMSG_COMBAT_EVENT_FAILED                             | 609 (0x0261)  | NO      |
| SMSG_COMBAT_LOG_MULTIPLE                             | 1300 (0x0514) | NO      |
| SMSG_COMMENTATOR_GET_PLAYER_INFO                     | 954 (0x03BA)  | NO      |
| SMSG_COMMENTATOR_MAP_INFO                            | 952 (0x03B8)  | NO      |
| SMSG_COMMENTATOR_PLAYER_INFO                         | 955 (0x03BB)  | NO      |
| SMSG_COMMENTATOR_SKIRMISH_QUEUE_RESULT1              | 1308 (0x051C) | NO      |
| SMSG_COMMENTATOR_SKIRMISH_QUEUE_RESULT2              | 1309 (0x051D) | NO      |
| SMSG_COMMENTATOR_STATE_CHANGED                       | 950 (0x03B6)  | NO      |
| SMSG_COMPLAINT_RESULT                                | 968 (0x03C8)  | NO      |
| SMSG_COMPRESSED_MOVES                                | 763 (0x02FB)  | YES     |
| SMSG_COMSAT_CONNECT_FAIL                             | 994 (0x03E2)  | NO      |
| SMSG_COMSAT_DISCONNECT                               | 993 (0x03E1)  | NO      |
| SMSG_COMSAT_RECONNECT_TRY                            | 992 (0x03E0)  | NO      |
| SMSG_CONVERT_RUNE                                    | 1158 (0x0486) | NO      |
| SMSG_CORPSE_MAP_POSITION_QUERY_RESPONSE              | 1207 (0x04B7) | NO      |
| SMSG_CROSSED_INEBRIATION_THRESHOLD                   | 961 (0x03C1)  | NO      |
| SMSG_DAMAGE_CALC_LOG                                 | 636 (0x027C)  | NO      |
| SMSG_DANCE_QUERY_RESPONSE                            | 1106 (0x0452) | NO      |
| SMSG_DBLOOKUP                                        | 3 (0x0003)    | NO      |
| SMSG_DEBUGAURAPROC                                   | 589 (0x024D)  | NO      |
| SMSG_DEBUG_AISTATE                                   | 47 (0x002F)   | NO      |
| SMSG_DEBUG_LIST_TARGETS                              | 985 (0x03D9)  | NO      |
| SMSG_DEBUG_SERVER_GEO                                | 1276 (0x04FC) | NO      |
| SMSG_DESTRUCTIBLE_BUILDING_DAMAGE                    | 50 (0x0032)   | NO      |
| SMSG_DISMOUNT                                        | 940 (0x03AC)  | NO      |
| SMSG_DISMOUNT_RESULT                                 | 367 (0x016F)  | NO      |
| SMSG_DISPEL_FAILED                                   | 610 (0x0262)  | NO      |
| SMSG_DUMP_OBJECTS_DATA                               | 1164 (0x048C) | NO      |
| SMSG_DYNAMIC_DROP_ROLL_RESULT                        | 1129 (0x0469) | NO      |
| SMSG_ECHO_PARTY_SQUELCH                              | 1014 (0x03F6) | NO      |
| SMSG_ENABLE_BARBER_SHOP                              | 1063 (0x0427) | NO      |
| SMSG_EQUIPMENT_SET_ID                                | 311 (0x0137)  | NO      |
| SMSG_FEIGN_DEATH_RESISTED                            | 692 (0x02B4)  | NO      |
| SMSG_FLIGHT_SPLINE_SYNC                              | 904 (0x0388)  | NO      |
| SMSG_FORCEACTIONSHOW                                 | 27 (0x001B)   | NO      |
| SMSG_FORCED_DEATH_UPDATE                             | 890 (0x037A)  | NO      |
| SMSG_FORCE_ANIM                                      | 1240 (0x04D8) | NO      |
| SMSG_FORCE_DISPLAY_UPDATE                            | 1027 (0x0403) | NO      |
| SMSG_FORCE_FLIGHT_BACK_SPEED_CHANGE                  | 899 (0x0383)  | YES     |
| SMSG_FORCE_PITCH_RATE_CHANGE                         | 1116 (0x045C) | YES     |
| SMSG_FORCE_SET_VEHICLE_REC_ID                        | 575 (0x023F)  | NO      |
| SMSG_FORCE_SWIM_BACK_SPEED_CHANGE                    | 732 (0x02DC)  | YES     |
| SMSG_GAMETIMEBIAS_SET                                | 788 (0x0314)  | NO      |
| SMSG_GAME_SPEED_SET                                  | 71 (0x0047)   | NO      |
| SMSG_GAME_TIME_SET                                   | 69 (0x0045)   | NO      |
| SMSG_GAME_TIME_UPDATE                                | 67 (0x0043)   | NO      |
| SMSG_GHOSTEE_GONE                                    | 806 (0x0326)  | NO      |
| SMSG_GMRESPONSE_CREATE_TICKET                        | 1266 (0x04F2) | NO      |
| SMSG_GMRESPONSE_DB_ERROR                             | 1262 (0x04EE) | NO      |
| SMSG_GMRESPONSE_RECEIVED                             | 1263 (0x04EF) | NO      |
| SMSG_GMRESPONSE_STATUS_UPDATE                        | 1265 (0x04F1) | NO      |
| SMSG_GM_PLAYER_INFO                                  | 560 (0x0230)  | NO      |
| SMSG_GM_TICKET_CREATE                                | 518 (0x0206)  | YES     |
| SMSG_GM_TICKET_DELETE_TICKET                         | 536 (0x0218)  | NO      |
| SMSG_GM_TICKET_GET_SYSTEM_STATUS                     | 539 (0x021B)  | NO      |
| SMSG_GM_TICKET_GET_TICKET                            | 530 (0x0212)  | NO      |
| SMSG_GM_TICKET_UPDATE_TEXT                           | 520 (0x0208)  | NO      |
| SMSG_GOD_MODE                                        | 35 (0x0023)   | NO      |
| SMSG_GOGOGO_OBSOLETE                                 | 1013 (0x03F5) | NO      |
| SMSG_GROUP_ACTION_THROTTLED                          | 1041 (0x0411) | NO      |
| SMSG_GROUP_CANCEL                                    | 113 (0x0071)  | NO      |
| SMSG_GROUP_LIST                                      | 125 (0x007D)  | YES     |
| SMSG_GUILD_EVENT                                     | 146 (0x0092)  | YES     |
| SMSG_GUILD_INFO                                      | 136 (0x0088)  | YES     |
| SMSG_IGNORE_DIMINISHING_RETURNS_CHEAT                | 1030 (0x0406) | NO      |
| SMSG_IGNORE_REQUIREMENTS_CHEAT                       | 937 (0x03A9)  | NO      |
| SMSG_INIT_EXTRA_AURA_INFO_OBSOLETE                   | 931 (0x03A3)  | NO      |
| SMSG_INSPECT_TALENT                                  | 1012 (0x03F4) | YES     |
| SMSG_INSTANCE_DIFFICULTY                             | 827 (0x033B)  | YES     |
| SMSG_INVALIDATE_DANCE                                | 1107 (0x0453) | NO      |
| SMSG_INVALID_PROMOTION_CODE                          | 487 (0x01E7)  | NO      |
| SMSG_ITEM_PURCHASE_REFUND_RESULT                     | 1205 (0x04B5) | NO      |
| SMSG_ITEM_QUERY_MULTIPLE_RESPONSE                    | 89 (0x0059)   | NO      |
| SMSG_ITEM_REFUND_INFO_RESPONSE                       | 1202 (0x04B2) | NO      |
| SMSG_ITEM_TIME_UPDATE                                | 490 (0x01EA)  | NO      |
| SMSG_JOINED_BATTLEGROUND_QUEUE                       | 906 (0x038A)  | NO      |
| SMSG_KICK_REASON                                     | 965 (0x03C5)  | NO      |
| SMSG_LEARNED_DANCE_MOVES                             | 1109 (0x0455) | YES     |
| SMSG_LEARNED_SPELL                                   | 299 (0x012B)  | YES     |
| SMSG_LFG_BOOT_PROPOSAL_UPDATE                        | 877 (0x036D)  | NO      |
| SMSG_LFG_LFR_LIST                                    | 864 (0x0360)  | NO      |
| SMSG_LFG_ROLE_CHOSEN                                 | 699 (0x02BB)  | NO      |
| SMSG_LFG_TELEPORT_DENIED                             | 512 (0x0200)  | NO      |
| SMSG_LFG_UPDATE_PARTY                                | 872 (0x0368)  | NO      |
| SMSG_LOOT_ITEM_NOTIFY                                | 356 (0x0164)  | NO      |
| SMSG_LOOT_SLOT_CHANGED                               | 1277 (0x04FD) | NO      |
| SMSG_LOTTERY_QUERY_RESULT_OBSOLETE                   | 821 (0x0335)  | NO      |
| SMSG_LOTTERY_RESULT_OBSOLETE                         | 823 (0x0337)  | NO      |
| SMSG_MINIGAME_MOVE_FAILED                            | 761 (0x02F9)  | NO      |
| SMSG_MINIGAME_SETUP                                  | 758 (0x02F6)  | NO      |
| SMSG_MINIGAME_STATE                                  | 759 (0x02F7)  | NO      |
| SMSG_MIRROR_IMAGE_COMPONENTED_DATA                   | 1026 (0x0402) | NO      |
| SMSG_MODIFY_COOLDOWN                                 | 1169 (0x0491) | NO      |
| SMSG_MOVE_CHARACTER_CHEAT                            | 14 (0x000E)   | NO      |
| SMSG_MOVE_SET_COLLISION_HGT                          | 1302 (0x0516) | NO      |
| SMSG_MOVE_SPLINE_SET_WALK_BACK_SPEED                 | 769 (0x0301)  | YES     |
| SMSG_MULTIPLE_MOVES                                  | 1310 (0x051E) | NO      |
| SMSG_MULTIPLE_PACKETS_2                              | 1229 (0x04CD) | NO      |
| SMSG_NOTIFY_DANCE                                    | 1098 (0x044A) | NO      |
| SMSG_NOTIFY_DEST_LOC_SPELL_CAST                      | 1166 (0x048E) | NO      |
| SMSG_NPC_WONT_TALK                                   | 385 (0x0181)  | NO      |
| SMSG_OFFER_PETITION_ERROR                            | 911 (0x038F)  | NO      |
| SMSG_OPEN_CONTAINER                                  | 275 (0x0113)  | NO      |
| SMSG_OPEN_LFG_DUNGEON_FINDER                         | 1301 (0x0515) | NO      |
| SMSG_PAGE_TEXT                                       | 479 (0x01DF)  | NO      |
| SMSG_PETGODMODE                                      | 29 (0x001D)   | NO      |
| SMSG_PET_DISMISS_SOUND                               | 805 (0x0325)  | NO      |
| SMSG_PET_LEARNED_SPELLS                              | 1177 (0x0499) | NO      |
| SMSG_PET_NAME_INVALID                                | 376 (0x0178)  | NO      |
| SMSG_PET_RENAMEABLE                                  | 1141 (0x0475) | NO      |
| SMSG_PET_UNLEARNED_SPELLS                            | 1178 (0x049A) | NO      |
| SMSG_PET_UPDATE_COMBO_POINTS                         | 1170 (0x0492) | NO      |
| SMSG_PLAYERBINDERROR                                 | 438 (0x01B6)  | NO      |
| SMSG_PLAYER_VEHICLE_DATA                             | 1191 (0x04A7) | NO      |
| SMSG_PLAY_DANCE                                      | 1100 (0x044C) | NO      |
| SMSG_PLAY_SPELL_IMPACT                               | 503 (0x01F7)  | NO      |
| SMSG_PRE_RESSURECT                                   | 1172 (0x0494) | NO      |
| SMSG_PROC_RESIST                                     | 608 (0x0260)  | NO      |
| SMSG_PROFILE_DATA_RESPONSE                           | 1226 (0x04CA) | NO      |
| SMSG_PROPOSE_LEVEL_GRANT                             | 1055 (0x041F) | NO      |
| SMSG_PVP_QUEUE_STATS                                 | 1244 (0x04DC) | NO      |
| SMSG_QUERY_OBJ_POSITION                              | 5 (0x0005)    | NO      |
| SMSG_QUERY_OBJ_ROTATION                              | 7 (0x0007)    | NO      |
| SMSG_QUERY_PLAYER_NAME_RESPONSE                      | 81 (0x0051)   | YES     |
| SMSG_QUERY_QUESTS_COMPLETED_RESPONSE                 | 1281 (0x0501) | NO      |
| SMSG_QUEST_FORCE_REMOVED                             | 542 (0x021E)  | NO      |
| SMSG_QUEST_LOG_FULL                                  | 405 (0x0195)  | NO      |
| SMSG_QUEST_UPDATE_ADD_KILL                           | 409 (0x0199)  | YES     |
| SMSG_QUEST_UPDATE_ADD_PVP_CREDIT                     | 1135 (0x046F) | NO      |
| SMSG_RAID_INSTANCE_INFO                              | 716 (0x02CC)  | YES     |
| SMSG_READY_CHECK_ERROR                               | 1032 (0x0408) | NO      |
| SMSG_REALM_SPLIT                                     | 907 (0x038B)  | NO      |
| SMSG_REFER_A_FRIEND_EXPIRED                          | 30 (0x001E)   | NO      |
| SMSG_REFER_A_FRIEND_FAILURE                          | 1057 (0x0421) | NO      |
| SMSG_REMOVED_FROM_PVP_QUEUE                          | 368 (0x0170)  | NO      |
| SMSG_REPORT_PVP_AFK_RESULT                           | 997 (0x03E5)  | NO      |
| SMSG_RESET_RANGED_COMBAT_TIMER                       | 664 (0x0298)  | NO      |
| SMSG_RESISTLOG                                       | 470 (0x01D6)  | NO      |
| SMSG_RESPOND_INSPECT_ACHIEVEMENTS                    | 1132 (0x046C) | NO      |
| SMSG_RESUME_CAST_BAR                                 | 333 (0x014D)  | NO      |
| SMSG_RESURRECT_FAILED                                | 594 (0x0252)  | NO      |
| SMSG_RESYNC_RUNES                                    | 1159 (0x0487) | NO      |
| SMSG_RWHOIS                                          | 510 (0x01FE)  | NO      |
| SMSG_SCRIPT_MESSAGE                                  | 694 (0x02B6)  | NO      |
| SMSG_SERVERINFO                                      | 1269 (0x04F5) | NO      |
| SMSG_SERVERTIME                                      | 73 (0x0049)   | NO      |
| SMSG_SERVER_BUCK_DATA                                | 1053 (0x041D) | NO      |
| SMSG_SERVER_BUCK_DATA_START                          | 1187 (0x04A3) | NO      |
| SMSG_SERVER_FIRST_ACHIEVEMENT                        | 1176 (0x0498) | NO      |
| SMSG_SERVER_INFO_RESPONSE                            | 1185 (0x04A1) | NO      |
| SMSG_SET_EXTRA_AURA_INFO_NEED_UPDATE_OBSOLETE        | 933 (0x03A5)  | NO      |
| SMSG_SET_EXTRA_AURA_INFO_OBSOLETE                    | 932 (0x03A4)  | NO      |
| SMSG_SET_FACTION_AT_WAR                              | 787 (0x0313)  | NO      |
| SMSG_SET_PROJECTILE_POSITION                         | 1215 (0x04BF) | NO      |
| SMSG_SHOW_BANK                                       | 440 (0x01B8)  | YES     |
| SMSG_SHOW_MAILBOX                                    | 663 (0x0297)  | NO      |
| SMSG_SOCKET_GEMS                                     | 1291 (0x050B) | NO      |
| SMSG_SPELL_BREAK_LOG                                 | 335 (0x014F)  | NO      |
| SMSG_SPELL_CHANCE_PROC_LOG                           | 938 (0x03AA)  | NO      |
| SMSG_SPELL_CHANCE_RESIST_PUSHBACK                    | 1028 (0x0404) | NO      |
| SMSG_SPELL_EXECUTE_LOG                               | 588 (0x024C)  | NO      |
| SMSG_SPELL_MISS_LOG                                  | 587 (0x024B)  | NO      |
| SMSG_SPELL_OR_DAMAGE_IMMUNE                          | 611 (0x0263)  | NO      |
| SMSG_SPELL_STEAL_LOG                                 | 819 (0x0333)  | NO      |
| SMSG_SPIRIT_HEALER_CONFIRM                           | 546 (0x0222)  | YES     |
| SMSG_STOP_DANCE                                      | 1103 (0x044F) | NO      |
| SMSG_SUMMON_CANCEL                                   | 1060 (0x0424) | NO      |
| SMSG_TALENTS_INVOLUNTARILY_RESET                     | 1274 (0x04FA) | NO      |
| SMSG_TEST_DROP_RATE_RESULT                           | 661 (0x0295)  | NO      |
| SMSG_TOGGLE_XP_GAIN                                  | 1261 (0x04ED) | NO      |
| SMSG_TRADE_STATUS_EXTENDED                           | 289 (0x0121)  | YES     |
| SMSG_TRAINER_BUY_SUCCEEDED                           | 435 (0x01B3)  | NO      |
| SMSG_TRIGGER_MOVIE                                   | 1124 (0x0464) | NO      |
| SMSG_UI_TIME                                         | 1271 (0x04F7) | NO      |
| SMSG_UPDATE_ACCOUNT_DATA_COMPLETE                    | 1123 (0x0463) | NO      |
| SMSG_UPDATE_INSTANCE_ENCOUNTER_UNIT                  | 532 (0x0214)  | NO      |
| SMSG_USERLIST_ADD                                    | 1008 (0x03F0) | NO      |
| SMSG_USERLIST_REMOVE                                 | 1009 (0x03F1) | NO      |
| SMSG_USERLIST_UPDATE                                 | 1010 (0x03F2) | NO      |
| SMSG_USE_EQUIPMENT_SET_RESULT                        | 1238 (0x04D6) | NO      |
| SMSG_VOICESESSION_FULL                               | 1020 (0x03FC) | NO      |
| SMSG_VOICE_CHAT_STATUS                               | 995 (0x03E3)  | NO      |
| SMSG_VOICE_PARENTAL_CONTROLS                         | 945 (0x03B1)  | NO      |
| SMSG_VOICE_SESSION_ADJUST_PRIORITY                   | 928 (0x03A0)  | NO      |
| SMSG_VOICE_SESSION_ENABLE                            | 944 (0x03B0)  | NO      |
| SMSG_VOICE_SESSION_LEAVE                             | 927 (0x039F)  | NO      |
| SMSG_VOICE_SESSION_ROSTER_UPDATE                     | 926 (0x039E)  | NO      |
| SMSG_VOICE_SET_TALKER_MUTED                          | 930 (0x03A2)  | NO      |
| SMSG_WARDEN_DATA                                     | 742 (0x02E6)  | NO      |
| SMSG_WHO_IS                                          | 101 (0x0065)  | NO      |
| SMSG_ZONE_MAP                                        | 11 (0x000B)   | NO      |
| UMSG_DELETE_GUILD_CHARTER                            | 704 (0x02C0)  | NO      |
| UMSG_UPDATE_GROUP_INFO                               | 1278 (0x04FE) | NO      |
| UMSG_UPDATE_GROUP_MEMBERS                            | 128 (0x0080)  | NO      |
| UMSG_UPDATE_GUILD                                    | 148 (0x0094)  | NO      |

## 3. Opcodes ONLY in Modern (3.4.3) - Not in Legacy

Count: 295

These opcodes exist in the modern client but have no legacy server equivalent.

| Opcode                                         | Modern Value   | Handled | In Universal |
|------------------------------------------------|----------------|---------|--------------|
| CMSG_ADDON_LIST                                | 13784 (0x35D8) | NO      | YES          |
| CMSG_AUCTION_HELLO_REQUEST                     | 13514 (0x34CA) | YES     | YES          |
| CMSG_BATTLEFIELD_PORT                          | 13605 (0x3525) | YES     | YES          |
| CMSG_BATTLEMASTER_JOIN_SKIRMISH                | 13602 (0x3522) | YES     | YES          |
| CMSG_BATTLENET_REQUEST                         | 14077 (0x36FD) | YES     | YES          |
| CMSG_BATTLE_PAY_GET_PRODUCT_LIST               | 14020 (0x36C4) | YES     | YES          |
| CMSG_BATTLE_PAY_GET_PURCHASE_LIST              | 14021 (0x36C5) | YES     | YES          |
| CMSG_BATTLE_PET_REQUEST_JOURNAL                | 13861 (0x3625) | YES     | YES          |
| CMSG_CANCEL_QUEUED_SPELL                       | 12674 (0x3182) | NO      | YES          |
| CMSG_CAN_DUEL                                  | 13924 (0x3664) | YES     | YES          |
| CMSG_CHANGE_REALM_TICKET                       | 14081 (0x3701) | YES     | YES          |
| CMSG_CHAT_ADDON_MESSAGE                        | 14318 (0x37EE) | YES     | YES          |
| CMSG_CHAT_IGNORED                              | 0 (0x0000)     | NO      | **NO**       |
| CMSG_CHAT_MESSAGE_AFK                          | 14291 (0x37D3) | YES     | YES          |
| CMSG_CHAT_MESSAGE_CHANNEL                      | 14287 (0x37CF) | YES     | YES          |
| CMSG_CHAT_MESSAGE_DND                          | 14292 (0x37D4) | YES     | YES          |
| CMSG_CHAT_MESSAGE_EMOTE                        | 14312 (0x37E8) | YES     | YES          |
| CMSG_CHAT_MESSAGE_GUILD                        | 14289 (0x37D1) | YES     | YES          |
| CMSG_CHAT_MESSAGE_INSTANCE_CHAT                | 14316 (0x37EC) | YES     | YES          |
| CMSG_CHAT_MESSAGE_OFFICER                      | 14290 (0x37D2) | YES     | YES          |
| CMSG_CHAT_MESSAGE_PARTY                        | 14314 (0x37EA) | YES     | YES          |
| CMSG_CHAT_MESSAGE_RAID                         | 14315 (0x37EB) | YES     | YES          |
| CMSG_CHAT_MESSAGE_RAID_WARNING                 | 14317 (0x37ED) | YES     | YES          |
| CMSG_CHAT_MESSAGE_SAY                          | 14311 (0x37E7) | YES     | YES          |
| CMSG_CHAT_MESSAGE_WHISPER                      | 14288 (0x37D0) | YES     | YES          |
| CMSG_CHAT_MESSAGE_YELL                         | 14313 (0x37E9) | YES     | YES          |
| CMSG_CHAT_REGISTER_ADDON_PREFIXES              | 14285 (0x37CD) | YES     | YES          |
| CMSG_CHAT_UNREGISTER_ALL_ADDON_PREFIXES        | 14286 (0x37CE) | YES     | YES          |
| CMSG_CLOSE_INTERACTION                         | 13459 (0x3493) | YES     | YES          |
| CMSG_CONFIRM_RESPEC_WIPE                       | 12813 (0x320D) | YES     | YES          |
| CMSG_DB_QUERY_BULK                             | 13797 (0x35E5) | YES     | YES          |
| CMSG_DECLINE_GUILD_INVITES                     | 13597 (0x351D) | YES     | YES          |
| CMSG_DECLINE_PETITION                          | 13620 (0x3534) | YES     | YES          |
| CMSG_DF_GET_SYSTEM_INFO                        | 13845 (0x3615) | YES     | YES          |
| CMSG_DF_JOIN                                   | 13835 (0x360B) | YES     | YES          |
| CMSG_DF_LEAVE                                  | 13844 (0x3614) | YES     | YES          |
| CMSG_DF_PROPOSAL_RESPONSE                      | 13833 (0x3609) | YES     | YES          |
| CMSG_DF_SET_ROLES                              | 13847 (0x3617) | NO      | YES          |
| CMSG_DISCARDED_TIME_SYNC_ACKS                  | 14913 (0x3A41) | YES     | YES          |
| CMSG_DO_READY_CHECK                            | 13877 (0x3635) | YES     | YES          |
| CMSG_DUEL_RESPONSE                             | 13538 (0x34E2) | YES     | YES          |
| CMSG_ENABLE_NAGLE                              | 14187 (0x376B) | NO      | YES          |
| CMSG_ENTER_ENCRYPTED_MODE_ACK                  | 14183 (0x3767) | NO      | YES          |
| CMSG_GENERATE_RANDOM_CHARACTER_NAME            | 13800 (0x35E8) | YES     | YES          |
| CMSG_GET_ACCOUNT_CHARACTER_LIST                | 14015 (0x36BF) | YES     | YES          |
| CMSG_GET_UNDELETE_CHARACTER_COOLDOWN_STATUS    | 14055 (0x36E7) | YES     | YES          |
| CMSG_GM_TICKET_GET_CASE_STATUS                 | 13967 (0x368F) | YES     | YES          |
| CMSG_GROUP_REMOVE_LEADER                       | 0 (0x0000)     | NO      | **NO**       |
| CMSG_GUILD_AUTO_DECLINE_INVITATION             | 12385 (0x3061) | YES     | YES          |
| CMSG_GUILD_BANK_LOG_QUERY                      | 12418 (0x3082) | YES     | YES          |
| CMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY_QUERY | 12419 (0x3083) | YES     | YES          |
| CMSG_GUILD_BANK_TEXT_QUERY                     | 12423 (0x3087) | YES     | YES          |
| CMSG_GUILD_EVENT_LOG_QUERY                     | 12421 (0x3085) | NO      | YES          |
| CMSG_GUILD_OFFICER_LIST                        | 0 (0x0000)     | NO      | **NO**       |
| CMSG_GUILD_SET_ACHIEVEMENT_TRACKING            | 12399 (0x306F) | YES     | YES          |
| CMSG_GUILD_SET_MEMBER_NOTE                     | 12402 (0x3072) | YES     | YES          |
| CMSG_HOTFIX_REQUEST                            | 13798 (0x35E6) | YES     | YES          |
| CMSG_INSPECT_PVP                               | 13987 (0x36A3) | YES     | YES          |
| CMSG_LEARN_TALENT_GROUP                        | 0 (0x0000)     | NO      | **NO**       |
| CMSG_LEAVE_GROUP                               | 13900 (0x364C) | YES     | YES          |
| CMSG_LFG_LIST_GET_STATUS                       | 13837 (0x360D) | YES     | YES          |
| CMSG_LOADING_SCREEN_NOTIFY                     | 13817 (0x35F9) | YES     | YES          |
| CMSG_LOG_DISCONNECT                            | 14185 (0x3769) | NO      | YES          |
| CMSG_LOOT_CURRENCY                             | 0 (0x0000)     | NO      | YES          |
| CMSG_LOOT_ITEM                                 | 12817 (0x3211) | YES     | YES          |
| CMSG_MINIMAP_PING                              | 13902 (0x364E) | YES     | YES          |
| CMSG_MOVE_DISMISS_VEHICLE                      | 14899 (0x3A33) | YES     | YES          |
| CMSG_MOVE_DOUBLE_JUMP                          | 14827 (0x39EB) | YES     | YES          |
| CMSG_MOVE_FALL_LAND                            | 14843 (0x39FB) | YES     | YES          |
| CMSG_MOVE_HEARTBEAT                            | 14864 (0x3A10) | YES     | YES          |
| CMSG_MOVE_INIT_ACTIVE_MOVER_COMPLETE           | 14918 (0x3A46) | YES     | YES          |
| CMSG_MOVE_JUMP                                 | 14826 (0x39EA) | YES     | YES          |
| CMSG_MOVE_REMOVE_MOVEMENT_FORCES               | 14871 (0x3A17) | YES     | YES          |
| CMSG_MOVE_SET_FACING                           | 14857 (0x3A09) | YES     | YES          |
| CMSG_MOVE_SET_FACING_HEARTBEAT                 | 14943 (0x3A5F) | YES     | YES          |
| CMSG_MOVE_SET_FLIGHT_SPEED_CHEAT               | 0 (0x0000)     | NO      | **NO**       |
| CMSG_MOVE_SET_PITCH                            | 14858 (0x3A0A) | YES     | YES          |
| CMSG_MOVE_SET_RUN_MODE                         | 14834 (0x39F2) | YES     | YES          |
| CMSG_MOVE_SET_WALK_MODE                        | 14835 (0x39F3) | YES     | YES          |
| CMSG_MOVE_START_ASCEND                         | 14889 (0x3A29) | YES     | YES          |
| CMSG_MOVE_START_BACKWARD                       | 14821 (0x39E5) | YES     | YES          |
| CMSG_MOVE_START_DESCEND                        | 14896 (0x3A30) | YES     | YES          |
| CMSG_MOVE_START_FORWARD                        | 14820 (0x39E4) | YES     | YES          |
| CMSG_MOVE_START_PITCH_DOWN                     | 14832 (0x39F0) | YES     | YES          |
| CMSG_MOVE_START_PITCH_UP                       | 14831 (0x39EF) | YES     | YES          |
| CMSG_MOVE_START_STRAFE_LEFT                    | 14823 (0x39E7) | YES     | YES          |
| CMSG_MOVE_START_STRAFE_RIGHT                   | 14824 (0x39E8) | YES     | YES          |
| CMSG_MOVE_START_SWIM                           | 14844 (0x39FC) | YES     | YES          |
| CMSG_MOVE_START_TURN_LEFT                      | 14828 (0x39EC) | YES     | YES          |
| CMSG_MOVE_START_TURN_RIGHT                     | 14829 (0x39ED) | YES     | YES          |
| CMSG_MOVE_STOP                                 | 14822 (0x39E6) | YES     | YES          |
| CMSG_MOVE_STOP_ASCEND                          | 14890 (0x3A2A) | YES     | YES          |
| CMSG_MOVE_STOP_BACKWARD                        | 0 (0x0000)     | NO      | **NO**       |
| CMSG_MOVE_STOP_PITCH                           | 14833 (0x39F1) | YES     | YES          |
| CMSG_MOVE_STOP_STRAFE                          | 14825 (0x39E9) | YES     | YES          |
| CMSG_MOVE_STOP_SWIM                            | 14845 (0x39FD) | YES     | YES          |
| CMSG_MOVE_STOP_TURN                            | 14830 (0x39EE) | YES     | YES          |
| CMSG_MOVE_TELEPORT_ACK                         | 14842 (0x39FA) | YES     | YES          |
| CMSG_OBJECT_UPDATE_FAILED                      | 12675 (0x3183) | YES     | YES          |
| CMSG_OVERRIDE_SCREEN_FLASH                     | 13598 (0x351E) | YES     | YES          |
| CMSG_PARTY_INVITE_RESPONSE                     | 13830 (0x3606) | YES     | YES          |
| CMSG_PARTY_UNINVITE                            | 13898 (0x364A) | YES     | YES          |
| CMSG_PETITION_RENAME_GUILD                     | 14033 (0x36D1) | YES     | YES          |
| CMSG_PLAYER_DIFFICULTY_CHANGE                  | 0 (0x0000)     | NO      | YES          |
| CMSG_PUSH_SPELL_TO_ACTION_BAR                  | 0 (0x0000)     | NO      | **NO**       |
| CMSG_PVP_LOG_DATA                              | 12671 (0x317F) | YES     | YES          |
| CMSG_QUERY_CORPSE_LOCATION_FROM_CLIENT         | 13922 (0x3662) | YES     | YES          |
| CMSG_QUERY_COUNTDOWN_TIMER                     | 12714 (0x31AA) | YES     | YES          |
| CMSG_QUERY_NEXT_MAIL_TIME                      | 13625 (0x3539) | YES     | YES          |
| CMSG_QUERY_PLAYER_NAMES                        | 14194 (0x3772) | YES     | YES          |
| CMSG_QUEST_GIVER_CLOSE_QUEST                   | 13641 (0x3549) | YES     | YES          |
| CMSG_QUEST_PUSH_RESULT                         | 13472 (0x34A0) | YES     | YES          |
| CMSG_QUEUED_MESSAGES_END                       | 14188 (0x376C) | YES     | YES          |
| CMSG_RAID_READY_CHECK                          | 0 (0x0000)     | NO      | **NO**       |
| CMSG_RAID_READY_CHECK_CONFIRM                  | 0 (0x0000)     | NO      | **NO**       |
| CMSG_RAID_READY_CHECK_FINISHED                 | 0 (0x0000)     | NO      | **NO**       |
| CMSG_RANDOM_ROLL                               | 13911 (0x3657) | YES     | YES          |
| CMSG_READY_CHECK_RESPONSE                      | 13878 (0x3636) | YES     | YES          |
| CMSG_REAL_GROUP_UPDATE                         | 0 (0x0000)     | NO      | **NO**       |
| CMSG_REPORT_CLIENT_VARIABLES                   | 14087 (0x3707) | YES     | YES          |
| CMSG_REPORT_ENABLED_ADDONS                     | 14086 (0x3706) | YES     | YES          |
| CMSG_REPORT_KEYBINDING_EXECUTION_COUNTS        | 14088 (0x3708) | YES     | YES          |
| CMSG_REQUEST_BATTLEFIELD_STATUS                | 13789 (0x35DD) | YES     | YES          |
| CMSG_REQUEST_CEMETERY_LIST                     | 12665 (0x3179) | YES     | YES          |
| CMSG_REQUEST_CONQUEST_FORMULA_CONSTANTS        | 12980 (0x32B4) | YES     | YES          |
| CMSG_REQUEST_FORCED_REACTIONS                  | 12805 (0x3205) | YES     | YES          |
| CMSG_REQUEST_LFG_LIST_BLACKLIST                | 12964 (0x32A4) | YES     | YES          |
| CMSG_REQUEST_PVP_REWARDS                       | 12694 (0x3196) | YES     | YES          |
| CMSG_REQUEST_RATED_PVP_INFO                    | 13796 (0x35E4) | YES     | YES          |
| CMSG_REQUEST_STABLED_PETS                      | 13457 (0x3491) | YES     | YES          |
| CMSG_SAVE_CUF_PROFILES                         | 12686 (0x318E) | YES     | YES          |
| CMSG_SAVE_GUILD_EMBLEM                         | 12968 (0x32A8) | YES     | YES          |
| CMSG_SEND_CONTACT_LIST                         | 14039 (0x36D7) | NO      | **NO**       |
| CMSG_SERVER_TIME_OFFSET_REQUEST                | 13980 (0x369C) | NO      | YES          |
| CMSG_SET_ACTIONBAR_TOGGLES                     | 0 (0x0000)     | NO      | **NO**       |
| CMSG_SET_DIFFICULTY_ID                         | 12834 (0x3222) | NO      | YES          |
| CMSG_SET_DUNGEON_DIFFICULTY                    | 13956 (0x3684) | YES     | YES          |
| CMSG_SET_EVERYONE_IS_ASSISTANT                 | 13850 (0x361A) | YES     | YES          |
| CMSG_SET_FACTION_ATWAR                         | 0 (0x0000)     | NO      | **NO**       |
| CMSG_SET_FACTION_NOT_AT_WAR                    | 13535 (0x34DF) | YES     | YES          |
| CMSG_SET_PVP                                   | 12972 (0x32AC) | YES     | YES          |
| CMSG_SOCIAL_CONTRACT_REQUEST                   | 14156 (0x374C) | NO      | YES          |
| CMSG_SOCKETSPELLS                              | 0 (0x0000)     | NO      | **NO**       |
| CMSG_SORT_BAGS                                 | 0 (0x0000)     | NO      | YES          |
| CMSG_TABARD_VENDOR_ACTIVATE                    | 12969 (0x32A9) | NO      | YES          |
| CMSG_UPDATE_RAID_TARGET                        | 13907 (0x3653) | YES     | YES          |
| CMSG_UPDATE_VAS_PURCHASE_STATES                | 14075 (0x36FB) | YES     | YES          |
| CMSG_VIOLENCE_LEVEL                            | 12679 (0x3187) | YES     | YES          |
| CMSG_WHOIS                                     | 0 (0x0000)     | NO      | **NO**       |
| CMSG_WORLD_PORT_RESPONSE                       | 13818 (0x35FA) | YES     | YES          |
| SMSG_ACCOUNT_HEIRLOOM_UPDATE                   | 9649 (0x25B1)  | NO      | YES          |
| SMSG_ACCOUNT_MOUNT_UPDATE                      | 9646 (0x25AE)  | NO      | YES          |
| SMSG_ACCOUNT_TOY_UPDATE                        | 9648 (0x25B0)  | NO      | YES          |
| SMSG_ACTIVE_GLYPHS                             | 11345 (0x2C51) | NO      | YES          |
| SMSG_ALL_ACCOUNT_CRITERIA                      | 9585 (0x2571)  | NO      | YES          |
| SMSG_ATTACK_SWING_ERROR                        | 10572 (0x294C) | NO      | YES          |
| SMSG_AUCTION_CLOSED_NOTIFICATION               | 9971 (0x26F3)  | NO      | YES          |
| SMSG_AUCTION_HELLO_RESPONSE                    | 9966 (0x26EE)  | NO      | YES          |
| SMSG_AUCTION_LIST_BIDDER_ITEMS_RESULT          | 10365 (0x287D) | NO      | **NO**       |
| SMSG_AUCTION_OUTBID_NOTIFICATION               | 9970 (0x26F2)  | NO      | YES          |
| SMSG_AUCTION_OWNER_BID_NOTIFICATION            | 9972 (0x26F4)  | NO      | YES          |
| SMSG_AUCTION_OWNER_LIST_RESULT                 | 10364 (0x287C) | NO      | **NO**       |
| SMSG_AUCTION_WON_NOTIFICATION                  | 9969 (0x26F1)  | NO      | YES          |
| SMSG_AVAILABLE_HOTFIXES                        | 10511 (0x290F) | NO      | YES          |
| SMSG_BATTLEFIELD_STATUS_ACTIVE                 | 10531 (0x2923) | NO      | YES          |
| SMSG_BATTLEFIELD_STATUS_FAILED                 | 10534 (0x2926) | NO      | YES          |
| SMSG_BATTLEFIELD_STATUS_NEED_CONFIRMATION      | 10530 (0x2922) | NO      | YES          |
| SMSG_BATTLEGROUND_INIT                         | 10575 (0x294F) | NO      | YES          |
| SMSG_BATTLEGROUND_PLAYER_POSITIONS             | 10536 (0x2928) | NO      | YES          |
| SMSG_BATTLENET_NOTIFICATION                    | 10248 (0x2808) | NO      | YES          |
| SMSG_BATTLENET_RESPONSE                        | 10247 (0x2807) | NO      | YES          |
| SMSG_BATTLE_NET_CONNECTION_STATUS              | 10249 (0x2809) | NO      | YES          |
| SMSG_BATTLE_PET_JOURNAL_LOCK_ACQUIRED          | 9709 (0x25ED)  | NO      | YES          |
| SMSG_CAN_DUEL_RESULT                           | 10567 (0x2947) | NO      | YES          |
| SMSG_CHANGE_REALM_TICKET_RESPONSE              | 10250 (0x280A) | NO      | YES          |
| SMSG_CHANNEL_NOTIFY_JOINED                     | 11202 (0x2BC2) | NO      | YES          |
| SMSG_CHANNEL_NOTIFY_LEFT                       | 11203 (0x2BC3) | NO      | YES          |
| SMSG_CHAT_PLAYER_NOT_FOUND                     | 0 (0x0000)     | NO      | **NO**       |
| SMSG_COIN_REMOVED                              | 9751 (0x2617)  | NO      | YES          |
| SMSG_CONQUEST_FORMULA_CONSTANTS                | 10121 (0x2789) | NO      | YES          |
| SMSG_CORPSE_LOCATION                           | 9807 (0x264F)  | NO      | YES          |
| SMSG_DB_REPLY                                  | 10510 (0x290E) | NO      | YES          |
| SMSG_DISPLAY_TOAST                             | 9764 (0x2624)  | NO      | YES          |
| SMSG_ENTER_ENCRYPTED_MODE                      | 12361 (0x3049) | NO      | YES          |
| SMSG_ENTITY_LOOK_ROTATION                      | 0 (0x0000)     | NO      | **NO**       |
| SMSG_FEATURE_SYSTEM_STATUS_GLUE_SCREEN         | 9664 (0x25C0)  | NO      | YES          |
| SMSG_GAMESPEED_SET                             | 0 (0x0000)     | NO      | **NO**       |
| SMSG_GENERATE_RANDOM_CHARACTER_NAME_RESULT     | 9605 (0x2585)  | NO      | YES          |
| SMSG_GET_ACCOUNT_CHARACTER_LIST_RESULT         | 10085 (0x2765) | NO      | YES          |
| SMSG_GUILD_BANK_LOG_QUERY_RESULTS              | 10720 (0x29E0) | NO      | YES          |
| SMSG_GUILD_BANK_REMAINING_WITHDRAW_MONEY       | 10721 (0x29E1) | NO      | YES          |
| SMSG_GUILD_BANK_TEXT_QUERY_RESULT              | 10724 (0x29E4) | NO      | YES          |
| SMSG_GUILD_EVENT_BANK_MONEY_CHANGED            | 10743 (0x29F7) | NO      | YES          |
| SMSG_GUILD_EVENT_DISBANDED                     | 10734 (0x29EE) | NO      | YES          |
| SMSG_GUILD_EVENT_LOG_QUERY_RESULTS             | 10723 (0x29E3) | NO      | YES          |
| SMSG_GUILD_EVENT_MOTD                          | 10735 (0x29EF) | NO      | YES          |
| SMSG_GUILD_EVENT_NEW_LEADER                    | 10733 (0x29ED) | NO      | YES          |
| SMSG_GUILD_EVENT_PLAYER_JOINED                 | 10731 (0x29EB) | NO      | YES          |
| SMSG_GUILD_EVENT_PLAYER_LEFT                   | 10732 (0x29EC) | NO      | YES          |
| SMSG_GUILD_EVENT_PRESENCE_CHANGE               | 10736 (0x29F0) | NO      | YES          |
| SMSG_GUILD_EVENT_RANKS_UPDATED                 | 10737 (0x29F1) | NO      | YES          |
| SMSG_GUILD_EVENT_TAB_ADDED                     | 10739 (0x29F3) | NO      | YES          |
| SMSG_GUILD_EVENT_TAB_MODIFIED                  | 10741 (0x29F5) | NO      | YES          |
| SMSG_GUILD_EVENT_TAB_TEXT_CHANGED              | 10742 (0x29F6) | NO      | YES          |
| SMSG_GUILD_RANKS                               | 10697 (0x29C9) | NO      | YES          |
| SMSG_GUILD_SEND_RANK_CHANGE                    | 10681 (0x29B9) | NO      | YES          |
| SMSG_HOTFIX_CONNECT                            | 10513 (0x2911) | NO      | YES          |
| SMSG_HOTFIX_MESSAGE                            | 10512 (0x2910) | NO      | YES          |
| SMSG_INITIAL_SETUP                             | 9600 (0x2580)  | NO      | YES          |
| SMSG_INSPECT_HONOR_STATS                       | 10547 (0x2933) | NO      | YES          |
| SMSG_INSPECT_PVP                               | 10018 (0x2722) | NO      | YES          |
| SMSG_LEARNED_SPELLS                            | 11338 (0x2C4A) | NO      | YES          |
| SMSG_LFG_LIST_UPDATE_BLACKLIST                 | 10794 (0x2A2A) | NO      | YES          |
| SMSG_LFG_TELEPORT                              | 0 (0x0000)     | NO      | **NO**       |
| SMSG_LFG_UPDATE_STATUS                         | 10788 (0x2A24) | NO      | YES          |
| SMSG_LOAD_CUF_PROFILES                         | 9660 (0x25BC)  | NO      | YES          |
| SMSG_LOOT_CONTENTS                             | 0 (0x0000)     | NO      | YES          |
| SMSG_LOOT_ROLLS_COMPLETE                       | 9760 (0x2620)  | NO      | YES          |
| SMSG_MAIL_QUERY_NEXT_TIME_RESULT               | 10071 (0x2757) | NO      | YES          |
| SMSG_MINIMAP_PING                              | 9934 (0x26CE)  | NO      | YES          |
| SMSG_MONEY_BALANCE                             | 0 (0x0000)     | NO      | **NO**       |
| SMSG_MOVE_SET_COLLISION_HEIGHT                 | 11795 (0x2E13) | NO      | YES          |
| SMSG_MOVE_SET_FLIGHT_BACK_SPEED                | 11765 (0x2DF5) | NO      | YES          |
| SMSG_MOVE_SET_FLIGHT_SPEED                     | 11764 (0x2DF4) | NO      | YES          |
| SMSG_MOVE_SET_PITCH_RATE                       | 11768 (0x2DF8) | NO      | YES          |
| SMSG_MOVE_SET_RUN_BACK_SPEED                   | 11761 (0x2DF1) | NO      | YES          |
| SMSG_MOVE_SET_RUN_SPEED                        | 11760 (0x2DF0) | NO      | YES          |
| SMSG_MOVE_SET_SWIM_BACK_SPEED                  | 11763 (0x2DF3) | NO      | YES          |
| SMSG_MOVE_SET_SWIM_SPEED                       | 11762 (0x2DF2) | NO      | YES          |
| SMSG_MOVE_SET_TURN_RATE                        | 11767 (0x2DF7) | NO      | YES          |
| SMSG_MOVE_SET_WALK_SPEED                       | 11766 (0x2DF6) | NO      | YES          |
| SMSG_MOVE_SPLINE_SET_WALK_SPEED                | 11757 (0x2DED) | YES     | YES          |
| SMSG_MOVE_TELEPORT                             | 11780 (0x2E04) | NO      | YES          |
| SMSG_MOVE_UPDATE                               | 11744 (0x2DE0) | NO      | YES          |
| SMSG_MOVE_UPDATE_KNOCK_BACK                    | 11746 (0x2DE2) | NO      | YES          |
| SMSG_PARTY_MEMBER_STATS_FULL                   | 0 (0x0000)     | NO      | **NO**       |
| SMSG_PARTY_UPDATE                              | 9716 (0x25F4)  | NO      | YES          |
| SMSG_PETITION_RENAME_GUILD_RESPONSE            | 10746 (0x29FA) | NO      | YES          |
| SMSG_PET_CLEAR_SPELLS                          | 11297 (0x2C21) | NO      | YES          |
| SMSG_PET_LEARNED_SPELL                         | 0 (0x0000)     | NO      | **NO**       |
| SMSG_PET_REMOVED_SPELL                         | 0 (0x0000)     | NO      | **NO**       |
| SMSG_PLAYER_SAVE_GUILD_EMBLEM                  | 10745 (0x29F9) | NO      | YES          |
| SMSG_PLAY_SPELL_VISUAL_KIT                     | 11334 (0x2C46) | NO      | YES          |
| SMSG_PRE_RESURRECT                             | 0 (0x0000)     | NO      | **NO**       |
| SMSG_PROCRESIST                                | 0 (0x0000)     | NO      | **NO**       |
| SMSG_PROPOSED_INVITE                           | 0 (0x0000)     | NO      | **NO**       |
| SMSG_QUERY_ARENA_TEAM_RESPONSE                 | 10528 (0x2920) | NO      | YES          |
| SMSG_QUERY_PLAYER_NAMES_RESPONSE               | 12315 (0x301B) | NO      | YES          |
| SMSG_QUESTLOG_FULL                             | 0 (0x0000)     | NO      | **NO**       |
| SMSG_QUEST_FORCE_REMOVE                        | 0 (0x0000)     | NO      | **NO**       |
| SMSG_QUEST_PUSH_RESULT                         | 10896 (0x2A90) | NO      | YES          |
| SMSG_QUEST_UPDATE_ADD_CREDIT                   | 10892 (0x2A8C) | NO      | YES          |
| SMSG_QUEST_UPDATE_ADD_CREDIT_SIMPLE            | 10893 (0x2A8D) | NO      | YES          |
| SMSG_RAID_READY_CHECK                          | 0 (0x0000)     | NO      | **NO**       |
| SMSG_RAID_READY_CHECK_CONFIRM                  | 0 (0x0000)     | NO      | **NO**       |
| SMSG_RAID_READY_CHECK_ERROR                    | 0 (0x0000)     | NO      | **NO**       |
| SMSG_RAID_READY_CHECK_RESULT                   | 0 (0x0000)     | NO      | **NO**       |
| SMSG_RAID_ROLL_VOTE                            | 0 (0x0000)     | NO      | **NO**       |
| SMSG_RANDOM_ROLL                               | 9776 (0x2630)  | NO      | YES          |
| SMSG_READY_CHECK_COMPLETED                     | 9720 (0x25F8)  | NO      | YES          |
| SMSG_READY_CHECK_RESPONSE                      | 9719 (0x25F7)  | NO      | YES          |
| SMSG_READY_CHECK_STARTED                       | 9718 (0x25F6)  | NO      | YES          |
| SMSG_RESPEC_WIPE_CONFIRM                       | 9746 (0x2612)  | NO      | YES          |
| SMSG_RESUME_TOKEN                              | 9641 (0x25A9)  | NO      | YES          |
| SMSG_SEASON_INFO                               | 9665 (0x25C1)  | NO      | YES          |
| SMSG_SEND_SPELL_CHARGES                        | 11306 (0x2C2A) | NO      | YES          |
| SMSG_SEND_SPELL_HISTORY                        | 11304 (0x2C28) | NO      | YES          |
| SMSG_SERVER_MESSAGE                            | 0 (0x0000)     | NO      | **NO**       |
| SMSG_SERVER_PERF                               | 0 (0x0000)     | NO      | YES          |
| SMSG_SERVER_TIME_OFFSET                        | 10004 (0x2714) | NO      | YES          |
| SMSG_SETUP_CURRENCY                            | 9587 (0x2573)  | NO      | YES          |
| SMSG_SET_DUNGEON_DIFFICULTY                    | 9892 (0x26A4)  | NO      | YES          |
| SMSG_SET_TIME_ZONE_INFORMATION                 | 9847 (0x2677)  | NO      | YES          |
| SMSG_SHOW_RATINGS                              | 0 (0x0000)     | NO      | YES          |
| SMSG_SOCIAL_CONTRACT_REQUEST_RESPONSE          | 10386 (0x2892) | NO      | YES          |
| SMSG_SOCKET_GEMS_SUCCESS                       | 10023 (0x2727) | NO      | YES          |
| SMSG_SPELLLOGEXECUTE                           | 0 (0x0000)     | NO      | **NO**       |
| SMSG_SPELL_CHANNEL_START                       | 11313 (0x2C31) | NO      | YES          |
| SMSG_SPELL_CHANNEL_UPDATE                      | 11314 (0x2C32) | NO      | YES          |
| SMSG_SPELL_EXTRA_ATTACKS                       | 0 (0x0000)     | NO      | **NO**       |
| SMSG_SPELL_PREPARE                             | 11317 (0x2C35) | NO      | YES          |
| SMSG_START_LIGHTNING_STORM                     | 9895 (0x26A7)  | NO      | YES          |
| SMSG_SUMMON_RAID_MEMBER_VALIDATE_FAILED        | 9613 (0x258D)  | NO      | YES          |
| SMSG_SUSPEND_TOKEN                             | 9640 (0x25A8)  | NO      | YES          |
| SMSG_TAXI_PATH_ACTIVATED                       | 0 (0x0000)     | NO      | **NO**       |
| SMSG_TRADE_UPDATED                             | 9601 (0x2581)  | NO      | YES          |
| SMSG_UPDATE_AURA_DURATION                      | 0 (0x0000)     | YES     | YES          |
| SMSG_VEHICLE_RIDE_ALLOWED_QUERY_RESPONSE       | 0 (0x0000)     | NO      | **NO**       |
| SMSG_WAIT_QUEUE_FINISH                         | 9583 (0x256F)  | NO      | YES          |
| SMSG_WAIT_QUEUE_UPDATE                         | 9582 (0x256E)  | NO      | YES          |
| SMSG_WHOIS                                     | 0 (0x0000)     | NO      | **NO**       |
| SMSG_WIPE_ALL_CRITERIA_FROM_CLIENT             | 0 (0x0000)     | NO      | **NO**       |
| SMSG_WORLD_SERVER_INFO                         | 9645 (0x25AD)  | NO      | YES          |
| SMSG_WORLD_STATE_UI_TIMER_UPDATE               | 0 (0x0000)     | NO      | **NO**       |
| SMSG_XP_GAIN_ABORTED                           | 9673 (0x25C9)  | NO      | YES          |

## 4. PRIORITY: Unhandled Opcodes Present in Both Versions

Count: 84

These opcodes exist in both legacy and modern but have NO handler.
They are the most likely candidates for missing functionality.

| Opcode                                    | Legacy Value  | Modern Value   |
|-------------------------------------------|---------------|----------------|
| CMSG_ALTER_APPEARANCE                     | 1062 (0x0426) | 13557 (0x34F5) |
| CMSG_ARENA_TEAM_INVITE                    | 847 (0x034F)  | 0 (0x0000)     |
| CMSG_AUTH_CONTINUED_SESSION               | 1298 (0x0512) | 14182 (0x3766) |
| CMSG_AUTH_SESSION                         | 493 (0x01ED)  | 14181 (0x3765) |
| CMSG_AUTO_STORE_BAG_ITEM                  | 267 (0x010B)  | 14745 (0x3999) |
| CMSG_BUG                                  | 458 (0x01CA)  | 13959 (0x3687) |
| CMSG_CANCEL_GROWTH_AURA                   | 667 (0x029B)  | 12911 (0x326F) |
| CMSG_CHAT_CHANNEL_BAN                     | 165 (0x00A5)  | 14305 (0x37E1) |
| CMSG_CHAT_CHANNEL_INVITE                  | 163 (0x00A3)  | 14303 (0x37DF) |
| CMSG_CHAT_CHANNEL_KICK                    | 164 (0x00A4)  | 14304 (0x37E0) |
| CMSG_CHAT_CHANNEL_MODERATOR               | 159 (0x009F)  | 14299 (0x37DB) |
| CMSG_CHAT_CHANNEL_PASSWORD                | 156 (0x009C)  | 14295 (0x37D7) |
| CMSG_CHAT_CHANNEL_SET_OWNER               | 157 (0x009D)  | 14296 (0x37D8) |
| CMSG_CHAT_CHANNEL_SILENCE_ALL             | 973 (0x03CD)  | 14308 (0x37E4) |
| CMSG_CHAT_CHANNEL_UNBAN                   | 166 (0x00A6)  | 14306 (0x37E2) |
| CMSG_CHAT_CHANNEL_UNMODERATOR             | 160 (0x00A0)  | 14300 (0x37DC) |
| CMSG_CHAT_CHANNEL_UNSILENCE_ALL           | 975 (0x03CF)  | 14309 (0x37E5) |
| CMSG_GM_TICKET_CREATE                     | 517 (0x0205)  | 0 (0x0000)     |
| CMSG_GM_TICKET_DELETE_TICKET              | 535 (0x0217)  | 0 (0x0000)     |
| CMSG_GM_TICKET_GET_TICKET                 | 529 (0x0211)  | 0 (0x0000)     |
| CMSG_GM_TICKET_UPDATE_TEXT                | 519 (0x0207)  | 0 (0x0000)     |
| CMSG_GROUP_DECLINE                        | 115 (0x0073)  | 0 (0x0000)     |
| CMSG_GUILD_BANK_SWAP_ITEMS                | 1001 (0x03E9) | 0 (0x0000)     |
| CMSG_HEARTH_AND_RESURRECT                 | 1180 (0x049C) | 13574 (0x3506) |
| CMSG_INSTANCE_LOCK_RESPONSE               | 319 (0x013F)  | 13579 (0x350B) |
| CMSG_ITEM_QUERY_SINGLE                    | 86 (0x0056)   | 0 (0x0000)     |
| CMSG_ITEM_TEXT_QUERY                      | 579 (0x0243)  | 12997 (0x32C5) |
| CMSG_KEEP_ALIVE                           | 1031 (0x0407) | 13953 (0x3681) |
| CMSG_MESSAGECHAT                          | 149 (0x0095)  | 0 (0x0000)     |
| CMSG_PING                                 | 476 (0x01DC)  | 14184 (0x3768) |
| CMSG_PLAYER_AI_CHEAT                      | 620 (0x026C)  | 0 (0x0000)     |
| CMSG_QUERY_QUESTS_COMPLETED               | 1280 (0x0500) | 0 (0x0000)     |
| CMSG_QUEST_POI_QUERY                      | 483 (0x01E3)  | 14002 (0x36B2) |
| CMSG_REMOVE_GLYPH                         | 1162 (0x048A) | 13056 (0x3300) |
| CMSG_REQUEST_VEHICLE_EXIT                 | 1142 (0x0476) | 12855 (0x3237) |
| CMSG_REQUEST_VEHICLE_NEXT_SEAT            | 1144 (0x0478) | 12857 (0x3239) |
| CMSG_REQUEST_VEHICLE_PREV_SEAT            | 1143 (0x0477) | 12856 (0x3238) |
| CMSG_REQUEST_VEHICLE_SWITCH_SEAT          | 1145 (0x0479) | 12858 (0x323A) |
| CMSG_RESET_FACTION_CHEAT                  | 641 (0x0281)  | 0 (0x0000)     |
| CMSG_SPELL_CLICK                          | 1016 (0x03F8) | 13461 (0x3495) |
| CMSG_STABLE_REVIVE_PET                    | 628 (0x0274)  | 0 (0x0000)     |
| CMSG_UPDATE_MISSILE_TRAJECTORY            | 1122 (0x0462) | 14915 (0x3A43) |
| CMSG_WARDEN_DATA                          | 743 (0x02E7)  | 0 (0x0000)     |
| CMSG_ZONEUPDATE                           | 500 (0x01F4)  | 0 (0x0000)     |
| MSG_NULL_ACTION                           | 0 (0x0000)    | 0 (0x0000)     |
| SMSG_ACHIEVEMENT_DELETED                  | 1183 (0x049F) | 9960 (0x26E8)  |
| SMSG_ADDON_INFO                           | 751 (0x02EF)  | 0 (0x0000)     |
| SMSG_AUCTION_REMOVED_NOTIFICATION         | 653 (0x028D)  | 9973 (0x26F5)  |
| SMSG_AUTH_CHALLENGE                       | 492 (0x01EC)  | 12360 (0x3048) |
| SMSG_AUTH_RESPONSE                        | 494 (0x01EE)  | 9581 (0x256D)  |
| SMSG_BATTLEFIELD_STATUS_QUEUED            | 744 (0x02E8)  | 10532 (0x2924) |
| SMSG_CHANNEL_MEMBER_COUNT                 | 981 (0x03D5)  | 0 (0x0000)     |
| SMSG_CONNECT_TO                           | 1293 (0x050D) | 12365 (0x304D) |
| SMSG_CRITERIA_DELETED                     | 1182 (0x049E) | 9959 (0x26E7)  |
| SMSG_EXPECTED_SPAM_RECORDS                | 818 (0x0332)  | 11185 (0x2BB1) |
| SMSG_GM_TICKET_STATUS_UPDATE              | 808 (0x0328)  | 0 (0x0000)     |
| SMSG_GROUP_DESTROYED                      | 124 (0x007C)  | 10132 (0x2794) |
| SMSG_HEALTH_UPDATE                        | 1151 (0x047F) | 9937 (0x26D1)  |
| SMSG_INSTANCE_LOCK_WARNING_QUERY          | 327 (0x0147)  | 0 (0x0000)     |
| SMSG_LFG_DISABLED                         | 920 (0x0398)  | 10803 (0x2A33) |
| SMSG_LFG_OFFER_CONTINUE                   | 659 (0x0293)  | 10804 (0x2A34) |
| SMSG_LFG_PARTY_INFO                       | 882 (0x0372)  | 10806 (0x2A36) |
| SMSG_LFG_PLAYER_REWARD                    | 511 (0x01FF)  | 10808 (0x2A38) |
| SMSG_LFG_ROLE_CHECK_UPDATE                | 867 (0x0363)  | 10785 (0x2A21) |
| SMSG_LFG_UPDATE_SEARCH                    | 873 (0x0369)  | 0 (0x0000)     |
| SMSG_MOUNT_RESULT                         | 366 (0x016E)  | 9595 (0x257B)  |
| SMSG_ON_CANCEL_EXPECTED_RIDE_VEHICLE_AURA | 1181 (0x049D) | 9958 (0x26E6)  |
| SMSG_OVERRIDE_LIGHT                       | 1042 (0x0412) | 9915 (0x26BB)  |
| SMSG_PET_ACTION_FEEDBACK                  | 710 (0x02C6)  | 10057 (0x2749) |
| SMSG_PET_GUIDS                            | 1194 (0x04AA) | 9988 (0x2704)  |
| SMSG_PET_MODE                             | 378 (0x017A)  | 9608 (0x2588)  |
| SMSG_PET_TAME_FAILURE                     | 371 (0x0173)  | 9907 (0x26B3)  |
| SMSG_PHASE_SHIFT_CHANGE                   | 1148 (0x047C) | 9592 (0x2578)  |
| SMSG_PLAY_TIME_WARNING                    | 757 (0x02F5)  | 0 (0x0000)     |
| SMSG_QUEST_POI_QUERY_RESPONSE             | 484 (0x01E4)  | 10909 (0x2A9D) |
| SMSG_REAL_GROUP_UPDATE                    | 919 (0x0397)  | 0 (0x0000)     |
| SMSG_RESUME_COMMS                         | 1297 (0x0511) | 12363 (0x304B) |
| SMSG_SET_PLAYER_DECLINED_NAMES_RESULT     | 1050 (0x041A) | 12291 (0x3003) |
| SMSG_SPELL_UPDATE_CHAIN_TARGETS           | 816 (0x0330)  | 0 (0x0000)     |
| SMSG_SUSPEND_COMMS                        | 1295 (0x050F) | 12362 (0x304A) |
| SMSG_THREAT_REMOVE                        | 1156 (0x0484) | 9947 (0x26DB)  |
| SMSG_TITLE_EARNED                         | 883 (0x0373)  | 9943 (0x26D7)  |
| SMSG_UPDATE_ACCOUNT_DATA                  | 524 (0x020C)  | 9993 (0x2709)  |
| SMSG_UPDATE_LAST_INSTANCE                 | 800 (0x0320)  | 9864 (0x2688)  |
