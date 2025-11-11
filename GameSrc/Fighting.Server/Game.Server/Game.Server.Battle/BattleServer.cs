using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using log4net;

namespace Game.Server.Battle
{
	public class BattleServer
	{
		private int int_0;

		private int int_1;

		private DateTime dateTime_0;

		private readonly ILog ilog_0;

		private FightServerConnector fightServerConnector_0;

		private Dictionary<int, BaseRoom> dictionary_0;

		private string string_0;

		private int int_2;

		private string string_1;

		public int RetryCount
		{
			get
			{
				return int_1;
			}
			set
			{
				int_1 = value;
			}
		}

		public DateTime LastRetryTime
		{
			get
			{
				return dateTime_0;
			}
			set
			{
				dateTime_0 = value;
			}
		}

		public FightServerConnector Server => fightServerConnector_0;

		public string LoginKey => string_1;

		public int ServerId => int_0;

		public bool IsActive => fightServerConnector_0.IsConnected;

		public string Ip => string_0;

		public int Port => int_2;

		public event EventHandler Disconnected;

		public BattleServer(int serverId, string ip, int port, string loginKey)
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
			int_0 = serverId;
			string_0 = ip;
			int_2 = port;
			string_1 = loginKey;
			int_1 = 0;
			dateTime_0 = DateTime.Now;
			fightServerConnector_0 = new FightServerConnector(this, ip, port, loginKey);
			dictionary_0 = new Dictionary<int, BaseRoom>();
			fightServerConnector_0.Disconnected += method_2;
			fightServerConnector_0.Connected += method_1;
		}

		public BattleServer Clone()
		{
			return new BattleServer(int_0, string_0, int_2, string_1);
		}

		public void Start()
		{
			if (!fightServerConnector_0.Connect())
			{
				ThreadPool.QueueUserWorkItem(method_0);
			}
		}

		private void method_0(object object_0)
		{
			method_2(fightServerConnector_0);
		}

		private void method_1(BaseClient baseClient_0)
		{
		}

		private void method_2(BaseClient baseClient_0)
		{
			RemoveAllRoom();
			if (this.Disconnected != null)
			{
				this.Disconnected(this, null);
			}
		}

		public void RemoveAllRoom()
		{
			BaseRoom[] baseRoomArray = null;
			lock (dictionary_0)
			{
				baseRoomArray = dictionary_0.Values.ToArray();
				dictionary_0.Clear();
			}
			BaseRoom[] array = baseRoomArray;
			foreach (BaseRoom room in array)
			{
				if (room != null)
				{
					room.RemoveAllPlayer();
					RoomMgr.StopProxyGame(room);
				}
			}
		}

		public BaseRoom FindRoom(int roomId)
		{
			BaseRoom baseRoom = null;
			lock (dictionary_0)
			{
				if (dictionary_0.ContainsKey(roomId))
				{
					return dictionary_0[roomId];
				}
				return baseRoom;
			}
		}

		public bool AddRoom(BaseRoom room)
		{
			bool flag = false;
			lock (dictionary_0)
			{
				if (!dictionary_0.ContainsKey(room.RoomId))
				{
					dictionary_0.Add(room.RoomId, room);
					flag = true;
				}
			}
			if (flag)
			{
				fightServerConnector_0.SendAddRoom(room);
			}
			return flag;
		}

		public bool RemoveRoom(BaseRoom room)
		{
			bool flag = false;
			lock (dictionary_0)
			{
				flag = dictionary_0.ContainsKey(room.RoomId);
			}
			if (flag)
			{
				fightServerConnector_0.SendRemoveRoom(room);
			}
			return flag;
		}

		public void RemoveRoomImp(int roomId)
		{
			BaseRoom room = null;
			lock (dictionary_0)
			{
				if (dictionary_0.ContainsKey(roomId))
				{
					room = dictionary_0[roomId];
					dictionary_0.Remove(roomId);
				}
			}
			if (room != null)
			{
				if (room.IsPlaying && room.Game == null)
				{
					RoomMgr.CancelPickup(this, room);
				}
				else
				{
					RoomMgr.StopProxyGame(room);
				}
			}
		}

		public void StartGame(int roomId, ProxyGame game)
		{
			BaseRoom room = FindRoom(roomId);
			if (room != null)
			{
				RoomMgr.StartProxyGame(room, game);
			}
		}

		public void StopGame(int roomId, int gameId)
		{
			BaseRoom room = FindRoom(roomId);
			if (room != null)
			{
				RoomMgr.StopProxyGame(room);
				lock (dictionary_0)
				{
					dictionary_0.Remove(roomId);
				}
			}
		}

		public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
		{
			BaseRoom room = FindRoom(roomId);
			if (room != null)
			{
				if (exceptId != 0)
				{
					GamePlayer player = WorldMgr.GetPlayerById(exceptId);
					if (player != null)
					{
						if (player.TempGameId == exceptGameId)
						{
							room.SendToAll(pkg, player);
						}
						else
						{
							room.SendToAll(pkg);
						}
					}
				}
				else
				{
					room.SendToAll(pkg);
				}
			}
		}

		public void SendToUser(int playerid, GSPacketIn pkg)
		{
			WorldMgr.GetPlayerById(playerid)?.SendTCP(pkg);
		}

		public void UpdatePlayerGameId(int playerid, int gamePlayerId)
		{
			GamePlayer player = WorldMgr.GetPlayerById(playerid);
			if (player != null)
            {
				player.GamePlayerId = gamePlayerId;
				player.TempGameId = gamePlayerId;
            }
			//GamePlayer playerById = WorldMgr.GetPlayerById(playerid);
			//if (playerById != null)
			//{
			//	playerById.GameId = gamePlayerId;
			//	return;
			//}
			//ilog_0.Error("//UPDATE GAMEID ERROR: " + playerid + "|" + gamePlayerId);
		}

		public override string ToString()
		{
			return $"ServerID:{int_0},Ip:{fightServerConnector_0.RemoteEP.Address},Port:{fightServerConnector_0.RemoteEP.Port},IsConnected:{fightServerConnector_0.IsConnected},RoomCount:{dictionary_0.Count}";
		}
	}
}
