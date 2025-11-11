using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class TwelveHardCrocodileBoss : ABrain
    {
        private int m_attackTurn = 0;

        private int npcID = 12207;

        private Player target = null;

        private PhysicalObj moive = null;

        private PhysicalObj m_moive = null;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] {
            "Xem các ngươi！！<br/>lợi hại như thế nào",

            "Lưỡi kiếm của ta！",

            "Nén không được đâu！！"
        };

        private static string[] ShootChat = new string[]{
             "让你知道什么叫百发百中！",

             "送你一个球~你可要接好啦",

             "你们这群无知的低等庶民"
        };

        private static string[] ShootedChat = new string[]{
           "哎呀~~你们为什么要攻击我？<br/>我在干什么？",

            "噢~~好痛!我为什么要战斗？<br/>我必须战斗…"

        };

        private static string[] AddBooldChat = new string[]{
            "扭啊扭~<br/>扭啊扭~~",

            "哈利路亚~<br/>路亚路亚~~",

            "呀呀呀，<br/>好舒服啊！"

        };

        private static string[] KillAttackChat = new string[]{
            "君临天下！！"
        };

        #endregion

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
            ((PVEGame)Game).SendLivingActionMapping(Body, "cry", "cryB");
            ((PVEGame)Game).SendLivingActionMapping(Body, "die", "dieB");

            //Body.Defence += 5000;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            target = Game.FindRandomPlayer();

            if (m_attackTurn == 0)
            {
                Walk2();
                m_attackTurn++;
            }
            else if (m_attackTurn == 1)
            {
                PersonalAttack();
                m_attackTurn++;
            }
            else if (m_attackTurn == 2)
            {
                AllAttack();
                m_attackTurn++;
            }
            else if (m_attackTurn == 3)
            {
                DameOneKill();
                m_attackTurn++;
            }
            else if (m_attackTurn == 4)
            {
                InDameOneKill();
                m_attackTurn++;
            }
            else
            {
                Walk();
                m_attackTurn = 0;
            }
        }

        private void DameOneKill()
        {
            int index = Game.Random.Next(0, AllAttackChat.Length);
            Body.Say(AllAttackChat[index], 1, 0);

            Body.PlayMovie("beatD", 2000, 0);

            Body.CallFuction(new LivingCallBack(CallPhysicalObj), 3400);
        }

        private void InDameOneKill()
        {
            int index = Game.Random.Next(0, AllAttackChat.Length);
            Body.Say(AllAttackChat[index], 1, 0);

            Body.PlayMovie("beatD", 2000, 0);

            Body.CallFuction(new LivingCallBack(CallPhysicalObjBeat), 3400);
        }

        private void CallPhysicalObj()
        {
            moive = ((PVEGame)Game).Createlayer(target.X, target.Y, "moive", "asset.game.nine.biaoji", "", 2, 0);
        }


        private void CallPhysicalObjBeat()
        {
            m_moive = ((PVEGame)Game).Createlayer(moive.X, moive.Y, "moive", "asset.game.nine.dapao", "beatA", 1, 0);
            ((PVEGame)Game).SendGameFocus(moive.X, moive.Y, 1, 0, 0);
            Body.CurrentDamagePlus = 1;
            Body.RangeAttacking(moive.X - 200, moive.X + 200, "cryA", 2000, null);
            if (moive != null)
            {
                Game.RemovePhysicalObj(moive, true);
                moive = null;
            }
        }

        private void AllAttack()
        {
            //Body.CurrentDamagePlus = 0.5f;

            int index = Game.Random.Next(0, AllAttackChat.Length);
            Body.Say(AllAttackChat[index], 1, 0);
            Body.PlayMovie("beatC", 2000, 0);
            List<Player> allPlayers = Game.GetAllFightPlayers();
            foreach (Player player in allPlayers)
            {
                Body.BeatDirect(player, "", 4000, 1, 1);
            }
        }

        private void Walk()
        {
            int mtX = 0;
            if (Body.X > target.X)
            {
                mtX = target.X + 150;
            }
            else
            {
                mtX = target.X - 150;
            }
            int delay = Game.GetDelayDistance(Body.X, mtX, 8) + 500;
            Body.MoveTo(mtX, Body.Y, "walk", 9, "", 1200, new LivingCallBack(NextAttack), delay);
        }

        private void Walk2()
        {
            int mtX = Game.Random.Next(1261, 1350);
            int delay = Game.GetDelayDistance(Body.X, mtX, 8) + 1000;
            Body.MoveTo(mtX, Body.Y, "walk", 8, "", 1200, new LivingCallBack(CreateChild), delay);
        }

        private void NextAttack()
        {
            Body.ChangeDirection(target, 0);
            Body.PlayMovie("beatB", 0, 3000);
            Body.RangeAttacking(target.X - 150, target.X + 150, "cry", 3000, null);
        }

        private void PersonalAttack()
        {
            Body.PlayMovie("beatA", 1200, 3000);
            Body.CallFuction(new LivingCallBack(CallBall), 2700);
        }

        private void CallBall()
        {
            Body.RangeAttacking(target.X - 100, target.X + 100, "cry", 1000, null);
        }

        public void CreateChild()
        {
            Body.Say("Lựu đạn đâu ra cho anh", 1, 0);
            Body.Direction = Game.FindlivingbyDir(Body);
            Body.PlayMovie("beatD", 0, 1500);
            int[] mtX = { 450, 750, 1000, 1350 };
            for (int i = 0; i < 4; i++)
            {
                ((SimpleBoss)Body).CreateChild(npcID, mtX[i], 700, -1, true, ((PVEGame)Game).BaseLivingConfig());
            }
            foreach (SimpleNpc npc in Game.GetNPCLivingWithID(npcID))
            {
                npc.Properties1 = 1;
            }
            //((SimpleBoss)Body).CreateChild(npcID, 641, 700, -1, 4, 3);
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
            if (m_moive != null)
            {
                Game.RemovePhysicalObj(m_moive, true);
                m_moive = null;
            }
        }
        public override void OnAfterTakeDamage(Living player)
        {
            base.OnAfterTakeDamage(player);
            if (player is SimpleNpc)
            {
                ((PVEGame)Game).SendLivingActionMapping(Body, "cry", "cryA");
                ((PVEGame)Game).SendLivingActionMapping(Body, "die", "dieA");
            }
            else
            {
                ((PVEGame)Game).SendLivingActionMapping(Body, "cry", "cryB");
                ((PVEGame)Game).SendLivingActionMapping(Body, "die", "dieB");
            }
        }
    }
}
