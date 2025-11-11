using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.SanXiao.Handle
{
    [SanXiaoHandleAttbute((byte)SanXiaoPackageType.GAIN_PRISE)]
    public class GainPrise : ISanXiaoCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            ThreeClearPointAwardInfo award = AwardMgr.FindThreeClearPointAward(id);
            if (award != null && award.Point <= player.Actives.Info.SXScore && award.Type == 1)
            {
                List<int> awardGets = JsonConvert.DeserializeObject<List<int>>(player.Actives.Info.SXRewardsGet);
                if (awardGets == null)
                    awardGets = new List<int>();

                if (!awardGets.Contains(id))
                {
                    awardGets.Add(id);
                    player.Actives.Info.SXRewardsGet = JsonConvert.SerializeObject(awardGets);

                    ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(award.ItemID), award.Count, 105);
                    item.IsBinds = award.IsBind;
                    item.ValidDate = award.Valid;

                    player.AddTemplate(item);

                    player.Actives.SendSXRewardsData(awardGets);
                }
                else
                {
                    player.SendMessage(LanguageMgr.GetTranslation("SanXiao.Handle.GainPrise.Fail"));
                }
            }

            return 1;
        }
    }
}