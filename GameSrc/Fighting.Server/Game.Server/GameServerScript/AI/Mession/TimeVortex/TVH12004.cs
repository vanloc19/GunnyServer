using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
    public class TVH12004 : AMissionControl
    {
        private int ChickenFriendID = 12213;

        private int BossID1 = 12214;

        private int BossID2 = 12215;

        private int BossID3 = 12216;

        private int NpcID1 = 12217;

        private int NpcID2 = 12218;

        private int NpcID3 = 12220;

        private int NpcID4 = 12219;

        private SimpleNpc chickenFriend;

        private SimpleBoss Boss1;

        private SimpleBoss Boss2;

        private SimpleBoss Boss3;

        private PhysicalObj m_KingMove;

        private PhysicalObj m_Boss_Present;

        public override int CalculateScoreGrade(int score)
        {
			base.CalculateScoreGrade(score);
			if (score > 900)
			{
				return 3;
			}
			if (score > 825)
			{
				return 2;
			}
			if (score <= 725)
			{
				return 0;
			}
			return 1;
        }

        public override bool CanGameOver()
        {
			base.CanGameOver();
			if (base.Game.TurnIndex > base.Game.TotalTurn)
			{
				return true;
			}
			if (base.Game.TotalKillCount >= base.Game.TotalMissionCount - 3)
			{
				return true;
			}
			return false;
        }

        public override void OnBeginNewTurn()
        {
			base.OnBeginNewTurn();
        }

        public override void OnDied()
        {
			base.OnDied();
			if (Boss1 != null && !Boss1.IsLiving)
			{
				Boss1.CallFuction(CreateSecondBoss, 8000);
			}
			if (Boss2 != null && !Boss2.IsLiving)
			{
				Boss2.CallFuction(CreateFinalBoss, 8000);
			}
			if (Boss3 != null && !Boss3.IsLiving)
			{
				base.Game.WaitTime(8000);
			}
        }

        public override void OnGameOver()
        {
			base.OnGameOver();
			if (base.Game.TotalKillCount >= base.Game.TotalMissionCount - 3)
			{
				base.Game.IsWin = true;
			}
			else
			{
				base.Game.IsWin = false;
			}
        }

        public override void OnNewTurnStarted()
        {
			base.OnNewTurnStarted();
        }

        public override void OnPrepareNewSession()
        {
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.fengkuangAsset");
			base.Game.AddLoadingFile(2, "image/game/effect/9/daodan.swf", "asset.game.nine.daodan");
			base.Game.AddLoadingFile(2, "image/game/effect/9/diancipao.swf", "asset.game.nine.diancipao");
			base.Game.AddLoadingFile(2, "image/game/effect/9/fengyin.swf", "asset.game.nine.fengyin");
			base.Game.AddLoadingFile(2, "image/game/effect/9/siwang.swf", "asset.game.nine.siwang");
			base.Game.AddLoadingFile(2, "image/game/effect/9/shexian.swf", "asset.game.nine.shexian");
			base.Game.AddLoadingFile(2, "image/game/effect/9/biaoji.swf", "asset.game.nine.biaoji");
			base.Game.AddLoadingFile(2, "image/game/effect/9/heidong.swf", "asset.game.nine.heidong");
			int[] npcIds = new int[8]
			{
				BossID1,
				BossID2,
				BossID3,
				NpcID1,
				NpcID2,
				NpcID3,
				ChickenFriendID,
				NpcID4
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1210);
        }

        public override void OnPrepareStartGame()
        {
			base.OnPrepareStartGame();
			CreateChickenFriend();
        }

        public override void OnStartGame()
        {
			base.OnStartGame();
			CreateFirstBoss();
			base.Game.SendFreeFocus(950, 300, 1, 1, 1);
        }

        private void CreateChickenFriend()
        {
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			livingConfig.CanTakeDamage = false;
			livingConfig.IsHelper = true;
			livingConfig.IsTurn = false;
			livingConfig.CanCollied = false;
			chickenFriend = base.Game.CreateNpc(ChickenFriendID, 219, 750, 1, 1, "", livingConfig);
        }

        private void CreateSecondBoss()
        {
			base.Game.SendFreeFocus(987, 342, 1, 1, 1);
			Boss1 = null;
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			livingConfig.CanCountKill = false;
			livingConfig.isBotom = 0;
			Boss2 = base.Game.CreateBoss(BossID2, 987, 342, -1, 1, "born", livingConfig);
        }

        private void CreateFinalBoss()
        {
			m_KingMove = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			m_Boss_Present = base.Game.Createlayer(775, 450, "font", "game.asset.living.fengkuangAsset", "out", 1, 0, CanPenetrate: true);
			base.Game.SendFreeFocus(987, 342, 1, 1, 1);
			Boss2 = null;
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			livingConfig.CanCountKill = true;
			livingConfig.isBotom = 0;
			Boss3 = base.Game.CreateBoss(BossID3, 987, 342, -1, 1, "born", livingConfig);
			Boss3.SetRelateDemagemRect(Boss3.NpcInfo.X, Boss3.NpcInfo.Y, Boss3.NpcInfo.Width, Boss3.NpcInfo.Height);
			m_KingMove.PlayMovie("in", 3000, 0);
			m_Boss_Present.PlayMovie("in", 3000, 0);
			m_KingMove.PlayMovie("out", 10000, 0);
        }

        private void CreateFirstBoss()
        {
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			livingConfig.CanCountKill = false;
			livingConfig.isBotom = 0;
			Boss1 = base.Game.CreateBoss(BossID1, 950, 400, -1, 1, "born", livingConfig);
			Boss1.SetRelateDemagemRect(Boss1.NpcInfo.X, Boss1.NpcInfo.Y, Boss1.NpcInfo.Width, Boss1.NpcInfo.Height);
			base.Game.WaitTime(7000);
        }

        public override int UpdateUIData()
        {
			base.UpdateUIData();
			return base.Game.TotalKillCount;
        }
    }
}
