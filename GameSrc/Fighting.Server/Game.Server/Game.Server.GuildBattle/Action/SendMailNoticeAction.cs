using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using Game.Server;
using Game.Server.GuildBattle;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Reflection;

namespace Game.Server.GuildBattle.Action
{
	public class SendMailNoticeAction : GInterface13
	{
		private readonly static ILog ilog_0;

		private long long_0;

		private bool bool_0;

		static SendMailNoticeAction()
		{

			SendMailNoticeAction.ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public SendMailNoticeAction(int delay)
		{


			this.bool_0 = false;
			this.long_0 = this.long_0 + TickHelper.GetTickCount() + (long)delay;
		}

		public void Execute(GuildBattleMgr game, long tick)
		{
			if (game.State == GuildBattleState.CHECKING && !game.IsSendMail)
			{
				game.IsSendMail = true;
				try
				{
					ConsortiaInfo[] tOPConsortiaDayOnline = game.GetTOPConsortiaWeekRiches();
					if (tOPConsortiaDayOnline != null && tOPConsortiaDayOnline.Length != 0)
					{
						string str = game.InstallConsortiaList(tOPConsortiaDayOnline);
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							ConsortiaInfo[] consortiaInfoArray = tOPConsortiaDayOnline;
							for (int i = 0; i < (int)consortiaInfoArray.Length; i++)
							{
								ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(consortiaInfoArray[i].ConsortiaID);
								for (int j = 0; j < (int)allMemberByConsortia.Length; j++)
								{
									ConsortiaUserInfo consortiaUserInfo = allMemberByConsortia[j];
									if (consortiaUserInfo.LastDate.AddMonths(1) >= DateTime.Now)
									{
										int userID = consortiaUserInfo.UserID;
										string userName = consortiaUserInfo.UserName;
										int areaId = GameServer.Instance.Configuration.ZoneId;
										string translation = LanguageMgr.GetTranslation("GameServer.GuildBattle.MailNotice.Title");
										object[] shortTimeString = new object[] { game.GuildBattleStartTime.ToShortTimeString(), null, null };
										DayOfWeek dayOfWeek = DateTime.Now.DayOfWeek;
										shortTimeString[1] = LanguageMgr.GetTranslation(string.Concat("Global.DayOfWeek.Msg.", dayOfWeek.ToString()));
										shortTimeString[2] = str;
										//Îòïðàâêà ïèñüìà GuildBattle
										//WorldEventMgr.SendMailToUser(userID, userName, areaId, translation, LanguageMgr.GetTranslation("GameServer.GuildBattle.MailNotice.Content", shortTimeString));
									}
								}
							}
						}
					}
				}
				catch (Exception exception)
				{
					SendMailNoticeAction.ilog_0.Error(exception);
				}
			}
			this.bool_0 = true;
		}

		public bool IsFinished(long tick)
		{
			return this.bool_0;
		}
	}
}