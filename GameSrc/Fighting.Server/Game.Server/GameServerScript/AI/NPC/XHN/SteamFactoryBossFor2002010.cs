using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class SteamFactoryBossFor2002010 : ABrain
    {
        protected Player m_targer;

        private static Random random = new Random();
        //http://gunny.vcdn.vn/image/game/bonesliving/asset_game_fifteen_472a.png?lv=837
        private int turn = 0;
        private int m_reduceBlood = 200000;
        private int m_reduceStreng = 800;

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
            FlyToRanDom();
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
        public void FlyToRanDom()
        {
            int x = random.Next(588, 1315);
            int y = random.Next(331, 637);
            Body.NewFlyTo(x, y, "fly", 0, "", 15);

            if (turn == 0)
            {
                turn++;
                Body.CallFuction(new LivingCallBack(BeatA),3000);
            }
            else if (turn == 1)
            {
                turn++;
                Body.CallFuction(new LivingCallBack(BeatB), 3000);
            }
            else if (turn == 2)
            {
                turn = 0;
                Body.CallFuction(new LivingCallBack(BeatC), 3000);
            }
        }
        public void BeatA()
        {
            Body.PlayMovie("beatA", 1000, 2000);
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 2000, null);
            var p = Game.FindRandomPlayer();
            if(p!=null)
            {
                p.AddEffect(new ReduceStrengthEffect(2, m_reduceStreng), 1000);
            }
        }
        public void BeatB()
        {
            Body.PlayMovie("beatB", 1000, 2000);
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 2000, null);
            foreach (Player p in base.Game.GetAllLivingPlayers())
            if (p != null)
            {
                p.AddEffect(new ContinueReduceBloodEffect(2, m_reduceBlood, Body), 500);
            }
        }
        public void BeatC()
        {
            Body.PlayMovie("beatC", 1000, 2000);
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 2000, null);
            var p = Game.FindRandomPlayer();
            if (p != null)
            {
                new IceFronzeEffect(2).Start(p);
            }
        }
    }
}
