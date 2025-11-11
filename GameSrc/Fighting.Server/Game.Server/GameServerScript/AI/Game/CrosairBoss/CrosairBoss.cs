using System;
using Game.Logic.AI;

namespace GameServerScript.AI.Game
{
    // Token: 0x020001B9 RID: 441
    public class CrosairBoss : APVEGameControl
    {
        // Token: 0x0600170D RID: 5901 RVA: 0x000B18DC File Offset: 0x000AFADC
        public override int CalculateScoreGrade(int score)
        {
            bool flag = score > 800;
            bool flag2 = flag;
            int result;
            if (flag2)
            {
                result = 3;
            }
            else
            {
                bool flag3 = score > 725;
                bool flag4 = flag3;
                if (flag4)
                {
                    result = 2;
                }
                else
                {
                    bool flag5 = score > 650;
                    bool flag6 = flag5;
                    if (flag6)
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

        // Token: 0x0600170E RID: 5902 RVA: 0x000B193B File Offset: 0x000AFB3B
        public override void OnCreated()
        {
            base.Game.SetupMissions("12016");
            base.Game.TotalMissionCount = 1;
        }

        // Token: 0x0600170F RID: 5903 RVA: 0x00002A44 File Offset: 0x00000C44
        public override void OnGameOverAllSession()
        {
        }

        // Token: 0x06001710 RID: 5904 RVA: 0x000B195B File Offset: 0x000AFB5B
        public override void OnPrepated()
        {
            base.Game.SessionId = 0;
        }
    }
}
