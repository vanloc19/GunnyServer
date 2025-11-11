using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Server.ConsortiaTask.Data;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.ConsortiaTask
{
	public class ConsortiaTaskMgr
	{
		private static Dictionary<int, ConsortiaTaskInfo> dictionary_0;

		private static Dictionary<int, BaseConsortiaTask> dictionary_1;

		private static Dictionary<int, ConsortiaTaskUserTempInfo> dictionary_2;

		private static object object_0;

		private static ThreadSafeRandom threadSafeRandom_0;

		private static readonly int[] int_0;

		private static readonly int[] int_1;

		private static readonly int[] int_2;

		private static readonly int[] int_3;

		public static bool Init()
		{
			dictionary_0 = new Dictionary<int, ConsortiaTaskInfo>();
			dictionary_1 = new Dictionary<int, BaseConsortiaTask>();
			dictionary_2 = new Dictionary<int, ConsortiaTaskUserTempInfo>();
			threadSafeRandom_0 = new ThreadSafeRandom();
			smethod_0();
			smethod_1();
			return true;
		}

		public static void ScanConsortiaTask()
		{
			lock (object_0)
			{
				ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
				List<BaseConsortiaTask> consortiaTaskData = ConsortiaTaskMgr.GetAllConsortiaTaskData();
				for (int index = 0; index < consortiaTaskData.Count; ++index)
				{
					bool flag = consortiaTaskData[index].Info.CanRemove;// || consortiaTaskData[index].Finish();
					ConsortiaInfo consortiaSingle = consortiaBussiness.GetConsortiaSingle(consortiaTaskData[index].Info.ConsortiaID);
					if (!flag && (DateTime.Now - consortiaBussiness.GetConsortiaSingle(consortiaTaskData[index].Info.ConsortiaID).DateOpenTask).TotalMinutes > 360.0)
						flag = true;
					if (flag)
					{
						foreach (KeyValuePair<int, ConsortiaTaskUserDataInfo> listUser in consortiaTaskData[index].ListUsers)
						{
							if (listUser.Value.Player != null)
								listUser.Value.Player.Out.SendConsortiaTaskInfo((BaseConsortiaTask)null);
						}
						consortiaTaskData[index].ClearAllUserData();
						ConsortiaTaskMgr.RemoveConsortiaTask(consortiaTaskData[index].Info.ConsortiaID);
						consortiaTaskData[index].SendSystemConsortiaChat("Hết thời gian, nhiệm vụ sứ mệnh Guild thất bại");
						consortiaTaskData[index] = (BaseConsortiaTask)null;
					}
				}
			}
		}

		public static bool AddConsortiaTask(int consortiaId, int level, bool ResetTask)
		{
			bool flag = false;
			BaseConsortiaTask baseConsortiaTask1 = GetSingleConsortiaTask(consortiaId);
			if (baseConsortiaTask1 != null && (baseConsortiaTask1.Info.CanRemove || ResetTask))
			{
				if(ResetTask)
                {
					baseConsortiaTask1.ClearAllUserData();
                }
				RemoveConsortiaTask(consortiaId);
				baseConsortiaTask1 = null;
			}
			if (baseConsortiaTask1 == null)
			{
				BaseConsortiaTask baseConsortiaTask2 = new BaseConsortiaTask(new ConsortiaTaskDataInfo(consortiaId, int_1[level - 1], int_0[level - 1], int_2[level - 1], GetRandomConsortiaBuff(level).id, GameProperties.MissionMinute), GetRandomTaskCondition(level, 3));
				lock (dictionary_1)
				{
					dictionary_1.Add(consortiaId, baseConsortiaTask2);
				}
				flag = true;
			}
			return flag;
		}

		public static void AddConsortiaTask(BaseConsortiaTask taskBase)
		{
			lock (dictionary_1)
			{
				dictionary_1.Add(taskBase.Info.ConsortiaID, taskBase);
			}
		}

		//Consortia Active Task
		public static bool ActiveTask(int consortiaId)
		{
			BaseConsortiaTask singleConsortiaTask = GetSingleConsortiaTask(consortiaId);
			if (singleConsortiaTask == null || singleConsortiaTask.Info.IsActive)
			{
				return false;
			}
			singleConsortiaTask.Info.IsActive = true;
			singleConsortiaTask.Info.StartTime = DateTime.Now;
			GamePlayer[] allPlayersWithConsortia = WorldMgr.GetAllPlayersWithConsortia(consortiaId);
			DateTime now = DateTime.Now;
			foreach (GamePlayer playersWithConsortium in allPlayersWithConsortia)
			{
				if (playersWithConsortium.IsActive && playersWithConsortium.PlayerCharacter.DateFinishTask.Date < now.Date)
				{
					singleConsortiaTask.AddToPlayer(playersWithConsortium);
					Console.WriteLine("Add ConsotiaTask to player: " + playersWithConsortium.PlayerCharacter.NickName);
                }
			}
			singleConsortiaTask.SendSystemConsortiaChat(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg4"));
			return true;
		}

		public static bool AddConsortiaTaskUserTemp(ConsortiaTaskUserTempInfo temp)
		{
			bool flag = false;
			lock (dictionary_2)
			{
				if (!dictionary_2.ContainsKey(temp.UserID))
				{
					dictionary_2.Add(temp.UserID, temp);
					return true;
				}
				return flag;
			}
		}

		public static bool CheckConsortiaTaskUserTemp(GamePlayer player)
		{
			bool flag = false;
			ConsortiaTaskUserTempInfo taskUserTempInfo = null;
			lock (dictionary_2)
			{
				if (dictionary_2.ContainsKey(player.PlayerId))
				{
					taskUserTempInfo = dictionary_2[player.PlayerId];
				}
			}
			if (taskUserTempInfo != null)
			{
				player.AddGP(taskUserTempInfo.Exp, false, false);
				player.AddOffer(taskUserTempInfo.Offer);
				player.SendMessage(LanguageMgr.GetTranslation("GameServer.ConsortiaTask.msg3", taskUserTempInfo.Total, taskUserTempInfo.Exp, taskUserTempInfo.Offer));
				flag = true;
				lock (dictionary_2)
				{
					dictionary_2.Remove(taskUserTempInfo.UserID);
					return flag;
				}
			}
			return flag;
		}

		public static List<ConsortiaTaskInfo> GetRandomTaskCondition(int level, int total)
		{
			List<ConsortiaTaskInfo> consortiaTaskInfoList = new List<ConsortiaTaskInfo>();
			List<ConsortiaTaskInfo> allConditionInfo = GetAllConditionInfo(level);
			for (int index1 = 0; index1 < total; index1++)
			{
				int index2 = threadSafeRandom_0.Next(allConditionInfo.Count);
				if (index2 < allConditionInfo.Count)
				{
					consortiaTaskInfoList.Add(allConditionInfo[index2]);
					allConditionInfo.RemoveAt(index2);
				}
			}
			return consortiaTaskInfoList;
		}

		public static ConsortiaBuffTempInfo GetRandomConsortiaBuff(int level)
		{
			List<ConsortiaBuffTempInfo> allConsortiaBuff = ConsortiaExtraMgr.GetAllConsortiaBuff(level, 1);
			int index = threadSafeRandom_0.Next(allConsortiaBuff.Count);
			return allConsortiaBuff[index];
		}

		public static List<ConsortiaTaskInfo> GetAllConditionInfo(int level)
		{
			List<ConsortiaTaskInfo> consortiaTaskInfoList = new List<ConsortiaTaskInfo>();
			lock (dictionary_0)
			{
				foreach (ConsortiaTaskInfo consortiaTaskInfo in dictionary_0.Values)
				{
					if (consortiaTaskInfo.Level == level)
					{
						consortiaTaskInfoList.Add(consortiaTaskInfo);
					}
				}
				return consortiaTaskInfoList;
			}
		}

		public static List<BaseConsortiaTask> GetAllConsortiaTaskData()
		{
			List<BaseConsortiaTask> baseConsortiaTaskList = new List<BaseConsortiaTask>();
			lock (dictionary_1)
			{
				foreach (BaseConsortiaTask baseConsortiaTask in dictionary_1.Values)
				{
					baseConsortiaTaskList.Add(baseConsortiaTask);
				}
				return baseConsortiaTaskList;
			}
		}

		public static BaseConsortiaTask GetSingleConsortiaTask(int consortiaId)
		{
			BaseConsortiaTask baseConsortiaTask = null;
			lock (dictionary_1)
			{
				if (dictionary_1.ContainsKey(consortiaId))
				{
					return dictionary_1[consortiaId];
				}
				return baseConsortiaTask;
			}
		}

		public static void RemoveConsortiaTask(int consortiaId)
		{
			lock (dictionary_1)
			{
				if (dictionary_1.ContainsKey(consortiaId))
				{
					dictionary_1.Remove(consortiaId);
				}
			}
		}

		public static void AddPlayer(GamePlayer player)
		{
			GetSingleConsortiaTask(player.PlayerCharacter.ConsortiaID)?.AddToPlayer(player);
		}

		public static void RemovePlayer(GamePlayer player)
		{
			GetSingleConsortiaTask(player.PlayerCharacter.ConsortiaID)?.RemoveToPlayer(player);
		}

		private static void smethod_0()
		{
			dictionary_0.Clear();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				ConsortiaTaskInfo[] allConsortiaTask = produceBussiness.GetAllConsortiaTask();
				foreach (ConsortiaTaskInfo consortiaTaskInfo in allConsortiaTask)
				{
					if (!dictionary_0.ContainsKey(consortiaTaskInfo.ID))
					{
						dictionary_0.Add(consortiaTaskInfo.ID, consortiaTaskInfo);
					}
				}
			}
		}

		private static void smethod_1()
		{
			ConsortiaTaskMgrProtobuf consortiaTaskMgrProtobuf = Marshal.LoadDataFile<ConsortiaTaskMgrProtobuf>("consortiatask", isEncrypt: true);
			if (consortiaTaskMgrProtobuf == null || consortiaTaskMgrProtobuf.tempUsers == null)
			{
				return;
			}
			foreach (ConsortiaTaskUserTempInfo tempUser in consortiaTaskMgrProtobuf.tempUsers)
			{
				AddConsortiaTaskUserTemp(tempUser);
			}
		}

		private static void smethod_2()
		{
			Marshal.SaveDataFile(new ConsortiaTaskMgrProtobuf
			{
				tempUsers = dictionary_2.Values.ToList()
			}, "consortiatask", isEncrypt: true);
		}

		public static void Stop()
		{
			ScanConsortiaTask();
			smethod_2();
		}

		static ConsortiaTaskMgr()
		{
			object_0 = new object();
			int_0 = GameProperties.MissionAwardRichesArr();
			int_1 = GameProperties.MissionAwardGPArr();
			int_2 = GameProperties.MissionAwardOfferArr();
			int_3 = GameProperties.MissionRichesArr();
		}
	}
}
