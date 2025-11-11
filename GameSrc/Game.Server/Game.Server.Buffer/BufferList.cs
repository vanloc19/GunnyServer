using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Buffer
{
	public class BufferList
	{
		private static readonly ILog ilog_0;

		private object object_0;

		protected List<AbstractBuffer> m_buffers;

		protected ArrayList m_clearList;

		protected volatile sbyte m_changesCount;

		private GamePlayer gamePlayer_0;

		protected ArrayList m_changedBuffers;

		private int int_0;

		public BufferList(GamePlayer player)
		{
			m_changedBuffers = new ArrayList();
			gamePlayer_0 = player;
			object_0 = new object();
			m_buffers = new List<AbstractBuffer>();
			m_clearList = new ArrayList();
		}

		public void LoadFromDatabase(int playerId)
		{
			lock (object_0)
            {
				using (PlayerBussiness pb = new PlayerBussiness())
                {
					BufferInfo[] infos = pb.GetUserBuffer(playerId);
					BeginChanges();
					foreach(BufferInfo info in infos)
                    {
						AbstractBuffer buffer = CreateBuffer(info);
						buffer?.Start(gamePlayer_0);
                    }
					ConsortiaBufferInfo[] consortiaBuffs = pb.GetUserConsortiaBuffer(gamePlayer_0.PlayerCharacter.ConsortiaID);
					foreach (ConsortiaBufferInfo info in consortiaBuffs)
                    {
						AbstractBuffer buffer = CreateConsortiaBuffer(info);
						if (buffer == null)
                        {
							buffer.Start(gamePlayer_0);
                        }
                    }
					CommitChanges();
                }
				Update();
				gamePlayer_0.ClearFightBuffOneMatch();
            }
		}

		public void SaveToDatabase()
		{
			lock (object_0)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					foreach (AbstractBuffer buffer in m_buffers)
					{
						buffer.Info.UserID = gamePlayer_0.PlayerId;
						playerBussiness.SaveBuffer(buffer.Info);
					}
					foreach (BufferInfo clear in m_clearList)
					{
						playerBussiness.SaveBuffer(clear);
					}
					m_clearList.Clear();
				}
			}
		}

		public bool AddBuffer(AbstractBuffer buffer)
		{
			lock (m_buffers)
			{
				m_buffers.Add(buffer);
			}
			OnBuffersChanged(buffer);
			return true;
		}

		public bool RemoveBuffer(AbstractBuffer buffer)
		{
			lock (m_buffers)
			{
				if (m_buffers.Remove(buffer))
				{
					m_clearList.Add(buffer.Info);
				}
			}
			OnBuffersChanged(buffer);
			return true;
		}

		public void UpdateBuffer(AbstractBuffer buffer)
		{
			OnBuffersChanged(buffer);
		}

		protected void OnBuffersChanged(AbstractBuffer buffer)
		{
			if (!m_changedBuffers.Contains(buffer))
			{
				m_changedBuffers.Add(buffer);
			}
			if (int_0 <= 0 && m_changedBuffers.Count > 0)
			{
				UpdateChangedBuffers();
			}
		}

		public void BeginChanges()
		{
			Interlocked.Increment(ref int_0);
		}

		public void CommitChanges()
		{
			int num = Interlocked.Decrement(ref int_0);
			if (num < 0)
			{
				if (ilog_0.IsErrorEnabled)
				{
					ilog_0.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
				}
				Thread.VolatileWrite(ref int_0, 0);
			}
			if (num <= 0 && m_changedBuffers.Count > 0)
			{
				UpdateChangedBuffers();
			}
		}

		public void UpdateChangedBuffers()
		{
			List<AbstractBuffer> abstractBufferList = new List<AbstractBuffer>();
			Dictionary<int, BufferInfo> bufflist = new Dictionary<int, BufferInfo>();
			foreach (AbstractBuffer changedBuffer in m_changedBuffers)
			{
				if (changedBuffer.Info.TemplateID > 100)
				{
					abstractBufferList.Add(changedBuffer);
				}
			}
			foreach (AbstractBuffer allBuffer in GetAllBuffers())
			{
				if (IsConsortiaBuff(allBuffer.Info.Type) && gamePlayer_0.IsConsortia())
				{
					bufflist.Add(allBuffer.Info.TemplateID, allBuffer.Info);
				}
			}
			GSPacketIn pkg = gamePlayer_0.Out.SendUpdateBuffer(gamePlayer_0, abstractBufferList.ToArray());
			if (gamePlayer_0.CurrentRoom != null)
			{
				gamePlayer_0.CurrentRoom.SendToAll(pkg, gamePlayer_0);
			}
			gamePlayer_0.Out.SendUpdateConsotiaBuffer(gamePlayer_0, bufflist);
			m_changedBuffers.Clear();
			bufflist.Clear();
		}

		public List<AbstractBuffer> GetAllBuffers()
		{
			List<AbstractBuffer> abstractBufferList = new List<AbstractBuffer>();
			lock (object_0)
			{
				foreach (AbstractBuffer buffer in m_buffers)
				{
					abstractBufferList.Add(buffer);
				}
				return abstractBufferList;
			}
		}

		public bool IsConsortiaBuff(int type)
		{
			if (type > 100)
			{
				return type < 115;
			}
			return false;
		}

		public virtual AbstractBuffer GetOfType(Type bufferType)
		{
			lock (m_buffers)
			{
				foreach (AbstractBuffer buffer in m_buffers)
				{
					if (buffer.GetType().Equals(bufferType))
					{
						return buffer;
					}
				}
			}
			return null;
		}

		public virtual AbstractBuffer GetOfType(BuffType type)
		{
			lock (m_buffers)
			{
				foreach (AbstractBuffer buffer in m_buffers)
				{
					if (buffer.Info.Type == (int)type)
					{
						return buffer;
					}
				}
			}
			return null;
		}

		public virtual void ResetAllPayBuffer()
		{
			new List<AbstractBuffer>();
			lock (m_buffers)
			{
				foreach (AbstractBuffer buffer in m_buffers)
				{
					if (buffer.Check() && buffer.IsPayBuff())
					{
						ItemTemplateInfo itemTemplate = ItemMgr.FindItemTemplate(buffer.Info.TemplateID);
						buffer.Info.ValidCount = itemTemplate.Property3;
						UpdateBuffer(buffer);
					}
				}
			}
		}

		public List<AbstractBuffer> GetAllBuffer()
		{
			List<AbstractBuffer> abstractBufferList = new List<AbstractBuffer>();
			lock (object_0)
			{
				foreach (AbstractBuffer buffer in m_buffers)
				{
					abstractBufferList.Add(buffer);
				}
				return abstractBufferList;
			}
		}

		public void Update()
		{
			foreach (AbstractBuffer abstractBuffer in GetAllBuffer())
			{
				try
				{
					if (!abstractBuffer.Check())
					{
						abstractBuffer.Stop();
					}
				}
				catch (Exception ex)
				{
					ilog_0.Error(ex);
				}
			}
		}

		public static AbstractBuffer CreateBuffer(ItemTemplateInfo template, int ValidDate)
		{
			return CreateBuffer(new BufferInfo
			{
				BeginDate = DateTime.Now,
				ValidDate = ValidDate * 24 * 60,
				Value = template.Property2,
				Type = template.Property1,
				ValidCount = template.Property3,
				TemplateID = template.TemplateID,
				IsExist = true
			});
		}

		public static AbstractBuffer CreateConsortiaBuffer(ConsortiaBufferInfo info)
        {
			BufferInfo buff = new BufferInfo();
			buff.TemplateID = info.BufferID;
			buff.BeginDate = info.BeginDate;
			buff.ValidDate = info.ValidDate;
			buff.Value = info.Value;
			buff.Type = info.Type;
			buff.ValidCount = 1;
			buff.IsExist = true;
			return CreateBuffer(buff);
        }

		public static AbstractBuffer CreatePayBuffer(int type, int Value, int ValidMinutes, int id)
		{
			BufferInfo buffer = new BufferInfo();
			buffer.TemplateID = id;
			buffer.BeginDate = DateTime.Now;
			buffer.ValidDate = ValidMinutes;
			buffer.Value = Value;
			buffer.Type = type;
			buffer.ValidCount = 1;
			buffer.IsExist = true;
			return CreateBuffer(buffer);
		}

		public static AbstractBuffer CreatePayBuffer(int type, int Value, int ValidMinutes)
		{
			BufferInfo buffer = new BufferInfo();
			buffer.TemplateID = 0;
			buffer.BeginDate = DateTime.Now;
			buffer.ValidDate = ValidMinutes;
			buffer.Value = Value;
			buffer.Type = type;
			buffer.ValidCount = 1;
			buffer.IsExist = true;
			return CreateBuffer(buffer);
		}

		public static AbstractBuffer CreateBufferHour(ItemTemplateInfo template, int ValidHour)
		{
			return CreateBuffer(new BufferInfo
			{
				BeginDate = DateTime.Now,
				ValidDate = ValidHour * 60,
				Value = template.Property2,
				Type = template.Property1,
				IsExist = true
			});
		}

		public static AbstractBuffer CreateBufferMinutes(ItemTemplateInfo template, int ValidMinutes)
		{
			return CreateBuffer(new BufferInfo
			{
				TemplateID = template.TemplateID,
				BeginDate = DateTime.Now,
				ValidDate = ValidMinutes,
				Value = template.Property2,
				Type = template.Property1,
				ValidCount = 1,
				IsExist = true
			});
		}

		public static AbstractBuffer CreateBuffer(BufferInfo info)
		{
			AbstractBuffer abstractBuffer = null;
			switch (info.Type)
			{
				case 11:
					abstractBuffer = new KickProtectBuffer(info);
					break;
				case 12:
					abstractBuffer = new OfferMultipleBuffer(info);
					break;
				case 13:
					abstractBuffer = new GPMultipleBuffer(info);
					break;
				case 15:
					abstractBuffer = new PropsBuffer(info);
					break;
				case 50:
					abstractBuffer = new AgiBuffer(info);
					break;
				case 51:
					abstractBuffer = new SaveLifeBuffer(info);
					break;
				case 52:
					abstractBuffer = new ReHealthBuffer(info);
					break;
				case 70:
					abstractBuffer = new CaddyGoodsBuffer(info);
					break;
				case 71:
					abstractBuffer = new TrainGoodsBuffer(info);
					break;
				case 72:
					abstractBuffer = new LevelTry(info);
					break;
				case 73:
					abstractBuffer = new CardGetBuffer(info);
					break;
				case 101:
					abstractBuffer = new ConsortionAddBloodGunCountBuffer(info);
					break;
				case 102:
					abstractBuffer = new ConsortionAddDamageBuffer(info);
					break;
				case 103:
					abstractBuffer = new ConsortionAddCriticalBuffer(info);
					break;
				case 104:
					abstractBuffer = new ConsortionAddMaxBloodBuffer(info);
					break;
				case 105:
					abstractBuffer = new ConsortionAddPropertyBuffer(info);
					break;
				case 106:
					abstractBuffer = new ConsortionReduceEnergyUseBuffer(info);
					break;
				case 107:
					abstractBuffer = new ConsortionAddEnergyBuffer(info);
					break;
				case 108:
					abstractBuffer = new ConsortionAddEffectTurnBuffer(info);
					break;
				case 109:
					abstractBuffer = new ConsortionAddOfferRateBuffer(info);
					break;
				case 110:
					abstractBuffer = new ConsortionAddPercentGoldOrGPBuffer(info);
					break;
				case 111:
					abstractBuffer = new ConsortionAddSpellCountBuffer(info);
					break;
				case 112:
					abstractBuffer = new ConsortionReduceDanderBuffer(info);
					break;
				case (int)BuffType.WorldBossHP:
					abstractBuffer = new WorldBossHPBuffer(info);
					break;
				case (int)BuffType.WorldBossAttrack:
					abstractBuffer = new WorldBossAttrackBuffer(info);
					break;
				case (int)BuffType.WorldBossHP_MoneyBuff:
					abstractBuffer = new WorldBossHP_MoneyBuffBuffer(info);
					break;
				case (int)BuffType.WorldBossAttrack_MoneyBuff:
					abstractBuffer = new WorldBossAttrack_MoneyBuffBuffer(info);
					break;
				case (int)BuffType.WorldBossMetalSlug:
					abstractBuffer = new WorldBossMetalSlugBuffer(info);
					break;
				case (int)BuffType.WorldBossAncientBlessings:
					abstractBuffer = new WorldBossAncientBlessingsBuffer(info);
					break;
				case (int)BuffType.WorldBossAddDamage:
					abstractBuffer = new WorldBossAddDamageBuffer(info);
					break;
			}
			return abstractBuffer;
		}

		public List<AbstractBuffer> GetAllBufferByTemplate()
		{
			List<AbstractBuffer> list = new List<AbstractBuffer>();
			lock (object_0)
			{
				foreach (AbstractBuffer buffer in m_buffers)
				{
					if (buffer.Info.TemplateID > 100)
					{
						list.Add(buffer);
					}
				}
				return list;
			}
		}

		static BufferList()
		{
			ilog_0 = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
		}
	}
}
