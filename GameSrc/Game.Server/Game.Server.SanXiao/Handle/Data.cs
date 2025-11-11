using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.DATA)]
    public class Data : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            player.Actives.SendSXData();

            List<int> awardGets = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.SXRewardsGet);
            if (awardGets != null)
                player.Actives.SendSXRewardsData(awardGets);

            Dictionary<int, int> storeBuys =
                JsonConvert.DeserializeObject<Dictionary<int, int>>(player.Actives.Info.MiniShopBuyCount);
            if (storeBuys != null)
                player.Actives.SendSXStoreData(storeBuys);

            return 1;
        }
    }
}