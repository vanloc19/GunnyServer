using System;
using Game.Logic.AI;

namespace GameServerScript.AI.Game
{
	// Token: 0x020001E4 RID: 484
	public class NguoiCa_Thuong : APVEGameControl
	{
		// Token: 0x060017E4 RID: 6116 RVA: 0x000B2EF3 File Offset: 0x000B10F3
		public override void OnCreated()
		{
			base.Game.SetupMissions("410001,410002,410003");
			base.Game.TotalMissionCount = 1;
		}

		// Token: 0x060017E5 RID: 6117 RVA: 0x000B2E1B File Offset: 0x000B101B
		public override void OnPrepated()
		{
			base.Game.SessionId = 0;
			base.OnPrepated();
		}

		// Token: 0x060017E6 RID: 6118 RVA: 0x000B2F14 File Offset: 0x000B1114
		public override int CalculateScoreGrade(int score)
		{
			bool flag = score > 800;
			int result;
			if (flag)
			{
				result = 3;
			}
			else
			{
				bool flag2 = score > 725;
				if (flag2)
				{
					result = 2;
				}
				else
				{
					bool flag3 = score > 650;
					if (flag3)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x060017E7 RID: 6119 RVA: 0x000B17C7 File Offset: 0x000AF9C7
		public override void OnGameOverAllSession()
		{
			base.OnGameOverAllSession();
		}
	}
}
