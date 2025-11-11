using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(230, "Send achievement finish")]
	public class AchievementFinishHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int id = packet.ReadInt();
			AchievementInfo achievement = client.Player.AchievementInventory.FindAchievement(id);
			if (new PlayerBussiness().GetUserAchievementData(client.Player.PlayerCharacter.ID, id).Count == 0)
			{
				GSPacketIn pkg = new GSPacketIn(230, client.Player.PlayerCharacter.ID);
				pkg.WriteInt(id);
				DateTime now = DateTime.Now;
				pkg.WriteInt(now.Year);
				pkg.WriteInt(now.Month);
				pkg.WriteInt(now.Day);
				client.Player.AchievementInventory.AddAchievementData(achievement);
				client.Player.AchievementInventory.SendReward(achievement);
				client.Player.OnAchievementQuest();
				client.Player.AchievementInventory.SaveToDatabase();
				client.SendTCP(pkg);
			}
			return 0;
		}
	}
}
