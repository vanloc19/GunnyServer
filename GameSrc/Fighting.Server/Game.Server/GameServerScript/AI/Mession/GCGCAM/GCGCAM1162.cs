using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using Bussiness;
namespace GameServerScript.AI.Messions
{
    public class GCGCAM1162 : AMissionControl
    {
        private SimpleBoss boss = null;

        private SimpleBoss m_king = null;

        private SimpleBoss king = null;

        private int bossID = 34317;

        private int CountBossTurn = 0;

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
            int[] resources = { bossID };
            int[] gameOverResource = { bossID };
            Game.LoadResources(resources);
            Game.LoadNpcGameOverResources(gameOverResource);
            Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
            Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
            Game.AddLoadingFile(2, "image/game/effect/7/jinquhd.swf", "asset.game.seven.jinquhd");
            Game.SetMap(1162);
        }

        public override void OnStartGame()
        {
            base.OnStartGame();
            LivingConfig config = Game.BaseLivingConfig();
            config.IsTurn = true;
            config.HaveShield = false;
            LivingConfig config1 = Game.BaseLivingConfig();
            config1.IsTurn = true;
            config1.HaveShield = false;
            LivingConfig config2 = Game.BaseLivingConfig();
            config2.IsTurn = true;
            config2.HaveShield = false;
            m_king = Game.CreateBoss(bossID, 1680, 315, -1, 1, "", config);
            boss = Game.CreateBoss(bossID, 1615, 565, -1, 1, "", config1);
            king = Game.CreateBoss(bossID, 1600, 849, -1, 1, "", config2);
            boss.FallFrom(boss.X, boss.Y, "", 0, 0, 2000);
            king.FallFrom(king.X, king.Y, "", 0, 0, 2000);
            m_king.FallFrom(m_king.X, m_king.Y, "", 0, 0, 2000);
            m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
            king.SetRelateDemagemRect(king.NpcInfo.X, king.NpcInfo.Y, king.NpcInfo.Width, king.NpcInfo.Height);
            boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
            m_king.Properties1 = 0;
            boss.Properties1 = 0;
            king.Properties1 = 2;
            //king.Delay += 2;
            //boss.Delay += 1;
            CreateEffectSaying();
        }

        private void CreateEffectSaying()
        {
            Game.SendObjectFocus(m_king, 1, 1000, 0);
            m_king.PlayMovie("speak", 1500, 0);
            m_king.Say("Vo ve ở đây ăn l à??", 0, 1500);
            Game.SendObjectFocus(boss, 1, 4000, 0);
            boss.PlayMovie("speak", 4500, 0);
            boss.Say("Ối thần kỳ vl!", 0, 4500);
            Game.SendObjectFocus(king, 1, 8000, 0);
            king.PlayMovie("speak", 8500, 0);
            king.Say("Tụi bay vắt dái lên mà chạy còn kịp đó.", 0, 8500, 1500);
        }

        public override void OnNewTurnStarted()
        {
            base.OnNewTurnStarted();
        }


        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override bool CanGameOver()
        {
            if (m_king != null && m_king.IsLiving == false && boss != null && boss.IsLiving == false && king != null && king.IsLiving == false)
                return true;

            if (Game.TotalTurn > Game.MissionInfo.TotalTurn)
                return true;

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
            if (m_king != null && m_king.IsLiving == false && boss != null && boss.IsLiving == false && king != null && king.IsLiving == false)
            {
                Game.IsWin = true;
                //Game.SessionId++;
            }
            else
            {
                Game.IsWin = false;
            }
        }
    }
}