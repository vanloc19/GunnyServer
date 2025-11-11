using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
    public class GAAM1 : AMissionControl
    {
        private List<SimpleNpc> redNpc = new List<SimpleNpc>();

        private List<SimpleNpc> blueNpc = new List<SimpleNpc>();

        private int redCount;

        private int blueCount;

        private int redTotalCount;

        private int blueTotalCount;

        private int dieRedCount;

        private int dieBlueCount;

        private int redNpcID = 13011;

        private int blueNpcID = 13020;

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 930)
            {
                return 3;
            }
            if (score > 850)
            {
                return 2;
            }
            if (score > 775)
            {
                return 1;
            }
            return 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] resources = { redNpcID, blueNpcID };
            base.Game.LoadResources(resources);
            base.Game.SetMap(1072);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            for (int i = 0; i < 4; i++)
            {
                redTotalCount++;
                if (i < 1)
                {
                    redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, -1, 1));
                }
                else if (i < 3)
                {
                    redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, -1, 1));
                }
                else
                {
                    redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, -1, 1));
                }
            }
            blueTotalCount++;
            blueNpc.Add(base.Game.CreateNpc(blueNpcID, 1467, 495, -1, 1));
        }

        public override void OnNewTurnStarted()
        {
            redCount = redTotalCount - dieRedCount;
            blueCount = blueTotalCount - dieBlueCount;
            if (base.Game.GetLivedLivings().Count == 0)
            {
                base.Game.PveGameDelay = 0;
            }
            if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || (blueCount == 3 && redCount == 12))
            {
                return;
            }
            if (redTotalCount < 12 && blueTotalCount < 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    redTotalCount++;
                    if (i < 1)
                    {
                        redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, -1, 1));
                    }
                    else if (i < 3)
                    {
                        redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, -1, 1));
                    }
                    else
                    {
                        redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, -1, 1));
                    }
                }
                blueTotalCount++;
                blueNpc.Add(base.Game.CreateNpc(blueNpcID, 1467, 495, -1, 1));
            }
            else
            {
                if (redCount >= 12)
                {
                    return;
                }
                if (12 - redCount >= 4)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (redTotalCount < 20 && redCount != 12)
                        {
                            redTotalCount++;
                            if (i < 1)
                            {
                                redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, -1, 1));
                            }
                            else if (i < 3)
                            {
                                redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, -1, 1));
                            }
                            else
                            {
                                redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, -1, 1));
                            }
                        }
                    }
                }
                else if (12 - redCount > 0)
                {
                    for (int i = 0; i < 12 - redCount; i++)
                    {
                        if (redTotalCount < 20 && redCount != 12)
                        {
                            redTotalCount++;
                            if (i < 1)
                            {
                                redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, -1, 1));
                            }
                            else if (i < 3)
                            {
                                redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, -1, 1));
                            }
                            else
                            {
                                redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, -1, 1));
                            }
                        }
                    }
                }
                if (blueCount < 3 && blueTotalCount < 5)
                {
                    blueTotalCount++;
                    blueNpc.Add(base.Game.CreateNpc(blueNpcID, 1467, 495, -1, 1));
                }
            }
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {
            bool result = true;
            dieRedCount = 0;
            dieBlueCount = 0;
            foreach (SimpleNpc item in redNpc)
            {
                if (item.IsLiving)
                {
                    result = false;
                }
                else
                {
                    dieRedCount++;
                }
            }
            foreach (SimpleNpc item2 in blueNpc)
            {
                if (item2.IsLiving)
                {
                    result = false;
                }
                else
                {
                    dieBlueCount++;
                }
            }
            if (result && redTotalCount == 20 && blueTotalCount == 5)
            {
                base.Game.IsWin = true;
                return true;
            }
            if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
            {
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return base.Game.TotalKillCount;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (base.Game.GetLivedLivings().Count == 0)
            {
                base.Game.IsWin = true;
            }
            else
            {
                base.Game.IsWin = false;
            }
            //List<LoadingFileInfo> loadingFileInfos = new List<LoadingFileInfo>();
            //loadingFileInfos.Add(new LoadingFileInfo(2, "image/map/3", ""));
            //base.Game.SendLoadResource(loadingFileInfos);
        }
    }
}
