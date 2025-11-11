using Game.Logic;
using Game.Logic.AI;
using System;

namespace GameServerScript.AI.Game
{
	public class LauDaiPhepThuatSimple : APVEGameControl
	{
		public override void OnCreated()
		{
			base.OnCreated();
			base.Game.SetupMissions("71055,71056");
			base.Game.TotalMissionCount = 2;
		}

		public override void OnPrepated()
		{
			base.OnPrepated();
		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			int result;
			if (score > 900)
			{
				result = 3;
			}
			else if (score > 825)
			{
				result = 2;
			}
			else if (score > 725)
			{
				result = 1;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public override void OnGameOverAllSession()
		{
			base.OnGameOverAllSession();
		}
	}
}
