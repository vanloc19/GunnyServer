using System;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.STORE_FINE_SUIT, "客户端日记")]
    public class FineStoreSuitHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            //client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
            //return 0;
            FineStorePackageType cmd = (FineStorePackageType)packet.ReadByte();
            switch (cmd)
            {
                case FineStorePackageType.FORGE_SUIT:
                    int count = packet.ReadInt();
                    int fineSuitExp = client.Player.PlayerCharacter.fineSuitExp;
                    bool result = false;
                    SetsBuildTempInfo useItem = SetsBuildTempMgr.FindNextSetsBuildExp(fineSuitExp);
                    if (useItem != null && fineSuitExp < SetsBuildTempMgr.SetsBuildMax())
                    {
                        ItemInfo item = client.Player.PropBag.GetItemByTemplateID(useItem.UseItemTemplate);
                        if (item != null && item.Count >= count)
                        {
                            if (count == 0)
                            {
                                count = item.Count;
                            }
                            int maxExp = useItem.Exp;
                            fineSuitExp += item.Template.Property2 * count;
                            if (fineSuitExp > maxExp)
                            {
                                int needExp = fineSuitExp - maxExp;
                                fineSuitExp = maxExp;
                                if (needExp >= item.Template.Property2)
                                {
                                    var count2 = needExp / item.Template.Property2;
                                    ItemInfo addBack = ItemInfo.CreateFromTemplate(item.Template, count2, 105);
                                    client.Player.AddTemplate(addBack);
                                }
                            }
                            result = client.Player.PropBag.RemoveTemplate(useItem.UseItemTemplate, count);
                        }
                        else
                        {
                            Console.WriteLine("FineStoreSuitHandler::item not found!");
                        }
                    }
                    GSPacketIn pkg = new GSPacketIn((int)ePackageType.STORE_FINE_SUIT);
                    pkg.WriteByte((byte)FineStorePackageType.FORGE_SUIT);
                    pkg.WriteBoolean(result);
                    pkg.WriteInt(fineSuitExp);
                    client.Player.SendTCP(pkg);
                    if (result)
                    {
                        client.Player.PlayerCharacter.fineSuitExp = fineSuitExp;
                        SetsBuildTempInfo newUseItem = SetsBuildTempMgr.FindNextSetsBuildExp(fineSuitExp);
                        if (newUseItem != null && useItem.Level < newUseItem.Level)
                        {
                            client.Player.EquipBag.UpdatePlayerProperties();
                        }
                    }
                    break;
                default:
                    Console.WriteLine("FineStoreSuitHandler cmd {0}, not found!", cmd);
                    break;
            }
            return 0;
        }
    }
}