using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
    public class GAAM5 : AMissionControl
    {
        private SimpleBoss m_boss;

        private int m_kill;

        private int bossID = 13080;

        private int npcID = 13111;

        private PhysicalObj m_moive;

        private PhysicalObj m_front;

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1330)
            {
                return 3;
            }
            if (score > 1150)
            {
                return 2;
            }
            if (score > 970)
            {
                return 1;
            }
            return 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] resources = { npcID, bossID };
            int[] gameOverResources = { bossID };
            base.Game.LoadResources(resources);
            base.Game.LoadNpcGameOverResources(gameOverResources);
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ZhenBombKingAsset");
            base.Game.SetMap(1084);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            m_boss = base.Game.CreateBoss(bossID, 888, 590, -1, 1, "");
            m_moive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
            m_front = base.Game.Createlayer(710, 380, "font", "game.asset.living.ZhenBombKingAsset", "out", 1, 1);
            m_boss.FallFrom(888, 590, "fall", 0, 2, 1000);
            m_boss.SetRelateDemagemRect(-41, -187, 83, 140);
            m_moive.PlayMovie("in", 1000, 0);
            m_front.PlayMovie("in", 2000, 2000);
            m_boss.AddDelay(16);
            base.Game.BossCardCount = 1;
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
            if (m_boss.State == 0)
            {
                m_boss.SetRelateDemagemRect(-41, -187, 83, 140);
            }
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            if (m_moive != null)
            {
                base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
                m_moive = null;
            }
            if (m_front != null)
            {
                base.Game.RemovePhysicalObj(m_front, sendToClient: true);
                m_front = null;
            }
        }

        public override bool CanGameOver()
        {
            if (!m_boss.IsLiving)
            {
                m_kill++;
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return m_kill;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            bool result = true;
            foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
            {
                if (allFightPlayer.IsLiving)
                {
                    result = false;
                }
            }
            if (!m_boss.IsLiving && !result)
            {
                base.Game.IsWin = true;
            }
            else
            {
                base.Game.IsWin = false;
            }
        }
    }
}
