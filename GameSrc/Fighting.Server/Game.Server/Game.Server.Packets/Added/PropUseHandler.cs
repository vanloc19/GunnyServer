using System;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((int)ePackageType.PROP_USE, "场景用户离开")]
    public class PropUseHandler : IPacketHandler
    {
        private Random rand = new Random();

        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            eBageType bagType = (eBageType)packet.ReadInt();
            int place = packet.ReadInt();
            PlayerInventory invent = client.Player.GetInventory(bagType);
            if (invent != null)
            {
                ItemInfo prop = invent.GetItemAt(place);
                {
                    if (prop != null)
                    {
                        int count = packet.ReadInt();
                        for (int i = 0; i < count; i++)
                        {
                            int templateid = packet.ReadInt();
                            {
                                switch (templateid)
                                {
                                    case 201316:
                                        UserChickActiveInfo chickInfo = client.Player.Actives.GetChickActiveData();
                                        if (chickInfo.IsKeyOpened == 0 && prop != null && prop.Count >= 1)
                                        {
                                            invent.RemoveCountFromStack(prop, 1);
                                            chickInfo.Active((client.Player.PlayerCharacter.Grade > 15) ? 2 : 1);
                                            client.Player.Actives.SaveChickActiveData(chickInfo);
                                            client.Player.SendMessage("Kích hoạt code gà hành thành công!");
                                        }
                                        else
                                        {
                                            client.Player.SendMessage("Kích hoạt code gà hành thất bại!");
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}