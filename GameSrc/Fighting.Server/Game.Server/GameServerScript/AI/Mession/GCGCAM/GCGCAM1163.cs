using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using Bussiness;

namespace GameServerScript.AI.Messions
{
    public class GCGCAM1163 : AMissionControl
    {
        private SimpleBoss boss = null;

        private int npcID = 34319;

        private int npcID2 = 34316;

        private int bossID = 34320;

        private int kill = 0;
        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 1750)
            {
                return 3;
            }
            else if (score > 1675)
            {
                return 2;
            }
            else if (score > 1600)
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
            int[] resources = { bossID, npcID, npcID2 };
            int[] gameOverResource = { bossID };
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(gameOverResource);
            Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
            Game.AddLoadingFile(2, "image/game/effect/7/jinquhd.swf", "asset.game.seven.jinquhd");
            Game.SetMap(1163);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            Game.IsBossWar = "7103";
            boss = Game.CreateBoss(bossID, 275, 950, 1, 1, "");
            boss.FallFrom(338, 950, "", 0, 0, 1000);
            boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);

        }

        public override void OnNewTurnStarted()
        {
            //base.OnBeginNewTurn();


        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {

            if (boss != null && boss.IsLiving == false)
            {
                kill++;
                return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            base.UpdateUIData();
            return kill;
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (boss != null && boss.IsLiving == false)
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