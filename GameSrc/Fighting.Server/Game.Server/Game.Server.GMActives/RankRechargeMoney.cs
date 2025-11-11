using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Base.Packets;

namespace Game.Server.GMActives
{
    public class RankRechargeMoney : IGMActive
    {
        public RankRechargeMoney(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
            //TO DO
        }

        public override void Start()
        {
            GmActivityMgr.RechargeMoney += GmActivityMgr_RechargeMoney;
        }

        private void GmActivityMgr_RechargeMoney(GamePlayer player, int value)
        {
            UserGmActivityInfo userinfo = GetPlayer(player);

            userinfo.NickName = player.PlayerCharacter.NickName;
            userinfo.VipLevel = player.PlayerCharacter.VIPLevel;
            userinfo.Value += value;

            /*if (userinfo.StatusList.Count == 0)
            {
                GMStatusInfo status = new GMStatusInfo();
                status.StatusID = 0;
                status.StatusValue = value;
            }*/
        }

        public override void Stop()
        {
            GmActivityMgr.RechargeMoney -= GmActivityMgr_RechargeMoney;
        }

        public override void SetUser(UserGmActivityInfo user)
        {
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

                foreach (GmGiftInfo giftPackage in allGiftPackage)
                {
                    Dictionary<int, GmActiveConditionInfo> conditions = giftPackage.Conditions;

                    if (rankLists.Count >= conditions[0].conditionValue)
                    {
                        // co xep hang nay
                        UserGmActivityInfo userRank = rankLists[conditions[0].conditionValue - 1];
                        WorldEventMgr.SendItemsToMails(GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftPackage.giftbagId), userRank.UserID, userRank.NickName, GameServer.Instance.Configuration.ZoneId, null, LanguageMgr.GetTranslation("Ranking de Recarga - Premiações recebidas!"), LanguageMgr.GetTranslation("Aqui está o conteúdo dos itens. Parabéns por ficar no ranking!", m_gmActive.activityName));
                    }
                }
            }
        }
    }
}
