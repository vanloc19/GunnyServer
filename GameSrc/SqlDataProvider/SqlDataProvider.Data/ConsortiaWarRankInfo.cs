using System;

namespace SqlDataProvider.Data
{
    public class ConsortiaWarRankInfo
    {
        public int ConsortiaID { get; set; }
        public string ConsortiaName { get; set; }
        public int Rank { get; set; }
        public int Score { get; set; }
        public DateTime TimeCreate { get; set; }
    }
}