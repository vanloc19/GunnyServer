using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Bussiness;

namespace Game.Server.GMActives
{
    public class AccumConsumeMoney : IGMActive
    {
        public AccumConsumeMoney(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                if (m_gmActive.GiftsGroup[giftGroupId].IsChoiceItem && index <= 0)
                    return false;

                UserGmActivityInfo userGm = GetPlayer(player);

                GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);

                // get condition
                Dictionary<int, GmActiveConditionInfo> allConditions = m_gmActive.GiftsGroup[giftGroupId].Conditions;

                if (allConditions != null && allConditions[0].conditionValue <= userGm.StatusList[0].StatusValue)
                {
                    int maxCanGet = allConditions[2].conditionValue == 0 ? userGm.StatusList[0].StatusValue / allConditions[0].conditionValue : 1;

                    if (curInfo != null && curInfo.times >= maxCanGet)
                        return false;

                    // save data
                    userGm.UpdateGiftReceive(giftGroupId, 1);

                    // get awards
                    List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, index);
                    
                    if(allAwards.Count > 0)
                        player.SendItemsToMail(allAwards, LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName), LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"), Packets.eMailType.BuyItem);

                    return true;
                }
            }

            return false;
        }

        public override void SetUser(UserGmActivityInfo user)
        {
            if (!user.StatusList.ContainsKey(0))
            {
                GMStatusInfo status = new GMStatusInfo();
                status.StatusID = 0;
                status.StatusValue = 0;
                user.StatusList.Add(0, status);
            }
        }

        public override void Start()
        {
            GmActivityMgr.ConsumeMoney += GmActivityMgr_RechargeMoney;
        }

        private void GmActivityMgr_RechargeMoney(GamePlayer player, int value)
        {
            GetPlayer(player).StatusList[0].StatusValue += value;
        }

        public override void Stop()
        {
            GmActivityMgr.ConsumeMoney -= GmActivityMgr_RechargeMoney;
        }

    }
}
