using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.MARRY_SCENE_LOGIN, "Player enter marry scene.")]
	public class UserEnterMarrySceneHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			GSPacketIn @in = new GSPacketIn(240, client.Player.PlayerCharacter.ID);
			if (WorldMgr.MarryScene.AddPlayer(client.Player))
			{
				@in.WriteBoolean(val: true);
			}
			else
			{
				@in.WriteBoolean(val: false);
			}
			client.Out.SendTCP(@in);
			if (client.Player.CurrentMarryRoom == null)
			{
				MarryRoom[] allMarryRoom = MarryRoomMgr.GetAllMarryRoom();
				foreach (MarryRoom room in allMarryRoom)
				{
					client.Player.Out.SendMarryRoomInfo(client.Player, room);
				}
			}
			return 0;
		}
	}
}
