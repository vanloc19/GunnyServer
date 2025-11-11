using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
    public class TVT12002 : AMissionControl
    {
        private int BossID = 12308;

        private int HelperNPCID = 12306;

        private int cannonball = 12307;

        private SimpleBoss CrockBoss;

        private SimpleBoss HelperNPC;

        private PhysicalObj physicalObj_0;

        private PhysicalObj physicalObj_1;

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
			if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
			{
				return true;
			}
			return !CrockBoss.IsLiving;
        }

        public override void OnBeginNewTurn()
        {
			base.OnBeginNewTurn();
			if (base.Game.TurnIndex > 1)
			{
				if (physicalObj_0 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_0, sendToClient: true);
					physicalObj_0 = null;
				}
				if (physicalObj_1 != null)
				{
					base.Game.RemovePhysicalObj(physicalObj_1, sendToClient: true);
					physicalObj_1 = null;
				}
			}
        }

        public override void OnGameOver()
        {
			base.OnGameOver();
			bool flag = true;
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				if (allFightPlayer.IsLiving)
				{
					flag = false;
				}
			}
			if (!(CrockBoss.IsLiving || flag))
			{
				base.Game.IsWin = true;
			}
        }

        public override void OnNewTurnStarted()
        {
			base.OnNewTurnStarted();
        }

        public override void OnPrepareNewSession()
        {
			base.OnPrepareNewSession();
			base.Game.AddLoadingFile(1, "bombs/88.swf", "tank.resource.bombs.Bomb88");
			base.Game.AddLoadingFile(2, "image/game/effect/9/biaoji.swf", "asset.game.nine.biaojiA");
			base.Game.AddLoadingFile(2, "image/game/effect/9/dapao.swf", "asset.game.nine.dapao");
			base.Game.AddLoadingFile(2, "image/game/effect/5/xiaopao.swf", "asset.game.4.xiaopao");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
			base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ducaizheAsset");
			int[] npcIds = new int[3]
			{
				BossID,
				HelperNPCID,
				cannonball
			};
			base.Game.LoadResources(npcIds);
			base.Game.LoadNpcGameOverResources(npcIds);
			base.Game.SetMap(1208);
        }

        public override void OnPrepareStartGame()
        {
			base.OnPrepareStartGame();
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsHelper = true;
			livingConfig.IsTurn = false;
			HelperNPC = base.Game.CreateBoss(HelperNPCID, 150, 998, 1, 0, "", livingConfig);
			HelperNPC.Properties2 = "phaodoan";
			base.Game.SendHideBlood(HelperNPC, 0);
			HelperNPC.OnSmallMap(state: false);
        }

        public override void OnStartGame()
        {
			base.OnStartGame();
			physicalObj_0 = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
			physicalObj_1 = base.Game.Createlayer(1300, 730, "font", "game.asset.living.ducaizheAsset", "out", 1, 0);
			CreateCrockBoss();
        }

        public override void OnDied()
        {
			base.OnDied();
        }

        private void CreateCrockBoss()
        {
			base.Game.SendFreeFocus(1728, 850, 1, 1, 1);
			CrockBoss = base.Game.CreateBoss(BossID, 1728, 990, -1, 1, "");
			CrockBoss.Properties3 = 0;
			CrockBoss.SetRelateDemagemRect(CrockBoss.NpcInfo.X, CrockBoss.NpcInfo.Y, CrockBoss.NpcInfo.Width, CrockBoss.NpcInfo.Height);
			CrockBoss.Say("Tuyệt đối không ai được đến gần bảo bối của ta nửa bước!", 0, 3000, 2000);
			physicalObj_0.PlayMovie("in", 6000, 0);
			physicalObj_1.PlayMovie("in", 6000, 0);
			physicalObj_0.PlayMovie("out", 11000, 0);
        }

        public override int UpdateUIData()
        {
			if (CrockBoss == null)
			{
				return 0;
			}
			if (!CrockBoss.IsLiving)
			{
				return 1;
			}
			return base.UpdateUIData();
        }
    }
}
