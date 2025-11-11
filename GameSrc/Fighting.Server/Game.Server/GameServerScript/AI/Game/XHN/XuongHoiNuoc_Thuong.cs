using Game.Logic.AI;
using System;

namespace GameServerScript.AI.Game
{
	public class XuongHoiNuoc_Thuong : APVEGameControl
	{
		public override void OnCreated()
		{
			base.Game.SetupMissions("510004,510005,510006,510007");
			//base.Game.SetupMissions("510007");
			base.Game.TotalMissionCount = 4;
		}

        public override void OnPrepated()
        {
            base.Game.SessionId = 0;
            base.OnPrepated();
        }

        public override int CalculateScoreGrade(int score)
		{
			int result;
			if (score > 800)
			{
				result = 3;
			}
			else if (score > 725)
			{
				result = 2;
			}
			else if (score > 650)
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
