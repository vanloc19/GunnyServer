using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
    public class SimpleNpcFor2002004 : ABrain
    {
        protected Player m_targer;

        private static Random random = new Random();
        private bool IsFist = false;
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

            int Index = random.Next(1, 6);
            if (IsFist)
            {
                IsFist = true;
                FlyToPoint(Index);
            }
            else
            {
                MovetoCenter(Index);
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void MovetoCenter(int Index)
        {
            base.Body.NewFlyTo(1445, 463, "fly", 0, "", 6, null);
            Body.CallFuction(new LivingCallBack(()=>FlyToPoint(Index)), 3000);
        }

        private void FlyToPoint(int Type)
        {
            Point p = new Point();
            switch(Type)
            {
                case 1:
                    {
                        p = new Point(1296, 296);
                        break;
                    }
                case 2:
                    {
                        p = new Point(1594, 289);
                        break;
                    }
                case 3:
                    {
                        p = new Point(1665, 472);
                        break;
                    }
                case 4:
                    {
                        p = new Point(1575, 624);
                        break;
                    }
                case 5:
                    {
                        p = new Point(1324, 621);
                        break;
                    }
                case 6:
                    {
                        p = new Point(1239, 474);
                        break;
                    }
            }
            base.Body.NewFlyTo(p.X, p.Y, "fly", 0, "", 7, null);
            Body.CallFuction(new LivingCallBack(suachua), 3000);
        }

        public void suachua()
        {
            Body.PlayMovie("beatA", 0, 3000);
        }
    }
}
