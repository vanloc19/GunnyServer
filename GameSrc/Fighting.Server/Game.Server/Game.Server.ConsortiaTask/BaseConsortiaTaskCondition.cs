using System.Reflection;
using Game.Server.ConsortiaTask.Condition;
using Game.Server.ConsortiaTask.Data;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask
{
	public class BaseConsortiaTaskCondition
	{
		protected ConsortiaTaskInfo m_info;

		private static readonly ILog ilog_0;

		private int int_0;

		private BaseConsortiaTask baseConsortiaTask_0;

		private ConsortiaTaskUserDataInfo consortiaTaskUserDataInfo_0;

		public ConsortiaTaskInfo Info => m_info;

		public int Value
		{
			get
			{
				return int_0;
			}
			set
			{
				if (int_0 < value)
				{
					int valueAdd = value - int_0;
					baseConsortiaTask_0.RemakeValue(m_info.ID, ref valueAdd);
					if (valueAdd > 0)
					{
						int_0 += valueAdd;
						baseConsortiaTask_0.Update(consortiaTaskUserDataInfo_0);
					}
				}
			}
		}

		public BaseConsortiaTaskCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask consortiaTask, ConsortiaTaskInfo info, int value)
		{
			baseConsortiaTask_0 = consortiaTask;
			m_info = info;
			consortiaTaskUserDataInfo_0 = player;
			int_0 = value;
		}

		public virtual void AddTrigger(ConsortiaTaskUserDataInfo player)
		{
		}

		public virtual void RemoveTrigger(ConsortiaTaskUserDataInfo player)
		{
		}

		public static BaseConsortiaTaskCondition CreateCondition(ConsortiaTaskUserDataInfo player, BaseConsortiaTask quest, ConsortiaTaskInfo info, int value)
		{
			switch (info.CondictionType)
			{
				case 3:
					return new UsingItemCondition(player, quest, info, value);
				case 4:
					return new UseBigBugleCondition(player, quest, info, value);
				case 5:
					return new GameFightByRoomCondition(player, quest, info, value);
				case 6:
					return new GameOverByRoomCondition(player, quest, info, value);
				case 13:
					return new GameMonsterCondition(player, quest, info, value);
				case 21:
					return new GameMissionOverCondition(player, quest, info, value);
				case 22:
					return new GameKillByGameCondition(player, quest, info, value);
				case 23:
					return new GameFightByGameCondition(player, quest, info, value);
				case 34:
					return new GameFight2v2Condition(player, quest, info, value);
				case 38:
					return new RechargeMoneyCondition(player, quest, info, value);
				default:
					if (ilog_0.IsErrorEnabled)
					{
						ilog_0.Error($"Can't find consortia task condition : {info.CondictionType}");
					}
					return null;
			}
		}

		static BaseConsortiaTaskCondition()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
