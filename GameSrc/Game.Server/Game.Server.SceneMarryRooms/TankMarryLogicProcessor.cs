using System;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.SceneMarryRooms.TankHandle;
using log4net;

namespace Game.Server.SceneMarryRooms
{
	[MarryProcessor(9, "礼堂逻辑")]
	public class TankMarryLogicProcessor : AbstractMarryProcessor
	{
		private MarryCommandMgr _commandMgr = new MarryCommandMgr();

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private ThreadSafeRandom random = new ThreadSafeRandom();

		public readonly int TIMEOUT = 60000;

		public override void OnGameData(MarryRoom room, GamePlayer player, GSPacketIn packet)
		{
			MarryCmdType type = (MarryCmdType)packet.ReadByte();
			try
			{
				IMarryCommandHandler handler = _commandMgr.LoadCommandHandler((int)type);
				if (handler != null)
				{
					handler.HandleCommand(this, player, packet);
				}
				else
				{
					log.Error($"IP: {player.Client.TcpEndpoint}");
				}
			}
			catch (Exception exception)
			{
				log.Error(string.Format("IP:{1}, OnGameData is Error: {0}", exception.ToString(), player.Client.TcpEndpoint));
			}
		}

		public override void OnTick(MarryRoom room)
		{
			try
			{
				if (room != null)
				{
					room.KickAllPlayer();
					using (PlayerBussiness bussiness = new PlayerBussiness())
					{
						bussiness.DisposeMarryRoomInfo(room.Info.ID);
					}
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.GroomID);
					GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(room.Info.BrideID);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.GroomID, state: false, room.Info);
					GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(room.Info.BrideID, state: false, room.Info);
					MarryRoomMgr.RemoveMarryRoom(room);
					GSPacketIn pkg = new GSPacketIn(254);
					pkg.WriteInt(room.Info.ID);
					WorldMgr.MarryScene.SendToALL(pkg);
					room.StopTimer();
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("OnTick", exception);
				}
			}
		}
	}
}
