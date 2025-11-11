using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fighting.Server.GameObjects;
using Game.Base.Packets;
using Game.Logic;
using log4net;
using System;

namespace Fighting.Server.Rooms
{
	public class ProxyRoom
	{
		private static readonly ILog ilog_0;

		private List<IGamePlayer> list_0;

		private int int_0;

		private int int_1;

		private ServerClient serverClient_0;

		public bool bool_0;

		public int PickUpNPCTotal;

		public int PickUpCount;

		public int selfId;

		public bool startWithNpc;

		private int int_4;

		private bool bool_2;

		private bool bool_3;

		private int rYyHcnovuT;

		private int int_5;

		public bool IsPlaying;

		public eGameType GameType;

		public eRoomType RoomType;

		public bool IsCrossZone;

		public int GuildId;

		public string GuildName;

		public int AvgLevel;

		public int FightPower;

		private BaseGame baseGame_0;

		public DateTime createDate;

		public bool HaveNewbie
		{
			get;
			set;
		}

		public int PickUpRate
		{
			get;
			set;
		}

		public int PickUpRateLevel
		{
			get;
			set;
		}

		public int RoomId => int_0;

		public ServerClient Client => serverClient_0;

		public int NpcId
		{
			get
			{
				return int_4;
			}
			set
			{
				int_4 = value;
			}
		}

		public bool isAutoBot => bool_2;

		public bool isBotSnape
		{
			get
			{
				return bool_3;
			}
			set
			{
				bool_3 = value;
			}
		}

		public int EliteGameType => rYyHcnovuT;

		public int ZoneId => int_5;

		public int PlayerCount => this.list_0.Where(p => !p.IsQuanChien).Count();

		public BaseGame Game => baseGame_0;

		public ProxyRoom(int roomId, int orientRoomId, int zoneID, IGamePlayer[] players, ServerClient client, int npcId, bool pickUpWithNPC, bool isBot, bool isSmartBot)
		{
			int_4 = npcId;
			int_0 = roomId;
			int_1 = orientRoomId;
			list_0 = new List<IGamePlayer>();
			list_0.AddRange(players);
			serverClient_0 = client;
			bool_0 = pickUpWithNPC;
			bool_2 = isBot;
			bool_3 = isSmartBot;
			PickUpCount = 0;
			HaveNewbie = false;
			if (RoomType == eRoomType.EliteGameScore)
			{
				method_0();
			}
			PickUpRate = 5;
			PickUpRateLevel = 1;
			PickUpNPCTotal = 0;
			int_5 = zoneID;
			createDate = DateTime.Now;
		}

		private void method_0()
		{
			if (list_0[0].PlayerCharacter.Grade <= 40)
			{
				rYyHcnovuT = 1;
			}
			else
			{
				rYyHcnovuT = 2;
			}
		}

		public void SendToAll(GSPacketIn pkg)
		{
			SendToAll(pkg, null);
		}

		public void SendToAll(GSPacketIn pkg, IGamePlayer except)
		{
			serverClient_0.SendToRoom(int_1, pkg, except);
		}

		public List<IGamePlayer> GetPlayers()
		{
			var list = new List<IGamePlayer>();
			lock (list_0)
            {
				list.AddRange(list_0);
            }
			return list;
		}

		public bool RemovePlayer(IGamePlayer player)
		{
			bool flag = false;
			lock (list_0)
			{
				if (list_0.Remove(player))
				{
					flag = true;
				}
			}
			if (PlayerCount == 0)
			{
				ProxyRoomMgr.RemoveRoom(this);
			}
			return flag;
		}

		public void SetDefaultDamageAll()
        {
			lock(list_0)
            {
				foreach (ProxyPlayer item in list_0)
                {
					if (item != null)
                    {
						item.PlayerCharacter.Attack = 1700;
						item.PlayerCharacter.Defence = 1500;
						item.PlayerCharacter.Agility = 1600;
						item.PlayerCharacter.Luck = 1500;
						item.PlayerCharacter.hp = 25000;
						item.GetBaseAttack();
						item.GetBaseDefence();
						item.SendMessage(string.Format("ATK = {0} | DEF = {1} | Agi = {2} | Luk = {3} | Hp = {4} | Dmg = {5} | Gua = {6}", item.PlayerCharacter.Attack, item.PlayerCharacter.Defence, item.PlayerCharacter.Agility, item.PlayerCharacter.Luck, item.PlayerCharacter.hp, item.GetBaseAttack(), item.GetBaseDefence()));
                    }
                }
            }
        }

		public void StartGame(BaseGame game)
		{
			IsPlaying = true;
			baseGame_0 = game;
			game.GameStopped += method_1;
			serverClient_0.SendStartGame(int_1, game);
		}

		private void method_1(AbstractGame abstractGame_0)
		{
			baseGame_0.GameStopped -= method_1;
			IsPlaying = false;
			serverClient_0.SendStopGame(int_1, baseGame_0.Id);
		}

		public void Dispose()
		{
			serverClient_0.RemoveRoom(int_1, this);
		}

		public override string ToString()
		{
			return $"RoomId:{int_0} OriendId:{int_1} PlayerCount:{list_0.Count},IsPlaying:{IsPlaying},GuildId:{GuildId},GuildName:{GuildName}";
		}

		static ProxyRoom()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
