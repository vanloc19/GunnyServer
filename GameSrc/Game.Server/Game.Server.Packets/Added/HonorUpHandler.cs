using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(96, "场景用户离开")]
    public class HonorUpHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadByte();
            bool isBland = packet.ReadBoolean();
            if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
            {
                client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
                return 0;
            }
            switch (type)
            {
                case 1:
                    if(!isBland && client.Player.PlayerCharacter.totemId > 0)
                    {
                        client.Player.Toemview = false;
                    }
                    break;
                case 2:
                    {
                        int maxBuyHonor = client.Player.PlayerCharacter.MaxBuyHonor + 1;
                        TotemHonorTemplateInfo temp = TotemHonorMgr.FindTotemHonorTemplateInfo(maxBuyHonor);
                        if (temp == null)
                        {
                            return 0;
                        }

                        int needMoney = temp.NeedMoney;
                        int addHonnor = temp.AddHonor;
                        if (client.Player.MoneyDirect(needMoney, false))
                        {
                            client.Player.AddHonor(addHonnor);
                            client.Player.AddMaxHonor(1);
                        }
                    }
                    break;
            }
            client.Player.Out.SendUpdateUpCount(client.Player.PlayerCharacter);
            return 0;
        }
    }
}