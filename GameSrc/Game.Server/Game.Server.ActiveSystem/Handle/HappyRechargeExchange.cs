using System;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Bussiness;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.HAPPYRECHARGE_EXCHANGE)]
    class HappyRechargeExchange : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            int ticket = packet.ReadInt();
            int itemId = packet.ReadInt();
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                HappyRechargeTicketReward template = ActiveSystemMgr.HappyTicketRewards.Find(delegate (HappyRechargeTicketReward t) { return t.TemplateID == itemId; });
                ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(itemId);
                if (itemTemplate != null)
                {
                    ItemInfo fromTemplate = ItemInfo.CreateFromTemplate(itemTemplate, 1, 102);
                    fromTemplate.IsBinds = true;
                    fromTemplate.ValidDate = template.ValiDate;
                    fromTemplate.Count = template.Count;
                    fromTemplate.AttackCompose = 0;
                    fromTemplate.DefendCompose = 0;
                    fromTemplate.AgilityCompose = 0;
                    fromTemplate.LuckCompose = 0;
                    Player.AddTemplate(fromTemplate);
                }
            }
            Player.HappyRechargeData.LotteryTicket -= ticket;
            GSPacketIn GSPacket = new GSPacketIn(145);
            GSPacket.WriteByte(177);
            GSPacket.WriteInt(Player.HappyRechargeData.LotteryTicket);
            Player.SendTCP(GSPacket);
            return true;
        }
    }
}
