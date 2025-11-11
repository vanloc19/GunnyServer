using System;
using System.Collections.Generic;
using System.Text;

namespace SqlDataProvider.Data
{
    public class GmGiftInfo
    {
        public GmGiftInfo()
        {
            Conditions = new Dictionary<int, GmActiveConditionInfo>();
            RewardsList = new List<GmActiveRewardInfo>();
        }
        public string giftbagId { get; set; }
        public string activityId { get; set; }
        public int rewardMark { get; set; }
        public int giftbagOrder { get; set; }

        public Dictionary<int, GmActiveConditionInfo> Conditions;
        public List<GmActiveRewardInfo> RewardsList;

        public bool IsChoiceItem
        {
            get
            {
                foreach (GmActiveRewardInfo reward in RewardsList)
                {
                    if (reward.rewardType != 0)
                        return true;
                }

                return false;
            }
            
        }
    }
}
