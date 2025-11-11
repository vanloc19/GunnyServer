using System;

namespace SqlDataProvider.Data
{
    public class ConsortiaWarPlayerRankInfo
    {
        public int UserID { get; set; }
        public string NickName { get; set; }
        public int ConsortiaID { get; set; }
        public int Score { get; set; }
        public string ZoneName { get; set; }
        public int ZoneID { get; set; }
        public DateTime TimeCreate { get; set; }
    }
}