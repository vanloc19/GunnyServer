using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{
	[PacketHandler(241, "礼堂创建")]
	public class MarryRoomCreateHandler : IPacketHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int HandlePacket(GameClient client, GSPacketIn packet)
		{
			if (client.Player.PlayerCharacter.IsMarried)
			{
				if (client.Player.PlayerCharacter.IsCreatedMarryRoom)
				{
					return 1;
				}
				if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
				{
					client.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Bag.Locked"));
					return 0;
				}
				if (client.Player.CurrentRoom != null)
				{
					client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
				}
				if (client.Player.CurrentMarryRoom != null)
				{
					client.Player.CurrentMarryRoom.RemovePlayer(client.Player);
				}
				MarryRoomInfo info = new MarryRoomInfo
				{
					Name = packet.ReadString(),
					Pwd = packet.ReadString(),
					MapIndex = packet.ReadInt(),
					AvailTime = packet.ReadInt(),
					MaxCount = packet.ReadInt(),
					GuestInvite = packet.ReadBoolean(),
					RoomIntroduction = packet.ReadString(),
					ServerID = GameServer.Instance.Configuration.ServerID,
					IsHymeneal = false
				};
				string[] strArray = GameProperties.PRICE_MARRY_ROOM.Split(',');
				if (strArray.Length < 3)
				{
					if (log.IsErrorEnabled)
					{
						log.Error("MarryRoomCreateMoney node in configuration file is wrong");
					}
					return 1;
				}
				int num;
				switch (info.AvailTime)
				{
				case 2:
					num = int.Parse(strArray[0]);
					break;
				case 3:
					num = int.Parse(strArray[1]);
					break;
				case 4:
					num = int.Parse(strArray[2]);
					break;
				default:
					num = int.Parse(strArray[2]);
					info.AvailTime = 4;
					break;
				}
				if (client.Player.MoneyDirect(num, IsAntiMult: false))
				{
					MarryRoom room = MarryRoomMgr.CreateMarryRoom(client.Player, info);
					if (room != null)
					{
						GSPacketIn @in = client.Player.Out.SendMarryRoomInfo(client.Player, room);
						client.Player.Out.SendMarryRoomLogin(client.Player, result: true);
						room.SendToScenePlayer(@in);
						CountBussiness.InsertSystemPayCount(client.Player.PlayerCharacter.ID, num, 0, 0, 0);
						DailyRecordInfo record = new DailyRecordInfo
						{
							UserID = client.Player.PlayerCharacter.ID,
							Type = 4,
							Value = client.Player.PlayerCharacter.SpouseName
						};
						new PlayerBussiness().AddDailyRecord(record);
					}
					return 0;
				}
			}
			return 1;
		}
	}
}
