using System.Reflection;
using Bussiness.Managers;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Event
{
	public class EventCondition
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected EventLiveInfo m_event;

		protected GamePlayer m_player;

		public EventCondition(EventLiveInfo eventLive, GamePlayer player)
		{
			m_event = EventLiveMgr.GetSingleEvent(eventLive.EventID);
			m_player = player;
		}

		public virtual void AddTrigger(GamePlayer player)
		{
		}

		public static EventCondition CreateCondition(EventLiveInfo eventLive, GamePlayer player)
		{
			switch (eventLive.CondictionType)
			{
			case 1:
				return new EventStrengthenCondition(eventLive, player);
			case 2:
				return new GameOverCondition(eventLive, player);
			case 3:
				return new MoneyChargeCondition(eventLive, player);
			case 4:
				return new GameKillCondition(eventLive, player);
			case 5:
				return new PlayerLoginCondition(eventLive, player);
			case 6:
				return new UseBalanceCondition(eventLive, player);
			case 7:
				return new FusionItemCondition(eventLive, player);
			case 8:
				return new LevelUpCondition(eventLive, player);
			default:
				return new UnknownCondition(eventLive, player);
			}
		}

		public virtual void RemoveTrigger(GamePlayer player)
		{
		}
	}
}
