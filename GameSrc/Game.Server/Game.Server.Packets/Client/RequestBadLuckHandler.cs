using System.Collections.Generic;
using System.Linq;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(45, "打开物品")]
	public class RequestBadLuckHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn pkg = new GSPacketIn(45);
			pkg.WriteString(WorldMgr.LastTimeUpdateCaddyRank.ToString());
			List<UsersExtraInfo> list = WorldMgr.CaddyRank.Values.ToList();
			pkg.WriteInt(list.Count);
			int val = 1;
			foreach (UsersExtraInfo userExtraInfo in list)
			{
				pkg.WriteInt(val);
				pkg.WriteInt(userExtraInfo.UserID);
				pkg.WriteInt(userExtraInfo.TotalCaddyOpen);
				pkg.WriteInt(0);
				pkg.WriteString(userExtraInfo.NickName);
				val++;
			}
			client.Player.SendTCP(pkg);
			return 0;
		}
	}
}
