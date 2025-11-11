using System;

namespace Game.Server.ConsortiaTask.Data
{
	public class ConsortiaTaskDataInfo
	{
		public int ConsortiaID
		{
			get;
			set;
		}

		public bool IsActive
		{
			get;
			set;
		}

		public bool CanRemove
		{
			get;
			set;
		}

		public int TotalExp
		{
			get;
			set;
		}

		public int TotalRiches
		{
			get;
			set;
		}

		public int TotalOffer
		{
			get;
			set;
		}

		public int BuffID
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

		public bool IsComplete
		{
			get;
			set;
		}

		public int VaildDate
		{
			get;
			set;
		}

		public DateTime StartTime
		{
			get;
			set;
		}

		public ConsortiaTaskDataInfo(int consortiaId, int totalExp, int totalRiches, int totalOffer, int buffId, int vaild)
		{
			ConsortiaID = consortiaId;
			TotalExp = totalExp;
			TotalRiches = totalRiches;
			TotalOffer = totalOffer;
			BuffID = buffId;
			VaildDate = vaild;
			StartTime = DateTime.Now;
			IsActive = false;
			CanRemove = false;
		}

		public int GetTotalConditionCompleted()
		{
			return Condition1 + Condition2 + Condition3;
		}

		public bool IsVaildDate()
		{
			return StartTime.AddMinutes(VaildDate) > DateTime.Now;
		}
	}
}
