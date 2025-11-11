using System.Collections.Generic;
using Bussiness;
using Game.Server.GameObjects;

namespace Game.Server.Managers
{
	public class LoginMgr
	{
		private static object _locker = new object();

		private static Dictionary<int, GameClient> _players = new Dictionary<int, GameClient>();

		public static void Add(int player, GameClient server)
		{
			GameClient client = null;
			lock (_locker)
			{
				if (_players.ContainsKey(player))
				{
					GameClient client2 = _players[player];
					if (client2 != null)
					{
						client = client2;
					}
					_players[player] = server;
				}
				else
				{
					_players.Add(player, server);
				}
			}
			if (client != null)
			{
				client.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
				client.Disconnect();
			}
		}

		public static void ClearLoginPlayer(int playerId)
		{
			GameClient client = null;
			lock (_locker)
			{
				if (_players.ContainsKey(playerId))
				{
					client = _players[playerId];
					_players.Remove(playerId);
				}
			}
			if (client != null)
			{
				client.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
				client.Disconnect();
			}
		}

		public static void ClearLoginPlayer(int playerId, GameClient client)
		{
			lock (_locker)
			{
				if (_players.ContainsKey(playerId) && _players[playerId] == client)
				{
					_players.Remove(playerId);
				}
			}
		}

		public static bool ContainsUser(int playerId)
		{
			lock (_locker)
			{
				return _players.ContainsKey(playerId) && _players[playerId].IsConnected;
			}
		}

		public static bool ContainsUser(string account)
		{
			lock (_locker)
			{
				foreach (GameClient client in _players.Values)
				{
					if (client != null && client.Player != null && client.Player.Account == account)
					{
						return true;
					}
				}
				return false;
			}
		}

		public static GamePlayer LoginClient(int playerId)
		{
			GameClient client = null;
			lock (_locker)
			{
				if (_players.ContainsKey(playerId))
				{
					client = _players[playerId];
					_players.Remove(playerId);
				}
			}
			return client?.Player;
		}

		public static void Remove(int player)
		{
			lock (_locker)
			{
				if (_players.ContainsKey(player))
				{
					_players.Remove(player);
				}
			}
		}
	}
}
