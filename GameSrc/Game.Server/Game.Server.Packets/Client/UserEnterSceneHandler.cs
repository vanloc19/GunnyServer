using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.Rooms;
using System;

namespace Game.Server.Packets.Client
{
	[PacketHandler((int)ePackageType.SCENE_LOGIN, "Player enter scene.")]
	public class UserEnterSceneHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			int typeScene = packet.ReadInt();
			switch(typeScene)
            {
				case 1:
					client.Player.PlayerState = ePlayerState.RoomList;
					break;
				case 2:
					client.Player.PlayerState = ePlayerState.Dungeon;
					break;
				default:
					Console.WriteLine("UserEnterScene: typeScene {0}", typeScene);
					break;
			}
			RoomMgr.WaitingRoom.SendSceneUpdate(client.Player);
			return 1;
			#region
			/*var typeScene = packet.ReadInt();
			client.Player.BeginChanges();
			switch (typeScene)
			{
				case 1:
					client.Player.PlayerState = ePlayerState.RoomList;
					break;
				case 2:
					client.Player.PlayerState = ePlayerState.Dungeon;
					break;
				default:
					Console.WriteLine("UserEnterScene: typeScene {0}", typeScene);
					break;
            }
            client.Player.CommitChanges();
			RoomMgr.EnterWaitingRoom(client.Player);
			if (WorldMgr.HotSpringScene.GetClientFromID(client.Player.PlayerCharacter.ID) != null)
			{
				WorldMgr.HotSpringScene.RemovePlayer(client.Player);
			}
			return 1;*/
			#endregion
		}
    }
}
