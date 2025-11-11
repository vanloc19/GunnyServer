using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
    public class TVS12003 : AMissionControl
    {

        private int BossID = 12010;

        private int ChickenNpcID1 = 12009;

        private int ChickenNpcID2 = 12011;

        private int EndChickenID = 12009;

        private SimpleBoss Boss;

        private SimpleNpc EndChicken;

        private List<SimpleNpc> ChickenList = new List<SimpleNpc>();

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1540)
            {
                return 3;
            }
            if (score > 1410)
            {
                return 2;
            }
            if (score <= 1285)
            {
                return 0;
            }
            return 1;
        }

        public override bool CanGameOver()
        {
            base.CanGameOver();
            if (Game.TotalKillCount > Game.MissionInfo.TotalCount)
            {
                Game.TotalKillCount = Game.MissionInfo.TotalCount;
                Game.CanEnd = true;
            }
            if (Game.Param4 > Game.Param2)
                Game.Param4 = Game.MissionInfo.Param2;
            if (Game.Param4 >= Game.Param2 && !Game.CanEnd)
                CreateEndChicken();
            return (Game.CanEnd && (Game.TotalKillCount >= Game.MissionInfo.TotalCount || Game.Param4 >= Game.Param2)) ? true : false;

        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

            if (Game.CurrentLiving != null && (Game.CurrentLiving is Player))
            {
                Player player = ((Player)Game.CurrentLiving);
                player.DeputyWeapon = null;
                player.SetBall(64);
                player.SetSystemState(true);
            }
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (Game.Param4 >= Game.Param2)
            {
                Game.IsWin = true;
            }
            else if (Game.TotalKillCount >= Game.MissionInfo.TotalCount)
                Game.IsWin = false;
        }

        private void CreateEndChicken()
        {
            Game.SendFreeFocus(190, 1204, 1, 1, 1);
            LivingConfig livingConfig = Game.BaseLivingConfig();
            livingConfig.isShowBlood = false;
            livingConfig.isShowSmallMapPoint = false;
            EndChicken = Game.CreateNpc(EndChickenID, 189, 1025, 1, 1, "born", livingConfig);
            Boss.PlayMovie("die", 1, 3000);
            Game.SendFreeFocus(Boss.X, Boss.Y, 1, 3000, 100);
            EndChicken.CallFuction(BossDead, 4000);
        }

        private void BossDead()
        {
            Boss.Die();
            Game.CanEnd = true;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            base.Game.AddLoadingFile(2, "image/game/effect/9/duqidd.swf", "asset.game.nine.duqidd");
            base.Game.LoadResources(new int[] { this.BossID, ChickenNpcID1, ChickenNpcID2, EndChickenID });
            base.Game.LoadNpcGameOverResources(new int[] { this.BossID, ChickenNpcID1, ChickenNpcID2, EndChickenID });
            base.Game.SetMap(1209);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            LivingConfig livingConfig = base.Game.BaseLivingConfig();
            livingConfig.CanTakeDamage = false;
            livingConfig.IsFly = true;
            livingConfig.CanCountKill = false;
            this.Boss = base.Game.CreateBoss(this.BossID, 951, 540, -1, 0, "born", livingConfig);
            Game.SendFreeFocus(951, 540, 1, 1, 1);
            Boss.CallFuction(CreateChicken, 2000);
        }

        private void CreateChicken()
        {
            for (int i = 0; i < 15; i++)
            {
                int x = Game.Random.Next(554, 1353);
                LivingConfig livingConfig = base.Game.BaseLivingConfig();
                livingConfig.IsHelper = true;
                livingConfig.CanHeal = false;
                ChickenList.Add(Game.CreateNpc(Game.Random.Next(50) < 25 ? ChickenNpcID1 : ChickenNpcID2, x, 1023, 1, Game.Random.Next(50) < 25 ? -1 : 1, livingConfig));
            }

        }

        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return Game.TotalKillCount;
        }
    }
}