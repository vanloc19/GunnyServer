using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.HotSpringRooms
{
	public class HotSpringRoom
	{
		private GInterface2 ginterface2_0;

		private static readonly ILog ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public HotSpringRoomInfo Info;

		private int int_0;

		private List<GamePlayer> list_0;

		private static object object_0 = new object();

		public int Count => int_0;

		public HotSpringRoom(HotSpringRoomInfo info, GInterface2 processor)
		{
			Info = info;
			ginterface2_0 = processor;
			list_0 = new List<GamePlayer>();
			int_0 = 0;
		}

		public bool AddPlayer(GamePlayer player)
		{
			lock (object_0)
			{
				if (player.CurrentRoom != null || player.CurrentHotSpringRoom != null)
				{
					return false;
				}
				if (list_0.Count > Info.maxCount)
				{
					player.Out.SendMessage(eMessageType.GM_NOTICE, LanguageMgr.GetTranslation("Phòng đã đầy"));
					return false;
				}
				if (player.Extra.Info.MinHotSpring <= 0)
				{
					GSPacketIn pkg = new GSPacketIn(191);
					pkg.WriteByte(11);
					player.SendTCP(pkg);
					player.Out.SendMessage(eMessageType.GM_NOTICE, string.Format("Đã hết thời gian vào suối nước nóng"));
					return false;
				}
				int_0++;
				list_0.Add(player);
				player.CurrentHotSpringRoom = this;
				player.Extra.Info.LastTimeHotSpring = DateTime.Now;
				SetDefaultPostion(player);
				if (player.CurrentRoom != null)
				{
					player.CurrentRoom.RemovePlayerUnsafe(player);
				}
				player.Extra.BeginHotSpringTimer();
				player.OnEnterHotSpring();
				HotSpringRoom[] rooms = new HotSpringRoom[1]
				{
					player.CurrentHotSpringRoom
				};
				HotSpringMgr.SendUpdateAllRoom(null, rooms);
				GSPacketIn packet = new GSPacketIn(198);
				packet.WriteInt(player.PlayerCharacter.ID);
				packet.WriteInt(player.PlayerCharacter.Grade);
				packet.WriteInt(player.PlayerCharacter.Hide);
				packet.WriteInt(player.PlayerCharacter.Repute);
				packet.WriteString(player.PlayerCharacter.NickName);
				packet.WriteByte(player.PlayerCharacter.typeVIP);
				packet.WriteInt(player.PlayerCharacter.VIPLevel);
				packet.WriteBoolean(player.PlayerCharacter.Sex);
				packet.WriteString(player.PlayerCharacter.Style);
				packet.WriteString(player.PlayerCharacter.Colors);
				packet.WriteString(player.PlayerCharacter.Skin);
				packet.WriteInt(player.Hot_X);
				packet.WriteInt(player.Hot_Y);
				packet.WriteInt(player.PlayerCharacter.FightPower);
				packet.WriteInt(player.PlayerCharacter.Win);
				packet.WriteInt(player.PlayerCharacter.Total);
				packet.WriteInt(player.Hot_Direction);
				SendToPlayerExceptSelf(packet, player);
			}
			return true;
		}

		public bool CanEnter()
		{
			return int_0 < Info.maxCount;
		}

		public GamePlayer[] GetAllPlayers()
		{
			lock (object_0)
			{
				return list_0.ToArray();
			}
		}

		public GamePlayer GetPlayerWithID(int playerId)
		{
			lock (object_0)
			{
				foreach (GamePlayer player in list_0)
				{
					if (player.PlayerCharacter.ID == playerId)
					{
						return player;
					}
				}
				return null;
			}
		}

		protected void OnTick(object obj)
		{
			ginterface2_0.OnTick(this);
		}

		public void ProcessData(GamePlayer player, GSPacketIn data)
		{
			lock (object_0)
			{
				ginterface2_0.OnGameData(this, player, data);
			}
		}

		public void RemovePlayer(GamePlayer player)
		{
			lock (object_0)
			{
				if (player.CurrentHotSpringRoom != null)
				{
					int_0--;
					list_0.Remove(player);
					player.Extra.StopHotSpringTimer();
					GSPacketIn packet = new GSPacketIn(199, player.PlayerCharacter.ID);
					packet.WriteInt(player.PlayerCharacter.ID);
					packet.WriteString("");
					player.CurrentHotSpringRoom.SendToAll(packet);
					SetDefaultPostion(player);
					HotSpringRoom[] rooms = new HotSpringRoom[1]
					{
						player.CurrentHotSpringRoom
					};
					HotSpringMgr.SendUpdateAllRoom(null, rooms);
					player.CurrentHotSpringRoom = null;
				}
			}
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

		public void SendToRoomPlayer(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			if (allPlayers != null)
			{
				GamePlayer[] playerArray2 = allPlayers;
				for (int i = 0; i < playerArray2.Length; i++)
				{
					playerArray2[i].Out.SendTCP(packet);
				}
			}
		}

		public void SetDefaultPostion(GamePlayer p)
		{
			p.Hot_X = 480;
			p.Hot_Y = 560;
			p.Hot_Direction = 3;
		}
	}
}
