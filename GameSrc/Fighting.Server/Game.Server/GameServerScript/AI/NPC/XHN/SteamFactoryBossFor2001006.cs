using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class SteamFactoryBossFor2001006 : ABrain
    {
        protected Player m_targer;

        private static Random random = new Random();
        private int turn;
        private int BaseX;
        private double BaseDef;
        private bool IsFist = false;
        private int m_reduceStreng = 600;
        private int redNpcID = 2001001;
        private int redNpcID3 = 2001003;
        private int redNpcID2 = 2001002;
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

            if (!IsFist)
            {
                IsFist = true;
                BaseDef = Body.Defence;
                BaseX = Body.X;
            }
            if (turn == 0)
            {
                MovetoLeft();
                turn++;
            }
            else
            {
                turn = 0;
                Attack();
            }

            Body.CurrentDamagePlus = 1;
            Body.Defence = BaseDef;
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }

        public void MovetoLeft()
        {
            base.Body.NewFlyTo(BaseX - 150, Body.Y, "fly", 0, "", 15);
            Body.CallFuction(new LivingCallBack(MovetoRight), 1000);
        }
        public void MovetoRight()
        {
            base.Body.NewFlyTo(BaseX - 150, Body.Y, "fly", 0, "", 15);
            Body.CallFuction(new LivingCallBack(MovetoCenter), 2000);
        }
        public void MovetoCenter()
        {
            base.Body.NewFlyTo(BaseX, Body.Y, "fly", 0, "", 15);
            Body.CallFuction(new LivingCallBack(TaoQuai), 1000);
        }
        public void TaoQuai()
        {
            LivingConfig config = base.BaseLivingConfig();
            config.HasTurn = false;

            ((PVEGame)Game).CreateNpc(redNpcID, 679 - 15, 1042, 0, 1, config);
            ((PVEGame)Game).CreateNpc(redNpcID, 679, 1042, 0, 1, config);
            ((PVEGame)Game).CreateNpc(redNpcID, 679 + 15, 1042, 0, 1, config);

            ((PVEGame)Game).CreateNpc(redNpcID3, 997 - 15, 1042, 0, 1, config);
            ((PVEGame)Game).CreateNpc(redNpcID3, 997, 1042, 0, 1, config);
            ((PVEGame)Game).CreateNpc(redNpcID3, 997 + 15, 1042, 0, 1, config);

            ((PVEGame)Game).CreateNpc(redNpcID2, 1322 - 15, 1042, 0, 1, config);
            ((PVEGame)Game).CreateNpc(redNpcID2, 1322, 1042, 0, 1, config);
            ((PVEGame)Game).CreateNpc(redNpcID2, 1322 + 15, 1042, 0, 1, config);
        }
        public void Attack()
        {
            Body.ChangeDirection(-1, 100);
            if(((PVEGame)Game).FindAllNpcByName("Gà Robot").Count>0)
            {
                Body.CurrentDamagePlus = 2f;
            }
            if (((PVEGame)Game).FindAllNpcByName("Gà Con Robot").Count > 0)
            {
                Body.AddBlood(1920);
                Body.AddBlood(1920);
                Body.AddBlood(1920);
            }
            if (((PVEGame)Game).FindAllNpcByName("Goblin Robot").Count > 0)
            {
                Body.Defence += 100;
            }
            Body.CallFuction(new LivingCallBack(beatB), 2000);
        }
        public void beatB()
        {
            Body.PlayMovie("beatB", 1000, 2000);
            Body.RangeAttacking(Body.X - 2000, Body.X + 2000, "cry", 2000, null);
            var ListNPC = ((PVEGame)Game).FindAllNpc2();
            foreach (var NPC in ListNPC)
            {
                if(NPC.NpcInfo.Name == "Gà Robot" || NPC.NpcInfo.Name == "Gà Con Robot" || NPC.NpcInfo.Name == "Goblin Robot")
                {
                    NPC.Die(-1);
                    ((PVEGame)Game).RemoveLiving2(NPC.Id);
                }
                
            }
        }
    }
}
