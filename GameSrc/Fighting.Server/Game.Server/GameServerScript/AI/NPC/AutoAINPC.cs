using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
    public class AutoAINPC : ABrain
    {
        private int[] ListBoss = new int[]
        {
            71081,
            71085,
            71089,
            71093
        };

        protected Player m_targer;

        private int m_turn = 0;

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            base.Body.CurrentDamagePlus = 1f;
            base.Body.CurrentShootMinus = 1f;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
            bool flag = false;
            foreach (Player current in base.Game.GetAllFightPlayers())
            {
                if (current.IsLiving && System.Math.Abs(current.X - base.Body.X) < 150)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                this.KillAttack(base.Body.X - 100, base.Body.X + 100);
            }
            else
            {
                this.m_targer = base.Game.FindRandomPlayer();
                this.KillAttack(this.m_targer);
            }

            this.m_targer = base.Game.FindRandomPlayer();
            if (this.m_turn == 0)
            {
                this.KillAttackPlayer1(this.m_targer, 0);
                this.m_turn++;
            }
            else if (this.m_turn == 1)
            {
                this.KillAttackPlayer1(this.m_targer, 1);
                this.m_turn++;
            }
            else
            {
                this.KillAttackPlayer1(this.m_targer, 2);
                this.m_turn = 0;
            }
            //}
        }

        private void KillAttackPlayer1(Player player, int t)
        {
            int num = player.Y - 50;
            int x;
            if (player.X + 300 < base.Body.X)
            {
                x = player.X + 300;
            }
            else if (player.X > base.Body.X + 300)
            {
                x = player.X - 300;
            }
            else if (player.X > base.Body.X)
            {
                x = base.Body.X + 1;
            }
            else
            {
                x = base.Body.X - 1;
            }
            /*if (num == base.Body.Y)
			{
				this.KillB();
			}*/
            if (t == 0)
            {
                base.Body.MoveTo(x, num, "fly", 2400, "", 6, new LivingCallBack(this.KillC));
            }
            else if (t == 1)
            {
                base.Body.MoveTo(x, num, "fly", 2400, "", 6, new LivingCallBack(this.KillB));
            }
            else
            {
                base.Body.MoveTo(x, num, "fly", 2400, "", 6, new LivingCallBack(this.KillA));
            }
        }

        private void KillA()
        {
            base.Body.CurrentDamagePlus = 1f;
            base.Body.PlayMovie("beatA", 2400, 0);
            base.Body.RangeAttacking(this.m_targer.X - 500, this.m_targer.X + 500, "cry", 4000, null);
        }

        private void KillB()
        {
            base.Body.CurrentDamagePlus = 1f;
            base.Body.PlayMovie("beatB", 2400, 0);
            base.Body.RangeAttacking(this.m_targer.X - 500, this.m_targer.X + 500, "cry", 4000, null);
        }

        private void KillC()
        {
            base.Body.CurrentDamagePlus = 1f;
            base.Body.PlayMovie("beatC", 2400, 0);
            base.Body.RangeAttacking(this.m_targer.X - 500, this.m_targer.X + 500, "cry", 4000, null);
        }

        private void KillAttack(int fx, int tx)
        {
            base.Body.CurrentDamagePlus = 1000f;
            base.Body.PlayMovie("beatB", 3000, 0);
            base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        private void KillAttack(Player player)
        {
            int num;
            if (player.X < base.Body.X)
            {
                num = -100;
            }
            else
            {
                num = 100;
            }
            base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", ((SimpleBoss)base.Body).NpcInfo.Delay, "", ((SimpleBoss)base.Body).NpcInfo.speed, this.Kill);
        }

        private void Kill()
        {
            if (System.Math.Abs(this.m_targer.Y - base.Body.Y) > 150 || System.Math.Abs(this.m_targer.X - base.Body.X) > 600)
            {
                base.Body.CurrentDamagePlus = 1f;
                base.Body.PlayMovie("beatC", 2400, 0);
                base.Body.RangeAttacking(this.m_targer.X - 100, this.m_targer.X + 100, "cry", 4000, null);
            }
            else
            {
                base.Body.CurrentDamagePlus = 1f;
                base.Body.PlayMovie("beatA", 1200, 0);
                base.Body.RangeAttacking(base.Body.X - 600, base.Body.X + 600, "cry", 4000, null);
            }
        }
    }
}
