using Game.Logic;
using Game.Logic.AI;
using System;

namespace GameServerScript.AI.Game
{
	public class CryptbossSimpleGame4 : APVEGameControl
	{
		public CryptbossSimpleGame4()
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

		public override void OnCreated()
		{
			base.Game.SetupMissions("5020401");
			base.Game.TotalMissionCount = 1;
		}

		public override void OnGameOverAllSession()
		{
		}

		public override void OnPrepated()
		{
			base.Game.SessionId = 0;
		}
	}
}