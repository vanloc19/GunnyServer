using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Rooms;
using SqlDataProvider.Data;
using Bussiness.Managers;
using Game.Server.Statics;
using Game.Server.GMActives;

namespace Game.Server.Packets.Client
{
    [PacketHandler((short)ePackageType.FIGHT_POWER_RANK, "场景用户离开")]
    public class fightPowerRankHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            List<IGMActive> lists = GmActivityMgr.GetAllGMActionByType(typeof(RankFightPower));

            foreach (IGMActive action in lists)
            {
                GSPacketIn pkg = new GSPacketIn((int)ePackageType.FIGHT_POWER_RANK);
                pkg.WriteString(action.Info.activityId); // actId = _loc4_.readUTF();
                pkg.WriteBoolean(action.Info.IsAvalibleShow);

                if (action.Info.IsAvalibleShow)
                {
                    pkg.WriteInt(action.Info.IsAvalible ? 1 : 2); // status | 2: event is end

                    List<UserGmActivityInfo> allTops = action.GetRankTop();

                    pkg.WriteInt(allTops.Count); // total players in rank

                    foreach (UserGmActivityInfo userRank in allTops)
                    {
                        pkg.WriteInt(userRank.UserID);//_loc6_.userId = _loc4_.readInt();
                        pkg.WriteString(userRank.NickName);//_loc6_.name = _loc4_.readUTF();
                        pkg.WriteByte((byte)userRank.VipLevel);//_loc6_.vipLvl = _loc4_.readByte();
                        pkg.WriteInt(userRank.Value);//_loc6_.consume = _loc4_.readInt();
                    }

                    pkg.WriteInt(action.GetPlayer(client.Player).Value); // my consume
                }

                client.SendTCP(pkg);
            }

            return 0;
        }
    }
}
