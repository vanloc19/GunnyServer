using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.DDTQiYuan.Handle
{
    [QiYuanHandleAttbute((byte)QiYuanPackageType.PACK_TYPE_RANK_REWARD_CONFIG)]
    public class RankAward : IQiYuanCommandHadler
    {
        public int CommandHandler(GamePlayer player, GSPacketIn packet)
        {
            GSPacketIn pkg = new GSPacketIn((int)ePackageType.SAN_XIAO);
            pkg.WriteByte((byte)QiYuanPackageType.PACK_TYPE_RANK_REWARD_CONFIG);

            List<QiYuanAwardInfo> itemList = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.BELIEF_AWARD);
            pkg.WriteInt(itemList.Count);
            foreach (QiYuanAwardInfo item in itemList)
            {
                pkg.WriteInt(item.Rank);
                pkg.WriteInt(item.TemplateID);
                pkg.WriteInt(item.Count);
                pkg.WriteBoolean(item.IsBind);
                pkg.WriteInt(item.VaildDate);
            }

            itemList = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.AREA_RANK_AWARD);
            pkg.WriteInt(itemList.Count);
            foreach (QiYuanAwardInfo item in itemList)
            {
                pkg.WriteInt(item.Rank);
                pkg.WriteInt(item.TemplateID);
                pkg.WriteInt(item.Count);
                pkg.WriteBoolean(item.IsBind);
                pkg.WriteInt(item.VaildDate);
            }

            itemList = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.ALL_AREA_RANK_AWARD);
            pkg.WriteInt(itemList.Count);
            foreach (QiYuanAwardInfo item in itemList)
            {
                pkg.WriteInt(item.Rank);
                pkg.WriteInt(item.TemplateID);
                pkg.WriteInt(item.Count);
                pkg.WriteBoolean(item.IsBind);
                pkg.WriteInt(item.VaildDate);
            }

            pkg.WriteInt(GameProperties.QiYuanJoinLeastOfferTimes); //  joinRewardLeastOfferTimes
            pkg.WriteInt(GameProperties.QiYuanRankLeastOfferTimes); // rankRewardLeastOfferTimes

            QiYuanAwardInfo itemInfo = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.JOIN_REWARD, 1);
            pkg.WriteInt(itemInfo.TemplateID);
            pkg.WriteInt(itemInfo.Count);
            pkg.WriteBoolean(itemInfo.IsBind);
            pkg.WriteInt(itemInfo.VaildDate);

            itemInfo = QiYuanAwardMgr.FindQiYuanAward(eQiYuanAwardType.JOIN_PROBABILITY_REWARD, 1);
            pkg.WriteInt(itemInfo.TemplateID);
            pkg.WriteInt(itemInfo.Count);
            pkg.WriteBoolean(itemInfo.IsBind);
            pkg.WriteInt(itemInfo.VaildDate);
            pkg.WriteInt(itemInfo.Probability);

            pkg.WriteInt(GameProperties.QiYuanOfferTimesPerBaoZhu); // offerTimesPerBaoZhu - số điểm tối thiểu reset
            pkg.WriteInt(GameProperties.QiYuanOfferTimesPerTreasureBox); // offerTimesPerTreasureBox - số dâng tối thiểu để đc 1 box
            pkg.WriteString(QiYuanAwardMgr.QiYuanRewardBoxTemplateIds); // offer1Or10TimesRewardBoxGoodArr
            pkg.WriteString(QiYuanAwardMgr.QiYuanTreasureBoxTemplateIds); // openTreasureBoxGoodArr

            player.SendTCP(pkg);

            return 1;
        }
    }
}