using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler(258, "Novice Activity")]
    public class NoviceActivityGetAward : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadInt();
            int subID = packet.ReadInt();
            if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(1.5), DateTime.Now) > 0)
            {
                return 0;
            }
            bool isPlus = false;
            int awardGot = 0;
            string message = "Nhận quà từ sự kiện thành công!";
            ProduceBussiness produceBussiness = new ProduceBussiness();
            EventRewardProcessInfo eventProcess = client.Player.Extra.GetEventProcess(type);
            List<ItemInfo> items = new List<ItemInfo>();
            awardGot = ((subID == 1) ? 1 : (eventProcess.AwardGot * 2 + 1));
            switch (awardGot)
            {
                case 1:
                    subID = 1;
                    break;
                case 3:
                    subID = 2;
                    break;
                case 7:
                    subID = 3;
                    break;
                case 15:
                    subID = 4;
                    break;
                case 31:
                    subID = 5;
                    break;
                case 63:
                    subID = 6;
                    break;
                case 127:
                    subID = 7;
                    break;
                case 255:
                    subID = 8;
                    break;
                case 511:
                    subID = 9;
                    break;
            }
            EventRewardInfo[] eventsRewardInfo = produceBussiness.GetEventRewardInfoByType(type, subID);
            EventRewardGoodsInfo[] eventRewardGoodsByType = produceBussiness.GetEventRewardGoodsByType(type, subID);
            int CheckCondition = 0;
            Console.WriteLine($"Type:{type}, subID:{subID},Condiction{eventProcess.Conditions}");
            foreach (EventRewardInfo evtRewardInfo in eventsRewardInfo)
            {
                Console.WriteLine($"{evtRewardInfo.ActivityType} , {evtRewardInfo.Condition}");
                CheckCondition = evtRewardInfo.Condition;
            }
            //player < checkcondiction
            Console.WriteLine($"Tesing With player: {eventProcess.Conditions}, Game: {CheckCondition}");
            if (eventProcess.Conditions < CheckCondition)
            {
                Console.WriteLine($"{CheckCondition}...");
                client.Player.SendMessage("Bug ???");
                string NoticeOnline = string.Format($"{client.Player.PlayerCharacter.NickName} - BUG Phúc lợi còn BUG nữa là pay acc !!!");
                GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendOnlineNotice(NoticeOnline));
                return 0;
            }
            foreach (EventRewardGoodsInfo eventRewardGoods in eventRewardGoodsByType)
            {
                if (eventRewardGoods.TemplateId != -4300)
                {
                    ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(eventRewardGoods.TemplateId), 1, 104);
                    item.StrengthenLevel = eventRewardGoods.StrengthLevel;
                    item.AttackCompose = eventRewardGoods.AttackCompose;
                    item.DefendCompose = eventRewardGoods.DefendCompose;
                    item.AgilityCompose = eventRewardGoods.AgilityCompose;
                    item.LuckCompose = eventRewardGoods.LuckCompose;
                    item.IsBinds = eventRewardGoods.IsBind;
                    item.Count = eventRewardGoods.Count;
                    item.ValidDate = eventRewardGoods.ValidDate;
                    items.Add(item);
                }
                /*else
                {
                    client.Player.Actives.Info.GoldMoney += eventRewardGoods.Count;
                    client.Player.SendMessage($"Bạn nhận được {eventRewardGoods.Count} đậu vàng.");
                }*/
            }
            string noviceActivityName = client.Player.Extra.GetNoviceActivityName((NoviceActiveType)type);
            EventRewardInfo[] array = eventsRewardInfo;
            foreach (EventRewardInfo eventRewardInfo in array)
            {
                Console.WriteLine($"Checking: {client.Player.PlayerCharacter.awardGot} = {awardGot}");
                if (awardGot != 999)
                {
                    if (awardGot > client.Player.PlayerCharacter.awardGot)
                    {
                        client.Player.PlayerCharacter.awardGot = awardGot;
                        client.Player.Extra.UpdateEventCondition(type, eventRewardInfo.Condition, isPlus, awardGot);
                        client.Player.SendItemsToMail(items, string.Format("Đây là thư gửi tự động từ quà mở server! vui lòng không reply."), LanguageMgr.GetTranslation("Quà mở Server"), eMailType.Manage);
                    }
                    else
                    {
                        message = "Nhận thưởng thất bại.";
                        WorldMgr.SendSysNotice($"[{client.Player.PlayerCharacter.NickName}] Có hành vi bug tại phúc lợi khả năng cao là bay acc!!!");
                    }
                }
            }
            client.Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation(message));
            client.Player.LastOpenCard = DateTime.Now;
            return 1;
        }
    }
}