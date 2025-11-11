using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(265, "客户端日记")]
    public class NewTitleCardHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bag = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            string validDate = null;
            ItemInfo item = client.Player.GetItemAt(bag, place);
            if (item == null)
            {
                client.Player.SendMessage("Vật phẩm không tồn tại.");
            }
            else
            {
                NewTitleInfo title = NewTitleMgr.FindNewTitle(item.Template.Property1);
                if (title == null)
                {
                    client.Player.SendMessage("Danh hiệu chưa mở.");
                }
                else
                {
                    /*UserRankInfo nt = client.Player.Rank.GetSingleRank(title.Name);
                    if (nt != null && nt.IsValidRank())
                    {
                        client.Player.SendMessage("Danh hiệu đã tồn tại!");
                        return 0;
                    }*/
                    if (client.Player.RemoveCountFromStack(item, 1))
                    {
                        client.Player.Rank.AddNewRank(title.ID, item.ValidDate);
                        if (item.ValidDate == 0)
                            validDate = "Vĩnh viễn";
                        else
                            validDate = item.ValidDate.ToString();

                        client.Player.SendMessage(string.Format("bạn nhận được danh hiệu {0} - {1} ngày.", title.Name, validDate));
                        GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(string.Format("|{0}| - Xin chúc mừng người chơi [{1}] Vừa nhận được danh hiệu {2}", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, title.Name)));
                        client.Player.Rank.SaveToDatabase();
                        client.Player.Rank.SendUserRanks();
                        client.Player.EquipBag.UpdatePlayerProperties();
                    }
                    else
                    {
                        client.Player.SendMessage("Xử lý dữ liệu thất bại. Vui lòng thử lại sau.");
                    }
                }
            }

            return 0;
        }
    }
}