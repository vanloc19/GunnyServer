using System.Collections.Generic;
using Game.Base;
using Game.Base.Packets;

namespace Game.Server.RingStation.Battle
{
	public class RingStationBattleServer
	{
		private string m_ip;

		private int m_port;

		private Dictionary<int, BaseRoomRingStation> m_rooms;

		private RingStationFightConnector m_server;

		private int m_serverId;

		public string Ip => m_ip;

		public bool IsActive => m_server.IsConnected;

		public int Port => m_port;

		public RingStationFightConnector Server => m_server;

		public RingStationBattleServer(int serverId, string ip, int port, string loginKey)
		{
			m_serverId = serverId;
			m_ip = ip;
			m_port = port;
			m_server = new RingStationFightConnector(this, ip, port, loginKey);
			m_rooms = new Dictionary<int, BaseRoomRingStation>();
			m_server.Disconnected += m_server_Disconnected;
			m_server.Connected += m_server_Connected;
		}

		public bool AddRoom(BaseRoomRingStation room)
		{
			bool flag = false;
			BaseRoomRingStation station = null;
			lock (m_rooms)
			{
				if (m_rooms.ContainsKey(room.RoomId))
				{
					station = m_rooms[room.RoomId];
					m_rooms.Remove(room.RoomId);
				}
			}
			if (station != null && station.Game != null)
			{
				station.Game.Stop();
			}
			lock (m_rooms)
			{
				if (!m_rooms.ContainsKey(room.RoomId))
				{
					m_rooms.Add(room.RoomId, room);
					flag = true;
				}
			}
			if (flag)
			{
				m_server.SendAddRoom(room);
			}
			room.BattleServer = this;
			return flag;
		}

		private BaseRoomRingStation FindRoom(int roomId)
		{
			BaseRoomRingStation station = null;
			lock (m_rooms)
			{
				if (m_rooms.ContainsKey(roomId))
				{
					return m_rooms[roomId];
				}
				return station;
			}
		}

		private void m_server_Connected(BaseClient client)
		{
		}

		private void m_server_Disconnected(BaseClient client)
		{
		}

		public bool RemoveRoom(BaseRoomRingStation room)
		{
			bool flag = false;
			lock (m_rooms)
			{
				flag = m_rooms.ContainsKey(room.RoomId);
				if (flag)
				{
					m_server.SendRemoveRoom(room);
					return flag;
				}
				return flag;
			}
		}

		public void RemoveRoomImp(int roomId)
		{
		}

		public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
		{
			BaseRoomRingStation station = FindRoom(roomId);
			if (station == null)
			{
				return;
			}
			if (exceptId != 0)
			{
				RingStationGamePlayer playerById = RingStationMgr.GetPlayerById(exceptId);
				if (playerById != null)
				{
					if (playerById.GamePlayerId == exceptGameId)
					{
						station.SendToAll(pkg, playerById);
					}
					else
					{
						station.SendToAll(pkg);
					}
				}
			}
			else
			{
				station.SendToAll(pkg);
			}
		}

		public void SendToUser(int playerid, GSPacketIn pkg)
		{
		}

		public bool Start()
		{
			return m_server.Connect();
		}

		public void StartGame(int roomId, ProxyRingStationGame game)
		{
			FindRoom(roomId)?.StartGame(game);
		}

		public void StopGame(int roomId, int gameId)
		{
			if (FindRoom(roomId) != null)
			{
				lock (m_rooms)
				{
					m_rooms.Remove(roomId);
				}
			}
		}

		public override string ToString()
		{
			return $"ServerID:{m_serverId},Ip:{m_server.RemoteEP.Address},Port:{m_server.RemoteEP.Port},IsConnected:{m_server.IsConnected},RoomCount:";
		}

		public void UpdatePlayerGameId(int playerid, int gamePlayerId)
		{
			RingStationGamePlayer playerById = RingStationMgr.GetPlayerById(playerid);
			if (playerById != null)
			{
				playerById.GamePlayerId = gamePlayerId;
			}
		}

		public void UpdateRoomId(int roomId, int fightRoomId)
		{
		}
	}
}
