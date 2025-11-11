using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(345, "购买物品")]
    public class NewBuyGoodsHandler : IPacketHandler
    {
        //public static int countConnect = 0;
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            string translateId = "";
            StringBuilder stringBuilder2 = new StringBuilder();
            Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
            int qtd = packet.ReadInt();
            int goodsType = packet.ReadInt();
            int goodsId = packet.ReadInt();
            int buyType = packet.ReadInt();
            string colors = packet.ReadString();
            bool dress = packet.ReadBoolean();
            string skins = packet.ReadString();
            int place = packet.ReadInt();
            bool isBind = packet.ReadBoolean();
            int buyFrom = packet.ReadInt();
            ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(goodsId);
            if (qtd < 1 || qtd > 999)
            {
                client.Out.SendMessage(eMessageType.GM_NOTICE, "Lỗi dữ liệu vui lòng thử lại sau.");
                return 0;
            }
            Console.WriteLine($"Pkg = {qtd}, {goodsType}, {goodsId}, {buyType}, {colors}, {dress}, {skins}, {place}, {isBind}, {buyFrom}");
            if (place == -1)
            {
                ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID), 1, 104);
                Console.WriteLine($"{client.Player.PlayerCharacter.jampsCurrency} >= {shopItemInfoById.AValue1 * qtd * itemInfo.Count}");
                if (client.Player.PlayerCharacter.jampsCurrency >= shopItemInfoById.AValue1 * qtd * itemInfo.Count)
                {
                    
                    client.Player.AddJampsCurrency(-shopItemInfoById.AValue1 * qtd * itemInfo.Count);
                    itemInfo.Count *= qtd;
                    client.Player.AddTemplate(itemInfo);
                    translateId = "Mua thành công！";
                    GSPacketIn gSPacketIn = new GSPacketIn(44, client.Player.PlayerId);
                    client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation(translateId));
                    gSPacketIn.WriteInt(1);
                    gSPacketIn.WriteInt(3);
                    client.Player.SendTCP(gSPacketIn);
                }
                else
                {
                    GSPacketIn gSPacketIn = new GSPacketIn(44, client.Player.PlayerId);
                    gSPacketIn.WriteInt(1);
                    gSPacketIn.WriteInt(3);
                    client.Player.SendTCP(gSPacketIn);
                }
            }

            return 0;
        }
    }
}