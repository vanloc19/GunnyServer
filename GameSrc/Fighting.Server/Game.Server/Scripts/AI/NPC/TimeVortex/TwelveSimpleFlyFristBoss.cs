using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class TwelveSimpleFlyFristBoss : ABrain
    {

        private int NpcID1 = 12017;

        private int NpcID2 = 12018;

        private int NpcID3 = 12020;

        private List<SimpleNpc> Bombs = new List<SimpleNpc>();

        private LayerTop boomEffect;

        private SimpleNpc BombLeft;

        private SimpleNpc BombCenterToLeft;

        private SimpleNpc BombCenterLeft;

        private SimpleNpc BombCenterRight;

        private SimpleNpc BombCenterToRight;

        private SimpleNpc BombRight;

        private bool BombState = false;

        private void LaserAttack()
        {
            Body.CurrentDamagePlus = 1.5f;
            Body.PlayMovie("beatA", 1000, 3000);
            Body.CallFuction(EffectShexianAllPlayer, 3000);
            Body.RangeAttacking(Body.X - 1000, Body.X + 1000, "", 3500, null);
            ((PVEGame)Game).SendFreeFocus(Body.X, 900, 1, 3000, 1);
        }
        private void EffectShexianAllPlayer()
        {
            foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
            {
                if (!allLivingPlayer.IsLiving)
                {
                    continue;
                }
                ((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.nine.shexian", "", 1, 1);
            }
        }

        private void BombCreate()
        {
            SimpleBoss body = ((SimpleBoss)Body);
            LivingConfig livingConfig = ((PVEGame)Game).BaseLivingConfig();
            livingConfig.IsFly = true;
            livingConfig.CanCountKill = false;
            livingConfig.isShowBlood = false;
            livingConfig.isShowSmallMapPoint = false;
            livingConfig.IsTurn = false;
            BombLeft = body.CreateChild(NpcID3, 550, 320, -1, false, livingConfig);
            BombCenterToLeft = body.CreateChild(NpcID1, 758, 383, -1, false, livingConfig);
            BombCenterLeft = body.CreateChild(NpcID2, 872, 383, -1, false, livingConfig);
            BombCenterRight = body.CreateChild(NpcID2, 1045, 383, 1, false, livingConfig);
            BombCenterToRight = body.CreateChild(NpcID1, 1155, 383, 1, false, livingConfig);
            BombRight = body.CreateChild(NpcID3, 1371, 320, 1, false, livingConfig);
            Game.WaitTime(3000);
        }
        private void BombAttack()
        {
            ((PVEGame)Game).SendFreeFocus(Body.X, 600, 1, 1, 1);
            if (BombLeft.IsLiving)
            {
                BombLeft.PlayMovie("beat", 1000, 1000);
                Bombs.Add(BombLeft);
            }
            if (BombCenterToLeft.IsLiving)
            {
                BombCenterToLeft.PlayMovie("beatA", 500, 1500);
                Bombs.Add(BombCenterToLeft);
            }
            if (BombCenterLeft.IsLiving)
            {
                BombCenterLeft.PlayMovie("beatA", 500, 1500);
                Bombs.Add(BombCenterLeft);
            }
            if (BombCenterRight.IsLiving)
            {
                BombCenterRight.PlayMovie("beatA", 500, 1500);
                Bombs.Add(BombCenterRight);
            }
            if (BombCenterToRight.IsLiving)
            {
                BombCenterToRight.PlayMovie("beatA", 500, 1500);
                Bombs.Add(BombCenterToRight);
            }
            if (BombRight.IsLiving)
            {
                BombRight.PlayMovie("beat", 1000, 1000);
                Bombs.Add(BombRight);
            }
            Body.CurrentDamagePlus = 10f * Bombs.Count;
            Body.CallFuction(BoomEffect, 2000);
            Body.CallFuction(RemoveAllObjects, 2500);
            Body.CallFuction(RemoveBombEffect, 2900);
        }

        private void BoomEffect()
        {
            boomEffect = ((PVEGame)Game).CreateLayerTop(500, 300, "top", "asset.game.nine.daodan", "", 1, 0);
        }

        private void RemoveBombEffect()
        {
            Body.RangeAttacking(Body.X - 1000, Body.Y + 1000, "", 1, null);
            ((PVEGame)Game).RemovePhysicalObj(boomEffect, true);
        }
        private void RemoveAllObjects()
        {
            foreach (SimpleNpc bomb in Bombs)
            {
                bomb.Die();
                bomb.Dispose();
            }
            Bombs.Clear();
        }
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

        public override void OnStartAttacking()
        {
            if (BombState)
            {
                BombState = false;
                BombAttack();
                return;
            }
            if (Game.Random.Next(10) >= 7)
            {
                BombCreate();
                BombState = true;
            }
            else
            {
                LaserAttack();
            }

        }

        public override void OnDie()
        {
            base.OnDie();
            Game.ClearAllChildByIDs(new int[3] { NpcID1, NpcID2, NpcID3 });
        }
        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}