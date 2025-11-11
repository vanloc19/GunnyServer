using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class ConsortiaBossMgr
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

		private static Dictionary<int, ConsortiaInfo> m_consortias = new Dictionary<int, ConsortiaInfo>();

		public static bool AddConsortia(ConsortiaInfo consortia)
		{
			GSPacketIn packet = new GSPacketIn(180);
			packet.WriteInt(consortia.ConsortiaID);
			packet.WriteInt(consortia.ChairmanID);
			packet.WriteByte((byte)consortia.bossState);
			packet.WriteDateTime(consortia.endTime);
			packet.WriteInt(consortia.extendAvailableNum);
			packet.WriteInt(consortia.callBossLevel);
			packet.WriteInt(consortia.Level);
			packet.WriteInt(consortia.SmithLevel);
			packet.WriteInt(consortia.StoreLevel);
			packet.WriteInt(consortia.SkillLevel);
			packet.WriteInt(consortia.Riches);
			packet.WriteDateTime(consortia.LastOpenBoss);
			GameServer.Instance.LoginServer.SendPacket(packet);
			return true;
		}

		public static bool AddConsortia(int consortiaId, ConsortiaInfo consortia)
		{
			m_clientLocker.AcquireWriterLock(-1);
			try
			{
				if (m_consortias.ContainsKey(consortiaId))
				{
					return false;
				}
				m_consortias.Add(consortiaId, consortia);
			}
			finally
			{
				m_clientLocker.ReleaseWriterLock();
			}
			return true;
		}

		public static void CreateBoss(ConsortiaInfo consortia, int npcId)
		{
			GSPacketIn packet = new GSPacketIn(183);
			packet.WriteInt(consortia.ConsortiaID);
			packet.WriteByte((byte)consortia.bossState);
			packet.WriteDateTime(consortia.endTime);
			packet.WriteDateTime(consortia.LastOpenBoss);
			int val = 15000000;
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
			if (npcInfoById != null)
			{
				val = npcInfoById.Blood;
			}
			packet.WriteInt(val);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}

		public static void ExtendAvailable(int consortiaId, int riches)
		{
			GSPacketIn packet = new GSPacketIn(182);
			packet.WriteInt(consortiaId);
			packet.WriteInt(riches);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}

		public static long GetConsortiaBossTotalDame(int consortiaId)
		{
			if (m_consortias.ContainsKey(consortiaId))
			{
				long totalAllMemberDame = m_consortias[consortiaId].TotalAllMemberDame;
				long maxBlood = m_consortias[consortiaId].MaxBlood;
				if (totalAllMemberDame > maxBlood)
				{
					totalAllMemberDame = maxBlood - 1000;
				}
				return totalAllMemberDame;
			}
			return 0L;
		}

		public static ConsortiaInfo GetConsortiaById(int consortiaId)
		{
			ConsortiaInfo info = null;
			m_clientLocker.AcquireReaderLock(10000);
			try
			{
				if (m_consortias.ContainsKey(consortiaId))
				{
					return m_consortias[consortiaId];
				}
				return info;
			}
			finally
			{
				m_clientLocker.ReleaseReaderLock();
			}
		}

		public static bool GetConsortiaExit(int consortiaId)
		{
			m_clientLocker.AcquireReaderLock(10000);
			try
			{
				return m_consortias.ContainsKey(consortiaId);
			}
			finally
			{
				m_clientLocker.ReleaseReaderLock();
			}
		}

		public static void reload(int consortiaId)
		{
			GSPacketIn packet = new GSPacketIn(184);
			packet.WriteInt(consortiaId);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}

		public static void SendConsortiaAward(int consortiaId)
		{
			m_clientLocker.AcquireWriterLock(-1);
			try
			{
				if (!m_consortias.ContainsKey(consortiaId))
				{
					return;
				}
				ConsortiaInfo info = m_consortias[consortiaId];
				int copyId = 50000 + info.callBossLevel;
				List<ItemInfo> list = null;
				DropInventory.CopyAllDrop(copyId, ref list);
				int riches = 0;
				if (info.RankList == null)
				{
					return;
				}
				foreach (RankingPersonInfo info2 in info.RankList.Values)
				{
					if (info.IsBossDie)
					{
						string title = "Recompensas chefe da guilda";
						if (list != null)
						{
							WorldEventMgr.SendItemsToMail(list, info2.UserID, info2.Name, title);
						}
						else
						{
							Console.WriteLine("Boss Guild award error dropID {0} ", copyId);
						}
					}
					riches += info2.Damage;
				}
				using (ConsortiaBussiness bussiness = new ConsortiaBussiness())
				{
					bussiness.ConsortiaRichAdd(consortiaId, ref riches, 5, "Boss Guild");
				}
			}
			finally
			{
				m_clientLocker.ReleaseWriterLock();
			}
		}

		public static void UpdateBlood(int consortiaId, int damage)
		{
			GSPacketIn packet = new GSPacketIn(186);
			packet.WriteInt(consortiaId);
			packet.WriteInt(damage);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}

		public static bool UpdateConsortia(ConsortiaInfo info)
		{
			m_clientLocker.AcquireWriterLock(-1);
			try
			{
				int consortiaID = info.ConsortiaID;
				if (m_consortias.ContainsKey(consortiaID))
				{
					m_consortias[consortiaID] = info;
				}
			}
			finally
			{
				m_clientLocker.ReleaseWriterLock();
			}
			return true;
		}

		public static void UpdateRank(int consortiaId, int damage, int richer, int honor, string Nickname, int userID)
		{
			GSPacketIn packet = new GSPacketIn(181);
			packet.WriteInt(consortiaId);
			packet.WriteInt(damage);
			packet.WriteInt(richer);
			packet.WriteInt(honor);
			packet.WriteString(Nickname);
			packet.WriteInt(userID);
			GameServer.Instance.LoginServer.SendPacket(packet);
		}
	}
}
