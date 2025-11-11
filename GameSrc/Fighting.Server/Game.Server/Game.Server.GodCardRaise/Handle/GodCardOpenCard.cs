using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle
{
    [GodCardRaiseHandleAttbute((byte)GodCardRaisePackageType.OPEN_CARD)]
    public class GodCardOpenCard : IGodCardRaiseCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            int times = packet.ReadInt();
            bool isBlind = packet.ReadBoolean();
            GodCardInfo[] listGodCard = GodCardMgr.GetAllGodCard().ToArray();
            List<GodCardUser> listCards =
                JsonConvert.DeserializeObject<List<GodCardUser>>(player.Actives.Info.cardListCard);
            GSPacketIn gSPacketIn = new GSPacketIn((short)ePackageType.SAN_XIAO);
            gSPacketIn.WriteByte((byte)GodCardRaisePackageType.OPEN_CARD);
            if (times > 1)
            {
                int money = GameProperties.GodCardOpenFiveTimeMoney;
                if (player.MoneyDirect(money, IsAntiMult: false) && listGodCard != null && listGodCard.Length > 0)
                {
                    ThreadSafeRandom rand = new ThreadSafeRandom();
                    int rd = rand.Next(25);
                    player.Actives.Info.cardScore += 1000 * times;
                    gSPacketIn.WriteInt(times);
                    for (int i = 0; i < times; i++)
                    {
                        int id = 0;
                        if (rd < 3)
                        {
                            id = listGodCard[rand.Next(listGodCard.Length)].ID;
                        }
                        else
                        {
                            while (id < (listGodCard.Length / 2))
                            {
                                id = listGodCard[rand.Next(listGodCard.Length)].ID;
                            }
                        }
                        if (id == 1 || id == 5 || id == 6)
                            id = 34;

                        if (id >= 1 && id <= 5)
                        {
                            GodCardInfo item = GodCardMgr.FindGodCard(id);
                            WorldMgr.SendSysNotice(string.Format("《Bói Thẻ》- Người chơi {0} bói thẻ nhận được Thẻ Vàng [ {1} ]", player.PlayerCharacter.NickName, item.Name));
                        }
                        else if (id >= 7 && id <= 12)
                        {
                            GodCardInfo item = GodCardMgr.FindGodCard(id);
                            WorldMgr.SendSysNotice(string.Format("《Bói Thẻ》- Người chơi {0} bói thẻ nhận được Thẻ Tím [ {1} ]", player.PlayerCharacter.NickName, item.Name));
                        }

                        gSPacketIn.WriteInt(id);
                        player.Actives.SaveListCard(id, 1);
                    }
                    player.SendMessage($"Bói thẻ thành công.");
                }
                else
                {
                    gSPacketIn.WriteInt(0);
                }
            }
            else
            {
                if (player.Actives.Info.cardFreeCount < 3)
                {
                    if (listGodCard != null && listGodCard.Length > 0)
                    {
                        player.Actives.Info.cardFreeCount++;
                        ThreadSafeRandom rand = new ThreadSafeRandom();
                        int rd = rand.Next(25);
                        player.Actives.Info.cardScore += 1000 * times;
                        gSPacketIn.WriteInt(times);
                        for (int i = 0; i < times; i++)
                        {
                            int id = 0;
                            if (rd < 3)
                            {
                                id = listGodCard[rand.Next(listGodCard.Length)].ID;
                            }
                            else
                            {
                                while (id < (listGodCard.Length / 2))
                                {
                                    id = listGodCard[rand.Next(listGodCard.Length)].ID;
                                }
                            }

                            if (id == 1 || id == 5 || id == 6)
                                id = 34;

                            if (id >= 1 && id <= 5)
                            {
                                GodCardInfo item = GodCardMgr.FindGodCard(id);
                                WorldMgr.SendSysNotice(string.Format("《Bói Thẻ》- Người chơi {0} bói thẻ miễn phí nhận được Thẻ Vàng [ {1} ]", player.PlayerCharacter.NickName, item.Name));
                            }
                            else if (id >= 7 && id <= 12)
                            {
                                GodCardInfo item = GodCardMgr.FindGodCard(id);
                                WorldMgr.SendSysNotice(string.Format("《Bói Thẻ》- Người chơi {0} bói thẻ miễn phí nhận được Thẻ Tím [ {1} ]", player.PlayerCharacter.NickName, item.Name));
                            }

                            gSPacketIn.WriteInt(id);
                            player.Actives.SaveListCard(id, 1);
                        }
                        player.SendMessage($"Bói thẻ miễn phí thành công.");
                    }
                    else
                    {
                        gSPacketIn.WriteInt(0);
                    }
                }
                else
                {
                    int money = GameProperties.GodCardOpenOneTimeMoney;
                    if (player.MoneyDirect(money, IsAntiMult: false) && listGodCard != null && listGodCard.Length > 0)
                    {
                        ThreadSafeRandom rand = new ThreadSafeRandom();
                        int rd = rand.Next(25);
                        player.Actives.Info.cardScore += 1000 * times;
                        gSPacketIn.WriteInt(times);
                        for (int i = 0; i < times; i++)
                        {
                            int id = 0;
                            if (rd < 3)
                            {
                                id = listGodCard[rand.Next(listGodCard.Length)].ID;
                            }
                            else
                            {
                                while (id < (listGodCard.Length / 2))
                                {
                                    id = listGodCard[rand.Next(listGodCard.Length)].ID;
                                }
                            }

                            if (id == 1 || id == 5 || id == 6)
                                id = 34;

                            if (id >= 1 && id <= 5)
                            {
                                GodCardInfo item = GodCardMgr.FindGodCard(id);
                                WorldMgr.SendSysNotice(string.Format("《Bói Thẻ》- Người chơi {0} bói thẻ nhận được Thẻ Vàng [ {1} ]", player.PlayerCharacter.NickName, item.Name));
                            }
                            else if (id >= 7 && id <= 12)
                            {
                                GodCardInfo item = GodCardMgr.FindGodCard(id);
                                WorldMgr.SendSysNotice(string.Format("《Bói Thẻ》- Người chơi {0} bói thẻ nhận được Thẻ Tím [ {1} ]", player.PlayerCharacter.NickName, item.Name));
                            }

                            gSPacketIn.WriteInt(id);
                            player.Actives.SaveListCard(id, 1);
                        }
                        player.SendMessage($"Bói thẻ thành công.");
                    }
                    else
                    {
                        gSPacketIn.WriteInt(0);
                    }
                }
            }

            gSPacketIn.WriteInt(player.Actives.Info.cardScore);
            gSPacketIn.WriteInt(player.Actives.Info.cardFreeCount);
            player.Actives.Info.cardListCard = JsonConvert.SerializeObject(listCards);
            player.Out.SendTCP(gSPacketIn);
            return 1;
        }
    }
}