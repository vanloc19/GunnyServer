namespace Game.Server.Packets
{
    public enum ActiveSystemPackageType
    {
        //PyramidPackageType
        PYRAMID_OPENORCLOSE = 0,
        PYRAMID_ENTER = 1,
        PYRAMID_STARTORSTOP = 2,
        PYRAMID_RESULT = 3,
        PYRAMID_DIEEVENT = 4,
        PYRAMID_SCORECONVERT = 5,

        //GuildMemberWeekPackageType
        OPEN = 7,
        PLAYERTOP10 = 8,
        ADDPOINTBOOK10 = 9,
        GET_MYRUNKING = 11,
        ADDPOINTBOOKRECORD = 10,
        UPADDPOINTBOOK = 12,
        SHOWACTIVITYEND = 13,
        ENTER_GAME = 8,
        CLOSE = 9,
        SEND_ADDRUNKING = 10,
        FINISHACTIVITY = 53,
        SHOWRUNKING = 54,

        //ChristmasPackageType
        CHRISTMAS_OPENORCLOSE = 16,
        CHRISTMAS_PLAYERING_SNOWMAN_ENTER = 17,
        CHRISTMAS_MAKING_SNOWMAN_ENTER = 24,
        GET_PAKCS_TO_PLAYER = 27,
        CHRISTMAS_PACKS = 26,
        CHRISTMAS_SCORECONVERT = 5,
        FIGHT_SPIRIT_LEVELUP = 25,
        MOVE = 21,
        ADDPLAYER = 18,
        CHRISTMAS_EXIT = 19,
        PLAYER_STATUE = 20,
        CHRISTMAS_MONSTER = 22,
        FIGHT_MONSTER = 22,
        CHRISTMAS_ROOM_SPEAK = 28,
        CHRISTMAS_BUY_TIMER = 29,
        UPDATE_TIMES_ROOM = 30,

        //CatchBeastPackageType
        CATCHBEAST_BEGIN = 32,
        CATCHBEAST_VIEWINFO = 33,
        CATCHBEAST_CHALLENGE = 34,
        CATCHBEAST_BUYBUFF = 35,
        CATCHBEAST_GETAWARD = 36,

        //LanternriddlesPackageType
        LANTERNRIDDLES_BEGIN = 37,
        LANTERNRIDDLES_QUESTION = 38,
        LANTERNRIDDLES_ANSWERRESULT = 39,
        LANTERNRIDDLES_ANSWER = 40,
        LANTERNRIDDLES_SKILL = 41,
        LANTERNRIDDLES_RANKINFO = 42,
        LANTERNRIDDLES_BEGINTIPS = 43,

        //SuperWinnerPackageType
        SUPER_WINNER_OPEN = 48,
        ENTER_ROOM = 49,
        ROLLS_DICES = 50,
        OUT_ROOM = 51,
        RETURN_DICES = 52,
        END_GAME = 53,
        TIMES_UP = 54,
        START_ROLL_DICES = 55,
        JOIN_ROOM = 57,

        //EntertainmentPackageType
        GET_SCORE = 71,
        BUY_ICON = 72,

        //DDPlay
        DDPLAY_BEGIN = 74,
        ENTER_DDPLAY = 75,
        DDPLAY_START = 76,
        DDPLAY_EXCHANGE = 77,

        //SevenDayTargetPackageType
        SEVENDAYTARGET_OPEN_CLOSE = 80,
        SEVENDAYTARGET_ENTER = 81,
        SEVENDAYTARGET_GET_REWARD = 82,
        NEWPLAYERREWARD_OPEN_CLOSE = 96,
        NEWPLAYERREWARD_ENTER = 97,
        NEWPLAYERREWARD_GET_REWARD = 98,

        //GodsRoadsPkgType
        GODS_ROADS_OPEN = 86,
        ENTER_GODS_ROADS = 87,
        GET_AWARDS = 88,

        //BoguAdventureType
        ACTIVITY_OPEN = 89,
        ENTER_BOGUADVENTURE = 90,
        UPDATE_CELL = 91,
        REVIVE_GAME = 92,
        ACQUIRE_AWARD = 93,
        FREE_RESET = 99,
        OUT_BOGUADVENTURE = 94,

        //TreasurePuzzlePackageType
        TREASUREPUZZLE_OPEN_CLOSE = 102,
        TREASUREPUZZLE_ENTER = 103,
        TREASUREPUZZLE_SEE_REWARD = 104,
        TREASUREPUZZLE_GET_REWARD = 105,
        TREASUREPUZZLE_USE_PICE = 106,
        TREASUREPUZZLE_SAVE_PLAYERINFO = 107,
        TREASUREPUZZLE_FLUSH = 108,

        //Halloween
        HALLOWEEN_OPEN = 115,
        HALLOWEEN_ENTER = 116,

        //CloudBuyLotteryPackageType
        OPEN_GAME = 117,
        Enter_GAME = 118,
        GET_REWARD = 119,
        BUY_GOODS = 120,
        QUIT_GAME = 121,
        UPDATE_INFO = 122,

        //CatchInsectPackageType
        OPEN_OR_CLOSE = 128,
        ENTER_SCENE = 143,
        ADDPLAYER_CATCHINSECT = 129,
        MOVE_CATCHINSECT = 141,
        PLAYER_STATUE_CATCHINSECT = 131,
        REMOVE_PLAYER = 130,
        MONSTER = 133,
        FIGHT_MONSTER_CATCHINSECT = 140,
        UPDATE_INFO_CATCHINSECT = 135,
        GET_PRIZE = 139,
        UPDATE_AREA_RANK = 137,
        AREA_SELF_INFO = 132,
        UPDATE_LOCAL_RANK = 136,
        LOCAL_SELF_INFO = 138,
        CAKE_STATUS = 134,
        WHISTLE_USE = 154,
        WHISTLE_BUY = 155,

        //TreasureLostPackageType
        ENTERGAME = 145,
        TREASURELOST_OPEN_CLOSE = 144,
        TREASURELOST_ROLLDICE = 146,
        TREASURELOST_EVENT_DISPATCH = 147,
        TREASURELOST_BUYITEM = 148,
        TREASURELOST_UPDATA_FIVEITEM = 149,
        EXITGAME = 150,

        //PrayIndiana
        PRAY_OPEN = 170,
        PRAYGOODSACTIVITYENTER = 171,
        PRAYGOODSACTIVITYREFRESH = 172,
        PRAYGOODSACTIVITYEXTRACT = 173,
        PROBABILITY = 174,

        //HappyRecharge
        HAPPYRECHARGE_PLAY = 176,
        HAPPYRECHARGE_EXCHANGE = 177,
        HAPPYRECHARGE_OPEN = 178,
        HAPPYRECHARGE_ENTER = 179,
        HAPPYRECHARGE_UPDATE = 180,
        HAPPYREChARGE_REQUESTITEM = 181,

        //PetIsland
        PETISLAND_OPENORCLOSE = 186,
        PETISLAND_ENTER = 187,
        PETISLAND_BUYBLOOD = 188,
        PETISLAND_MOVE = 189,
        PETISLAND_PRIZE = 190,

        // sign buff
        SIGNBUFF_MESSAGE = 12,
    }
}