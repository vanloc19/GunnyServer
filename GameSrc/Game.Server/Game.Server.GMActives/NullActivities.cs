using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Bussiness;

namespace Game.Server.GMActives
{
    public class NullActivities : IGMActive
    {
        public NullActivities(GmActivityInfo gmActive, List<UserGmActivityInfo> listUsersGmInfo) : base(gmActive, listUsersGmInfo)
        {
        }

        public override bool GetAward(GamePlayer player, string giftGroupId, int index)
        {
            return false;
        }

        public override void SetUser(UserGmActivityInfo user)
        {
        }

        public override void Start()
        {
        }

        public override void Stop()
        {
        }
    }
}
