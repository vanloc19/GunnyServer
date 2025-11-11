using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Game.Base.Packets;
//using Game.Server.Managers;
using Game.Server.GameObjects;
using Bussiness.Managers;
using System.Collections.Generic;
using SqlDataProvider.Data;
using Game.Server.Statics;
using Game.Server.Managers;
using System.Linq;
using Game.Server.GMActives;
//using Game.Server.GameUtils;
//using Game.Server.Rooms;

namespace Game.Server.Packets.Client
{
    [PacketHandler((short)ePackageType.WONDERFUL_ACTIVITY_INIT, "场景用户离开")]
    public class WonderfulActivityInitHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadInt();

            List<IGMActive> listsGmInfos = GmActivityMgr.GetAllGmAction();
            
            client.Out.WonderfulActivityInit(listsGmInfos, client.Player, type);


            return 0;

        }
    }
}
