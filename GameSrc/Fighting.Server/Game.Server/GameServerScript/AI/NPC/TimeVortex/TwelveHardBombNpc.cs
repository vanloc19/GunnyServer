using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;

namespace GameServerScript.AI.NPC
{
    public class TwelveHardBombNpc : ABrain
    {
        private Player m_target = null;

        private int m_targetDis = 0;

        private int m_binhdoanID = 12006;
        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            base.Body.CurrentDamagePlus = 1f;
            base.Body.CurrentShootMinus = 1f;
            Body.Properties1 = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            if (Body.Properties1 > 0)
            {
                Body.Properties1--;
                return;
            }
            this.m_target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
            this.m_targetDis = (int)this.m_target.Distance(base.Body.X, base.Body.Y);
            if (this.m_targetDis < 50)
            {
                base.Body.PlayMovie("beatA", 100, 0);
                base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1500, null);
                base.Body.Die(1000);
            }
            else
            {
                this.MoveToPlayer(this.m_target);
            }
        }
        public override void OnDieByBomb()
        {
            SimpleBoss simpleBoss = Game.FindBossWithID(m_binhdoanID);
            if (simpleBoss != null)
            {
                simpleBoss.Config.IsTurn = true;
                simpleBoss.ShootCount++;
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void MoveToPlayer(Player player)
        {
            int num = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
            int delay = 0;
            if (player.X > base.Body.X)
            {
                if (base.Body.X + num >= player.X)
                {
                    delay = Game.GetDelayDistance(Body.X, player.X - 10, 3) + 500;
                    base.Body.MoveTo(player.X - 50, base.Body.Y, "walk", ((SimpleNpc)base.Body).NpcInfo.speed, "", 2000, new LivingCallBack(this.Beat), delay);
                }
                else
                {
                    delay = Game.GetDelayDistance(Body.X, Body.X + num, 3) + 500;
                    base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", ((SimpleNpc)base.Body).NpcInfo.speed, "", 2000, new LivingCallBack(this.Beat), delay);
                }
            }
            else
            {
                if (base.Body.X - num <= player.X)
                {
                    delay = Game.GetDelayDistance(Body.X, player.X + 10, 3) + 500;
                    base.Body.MoveTo(player.X + 50, base.Body.Y, "walk", ((SimpleNpc)base.Body).NpcInfo.speed, "", 2000, new LivingCallBack(this.Beat), delay);
                }
                else
                {
                    delay = Game.GetDelayDistance(Body.X, Body.X - num, 3) + 500;
                    base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", ((SimpleNpc)base.Body).NpcInfo.speed, "", 2000, new LivingCallBack(this.Beat), delay);
                }
            }
        }

        public void Beat()
        {
            this.m_targetDis = (int)this.m_target.Distance(base.Body.X, base.Body.Y);
            if (this.m_targetDis <= 50)
            {
                base.Body.PlayMovie("beatA", 100, 0);
                base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1500, null);
                base.Body.Die(1000);
            }
        }
    }
}
