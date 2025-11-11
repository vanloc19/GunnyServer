using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(204, "打开物品")]
	public class OpenAllCardBoxHandler : IPacketHandler
	{
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            PlayerInventory caddyBag = client.Player.CaddyBag;
            CardInventory cardBag = client.Player.CardBag;
            int num = 0;
            for (int i = 0; i < caddyBag.Capalility; i++)
            {
                ItemInfo itemAt = caddyBag.GetItemAt(i);
                bool flag = (itemAt != null);
                if (flag)
                {
                    caddyBag.RemoveItem(itemAt);
                    Random random = new Random();
                    int property = itemAt.Template.Property5;
                    num = random.Next(1, 3);
                    cardBag.AddCard(property, num);
                    client.Player.SendMessage("Bạn vừa mở " + itemAt.Template.Name + " và nhận được " + num.ToString() + " thẻ bài.");
                }
            }
            return 1;
        }
        #region Old Code
        //public int HandlePacket(GameClient client, GSPacketIn packet)
        //{
        //	ItemInfo info = client.Player.GetItemByTemplateID(112150);
        //	List<ItemInfo> itemInfos = new List<ItemInfo>();
        //	int gold = 0;
        //	int giftToken = 0;
        //	int point = 0;
        //	int exp = 0;
        //	for (int i = 0; i < info.Count; i++)
        //	{
        //		ItemBoxMgr.CreateItemBox(info.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref exp);
        //	}
        //	client.Player.PropBag.RemoveItem(info);
        //	if (client.Player.SendItemsToMail(itemInfos, "Ao abrir todos os cartões de uma vez, você recebeu na sua caixa de correio.", "Caixa de cartões misteriosos", eMailType.Default))
        //	{
        //		client.Player.SendMessage("As recompensas foram enviadas para a sua caixa de correio");
        //	}
        //	return 1;
        #endregion
    }
}
