using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic
{
	public class PVPGame : BaseGame
	{
		private int BeginPlayerCount;

		private DateTime beginTime;

		private static readonly int MONEY_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_LOSE"]);

		private static readonly int MONEY_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_LOSE"]);

		private static readonly int MONEY_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_WIN"]);

		private static readonly int MONEY_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_WIN"]);

		private static readonly int EXP_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_LOSE"]);

		private static readonly int EXP_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_LOSE"]);

		private static readonly int EXP_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_WIN"]);

		private static readonly int EXP_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_WIN"]);

		private static readonly double GP_RATE = int.Parse(ConfigurationManager.AppSettings["GP_RATE"]);

		private static readonly int LeagueMoney_Lose = new Random().Next(1, int.Parse(ConfigurationManager.AppSettings["LeagueMoney_Lose"]));

		private static readonly int LeagueMoney_Win = new Random().Next(3, int.Parse(ConfigurationManager.AppSettings["LeagueMoney_Win"]));

		private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private float m_blueAvgLevel;

		private List<Player> m_blueTeam;

		private float m_redAvgLevel;

		private List<Player> m_redTeam;

		private string teamAStr;

		private string teamBStr;

		public string m_continuousNick;

		public string ContinuousRunningPlayer
		{
			get
			{
				return m_continuousNick;
			}
			set
			{
				m_continuousNick = value;
			}
		}

		public Player CurrentPlayer => m_currentLiving as Player;

		public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType)
			: base(id, roomId, map, roomType, gameType, timeType)
		{
			m_redTeam = new List<Player>();
			m_blueTeam = new List<Player>();
			StringBuilder builder = new StringBuilder();
			m_redAvgLevel = 0f;
			foreach (IGamePlayer player in red)
			{
				Player fp = new Player(player, PhysicalId++, this, 1, player.PlayerCharacter.hp);
				builder.Append(player.PlayerCharacter.ID).Append(",");
				fp.Reset();
				fp.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player, fp);
				m_redTeam.Add(fp);
				m_redAvgLevel += player.PlayerCharacter.Grade;
				if (!FrozenWind && player.PlayerCharacter.Grade <= 9)
                {
					FrozenWind = true;
                }
			}
			m_redAvgLevel /= m_redTeam.Count;
			teamAStr = builder.ToString();
			StringBuilder builder2 = new StringBuilder();
			m_blueAvgLevel = 0f;
			foreach (IGamePlayer player2 in blue)
			{
				Player player3 = new Player(player2, PhysicalId++, this, 2, player2.PlayerCharacter.hp);
				builder2.Append(player2.PlayerCharacter.ID).Append(",");
				player3.Reset();
				player3.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
				AddPlayer(player2, player3);
				m_blueTeam.Add(player3);
				m_blueAvgLevel += player2.PlayerCharacter.Grade;
				if (!FrozenWind && player2.PlayerCharacter.Grade <= 9)
				{
					FrozenWind = true;
				}
			}
			m_blueAvgLevel /= blue.Count;
			teamBStr = builder2.ToString();
			BeginPlayerCount = m_redTeam.Count + m_blueTeam.Count;
			beginTime = DateTime.Now;
		}

		public override bool TakeCard(Player player)
		{
			int index = 0;

			for (int i = 0; i < Cards.Length; i++)
			{
				if (Cards[i] == 0)
				{
					index = i;
					break;
				}
			}

			return TakeCard(player, index, true);
		}

		public override bool TakeCard(Player player, int index, bool isAuto)
		{
			if (player.CanTakeOut == 0 || (index < 0 || (index > this.Cards.Length || player.FinishTakeCard) || this.Cards[index] > 0))
				return false;

			player.CanTakeOut--;
			int templateID = 0;
			int count = 0;
			List<ItemInfo> infos = null;
			if (DropInventory.CardDrop(RoomType, ref infos))
			{
				if (infos != null)
				{
					foreach (ItemInfo info in infos)
					{
						if (info != null && info.TemplateID > 0)
						{
							templateID = info.TemplateID;
							count = info.Count;
							player.PlayerDetail.AddTemplate(info, eBageType.TempBag, info.Count, eGameView.BatleTypeGet);
						}
						else
						{
							if (info != null && info.TemplateID == -100) //gold
							{
								templateID = info.TemplateID;
								count = info.Count;
								player.PlayerDetail.AddGold(info.Count);
							}
						}

						if (info.IsTips)
						{
						}
					}

					if (infos.Count == 0)
                    {
						log.ErrorFormat("Have not DropItem for RoomType.{0}", RoomType);
					}
				}
			}
			else
			{
				log.ErrorFormat("Have not DropCondition for RoomType.{0}", RoomType);
			}

			if (player.CanTakeOut == 0)
			{
				player.FinishTakeCard = true;
			}
			bool isVip = false;
			if (player.PlayerDetail.PlayerCharacter.typeVIP > 0)
            {
				isVip = true;
            }

			Cards[index] = 1;
			SendGamePlayerTakeCard(player, isAuto, index, templateID, count, isVip);
			return true;
		}

		private int CalculateExperience(Player player, int winTeam, ref int reward, ref int rewardServer)
		{
			if (m_roomType == eRoomType.Match)
			{
				float againstTeamLevel = ((player.Team == 1) ? m_blueAvgLevel : m_redAvgLevel);
				float againstTeamCount = ((player.Team == 1) ? m_blueTeam.Count : m_redTeam.Count);
				Math.Abs(againstTeamLevel - (float)player.PlayerDetail.PlayerCharacter.Grade);
				if (player.TotalHurt == 0)
				{
					if (againstTeamLevel - (float)player.PlayerDetail.PlayerCharacter.Grade >= 5f && TotalHurt > 0)
					{
						SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward"), null, 2);
						reward = 200;
						return 201;
					}
					return 1;
				}
				float winPlus = ((player.Team == winTeam) ? 2 : 0);
				player.TotalShootCount = ((player.TotalShootCount == 0) ? 1 : player.TotalShootCount);
				if (player.TotalShootCount < player.TotalHitTargetCount)
				{
					player.TotalShootCount = player.TotalHitTargetCount;
				}
				int maxHurt = (int)((player.Team == 1) ? ((float)m_blueTeam.Count * m_blueAvgLevel * 300f) : (m_redAvgLevel * (float)m_redTeam.Count * 300f));
				int totalHurt = ((player.TotalHurt > maxHurt) ? maxHurt : player.TotalHurt);
				int gp = (int)Math.Ceiling(((double)winPlus + (double)totalHurt * 0.001 + (double)player.TotalKill * 0.5 + (double)(player.TotalHitTargetCount / player.TotalShootCount * 2)) * (double)againstTeamLevel * (0.9 + (double)(againstTeamCount - 1f) * 0.3));
				if (againstTeamLevel - (float)player.PlayerDetail.PlayerCharacter.Grade >= 5f && TotalHurt > 0)
				{
					SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward"), null, 2);
					reward = 200;
					gp += 200;
				}
				gp = GainCoupleGP(player, gp);
				if (Convert.ToBoolean(ConfigurationManager.AppSettings["DoubleEvent"]))
				{
					gp *= 2;
					rewardServer = gp / 2;
				}
				if (gp > 12000)
				{
					log.Error($"pvpgame ====== player.nickname : {player.PlayerDetail.PlayerCharacter.NickName}, add gp : {gp} ======== gp > 10000");
					log.Error($"pvpgame ====== player.nickname : {player.PlayerDetail.PlayerCharacter.NickName}, parameters winPlus: {winPlus}, totalHurt : {player.TotalHurt}, totalKill : {player.TotalKill}, totalHitTargetCount : {player.TotalHitTargetCount}, totalShootCount : {player.TotalShootCount}, againstTeamLevel : {againstTeamLevel}, againstTeamCount : {againstTeamCount}");
					gp = 12000;
				}
				return (gp < 1) ? 1 : gp;
			}
			return 0;
		}

		private int CalculateGuildMatchResult(List<Player> players, int winTeam)
		{
			if (base.RoomType == eRoomType.Match && base.GameType == eGameType.Guild)
			{
				StringBuilder winStr = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
				IGamePlayer winPlayer = null;
				IGamePlayer losePlayer = null;
				int teamTotalHurt = 0;
				foreach (Player p2 in players)
				{
					if (p2.Team == winTeam)
					{
						winStr.Append($"[{p2.PlayerDetail.PlayerCharacter.NickName}]");
						winPlayer = p2.PlayerDetail;
						int maxHurt = (int)((p2.Team == 1) ? ((float)m_blueTeam.Count * m_blueAvgLevel * 300f) : (m_redAvgLevel * (float)m_redTeam.Count * 300f));
						int totalHurt = ((p2.TotalHurt > maxHurt) ? maxHurt : p2.TotalHurt);
						teamTotalHurt += totalHurt;
					}
					else
					{
						losePlayer = p2.PlayerDetail;
					}
				}
				if (losePlayer != null)
				{
					winStr.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1") + losePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2"));
					winPlayer.ConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, losePlayer.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, teamTotalHurt, players.Count);
					int riches = 0;
					int count = ((winTeam == 1) ? m_blueTeam.Count : m_redTeam.Count);
					riches = (int)(float)(count + teamTotalHurt / 2000);
					winPlayer.SendConsortiaFight(winPlayer.PlayerCharacter.ConsortiaID, riches, winStr.ToString());
					if (riches > 100000)
					{
						log.Error(string.Format("pvpgame ======= riches : {0}, count : {1}, teamTotalHurt : {2}", new object[3]
						{
							riches,
							count,
							teamTotalHurt
						}));
					}
					{
						foreach (Player p in players)
						{
							if (p.Team == winTeam)
							{
								p.PlayerDetail.AddRobRiches(riches);
							}
						}
						return riches;
					}
				}
			}
			return 0;
		}

		public bool CanGameOver()
		{
			if (RoomType == eRoomType.ConsortiaBattle && beginTime.AddSeconds(300) <= DateTime.Now)
			{
				return true;
			}
			else
			{
				bool red = true;
				bool blue = true;
				foreach (Player item in m_redTeam.Where(p => !p.IsQuanChien))
				{
					if (item.IsLiving)
					{
						red = false;
						break;
					}
				}
				foreach (Player item2 in m_blueTeam.Where(p => !p.IsQuanChien))
				{
					if (item2.IsLiving)
					{
						blue = false;
						break;
					}
				}
				return red || blue;
			}
		}

		public override void CheckState(int delay)
		{
			AddAction(new CheckPVPGameStateAction(delay));
		}

		public void GameOver()
		{
			if (base.GameState != eGameState.Playing)
			{
				return;
			}
			m_gameState = eGameState.GameOver;
			ClearWaitTimer();
			CurrentTurnTotalDamage = 0;
			List<Player> allFightPlayers = GetAllFightPlayersNoQuanChien();
			int winTeam = GetWinTeam(allFightPlayers);
			int num2 = 0;
			int num3 = 0;
			if (base.RoomType != eRoomType.ConsortiaBattle)
			{
				foreach (Player player3 in allFightPlayers)
				{
					if (player3.TotalHurt > 0)
					{
						if (player3.Team == 1)
						{
							num3 = 1;
						}
						else
						{
							num2 = 1;
						}
					}
				}
			}
			int guildMatchResult = CalculateGuildMatchResult(allFightPlayers, winTeam);
			if (base.RoomType == eRoomType.Match && base.GameType == eGameType.Guild)
			{
				int num4 = -10;
				int num7 = allFightPlayers.Count / 2;
				int num9 = num4 + (int)Math.Round((double)(allFightPlayers.Count / 2) * 0.5);
			}
			int tieStatus = winTeam == -1 ? winTeam : 0;
			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(100);
			pkg.WriteInt(tieStatus);
			pkg.WriteInt(base.PlayerCount);
			int beginPlayerCount = BeginPlayerCount;
			foreach (Player player4 in allFightPlayers.Where(p => !p.IsQuanChien).ToList())
			{
				double num5 = ((player4.Team == 1) ? ((double)m_blueAvgLevel) : ((double)m_redAvgLevel));
				if (player4.Team != 1)
				{
					int count1 = m_redTeam.Count;
				}
				else
				{
					int count2 = m_blueTeam.Count;
				}
				double grade1 = player4.PlayerDetail.PlayerCharacter.Grade;
				float num6 = Math.Abs((float)(num5 - grade1));
				int team = player4.Team;
				int num8 = 0;
				int val1 = 0;
				int reward = 0;
				int rewardServer = 0;
				if (player4.TotalShootCount != 0)
				{
					int totalShootCount = player4.TotalShootCount;
				}
				if (m_roomType == eRoomType.Match || (double)num6 < 5.0)
				{
					val1 = CalculateOffer(player4, winTeam);
					num8 = CalculateExperience(player4, winTeam, ref reward, ref rewardServer);
				}
				if (player4.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
				{
					num8 += num8 * player4.FightBuffers.ConsortionAddPercentGoldOrGP / 100;
				}
				if (player4.FightBuffers.ConsortionAddOfferRate > 0)
				{
					guildMatchResult *= player4.FightBuffers.ConsortionAddOfferRate;
				}
				double num10 = Math.Ceiling((double)num8 * player4.PlayerDetail.GPApprenticeOnline);
				double num11 = Math.Ceiling((double)num8 * player4.PlayerDetail.GPApprenticeTeam);
				double GPSTeam = Math.Ceiling((double)num8 * player4.PlayerDetail.GPSpouseTeam);
				int num12 = ((num8 == 0) ? 1 : num8);
				string msg = "";
				bool val2 = player4.Team == winTeam;
				if (base.RoomType == eRoomType.Match)
				{
					int leagueMoneyLose1 = LeagueMoney_Lose;
					double moneyPVP = 0;
					double expPVP = 0;
					double giftPVP = 0;
					double bonusMoney = 1;

					Random random = new Random();
					DateTime now = DateTime.Now;

					DateTime dateTime1 = Convert.ToDateTime(ConfigurationManager.AppSettings["datetime1"]);
					DateTime dateTime2 = Convert.ToDateTime(ConfigurationManager.AppSettings["datetime2"]);
					DateTime dateTime3 = Convert.ToDateTime(ConfigurationManager.AppSettings["datetime3"]);
					DateTime dateTime4 = Convert.ToDateTime(ConfigurationManager.AppSettings["datetime4"]);

					DayOfWeek today = DateTime.Today.DayOfWeek;
					Console.WriteLine($"time3 = {dateTime3} | time4 = {dateTime4}");
                    #region OLD
                    if (player4.TotalHurt > 0)
					{
						#region Check IP All Fighting Player
						/*if (CheckIp(allFightPlayers, player4))
						{
							player4.PlayerDetail.SendHideMessage("Trận đấu của bạn có chung cùng 1 đường mạng giống nhau nên số xu mà bạn nhận được là 0Xu.");
						}*/
						#endregion

						double timex2 = pvprate();
						
						#region Add Money For PVP
						if (val2)
						{
							int money_min_rate_win = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_WIN"]);
							int money_max_rate_win = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_WIN"]);
							moneyPVP = random.Next(money_min_rate_win, money_max_rate_win) * timex2;
							int exp_min_rate_win = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_WIN"]);
							int exp_max_rate_win = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_WIN"]);
							expPVP = random.Next(exp_min_rate_win, exp_max_rate_win) * timex2;
							giftPVP = random.Next(20, 30) * timex2;
						}
						else
						{
							int money_min_rate_lose = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_LOSE"]);
							int money_max_rate_lose = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_LOSE"]);
							moneyPVP = random.Next(money_min_rate_lose, money_max_rate_lose) * timex2;
							int exp_min_rate_lose = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_LOSE"]);
							int exp_max_rate_lose = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_LOSE"]);
							expPVP = random.Next(exp_min_rate_lose, exp_max_rate_lose) * timex2;
							giftPVP = random.Next(10, 20) * timex2;
						}
						#endregion

						if (player4.PlayerDetail.PlayerCharacter.VIPLevel >= 1 && player4.PlayerDetail.PlayerCharacter.VIPLevel <= 3)
							bonusMoney = moneyPVP * 5 / 100;
						if (player4.PlayerDetail.PlayerCharacter.VIPLevel >= 4 && player4.PlayerDetail.PlayerCharacter.VIPLevel <= 6)
							bonusMoney = moneyPVP * 10 / 100;
						if (player4.PlayerDetail.PlayerCharacter.VIPLevel >= 7 && player4.PlayerDetail.PlayerCharacter.VIPLevel <= 9)
							bonusMoney = moneyPVP * 25 / 100;


						player4.PlayerDetail.AddMoney((int)Math.Floor(moneyPVP + bonusMoney), igroneAll: false);
						player4.PlayerDetail.AddGiftToken((int)Math.Floor(giftPVP));

						if (player4.PlayerDetail.PlayerCharacter.VIPLevel > 0)
							player4.PlayerDetail.SendMessage($"Bạn nhận được {moneyPVP} xu, {giftPVP} lễ kim, VIP Bonus {bonusMoney} xu");
						else
							player4.PlayerDetail.SendMessage($"Bạn nhận được {moneyPVP} xu, {giftPVP} lễ kim");
					}
					else
					{
						if (val2)
						{
							player4.PlayerDetail.SendHideMessage("Chiến thắng | Không có sát thương không nhận được xu và Exp");
						}
						else
						{
							player4.PlayerDetail.SendHideMessage("Thua cuộc | Không có sát thương không nhận được xu và Exp");
						}
					}
                    #endregion

                    num12 += (int)Math.Floor(expPVP);
					if (msg != "" && msg != null)
					{
						player4.PlayerDetail.SendHideMessage(msg);
					}
					if (base.GameType == eGameType.Guild)
					{
						new ConsortiaBussiness().ConsortiaRichAdd(player4.PlayerDetail.PlayerCharacter.ConsortiaID, ref guildMatchResult);
					}
					int restCount = player4.PlayerDetail.MatchInfo.restCount;
					int maxCount = player4.PlayerDetail.MatchInfo.maxCount;
					int grade2 = player4.PlayerDetail.PlayerCharacter.Grade;
				}
				else if(m_roomType == eRoomType.ConsortiaBattle)
                {
					player4.PlayerDetail.UpdateConsortiaBattle(player4.Blood, val2, tieStatus);
                }
				if (player4.PlayerDetail.PlayerCharacter.typeVIP > 0)
				{
					num12 += 10;
					val1++;
				}
				player4.GainGP = player4.PlayerDetail.AddGP(num12);
				player4.GainOffer = player4.PlayerDetail.AddOffer(val1);
				Random randomLeaguePoint = new Random();
				int leageScoreLose = randomLeaguePoint.Next(50, 100);//CalculateLeageScore(player4, winTeam);
				int leageScoreWin = randomLeaguePoint.Next(200, 300);

				player4.CanTakeOut = ((player4.Team == 1) ? num3 : num2);
				pkg.WriteInt(player4.Id);
				pkg.WriteBoolean(val2);
				pkg.WriteInt(player4.PlayerDetail.PlayerCharacter.Grade);
				pkg.WriteInt(player4.PlayerDetail.PlayerCharacter.GP);
				pkg.WriteInt(player4.TotalKill);
				pkg.WriteInt(num12);
				pkg.WriteInt(player4.TotalHitTargetCount);
				pkg.WriteInt(player4.psychic);
				pkg.WriteInt((player4.PlayerDetail.PlayerCharacter.typeVIP > 0) ? 10 : 0);
				pkg.WriteInt(0);
				pkg.WriteInt((int)GPSTeam);
				pkg.WriteInt(rewardServer);
				pkg.WriteInt((int)num10);
				pkg.WriteInt((int)num11);
				pkg.WriteInt(0);
				pkg.WriteInt(reward);
				pkg.WriteInt(0);
				pkg.WriteInt(rewardServer);
				pkg.WriteInt(player4.GainGP);
				pkg.WriteInt(val1);
				pkg.WriteInt(0);
				pkg.WriteInt((player4.PlayerDetail.PlayerCharacter.typeVIP > 0) ? 1 : 0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(0);
				pkg.WriteInt(rewardServer);
				pkg.WriteInt(player4.GainOffer);
				pkg.WriteInt(player4.CanTakeOut);
			}

            pkg.WriteInt(guildMatchResult);
			SendToAll(pkg);
			bool isNoticed = false;
			foreach (Player player in allFightPlayers)
			{
				player.PlayerDetail.OnGameOver(this, player.Team == winTeam, player.GainGP, false, player.PlayerDetail.GPSpouseTeam > 0.0, player.Blood, BeginPlayerCount);
				if (!isNoticed && base.RoomType == eRoomType.Match && player.Team == winTeam)
                {
					string msg = $"Chúc mừng ";
					string losers = $"";
					string winners = "";
					foreach (Player p in allFightPlayers)
					{
						string t = $"[{p.PlayerDetail.PlayerCharacter.NickName}]";
						if (p.PlayerDetail.PlayerCharacter.IsAutoBot)
                        {
							t = "[Bot]";
                        }
						if (p.Team != winTeam)
                        {
							losers += t;
                        } else
                        {
							winners += t;
                        }
					}
					msg += winners;
					msg += " chiến thắng ";
					msg += losers;
					player.PlayerDetail.PVPFightNotice(msg);
					isNoticed = true;
				}
			}
			OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, beginTime, DateTime.Now, BeginPlayerCount, base.Map.Info.ID, teamAStr, teamBStr, "", winTeam, BossWarField);
			if (RoomType == eRoomType.ConsortiaBattle)
			{
				WaitTime(1000);
			}
			else
			{
				WaitTime(20000);
			}
			OnGameOverred();
		}

		public static double pvprate()
        {
			DateTime now = DateTime.Now;
			string goldenTimeBegin = ConfigurationManager.AppSettings["GOLDEN_TIME_BEGIN"];
			string goldenTimeEnd = ConfigurationManager.AppSettings["GOLDEN_TIME_END"];
			double goldenTimeRate = int.Parse(ConfigurationManager.AppSettings["GOLDEN_TIME_RATE"]);
			string superGoldenDayOfWeek = ConfigurationManager.AppSettings["SUPER_GOLDEN_TIME_DATE_OF_WEEK"];
			string superGoldenTimeBegin = ConfigurationManager.AppSettings["SUPER_GOLDEN_TIME_BEGIN"];
			string superGoldenTimeEnd = ConfigurationManager.AppSettings["SUPER_GOLDEN_TIME_END"];
			double superGoldenTimeRate = int.Parse(ConfigurationManager.AppSettings["SUPER_GOLDEN_TIME_RATE"]);
			int goldenTimeCombineMode = int.Parse(ConfigurationManager.AppSettings["GOLDEN_TIME_COMBINE_MODE"]);
			switch (goldenTimeCombineMode)
			{
				case 1:
					return pvprateSeparate(goldenTimeBegin, goldenTimeEnd, goldenTimeRate, superGoldenDayOfWeek,
						superGoldenTimeBegin, superGoldenTimeEnd, superGoldenTimeRate);
				case 2:
					return pvprateCombine(goldenTimeBegin, goldenTimeEnd, goldenTimeRate, superGoldenDayOfWeek,
						superGoldenTimeBegin, superGoldenTimeEnd, superGoldenTimeRate);
			}

			return 0;
        }

		private static double pvprateSeparate(string goldenTimeBegin, string goldenTimeEnd, double goldenTimeRate, string superGoldenDayOfWeek, string superGoldenTimeBegin, string superGoldenTimeEnd, double superGoldenTimeRate)
		{
			DateTime now = DateTime.Now;
			DayOfWeek today = DateTime.Today.DayOfWeek;
			bool superApplied = false;
			string[] superGoldenDaysOfWeek = superGoldenDayOfWeek.Split('|');
			for (int j = 0; j < superGoldenDaysOfWeek.Length; j++)
			{
				if (((int)today) == int.Parse(superGoldenDaysOfWeek[j]))
				{
					superApplied = true;
					break;
				}
			}

			double rate = goldenTimeRate;
			if (superApplied)
			{
				goldenTimeBegin = superGoldenTimeBegin;
				goldenTimeEnd = superGoldenTimeEnd;
				rate = superGoldenTimeRate;
			}

			return pvprateGoldenTime(goldenTimeBegin, goldenTimeEnd, rate);
		}

		private static double pvprateGoldenTime(string timeBegin, string timeEnd, double rate)
		{
			DateTime now = DateTime.Now;
			string[] goldenTimesBegin = timeBegin.Split('|');
			string[] goldenTimesEnd = timeEnd.Split('|');
			if (goldenTimesBegin.Length != goldenTimesEnd.Length)
			{
				return 0;
			}
			for (int i = 0; i < goldenTimesBegin.Length; i++)
			{
				DateTime dateTime1 = Convert.ToDateTime(goldenTimesBegin[i]);
				DateTime dateTime2 = Convert.ToDateTime(goldenTimesEnd[i]);
				if (now >= dateTime1 && now <= dateTime2)
				{
					//it's golden time now
					return rate;
				}
			}
			return 0;
		}
		
		private static double pvprateCombine(string goldenTimeBegin, string goldenTimeEnd, double goldenTimeRate, string superGoldenDayOfWeek, string superGoldenTimeBegin, string superGoldenTimeEnd, double superGoldenTimeRate)
		{
			DateTime now = DateTime.Now;
			DayOfWeek today = DateTime.Today.DayOfWeek;
			bool superApplied = false;
			string[] superGoldenDaysOfWeek = superGoldenDayOfWeek.Split('|');
			for (int j = 0; j < superGoldenDaysOfWeek.Length; j++)
			{
				if (((int)today) == int.Parse(superGoldenDaysOfWeek[j]))
				{
					superApplied = true;
					break;
				}
			}

			double rate = pvprateGoldenTime(goldenTimeBegin, goldenTimeEnd, goldenTimeRate);
			if (superApplied)
			{
				double rate2 = pvprateGoldenTime(superGoldenTimeBegin, superGoldenTimeEnd, superGoldenTimeRate);
				if (rate2 > 0)
				{
					rate = rate2;
				}
			}

			return rate;
		}

		private int CalculateLeageScore(Player player, int winTeam)
		{
			int LeageScore = 1;
			if (m_roomType == eRoomType.Match)
			{
				float avgLevel = player.Team == 1 ? m_blueAvgLevel : m_redAvgLevel;
				float teamCount = player.Team == 1 ? m_blueTeam.Count : m_redTeam.Count;
				Math.Abs(avgLevel - player.PlayerDetail.PlayerCharacter.Grade);
				float isWin = player.Team == winTeam ? 2f : 0.0f;
				double expRate = 0.2;
				player.TotalShootCount = player.TotalShootCount == 0 ? 1 : player.TotalShootCount;
				if (player.TotalShootCount < player.TotalHitTargetCount)
				{
					player.TotalShootCount = player.TotalHitTargetCount;
				}
				int avgLevelCount = player.Team == 1 ? (int)(m_blueTeam.Count * m_blueAvgLevel * 300.0) : (int)(m_redAvgLevel * m_redTeam.Count * 300.0);
				int totalHurt = player.TotalHurt > avgLevelCount ? avgLevelCount : player.TotalHurt;
				LeageScore = (int)Math.Ceiling((isWin + totalHurt * (0.019 + expRate) + player.TotalKill * 0.5 + (player.TotalHitTargetCount / player.TotalShootCount * 2)) * avgLevel * (0.9 + (teamCount - 1.0) * 0.3) / 100);
			}
			return LeageScore < 1 ? 1 : LeageScore;
		}

		private int GetWinTeam(List<Player> players)
		{
			int winteam = -1;
			if (m_roomType == eRoomType.ConsortiaBattle)
			{
				bool flag = false;
				int total1 = 0;
				int total2 = 0;
				int team = -1;
				int totalFullHp = 0;
				foreach (Player current in players)
				{
					if (!current.IsLiving)
					{
						flag = true;
						break;
					}
					else
					{
						if (current.Team == 1)
						{
							total1 += current.TotalDamagePlayer;
						}
						else
						{
							total2 += current.TotalDamagePlayer;
							team = current.Team;
						}
						if (current.TotalDamagePlayer <= 0)
							totalFullHp++;
					}
				}
				if (totalFullHp >= players.Count && m_roomType == eRoomType.ConsortiaBattle)
					return -1;
				if (!flag)
				{
					return (total1 > total2) ? 1 : team;
				}
			}
			foreach (Player current in players)
			{
				if (current.IsLiving)
				{
					winteam = current.Team;
					break;
				}
			}
			int result;
			if (winteam == -1)
			{
				if (CurrentPlayer != null)
				{
					result = CurrentPlayer.Team;
					return result;
				}
				if (CurrentLiving != null)
				{
					winteam = CurrentLiving.Team;
				}
			}
			result = winteam;
			return result;
		}

		public bool CheckIp(List<Player> players, Player self)
        {
			string ip = self.PlayerDetail.TcpEndPoint();
			foreach (int p in m_logStartIps.Keys)
            {
				if (p != self.Id && m_logStartIps[p] == ip)
					return true;
            }
			return false;
        }

		private int CalculateOffer(Player player, int winTeam)
		{
			if (base.RoomType != 0)
			{
				return 0;
			}
			int appendOffer = 0;
			if (base.GameType == eGameType.Guild)
			{
				int againstTeamCount = ((player.Team == 1) ? m_blueTeam.Count : m_redTeam.Count);
				appendOffer = ((player.Team != winTeam) ? ((int)((double)againstTeamCount * 0.5)) : againstTeamCount);
			}
			int baseOffer = player.GainOffer;
			int offer = (int)(double)((float)baseOffer + (float)appendOffer);
			offer -= player.KilledPunishmentOffer;
			if (Convert.ToBoolean(ConfigurationManager.AppSettings["DoubleEvent"]))
			{
				offer *= 2;
			}
			if (offer > 1000)
			{
				log.Error($"pvegame ====== player.nickname : {player.PlayerDetail.PlayerCharacter.NickName}, add offer : {offer} ======== offer > 1000");
				log.Error($"pvegame ====== player.nickname : {player.PlayerDetail.PlayerCharacter.NickName}, parameters RoomType : {base.RoomType}, baseOffer : {baseOffer}, appendOffer : {appendOffer}");
			}
			return offer;
		}

		public void NextTurn()
		{
			if (base.GameState != eGameState.Playing)
			{
				return;
			}
			ClearWaitTimer();
			ClearDiedPhysicals();
			CheckBox();
			base.m_turnIndex++;
			List<Box> newBoxes = CreateBox();
			foreach (Physics item in m_map.GetAllPhysicalSafe())
			{
				item.PrepareNewTurn();
			}
			LastTurnLiving = m_currentLiving;
			m_currentLiving = FindNextTurnedLiving();
			if (m_currentLiving.VaneOpen)
			{
				UpdateWind(GetNextWind(), false);
			}
			if (m_currentLiving is Player && (m_currentLiving as Player).PlayerDetail.PlayerCharacter.NickName == ContinuousRunningPlayer)
			{
				foreach (Player allFightPlayer in GetAllFightPlayers())
				{
					allFightPlayer.PlayerDetail.SendMessage(string.Format("Người chơi {0} nhận được thêm 1 lần tấn công", ContinuousRunningPlayer));
				}
			}
			MinusDelays(m_currentLiving.Delay);
			m_currentLiving.PrepareSelfTurn();
			if (!base.CurrentLiving.IsFrost && m_currentLiving.IsLiving)
			{
				m_currentLiving.StartAttacking();
				SendGameNextTurn(m_currentLiving, this, newBoxes);
				if (m_currentLiving.IsAttacking)
				{
					AddAction(new WaitLivingAttackingAction(m_currentLiving, base.m_turnIndex, (m_timeType + 20) * 1000));
				}
			}
			if (m_currentLiving is Player && FindNextTurnedLiving() is Player)
			{
				ContinuousRunningPlayer = (m_currentLiving as Player).PlayerDetail.PlayerCharacter.NickName;
			}
			OnBeginNewTurn();
		}

		public void Prepare()
		{
			if (base.GameState == eGameState.Inited)
			{
				SendCreateGame();
				m_gameState = eGameState.Prepared;
				CheckState(0);
			}
		}

		public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
		{
			Player player = base.RemovePlayer(gp, IsKick);
			if (player != null && player.IsLiving && base.GameState != eGameState.Loading)
			{
				gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
				string msg = null;
				string translation = null;
				if (base.RoomType == eRoomType.Match)
				{
					if (base.GameType == eGameType.Guild)
					{
						msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 15);
						gp.RemoveOffer(15);
						translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 15);
					}
					else if (base.GameType == eGameType.Free)
					{
						msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 5);
						gp.RemoveOffer(5);
						translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 5);
					}
				}
				else
				{
					msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", gp.PlayerCharacter.Grade * 12);
					translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12);
				}
				SendMessage(gp, msg, translation, 3);
				if (GetSameTeam() && CurrentLiving != null)
				{
					base.CurrentLiving.StopAttacking();
					CheckState(0);
				}
			}
			return player;
		}

		private int Load_Kill_Suit(int userid)
		{
			List<int> allkill = new List<int>();
			using (PlayerBussiness A = new PlayerBussiness())
			{
				try
				{
					Suit_Manager S = new Suit_Manager();
					S = A.Get_Suit_Manager(userid);//////////////////
					if (S.UserID > 0)
					{
						string chuoi = S.Kill_List;
						if (chuoi.Length > 2)
						{
							while (chuoi.Contains(","))
							{
								int kq = 0;
								int.TryParse(chuoi.Substring(0, chuoi.IndexOf(",")), out kq);
								if (kq == 0)
								{
									break;
								}
								allkill.Add(kq);
								chuoi = chuoi.Remove(0, chuoi.IndexOf(",") + 1);
							}
							if (!chuoi.Contains(","))
							{
								int kq = 0;
								int.TryParse(chuoi, out kq);
								if (kq > 0)
								{
									allkill.Add(kq);
								}
							}
						}
					}
				}
				catch
				{

				}
			}
			if (allkill.Count > 0)
			{
				List<int> B = allkill;
				int kill = 0;
				foreach (int _kill in B)
				{
					switch (_kill)
					{
						case 1010807:
							kill += 5;
							break;
						case 1010808:
							kill += 5;
							break;
						case 1010812:
							kill += 1;
							break;
						case 1010814:
							kill += 1;
							break;
						case 1010813:
							kill += 2;
							break;
						case 1010815:
							kill += 2;
							break;
						case 1010816:
							kill += 2;
							break;
						case 1010822:
							kill += 2;
							break;
						case 1010809:
							kill += 5;
							break;
					}
				}
				return kill;
			}
			return 0;
		}

		public void StartGame()
		{
			if (base.GameState != eGameState.Loading)
			{
				return;
			}
			m_gameState = eGameState.Playing;
			ClearWaitTimer();
			SendSyncLifeTime();
			List<Player> allFightPlayers = GetAllFightPlayers();
			MapPoint mapRandomPos = MapMgr.GetMapRandomPos(m_map.Info.ID);

			// GSPacketIn pkg1 = new GSPacketIn(3);
			// pkg1.WriteInt(2);
			// pkg1.WriteString(string.Format("Map : {0}, ID : {1}", this.m_map.Info.Name, this.m_map.Info.ID));
			// this.SendToAll(pkg1, null);

			GSPacketIn pkg = new GSPacketIn(91);
			pkg.WriteByte(99);
			pkg.WriteInt(allFightPlayers.Count);
			foreach (Player player in allFightPlayers)
			{

				int kill = this.Load_Kill_Suit(player.PlayerDetail.PlayerCharacter.ID);
				if (kill > 0)
                {
					GSPacketIn pkg2 = new GSPacketIn(3);
					pkg2.WriteInt(2);
					pkg2.WriteString(string.Format("Trang bị VIP giúp [{0}] giảm thương thêm {1}%", player.PlayerDetail.PlayerCharacter.NickName, kill));
					this.SendToAll(pkg2, null);
				}
				player.Reset();
				Point playerPoint = GetPlayerPoint(mapRandomPos, player.Team);
				player.SetXY(playerPoint);
				m_map.AddPhysical(player);
				player.StartMoving();
				player.StartGame();
				pkg.WriteInt(player.Id);
				pkg.WriteInt(player.X);
				pkg.WriteInt(player.Y);
				pkg.WriteInt(player.Direction);
				pkg.WriteInt(player.Blood);
				pkg.WriteInt(player.MaxBlood);
				pkg.WriteInt(player.Team);
				pkg.WriteInt(player.Weapon.RefineryLevel);
				pkg.WriteInt(50);
				pkg.WriteInt(player.Dander);
				pkg.WriteInt(player.PlayerDetail.FightBuffs.Count);
				foreach (BufferInfo fightBuff in player.PlayerDetail.FightBuffs)
				{
					pkg.WriteInt(fightBuff.Type);
					pkg.WriteInt(fightBuff.Value);
				}
				pkg.WriteInt(0);
				pkg.WriteBoolean(player.IsFrost);
				pkg.WriteBoolean(player.IsHide);
				pkg.WriteBoolean(player.IsNoHole);
				pkg.WriteBoolean(val: false);
				pkg.WriteInt(0);
				AddLogIp(player.Id, player.PlayerDetail.TcpEndPoint());
			}
			pkg.WriteDateTime(beginTime);
			SendToAll(pkg);
			VaneLoading();
			WaitTime(allFightPlayers.Count * 1000);
			OnGameStarted();
		}

		public void StartLoading()
		{
            if (GameState == eGameState.Prepared)
            {
                ClearWaitTimer();
                SendStartLoading(60);
                VaneLoading();
                AddAction(new WaitPlayerLoadingAction(this, 61 * 1000));
                m_gameState = eGameState.Loading;
            }
        }

		public override void Stop()
		{
			if (base.GameState == eGameState.GameOver)
			{
				this.m_gameState = eGameState.Stopped;
				List<Player> players = base.GetAllFightPlayers();
				foreach (Player p in players)
				{
					if (p.IsActive && !p.FinishTakeCard && p.CanTakeOut > 0)
					{
						this.TakeCard(p);
					}
				}
				Dictionary<int, Player> players2;
				Monitor.Enter(players2 = this.m_players);
				try
				{
					this.m_players.Clear();
				}
				finally
				{
					Monitor.Exit(players2);
				}
				base.Stop();
			}
		}
	}
}
