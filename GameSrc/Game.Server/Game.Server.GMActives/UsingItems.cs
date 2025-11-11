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
    public class UsingItems : IGMActive
    {
        public UsingItems(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
        }
        private const int conditionIndex = 55;
        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            if (!m_gmActive.IsAvalible)
                return false;//event end

            //Console.WriteLine($"GetAward giftGroupId:{giftGroupId} value2:{index}");
            if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                UserGmActivityInfo userGm = GetPlayer(player);

                GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);

                // get condition
                Dictionary<int, GmActiveConditionInfo> allConditions = m_gmActive.GiftsGroup[giftGroupId].Conditions;
                if (!allConditions.ContainsKey(conditionIndex))
                {
                    Console.WriteLine($"conditionIndex:{conditionIndex} not found!");
                    return false;
                }
                int propId = allConditions[conditionIndex].conditionValue;
                if (!userGm.StatusList.ContainsKey(propId))
                {
                    Console.WriteLine($"propId:{propId} not found!");
                    return false;
                }
                if (userGm.StatusList[propId].StatusValue >= allConditions[conditionIndex].remain1)
                {
                    if (curInfo != null && curInfo.times > 0)// ready get reward
                        return false;

                    // save data
                    userGm.UpdateGiftReceive(giftGroupId, 1);

                    // get awards
                    List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, index);

                    if (allAwards.Count > 0)
                    {
                        WorldEventMgr.SendItemsToMails(allAwards, player.PlayerCharacter.ID, player.PlayerCharacter.NickName,
                                GameServer.Instance.Configuration.ZoneId, null,
                                LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"),
                                LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName));
                    }                   
                    return true;
                }
            }
            return false;
        }

        public override void SetUser(UserGmActivityInfo user)
        {
            if (!m_gmActive.IsAvalible)
                return;

            List<GmGiftInfo> allGiftPackage = m_gmActive.GiftsGroup.Values.ToList();

            foreach (GmGiftInfo giftPackage in allGiftPackage)
            {
                Dictionary<int, GmActiveConditionInfo> allConditions = giftPackage.Conditions;
                if (allConditions != null && allConditions.Count > 0)
                {
                    int propId = allConditions[conditionIndex].conditionValue;
                    if (!user.StatusList.ContainsKey(propId))
                    {
                        GMStatusInfo status = new GMStatusInfo
                        {
                            StatusID = propId,
                            StatusValue = 0
                        };
                        user.StatusList.Add(propId, status);
                    }
                }
            }
        }

        public override void Start()
        {
            GmActivityMgr.UsingItems += WonderfulActivityMgr_UsingItems;
        }

        private void WonderfulActivityMgr_UsingItems(GamePlayer player, int value1, int value2)
        {
            if (!m_gmActive.IsAvalible)
                return;

            List<GmGiftInfo> allGiftPackage = m_gmActive.GiftsGroup.Values.ToList();

            foreach (GmGiftInfo giftPackage in allGiftPackage)
            {
                // get condition
                Dictionary<int, GmActiveConditionInfo> allConditions = giftPackage.Conditions;
                if (allConditions != null && allConditions.Count > 0 &&  allConditions[conditionIndex].conditionValue == value1)
                {
                    GetPlayer(player).StatusList[value1].StatusValue += value2;
                }
                
            }            
        }

        public override void Stop()
        {
            GmActivityMgr.UsingItems -= WonderfulActivityMgr_UsingItems;
        }
    }
}
