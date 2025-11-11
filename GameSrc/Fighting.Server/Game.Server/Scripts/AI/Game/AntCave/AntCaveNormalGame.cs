using Game.Logic;
using Game.Logic.AI;
using System;

namespace GameServerScript.AI.Game
{
	public class AntCaveNormalGame : APVEGameControl
	{
		public override int CalculateScoreGrade(int score)
		{
			if (score > 800)
			{
				return 3;
			}
			if (score > 725)
			{
				return 2;
			}
			if (score > 650)
			{
				return 1;
			}
			return 0;
		}

		public override void OnCreated()
		{
			base.Game.SetupMissions("2101, 2102");
			base.Game.TotalMissionCount = 2;
		}

		public override void OnGameOverAllSession()
		{
			base.OnGameOverAllSession();
		}

		public override void OnPrepated()
		{
		}
	}
}
