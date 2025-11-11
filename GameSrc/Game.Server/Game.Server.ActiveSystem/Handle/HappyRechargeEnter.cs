using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Bussiness;
using SqlDataProvider.Data;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.ActiveSystem.Handle
{
    [ActiveSystemHandleAttbute((byte)ActiveSystemPackageType.HAPPYRECHARGE_ENTER)]
    class HappyRechargeEnter : IActiveSystemCommandHadler
    {
        public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
        {
            GSPacketIn pkg179 = new GSPacketIn(145);
            pkg179.WriteByte(179);
            pkg179.WriteInt(3000);//_moneyCount = pkg.readInt();
            pkg179.WriteInt(Player.HappyRechargeData.LotteryCount);//_lotteryCount = pkg.readInt();
            pkg179.WriteInt(Player.HappyRechargeData.LotteryTicket);//_ticketCount = pkg.readInt();
            pkg179.WriteDateTime(DateTime.Parse(GameProperties.HappyRechargeBeginDate));//var startDate:Date = pkg.readDate();
            pkg179.WriteDateTime(DateTime.Parse(GameProperties.HappyRechargeEndDate));//var endDate:Date = pkg.readDate();
            using (ProduceBussiness producrBussiness = new ProduceBussiness())
            {
                HappyRechargeData prize = producrBussiness.HappyRechargePrize();
                pkg179.WriteInt(prize.TemplateID);//_prizeItemID = pkg.readInt();
                pkg179.WriteInt(prize.Count);//_prizeCount = pkg.readInt();
                pkg179.WriteInt(prize.ValiDate);// var valid:int = pkg.readInt();
                pkg179.WriteString("0,0,0,0");// var attributes:String = pkg.readUTF();
                pkg179.WriteInt(1);//var isbind:int = pkg.readInt();
            }
            List<HappyRechargeReward> rewardTemp = ActiveSystemMgr.HappyRechargeRewards;
            pkg179.WriteInt(rewardTemp.Count);//var len:int = pkg.readInt(); counts = 8 
            foreach (HappyRechargeReward temp in rewardTemp)
            {
                pkg179.WriteInt(temp.TemplateID);//itemid = pkg.readInt();
                pkg179.WriteInt(temp.Count);//count = pkg.readInt();
                pkg179.WriteInt(temp.ValiDate);//tvalid = pkg.readInt();
                pkg179.WriteString("0,0,0,0");// tattributes = pkg.readUTF();
                pkg179.WriteInt(temp.ID);//index = pkg.readInt();
                pkg179.WriteInt(1);//tisbind = pkg.readInt();
            }
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                List<HappyRechargeTicketReward> rewardTicket = ActiveSystemMgr.HappyTicketRewards;
                pkg179.WriteInt(rewardTicket.Count);//var len2:int = pkg.readInt(); 
                foreach (HappyRechargeTicketReward item in rewardTicket)
                {
                    pkg179.WriteInt(item.TemplateID);//exId = pkg.readInt(); 
                    pkg179.WriteInt(item.Count);//exCount = pkg.readInt(); 
                    pkg179.WriteInt(item.ValiDate);//exValid = pkg.readInt(); 
                    pkg179.WriteString("0,0,0,0");//exAttribute = pkg.readUTF(); 
                    pkg179.WriteString(item.TicketPrice);//ticketCount = pkg.readUTF(); 
                    pkg179.WriteInt(1);//exisbind = pkg.readInt(); 
                }
            }
            using (ProduceBussiness produceBussiness = new ProduceBussiness())
            {
                HappyRechargeDataPrize[] dataItem = produceBussiness.GetHappyRechargeData();
                pkg179.WriteInt(dataItem.Length);//var len3:int = pkg.readInt();
                for (int k = 0; k < dataItem.Length;)
                {
                    pkg179.WriteInt(dataItem[k].PrizeType);//recordItem.prizeType = pkg.readInt();
                    pkg179.WriteInt(dataItem[k].Count);//recordItem.count = pkg.readInt();
                    pkg179.WriteString(dataItem[k].NickName);//recordItem.nickName = pkg.readUTF();
                    k++;
                }
            }
            Player.SendTCP(pkg179);
            return true;
        }
    }
}
