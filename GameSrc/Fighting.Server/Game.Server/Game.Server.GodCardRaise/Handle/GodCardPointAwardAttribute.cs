using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle
{
    [GodCardRaiseHandleAttbute((byte)GodCardRaisePackageType.POINT_AWARD_ATTRIBUTE)]
    public class GodCardPointAwardAttribute : IGodCardRaiseCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int id = packet.ReadInt();
            GodCardPointRewardInfo reward = GodCardPointRewardMgr.FindGodCardPointReward(id);
            if (reward != null)
            {
                var itemnew = ItemMgr.FindItemTemplate(reward.ItemID);
                if (itemnew != null)
                {
                    ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemnew, 1, 102);
                    if (itemInfo != null)
                    {
                        itemInfo.Count = reward.Count;
                        itemInfo.IsBinds = reward.IsBind;
                        itemInfo.ValidDate = reward.Valid;
                        player.SendItemsToMail(itemInfo, $"Quà tặng từ mốc tích lũy {reward.Point} điểm", "Đổi Điểm Bói Thẻ", eMailType.BuyItem);
                        //player.AddItem(itemInfo);
                        player.Actives.ListAwards.Add(id);
                    }
                }
            }

            List<int> infos = player.Actives.ListAwards;
            GSPacketIn pkg = new GSPacketIn((short)ePackageType.SAN_XIAO);
            pkg.WriteByte((byte)GodCardRaisePackageType.AWARD_INFO);
            pkg.WriteInt(infos.Count);
            player.SendMessage("Nhận thành công vật phẩm gửi ra thư.");
            for (int i = 0; i < infos.Count; i++)
            {
                pkg.WriteInt(infos[i]);
            }

            player.Out.SendTCP(pkg);
            return 1;
        }
    }
}