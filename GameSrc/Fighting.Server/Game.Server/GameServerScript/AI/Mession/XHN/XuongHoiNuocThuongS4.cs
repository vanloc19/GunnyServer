using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.Messions
{
    public class XuongHoiNuocThuongS4 : AMissionControl
    {
        private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

        private SimpleBoss boss = null;
        private SimpleBoss boss2 = null;

        private int kill = 0;

        private int bossid1 = 2001006;
        private int bossid2 = 2001010;

        private int redNpcID = 2001001;
        private int redNpcID3 = 2001003;
        private int redNpcID2 = 2001002;

        private int cotid1 = 2001007;
        private int cotid2 = 2001008;
        private int cotid3 = 2001009;

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
            int[] resources = { redNpcID, redNpcID2, redNpcID3 };
            int[] resources1 = { bossid1, bossid2, cotid1, cotid2, cotid3 };
            // Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
            Game.LoadResources3D(resources);
            Game.LoadResources(resources1);
            Game.LoadNpcGameOverResources(resources);
            Game.LoadNpcGameOverResources(resources1);
            Game.SetMap(1503);
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

            // Game.SendFreeFocus(930, 400, 1, 0, 3000);
            LivingConfig NpcConfig = Game.BaseLivingConfig();
            NpcConfig.HasTurn = false;
            NpcConfig.DuocBan = false;
            Game.CreateNpc(cotid1, 679, 1042, 0, 1, NpcConfig);
            Game.CreateNpc(cotid2, 997, 1042, 0, 1, NpcConfig);
            Game.CreateNpc(cotid3, 1322, 1042, 0, 1, NpcConfig);

            boss = Game.CreateBoss(bossid1, 930, 470, -1, 1, "", BossConfig);
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
        }

        public override bool CanGameOver()
        {

            base.CanGameOver();
            if (boss != null && !boss.IsLiving && boss2 == null)
            {
                LivingConfig BossConfig = Game.BaseLivingConfig();
                BossConfig.IsFly = true;
                boss2 = Game.CreateBoss(bossid2, 930, 470, -1, 1, "", BossConfig);
                var ListNPC = ((PVEGame)Game).FindAllNpc2();
                foreach (var NPC in ListNPC)
                {
                    if (NPC.NpcInfo.Name == "Gà Robot" || NPC.NpcInfo.Name == "Gà Con Robot" || NPC.NpcInfo.Name == "Goblin Robot")
                    {
                        NPC.Die(-1);
                        ((PVEGame)Game).RemoveLiving2(NPC.Id);
                    }

                }
            }
            if (boss2 != null && !boss2.IsLiving)
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

            if (boss2 != null && !boss2.IsLiving)
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
