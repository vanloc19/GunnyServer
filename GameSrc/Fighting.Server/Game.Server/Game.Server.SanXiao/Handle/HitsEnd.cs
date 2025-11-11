using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.HITS_END)]
    public class HitsEnd : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            if (player.Actives.Info.SXStepRemain > 0)
            {
                player.Actives.Info.SXStepRemain--;

                SpecialItemDataInfo specialValue = new SpecialItemDataInfo();

                List<ItemInfo> infos = new List<ItemInfo>();

                if (ItemBoxMgr.CreateItemBox(GameProperties.ThreeCleanAwardBox, infos, specialValue))
                {
                    if (specialValue.CrystalScore > 0)
                        player.Actives.Info.SXCrystal += specialValue.CrystalScore;

                    if (infos.Count > 0)
                    {
                        foreach (ItemInfo item in infos)
                        {
                            player.AddTemplate(item);
                        }

                        player.Actives.SendSXGainedDropItems(infos);
                    }
                }

                player.Actives.SendSXData();
            }

            return 1;
        }
    }
}