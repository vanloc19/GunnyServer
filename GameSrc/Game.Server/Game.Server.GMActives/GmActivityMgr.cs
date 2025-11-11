using Bussiness;
using log4net;
using SqlDataProvider.Data;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Game.Server.GameObjects;
using Bussiness.Managers;
using Game.Base;

namespace Game.Server.GMActives
{
    public class GmActivityMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Dictionary<string, GmActivityInfo> m_GmActivitys;

        // setup action
        private static Dictionary<string, IGMActive> m_listAction = new Dictionary<string, IGMActive>();

        public static bool ReLoad(bool reScan)
        {
            try
            {
                GmActivityInfo[] tempGmActivity = LoadGmActivityDb();
                GmGiftInfo[] tempGmGift = LoadGmGiftDb();
                GmActiveConditionInfo[] tempGmConditions = LoadAllGmActiveConditions();
                GmActiveRewardInfo[] tempGmRewards = LoadAllGmActiveRewards();

                Dictionary<string, GmActivityInfo> tempFinalGmActive = new Dictionary<string, GmActivityInfo>();
                foreach (GmActivityInfo tempGm in tempGmActivity)
                {
                    if (!tempFinalGmActive.ContainsKey(tempGm.activityId))
                    {
                        foreach (GmGiftInfo tGm in tempGmGift.Where(a => a.activityId == tempGm.activityId))
                        {
                            foreach (GmActiveConditionInfo tCm in tempGmConditions.Where(a => a.giftbagId == tGm.giftbagId))
                            {
                                tGm.Conditions.Add(tCm.conditionIndex, tCm);
                            }

                            tGm.RewardsList = tempGmRewards.Where(a => a.giftId == tGm.giftbagId).ToList();

                            tempGm.GiftsGroup.Add(tGm.giftbagId, tGm);
                        }
                        tempFinalGmActive.Add(tempGm.activityId, tempGm);
                    }
                }

                Interlocked.Exchange(ref m_GmActivitys, tempFinalGmActive);

                if (reScan)
                    ScanAction();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("ReLoad GmActivity", e);
                return false;
            }
            return true;
        }


        public static bool Init()
        {
            if (ReLoad(false))
            {
                UpdateAllAction(LoadData());
                return true;
            }
            return false;
        }

        private static RankRechargeInfo LoadData()
        {
            using (MarshalBussiness mb = new MarshalBussiness())
            {
                RankRechargeInfo data = mb.LoadDataJsonFile<RankRechargeInfo>("gmactivity", false);

                if (data == null)
                {
                    data = new RankRechargeInfo();
                }

                return data;
            }
        }

        public static void SaveData()
        {
            RankRechargeInfo data = new RankRechargeInfo();
            // save all data
            List<IGMActive> allActions = GetAllGmAction();

            foreach (IGMActive action in allActions)
            {
                if (!data.UsersData.ContainsKey(action.Info.activityId))
                {
                    data.UsersData.Add(action.Info.activityId, action.GetAllPlayers());
                }

                if (action.IsLockAuto && !data.ListActIDAwardsSended.Contains(action.Info.activityId))
                {
                    data.ListActIDAwardsSended.Add(action.Info.activityId);
                }
            }
            using (MarshalBussiness mb = new MarshalBussiness())
                mb.SaveDataJsonFile(data, "gmactivity", false);
        }

        public static List<GmActivityInfo> GetAllGmActivity()
        {
            lock (m_GmActivitys)
            {
                List<GmActivityInfo> listActivity = new List<GmActivityInfo>();

                foreach (GmActivityInfo gmActivity in m_GmActivitys.Values)
                {
                    if (gmActivity.IsAvalibleShow)
                    {
                        listActivity.Add(gmActivity);
                    }
                }
                return listActivity;
            }
        }

        public static List<IGMActive> GetAllGmAction()
        {
            List<IGMActive> lists = new List<IGMActive>();
            lock (m_listAction)
            {
                foreach (IGMActive gm in m_listAction.Values)
                {
                    lists.Add(gm);
                }
            }

            return lists;
        }

        public static IGMActive GetSingleGMAction(string activityId)
        {
            lock (m_listAction)
            {
                if (m_listAction.ContainsKey(activityId))
                {
                    return m_listAction[activityId];
                }
            }

            return null;
        }

        public static List<IGMActive> GetAllGMActionByType(Type type)
        {
            List<IGMActive> lists = new List<IGMActive>();
            lock (m_listAction)
            {
                foreach (IGMActive active in m_listAction.Values)
                {
                    if (active.GetType() == type)
                    {
                        lists.Add(active);
                    }
                }
            }
            return lists;
        }

        private static void UpdateAllAction(RankRechargeInfo tempAllUserGmInfo)
        {
            lock (m_GmActivitys)
            {
                foreach (GmActivityInfo gmInfo in m_GmActivitys.Values)
                {
                    if (gmInfo.IsAvalibleShow)
                    {
                        IGMActive temp = null;

                        if (tempAllUserGmInfo.UsersData.ContainsKey(gmInfo.activityId))
                        {
                            temp = AddGMActivityAction(gmInfo, tempAllUserGmInfo.UsersData[gmInfo.activityId]);
                        }
                        else
                        {
                            temp = AddGMActivityAction(gmInfo, null);
                        }

                        if (temp != null && tempAllUserGmInfo.ListActIDAwardsSended.Contains(gmInfo.activityId))
                            temp.IsLockAuto = true;
                    }
                }
            }
            Console.WriteLine(string.Format("Load {0} actions GMActivity success.", m_listAction.Count));
        }

        public static IGMActive AddGMActivityAction(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo)
        {
            lock (m_listAction)
            {
                if (!m_listAction.ContainsKey(gmActive.activityId))
                {
                    // add package
                    IGMActive temp = CreateTemplateGMActiveAction(gmActive, listUsersGmInfo);
                    if (temp != null)
                    {
                        m_listAction.Add(gmActive.activityId, temp);
                        temp.Start();

                        return temp;
                    }
                }
            }

            return null;
        }

        private static IGMActive CreateTemplateGMActiveAction(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo)
        {
            switch (gmActive.activityType)
            {
                #region New By Hung
                case 15://Canival
                    switch(gmActive.activityChildType)
                    {
                        case 1://vua tăng cấp
                            return new KingOfGrade(gmActive, listUsersGmInfo);
                        case 2://vua cường hóa
                            return new KingOfStrengthen(gmActive, listUsersGmInfo);
                        case 3://Vua hợp thành
                            return new KingOfCompose(gmActive, listUsersGmInfo);
                        case 4://vua chí tôn
                            return new KingOfPower(gmActive, listUsersGmInfo);
                        case 6://tiên phong vật tổ
                            return new CanivalTotem(gmActive, listUsersGmInfo);
                        case 7://vua tu luyện
                            return new CanivalTexp(gmActive, listUsersGmInfo);
                        case 8://vua thẻ bài
                            return new CanivalCard(gmActive, listUsersGmInfo);
                        case 9://chiến hồn rực rỡ
                            return new CanivalFigSpirit(gmActive, listUsersGmInfo);
                        case 11://tiến hóa pet
                            return new PetEvolution(gmActive, listUsersGmInfo);
                        default:
                            return new NullActivities(gmActive, listUsersGmInfo);
                    }

                case 232://BXH lực chiến
                    return new RankFightPower(gmActive, listUsersGmInfo);

                case 233://BXH cấp độ
                    return new RankGrade(gmActive, listUsersGmInfo);

                case 234://BXH trực tuyến
                    return new RankOnline(gmActive, listUsersGmInfo);

                case 235://BXH trận thắng
                    return new RankWin(gmActive, listUsersGmInfo);

                case 236:// BXH trận thắng Guild
                    return new RankGuildWin(gmActive, listUsersGmInfo);

                case 237:// BXH điểm hấp dẫn
                    return new GiftRank(gmActive, listUsersGmInfo);

                case 238:// BXH Tân Vương Lực Chiến
                    return new RankBigBugle(gmActive, listUsersGmInfo);

                case 239:// BXH Tân Vương Cấp Độ
                    return new RankLeagueWin(gmActive, listUsersGmInfo);
                #endregion
                case 0: // tich luy nap the
                    return new AccumRechargeMoney(gmActive, listUsersGmInfo);

                case 1: // tich luy tieu xu
                    return new AccumConsumeMoney(gmActive, listUsersGmInfo);

                case 2: // doi vat pham
                    return new ExchangeItem(gmActive, listUsersGmInfo);
 

                case 5: // dang nhap nhan thuong
                    return new DirectReceive(gmActive, listUsersGmInfo);

                case 12: // bxh tieu xu
                    return new RankConsumeMoney(gmActive, listUsersGmInfo);
                case 16:
                    return new WholePeopleVipActItem(gmActive, listUsersGmInfo);

                case 28://su dung vat pham
                    return new UsingItems(gmActive, listUsersGmInfo);

                case 31: // sign activity
                    return new SignActivity(gmActive, listUsersGmInfo);

                case 60: // connRecharge
                    return new ConRecharge(gmActive, listUsersGmInfo);

                case 61: // bxh nap xu
                    return new RankRechargeMoney(gmActive, listUsersGmInfo);

                case 65: // DDTank Brasil - Copa da Recarga
                    return new RankFragment(gmActive, listUsersGmInfo);

                default:
                    return new NullActivities(gmActive, listUsersGmInfo);
            }
        }

        public static List<ItemInfo> CreateGmAwardsToItems(GmActivityInfo gmActivity, string giftId, bool sex)
        {
            return CreateGmAwardsToItems(gmActivity, giftId, sex, -1);
        }

        public static List<ItemInfo> CreateGmAwardsToItems(GmActivityInfo gmActivity, string giftId, bool sex = false, int rewardType = -1)
        {
            List<ItemInfo> itemList = new List<ItemInfo>();

            if (gmActivity.GiftsGroup.ContainsKey(giftId))
            {
                List<GmActiveRewardInfo> allRewards = gmActivity.GiftsGroup[giftId].RewardsList;

                foreach (GmActiveRewardInfo awn in allRewards)
                {
                    int sexCompare = sex ? 1 : 2;

                    if (awn.occupationOrSex != 0 && awn.occupationOrSex != sexCompare)
                        continue;

                    if (rewardType != -1 && awn.rewardType != rewardType)
                        continue;

                    itemList.Add(CloneFromGmReward(awn));
                }
            }

            return itemList;
        }


        private static ItemInfo CloneFromGmReward(GmActiveRewardInfo awn)
        {
            string[] props = awn.property.Split(',');

            ItemInfo tempItem = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(awn.templateId), awn.count, 103);
            tempItem.IsBinds = awn.isBind;
            tempItem.ValidDate = awn.validDate;

            tempItem.StrengthenLevel = int.Parse(props[0]);
            tempItem.AttackCompose = int.Parse(props[1]);
            tempItem.DefendCompose = int.Parse(props[2]);
            tempItem.AgilityCompose = int.Parse(props[3]);
            tempItem.LuckCompose = int.Parse(props[4]);

            /*if (tempItem.IsMagicStone())
            {
                tempItem.MagicAttack = int.Parse(props[6]);
                tempItem.MagicDefence = int.Parse(props[7]);
                tempItem.StrengthenExp = int.Parse(props[8]);
            }*/

            return tempItem;
        }

        public static GmGiftInfo[] LoadGmGiftDb()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                GmGiftInfo[] infos = pb.GetAllGmGift();
                return infos;
            }
        }

        public static GmActiveRewardInfo[] LoadAllGmActiveRewards()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                return pb.GetAllGmActiveReward();
            }
        }

        public static GmActiveConditionInfo[] LoadAllGmActiveConditions()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                return pb.GetAllGmActiveCondition();
            }
        }
        public static GmActivityInfo[] LoadGmActivityDb()
        {
            using (PlayerBussiness pb = new PlayerBussiness())
            {
                GmActivityInfo[] infos = pb.GetAllGmActivity();
                return infos;
            }
        }

        #region Actions Listener
        public static void ScanAction()
        {
            foreach (GmActivityInfo gmInfo in m_GmActivitys.Values)
            {
                if (gmInfo.IsAvalibleShow)
                    AddGMActivityAction(gmInfo, null);
            }
        }
        public static void DoAction()
        {
            List<IGMActive> tempRemoved = new List<IGMActive>();
            lock (m_listAction)
            {
                foreach (IGMActive active in m_listAction.Values)
                {
                    //if (active.Info.IsAvalible)
                    //if(active.Info.IsAvalibleShow && !IsLockAuto)
                        active.DoAction();

                    if (!active.Info.IsAvalibleShow)
                    {
                        tempRemoved.Add(active);
                    }
                }

                if (tempRemoved.Count > 0)
                {
                    foreach (IGMActive active in tempRemoved)
                    {
                        active.Stop();
                        m_listAction.Remove(active.Info.activityId);
                    }
                }
            }
        }

        // define listenner
        public delegate void PlayerEventHandle(GamePlayer player, int value);
        public delegate void PlayerEventLoginHandle(GamePlayer player, DateTime date);

        // setup listenner
        public static event PlayerEventHandle UpdateProperties;
        public static void OnUpdateProperties(GamePlayer player, int value)
        {
            UpdateProperties?.Invoke(player, value);
        }

        public static event PlayerEventHandle RechargeMoney;
        public static void OnRechargeMoney(GamePlayer player, int value)
        {
            RechargeMoney?.Invoke(player, value);
        }
        #region Add By Hung
        public static event PlayerEventHandle Level;
        public static event PlayerEventHandle Power;
        public static event PlayerEventHandle Evolution;
        public static event PlayerEventHandle StrenghUp;
        public static event PlayerEventHandle ComposeUp;
        public static event PlayerEventHandle TotemUp;
        public static event PlayerEventHandle TexpUp;
        public static event PlayerEventHandle CardUp;
        public static event PlayerEventHandle FigSpiritUp;
        public static void OnFigSpiritUp(GamePlayer player, int value)
        {
            FigSpiritUp?.Invoke(player, value);
        }
        public static void OnCardUp(GamePlayer player, int value)
        {
            CardUp?.Invoke(player, value);
        }
        public static void OnTexpUp(GamePlayer player, int value)
        {
            TexpUp?.Invoke(player, value);
        }
        public static void OnTotemUp(GamePlayer player, int value)
        {
            TotemUp?.Invoke(player, value);
        }
        public static void OnComposeUp(GamePlayer player, int value)
        {
            ComposeUp?.Invoke(player, value);
        }
        public static void OnStrenghUp(GamePlayer player, int value)
        {
            StrenghUp?.Invoke(player, value);
        }
        public static void OnLevel(GamePlayer player, int value)
        {
            Level?.Invoke(player, value);
        }
        public static void OnPower(GamePlayer player, int value)
        {
            Power?.Invoke(player, value);
        }
        public static void OnEvolution(GamePlayer player, int value)
        {
            Evolution?.Invoke(player, value);
        }
        #endregion

        public static event PlayerEventHandle ConsumeMoney;
        public static void OnConsumeMoney(GamePlayer player, int value)
        {
            ConsumeMoney?.Invoke(player, value);
        }

        public static event PlayerEventHandle ConsumeGift;
        public static void OnConsumeGift(GamePlayer player, int value)
        {
            ConsumeGift?.Invoke(player, value);
        }

        public static event PlayerEventLoginHandle PlayerLogin;
        public static void OnPlayerLogin(GamePlayer player, DateTime date)
        {
            PlayerLogin?.Invoke(player, date);
        }
        public static event PlayerEventHandle PlayerVIPLevel;
        public static void OnPlayerUpgradeVIP(GamePlayer player, int VIPLevel)
        {
            PlayerVIPLevel?.Invoke(player, VIPLevel);
        }

        public static event PlayerEventLoginHandle PlayerNewDay;
        public static void OnPlayerNewDay(GamePlayer player, DateTime date)
        {
            PlayerNewDay?.Invoke(player, date);
        }

        public delegate void PlayerRankEventHandle(GamePlayer player, int value1, int value2);
        public static event PlayerRankEventHandle UsingItems;
        public static void OnUsingItemsEvent(GamePlayer player, int value1, int value2)
        {
            UsingItems?.Invoke(player, value1, value2);
        }
        //2021.09
        public static event PlayerEventHandle UpdateFightPower;
        public static event PlayerEventHandle UpdateGrade;
        public static void OnUpdateGrade(GamePlayer player, int value)
        {
            UpdateGrade?.Invoke(player, value);
        }
        public static void OnUpdateFightPower(GamePlayer player, int value)
        {
            UpdateFightPower?.Invoke(player, value);
        }
        public static event PlayerEventHandle UpdateExp;
        public static void OnUpdateExp(GamePlayer player, int value)
        {
            UpdateExp?.Invoke(player, value);
        }
        public static event PlayerEventHandle UpdateOnline;
        public static void OnUpdateOnline(GamePlayer player, int value)
        {
            UpdateOnline?.Invoke(player, value);
        }
        public static event PlayerEventHandle UpdateUseBigBugle;
        public static void OnUpdateUseBigBugle(GamePlayer player, int value)
        {
            UpdateUseBigBugle?.Invoke(player, value);
        }
        public static event PlayerEventHandle UpdateGvG;
        public static void OnUpdateGvG(GamePlayer player, int value)
        {
            UpdateGvG?.Invoke(player, value);
        }
        public static event PlayerEventHandle UpdateWin;
        public static void OnUpdateWin(GamePlayer player, int value)
        {
            UpdateWin?.Invoke(player, value);
        }

        public static event PlayerEventHandle UpdateWinGuild;
        public static void OnUpdateWinGuild(GamePlayer player, int value)
        {
            UpdateWinGuild?.Invoke(player, value);
        }

        public static event PlayerEventHandle UpdateLeaguePoint;
        public static void OnUpdateLeaguePoint(GamePlayer player, int value)
        {
            UpdateLeaguePoint?.Invoke(player, value);
        }
        #endregion

        public static void Stop()
        {
            SaveData();

            lock (m_listAction)
            {
                foreach (IGMActive action in m_listAction.Values)
                {
                    action.Stop();
                }

                m_listAction.Clear();
            }
        }
    }
}