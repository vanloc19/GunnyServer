using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Managers
{
	public class AwardMgr
	{
		private static List<DailyLeagueAwardInfo> _LeagueAward;

		private static Dictionary<int, DailyAwardInfo> _dailyAward;

		private static Dictionary<int, ThreeClearPointAwardInfo> _threeClearPointAwards;

		private static Dictionary<int, WorldBossTopTenAwardInfo> _worldBossAwards;

		private static bool _dailyAwardState;

		private static Dictionary<int, SearchGoodsTempInfo> _searchGoodsTemp;

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static ReaderWriterLock m_lock;

		public static bool DailyAwardState
		{
			get
			{
				return _dailyAwardState;
			}
			set
			{
				_dailyAwardState = value;
			}
		}
		
		public static bool AddEggAward(GamePlayer player)
        {
			if (DateTime.Now.Date != player.PlayerCharacter.LastGetEgg.Date)
            {
				player.PlayerCharacter.LastGetEgg = DateTime.Now;
            }
			return false;
        }

		public static bool AddDailyAward(GamePlayer player)
		{
			if (DateTime.Now.Date != player.PlayerCharacter.LastAward.Date)
			{
				player.PlayerCharacter.DayLoginCount++;
				player.PlayerCharacter.LastAward = DateTime.Now;
				DailyAwardInfo[] allAwardInfo = GetAllAwardInfo();
				foreach (DailyAwardInfo info in allAwardInfo)
				{
					if (info.Type == 0)
					{
						ItemTemplateInfo template = ItemMgr.FindItemTemplate(info.TemplateID);
						if (template != null)
						{
							BufferList.CreateBufferMinutes(template, info.ValidDate).Start(player);
							return true;
						}
					}
				}
			}
			return false;
		}

		public static bool AddSignAwards(GamePlayer player, int DailyLog)
		{
			GetAllAwardInfo();
			DailyAwardInfo[] singleAwardInfo = new ProduceBussiness().GetSingleDailyAward(DailyLog);
			new StringBuilder();
			string value = string.Empty;
			bool flag = false;
			int templateId = 0;
			int num = 1;
			int validDate = 0;
			bool isBinds = true;
			bool result = false;
			foreach (DailyAwardInfo singleAward in singleAwardInfo)
			{
				flag = true;
				if (singleAward.AwardDays == DailyLog && singleAward.Type == 7)
				{
					player.AddGiftToken(singleAward.Count);
					result = true;
				}
				switch (DailyLog)
				{
				case 6:
					if (singleAward.AwardDays == DailyLog && singleAward.Type != 7)
					{
						templateId = singleAward.TemplateID;
						num = singleAward.Count;
						validDate = singleAward.ValidDate;
						isBinds = singleAward.IsBinds;
						result = true;
					}
					break;
				case 3:
					if (singleAward.AwardDays == DailyLog && singleAward.Type != 7)
					{
						templateId = singleAward.TemplateID;
						num = singleAward.Count;
						validDate = singleAward.ValidDate;
						isBinds = singleAward.IsBinds;
						result = true;
					}
					break;
				case 18:
					if (singleAward.AwardDays == DailyLog && singleAward.Type != 7)
					{
						templateId = singleAward.TemplateID;
						num = singleAward.Count;
						validDate = singleAward.ValidDate;
						isBinds = singleAward.IsBinds;
						result = true;
					}
					break;
				case 12:
					if (singleAward.AwardDays == DailyLog && singleAward.Type != 7)
					{
						templateId = singleAward.TemplateID;
						num = singleAward.Count;
						validDate = singleAward.ValidDate;
						isBinds = singleAward.IsBinds;
						result = true;
					}
					break;
				}
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
				if (itemTemplateInfo == null)
				{
					continue;
				}
				int num2 = num;
				for (int i = 0; i < num2; i += itemTemplateInfo.MaxCount)
				{
					int count = ((i + itemTemplateInfo.MaxCount > num2) ? (num2 - i) : itemTemplateInfo.MaxCount);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 113);
					itemInfo.ValidDate = validDate;
					itemInfo.IsBinds = isBinds;
					if (!player.AddTemplate(itemInfo, itemInfo.Template.BagType, itemInfo.Count, eGameView.CaddyTypeGet))
					{
						flag = true;
						using (PlayerBussiness playerBussiness = new PlayerBussiness())
						{
							itemInfo.UserID = 0;
							playerBussiness.AddGoods(itemInfo);
							MailInfo mailInfo = new MailInfo();
							mailInfo.Annex1 = itemInfo.ItemID.ToString();
							mailInfo.Content = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Content", itemInfo.Template.Name);
							mailInfo.Gold = 0;
							mailInfo.Money = 0;
							mailInfo.Receiver = player.PlayerCharacter.NickName;
							mailInfo.ReceiverID = player.PlayerCharacter.ID;
							mailInfo.Sender = mailInfo.Receiver;
							mailInfo.SenderID = mailInfo.ReceiverID;
							mailInfo.Title = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Title", itemInfo.Template.Name);
							mailInfo.Type = 15;
							playerBussiness.SendMail(mailInfo);
							value = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Mail");
						}
					}
				}
			}
			if (flag && !string.IsNullOrEmpty(value))
			{
				player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			return result;
		}

		public static DailyAwardInfo[] GetAllAwardInfo()
		{
			DailyAwardInfo[] infoArray = null;
			m_lock.AcquireReaderLock(10000);
			try
			{
				infoArray = _dailyAward.Values.ToArray();
			}
			catch
			{
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			if (infoArray != null)
			{
				return infoArray;
			}
			return new DailyAwardInfo[0];
		}

		public static SearchGoodsTempInfo GetSearchGoodsTempInfo(int starId)
		{
			m_lock.AcquireReaderLock(10000);
			try
			{
				if (_searchGoodsTemp.ContainsKey(starId))
				{
					return _searchGoodsTemp[starId];
				}
			}
			catch
			{
			}
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			return null;
		}

		public static bool Init()
		{
			try
			{
				m_lock = new ReaderWriterLock();
				_dailyAward = new Dictionary<int, DailyAwardInfo>();
				_threeClearPointAwards = new Dictionary<int, ThreeClearPointAwardInfo>();
				_worldBossAwards = new Dictionary<int, WorldBossTopTenAwardInfo>();
				_searchGoodsTemp = new Dictionary<int, SearchGoodsTempInfo>();
				_dailyAwardState = false;
				_LeagueAward = new List<DailyLeagueAwardInfo>();
				return LoadDailyAward(_dailyAward, _searchGoodsTemp, _threeClearPointAwards, _worldBossAwards, _LeagueAward);
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("AwardMgr", exception);
				}
				return false;
			}
		}

		private static bool LoadDailyAward(Dictionary<int, DailyAwardInfo> awards, Dictionary<int, SearchGoodsTempInfo> searchGoodsTemp, Dictionary<int, ThreeClearPointAwardInfo> threeClearAward, Dictionary<int, WorldBossTopTenAwardInfo> worldBossAward, List<DailyLeagueAwardInfo> tempCamp)
		{
			using (ProduceBussiness bussiness = new ProduceBussiness())
			{
				DailyAwardInfos = bussiness.GetAllDailyAward().ToList();
				foreach (DailyAwardInfo info in bussiness.GetAllDailyAward())
				{
					if (!awards.ContainsKey(info.ID))
					{
						awards.Add(info.ID, info);
					}
				}
				SearchGoodsTempInfo[] allSearchGoodsTemp = bussiness.GetAllSearchGoodsTemp();
				foreach (SearchGoodsTempInfo info2 in allSearchGoodsTemp)
				{
					if (!searchGoodsTemp.ContainsKey(info2.StarID))
					{
						searchGoodsTemp.Add(info2.StarID, info2);
					}
				}
				ThreeClearPointAwardInfo[] listThreeClears = bussiness.GetThreeCleanPointAwards();
				foreach(ThreeClearPointAwardInfo info3 in listThreeClears)
                {
					if(!threeClearAward.ContainsKey(info3.ID))
                    {
						threeClearAward.Add(info3.ID, info3);
                    }
                }
				WorldBossTopTenAwardInfo[] listWorldBoss = bussiness.GetAllWorldbossTopTenAward();
				foreach(WorldBossTopTenAwardInfo info4 in listWorldBoss)
                {
					if (!worldBossAward.ContainsKey(info4.ID))
                    {
						worldBossAward.Add(info4.ID, info4);
                    }
                }
				DailyLeagueAwardInfo[] infos = bussiness.GetAllDailyLeagueAwardInfo();
				foreach(DailyLeagueAwardInfo info in infos)
                {
					tempCamp.Add(info);
                }
			}
			return true;
		}

		public static int MaxStar()
		{
			return _searchGoodsTemp.Count;
		}

		public static bool ReLoad()
		{
			try
			{
				Dictionary<int, DailyAwardInfo> awards = new Dictionary<int, DailyAwardInfo>();
				Dictionary<int, SearchGoodsTempInfo> searchGoodsTemp = new Dictionary<int, SearchGoodsTempInfo>();
				Dictionary<int, ThreeClearPointAwardInfo> tempThree = new Dictionary<int, ThreeClearPointAwardInfo>();
				Dictionary<int, WorldBossTopTenAwardInfo> tempWorld = new Dictionary<int, WorldBossTopTenAwardInfo>();
				List<DailyLeagueAwardInfo> tempLeague = new List<DailyLeagueAwardInfo>();
				if (LoadDailyAward(awards, searchGoodsTemp, tempThree, tempWorld, tempLeague))
				{
					m_lock.AcquireWriterLock(-1);
					try
					{
						_dailyAward = awards;
						_searchGoodsTemp = searchGoodsTemp;
						_threeClearPointAwards = tempThree;
						_worldBossAwards = tempWorld;
						_LeagueAward = tempLeague;
						return true;
					}
					catch
					{
					}
					finally
					{
						m_lock.ReleaseWriterLock();
					}
				}
			}
			catch (Exception exception)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("AwardMgr", exception);
				}
			}
			return false;
		}

		public static DailyLeagueAwardInfo[] GetLeagueAwardWithID(int level, int type)
		{
			List<DailyLeagueAwardInfo> list = new List<DailyLeagueAwardInfo>();
			m_lock.AcquireReaderLock(-1);
			try
			{
				foreach (DailyLeagueAwardInfo value in _LeagueAward)
				{
					if (value.Level == level && value.Class == type)
					{
						list.Add(value);
					}
				}
			}
			catch
			{ }
			finally
			{
				m_lock.ReleaseReaderLock();
			}
			return list.ToArray();
		}

		public static ThreeClearPointAwardInfo FindThreeClearPointAward(int ID)
		{
			if (_threeClearPointAwards.ContainsKey(ID))
				return _threeClearPointAwards[ID];
			return null;
		}

		public static WorldBossTopTenAwardInfo GetWorldBossAwardByID(int Type)
        {
			if (_worldBossAwards.ContainsKey(Type))
				return _worldBossAwards[Type];
			return null;
        }

		public static List<DailyAwardInfo> DailyAwardInfos;

		public static List<DailyAwardInfo> FindDailyAward(int GetWay)
		{
			return DailyAwardInfos.FindAll(q => q.GetWay == GetWay);
		}
	}
}
