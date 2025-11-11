using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
namespace Game.Server.Packets.Client
{
    [PacketHandler(95, "客户端日记")]
    public class NecklaceStrengthHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
            //return 0;
            int num = (int)packet.ReadByte();
            int slot = packet.ReadInt();
            int num2 = packet.ReadInt();
            ItemInfo itemAt = client.Player.PropBag.GetItemAt(slot);
            if (itemAt.Count < num2)
            {
                num2 = itemAt.Count;
            }
            switch (num)
            {
                case 2:
                    {
                        int lv = 100;
                        int necklaceExpAdd = client.Player.PlayerCharacter.necklaceExpAdd;
                        int num3 = client.Player.PlayerCharacter.necklaceExp;
                        int necklaceMaxExp = 1317000000;  // Tăng exp tối đa cho level 100
                        if (num3 <= necklaceMaxExp)
                        {
                            if (itemAt != null && itemAt.TemplateID == 11160 && itemAt.Count > 0 && num2 > 0)
                            {
                                int property = itemAt.Template.Property2;
                                num3 += property * num2;
                                int num4 = StrengthenMgr.GetNecklacePlus(num3, necklaceExpAdd);
                                if (num3 >= necklaceMaxExp)
                                {
                                    num4 = StrengthenMgr.GetNecklaceMaxPlus(lv);
                                    num2 -= (num3 - necklaceMaxExp) / property;
                                    client.Player.PlayerCharacter.necklaceExp = necklaceMaxExp;
                                }
                                else
                                {
                                    client.Player.PlayerCharacter.necklaceExp = num3;
                                }
                                if (num4 > necklaceExpAdd)
                                {
                                    client.Player.PlayerCharacter.necklaceExpAdd = num4;
                                    client.Player.EquipBag.UpdatePlayerProperties();
                                }
                                client.Player.RemoveTemplate(itemAt.TemplateID, num2);
                            }
                        }
                        else
                        {
                            client.Player.SendMessage("Đã đạt cấp tối đa!");
                        }
                        break;
                    }
            }
            client.Player.Out.SendNecklaceStrength(client.Player.PlayerCharacter);
            return 0;
        }
    }
}
