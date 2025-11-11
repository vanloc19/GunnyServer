using Game.Logic;
using Game.Server.GuildBattle;
using log4net;
using System;
using System.Reflection;

namespace Game.Server.GuildBattle.Action
{
	public class CheckGuildBattleStateAction : GInterface13
	{
		private readonly static ILog ilog_0;

		private long long_0;

		private bool bool_0;

		static CheckGuildBattleStateAction()
		{

			CheckGuildBattleStateAction.ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public CheckGuildBattleStateAction(int delay)
		{


			this.bool_0 = false;
			this.long_0 = this.long_0 + TickHelper.GetTickCount() + (long)delay;
		}

		public void Execute(GuildBattleMgr game, long tick)
		{
			if (this.long_0 <= tick)
			{
				DateTime now = DateTime.Now;
				switch (game.State)
				{
					case GuildBattleState.CLOSE:
						{
							game.State = GuildBattleState.CHECKING;
							break;
						}
					case GuildBattleState.CHECKING:
						{
							if (!game.IsSendMail && now.Hour >= 17 && now.Hour < 18 && !game.CheckAction(typeof(SendMailNoticeAction)))
							{
								game.AddAction(new SendMailNoticeAction(1000));
							}
							if (!game.CanStartGame(now))
							{
								break;
							}
							game.ChangeOpenClose(true, DateTime.Now.AddHours(1));
							break;
						}
					case GuildBattleState.OPEN:
						{
							if (game.TimeStop > now)
							{
								break;
							}
							game.ChangeOpenClose(false);
							if (game.CheckAction(typeof(SendAwardAction)))
							{
								break;
							}
							game.AddAction(new SendAwardAction(1000));
							break;
						}
				}
				game.WaitTime(1000);
				this.bool_0 = true;
			}
		}

		public bool IsFinished(long tick)
		{
			return this.bool_0;
		}
	}
}