using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class XuongHoiNuocKhoS1 : AMissionControl
    {
        private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

        private SimpleBoss boss = null;

        private int kill = 0;

        private int redTotalCount = 0;

        private int dieRedCount = 0;

        private int redNpcID = 2002001;
        private int redNpcID2 = 2002003;

        private int blueNpcID = 2002002;

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 600)
            {
                return 3;
            }
            else if (score > 520)
            {
                return 2;
            }
            else if (score > 450)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] npcIds = new int[3]
            {
                blueNpcID,
                redNpcID,
                redNpcID2
            };
            // Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
            Game.LoadResources(npcIds);
            Game.LoadNpcGameOverResources(npcIds);
            Game.SetMap(1499);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();

            if (Game.GetLivedLivings().Count == 0)
            {
                Game.PveGameDelay = 0;
            }

            for (int i = 0; i < 4; i++)
            {
                redTotalCount++;

                if (i < 1)
                {
                    SomeNpc.Add(Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 623, -1, 1));

                }
                else if (i < 3)
                {
                    SomeNpc.Add(Game.CreateNpc(redNpcID2, 920 + (i + 1) * 100, 623, -1, 1));
                }
                else
                {
                    SomeNpc.Add(Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 623, -1, 1));
                }
            }

            redTotalCount++;
            boss = Game.CreateBoss(blueNpcID, 1300, 623, -1, 10, "");
            boss.FallFrom(boss.X, boss.Y, "", 0, 0, 1000, null);
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();

            if (Game.GetLivedLivings().Count == 0)
            {
                Game.PveGameDelay = 0;
            }

            if (Game.TurnIndex > 1 && Game.GetLivedLivings().Count < 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (redTotalCount < 20)
                    {
                        redTotalCount++;

                        if (i < 1)
                        {
                            SomeNpc.Add(Game.CreateNpc(redNpcID, 850 + (i + 1) * 100, 623, -1, 1));
                        }
                        else if (i < 3)
                        {
                            SomeNpc.Add(Game.CreateNpc(redNpcID2, 893 + (i + 1) * 100, 623, -1, 1));
                        }
                        else
                        {
                            SomeNpc.Add(Game.CreateNpc(redNpcID, 921 + (i + 1) * 100, 623, -1, 1));
                        }
                    }
                }

                if (redTotalCount < 20 && boss.IsLiving == false)
                {
                    redTotalCount++;
                    boss = Game.CreateBoss(blueNpcID, 1300, 623, -1, 10, "");
                    boss.FallFrom(boss.X, boss.Y, "", 0, 0, 1000, null);
                }
            }
        }
        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {

            base.CanGameOver();
            if (Game.GetLivedLivings().Count == 0 && boss.IsLiving == false && Game.TotalKillCount == Game.MissionInfo.TotalCount)
            {
                Game.IsWin = true;
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            return Game.TotalKillCount;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();

            if (Game.GetLivedLivings().Count == 0 && boss.IsLiving == false && Game.TotalKillCount == Game.MissionInfo.TotalCount)
            {
                Game.IsWin = true;
            }
            else
            {
                Game.IsWin = false;
            }
        }
    }
}
