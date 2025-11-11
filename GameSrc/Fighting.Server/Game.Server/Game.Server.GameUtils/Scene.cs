using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils
{
	public class Scene
	{
		protected ReaderWriterLock _locker = new ReaderWriterLock();

		protected Dictionary<int, GamePlayer> _players = new Dictionary<int, GamePlayer>();

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public Scene(ServerInfo info)
		{
		}

		public bool AddPlayer(GamePlayer player)
		{
			_locker.AcquireWriterLock(-1);
			try
			{
				if (_players.ContainsKey(player.PlayerCharacter.ID))
				{
					_players[player.PlayerCharacter.ID] = player;
					return true;
				}
				_players.Add(player.PlayerCharacter.ID, player);
				return true;
			}
			finally
			{
				_locker.ReleaseWriterLock();
			}
		}

		public GamePlayer[] GetAllPlayer()
		{
			GamePlayer[] playerArray = null;
			_locker.AcquireReaderLock(10000);
			try
			{
				playerArray = _players.Values.ToArray();
			}
			finally
			{
				_locker.ReleaseReaderLock();
			}
			if (playerArray != null)
			{
				return playerArray;
			}
			return new GamePlayer[0];
		}

		public GamePlayer GetClientFromID(int id)
		{
			if (_players.Keys.Contains(id))
			{
				return _players[id];
			}
			return null;
		}

		public void RemovePlayer(GamePlayer player)
		{
			_locker.AcquireWriterLock(-1);
			try
			{
				if (_players.ContainsKey(player.PlayerCharacter.ID))
				{
					_players.Remove(player.PlayerCharacter.ID);
				}
			}
			finally
			{
				_locker.ReleaseWriterLock();
			}
			GamePlayer[] allPlayer = GetAllPlayer();
			GSPacketIn packet = null;
			GamePlayer[] array = allPlayer;
			foreach (GamePlayer player2 in array)
			{
				if (packet == null)
				{
					packet = player2.Out.SendSceneRemovePlayer(player);
				}
				else
				{
					player2.Out.SendTCP(packet);
				}
			}
		}

		public void SendToALL(GSPacketIn pkg)
		{
			SendToALL(pkg, null);
		}

		public void SendToALL(GSPacketIn pkg, GamePlayer except)
		{
			GamePlayer[] allPlayer = GetAllPlayer();
			foreach (GamePlayer player in allPlayer)
			{
				if (player != except)
				{
					player.Out.SendTCP(pkg);
				}
			}
		}
	}
}
