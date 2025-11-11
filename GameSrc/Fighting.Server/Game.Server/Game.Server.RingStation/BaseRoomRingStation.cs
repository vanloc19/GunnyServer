using System.Collections.Generic;
using System.Timers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.RingStation.Battle;

namespace Game.Server.RingStation
{
	public class BaseRoomRingStation
	{
		private Timer addrom = new Timer();

		public RingStationBattleServer BattleServer;

		public bool IsAutoBot;

		public bool IsFreedom;

		private AbstractGame m_game;

		private List<RingStationGamePlayer> m_places;

		public int PickUpNpcId;

		public int RoomId;

		public AbstractGame Game => m_game;

		public int GameType
		{
			get;
			set;
		}

		public int GuildId
		{
			get;
			set;
		}

		public bool IsPlaying
		{
			get;
			set;
		}

		public int RoomType
		{
			get;
			set;
		}

		public BaseRoomRingStation(int roomId)
		{
			RoomId = roomId;
			m_places = new List<RingStationGamePlayer>();
		}

		public bool AddPlayer(RingStationGamePlayer player)
		{
			lock (m_places)
			{
				player.CurRoom = this;
				m_places.Add(player);
			}
			return true;
		}

		internal List<RingStationGamePlayer> GetPlayers()
		{
			return m_places;
		}

		public void RemovePlayer(RingStationGamePlayer player)
		{
			if (BattleServer != null)
			{
				if (m_game != null)
				{
					BattleServer.Server.SendPlayerDisconnet(Game.Id, player.GamePlayerId, RoomId);
					BattleServer.Server.SendRemoveRoom(this);
				}
				IsPlaying = false;
			}
			if (Game != null)
			{
				Game.Stop();
			}
		}

		internal void SendTCP(GSPacketIn pkg)
		{
			if (m_game != null)
			{
				BattleServer.Server.SendToGame(m_game.Id, pkg);
			}
		}

		public void SendToAll(GSPacketIn pkg)
		{
			SendToAll(pkg, null);
		}

		public void SendToAll(GSPacketIn pkg, RingStationGamePlayer except)
		{
			lock (m_places)
			{
				foreach (RingStationGamePlayer player in m_places)
				{
					if (player != null && player != except)
					{
						player.ProcessPacket(pkg);
					}
				}
			}
		}

		public void StartGame(AbstractGame game)
		{
			m_game = game;
		}
	}
}
