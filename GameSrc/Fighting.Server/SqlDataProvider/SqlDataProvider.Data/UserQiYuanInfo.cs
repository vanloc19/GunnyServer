using System.Collections.Generic;

namespace SqlDataProvider.Data
{
    public class UserQiYuanInfo
    {
        public int UserID { get; set; }
        public int ZoneID { get; set; }
        public string ZoneName { get; set; }
        public string NickName { get; set; }
        public List<int> HasGetGoods { get; set; }
        public Dictionary<int, int> ListTowerItemsCount { get; set; }
        public List<int> HasGetTowerGoods { get; set; }
        public int myOfferTimes { get; set; }
        public int hasGainTreasureBoxNum { get; set; }
        public int hasGainJoinRewardCount { get; set; }
    }
}