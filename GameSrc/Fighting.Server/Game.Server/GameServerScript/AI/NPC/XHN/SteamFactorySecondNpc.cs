using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class SteamFactorySecondNpc : ABrain
    {
        protected Player m_targer;

        private static Random random = new Random();


        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();

        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            MovetoRandom();
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void MovetoRandom()
        {
            int X = random.Next(1310, 1535);
            base.Body.MoveToSpeed(X, Body.Y, "walk", 0, 7, new LivingCallBack(Attack));
            Body.ChangeDirection(-1, 100);
        }

        public void Attack()
        {
            Body.ChangeDirection(-1, 100);
            Body.CurrentDamagePlus = 1.2f;
            Body.PlayMovie("beatA", 1000, 2000);
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 4000, null);
        }
    }
}
