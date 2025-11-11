using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class Bolacnguyenthuys2 : AMissionControl
    {
        private SimpleBoss king = null;

        private SimpleBoss boss = null;

        private int bossMainId = 33160;

        private int bossTwoId = 33170;

        private int npcID = 33060;

        private int fearId = 33100;

        private int lockId = 33110;

        private int m_killCount = 0;

        private PhysicalObj m_moive;

        private PhysicalObj m_front = null;


        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1330)
            {
                return 3;
            }
            else if (score > 1150)
            {
                return 2;
            }
            else if (score > 970)
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
            int[] resources = { bossMainId, bossTwoId, npcID, fearId, lockId };
            int[] gameOverResource = { bossMainId, bossTwoId };
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(gameOverResource);
            base.Game.AddLoadingFile(1, "bombs/55.swf", "tank.resource.bombs.Bomb55");
            base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
            base.Game.AddLoadingFile(1, "bombs/53.swf", "tank.resource.bombs.Bomb53");
            base.Game.AddLoadingFile(2, "image/map/1126/object/1126object.swf", "game.assetmap.Flame");
            base.Game.AddLoadingFile(2, "image/map/1076/objects/1076mapasset.swf", "com.mapobject.asset.wordtip75");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
            base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanLeaderAsset");
            Game.SetMap(1496);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();

            Game.PveGameDelay = 0;

            m_moive = Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
            m_front = Game.Createlayer(700, 355, "font", "game.asset.living.ClanLeaderAsset", "out", 1, 1);

            king = Game.CreateBoss(bossMainId, 109, 1430, 1, 1, "");
            //king.FallFrom(king.X, king.Y, "stand", 0, 0, 1000, null);
            king.SetRelateDemagemRect(king.NpcInfo.X, king.NpcInfo.Y, king.NpcInfo.Width, king.NpcInfo.Height);

            boss = Game.CreateBoss(bossTwoId, 2400, 1425, -1, 1, "");
            //boss.FallFrom(boss.X, boss.Y, "stand", 0, 0, 1000, null);
            boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);

            // hiệu ứng nói bla bla
            ((PVEGame)Game).SendObjectFocus(king, 1, 2000, 0);
            king.PlayMovie("call", 3000, 0);
            king.Say("Chạy à? Định chạy đi đâu?", 0, 3300);

            ((PVEGame)Game).SendObjectFocus(boss, 1, 6200, 0);
            boss.PlayMovie("castA", 7000, 0);
            boss.Say("Đại ca. Đây là vật tế cuối cùng rồi đấy!", 0, 7300);

            king.Delay += 1;
            ((PVEGame)Game).SendFreeFocus(827, 534, 1, 10000, 0);
            m_moive.PlayMovie("in", 10000, 0);
            m_front.PlayMovie("in", 11000, 0);
            m_moive.PlayMovie("out", 14000, 0);
            m_front.PlayMovie("out", 15000, 0);
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

            if (m_moive != null)
            {
                Game.RemovePhysicalObj(m_moive, true);
                m_moive = null;
            }
            if (m_front != null)
            {
                Game.RemovePhysicalObj(m_front, true);
                m_front = null;
            }
        }

        public override bool CanGameOver()
        {
            base.CanGameOver();
            List<Living> bossLivings = Game.FindAllTurnBossLiving();

            if (bossLivings.Count <= 0)
            {
                Game.TotalKillCount = 2;
                return true;
            }

            if (Game.TurnIndex > 200)
            {
                return true;
            }

            return false;
        }


        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return Game.TotalKillCount;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();

            // get all living boss
            List<Living> bossLivings = Game.FindAllTurnBossLiving();

            if (bossLivings.Count <= 0)
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