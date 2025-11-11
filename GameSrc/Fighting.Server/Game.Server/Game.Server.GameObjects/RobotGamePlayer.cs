using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.ConsortiaTask;
using Game.Server.GameUtils;
using Game.Server.GMActives;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.GameObjects
{
    internal class RobotGamePlayer : GamePlayer
    {

        public static IPacketLib NULLOUT = new NullOut();

        public override IPacketLib Out => NULLOUT;

        public override bool IsActive => true;

        public RobotGamePlayer(int playerId, PlayerInfo info) : base(playerId, "", null, info)
        {
        }

        public void Equip(int equipid, int strength, int compose)
        {
            var item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(equipid), 1, 0);
            item.StrengthenLevel = strength;
            item.AgilityCompose = compose;
            item.AttackCompose = compose;
            item.DefendCompose = compose;
            item.LuckCompose = compose;
            this.EquipBag.AddItem(item, this.EquipBag.FindItemEpuipSlot(item.Template));
        }

        public class NullOut : IPacketLib
        {
            public void SendLeftRouleteResult(UsersExtraInfo info) => Nothing();

            public void SendLeftRouleteOpen(UsersExtraInfo info) => Nothing();

            public void SendAcademyGradute(GamePlayer app, int type) => Nothing();

            public GSPacketIn SendAcademyAppState(PlayerInfo player, int removeUserId) => null;

            public GSPacketIn SendAcademySystemNotice(string text, bool isAlert) => null;

            public GSPacketIn SendConsortiaTaskInfo(BaseConsortiaTask baseTask) => null;

            public GSPacketIn SendSystemConsortiaChat(string content, bool sendToSelf) => null;

            public void SendShopGoodsCountUpdate(List<ShopFreeCountInfo> list) => Nothing();

            public void SendEliteGameStartRoom() => Nothing();

            public GSPacketIn SendLabyrinthUpdataInfo(int ID, UserLabyrinthInfo laby) => null;

            public GSPacketIn SendPetInfo(int id, int zoneId, UsersPetInfo[] pets, EatPetsInfo eatpet) => null;

            public GSPacketIn SendUpdateUserPet(PetInventory bag, int[] slots) => null;

            public GSPacketIn sendBuyBadge(int consortiaID, int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid) => null;

            public void SendEdictumVersion() => Nothing();

            public void SendEnthrallLight() => Nothing();

            public void SendUpdateFirstRecharge(bool isRecharge, bool isGetAward) => Nothing();

            public void SendOpenNoviceActive(int channel, int activeId, int condition, int awardGot, DateTime startTime, DateTime endTime) => Nothing();

            public GSPacketIn SendOpenTimeBox(int condtion, bool isSuccess) => null;

            public void SendUpdateCardData(PlayerInfo player, List<UsersCardInfo> userCard) => Nothing();

			public GSPacketIn SendAddFriend(PlayerInfo user, int relation, bool state) => null;

			public GSPacketIn SendFriendRemove(int FriendID) => null;

			public GSPacketIn SendFriendState(int playerID, int state, byte typeVip, int viplevel) => null;

			//public GSPacketIn sendBuyBadge(int BadgeID, int ValidDate, bool result, string BadgeBuyTime, int playerid) => null;

			public GSPacketIn SendConsortiaMail(bool result, int playerid) => null;

			public GSPacketIn sendOneOnOneTalk(int receiverID, bool isAutoReply, string SenderNickName, string msg, int playerid) => null;

			public GSPacketIn SendUpdateConsotiaBoss(ConsortiaBossInfo bossInfo) => null;

			public GSPacketIn SendHotSpringUpdateTime(GamePlayer player, int expAdd) => null;

			public GSPacketIn SendPlayerDrill(int ID, Dictionary<int, UserDrillInfo> drills) => null;

			public void SendTCP(GSPacketIn packet) => Nothing();

			public GSPacketIn SendConsortiaLevelUp(byte type, byte level, bool result, string msg, int playerid) => null;

			public GSPacketIn SendUpdateConsotiaBuffer(GamePlayer player, Dictionary<int, BufferInfo> bufflist) => null;

			public void SendLoginSuccess() => Nothing();

			public void SendLoginSuccess2() => Nothing();

			public void SendCheckCode() => Nothing();

			public void SendLoginFailed(string msg) => Nothing();

			public void SendKitoff(string msg) => Nothing();

			public void SendEditionError(string msg) => Nothing();

			public void SendWeaklessGuildProgress(PlayerInfo player) => Nothing();

			public GSPacketIn SendUpdateAchievementData(List<AchievementDataInfo> infos) => null;

			public GSPacketIn SendAchievementSuccess(AchievementDataInfo d) => null;

			public GSPacketIn SendUpdateAchievements(List<UsersRecordInfo> infos) => null;

			public GSPacketIn SendInitAchievements(List<UsersRecordInfo> infos) => null;

			public GSPacketIn SendUpdateAchievements(UsersRecordInfo info) => null;

			public GSPacketIn SendGameRoomSetupChange(BaseRoom room) => null;

			public void SendDateTime() => Nothing();

			public GSPacketIn SendDailyAward(GamePlayer player) => null;

			public void SendPingTime(GamePlayer player) => Nothing();

			public void SendUpdatePrivateInfo(PlayerInfo info, int medal) => Nothing();

			public GSPacketIn SendUpdatePublicPlayer(PlayerInfo info, UserMatchInfo matchInfo, UsersExtraInfo extraInfo) => null;

			public GSPacketIn SendNetWork(int id, long delay) => null;

            public GSPacketIn SendUserEquip(PlayerInfo player, List<ItemInfo> items, List<UserGemStone> userGemStone, ExplorerManualInfo explorerInfo) => null;

            public GSPacketIn SendMessage(eMessageType type, string message) => null;

			public void SendWaitingRoom(bool result) => Nothing();

			public GSPacketIn SendUpdateRoomList(List<BaseRoom> room) => null;

			public GSPacketIn SendSceneAddPlayer(GamePlayer player) => null;

			public GSPacketIn SendSceneRemovePlayer(GamePlayer player) => null;

			public GSPacketIn SendRoomCreate(BaseRoom room) => null;

			public GSPacketIn SendRoomLoginResult(bool result) => null;

			public GSPacketIn SendRoomPlayerAdd(GamePlayer player) => null;

			public GSPacketIn SendRoomPlayerRemove(GamePlayer player) => null;

			public GSPacketIn SendRoomUpdatePlayerStates(byte[] states) => null;

			public GSPacketIn SendRoomUpdatePlacesStates(int[] states) => null;

			public GSPacketIn SendRoomPlayerChangedTeam(GamePlayer player) => null;

			public GSPacketIn SendRoomPairUpStart(BaseRoom room) => null;

			public GSPacketIn SendRoomPairUpCancel(BaseRoom room) => null;

			public GSPacketIn SendEquipChange(GamePlayer player, int place, int goodsID, string style) => null;

			public GSPacketIn SendRoomChange(BaseRoom room) => null;

			public GSPacketIn SendFusionPreview(GamePlayer player, Dictionary<int, double> previewItemList, bool isBind, int MinValid) => null;

			public GSPacketIn SendFusionResult(GamePlayer player, bool result) => null;

			public GSPacketIn SendRefineryPreview(GamePlayer player, int templateid, bool isbind, ItemInfo item) => null;

			public void SendUpdateInventorySlot(PlayerInventory bag, int[] updatedSlots) => Nothing();

			public void SendUpdateCardData(CardInventory bag, int[] updatedSlots) => Nothing();

			public GSPacketIn SendUpdateBuffer(GamePlayer player, AbstractBuffer[] infos) => null;

			public GSPacketIn SendBufferList(GamePlayer player, List<AbstractBuffer> infos) => null;

			public GSPacketIn SendUpdateQuests(GamePlayer player, byte[] states, BaseQuest[] quests) => null;

			public GSPacketIn SendMailResponse(int playerID, eMailRespose type) => null;

			public GSPacketIn SendAuctionRefresh(AuctionInfo info, int auctionID, bool isExist, ItemInfo item) => null;

			public GSPacketIn SendIDNumberCheck(bool result) => null;

			public GSPacketIn SendAASState(bool result) => null;

			public GSPacketIn SendAASInfoSet(bool result) => null;

			public GSPacketIn SendAASControl(bool result, bool bool_0, bool IsMinor) => null;

			public GSPacketIn SendGameRoomInfo(GamePlayer player, BaseRoom game) => null;

			public GSPacketIn SendMarryInfoRefresh(MarryInfo info, int ID, bool isExist) => null;

			public GSPacketIn SendMarryRoomInfo(GamePlayer player, MarryRoom room) => null;

			public GSPacketIn SendPlayerEnterMarryRoom(GamePlayer player) => null;

			public GSPacketIn SendPlayerMarryStatus(GamePlayer player, int userID, bool isMarried) => null;

			public GSPacketIn SendPlayerMarryApply(GamePlayer player, int userID, string userName, string loveProclamation, int ID) => null;

			public GSPacketIn SendPlayerDivorceApply(GamePlayer player, bool result, bool isProposer) => null;

			public GSPacketIn SendMarryApplyReply(GamePlayer player, int UserID, string UserName, bool result, bool isApplicant, int ID) => null;

			public GSPacketIn SendBigSpeakerMsg(GamePlayer player, string msg) => null;

			public GSPacketIn SendPlayerLeaveMarryRoom(GamePlayer player) => null;

			public GSPacketIn SendMarryRoomLogin(GamePlayer player, bool result) => null;

			public GSPacketIn SendMarryRoomInfoToPlayer(GamePlayer player, bool state, MarryRoomInfo info) => null;

			public GSPacketIn SendMarryInfo(GamePlayer player, MarryInfo info) => null;

			public GSPacketIn SendContinuation(GamePlayer player, MarryRoomInfo info) => null;

			public GSPacketIn SendMarryProp(GamePlayer player, MarryProp info) => null;

			public GSPacketIn SendRoomType(GamePlayer player, BaseRoom game) => null;

			public void SendUserLuckyNum() => Nothing();

			public GSPacketIn SendOpenVIP(GamePlayer Player) => null;

			public GSPacketIn SendUserRanks(int Id, List<UserRankInfo> ranks) => null;

			public GSPacketIn SendEnterHotSpringRoom(GamePlayer player) => null;

			public GSPacketIn SendGetUserGift(PlayerInfo player, UserGiftInfo[] allGifts) => null;

			public GSPacketIn SendEatPetsInfo(EatPetsInfo info) => null;

			public GSPacketIn SendRefreshPet(GamePlayer player, UsersPetInfo[] pets, ItemInfo[] items, bool refreshBtn) => null;

			public void SendLeagueNotice(int id) => Nothing();

			public GSPacketIn SendEnterFarm(PlayerInfo Player, UserFarmInfo farm, UserFieldInfo[] fields) => null;

			public GSPacketIn SendSeeding(PlayerInfo Player, UserFieldInfo field) => null;

			public GSPacketIn SenddoMature(PlayerFarm farm) => null;

			public GSPacketIn SendtoGather(PlayerInfo Player, UserFieldInfo field) => null;

			public GSPacketIn SendKillCropField(PlayerInfo Player, UserFieldInfo field) => null;

			public GSPacketIn SendHelperSwitchField(PlayerInfo Player, UserFarmInfo farm) => null;

			public GSPacketIn SendPayFields(GamePlayer Player, List<int> fieldIds) => null;

			public void SendLittleGameActived() => Nothing();

			public GSPacketIn SendChickenBoxOpen(int ID, int flushPrice, int[] openCardPrice, int[] eagleEyePrice) => null;

			public GSPacketIn SendSingleRoomCreate(BaseRoom room) => null;

			public GSPacketIn SendSingleRoomCreate(BaseRoom room, GamePlayer player) => null;

			public GSPacketIn SendConsortiaBattleOpenClose(int ID, bool result) => null;

			public GSPacketIn SendUpdatePlayerProperty(PlayerInfo info, PlayerProperty prop) => null;

			public void SendOpenHappyRecharge(int playerID) => Nothing();

			public void SendCatchBeastOpen(int playerID, bool isOpen) => Nothing();

			public GSPacketIn SendUpdateUpCount(PlayerInfo player) => null;

			public GSPacketIn SendPlayerRefreshTotem(PlayerInfo player) => null;

			public GSPacketIn SendPlayerFigSpiritinit(int ID, List<UserGemStone> gems) => null;

			public GSPacketIn SendPlayerFigSpiritUp(int ID, UserGemStone gem, bool isUp, bool isMaxLevel, bool isFall, int num, int dir) => null;

			public GSPacketIn SendNecklaceStrength(PlayerInfo player) => null;

			public void WonderfulSingleActivityInit(IGMActive gmActivity, GamePlayer player) => Nothing();

			public void WonderfulActivityInit(List<IGMActive> allActions, GamePlayer player, int type) => Nothing();

			public void SendOpenOrCloseChristmas(int lastPacks, bool isOpen) => Nothing();

			public GSPacketIn SendUserSyncEquipGhost(GamePlayer p) => null;

			public GSPacketIn SendAvatarColectionAllInfo(int PlayerId, Dictionary<int, UserAvatarColectionInfo> infos) => null;

			public void SendOpenWorldBoss(int pX, int pY) => Nothing();

			public void SendLoginChickActivation(UserChickActiveInfo chickInfo) => Nothing();

			public void SendUpdateChickActivation(UserChickActiveInfo chickInfo) => Nothing();

			public GSPacketIn SendInviteFriends(PlayerInfo player, int type) => null;

			public void SendPyramidOpenClose(PyramidConfigInfo info) => Nothing();

			private void Nothing()
            {
            }
        }

        public override void SendTCP(GSPacketIn pkg)
        {
        }

        public override bool LoadFromDatabase()
        {
            return true;
        }

        public override bool SaveIntoDatabase()
        {
            return true;
        }

        public override void Disconnect()
        {
        }
	}
}
