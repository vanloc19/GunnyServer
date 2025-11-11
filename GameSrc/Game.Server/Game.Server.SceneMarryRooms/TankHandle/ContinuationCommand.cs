using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;

namespace Game.Server.SceneMarryRooms.TankHandle
{
	[MarryCommandAttbute(3)]
	public class ContinuationCommand : IMarryCommandHandler
	{
		protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
		{
			if (player.CurrentMarryRoom == null)
			{
				return false;
			}
			if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				return false;
			}
			int time = packet.ReadInt();
			string[] strArray = GameProperties.PRICE_MARRY_ROOM.Split(',');
			if (strArray.Length < 3)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("MarryRoomCreateMoney node in configuration file is wrong");
				}
				return false;
			}
			int num2;
			switch (time)
			{
			case 2:
				num2 = int.Parse(strArray[0]);
				break;
			case 3:
				num2 = int.Parse(strArray[1]);
				break;
			case 4:
				num2 = int.Parse(strArray[2]);
				break;
			default:
				num2 = int.Parse(strArray[2]);
				time = 4;
				break;
			}
			if (player.PlayerCharacter.Money + player.PlayerCharacter.MoneyLock < num2)
			{
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyHandler.Msg1"));
				return false;
			}
			player.RemoveMoney(num2, isConsume: false);
			CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, num2, 0, 0, 0);
			player.CurrentMarryRoom.RoomContinuation(time);
			GSPacketIn @in = player.Out.SendContinuation(player, player.CurrentMarryRoom.Info);
			int brideID = ((player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID) ? player.CurrentMarryRoom.Info.GroomID : player.CurrentMarryRoom.Info.BrideID);
			WorldMgr.GetPlayerById(brideID)?.Out.SendTCP(@in);
			player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("ContinuationCommand.Successed"));
			return true;
		}
	}
}
