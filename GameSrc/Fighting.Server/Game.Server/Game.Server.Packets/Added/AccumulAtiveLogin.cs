using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(338, "撤消征婚信息")]
    public class AccumulAtiveLoginAwardHandler : IPacketHandler
    {
        public int HandlePacket (GameClient client, GSPacketIn packet)
        {
            int fiveWeaponId = packet.ReadInt();
            GSPacketIn pkg = new GSPacketIn(338, client.Player.PlayerCharacter.ID);
            string msg = string.Format("Nhận quà đăng nhập thất bại!");
            if (client.Player.PlayerCharacter.accumulativeAwardDays < client.Player.PlayerCharacter.accumulativeLoginDays)
            {
                for (int i = client.Player.PlayerCharacter.accumulativeAwardDays; i < client.Player.PlayerCharacter.accumulativeLoginDays; i++)
                {
                    int days = i + 1;
                    List<ItemInfo> AccumulActiveLoginItems = new List<ItemInfo>();
                    if (days >= 7)
                    {
                        AccumulActiveLoginItems = AccumulActiveLoginMgr.GetSelecedAccumulAtiveLoginAward(fiveWeaponId);
                    }
                    else
                    {
                        AccumulActiveLoginItems = AccumulActiveLoginMgr.GetAllAccumulAtiveLoginAward(days);
                    }
                    if (AccumulActiveLoginItems.Count > 0)
                    {
                        msg = string.Format("Quà đăng nhập {0} ngày", days);
                        WorldEventMgr.SendItemsToMails(AccumulActiveLoginItems, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, null, msg);
                        GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(string.Format("|{0}| - Xin chúc mừng người chơi [{1}] tại <Đăng Nhập Tích Luỹ> nhận thưởng quà đăng nhập {2} ngày ", client.Player.ZoneName, client.Player.PlayerCharacter.NickName, days)));
                        client.Player.PlayerCharacter.accumulativeAwardDays++;
                    }
                    else
                    {
                        client.Player.SendMessage(msg);
                    }
                }
            }
            pkg.WriteInt(client.Player.PlayerCharacter.accumulativeLoginDays);
            pkg.WriteInt(client.Player.PlayerCharacter.accumulativeAwardDays);
            client.Player.SendTCP(pkg);
            return 0;
        }
    }
}