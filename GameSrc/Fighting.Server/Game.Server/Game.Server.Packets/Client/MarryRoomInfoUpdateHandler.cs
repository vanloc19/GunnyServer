using Bussiness;
using Game.Base.Packets;
using Game.Server.SceneMarryRooms;

namespace Game.Server.Packets.Client
{
	[PacketHandler(237, "更新礼堂信息")]
	internal class MarryRoomInfoUpdateHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.CurrentMarryRoom != null && client.Player.PlayerCharacter.ID == client.Player.CurrentMarryRoom.Info.PlayerID)
			{
				string str = packet.ReadString();
				bool num = packet.ReadBoolean();
				string str2 = packet.ReadString();
				string str3 = packet.ReadString();
				MarryRoom currentMarryRoom = client.Player.CurrentMarryRoom;
				currentMarryRoom.Info.RoomIntroduction = str3;
				currentMarryRoom.Info.Name = str;
				if (num)
				{
					currentMarryRoom.Info.Pwd = str2;
				}
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					bussiness.UpdateMarryRoomInfo(currentMarryRoom.Info);
				}
				currentMarryRoom.SendMarryRoomInfoUpdateToScenePlayers(currentMarryRoom);
				client.Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("MarryRoomInfoUpdateHandler.Successed"));
				return 0;
			}
			return 1;
		}
	}
}
