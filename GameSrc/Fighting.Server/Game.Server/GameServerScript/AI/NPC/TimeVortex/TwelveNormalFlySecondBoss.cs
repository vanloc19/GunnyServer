using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GameServerScript.AI.NPC
{
    public class TwelveNormalFlySecondBoss : ABrain
    {
        private int indexTarget = 0;

        private int BombID = 12119;

        private int turnIndex = 0;

        private Point bPoint;

        private int bPointIndex = 0;

        private List<Layer> targets = new List<Layer>();

        private List<SimpleNpc> Bombs = new List<SimpleNpc>();

        private Player player;

        private List<Point> ps = new List<Point>();

        private List<Point> tpoints = new List<Point>();

        private List<Point> points = new List<Point>
        {
            new Point(150, 460),
            new Point(350, 361),
            new Point(310, 635),
            new Point(595, 425),
            new Point(790, 370),
            new Point(1015, 525),
            new Point(710, 380),
            new Point(1145, 560),
            new Point(850, 625),
            new Point(1415, 325),
            new Point(1640, 545),
            new Point(1785, 620),
            new Point(1405, 580),
            new Point(180, 645),
            new Point(520, 490),
            new Point(930, 620),
            new Point(1140, 550),
            new Point(270, 430),
            new Point(520, 625),
            new Point(777, 440),
            new Point(940, 690),
            new Point(1105, 455),
            new Point(1270, 620),
            new Point(1450, 700)
        };

        private List<Point> targetPoints = new List<Point>
        {
            new Point(311, 900),
            new Point(712, 900),
            new Point(1056, 900),
            new Point(1524, 900)
        };

        public override void OnBeginNewTurn()
        {
            base.OnBeginNewTurn();
            base.Body.CurrentDamagePlus = 1f;
            base.Body.CurrentShootMinus = 1f;
            base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
            if (base.Body.Direction == -1)
            {
                base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
            }
            else
            {
                base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
            }
        }

        public override void OnBeginSelfTurn()
        {
            base.OnBeginSelfTurn();
        }

        public override void OnCreated()
        {
            base.OnCreated();
            LivingConfig livingConfig = ((PVEGame)base.Game).BaseLivingConfig();
            livingConfig.IsFly = true;
            livingConfig.IsTurn = false;
            livingConfig.CanTakeDamage = false;
            livingConfig.CanCountKill = false;
            livingConfig.isShowSmallMapPoint = false;
            livingConfig.CanCollied = false;
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 250, 200, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 250, 400, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 250, 600, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 550, 200, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 550, 400, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 550, 600, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 1400, 200, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 1400, 400, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 1400, 600, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 1700, 200, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 1700, 400, 1, showBlood: false, livingConfig));
            Bombs.Add(((SimpleBoss)base.Body).CreateChild(BombID, 1700, 600, 1, showBlood: false, livingConfig));
            foreach (SimpleNpc npc in Game.GetNPCLivingWithID(BombID))
            {
                ((PVEGame)Game).SendHideBlood(npc, 0);
                npc.OnSmallMap(false);
            }
            base.Game.WaitTime(5000);
        }

        private void BombRandomMove()
        {
            player = base.Game.FindRandomPlayer();
            tpoints.AddRange(targetPoints);
            if (player.X <= 311 || player.X < 500)
            {
                bPointIndex = 0;
            }
            else if ((player.X > 500 && player.X <= 712) || player.X < 900)
            {
                bPointIndex = 1;
            }
            else if ((player.X > 900 && player.X <= 1056) || player.X < 1250)
            {
                bPointIndex = 2;
            }
            else
            {
                bPointIndex = 3;
            }
            targets.Add(((PVEGame)base.Game).Createlayer(player.X, targetPoints[bPointIndex].Y, "", "asset.game.nine.biaoji", "", 1, 0, CanPenetrate: true));
            tpoints.Remove(targetPoints[bPointIndex]);
            if (bPointIndex > 0)
            {
                bPoint = targetPoints[bPointIndex - 1];
            }
            else
            {
                bPoint = targetPoints[bPointIndex];
            }
            for (int i = 0; i < tpoints.Count; i++)
            {
                if (tpoints[i] == bPoint)
                {
                    switch (bPointIndex)
                    {
                        case 1:
                        case 3:
                            targets.Add(((PVEGame)base.Game).Createlayer(player.X - 400, tpoints[i].Y, "", "asset.game.nine.biaoji", "", 1, 0, CanPenetrate: true));
                            break;
                        case 0:
                        case 2:
                            targets.Add(((PVEGame)base.Game).Createlayer(player.X + 400, tpoints[i].Y, "", "asset.game.nine.biaoji", "", 1, 0, CanPenetrate: true));
                            break;
                    }
                }
                else
                {
                    targets.Add(((PVEGame)base.Game).Createlayer(tpoints[i].X, tpoints[i].Y, "", "asset.game.nine.biaoji", "", 1, 0, CanPenetrate: true));
                }
            }
            ps.AddRange(points);
            foreach (SimpleNpc bomb in Bombs)
            {
                Point randPoint = ps[ThreadSafeRandom.NextStatic(ps.Count - 1)];
                ps.Remove(randPoint);
                bomb.MoveTo(randPoint.X, randPoint.Y, "fly", 12, "", 0, delegate
                {
                    CreateBombState(bomb, randPoint);
                }, 500);
            }
            ps.Clear();
        }

        private void CreateBombState(Living bomb, Point point)
        {
            if (indexTarget >= targets.Count)
            {
                indexTarget = 0;
            }
            Layer layer = targets[indexTarget];
            indexTarget++;
            Point point2 = new Point(layer.X, layer.Y);
            Point point3 = new Point(point.X, point2.Y);
            double num = Math.Sqrt(Math.Pow(point3.X - point2.X, 2.0) + Math.Pow(point3.Y - point.Y, 2.0));
            double num2 = ((layer.X < point.X) ? Math.Acos((double)(point3.Y - point.Y) / num) : (Math.Acos((double)(point3.Y - point.Y) / num) * -1.0));
            double num3 = Math.Round(num2 * 180.0 / Math.PI);
            //Console.WriteLine($"Cos óãëà = {num2}\nÓãîë = {num3}");
            base.Game.LivingChangeAngle(bomb, 10, (int)num3 * 10, "beat");
        }

        private void BombAttack()
        {
            base.Body.CurrentDamagePlus = 15f;
            foreach (SimpleNpc bomb in Bombs)
            {
                bomb.PlayMovie("beatA", 1, 2000);
            }
            base.Body.CallFuction(RemoveTarget, 1500);
        }

        private void RemoveTarget()
        {
            ((PVEGame)base.Game).SendFreeFocus(930, 900, 1, 1, 1);
            foreach (Layer target in targets)
            {
                base.Body.RangeAttacking(target.X - 125, target.X + 125, "", 100, null);
                ((PVEGame)base.Game).RemovePhysicalObj(target, sendToClient: true);
            }
            targets.Clear();
        }

        public override void OnStartAttacking()
        {
            if (turnIndex != 0)
            {
                if (turnIndex == 1)
                {
                    turnIndex = 0;
                    BombAttack();
                    return;
                }
            }
            BombRandomMove();
            turnIndex++;
        }

        public override void OnDie()
        {
            base.OnDie();
            base.Game.ClearAllChildByID(BombID);
            if (targets.Count <= 0)
            {
                return;
            }
            foreach (Layer target in targets)
            {
                ((PVEGame)base.Game).RemovePhysicalObj(target, sendToClient: true);
            }
        }

        public override void OnStopAttacking()
        {
            base.OnStopAttacking();
        }
    }
}
