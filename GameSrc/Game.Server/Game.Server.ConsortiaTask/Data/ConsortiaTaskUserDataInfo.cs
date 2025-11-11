using System;
using System.Collections.Generic;
using Game.Server.GameObjects;

namespace Game.Server.ConsortiaTask.Data
{
	public class ConsortiaTaskUserDataInfo
	{
		public int UserID
		{
			get;
			set;
		}

		public int Condition1
		{
			get;
			set;
		}

		public int Condition2
		{
			get;
			set;
		}

		public int Condition3
		{
			get;
			set;
		}

		public GamePlayer Player
		{
			get;
			set;
		}

		public List<BaseConsortiaTaskCondition> ConditionList
		{
			get;
			set;
		}

		public int GetTotalConditionCompleted()
		{
			return Condition1 + Condition2 + Condition3;
		}

		public int GetConditionValue(int index)
		{
			switch (index)
			{
			case 0:
				return Condition1;
			case 1:
				return Condition2;
			case 2:
				return Condition3;
			default:
				throw new Exception("Consortia Task condition index out of range.");
			}
		}

		public void SaveConditionValue(int index, int value)
		{
			switch (index)
			{
			case 0:
				Condition1 = value;
				break;
			case 1:
				Condition2 = value;
				break;
			case 2:
				Condition3 = value;
				break;
			default:
				throw new Exception("Consortia Task condition index out of range.");
			}
		}
	}
}
