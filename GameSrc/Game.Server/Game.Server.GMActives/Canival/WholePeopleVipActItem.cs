using Bussiness;
using Game.Server.GameObjects;
using Newtonsoft.Json.Linq;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Server.GMActives
{
    public class WholePeopleVipActItem : IGMActive
    {
        public WholePeopleVipActItem(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {

        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            Console.WriteLine($"giftGroupId = {giftGroupId}");
            if (!m_gmActive.IsAvalible)
                return false;

            if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                if (m_gmActive.GiftsGroup[giftGroupId].IsChoiceItem && index <= 0)
                {
                    return false;
                }
                UserGmActivityInfo userGm = GetPlayer(player);
                GiftCurInfo curInfo = userGm.GiftsReceivedList.SingleOrDefault(a => a.giftID == giftGroupId);
                Dictionary<int, GmActiveConditionInfo> allConditions = m_gmActive.GiftsGroup[giftGroupId].Conditions;

                if (allConditions != null && allConditions[0].conditionValue <= userGm.StatusList[0].StatusValue)
                {
                    if (curInfo != null && curInfo.times >= 1)
                    {
                        return false;
                    }
                    userGm.UpdateGiftReceive(giftGroupId, 1);
                    List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, index);
                    if (allAwards.Count > 0)
                    {
                        player.SendItemsToMail(allAwards, LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.ContentMail", m_gmActive.activityName), LanguageMgr.GetTranslation("WonderfulActivity.GetRewards.TitleMail"), Packets.eMailType.BuyItem);
                    }
                    return true;
                }
            }
            return false;
        }

        public override void SetUser(UserGmActivityInfo user)
        {
            if (!user.StatusList.ContainsKey(0))
            {
                Console.WriteLine("Added 0");
                GMStatusInfo status = new GMStatusInfo();
                status.StatusID = 0;
                status.StatusValue = 0;
                user.StatusList.Add(0, status);
            }
            if (!user.StatusList.ContainsKey(1))
            {
                Console.WriteLine("Added 1");
                GMStatusInfo status = new GMStatusInfo();
                status.StatusID = 1;
                var GetAllPlayerVIP = GetAllPlayers().Where(a => a.VipLevel > 0).ToList();
                Console.WriteLine($"Count = {GetAllPlayerVIP.Count}");
                status.StatusValue = GetAllPlayerVIP.Count;
                user.StatusList.Add(1, status);
            }
        }

        public override void Start()
        {
            GmActivityMgr.PlayerVIPLevel += VIP_Upgrade;
        }

        private void VIP_Upgrade(GamePlayer player, int value)
        {
            //Kết thúc Event
            if (!m_gmActive.IsAvalible)
                return;
            
            //Cập nhật cấp độ VIP
            GetPlayer(player).VipLevel = value;
            GetPlayer(player).StatusList[0].StatusValue = value;
            //Toàn server đạt {0} người đạt VIP tương ứng
            var GetAllPlayerVIP = GetAllPlayers().Where(a => a.VipLevel > 0).ToList();    
            GetPlayer(player).StatusList[1].StatusValue = GetAllPlayerVIP.Count;   

        }

        public override void Stop()
        {
            GmActivityMgr.PlayerVIPLevel -= VIP_Upgrade;

        }
    }
}