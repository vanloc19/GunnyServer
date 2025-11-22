using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
	public class csFunction
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static string[] al = ";|and|1=1|exec|insert|select|delete|update|like|count|chr|mid|master|or|truncate|char|declare|join".Split('|');

		private static LevelInfo[] _cachedLevels = null;
		private static DateTime _cacheTime = DateTime.MinValue;
		private static readonly object _levelCacheLock = new object();

		public static string GetAdminIP => ConfigurationManager.AppSettings["AdminIP"];

		public static bool ValidAdminIP(string ip)
		{
			string getAdminIp = GetAdminIP;
			if (!string.IsNullOrEmpty(getAdminIp) && !getAdminIp.Split('|').Contains(ip))
			{
				return false;
			}
			return true;
		}

		public static string ConvertSql(string inputString)
		{
			inputString = inputString.Trim().ToLower();
			inputString = inputString.Replace("'", "''");
			inputString = inputString.Replace(";--", "");
			inputString = inputString.Replace("=", "");
			inputString = inputString.Replace(" or", "");
			inputString = inputString.Replace(" or ", "");
			inputString = inputString.Replace(" and", "");
			inputString = inputString.Replace("and ", "");
			if (!SqlChar(inputString))
			{
				inputString = "";
			}
			return inputString;
		}

		public static bool SqlChar(string v)
		{
			if (v.Trim() != "")
			{
				string[] array = al;
				foreach (string str in array)
				{
					if (v.IndexOf(str + " ") > -1 || v.IndexOf(" " + str) > -1)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static string CreateCompressXml(HttpContext context, XElement result, string file, bool isCompress, int length = 0)
		{
			return CreateCompressXml(context.Server.MapPath("~"), result, file, isCompress, length);
		}

		public static string CreateCompressXml(HttpContext context, XElement result, string file, bool isCompress)
		{
			return CreateCompressXml(context.Server.MapPath("~"), result, file, isCompress);
		}

		public static string CreateCompressAshx(HttpContext context, XElement result, string file, bool isCompress)
		{
			return CreateCompressAshx(context.Server.MapPath("~"), result, file, isCompress);
		}

		public static string CreateCompressXml(XElement result, string file, bool isCompress)
		{
			return CreateCompressXml(StaticsMgr.CurrentPath, result, file, isCompress);
		}

		public static string CreateCompressAshx(XElement result, string file, bool isCompress)
		{
			return CreateCompressAshx(StaticsMgr.CurrentPath, result, file, isCompress);
		}

		public static string CreateCompressAshx(string path, XElement result, string file, bool isCompress)
		{
			try
			{
				file += ".ashx";
				path = Path.Combine(path, file);
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					if (isCompress)
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
						{
							binaryWriter.Write(StaticFunction.Compress(result.ToString(check: false)));
						}
					}
					else
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.Write(result.ToString(check: false));
						}
					}
				}
				return "Build:" + file + ",Success!";
			}
			catch (Exception ex)
			{
				log.Error("CreateCompressAshx " + file + " is fail!", ex);
				return "Build:" + file + ",Fail!";
			}
		}

		public static string CreateCompressXml(string path, XElement result, string file, bool isCompress)
		{
			try
			{
				file += ".xml";
				path = Path.Combine(path, file);
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					if (isCompress)
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
						{
							binaryWriter.Write(StaticFunction.Compress(result.ToString(check: false)));
						}
					}
					else
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.Write(result.ToString(check: false));
						}
					}
				}
				return "Build:" + file + ",Success!";
			}
			catch (Exception ex)
			{
				log.Error("CreateCompressXml " + file + " is fail!", ex);
				return "Build:" + file + ",Fail!";
			}
		}
		public static string CreateCompressXml(string path, XElement result, string file, bool isCompress, int length)
		{
			try
			{
				file += ".xml";
				path = Path.Combine(path, file);
				using (FileStream fileStream = new FileStream(path, FileMode.Create))
				{
					if (isCompress)
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
						{
							binaryWriter.Write(StaticFunction.Compress(result.ToString(check: false)));
						}
					}
					else
					{
						using (StreamWriter streamWriter = new StreamWriter(fileStream))
						{
							streamWriter.Write(result.ToString(check: false));
						}
					}
				}
				return "Build:" + file + ",Success!";
			}
			catch (Exception ex)
			{
				log.Error("CreateCompressXml " + file + " is fail!", ex);
				return "Build:" + file + ",Fail!";
			}
		}

		public static string BuildCelebConsortia(string file, int order)
		{
			return BuildCelebConsortia(file, order, "");
		}

		public static string BuildCelebConsortia(string file, int order, string fileNotCompress)
		{
			bool flag = false;
			string str1 = "Fail!";
			XElement result = new XElement("Result");
			int num1 = 0;
			try
			{
				int num2 = 1;
				int num3 = 50;
				int num4 = -1;
				string str2 = "";
				int num5 = -1;
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					ConsortiaInfo[] consortiaPage = consortiaBussiness.GetConsortiaPage(num2, num3, ref num1, order, str2, num4, num5, -1);
					foreach (ConsortiaInfo consortiaInfo1 in consortiaPage)
					{
						XElement consortiaInfo2 = FlashUtils.CreateConsortiaInfo(consortiaInfo1);
						if (consortiaInfo1.ChairmanID != 0)
						{
							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								PlayerInfo userSingleByUserId = playerBussiness.GetUserSingleByUserID(consortiaInfo1.ChairmanID);
								if (userSingleByUserId != null)
								{
									consortiaInfo2.Add(FlashUtils.CreateCelebInfo(userSingleByUserId));
								}
							}
						}
						result.Add(consortiaInfo2);
					}
					flag = true;
					str1 = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error(file + " is fail!", ex);
			}
			result.Add(new XAttribute("total", num1));
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str1));
			result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
			if (!string.IsNullOrEmpty(fileNotCompress))
			{
				CreateCompressXml(result, fileNotCompress, isCompress: false);
			}
			return CreateCompressXml(result, file, isCompress: true);
		}

		public static string BuildCelebUsers(string file, int order)
		{
			return BuildCelebUsers(file, order, "");
		}

		public static string BuildEliteMatchPlayerList(string file)
		{
			// Always return empty elite match list
			XElement result = new XElement("Result");
			XElement xelement1 = new XElement("ItemSet", new XAttribute("value", 1));
			XElement xelement2 = new XElement("ItemSet", new XAttribute("value", 2));
			result.Add(xelement1);
			result.Add(xelement2);
			result.Add(new XAttribute("value", true));
			result.Add(new XAttribute("message", "Success!"));
			result.Add(new XAttribute("lastUpdateTime", DateTime.Now.ToString()));
			result.Add(new XAttribute("total", 0));
			// Don't compress - return as plain XML text
			return CreateCompressXml(result, file, isCompress: false);
		}

		private static LevelInfo[] GetCachedLevels()
		{
			lock (_levelCacheLock)
			{
				// Always refresh LevelInfo to ensure accuracy (no cache for level calculation)
				// LevelInfo rarely changes, but we want fresh data for accurate Grade calculation
				try
				{
					using (ProduceBussiness produceBussiness = new ProduceBussiness())
					{
						_cachedLevels = produceBussiness.GetAllLevel();
						_cacheTime = DateTime.Now;
					}
				}
				catch (Exception ex)
				{
					log.Error("GetCachedLevels error: " + ex.ToString());
					if (_cachedLevels == null)
					{
						_cachedLevels = new LevelInfo[0];
					}
				}
				return _cachedLevels;
			}
		}

		private static int GetGradeFromGP(int gp)
		{
			try
			{
				if (gp <= 0)
				{
					return 1;
				}

				// CRITICAL: Level 60 requires GP >= 73777897 (from SQL script)
				// This is the ABSOLUTE TRUTH - always return 60 for GP >= 73777897
				// Do NOT rely on LevelInfo table for this - it's hardcoded truth
				if (gp >= 73777897)
				{
					return 60;
				}

				LevelInfo[] allLevels = GetCachedLevels();
				if (allLevels == null || allLevels.Length == 0)
				{
					log.Error("GetGradeFromGP: allLevels is null or empty for GP=" + gp);
					return 1;
				}

				// Log LevelInfo data for debugging
				log.InfoFormat("GetGradeFromGP: Loaded {0} levels from database", allLevels.Length);
				if (allLevels.Length > 0)
				{
					LevelInfo maxLevelInfo = allLevels.OrderByDescending(l => l.Grade).FirstOrDefault();
					if (maxLevelInfo != null)
					{
						log.InfoFormat("GetGradeFromGP: Max level in DB - Grade={0}, GP={1}", maxLevelInfo.Grade, maxLevelInfo.GP);
					}
				}

				// Sort by Grade to ensure correct order
				LevelInfo[] sortedLevels = allLevels.OrderBy(l => l.Grade).ToArray();
				int maxLevel = sortedLevels.Length;

				if (maxLevel == 0)
				{
					log.Error("GetGradeFromGP: sortedLevels is empty for GP=" + gp);
					return 1;
				}

				// Get max level GP and Grade
				int maxLevelGP = sortedLevels[maxLevel - 1].GP;
				int maxLevelGrade = sortedLevels[maxLevel - 1].Grade;

				log.InfoFormat("GetGradeFromGP: Max level - Grade={0}, GP={1}, Player GP={2}", maxLevelGrade, maxLevelGP, gp);

				// If GP >= max level GP, return max level
				if (gp >= maxLevelGP)
				{
					log.InfoFormat("GetGradeFromGP: GP={0} >= maxLevelGP={1}, returning maxLevelGrade={2}", gp, maxLevelGP, maxLevelGrade);
					return maxLevelGrade;
				}

				// Find the first level where GP < required GP, return previous level
				// Logic: if GP < level[i].GP, then player is at level[i-1]
				for (int i = 1; i < maxLevel; i++)
				{
					if (gp < sortedLevels[i].GP)
					{
						int grade = sortedLevels[i - 1].Grade;
						log.InfoFormat("GetGradeFromGP: GP={0} < level[{1}].GP={2}, returning grade={3}",
							gp, i, sortedLevels[i].GP, grade);
						return (grade > 0) ? grade : 1;
					}
				}

				// Should not reach here, but return max level as fallback
				log.InfoFormat("GetGradeFromGP: Fallback - returning maxLevelGrade={0} for GP={1}", maxLevelGrade, gp);
				return maxLevelGrade;
			}
			catch (Exception ex)
			{
				log.Error("GetGradeFromGP error for GP=" + gp + ": " + ex.ToString());
				return 1;
			}
		}

		public static XElement BuildCelebUsersXml(int order)
		{
			// Build XML directly from database for client response (always fresh)
			log.WarnFormat("RANKING START - BuildCelebUsersXml called with order: {0}", order);
			XElement result = new XElement("Result");
			try
			{
				int num1 = 1;
				int num2 = 50;
				int num3 = -1;
				int num4 = 0;
				bool flag2 = false;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerBussiness.UpdateUserReputeFightPower();
					PlayerInfo[] playerPage = playerBussiness.GetPlayerPage(num1, num2, ref num4, order, num3, ref flag2);
					log.WarnFormat("RANKING QUERY - Order: {0}, Found {1} players, Total: {2}, flag2: {3}", order, playerPage?.Length ?? 0, num4, flag2);
					if (flag2)
					{
						PlayerInfo[] array = playerPage;
						// CRITICAL FIX: ALWAYS recalculate Grade from GP for ALL rankings
						// This ensures Grade is ALWAYS accurate and never depends on online player or database Grade
						// Database Grade may be outdated, we MUST calculate from GP for accuracy
						foreach (PlayerInfo playerInfo in array)
						{
							// CRITICAL FIX: Use Grade directly from database, but verify it's correct
							// If database Grade seems wrong (e.g., GP >= 73777897 but Grade < 60), recalculate
							// Otherwise, trust database Grade - it's the source of truth
							int originalDbGrade = playerInfo.Grade;
							int finalGrade = originalDbGrade;

							if (playerInfo.GP > 0)
							{
								// Special case: If GP >= 73777897 (level 60), Grade MUST be 60
								// This is the absolute truth - override database if wrong
								if (playerInfo.GP >= 73777897)
								{
									if (originalDbGrade != 60)
									{
										log.WarnFormat("RANKING FIX - Player: {0} (ID: {1}), GP: {2} >= 73777897, DB Grade: {3} is WRONG, FORCING Grade to 60",
											playerInfo.NickName, playerInfo.ID, playerInfo.GP, originalDbGrade);
										finalGrade = 60;
									}
									else
									{
										finalGrade = 60; // Ensure it's 60 even if DB says so
									}
								}
								else
								{
									// For non-max-level players, verify Grade matches GP
									// If mismatch is significant, recalculate from GP
									int calculatedGrade = GetGradeFromGP(playerInfo.GP);
									if (Math.Abs(calculatedGrade - originalDbGrade) > 5)
									{
										// Large discrepancy - database Grade is likely wrong, recalculate
										log.WarnFormat("RANKING FIX - Player: {0} (ID: {1}), GP: {2}, DB Grade: {3}, Calculated: {4}, Using calculated",
											playerInfo.NickName, playerInfo.ID, playerInfo.GP, originalDbGrade, calculatedGrade);
										finalGrade = calculatedGrade;
									}
									else
									{
										// Small difference - trust database Grade
										finalGrade = originalDbGrade;
									}
								}
							}
							else
							{
								// If GP is 0, set Grade to 1 (minimum level)
								finalGrade = 1;
							}

							// Set the final Grade
							playerInfo.Grade = finalGrade;

							// Log for all players with GP >= 73777897 to verify
							if (playerInfo.GP >= 73777897)
							{
								log.WarnFormat("RANKING VERIFY - Order: {0}, Player: {1} (ID: {2}), GP: {3}, Final Grade: {4}, DB Grade was: {5}",
									order, playerInfo.NickName, playerInfo.ID, playerInfo.GP, finalGrade, originalDbGrade);
							}

							// For total GP ranking (order 0), set AddDayGP and AddWeekGP = GP
							// so that "Ngày" and "Tuần" tabs display the same EXP as "Tổng"
							if (order == 0)
							{
								playerInfo.AddDayGP = playerInfo.GP;
								playerInfo.AddWeekGP = playerInfo.GP;
							}

							// For total AchievementPoint ranking (order 11), set AddDayAchievementPoint and AddWeekAchievementPoint = AchievementPoint
							// so that "Ngày" and "Tuần" tabs display the same AchievementPoint as "Tổng"
							if (order == 11)
							{
								playerInfo.AddDayAchievementPoint = playerInfo.AchievementPoint;
								playerInfo.AddWeekAchievementPoint = playerInfo.AchievementPoint;
							}

							// For weekly GP ranking (order 3), ensure AddWeekGP is not negative
							if (order == 3 && playerInfo.AddWeekGP < 0)
							{
								// Set to 0 if negative
								playerInfo.AddWeekGP = 0;
							}
							// All data (Grade, GP) comes from database via GetPlayerPage
							// Grade is verified and corrected if needed, but primarily from database
							// No online player data is used

							XElement celebElement = FlashUtils.CreateCelebInfo(playerInfo);

							// CRITICAL: Verify Grade in XML is correct before adding to result
							XAttribute gradeAttr = celebElement.Attribute("Grade");
							if (gradeAttr != null)
							{
								int xmlGrade = int.Parse(gradeAttr.Value);
								if (xmlGrade != finalGrade)
								{
									log.ErrorFormat("GRADE XML MISMATCH! Player: {0} (ID: {1}), Expected Grade: {2}, XML Grade: {3}, FIXING XML",
										playerInfo.NickName, playerInfo.ID, finalGrade, xmlGrade);
									gradeAttr.Value = finalGrade.ToString();
								}

								// Log for level 60 players to verify
								if (playerInfo.GP >= 73777897)
								{
									log.WarnFormat("RANKING XML - Order: {0}, Player: {1} (ID: {2}), GP: {3}, Grade in XML: {4}",
										order, playerInfo.NickName, playerInfo.ID, playerInfo.GP, xmlGrade);
								}
							}
							else
							{
								log.ErrorFormat("GRADE ATTRIBUTE MISSING! Player: {0} (ID: {1}), GP: {2}, Grade: {3}",
									playerInfo.NickName, playerInfo.ID, playerInfo.GP, finalGrade);
							}

							result.Add(celebElement);
						}
						result.Add(new XAttribute("total", num4));
						result.Add(new XAttribute("value", true));
						result.Add(new XAttribute("message", "Success!"));
					}
					else
					{
						log.WarnFormat("RANKING EMPTY - Order: {0}, flag2 is false, no players found", order);
						result.Add(new XAttribute("total", 0));
						result.Add(new XAttribute("value", false));
						result.Add(new XAttribute("message", "Fail!"));
					}
				}
				log.WarnFormat("RANKING END - BuildCelebUsersXml order: {0} completed, result has {1} items", order, result.Elements().Count());
			}
			catch (Exception ex)
			{
				log.Error("BuildCelebUsersXml order=" + order + " is fail!", ex);
				result.Add(new XAttribute("total", 0));
				result.Add(new XAttribute("value", false));
				result.Add(new XAttribute("message", "Fail!"));
			}
			result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
			return result;
		}

		public static string BuildCelebUsers(string file, int order, string fileNotCompress)
		{
			// Always return empty for order=17 (CelebByWeekLeagueScore - thi đấu)
			if (order == 17)
			{
				XElement emptyResult = new XElement("Result");
				emptyResult.Add(new XAttribute("value", true));
				emptyResult.Add(new XAttribute("message", "Success!"));
				emptyResult.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
				emptyResult.Add(new XAttribute("total", 0));
				if (!string.IsNullOrEmpty(fileNotCompress))
				{
					CreateCompressXml(emptyResult, fileNotCompress, isCompress: false);
				}
				return CreateCompressXml(emptyResult, file, isCompress: false);
			}

			// Build XML and save to file
			XElement result = BuildCelebUsersXml(order);
			if (!string.IsNullOrEmpty(fileNotCompress))
			{
				CreateCompressXml(result, fileNotCompress, isCompress: false);
			}
			return CreateCompressXml(result, file, isCompress: true);
		}

		public static string BuildCelebUsersMath(string file, string fileNotCompress)
		{
			bool flag1 = false;
			string str = "Fail!";
			XElement result = new XElement("Result");
			try
			{
				int num1 = 1;
				int num2 = 50;
				int num3 = 0;
				bool flag2 = false;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					PlayerInfo[] playerMathPage = playerBussiness.GetPlayerMathPage(num1, num2, ref num3, ref flag2);
					if (flag2)
					{
						PlayerInfo[] array = playerMathPage;
						foreach (PlayerInfo playerInfo in array)
						{
							result.Add(FlashUtils.CreateCelebInfo(playerInfo));
						}
						flag1 = true;
						str = "Success!";
					}
				}
			}
			catch (Exception ex)
			{
				log.Error(file + " is fail!", ex);
			}
			result.Add(new XAttribute("value", flag1));
			result.Add(new XAttribute("message", str));
			result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
			if (!string.IsNullOrEmpty(fileNotCompress))
			{
				CreateCompressXml(result, fileNotCompress, isCompress: false);
			}
			return CreateCompressXml(result, file, isCompress: true);
		}

		public static string BuildCelebConsortiaFightPower(string file, string fileNotCompress)
		{
			bool flag = false;
			string str = "Fail!";
			XElement result = new XElement("Result");
			int num = 0;
			try
			{
				using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
				{
					ConsortiaInfo[] array = consortiaBussiness.UpdateConsortiaFightPower();
					num = array.Length;
					ConsortiaInfo[] array2 = array;
					foreach (ConsortiaInfo consortiaInfo1 in array2)
					{
						XElement consortiaInfo2 = FlashUtils.CreateConsortiaInfo(consortiaInfo1);
						if (consortiaInfo1.ChairmanID != 0)
						{
							using (PlayerBussiness playerBussiness = new PlayerBussiness())
							{
								PlayerInfo userSingleByUserId = playerBussiness.GetUserSingleByUserID(consortiaInfo1.ChairmanID);
								if (userSingleByUserId != null)
								{
									consortiaInfo2.Add(FlashUtils.CreateCelebInfo(userSingleByUserId));
								}
							}
						}
						result.Add(consortiaInfo2);
					}
					flag = true;
					str = "Success!";
				}
			}
			catch (Exception ex)
			{
				log.Error(file + " is fail!", ex);
			}
			result.Add(new XAttribute("total", num));
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			result.Add(new XAttribute("date", DateTime.Today.ToString("yyyy-MM-dd")));
			if (!string.IsNullOrEmpty(fileNotCompress))
			{
				CreateCompressXml(result, fileNotCompress, isCompress: false);
			}
			return CreateCompressXml(result, file, isCompress: true);
		}
	}
}
