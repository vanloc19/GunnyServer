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
    public class DirectReceive : IGMActive
    {
        public DirectReceive(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            if(m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                UserGmActivityInfo userGm = GetPlayer(player);

                GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);

                // get condition
                Dictionary<int, GmActiveConditionInfo> allConditions = m_gmActive.GiftsGroup[giftGroupId].Conditions;

                if (allConditions != null)
                {
                    if (curInfo != null && curInfo.times >= allConditions[0].conditionValue)
                        return false;

                    // save data
                    //SetGetAward(player, giftGroupId, 1);
                    userGm.UpdateGiftReceive(giftGroupId, 1);

                    // get awards
                    List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex);

                    player.SendItemsToMail(allAwards, LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName), LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"), Packets.eMailType.BuyItem);

                    return true;
                }
            }
            
            return false;
        }

        public override void SetUser(UserGmActivityInfo user)
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}
