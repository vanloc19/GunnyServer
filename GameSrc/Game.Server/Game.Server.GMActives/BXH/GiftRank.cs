using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Bussiness.Managers;
using Bussiness;
using Game.Base.Packets;

namespace Game.Server.GMActives
{
    public class GiftRank : IGMActive
    {
        public GiftRank(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
        }

        public override void SetUser(UserGmActivityInfo user)
        {
        }

        public override void Start()
        {
            GmActivityMgr.ConsumeGift += GmActivityMgr_ConsumeGift;
        }

        private void GmActivityMgr_ConsumeGift(GamePlayer player, int value)
        {
            UserGmActivityInfo userinfo = GetPlayer(player);

            userinfo.NickName = player.PlayerCharacter.NickName;
            userinfo.VipLevel = player.PlayerCharacter.VIPLevel;
            userinfo.Value += value;
        }

        public override void Stop()
        {
            GmActivityMgr.ConsumeGift -= GmActivityMgr_ConsumeGift;
        }

        public override void DoAction()
        {
            if (!m_gmActive.IsAvalible && !IsLockAuto)
            {
                IsLockAuto = true;
                // check active is not sended
                List<GmGiftInfo> allGiftPackage = m_gmActive.GiftsGroup.Values.ToList();

                // get all rank
                List<UserGmActivityInfo> rankLists = GetRankTop();

                //Console.WriteLine("it working 1");
                foreach (GmGiftInfo giftPackage in allGiftPackage)
                {
                    Dictionary<int, GmActiveConditionInfo> conditions = giftPackage.Conditions;
                    //Console.WriteLine("it working 2");
                    if (rankLists.Count >= conditions[0].conditionValue)
                    {
                        // co xep hang nay
                        UserGmActivityInfo userRank = rankLists[conditions[0].conditionValue - 1];
                        WorldEventMgr.SendItemsToMails(GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftPackage.giftbagId), userRank.UserID, userRank.NickName, GameServer.Instance.Configuration.ZoneId, null, LanguageMgr.GetTranslation("Ranking de Consumo - Premiações recebidas!"), LanguageMgr.GetTranslation("Aqui está o conteúdo dos itens. Parabéns por ficar no ranking!", m_gmActive.activityName));
                        //Console.WriteLine($"Send Reward event '{m_gmActive.activityName}' for user '{userRank.NickName}' sucess!");
                    }
                }
            }
            else
            {
                //Console.WriteLine($"Event '{m_gmActive.activityName}' in progress!");
            }
        }
    }
}
