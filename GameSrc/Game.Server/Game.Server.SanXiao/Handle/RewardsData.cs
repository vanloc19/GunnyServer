using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.REWARDS_DATA)]
    public class RewardsData : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            List<int> awardGets = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.SXRewardsGet);
            if (awardGets != null)
            {
                player.Actives.SendSXRewardsData(awardGets);
            }

            return 1;
        }
    }
}