using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(203, "场景用户离开")]
	public class LookupEffortHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int num = packet.ReadInt();
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				AchievementData[] data = bussiness.GetUserAchievement(num);
				PlayerInfo playerCharacter = bussiness.GetUserSingleByUserID(num);
				GSPacketIn pkg = new GSPacketIn(203, num);
				pkg.WriteInt(playerCharacter.AchievementPoint);
				pkg.WriteInt(data.Length);
				foreach (AchievementData achievement in data)
				{
					pkg.WriteInt(achievement.AchievementID);
				}
				client.SendTCP(pkg);
			}
			return 0;
		}
	}
}
