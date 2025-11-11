using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Game.Logic.Actions;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object
{
	public class SimpleBoss : TurnedLiving
	{
		private static readonly ILog ilog_1;

		private NpcInfo npcInfo_0;

		private ABrain abrain_0;

		private List<SimpleNpc> list_0;

		private Dictionary<Player, int> dictionary_0;

		public NpcInfo NpcInfo => npcInfo_0;

		public List<SimpleNpc> Child => list_0;

		public int CurrentLivingNpcNum
		{
			get
			{
				int num = 0;
				foreach (SimpleNpc item in Child)
				{
					if (!item.IsLiving)
					{
						num++;
					}
				}
				return Child.Count - num;
			}
		}

		public SimpleBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type, string actions)
			: base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
		{
			list_0 = new List<SimpleNpc>();
			switch (type)
			{
				default:
					base.Type = (eLivingType)type;
					break;
				case 0:
					base.Type = eLivingType.SimpleBoss;
					break;
				case 1:
					base.Type = eLivingType.ClearEnemy;
					break;
				case 2:
					base.Type = eLivingType.SimpleNpc1;
					break;
				case 3:
					base.Type = eLivingType.BossSpecialDie;
					break;
			}
			base.ActionStr = actions;
			dictionary_0 = new Dictionary<Player, int>();
			npcInfo_0 = npcInfo;
			abrain_0 = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
			if (abrain_0 == null)
			{
				ilog_1.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
				abrain_0 = SimpleBrain.Simple;
			}
			abrain_0.Game = m_game;
			abrain_0.Body = this;
			try
			{
				abrain_0.OnCreated();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss Created error:{1}", arg);
			}
		}

		public override void Reset()
		{
			if (Config.IsWorldBoss)
				m_maxBlood = int.MaxValue;
			else
				m_maxBlood = npcInfo_0.Blood;
			BaseDamage = npcInfo_0.BaseDamage;
			BaseGuard = npcInfo_0.BaseGuard;
			Attack = npcInfo_0.Attack;
			Defence = npcInfo_0.Defence;
			Agility = npcInfo_0.Agility;
			Lucky = npcInfo_0.Lucky;
			Grade = npcInfo_0.Level;
			Experience = npcInfo_0.Experience;
			SetRect(npcInfo_0.X, npcInfo_0.Y, npcInfo_0.Width, npcInfo_0.Height);
			SetRelateDemagemRect(npcInfo_0.X, npcInfo_0.Y, npcInfo_0.Width, npcInfo_0.Height);
			if (m_direction == 1)
			{
				ReSetRectWithDir();
			}
			base.FireX = NpcInfo.FireX;
			base.FireY = NpcInfo.FireY;
			base.Reset();
		}

		public override void Die()
		{
			base.Die();
			try
            {
				this.abrain_0.OnDie();
            }
			catch (Exception ex)
            {
				ilog_1.ErrorFormat("SimpleBoss Die Error: {1}", ex);
            }
		}

		public override void Die(int delay)
		{
			base.Die(delay);
			try
            {
				this.abrain_0.OnDie();
            }
			catch (Exception ex)
            {
				ilog_1.ErrorFormat("SimpleBoss1 Die Error: {1}", ex);
            }
		}

		public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
		{
			bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
			if (source is Player)
			{
				Player key = source as Player;
				int num = damageAmount + criticalAmount;
				if (dictionary_0.ContainsKey(key))
				{
					dictionary_0[key] += num;
					return result;
				}
				dictionary_0.Add(key, num);
			}
			return result;
		}

		public SimpleNpc CreateBoss(int id, int x, int y, int direction, int type)
		{
			SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(id, x, y, type, direction);
			Child.Add(simpleNpc);
			return simpleNpc;
		}

		public void CreateChild(int id, int x, int y, int disToSecond, int maxCount)
		{
			CreateChild(id, x, y, disToSecond, maxCount, -1);
		}

		public void CreateChild(int id, int x, int y, int disToSecond, int maxCount, int direction)
		{
			if (CurrentLivingNpcNum < maxCount)
			{
				if (maxCount - CurrentLivingNpcNum >= 2)
				{
					Child.Add(((PVEGame)base.Game).CreateNpc(id, x + disToSecond, y, 1, direction));
					Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1, direction));
				}
				else if (maxCount - CurrentLivingNpcNum == 1)
				{
					Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1, direction));
				}
			}
		}

		public SimpleNpc CreateChild(int id, int x, int y, bool showBlood, LivingConfig config)
		{
			return CreateChild(id, x, y, 1, -1, showBlood, config);
		}

		public SimpleNpc CreateChild(int id, int x, int y, int dir, bool showBlood, LivingConfig config)
		{
			return CreateChild(id, x, y, 1, dir, showBlood, config);
		}

		public SimpleNpc CreateChild(int id, int x, int y, int type, int dir, bool showBlood, LivingConfig config)
		{
			SimpleNpc simpleNpc = ((PVEGame)base.Game).CreateNpc(id, x, y, type, dir, config);
			Child.Add(simpleNpc);
			if (!showBlood)
			{
				base.Game.SendLivingShowBlood(simpleNpc, 0);
			}
			return simpleNpc;
		}

		public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type)
		{
			int num = 0;
			int num2 = base.Game.Random.Next(0, maxCountForOnce);
			for (int i = 0; i < num2; i++)
			{
				num = base.Game.Random.Next(0, brithPoint.Length);
				CreateChild(id, brithPoint[num].X, brithPoint[num].Y, 4, maxCount);
			}
		}

		public List<SimpleNpc> FindChildLiving(int npcId)
		{
			List<SimpleNpc> list = new List<SimpleNpc>();
			foreach (SimpleNpc item in list_0)
			{
				if (item != null && item.IsLiving && item.NpcInfo.ID == npcId)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public void RemoveAllChild()
		{
			foreach (SimpleNpc item in Child)
			{
				if (item.IsLiving)
				{
					item.Die();
				}
			}
			list_0 = new List<SimpleNpc>();
		}

		public void RandomSay(string[] msg, int type, int delay, int finishTime)
		{
			string text = null;
			int num = base.Game.Random.Next(0, msg.Length);
			text = msg[num];
			m_game.AddAction(new LivingSayAction(this, text, type, delay, finishTime));
		}

		public override void PrepareNewTurn()
		{
			base.PrepareNewTurn();
			try
			{
				abrain_0.OnBeginNewTurn();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss BeginNewTurn error:{1}", arg);
			}
		}

		public override void PrepareSelfTurn()
		{
			base.PrepareSelfTurn();
			AddDelay(npcInfo_0.Delay);
			try
			{
				abrain_0.OnBeginSelfTurn();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss BeginSelfTurn error:{1}", arg);
			}
		}

		public override void StartAttacking()
		{
			base.StartAttacking();
			try
			{
				abrain_0.OnStartAttacking();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss StartAttacking error:{1}", arg);
			}
			if (base.IsAttacking)
			{
				StopAttacking();
			}
		}

		public override void StopAttacking()
		{
			base.StopAttacking();
		}

		public override void OnAfterTakedBomb()
		{
			try
			{
				abrain_0.OnAfterTakedBomb();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss OnAfterTakedBomb error:{1}", arg);
			}
		}

		public override void OnAfterTakeDamage(Living source)
		{
			try
			{
				abrain_0.OnAfterTakeDamage(source);
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss OnAfterTakedDamage error:{1}", arg);
			}
		}

		public override void OnAfterTakedFrozen()
		{
			try
			{
				abrain_0.OnAfterTakedFrozen();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss OnAfterTakedFrozen error:{1}", arg);
			}
			base.OnAfterTakedFrozen();
		}

		public override void OnHeal(int blood)
		{
			try
			{
				abrain_0.OnHeal(blood);
			}
			catch (Exception ex)
			{
				ilog_1.ErrorFormat("SimpleBoss OnHeal error:{0}", (object)ex);
			}
			base.OnHeal(blood);
		}

		public override void OnDie()
        {
			try
            {
				abrain_0.OnDie();
            }
			catch(Exception ex)
            {
				ilog_1.ErrorFormat("SimpleBoss OnDie Error:{0}", ex);
            }
            base.OnDie();
        }

        public override void OnDieByBomb()
        {
            try
            {
                abrain_0.OnDieByBomb();
            }
            catch (Exception arg)
            {
                ilog_1.ErrorFormat("SimpleBoss OnDieByBomb error:{1}", arg);
            }
        }

        public override void Dispose()
		{
			base.Dispose();
			try
			{
				abrain_0.Dispose();
			}
			catch (Exception arg)
			{
				ilog_1.ErrorFormat("SimpleBoss Dispose error:{1}", arg);
			}
		}

		public override void OnDieNewMethod()
		{
			try
			{
				abrain_0.OnDieNewMethod();
			}
			catch (Exception ex)
			{
				ilog_1.ErrorFormat("SimpleBoss OnDie Error:{0}", (object)ex);
			}
			base.OnDieNewMethod();
		}

		public override void DieAction(Player p)
		{
			this.abrain_0.OnDieAction(p);
		}

        public void CreateChildNEW(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type)
        {
            CreateChildNEW(id, brithPoint, maxCount, maxCountForOnce, type, 0);
        }

        public void CreateChildNEW(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type, int objtype)
        {
            Point[] tag = new Point[brithPoint.Length];
            if (CurrentLivingNpcNum < maxCount)
            {
                int CountForOnce;
                if (maxCount - CurrentLivingNpcNum >= maxCountForOnce)
                {
                    CountForOnce = maxCountForOnce;
                }
                else
                {
                    CountForOnce = maxCount - CurrentLivingNpcNum;
                }
                for (int i = 0; i < CountForOnce; i++)
                {
                    int index = 0;
                    if (CountForOnce <= brithPoint.Length)
                    {
                        for (int j = 0; j < brithPoint.Length; j++)
                        {
                            index = base.Game.Random.Next(0, brithPoint.Length);
                            bool result = false;
                            for (int length = 0; length < tag.Length; length++)
                            {
                                if (brithPoint[index] == tag[length])
                                {
                                    result = true;
                                    break;
                                }
                            }
                            if (!result)
                            {
                                tag[index] = brithPoint[index];
                                break;
                            }
                            j--;
                        }
                    }
                    if (objtype == 0)
                    {
                        Child.Add(((PVEGame)base.Game).CreateNpc(id, brithPoint[index].X, brithPoint[index].Y, type));
                    }
                    /*else
                    {
                        Boss.Add(((PVEGame)base.Game).CreateBoss(id, brithPoint[index].X, brithPoint[index].Y, -1, type));
                    }*/
                }
            }
        }

        static SimpleBoss()
		{
			ilog_1 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
