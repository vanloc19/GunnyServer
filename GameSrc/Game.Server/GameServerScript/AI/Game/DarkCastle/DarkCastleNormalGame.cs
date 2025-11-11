using Game.Logic.AI;

namespace GameServerScript.AI.Game
{
	public class DarkCastleNormalGame : APVEGameControl
	{
		public override void OnCreated()
		{
			base.Game.SetupMissions("4101,4102,4103");
			base.Game.TotalMissionCount = 3;
		}

		public override void OnPrepated()
		{
		}

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

		public override void OnGameOverAllSession()
		{
		}
	}
}
