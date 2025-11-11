using System;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Event
{
	public class EventInventory
	{
		private object m_lock;

		private GamePlayer m_player;

		public EventInventory(GamePlayer player)
		{
			m_player = player;
			m_lock = new object();
		}

		public void LoadFromDatabase()
		{
			lock (m_lock)
			{
				foreach (EventLiveInfo eventL in EventLiveMgr.GetAllEventInfo())
				{
					if (eventL.StartDate < DateTime.Now && eventL.EndDate > DateTime.Now)
					{
						EventCondition.CreateCondition(eventL, m_player);
					}
				}
			}
		}
	}
}
