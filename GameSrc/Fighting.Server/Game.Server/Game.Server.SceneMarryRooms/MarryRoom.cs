using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.SceneMarryRooms
{
	public class MarryRoom
	{
		private int _count;

		private List<GamePlayer> _guestsList;

		private IMarryProcessor _processor;

		private eRoomState _roomState;

		private static object _syncStop = new object();

		private Timer _timer;

		private Timer _timerForHymeneal;

		private List<int> _userForbid;

		private List<int> _userRemoveList;

		public MarryRoomInfo Info;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int Count => _count;

		public eRoomState RoomState
		{
			get
			{
				return _roomState;
			}
			set
			{
				if (_roomState != value)
				{
					_roomState = value;
					SendMarryRoomInfoUpdateToScenePlayers(this);
				}
			}
		}

		public MarryRoom(MarryRoomInfo info, IMarryProcessor processor)
		{
			Info = info;
			_processor = processor;
			_guestsList = new List<GamePlayer>();
			_count = 0;
			_roomState = eRoomState.FREE;
			_userForbid = new List<int>();
			_userRemoveList = new List<int>();
		}

		public bool AddPlayer(GamePlayer player)
		{
			lock (_syncStop)
			{
				if (player.CurrentRoom != null || player.IsInMarryRoom)
				{
					return false;
				}
				if (_guestsList.Count > Info.MaxCount)
				{
					player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("MarryRoom.Msg1"));
					return false;
				}
				_count++;
				_guestsList.Add(player);
				player.CurrentMarryRoom = this;
				player.MarryMap = 1;
				if (player.CurrentRoom != null)
				{
					player.CurrentRoom.RemovePlayerUnsafe(player);
				}
			}
			return true;
		}

		public void BeginTimer(int interval)
		{
			if (_timer == null)
			{
				_timer = new Timer(OnTick, null, interval, interval);
			}
			else
			{
				_timer.Change(interval, interval);
			}
		}

		public void BeginTimerForHymeneal(int interval)
		{
			if (_timerForHymeneal == null)
			{
				_timerForHymeneal = new Timer(OnTickForHymeneal, null, interval, interval);
			}
			else
			{
				_timerForHymeneal.Change(interval, interval);
			}
		}

		public bool CheckUserForbid(int userID)
		{
			lock (_syncStop)
			{
				return _userForbid.Contains(userID);
			}
		}

		public GamePlayer[] GetAllPlayers()
		{
			lock (_syncStop)
			{
				return _guestsList.ToArray();
			}
		}

		public GamePlayer GetPlayerByUserID(int userID)
		{
			lock (_syncStop)
			{
				foreach (GamePlayer player in _guestsList)
				{
					if (player.PlayerCharacter.ID == userID)
					{
						return player;
					}
				}
			}
			return null;
		}

		public void KickAllPlayer()
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				RemovePlayer(player);
				player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.TimeOver"));
			}
		}

		public bool KickPlayerByUserID(GamePlayer player, int userID)
		{
			GamePlayer playerByUserID = GetPlayerByUserID(userID);
			if (playerByUserID != null && playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
			{
				RemovePlayer(playerByUserID);
				playerByUserID.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));
				GSPacketIn packet = player.Out.SendMessage(eMessageType.ChatERROR, playerByUserID.PlayerCharacter.NickName + "  " + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2"));
				player.CurrentMarryRoom.SendToPlayerExceptSelf(packet, player);
				return true;
			}
			return false;
		}

		protected void OnTick(object obj)
		{
			_processor.OnTick(this);
		}

		protected void OnTickForHymeneal(object obj)
		{
			try
			{
				_roomState = eRoomState.FREE;
				GSPacketIn packet = new GSPacketIn(249);
				packet.WriteByte(9);
				SendToAll(packet);
				StopTimerForHymeneal();
				SendUserRemoveLate();
				SendMarryRoomInfoUpdateToScenePlayers(this);
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("OnTickForHymeneal", exception);
				}
			}
		}

		public void ProcessData(GamePlayer player, GSPacketIn data)
		{
			lock (_syncStop)
			{
				_processor.OnGameData(this, player, data);
			}
		}

		public void RemovePlayer(GamePlayer player)
		{
			lock (_syncStop)
			{
				if (RoomState == eRoomState.FREE)
				{
					_count--;
					_guestsList.Remove(player);
					GSPacketIn packet = player.Out.SendPlayerLeaveMarryRoom(player);
					player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet, player);
					player.CurrentMarryRoom = null;
					player.MarryMap = 0;
				}
				else if (RoomState == eRoomState.Hymeneal)
				{
					_userRemoveList.Add(player.PlayerCharacter.ID);
					_count--;
					_guestsList.Remove(player);
					player.CurrentMarryRoom = null;
				}
				SendMarryRoomInfoUpdateToScenePlayers(this);
			}
		}

		public void ReturnPacket(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn @in = packet.Clone();
			@in.ClientID = player.PlayerCharacter.ID;
			SendToPlayerExceptSelf(@in, player);
		}

		public void ReturnPacketForScene(GamePlayer player, GSPacketIn packet)
		{
			GSPacketIn @in = packet.Clone();
			@in.ClientID = player.PlayerCharacter.ID;
			SendToPlayerExceptSelfForScene(@in, player);
		}

		public void RoomContinuation(int time)
		{
			TimeSpan span = DateTime.Now - Info.BeginTime;
			int num = Info.AvailTime * 60 - span.Minutes + time * 60;
			Info.AvailTime += time;
			using (PlayerBussiness bussiness = new PlayerBussiness())
			{
				bussiness.UpdateMarryRoomInfo(Info);
			}
			BeginTimer(60000 * num);
		}

		public GSPacketIn SendMarryRoomInfoUpdateToScenePlayers(MarryRoom room)
		{
			GSPacketIn packet = new GSPacketIn(255);
			bool val = room != null;
			packet.WriteBoolean(val);
			if (val)
			{
				packet.WriteInt(room.Info.ID);
				packet.WriteBoolean(room.Info.IsHymeneal);
				packet.WriteString(room.Info.Name);
				packet.WriteBoolean(!(room.Info.Pwd == ""));
				packet.WriteInt(room.Info.MapIndex);
				packet.WriteInt(room.Info.AvailTime);
				packet.WriteInt(room.Count);
				packet.WriteInt(room.Info.PlayerID);
				packet.WriteString(room.Info.PlayerName);
				packet.WriteInt(room.Info.GroomID);
				packet.WriteString(room.Info.GroomName);
				packet.WriteInt(room.Info.BrideID);
				packet.WriteString(room.Info.BrideName);
				packet.WriteDateTime(room.Info.BeginTime);
				packet.WriteByte((byte)room.RoomState);
				packet.WriteString(room.Info.RoomIntroduction);
			}
			SendToScenePlayer(packet);
			return packet;
		}

		public void SendToAll(GSPacketIn packet)
		{
			SendToAll(packet, null, isChat: false);
		}

		public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			if (allPlayers == null)
			{
				return;
			}
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer player in array)
			{
				if (!isChat || !player.IsBlackFriend(self.PlayerCharacter.ID))
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void SendToAllForScene(GSPacketIn packet, int sceneID)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			if (allPlayers == null)
			{
				return;
			}
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer player in array)
			{
				if (player.MarryMap == sceneID)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void SendToPlayerExceptSelf(GSPacketIn packet, GamePlayer self)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			if (allPlayers == null)
			{
				return;
			}
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer player in array)
			{
				if (player != self)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void SendToPlayerExceptSelfForScene(GSPacketIn packet, GamePlayer self)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			if (allPlayers == null)
			{
				return;
			}
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer player in array)
			{
				if (player != self && player.MarryMap == self.MarryMap)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void SendToRoomPlayer(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Out.SendTCP(packet);
				}
			}
		}

		public void SendToScenePlayer(GSPacketIn packet)
		{
			WorldMgr.MarryScene.SendToALL(packet);
		}

		public void SendUserRemoveLate()
		{
			lock (_syncStop)
			{
				foreach (int num in _userRemoveList)
				{
					GSPacketIn packet = new GSPacketIn(244, num);
					SendToAllForScene(packet, 1);
				}
				_userRemoveList.Clear();
			}
		}

		public void SetUserForbid(int userID)
		{
			lock (_syncStop)
			{
				_userForbid.Add(userID);
			}
		}

		public void StopTimer()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}

		public void StopTimerForHymeneal()
		{
			if (_timerForHymeneal != null)
			{
				_timerForHymeneal.Dispose();
				_timerForHymeneal = null;
			}
		}
	}
}
