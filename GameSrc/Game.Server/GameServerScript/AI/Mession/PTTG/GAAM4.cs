using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
    public class GAAM4 : AMissionControl
    {
        private PhysicalObj m_kingMoive;

        private PhysicalObj m_kingFront;

        private SimpleBoss m_king;

        private SimpleBoss m_secondKing;

        private PhysicalObj[] m_leftWall;

        private PhysicalObj[] m_rightWall;

        private int m_kill;

        private int m_state = 13050;

        private int turn;

        private int IsSay;

        private int firstBossID = 13050;

        private int secondBossID = 13060;

        private int npcID = 13100;

        private int direction;

        private static string[] KillChat = {
                "Tôi cuối cùng cũng thoát <br/> khỏi khống chế của <br/> Matthias, thật nhức đầu! "
        };

        private static string[] ShootedChat = {
                "Ai ya, các bạn <br/> sao lại đánh tôi? <br/> Tôi làm gì ?... ",
                "Ui~đau quá, sao phải đánh nhau, mình phải chiến đấu ?"
        };

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1150)
            {
                return 3;
            }
            if (score > 925)
            {
                return 2;
            }
            if (score > 700)
            {
                return 1;
            }
            return 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            base.Game.AddLoadingFile(1, "bombs/61.swf", "tank.resource.bombs.Bomb61");
            base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
            base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
            int[] resources = { firstBossID, secondBossID, npcID };
            base.Game.LoadResources(resources);
            int[] gameOverResources = { firstBossID };
            base.Game.LoadNpcGameOverResources(gameOverResources);
            base.Game.SetMap(1076);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
            m_kingFront = base.Game.Createlayer(720, 495, "font", "game.asset.living.boguoKingAsset", "out", 1, 1);
            m_king = base.Game.CreateBoss(m_state, 888, 590, -1, 1, "");
            m_king.FallFrom(m_king.X, 0, "", 0, 2, 2000);
            m_king.SetRelateDemagemRect(-21, -87, 72, 59);
            m_king.AddDelay(10);
            m_king.Say(LanguageMgr.GetTranslation("Tất cả các bạn dân thường thấp hèn, dám tự tin trong cung điện của tôi!"), 0, 3000);
            m_kingMoive.PlayMovie("in", 9000, 0);
            m_kingFront.PlayMovie("in", 9000, 0);
            m_kingMoive.PlayMovie("out", 13000, 0);
            m_kingFront.PlayMovie("out", 13400, 0);
            turn = base.Game.TurnIndex;
            base.Game.BossCardCount = 1;
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            if (base.Game.TurnIndex > turn + 1)
            {
                if (m_kingMoive != null)
                {
                    base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
                    m_kingMoive = null;
                }
                if (m_kingFront != null)
                {
                    base.Game.RemovePhysicalObj(m_kingFront, sendToClient: true);
                    m_kingFront = null;
                }
            }
        }

        public override bool CanGameOver()
        {
            base.CanGameOver();
            if (!m_king.IsLiving && m_state == firstBossID)
            {
                m_state++;
            }
            if (m_state == secondBossID && m_secondKing == null)
            {
                m_secondKing = base.Game.CreateBoss(m_state, m_king.X, m_king.Y, m_king.Direction, 1, "");
                base.Game.RemoveLiving(m_king.Id);
                if (m_secondKing.Direction == 1)
                {
                    m_secondKing.SetRect(-21, -87, 72, 59);
                }
                m_secondKing.SetRelateDemagemRect(-21, -87, 72, 59);
                m_secondKing.Say(LanguageMgr.GetTranslation("Bạn tức giận tôi, tôi không tha thứ cho bạn!"), 0, 3000);
                List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
                Player player = base.Game.FindRandomPlayer();
                int minDelay = 0;
                if (player != null)
                {
                    minDelay = player.Delay;
                }
                foreach (Player item in allFightPlayers)
                {
                    if (item.Delay < minDelay)
                    {
                        minDelay = item.Delay;
                    }
                }
                m_secondKing.AddDelay(minDelay - 2000);
                turn = base.Game.TurnIndex;
            }
            if (m_secondKing != null && !m_secondKing.IsLiving)
            {
                direction = m_secondKing.Direction;
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
            if (m_state == secondBossID && !m_secondKing.IsLiving)
            {
                base.Game.IsWin = true;
            }
            else
            {
                base.Game.IsWin = false;
            }
            List<LoadingFileInfo> list = new List<LoadingFileInfo>();
            list.Add(new LoadingFileInfo(2, "image/map/show7.jpg", ""));
            base.Game.SendLoadResource(list);
            m_leftWall = base.Game.FindPhysicalObjByName("wallLeft");
            m_rightWall = base.Game.FindPhysicalObjByName("wallRight");
            for (int i = 0; i < m_leftWall.Length; i++)
            {
                base.Game.RemovePhysicalObj(m_leftWall[i], sendToClient: true);
            }
            for (int i = 0; i < m_rightWall.Length; i++)
            {
                base.Game.RemovePhysicalObj(m_rightWall[i], sendToClient: true);
            }
        }

        public override void DoOther()
        {
            base.DoOther();
            if (m_king != null)
            {
                if (m_king.IsLiving)
                {
                    int index = base.Game.Random.Next(0, KillChat.Length);
                    m_king.Say(KillChat[index], 0, 0);
                }
                else
                {
                    int index = base.Game.Random.Next(0, KillChat.Length);
                    m_king.Say(KillChat[index], 0, 0);
                }
            }
        }

        public override void OnShooted()
        {
            base.OnShooted();
            if (IsSay == 0)
            {
                if (m_king.IsLiving)
                {
                    int index = base.Game.Random.Next(0, ShootedChat.Length);
                    m_king.Say(ShootedChat[index], 0, 1500);
                }
                else
                {
                    int index = base.Game.Random.Next(0, ShootedChat.Length);
                    m_secondKing.Say(ShootedChat[index], 0, 1500);
                }
                IsSay = 1;
            }
        }
    }
}
