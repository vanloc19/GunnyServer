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
    public class RankExp : IGMActive
    {
        public RankExp(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
            //TO DO
        }

        public override void Start()
        {
            GmActivityMgr.UpdateExp += GmActivityMgr_UpdateProperties;
        }

        private void GmActivityMgr_UpdateProperties(GamePlayer player, int value)
        {
            UserGmActivityInfo userinfo = GetPlayer(player);

            userinfo.NickName = player.PlayerCharacter.NickName;
            userinfo.VipLevel = player.PlayerCharacter.VIPLevel;
            

            if (userinfo.Value < value)
            {
                userinfo.Value = value;
            }
        }

        public override void Stop()
        {
            GmActivityMgr.UpdateExp -= GmActivityMgr_UpdateProperties;
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

                StringBuilder listTop = new StringBuilder();
                int rank = 1;
                listTop.AppendLine($"List TOP 10 Copa da Experiência ");
                foreach (UserGmActivityInfo r in rankLists)
                {
                    listTop.AppendLine($"Rank {rank}: {r.NickName} Experiência:{r.Value}");
                    rank++;
                }

                foreach (GmGiftInfo giftPackage in allGiftPackage)
                {
                    Dictionary<int, GmActiveConditionInfo> conditions = giftPackage.Conditions;

                    if (rankLists.Count >= conditions[0].conditionValue)
                    {
                        UserGmActivityInfo userRank = rankLists[conditions[0].conditionValue - 1];
                        List<ItemInfo> items = GmActivityMgr.CreateGmAwardsToItems(m_gmActive, giftPackage.giftbagId);
                       
                        WorldEventMgr.SendItemsToMails(items, userRank.UserID, userRank.NickName, GameServer.Instance.Configuration.ZoneId, null, 
                        LanguageMgr.GetTranslation("Copa da Recarga - Premiações recebidas!"), LanguageMgr.GetTranslation("Aqui está o conteúdo dos itens. Parabéns por ficar no ranking!", m_gmActive.activityName));

                    }
                }
            }
        }
    }
}
