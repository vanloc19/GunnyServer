using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
    public class GAAM2 : AMissionControl
    {
        private SimpleBoss m_boss;

        private PhysicalObj m_moive;

        private PhysicalObj m_front;

        private int IsSay = 0;

        private int bossID = 13030;

        private int npcID = 13090;

        private static string[] KillChat = {
                "Gửi cho bạn trở về nhà!",
                "Một mình, bạn có ảo tưởng có thể đánh bại tôi?"
            };

        private static string[] ShootedChat = {
                " Đau ah! Đau ...",
                "Quốc vương vạn tuế ..."
            };

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
            if (score > 1285)
            {
                return 1;
            }
            return 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
            int[] resources = { bossID, npcID };
            base.Game.LoadResources(resources);
            int[] gameOverResources = { bossID };
            base.Game.LoadNpcGameOverResources(gameOverResources);
            base.Game.SetMap(1073);
        }

        public override void OnStartGame()
        {
            m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
            m_front = base.Game.Createlayer(680, 330, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 1);
            m_boss = base.Game.CreateBoss(bossID, 770, -1500, -1, 1, "");
            m_boss.FallFrom(m_boss.X, m_boss.Y, "", 0, 2, 2000);
            m_boss.SetRelateDemagemRect(34, -35, 11, 18);
            m_boss.AddDelay(10);
            m_boss.Say("Dám xâm phạm địa bàn của ta, chờ chết!", 0, 6000);
            m_boss.PlayMovie("call", 5900, 0);
            m_moive.PlayMovie("in", 9000, 0);
            m_boss.PlayMovie("weakness", 10000, 5000);
            m_front.PlayMovie("in", 9000, 0);
            m_moive.PlayMovie("out", 15000, 0);
            base.Game.BossCardCount = 1;
            base.OnStartGame();
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            if (base.Game.TurnIndex > 1)
            {
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
        }

        public override bool CanGameOver()
        {
            base.CanGameOver();
            if (!m_boss.IsLiving)
            {
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            if (m_boss == null)
            {
                return 0;
            }
            if (!m_boss.IsLiving)
            {
                return 1;
            }
            return base.UpdateUIData();
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (!m_boss.IsLiving)
            {
                base.Game.IsWin = true;
            }
            else
            {
                base.Game.IsWin = false;
            }
        }

        public override void DoOther()
        {
            base.DoOther();
            if (m_boss != null)
            {
                int index = base.Game.Random.Next(0, KillChat.Length);
                if (m_boss != null)
                {
                    m_boss.Say(KillChat[index], 0, 0);
                }
            }
        }

        public override void OnShooted()
        {
            base.OnShooted();
            if (m_boss != null && m_boss.IsLiving && IsSay == 0)
            {
                int index = base.Game.Random.Next(0, ShootedChat.Length);
                m_boss.Say(ShootedChat[index], 0, 1500);
                IsSay = 1;
            }
        }
    }
}
