#region
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Game.Base.Packets;
//using Game.Server.Managers;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client
//{
//    [PacketHandler(377, "ExplorerManualHandler")]
//    public class ExplorerManualHandler : IPacketHandler
//    {
//        public int HandlePacket(GameClient client, GSPacketIn packet)
//        {
//            byte op = packet.ReadByte();
//            switch (op)
//            {
//                case 1:
//                    GSPacketIn op1 = new GSPacketIn(377);
//                    op1.WriteByte(1);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.manualLevel);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.progress);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.maxProgress);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.activesPage.Count());
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.conditionCount);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Agility);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Armor);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Attack);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Damage);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Defense);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.HP);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Lucky);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.MagicAttack);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.MagicResistance);
//                    op1.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.Stamina);
//                    List<PagesInfo> pages = client.Player.PlayerCharacter.explorerManualInfo.activesPage.Where(o => o.activate).ToList();
//                    op1.WriteInt(pages.Count);
//                    foreach (PagesInfo i in pages)
//                    {
//                        op1.WriteInt(i.pageID);
//                    }
//                    client.SendTCP(op1);
//                    break;
//                case 2:
//                    GSPacketIn op2 = new GSPacketIn(377);
//                    op2.WriteByte(2);
//                    int capitulo = packet.ReadInt();
//                    DebrisInfo[] debris = client.Player.PlayerCharacter.explorerManualInfo.debris.Values.Where(o => o.chapterID == capitulo).ToArray();
//                    int count = debris.Length;
//                    op2.WriteInt(count);
//                    foreach (DebrisInfo i in debris)
//                    {
//                        op2.WriteInt(i.ID);
//                        op2.WriteDateTime(i.date);
//                    }
//                    client.SendTCP(op2);
//                    break;
//                case 3:
//                    int tipoActivate = packet.ReadInt();
//                    int pageID = packet.ReadInt();
//                    bool param3 = packet.ReadBoolean();
//                    GSPacketIn op3 = new GSPacketIn(377);
//                    op3.WriteByte(3);
//                    JampsPageItemList page = JampsManualMgr.getPageFromID(pageID);
//                    if (tipoActivate == 2)
//                    {
//                        op3.WriteBoolean(client.Player.PlayerCharacter.Money >= page.ActivateCurrency);
//                        op3.WriteInt(tipoActivate);
//                        if (client.Player.PlayerCharacter.Money >= page.ActivateCurrency)
//                        {
//                            client.Player.RemoveMoney(page.ActivateCurrency);
//                        }
//                        else
//                        {
//                            break;
//                        }
//                    }
//                    else if (tipoActivate == 1)
//                    {
//                        op3.WriteBoolean(client.Player.PlayerCharacter.explorerManualInfo.acivatePage(pageID));
//                        op3.WriteInt(tipoActivate);
//                    }
//                    client.Player.EquipBag.UpdatePlayerProperties();
//                    client.SendTCP(op3);
//                    break;
//                case 4:
//                    bool autoBuy = packet.ReadBoolean();
//                    bool param22 = packet.ReadBoolean();
//                    bool autoEvolution = packet.ReadBoolean();
//                    int maxLevel = JampsManualMgr.maxLevel;
//                    JampsUpgradeItemList[] condictions = JampsManualMgr.getCondictionsByLevelAndType(client.Player.PlayerCharacter.explorerManualInfo.manualLevel, 1);
//                    ItemInfo consume = client.Player.PropBag.GetItemByTemplateID(0, condictions[0].Parameter1);
//                    while (true)
//                    {
//                        if (consume?.Count >= condictions[0].Parameter2)
//                        {
//                            client.Player.PropBag.BeginChanges();
//                            client.Player.PropBag.RemoveCountFromStack(consume, 1);
//                            client.Player.PropBag.CommitChanges();
//                            client.Player.PlayerCharacter.explorerManualInfo.progress += 1;
//                            if (client.Player.PlayerCharacter.explorerManualInfo.progress ==
//                                client.Player.PlayerCharacter.explorerManualInfo.manualLevel)
//                            {
//                                client.Player.PlayerCharacter.explorerManualInfo.progress = 0;
//                                client.Player.PlayerCharacter.explorerManualInfo.conditionCount = 0;
//                                client.Player.PlayerCharacter.explorerManualInfo.manualLevel += 1;
//                                if (client.Player.PlayerCharacter.explorerManualInfo.manualLevel > maxLevel)
//                                {
//                                    client.Player.PlayerCharacter.explorerManualInfo.manualLevel = maxLevel;
//                                }

//                                break;
//                            }

//                            if (!autoEvolution)
//                                break;
//                        }
//                        else
//                        {
//                            break;
//                        }
//                    }
//                    JampsManualItemList level = JampsManualMgr._jampsManualItemList[client.Player.PlayerCharacter.explorerManualInfo.manualLevel];
//                    bool flag = client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 6 || client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 11 || client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 16 || client.Player.PlayerCharacter.explorerManualInfo.manualLevel == 21;
//                    GSPacketIn op4 = new GSPacketIn(377);
//                    op4.WriteByte(4);
//                    op4.WriteBoolean(flag);
//                    op4.WriteBoolean(true);
//                    op4.WriteBoolean(level.MagicAttack > 0);
//                    op4.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.manualLevel);
//                    op4.WriteInt(client.Player.PlayerCharacter.explorerManualInfo.progress);
//                    client.Player.EquipBag.UpdatePlayerProperties();
//                    client.SendTCP(op4);
//                    break;
//                default:
//                    Console.WriteLine("ExplorerManualHanlder Op:" + op);
//                    break;
//            }

//            return 0;
//        }
//    }
//}
#endregion
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{
    [PacketHandler((short)ePackageType.EXPLORER_MANUAL, "场景用户离开")]
    public class ExplorerManualHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.ExplorerManual != null)
                client.Player.ExplorerManual.ProcessData(client.Player, packet);
            return 0;
        }
    }
}