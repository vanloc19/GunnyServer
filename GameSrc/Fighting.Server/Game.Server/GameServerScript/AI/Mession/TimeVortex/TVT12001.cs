using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using SqlDataProvider.Data;

namespace GameServerScript.AI.Messions
{
    public class TVT12001 : AMissionControl
    {
        private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

        private SimpleBoss npc = null;

        private SimpleBoss m_boss = null;

        private int CountBigVolverine = 2;

        private int CountVolverine = 3;
        private int BossID = 12003;

        private int BigVolverineNPC = 12302;

        private int VolverineNPC = 12301;

        private int ChickenHelp = 12304;
        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 900)
            {
                return 3;
            }
            else if (score > 825)
            {
                return 2;
            }
            else if (score > 725)
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
            int[] resources = { BossID, ChickenHelp, VolverineNPC, BigVolverineNPC };
            Game.LoadResources(resources);

            Game.LoadNpcGameOverResources(resources);
            Game.SetMap(1207);
        }
        public override void OnPrepareStartGame()
        {
            base.OnPrepareStartGame();
            LivingConfig binhdoan = Game.BaseLivingConfig();
            binhdoan.IsHelper = true;
            binhdoan.IsTurn = false;
            binhdoan.CanTakeDamage = false;

            npc = Game.CreateBoss(ChickenHelp, 904, 700, -1, 0, "", binhdoan);
            npc.PlayMovie("standA", 0, 0);
            npc.FallFrom(npc.X, npc.Y, "", 0, 0, 1230, null);

        }
        public override void OnStartGame()
        {
            base.OnStartGame();
            CreateNPC();
        }
        private int CanCreateNPC()
        {
            int num = 0;
            foreach (SimpleNpc npc in SomeNpc)
            {
                if (!npc.IsLiving)
                {
                    continue;
                }
                num++;
            }
            return num;
        }
        private void CreateNPC()
        {
            for (int i = 0; i < this.CountBigVolverine; i++)
            {
                SomeNpc.Add(base.Game.CreateNpc(this.BigVolverineNPC, 1735, 766, 1, -1));
            }
            for (int j = 0; j < this.CountVolverine; j++)
            {
                SomeNpc.Add(base.Game.CreateNpc(this.VolverineNPC, 1735, 766, 1, -1));
            }
        }
        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
            if (this.CanCreateNPC() <= 0 && Game.TotalKillCount < Game.TotalCount)
            {
                base.Game.PveGameDelay = 0;
                this.CreateNPC();
            }
            foreach (SimpleNpc npc in SomeNpc)
            {
                npc.NpcInfo.speed = 4;
            }
            if (Game.TurnIndex == 5)
            {
                LivingConfig config = Game.BaseLivingConfig();
                config.CanFrost = true;
                m_boss = Game.CreateBoss(BossID, 100, 700, 1, 2, "", config);
                m_boss.FallFrom(100, 700, "", 0, 0, 1000, null);
                m_boss.SetRelateDemagemRect(-41, -187, 83, 140);
                m_boss.DoAction = 3;//bi bang
            }
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

        }

        public override bool CanGameOver()
        {
            base.CanGameOver();

            if (Game.TurnIndex > Game.TotalTurn)
            {
                return true;
            }

            if (CanCreateNPC() <= 0 && Game.TotalKillCount >= Game.TotalCount)
                return true;
            if (Game.GetAllLivingPlayers().Count <= 0)
                return true;

            return false;
        }
        private void Ending()
        {
            Game.CanEndGame = true;
        }
        public override void OnPrepareGameOver()
        {
            base.OnPrepareGameOver();
            if (CanCreateNPC() <= 0 && Game.TotalKillCount >= Game.TotalCount)
            {
                Game.SendObjectFocus(m_boss, 1, 1, 1);
                m_boss.PlayMovie("die", 500, 500);
                m_boss.Say("Không thể nào...", 0, 500);
                m_boss.CallFuction(Ending, 2000);
                return;
            }
            if (Game.TurnIndex > Game.TotalTurn || Game.GetAllLivingPlayers().Count <= 0)
            {
                Game.CanEndGame = true;
            }
        }

        public override int UpdateUIData()
        {
            return Game.TotalKillCount;
        }

        public override void OnGameOver()
        {
            if (CanCreateNPC() <= 0 && Game.TotalKillCount >= Game.TotalCount)
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
