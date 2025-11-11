using System.Collections.Generic;

namespace SqlDataProvider.Data
{
    public class RankRechargeInfo
    {
        public RankRechargeInfo()
        {
            UsersData = new Dictionary<string, List<UserGmActivityInfo>>();
            ListActIDAwardsSended = new List<string>();
        }
        public Dictionary<string, List<UserGmActivityInfo>> UsersData { get; set; }

        public List<string> ListActIDAwardsSended { get; set; }
    }
}
