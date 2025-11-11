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
			bool flag1 = false;
			string str = "Fail!";
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
					PlayerInfo[] playerPage = playerBussiness.GetPlayerPage(num1, num2, ref num4, 7, num3, ref flag2);
					if (flag2)
					{
						int num5 = 1;
						int num6 = 1;
						XElement xelement1 = new XElement("ItemSet", new XAttribute("value", 1));
						XElement xelement2 = new XElement("ItemSet", new XAttribute("value", 2));
						PlayerInfo[] array = playerPage;
						foreach (PlayerInfo playerInfo in array)
						{
							if (playerInfo.Grade <= 40)
							{
								xelement1.Add(FlashUtils.CreateEliteMatchPlayersList(playerInfo, num5));
								num5++;
							}
							else
							{
								xelement2.Add(FlashUtils.CreateEliteMatchPlayersList(playerInfo, num6));
								num6++;
							}
						}
						result.Add(xelement1);
						result.Add(xelement2);
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
			result.Add(new XAttribute("lastUpdateTime", DateTime.Now.ToString()));
			return CreateCompressXml(result, file, isCompress: true);
		}

		public static string BuildCelebUsers(string file, int order, string fileNotCompress)
		{
			bool flag1 = false;
			string str = "Fail!";
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
					if (order == 6)
					{
						playerBussiness.UpdateUserReputeFightPower();
					}
					PlayerInfo[] playerPage = playerBussiness.GetPlayerPage(num1, num2, ref num4, order, num3, ref flag2);
					if (flag2)
					{
						PlayerInfo[] array = playerPage;
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
