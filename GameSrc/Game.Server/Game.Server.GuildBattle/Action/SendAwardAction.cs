using Bussiness;
using Bussiness.Managers;
using Game.Logic;
using Game.Server;
using Game.Server.GuildBattle;
using log4net;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Game.Server.GuildBattle.Action
{
	public class SendAwardAction : GInterface13
	{
		private readonly static ILog ilog_0;

		private long long_0;

		private bool bool_0;

		static SendAwardAction()
		{

			SendAwardAction.ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public SendAwardAction(int delay)
		{


			this.bool_0 = false;
			this.long_0 = this.long_0 + TickHelper.GetTickCount() + (long)delay;
		}

		public void Execute(GuildBattleMgr game, long tick)
		{
			if (game.State == GuildBattleState.CLOSE && !game.IsSendAward)
			{
				game.IsSendAward = true;
				List<ItemInfo> eventAwardByType = null;
				string translation = "";
				GuildBattleConsortiaInfo[] array = (
					from a in (IEnumerable<GuildBattleConsortiaInfo>)game.GetAllConsortia()
					orderby a.Score descending
					select a).ToArray<GuildBattleConsortiaInfo>();
				UserGuildBattleInfo[] userGuildBattleInfoArray = game.GetAllUser().ToArray<UserGuildBattleInfo>();
				int num = 1;
				int num1 = 1;
				GuildBattleConsortiaInfo[] guildBattleConsortiaInfoArray = array;
				for (int i = 0; i < (int)guildBattleConsortiaInfoArray.Length; i++)
				{
					GuildBattleConsortiaInfo guildBattleConsortiaInfo = guildBattleConsortiaInfoArray[i];
					IEnumerable<UserGuildBattleInfo> consortiaID =
						from a in userGuildBattleInfoArray
						where a.ConsortiaID == guildBattleConsortiaInfo.ConsortiaID
						select a;
					num1 = 1;
					UserGuildBattleInfo[] array1 = (
						from a in consortiaID
						orderby a.Score descending
						select a).ToArray<UserGuildBattleInfo>();
					for (int j = 0; j < (int)array1.Length; j++)
					{
						UserGuildBattleInfo userGuildBattleInfo = array1[j];
						switch (num)
						{
							case 1:
								{
									eventAwardByType = EventAwardMgr.GetEventAwardByType(eEventType.GUILD_BATTLE_TOP_1);
									translation = LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAward.Content", new object[] { num });
									break;
								}
							case 2:
								{
									eventAwardByType = EventAwardMgr.GetEventAwardByType(eEventType.GUILD_BATTLE_TOP_2);
									translation = LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAward.Content", new object[] { num });
									break;
								}
							case 3:
								{
									eventAwardByType = EventAwardMgr.GetEventAwardByType(eEventType.GUILD_BATTLE_TOP_3);
									translation = LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAward.Content", new object[] { num });
									break;
								}
							default:
								{
									eventAwardByType = EventAwardMgr.GetEventAwardByType(eEventType.GUILD_BATTLE_TOP_4);
									translation = LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAward.Content", new object[] { num });
									break;
								}
						}
						if (eventAwardByType != null && eventAwardByType.Count > 0)
						{
							WorldEventMgr.SendItemsToMails(eventAwardByType, userGuildBattleInfo.UserID, userGuildBattleInfo.NickName, GameServer.Instance.Configuration.ZoneId, null, translation);
						}
						if (num1 <= 20)
						{
							eventAwardByType = EventAwardMgr.GetEventAwardByType(eEventType.GUILD_BATTLE_PERSON_TOP);
							if (eventAwardByType != null && eventAwardByType.Count > 0)
							{
								WorldEventMgr.SendItemsToMails(eventAwardByType, userGuildBattleInfo.UserID, userGuildBattleInfo.NickName, GameServer.Instance.Configuration.ZoneId, null, LanguageMgr.GetTranslation("GameServer.GuildBattle.MailAwardPerson.Title"));
							}
						}
						num1++;
					}
					num++;
				}
				game.SaveCurrentRankToDatabase(true);
			}
			this.bool_0 = true;
		}

		public bool IsFinished(long tick)
		{
			return this.bool_0;
		}
	}
}