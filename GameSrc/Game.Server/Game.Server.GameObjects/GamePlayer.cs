using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Achievement;
using Game.Server.Buffer;
using Game.Server.Consortia;
using Game.Server.ConsortiaTask;
using Game.Server.Event;
using Game.Server.Farm;
using Game.Server.GameUtils;
using Game.Server.HotSpringRooms;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Pet;
using Game.Server.Quests;
using Game.Server.SceneMarryRooms;
using Game.Server.Statics;
using log4net;
using SqlDataProvider.Data;
using Game.Server.LittleGame;
using Game.Server.LittleGame.Data;
using Game.Server.GameRoom;
using Game.Server.Rooms;
using Game.Server.Games;
using Game.Server.DDTQiYuan;
using Game.Server.GodCardRaise;
using Game.Server.SanXiao;
using Game.Server.ActiveSystem;
using Game.Server.Managers.EliteGame;
using System.Linq;
using Game.Server.GMActives;
using Newtonsoft.Json;
using Game.Server.AvatarCollection;
using Game.Server.ExplorerManual;
using System.Configuration;
using Game.Server.GuildBattle;

namespace Game.Server.GameObjects
{
	public class GamePlayer : IGamePlayer
	{
		public delegate void PlayerOwnSpaEventHandle(int onlineTimeSpa);

		public delegate void PlayerAddItemEventHandel(string type, int value);

		public delegate void GameKillDropEventHandel(AbstractGame game, int type, int npcId, bool playResult);

		public delegate void PlayerAchievementFinish(AchievementData info);

		public delegate void PlayerAdoptPetEventHandle();

		public delegate void PlayerCropPrimaryEventHandle();

		public delegate void PlayerEnterHotSpring(GamePlayer player);

		public delegate void PlayerEventHandle(GamePlayer player);

		public delegate void PlayerFightAddOffer(int offer);

		public delegate void PlayerFightOneBloodIsWin(eRoomType roomType, bool isWin);

		public delegate void PlayerGameKillBossEventHandel(AbstractGame game, NpcInfo npc, int damage);

		public delegate void PlayerGameDamageBossEventHandel(AbstractGame game, NpcInfo npc, int damage);

		public delegate void PlayerGameKillEventHandel(AbstractGame game, int type, int id, bool isLiving, int demage, bool isSpanArea);

		public delegate void PlayerGoldCollection(int value);

		public delegate void PlayerGiftTokenCollection(int value);

		public delegate void PlayerHotSpingExpAdd(int minutes, int exp);

		public delegate void PlayerOnlineAdd();

		public delegate void PlayerLoginDeviceAdd();

		public delegate void PlayerLoginEventHandle();

		public delegate void PlayerItemComposeEventHandle(int composeType);

		public delegate void PlayerMoneyChargeHandle(int money);

		public delegate void PlayerUseMoneyHandle(string Type, int Value);

		public delegate void PlayerItemFusionEventHandle(int fusionType);

		public delegate void PlayerItemInsertEventHandle();

		public delegate void PlayerItemMeltEventHandle(int categoryID);

		public delegate void PlayerItemPropertyEventHandle(int templateID, int count);

		public delegate void PlayerItemStrengthenEventHandle(int categoryID, int level);

		public delegate void PlayerMissionFullOverEventHandle(AbstractGame game, int missionId, bool isWin, int turnNum);

		public delegate void PlayerMissionOverEventHandle(AbstractGame game, int missionId, bool isWin);

		public delegate void PlayerMissionTurnOverEventHandle(AbstractGame game, int missionId, int turnNum);

		public delegate void PlayerNewGearEventHandle(ItemInfo item);

		public delegate void PlayerNewGearEventHandle2(ItemInfo item);

		public delegate void PlayerOwnConsortiaEventHandle();

		public delegate void PlayerAchievementQuestHandle();

		public delegate void PlayerPropertisChange(PlayerInfo player);

		public delegate void PlayerSeedFoodPetEventHandle();

		public delegate void PlayerShopEventHandle(int money, int gold, int offer, int gifttoken, int petScore, int medal, string payGoods);

		public delegate void PlayerUnknowQuestConditionEventHandle();

		public delegate void PlayerUpLevelPetEventHandle();

		public delegate void PlayerUseBugle(int value);

		public delegate void PlayerUserToemGemstoneEventHandle();

		public delegate void PlayerVIPUpgrade(int level, int exp);

		public delegate void PlayerQuestFinishEventHandel(BaseQuest baseQuest);

		public delegate void PlayerPropertyChangedEventHandel(PlayerInfo character);

		public delegate void PlayerMarryTeamEventHandle(AbstractGame game, bool isWin, int gainXp, int countPlayersTeam);

		public delegate void PlayerGameOverCountTeamEventHandle(AbstractGame game, bool isWin, int gainXp, int countPlayersTeam);

		public delegate void PlayerMarryEventHandel();

		public delegate void PlayerDispatchesEventHandel();

		public delegate void PlayerGameOverEventHandle(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple);

		public delegate void PlayerGameOverEvent2v2Handle(bool isWin);

		public delegate void PlayerGameOverEvent3v3Handle(bool isWin);

		public delegate void PlayerGameOverEvent4v4Handle(bool isWin);

		public delegate void PlayerAcademyEventHandle(GamePlayer friendly, int type);

		public delegate void PlayerUpLevelVipEventHandle(GamePlayer player);

		public delegate void WorldBossDamageEventHandle(GamePlayer player);

		public ItemInfo LastTakeCardItem;

		private Dictionary<int, int> _friends;

		public DateTime BossBoxStartTime;

		public bool BlockReceiveMoney;

		public bool FirstLoginCheckCode;

		public DateTime LastOpenHole;

		public DateTime WaitingProcessor;

		public int canTakeOut;

		public Dictionary<int, CardInfoOld> Card = new Dictionary<int, CardInfoOld>();

		public CardInfoOld[] CardsTakeOut = new CardInfoOld[9];

		public int CurrentRoomIndex;

		public int CurrentRoomTeam;

		public int FightPower;

		public double GuildRichAddPlus = 1.0;

		public int Hot_Direction;

		public int Hot_X;

		public int Hot_Y;

		public int HotMap;

		private HotSpringRoom hotSpringRoom_0;

		public bool IsInChristmasRoom;

		public int GuildBattleEnemyId { get; set; }

		public List<int> CardBuff { get; set; }

		public bool isPowerFullUsed;

		public bool KickProtect;

		public bool RunningGold;

		public readonly string[] labyrinthGolds = new string[40]
		{
		"0|0",
		"2|2",
		"0|0",
		"2|2",
		"0|0",
		"2|3",
		"0|0",
		"3|3",
		"0|0",
		"3|4",
		"0|0",
		"3|4",
		"0|0",
		"4|5",
		"0|0",
		"4|5",
		"0|0",
		"4|6",
		"0|0",
		"5|6",
		"0|0",
		"5|7",
		"0|0",
		"5|7",
		"0|0",
		"6|8",
		"0|0",
		"6|8",
		"0|0",
		"6|10",
		"0|0",
		"8|10",
		"0|0",
		"8|11",
		"0|0",
		"8|11",
		"0|0",
		"10|12",
		"0|0",
		"10|12"
		};

		public DateTime LastAttachMail;

		public DateTime LastChatTime;

		public DateTime LastDrillUpTime;

		public DateTime LastFigUpTime;

		public DateTime LastOpenCard;

		public DateTime LastOpenChristmasPackage;

		public DateTime LastOpenGrowthPackage;

		public DateTime LastOpenPack;

		public DateTime LastOpenYearMonterPackage;

		public DateTime LastEnterWorldBoss;

		public List<ItemInfo> LotteryAwardList;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string m_account;

		private AchievementInventory m_achievementInventory;

		private EventInventory m_eventLiveInventory;

		private PlayerBattle m_battle;

		private BufferList m_bufferList;

		private PlayerInventory m_caddyBag;

		public PlayerInventory FarmBag { get; }

		public PlayerInventory Vegetable { get; }

		private CardInventory m_cardBag;

		protected GameClient m_client;

		private PlayerInventory m_ConsortiaBag;

		private Dictionary<string, UserEquipGhostInfo> m_equipGhostList;

		private PlayerInventory m_BankBag;

		private UTF8Encoding m_converter;

		private MarryRoom m_currentMarryRoom;

		private BaseRoom m_currentRoom;

		private ItemInfo m_currentSecondWeapon;

		private int m_changed;

		private PlayerInfo m_character;

		private PlayerEquipInventory m_equipBag;

		private List<int> m_equipEffect;

		public List<UserGemStone> GemStone { get; set; }

		private PlayerExtra m_extra;

		private PlayerInventory m_fightBag;

		private List<BufferInfo> m_fightBuffInfo;

		private PlayerInventory m_food;

		protected BaseGame m_game;

		private ItemInfo m_healstone;

		private int m_immunity = 255;

		private bool m_isAASInfo;

		private bool m_isMinor;

		private ItemInfo m_MainWeapon;

		private UsersPetInfo m_pet;

		private PlayerInventory m_petEgg;

		private long m_pingTime;

		private int m_playerId;

		private PlayerProperty m_playerProp;

		protected Player m_players;

		private ePlayerState m_playerState;

		private PlayerInventory m_propBag;

		private char[] m_pvepermissions;

		private QuestInventory m_questInventory;

		private PlayerRank m_rank;

		private bool m_showPP;

		private PlayerInventory m_storeBag;

		private PlayerInventory m_tempBag;

		private Dictionary<string, object> m_tempProperties = new Dictionary<string, object>();

		public bool m_toemview;

		public DateTime BoxBeginTime;

		private Dictionary<int, UserDrillInfo> m_userDrills;

		public int MarryMap;

		public int HoGiap;

		private static char[] permissionChars = new char[4]
		{
		'1',
		'3',
		'7',
		'F'
		};

		public long PingStart;

		public byte States;

		private static readonly int[] StyleIndex = new int[15]
		{
		1,
		2,
		3,
		4,
		5,
		6,
		11,
		13,
		14,
		15,
		16,
		17,
		18,
		19,
		20
		};

		public int takeoutCount;

		public int winningStreak;

		public int X;

		public int Y;

		private char[] m_fightlabpermissions;

		private static char[] fightlabpermissionChars = new char[4]
		{
		'0',
		'1',
		'2',
		'3'
		};

		public int missionPlayed;

		public int playersKilled;

		protected ConsortiaLogicProcessor m_consortiaProcessor;

		protected GameRoomLogicProcessor m_gameroomProcessor = new GameRoomLogicProcessor();

		protected GameRoomProcessor m_gameRoom;

		protected AvatarCollectionLogicProcessor m_avatarCollectionProcessor = new AvatarCollectionLogicProcessor();

        protected ExplorerManualLogicProcessor m_explorerManualProcessor = new ExplorerManualLogicProcessor();

        public ExplorerManualProcessor ExplorerManual { get; private set; }

        public AvatarCollectionProcessor AvatarCollection { get; private set; }

		private ConsortiaProcessor consortiaProcessor_0;

		protected PetLogicProcessor m_petProcessor;

		private PetProcessor petProcessor_0;

		private PetInventory m_petBag;

		public DateTime LastMovePlaceItem;

		public DateTime LastBuffPropItem;

		public Dictionary<int, int[]> CardResetTempProp;

		private UserLabyrinthInfo userLabyrinthInfo;

		public int TakeCardPlace;

		public int TakeCardTemplateID;

		public int TakeCardCount;

		private PlayerActives m_playerActive;

		private int int_6;

		public static List<Suit_TemplateInfo> DS_Template_Suit_info = Load_Template_Suit_info();

		public FarmProcessor FarmHandler { get; private set; }

		public PlayerLittleGameInfo LittleGameInfo
		{
			get;
		}

		protected FarmLogicProcessor m_farmProcessor = new FarmLogicProcessor();

		protected LittleGameLogicProcessor m_LittleGameProcessor = new LittleGameLogicProcessor();

		protected LittleGameProcessor m_LittleGame;

		public LittleGameProcessor LittleGame
		{
			get { return m_LittleGame; }
		}

		public GameRoomProcessor GameRoom
		{
			get { return m_gameRoom; }
		}

		protected QiYuanLogicProcessor m_QiYuanProcessor = new QiYuanLogicProcessor();
		protected GodCardRaiseLogicProcessor m_GodCardRaiseProcessor = new GodCardRaiseLogicProcessor();
		protected SanXiaoLogicProcessor m_SanXiaoProcessor = new SanXiaoLogicProcessor();
		protected ActiveSystemLogicProcessor m_activeSystemProcessor = new ActiveSystemLogicProcessor();
		public QiYuanProcessor DDTQiYuan { get; private set; }
		public GodCardRaiseProcessor GodCardRaise { get; private set; }
		public SanXiaoProcessor SanXiao { get; private set; }
		public ActiveSystemProcessor ActiveSystemHandler { get; private set; }

		private void SetupProcessor()
		{
			FarmHandler = new FarmProcessor(m_farmProcessor);
			m_LittleGame = new LittleGameProcessor(m_LittleGameProcessor);
			m_gameRoom = new GameRoomProcessor(m_gameroomProcessor);
			DDTQiYuan = new QiYuanProcessor(m_QiYuanProcessor);
			GodCardRaise = new GodCardRaiseProcessor(m_GodCardRaiseProcessor);
			SanXiao = new SanXiaoProcessor(m_SanXiaoProcessor);
			ActiveSystemHandler = new ActiveSystemProcessor(m_activeSystemProcessor);
			AvatarCollection = new AvatarCollectionProcessor(m_avatarCollectionProcessor);
            ExplorerManual = new ExplorerManualProcessor(m_explorerManualProcessor);
        }

		public double GPApprenticeOnline
		{
			get
			{
				if (m_character.MasterOrApprenticesArr.Count > 0)
				{
					foreach (KeyValuePair<int, string> item in m_character.MasterOrApprenticesArr)
					{
						if (WorldMgr.GetPlayerById(item.Key) != null)
						{
							return 0.05;
						}
					}
				}
				return 0.0;
			}
			set
			{
			}
		}

		public double GPApprenticeTeam
		{
			get
			{
				if (CurrentRoom != null)
				{
					foreach (GamePlayer allPlayer in CurrentRoom.GetPlayers())
					{
						if (allPlayer != this && allPlayer.PlayerCharacter.MasterOrApprenticesArr.ContainsKey(PlayerId))
						{
							return 0.1;
						}
					}
				}
				return 0.0;
			}
			set
			{
			}
		}

		public double GPSpouseTeam
		{
			get
			{
				if (CurrentRoom != null)
				{
					foreach (GamePlayer allPlayer in CurrentRoom.GetPlayers())
					{
						if (allPlayer != this && allPlayer.PlayerCharacter.SpouseID == PlayerId)
						{
							return 0.05;
						}
					}
				}
				return 0.0;
			}
			set
			{
			}
		}

		public PetProcessor PetHandler => petProcessor_0;

		public PlayerActives Actives => m_playerActive;

		public ConsortiaProcessor Consortia => consortiaProcessor_0;

		public UserLabyrinthInfo Labyrinth
		{
			get
			{
				return userLabyrinthInfo;
			}
			set
			{
				userLabyrinthInfo = value;
			}
		}

		public string Account => m_account;

		public AchievementInventory AchievementInventory => m_achievementInventory;

		public EventInventory EventLiveInventory => m_eventLiveInventory;

		public PlayerBattle BattleData => m_battle;

		public bool bool_1
		{
			get;
			set;
		}

		public bool Boolean_0
		{
			get
			{
				return bool_1;
			}
			set
			{
				bool_1 = value;
			}
		}

		public BufferList BufferList => m_bufferList;

		public PlayerInventory CaddyBag => m_caddyBag;

		public bool CanUseProp
		{
			get;
			set;
		}

		public double GPAddPlus
		{
			get;
			set;
		}

		public double OfferAddPlus
		{
			get;
			set;
		}

		public bool CanX2Exp
		{
			get;
			set;
		}

		public bool CanX3Exp
		{
			get;
			set;
		}

		public CardInventory CardBag => m_cardBag;

		public GameClient Client => m_client;

		public PlayerInventory ConsortiaBag => m_ConsortiaBag;

		public PlayerInventory BankBag => m_BankBag;

		public HotSpringRoom CurrentHotSpringRoom
		{
			get
			{
				return hotSpringRoom_0;
			}
			set
			{
				hotSpringRoom_0 = value;
			}
		}

		public MarryRoom CurrentMarryRoom
		{
			get
			{
				return m_currentMarryRoom;
			}
			set
			{
				m_currentMarryRoom = value;
			}
		}

		public BaseRoom CurrentRoom
		{
			get
			{
				return m_currentRoom;
			}
			set
			{
				BaseRoom room = Interlocked.Exchange(ref m_currentRoom, value);
				if (room != null)
				{
					RoomMgr.ExitRoom(room, this);
				}
			}
		}

		public PlayerEquipInventory EquipBag => m_equipBag;

		public PlayerAvataInventory AvatarBag { get; }

		public List<int> EquipEffect
		{
			get
			{
				return m_equipEffect;
			}
			set
			{
				m_equipEffect = value;
			}
		}

		public PlayerExtra Extra => m_extra;

		public PlayerInventory FightBag => m_fightBag;

		public List<BufferInfo> FightBuffs
		{
			get
			{
				return m_fightBuffInfo;
			}
			set
			{
				m_fightBuffInfo = value;
			}
		}

		public PlayerInventory Food => m_food;

		public Dictionary<int, int> Friends => _friends;

		public BaseGame game
		{
			get
			{
				return m_game;
			}
			set
			{
				m_game = value;
			}
		}

		public int GameId
		{
			get
			{
				return int_6;
			}
			set
			{
				int_6 = value;
			}
		}

		public int GamePlayerId { get; set; }

		public int TempGameId { get; set; }

		public ItemInfo Healstone
		{
			get
			{
				if (m_healstone == null)
				{
					return null;
				}
				return m_healstone;
			}
		}

		public int Immunity
		{
			get
			{
				return m_immunity;
			}
			set
			{
				m_immunity = value;
			}
		}

		public bool IsAASInfo
		{
			get
			{
				return m_isAASInfo;
			}
			set
			{
				m_isAASInfo = value;
			}
		}

		public virtual bool IsActive => m_client.IsConnected;

		public bool IsInMarryRoom => m_currentMarryRoom != null;

		public bool IsMinor
		{
			get
			{
				return m_isMinor;
			}
			set
			{
				m_isMinor = value;
			}
		}

		public int Level
		{
			get
			{
				return m_character.Grade;
			}
			set
			{
				if (value != m_character.Grade)
				{
					int grade = m_character.Grade;
					DailyRecordInfo record = new DailyRecordInfo
					{
						UserID = m_character.ID,
						Type = 2,
						Value = $"{m_character.Grade},{value}"
					};
					new PlayerBussiness().AddDailyRecord(record);
					//Extra.UpdateEventCondition(1, value);
					m_character.Grade = value;
					if (value == 6)
					{
						ItemInfo itemTemplate = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(112098), 1, 104);
						AddTemplate(itemTemplate);
					}
					if (value == 8)
					{
						PlayerCharacter.WeaklessGuildProgressStr = "////b7D/ht8WDQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
					}
					if (m_character.masterID != 0 && grade < m_character.Grade)
					{
						AcademyMgr.UpdateAwardApp(this, grade);
					}
					OnLevelUp(value);
					//GmActivityMgr.OnUpdateGrade(this, value);
					GmActivityMgr.OnLevel(this, value);
					OnPropertiesChanged();
				}
			}
		}

		public int LevelPlusBlood => LevelMgr.LevelPlusBlood(PlayerCharacter.Grade);

		//public int LevelPlusBlood => LevelMgr.FindLevel(m_character.Grade).Blood;

		public ItemInfo MainWeapon => m_MainWeapon;

		public UserMatchInfo MatchInfo => m_battle.MatchInfo;

		public virtual IPacketLib Out => m_client.Out;

		public UsersPetInfo Pet => m_pet;


		public long PingTime
		{
			get
			{
				return m_pingTime;
			}
			set
			{
				m_pingTime = value;
				GSPacketIn pkg = Out.SendNetWork(PlayerCharacter.ID, m_pingTime);
				if (m_currentRoom != null)
				{
					m_currentRoom.SendToAll(pkg, this);
				}
			}
		}

		public PlayerInfo PlayerCharacter => m_character;

		public PetInventory PetBag => m_petBag;

		public PlayerFarm Farm { get; }

		public int PlayerId => m_playerId;

		public PlayerProperty PlayerProp => m_playerProp;

		public Player Players => m_players;

		public ePlayerState PlayerState
		{
			get
			{
				return m_playerState;
			}
			set
			{
				m_playerState = value;
			}
		}

		public string ProcessLabyrinthAward
		{
			get;
			set;
		}

		public PlayerInventory PropBag => m_propBag;

		public QuestInventory QuestInventory => m_questInventory;

		public PlayerRank Rank => m_rank;

		public ItemInfo SecondWeapon
		{
			get
			{
				if (m_currentSecondWeapon == null)
				{
					return null;
				}
				return m_currentSecondWeapon;
			}
		}

		public int ServerID
		{
			get;
			set;
		}

		public bool IsAccountLimit
		{
			get;
			set;
		}

		public bool IsMoAiCuoi
		{
			get;
			set;
		}

		public bool ShowPP
		{
			get
			{
				return m_showPP;
			}
			set
			{
				m_showPP = value;
			}
		}

		public PlayerInventory StoreBag => m_storeBag;

		public PlayerInventory TempBag => m_tempBag;

		public Dictionary<string, object> TempProperties => m_tempProperties;

		public bool Toemview
		{
			get
			{
				return m_toemview;
			}
			set
			{
				m_toemview = value;
			}
		}

		public Dictionary<int, UserDrillInfo> UserDrills
		{
			get
			{
				return m_userDrills;
			}
			set
			{
				m_userDrills = value;
			}
		}

		public PlayerInfo UserVIPInfo => m_character;

		public int ZoneId => GameServer.Instance.Configuration.ServerID;

		public string ZoneName => GameServer.Instance.Configuration.ServerName;

		public int Lottery
		{
			get;
			internal set;
		}

		public List<ItemBoxInfo> LotteryItems
		{
			get;
			internal set;
		}

		public int LotteryID
		{
			get;
			internal set;
		}

		public DateTime LastRequestTime
		{
			get;
			internal set;
		}

		public int CurrentEnemyId
		{
			get;
			set;
		}

		public double BaseAgility
		{
			get;
			set;
		}

		public double BaseDamage
		{
			get;
			set;
		}

		public event PlayerAchievementFinish AchievementFinishEvent;

		public event PlayerAdoptPetEventHandle AdoptPetEvent;

		public event PlayerGameKillBossEventHandel AfterKillingBoss;

		public event PlayerGameDamageBossEventHandel AfterDamageBoss;

		public event PlayerGameKillEventHandel AfterKillingLiving;

		public event PlayerItemPropertyEventHandle AfterUsingItem;

		public event PlayerCropPrimaryEventHandle CropPrimaryEvent;

		public event PlayerEnterHotSpring EnterHotSpringEvent;

		public event PlayerVIPUpgrade Event_0;

		public event PlayerFightAddOffer FightAddOfferEvent;

		public event PlayerFightOneBloodIsWin FightOneBloodIsWin;

		public event GameKillDropEventHandel GameKillDrop;

		public event PlayerOwnConsortiaEventHandle GuildChanged;

		public event PlayerHotSpingExpAdd HotSpingExpAdd;

		public event PlayerOnlineAdd OnlineGameAdd;

		public event PlayerLoginDeviceAdd LoginDeviceAdd;

		public event PlayerLoginEventHandle PlayerLogin;

		public event PlayerItemComposeEventHandle ItemCompose;

		public event PlayerItemFusionEventHandle ItemFusion;

		public event PlayerItemInsertEventHandle ItemInsert;

		public event PlayerItemMeltEventHandle ItemMelt;

		public event PlayerItemStrengthenEventHandle ItemStrengthen;

		public event PlayerMoneyChargeHandle MoneyCharge;

		public event PlayerUseMoneyHandle UseMoney;

		public event PlayerAchievementQuestHandle AchievementQuest;

		public event PlayerEventHandle LevelUp;

		public event PlayerMissionOverEventHandle MissionOver;

		public event PlayerMissionTurnOverEventHandle MissionTurnOver;

		public event PlayerNewGearEventHandle NewGearEvent;

		public event PlayerSeedFoodPetEventHandle SeedFoodPetEvent;

		public event PlayerShopEventHandle Paid;

		public event PlayerUnknowQuestConditionEventHandle UnknowQuestConditionEvent;

		public event PlayerUpLevelPetEventHandle UpLevelPetEvent;

		public event PlayerEventHandle UseBuffer;

		public event PlayerUserToemGemstoneEventHandle UserToemGemstonetEvent;

		public event PlayerPropertyChangedEventHandel PlayerPropertyChanged;

		public event PlayerAddItemEventHandel PlayerAddItem;

		public event PlayerOwnSpaEventHandle PlayerSpa;

		public event PlayerPropertisChange PropertiesChange;

		public event PlayerMissionFullOverEventHandle MissionFullOver;

		public event PlayerQuestFinishEventHandel PlayerQuestFinish;

		public event PlayerGameOverCountTeamEventHandle GameOverCountTeam;

		public event PlayerMarryTeamEventHandle GameMarryTeam;

		public event PlayerUseBugle UseBugle;

		public event PlayerMarryEventHandel PlayerMarry;

		public event PlayerDispatchesEventHandel PlayerDispatches;

		public event PlayerGameOverEventHandle GameOver;

		public event PlayerGameOverEvent2v2Handle GameOver2v2;

		public event PlayerGameOverEvent3v3Handle GameOver3v3;

		public event PlayerGameOverEvent4v4Handle GameOver4v4;

		public event PlayerAcademyEventHandle AcademyEvent;

		public event PlayerUpLevelVipEventHandle UpLevelVipEvent;

		public event WorldBossDamageEventHandle WorldBossDamageEvent;

		public GamePlayer(int playerId, string account, GameClient client, PlayerInfo info)
		{
			m_playerId = playerId;
			m_account = account;
			m_client = client;
			m_character = info;
			m_equipBag = new PlayerEquipInventory(this);
			AvatarBag = new PlayerAvataInventory(this);
			m_propBag = new PlayerInventory(this, true, 96, 1, 0, true);
			m_ConsortiaBag = new PlayerInventory(this, true, 100, 11, 0, true);
			m_BankBag = new PlayerInventory(this, true, 100, 51, 0, true);
			m_storeBag = new PlayerInventory(this, true, 20, 12, 0, true);
			m_fightBag = new PlayerInventory(this, false, 3, 3, 0, false);
			m_tempBag = new PlayerInventory(this, false, 60, 4, 0, true);
			m_caddyBag = new PlayerInventory(this, false, 30, 5, 0, true);
			FarmBag = new PlayerInventory(this, true, 30, 13, 0, true);
			Vegetable = new PlayerInventory(this, true, 30, 14, 0, true);
			m_food = new PlayerInventory(this, true, 30, 34, 0, true);
			m_petEgg = new PlayerInventory(this, true, 30, 35, 0, true);
			m_cardBag = new CardInventory(this, true, 100, 5);
			Farm = new PlayerFarm(this, true, 30, 0);
			m_petBag = new PetInventory(this, true, 20, 8, 0);
			m_rank = new PlayerRank(this, true);
			m_playerProp = new PlayerProperty(this);
			m_battle = new PlayerBattle(this, true);
			m_extra = new PlayerExtra(this, true);
			m_playerActive = new PlayerActives(this, true);
			m_questInventory = new QuestInventory(this);
			m_achievementInventory = new AchievementInventory(this);
			m_eventLiveInventory = new EventInventory(this);
			m_bufferList = new BufferList(this);
			m_fightBuffInfo = new List<BufferInfo>();
			m_equipEffect = new List<int>();
			GemStone = new List<UserGemStone>();
			m_userDrills = new Dictionary<int, UserDrillInfo>();
			CardBuff = new List<int>();
			GPAddPlus = 1.0;
			OfferAddPlus = 1.0;
			m_toemview = true;
			X = 646;
			Y = 1241;
			MarryMap = 0;
			LastChatTime = DateTime.Today;
			LastFigUpTime = DateTime.Today;
			LastDrillUpTime = DateTime.Today;
			LastOpenPack = DateTime.Today;
			LastMovePlaceItem = DateTime.Today;
			LastBuffPropItem = DateTime.Today;
			m_showPP = false;
			m_converter = new UTF8Encoding();
			BossBoxStartTime = DateTime.Now;
			WaitingProcessor = DateTime.Now;
			ResetLottery();
			IsAccountLimit = false;
			CardResetTempProp = new Dictionary<int, int[]>();
			userLabyrinthInfo = null;
			m_consortiaProcessor = new ConsortiaLogicProcessor();
			m_petProcessor = new PetLogicProcessor();
			BlockReceiveMoney = false;
			LastOpenHole = DateTime.Now;
			LastTakeCardItem = null;
			m_equipGhostList = new Dictionary<string, UserEquipGhostInfo>();
			LittleGameInfo = new PlayerLittleGameInfo()
			{
				Actions = new TriggeredQueue<string, GamePlayer>(this),
				X = 275,
				Y = 30
			};
		}

		public void UpdatePublicPlayer(string tempStyle = "")
		{
			PlayerCharacter.tempStyle = tempStyle;
			var pkg = Out.SendUpdatePublicPlayer(PlayerCharacter, MatchInfo, Extra.Info);
			if (m_currentRoom != null)
			{
				m_currentRoom.SendToAll(pkg, this);
			}
		}

		public int AddAchievementPoint(int value)
		{
			if (value > 0)
			{
				m_character.AchievementPoint += value;
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public void SendUpdatePublicPlayer()
		{
			Out.SendUpdatePublicPlayer(PlayerCharacter, MatchInfo, Extra.Info);
		}

		public void AddExpVip(int value)
		{
			List<int> list = GameProperties.VIPExp();

			this.PlayerCharacter.VIPExp += value;


			for (int i = 0; i < list.Count; i++)
			{
				int vIPExp = m_character.VIPExp;
				int vIPLevel = m_character.VIPLevel;
				if (vIPLevel == 9)
				{
					m_character.VIPExp = list[8];
					break;
				}
				if (vIPLevel < 9 && canUpLv(vIPExp, vIPLevel))
				{
					//Console.WriteLine($"Before{m_character.VIPLevel}");
					m_character.VIPLevel++;
                    //Console.WriteLine($"After{m_character.VIPLevel}");
                    #region Vip OpenBoss
                    if (m_character.VIPLevel == 6)
						m_extra.Info.BuyCountOpenBoss = 3;
					if (m_character.VIPLevel == 7)
						m_extra.Info.BuyCountOpenBoss = 4;
					if (m_character.VIPLevel == 8)
						m_extra.Info.BuyCountOpenBoss = 5;
                    if (m_character.VIPLevel == 9)
                        m_extra.Info.BuyCountOpenBoss = 7;
                    #endregion
                    if (m_character.VIPLevel >= 7 && PetBag != null)
					{
						PetBag.UpdatePetFiveKillSlot(m_character.VIPLevel);
					}
					DailyRecordInfo record = new DailyRecordInfo
					{
						UserID = PlayerCharacter.ID,
						Type = 28,
						Value = m_character.VIPLevel.ToString()
					};
					new PlayerBussiness().AddDailyRecord(record);
					OnUpLevelVipEvent();
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TANG_CAP_VIP))
				{
					Extra.UpdateEventCondition(1, PlayerCharacter.VIPLevel);
				}
			}
			if (true)
			{
                GmActivityMgr.OnPlayerUpgradeVIP(this, m_character.VIPLevel);
                Out.SendOpenVIP(this);
				OnPropertiesChange();
			}
		}

		public int AddGold(int value)
		{
			if (value > 0)
			{
				m_character.Gold += value;
				if (m_character.Gold == int.MinValue)
				{
					m_character.Gold = int.MaxValue;
					SendMessage("Limite excedido!");
				}
				OnPlayerAddItem("Gold", value);
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

        public bool MaxLevel()
        {
            return Level == LevelMgr.MaxLevel;
        }

        public int AddGP(int gp)
        {
            return AddGP(gp, true, true);
        }

        public int AddGP(int gp, bool CanDupeExp, bool GmActivity)
        {
            if (gp > 0)
            {
                long addGp = gp;
                if (AntiAddictionMgr.ISASSon)
                {
                    addGp = (long)(addGp * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
                }
                addGp = (int)(addGp * RateMgr.GetRate(eRateType.Experience_Rate));
                if (CanDupeExp)
                {
                    var oldGp = addGp;
                    if (GPAddPlus > 0)
                    {
                        addGp = (long)(oldGp * GPAddPlus);
                    }
                }
                if (MaxLevel())
                {
                    var offer = addGp / 100;
                    if (offer > 0)
                    {
                        AddOffer(offer > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(offer));
                        SendMessage($"Max level kinh nghiệm quy đổi thành {offer} công trạng");
                    }
                }
                else
                {
                    PlayerCharacter.GP += Convert.ToInt32(addGp);

                    if (PlayerCharacter.GP < 1)
                    {
                        PlayerCharacter.GP = 1;
                    }

                    Level = LevelMgr.GetLevel(PlayerCharacter.GP);
                    UpdateFightPower();
                }
                if (GmActivity)
                {
                    GmActivityMgr.OnUpdateExp(this, Convert.ToInt32(addGp));
                }
                OnPropertiesChanged();
                return addGp > Int32.MaxValue ? Int32.MaxValue : Convert.ToInt32(addGp);
            }
            return 0;
        }

        /*public int AddGP(int gp)
		{
			if (gp >= 0)
			{
				if (AntiAddictionMgr.ISASSon)
				{
					gp = (int)((double)gp * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
				}
				gp = (int)((float)gp * RateMgr.GetRate(eRateType.Experience_Rate));
				if (GPAddPlus > 0.0)
				{
					gp = (int)((double)gp * GPAddPlus);
				}
				
				Level = LevelMgr.GetLevel(m_character.GP + gp);
				int maxLevel = LevelMgr.MaxLevel;
				LevelInfo info = LevelMgr.FindLevel(maxLevel);
				int addGp = 0;
				if (Level == maxLevel && info != null)
				{
					addGp = info.GP - m_character.GP;
					m_character.GP = info.GP;
					int num2 = (gp - addGp) / 1000;
					if (num2 > 0)
					{
						AddOffer(num2);
						this.Out.SendMessage(eMessageType.Normal, "Vì bạn đã đạt cấp tối đa nên số kinh nghiệm tương ứng sẽ chuyển thành công trạng");
					}
				}
				else
				{
					addGp = gp;
					m_character.GP += gp;
					if (m_character.GP < 1)
					{
						m_character.GP = 1;
					}
				}
				UpdateFightPower();
				GmActivityMgr.OnUpdateExp(this, addGp);
				OnPropertiesChanged();
				return gp;
			}
			return 0;
		}*/

        public void AddGift(eGiftType type)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			bool testActive = GameProperties.TestActive;
			switch (type)
			{
				case eGiftType.MONEY:
					if (testActive)
					{
						AddMoney(GameProperties.FreeMoney);
					}
					break;
				case eGiftType.SMALL_EXP:
					{
						string[] strArray = GameProperties.FreeExp.Split('|');
						ItemTemplateInfo info = ItemMgr.FindItemTemplate(Convert.ToInt32(strArray[0]));
						if (info != null)
						{
							list.Add(ItemInfo.CreateFromTemplate(info, Convert.ToInt32(strArray[1]), 102));
						}
						break;
					}
				case eGiftType.BIG_EXP:
					{
						string[] strArray2 = GameProperties.BigExp.Split('|');
						ItemTemplateInfo info2 = ItemMgr.FindItemTemplate(Convert.ToInt32(strArray2[0]));
						if (info2 != null && testActive)
						{
							list.Add(ItemInfo.CreateFromTemplate(info2, Convert.ToInt32(strArray2[1]), 102));
						}
						break;
					}
				case eGiftType.PET_EXP:
					{
						string[] strArray3 = GameProperties.PetExp.Split('|');
						ItemTemplateInfo info3 = ItemMgr.FindItemTemplate(Convert.ToInt32(strArray3[0]));
						if (info3 != null && testActive)
						{
							list.Add(ItemInfo.CreateFromTemplate(info3, Convert.ToInt32(strArray3[1]), 102));
						}
						break;
					}
			}
			foreach (ItemInfo info4 in list)
			{
				info4.IsBinds = true;
				AddTemplate(info4, info4.Template.BagType, info4.Count, eGameView.dungeonTypeGet);
			}
		}

		public int AddGiftToken(int value)
		{
			if (value > 0)
			{
				m_character.GiftToken += value;
				OnPlayerAddItem("GiftToken", value);
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public bool AddItem(ItemInfo item)
		{
			AbstractInventory itemInventory = GetItemInventory(item.Template);
			return itemInventory.AddItem(item, itemInventory.BeginSlot);
		}

		public int AddLeagueMoney(int value)
		{
			if (value > 0)
			{
				m_battle.MatchInfo.dailyScore += value;
				m_battle.MatchInfo.weeklyScore += value;
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

        public void AddJampsCurrency(int value)
        {
            this.PlayerCharacter.jampsCurrency += value;
            if (this.PlayerCharacter.jampsCurrency <= int.MinValue)
                this.PlayerCharacter.jampsCurrency = int.MaxValue;
            this.OnPropertiesChanged();
        }


        public void AddLog(string type, string content)
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				bussiness.AddUserLogEvent(PlayerCharacter.ID, PlayerCharacter.UserName, PlayerCharacter.NickName, type, content);
			}
		}

		public int AddMoney(int value)
		{
			return AddMoney(value, igroneAll: true);
		}

		public int AddMoney(int value, bool igroneAll)
		{
			if (value > 0)
			{
				if (!igroneAll && BlockReceiveMoney)
				{
					SendMessage("Vì bạn đăng nhập quá 3 tài khoản trên cùng một máy nên bạn chỉ được nhận 30% xu đạt được");
					m_character.Money += (int)Math.Round((double)value * 0.3);
					OnPropertiesChanged();
					return (int)Math.Round((double)value * 0.3);
				}
				m_character.Money += value;
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public int AddBadLuckCaddy(int value)
		{
			if (value > 0)
			{
				m_character.badLuckNumber += value;
				if (m_character.badLuckNumber == int.MinValue)
				{
					m_character.badLuckNumber = int.MaxValue;
					SendMessage("Limite de gemas excedido.");
				}
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public int AddOffer(int value)
		{
			return AddOffer(value, IsRate: true);
		}

		public int RefreshLeagueGetReward(int awardGot, int Score)
		{
			if (awardGot > 0)
			{
				MatchInfo.leagueItemsGet = awardGot;
				MatchInfo.weeklyScore -= Score;
				OnPropertiesChanged();
				return awardGot;
			}
			return 0;
		}

		public int AddOffer(int value, bool IsRate)
		{
			if (value > 0)
			{
				if (AntiAddictionMgr.ISASSon)
				{
					value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
				}
				if (IsRate)
				{
					value *= (((int)OfferAddPlus == 0) ? 1 : ((int)OfferAddPlus));
				}
				m_character.Offer += value;	
				OnFightAddOffer(value);
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public int AddPetScore(int value)
		{
			if (value > 0)
			{
				m_character.petScore += value;
				if (m_character.petScore == int.MinValue)
				{
					m_character.petScore = int.MaxValue;
					SendMessage("Vượt quá giới hạn petScore");
				}
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public void AddPrestige(bool isWin)
		{
			BattleData.AddPrestige(isWin);
		}

		public void UpdateRestCount()
		{
			BattleData.Update();
		}

		public int AddRichesOffer(int value)
		{
			if (value > 0)
			{
				m_character.RichesOffer += value;
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public int AddRobRiches(int value)
		{
			if (value > 0)
			{
				if (AntiAddictionMgr.ISASSon)
				{
					value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
				}
				m_character.RichesRob += value;
				OnPlayerAddItem("RichesRob", value);
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public int AddScore(int value)
		{
			if (value > 0)
			{
				m_character.Score += value;
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public bool AddTemplate(ItemInfo cloneItem)
		{
			return AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eGameView.OtherTypeGet);
		}

		/*public bool AddTemplate(List<ItemInfo> infos)
		{
			return AddTemplate(infos, eGameView.OtherTypeGet);
		}*/

		public bool AddTemplate(ItemInfo cloneItem, string name)
		{
			return AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eGameView.OtherTypeGet, name);
		}

		public bool AddTemplate(List<ItemInfo> infos)
		{
			if (infos != null && infos.Count > 0)
			{
				var itemOverDue = new List<ItemInfo>();
				foreach (var item in infos)
				{
					//item.IsBinds = true;
					if (!StackItemToAnother(item) && !AddItem(item))
					{
						itemOverDue.Add(item);
					}
				}
				BagFullSendToMail(itemOverDue);
				return true;
			}
			return false;
            #region
            /*if (infos != null && infos.Count > 0)
			{
				List<ItemInfo> list = new List<ItemInfo>();
				foreach (ItemInfo info in infos)
				{
					info.IsBinds = true;
					if (!StackItemToAnother(info) && !AddItem(info))
					{
						list.Add(info);
					}
				}
				BagFullSendToMail(list);
				return true;
			}
			return false;*/
            #endregion
        }

        public bool AddTemplate(List<ItemInfo> infos, int count, eGameView gameView)
		{
			if (infos != null)
			{
				List<ItemInfo> list = new List<ItemInfo>();
				foreach (ItemInfo info in infos)
				{
					info.IsBinds = true;
					info.Count = count;
					if (!StackItemToAnother(info) && !AddItem(info))
					{
						list.Add(info);
					}
				}
				BagFullSendToMail(list);
				return true;
			}
			return false;
		}

		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView)
		{
			if (eBageType.FightBag == bagType)
			{
				return FightBag.AddItem(cloneItem);
			}
			return AddTemplate(cloneItem, bagType, count, gameView, "no");
		}

		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eGameView gameView, string Name)
		{
			PlayerInventory inventory = GetInventory(bagType);
			if (cloneItem != null)
			{
				List<ItemInfo> infos = new List<ItemInfo>();
				if (!inventory.StackItemToAnother(cloneItem) && !inventory.AddItem(cloneItem))
				{
					infos.Add(cloneItem);
				}
				BagFullSendToMail(infos);
				if (Name != "no")
				{
					//SendItemNotice(cloneItem, (int)gameView, Name);
				}
				return true;
			}
			return false;
		}

		public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, bool backToMail)
		{
			PlayerInventory inventory = GetInventory(bagType);
			if (inventory != null && !cloneItem.Template.IsSpecial())
			{
				if (inventory.AddTemplate(cloneItem, count))
				{
					if (CurrentRoom != null && CurrentRoom.IsPlaying)
					{
						SendItemNotice(cloneItem);
					}
					return true;
				}
				if (backToMail && cloneItem.Template.CategoryID != 10)
				{
					SendItemsToMail(cloneItem, LanguageMgr.GetTranslation("GamePlayer.Msg18"), LanguageMgr.GetTranslation("GamePlayer.Msg18"), eMailType.BuyItem);
				}
			}
			return false;
		}

		public bool RemoveTemplateInShop(int templateid, int count)
		{
			ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateid);
			if (itemTemplate != null)
			{
				PlayerInventory itemInventory = GetItemInventory(itemTemplate);
				if (itemInventory != null)
				{
					return itemInventory.RemoveTemplate(templateid, count);
				}
			}
			return false;
		}

		public int GetTemplateCount(int templateId)
		{
			ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(templateId);
			if (itemTemplate != null)
			{
				PlayerInventory itemInventory = GetItemInventory(itemTemplate);
				if (itemInventory != null)
				{
					return itemInventory.GetItemCount(templateId);
				}
			}
			return 0;
		}

		private void SendItemNotice(ItemInfo item)
		{
			GSPacketIn gsPacketIn = new GSPacketIn(14);
			gsPacketIn.WriteString(PlayerCharacter.NickName);
			gsPacketIn.WriteInt(1);
			gsPacketIn.WriteInt(item.TemplateID);
			gsPacketIn.WriteBoolean(item.IsBinds);
			gsPacketIn.WriteInt(1);
			if (item.Template.Quality >= 3 && item.Template.Quality < 5)
			{
				if (CurrentRoom != null)
				{
					CurrentRoom.SendToTeam(gsPacketIn, CurrentRoomTeam, this);
				}
			}
			else
			{
				if (item.Template.Quality < 5)
				{
					return;
				}
				GameServer.Instance.LoginServer.SendPacket(gsPacketIn);
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				GamePlayer[] array = allPlayers;
				foreach (GamePlayer allPlayer in array)
				{
					if (allPlayer != this)
					{
						allPlayer.Out.SendTCP(gsPacketIn);
					}
				}
			}
		}

		public void ApertureEquip(int level)
		{
			EquipShowImp(0, (level < 5) ? 1 : ((level < 7) ? 2 : 3));
		}

		public void BagFullSendToMail(List<ItemInfo> infos)
		{
			if (infos.Count > 0)
			{
				bool flag = false;
				using (new PlayerBussiness())
				{
					flag = SendItemsToMail(infos, "Túi của bạn đầy vật phẩm được gửi ra thư", "Túi đầy", eMailType.BuyItem);
				}
				if (flag)
				{
					Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
		}

		public void BeginAllChanges()
		{
			BeginChanges();
			m_bufferList.BeginChanges();
			m_equipBag.BeginChanges();
			m_propBag.BeginChanges();
			FarmBag.BeginChanges();
			Vegetable.BeginChanges();
		}

		public void BeginChanges()
		{
			Interlocked.Increment(ref m_changed);
		}

		public void RemoveLotteryItems(int templateId, int count)
		{
			foreach (ItemBoxInfo lotteryItem in LotteryItems)
			{
				if (lotteryItem.TemplateId == templateId && lotteryItem.ItemCount == count)
				{
					LotteryItems.Remove(lotteryItem);
					break;
				}
			}
		}

		public bool CanEquip(ItemTemplateInfo item)
		{
			bool flag = true;
			string message = "";
			if (!item.CanEquip)
			{
				flag = false;
				message = LanguageMgr.GetTranslation("Game.Server.GameObjects.NoEquip");
			}
			else if (m_character.Grade < item.NeedLevel)
			{
				flag = false;
				message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanLevel");
			}
			if (!flag)
			{
				Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, message);
			}
			return flag;
		}

		public int GetVIPNextLevelDaysNeeded(int viplevel, int vipexp)
		{
			if (viplevel != 0 && vipexp > 0 && viplevel <= 8)
			{
				int result = 0;
				if (PlayerCharacter.typeVIP == 2)
				{
					result = (Array.ConvertAll(GameProperties.VIPExpForEachLv.Split('|'), int.Parse)[viplevel] - vipexp) / 15;
				}
				else if (PlayerCharacter.typeVIP == 1)
				{
					result = (Array.ConvertAll(GameProperties.VIPExpForEachLv.Split('|'), int.Parse)[viplevel] - vipexp) / 10;
				}
				OnVIPUpgrade(m_character.VIPLevel, m_character.VIPExp);
				return result;
			}
			OnVIPUpgrade(m_character.VIPLevel, m_character.VIPExp);
			return 0;
		}

		public bool canUpLv(int exp, int _curLv)
		{
			List<int> exps = GameProperties.VIPExp();
			if (exp >= exps[0] && _curLv == 0)
			{
				return true;
			}
			if (exp >= exps[1] && _curLv == 1)
			{
				return true;
			}
			if (exp >= exps[2] && _curLv == 2)
			{
				return true;
			}
			if (exp >= exps[3] && _curLv == 3)
			{
				return true;
			}
			if (exp >= exps[4] && _curLv == 4)
			{
				return true;
			}
			if (exp >= exps[5] && _curLv == 5)
			{
				return true;
			}
			if (exp >= exps[6] && _curLv == 6)
			{
				return true;
			}
			if (exp >= exps[7] && _curLv == 7)
			{
				return true;
			}
			if (exp >= exps[8] && _curLv == 8)
			{
				return true;
			}
			return false;
		}

		public void ClearCaddyBag()
		{
			List<ItemInfo> infos = new List<ItemInfo>();
			for (int i = 0; i < CaddyBag.Capalility; i++)
			{
				ItemInfo itemAt = CaddyBag.GetItemAt(i);
				if (itemAt != null)
				{
					ItemInfo item = ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
					item.Count = 1;
					infos.Add(item);
				}
			}
			CaddyBag.ClearBag();
			AddTemplate(infos);
		}

		public int GetMedalNum()
		{
			int itemCount = PropBag.GetItemCount(11408);
			int value2 = 0;
			if (m_character.IsConsortia)
			{
				value2 = ConsortiaBag.GetItemCount(11408);
			}
			int value3 = BankBag.GetItemCount(11408);
			return itemCount + value2 + value3;
		}

		public bool SendEventLiveRewards(EventLiveInfo eventLiveInfo)
		{
			List<EventLiveGoods> eventGoods = EventLiveMgr.GetEventGoods(eventLiveInfo);
			new List<ItemInfo>();
			foreach (EventLiveGoods reward in eventGoods)
			{
				if (reward.TemplateID != -100 && reward.TemplateID != -200)
				{
					ItemTemplateInfo temp = ItemMgr.FindItemTemplate(reward.TemplateID);
					if (temp != null)
					{
						int NeedSex = (PlayerCharacter.Sex ? 1 : 2);
						if (temp.NeedSex != 0 && temp.NeedSex != NeedSex)
						{
							continue;
						}
						int tempCount = reward.Count;
						for (int len = 0; len < tempCount; len += temp.MaxCount)
						{
							int count = ((len + temp.MaxCount > tempCount) ? (tempCount - len) : temp.MaxCount);
							ItemInfo item = ItemInfo.CreateFromTemplate(temp, count, 120);
							if (item != null)
							{
								item.StrengthenLevel = reward.StrengthenLevel;
								item.AttackCompose = reward.AttackCompose;
								item.DefendCompose = reward.DefendCompose;
								item.AgilityCompose = reward.AgilityCompose;
								item.LuckCompose = reward.LuckCompose;
								item.IsBinds = reward.IsBind;
								item.ValidDate = reward.ValidDate;
								SendItemToMail(item, LanguageMgr.GetTranslation("Game.Server.GameObjects.SendEventLiveRewards.Content", eventLiveInfo.Description), LanguageMgr.GetTranslation("Game.Server.GameObjects.SendEventLiveRewards.Title"), eMailType.Manage);
							}
						}
					}
				}
				if (reward.TemplateID == -100)
				{
					AddGold(reward.Count);
				}
				if (reward.TemplateID == -200)
				{
					AddMoney(reward.Count);
				}
				if (reward.TemplateID == -300)
				{
					AddGiftToken(reward.Count);
				}
			}
			return true;
		}

		public void ClearConsortia()
		{
			PlayerCharacter.ClearConsortia();
			OnPropertiesChanged();
			QuestInventory.ClearConsortiaQuest();
			var sender = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender");
			var title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
			//StoreBag.SendAllItemsToMail(sender, title, eMailType.StoreCanel);
            #region OLD Clear
            /*if (isclear)
			{
				PlayerCharacter.ClearConsortia();
			}
			if (PlayerCharacter.ConsortiaID != 0 || ConsortiaBag.GetItems().Count <= 0)
			{
				return;
			}
			List<ItemInfo> Items = new List<ItemInfo>();
			foreach (ItemInfo item2 in ConsortiaBag.GetItems())
			{
				if (item2.IsValidItem())
				{
					Items.Add(item2);
				}
			}
			OnPropertiesChanged();
			QuestInventory.ClearConsortiaQuest();
			string sender = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender");
			string title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
			if (SendItemsToMail(Items, sender, title, eMailType.StoreCanel))
			{
				ConsortiaBag.ClearBag();
			}*/
            #endregion
        }

        public bool ClearFightBag()
		{
			FightBag.ClearBag();
			return true;
		}

        #region OLD ClearFightBuffOneMatch
        /*public void ClearFightBuffOneMatch()
		{
			List<BufferInfo> list = new List<BufferInfo>();
			foreach (BufferInfo info in FightBuffs)
			{
				if (info != null && info.Type >= 400 && info.Type <= 406)
				{
					list.Add(info);
				}
			}
			foreach (BufferInfo info2 in list)
			{
				FightBuffs.Remove(info2);
			}
			list.Clear();
		}*/
        #endregion

        public void ClearFightBuffOneMatch()
		{
			var tempBuff = new List<BufferInfo>();
			foreach (var info in FightBuffs)
			{
				if (info != null)
				{
					switch (info.Type)
					{
						case (int)BuffType.WorldBossHP:
						case (int)BuffType.WorldBossHP_MoneyBuff:
						case (int)BuffType.WorldBossAttrack:
						case (int)BuffType.WorldBossAttrack_MoneyBuff:
						case (int)BuffType.WorldBossMetalSlug:
						case (int)BuffType.WorldBossAncientBlessings:
						case (int)BuffType.WorldBossAddDamage:
							tempBuff.Add(info);
							break;
					}
				}
			}

			foreach (var info in tempBuff)
			{
				FightBuffs.Remove(info);
			}

			tempBuff.Clear();
		}

		public void ClearFootballCard()
		{
			for (int i = 0; i < CardsTakeOut.Length; i++)
			{
				CardsTakeOut[i] = null;
			}
		}

		public void ClearStoreBag()
		{
			var infos = new List<ItemInfo>();
			for (var i = 0; i < StoreBag.Capalility; i++)
            {
				var itemAt = StoreBag.GetItemAt(i);
				if ((itemAt != null))
                {
					var newitem = ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
					newitem.Count = itemAt.Count;
					infos.Add(newitem);
                }
            }
			StoreBag.ClearBag();
			AddTemplate(infos);
            #region OLD
            /*List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 0; i < StoreBag.Capalility; i++)
			{
				ItemInfo itemAt = StoreBag.GetItemAt(i);
				int num = 0;
				if (itemAt == null)
				{
					continue;
				}
				if (itemAt.Template.BagType == eBageType.PropBag)
				{
					num = PropBag.FindFirstEmptySlot();
					if (!PropBag.StackItemToAnother(itemAt) && !PropBag.AddItemTo(itemAt, num))
					{
						list.Add(itemAt);
					}
					else
					{
						StoreBag.TakeOutItem(itemAt);
					}
				}
				else
				{
					num = EquipBag.FindFirstEmptySlot(31);
					if (!EquipBag.StackItemToAnother(itemAt) && !EquipBag.AddItemTo(itemAt, num))
					{
						list.Add(itemAt);
					}
					else
					{
						StoreBag.TakeOutItem(itemAt);
					}
				}
			}
			StoreBag.ClearBag();
			if (list.Count > 0)
			{
				SendItemsToMail(list, "Túi đầy vật phẩm từ tiệm rèn trả về thư.", "Vật phẩm trả về từ Tiệm rèn.", eMailType.StoreCanel);
				
			}*/
            #endregion
        }

        public bool ClearTempBag()
		{
			TempBag.ClearBag();
			return true;
		}

		public void CommitAllChanges()
		{
			CommitChanges();
			m_bufferList.CommitChanges();
			m_equipBag.CommitChanges();
			m_propBag.CommitChanges();
			FarmBag.CommitChanges();
			Vegetable.CommitChanges();
		}

		public void CommitChanges()
		{
			Interlocked.Decrement(ref m_changed);
			OnPropertiesChanged();
		}

		public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
		{
			return ConsortiaMgr.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
		}

		public void ContinousVIP(int days)
		{
			DateTime now = DateTime.Now;
			DateTime dateTime2 = (m_character.VIPExpireDay = ((!(m_character.VIPExpireDay < DateTime.Now)) ? m_character.VIPExpireDay.AddDays(days) : DateTime.Now.AddDays(days)));
			now = dateTime2;
			m_character.typeVIP = SetTypeVIP(days);
		}

		public string ConverterPvePermission(char[] chArray)
		{
			string str = "";
			for (int i = 0; i < chArray.Length; i++)
			{
				str += chArray[i];
			}
			return str;
		}

		public List<ItemInfo> CopyDrop(int SessionId, int m_missionInfoId)
		{
			List<ItemInfo> info = null;
			DropInventory.CopyDrop(m_missionInfoId, SessionId, ref info);
			return info;
		}

		public void ChargeToUser()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				int money = 0;
				string title = LanguageMgr.GetTranslation("ChargeToUser.Title");
				if (!bussiness.ChargeToUser(m_character.UserName, ref money, m_character.NickName))
					return;
				string content = LanguageMgr.GetTranslation("ChargeToUser.Content", money);
				if (money <= 0)
					return;
				// PlayerBussiness pb2 = new PlayerBussiness();
				DateTime ExpireDayOut = DateTime.Now;
				int renewalDays = money / 1000;
				int typeVIP = SetTypeVIP(money / 1000);
				bussiness.VIPRenewal(PlayerCharacter.NickName, renewalDays, typeVIP, ref ExpireDayOut);
				if (PlayerCharacter.typeVIP == 0)
				{
					OpenVIP(money / 1000);
					AddExpVip(money / 1000);
				}
				else
				{
					ContinousVIP(money / 1000);
					AddExpVip(money / 1000);
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.NAPXU_TICHLUY))
				{
					Extra.UpdateEventCondition(3, money, true, 0);
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.NAPXU_DACBIET))
				{
					Extra.UpdateEventCondition(5, money, true, 0);
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TANG_CAP_VIP))
				{
					Extra.UpdateEventCondition(1, PlayerCharacter.VIPLevel);
				}
				// Out.SendOpenVIP(this);
				// OnVIPUpgrade(PlayerCharacter.VIPLevel, PlayerCharacter.VIPExp);
				OnPropertiesChange();
				#region OLD Charge
				/*PlayerBussiness pb = new PlayerBussiness();
				DateTime now = DateTime.Now;
				int typeVIP = SetTypeVIP(money / 600);
				int validDate = money / 600;
				pb.VIPRenewal(PlayerCharacter.NickName, validDate, typeVIP, ref now);
				if (PlayerCharacter.typeVIP == 0)
				{
					OpenVIP(money / 600);
					AddExpVip(money / 600);
				}
				else
				{
					ContinousVIP(money / 600);
					AddExpVip(money / 600);
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.UPGRADE_VIP_ACTIVE))
				{
					Extra.UpdateEventCondition(5, PlayerCharacter.VIPLevel);
				}*/

				#endregion
				SendMailToUser(bussiness, content, title, eMailType.Manage);
				HappyRechargeData.LotteryCount += money / 3000;
				AddMoney(money);
				OnMoneyCharge(money);
				GmActivityMgr.OnRechargeMoney(this, money);
				Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
				/*if (Extra.CheckNoviceActiveOpen(NoviceActiveType.RECHANGE_MONEY_ACTIVE))
				{
					Extra.UpdateEventCondition(4, money, isPlus: true, 0);
				}
				if(Extra.CheckNoviceActiveOpen(NoviceActiveType.RECHARGE_SPECIAL))
                {
					Extra.UpdateEventCondition(7, money, isPlus: true, 0);
                }*/
				if (!PlayerCharacter.IsRecharged)
				{
					PlayerCharacter.IsRecharged = true;
					Out.SendUpdateFirstRecharge(PlayerCharacter.IsRecharged, PlayerCharacter.IsGetAward);
				}
				if (Extra.Info.LeftRoutteRate > 0f)
				{
					int Rate = (int)((float)money * Extra.Info.LeftRoutteRate);
					if (Rate > 0)
					{
						SendMoneyMailToUser(LanguageMgr.GetTranslation("GameServer.LeftRotterMail.Title"), LanguageMgr.GetTranslation("GameServer.LeftRotterMail.Content", Rate), Rate, eMailType.BuyItem);
					}
					Extra.Info.LeftRoutteRate = 0f;
					Out.SendLeftRouleteOpen(Extra.Info);
				}
			}
		}

		public void ChecVipkExpireDay()
		{
			if (m_character.IsVIPExpire())
			{
				m_character.CanTakeVipReward = false;
				m_character.typeVIP = 0;
			}
			else if (m_character.IsLastVIPPackTime())
			{
				m_character.CanTakeVipReward = true;
			}
			else
			{
				m_character.CanTakeVipReward = false;
			}
		}

		public bool DeletePropItem(int place)
		{
			FightBag.RemoveItemAt(place);
			return true;
		}

		public virtual void Disconnect()
		{
			m_client.Disconnect();
		}

		public bool EquipItem(ItemInfo item, int place)
		{
			if (!item.CanEquip() || item.BagType != m_equipBag.BagType)
			{
				return false;
			}
			int toSlot = m_equipBag.FindItemEpuipSlot(item.Template);
			int num2;
			if ((uint)(toSlot - 9) <= 1u)
			{
				int num3;
				switch (place)
				{
					case 10:
						num3 = 0;
						break;
					default:
						num3 = 1;
						break;
					case 9:
						num3 = 0;
						break;
				}
				num2 = num3;
			}
			else
			{
				num2 = 1;
			}
			if (num2 == 0)
			{
				toSlot = place;
			}
			else if ((toSlot == 7 || toSlot == 8) && (place == 7 || place == 8))
			{
				toSlot = place;
			}
			return m_equipBag.MoveItem(item.Place, toSlot, item.Count);
		}

		private void EquipShowImp(int categoryID, int para)
		{
			UpdateHide(m_character.Hide + (int)(Math.Pow(10.0, categoryID) * (double)(para - m_character.Hide / (int)Math.Pow(10.0, categoryID) % 10)));
		}

		public bool FindEmptySlot(eBageType bagType)
		{
			PlayerInventory inventory = GetInventory(bagType);
			inventory.FindFirstEmptySlot();
			return inventory.FindFirstEmptySlot() > 0;
		}

		public void FriendsAdd(int playerID, int relation)
		{
			if (!_friends.ContainsKey(playerID))
			{
				_friends.Add(playerID, relation);
			}
			else
			{
				_friends[playerID] = relation;
			}
		}

		public void FriendsRemove(int playerID)
		{
			if (_friends.ContainsKey(playerID))
			{
				_friends.Remove(playerID);
			}
		}

		public List<ItemInfo> GetAllEquipItems()
        {
			var list = new List<ItemInfo>();
			for (var place = 0; place < EquipBag.BeginSlot; place++)
			{
				var item = EquipBag.GetItemAt(place);
				if(item != null)
                {
					list.Add(item);
                }
            }
			return list;
        }

		public double GetBaseAgility()
		{
			return 1.0 - (double)m_character.Agility * 0.001;
		}

		public double GetBaseAttack()
		{
			int num1 = 0;
			int num2 = 0;
			var avatarAttack = 0.0;
			var avatarDefend = 0.0;
			foreach (UsersCardInfo card in CardBag.GetCards(0, 4))
			{
				ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(card.TemplateID);
				if (itemTemplate != null)
				{
					num2 += itemTemplate.Property4 + card.Damage;
				}
			}

			var allEquipItems = GetAllEquipItems();
			foreach (var item in allEquipItems)
			{
				var info = SubActiveMgr.GetSubActiveInfo(item);
				if (info != null && SubActiveMgr.Checked())
				{
					num2 += info.GetValue("6");
				}
			}
			PlayerProp.totalDamage = (int)num2;// + PlayerCharacter.explorerManualInfo.Damage;
			AvatarBag.AddBasePropAvatarColection(ref avatarDefend, ref avatarAttack);
			PlayerProp.UpadateBaseProp(true, "Damage", "Avatar", avatarAttack);
			var rankProp = Rank.GetSingleRank(PlayerCharacter.Honor);
			if (rankProp != null)
			{
				num2 += rankProp.Damage;
			}
			int baseattack = num1 + num2;
			double totemAttack = TotemMgr.GetTotemProp(PlayerCharacter.totemId, "dam");
			ItemInfo itemAt1 = m_equipBag.GetItemAt(6);
			if (itemAt1 != null)
			{
				double property7 = itemAt1.Template.Property7;
				int num3 = (itemAt1.isGold ? 1 : 0);
				double para2 = itemAt1.StrengthenLevel + num3;
				baseattack += (int)(getHertAddition(property7, para2) + property7);

				/*var egInfo = GetGhostEquip(itemAt1.BagType, itemAt1.Place);
				if(egInfo != null)
                {
					baseattack += Convert.ToInt32(property7 / 200 * Math.Pow(egInfo.Level, 1.2) / 100) * baseattack;
                }*/

				var egInfo = GetGhostEquip(itemAt1.BagType, itemAt1.Place);
				if (egInfo != null)
				{
					baseattack += Convert.ToInt32((property7 / 200 * Math.Pow(egInfo.Level, 1.2) / 100) * baseattack);
				}

				if (itemAt1.Hole1 > 0)
				{
					BaseAttack(itemAt1.Hole1, ref baseattack);
				}
				if (itemAt1.Hole2 > 0)
				{
					BaseAttack(itemAt1.Hole2, ref baseattack);
				}
				if (itemAt1.Hole3 > 0)
				{
					BaseAttack(itemAt1.Hole3, ref baseattack);
				}
				if (itemAt1.Hole4 > 0)
				{
					BaseAttack(itemAt1.Hole4, ref baseattack);
				}
				if (itemAt1.Hole5 > 0)
				{
					BaseAttack(itemAt1.Hole5, ref baseattack);
				}
				if (itemAt1.Hole6 > 0)
				{
					BaseAttack(itemAt1.Hole6, ref baseattack);
				}
			}
			ItemInfo itemAt2 = m_equipBag.GetItemAt(0);
			ItemInfo itemAt3 = m_equipBag.GetItemAt(4);
			if (itemAt2 != null)
			{
				if (itemAt2.Hole1 > 0)
				{
					BaseAttack(itemAt2.Hole1, ref baseattack);
				}
				if (itemAt2.Hole2 > 0)
				{
					BaseAttack(itemAt2.Hole2, ref baseattack);
				}
				if (itemAt2.Hole3 > 0)
				{
					BaseAttack(itemAt2.Hole3, ref baseattack);
				}
				if (itemAt2.Hole4 > 0)
				{
					BaseAttack(itemAt2.Hole4, ref baseattack);
				}
				if (itemAt2.Hole5 > 0)
				{
					BaseAttack(itemAt2.Hole5, ref baseattack);
				}
				if (itemAt2.Hole6 > 0)
				{
					BaseAttack(itemAt2.Hole6, ref baseattack);
				}
			}
			if (itemAt3 != null)
			{
				if (itemAt3.Hole1 > 0)
				{
					BaseAttack(itemAt3.Hole1, ref baseattack);
				}
				if (itemAt3.Hole2 > 0)
				{
					BaseAttack(itemAt3.Hole2, ref baseattack);
				}
				if (itemAt3.Hole3 > 0)
				{
					BaseAttack(itemAt3.Hole3, ref baseattack);
				}
				if (itemAt3.Hole4 > 0)
				{
					BaseAttack(itemAt3.Hole4, ref baseattack);
				}
				if (itemAt3.Hole5 > 0)
				{
					BaseAttack(itemAt3.Hole5, ref baseattack);
				}
				if (itemAt3.Hole6 > 0)
				{
					BaseAttack(itemAt3.Hole6, ref baseattack);
				}
			}
			return baseattack + totemAttack + avatarAttack;
		}

		public void BaseAttack(int template, ref int baseattack)
		{
			ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(template);
			if (itemTemplate != null && itemTemplate.CategoryID == 11 && itemTemplate.Property1 == 31 && itemTemplate.Property2 == 3)
			{
				baseattack += itemTemplate.Property7;
			}
		}

		public double GetBaseBlood()
		{
			ItemInfo itemAt = EquipBag.GetItemAt(12);
			if (itemAt != null)
			{
				return (100.0 + (double)itemAt.Template.Property1 + this.PlayerCharacter.necklaceExpAdd) / 100.0;
			}
			return 1.0;
		}

		public double GetBaseDefence()
		{
			int defence = 0;
			int num1 = 0;
			int mh_guard = 0;
			var setGUARD = 0.0;
			var avatarAttack = 0.0;
			var avatarDefend = 0.0;
			SetsBuildTempMgr.GetSetsBuildProp(PlayerCharacter.fineSuitExp, ref setGUARD);

			foreach (UsersCardInfo card in CardBag.GetCards(0, 4))
			{
				ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(card.TemplateID);
				if (itemTemplate != null)
				{
					num1 += itemTemplate.Property5 + card.Guard;
				}
			}

			var allEquipItems = GetAllEquipItems();
			foreach (var item in allEquipItems)
			{
				var info = SubActiveMgr.GetSubActiveInfo(item);
				if (info != null && SubActiveMgr.Checked())
				{
					num1 += info.GetValue("7");
				}
			}

			var eatPEt = PetBag.EatPets;
			if(eatPEt != null)
            {
				var petMoe = PetMoePropertyMgr.FindPetMoeProperty(eatPEt.hatLevel);
				if(petMoe != null)
                {
					mh_guard += petMoe.Guard;
                }
            }

			var rankProp = Rank.GetSingleRank(PlayerCharacter.Honor);
			if (rankProp != null)
			{
				num1 += rankProp.Guard;
			}

			PlayerProp.totalArmor = (int)num1 + mh_guard;// + PlayerCharacter.explorerManualInfo.Armor;
			AvatarBag.AddBasePropAvatarColection(ref avatarDefend, ref avatarAttack);
			PlayerProp.UpadateBaseProp(true, "Armor", "Avatar", avatarDefend);
			PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Pet", HoGiap);
			ItemInfo itemAt1 = m_equipBag.GetItemAt(6);
			defence += TotemMgr.GetTotemProp(PlayerCharacter.totemId, "gua");
			ItemInfo itemAt2 = m_equipBag.GetItemAt(0);
			ItemInfo itemAt3 = m_equipBag.GetItemAt(4);
			if (itemAt2 != null)
			{
				double property8 = itemAt2.Template.Property7;
				int num3 = (itemAt2.isGold ? 1 : 0);
				double para3 = itemAt2.StrengthenLevel + num3;
				defence += (int)(getHertAddition(property8, para3) + property8);
				var egInfo = GetGhostEquip(itemAt2.BagType, itemAt2.Place);
				if (egInfo != null)
				{
					defence += Convert.ToInt32((property8 / 60 * Math.Pow(egInfo.Level, 1.2) / 100) * defence);
				}
				AddProperty(itemAt2, ref defence);
			}
			if (itemAt3 != null)
			{
				double property7 = itemAt3.Template.Property7;
				int num2 = (itemAt3.isGold ? 1 : 0);
				double para2 = itemAt3.StrengthenLevel + num2;
				defence += (int)(getHertAddition(property7, para2) + property7);
				var egInfo = GetGhostEquip(itemAt3.BagType, itemAt3.Place);
				if (egInfo != null)
				{
					defence += Convert.ToInt32((property7 / 60 * Math.Pow(egInfo.Level, 1.2) / 100) * defence);
				}
				AddProperty(itemAt3, ref defence);
			}
			if (itemAt1 != null)
			{
				AddProperty(itemAt1, ref defence);
			}
			defence += num1 + mh_guard + (int)setGUARD + (int)avatarDefend;
			return defence + HoGiap;
		}

		public void AddProperty(ItemInfo item, ref int defence)
		{
			if (item.Hole1 > 0)
			{
				BaseDefence(item.Hole1, ref defence);
			}
			if (item.Hole2 > 0)
			{
				BaseDefence(item.Hole2, ref defence);
			}
			if (item.Hole3 > 0)
			{
				BaseDefence(item.Hole3, ref defence);
			}
			if (item.Hole4 > 0)
			{
				BaseDefence(item.Hole4, ref defence);
			}
			if (item.Hole5 > 0)
			{
				BaseDefence(item.Hole5, ref defence);
			}
			if (item.Hole6 > 0)
			{
				BaseDefence(item.Hole6, ref defence);
			}
		}

		public void BaseDefence(int template, ref int defence)
		{
			ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(template);
			if (itemTemplate != null && itemTemplate.CategoryID == 11 && itemTemplate.Property1 == 31 && itemTemplate.Property2 == 3)
			{
				defence += itemTemplate.Property8;
			}
		}

		public void PVEFightMessage(string translation, ItemInfo itemInfo, int areaID)
		{
			if (translation != null)
			{
				GSPacketIn in2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, (itemInfo.ItemID == 0) ? 1 : itemInfo.ItemID, itemInfo.TemplateID, "");
				GameServer.Instance.LoginServer.SendPacket(in2);
			}
		}

		public void PVEFightNotice(string msg)
		{
			if (msg != null)
			{
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					allPlayers[i].Out.SendMessage(eMessageType.ChatNormal, msg);
				}
			}
		}

		public void PVERewardNotice(string msg, int itemID, int templateID)
		{
			if (msg != null)
			{
				GSPacketIn RewardNotice = WorldMgr.SendSysNotice(eMessageType.ChatNormal, msg, itemID, templateID, null);
				GameServer.Instance.LoginServer.SendPacket(RewardNotice);
			}
		}

		public void PVPFightMessage(string translation, ItemInfo itemInfo, int areaID)
		{
			if (translation != null)
			{
				GSPacketIn in2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, (itemInfo.ItemID == 0) ? 1 : itemInfo.ItemID, itemInfo.TemplateID, "");
				GameServer.Instance.LoginServer.SendPacket(in2);
			}
		}

		public double getHertAddition(double para1, double para2)
		{
			return Math.Round(para1 * Math.Pow(1.1, para2) - para1);
		}

		public PlayerInventory GetInventory(eBageType bageType)
		{
			switch (bageType)
			{
				case eBageType.EquipBag:
					return m_equipBag;
				case eBageType.PropBag:
					return m_propBag;
				case eBageType.FightBag:
					return m_fightBag;
				case eBageType.TempBag:
					return m_tempBag;
				case eBageType.CaddyBag:
					return m_caddyBag;
				case eBageType.FarmBag:
					return FarmBag;
				case eBageType.Vegetable:
					return Vegetable;
				case eBageType.Consortia:
					return m_ConsortiaBag;
				case eBageType.BankBag:
					return m_BankBag;
				case eBageType.Store:
					return m_storeBag;
				case eBageType.Food:
					return m_food;
				case eBageType.PetEgg:
					return m_petEgg;
				case eBageType.MagicStone:
					return null;
				default:
					throw new NotSupportedException($"Did not support this type bag: {bageType} PlayerID: {PlayerCharacter.ID} Nickname: {PlayerCharacter.NickName}");
			}
		}

		public string GetInventoryName(eBageType bageType)
		{
			switch (bageType)
			{
				case eBageType.EquipBag:
					return LanguageMgr.GetTranslation("Game.Server.GameObjects.Equip");
				case eBageType.PropBag:
					return LanguageMgr.GetTranslation("Game.Server.GameObjects.Prop");
				case eBageType.FightBag:
					return LanguageMgr.GetTranslation("Game.Server.GameObjects.FightBag");
				case eBageType.BeadBag:
					return LanguageMgr.GetTranslation("Game.Server.GameObjects.BeadBag");
				case eBageType.FarmBag:
					return LanguageMgr.GetTranslation("Game.Server.GameObjects.FarmBag");
				default:
					return bageType.ToString();
			}
		}

		public ItemInfo GetItemAt(eBageType bagType, int place)
		{
			return GetInventory(bagType)?.GetItemAt(place);
		}

		public ItemInfo GetItemByTemplateID(int templateID)
		{
			ItemInfo itemByTemplateID = GetInventory(eBageType.EquipBag).GetItemByTemplateID(31, templateID);
			if (itemByTemplateID == null)
			{
				itemByTemplateID = GetInventory(eBageType.PropBag).GetItemByTemplateID(0, templateID);
			}
			if (itemByTemplateID == null)
			{
				itemByTemplateID = GetInventory(eBageType.Consortia).GetItemByTemplateID(0, templateID);
			}
			if (itemByTemplateID == null)
			{
				itemByTemplateID = GetInventory(eBageType.BankBag).GetItemByTemplateID(0, templateID);
			}
			return itemByTemplateID;
		}

		public int GetItemCount(int templateId)
		{
			return m_propBag.GetItemCount(templateId) + m_equipBag.GetItemCount(templateId) + m_ConsortiaBag.GetItemCount(templateId) + m_BankBag.GetItemCount(templateId);
		}

		public PlayerInventory GetItemInventory(ItemTemplateInfo template)
		{
			return GetInventory(template.BagType);
		}

		public void HideEquip(int categoryID, bool hide)
		{
			if (categoryID >= 0 && categoryID < 10)
			{
				EquipShowImp(categoryID, (!hide) ? 1 : 2);
			}
		}

		public char[] InitPvePermission()
		{
			char[] chArray = new char[50];
			for (int i = 0; i < chArray.Length; i++)
			{
				chArray[i] = '1';
			}
			return chArray;
		}

		public bool IsBlackFriend(int playerID)
		{
			if (_friends != null)
			{
				if (_friends.ContainsKey(playerID))
				{
					return _friends[playerID] == 1;
				}
				return false;
			}
			return true;
		}

		public bool IsConsortia()
		{
			return ConsortiaMgr.FindConsortiaInfo(PlayerCharacter.ConsortiaID) != null;
		}

		public bool IsLimitCount(int count)
		{
			if (GameProperties.IsLimitCount && count > GameProperties.LimitCount)
			{
				SendMessage($"O limite de {GameProperties.LimitCount} foi alcançado.");
				return true;
			}
			return false;
		}

		public bool IsLimitMoney(int count)
		{
			if (GameProperties.IsLimitMoney && count > GameProperties.LimitMoney)
			{
				SendMessage($"O limite de {GameProperties.LimitMoney} foi alcançado de cupons.");
				return true;
			}
			return false;
		}

		public bool IsPveEpicPermission(int copyId)
		{
			string str = "1-2-3-4-5-6-7-8-9-10-11-12-13";
			if (str.Length > 0)
			{
				string[] array = str.Split('-');
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] == copyId.ToString())
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool UsePayBuff(BuffType type)
		{
			bool ketqua = false;
			AbstractBuffer ofType = BufferList.GetOfType(type);
			if (ofType != null && ofType.Check())
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(ofType.Info.TemplateID);
				if (itemTemplateInfo != null)
				{
					if (itemTemplateInfo.Property3 > 0 && ofType.Info.ValidCount > 0)
					{
						ofType.Info.ValidCount--;
						BufferList.UpdateBuffer(ofType);
						ketqua = true;
					}
					else if (itemTemplateInfo.Property3 == 0)
					{
						ketqua = true;
					}
				}
			}
			return ketqua;
		}

		public bool IsPvePermission(int copyId, eHardLevel hardLevel)
		{
			if (copyId <= m_pvepermissions.Length && copyId > 0)
			{
				return m_pvepermissions[copyId - 1] >= permissionChars[(int)hardLevel];
			}
			return true;
		}

		public void OnPropertiesChange()
		{
			if (this.PropertiesChange != null)
			{
				this.PropertiesChange(PlayerCharacter);
			}
		}

		public void LastVIPPackTime()
		{
			m_character.LastVIPPackTime = DateTime.Now;
			m_character.CanTakeVipReward = false;
		}

		public virtual bool LoadFromDatabase()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				PlayerInfo userSingleByUserID = bussiness.GetUserSingleByUserID(m_character.ID);
				if (userSingleByUserID == null)
				{
					Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid"));
					Client.Disconnect();
					return false;
				}
				if (userSingleByUserID == null || userSingleByUserID.MyInviteCode.Length < 10)
				{
					userSingleByUserID.MyInviteCode = CreateRandomInviteCode(9);
				}
				m_character = userSingleByUserID;
				m_battle.LoadFromDatabase();
				m_battle.UpdateLeagueGrade();
				m_character.Texp = bussiness.GetUserTexpInfoSingle(m_character.ID);
				if (m_character.Texp.IsValidadteTexp())
				{
					m_character.Texp.texpCount = 0;
				}
				int[] updatedSlots = new int[3]
				{
				0,
				1,
				2
				};
				if (userSingleByUserID.Grade >= 10)
				{
					LoadGemStone(bussiness);
				}
				HappyRechargeData = bussiness.GetHappyRechargeData(PlayerCharacter.ID) ?? new HappyRechargeInfo()
				{
					UserID = PlayerCharacter.ID,
					LotteryCount = 0,
					LotteryTicket = 0
				};
				m_equipGhostList = JsonConvert.DeserializeObject<Dictionary<string, UserEquipGhostInfo>>(PlayerCharacter.GhostEquipList) ?? new Dictionary<string, UserEquipGhostInfo>();

                userSingleByUserID.explorerManualInfo = bussiness.InitExplorerManualInfo(m_character.ID);
                userSingleByUserID.explorerManualInfo.activesPage = bussiness.InitExplorerManualInfoPages(m_character.ID);
                userSingleByUserID.explorerManualInfo.debris = bussiness.InitExplorerManualDebris(m_character.ID);

                Out.SendUpdateInventorySlot(FightBag, updatedSlots);
				UpdateWeaklessGuildProgress();
				UpdateItemForUser(1);
				ChecVipkExpireDay();
				UpdateLevel();
				Extra.UpdateEventCondition(1, PlayerCharacter.VIPLevel);
				ResetTotem(true);
				UpdatePet(m_petBag.GetPetIsEquip());
				if (m_character.NewDay < DateTime.Now.AddDays(-15))
				{
					ItemTemplateInfo temp = ItemMgr.FindItemTemplate(52000517);
					ItemInfo item = ItemInfo.CreateFromTemplate(temp, 1, 120);
					if (item != null)
					{
						item.IsBinds = true;
						SendItemToMail(item, string.Format("Bạn đã Offline quá 15 ngày nên chúng tôi có phần thưởng nho nhỏ dành cho bạn!!!"), string.Format("Quà Trở Về"), eMailType.Manage);
					}
				}
				if (m_character.LoginDevice == 1)
				{
					OnLoginDeviceAdd();
				}
                EventRewardProcessInfo eventProcess1 = this.Extra.GetEventProcess(4);
				EventRewardProcessInfo eventProcess2 = this.Extra.GetEventProcess(5);
				if (m_character.CheckNewDay())
				{
					if (DateTime.Today.DayOfWeek == DayOfWeek.Monday)
                    {
						//Reset GodCardRaise
						m_playerActive.Info.cardScore = 0;
						m_playerActive.Info.cardFreeCount = 0;
						m_playerActive.Info.cardChipCount = 0;
						m_playerActive.ListCards.Clear();
						m_playerActive.ListAwards.Clear();
						m_playerActive.ListExchanges.Clear();
                    }
					m_playerActive.ResetCryptBossData();
					GmActivityMgr.OnPlayerLogin(this, DateTime.Now);
					QuestInventory.Restart();
					QuestInventory.LoadFromDatabase(PlayerCharacter.ID);
					OnPlayerLogin();
					m_character.NewDay = DateTime.Now;
					m_battle.Reset();
					if (m_extra.Info.MinHotSpring < 5)
					{
						m_extra.Info.MinHotSpring = 30;
					}
					//if (m_character.typeVIP == 2)
					//	AddExpVip(15);
					//if (m_character.typeVIP == 1)
					//	AddExpVip(10);

					#region Vip OpenBoss
					if (m_character.VIPLevel == 5)
						m_extra.Info.BuyCountOpenBoss = 2;
					if (m_character.VIPLevel == 6)
						m_extra.Info.BuyCountOpenBoss = 3;
					if (m_character.VIPLevel >= 7)
						m_extra.Info.BuyCountOpenBoss = 4;
					#endregion
					m_extra.Info.FreeSendMailCount = 0;
					m_extra.Info.BuyTimeHotSpringCount = 0;
					m_extra.Info.LeftRoutteCount = 1;
					m_extra.Info.LeftRoutteRate = 0f;
					PlayerCharacter.MaxBuyHonor = 0;
					m_extra.Info.coupleNum = 0;
					m_extra.Info.propsNum = 0;
					PlayerCharacter.damageScores = 0;
					PlayerCharacter.Score = 0;

                    #region Daily
                    m_character.DailyGameCount = 0;
					m_character.DailyScore = 0;
					m_character.DailyWinCount = 0;
					#endregion

					#region Reset Novice
						if (eventProcess1.Conditions >= 100000 && eventProcess1.AwardGot >= 7)
							Extra.ResetNoviceEvent(NoviceActiveType.TIEUXU_DACBIET);
					if (eventProcess2.Conditions >= 500000 && eventProcess2.AwardGot >= 31)
						Extra.ResetNoviceEvent(NoviceActiveType.NAPXU_DACBIET);
                    #endregion

                    Extra.ResetNoviceEvent(NoviceActiveType.TIEUXU_TICHLUY);
					Extra.ResetNoviceEvent(NoviceActiveType.NAPXU_TICHLUY);
					BufferList.ResetAllPayBuffer();
					Farm.ResetFarmProp();
					this.AccumulativeUpdate();
					string content = $"{PlayerCharacter.NickName} - {DateTime.Now} \r\n - Restart Nhiệm vụ hàng ngày \r\n - Restart Tu luyện hàng ngày \r\n - Restart Nạp, Tiêu tích lũy. \r\n - Restart Lượt vòng quay may mắn. ";
					string title = "Đăng nhập ngày mới.";
					SendMailToUser(bussiness, content, title, eMailType.Manage);
				}
				m_pvepermissions = (string.IsNullOrEmpty(m_character.PvePermission) ? InitPvePermission() : m_character.PvePermission.ToCharArray());
				m_fightlabpermissions = (string.IsNullOrEmpty(m_character.FightLabPermission) ? InitFightLabPermission() : m_character.FightLabPermission.ToCharArray());
				LoadPvePermission();
				_friends = new Dictionary<int, int>();
				_friends = bussiness.GetFriendsIDAll(m_character.ID);
				ViFarms = new List<int>();
				m_character.State = 1;
				ClearStoreBag();
				ClearCaddyBag();
				PlayerCharacter.VIPNextLevelDaysNeeded = GetVIPNextLevelDaysNeeded(PlayerCharacter.VIPLevel, PlayerCharacter.VIPExp);
				LoadMedals();
				bussiness.UpdateUserTexpInfo(m_character.Texp);
				bussiness.UpdatePlayer(m_character);
				bussiness.UpdateUserMatchInfo(MatchInfo);
				SavePlayerInfo();
				return true;
			}
		}

		public int[] FightLabPermissionInt()
		{
			int[] pp = new int[50];
			for (int i = 0; i < 50; i++)
			{
				pp[i] = m_fightlabpermissions[i];
			}
			return pp;
		}

		public char[] InitFightLabPermission()
		{
			char[] tempByte = new char[50];
			for (int i = 0; i < 50; i++)
			{
				if (i == 0)
				{
					tempByte[i] = '1';
				}
				else
				{
					tempByte[i] = '0';
				}
			}
			return tempByte;
		}

		public bool SetFightLabPermission(int copyId, eHardLevel hardLevel, int missionId)
		{
			switch (copyId)
			{
				case 1000:
					copyId = 5;
					break;
				case 1001:
					copyId = 6;
					break;
				case 1002:
					copyId = 7;
					break;
				case 1003:
					copyId = 8;
					break;
				case 1004:
					copyId = 9;
					break;
			}
			if (copyId > m_fightlabpermissions.Length || copyId <= 0)
			{
				return true;
			}
			int ID = (copyId - 5) * 2;
			if (m_fightlabpermissions[ID] != fightlabpermissionChars[(int)(hardLevel + 1)])
			{
				return true;
			}
			if (m_fightlabpermissions[ID + 1] <= '2' && m_fightlabpermissions[ID] - m_fightlabpermissions[ID + 1] == 1)
			{
				m_fightlabpermissions[ID + 1] = m_fightlabpermissions[ID];
				string msg = "";
				int gold = 0;
				int money = 0;
				int giftToken = 0;
				int gp = 0;
				List<ItemInfo> infos = new List<ItemInfo>();
				if (DropInventory.FightLabUserDrop(missionId, ref infos) && infos != null)
				{
					bool confine = false;
					msg = LanguageMgr.GetTranslation("Phần thưởng từ phòng tập") + ": ";
					foreach (ItemInfo info in infos)
					{
						msg = msg + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", info.Template.Name, info.Count) + " ";
						if (infos.Count > 0 && PropBag.GetEmptyCount() < 1)
						{
							if (info.TemplateID != 11107 && info.TemplateID != -100 && info.TemplateID != -200 && info.TemplateID != -300)
							{
								string str = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content2");
								string str2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2");
								if (SendItemsToMail(new List<ItemInfo>
							{
								info
							}, str, str2, eMailType.ItemOverdue))
								{
									Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
								}
								confine = true;
							}
						}
						else if (!PropBag.StackItemToAnother(info) && info.TemplateID != 11107 && info.TemplateID != -100 && info.TemplateID != -200 && info.TemplateID != -300)
						{
							PropBag.AddItem(info);
						}
						ItemInfo.FindSpecialItemInfo(info, ref gold, ref money, ref giftToken, ref gp);
					}
					AddGold(gold);
					AddMoney(money);
					AddGiftToken(giftToken);
					AddGP(gp, false, false);
					if (confine)
					{
						msg += LanguageMgr.GetTranslation("Game.Server.GameUtils.Title2");
					}
					Out.SendMessage(eMessageType.GM_NOTICE, msg);
				}
			}
			if (copyId == 5 && hardLevel == eHardLevel.Normal)
			{
				if (m_fightlabpermissions[2] == '0')
				{
					m_fightlabpermissions[2] = '1';
				}
				if (m_fightlabpermissions[4] == '0')
				{
					m_fightlabpermissions[4] = '1';
				}
				if (m_fightlabpermissions[6] == '0')
				{
					m_fightlabpermissions[6] = '1';
				}
			}
			if ((copyId == 7 || copyId == 8) && hardLevel == eHardLevel.Hard && m_fightlabpermissions[8] == '0')
			{
				m_fightlabpermissions[8] = '1';
			}
			if (hardLevel < eHardLevel.Hard && m_fightlabpermissions[ID] < fightlabpermissionChars[(int)(hardLevel + 2)])
			{
				m_fightlabpermissions[ID] = fightlabpermissionChars[(int)(hardLevel + 2)];
			}
			m_character.FightLabPermission = new string(m_fightlabpermissions).ToString();
			OnPropertiesChanged();
			return true;
		}

		public bool IsFightLabPermission(int copyId, eHardLevel hardLevel)
		{
			if (copyId > m_fightlabpermissions.Length || copyId <= 0)
			{
				return true;
			}
			int ID = (copyId - 5) * 2;
			return m_fightlabpermissions[ID] >= fightlabpermissionChars[(int)(hardLevel + 1)];
		}

		public eHardLevel GetMaxFightLabPermission(int copyId)
		{
			if (copyId > m_fightlabpermissions.Length)
			{
				return eHardLevel.Simple;
			}
			switch (m_fightlabpermissions[copyId - 5])
			{
				case '3':
					return eHardLevel.Hard;
				case '2':
					return eHardLevel.Normal;
				default:
					return eHardLevel.Simple;
			}
		}

		public void LoadMedals()
		{
			m_character.medal = GetMedalNum();
			Client.Player.SaveIntoDatabase();
		}

		public void LoadMarryMessage()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				MarryApplyInfo[] playerMarryApply = bussiness.GetPlayerMarryApply(PlayerCharacter.ID);
				if (playerMarryApply == null)
				{
					return;
				}
				MarryApplyInfo[] array = playerMarryApply;
				MarryApplyInfo[] array2 = array;
				foreach (MarryApplyInfo info in array2)
				{
					switch (info.ApplyType)
					{
						case 1:
							Out.SendPlayerMarryApply(this, info.ApplyUserID, info.ApplyUserName, info.LoveProclamation, info.ID);
							break;
						case 2:
							Out.SendMarryApplyReply(this, info.ApplyUserID, info.ApplyUserName, info.ApplyResult, isApplicant: true, info.ID);
							if (!info.ApplyResult)
							{
								Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
							}
							break;
						case 3:
							Out.SendPlayerDivorceApply(this, result: true, isProposer: false);
							break;
					}
				}
			}
		}

		public void LoadMarryProp()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				MarryProp marryProp = bussiness.GetMarryProp(PlayerCharacter.ID);
				PlayerCharacter.IsMarried = marryProp.IsMarried;
				PlayerCharacter.SpouseID = marryProp.SpouseID;
				PlayerCharacter.SpouseName = marryProp.SpouseName;
				PlayerCharacter.IsCreatedMarryRoom = marryProp.IsCreatedMarryRoom;
				PlayerCharacter.SelfMarryRoomID = marryProp.SelfMarryRoomID;
				PlayerCharacter.IsGotRing = marryProp.IsGotRing;
				Out.SendMarryProp(this, marryProp);
			}
		}

		public void LoadPvePermission()
		{
			PveInfo[] pveInfo = PveInfoMgr.GetPveInfo();
			PveInfo[] array = pveInfo;
			foreach (PveInfo info in array)
			{
				if (m_character.Grade > info.LevelLimits)
				{
					bool flag = SetPvePermission(info.ID, eHardLevel.Easy);
					if (flag)
					{
						flag = SetPvePermission(info.ID, eHardLevel.Normal);
					}
					if (flag)
					{
						flag = SetPvePermission(info.ID, eHardLevel.Hard);
					}
				}
			}
		}

		public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
		{
		}

		public bool Login()
		{
			if (WorldMgr.AddPlayer(m_character.ID, this))
			{
				try
				{
					if (LoadFromDatabase())
					{
						if (PlayerCharacter.BoxGetDate.ToShortDateString() != DateTime.Now.ToShortDateString())
						{
							PlayerCharacter.AlreadyGetBox = 0;
							PlayerCharacter.BoxProgression = 0;
						}
						//add exp
						if (PlayerCharacter.GP < 25899)
						{
							AddGP(25899 - PlayerCharacter.GP);
						}
						FirstLoginCheckCode = true;
						Out.SendLoginSuccess();
						Out.SendUpdatePublicPlayer(PlayerCharacter, MatchInfo, Extra.Info);
						Out.SendNecklaceStrength(PlayerCharacter);
						Out.SendWeaklessGuildProgress(PlayerCharacter);
						ProcessConsortiaAndPet();
						Out.SendDateTime();
						SendPkgLimitGrade();
						Out.SendDailyAward(this);
						LoadMarryMessage();
						if (!m_showPP)
						{
							m_playerProp.ViewCurrent();
							m_showPP = true;
						}
						Rank.SendUserRanks();
						Farm.LoadFarmLand();
						Out.SendOpenVIP(this);
						SendUpdatePlayerFigSpirit();
						EquipBag.UpdatePlayerProperties();
						PetBag.UpdateEatPets();
						SetupProcessor();
						Actives.SendEvent();
						Out.SendUpdateChickActivation(this.Actives.GetChickActiveData());
						AvatarBag.UpdateInfo();
						Out.SendOpenHappyRecharge(this.PlayerCharacter.ID);
						Out.SendEnthrallLight();
						Out.SendEdictumVersion();
						this.Out.SendInviteFriends(this.PlayerCharacter, 5);
						m_playerState = ePlayerState.Manual;
						Out.SendBufferList(this, m_bufferList.GetAllBufferByTemplate());
						Out.SendUpdateAchievementData(AchievementInventory.GetSuccessAchievement());
						BoxBeginTime = DateTime.Now;
						OpenAllNoviceActive();
						Extra.BeginOnlineGameTimer();
                        DateTime now = DateTime.Now;
						if (PlayerCharacter.DateFinishTask.Date < now.Date)
						{
							ConsortiaTaskMgr.AddPlayer(this);
						}
						Out.SendUpdateFirstRecharge(PlayerCharacter.IsRecharged, PlayerCharacter.IsGetAward);
						ChargeToUser();
						Out.SendUserSyncEquipGhost(this);
						bool isOpen = this.IsOpenSanXiao();
						sendMiniGame(isOpen);
                        SignInActivity(2);
                        Out.SendLittleGameActived();
						if (DateTime.Parse(GameProperties.LeftRouterEndDate) > DateTime.Now)
						{
							Out.SendLeftRouleteOpen(Extra.Info);
						}
						if(WorldMgr.IsLeagueOpen)
                        {
							Out.SendLeagueNotice(m_character.ID);
                        }
						#region
						if (PlayerCharacter.GhostEquipList == "{}")
						{
							//Weapon
							List<SpiritInfo> spiList = SpiritInfoMgr.GetSpirit(7);
							UserEquipGhostInfo used = new UserEquipGhostInfo();
							used.UserID = PlayerCharacter.ID;
							used.BagType = spiList[0].BagType;
							used.Place = spiList[0].BagPlace;
							used.Level = 0;
							used.TotalGhost = 0;
							//Cloth
							List<SpiritInfo> spiList1 = SpiritInfoMgr.GetSpirit(5);
							UserEquipGhostInfo used1 = new UserEquipGhostInfo();
							used1.UserID = PlayerCharacter.ID;
							used1.BagType = spiList1[0].BagType;
							used1.Place = spiList1[0].BagPlace;
							used1.Level = 0;
							used1.TotalGhost = 0;
							//Head
							List<SpiritInfo> spiList2 = SpiritInfoMgr.GetSpirit(1);
							UserEquipGhostInfo used2 = new UserEquipGhostInfo();
							used2.UserID = PlayerCharacter.ID;
							used2.BagType = spiList2[0].BagType;
							used2.Place = spiList2[0].BagPlace;
							used2.Level = 0;
							used2.TotalGhost = 0;
							AddEquipGhost(used);
							AddEquipGhost(used1);
							AddEquipGhost(used2);
						}
						#endregion
						WorldMgr.Test();
						if (PlayerCharacter.Repute > 0 && PlayerCharacter.Repute <= 10)
						{
							string Ranked = PlayerCharacter.Honor;
							if (Ranked == null | Ranked.Length < 1)
								Ranked = "Người Chơi";
							string NoticeOnline = string.Format("|{0}| - {1} [{2}] lực chiến {3} hạng {4} đã online!!!", ZoneName, Ranked, PlayerCharacter.NickName, PlayerCharacter.FightPower, PlayerCharacter.Repute);
							GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendOnlineNotice(NoticeOnline));

						}
						#region OLD
						/*if (PlayerCharacter.UserName == "hungbilly" || PlayerCharacter.UserName == "hungbilly")
						{
							GamePlayer[] allplayer = WorldMgr.GetAllPlayers();
							foreach (GamePlayer gamePlayer in allplayer)
							{
								GSPacketIn in23 = new GSPacketIn(10);
								in23.WriteInt(2);
								in23.WriteString("|ADMINISTRATOR| - [" + PlayerCharacter.NickName + "] - ĐÃ ONLINE NẾU CÓ VẤN ĐỀ LIÊN HỆ NGAY ĐỂ ĐƯỢC HỖ TRỢ !");
								gamePlayer.SendTCP(in23);
							}
						}
						else
						{
							string name = PlayerCharacter.Honor;
							if (name == null || name.Length < 1)
							{
								name = "Tân Binh Gunny";
							}
							WorldMgr.SendSysNotice($"|{ZoneName}| - {name} [{PlayerCharacter.NickName}] đã online!");
						}*/
						#endregion

						GmActivityMgr.OnPlayerUpgradeVIP(this, m_character.VIPLevel);

                        //open full funcitons
                        PlayerCharacter.openFunction(Step.PICK_TWO_TWENTY);
                        PlayerCharacter.openFunction(Step.POP_WIN);
                        PlayerCharacter.openFunction(Step.FIFTY_OPEN);
                        PlayerCharacter.openFunction(Step.FORTY_OPEN);
                        PlayerCharacter.openFunction(Step.THIRTY_OPEN);
                        PlayerCharacter.openFunction(Step.POP_TWO_TWENTY);
                        PlayerCharacter.openFunction(Step.GAIN_TEN_PERSENT);
                        PlayerCharacter.openFunction(Step.PICK_ONE);
                        PlayerCharacter.openFunction(Step.PLANE_OPEN);
                        return true;
					}
					WorldMgr.RemovePlayer(m_character.ID);
				}
				catch (Exception exception)
				{
					log.Error("Error Login!", exception);
				}
				return false;
			}
			return false;
		}

		private void ProcessConsortiaAndPet()
		{
			consortiaProcessor_0 = new ConsortiaProcessor(m_consortiaProcessor);
			petProcessor_0 = new PetProcessor(m_petProcessor);
		}

		public bool MoneyDirect(int value, bool IsAntiMult)
		{
			return MoneyDirect(MoneyType.Money, value, IsAntiMult);
		}

		public bool CheckOpenBoss()//(eHardLevel selectLevel)
        {
			return m_extra.Info.BuyCountOpenBoss > 0;
        }

		public bool MoneyDirect(MoneyType type, int value, bool IsAntiMult)
		{
			if (value >= 0 && value <= int.MaxValue)
			{
				if (type == MoneyType.Money)
				{
					if (PlayerCharacter.Money + PlayerCharacter.MoneyLock >= value)
					{
						RemoveMoney1(value, IsAntiMult);
						UpdateProperties();
						return true;
					}
					SendInsufficientMoney(0);
				}
				else
				{
					if (PlayerCharacter.GiftToken >= value)
					{
						RemoveGiftToken(value);
						UpdateProperties();
						return true;
					}
					SendMessage("Không đủ xu khóa.");
				}
			}
			return false;
		}

		public void OnAchievementFinish(AchievementData info)
		{
			if (this.AchievementFinishEvent != null)
			{
				this.AchievementFinishEvent(info);
			}
		}

		public void OnAdoptPetEvent()
		{
			if (this.AdoptPetEvent != null)
			{
				this.AdoptPetEvent();
			}
		}

		public void OnCropPrimaryEvent()
		{
			if (this.CropPrimaryEvent != null)
			{
				this.CropPrimaryEvent();
			}
		}

		public void OnEnterHotSpring()
		{
			if (this.EnterHotSpringEvent != null)
			{
				this.EnterHotSpringEvent(this);
			}
		}

		public void OnFightAddOffer(int offer)
		{
			if (this.FightAddOfferEvent != null)
			{
				this.FightAddOfferEvent(offer);
			}
		}

		public void OnGuildChanged()
		{
			if (this.GuildChanged != null)
			{
				this.GuildChanged();
			}
		}

		public void OnHotSpingExpAdd(int minutes, int exp)
		{
			if (this.HotSpingExpAdd != null)
			{
				this.HotSpingExpAdd(minutes, exp);
			}
		}

		public void OnOnlineGameAdd()
		{
			if (this.OnlineGameAdd != null)
			{
				this.OnlineGameAdd();
			}
		}

		public void OnLoginDeviceAdd()
		{
			if (this.LoginDeviceAdd != null)
			{
				this.LoginDeviceAdd();
			}
		}

		public void OnItemCompose(int composeType)
		{
			if (this.ItemCompose != null)
			{
				this.ItemCompose(composeType);
			}
		}

		public void OnItemFusion(int fusionType)
		{
			if (this.ItemFusion != null)
			{
				this.ItemFusion(fusionType);
			}
		}

		public void OnItemInsert()
		{
			if (this.ItemInsert != null)
			{
				this.ItemInsert();
			}
		}

		public void OnItemMelt(int categoryID)
		{
			if (this.ItemMelt != null)
			{
				this.ItemMelt(categoryID);
			}
		}

		public void OnItemStrengthen(int categoryID, int level)
		{
			if (this.ItemStrengthen != null)
			{
				this.ItemStrengthen(categoryID, level);
			}
		}

		public void OnMoneyCharge(int money)
		{
			if (this.MoneyCharge != null)
			{
				this.MoneyCharge(money);
			}
		}

		public void OnAchievementQuest()
		{
			if (this.AchievementQuest != null)
			{
				this.AchievementQuest();
			}
		}

		public void OnKillingBoss(AbstractGame game, NpcInfo npc, int damage)
		{
			if (this.AfterKillingBoss != null)
			{
				this.AfterKillingBoss(game, npc, damage);
			}
		}

		public void OnDamageBoss(AbstractGame game, NpcInfo npc, int damage)
		{
			if (this.AfterDamageBoss != null)
			{
				this.AfterDamageBoss(game, npc, damage);
			}
		}

		public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage)
		{
			if (this.AfterKillingLiving != null)
			{
				this.AfterKillingLiving(game, type, id, isLiving, damage, isSpanArea: false);
			}
			if (!(this.GameKillDrop == null || isLiving))
			{
				this.GameKillDrop(game, type, id, isLiving);
			}
		}

		public void OnLevelUp(int grade)
		{
			if (this.LevelUp != null)
			{
				this.LevelUp(this);
			}
		}

		public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum)
		{
			if (this.MissionOver != null)
			{
				this.MissionOver(game, missionId, isWin);
			}
			if (this.MissionTurnOver != null && isWin)
			{
				this.MissionTurnOver(game, missionId, turnNum);
			}
			if (this.MissionFullOver != null)
			{
				this.MissionFullOver(game, missionId, isWin, turnNum);
			}
			//top pho ban
			if (isWin)
			{
				AddLog("WinMission", missionId.ToString());
			}
		}

		public void OnNewGearEvent(ItemInfo item)
		{
			if (this.NewGearEvent != null)
			{
				this.NewGearEvent(item);
			}
		}

		public void OnSeedFoodPetEvent()
		{
			if (this.SeedFoodPetEvent != null)
			{
				this.SeedFoodPetEvent();
			}
		}

		public void OnPaid(int money, int gold, int offer, int gifttoken, int petScore, int medal, string payGoods)
		{
			if (this.Paid != null)
			{
				this.Paid(money, gold, offer, gifttoken, petScore, medal, payGoods);
			}
		}

		protected void OnPropertiesChanged()
		{
			UpdateProperties();
			OnPlayerPropertyChanged(m_character);
		}

		public void OnUnknowQuestConditionEvent()
		{
			if (this.UnknowQuestConditionEvent != null)
			{
				this.UnknowQuestConditionEvent();
			}
		}

		public void OnUpLevelPetEvent()
		{
			if (this.UpLevelPetEvent != null)
			{
				this.UpLevelPetEvent();
			}
		}

		public void OnUpLevelVipEvent()
		{
			if (this.Event_0 != null)
			{
				this.Event_0(m_character.VIPLevel, 0);
			}
		}

		public void OnUseBuffer()
		{
			if (this.UseBuffer != null)
			{
				this.UseBuffer(this);
			}
		}

		public void OnUserToemGemstoneEvent()
		{
			if (this.UserToemGemstonetEvent != null)
			{
				this.UserToemGemstonetEvent();
			}
		}

		public void OnUsingItem(int templateID, int count)
		{
			if (this.AfterUsingItem != null)
			{
				this.AfterUsingItem(templateID, count);
			}
		}

		public void OpenVIP(int days)
		{
			DateTime VipExpireDay = DateTime.Now.AddDays(days);
			m_character.typeVIP = SetTypeVIP(days);
			m_character.VIPLevel = 1;
			m_character.VIPExp = 0;
			m_character.VIPExpireDay = VipExpireDay;
			m_character.VIPLastDate = DateTime.Now;
			m_character.VIPNextLevelDaysNeeded = 0;
			m_character.CanTakeVipReward = true;
			this.OnUpLevelVipEvent();
		}

		public void OpenVIP(int days, DateTime ExpireDayOut)
		{
			this.m_character.typeVIP = 1;//this.SetTypeVIP(1);
			this.m_character.VIPExpireDay = ExpireDayOut;
			this.m_character.VIPLastDate = DateTime.Now;
			this.m_character.VIPNextLevelDaysNeeded = 10;
			this.m_character.CanTakeVipReward = false;
			this.OnUpLevelVipEvent();
		}

		public void ContinuousVIP(int days, DateTime ExpireDayOut)
		{
			int vipLevel = this.m_character.VIPLevel;
			if (vipLevel < 6 && days == 180)
			{
				this.m_character.VIPLevel = 6;
				this.m_character.VIPExp = 4100;
				this.m_character.VIPExpireDay = ExpireDayOut;
				this.m_character.typeVIP = 1;// this.SetTypeVIP(days);
			}
			else if (vipLevel < 4 && days == 90)
			{
				this.m_character.VIPLevel = 4;
				this.m_character.VIPExp = 1400;
				this.m_character.VIPExpireDay = ExpireDayOut;
				this.m_character.typeVIP = 1;//this.SetTypeVIP(days);
			}
			else
			{
				m_character.VIPExpireDay = ExpireDayOut;
				this.m_character.typeVIP = 1;// this.SetTypeVIP(days);
			}
			this.OnUpLevelVipEvent();
		}

		public byte SetTypeVIP(int days)
		{
			byte type = 1;
			if (m_character.typeVIP == 2)
			{
				type = 2;
			}						
			else
			{
				if (days / 31 >= 3)
				{
					type = 2;
				}
			}
			return type;
		}

		public void ResetLottery()
		{
			Lottery = -1;
			LotteryID = 0;
			LotteryItems = new List<ItemBoxInfo>();
			LotteryAwardList = new List<ItemInfo>();
		}

		public virtual bool Quit()
		{
			try
			{
				try
				{
					if (Level == 1)
					{
						ItemInfo weapon = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(7008), 1, 105);
						weapon.ValidDate = 365;
						EquipBag.AddItemTo(weapon, 6);
					}
					if (CurrentRoom != null)
					{
						CurrentRoom.RemovePlayerUnsafe(this);
						if (PlayerCharacter.Repute < 51)
						{
							RoomMgr.WaitingRoom.AddPlayer(this);
						}
						CurrentRoom = null;
					}
					else
					{
						if (PlayerCharacter.Repute > 50)
						{
							RoomMgr.WaitingRoom.RemovePlayer(this);
						}
					}
					if (CurrentMarryRoom != null)
					{
						CurrentMarryRoom.RemovePlayer(this);
						CurrentMarryRoom = null;
					}
					if (CurrentHotSpringRoom != null)
					{
						CurrentHotSpringRoom.RemovePlayer(this);
						CurrentHotSpringRoom = null;
					}
					if (LotteryAwardList.Count > 0 && Lottery != -1)
					{
						SendItemsToMail(LotteryAwardList, "", LanguageMgr.GetTranslation("Game.Server.Lottery.Oversea.MailTitle"), eMailType.BuyItem);
						ResetLottery();
					}
                    #region

                    var ListItem = CaddyBag.GetItems();
					if (ListItem.Count > 0 && Lottery == -1)
					{
                        SendItemsToMail(ListItem, "", "Lu Đồng Trả Về", eMailType.BuyItem);
                    }	

                    #endregion
                    if (LittleGameInfo.ID != 0)
					{
						LittleGameWorldMgr.RemovePlayer(this);
					}
					Extra.StopAllTimer();
					ConsortiaTaskMgr.RemovePlayer(this);
					RoomMgr.WorldBossRoom.RemovePlayer(this);
					RoomMgr.ChristmasRoom.SetMonterDie(PlayerCharacter.ID);
					RoomMgr.ChristmasRoom.RemovePlayer(this);
					Actives.StopChristmasTimer();
				}
				catch (Exception exception)
				{
					log.Error("Player exit Game Error!", exception);
				}
				m_character.State = 0;
				SaveIntoDatabase();
			}
			catch (Exception exception2)
			{
				log.Error("Player exit Error!!!", exception2);
			}
			finally
			{
				WorldMgr.RemovePlayer(m_character.ID);
			}
			return true;
		}

		public bool RemoveAt(eBageType bagType, int place)
		{
			return GetInventory(bagType)?.RemoveItemAt(place) ?? false;
		}

		public bool RemoveCountFromStack(ItemInfo item, int count)
		{
			if (item.BagType == m_propBag.BagType)
			{
				return m_propBag.RemoveCountFromStack(item, count);
			}
			if (item.BagType == m_ConsortiaBag.BagType)
			{
				return m_ConsortiaBag.RemoveCountFromStack(item, count);
			}
			if (item.BagType == m_BankBag.BagType)
			{
				return m_BankBag.RemoveCountFromStack(item, count);
			}
			return m_equipBag.RemoveCountFromStack(item, count);
		}

		public int RemoveGold(int value)
		{
			if (value > 0 && value <= m_character.Gold)
			{
				m_character.Gold -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public int RemoveGP(int gp)
		{
			if (gp > 0)
			{
				m_character.GP -= gp;
				if (m_character.GP < 1)
				{
					m_character.GP = 1;
				}
				int level = LevelMgr.GetLevel(m_character.GP);
				if (Level > level)
				{
					m_character.GP += gp;
				}
				UpdateProperties();
				UpdateLevel();
				return gp;
			}
			return 0;
		}

		public int RemoveGiftToken(int value)
		{
			if (value > 0 && value <= m_character.GiftToken)
			{
				m_character.GiftToken -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public bool RemoveHealstone()
		{
			ItemInfo itemAt = m_equipBag.GetItemAt(18);
			if (itemAt != null && itemAt.Count > 0)
			{
				return m_equipBag.RemoveCountFromStack(itemAt, 1);
			}
			return false;
		}

		public bool RemoveItem(ItemInfo item)
		{
			if (item.BagType == FarmBag.BagType)
			{
				return FarmBag.RemoveItem(item);
			}
			if (item.BagType == m_propBag.BagType)
			{
				return m_propBag.RemoveItem(item);
			}
			if (item.BagType == m_fightBag.BagType)
			{
				return m_fightBag.RemoveItem(item);
			}
			return m_equipBag.RemoveItem(item);
		}

		public int AddMedal(int value)
		{
			if (value > 0)
			{
				ItemInfo myMedals = GetInventory(eBageType.PropBag).GetItemByTemplateID(1, 11408);
				if (myMedals != null)
				{
					PropBag.AddCountToStack(myMedals, value);
					PropBag.UpdateItem(myMedals);
				}
				else
				{
					PropBag.AddTemplate(ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11408), value, 104), value);
				}
				m_character.medal = GetMedalNum();
				OnPropertiesChanged();
				UpdateProperties();
				UpdateChangedPlaces();
				return value;
			}
			return 0;
		}

		public int RemoveMedal(int value)
		{
			if (value > 0 && value <= m_character.medal)
			{
				RemoveTemplate(11408, value);
				m_character.medal = GetMedalNum();
				OnPropertiesChanged();
				UpdateProperties();
				UpdateChangedPlaces();
				return value;
			}
			return 0;
		}

		public int RemoveMoney(int value, bool isConsume = false)//isConsume tieu xu, fix ddi maay
		{
			if (value > 0 && value <= m_character.MoneyLock)
			{
				m_character.MoneyLock -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			if (value > 0 && value <= m_character.Money)
			{
				m_character.Money -= value;
				if (isConsume)
				{
					GmActivityMgr.OnConsumeMoney(this, value);
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TIEUXU_TICHLUY))
					Extra.UpdateEventCondition(2, value, true, 0);
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TIEUXU_DACBIET))
					Extra.UpdateEventCondition(4, value, true, 0);
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public int RemoveMoney1(int value, bool IsAntiMult)
		{
			if (value > 0 && value <= m_character.MoneyLock && !IsAntiMult)
			{
				m_character.MoneyLock -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			if (value > 0 && value <= m_character.Money)
			{
				m_character.Money -= value;
				if (!IsAntiMult)
				{
					GmActivityMgr.OnConsumeMoney(this, value);
				}
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TIEUXU_TICHLUY) && !IsAntiMult)
					Extra.UpdateEventCondition(2, value, true, 0);
				if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TIEUXU_DACBIET) && !IsAntiMult)
					Extra.UpdateEventCondition(4, value, true, 0);
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public int RemoveOffer(int value)
		{
			if (value > 0)
			{
				if (value >= m_character.Offer)
				{
					value = m_character.Offer;
				}
				m_character.Offer -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public int RemoveRichesOffer(int value)
		{
			if (value > 0)
			{
				if (value >= m_character.RichesOffer)
				{
					value = m_character.RichesOffer;
				}
				m_character.RichesOffer -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public int RemoveConsortiaRiches(int value)
		{
			if (value > 0)
			{
				if (value >= m_character.ConsortiaRiches)
				{
					value = m_character.ConsortiaRiches;
				}
				m_character.ConsortiaRiches -= value;
				OnPropertiesChanged();
				UpdateProperties();
				OnGuildChanged();
				return value;
			}
			return 0;
		}

		public int RemovePetScore(int value)
		{
			if (value > 0 && value <= m_character.petScore)
			{
				m_character.petScore -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public int RemoveScore(int value)
		{
			if (value > 0 && value <= m_character.Score)
			{
				m_character.Score -= value;
				OnPropertiesChanged();
				UpdateProperties();
				return value;
			}
			return 0;
		}

		public bool RemoveTempate(eBageType bagType, ItemTemplateInfo template, int count)
		{
			return GetInventory(bagType)?.RemoveTemplate(template.TemplateID, count) ?? false;
		}

		public bool RemoveTemplate(ItemTemplateInfo template, int count)
		{
			return GetItemInventory(template)?.RemoveTemplate(template.TemplateID, count) ?? false;
		}

		public bool NewRemoveTemplate(int templateId, int count)
        {
			var mainItem = EquipBag.GetItemCount(0, templateId);
			var propItem = PropBag.GetItemCount(templateId);
			var tempCount = mainItem + propItem;
			var template = ItemMgr.FindItemTemplate(templateId);
			if (template != null && tempCount >= count)
            {
				if ((mainItem > 0) && (count > 0) &&
					NewRemoveTemplate(eBageType.EquipBag, template, mainItem > count ? count : mainItem))
				{
					count = count < mainItem ? 0 : count - mainItem;
				}

				if ((propItem > 0) && (count > 0) &&
					NewRemoveTemplate(eBageType.PropBag, template, propItem > count ? count : propItem))
				{
					count = count < propItem ? 0 : count - propItem;
				}

				if (count == 0)
					return true;

				if (log.IsErrorEnabled)
					log.Error(string.Format("Item Remove Error, PlayerId {0} Remove TemplateId {1} Is Not Zero!",
						PlayerId, templateId));
			}

			return false;
		}

		public bool NewRemoveTemplate(eBageType bagType, ItemTemplateInfo template, int count)
        {
			var bag = GetInventory(bagType);
			if (bag != null)
			{
				return bag.RemoveTemplate(template.TemplateID, count);
			}

			return false;
		}

		public bool RemoveTemplate(int templateId, int count)
		{
			var equipItem = EquipBag.GetItemCount(0, templateId);
			var propItem = PropBag.GetItemCount(templateId);
			var consortiaItem = ConsortiaBag.GetItemCount(templateId);
			var bankItem = BankBag.GetItemCount(templateId);
			var tempCount = equipItem + propItem + consortiaItem + bankItem;
			var template = ItemMgr.FindItemTemplate(templateId);
			if(templateId == 11408 && count <= propItem + consortiaItem + bankItem)
            {
				m_character.medal -= count;
				UpdateProperties();
			}
			if(template != null && tempCount >= count)
            {
				if ((equipItem > 0) && (count > 0) && RemoveTempate(eBageType.EquipBag, template, equipItem > count ? count : equipItem))
                {
					count = count < equipItem ? 0 : count - equipItem;
                }
				if ((propItem > 0) && (count > 0) && RemoveTempate(eBageType.PropBag, template, propItem > count ? count : propItem))
				{
					count = count < propItem ? 0 : count - propItem;
				}
				if ((consortiaItem > 0) && (count > 0) && RemoveTempate(eBageType.Consortia, template, consortiaItem > count ? count : consortiaItem))
				{
					count = count < consortiaItem ? 0 : count - consortiaItem;
				}
				if ((bankItem > 0) && (count > 0) && RemoveTempate(eBageType.BankBag, template, bankItem > count ? count : bankItem))
				{
					count = count < bankItem ? 0 : count - bankItem;
				}
				if (count == 0)
					return true;
				if (log.IsErrorEnabled)
					log.Error(string.Format("Item Remove Error, PlayerId {0} Remove TemplateId {1} Is Not Zero!",
						PlayerId, templateId));
			}
			return false;
            #region OLD
            /*int itemCount = m_equipBag.GetItemCount(templateId);
			int num2 = m_propBag.GetItemCount(templateId);
			int num3 = m_ConsortiaBag.GetItemCount(templateId);
			int num4 = m_BankBag.GetItemCount(templateId);
			int num5 = itemCount + num2 + num3 + num4;
			ItemTemplateInfo template = ItemMgr.FindItemTemplate(templateId);
			if (templateId == 11408 && count <= num2 + num3 + num4)
			{
				m_character.medal -= count;
				UpdateProperties();
			}
			if (template != null && num5 >= count)
			{
				if (itemCount > 0 && count > 0 && RemoveTempate(eBageType.EquipBag, template, (itemCount > count) ? count : itemCount))
				{
					count = ((count >= itemCount) ? (count - itemCount) : 0);
				}
				if (num2 > 0 && count > 0 && RemoveTempate(eBageType.PropBag, template, (num2 > count) ? count : num2))
				{
					count = ((count >= num2) ? (count - num2) : 0);
				}
				if (num3 > 0 && count > 0 && RemoveTempate(eBageType.Consortia, template, (num3 > count) ? count : num3))
				{
					count = ((count >= num3) ? (count - num3) : 0);
				}
				if (num4 > 0 && count > 0 && RemoveTempate(eBageType.BankBag, template, (num4 > count) ? count : num4))
				{
					count = ((count >= num4) ? (count - num4) : 0);
				}
				if (count == 0)
				{
					return true;
				}
				if (log.IsErrorEnabled)
				{
					log.Error($"Item Remover Error：PlayerId {m_playerId} Remover TemplateId{templateId} Is Not Zero!");
				}
			}
			return false;*/
            #endregion
        }

        public UserLabyrinthInfo LoadLabyrinth(int sType)
		{
			if (userLabyrinthInfo == null)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					userLabyrinthInfo = playerBussiness.GetSingleLabyrinth(PlayerCharacter.ID);
					if (userLabyrinthInfo == null)
					{
						userLabyrinthInfo = new UserLabyrinthInfo();
						userLabyrinthInfo.UserID = PlayerCharacter.ID;
						userLabyrinthInfo.sType = sType;
						userLabyrinthInfo.myProgress = 0;
						userLabyrinthInfo.myRanking = 0;
						userLabyrinthInfo.completeChallenge = true;
						userLabyrinthInfo.isDoubleAward = false;
						userLabyrinthInfo.currentFloor = 1;
						userLabyrinthInfo.accumulateExp = 0;
						userLabyrinthInfo.remainTime = 0;
						userLabyrinthInfo.currentRemainTime = 0;
						userLabyrinthInfo.cleanOutAllTime = 0;
						userLabyrinthInfo.cleanOutGold = 50;
						userLabyrinthInfo.tryAgainComplete = true;
						userLabyrinthInfo.isInGame = false;
						userLabyrinthInfo.isCleanOut = false;
						userLabyrinthInfo.serverMultiplyingPower = false;
						userLabyrinthInfo.LastDate = DateTime.Now;
						userLabyrinthInfo.ProcessAward = InitProcessAward();
						playerBussiness.AddUserLabyrinth(userLabyrinthInfo);
					}
					else
					{
						ProcessLabyrinthAward = userLabyrinthInfo.ProcessAward;
						userLabyrinthInfo.sType = sType;
					}
				}
			}
			return Labyrinth;
		}

		public string InitProcessAward()
		{
			string[] strArray = new string[99];
			for (int index = 0; index < strArray.Length; index++)
			{
				strArray[index] = index.ToString();
			}
			ProcessLabyrinthAward = string.Join("-", strArray);
			return ProcessLabyrinthAward;
		}

		public string CompleteGetAward(int floor)
		{
			string[] strArray1 = new string[floor];
			for (int index2 = 0; index2 < floor; index2++)
			{
				strArray1[index2] = "i";
			}
			string[] strArray2 = userLabyrinthInfo.ProcessAward.Split('-');
			string str = string.Join("-", strArray1);
			for (int index = floor; index < strArray2.Length; index++)
			{
				str = str + "-" + strArray2[index];
			}
			return str;
		}

		public bool isDoubleAward()
		{
			if (userLabyrinthInfo == null)
			{
				return false;
			}
			return userLabyrinthInfo.isDoubleAward;
		}

		public void OutLabyrinth(bool isWin)
		{
			if (!isWin && userLabyrinthInfo != null && userLabyrinthInfo.currentFloor > 1)
			{
				SendLabyrinthTryAgain();
			}
			ResetLabyrinth();
		}

		public void SendLabyrinthTryAgain()
		{
			GSPacketIn pkg = new GSPacketIn(131, PlayerId);
			pkg.WriteByte(9);
			pkg.WriteInt(LabyrinthTryAgainMoney());
			SendTCP(pkg);
		}

		public int LabyrinthTryAgainMoney()
		{
			for (int num = 0; num < Labyrinth.myProgress; num += 2)
			{
				if (Labyrinth.currentFloor == num)
				{
					return GameProperties.WarriorFamRaidPriceBig;
				}
			}
			return GameProperties.WarriorFamRaidPriceSmall;
		}

		public void ResetLabyrinth()
		{
			if (userLabyrinthInfo != null)
			{
				userLabyrinthInfo.isInGame = false;
				userLabyrinthInfo.completeChallenge = false;
				userLabyrinthInfo.ProcessAward = InitProcessAward();
			}
		}

		public void CalculatorClearnOutLabyrinth()
		{
			if (userLabyrinthInfo != null)
			{
				int num1 = 0;
				for (int currentFloor = userLabyrinthInfo.currentFloor; currentFloor <= userLabyrinthInfo.myProgress; currentFloor++)
				{
					num1 += 2;
				}
				int num2 = num1 * 60;
				userLabyrinthInfo.remainTime = num2;
				userLabyrinthInfo.currentRemainTime = num2;
				userLabyrinthInfo.cleanOutAllTime = num2;
			}
		}

		public int[] CreateExps()
		{
			int[] numArray = new int[99];
			int num = 660;
			for (int index = 0; index < numArray.Length; index++)
			{
				numArray[index] = num;
				num += 690;
			}
			return numArray;
		}

		public void UpdateLabyrinth(int floor, int m_missionInfoId, bool bigAward)
		{
			int[] exps = CreateExps();
			int num1 = ((floor - 1 > exps.Length) ? (exps.Length - 1) : (floor - 1));
			int index = ((num1 >= 0) ? num1 : 0);
			int gp = exps[index];
			string labyrinthGold = labyrinthGolds[index];
			int count = int.Parse(labyrinthGold.Split('|')[0]);
			int num2 = int.Parse(labyrinthGold.Split('|')[1]);
			if (userLabyrinthInfo != null)
			{
				floor++;
				ProcessLabyrinthAward = CompleteGetAward(floor);
				userLabyrinthInfo.ProcessAward = ProcessLabyrinthAward;
				if (PropBag.GetItemByTemplateID(0, 11916) == null || !RemoveTemplate(11916, 1))
				{
					userLabyrinthInfo.isDoubleAward = false;
				}
				if (userLabyrinthInfo.isDoubleAward)
				{
					gp *= 2;
					count *= 2;
					num2 *= 2;
				}
				if (floor > userLabyrinthInfo.myProgress)
				{
					userLabyrinthInfo.myProgress = floor;
				}
				if (floor > userLabyrinthInfo.currentFloor)
				{
					userLabyrinthInfo.currentFloor = floor;
				}
				userLabyrinthInfo.accumulateExp += gp;
				string msg = LanguageMgr.GetTranslation("UpdateLabyrinth.Exp", gp);
				AddGP(gp, false, false);
				if (bigAward)
				{
					List<ItemInfo> itemInfoList = CopyDrop(2, 40002);
					if (itemInfoList != null)
					{
						foreach (ItemInfo cloneItem in itemInfoList)
						{
							cloneItem.IsBinds = true;
							AddTemplate(cloneItem, cloneItem.Template.BagType, count, backToMail: true);
							msg += $", {cloneItem.Template.Name} x{count}";
						}
					}
					AddHardCurrency(num2);
					msg = msg + LanguageMgr.GetTranslation("UpdateLabyrinth.GoldLaby") + num2;
				}
				SendHideMessage(msg);
			}
			Out.SendLabyrinthUpdataInfo(userLabyrinthInfo.UserID, userLabyrinthInfo);
		}

		public int AddHardCurrency(int value)
		{
			if (value > 0)
			{
				PlayerCharacter.hardCurrency += value;
				OnPropertiesChanged();
				return value;
			}
			return 0;
		}

		public virtual bool SaveIntoDatabase()
		{
			try
			{
				SaveEquipGhost();
				if (m_character.IsDirty)
				{
					using (PlayerBussiness bussiness = new PlayerBussiness())
					{
						m_bufferList.SaveToDatabase();
						bussiness.UpdateHappyRechargeData(HappyRechargeData);
						bussiness.UpdatePlayer(m_character);
                        bussiness.updateExplorerManualInfo(m_character);
                        bussiness.updateExplorerManualInfoPages(m_character);
                        bussiness.updateExplorerManualInfoDebris(m_character);
                        if (userLabyrinthInfo != null)
						{
							bussiness.UpdateLabyrinthInfo(userLabyrinthInfo);
						}
						foreach(var gemStone in GemStone)
                        {
							bussiness.UpdateGemStoneInfo(gemStone);
						}
					}
				}
				EquipBag.SaveToDatabase();
				PropBag.SaveToDatabase();
				ConsortiaBag.SaveToDatabase();
				BankBag.SaveToDatabase();
				CardBag.SaveToDatabase();
				StoreBag.SaveToDatabase();
				Rank.SaveToDatabase();
				QuestInventory.SaveToDatabase();
				AchievementInventory.SaveToDatabase();
				//BufferList.SaveToDatabase();
				BattleData.SaveToDatabase();
				Extra.SaveToDatabase();
				PetBag.SaveToDatabase(saveAdopt: true);
				FarmBag.SaveToDatabase();
				Farm.SaveToDatabase();
				Actives.SaveToDatabase();
				AvatarBag.SaveToDatabase();
				return true;
			}
			catch (Exception exception)
			{
				log.Error("Error saving player " + m_character.NickName + "!", exception);
				return false;
			}
		}

		public bool SaveNewItems()
		{
			try
			{
				EquipBag.SaveToDatabase();
				PropBag.SaveToDatabase();
				ConsortiaBag.SaveToDatabase();
				BankBag.SaveToDatabase();
				CardBag.SaveToDatabase();
				StoreBag.SaveToDatabase();
				PetBag.SaveToDatabase(saveAdopt: true);
				FarmBag.SaveToDatabase();
				Farm.SaveToDatabase();
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public bool SaveNewsItemIntoDatabase()
		{
			try
			{
				EquipBag.SaveNewsItemIntoDatabas();
				PropBag.SaveNewsItemIntoDatabas();
				return true;
			}
			catch (Exception exception)
			{
				log.Error("Error saving Save Bag Into Database " + m_character.NickName + "!", exception);
				return false;
			}
		}

		public bool SavePlayerInfo()
		{
			try
			{
				if (m_character.IsDirty)
				{
					using (PlayerBussiness bussiness = new PlayerBussiness())
					{
						bussiness.UpdatePlayer(m_character);
					}
				}
				return true;
			}
			catch (Exception exception)
			{
				log.Error("Error saving player info of " + m_character.UserName + "!", exception);
				return false;
			}
		}

		public void SendConsortiaBossInfo(ConsortiaInfo info)
		{
			RankingPersonInfo info2 = null;
			List<RankingPersonInfo> list = new List<RankingPersonInfo>();
			foreach (RankingPersonInfo info3 in info.RankList.Values)
			{
				if (info3.Name == PlayerCharacter.NickName)
				{
					info2 = info3;
				}
				else
				{
					list.Add(info3);
				}
			}
			GSPacketIn pkg = new GSPacketIn(129, PlayerCharacter.ID);
			pkg.WriteByte(30);
			pkg.WriteByte((byte)info.bossState);
			pkg.WriteBoolean(info2 != null);
			if (info2 != null)
			{
				pkg.WriteInt(info2.ID);
				pkg.WriteInt(info2.TotalDamage);
				pkg.WriteInt(info2.Honor);
				pkg.WriteInt(info2.Damage);
			}
			pkg.WriteByte((byte)list.Count);
			foreach (RankingPersonInfo info4 in list)
			{
				pkg.WriteString(info4.Name);
				pkg.WriteInt(info4.ID);
				pkg.WriteInt(info4.TotalDamage);
				pkg.WriteInt(info4.Honor);
				pkg.WriteInt(info4.Damage);
			}
			pkg.WriteByte((byte)info.extendAvailableNum);
			pkg.WriteDateTime(info.endTime);
			pkg.WriteInt(info.callBossLevel);
			SendTCP(pkg);
		}

		public bool OpenConsortiaBattleT7()
        {
			return DateTime.Now.DayOfWeek == DayOfWeek.Saturday;
		}

		public void SendConsortiaBossOpenClose(int type)
		{
			GSPacketIn pkg = new GSPacketIn(129, PlayerCharacter.ID);
			pkg.WriteByte(31);
			pkg.WriteByte((byte)type);
			SendTCP(pkg);
		}

		public void LoadGemStone(PlayerBussiness db)
		{
			lock (GemStone)
			{
				GemStone = db.GetSingleGemStones(PlayerCharacter.ID);
				if (GemStone.Count == 0)
				{
					var equipPlace = new List<int> { 11, 5, 2, 3, 13 }; //11:Set, 5:mắt, 2:Tóc, 3:mặt, 13:cánh
					var figSpiritId = new List<int> { 100002, 100003, 100001, 100004, 100005 };
					for (var p = 0; p < equipPlace.Count; p++)
					{
						var gemStone = new UserGemStone
						{
							ID = 0,
							UserID = PlayerCharacter.ID,
							FigSpiritId = figSpiritId[p],
							FigSpiritIdValue = "0,0,0|0,0,1|0,0,2",
							EquipPlace = equipPlace[p]
						};
						GemStone.Add(gemStone);
						db.AddUserGemStone(gemStone);
					}
				}
			}
		}

		public UserGemStone GetGemStone(int place)
		{
			return GemStone.FirstOrDefault(g => place == g.EquipPlace);
		}

		public void UpdateGemStone(int place, UserGemStone gem)
		{
			lock (GemStone)
			{
				for (var i = 0; i < GemStone.Count; i++)
				{
					if (place == GemStone[i].EquipPlace)
					{
						GemStone[i] = gem;
						break;
					}
				}
			}
		}

		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			GSPacketIn packet = new GSPacketIn(158);
			packet.WriteInt(consortiaID);
			packet.WriteInt(riches);
			packet.WriteString(msg);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}

		public void SendHideMessage(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(3);
			pkg.WriteString(msg);
			SendTCP(pkg);
		}

		public void SendInsufficientMoney(int type)
		{
			GSPacketIn pkg = new GSPacketIn(88, PlayerId);
			pkg.WriteByte((byte)type);
			pkg.WriteBoolean(val: false);
			SendTCP(pkg);
		}

		public void SendItemNotice(ItemInfo info, int typeGet, string Name)
		{
			if (info == null)
			{
				return;
			}
			int isBroadcast = 0;
			switch (typeGet)
			{
				case 0:
				case 1:
					isBroadcast = 2;
					break;
				case 2:
				case 3:
				case 4:
					isBroadcast = 1;
					break;
				default:
					isBroadcast = 3;
					break;
			}
			GSPacketIn pkg = new GSPacketIn(14);
			pkg.WriteString(PlayerCharacter.NickName);
			pkg.WriteInt(typeGet);
			pkg.WriteInt(info.TemplateID);
			pkg.WriteBoolean(info.IsBinds);
			pkg.WriteInt(isBroadcast);
			if (isBroadcast == 3)
			{
				pkg.WriteString(Name);
			}
			if (info.IsTips)
			{
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				for (int i = 0; i < allPlayers.Length; i++)
				{
					allPlayers[i].Out.SendTCP(pkg);
				}
			}
		}

		public bool SendItemsToMail(ItemInfo item, string content, string title, eMailType type)
		{
			return SendItemsToMail(new List<ItemInfo>
		{
			item
		}, content, title, type);
		}

		public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type)
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				List<ItemInfo> list = new List<ItemInfo>();
				foreach (ItemInfo info in items)
				{
					if (info.Template.MaxCount == 1)
					{
						for (int i = 0; i < info.Count; i++)
						{
							ItemInfo item = ItemInfo.CloneFromTemplate(info.Template, info);
							item.Count = 1;
							list.Add(item);
						}
					}
					else
					{
						list.Add(info);
					}
				}
				return SendItemsToMail(list, content, title, type, bussiness);
			}
		}

		public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb)
		{
			bool flag = true;
			for (int num = 0; num < items.Count; num += 5)
			{
				MailInfo mail = new MailInfo
				{
					Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title")),
					Gold = 0,
					IsExist = true,
					Money = 0,
					Receiver = PlayerCharacter.NickName,
					ReceiverID = PlayerId,
					Sender = PlayerCharacter.NickName,
					SenderID = PlayerId,
					Type = (int)type,
					GiftToken = 0
				};
				List<ItemInfo> list = new List<ItemInfo>();
				StringBuilder builder = new StringBuilder();
				StringBuilder builder2 = new StringBuilder();
				builder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
				content = ((content != null) ? LanguageMgr.GetTranslation(content) : "");
				int num2 = num;
				if (items.Count > num2)
				{
					ItemInfo info5 = items[num2];
					if (info5.ItemID == 0)
					{
						pb.AddGoods(info5);
					}
					else
					{
						list.Add(info5);
					}
					if (title == null)
					{
						mail.Title = info5.Template.Name;
					}
					mail.Annex1 = info5.ItemID.ToString();
					mail.Annex1Name = info5.Template.Name;
					builder.Append("1、" + mail.Annex1Name + "x" + info5.Count + ";");
					builder2.Append("1、" + mail.Annex1Name + "x" + info5.Count + ";");
				}
				num2 = num + 1;
				if (items.Count > num2)
				{
					ItemInfo info6 = items[num2];
					if (info6.ItemID == 0)
					{
						pb.AddGoods(info6);
					}
					else
					{
						list.Add(info6);
					}
					mail.Annex2 = info6.ItemID.ToString();
					mail.Annex2Name = info6.Template.Name;
					builder.Append("2、" + mail.Annex2Name + "x" + info6.Count + ";");
					builder2.Append("2、" + mail.Annex2Name + "x" + info6.Count + ";");
				}
				num2 = num + 2;
				if (items.Count > num2)
				{
					ItemInfo info4 = items[num2];
					if (info4.ItemID == 0)
					{
						pb.AddGoods(info4);
					}
					else
					{
						list.Add(info4);
					}
					mail.Annex3 = info4.ItemID.ToString();
					mail.Annex3Name = info4.Template.Name;
					builder.Append("3、" + mail.Annex3Name + "x" + info4.Count + ";");
					builder2.Append("3、" + mail.Annex3Name + "x" + info4.Count + ";");
				}
				num2 = num + 3;
				if (items.Count > num2)
				{
					ItemInfo info3 = items[num2];
					if (info3.ItemID == 0)
					{
						pb.AddGoods(info3);
					}
					else
					{
						list.Add(info3);
					}
					mail.Annex4 = info3.ItemID.ToString();
					mail.Annex4Name = info3.Template.Name;
					builder.Append("4、" + mail.Annex4Name + "x" + info3.Count + ";");
					builder2.Append("4、" + mail.Annex4Name + "x" + info3.Count + ";");
				}
				num2 = num + 4;
				if (items.Count > num2)
				{
					ItemInfo info2 = items[num2];
					if (info2.ItemID == 0)
					{
						pb.AddGoods(info2);
					}
					else
					{
						list.Add(info2);
					}
					mail.Annex5 = info2.ItemID.ToString();
					mail.Annex5Name = info2.Template.Name;
					builder.Append("5、" + mail.Annex5Name + "x" + info2.Count + ";");
					builder2.Append("5、" + mail.Annex5Name + "x" + info2.Count + ";");
				}
				mail.AnnexRemark = builder.ToString();
				if (content == null && builder2.ToString() == null)
				{
					mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content");
				}
				else if (content != "")
				{
					mail.Content = content;
				}
				else
				{
					mail.Content = builder2.ToString();
				}
				if (pb.SendMail(mail))
				{
					foreach (ItemInfo info7 in list)
					{
						TakeOutItem(info7);
					}
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public static bool SendItemsToMailSMG(List<ItemInfo> items, string content, string title, eMailType type, int ReceiverID, string ReceiverNickName)
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				List<ItemInfo> list = new List<ItemInfo>();
				foreach (ItemInfo info in items)
				{
					if (info.Template.MaxCount == 1)
					{
						for (int i = 0; i < info.Count; i++)
						{
							ItemInfo item = ItemInfo.CloneFromTemplate(info.Template, info);
							item.Count = 1;
							list.Add(item);
						}
					}
					else
					{
						list.Add(info);
					}
				}
				return SendItemsToMailSMG(list, content, title, type, bussiness, ReceiverID, ReceiverNickName);
			}
		}

		public static bool SendItemsToMailSMG(List<ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb, int ReceiverID, string ReceiverNickName)
		{
			bool flag = true;
			for (int num = 0; num < items.Count; num += 5)
			{
				MailInfo mail = new MailInfo
				{
					Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title")),
					Gold = 0,
					IsExist = true,
					Money = 0,
					Receiver = ReceiverNickName,
					ReceiverID = ReceiverID,
					Sender = ReceiverNickName,
					SenderID = ReceiverID,
					Type = (int)type,
					GiftToken = 0
				};
				List<ItemInfo> list = new List<ItemInfo>();
				StringBuilder builder = new StringBuilder();
				StringBuilder builder2 = new StringBuilder();
				builder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
				content = ((content != null) ? LanguageMgr.GetTranslation(content) : "");
				int num2 = num;
				if (items.Count > num2)
				{
					ItemInfo info5 = items[num2];
					if (info5.ItemID == 0)
					{
						pb.AddGoods(info5);
					}
					else
					{
						list.Add(info5);
					}
					if (title == null)
					{
						mail.Title = info5.Template.Name;
					}
					mail.Annex1 = info5.ItemID.ToString();
					mail.Annex1Name = info5.Template.Name;
					builder.Append("1、" + mail.Annex1Name + "x" + info5.Count + ";");
					builder2.Append("1、" + mail.Annex1Name + "x" + info5.Count + ";");
				}
				num2 = num + 1;
				if (items.Count > num2)
				{
					ItemInfo info6 = items[num2];
					if (info6.ItemID == 0)
					{
						pb.AddGoods(info6);
					}
					else
					{
						list.Add(info6);
					}
					mail.Annex2 = info6.ItemID.ToString();
					mail.Annex2Name = info6.Template.Name;
					builder.Append("2、" + mail.Annex2Name + "x" + info6.Count + ";");
					builder2.Append("2、" + mail.Annex2Name + "x" + info6.Count + ";");
				}
				num2 = num + 2;
				if (items.Count > num2)
				{
					ItemInfo info4 = items[num2];
					if (info4.ItemID == 0)
					{
						pb.AddGoods(info4);
					}
					else
					{
						list.Add(info4);
					}
					mail.Annex3 = info4.ItemID.ToString();
					mail.Annex3Name = info4.Template.Name;
					builder.Append("3、" + mail.Annex3Name + "x" + info4.Count + ";");
					builder2.Append("3、" + mail.Annex3Name + "x" + info4.Count + ";");
				}
				num2 = num + 3;
				if (items.Count > num2)
				{
					ItemInfo info3 = items[num2];
					if (info3.ItemID == 0)
					{
						pb.AddGoods(info3);
					}
					else
					{
						list.Add(info3);
					}
					mail.Annex4 = info3.ItemID.ToString();
					mail.Annex4Name = info3.Template.Name;
					builder.Append("4、" + mail.Annex4Name + "x" + info3.Count + ";");
					builder2.Append("4、" + mail.Annex4Name + "x" + info3.Count + ";");
				}
				num2 = num + 4;
				if (items.Count > num2)
				{
					ItemInfo info2 = items[num2];
					if (info2.ItemID == 0)
					{
						pb.AddGoods(info2);
					}
					else
					{
						list.Add(info2);
					}
					mail.Annex5 = info2.ItemID.ToString();
					mail.Annex5Name = info2.Template.Name;
					builder.Append("5、" + mail.Annex5Name + "x" + info2.Count + ";");
					builder2.Append("5、" + mail.Annex5Name + "x" + info2.Count + ";");
				}
				mail.AnnexRemark = builder.ToString();
				if (content == null && builder2.ToString() == null)
				{
					mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content");
				}
				else if (content != "")
				{
					mail.Content = content;
				}
				else
				{
					mail.Content = builder2.ToString();
				}
				if (pb.SendMail(mail))
				{
					foreach (ItemInfo info7 in list)
					{
						TakeOutItemSMG(info7);
					}
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public static bool TakeOutItemSMG(ItemInfo item)
		{
			return false;
		}

		public List<int> ViFarms { get; private set; }

		public void ViFarmsAdd(int playerID)
		{
			if (!ViFarms.Contains(playerID))
			{
				ViFarms.Add(playerID);
			}
		}

		public void ViFarmsRemove(int playerID)
		{
			if (ViFarms.Contains(playerID))
			{
				ViFarms.Remove(playerID);
			}
		}

		public bool SendItemToMail(int templateID, string content, string title)
		{
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(templateID);
			if (goods == null)
			{
				return false;
			}
			if (content == "")
			{
				content = goods.Name + "x1";
			}
			ItemInfo item = ItemInfo.CreateFromTemplate(goods, 1, 104);
			item.IsBinds = true;
			return SendItemToMail(item, content, title, eMailType.Active);
		}

		public bool SendItemToMail(ItemInfo item, string content, string title, eMailType type)
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				return SendItemToMail(item, bussiness, content, title, type);
			}
		}

		public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, string content, string title, eMailType type)
		{
			MailInfo mail = new MailInfo
			{
				Content = ((content != null) ? content : LanguageMgr.GetTranslation("Game.Server.GameUtils.Content")),
				Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title")),
				Gold = 0,
				IsExist = true,
				Money = 0,
				GiftToken = 0,
				Receiver = PlayerCharacter.NickName,
				ReceiverID = PlayerCharacter.ID,
				Sender = PlayerCharacter.NickName,
				SenderID = PlayerCharacter.ID,
				Type = (int)type
			};
			if (item.ItemID == 0)
			{
				pb.AddGoods(item);
			}
			mail.Annex1 = item.ItemID.ToString();
			mail.Annex1Name = item.Template.Name;
			if (pb.SendMail(mail))
			{
				TakeOutItem(item);
				return true;
			}
			return false;
		}

		public bool SendMailToUser(PlayerBussiness pb, string content, string title, eMailType type)
		{
			MailInfo mail = new MailInfo
			{
				Content = content,
				Title = title,
				Gold = 0,
				IsExist = true,
				Money = 0,
				GiftToken = 0,
				Receiver = PlayerCharacter.NickName,
				ReceiverID = PlayerCharacter.ID,
				Sender = PlayerCharacter.NickName,
				SenderID = PlayerCharacter.ID,
				Type = (int)type
			};
			mail.Annex1 = "";
			mail.Annex1Name = "";
			return pb.SendMail(mail);
		}

		public void SendMessage(string msg)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(0);
			pkg.WriteString(msg);
			SendTCP(pkg);
		}

		public void SendMessage(eMessageType type, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt((int)type);
			pkg.WriteString(msg);
			SendTCP(pkg);

		}

		public bool SendMoneyMailToUser(string title, string content, int money, eMailType type)
		{
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				return SendMoneyMailToUser(pb, content, title, money, type);
			}
		}

		public bool SendMoneyMailToUser(PlayerBussiness pb, string content, string title, int money, eMailType type)
		{
			MailInfo mail = new MailInfo
			{
				Content = content,
				Title = title,
				Gold = 0,
				IsExist = true,
				Money = money,
				GiftToken = 0,
				Receiver = PlayerCharacter.NickName,
				ReceiverID = PlayerCharacter.ID,
				Sender = PlayerCharacter.NickName,
				SenderID = PlayerCharacter.ID,
				Type = (int)type
			};
			mail.Annex1 = "";
			mail.Annex1Name = "";
			return pb.SendMail(mail);
		}

		public void SendPrivateChat(int receiverID, string receiver, string sender, string msg, bool isAutoReply)
		{
			GSPacketIn pkg = new GSPacketIn(37, PlayerCharacter.ID);
			pkg.WriteInt(receiverID);
			pkg.WriteString(receiver);
			pkg.WriteString(sender);
			pkg.WriteString(msg);
			pkg.WriteBoolean(isAutoReply);
			SendTCP(pkg);
		}

		public virtual void SendTCP(GSPacketIn pkg)
		{
			if (m_client.IsConnected)
			{
				m_client.SendTCP(pkg);
			}
		}

		public bool SetPvePermission(int copyId, eHardLevel hardLevel)
		{
			if (hardLevel == eHardLevel.Nightmare)
				return true;
			if (copyId <= m_pvepermissions.Length && copyId > 0 && hardLevel != eHardLevel.Terror && m_pvepermissions[copyId - 1] == permissionChars[(int)hardLevel])
			{
				m_pvepermissions[copyId - 1] = permissionChars[(int)(hardLevel + 1)];
				m_character.PvePermission = ConverterPvePermission(m_pvepermissions);
				OnPropertiesChanged();
				return true;
			}
			return false;
		}

		public void OpenAllNoviceActive()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				EventRewardProcessInfo[] eventRewardProcess = bussiness.GetUserEventProcess(PlayerId);
				EventRewardProcessInfo[] array = eventRewardProcess;
				foreach (EventRewardProcessInfo infos in array)
				{
					Out.SendOpenNoviceActive(0, infos.ActiveType, infos.Conditions, infos.AwardGot, DateTime.Now, DateTime.Now.AddYears(2));
				}
			}
		}

		public void ShowAllFootballCard()
		{
			for (int i = 0; i < CardsTakeOut.Length; i++)
			{
				if (CardsTakeOut[i] == null)
				{
					CardsTakeOut[i] = Card[i];
					if (takeoutCount > 0)
					{
						TakeFootballCard(Card[i]);
					}
				}
			}
		}

		public bool StackItemToAnother(ItemInfo item)
		{
			return GetItemInventory(item.Template).StackItemToAnother(item);
		}

		public void TakeFootballCard(CardInfoOld card)
		{
			List<ItemInfo> infos = new List<ItemInfo>();
			for (int i = 0; i < CardsTakeOut.Length; i++)
			{
				if (card.place == i)
				{
					CardsTakeOut[i] = card;
					CardsTakeOut[i].IsTake = true;
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(card.templateID);
					if (goods != null)
					{
						infos.Add(ItemInfo.CreateFromTemplate(goods, card.count, 110));
					}
					takeoutCount--;
					break;
				}
			}
			if (infos.Count <= 0)
			{
				return;
			}
			foreach (ItemInfo item in infos)
			{
				AddTemplate(infos);
			}
		}

		public bool TakeOutItem(ItemInfo item)
		{
			if (item.BagType == m_propBag.BagType)
			{
				return m_propBag.TakeOutItem(item);
			}
			if (item.BagType == m_fightBag.BagType)
			{
				return m_fightBag.TakeOutItem(item);
			}
			if (item.BagType == m_ConsortiaBag.BagType)
			{
				return m_ConsortiaBag.TakeOutItem(item);
			}
			if (item.BagType == m_BankBag.BagType)
			{
				return m_BankBag.TakeOutItem(item);
			}
			return m_equipBag.TakeOutItem(item);
		}

		public void TestQuest()
		{
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				QuestInfo[] aLlQuest = bussiness.GetALlQuest();
				QuestInfo[] array = aLlQuest;
				foreach (QuestInfo info in array)
				{
					QuestInventory.AddQuest(info, out var _);
				}
			}
		}

		public override string ToString()
		{
			return $"Id:{PlayerId} nickname:{PlayerCharacter.NickName} room:{CurrentRoom} ";
		}

		public void RemoveFistGetPet()
		{
			PlayerCharacter.IsFistGetPet = false;
			PlayerCharacter.LastRefreshPet = DateTime.Now.AddDays(-1);
		}

		public void RemoveLastRefreshPet()
		{
			PlayerCharacter.LastRefreshPet = DateTime.Now;
		}

		public void UpdateAnswerSite(int id)
		{
			if (PlayerCharacter.AnswerSite < id)
			{
				PlayerCharacter.AnswerSite = id;
			}
			UpdateWeaklessGuildProgress();
			Out.SendWeaklessGuildProgress(PlayerCharacter);
		}

		public void UpdateBadgeId(int Id)
		{
			m_character.badgeID = Id;
		}

		public void UpdateBarrier(int barrier, string pic)
		{
			if (CurrentRoom != null)
			{
				CurrentRoom.Pic = pic;
				CurrentRoom.barrierNum = barrier;
				CurrentRoom.currentFloor = barrier;
			}
		}

		public void UpdateBaseProperties(int attack, int defence, int agility, int lucky, int hp, int Guard)
		{
			if (attack != m_character.Attack || defence != m_character.Defence || agility != m_character.Agility || lucky != m_character.Luck)
			{
				m_character.Attack = attack;
				m_character.Defence = defence;
				m_character.Agility = agility;
				m_character.Luck = lucky;
				OnPropertiesChanged();
			}
			m_character.hp = (int)((double)(hp + LevelPlusBlood + m_character.Defence / 10) * GetBaseBlood());
			HoGiap = Guard;
		}

		public bool UpdateChangedPlaces()
		{
			try
			{
				EquipBag.UpdateChangedPlaces();
				PropBag.UpdateChangedPlaces();
				return true;
			}
			catch (Exception exception)
			{
				log.Error("Error Update Changed Places " + m_character.NickName + "!", exception);
				return false;
			}
		}

		public void UpdateDrill(int index, UserDrillInfo drill)
		{
			m_userDrills[index] = drill;
		}

		public void UpdateFightBuff(BufferInfo info)
		{
			int type = -1;
			for (int i = 0; i < FightBuffs.Count; i++)
			{
				if (info != null && info.Type == FightBuffs[i].Type)
				{
					FightBuffs[i] = info;
					type = info.Type;
				}
			}
			if (type == -1)
			{
				FightBuffs.Add(info);
			}
		}

		public void UpdateFightPower()
		{
			int num = 0;
			FightPower = 0;
			int hp = PlayerCharacter.hp;
			num += PlayerCharacter.Attack;
			num += PlayerCharacter.Defence;
			num += PlayerCharacter.Agility;
			num += PlayerCharacter.Luck;
			double baseAttack = GetBaseAttack();
			double baseDefence = GetBaseDefence();
			double probability = Convert.ToDouble(ConfigurationManager.AppSettings["probability"]);
			FightPower += (int)((double)(num + 1000) * ((probability * baseAttack * baseAttack * baseAttack) + 3.5 * baseDefence * baseDefence * baseDefence) / 100000000.0 + (double)hp * 0.95);
			if (m_currentSecondWeapon != null)
			{
				int multiple = 1;
				if (m_currentSecondWeapon.Template.Property5 == 2)
				{
					multiple = 10;
				}
				FightPower += (int)((double)m_currentSecondWeapon.Template.Property7 * Math.Pow(1.1, m_currentSecondWeapon.StrengthenLevel)) * multiple;
			}
			if (FightPower < 0)
			{
				FightPower = int.MaxValue;
			}
			PlayerCharacter.FightPower = FightPower;
            //<add key="DATE" value="3/13/2023 00:00:00"/>
			// tan vuong
            var GetDateTime = Convert.ToDateTime("08/06/2023 00:00:00");
            if (m_character.LastWeekly.Date >= GetDateTime.Date)
            {
				//LC
                GmActivityMgr.OnUpdateUseBigBugle(this, FightPower);
            }
            if (m_character.LastWeekly.Date >= GetDateTime.Date)
            {
				//Grade
                GmActivityMgr.OnUpdateLeaguePoint(this, this.PlayerCharacter.GP);
            }

            GmActivityMgr.OnPower(this, FightPower);
			

			GmActivityMgr.OnUpdateFightPower(this, FightPower);


			OnPlayerPropertyChanged(m_character);
		}

		public void UpdateHealstone(ItemInfo item)
		{
			if (item != null)
			{
				m_healstone = item;
			}
		}

		public void UpdateHide(int hide)
		{
			if (hide != m_character.Hide)
			{
				m_character.Hide = hide;
				OnPropertiesChanged();
			}
		}

		public void UpdateHonor(string honor)
		{
			var myRank = Rank.GetSingleRank(honor);
			if (myRank != null)
			{
				PlayerCharacter.honorId = myRank.NewTitleID;
				PlayerCharacter.Honor = honor;
				EquipBag.UpdatePlayerProperties();
			}
			else
			{
				SendMessage("Cập nhật danh hiệu thất bại!");
			}
		}


		public void UpdateItem(ItemInfo item)
		{
			//GetInventory((eBageType)item.BagType)?.UpdateItem(item);
			if (item.BagType == (int)eBageType.EquipBag)
				EquipBag.UpdateItem(item);

			if (item.BagType == (int)eBageType.PropBag)
				PropBag.UpdateItem(item);

			if (item.BagType == (int)eBageType.Store)
				StoreBag.UpdateItem(item);
		}

		public void AccumulativeUpdate()
		{
			if (PlayerCharacter.accumulativeLoginDays > 6)
				return;
			if (PlayerCharacter.accumulativeLoginDays == 0)
			{
				PlayerCharacter.accumulativeLoginDays = 1;
			}
			else
			{
				PlayerCharacter.accumulativeLoginDays++;
			}
		}

		public void UpdateItemForUser(object state)
		{
			m_battle.LoadFromDatabase();
			m_equipBag.LoadFromDatabase();
			AvatarBag.LoadFromDatabase();
			m_propBag.LoadFromDatabase();
			m_ConsortiaBag.LoadFromDatabase();
			m_BankBag.LoadFromDatabase();
			m_storeBag.LoadFromDatabase();
			m_cardBag.LoadFromDatabase();
			m_questInventory.LoadFromDatabase(m_character.ID);
			m_achievementInventory.LoadFromDatabase(m_character.ID);
			m_eventLiveInventory.LoadFromDatabase();
			m_bufferList.LoadFromDatabase(m_character.ID);
			m_rank.LoadFromDatabase();
			m_extra.LoadFromDatabase();
			m_petBag.LoadFromDatabase();
			FarmBag.LoadFromDatabase();
			m_playerActive.LoadFromDatabase();
		}

		public void UpdateLevel()
		{
			Level = LevelMgr.GetLevel(m_character.GP);
			int maxLevel = LevelMgr.MaxLevel;
			LevelInfo info = LevelMgr.FindLevel(maxLevel);
			/*if (Extra.CheckNoviceActiveOpen(NoviceActiveType.TANG_CAP_VIP))
			{
				Extra.UpdateEventCondition(1, Level);
			}*/
			OnLevelUp(Level);
			//GmActivityMgr.OnUpdateGrade(this, Level);
			GmActivityMgr.OnLevel(this, Level);
			if (Level == maxLevel && info != null)
			{
				m_character.GP = info.GP;
			}
		}

		public void UpdatePet(UsersPetInfo pet)
		{
			m_pet = pet;
		}

		public void UpdateProperties()
		{
			checkBug();
			Out.SendUpdatePrivateInfo(m_character, GetMedalNum());
			GSPacketIn pkg = Out.SendUpdatePublicPlayer(m_character, MatchInfo, m_extra.Info);
			if (m_currentRoom != null)
			{
				m_currentRoom.SendToAll(pkg, this);
			}
		}

		public void checkBug()
        {
			if (PlayerCharacter.Gold < 0)
				PlayerCharacter.Gold = 0;

			if (PlayerCharacter.Money < 0)
				PlayerCharacter.Money = 0;

			if (PlayerCharacter.GiftToken < 0)
				PlayerCharacter.GiftToken = 0;

			if (PlayerCharacter.myHonor < 0)
				PlayerCharacter.myHonor = 0;
        }

		public void UpdatePveResult(string type, int value, bool isWin)
		{
			var damageScore = 0;
			var honor = 0;
			var msg = "";
			switch(type)
            {
				case "worldboss":
                    {
						damageScore = value / 800;
						honor = value / 2400;
						msg = string.Format("Lần tấn công này nhận {0} cống hiến và {1} điểm vinh dự.", damageScore, honor);
						AddDamageScores(damageScore);
						RoomMgr.WorldBossRoom.UpdateRank(damageScore, honor, PlayerCharacter.NickName);
						RoomMgr.WorldBossRoom.ReduceBlood(value);
						if(isWin)
                        {
							RoomMgr.WorldBossRoom.FightOverAll();
                        }
					}
					break;
				case "cryotboss":
                    {
						if(isWin)
                        {
							Actives.UpdateStar(value);
                        }
                    }
					break;
                case "qx":
                    if (!(type == "qx"))
                    {
                        if (type == "cryotboss")
                        {
                            RoomMgr.ExitRoom(this.CurrentRoom, this);
                        }
                    }
                    else if (!isWin)
                        this.Actives.SendQXAward(value);
                    break;
                case "yearmonter":
					{
						Actives.Info.DamageNum = value;
						Actives.CreateYearMonterBoxState();
					}
					break;
			}
			AddHonor(honor);
			if (!string.IsNullOrEmpty(msg))
				SendMessage(msg);
        }

		public void UpdateEliteRank(int value)
		{
			PlayerCharacter.EliteRank = value;
		}
		public int AddEliteScore(int value)
		{
			if (value > 0 && PlayerCharacter.Grade >= 30)
			{
				PlayerCharacter.EliteScore += value;
				// send update score
				EliteGameMgr.EliteGameUpdateScore(PlayerCharacter, EliteGroup());
			}
			return 0;
		}
		public int RemoveEliteScore(int value)
		{
			if (value > 0 && PlayerCharacter.Grade >= 30)
			{
				PlayerCharacter.EliteScore -= value;

				if (PlayerCharacter.EliteScore <= 0)
					PlayerCharacter.EliteScore = 1;
				// send update score
				EliteGameMgr.EliteGameUpdateScore(PlayerCharacter, EliteGroup());
			}
			return 0;
		}
		public void SendWinEliteChampion(int blood, int win)
		{
			PlayerCharacter.EliteHpEndMatch = blood;
			EliteGameMgr.EliteGameUpdateScore(PlayerCharacter, EliteGroup(), win);
		}
		public int EliteGroup()
		{
			return PlayerCharacter.Grade <= 40 ? 1 : 2;
		}

		public void OnTakeCard(int roomType, int place, int templateId, int count)
		{
			TakeCardPlace = place;
			TakeCardTemplateID = templateId;
			TakeCardCount = count;
		}

		public void UpdateReduceDame(ItemInfo item)
		{
			if (item != null && item.Template != null)
			{
				PlayerCharacter.ReduceDamePlus = item.Template.Property1;
			}
		}

		public void UpdateSecondWeapon(ItemInfo item)
		{
			if (item != m_currentSecondWeapon)
			{
				m_currentSecondWeapon = item;
				OnPropertiesChanged();
			}
		}

		public void UpdateStyle(string style, string colors, string skin)
		{
			if (style != m_character.Style || colors != m_character.Colors || skin != m_character.Skin)
			{
				m_character.Style = style;
				m_character.Colors = colors;
				m_character.Skin = skin;
				OnPropertiesChanged();
			}
		}

		public void UpdateWeaklessGuildProgress()
		{
			if (PlayerCharacter.weaklessGuildProgress == null)
			{
				PlayerCharacter.weaklessGuildProgress = Base64.decodeToByteArray(PlayerCharacter.WeaklessGuildProgressStr);
			}
			PlayerCharacter.CheckLevelFunction();
			if (PlayerCharacter.Grade == 1)
			{
				PlayerCharacter.openFunction(Step.GAIN_ADDONE);
			}
			if (PlayerCharacter.IsOldPlayer)
			{
				PlayerCharacter.openFunction(Step.OLD_PLAYER);
			}
			PlayerCharacter.WeaklessGuildProgressStr = Base64.encodeByteArray(PlayerCharacter.weaklessGuildProgress);
		}

		public void UpdateWeapon(ItemInfo item)
		{
			if (item != m_MainWeapon)
			{
				m_MainWeapon = item;
				OnPropertiesChanged();
			}
		}

		public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
		{
			if (bag == 1)
			{
				ItemTemplateInfo template = PropItemMgr.FindFightingProp(templateId);
				if (isLiving && template != null)
				{
					OnUsingItem(template.TemplateID, 1);
					if (place == -1 && CanUseProp)
					{
						return true;
					}
					ItemInfo item2 = m_propBag.GetItemAt(place);
					if (item2 != null && item2.IsValidItem() && item2.Count >= 0)
					{
						m_propBag.RemoveCountFromStack(item2, 1);
						GmActivityMgr.OnUsingItemsEvent(this, template.TemplateID, 1);
						return true;
					}
				}
			}
			else
			{
				ItemInfo item = m_fightBag.GetItemAt(place);
				if (item != null)
				{
					OnUsingItem(item.TemplateID, 1);
					if (item.TemplateID == templateId)
					{
						GmActivityMgr.OnUsingItemsEvent(this, item.TemplateID, 1);
						return m_fightBag.RemoveItem(item);
					}
				}
			}
			return false;
		}

		public void OnPlayerAddItem(string type, int value)
		{
			if (this.PlayerAddItem != null)
			{
				this.PlayerAddItem(type, value);
			}
		}

		public void OnPlayerSpa(int onlineTimeSpa)
		{
			if (this.PlayerSpa != null)
			{
				this.PlayerSpa(onlineTimeSpa);
			}
		}

		public void OnPlayerQuestFinish(BaseQuest baseQuest)
		{
			if (this.PlayerQuestFinish != null)
			{
				this.PlayerQuestFinish(baseQuest);
			}
		}

		public void OnPlayerLogin()
		{
			if (this.PlayerLogin != null)
			{
				this.PlayerLogin();
				this.Event_0(m_character.VIPLevel, 0);
			}
		}

		public void OnPlayerPropertyChanged(PlayerInfo character)
		{
			if (this.PlayerPropertyChanged != null)
			{
				this.PlayerPropertyChanged(character);
			}
		}

		public void OnVIPUpgrade(int level, int exp)
		{
			if (this.Event_0 != null && m_character.typeVIP > 0 && m_character.VIPLevel == level)
			{
				this.Event_0(level, exp);
			}
		}

		public void OnUseBugle(int value)
		{
			if (this.UseBugle != null)
			{
				this.UseBugle(value);
			}
		}

		public void OnPlayerMarry()
		{
			if (this.PlayerMarry != null)
			{
				this.PlayerMarry();
			}
		}

		public void OnPlayerDispatches()
		{
			if (this.PlayerDispatches != null)
			{
				this.PlayerDispatches();
			}
		}

		public void OnGameOver(AbstractGame game, bool isWin, int gainXp, bool isSpanArea, bool isCouple, int blood, int playerCount)
		{
			if (game.RoomType == eRoomType.Match)
			{
				if (isWin)
				{
					m_character.Win++;
					GmActivityMgr.OnUpdateWin(this, 1);
					if (game.GameType == eGameType.Guild)
					{
						GmActivityMgr.OnUpdateGvG(this, 1);
						GmActivityMgr.OnUpdateWinGuild(this, 1);
					}
					
					if (playerCount == 2)
					{
						string timeForBxh1vs1 = GameProperties.TimeForBxh1vs1;
						string[] timeForBxh1vs1s = timeForBxh1vs1.Split('|');
						DateTime dateTime1 = Convert.ToDateTime(timeForBxh1vs1s[0]);
						DateTime dateTime2 = Convert.ToDateTime(timeForBxh1vs1s[1]);
						DateTime now = DateTime.Now;
						if (now >= dateTime1 && now <= dateTime2)
						{
							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								playerBussiness.IncreasePVP1vs1(m_character.ID);
							}

							//Out.SendMessage(eMessageType.Normal, "Chiến thắng, +1 điểm cho bảng xếp hạng 1vs1");
						}
					}
				}
				m_character.Total++;
				AddLoveScore(1, 0);
			}
			if(game.RoomType == eRoomType.Dungeon)
            {
				AddLoveScore(5, 1);
            }
			if (blood == 1)
			{
				OnFightOneBloodIsWin(game.RoomType, isWin);
			}
			if (playerCount == 4)
			{
				OnGameOver2v2(isWin);
			}
			if (playerCount == 6)
            {
				OnGameOver3v3(isWin);
            }
			if (playerCount == 8)
			{
				OnGameOver4v4(isWin);
			}
			if (isCouple && this.GameMarryTeam != null)
			{
				this.GameMarryTeam(game, isWin, gainXp, playerCount);
			}
			if (this.GameOverCountTeam != null)
			{
				this.GameOverCountTeam(game, isWin, gainXp, playerCount);
			}
			if (this.GameOver != null)
			{
				this.GameOver(game, isWin, gainXp, isSpanArea, isCouple);
			}
		}

		public void OnFightOneBloodIsWin(eRoomType roomType, bool isWin)
		{
			if (this.FightOneBloodIsWin != null)
			{
				this.FightOneBloodIsWin(roomType, isWin);
			}
		}

		public void OnGameOver2v2(bool isWin)
		{
			if (this.GameOver2v2 != null)
			{
				this.GameOver2v2(isWin);
			}
		}

		public void OnGameOver3v3(bool isWin)
        {
			if(this.GameOver3v3 != null)
            {
				this.GameOver3v3(isWin);
            }
        }

		public void OnGameOver4v4(bool isWin)
		{
			if (this.GameOver4v4 != null)
			{
				this.GameOver4v4(isWin);
			}
		}

		public void OnAcademyEvent(GamePlayer friendly, int type)
		{
			if (this.AcademyEvent != null)
			{
				this.AcademyEvent(friendly, type);
			}
		}

		public bool IsLimitMail()
		{
			if (!GameProperties.IsLimitMail)
			{
				return false;
			}
			if (Extra.Info.FreeSendMailCount >= GameProperties.LimitMail)
			{
				SendMessage($"Sô\u0301 lâ\u0300n gư\u0309i = {GameProperties.LimitMail}");
				return true;
			}
			Extra.Info.FreeSendMailCount++;
			return false;
		}

		public static List<Suit_TemplateInfo> Load_Template_Suit_info()
		{
			List<Suit_TemplateInfo> A = new List<Suit_TemplateInfo>();
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				Suit_TemplateInfo[] loadsuit = bussiness.Load_Suit_TemplateInfo();
				Suit_TemplateInfo[] array = loadsuit;
				foreach (Suit_TemplateInfo B in array)
				{
					A.Add(B);
				}
			}
			return A;
		}

		public static List<Suit_TemplateID> Load_Suit_TemplateID()
		{
			List<Suit_TemplateID> A = new List<Suit_TemplateID>();
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				Suit_TemplateID[] loadsuit = bussiness.Load_Suit_TemplateID();
				for (int i = 0; i < loadsuit.Length; i++)
				{
					A.Add(loadsuit[i]);
				}
			}
			return A;
		}

		private static List<int> DS_Item_Suit()
		{
			List<int> A = new List<int>();
			List<Suit_TemplateID> B = Load_Suit_TemplateID();
			for (int i = 0; i < B.Count; i++)
			{
				if (tachchuoi(B[i].ContainEquip).Length > 1)
				{
					int j = 0;
					while (i < tachchuoi(B[i].ContainEquip).Length)
					{
						A.Add(tachchuoi(B[i].ContainEquip)[j]);
						j++;
					}
				}
				else
				{
					A.Add(tachchuoi(B[i].ContainEquip)[0]);
				}
			}
			return A;
		}

		private static int[] tachchuoi(string A)
		{
			List<int> B = new List<int>();
			if (!A.Contains(","))
			{
				B.Add(int.Parse(A));
			}
			else
			{
				bool xet = true;
				while (xet)
				{
					if (!A.Contains(","))
					{
						B.Add(int.Parse(A));
						xet = false;
						break;
					}
					if (A.IndexOf(",") > 0)
					{
						int vtri = A.IndexOf(",");
						B.Add(int.Parse(A.Substring(0, vtri)));
						A = A.Remove(0, vtri + 1);
					}
				}
			}
			return B.ToArray();
		}

		public void ClearStoreBagWithOutPlace(int place)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 0; i < StoreBag.Capalility; i++)
			{
				if (i == place)
				{
					continue;
				}
				ItemInfo itemAt = StoreBag.GetItemAt(i);
				int num = 0;
				if (itemAt == null)
				{
					continue;
				}
				if (itemAt.Template.BagType == eBageType.PropBag)
				{
					num = PropBag.FindFirstEmptySlot();
					if (!PropBag.StackItemToAnother(itemAt) && !PropBag.AddItemTo(itemAt, num))
					{
						list.Add(itemAt);
					}
					else
					{
						StoreBag.TakeOutItem(itemAt);
					}
				}
				else
				{
					num = EquipBag.FindFirstEmptySlot(31);
					if (!EquipBag.StackItemToAnother(itemAt) && !EquipBag.AddItemTo(itemAt, num))
					{
						list.Add(itemAt);
					}
					else
					{
						StoreBag.TakeOutItem(itemAt);
					}
				}
			}
			if (list.Count > 0)
			{
				SendItemsToMail(list, "Túi đầy vật phẩm từ tiệm rèn trả về thư.", "Vật phẩm trả về từ Tiệm rèn.", eMailType.StoreCanel);
				StoreBag.ClearBagWithoutPlace(place);
			}
		}

		public void ResetRoom(bool isWin, string parram)
        {
			if(CurrentRoom != null)
            {
				if (parram == "CreateInstanceFail")
                {
					CurrentRoom.RemovePlayerUnsafe(this);
					CurrentRoom.SendCancelPickUp();
					SendMessage(LanguageMgr.GetTranslation(parram));
					return;
                }

				if (CurrentRoom.RoomType == eRoomType.CryptBoss)
				{
					RoomMgr.ExitRoom(CurrentRoom, this);
				}

				if (CurrentRoom.RoomType == eRoomType.Dungeon && parram != "RemovePlayer")
				{
					CurrentRoom.UpdateGameStyle();
					CurrentRoom.ResetPlayerState();
					CurrentRoom.Pic = "";
					CurrentRoom.MapId = 10000;
					CurrentRoom.currentFloor = 0;
					CurrentRoom.isOpenBoss = false;
					this.IsMoAiCuoi = false;
					CurrentRoom.SendRoomSetupChange(CurrentRoom);
				}
			}
        }

		/*public void ResetRoom(bool isWin, string parram)
		{
			if (CurrentRoom != null)
			{
				if (CurrentRoom.RoomType == eRoomType.Dungeon)
				{
					CurrentRoom.UpdateGameStyle();
					CurrentRoom.ResetPlayerState();
					CurrentRoom.Pic = "";
					CurrentRoom.MapId = 10000;
					CurrentRoom.currentFloor = 0;
					CurrentRoom.isOpenBoss = false;
					this.IsMoAiCuoi = false;
					CurrentRoom.SendRoomSetupChange(CurrentRoom);
				}
				if(CurrentRoom.RoomType == eRoomType.Cryptboss)
                {
					RoomMgr.ExitRoom(CurrentRoom, this);
                }
			}
		}*/

		public string TcpEndPoint()
		{
			return Client.TcpEndpoint.Split(':')[0];
		}

		public bool CanActive(string name)
		{
			if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
			{
				return true;
			}
			return false;
		}

		public void UpdateConsortiaBattle(int leftBlood, bool isWin, int tieStatus)
		{
			var winid = isWin == true ? PlayerId : GuildBattleEnemyId;
			var lostid = isWin == false ? PlayerId : GuildBattleEnemyId;

			var u = GameMgr.GuildBattle.FindUser(PlayerId);
			if (u == null) return;
			if (tieStatus != -1)
			{
				GameMgr.GuildBattle.UpdateScoreMatch(winid, lostid);
				if (isWin)
				{
					PlayerCharacter.ReduceStartBlood = leftBlood;
				}
				else
				{
					PlayerCharacter.ReduceStartBlood = PlayerCharacter.hp;
					GameMgr.GuildBattle.AddCountDownRevive(u, 30);
				}
			}
			else
			{
				SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.TieStatusEnding"));				
				//Console.WriteLine($"NickName = {u.NickName}___ Status = {u.Status}");
            }
			GuildBattleEnemyId = 0;
			PlayerCharacter.ActivePowFirstGame = false;
			if (u.IsActive)
			{
				Console.WriteLine($"Before > {u.NickName}, {this.PlayerCharacter.NickName}, {CurrentRoom.IsPlaying}");
                u.Status = UserGuildBattleStatus.NORMAL;
                GameMgr.GuildBattle.SendUpdateSceneInfo(u);
				GameMgr.GuildBattle.SendUpdatePlayerStatus(u);
                Console.WriteLine($"After > {u.NickName}, {this.PlayerCharacter.NickName}, {CurrentRoom.IsPlaying}");
            }
		}

		private bool IsOpenSanXiao()
        {
			return DateTime.Now > Convert.ToDateTime(GameProperties.SanXiaoStartTime) && DateTime.Now < Convert.ToDateTime(GameProperties.SanXiaoEndTime).AddDays(1.0);
        }

		public void sendMiniGame(bool isOpen)
        {
			GSPacketIn pkg = new GSPacketIn(346, PlayerId);
			pkg.WriteByte(4);
			pkg.WriteBoolean(isOpen);
			if (isOpen)
				pkg.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
			this.SendTCP(pkg);

			pkg = new GSPacketIn(329, PlayerId);
			pkg.WriteByte(52);
			pkg.WriteBoolean(isOpen);
			if (isOpen)
				pkg.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
			this.SendTCP(pkg);

			pkg = new GSPacketIn(329, PlayerId);
			pkg.WriteByte(6);
			pkg.WriteBoolean(false);//XepGa
			pkg.WriteDateTime(Convert.ToDateTime(GameProperties.SanXiaoEndTime));
			pkg.WriteInt(100);
			this.SendTCP(pkg);
		}

		public void SendUpdatePlayerFigSpirit()
		{
			if (PlayerCharacter.Grade >= 30)
			{
				Out.SendPlayerFigSpiritinit(PlayerCharacter.ID, GemStone);
			}
		}

		public HappyRechargeInfo HappyRechargeData
		{
			get;
			set;
		}

		public void SendPkgLimitGrade()
        {
			var id = PlayerCharacter.ID;
			if(Actives.IsYearMonsterOpen())
            {
				Out.SendCatchBeastOpen(id, true);
			}
			if (PlayerCharacter.Grade >= 10)
			{
				if (Actives.IsChristmasOpen())
				{
					Out.SendOpenOrCloseChristmas(Actives.Christmas.lastPacks, Actives.IsChristmasOpen());
				}
			}
			if(PlayerCharacter.Grade >= 20)
            {
				if (RoomMgr.WorldBossRoom.WorldbossOpen)
				{
					this.Out.SendOpenWorldBoss(this.X, this.Y);
				}
			}
			if(PlayerCharacter.Grade >= 13)
            {
				if(Actives.IsPyramidOpen())
                {
					Out.SendPyramidOpenClose(Actives.PyramidConfig);
				}
            }
		}

		public void AddExpGift(int value)
        {
			List<int> intList = GameProperties.GiftExp();
			m_character.charmGP += value;
			GmActivityMgr.OnConsumeGift(this, value);
			for (int i = 0; i < intList.Count; i++)
            {
				int giftExp = this.m_character.charmGP;
				int giftLevel = this.m_character.charmLevel;
				if (giftLevel == 100)
                {
					m_character.charmGP = intList[99];
					break;
                }
				if (giftLevel < 100 && this.canUpGiftLv(giftExp, giftLevel))
                {
					m_character.charmLevel++;
                }
				OnPropertiesChange();
            }
        }

		public bool canUpGiftLv(int exp, int curLv)
        {
			List<int> exps = GameProperties.GiftExp();
			for (int i = 0; i < exps.Count; i++)
			{
				if (exp >= exps[i] && curLv == i)
				{
					return true;
				}
			}
			return false;
		}

		public void ResetTotem(bool saveToDb)
        {
			if (PlayerCharacter.totemId > TotemMgr.MaxTotem())
				PlayerCharacter.totemId = TotemMgr.MaxTotem();
			if(saveToDb)
            {
				using (var db = new PlayerBussiness())
                {
					db.UpdatePlayer(PlayerCharacter);
                }
            }
        }

		public int AddTotem(int value)
		{
			if (value > 0)
			{
				PlayerCharacter.totemId = value;
				OnPropertiesChanged();
				return value;
			}

			return PlayerCharacter.totemId;
		}

		public int AddHonor(int value)
		{
			if (value > 0)
			{
				PlayerCharacter.myHonor += value;
				OnPropertiesChanged();
				return value;
			}
			else
			{
				return 0;
			}
		}

		public int AddDamageScores(int value) //trminhpc
		{
			if (value > 0)
			{
				PlayerCharacter.damageScores += value;
				if (PlayerCharacter.damageScores <= int.MinValue)
				{
					PlayerCharacter.damageScores = int.MaxValue;
					SendMessage(LanguageMgr.GetTranslation("GamePlayer.Msg11"));
				}
				if (WorldBossDamageEvent != null)
				{
					WorldBossDamageEvent(this);
				}
				OnPropertiesChanged();
				return value;
			}

			return 0;
		}

		public int RemoveDamageScores(int value)
		{
			if (value > 0 && value <= PlayerCharacter.damageScores)
			{
				PlayerCharacter.damageScores -= value;
                if (WorldBossDamageEvent != null)
                {
                    WorldBossDamageEvent(this);
                }
                OnPropertiesChanged();
				return value;
			}

			return 0;
		}

		public int RemoveHonor(int value)
		{
			if (value > 0)
			{
				PlayerCharacter.myHonor -= value;
				OnPropertiesChanged();
				return value;
			}
			else
			{
				return 0;
			}
		}

		public int RemovemyHonor(int value)
		{
			if (value > 0 && value <= PlayerCharacter.myHonor)
			{
				PlayerCharacter.myHonor -= value;
				OnPropertiesChanged();
				return value;
			}

			return 0;
		}

		public int AddMaxHonor(int value)
		{
			if (value > 0)
			{
				PlayerCharacter.MaxBuyHonor += value;
				OnPropertiesChanged();
				return value;
			}

			return 0;
		}

		public int AddLoveScore(int value, int type)
        {
			if(PlayerCharacter.IsMarried == false)
            {
				return 0;
            }

			if (value > 0 && Extra.CanRingExp(type))
            {
				PlayerCharacter.RingExp += value;
				OnPropertiesChanged();
				Extra.UpdateLoveScoreLimit(type);
				return value;
			}
			return 0;
        }
		#region EquipGhost
		public void SaveEquipGhost()
		{
			lock (m_equipGhostList)
			{
				PlayerCharacter.GhostEquipList = JsonConvert.SerializeObject(m_equipGhostList);
			}
		}

		public void AddEquipGhost(UserEquipGhostInfo equipGhost)
		{
			lock (m_equipGhostList)
			{
				if (!m_equipGhostList.ContainsKey(equipGhost.BagType + "_" + equipGhost.Place))
				{
					m_equipGhostList.Add(equipGhost.BagType + "_" + equipGhost.Place, equipGhost);
				}
			}
		}

		public UserEquipGhostInfo GetGhostEquip(int bagType, int place)
		{
			lock (m_equipGhostList)
			{
				if (m_equipGhostList.ContainsKey(bagType + "_" + place))
				{
					return m_equipGhostList[bagType + "_" + place];
				}

				return null;
			}
		}

		public List<UserEquipGhostInfo> GetAllEquipGhost()
		{
			var list = new List<UserEquipGhostInfo>();
			lock (m_equipGhostList)
			{
				foreach (var info in m_equipGhostList.Values)
					list.Add(info);
			}

			return list;
		}
		#endregion

		private static Random random = new Random();
		public static string CreateRandomInviteCode(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string randomString = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
			string createCode = "LIKE" + randomString;
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				PlayerInfo checkInviteNum = bussiness.GetUserSingleByInviteCode(createCode);
				if (checkInviteNum == null)
				{
					return createCode;
				}
				else
				{
					createCode = new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()) + "9";
					return createCode;
				}
			}
			//return null;
		}

		public int AddLeageScore(bool isWin, int Value)
		{
			if (Value > 0)
			{
				/*if (m_character.DailyGameCount <= 10)
				{
					Value += ThreadSafeRandom.NextStatic(50, 100);
				}*/
				m_character.DailyScore += Value;
				m_character.WeeklyScore += Value;
				m_character.DailyGameCount++;
				m_character.WeeklyGameCount++;
				if (isWin)
				{
					m_character.DailyWinCount++;
				}
				OnPlayerAddItem("DailyScore", Value);
				OnPlayerAddItem("WeeklyScore", Value);
				//GmActivityMgr.OnUpdateLeaguePoint(this, Value);
				OnPropertiesChanged();
				return Value;
			}
			else
			{
				return 0;
			}
		}

        public void SignInActivity(int type)
        {
            List<IGMActive> lists = GmActivityMgr.GetAllGMActionByType(typeof(SignActivity));
            if (lists.Count > 0)
            {
                GSPacketIn pkg = new GSPacketIn(405, PlayerCharacter.ID);
                pkg.WriteInt(type);
                pkg.WriteInt(lists.Count);
                foreach (var gm in lists)
                {
                    pkg.WriteString(gm.Info.activityId);
                    gm.SetStatusPacket(this, pkg);

                    Dictionary<string, GmGiftInfo> gmGiftList = gm.Info.GiftsGroup;
                    UserGmActivityInfo userInfo = gm.GetPlayer(this);
                    pkg.WriteInt(gmGiftList.Count);

                    foreach (GmGiftInfo gmGiftInfo in gmGiftList.Values)
                    {
                        pkg.WriteString(gmGiftInfo.giftbagId);
                        GiftCurInfo giftCurInfo = userInfo.GiftsReceivedList.SingleOrDefault(a => a.giftID == gmGiftInfo.giftbagId);
                        if (giftCurInfo == null)
                        {
                            pkg.WriteInt(0);
                            pkg.WriteInt(0);
                        }
                        else
                        {
                            pkg.WriteInt(giftCurInfo.times);
                            pkg.WriteInt(giftCurInfo.allGiftGetTimes);
                        }
                    }
                }
                //Console.WriteLine("Running SignActivity..");
            }
        }

        public void UpdateLeage()
		{
			m_character.DailyLeagueLastScore = m_character.WeeklyScore;
			m_character.DailyScore = 0;
			m_character.DailyWinCount = 0;
			m_character.DailyGameCount = 0;
			m_character.WeeklyScore = 0;
			m_character.WeeklyGameCount = 0;
			m_character.WeeklyRanking = 0;
			if (m_character.DailyLeagueFirst)
			{
				m_character.DailyLeagueFirst = false;
			}
		}

        public void PVPFightNotice(string msg)
        {
			if (msg != null)
			{
				WorldMgr.SendSysTipNotice(msg);
			}
		}

        public long WorldbossBood { get; set; }

		public bool IsInWorldBossRoom;

		public bool IsQuanChien { get; set; }
		public int Place { get; set; }
	}
}