using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.SceneMarryRooms;
using log4net;
using log4net.Util;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class MarryRoomMgr
	{
		protected static ReaderWriterLock _locker = new ReaderWriterLock();

		protected static TankMarryLogicProcessor _processor = new TankMarryLogicProcessor();

		protected static Dictionary<int, MarryRoom> _Rooms;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static MarryRoom CreateMarryRoom(GamePlayer player, MarryRoomInfo info)
		{
			if (player.PlayerCharacter.IsMarried)
			{
				MarryRoom room = null;
				DateTime now = DateTime.Now;
				info.PlayerID = player.PlayerCharacter.ID;
				info.PlayerName = player.PlayerCharacter.NickName;
				if (player.PlayerCharacter.Sex)
				{
					info.GroomID = info.PlayerID;
					info.GroomName = info.PlayerName;
					info.BrideID = player.PlayerCharacter.SpouseID;
					info.BrideName = player.PlayerCharacter.SpouseName;
				}
				else
				{
					info.BrideID = info.PlayerID;
					info.BrideName = info.PlayerName;
					info.GroomID = player.PlayerCharacter.SpouseID;
					info.GroomName = player.PlayerCharacter.SpouseName;
				}
				info.BeginTime = now;
				info.BreakTime = now;
				using (PlayerBussiness bussiness = new PlayerBussiness())
				{
					if (bussiness.InsertMarryRoomInfo(info))
					{
						room = new MarryRoom(info, _processor);
						GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.GroomID);
						GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.BrideID);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.GroomID, state: true, info);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.BrideID, state: true, info);
					}
				}
				if (room != null)
				{
					_locker.AcquireWriterLock();
					try
					{
						_Rooms.Add(room.Info.ID, room);
					}
					finally
					{
						_locker.ReleaseWriterLock();
					}
					if (room.AddPlayer(player))
					{
						room.BeginTimer(3600000 * room.Info.AvailTime);
						return room;
					}
				}
			}
			return null;
		}

		public static MarryRoom CreateMarryRoomFromDB(MarryRoomInfo roomInfo, int timeLeft)
		{
			_locker.AcquireWriterLock();
			try
			{
				MarryRoom room = new MarryRoom(roomInfo, _processor);
				if (room != null)
				{
					_Rooms.Add(room.Info.ID, room);
					room.BeginTimer(60000 * timeLeft);
					return room;
				}
			}
			finally
			{
				_locker.ReleaseWriterLock();
			}
			return null;
		}

		private static void CheckRoomStatus()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				MarryRoomInfo[] marryRoomInfo = bussiness.GetMarryRoomInfo();
				foreach (MarryRoomInfo info in marryRoomInfo)
				{
					if (info.ServerID != GameServer.Instance.Configuration.ServerID)
					{
						continue;
					}
					TimeSpan span = DateTime.Now - info.BeginTime;
					int timeLeft = info.AvailTime * 60 - (int)span.TotalMinutes;
					if (timeLeft > 0)
					{
						CreateMarryRoomFromDB(info, timeLeft);
						continue;
					}
					bussiness.DisposeMarryRoomInfo(info.ID);
					if (GameServer.Instance.LoginServer != null)
					{
						GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.GroomID);
						GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(info.BrideID);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.GroomID, state: false, info);
						GameServer.Instance.LoginServer.SendMarryRoomInfoToPlayer(info.BrideID, state: false, info);
					}
				}
			}
		}

		public static MarryRoom[] GetAllMarryRoom()
		{
			MarryRoom[] array = null;
			_locker.AcquireReaderLock();
			try
			{
				array = new MarryRoom[_Rooms.Count];
				_Rooms.Values.CopyTo(array, 0);
			}
			finally
			{
				_locker.ReleaseReaderLock();
			}
			if (array != null)
			{
				return array;
			}
			return new MarryRoom[0];
		}

		public static MarryRoom GetMarryRoombyID(int id, string pwd, ref string msg)
		{
			MarryRoom room = null;
			_locker.AcquireReaderLock();
			try
			{
				if (id <= 0 || !_Rooms.Keys.Contains(id))
				{
					return room;
				}
				if (_Rooms[id].Info.Pwd != pwd)
				{
					msg = "Game.Server.Managers.PWDError";
					return room;
				}
				return _Rooms[id];
			}
			finally
			{
				_locker.ReleaseReaderLock();
			}
		}

		public static bool Init()
		{
			_Rooms = new Dictionary<int, MarryRoom>();
			CheckRoomStatus();
			return true;
		}

		public static void RemoveMarryRoom(MarryRoom room)
		{
			_locker.AcquireReaderLock();
			try
			{
				if (_Rooms.Keys.Contains(room.Info.ID))
				{
					_Rooms.Remove(room.Info.ID);
				}
			}
			finally
			{
				_locker.ReleaseReaderLock();
			}
		}

		public static bool UpdateBreakTimeWhereServerStop()
		{
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				return bussiness.UpdateBreakTimeWhereServerStop();
			}
		}
	}
}
