using Game.Logic.AI;

namespace GameServerScript.AI.Game
{
	public class GuluKingdomTerrorGame : APVEGameControl
	{
		public override void OnCreated()
		{
			base.OnCreated();
			base.Game.SetupMissions("1371,1372,1373,1374,1375");
			base.Game.TotalMissionCount = 5;
		}

		public override void OnPrepated()
		{
		}

		public override int CalculateScoreGrade(int score)
		{
			base.CalculateScoreGrade(score);
			if (score > 800)
			{
				return 3;
			}
			if (score > 825)
			{
				return 2;
			}
			if (score > 725)
			{
				return 1;
			}
			return 0;
		}

		public override void OnGameOverAllSession()
		{
			base.OnGameOverAllSession();
		}

		public GuluKingdomTerrorGame()
		{


		}
	}
}
