using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class XuongHoiNuocThuongS3 : AMissionControl
    {
        private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

        private SimpleBoss boss = null;

        private int bossId = 2001005;

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
            int[] resources = { bossId };
            // Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(resources);
            Game.SetMap(1502);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();

            if (Game.GetLivedLivings().Count == 0)
            {
                Game.PveGameDelay = 0;
            }
            boss = Game.CreateBoss(bossId, 1445, 510, -1, 1, "");
            boss.Say("Chịu phạt đi", 0, 0);
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();

            if (Game.GetLivedLivings().Count == 0)
            {
                Game.PveGameDelay = 0;
            }


        }
        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            var players = Game.GetAllLivingPlayers();
            foreach (var p in players)
            {
                if (p.X > 711)
                {
                    p.Die();
                    p.IsLiving = false;
                }
            }
        }

        public override bool CanGameOver()
        {

            base.CanGameOver();
            if (boss != null && !boss.IsLiving)
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

            if (boss != null && !boss.IsLiving)
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
