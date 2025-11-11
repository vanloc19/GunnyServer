using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness;

namespace Game.Server.GMActives
{
    public class ConRecharge : IGMActive
    {
        public ConRecharge(GmActivityInfo gmActive, List<UserGmActivityInfo> tempUserList) : base(gmActive, tempUserList)
        {
        }

        public override void SetUser(UserGmActivityInfo user)
        {
            GMStatusInfo status = null;

            if (!user.StatusList.ContainsKey(0))
            {
                status = new GMStatusInfo();
                status.StatusID = 0;
                status.StatusValue = 0;
                user.StatusList.Add(0, status);
            }

            // get current day

            int currId = GetCurrentDayID();

            if (!user.StatusList.ContainsKey(currId) && user.StatusList.Where(a => a.Value.StatusID != 0).Count() < 3)
            {
                status = new GMStatusInfo();
                status.StatusID = currId;
                status.StatusValue = 0;
                user.StatusList.Add(currId, status);
            }
        }

        public override void Start()
        {
            GmActivityMgr.RechargeMoney += GmActivityMgr_RechargeMoney;
        }

        private void GmActivityMgr_RechargeMoney(GamePlayer player, int value)
        {
            UserGmActivityInfo gmUsers = GetPlayer(player);
            SetUser(gmUsers);

            gmUsers.StatusList[0].StatusValue += value;

            // set recharge day
            int currId = GetCurrentDayID();
            gmUsers.StatusList[currId].StatusValue += value;

        }

        public override void Stop()
        {
            GmActivityMgr.RechargeMoney -= GmActivityMgr_RechargeMoney;
        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                if (m_gmActive.GiftsGroup[giftGroupId].IsChoiceItem && index <= 0)
                    return false;

                UserGmActivityInfo userGm = GetPlayer(player);

                GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);
                if (curInfo != null && curInfo.times >= 1)
                    return false;

                // get condition
                Dictionary<int, GmActiveConditionInfo> allConditions = m_gmActive.GiftsGroup[giftGroupId].Conditions;
                
                if (allConditions != null)
                {
                    // phan thuong theo ngay
                    List<GMStatusInfo> listStatus = userGm.StatusList.Values.OrderBy(a => a.StatusID).ToList();
                    bool isSuccess = false;

                    if (allConditions.ContainsKey(2) && allConditions[2].remain1 <= listStatus.Count && allConditions[2].conditionValue <= listStatus[allConditions[2].remain1].StatusValue)
                    {
                        // dat dk de nhan thuong theo ngay
                        isSuccess = true;
                    }
                    else if(allConditions.ContainsKey(1) && listStatus[0].StatusValue >= allConditions[1].conditionValue)
                    {
                        // phan thuong nhan 1 lan cho toan bo su kien
                        isSuccess = true;
                    }

                    if(isSuccess)
                    {
                        userGm.UpdateGiftReceive(giftGroupId, 1);

                        // get awards
                        List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, index);

                        if (allAwards.Count > 0)
                            player.SendItemsToMail(allAwards, LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName), LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"), Packets.eMailType.BuyItem);
                    }

                    return isSuccess;
                }
            }

            return false;
        }
        
        private int GetCurrentDayID()
        {
            DateTime now = DateTime.Now;
            return 10000 * now.Year + 100 * (now.Month + 1) + now.Day;
        }
    }
}
