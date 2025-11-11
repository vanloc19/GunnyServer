using System;
namespace SqlDataProvider.Data
{
    public class GiftCurInfo
    {
        public string giftID { get; set; }
        public int times { get; set; }
        public int allGiftGetTimes { get; set; }
        public DateTime lastTimeGet { get; set; }
        public int TotalCount { get; set; }
    }
}
