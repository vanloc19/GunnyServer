using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic.Effects;

namespace GameServerScript.AI.NPC
{
    public class DHTAHBossS1 : ABrain
    {
        private int attackingTurn = 1;

        private int orchinIndex = 1;

        private int currentCount = 0;

        private int Dander = 0;

        private int npcID = 20302;

        #region NPC 说话内容
        private static string[] AllAttackChat = new string[] {
             "Xem tuyệt chiêu nè!",
          
             "Di chuyển mát mẻ!<br/>Bạn muốn tìm hiểu không?",
          
             "Chịu không nỗi!",

             "Bạn sẽ trả giá cho việc này! "
        };

        private static string[] CallChat = new string[]{
            "Nào, <br/>cho thử sức mạnh của lựu đạn!"                          
        };

        private static string[] AngryChat = new string[]{
            "Bạn buộc tôi để lừa!"                          
        };

        private static string[] KillAttackChat = new string[]{
            "Bạn đến chết?"                          
        };

        private static string[] SealChat = new string[]{
            "Chầu Diêm Vương!"                          
        };

        private static string[] KillPlayerChat = new string[]{
            "Địa ngục là điểm đến duy nhất của bạn!",                  
 
            "Quá dễ bị tổn thương."
        };
        #endregion


        public List<SimpleNpc> orchins = new List<SimpleNpc>();

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            Body.CurrentDamagePlus = 1;
            Body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            bool result = false;
            int maxdis = 0;
			Body.Direction = Game.FindlivingbyDir(Body);
            foreach (Player player in Game.GetAllFightPlayers())
            {
                if (player.IsLiving && player.X > 620 && player.X < 1160)
                {
                    int dis = (int)Body.Distance(player.X, player.Y);
                    if (dis > maxdis)
                    {
                        maxdis = dis;
                    }
                    result = true;
                }
            }

            if (result)
            {
                KillAttack(620, 1160);
                return;
            }

            if (attackingTurn == 1)
            {
                Healing();
                HalfAttack();
            }
            else if (attackingTurn == 2)
            {
                Healing();
                Summon();
            }
            else if (attackingTurn == 3)
            {
                Healing();
                Seal();
            }
            else if (attackingTurn == 4)
            {
                Healing();
                Angger();
            }
            else
            {
                GoOnAngger();
                attackingTurn = 0;
            }
            attackingTurn++;
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void HalfAttack()
        {
            Body.CurrentDamagePlus = 1.5f;
            int index = Game.Random.Next(0, SealChat.Length);
            Body.Say(AllAttackChat[index], 1, 500);
            Body.PlayMovie("beatB", 2500, 0);
            if (Body.Direction == 1)
            {
                Body.RangeAttacking(Body.X, Body.X + 1000, "cry", 3300, null);
            }
            else
            {
                Body.RangeAttacking(Body.X - 1000, Body.X, "cry", 3300, null);
            }
        }

        public void Summon()
        {
            int index = Game.Random.Next(0, CallChat.Length);
            Body.Say(CallChat[index], 1, 0);
            Body.PlayMovie("beatA", 100, 0);
            Body.CallFuction(new LivingCallBack(CreateChild), 2500);
        }

        public void Seal()
        {
            int index = Game.Random.Next(0, SealChat.Length);
            ((SimpleBoss)Body).Say(SealChat[index], 1, 0);
            Player m_player = Game.FindRandomPlayer();
            Body.PlayMovie("mantra", 2000, 2000);
			List<Player> players = Game.GetAllLivingPlayers();

                foreach (Player player in players)
                {
					player.AddEffect(new LockDirectionEffect(2), 0);
                }
            Body.Seal(m_player, 1, 3000);
        }

        public void Angger()
        {
            int index = Game.Random.Next(0, AngryChat.Length);
            Body.Say(AngryChat[index], 1, 0);
            Body.State = 1;
            Dander = Dander + 100;
			Body.PlayMovie("angry", 1000, 0);
            ((SimpleBoss)Body).SetDander(Dander);

            if (Body.Direction == -1)
            {
                ((SimpleBoss)Body).SetRelateDemagemRect(8, -252, 74, 50);
            }
            else
            {
                ((SimpleBoss)Body).SetRelateDemagemRect(-82, -252, 74, 50);
            }
        }

        public void GoOnAngger()
        {
            if (Body.State == 1)
            {
                Body.CurrentDamagePlus = 1000;
                Body.PlayMovie("beatC", 3500, 0);
                Body.RangeAttacking(Body.X - 1000, Body.X + 1000, "cry", 5600, null);
                Body.Die(5600);
            }
            else
            {
                ((SimpleBoss)Body).SetRelateDemagemRect(-41, -187, 83, 140);

                Body.PlayMovie("mantra", 0, 2000);
                List<Player> players = Game.GetAllLivingPlayers();

                foreach (Player player in players)
                {
                    player.AddEffect(new ContinueReduceBloodEffect(2, 700, player), 0);
                }
            }
        }

        public void KillAttack(int fx, int tx)
        {
            Body.CurrentDamagePlus = 100f;
            int index = Game.Random.Next(0, KillAttackChat.Length);
            ((SimpleBoss)Body).Say(KillAttackChat[index], 1, 500);
            Body.PlayMovie("beatB", 2500, 0);
            Body.RangeAttacking(fx, tx, "cry", 3300, null);
        }

        public void Healing()
        {
            Body.SyncAtTime = true;
            Body.AddBlood(10000);
            Body.Say("Haha, Mấy thằng oắt con này!", 1, 0);
        }

        public void CreateChild()
        {
            ((SimpleBoss)Body).CreateChild(npcID, 680, 680, -1, 405, 6);
        }
    }
}
