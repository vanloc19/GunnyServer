using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Fighting.Server.GameObjects;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Logic.Protocol;
using log4net;
using SqlDataProvider.Data;

namespace Fighting.Server
{
	public class ServerClient : BaseClient
	{
		private static readonly ILog ilog_1;

		private RSACryptoServiceProvider rsacryptoServiceProvider_0;

		private FightServer m_svr;

		private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

		protected override void OnConnect()
		{
			base.OnConnect();
			rsacryptoServiceProvider_0 = new RSACryptoServiceProvider();
			RSAParameters rsaParameters = rsacryptoServiceProvider_0.ExportParameters(includePrivateParameters: false);
			SendRSAKey(rsaParameters.Modulus, rsaParameters.Exponent);
		}

		protected override void OnDisconnect()
		{
			base.OnDisconnect();
			rsacryptoServiceProvider_0 = null;
		}

		public override void OnRecvPacket(GSPacketIn pkg)
		{
			int code = pkg.Code;
			switch (code)
			{
				case (int)eFightPackageType.LOGIN:
					HandleLogin(pkg);
					break;
				case (int)eFightPackageType.SEND_TO_GAME:
					HanleSendToGame(pkg);
					break;
				case (int)eFightPackageType.SYS_NOTICE:
					HandleSysNotice(pkg);
					break;
				case (int)eFightPackageType.CHAT:
					HandlePlayerMessage(pkg);
					break;
				case (int)eFightPackageType.PLAYER_USE_PROP_INGAME:
					HandlePlayerUsingProp(pkg);
					break;
				case (int)eFightPackageType.ROOM_CREATE:
					HandleGameRoomCreate(pkg);
					break;
				case (int)eFightPackageType.ROOM_REMOVE:
					HandleGameRoomCancel(pkg);
					break;
				case (int)eFightPackageType.FIND_CONSORTIA_ALLY:
					HandleConsortiaAlly(pkg);
					break;
				case (int)eFightPackageType.DISCONNECT:
					HandlePlayerExit(pkg);
					break;
				default:
					Console.WriteLine("??????????ServerClient: " + (eFightPackageType)code);
					break;
			}
		}

		private void HandlePlayerExit(GSPacketIn pkg)
        {
            BaseGame game = GameMgr.FindGame(pkg.ClientID);
            if (game != null)
            {
                Player player = game.FindPlayer(pkg.Parameter1);
                if (player != null)
                {
					GSPacketIn pkg1 = new GSPacketIn(83, player.PlayerDetail.PlayerCharacter.ID);
                    game.SendToAll(pkg1);
                    game.RemovePlayer(player.PlayerDetail, false);

                    #region group 1

                    ProxyRoom room1 = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Red.RoomId);
                    if (room1 != null)
                    {
                        if (!room1.RemovePlayer(player.PlayerDetail))
                        {
                            ProxyRoom room11 = ProxyRoomMgr.GetRoomUnsafe((game as BattleGame).Blue.RoomId);
                            if (room11 != null)
                            {
                                room11.RemovePlayer(player.PlayerDetail);
                            }
                        }
                    }

                    #endregion
                }
            }
        }

		private void HandlePlayerUsingProp(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				game.Resume();
				if (pkg.ReadBoolean())
				{
					Player player = game.FindPlayer(pkg.Parameter1);
					ItemTemplateInfo template = ItemMgr.FindItemTemplate(pkg.Parameter2);
					if (player != null && template != null)
					{
						player.UseItem(template);
					}
				}
			}
		}

		private void HandleSysNotice(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				Player player = game.FindPlayer(pkg.Parameter1);
				GSPacketIn pkg1 = new GSPacketIn(3);
				pkg1.WriteInt(3);
				pkg1.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6",
					player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
				player.PlayerDetail.SendTCP(pkg1);
				pkg1.ClearContext();
				pkg1.WriteInt(3);
				pkg1.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7",
					player.PlayerDetail.PlayerCharacter.NickName, player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
				game.SendToAll(pkg1, player.PlayerDetail);
			}
		}

		private void HandlePlayerMessage(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				Player player = game.FindPlayer(pkg.ReadInt());
				bool team = pkg.ReadBoolean();
				string msg = pkg.ReadString();
				if (player != null)
				{
					GSPacketIn pkg1 = new GSPacketIn(19);
					pkg1.ClientID = player.PlayerDetail.PlayerCharacter.ID;
					pkg1.WriteInt(player.PlayerDetail.ZoneId);
					pkg1.WriteByte((byte)ChatInputView.CURRENT);
					pkg1.WriteBoolean(team);
					pkg1.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
					pkg1.WriteString(msg);
					if (team)
						game.SendToTeam(pkg, player.Team);
					else
						game.SendToAll(pkg1);
				}
			}
		}

		public void HandleConsortiaAlly(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				game.ConsortiaAlly = pkg.ReadInt();
				game.RichesRate = pkg.ReadInt();
			}
		}

		public void HandleLogin(GSPacketIn pkg)
		{
			string[] strArray = Encoding.UTF8.GetString(rsacryptoServiceProvider_0.Decrypt(pkg.ReadBytes(), fOAEP: false)).Split(',');
			if (strArray.Length == 2)
			{
				rsacryptoServiceProvider_0 = null;
				int.Parse(strArray[0]);
				base.Strict = false;
			}
			else
			{
				ilog_1.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
				Disconnect();
			}
		}

		public void HandleGameRoomCreate(GSPacketIn pkg)
		{
			int num1 = pkg.ReadInt();
			int num5 = pkg.ReadInt();
			int num6 = pkg.ReadInt();
			int num7 = pkg.ReadInt();
			int npcId = pkg.ReadInt();
			bool pickUpWithNPC = pkg.ReadBoolean();
			bool isBot = pkg.ReadBoolean();
			bool flag = pkg.ReadBoolean();
			int length = pkg.ReadInt();
			int num8 = 0;
			int num9 = 0;
			int zoneID = 0;
			int totalFightPower = 0;
			IGamePlayer[] players = new IGamePlayer[length];
			for (int index1 = 0; index1 < length; index1++)
			{
				PlayerInfo character = new PlayerInfo();
				ProxyPlayerInfo proxyPlayer = new ProxyPlayerInfo();
				character.ID = pkg.ReadInt();
				int num23 = (proxyPlayer.ZoneId = pkg.ReadInt());
				zoneID = num23;
				proxyPlayer.ZoneName = pkg.ReadString();
				//--
				bool QuanChien = pkg.ReadBoolean();
				int Place = pkg.ReadInt();
				//--
				int num11 = pkg.ReadInt();
				character.NickName = pkg.ReadString();
				character.typeVIP = pkg.ReadByte();
				character.VIPLevel = pkg.ReadInt();
				character.Sex = pkg.ReadBoolean();
				character.Hide = pkg.ReadInt();
				character.Style = pkg.ReadString();
				character.Colors = pkg.ReadString();
				character.Skin = pkg.ReadString();
				character.Offer = pkg.ReadInt();
				character.GP = pkg.ReadInt();
				character.Grade = pkg.ReadInt();
				character.Repute = pkg.ReadInt();
				character.ConsortiaID = pkg.ReadInt();
				character.ConsortiaName = pkg.ReadString();
				character.ConsortiaLevel = pkg.ReadInt();
				character.ConsortiaRepute = pkg.ReadInt();
				character.IsShowConsortia = pkg.ReadBoolean();
				character.badgeID = pkg.ReadInt();
				character.Honor = pkg.ReadString();
				character.AchievementPoint = pkg.ReadInt();
				character.WeaklessGuildProgressStr = pkg.ReadString();
				character.MoneyPlus = pkg.ReadInt();
				character.FightPower = pkg.ReadInt();
				character.Nimbus = pkg.ReadInt();
				character.apprenticeshipState = pkg.ReadInt();
				character.masterID = pkg.ReadInt();
				character.masterOrApprentices = pkg.ReadString();
				character.IsAutoBot = isBot;
				totalFightPower += character.FightPower;
				character.Attack = pkg.ReadInt();
				character.Defence = pkg.ReadInt();
				character.Agility = pkg.ReadInt();
				character.Luck = pkg.ReadInt();
				character.hp = pkg.ReadInt();
				proxyPlayer.BaseAttack = pkg.ReadDouble();
				proxyPlayer.BaseDefence = pkg.ReadDouble();
				proxyPlayer.BaseAgility = pkg.ReadDouble();
				proxyPlayer.BaseBlood = pkg.ReadDouble();
				proxyPlayer.TemplateId = pkg.ReadInt();
				proxyPlayer.WeaponStrengthLevel = pkg.ReadInt();
				int num13 = pkg.ReadInt();
				if (num13 != 0)
				{
					proxyPlayer.GoldTemplateId = num13;
					proxyPlayer.goldBeginTime = pkg.ReadDateTime();
					proxyPlayer.goldValidDate = pkg.ReadInt();
				}
				proxyPlayer.CanUserProp = pkg.ReadBoolean();
				proxyPlayer.SecondWeapon = pkg.ReadInt();
				proxyPlayer.StrengthLevel = pkg.ReadInt();
				proxyPlayer.Healstone = pkg.ReadInt();
				proxyPlayer.HealstoneCount = pkg.ReadInt();
				double num12 = pkg.ReadDouble();
				double num15 = pkg.ReadDouble();
				double num16 = pkg.ReadDouble();
				double num17 = pkg.ReadDouble();
				double num18 = pkg.ReadDouble();
				pkg.ReadInt();
				List<BufferInfo> buffers = new List<BufferInfo>();
				int num19 = pkg.ReadInt();
				for (int index4 = 0; index4 < num19; index4++)
				{
					BufferInfo bufferInfo = new BufferInfo();
					bufferInfo.Type = pkg.ReadInt();
					bufferInfo.IsExist = pkg.ReadBoolean();
					bufferInfo.BeginDate = pkg.ReadDateTime();
					bufferInfo.ValidDate = pkg.ReadInt();
					bufferInfo.Value = pkg.ReadInt();
					if (character != null)
					{
						buffers.Add(bufferInfo);
					}
				}
				List<int> equipEffect = new List<int>();
				int num20 = pkg.ReadInt();
				for (int index3 = 0; index3 < num20; index3++)
				{
					int num21 = pkg.ReadInt();
					equipEffect.Add(num21);
				}
				List<BufferInfo> fightBuffer = new List<BufferInfo>();
				int num3 = pkg.ReadInt();
				for (int index2 = 0; index2 < num3; index2++)
				{
					int num2 = pkg.ReadInt();
					int num4 = pkg.ReadInt();
					fightBuffer.Add(new BufferInfo
					{
						Type = num2,
						Value = num4
					});
				}
				proxyPlayer.TcpEndPoint = pkg.ReadString();
				UserMatchInfo matchInfo = new UserMatchInfo();
				character.VIPExpireDay = pkg.ReadDateTime();
				character.DailyLeagueFirst = pkg.ReadBoolean();
				character.DailyLeagueLastScore = pkg.ReadInt();
				int num14 = (pkg.ReadBoolean() ? 1 : 0);
				UsersPetInfo pet = null;
				if (num14 != 0)
				{
					pet = new UsersPetInfo();
					pet.Place = pkg.ReadInt();
					pet.TemplateID = pkg.ReadInt();
					pet.ID = pkg.ReadInt();
					pet.Name = pkg.ReadString();
					pet.UserID = pkg.ReadInt();
					pet.Level = pkg.ReadInt();
					pet.Skill = pkg.ReadString();
					pet.SkillEquip = pkg.ReadString();
				}
                #region card buff
                List<int> CardBuff = new List<int>();
                int countCard = pkg.ReadInt();
                for (int j = 0; j < countCard; j++)
                {
					CardBuff.Add(pkg.ReadInt());
                }
				#endregion
				players[index1] = new ProxyPlayer(this, character, pet, buffers, equipEffect, fightBuffer, proxyPlayer, matchInfo)
				{
					CurrentEnemyId = num11,
					GPApprenticeOnline = num16,
					GPAddPlus = num12,
					OfferAddPlus = num15,
					GPApprenticeTeam = num17,
					GPSpouseTeam = num18,
					CardBuff = CardBuff,
					IsQuanChien = QuanChien,
					Place = Place
                };
				if (!QuanChien)
				{
					num9 = character.ID;
				}
				num8 += character.Grade;
			}
			ProxyRoom room = new ProxyRoom(ProxyRoomMgr.NextRoomId(), num1, zoneID, players, this, npcId, pickUpWithNPC, isBot, isSmartBot: false);
			room.GuildId = num7;
			room.selfId = num9;
			room.AvgLevel = num8;
			room.startWithNpc = pickUpWithNPC;
			room.RoomType = (eRoomType)num5;
			room.GameType = (eGameType)num6;
			room.IsCrossZone = flag;
			room.FightPower = totalFightPower;
			lock (m_rooms)
			{
				if (!m_rooms.ContainsKey(num1))
				{
					m_rooms.Add(num1, room);
				}
				else
				{
					room = null;
				}
			}
			if (room != null)
			{
				ProxyRoomMgr.AddRoom(room);
				return;
			}
			RemoveRoom(num1, room);
			ilog_1.ErrorFormat("Room already exists:{0}.", num1);
		}

		public void HandleGameRoomCancel(GSPacketIn pkg)
		{
			ProxyRoom room = null;
			lock (m_rooms)
			{
				if (m_rooms.ContainsKey(pkg.Parameter1))
				{
					room = m_rooms[pkg.Parameter1];
				}
			}
			if (room != null)
			{
				ProxyRoomMgr.RemoveRoom(room);
			}
		}

		public void HanleSendToGame(GSPacketIn pkg)
		{
			BaseGame game = GameMgr.FindGame(pkg.ClientID);
			if (game != null)
			{
				GSPacketIn pkg2 = pkg.ReadPacket();
				game.ProcessData(pkg2);
			}
		}

		public void SendRSAKey(byte[] m, byte[] e)
		{
			GSPacketIn pkg = new GSPacketIn(0);
			pkg.Write(m);
			pkg.Write(e);
			SendTCP(pkg);
		}

		public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
		{
			GSPacketIn p = new GSPacketIn(32, playerId);
			p.WritePacket(pkg);
			SendTCP(p);
		}

		public void SendRemoveRoom(int roomId)
		{
			GSPacketIn pkg = new GSPacketIn(65, roomId);
			SendTCP(pkg);
		}

		public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
		{
			GSPacketIn p = new GSPacketIn(67, roomId);
			if (except != null)
			{
				p.Parameter1 = except.PlayerCharacter.ID;
				p.Parameter2 = except.GamePlayerId;
			}
			else
			{
				p.Parameter1 = 0;
				p.Parameter2 = 0;
			}

			p.WritePacket(pkg);
			SendTCP(p);
		}

		public void SendStartGame(int roomId, AbstractGame game)
		{
			GSPacketIn pkg = new GSPacketIn(66);
			pkg.Parameter1 = roomId;
			pkg.Parameter2 = game.Id;
			pkg.WriteInt((int)game.RoomType);
			pkg.WriteInt((int)game.GameType);
			pkg.WriteInt(game.TimeType);
			SendTCP(pkg);
		}

		public void SendStopGame(int roomId, int gameId)
		{
			GSPacketIn pkg = new GSPacketIn(68);
			pkg.Parameter1 = roomId;
			pkg.Parameter2 = gameId;
			SendTCP(pkg);
		}

		public void SendGamePlayerId(IGamePlayer player)
		{
			GSPacketIn pkg = new GSPacketIn(33);
			pkg.Parameter1 = player.PlayerCharacter.ID;
			pkg.Parameter2 = player.GamePlayerId;
			SendTCP(pkg);
		}

		public void SendAddRobRiches(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(52, playerId);
			pkg.Parameter1 = value;
			pkg.WriteInt(value);
			SendTCP(pkg);
		}

		public void SendDisconnectPlayer(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn(34, playerId);
			SendTCP(pkg);
		}

		public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp, bool isSpanArea, bool isCouple, int blood, int playerCount)
		{
			GSPacketIn pkg = new GSPacketIn(35, playerId)
			{
				Parameter1 = gameId
			};
			pkg.WriteBoolean(isWin);
			pkg.WriteInt(gainXp);
			pkg.WriteBoolean(isSpanArea);
			pkg.WriteBoolean(isCouple);
			pkg.WriteInt(blood);
			pkg.WriteInt(playerCount);
			SendTCP(pkg);
		}

		public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
		{
			GSPacketIn pkg = new GSPacketIn(36, playerId);
			pkg.Parameter1 = bag;
			pkg.Parameter2 = place;
			pkg.WriteInt(templateId);
			pkg.WriteBoolean(isLiving);
			SendTCP(pkg);
		}
		#region EliteGame
		public void SendAddEliteGameScore(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ADD_ELITEGAME_SCORE, playerId);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}
		public void SendRemoveEliteGameScore(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_REMOVE_ELITEGAME_SCORE, playerId);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}
		public void SendEliteGameWinUpdate(int playerId, int blood, int win)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_ELITEGAME_WINUPDATE, playerId);
			pkg.WriteInt(blood);
			pkg.WriteInt(win);
			SendTCP(pkg);
		}
		#endregion

		#region ADD (GiftToken, GP, MoneyLock, Money, Gold)
		public void SendPlayerAddOffer(int playerId, int value)
        {
			GSPacketIn pkg = new GSPacketIn(51, playerId);
			pkg.WriteInt(value);
			SendTCP(pkg);
        }

		public void SendPlayerAddGold(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(38, playerId);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}

		public void SendPlayerAddMoney(int playerId, int value, bool isAll)
		{
			GSPacketIn pkg = new GSPacketIn(74, playerId);
			pkg.Parameter1 = value;
			pkg.Parameter2 = (isAll ? 1 : 0);
			SendTCP(pkg);
		}

		public void SendPlayerAddGiftToken(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(75, playerId);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}

		public void SendPlayerAddGP(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(39, playerId);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}
        #endregion

        public void SendPlayerRemoveGP(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(49, playerId);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}

		public void SendUpdateRestCount(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn((byte)eFightPackageType.PLAYER_UPDATE_REST_COUNT, playerId);
			SendTCP(pkg);
		}

		public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
		{
			GSPacketIn pkg = new GSPacketIn(40, playerId);
			pkg.WriteInt(type);
			pkg.WriteBoolean(isLiving);
			pkg.WriteInt(demage);
			SendTCP(pkg);
		}

		public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
		{
			GSPacketIn pkg = new GSPacketIn(41, playerId);
			pkg.WriteBoolean(isWin);
			pkg.WriteInt(MissionID);
			pkg.WriteInt(turnNum);
			SendTCP(pkg);
		}

		public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
		{
			var newplayers = players.Where(p => p.Value.IsQuanChien).Select(p => p.Value).ToList();
			try
            {
				GSPacketIn pkg = new GSPacketIn(42, playerId);
				pkg.WriteInt(consortiaWin);
				pkg.WriteInt(consortiaLose);
				pkg.WriteInt(newplayers.Count);
				for (int i = 0; i < newplayers.Count; i++)
				{
					pkg.WriteInt(newplayers[i].PlayerDetail.PlayerCharacter.ID);
				}

				pkg.WriteByte((byte)roomType);
				pkg.WriteByte((byte)gameClass);
				pkg.WriteInt(totalKillHealth);
				SendTCP(pkg);
			}
			catch
			{
				ilog_1.ErrorFormat("SendPlayerConsortiaFight players.Count {0}", newplayers.Count);
			}
		}

		public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
		{
			GSPacketIn pkg = new GSPacketIn(43, playerId);
			pkg.WriteInt(consortiaID);
			pkg.WriteInt(riches);
			pkg.WriteString(msg);
			SendTCP(pkg);
		}

		public void SendPlayerRemoveGold(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(44, playerId);
			pkg.WriteInt(value);
			SendTCP(pkg);
		}

		public void SendPlayerRemoveMoney(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(45, playerId);
			pkg.WriteInt(value);
			SendTCP(pkg);
		}

		public void SendPlayerRemoveOffer(int playerId, int value)
		{
			GSPacketIn pkg = new GSPacketIn(50, playerId);
			pkg.WriteInt(value);
			SendTCP(pkg);
		}

		public void SendPlayerAddTemplate(int playerId, ItemInfo cloneItem, eBageType bagType, int count)
		{
			if (cloneItem != null)
			{
				GSPacketIn pkg = new GSPacketIn(48, playerId);
				pkg.WriteInt(cloneItem.TemplateID);
				pkg.WriteByte((byte)bagType);
				pkg.WriteInt(count);
				pkg.WriteInt(cloneItem.ValidDate);
				pkg.WriteBoolean(cloneItem.IsBinds);
				pkg.WriteBoolean(cloneItem.IsUsed);
				pkg.WriteInt(cloneItem.StrengthenLevel);
				pkg.WriteInt(cloneItem.AttackCompose);
				pkg.WriteInt(cloneItem.DefendCompose);
				pkg.WriteInt(cloneItem.AgilityCompose);
				pkg.WriteInt(cloneItem.LuckCompose);
				pkg.WriteBoolean(cloneItem.IsGold);
				if (cloneItem.IsGold)
				{
					pkg.WriteDateTime(cloneItem.goldBeginTime);
					pkg.WriteInt(cloneItem.goldValidDate);
				}

				SendTCP(pkg);
			}
		}

		public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
		{
			GSPacketIn pkg = new GSPacketIn(77);
			pkg.WriteInt(Consortia1);
			pkg.WriteInt(Consortia2);
			pkg.WriteInt(GameId);
			SendTCP(pkg);
		}

		public void SendBeginFightNpc(int playerId, int RoomType, int GameType, int OrientRoomId)
		{
			GSPacketIn pkg = new GSPacketIn(88);
			pkg.Parameter1 = playerId;
			pkg.WriteInt(RoomType);
			pkg.WriteInt(GameType);
			pkg.WriteInt(OrientRoomId);
			SendTCP(pkg);
		}

		public void SendPlayerRemoveHealstone(int playerId)
		{
			GSPacketIn pkg = new GSPacketIn(73, playerId);
			SendTCP(pkg);
		}

		public ServerClient(FightServer svr) : base(new byte[2048 * 4], new byte[2048 * 4])
		{
			m_svr = svr;
		}

		public override string ToString()
		{
			return $"Server Client: {0} IsConnected:{base.IsConnected}  RoomCount:{m_rooms.Count}";
		}

		public void RemoveRoom(int orientId, ProxyRoom room)
		{
			bool flag = false;
			lock (m_rooms)
			{
				if (m_rooms.ContainsKey(orientId) && m_rooms[orientId] == room)
				{
					flag = m_rooms.Remove(orientId);
				}
			}
			if (flag)
			{
				SendRemoveRoom(orientId);
			}
		}

		public void SendPlayerLeageScoure(int playerId, bool isWin, int value)
		{
			GSPacketIn pkg = new GSPacketIn(82, playerId);
			pkg.WriteBoolean(isWin);
			pkg.Parameter1 = value;
			SendTCP(pkg);
		}

		static ServerClient()
		{
			ilog_1 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
