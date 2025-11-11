using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
    public class TVT12003 : AMissionControl
    {
        private int BossID = 12310;

        private int ChickenNpcID1 = 12309;

        private int ChickenNpcID2 = 12311;

        private int EndChickenID = 12312;

		private int m_bloodBuff = 1000;

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
			if (base.Game.TotalKillCount >= base.Game.MissionInfo.TotalCount)
			{
				base.Game.TotalKillCount = base.Game.MissionInfo.TotalCount;
				base.Game.CanEnd = true;
				return true;
			}
			if (base.Game.Param1 >= base.Game.Param2)
			{
				base.Game.Param1 = base.Game.MissionInfo.Param2;
				return true;
			}
			return false;
			//return (base.Game.CanEnd && (base.Game.TotalKillCount >= base.Game.MissionInfo.TotalCount || base.Game.Param1 >= base.Game.Param2)) ? true : false;
        }
        public override void OnPrepareGameOver()
        {
            base.OnPrepareGameOver();
			if (base.Game.TotalKillCount >= base.Game.MissionInfo.TotalCount)
			{
				Game.CanEndGame = true;
				return;
			}
			if (base.Game.Param1 >= base.Game.Param2 && !base.Game.CanEnd)
			{
				CreateEndChicken();
			}

		}
		public override void OnNewTurnStarted()
        {
			base.OnNewTurnStarted();
        }

        public override void OnBeginNewTurn()
        {
			base.OnBeginNewTurn();
			if (base.Game.CurrentLiving != null && base.Game.CurrentLiving is Player)
			{
				Player player = (Player)base.Game.CurrentLiving;
				player.DeputyWeapon = null;
				player.SetBall(64);
				player.SetSystemState(state: true);
			}
        }

        public override void OnGameOver()
        {
			base.OnGameOver();
			if (base.Game.Param1 >= base.Game.Param2)
			{
				base.Game.IsWin = true;
			}
			else if (base.Game.TotalKillCount >= base.Game.MissionInfo.TotalCount)
			{
				base.Game.IsWin = false;
			}
        }

        private void CreateEndChicken()
        {
			base.Game.SendFreeFocus(190, 1204, 1, 1, 1);
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
            livingConfig.isShowBlood = false;
            livingConfig.IsTurn = false;
            livingConfig.isShowSmallMapPoint = false;
            EndChicken = base.Game.CreateNpc(EndChickenID, 189, 1025, 1, -1, "", livingConfig);
			EndChicken.PlayMovie("born", 0, 3000);
			Boss.PlayMovie("die", 3500, 3000);
			base.Game.SendFreeFocus(Boss.X, Boss.Y, 0, 3000, 100);
			EndChicken.CallFuction(BossDead, 4500);
        }

        private void BossDead()
        {
			Boss.Die();
			base.Game.CanEnd = true;
			Game.CanEndGame = true;
        }

        public override void OnPrepareNewSession()
        {
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(2, "image/game/effect/9/duqidd.swf", "asset.game.nine.duqidd");
			base.Game.LoadResources(new int[4]
			{
				BossID,
				ChickenNpcID1,
				ChickenNpcID2,
				EndChickenID
			});
			base.Game.LoadNpcGameOverResources(new int[4]
			{
				BossID,
				ChickenNpcID1,
				ChickenNpcID2,
				EndChickenID
			});
			base.Game.SetMap(1209);
        }
        public override void OnPrepareStartGame()
        {
            base.OnPrepareStartGame();
			CreateChicken();
        }

        public override void OnStartGame()
        {
			base.OnStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.CanTakeDamage = false;
			livingConfig.IsFly = true;
			livingConfig.CanCountKill = false;
			Boss = base.Game.CreateBoss(BossID, 951, 540, -1, 0, "born", livingConfig);
			base.Game.SendFreeFocus(951, 540, 1, 1, 1);
			Game.BloodBuff = m_bloodBuff;
			//Boss.CallFuction(CreateChicken, 2000);
        }

        private void CreateChicken()
        {
			for (int i = 0; i < 15; i++)
			{
				int x = base.Game.Random.Next(554, 1353);
				LivingConfig livingConfig = base.Game.BaseLivingConfig();
				livingConfig.IsHelper = true;
				livingConfig.CanHeal = false;
				livingConfig.CanCountKill = false;
				ChickenList.Add(base.Game.CreateNpc((base.Game.Random.Next(50) < 25) ? ChickenNpcID1 : ChickenNpcID2, x + 20, 1023, 1, (base.Game.Random.Next(50) >= 25) ? 1 : (-1), livingConfig));
			}
        }

        public override int UpdateUIData()
        {
			base.UpdateUIData();
			return base.Game.TotalKillCount;
        }
    }
}
