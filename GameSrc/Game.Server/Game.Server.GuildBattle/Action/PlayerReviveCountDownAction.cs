using Game.Logic;
using Game.Server.Games;
using Game.Server.GuildBattle;
using log4net;
using System;
using System.Reflection;

namespace Game.Server.GuildBattle.Action
{
	public class PlayerReviveCountDownAction : GInterface13
	{
		private readonly static ILog ilog_0;

		private long long_0;

		private bool bool_0;

		private UserGuildBattleInfo userGuildBattleInfo_0;

		private bool bool_1;

		static PlayerReviveCountDownAction()
		{

			PlayerReviveCountDownAction.ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}

		public PlayerReviveCountDownAction(UserGuildBattleInfo player, int delay, bool stayRespawn)
		{


			this.userGuildBattleInfo_0 = player;
			this.bool_0 = false;
			this.bool_1 = stayRespawn;
			this.long_0 = this.long_0 + TickHelper.GetTickCount() + (long)delay;
		}

		public void Execute(GuildBattleMgr game, long tick)
		{
			if (this.long_0 <= tick)
			{
				if (this.userGuildBattleInfo_0 != null && this.userGuildBattleInfo_0.IsActive && this.userGuildBattleInfo_0.IsDead)
				{
					this.userGuildBattleInfo_0.IsDead = false;
					this.userGuildBattleInfo_0.TombStoneEndTime = DateTime.Now.AddHours(-1);
					if (!this.bool_1)
					{
						GuildBattleConsortiaInfo guildBattleConsortiaInfo = GameMgr.GuildBattle.FindConsortia(this.userGuildBattleInfo_0.ConsortiaID);
						if (guildBattleConsortiaInfo != null)
						{
							this.userGuildBattleInfo_0.Postion = guildBattleConsortiaInfo.DefaultPoint;
						}
					}
					GameMgr.GuildBattle.SendUpdateSceneInfo(this.userGuildBattleInfo_0);
					GameMgr.GuildBattle.SendUpdatePlayerStatus(this.userGuildBattleInfo_0);
				}
				this.bool_0 = true;
			}
		}

		public bool IsFinished(long tick)
		{
			return this.bool_0;
		}
	}
}