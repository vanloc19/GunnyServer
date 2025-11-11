using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
    public class ActiveSystemMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Random rand;
        private static bool m_sendOpenToClient;
        private static bool m_sendCloseToClient;

        public static List<HappyRechargeReward> HappyRechargeRewards
        {
            get;
            set;
        }

        public static List<HappyRechargeTicketReward> HappyTicketRewards { get; set; }

        public static bool Init()
        {
            bool result;
            try
            {
                rand = new Random();
                m_sendOpenToClient = true;
                m_sendCloseToClient = true;
                using (ProduceBussiness pb = new ProduceBussiness())
                {
                    HappyTicketRewards = pb.GetRewardHappyTicket();
                    HappyRechargeRewards = pb.HappyRechargeReward();
                }
                result = true;
            }
            catch (Exception ex)
            {
                if (log.IsErrorEnabled)
                    log.Error("ActiveSystemMgr", ex);
                result = false;
            }
            return result;
        }

        public static List<ItemInfo> GetPyramidAward(int layer)
        {
            List<ItemInfo> infos = new List<ItemInfo>();
            List<ActivitySystemItemInfo> filtInfos = new List<ActivitySystemItemInfo>();
            List<ActivitySystemItemInfo> unFiltInfos = ActiveMgr.GetActivitySystemItemByLayer(layer);
            int dropItemCount = 1;
            int maxRound = ThreadSafeRandom.NextStatic(unFiltInfos.Select(s => s.Probability).Max());
            List<ActivitySystemItemInfo> roundInfos = unFiltInfos.Where(s => s.Probability >= maxRound).ToList();
            int maxItems = roundInfos.Count();
            if (maxItems > 0)
            {
                dropItemCount = dropItemCount > maxItems ? maxItems : dropItemCount;
                int[] randomArray = GetRandomUnrepeatArray(0, maxItems - 1, dropItemCount);
                foreach (int i in randomArray)
                {
                    ActivitySystemItemInfo item = roundInfos[i];
                    filtInfos.Add(item);
                }
            }

            foreach (ActivitySystemItemInfo info in filtInfos)
            {
                ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(info.TemplateID), info.Count, (int)eItemAddType.Buy);
                item.TemplateID = info.TemplateID;
                item.IsBinds = info.IsBind;
                item.ValidDate = info.ValidDate;
                item.Count = info.Count;
                item.StrengthenLevel = info.StrengthLevel;
                item.AttackCompose = 0;
                item.DefendCompose = 0;
                item.AgilityCompose = 0;
                item.LuckCompose = 0;
                infos.Add(item);
            }

            return infos;
        }

        public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
        {
            int j;
            int[] resultRound = new int[count];
            for (j = 0; j < count; j++)
            {
                int i = rand.Next(minValue, maxValue + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (resultRound[k] == i)
                    {
                        num = num + 1;
                    }
                }

                if (num == 0)
                {
                    resultRound[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }

            return resultRound;
        }
    }
}