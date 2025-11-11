using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlDataProvider.Data;
using Game.Server.GameObjects;
using Bussiness;
using Bussiness.Helpers;

namespace Game.Server.GMActives
{
    public class SignActivity : IGMActive
    {
        public SignActivity(GmActivityInfo gmActive, List<UserGmActivityInfo> tempUserList) : base(gmActive, tempUserList)
        {
        }

        public override void SetUser(UserGmActivityInfo user)
        {
            GMStatusInfo status = null;

            int totalBig = m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(2)).Count();

            for (int i = 0; i < (14 + totalBig); i++)
            {
                if (!user.StatusList.ContainsKey(i))
                {
                    status = new GMStatusInfo();
                    status.StatusID = i;
                    status.StatusValue = 0;
                    user.StatusList.Add(i, status);
                }
            }
        }

        public override void Start()
        {
            GmActivityMgr.PlayerLogin += GmActivityMgr_PlayerLogin;
        }

        private void GmActivityMgr_PlayerLogin(GamePlayer player, DateTime date)
        {
            UserGmActivityInfo gmUsers = GetPlayer(player);
            SetUser(gmUsers);
            var CountDailyActive = gmUsers.GiftsReceivedList.Where(a => a.TotalCount == 1);

            int Daily = 0;
            foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(1)).OrderBy(a => a.Conditions[1].conditionValue))
            {
                GiftCurInfo userGiftInfo = gmUsers.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftInfo.giftbagId);
                if (userGiftInfo == null)
                {
                    if (gmUsers.StatusList[Daily].StatusValue == 0)
                    {
                        gmUsers.StatusList[Daily].StatusValue = 1;
                    }
                    userGiftInfo = new GiftCurInfo();
                    userGiftInfo.giftID = giftInfo.giftbagId;
                    userGiftInfo.lastTimeGet = date;
                    userGiftInfo.TotalCount = 1;
                    gmUsers.GiftsReceivedList.Add(userGiftInfo);
                    break;
                }
                else if (userGiftInfo.lastTimeGet.Date >= date.Date)
                {
                    break;
                }
                Daily++;
            }


            int Continous = 14;
            foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(2)).OrderBy(a => a.Conditions[2].conditionValue))
            {
                GiftCurInfo userGiftInfo = gmUsers.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftInfo.giftbagId);
                if (Convert.ToInt32(CountDailyActive.Count()) >= giftInfo.Conditions[2].conditionValue)
                {
                    if (userGiftInfo == null)
                    {
                        if (gmUsers.StatusList[Continous].StatusValue == 0)
                        {
                            gmUsers.StatusList[Continous].StatusValue = 1;
                        }
                        userGiftInfo = new GiftCurInfo();
                        userGiftInfo.giftID = giftInfo.giftbagId;
                        userGiftInfo.lastTimeGet = date;
                        userGiftInfo.TotalCount = 2;
                        gmUsers.GiftsReceivedList.Add(userGiftInfo);
                        break;
                    }
                    else if (userGiftInfo.lastTimeGet.Date >= date.Date)
                    {
                        break;
                    }
                    Continous++;
                }
            }
            #region
            // check day by day can get or not
            /*if (!m_userGmList.ContainsKey(player.PlayerId))
                AddPlayer(player);

            UserGmActivityInfo user = m_userGmList[player.PlayerId];


            int step = 0;
            foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(1)).OrderBy(a => a.Conditions[1].conditionValue))
            {
                GiftCurInfo userGiftInfo = user.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftInfo.giftbagId);



                if (userGiftInfo == null)
                {
                    if (user.StatusList[step].StatusValue == 0)
                    {
                        user.StatusList[step].StatusValue = 1;
                    }

                    userGiftInfo = new GiftCurInfo();
                    userGiftInfo.giftID = giftInfo.giftbagId;
                    userGiftInfo.lastTimeGet = date;
                    user.GiftsReceivedList.Add(userGiftInfo);
                    break;
                }
                else if (userGiftInfo.lastTimeGet.Date >= date.Date)
                {
                    // trong cung 1 ngay => break
                    break;
                }
                step++;
            }

            step = 14;
            foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(2)).OrderBy(a => a.Conditions[2].conditionValue))
            {
                if (user.GiftsReceivedList.Count >= giftInfo.Conditions[2].conditionValue && user.StatusList[step].StatusValue == 0)
                {
                    user.StatusList[step].StatusValue = 1;
                }
                step++;
            }*/
            #endregion
        }

        public override void Stop()
        {
            GmActivityMgr.PlayerLogin -= GmActivityMgr_PlayerLogin;
        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            for (int i = 0; i < 1; i++)
            {
                Console.WriteLine("GetAward");
                if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
                {
                    if (m_gmActive.GiftsGroup[giftGroupId].IsChoiceItem && index <= 0)
                        return false;

                    UserGmActivityInfo userGm = GetPlayer(player);

                    GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);

                    if (curInfo != null && curInfo.times >= 1)
                        return false;

                    Dictionary<int, GmActiveConditionInfo> allConditions = m_gmActive.GiftsGroup[giftGroupId].Conditions;
                    if (allConditions != null)
                    {
                        bool CanGetAward = false;
                        int Daily = 0;
                        int Continous = 14;

                        if (allConditions.ContainsKey(1) && curInfo != null)
                        {
                            foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(1)).OrderBy(a => a.Conditions[1].conditionValue))
                            {
                                if (giftInfo == m_gmActive.GiftsGroup[giftGroupId])
                                {
                                    if (userGm.StatusList[Daily].StatusValue == 1)
                                    {
                                        userGm.StatusList[Daily].StatusValue = 2;
                                        CanGetAward = true;
                                    }
                                    break;
                                }
                                Daily++;
                            }
                        }

                        else if (allConditions.ContainsKey(2) && curInfo != null)
                        {
                            foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(2)).OrderBy(a => a.Conditions[2].conditionValue))
                            {
                                if (giftInfo == m_gmActive.GiftsGroup[giftGroupId])
                                {
                                    if (userGm.StatusList[Continous].StatusValue == 1)
                                    {
                                        userGm.StatusList[Continous].StatusValue = 2;
                                        CanGetAward = true;
                                    }
                                    break;
                                }
                                Continous++;
                            }
                        }

                        if (CanGetAward)
                        {
                            userGm.UpdateGiftReceive(giftGroupId, 1);
                            List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, index);

                            if (allAwards.Count > 0)
                                player.SendItemsToMail(allAwards, LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName), LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"), Packets.eMailType.BuyItem);
                        }

                        return CanGetAward;
                    }
                }
            }
            return false;
            #region
            /*if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                if (m_gmActive.GiftsGroup[giftGroupId].IsChoiceItem && index <= 0)
                    return false;

                UserGmActivityInfo userGm = GetPlayer(player);

                GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);

                bool canGet = false;
                int step = 0;
                UserGmActivityInfo user = m_userGmList[player.PlayerId];

                if (m_gmActive.GiftsGroup[giftGroupId].Conditions.ContainsKey(1) && curInfo != null)
                {
                    // nhan moi ngay
                    foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(1)).OrderBy(a => a.Conditions[1].conditionValue))
                    {
                        if (giftInfo == m_gmActive.GiftsGroup[giftGroupId])
                        {
                            if (user.StatusList[step].StatusValue == 1)
                            {
                                user.StatusList[step].StatusValue = 2;
                                canGet = true;
                            }
                            break;
                        }
                        step++;
                    }
                }
                else if (m_gmActive.GiftsGroup[giftGroupId].Conditions.ContainsKey(2))
                {
                    step = 14;
                    foreach (GmGiftInfo giftInfo in m_gmActive.GiftsGroup.Values.Where(a => a.Conditions.ContainsKey(2)).OrderBy(a => a.Conditions[2].conditionValue))
                    {
                        if (giftInfo == m_gmActive.GiftsGroup[giftGroupId])
                        {
                            if (user.StatusList[step].StatusValue == 1)
                            {
                                user.StatusList[step].StatusValue = 2;
                                canGet = true;
                            }
                        }
                        step++;
                    }
                }

                if (canGet)
                {
                    userGm.UpdateGiftReceive(giftGroupId, 1);
                    List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, index);

                    if (allAwards.Count > 0)
                        player.SendItemsToMail(allAwards, LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName), LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"), Packets.eMailType.BuyItem);
                }

                return canGet;
            }*/
            #endregion
        }
    }
}
