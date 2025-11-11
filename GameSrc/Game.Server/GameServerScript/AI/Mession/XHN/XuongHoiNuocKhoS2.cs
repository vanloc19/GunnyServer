using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class XuongHoiNuocKhoS2 : AMissionControl
    {
        private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

        private SimpleBoss boss = null;
        private SimpleBoss Phymayban = null;

        private int kill = 0;

        private int redTotalCount = 0;

        private int dieRedCount = 0;

        private int redNpcID = 2002001;
        private int redNpcID2 = 2002003;

        private int robotsuachua = 2002004;

        private int mayban = 2002222;

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
            int[] resources = { redNpcID, redNpcID2 };
            int[] resources1 = { robotsuachua, mayban };
            // Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
            Game.LoadResources3D(resources);
            Game.LoadResources(resources1);
            Game.LoadNpcGameOverResources(resources);
            Game.LoadNpcGameOverResources(resources1);
            Game.SetMap(1500);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();

            if (Game.GetLivedLivings().Count == 0)
            {
                Game.PveGameDelay = 0;
            }
            LivingConfig BossConfig = Game.BaseLivingConfig();
            BossConfig.IsFly = true;
            
            LivingConfig ConfigMayBan = Game.BaseLivingConfig();
            ConfigMayBan.isShowBlood = false;
            ConfigMayBan.isShowSmallMapPoint = false;
            ConfigMayBan.IsFly = true;
            Phymayban = Game.CreateBoss(mayban, 1445, 463, -1, 10, "", ConfigMayBan);
            Phymayban.XuyenThau = true;
            boss = Game.CreateBoss(robotsuachua, 1445, 510, -1, 1, "", BossConfig);
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
                if (p.X >981)
                {
                    p.Die();
                    p.IsLiving = false;
                }
            }
            int Blood = Convert.ToInt32(this.boss.MaxBlood * 0.75);
            if(this.boss.Blood<Blood)
            {
                base.Game.MauDuoiMucChoPhep = true;
            }
        }

        public override bool CanGameOver()
        {

            base.CanGameOver();
            if (boss!=null && !boss.IsLiving)
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
