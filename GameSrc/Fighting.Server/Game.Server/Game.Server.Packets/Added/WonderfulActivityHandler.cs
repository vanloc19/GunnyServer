using System;
using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GMActives;
using Game.Server.Statics;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
    [PacketHandler((byte)ePackageType.WONDERFUL_ACTIVITY, "场景用户离开")]
    public class WonderfulActivityHandler : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            int type = packet.ReadInt();
            int num = packet.ReadInt();

            GSPacketIn pkg = new GSPacketIn((byte)ePackageType.WONDERFUL_ACTIVITY);
            pkg.WriteByte((byte)type);
            switch (type)
            {
                case 0:
                    List<GmActivityInfo> activitys = GmActivityMgr.GetAllGmActivity();
                    pkg.WriteInt(activitys.Count);
                    foreach (GmActivityInfo gm in activitys)
                    {
                        pkg.WriteInt(gm.activityType);//data.id = e.pkg.readInt();
                        switch ((ActivityType)gm.activityType)
                        {
                            case ActivityType.ZHANYOUCHONGZHIHUIKUI:
                                pkg.WriteInt(1);//t = e.pkg.readInt();
                                break;
                            case ActivityType.CHONGZHIHUIKUI:
                                pkg.WriteInt(2);//this.chongZhiScore = e.pkg.readInt();
                                break;
                            case ActivityType.XIAOFEIHUIKUI:
                                pkg.WriteInt(3);//this.xiaoFeiScore = e.pkg.readInt();
                                break;
                            default:
                                pkg.WriteInt(4);//tt = e.pkg.readInt();
                                break;
                        }
                        pkg.WriteInt(num);//data.num = e.pkg.readInt();
                        pkg.WriteDateTime(gm.beginTime);//startTime = e.pkg.readDate();
                        pkg.WriteDateTime(gm.endTime);//endTime = e.pkg.readDate();
                    }
                    break;
            }
            client.Player.SendTCP(pkg);
            return 0;
        }
    }
}
