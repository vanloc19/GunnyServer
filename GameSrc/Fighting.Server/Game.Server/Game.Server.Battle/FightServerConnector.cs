using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Logic.Protocol;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.RingStation;
using Game.Server.Rooms;
using Game.Server.Statics;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Battle
{
	public class FightServerConnector : BaseConnector
	{
		private static readonly ILog log;

		private BattleServer m_server;

		private string m_key;

		protected override void OnDisconnect()
		{
			base.OnDisconnect();
		}

		public override void OnRecvPacket(GSPacketIn pkg)
		{
			ThreadPool.QueueUserWorkItem(AsynProcessPacket, pkg);
		}

		protected void AsynProcessPacket(object state)
		{
			try
			{
				GSPacketIn gsPacketIn = state as GSPacketIn;
				switch (gsPacketIn.Code)
				{
					case (int)eFightPackageType.RSAKey://0
						HandleRSAKey(gsPacketIn);
						break;
					case (int)eFightPackageType.CHAT://19
						HandlePlayerChatSend(gsPacketIn);
						break;
					case (int)eFightPackageType.SEND_TO_USER://32
						HandleSendToPlayer(gsPacketIn);
						break;
					case (int)eFightPackageType.SEND_GAME_PLAYER_ID://33
						HandleUpdatePlayerGameId(gsPacketIn);
						break;
					case (int)eFightPackageType.DISCONNECT_PLAYER://34
						HandleDisconnectPlayer(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ON_GAME_OVER://35
						HandlePlayerOnGameOver(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_USE_PROP_INGAME://36
						HandlePlayerOnUsingItem(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_GOLD://38
						HandlePlayerAddGold(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_GP://39
						HandlePlayerAddGP(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ONKILLING_LIVING://40
						HandlePlayerOnKillingLiving(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ONMISSION_OVER://41
						HandlePlayerOnMissionOver(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_CONSORTIAFIGHT://42
						HandlePlayerConsortiaFight(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_SEND_CONSORTIAFIGHT://43
						HandlePlayerSendConsortiaFight(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_REMOVE_GOLD://44
						HandlePlayerRemoveGold(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_REMOVE_MONEY://45
						HandlePlayerRemoveMoney(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_TEMPLATE1://48
						HandlePlayerAddTemplate1(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_REMOVE_GP://49
						HandlePlayerRemoveGP(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_REMOVE_OFFER://50
						HandlePlayerRemoveOffer(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_OFFER://51
						HandlePlayerAddOffer(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_ROBRICHES://52
						HandlePlayerAddRobRiches(gsPacketIn);
						break;
					case (int)eFightPackageType.ROOM_REMOVE://65
						HandleRoomRemove(gsPacketIn);
						break;
					case (int)eFightPackageType.ROOM_START_GAME://66
						HandleStartGame(gsPacketIn);
						break;
					case (int)eFightPackageType.SEND_TO_ROOM://67
						HandleSendToRoom(gsPacketIn);
						break;
					case (int)eFightPackageType.ROOM_STOP_GAME://68
						HandleStopGame(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_HEALSTONE://73
						HandlePlayerHealstone(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_MONEY://74
						HandlePlayerAddMoney(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_GIFTTOKEN://75
						HandlePlayerAddGiftToken(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_MEDAL://76
						HandlePlayerAddMedal(gsPacketIn);
						break;
					case (int)eFightPackageType.FIND_CONSORTIA_ALLY://77
						HandleFindConsortiaAlly(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_LEAGUEMONEY://84
						HandlePlayerAddLeagueMoney(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_PRESTIGE://85
						HandlePlayerAddPrestige(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_UPDATE_REST_COUNT://86
						HandlePlayerUpdateRestCount(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_LEAGESCORE:
						HandlePlayerAddleageScore(gsPacketIn);
						break;
					case (int)eFightPackageType.FIGHT_NPC:
						HandleFightNPC(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ADD_ELITEGAME_SCORE:
						HandleAddEliteScore(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_REMOVE_ELITEGAME_SCORE:
						HandleRemoveEliteScore(gsPacketIn);
						break;
					case (int)eFightPackageType.PLAYER_ELITEGAME_WINUPDATE:
						HandleEliteScoreWinUpdate(gsPacketIn);
						break;
					default:
						Console.WriteLine("Not Found PKG {0}", gsPacketIn.Code);
						break;
				}
			}
			catch (Exception ex)
			{
				GameServer.log.Error("AsynProcessPacket", ex);
			}
		}

		private void HandleEliteScoreWinUpdate(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.SendWinEliteChampion(pkg.ReadInt(), pkg.ReadInt());
			}
		}

		private void HandleAddEliteScore(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddEliteScore(pkg.Parameter1);
			}
		}

		private void HandleRemoveEliteScore(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.RemoveEliteScore(pkg.Parameter1);
			}
		}

		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters parameters = default(RSAParameters);
			parameters.Modulus = packet.ReadBytes(128);
			parameters.Exponent = packet.ReadBytes();
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(parameters);
			SendRSALogin(rsa, m_key);
		}

		private void HandlePlayerChatSend(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.SendMessage(pkg.ReadString());
			}
		}

		protected void HandleSendToPlayer(GSPacketIn pkg)
		{
			int playerId = pkg.ClientID;
			try
			{
				GSPacketIn inner = pkg.ReadPacket();
				m_server.SendToUser(playerId, inner);
			}
			catch (Exception ex)
			{
				log.Error(string.Format("pkg len:{0}", pkg.Length), ex);
				log.Error(Marshal.ToHexDump("pkg content:", pkg.Buffer, 0, pkg.Length));
			}
		}

		private void HandleUpdatePlayerGameId(GSPacketIn pkg)
		{
			m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
		}

		private void HandleDisconnectPlayer(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.Disconnect();
			}
		}

		private void HandlePlayerOnGameOver(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null && player.CurrentRoom != null && player.CurrentRoom.Game != null)
			{
				player.OnGameOver(player.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadBoolean(), pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
			}
		}

		private void HandlePlayerOnUsingItem(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				int templateId = pkg.ReadInt();
				bool result = player.UsePropItem(null, pkg.Parameter1, pkg.Parameter2, templateId, pkg.ReadBoolean());
				SendUsingPropInGame(player.CurrentRoom.Game.Id, player.GamePlayerId, templateId, result);
			}
		}

		private void HandlePlayerAddGold(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddGold(pkg.Parameter1);
			}
		}

		private void HandlePlayerAddGP(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddGP(pkg.Parameter1);
			}
		}

		private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			AbstractGame game = player.CurrentRoom.Game;
			if (player != null)
			{
				player.OnKillingLiving(game, pkg.ReadInt(), pkg.ClientID, pkg.ReadBoolean(), pkg.ReadInt());
			}
		}

		private void HandlePlayerOnMissionOver(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			AbstractGame game = player.CurrentRoom.Game;

			if (player != null)
			{
				player.OnMissionOver(game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
			}
		}

		private void HandlePlayerConsortiaFight(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			Dictionary<int, Player> players = new Dictionary<int, Player>();
			int consortiaWin = pkg.ReadInt();
			int consortiaLose = pkg.ReadInt();
			int count = pkg.ReadInt();
			int offer = 0;
			for (int i = 0; i < count; i++)
			{
				GamePlayer temp = WorldMgr.GetPlayerById(pkg.ReadInt());
				if (temp != null)
				{
					Player tempplayer = new Player(temp, 0, null, 0, temp.PlayerCharacter.hp);
					players.Add(i, tempplayer);
				}
			}

			eRoomType roomtype = (eRoomType)pkg.ReadByte();
			eGameType gametype = (eGameType)pkg.ReadByte();
			int totalKillHealth = pkg.ReadInt();
			if (player != null)
			{
				offer = player.ConsortiaFight(consortiaWin, consortiaLose, players, roomtype, gametype, totalKillHealth,
					count);
			}

			if (offer != 0)
			{
			}
		}

		private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.SendConsortiaFight(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadString());
			}
		}

		private void HandlePlayerRemoveGold(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.RemoveGold(pkg.ReadInt());
			}
		}

		private void HandlePlayerRemoveMoney(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.RemoveMoney(pkg.ReadInt(), isConsume:false);
			}
		}

		private void HandlePlayerAddTemplate1(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				ItemTemplateInfo template = ItemMgr.FindItemTemplate(pkg.ReadInt());
				eBageType type = (eBageType)pkg.ReadByte();
				if (template != null)
				{
					int Count = pkg.ReadInt();
					ItemInfo item = ItemInfo.CreateFromTemplate(template, Count, (int)ItemAddType.FightGet);
					item.Count = Count;
					item.ValidDate = pkg.ReadInt();
					item.IsBinds = pkg.ReadBoolean();
					item.IsUsed = pkg.ReadBoolean();
					item.StrengthenLevel = pkg.ReadInt();
					item.AttackCompose = pkg.ReadInt();
					item.DefendCompose = pkg.ReadInt();
					item.AgilityCompose = pkg.ReadInt();
					item.LuckCompose = pkg.ReadInt();
					if (pkg.ReadBoolean())
					{
						GoldEquipTemplateInfo goldEquip = GoldEquipMgr.FindGoldEquipByTemplate(template.TemplateID);
						if (goldEquip != null)
						{
							ItemTemplateInfo temp = ItemMgr.FindItemTemplate(goldEquip.NewTemplateId);
							if (temp != null)
							{
								item.GoldEquip = temp;
								item.goldBeginTime = pkg.ReadDateTime();
								item.goldValidDate = pkg.ReadInt();
							}
						}
					}

					player.AddTemplate(item, type, item.Count, eGameView.BatleTypeGet);
				}
			}
		}

		private void HandlePlayerRemoveGP(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.RemoveGP(pkg.Parameter1);
			}
		}

		private void HandlePlayerRemoveOffer(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.RemoveOffer(pkg.ReadInt());
			}
		}

		private void HandlePlayerAddOffer(GSPacketIn pkg)
        {
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
            {
				player.AddOffer(pkg.ReadInt(), false);
            }			
        }

		private void HandlePlayerAddRobRiches(GSPacketIn pkg)
        {
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
            {
				player.AddRobRiches(pkg.Parameter1);
            }
        }

		protected void HandleRoomRemove(GSPacketIn packet)
		{
			m_server.RemoveRoomImp(packet.ClientID);
		}

		protected void HandleStartGame(GSPacketIn pkg)
		{
			ProxyGame game = new ProxyGame(pkg.Parameter2, this, (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt());
			m_server.StartGame(pkg.Parameter1, game);
		}

		protected void HandleSendToRoom(GSPacketIn pkg)
		{
			int roomId = pkg.ClientID;
			GSPacketIn inner = pkg.ReadPacket();
			m_server.SendToRoom(roomId, inner, pkg.Parameter1, pkg.Parameter2);
		}

		protected void HandleStopGame(GSPacketIn pkg)
		{
			int roomId = pkg.Parameter1;
			int gameId = pkg.Parameter2;
			m_server.StopGame(roomId, gameId);
		}

		private void HandlePlayerHealstone(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.RemoveHealstone();
			}
		}

		private void HandlePlayerAddMoney(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddMoney(pkg.Parameter1, (pkg.Parameter2 == 1) ? true : false);
			}
		}

		private void HandlePlayerAddGiftToken(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddGiftToken(pkg.Parameter1);
			}
		}

		private void HandlePlayerAddMedal(GSPacketIn pkg) //trminhpc
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddMedal(pkg.Parameter1);
			}
		}


		public void HandleFindConsortiaAlly(GSPacketIn pkg)
		{
			int state = ConsortiaMgr.FindConsortiaAlly(pkg.ReadInt(), pkg.ReadInt());
			SendFindConsortiaAlly(state, pkg.ReadInt());
		}

		private void HandlePlayerAddLeagueMoney(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddLeagueMoney(pkg.Parameter1);
			}
		}

		private void HandlePlayerAddPrestige(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddPrestige(pkg.ReadBoolean());
			}
		}

		private void HandlePlayerUpdateRestCount(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.UpdateRestCount();
			}
		}

		private void HandleFightNPC(GSPacketIn pkg)
		{
			int roomtype = pkg.ReadInt();
			int gametype = pkg.ReadInt();
			int npcId = pkg.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.Parameter1);
			if (playerById == null)
			{
				RingStationMgr.CreateAutoBot(roomtype, gametype, npcId);
				Console.WriteLine("Create autobot by default");
			}
			else
			{
				RingStationMgr.CreateAutoBot(playerById, roomtype, gametype, npcId);
				Console.WriteLine("Create autobot by " + playerById.PlayerCharacter.NickName);
			}
		}

		public void SendFindConsortiaAlly(int state, int gameid)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.FIND_CONSORTIA_ALLY, gameid);
			pkg.WriteInt(state);
			pkg.WriteInt((int)RateMgr.GetRate(eRateType.Riches_Rate));
			SendTCP(pkg);
		}

		private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_USE_PROP_INGAME, gameId);
			pkg.Parameter1 = playerId;
			pkg.Parameter2 = templateId;
			pkg.WriteBoolean(result);
			SendTCP(pkg);
		}

		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.LOGIN);
			pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), false));
			SendTCP(pkg);
		}

		public void HandleTakeCardTemp(GSPacketIn pkg)
		{
			WorldMgr.GetPlayerById(pkg.ClientID)?.OnTakeCard(pkg.ReadInt(), pkg.ReadInt(), pkg.ReadInt(), pkg.ReadInt());
		}

		private void method_3(GSPacketIn gspacketIn_0)
		{
			WorldMgr.GetPlayerById(gspacketIn_0.ClientID)?.AddEliteScore(gspacketIn_0.Parameter1);
		}

		private void method_4(GSPacketIn gspacketIn_0)
		{
			WorldMgr.GetPlayerById(gspacketIn_0.ClientID)?.RemoveEliteScore(gspacketIn_0.Parameter1);
		}

		private void method_7(GSPacketIn gspacketIn_0)
		{
			WorldMgr.GetPlayerById(gspacketIn_0.ClientID)?.OnFightOneBloodIsWin((eRoomType)gspacketIn_0.Parameter1, isWin: true);
		}

		private void method_8(GSPacketIn gspacketIn_0)
		{
			WorldMgr.GetPlayerById(gspacketIn_0.ClientID)?.OnFightAddOffer(gspacketIn_0.Parameter1);
		}

		private void method_9(GSPacketIn gspacketIn_0)
		{
			WorldMgr.GetPlayerById(gspacketIn_0.ClientID)?.SendMessage(gspacketIn_0.ReadString());
		}

		private void method_10(GSPacketIn gspacketIn_0)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(gspacketIn_0.ClientID);
			AbstractGame game = playerById.CurrentRoom.Game;
			playerById?.OnKillingLiving(game, gspacketIn_0.ReadInt(), gspacketIn_0.ClientID, gspacketIn_0.ReadBoolean(), gspacketIn_0.ReadInt());
		}

		private void method_11(GSPacketIn gspacketIn_0)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(gspacketIn_0.ClientID);
			AbstractGame game = playerById.CurrentRoom.Game;
			playerById?.OnMissionOver(game, gspacketIn_0.ReadBoolean(), gspacketIn_0.ReadInt(), gspacketIn_0.ReadInt());
		}

		private void method_12(GSPacketIn gspacketIn_0)
		{
			GamePlayer playerById1 = WorldMgr.GetPlayerById(gspacketIn_0.ClientID);
			Dictionary<int, Player> players = new Dictionary<int, Player>();
			int consortiaWin = gspacketIn_0.ReadInt();
			int consortiaLose = gspacketIn_0.ReadInt();
			int count = gspacketIn_0.ReadInt();
			for (int key = 0; key < count; key++)
			{
				GamePlayer playerById2 = WorldMgr.GetPlayerById(gspacketIn_0.ReadInt());
				if (playerById2 != null)
				{
					Player player = new Player(playerById2, 0, null, 0, playerById2.PlayerCharacter.hp);
					players.Add(key, player);
				}
			}
			eRoomType roomType = (eRoomType)gspacketIn_0.ReadByte();
			eGameType gameClass = (eGameType)gspacketIn_0.ReadByte();
			int totalKillHealth = gspacketIn_0.ReadInt();
			playerById1?.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
		}


		private void method_25(int int_2, int int_3, int int_4, bool bool_4)
		{
			GSPacketIn pkg = new GSPacketIn(36, int_2);
			pkg.Parameter1 = int_3;
			pkg.Parameter2 = int_4;
			pkg.WriteBoolean(bool_4);
			SendTCP(pkg);
		}

		public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.DISCONNECT, gameId);
			pkg.Parameter1 = playerId;
			SendTCP(pkg);
		}

		//private void HandlePlayerOnGameOver(GSPacketIn pkg)
		//{
		//	GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
		//	if (player != null && player.CurrentRoom != null && player.CurrentRoom.Game != null)
		//	{
		//		player.OnGameOver(player.CurrentRoom.Game, pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadBoolean(), pkg.ReadBoolean(), pkg.ReadInt(), pkg.ReadInt());
		//	}
		//}

		private void method_27(GSPacketIn gspacketIn_0)
		{
			WorldMgr.GetPlayerById(gspacketIn_0.ClientID)?.Disconnect();
		}

		public FightServerConnector(BattleServer server, string ip, int port, string key) : base(ip, port, autoReconnect: true, new byte[8192], new byte[8192])
		{
			m_server = server;
			m_key = key;
			base.Strict = true;
		}

		public void SendAddRoom(BaseRoom room)
		{
			GSPacketIn pkg = new GSPacketIn(64);
			pkg.WriteInt(room.RoomId);
			pkg.WriteInt((int)room.RoomType);
			pkg.WriteInt((int)room.GameType);
			pkg.WriteInt(room.GuildId);
			pkg.WriteInt(room.PickUpNpcId);
			pkg.WriteBoolean(room.StartWithNpc);
			pkg.WriteBoolean(val: false);
			pkg.WriteBoolean(room.isCrosszone);
			List<GamePlayer> players = room.GetPlayers();
			pkg.WriteInt(players.Count);
			bool OpenCanBang = Convert.ToBoolean(ConfigurationManager.AppSettings["CanBangLucChien"]);
			bool isCanBang = room.RoomType == eRoomType.Match;
			List<string> ConfigMeet = ConfigurationManager.AppSettings["MeetRoom"].Split('|').ToList();
			foreach (GamePlayer gamePlayer in players)
			{
				pkg.WriteInt(gamePlayer.PlayerCharacter.ID);
				pkg.WriteInt(gamePlayer.ZoneId);
				pkg.WriteString(gamePlayer.ZoneName);
				pkg.WriteBoolean(gamePlayer.IsQuanChien);
				pkg.WriteInt(gamePlayer.Place);
				pkg.WriteInt(gamePlayer.CurrentEnemyId);
				pkg.WriteString(gamePlayer.PlayerCharacter.NickName);
				pkg.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
				pkg.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
				pkg.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
				pkg.WriteInt(gamePlayer.PlayerCharacter.Hide);
				pkg.WriteString(gamePlayer.PlayerCharacter.Style);
				pkg.WriteString(gamePlayer.PlayerCharacter.Colors);
				pkg.WriteString(gamePlayer.PlayerCharacter.Skin);
				pkg.WriteInt(gamePlayer.PlayerCharacter.Offer);
				pkg.WriteInt(gamePlayer.PlayerCharacter.GP);
				pkg.WriteInt(gamePlayer.PlayerCharacter.Grade);
				pkg.WriteInt(gamePlayer.PlayerCharacter.Repute);
				pkg.WriteInt(gamePlayer.PlayerCharacter.ConsortiaID);
				pkg.WriteString(gamePlayer.PlayerCharacter.ConsortiaName);
				pkg.WriteInt(gamePlayer.PlayerCharacter.ConsortiaLevel);
				pkg.WriteInt(gamePlayer.PlayerCharacter.ConsortiaRepute);
				pkg.WriteBoolean(gamePlayer.PlayerCharacter.IsShowConsortia);
				pkg.WriteInt(gamePlayer.PlayerCharacter.badgeID);
				pkg.WriteString(gamePlayer.PlayerCharacter.Honor);
				pkg.WriteInt(gamePlayer.PlayerCharacter.AchievementPoint);
				pkg.WriteString(gamePlayer.PlayerCharacter.WeaklessGuildProgressStr);
				pkg.WriteInt(gamePlayer.PlayerCharacter.MoneyPlus);
				pkg.WriteInt(gamePlayer.PlayerCharacter.FightPower);
				pkg.WriteInt(gamePlayer.PlayerCharacter.Nimbus);
				pkg.WriteInt(gamePlayer.PlayerCharacter.apprenticeshipState);
				pkg.WriteInt(gamePlayer.PlayerCharacter.masterID);
				pkg.WriteString(gamePlayer.PlayerCharacter.masterOrApprentices);
                pkg.WriteInt(gamePlayer.PlayerCharacter.Attack);
                pkg.WriteInt(gamePlayer.PlayerCharacter.Defence);
                pkg.WriteInt(gamePlayer.PlayerCharacter.Agility);
                pkg.WriteInt(gamePlayer.PlayerCharacter.Luck);
                pkg.WriteInt(gamePlayer.PlayerCharacter.hp);
                pkg.WriteDouble(gamePlayer.GetBaseAttack());
                pkg.WriteDouble(gamePlayer.GetBaseDefence());
                pkg.WriteDouble(gamePlayer.GetBaseAgility());
                pkg.WriteDouble(gamePlayer.GetBaseBlood());
                /*if (isCanBang && !OpenCanBang)
				{
					pkg.WriteInt(gamePlayer.PlayerCharacter.Attack);
					pkg.WriteInt(gamePlayer.PlayerCharacter.Defence);
					pkg.WriteInt(gamePlayer.PlayerCharacter.Agility);
					pkg.WriteInt(gamePlayer.PlayerCharacter.Luck);
					pkg.WriteInt(gamePlayer.PlayerCharacter.hp);
					pkg.WriteDouble(gamePlayer.GetBaseAttack());
					pkg.WriteDouble(gamePlayer.GetBaseDefence());
					pkg.WriteDouble(gamePlayer.GetBaseAgility());
					pkg.WriteDouble(gamePlayer.GetBaseBlood());
				}
				if(isCanBang && OpenCanBang)
                {
					pkg.WriteInt(int.Parse(ConfigMeet[0]));//Attack
					pkg.WriteInt(int.Parse(ConfigMeet[1]));//Defend
					pkg.WriteInt(int.Parse(ConfigMeet[2]));//Agility
					pkg.WriteInt(int.Parse(ConfigMeet[3]));//Luck
					pkg.WriteInt(int.Parse(ConfigMeet[4]));//Blood
					pkg.WriteDouble(int.Parse(ConfigMeet[5]));//GetBaseAttack
					pkg.WriteDouble(int.Parse(ConfigMeet[6]));//GetBaseDefence
					pkg.WriteDouble(int.Parse(ConfigMeet[7]));//GetBaseAgility
					pkg.WriteDouble(int.Parse(ConfigMeet[4]));//GetBaseBlood
				}*/
                pkg.WriteInt(gamePlayer.MainWeapon.TemplateID);
				pkg.WriteInt(gamePlayer.MainWeapon.StrengthenLevel);
				if (gamePlayer.MainWeapon.GoldEquip == null)
				{
					pkg.WriteInt(0);
				}
				else
				{
					pkg.WriteInt(gamePlayer.MainWeapon.GoldEquip.TemplateID);
					pkg.WriteDateTime(gamePlayer.MainWeapon.goldBeginTime);
					pkg.WriteInt(gamePlayer.MainWeapon.goldValidDate);
				}
				pkg.WriteBoolean(gamePlayer.CanUseProp);
				if (gamePlayer.SecondWeapon != null)
				{
					pkg.WriteInt(gamePlayer.SecondWeapon.TemplateID);
					pkg.WriteInt(gamePlayer.SecondWeapon.StrengthenLevel);
				}
				else
				{
					pkg.WriteInt(0);
					pkg.WriteInt(0);
				}
				if (gamePlayer.Healstone != null)
				{
					pkg.WriteInt(gamePlayer.Healstone.TemplateID);
					pkg.WriteInt(gamePlayer.Healstone.Count);
				}
				else
				{
					pkg.WriteInt(0);
					pkg.WriteInt(0);
				}
				pkg.WriteDouble((gamePlayer.GPAddPlus == 0.0) ? 1.0 : gamePlayer.GPAddPlus);
				pkg.WriteDouble((gamePlayer.OfferAddPlus == 0.0) ? 1.0 : gamePlayer.OfferAddPlus);
				pkg.WriteDouble(gamePlayer.GPApprenticeOnline);
				pkg.WriteDouble(gamePlayer.GPApprenticeTeam);
				pkg.WriteDouble(gamePlayer.GPSpouseTeam);
				pkg.WriteInt(GameServer.Instance.Configuration.ServerID);
				List<AbstractBuffer> allBuffer = gamePlayer.BufferList.GetAllBuffer();
				pkg.WriteInt(allBuffer.Count);
				foreach (AbstractBuffer item in allBuffer)
				{
					BufferInfo info = item.Info;
					pkg.WriteInt(info.Type);
					pkg.WriteBoolean(info.IsExist);
					pkg.WriteDateTime(info.BeginDate);
					pkg.WriteInt(info.ValidDate);
					pkg.WriteInt(info.Value);
				}
				pkg.WriteInt(gamePlayer.EquipEffect.Count);
				foreach (int val in gamePlayer.EquipEffect)
				{
					pkg.WriteInt(val);
				}
				pkg.WriteInt(gamePlayer.FightBuffs.Count);
				foreach (BufferInfo fightBuff in gamePlayer.FightBuffs)
				{
					pkg.WriteInt(fightBuff.Type);
					pkg.WriteInt(fightBuff.Value);
				}
				pkg.WriteString(gamePlayer.TcpEndPoint());
				pkg.WriteDateTime(gamePlayer.PlayerCharacter.VIPExpireDay);
				pkg.WriteBoolean(gamePlayer.PlayerCharacter.DailyLeagueFirst);
				pkg.WriteInt(gamePlayer.PlayerCharacter.DailyLeagueLastScore);
				bool val2 = gamePlayer.Pet != null;
				pkg.WriteBoolean(val2);
				if (val2)
				{
					pkg.WriteInt(gamePlayer.Pet.Place);
					pkg.WriteInt(gamePlayer.Pet.TemplateID);
					pkg.WriteInt(gamePlayer.Pet.ID);
					pkg.WriteString(gamePlayer.Pet.Name);
					pkg.WriteInt(gamePlayer.Pet.UserID);
					pkg.WriteInt(gamePlayer.Pet.Level);
					pkg.WriteString(gamePlayer.Pet.Skill);
					pkg.WriteString(gamePlayer.Pet.SkillEquip);
				}
                pkg.WriteInt(gamePlayer.CardBuff.Count);
                foreach (int id in gamePlayer.CardBuff)
                {
                    pkg.WriteInt(id);
                }
            }
			SendTCP(pkg);
		}

		public void SendRemoveRoom(BaseRoom room)
		{
			SendTCP(new GSPacketIn(65)
			{
				Parameter1 = room.RoomId
			});
		}

		public void SendToGame(int gameId, GSPacketIn pkg)
		{
			GSPacketIn wrapper = new GSPacketIn((int)eFightPackageType.SEND_TO_GAME, gameId);
			wrapper.WritePacket(pkg);
			SendTCP(wrapper);
		}

		private void method_29(GSPacketIn gspacketIn_0)
		{
			m_server.UpdatePlayerGameId(gspacketIn_0.Parameter1, gspacketIn_0.Parameter2);
		}

		public void SendChatMessage(string msg, GamePlayer player, bool team)
		{
			GSPacketIn pkg = new GSPacketIn(19, player.CurrentRoom.Game.Id);
			pkg.WriteInt(player.GamePlayerId);
			pkg.WriteBoolean(team);
			pkg.WriteString(msg);
			SendTCP(pkg);
		}

		public void SendFightNotice(GamePlayer player, int GameId)
		{
			SendTCP(new GSPacketIn(3, GameId)
			{
				Parameter1 = player.GameId
			});
		}

		private void HandlePlayerAddleageScore(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			if (player != null)
			{
				player.AddLeageScore(pkg.ReadBoolean(), pkg.Parameter1);
			}
		}

		static FightServerConnector()
		{
			log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}