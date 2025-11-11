using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server
{
	public class WorldServerConnector : BaseConnector
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private string m_loginKey;

		private int m_serverId;

		public WorldServerConnector(string ip, int port, int serverid, string name, byte[] readBuffer, byte[] sendBuffer)
			: base(ip, port, autoReconnect: true, readBuffer, sendBuffer)
		{
			m_serverId = serverid;
			m_loginKey = $"{serverid},{name}";
			base.Strict = true;
		}

		protected void AsynProcessPacket(object state)
		{
			try
			{
				GSPacketIn packet = state as GSPacketIn;
				int code = packet.Code;
				if (code <= 117)
				{
					switch (code)
					{
						case 0:
							HandleRSAKey(packet);
							break;
						case 2:
							HandleKitoffPlayer(packet);
							break;
						case 3:
							HandleAllowUserLogin(packet);
							break;
						case 4:
							HandleUserOffline(packet);
							break;
						case 5:
							HandleUserOnline(packet);
							break;
						case 7:
							HandleASSState(packet);
							break;
						case 8:
							HandleConfigState(packet);
							break;
						case 9:
							HandleChargeMoney(packet);
							break;
						case 10:
							HandleSystemNotice(packet);
							break;
						case 13:
							HandleUpdatePlayerMarriedState(packet);
							break;
						case 14:
							HandleMarryRoomInfoToPlayer(packet);
							break;
						case 15:
							HandleShutdown(packet);
							break;
						case 19:
							HandleChatConsortia(packet);
							break;
						case 37:
							HandleChatPersonal(packet);
							break;
						case 38:
							HandleSysMess(packet);
							break;
						case 72:
							HandleBigBugle(packet);
							break;
						case 73:
							HandleCBugle(packet);
							break;
						case 117:
							HandleMailResponse(packet);
							break;
						case 329:
							HandleSanXiao(packet);
							break;
					}
				}
				else if (code <= 160)
				{
					switch (code)
					{
						case 128:
							HandleConsortiaResponse(packet);
							break;
						case 130:
							HandleConsortiaCreate(packet);
							break;
						case 158:
							HandleConsortiaFight(packet);
							break;
						case 160:
							HandleFriend(packet);
							break;
					}
				}
				else
				{
					switch (code)
					{
						case 177:
							HandleRate(packet);
							return;
						case 178:
							HandleMacroDrop(packet);
							return;
						case 179:
							return;
						case 180:
							HandleConsortiaBossInfo(packet);
							return;
						case 904:
							HandlerEliteGameStatusUpdate(packet);
							break;
						case 906:
							HandlerEliteGameUpdateRank(packet);
							break;
						case 907:
							HandleEliteGameRequestStart(packet);
							break;
						case 909:
							HandlerEliteGameRoundAdd(packet);
							break;
						case 911:
							HandlerEliteGameSynPlayers(packet);
							break;
						case 912:
							HandleEliteGameReload(packet);
							break;
					}
					if (code == 185)
					{
						HandleConsortiaBossSendAward(packet);
					}
				}
			}
			catch (Exception exception)
			{
				GameServer.log.Error("AsynProcessPacket", exception);
			}
		}

		protected void HandleAllowUserLogin(object stateInfo)
		{
			try
			{
				GSPacketIn obj = (GSPacketIn)stateInfo;
				int playerId = obj.ReadInt();
				if (!obj.ReadBoolean())
				{
					return;
				}
				GamePlayer player = LoginMgr.LoginClient(playerId);
				if (player != null)
				{
					if (player.Login())
					{
						SendUserOnline(playerId, player.PlayerCharacter.ConsortiaID);
						WorldMgr.OnPlayerOnline(playerId, player.PlayerCharacter.ConsortiaID);
					}
					else
					{
						player.Client.Disconnect();
						SendUserOffline(playerId, 0);
					}
				}
				else
				{
					SendUserOffline(playerId, 0);
				}
			}
			catch (Exception exception)
			{
				GameServer.log.Error("HandleAllowUserLogin", exception);
			}
		}

		public void HandleASSState(GSPacketIn packet)
		{
			bool aSSState = packet.ReadBoolean();
			AntiAddictionMgr.SetASSState(aSSState);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				player.Out.SendAASControl(aSSState, player.IsAASInfo, player.IsMinor); 
			}
		}

		protected void HandleBigBugle(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].Out.SendTCP(packet);
			}
		}

		protected void HandleCBugle(GSPacketIn pkg)
        {
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].Out.SendTCP(pkg);
			}
		}

		public void HandleConfigState(GSPacketIn packet)
		{
			bool aSSState = packet.ReadBoolean();
			AwardMgr.DailyAwardState = packet.ReadBoolean();
			AntiAddictionMgr.SetASSState(aSSState);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				player.Out.SendAASControl(aSSState, player.IsAASInfo, player.IsMinor);
			}
		}

		public void HandleConsortiaSkillUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			string consortiaName = packet.ReadString();
			int skillLevel = packet.ReadInt();

			ConsortiaMgr.ConsortiaSkillUpGrade(consortiaID, skillLevel);

			GamePlayer[] players = WorldMgr.GetAllPlayers();
			foreach (GamePlayer p in players)
			{
				if (p.PlayerCharacter.ConsortiaID == consortiaID)
				{
					p.PlayerCharacter.SkillLevel = skillLevel;
					p.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaAlly(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			int state = packet.ReadInt();
			ConsortiaMgr.UpdateConsortiaAlly(num, num2, state);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num || player.PlayerCharacter.ConsortiaID == num2)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaBanChat(GSPacketIn packet)
		{
			bool flag = packet.ReadBoolean();
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ID == num)
				{
					player.PlayerCharacter.IsBanChat = flag;
					player.Out.SendTCP(packet);
					break;
				}
			}
		}

		public void HandleConsortiaBossClose(ConsortiaInfo consortia)
		{
			SendToAllConsortiaMember(consortia, 1);
		}

		public void HandleConsortiaBossCreateBoss(ConsortiaInfo consortia)
		{
			SendToAllConsortiaMember(consortia, 0);
		}

		public void HandleConsortiaBossDie(ConsortiaInfo consortia)
		{
			SendToAllConsortiaMember(consortia, 2);
		}

		public void HandleConsortiaBossExtendAvailable(ConsortiaInfo consortia)
		{
			SendToAllConsortiaMember(consortia, 3);
		}

		public void HandleConsortiaBossInfo(GSPacketIn pkg)
		{
			ConsortiaInfo consortia = new ConsortiaInfo
			{
				ConsortiaID = pkg.ReadInt(),
				ChairmanID = pkg.ReadInt(),
				bossState = pkg.ReadByte(),
				endTime = pkg.ReadDateTime(),
				extendAvailableNum = pkg.ReadInt(),
				callBossLevel = pkg.ReadInt(),
				Level = pkg.ReadInt(),
				SmithLevel = pkg.ReadInt(),
				StoreLevel = pkg.ReadInt(),
				SkillLevel = pkg.ReadInt(),
				Riches = pkg.ReadInt(),
				LastOpenBoss = pkg.ReadDateTime(),
				MaxBlood = pkg.ReadLong(),
				TotalAllMemberDame = pkg.ReadLong(),
				IsBossDie = pkg.ReadBoolean(),
				RankList = new Dictionary<string, RankingPersonInfo>()
			};
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				RankingPersonInfo info2 = new RankingPersonInfo
				{
					Name = pkg.ReadString(),
					ID = pkg.ReadInt(),
					TotalDamage = pkg.ReadInt(),
					Honor = pkg.ReadInt(),
					Damage = pkg.ReadInt()
				};
				consortia.RankList.Add(info2.Name, info2);
			}
			switch (pkg.ReadByte())
			{
				case 180:
					SendToAllConsortiaMember(consortia, -1);
					break;
				case 182:
					HandleConsortiaBossExtendAvailable(consortia);
					break;
				case 183:
					HandleConsortiaBossCreateBoss(consortia);
					break;
				case 184:
					HandleConsortiaBossReload(consortia);
					break;
				case 187:
					HandleConsortiaBossClose(consortia);
					break;
				case 188:
					HandleConsortiaBossDie(consortia);
					break;
				case 181:
				case 185:
				case 186:
					break;
			}
		}

		public void HandleConsortiaBossReload(ConsortiaInfo consortia)
		{
			SendToAllConsortiaMember(consortia, -1);
		}

		public void HandleConsortiaBossSendAward(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				ConsortiaBossMgr.SendConsortiaAward(pkg.ReadInt());
			}
		}

		public void HandleConsortiaCreate(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			packet.ReadInt();
			ConsortiaMgr.AddConsortia(consortiaID);
		}

		public void HandleConsortiaDelete(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num)
				{
					//player.ClearConsortia(isclear: true);
					player.ClearConsortia();
					player.AddRobRiches(-player.PlayerCharacter.RichesRob);
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaDuty(GSPacketIn packet)
		{
			int num = packet.ReadByte();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			packet.ReadString();
			int num4 = packet.ReadInt();
			string str = packet.ReadString();
			int num5 = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num2)
				{
					if (num == 2 && player.PlayerCharacter.DutyLevel == num4)
					{
						player.PlayerCharacter.DutyName = str;
					}
					else if (player.PlayerCharacter.ID == num3 && (num == 5 || num == 6 || num == 7 || num == 8 || num == 9))
					{
						player.PlayerCharacter.DutyLevel = num4;
						player.PlayerCharacter.DutyName = str;
						player.PlayerCharacter.Right = num5;
					}
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaFight(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			packet.ReadInt();
			string message = packet.ReadString();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num)
				{
					player.Out.SendMessage(eMessageType.ChatNormal, message);
				}
			}
		}

		protected void HandleConsortiaResponse(GSPacketIn packet)
		{
			switch (packet.ReadByte())
			{
				case 1:
					HandleConsortiaUserPass(packet);
					break;
				case 2:
					HandleConsortiaDelete(packet);
					break;
				case 3:
					HandleConsortiaUserDelete(packet);
					break;
				case 4:
					HandleConsortiaUserInvite(packet);
					break;
				case 5:
					HandleConsortiaBanChat(packet);
					break;
				case 6:
					HandleConsortiaUpGrade(packet);
					break;
				case 7:
					HandleConsortiaAlly(packet);
					break;
				case 8:
					HandleConsortiaDuty(packet);
					break;
				case 9:
					HandleConsortiaRichesOffer(packet);
					break;
				case 10:
					HandleConsortiaShopUpGrade(packet);
					break;
				case 11:
					HandleConsortiaSmithUpGrade(packet);
					break;
				case 12:
					HandleConsortiaStoreUpGrade(packet);
					break;
				case 13:
					HandleConsortiaSkillUpGrade(packet);
					break;
			}
		}

		public void HandleConsortiaRichesOffer(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaShopUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			packet.ReadString();
			int shopLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaShopUpGrade(consortiaID, shopLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == consortiaID)
				{
					player.PlayerCharacter.ShopLevel = shopLevel;
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaSmithUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			packet.ReadString();
			int smithLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaSmithUpGrade(consortiaID, smithLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == consortiaID)
				{
					player.PlayerCharacter.SmithLevel = smithLevel;
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaStoreUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			packet.ReadString();
			int storeLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaStoreUpGrade(consortiaID, storeLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == consortiaID)
				{
					player.PlayerCharacter.StoreLevel = storeLevel;
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaUpGrade(GSPacketIn packet)
		{
			int consortiaID = packet.ReadInt();
			packet.ReadString();
			int consortiaLevel = packet.ReadInt();
			ConsortiaMgr.ConsortiaUpGrade(consortiaID, consortiaLevel);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == consortiaID)
				{
					player.PlayerCharacter.ConsortiaLevel = consortiaLevel;
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaUserDelete(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num2 || player.PlayerCharacter.ID == num)
				{
					if (player.PlayerCharacter.ID == num)
					{
						//player.ClearConsortia(isclear: true);
						player.ClearConsortia();
					}
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleConsortiaUserInvite(GSPacketIn packet)
		{
			packet.ReadInt();
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ID == num)
				{
					player.Out.SendTCP(packet);
					break;
				}
			}
		}

		public void HandleConsortiaUserPass(GSPacketIn packet)
		{
			packet.ReadInt();
			packet.ReadBoolean();
			int consortiaID = packet.ReadInt();
			string str = packet.ReadString();
			int num2 = packet.ReadInt();
			packet.ReadString();
			packet.ReadInt();
			packet.ReadString();
			packet.ReadInt();
			string str2 = packet.ReadString();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadDateTime();
			packet.ReadInt();
			int num3 = packet.ReadInt();
			packet.ReadInt();
			packet.ReadBoolean();
			int num4 = packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadInt();
			int num5 = packet.ReadInt();
			packet.ReadString();
			packet.ReadInt();
			packet.ReadInt();
			packet.ReadString();
			packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ID == num2)
				{
					player.BeginChanges();
					player.PlayerCharacter.ConsortiaID = consortiaID;
					player.PlayerCharacter.ConsortiaName = str;
					player.PlayerCharacter.DutyName = str2;
					player.PlayerCharacter.DutyLevel = num3;
					player.PlayerCharacter.Right = num4;
					player.PlayerCharacter.ConsortiaRepute = num5;
					ConsortiaInfo info = ConsortiaMgr.FindConsortiaInfo(consortiaID);
					if (info != null)
					{
						player.PlayerCharacter.ConsortiaLevel = info.Level;
					}
					player.CommitChanges();
				}
				if (player.PlayerCharacter.ConsortiaID == consortiaID)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		public void HandleChargeMoney(GSPacketIn packet)
		{
			WorldMgr.GetPlayerById(packet.ClientID)?.ChargeToUser();
		}

		protected void HandleChatConsortia(GSPacketIn packet)
		{
			packet.ReadByte();
			packet.ReadBoolean();
			packet.ReadString();
			packet.ReadString();
			int num = packet.ReadInt();
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == num)
				{
					player.Out.SendTCP(packet);
				}
			}
		}

		protected void HandleChatPersonal(GSPacketIn packet)
		{
			int receiverID = packet.ReadInt();
			string nickName = packet.ReadString();
			string str2 = packet.ReadString();
			string msg = packet.ReadString();
			bool isAutoReply = packet.ReadBoolean();
			int playerID = 0;
			GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(nickName);
			GamePlayer player2 = WorldMgr.GetClientByPlayerNickName(str2);
			if (player2 != null)
			{
				playerID = player2.PlayerCharacter.ID;
			}
			if (clientByPlayerNickName != null && !clientByPlayerNickName.IsBlackFriend(playerID))
			{
				receiverID = clientByPlayerNickName.PlayerCharacter.ID;
				clientByPlayerNickName.SendPrivateChat(receiverID, nickName, str2, msg, isAutoReply);
			}
		}

		public void HandleFirendResponse(GSPacketIn packet)
		{
			WorldMgr.GetPlayerById(packet.ReadInt())?.Out.SendTCP(packet);
		}

		public void HandleFriend(GSPacketIn pkg)
		{
			switch (pkg.ReadByte())
			{
				case 165:
					HandleFriendState(pkg);
					break;
				case 166:
					HandleFirendResponse(pkg);
					break;
			}
		}

		public void HandleFriendState(GSPacketIn pkg)
		{
			WorldMgr.ChangePlayerState(pkg.ClientID, pkg.ReadInt(), pkg.ReadInt());
		}

		protected void HandleKitoffPlayer(object stateInfo)
		{
			try
			{
				GSPacketIn @in = (GSPacketIn)stateInfo;
				int playerId = @in.ReadInt();
				GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
				if (playerById != null)
				{
					string msg = @in.ReadString();
					playerById.Out.SendKitoff(msg);
					playerById.Client.Disconnect();
				}
				else
				{
					SendUserOffline(playerId, 0);
				}
			}
			catch (Exception exception)
			{
				GameServer.log.Error("HandleKitoffPlayer", exception);
			}
		}

		public void HandleMacroDrop(GSPacketIn pkg)
		{
			Dictionary<int, MacroDropInfo> temp = new Dictionary<int, MacroDropInfo>();
			int num = pkg.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int key = pkg.ReadInt();
				int dropCount = pkg.ReadInt();
				int maxDropCount = pkg.ReadInt();
				MacroDropInfo info = new MacroDropInfo(dropCount, maxDropCount);
				temp.Add(key, info);
			}
			MacroDropMgr.UpdateDropInfo(temp);
		}

		public void HandleMailResponse(GSPacketIn packet)
		{
			WorldMgr.GetPlayerById(packet.ReadInt())?.Out.SendTCP(packet);
		}

		public void HandleMarryRoomInfoToPlayer(GSPacketIn packet)
		{
			int playerId = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null)
			{
				packet.Code = 252;
				packet.ClientID = playerId;
				playerById.Out.SendTCP(packet);
			}
		}

		public void HandleRate(GSPacketIn packet)
		{
			RateMgr.ReLoad();
		}

		public void HandleReload(GSPacketIn packet)
		{
			eReloadType type = (eReloadType)packet.ReadInt();
			bool val = false;
			switch (type)
			{
				case eReloadType.ball:
					val = BallMgr.ReLoad();
					break;
				case eReloadType.map:
					val = MapMgr.ReLoadMap();
					break;
				case eReloadType.mapserver:
					val = MapMgr.ReLoadMapServer();
					break;
				case eReloadType.item:
					val = ItemMgr.ReLoad();
					break;
				case eReloadType.quest:
					val = QuestMgr.ReLoad();
					break;
				case eReloadType.fusion:
					val = FusionMgr.ReLoad();
					break;
				case eReloadType.server:
					GameServer.Instance.Configuration.Refresh();
					break;
				case eReloadType.rate:
					val = RateMgr.ReLoad();
					break;
				case eReloadType.consortia:
					val = ConsortiaMgr.ReLoad();
					break;
				case eReloadType.shop:
					val = ShopMgr.ReLoad();
					break;
				case eReloadType.fight:
					val = FightRateMgr.ReLoad();
					break;
				case eReloadType.dailyaward:
					val = AwardMgr.ReLoad();
					break;
				case eReloadType.language:
					val = LanguageMgr.Reload("");
					break;
				case eReloadType.petmoeproperty:
					val = PetMoePropertyMgr.ReLoad();
					break;
				case eReloadType.accumulactivelogin:
					val = AccumulActiveLoginMgr.ReLoad();
					break;
				case eReloadType.newtitle:
					val = NewTitleMgr.ReLoad();
					break;
			}
			packet.WriteInt(GameServer.Instance.Configuration.ServerID);
			packet.WriteBoolean(val);
			SendTCP(packet);
		}

		protected void HandleRSAKey(GSPacketIn packet)
		{
			RSAParameters rSAParameters = default(RSAParameters);
			rSAParameters.Modulus = packet.ReadBytes(128);
			rSAParameters.Exponent = packet.ReadBytes();
			RSAParameters parameters = rSAParameters;
			RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
			rsa.ImportParameters(parameters);
			SendRSALogin(rsa, m_loginKey);
			SendListenIPPort(IPAddress.Parse(GameServer.Instance.Configuration.Ip), GameServer.Instance.Configuration.Port);
		}

		public void HandleShutdown(GSPacketIn pkg)
		{
			GameServer.Instance.Shutdown();
		}

		public void HandleSysMess(GSPacketIn packet)
		{
			if (packet.ReadInt() == 1)
			{
				int playerId = packet.ReadInt();
				string str = packet.ReadString();
				WorldMgr.GetPlayerById(playerId)?.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("WorldServerConnector.HandleSysMess.Msg1", str));
			}
		}

		public void HandleSystemNotice(GSPacketIn packet)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			for (int i = 0; i < allPlayers.Length; i++)
			{
				allPlayers[i].Out.SendTCP(packet);
			}
		}

		public void HandleUpdatePlayerMarriedState(GSPacketIn packet)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(packet.ReadInt());
			if (playerById != null)
			{
				playerById.LoadMarryProp();
				playerById.LoadMarryMessage();
				playerById.QuestInventory.ClearMarryQuest();
			}
		}

		protected void HandleUserOffline(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int playerId = packet.ReadInt();
				int consortiaID = packet.ReadInt();
				if (LoginMgr.ContainsUser(playerId))
				{
					SendAllowUserLogin(playerId);
				}
				WorldMgr.OnPlayerOffline(playerId, consortiaID);
			}
		}

		protected void HandleUserOnline(GSPacketIn packet)
		{
			int num = packet.ReadInt();
			for (int i = 0; i < num; i++)
			{
				int num2 = packet.ReadInt();
				int consortiaID = packet.ReadInt();
				LoginMgr.ClearLoginPlayer(num2);
				GamePlayer playerById = WorldMgr.GetPlayerById(num2);
				if (playerById != null)
				{
					GameServer.log.Error("Player hang in server!!!");
					playerById.Out.SendKitoff(LanguageMgr.GetTranslation("Game.Server.LoginNext"));
					playerById.Client.Disconnect();
				}
				WorldMgr.OnPlayerOnline(num2, consortiaID);
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

		public void SendAllowUserLogin(int playerid)
		{
			GSPacketIn pkg = new GSPacketIn(3);
			pkg.WriteInt(playerid);
			SendTCP(pkg);
		}

		public void SendConsortiaAlly(int consortiaID1, int consortiaID2, int state)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(7);
			pkg.WriteInt(consortiaID1);
			pkg.WriteInt(consortiaID2);
			pkg.WriteInt(state);
			SendTCP(pkg);
			ConsortiaMgr.UpdateConsortiaAlly(consortiaID1, consortiaID2, state);
		}

		public void SendConsortiaBanChat(int playerid, string playerName, int handleID, string handleName, bool isBan)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(5);
			pkg.WriteBoolean(isBan);
			pkg.WriteInt(playerid);
			pkg.WriteString(playerName);
			pkg.WriteInt(handleID);
			pkg.WriteString(handleName);
			SendTCP(pkg);
		}

		public void SendConsortiaCreate(int consortiaID, int offer, string consotiaName)
		{
			GSPacketIn pkg = new GSPacketIn(130);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(offer);
			pkg.WriteString(consotiaName);
			SendTCP(pkg);
		}

		public void SendConsortiaDelete(int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(2);
			pkg.WriteInt(consortiaID);
			SendTCP(pkg);
		}

		public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID)
		{
			SendConsortiaDuty(info, updateType, consortiaID, 0, "", 0, "");
		}

		public void SendConsortiaDuty(ConsortiaDutyInfo info, int updateType, int consortiaID, int playerID, string playerName, int handleID, string handleName)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(8);
			pkg.WriteByte((byte)updateType);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(playerID);
			pkg.WriteString(playerName);
			pkg.WriteInt(info.Level);
			pkg.WriteString(info.DutyName);
			pkg.WriteInt(info.Right);
			pkg.WriteInt(handleID);
			pkg.WriteString(handleName);
			SendTCP(pkg);
		}

		public void SendConsortiaFight(int consortiaID, int riches, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(158);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(riches);
			pkg.WriteString(msg);
			SendTCP(pkg);
		}

		public void SendConsortiaInvite(int ID, int playerid, string playerName, int inviteID, string intviteName, string consortiaName, int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(4);
			pkg.WriteInt(ID);
			pkg.WriteInt(playerid);
			pkg.WriteString(playerName);
			pkg.WriteInt(inviteID);
			pkg.WriteString(intviteName);
			pkg.WriteInt(consortiaID);
			pkg.WriteString(consortiaName);
			SendTCP(pkg);
		}

		public void SendConsortiaKillUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(13);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.SkillLevel);
			SendTCP(pkg);
		}

		public void SendConsortiaOffer(int consortiaID, int offer, int riches)
		{
			GSPacketIn pkg = new GSPacketIn(156);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(offer);
			pkg.WriteInt(riches);
			SendTCP(pkg);
		}

		public void SendConsortiaRichesOffer(int consortiaID, int playerID, string playerName, int riches)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(9);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(playerID);
			pkg.WriteString(playerName);
			pkg.WriteInt(riches);
			SendTCP(pkg);
		}

		public void SendConsortiaShopUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(10);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.ShopLevel);
			SendTCP(pkg);
		}

		public void SendConsortiaSmithUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(11);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.SmithLevel);
			SendTCP(pkg);
		}

		public void SendConsortiaStoreUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(12);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.StoreLevel);
			SendTCP(pkg);
		}

		public void SendConsortiaUpGrade(ConsortiaInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(6);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.Level);
			SendTCP(pkg);
		}

		public void SendConsortiaUserDelete(int playerid, int consortiaID, bool isKick, string nickName, string kickName)
		{
			GSPacketIn pkg = new GSPacketIn(128);
			pkg.WriteByte(3);
			pkg.WriteInt(playerid);
			pkg.WriteInt(consortiaID);
			pkg.WriteBoolean(isKick);
			pkg.WriteString(nickName);
			pkg.WriteString(kickName);
			SendTCP(pkg);
		}

		public void SendConsortiaUserPass(int playerid, string playerName, ConsortiaUserInfo info, bool isInvite, int consortiaRepute)
		{
			GSPacketIn pkg = new GSPacketIn(128, playerid);
			pkg.WriteByte(1);
			pkg.WriteInt(info.ID);
			pkg.WriteBoolean(isInvite);
			pkg.WriteInt(info.ConsortiaID);
			pkg.WriteString(info.ConsortiaName);
			pkg.WriteInt(info.UserID);
			pkg.WriteString(info.UserName);
			pkg.WriteInt(playerid);
			pkg.WriteString(playerName);
			pkg.WriteInt(info.DutyID);
			pkg.WriteString(info.DutyName);
			pkg.WriteInt(info.Offer);
			pkg.WriteInt(info.RichesOffer);
			pkg.WriteInt(info.RichesRob);
			pkg.WriteDateTime(info.LastDate);
			pkg.WriteInt(info.Grade);
			pkg.WriteInt(info.Level);
			pkg.WriteInt(info.State);
			pkg.WriteBoolean(info.Sex);
			pkg.WriteInt(info.Right);
			pkg.WriteInt(info.Win);
			pkg.WriteInt(info.Total);
			pkg.WriteInt(info.Escape);
			pkg.WriteInt(consortiaRepute);
			pkg.WriteString(info.LoginName);
			pkg.WriteInt(info.FightPower);
			pkg.WriteInt(info.AchievementPoint);
			pkg.WriteString(info.honor);
			pkg.WriteInt(info.UseOffer);
			SendTCP(pkg);
		}

		public void SendListenIPPort(IPAddress ip, int port)
		{
			GSPacketIn pkg = new GSPacketIn(240);
			pkg.Write(ip.GetAddressBytes());
			pkg.WriteInt(port);
			SendTCP(pkg);
		}

		public void SendMailResponse(int playerid)
		{
			GSPacketIn pkg = new GSPacketIn(117);
			pkg.WriteInt(playerid);
			SendTCP(pkg);
		}

		public void SendMarryRoomDisposeToPlayer(int roomId)
		{
			GSPacketIn pkg = new GSPacketIn(241);
			pkg.WriteInt(roomId);
			SendTCP(pkg);
		}

		public void SendMarryRoomInfoToPlayer(int playerId, bool state, MarryRoomInfo info)
		{
			GSPacketIn pkg = new GSPacketIn(14);
			pkg.WriteInt(playerId);
			pkg.WriteBoolean(state);
			if (state)
			{
				pkg.WriteInt(info.ID);
				pkg.WriteString(info.Name);
				pkg.WriteInt(info.MapIndex);
				pkg.WriteInt(info.AvailTime);
				pkg.WriteInt(info.PlayerID);
				pkg.WriteInt(info.GroomID);
				pkg.WriteInt(info.BrideID);
				pkg.WriteDateTime(info.BeginTime);
				pkg.WriteBoolean(info.IsGunsaluteUsed);
			}
			SendTCP(pkg);
		}

		public void SendPacket(GSPacketIn packet)
		{
			SendTCP(packet);
		}

		public void SendPingCenter()
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			int val = ((allPlayers != null) ? allPlayers.Length : 0);
			GSPacketIn pkg = new GSPacketIn(12);
			pkg.WriteInt(val);
			SendTCP(pkg);
		}

		public void SendRSALogin(RSACryptoServiceProvider rsa, string key)
		{
			GSPacketIn pkg = new GSPacketIn(1);
			pkg.Write(rsa.Encrypt(Encoding.UTF8.GetBytes(key), fOAEP: false));
			SendTCP(pkg);
		}

		public void SendShutdown(bool isStoping)
		{
			GSPacketIn pkg = new GSPacketIn(15);
			pkg.WriteInt(m_serverId);
			pkg.WriteBoolean(isStoping);
			SendTCP(pkg);
		}

		public void SendToAllConsortiaMember(ConsortiaInfo consortia, int type)
		{
			if (!ConsortiaBossMgr.AddConsortia(consortia.ConsortiaID, consortia))
			{
				ConsortiaBossMgr.UpdateConsortia(consortia);
			}
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer player in allPlayers)
			{
				if (player.PlayerCharacter.ConsortiaID == consortia.ConsortiaID)
				{
					player.SendConsortiaBossInfo(consortia);
					switch (type)
					{
						case 0:
							player.SendConsortiaBossOpenClose(0);
							break;
						case 1:
							player.SendConsortiaBossOpenClose(1);
							break;
						case 2:
							player.SendConsortiaBossOpenClose(2);
							break;
						case 3:
							player.SendConsortiaBossOpenClose(3);
							break;
					}
				}
			}
		}

		public void SendUpdatePlayerMarriedStates(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn(13);
			pkg.WriteInt(playerId);
			SendTCP(pkg);
		}

		public GSPacketIn SendUserOffline(int playerid, int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(4);
			pkg.WriteInt(1);
			pkg.WriteInt(playerid);
			pkg.WriteInt(consortiaID);
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendUserOnline(Dictionary<int, int> users)
		{
			GSPacketIn pkg = new GSPacketIn(5);
			pkg.WriteInt(users.Count);
			foreach (KeyValuePair<int, int> pair in users)
			{
				pkg.WriteInt(pair.Key);
				pkg.WriteInt(pair.Value);
			}
			SendTCP(pkg);
			return pkg;
		}

		public GSPacketIn SendUserOnline(int playerid, int consortiaID)
		{
			GSPacketIn pkg = new GSPacketIn(5);
			pkg.WriteInt(1);
			pkg.WriteInt(playerid);
			pkg.WriteInt(consortiaID);
			SendTCP(pkg);
			return pkg;
		}

		public void SendEliteChampionBattleStatus(int userId, bool isReady)
		{
			GSPacketIn pkg = new GSPacketIn(910);
			pkg.WriteInt(userId);
			pkg.WriteBoolean(isReady);
			SendTCP(pkg);
		}

		public void SendEliteScoreUpdate(int playerId, string NickName, int type, int score)
		{
			GSPacketIn pkg = new GSPacketIn(905);
			pkg.WriteInt(playerId);
			pkg.WriteString(NickName);
			pkg.WriteInt(type);
			pkg.WriteInt(score);
			SendTCP(pkg);
		}

		public void SendEliteChampionRoundUpdate(EliteGameRoundInfo round)
		{
			GSPacketIn pkg = new GSPacketIn(908);
			pkg.WriteInt(round.RoundID);
			pkg.WriteInt(round.RoundType);
			pkg.WriteInt(round.PlayerWin.UserID);
			SendTCP(pkg);
		}

		public void HandleEliteGameReload(GSPacketIn pkg)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer obj in allPlayers)
			{
				obj.PlayerCharacter.EliteScore = 1000;
				obj.PlayerCharacter.EliteRank = 0;
			}
			ExerciseMgr.ResetEliteGame();
		}

		public void HandleEliteGameRequestStart(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			for (int index = 0; index < num; index++)
			{
				WorldMgr.GetPlayerById(pkg.ReadInt())?.Out.SendEliteGameStartRoom();
			}
		}

		public void HandlerEliteGameSynPlayers(GSPacketIn pkg)
		{
			int num = pkg.ReadInt();
			for (int index = 0; index < num; index++)
			{
				ExerciseMgr.UpdateEliteGameChapionPlayerList(new PlayerEliteGameInfo
				{
					UserID = pkg.ReadInt(),
					NickName = pkg.ReadString(),
					GameType = pkg.ReadInt(),
					Status = pkg.ReadInt(),
					Winer = pkg.ReadInt(),
					Rank = pkg.ReadInt(),
					CurrentPoint = pkg.ReadInt()
				});
			}
		}

		public void HandlerEliteGameStatusUpdate(GSPacketIn pkg)
		{
			ExerciseMgr.EliteStatus = pkg.ReadInt();
		}

		public void HandlerEliteGameRoundAdd(GSPacketIn pkg)
		{
			ExerciseMgr.AddEliteRound(new EliteGameRoundInfo
			{
				RoundID = pkg.ReadInt(),
				RoundType = pkg.ReadInt(),
				PlayerOne = new PlayerEliteGameInfo
				{
					UserID = pkg.ReadInt()
				},
				PlayerTwo = new PlayerEliteGameInfo
				{
					UserID = pkg.ReadInt()
				}
			});
		}

		public void HandlerEliteGameUpdateRank(GSPacketIn pkg)
		{
			pkg.UnCompress();
			int num = pkg.ReadInt();
			for (int index = 0; index < num; index++)
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(pkg.ReadInt());
				if (playerById != null)
				{
					playerById.PlayerCharacter.EliteRank = pkg.ReadInt();
				}
				else
				{
					pkg.ReadInt();
				}
			}
		}

		public void HandleSanXiao(GSPacketIn pkg)
		{
			GamePlayer p = WorldMgr.GetPlayerById(pkg.ClientID);
			if (p != null)
			{
				pkg.ClearOffset();

				GSPacketIn tempPkg = pkg.Clone();
				tempPkg.ClearOffset();

				byte code = tempPkg.ReadByte();

				if (code == (byte)QiYuanPackageType.PACK_TYPE_OPEN_BOGU_BOX)
				{
					p.DDTQiYuan.ProcessData(p, pkg);
				}
				else
				{
					p.SendTCP(pkg);
				}
			}
		}
	}
}
