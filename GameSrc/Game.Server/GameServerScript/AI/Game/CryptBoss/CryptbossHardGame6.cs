using Game.Logic;
using Game.Logic.AI;
using System;

namespace GameServerScript.AI.Game
{
	public class CryptbossHardGame6 : APVEGameControl
	{
		public CryptbossHardGame6()
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
			base.Game.SetupMissions("5020603");
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