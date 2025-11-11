using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Bussiness;
using Game.Base.Packets;
using Lsj.Util;
using Lsj.Util.Binary;

namespace Game.Server.GMActives
{
    public class ExchangeItem : IGMActive
    {
        public ExchangeItem(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            // get item exchange
            if (m_gmActive.GiftsGroup.ContainsKey(giftGroupId))
            {
                List<GmActiveRewardInfo> allItemNeedExchange = m_gmActive.GiftsGroup[giftGroupId].RewardsList.Where(a => a.rewardType == 0).ToList();

                if (allItemNeedExchange != null && allItemNeedExchange.Count > 0)
                {
                    List<GmActiveRewardInfo> itemsNeedEchange = new List<GmActiveRewardInfo>();
                    List<ItemInfo> itemsRemove = new List<ItemInfo>();
                    List<GmActiveRewardInfo> itemsRemove2 = new List<GmActiveRewardInfo>();

                    foreach (GmActiveRewardInfo itemRq in allItemNeedExchange)// get items match conditon
                    {
                            ItemInfo item = player.GetItemByTemplateID(itemRq.templateId);

                            itemsNeedEchange.Add(itemRq);
                            itemsRemove2.Add(new GmActiveRewardInfo() { templateId = itemRq.templateId, count = itemRq.count });
                        
                    }

                    foreach (GmActiveRewardInfo gmactiverewardinfo in itemsRemove2)
                    {
                        Console.WriteLine($"Name: {gmactiverewardinfo.templateId} {gmactiverewardinfo.count}");

                        player.RemoveTemplate(gmactiverewardinfo.templateId, gmactiverewardinfo.count);
                        // get awards
                    }
                    List<ItemInfo> allAwards = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftGroupId, player.PlayerCharacter.Sex, 1);

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
