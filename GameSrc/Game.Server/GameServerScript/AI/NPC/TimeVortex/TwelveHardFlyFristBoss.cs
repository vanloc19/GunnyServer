using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System.Collections.Generic;

namespace GameServerScript.AI.NPC
{
    public class TwelveHardFlyFristBoss : ABrain
    {
        private int NpcID1 = 12217;

        private int NpcID2 = 12218;

        private int NpcID3 = 12220;

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
            base.Body.CurrentDamagePlus = 1.5f;
            base.Body.PlayMovie("beatA", 1000, 1000);
            base.Body.CallFuction(EffectShexianAllPlayer, 1000);
            base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "", 3500, null);
            ((PVEGame)base.Game).SendFreeFocus(base.Body.X, 900, 1, 3000, 1);
        }

        private void EffectShexianAllPlayer()
        {
            foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
            {
                if (allLivingPlayer.IsLiving)
                {
                    ((PVEGame)base.Game).Createlayer(allLivingPlayer.X, allLivingPlayer.Y, "", "asset.game.nine.shexian", "", 1, 1);
                }
            }
        }

        private void BombCreate()
        {
            SimpleBoss simpleBoss = (SimpleBoss)base.Body;
            LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
            livingConfig.IsFly = true;
            livingConfig.CanCountKill = false;
            livingConfig.isShowBlood = false;
            livingConfig.isShowSmallMapPoint = false;
            livingConfig.IsTurn = false;
            BombLeft = simpleBoss.CreateChild(NpcID3, 550, 320, -1, showBlood: false, livingConfig);
            BombCenterToLeft = simpleBoss.CreateChild(NpcID1, 758, 383, -1, showBlood: false, livingConfig);
            BombCenterLeft = simpleBoss.CreateChild(NpcID2, 872, 383, -1, showBlood: false, livingConfig);
            BombCenterRight = simpleBoss.CreateChild(NpcID2, 1045, 383, 1, showBlood: false, livingConfig);
            BombCenterToRight = simpleBoss.CreateChild(NpcID1, 1155, 383, 1, showBlood: false, livingConfig);
            BombRight = simpleBoss.CreateChild(NpcID3, 1371, 320, 1, showBlood: false, livingConfig);
            foreach (SimpleNpc npc in Game.FindAllNpcLiving())
            {
                ((PVEGame)Game).SendHideBlood(npc, 0);
                npc.OnSmallMap(false);
            }

            base.Game.WaitTime(3000);
        }

        private void BombAttack()
        {
            ((PVEGame)base.Game).SendFreeFocus(base.Body.X, 600, 1, 1, 1);
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
            base.Body.CurrentDamagePlus = 10f * (float)Bombs.Count;
            base.Body.CallFuction(BoomEffect, 2000);
            base.Body.CallFuction(RemoveAllObjects, 2500);
            base.Body.CallFuction(RemoveBombEffect, 2900);
        }

        private void BoomEffect()
        {
            boomEffect = ((PVEGame)base.Game).CreateLayerTop(500, 300, "top", "asset.game.nine.daodan", "", 1, 0);
        }

        private void RemoveBombEffect()
        {
            base.Body.RangeAttacking(base.Body.X - 1000, base.Body.Y + 1000, "", 1, null);
            ((PVEGame)base.Game).RemovePhysicalObj(boomEffect, sendToClient: true);
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
            int rand = Game.Random.Next(1, 10);
            if (BombState)
            {
                BombState = false;
                BombAttack();
            }
            else if (rand >= 7)
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
            base.Game.ClearAllChildByIDs(new int[3]
            {
                NpcID1,
                NpcID2,
                NpcID3
            });
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}
