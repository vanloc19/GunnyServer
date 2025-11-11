using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server;
using Game.Server.GameUtils;
using Game.Server.GMActives;
using Game.Server.Managers;
using Game.Server.Packets.Client;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using Game.Server.Packets;

[PacketHandler(216, "卡牌系统")]
internal class CardDataHandler : IPacketHandler
{
    public static Random random = new Random();
    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
        //client.Out.SendMessage(eMessageType.Normal, "Tính năng bị khóa!");
        //return 0;
        int num = packet.ReadInt();
        int num2 = 0;
        int num3 = 0;
        CardInventory cardBag = client.Player.CardBag;
        ItemInfo itemInfo = null;
        List<ItemInfo> iteminfos = new List<ItemInfo>();
        cardBag.BeginChanges();
        switch (num)
        {
            case 0://移动卡牌
                num2 = packet.ReadInt();
                num3 = packet.ReadInt();
                if (num2 == num3 && num2 >= 5)
                {
                    return 0;
                }
                if ((num2 < 5 && num3 >= 5) || (num2 == num3 && num2 < 5))
                {
                    cardBag.RemoveCardAt(num2);
                    client.Player.EquipBag.UpdatePlayerProperties();
                }
                else if (num2 >= 5 && num3 < 5)
                {
                    UsersCardInfo itemAt2 = cardBag.GetItemAt(num2);
                    if (itemAt2 != null)
                    {
                        if (!cardBag.IsCardEquip(itemAt2.TemplateID))
                        {
                            cardBag.RemoveCardAt(num3);
                            UsersCardInfo userCardInfo = itemAt2.Clone();
                            userCardInfo.Count = 0;
                            cardBag.AddCardTo(userCardInfo, num3);
                            client.Player.CardBag.SaveToDatabase();
                            client.Player.EquipBag.UpdatePlayerProperties();
                            var CardOne = cardBag.GetItemAt(1);
                            var CardTwo = cardBag.GetItemAt(2);
                            var CardThree = cardBag.GetItemAt(0);
                            if (CardOne != null && CardTwo != null && CardThree != null)
                            {
                                var KingOfCard = new int[] { CardOne.Level, CardTwo.Level, CardThree.Level };
                                var min = KingOfCard[0];
                                for (int i = 0; i < KingOfCard.Length; i++)
                                {
                                    if(KingOfCard[i] < min)
                                    {
                                        min = KingOfCard[i];
                                    }
                                }
                                GmActivityMgr.OnCardUp(client.Player, min);
                            }

                        }
                        else
                        {
                            client.Player.SendMessage("Thẻ bài này đã được trang bị.");
                        }
                    }
                }
                else
                {
                    cardBag.MoveCard(num2, num3);
                }
                break;
            case 1://开启卡牌位置
                {
                    int slot = packet.ReadInt();
                    UsersCardInfo card;
                    card = new UsersCardInfo();
                    card.Count = -1;
                    card.UserID = client.Player.PlayerCharacter.ID;
                    card.Place = slot;
                    card.TemplateID = 314101;
                    card.isFirstGet = true;
                    card.Damage = 0;
                    card.Guard = 0;
                    card.Attack = 0;
                    card.Defence = 0;
                    card.Luck = 0;
                    card.Agility = 0;
                    client.Player.CardBag.AddCardTo(card, slot);
                    break;
                }
            case 2://开启卡盒
                {
                    int slot = packet.ReadInt();
                    int num4 = packet.ReadInt();
                    itemInfo = client.Player.EquipBag.GetItemAt(slot);
                    if (itemInfo != null)
                    {
                        if (num4 <= 0 || num4 > itemInfo.Count)
                        {
                            client.Player.SendMessage("Thẻ bài không tồn tại.");
                            return 0;
                        }
                        int property = itemInfo.Template.Property5;
                        ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(property);
                        if (itemTemplateInfo == null || itemTemplateInfo.CategoryID != 26)
                        {
                            client.Player.SendMessage("Thẻ bài không tồn tại.");
                            return 0;
                        }
                        client.Player.EquipBag.RemoveCountFromStack(itemInfo, itemInfo.Count);
                        int num5 = itemInfo.Count;
                        Random random2 = new Random();
                        for (int i = 0; i < itemInfo.Count; i++)
                        {
                            num5 += random2.Next(1, 3);
                        }
                        cardBag.AddCard(property, num5);
                    }
                    else
                    {
                        client.Player.SendMessage("Thẻ bài không tồn tại.");
                    }
                    break;
                }
            case 3://卡牌升级
                {
                    num2 = packet.ReadInt();
                    if (num2 < 5)
                    {
                        return 0;
                    }
                    UsersCardInfo itemAt = cardBag.GetItemAt(num2);
                    if (itemAt == null)
                    {
                        break;
                    }
                    if (itemAt.Level < CardMgr.MaxLevel())
                    {
                        CardUpdateConditionInfo cardUpdateCondition = CardMgr.GetCardUpdateCondition(itemAt.Level + 1);
                        if (cardUpdateCondition != null && itemAt.Count >= cardUpdateCondition.UpdateCardCount)
                        {
                            Random random = new Random();
                            itemAt.Count -= cardUpdateCondition.UpdateCardCount;
                            itemAt.CardGP += random.Next(cardUpdateCondition.MinExp, cardUpdateCondition.MaxExp);
                            if (itemAt.CardGP >= cardUpdateCondition.Exp)
                            {
                                CardUpdateInfo cardUpdateInfo = CardMgr.GetCardUpdateInfo(itemAt.TemplateID, cardUpdateCondition.Level);
                                if (cardUpdateInfo != null)
                                {
                                    itemAt.Level++;
                                    itemAt.Attack += cardUpdateInfo.Attack;
                                    itemAt.Defence += cardUpdateInfo.Defend;
                                    itemAt.Agility += cardUpdateInfo.Agility;
                                    itemAt.Luck += cardUpdateInfo.Lucky;
                                    itemAt.Damage += cardUpdateInfo.Damage;
                                    itemAt.Guard += cardUpdateInfo.Guard;
                                    UsersCardInfo cardEquip = cardBag.GetCardEquip(itemAt.TemplateID);
                                    if (cardEquip != null)
                                    {
                                        cardEquip.CopyProp(itemAt);
                                        cardBag.UpdateCard(cardEquip);
                                        client.Player.EquipBag.UpdatePlayerProperties();
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("cardUpInfo is null - " + itemAt.TemplateID + " - " + cardUpdateCondition.Level + " - exp: " + cardUpdateCondition.Exp);
                                }
                            }
                            cardBag.UpdateCard(itemAt);
                        }
                        else
                        {
                            client.Player.SendMessage("Bạn không có đủ số lượng thẻ bài để tăng cấp.");
                        }
                    }
                    else
                    {
                        client.Player.SendMessage("Thẻ của bạn đã đạt cấp cao nhất không thể tiếp tục.");
                    }
                    break;
                }
            case 4://整理卡牌背包
                packet.ReadInt();//slot
                packet.ReadInt();//count
                //packet.ReadInt();
                //num2 = packet.ReadInt();
                //num3 = packet.ReadInt();
                break;
        }
        cardBag.CommitChanges();
        return 0;
    }
}
