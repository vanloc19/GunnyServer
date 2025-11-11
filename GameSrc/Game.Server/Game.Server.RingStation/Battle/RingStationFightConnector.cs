using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;

namespace Game.Server.RingStation.Battle
{
	public class RingStationFightConnector : BaseConnector
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string m_key;

		private RingStationBattleServer m_server;

		public RingStationFightConnector(RingStationBattleServer server, string ip, int port, string key)
			: base(ip, port, autoReconnect: true, new byte[8192], new byte[8192])
		{
			m_server = server;
			m_key = key;
			base.Strict = true;
		}

		protected void AsynProcessPacket(object state)
		{
			try
			{
				GSPacketIn pkg = state as GSPacketIn;
				switch (pkg.Code)
				{
                    #region OLD
                    case 19:
                        HandlePlayerChatSend(pkg);
                        break;
                    case 20:
                    case 21:
                    case 22:
                    case 23:
                    case 24:
                    case 25:
                    case 26:
                    case 27:
                    case 28:
                    case 29:
                    case 30:
                    case 31:
                    case 37:
                    case 46:
                    case 47:
                    case 54:
                    case 55:
                    case 56:
                    case 57:
                    case 58:
                    case 59:
                    case 60:
                    case 61:
                    case 62:
                    case 63:
                    case 64:
                    case 71:
                    case 72:
                        break;
                    case 32:
                        HandleSendToPlayer(pkg);
                        break;
                    case 33:
                        HandleUpdatePlayerGameId(pkg);
                        break;
                    case 34:
                        HandleDisconnectPlayer(pkg);
                        break;
                    case 35:
                        HandlePlayerOnGameOver(pkg);
                        break;
                    case 36:
                        HandlePlayerOnUsingItem(pkg);
                        break;
                    case 38:
                        HandlePlayerAddGold(pkg);
                        break;
                    case 39:
                        HandlePlayerAddGP(pkg);
                        break;
                    case 40:
                        HandlePlayerOnKillingLiving(pkg);
                        break;
                    case 41:
                        HandlePlayerOnMissionOver(pkg);
                        break;
                    case 42:
                        HandlePlayerConsortiaFight(pkg);
                        break;
                    case 43:
                        HandlePlayerSendConsortiaFight(pkg);
                        break;
                    case 44:
                        HandlePlayerRemoveGold(pkg);
                        break;
                    case 45:
                        HandlePlayerRemoveMoney(pkg);
                        break;
                    case 48:
                        HandlePlayerAddTemplate1(pkg);
                        break;
                    case 49:
                        HandlePlayerRemoveGP(pkg);
                        break;
                    case 50:
                        HandlePlayerRemoveOffer(pkg);
                        break;
                    case 51:
                        HandlePlayerAddOffer(pkg);
                        break;
                    case 52:
                        HandPlayerAddRobRiches(pkg);
                        break;
                    case 53:
                        HandleClearBag(pkg);
                        break;
                    case 65:
                        HandleRoomRemove(pkg);
                        break;
                    case 66:
                        HandleStartGame(pkg);
                        break;
                    case 67:
                        HandleSendToRoom(pkg);
                        break;
                    case 68:
                        HandleStopGame(pkg);
                        break;
                    case 69:
                        HandleUpdateRoomId(pkg);
                        break;
                    case 70:
                        HandlePlayerRemove(pkg);
                        break;
                    case 73:
                        HandlePlayerHealstone(pkg);
                        break;
                    case 74:
                        HandlePlayerAddMoney(pkg);
                        break;
                    case 75:
                        HandlePlayerAddGiftToken(pkg);
                        break;
                    case 76:
                        HandlePlayerAddMedal(pkg);
                        break;
                    case 77:
                        HandleFindConsortiaAlly(pkg);
                        break;
                    case 0:
                        HandleRSAKey(pkg);
                        break;
                    case 84:
                        HandlePlayerAddLeagueMoney(pkg);
                        break;
                    case 85:
                        HandlePlayerAddPrestige(pkg);
                        break;
                    case 86:
                        HandlePlayerUpdateRestCount(pkg);
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 15:
                    case 16:
                    case 17:
                    case 18:
                    case 78:
                    case 79:
                    case 80:
                    case 81:
                    case 82:
                    case 83:
                        break;
                        #endregion
                }
            }
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}

		private void HandleClearBag(GSPacketIn pkg)
		{
		}

		private void HandleDisconnectPlayer(GSPacketIn pkg)
		{
		}

		public void HandleFindConsortiaAlly(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddGold(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddGP(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddGiftToken(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddLeagueMoney(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddMedal(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddMoney(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddPrestige(GSPacketIn pkg)
		{
		}

		private void HandlePlayerAddTemplate1(GSPacketIn pkg)
		{
		}

		private void HandlePlayerConsortiaFight(GSPacketIn pkg)
		{
		}

		private void HandlePlayerChatSend(GSPacketIn pkg)
		{
		}

		private void HandlePlayerHealstone(GSPacketIn pkg)
		{
		}

		private void HandlePlayerOnGameOver(GSPacketIn pkg)
		{
		}

		private void HandlePlayerOnKillingLiving(GSPacketIn pkg)
		{
		}

		private void HandlePlayerOnMissionOver(GSPacketIn pkg)
		{
		}

		private void HandlePlayerOnUsingItem(GSPacketIn pkg)
		{
		}

		private void HandlePlayerRemove(GSPacketIn pkg)
		{
		}

		private void HandlePlayerRemoveGold(GSPacketIn pkg)
		{
		}

		private void HandlePlayerRemoveGP(GSPacketIn pkg)
		{
		}

		private void HandlePlayerRemoveMoney(GSPacketIn pkg)
		{
		}

		private void HandlePlayerRemoveOffer(GSPacketIn pkg)
		{
		}

		private void HandlePlayerSendConsortiaFight(GSPacketIn pkg)
		{
		}

		private void HandlePlayerUpdateRestCount(GSPacketIn pkg)
		{
		}

		protected void HandleRoomRemove(GSPacketIn packet)
		{
			m_server.RemoveRoomImp(packet.ClientID);
		}

		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters rSAParameters = default(RSAParameters);
			rSAParameters.Modulus = packet.ReadBytes(128);
			rSAParameters.Exponent = packet.ReadBytes();
			RSAParameters parameters = rSAParameters;
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(parameters);
			SendRSALogin(rsa, m_key);
		}

		protected void HandleSendToPlayer(GSPacketIn pkg)
		{
		}

		protected void HandleSendToRoom(GSPacketIn pkg)
		{
			int clientID = pkg.ClientID;
			GSPacketIn @in = pkg.ReadPacket();
			m_server.SendToRoom(clientID, @in, pkg.Parameter1, pkg.Parameter2);
		}

		private void HandlePlayerAddOffer(GSPacketIn pkg)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ClientID);
			playerById?.AddOffer(pkg.Parameter1, IsRate: false);
		}

		protected void HandleStartGame(GSPacketIn pkg)
		{
			ProxyRingStationGame game = new ProxyRingStationGame(pkg.Parameter2, this, (eRoomType)pkg.ReadInt(), (eGameType)pkg.ReadInt(), pkg.ReadInt());
			m_server.StartGame(pkg.Parameter1, game);
		}

		protected void HandleStopGame(GSPacketIn pkg)
		{
			int roomId = pkg.Parameter1;
			int gameId = pkg.Parameter2;
			m_server.StopGame(roomId, gameId);
		}

		private void HandleUpdatePlayerGameId(GSPacketIn pkg)
		{
			m_server.UpdatePlayerGameId(pkg.Parameter1, pkg.Parameter2);
		}

		private void HandleUpdateRoomId(GSPacketIn pkg)
		{
		}

		private void HandPlayerAddRobRiches(GSPacketIn pkg)
		{
			GamePlayer player = WorldMgr.GetPlayerById(pkg.ClientID);
			int check = pkg.ReadInt();
			if (player != null && check == pkg.Parameter1)
			{
				player.AddRobRiches(pkg.Parameter1);
			}
		}

		protected override void OnDisconnect()
		{
			base.OnDisconnect();
		}

		public override void OnRecvPacket(GSPacketIn pkg)
		{
			ThreadPool.QueueUserWorkItem(AsynProcessPacket, pkg);
		}

		public void SendAddRoom(BaseRoomRingStation room)
		{
			GSPacketIn pkg = new GSPacketIn(64);
			pkg.WriteInt(room.RoomId);
			pkg.WriteInt(room.RoomType);
			pkg.WriteInt(room.GameType);
			pkg.WriteInt(room.GuildId);
			pkg.WriteInt(room.PickUpNpcId);
			pkg.WriteBoolean(val: false);
			pkg.WriteBoolean(room.IsAutoBot);
			pkg.WriteBoolean(val: false);
			List<RingStationGamePlayer> stationGamePlayerList = room.GetPlayers();
			pkg.WriteInt(stationGamePlayerList.Count);
			foreach (RingStationGamePlayer stationGamePlayer in stationGamePlayerList)
			{
				pkg.WriteInt(stationGamePlayer.ID);
				pkg.WriteInt(GameServer.Instance.Configuration.ZoneId);
				pkg.WriteString(GameServer.Instance.Configuration.ZoneName);
				pkg.WriteBoolean(false);//isQuanChien
				pkg.WriteInt(0);//Place;
				pkg.WriteInt(0);
				pkg.WriteString(stationGamePlayer.NickName);
				pkg.WriteByte(1);
				pkg.WriteInt(1);
				pkg.WriteBoolean(stationGamePlayer.Sex);
				pkg.WriteInt(stationGamePlayer.Hide);
				pkg.WriteString(stationGamePlayer.Style);
				pkg.WriteString(stationGamePlayer.Colors);
				pkg.WriteString(stationGamePlayer.Skin);
				pkg.WriteInt(stationGamePlayer.Offer);
				pkg.WriteInt(stationGamePlayer.GP);
				pkg.WriteInt(stationGamePlayer.Grade);
				pkg.WriteInt(stationGamePlayer.Repute);
				pkg.WriteInt(stationGamePlayer.ConsortiaID);
				pkg.WriteString(stationGamePlayer.ConsortiaName);
				pkg.WriteInt(stationGamePlayer.ConsortiaLevel);
				pkg.WriteInt(stationGamePlayer.ConsortiaRepute);
				pkg.WriteBoolean(val: false);
				pkg.WriteInt(stationGamePlayer.badgeID);
				pkg.WriteString(stationGamePlayer.Honor);
				pkg.WriteInt(0);
				pkg.WriteString(stationGamePlayer.WeaklessGuildProgressStr);
				pkg.WriteInt(0);
				pkg.WriteInt(stationGamePlayer.FightPower);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteString("");
				pkg.WriteInt(stationGamePlayer.Attack);
				pkg.WriteInt(stationGamePlayer.Defence);
				pkg.WriteInt(stationGamePlayer.Agility);
				pkg.WriteInt(stationGamePlayer.Luck);
				pkg.WriteInt(stationGamePlayer.hp);
				pkg.WriteDouble(stationGamePlayer.BaseAttack);
				pkg.WriteDouble(stationGamePlayer.BaseDefence);
				pkg.WriteDouble(stationGamePlayer.BaseAgility);
				pkg.WriteDouble(stationGamePlayer.BaseBlood);
				pkg.WriteInt(stationGamePlayer.TemplateID);
				pkg.WriteInt(stationGamePlayer.StrengthLevel);
				pkg.WriteInt(0);
				pkg.WriteBoolean(stationGamePlayer.CanUserProp);
				pkg.WriteInt(stationGamePlayer.SecondWeapon);
				pkg.WriteInt(stationGamePlayer.StrengthLevel);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteDouble(stationGamePlayer.GPAddPlus);
				pkg.WriteDouble(stationGamePlayer.GMExperienceRate);
				pkg.WriteDouble(0.0);
				pkg.WriteDouble(0.0);
				pkg.WriteDouble(0.0);
				pkg.WriteInt(RingStationConfiguration.ServerID);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				//pkg.WriteByte(stationGamePlayer.typeVIP);
				//pkg.WriteInt(stationGamePlayer.VIPLevel);
				pkg.WriteString("");
				pkg.WriteDateTime(DateTime.Now);
				pkg.WriteBoolean(val: false);
				pkg.WriteInt(0);
				pkg.WriteBoolean(val: false);
				pkg.WriteInt(0);
			}
			SendTCP(pkg);
		}

		public void SendChangeGameType()
		{
		}

		public void SendChatMessage()
		{
		}

		public void SendFightNotice()
		{
		}

		public void SendFindConsortiaAlly(int state, int gameid)
		{
		}

		public void SendKitOffPlayer(int playerid)
		{
		}

		public void SendPlayerDisconnet(int gameId, int playerId, int roomid)
		{
		}

		public void SendRemoveRoom(BaseRoomRingStation room)
		{
			GSPacketIn pkg = new GSPacketIn(65)
			{
				Parameter1 = room.RoomId
			};
			SendTCP(pkg);
		}

		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn pkg = new GSPacketIn(1);
			pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), fOAEP: false));
			SendTCP(pkg);
		}

		public void SendToGame(int gameId, GSPacketIn pkg)
		{
			GSPacketIn @in = new GSPacketIn(2, gameId);
			@in.WritePacket(pkg);
			SendTCP(@in);
		}

		private void SendUsingPropInGame(int gameId, int playerId, int templateId, bool result)
		{
		}
	}
}
