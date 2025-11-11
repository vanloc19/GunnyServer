using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(133, "场景用户离开")]
    public class LatentEnergyHandler : IPacketHandler
    {
        public static Random random = new Random();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadByte();
            int BagType = packet.ReadInt();
            int Place = packet.ReadInt();
            int temBagType = -1;
            int temPlace = -1;
            ItemInfo equipCell = client.Player.GetItemAt((eBageType)BagType, Place);
            if (!equipCell.CanLatentEnergy())
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LatentEnergyHandler.Msg1"));
                return 0;
            }

            ItemInfo itemCell = null;
            PlayerInventory inventory = client.Player.GetInventory((eBageType)BagType);
            string msg = LanguageMgr.GetTranslation("LatentEnergyHandler.Msg2");
            GSPacketIn pkg = new GSPacketIn(133, client.Player.PlayerCharacter.ID);
            if (type == 1)
            {
                temBagType = packet.ReadInt();
                temPlace = packet.ReadInt();
                itemCell = client.Player.GetItemAt((eBageType)temBagType, temPlace);
                if (itemCell == null || itemCell.Count < 1)
                {
                    client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LatentEnergyHandler.Msg3"));
                    return 0;
                }

                int CurStr = int.Parse(equipCell.latentEnergyCurStr.Split(',')[0]);
                ItemTemplateInfo Temp = itemCell.Template;
                if (equipCell.IsValidLatentEnergy() || CurStr >= (Temp.Property3 - 5) || CurStr <= (Temp.Property2 - 5))
                {
                    equipCell.ResetLatentEnergy();
                }

                int curStr = random.Next(Temp.Property2, Temp.Property3);
                string tepmCurStr = curStr.ToString();
                for (int i = 1; i < 4; i++)
                {
                    curStr = random.Next(Temp.Property2, Temp.Property3);
                    tepmCurStr += "," + curStr.ToString();
                }

                if (equipCell.latentEnergyCurStr.Split(',')[0] == "0")
                {
                    equipCell.latentEnergyCurStr = tepmCurStr; // "1,1,1,1";
                }

                equipCell.latentEnergyNewStr = tepmCurStr;
                equipCell.latentEnergyEndTime = DateTime.Now.AddDays(7.0);
                PlayerInventory storeBag = client.Player.GetInventory((eBageType)temBagType);
                storeBag.RemoveCountFromStack(itemCell, 1);
            }
            else
            {
                equipCell.latentEnergyCurStr = equipCell.latentEnergyNewStr;
                msg = LanguageMgr.GetTranslation("LatentEnergyHandler.Msg4");
            }

            pkg.WriteInt(equipCell.Place); //_loc_3.place = _loc_2.readInt();
            pkg.WriteString(equipCell.latentEnergyCurStr); //_loc_3.curStr = _loc_2.readUTF();
            pkg.WriteString(equipCell.latentEnergyNewStr); //_loc_3.newStr = _loc_2.readUTF();
            pkg.WriteDateTime(equipCell.latentEnergyEndTime); //_loc_3.endTime = _loc_2.readDate();                
            equipCell.IsBinds = true;
            inventory.UpdateItem(equipCell);
            client.Player.EquipBag.UpdatePlayerProperties();
            client.Out.SendTCP(pkg);
            client.Player.SendMessage(msg);
            return 0;
        }
    }
}