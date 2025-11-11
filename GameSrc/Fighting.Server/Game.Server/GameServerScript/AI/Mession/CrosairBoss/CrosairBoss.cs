using System;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
    // Token: 0x0200012E RID: 302
    public class CrosairBoss : AMissionControl
    {
        // Token: 0x060010A7 RID: 4263 RVA: 0x0007CBA2 File Offset: 0x0007ADA2
        public CrosairBoss()
        {
            this.int_0 = 72000;
        }

        // Token: 0x060010A8 RID: 4264 RVA: 0x0007CBB8 File Offset: 0x0007ADB8
        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            bool flag = score > 1750;
            bool flag2 = flag;
            int result;
            if (flag2)
            {
                result = 3;
            }
            else
            {
                bool flag3 = score > 1675;
                bool flag4 = flag3;
                if (flag4)
                {
                    result = 2;
                }
                else
                {
                    bool flag5 = score > 1600;
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

        // Token: 0x060010A9 RID: 4265 RVA: 0x0007CC20 File Offset: 0x0007AE20
        public override bool CanGameOver()
        {
            bool flag = this.simpleBoss_0 == null || this.simpleBoss_0.IsLiving;
            bool flag2 = flag;
            bool result;
            if (flag2)
            {
                result = false;
            }
            else
            {
                this.int_1++;
                result = true;
            }
            return result;
        }

        // Token: 0x060010AA RID: 4266 RVA: 0x00077BC5 File Offset: 0x00075DC5
        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        // Token: 0x060010AB RID: 4267 RVA: 0x0007CC68 File Offset: 0x0007AE68
        public override void OnGameOver()
        {
            base.OnGameOver();
            bool flag = this.simpleBoss_0 != null && !this.simpleBoss_0.IsLiving;
            bool flag2 = flag;
            if (flag2)
            {
                base.Game.IsWin = true;
            }
            else
            {
                base.Game.IsWin = false;
            }
        }

        // Token: 0x060010AC RID: 4268 RVA: 0x000787E5 File Offset: 0x000769E5
        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }

        // Token: 0x060010AD RID: 4269 RVA: 0x0007CCBC File Offset: 0x0007AEBC
        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] npcIds = new int[]
            {
                this.int_0
            };
            int[] npcIds2 = new int[]
            {
                this.int_0
            };
            base.Game.LoadResources(npcIds);
            base.Game.LoadNpcGameOverResources(npcIds2);
            base.Game.SetMap(12016);
        }

        // Token: 0x060010AE RID: 4270 RVA: 0x0007CD1C File Offset: 0x0007AF1C
        public override void OnStartGame()
        {
            base.OnStartGame();
            this.simpleBoss_0 = base.Game.CreateBoss(this.int_0, 899, 190, -1, 4, "");
            this.simpleBoss_0.SetRelateDemagemRect(this.simpleBoss_0.NpcInfo.X, this.simpleBoss_0.NpcInfo.Y, this.simpleBoss_0.NpcInfo.Width, this.simpleBoss_0.NpcInfo.Height);
        }

        // Token: 0x060010AF RID: 4271 RVA: 0x0007CDA8 File Offset: 0x0007AFA8
        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return this.int_1;
        }

        // Token: 0x040007B9 RID: 1977
        private SimpleBoss simpleBoss_0;

        // Token: 0x040007BA RID: 1978
        private int int_0;

        // Token: 0x040007BB RID: 1979
        private int int_1;
    }
}
