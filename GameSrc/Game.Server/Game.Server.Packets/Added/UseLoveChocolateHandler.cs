using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(330, "客户端日记")]
    public class UseLoveChocolateHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            if (client.Player.PlayerCharacter.IsMarried == false)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.UnMerry"));
                return 0;
            }

            eBageType bag = (eBageType)packet.ReadByte();
            int place = packet.ReadInt();
            int count = packet.ReadInt();
            ItemInfo item = client.Player.GetItemAt(bag, place);
            if (item == null)
            {
                client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.ItemNotFound"));
            }
            else
            {
                int totalItems = client.Player.PropBag.GetItemCount(item.TemplateID);
                if (totalItems < count || item.isLove() == false)
                {
                    client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.ItemNotEnough"));
                }
                else
                {
                    if (client.Player.Extra.CanRingExp(2))
                    {
                        int loveScore = item.Template.Property2 * count;
                        client.Player.PropBag.RemoveCountFromStack(item, count);
                        client.Player.AddLoveScore(loveScore, 2);
                        client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.UseSuccess",
                            item.Template.Name, loveScore));
                    }
                    else
                    {
                        client.Player.SendMessage(LanguageMgr.GetTranslation("UseLoveChocolateHandler.UseFail"));
                    }
                }
            }

            return 0;
        }
    }
}