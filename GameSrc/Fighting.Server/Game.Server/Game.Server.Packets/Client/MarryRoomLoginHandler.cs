using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(242, "进入礼堂")]
	public class MarryRoomLoginHandler : IPacketHandler
	{
		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			MarryRoom room = null;
			string msg = "";
			int id = packet.ReadInt();
			string str2 = packet.ReadString();
			int num2 = packet.ReadInt();
			if (id != 0)
			{
				room = MarryRoomMgr.GetMarryRoombyID(id, (str2 == null) ? "" : str2, ref msg);
			}
			else
			{
				if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
				{
					MarryRoom[] allMarryRoom = MarryRoomMgr.GetAllMarryRoom();
					foreach (MarryRoom room2 in allMarryRoom)
					{
						if (room2.Info.GroomID == client.Player.PlayerCharacter.ID || room2.Info.BrideID == client.Player.PlayerCharacter.ID)
						{
							room = room2;
							break;
						}
					}
				}
				if (room == null && client.Player.PlayerCharacter.SelfMarryRoomID != 0)
				{
					client.Player.Out.SendMarryRoomLogin(client.Player, result: false);
					MarryRoomInfo marryRoomInfoSingle = null;
					using (PlayerBussiness bussiness = new PlayerBussiness())
					{
						marryRoomInfoSingle = bussiness.GetMarryRoomInfoSingle(client.Player.PlayerCharacter.SelfMarryRoomID);
					}
					if (marryRoomInfoSingle != null)
					{
						client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoomLoginHandler.RoomExist", marryRoomInfoSingle.ServerID, client.Player.PlayerCharacter.SelfMarryRoomID));
						return 0;
					}
				}
			}
			if (room != null)
			{
				if (room.CheckUserForbid(client.Player.PlayerCharacter.ID))
				{
					client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation("MarryRoomLoginHandler.Forbid"));
					client.Player.Out.SendMarryRoomLogin(client.Player, result: false);
					return 1;
				}
				if (room.RoomState == eRoomState.FREE)
				{
					if (room.AddPlayer(client.Player))
					{
						client.Player.MarryMap = num2;
						client.Player.Out.SendMarryRoomLogin(client.Player, result: true);
						room.SendMarryRoomInfoUpdateToScenePlayers(room);
						return 0;
					}
				}
				else
				{
					client.Player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("MarryRoomLoginHandler.AlreadyBegin"));
				}
				client.Player.Out.SendMarryRoomLogin(client.Player, result: false);
			}
			else
			{
				client.Out.SendMessage(eMessageType.BIGBUGLE_NOTICE, LanguageMgr.GetTranslation(string.IsNullOrEmpty(msg) ? "MarryRoomLoginHandler.Failed" : msg));
				client.Player.Out.SendMarryRoomLogin(client.Player, result: false);
			}
			return 1;
		}
	}
}
