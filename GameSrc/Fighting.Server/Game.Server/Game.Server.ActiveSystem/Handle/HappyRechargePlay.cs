using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using System;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle
{
    //[ActiveSystem(176)]
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.HAPPYRECHARGE_PLAY)]
    class HappyRechargePlay : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn GSPacket = new GSPacketIn(145);
            GSPacket.WriteByte(176);
            bool IsLog = false;
            HappyRechargeReward happyItem = null;
            int prizeType = ThreadSafeRandom.NextStatic(0, 12);
            Player.HappyRechargeData.LotteryCount -= 1;
            if ((HappyRechargePrizeType)prizeType == HappyRechargePrizeType.FreePlay)
            {
                Player.HappyRechargeData.LotteryCount += 1;
                Player.Out.SendMessage(Packets.eMessageType.Normal, LanguageMgr.GetTranslation("HappyRecharge." + (HappyRechargePrizeType)prizeType));
                prizeType = ThreadSafeRandom.NextStatic(0, 7);
            }
            if ((HappyRechargePrizeType)prizeType == HappyRechargePrizeType.Ticket)
                Player.HappyRechargeData.LotteryTicket += 1;
            if ((HappyRechargePrizeType)prizeType == HappyRechargePrizeType.SmallPrize)
                IsLog = true;
            if ((HappyRechargePrizeType)prizeType == HappyRechargePrizeType.MediumPrize)
                IsLog = true;
            if ((HappyRechargePrizeType)prizeType == HappyRechargePrizeType.Jackpot)
            {
                GamePlayer[] players = WorldMgr.GetAllPlayers();
                for (int i = 0; i < players.Length; i++)
                    players[i].Out.SendMessage(Packets.eMessageType.ChatNormal, String.Format(LanguageMgr.GetTranslation("HappyRecharge.JackPot", Player.PlayerCharacter.NickName)));
                IsLog = true;
            }
            if ((int)prizeType < 8)
                using (ProduceBussiness pb = new ProduceBussiness())
                    happyItem = ActiveSystemMgr.HappyRechargeRewards[(int)prizeType];
            string message = "Default";
            if (prizeType > 7)
                message = ((HappyRechargePrizeType)prizeType).ToString();
            Player.Out.SendMessage(Packets.eMessageType.Normal, LanguageMgr.GetTranslation("HappyRecharge." + message));
            GSPacket.WriteInt(prizeType < 8 ? happyItem.TemplateID : (int)prizeType);//_currentPrizeItemID = param1.readInt();
            GSPacket.WriteInt(prizeType);//_currentPrizeItemSortID = param1.readInt();
            GSPacket.WriteInt(prizeType < 7 ? happyItem.Count : 1);//_currentPrizeItemCount = param1.readInt();
            GSPacket.WriteInt(Player.HappyRechargeData.LotteryCount);//_lotteryCount = param1.readInt();
            GSPacket.WriteInt(Player.HappyRechargeData.LotteryTicket);//_ticketCount = param1.readInt();
            Player.SendTCP(GSPacket);
            HappyRecharegeUpdate(happyItem, Player, IsLog, (int)prizeType);
            return true;
        }
        private void HappyRecharegeUpdate(HappyRechargeReward happyItem, GamePlayer player, bool isLog, int type)
        {
            if (happyItem != null)
            {
                using (ProduceBussiness produceBussiness = new ProduceBussiness())
                {
                    ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(happyItem.TemplateID);
                    if (itemTemplate != null)
                    {
                        ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(itemTemplate, 1, 102);
                        fromTemplate.IsBinds = true;
                        fromTemplate.ValidDate = happyItem.ValiDate;
                        fromTemplate.Count = happyItem.Count;
                        fromTemplate.AttackCompose = 0;
                        fromTemplate.DefendCompose = 0;
                        fromTemplate.AgilityCompose = 0;
                        fromTemplate.LuckCompose = 0;
                        player.AddTemplate(fromTemplate);
                    }
                }
            }

            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                HappyRechargeData temp = produceBussiness.HappyRechargePrize();
                temp.Count++;

                int count = 0;
                if (type == 10)
                    count = (temp.Count / 100) * 5;
                if (type == 11)
                    count = (temp.Count / 100) * 10;
                if (type == 12)
                    count = temp.Count;
                if (type > 9)
                {
                    temp.Count -= count;
                    produceBussiness.UpdateHappyRechargeDataPrize(type, count, player.PlayerCharacter.NickName);
                }
                produceBussiness.UpdateHappyRechargePrize(temp.Count);//(temp.Count == 0 ? 1000 : temp.Count);
                ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(temp.TemplateID);
                if (itemTemplate != null)
                {
                    ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(itemTemplate, 1, 102);
                    fromTemplate.IsBinds = true;
                    fromTemplate.ValidDate = temp.ValiDate;
                    fromTemplate.Count = count;
                    fromTemplate.AttackCompose = 0;
                    fromTemplate.DefendCompose = 0;
                    fromTemplate.AgilityCompose = 0;
                    fromTemplate.LuckCompose = 0;
                    player.AddTemplate(fromTemplate);
                }
                GSPacketIn GSPacket = new GSPacketIn(145);
                GSPacket.WriteByte(180);
                GSPacket.WriteInt(1); //var _loc6_:int = param1.readInt();
                GSPacket.WriteInt(temp.Count);//_prizeCount = param1.readInt();
                GSPacket.WriteInt(type);// prizeType_ = param1.readInt();
                GSPacket.WriteString(player.PlayerCharacter.NickName);// NickName = param1.readUTF();
                GSPacket.WriteInt(count);//_loc2_ = param1.readInt();
                player.SendTCP(GSPacket);
                GSPacket = new GSPacketIn(145);
                GSPacket.WriteByte(180);
                GSPacket.WriteInt(2); //var _loc6_:int = param1.readInt();
                GSPacket.WriteInt(player.HappyRechargeData.LotteryCount);
            }
        }
    }
    enum HappyRechargePrizeType
    {
        FreePlay = 8,
        Ticket,
        SmallPrize,
        MediumPrize,
        Jackpot
    }
}
