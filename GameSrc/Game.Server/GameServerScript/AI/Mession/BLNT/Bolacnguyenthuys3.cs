using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.Messions
{
    public class Bolacnguyenthuys3 : AMissionControl
    {
        private int mapId = 1630;
        private int npc1ID = 33099;
        private int npc2ID = 33097;
        private int bossID = 33098;
        private SimpleBoss m_boss;
        private PhysicalObj m_moive;
        private PhysicalObj m_front;
        private List<SimpleNpc> someNpc = new List<SimpleNpc>();
        private List<SimpleNpc> someNpc1 = new List<SimpleNpc>();

        public override int CalculateScoreGrade(int score)
        {
            base.CalculateScoreGrade(score);
            if (score > 900)
                return 3;
            if (score > 825)
                return 2;
            return score > 725 ? 1 : 0;
        }

        public override void OnPrepareNewSession()
        {
            base.OnPrepareNewSession();
            int[] npcIds = new int[3]
            {
        this.npc1ID,
        this.npc2ID,
        this.bossID
            };
            this.Game.LoadResources(npcIds);
            this.Game.LoadNpcGameOverResources(npcIds);
            this.Game.SetMap(this.mapId);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            LivingConfig config = this.Game.BaseLivingConfig();
            config.IsFly = true;
            this.someNpc.Add(this.Game.CreateNpc(this.npc1ID, 694, 322, -1, 1, "", config));
            this.someNpc1.Add(this.Game.CreateNpc(this.npc2ID, 1014, 380, -1, -1, "", config));
            this.CreateBoss();
            this.m_boss.Say(LanguageMgr.GetTranslation("Những kẻ đã đặt chân tới đây thì đừng hòng chạy thoát"), 0, 3000);
        }

        public void CreateBoss()
        {
            LivingConfig config = this.Game.BaseLivingConfig();
            config.IsFly = true;
            this.m_boss = this.Game.CreateBoss(this.bossID, 903, 764, -1, 1, "", config);
            this.m_boss.SetRelateDemagemRect(this.m_boss.NpcInfo.X, this.m_boss.NpcInfo.Y, this.m_boss.NpcInfo.Width, this.m_boss.NpcInfo.Height);
        }

        public override void OnNewTurnStarted() => base.OnNewTurnStarted();

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            if (this.Game.TurnIndex <= 1)
                return;
            if (this.m_moive != null)
            {
                this.Game.RemovePhysicalObj(this.m_moive, true);
                this.m_moive = (PhysicalObj)null;
            }
            if (this.m_front != null)
            {
                this.Game.RemovePhysicalObj(this.m_front, true);
                this.m_front = (PhysicalObj)null;
            }
        }

        public override bool CanGameOver()
        {
            bool flag = true;
            base.CanGameOver();
            foreach (Physics physics in this.someNpc)
            {
                if (physics.IsLiving)
                    flag = false;
            }
            if (this.m_boss != null)
            {
                if (!this.m_boss.IsLiving && !flag)
                {
                    this.CreateBoss();
                    this.m_boss.Say(LanguageMgr.GetTranslation("Ta đã trở lại rồi đây !! "), 0, 1000);
                }
                else if (!this.m_boss.IsLiving & flag)
                    return true;
            }
            return false;
        }

        public override int UpdateUIData()
        {
            if (this.m_boss == null)
                return 0;
            return !this.m_boss.IsLiving ? 1 : base.UpdateUIData();
        }

        public override void OnGameOver()
        {
            base.OnGameOver();
            if (this.m_boss == null)
                return;
            if (!this.m_boss.IsLiving)
                this.Game.IsWin = true;
            else
                this.Game.IsWin = false;
        }
    }
}
