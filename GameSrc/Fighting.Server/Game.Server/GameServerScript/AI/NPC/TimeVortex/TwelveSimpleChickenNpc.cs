using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class TwelveSimpleChickenNpc : ABrain
    {
        private int BossID = 12010;

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
        }

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnCreated()
        {
            base.OnCreated();
        }

        private void RandMove()
        {
            int x = Game.Random.Next(Body.X - Game.Random.Next(200, 400), Body.X + Game.Random.Next(200, 400));
            SimpleBoss boss = Game.FindBossWithID(BossID);
            if (Body.Config.CanHeal)
            {
                Body.MoveTo(Body.X <= 600 ? Body.X + Game.Random.Next(200, 400) : (Body.X >= 1300) ? Body.X - Game.Random.Next(200, 400) : x, Body.Y, "walkA", 500, 7);
                Body.PlayMovie("standC", 1000, 1000);
            }
            else
            {
                Body.MoveTo(Body.X <= 600 ? Body.X + Game.Random.Next(200, 400) : (Body.X >= 1300) ? Body.X - Game.Random.Next(200, 400) : x, Body.Y, "walk", 500, 5);
                Body.Direction = boss.X < Body.X ? -1 : 1;
                Body.PlayMovie((x > 600 && x < 1300) ? "standB" : "standA", 1000, 1000);
            }
        }
        public override void OnStartAttacking()
        {
            base.OnStartAttacking();
            RandMove();
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public override void OnDieNewMethod()
        {
            base.OnDieNewMethod();
            if ((int)Body.Properties1 == 1)
            {
                ((PVEGame)Game).TotalKillCount -= 1;
                ((PVEGame)Game).Param4 += 1;
            }
            else
            {
                ((PVEGame)Game).TotalKillCount += 1;
            }
        }
        public override void OnHeal(int blood)
        {
            base.OnHeal(blood);
            if (!Body.Config.CanHeal)
                return;
            if (Body.Blood >= Body.MaxBlood)
            {
                Body.Properties1 = 1;
                Body.CallFuction(RemoveChicken, 1500);
            }
        }
        private void RemoveChicken()
        {
            Body.PlayMovie("standD", 1000, 1000);
            Body.Die(3000);
            Body.Dispose();
        }
    }
}