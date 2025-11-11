using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Packets.Client
{
	[PacketHandler(233, "结婚场景切换")]
	internal class MarrySceneChangeHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentMarryRoom == null || client.Player.MarryMap == 0)
			{
				return 1;
			}
			int num = packet.ReadInt();
			if (num == client.Player.MarryMap)
			{
				return 1;
			}
			GSPacketIn @in = new GSPacketIn(244, client.Player.PlayerCharacter.ID);
			client.Player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(@in, client.Player);
			client.Player.MarryMap = num;
			switch (num)
			{
			case 1:
				client.Player.X = 514;
				client.Player.Y = 637;
				break;
			case 2:
				client.Player.X = 800;
				client.Player.Y = 763;
				break;
			}
			GamePlayer[] allPlayers = client.Player.CurrentMarryRoom.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player != client.Player && player.MarryMap == client.Player.MarryMap)
				{
					player.Out.SendPlayerEnterMarryRoom(client.Player);
					client.Player.Out.SendPlayerEnterMarryRoom(player);
				}
			}
			return 0;
		}
	}
}
