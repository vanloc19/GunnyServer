using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace Game.Server.Quests
{
	public class BaseQuest
	{
		private QuestDataInfo m_data;

		private QuestInfo m_info;

		private List<BaseCondition> m_list;

		private DateTime m_oldFinishDate;

		private GamePlayer m_player;

		public QuestDataInfo Data => m_data;

		public QuestInfo Info => m_info;

		public BaseQuest(QuestInfo info, QuestDataInfo data)
		{
			m_info = info;
			m_data = data;
			m_data.QuestID = m_info.ID;
			m_list = new List<BaseCondition>();
			List<QuestConditionInfo> questCondiction = QuestMgr.GetQuestCondiction(info);
			int num = 0;
			foreach (QuestConditionInfo info2 in questCondiction)
			{
				BaseCondition item = BaseCondition.CreateCondition(this, info2, data.GetConditionValue(num++));
				if (item != null)
				{
					m_list.Add(item);
				}
			}
		}

		public void AddToPlayer(GamePlayer player)
		{
			m_player = player;
			if (!m_data.IsComplete)
			{
				AddTrigger(player);
			}
		}

		private void AddTrigger(GamePlayer player)
		{
			foreach (BaseCondition item in m_list)
			{
				item.AddTrigger(player);
			}
		}

		public bool CancelFinish(GamePlayer player)
		{
			m_data.IsComplete = false;
			m_data.CompletedDate = m_oldFinishDate;
			foreach (BaseCondition item in m_list)
			{
				item.CancelFinish(player);
			}
			return true;
		}

		public bool CanCompleted(GamePlayer player)
		{
			if (m_data.IsComplete)
			{
				return false;
			}
			int notMustCount = m_info.NotMustCount;
			foreach (BaseCondition condition in m_list)
			{
				if (!condition.IsCompleted(player))// && m_data.QuestID != 70)
				{
					if (!condition.Info.isOpitional)
					{
						return false;
					}
				}
				else
				{
					notMustCount--;
				}
			}
			return notMustCount <= 0;
		}

		public bool Finish(GamePlayer player)
		{
			if (CanCompleted(player))
			{
				foreach (BaseCondition item in m_list)
				{
					if (!item.Finish(player))
					{
						return false;
					}
				}
				if (!Info.CanRepeat)
				{
					m_data.IsComplete = true;
					RemveTrigger(player);
				}
				m_oldFinishDate = m_data.CompletedDate;
				m_data.CompletedDate = DateTime.Now;
				return true;
			}
			return false;
		}

		public BaseCondition GetConditionById(int id)
		{
			foreach (BaseCondition condition in m_list)
			{
				if (condition.Info.CondictionID == id)
				{
					return condition;
				}
			}
			return null;
		}

		public void RemoveFromPlayer(GamePlayer player)
		{
			if (!m_data.IsComplete)
			{
				RemveTrigger(player);
			}
			m_player = null;
		}

		private void RemveTrigger(GamePlayer player)
		{
			foreach (BaseCondition item in m_list)
			{
				item.RemoveTrigger(player);
			}
		}

		public void Reset(GamePlayer player)
		{
			foreach (BaseCondition item in m_list)
			{
				item.Reset(player);
			}
		}

		public void CheckRepeat()
        {
			if((DateTime.Now.Date - m_data.CompletedDate.Date).TotalDays >= (double)m_info.RepeatInterval && m_info.CanRepeat && m_info.RepeatInterval > 0)
            {
				m_data.RepeatFinish = m_info.RepeatMax;
            }
        }

		public void Reset(GamePlayer player, int rand)
		{
			m_data.QuestID = m_info.ID;
			m_data.UserID = player.PlayerId;
			m_data.IsComplete = false;
			m_data.IsExist = true;
			if (m_data.CompletedDate == DateTime.MinValue)
			{
				m_data.CompletedDate = DateTime.Now;
			}
			CheckRepeat();
			//if ((DateTime.Now - m_data.CompletedDate).TotalDays >= (double)m_info.RepeatInterval)
			//{
			//	m_data.RepeatFinish = m_info.RepeatMax;
			//}
			//if (!m_info.CanRepeat)
			//{
			//	m_data.RepeatFinish--;
			//}
			//else
			//{
			//	m_data.RepeatFinish = m_info.RepeatMax;
			//}
			m_data.RandDobule = rand;
			if (!m_info.CanRepeat || player.QuestInventory.FindQuest(m_info.ID).Data != null)
			{
				foreach (BaseCondition item in m_list)
				{
					item.Reset(player);
				}
			}
			SaveData();
		}

		public void SaveData()
		{
			int num = 0;
			foreach (BaseCondition condition in m_list)
			{
				m_data.SaveConditionValue(num++, condition.Value);
			}
		}

		public void Update()
		{
			SaveData();
			if (m_data.IsDirty && m_player != null)
			{
				m_player.QuestInventory.Update(this);
			}
		}
	}
}
