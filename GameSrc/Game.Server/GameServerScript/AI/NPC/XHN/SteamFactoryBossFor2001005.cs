using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class SteamFactoryBossFor2001005 : ABrain
    {
        protected Player m_targer;

        private static Random random = new Random();
        private int turn = 0;
        private int m_reduceStreng = 600;

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
            foreach (Player player in base.Game.GetAllLivingPlayers())
            {
                bool flag = !(player.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) is ReduceStrengthEffect);
                bool flag2 = flag;
                if (flag2)
                {
                    player.AddEffect(new ReduceStrengthEffect(600, this.m_reduceStreng), 0);
                }
            }
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            Body.CurrentDamagePlus = 1;
            if (turn == 0)
            {
                turn++;
                beatA();
            }
            else if (turn == 1)
            {
                turn++;
                beatB();
            }
            else if (turn == 2)
            {
                turn++;
                beatC();
            }
            else
            {
                turn = 0;
                beatD();
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        private void beatA()
        {
            Body.Say("Đòn lúc nãy đau không!", 0, 0);
            Body.PlayMovie("beatA", 1000, 1000);
            Body.CallFuction(new LivingCallBack(() => Attack(1)), 2000);
        }
        private void beatB()
        {
            Body.Say("Ầm ầm ầm!", 0, 0);
            Body.PlayMovie("beatB", 0, 1000);
            Body.CallFuction(new LivingCallBack(() => ShootBeatB()), 0);
        }
        private void ShootBeatB()
        {
            Player p = Game.FindRandomPlayer();
            if (p != null)
            {
                Body.ShootPoint(p.X, p.Y, 150, 1000, 10000, 3, 3f, 2500);
            }
        }
        private void beatC()
        {
            Body.Say("Thịt xông khói!", 0, 0);
            Body.PlayMovie("beatC", 0, 1000);
            Body.CallFuction(new LivingCallBack(() => Attack2(1)), 2000);
        }
        private void beatD()
        {
            Body.Say("Mưa lửa!", 0, 0);
            Body.PlayMovie("beatD", 0, 1000);
            Body.CallFuction(new LivingCallBack(() => Attack(1.5f)), 2000);
        }
        public void Attack(float CurrentDamagePlus)
        {
            Body.ChangeDirection(-1, 100);
            Body.CurrentDamagePlus = CurrentDamagePlus;
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 1000, null);
        }

        public void Attack2(float CurrentDamagePlus)
        {
            Body.ChangeDirection(-1, 100);
            Body.CurrentDamagePlus = CurrentDamagePlus;
            Body.RangeAttacking(Body.X - 600, Body.X + 600, "cry", 1000, null);
        }
    }
}
