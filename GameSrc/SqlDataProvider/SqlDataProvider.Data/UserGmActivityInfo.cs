using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlDataProvider.Data
{
    public class UserGmActivityInfo
    {
        public UserGmActivityInfo()
        {
            GiftsReceivedList = new List<GiftCurInfo>();
            StatusList = new Dictionary<int, GMStatusInfo>();
        }

        public void UpdateGiftReceive(string giftId, int total)
        {
            GiftCurInfo cur = GiftsReceivedList.SingleOrDefault(a => a.giftID == giftId);

            if (cur == null)
            {
                cur = new GiftCurInfo();
                cur.giftID = giftId;
                GiftsReceivedList.Add(cur);
            }

            cur.times += total;
            cur.lastTimeGet = DateTime.Now;
        }

        public int UserID { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public int VipLevel { get; set; }

        public List<GiftCurInfo> GiftsReceivedList { get; set; }

        public Dictionary<int, GMStatusInfo> StatusList { get; set; }

        public int Value { get; set; }
    }
}
