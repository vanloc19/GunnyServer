using System;
using System.Collections.Generic;
using System.Text;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using Game.Logic;
using System.Drawing;
using Game.Logic.Actions;
using Bussiness;
using Game.Logic.Effects;

namespace Game.Server.Scripts.AI.NPC
{
    public class Mession0002Npc2 : ABrain
    {
        protected Player m_targer;

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            m_body.CurrentDamagePlus = 1;
            m_body.CurrentShootMinus = 1;
        }

        public override void OnCreated()
        {
            base.OnCreated();
            Body.MaxBeatDis = 200;
        }

        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
            bool flag = false;

            foreach (Player current in base.Game.GetAllFightPlayers())
            {
                if (current.IsLiving)
                {
                    if (current.X > base.Body.X && current.X - base.Body.X < 150)
                    {
                        flag = true;
                    }
                    if (current.X < base.Body.X && base.Body.X - current.X < 150)
                    {
                        flag = true;
                    }
                }
            }
        }

        private void BeatTarget()
        {
            Body.Beat(m_targer, "beatA", 100, 0, 500);
        }

        public override void OnDie()
        {
            base.OnDie();
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}