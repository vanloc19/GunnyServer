
using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;

namespace GameServerScript.AI.NPC
{
    public class TwelveTerrorCaptainBoss : ABrain
    {
        private int m_attackTurn = 0;

        private Player m_target;

        private int m_dis;

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
            Body.MaxBeatDis = 190;
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            if (Body.Properties1 > 0)
            {
                Body.Properties1--;
                return;
            }
            if (m_attackTurn == 0)
            {
                SayToPlayer();
                m_attackTurn = 1;
            }
            else
            {
                MoveToPlayerAndAttack();
            }
            //Body.Direction = Game.FindlivingbyDir(Body);
        }

        private void SayToPlayer()
        {
            (Game as PVEGame).SendObjectFocus(Body, 0, 1000, 0);
            Body.FallFrom(Body.X, Body.Y, "", 2000, 0, 0, null);
            Body.Say("Ta sẽ cho các người biết thế nào là lợi hại của ta!", 0, 2500, 2000);
        }

        private void MoveToPlayerAndAttack()
        {
            m_target = Game.FindNearestPlayer(Body.X, Body.Y);

            if (m_target != null && m_target.IsLiving)
            {
                m_dis = (int)m_target.Distance(Body.X, Body.Y);

                if (m_dis > Body.MaxBeatDis)
                {
                    Body.CallFuction(MoveToTarget, 1000);
                }
                else
                {
                    Body.CallFuction(Beat, 1000);
                }
            }
        }

        private void MoveToTarget()
        {
            int ramdis = Game.Random.Next(((SimpleBoss)Body).NpcInfo.MoveMax, ((SimpleBoss)Body).NpcInfo.MoveMax);

            if (m_dis < ramdis)
                ramdis = m_dis;

            int moveX = Body.Direction == -1 ? Body.X - ramdis : Body.X + ramdis;
            int delay = Game.GetDelayDistance(Body.X, moveX, 3) + 1000;
            Body.MoveTo(moveX, Body.Y, "walk", ((SimpleBoss)Body).NpcInfo.speed, "", 1200, new LivingCallBack(Beat), delay);
        }

        public override void OnAfterTakedFrozen()
        {
            base.OnAfterTakedFrozen();
            Body.Properties1 = 2;
        }

        private void Beat()
        {
            Body.Beat(m_target, "beatA", 100, 0, 500, 1, 1);
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}