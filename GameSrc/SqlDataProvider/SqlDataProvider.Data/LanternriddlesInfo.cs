using System;
using System.Collections.Generic;

namespace SqlDataProvider.Data
{
	public class LanternriddlesInfo
	{
		private int m_doubleFreeCount;

		private int m_doublePrice;

		private DateTime m_endDate;

		private int m_hitFreeCount;

		private int m_hitPrice;

		private bool m_isDouble;

		private bool m_isHint;

		private int m_myInteger;

		private int m_option;

		private int m_playerID;

		private int m_questionIndex;

		private int m_questionNum;

		private int m_questionView;

		private Dictionary<int, LightriddleQuestInfo> m_questViews;

		public bool CanNextQuest
		{
			get
			{
				return this.m_questionIndex <= this.m_questionView - 1;
			}
		}

		public int DoubleFreeCount
		{
			get
			{
				return this.m_doubleFreeCount;
			}
			set
			{
				this.m_doubleFreeCount = value;
			}
		}

		public int DoublePrice
		{
			get
			{
				return this.m_doublePrice;
			}
			set
			{
				this.m_doublePrice = value;
			}
		}

		public DateTime EndDate
		{
			get
			{
				return this.m_endDate;
			}
			set
			{
				this.m_endDate = value;
			}
		}

		public LightriddleQuestInfo GetCurrentQuestion
		{
			get
			{
				LightriddleQuestInfo result;
				if (this.m_questViews != null)
				{
					result = this.m_questViews[this.m_questionIndex];
				}
				else
				{
					result = this.m_questViews[1];
				}
				return result;
			}
		}

		public int GetQuestionID
		{
			get
			{
				int result;
				if (this.m_questViews != null)
				{
					result = this.m_questViews[this.m_questionIndex].QuestionID;
				}
				else
				{
					result = 1;
				}
				return result;
			}
		}

		public int HitFreeCount
		{
			get
			{
				return this.m_hitFreeCount;
			}
			set
			{
				this.m_hitFreeCount = value;
			}
		}

		public int HitPrice
		{
			get
			{
				return this.m_hitPrice;
			}
			set
			{
				this.m_hitPrice = value;
			}
		}

		public bool IsDouble
		{
			get
			{
				return this.m_isDouble;
			}
			set
			{
				this.m_isDouble = value;
			}
		}

		public bool IsHint
		{
			get
			{
				return this.m_isHint;
			}
			set
			{
				this.m_isHint = value;
			}
		}

		public int MyInteger
		{
			get
			{
				return this.m_myInteger;
			}
			set
			{
				this.m_myInteger = value;
			}
		}

		public int Option
		{
			get
			{
				return this.m_option;
			}
			set
			{
				this.m_option = value;
			}
		}

		public int PlayerID
		{
			get
			{
				return this.m_playerID;
			}
			set
			{
				this.m_playerID = value;
			}
		}

		public int QuestionIndex
		{
			get
			{
				return this.m_questionIndex;
			}
			set
			{
				this.m_questionIndex = value;
			}
		}

		public int QuestionNum
		{
			get
			{
				return this.m_questionNum;
			}
			set
			{
				this.m_questionNum = value;
			}
		}

		public int QuestionView
		{
			get
			{
				return this.m_questionView;
			}
			set
			{
				this.m_questionView = value;
			}
		}

		public Dictionary<int, LightriddleQuestInfo> QuestViews
		{
			get
			{
				return this.m_questViews;
			}
			set
			{
				this.m_questViews = value;
			}
		}
	}
}
