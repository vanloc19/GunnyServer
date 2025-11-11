using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using Bussiness;
using Bussiness.Interface;
using log4net;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class SentReward : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static string GetSentRewardIP => ConfigurationManager.AppSettings["SentRewardIP"];

		public static string GetSentRewardKey => ConfigurationManager.AppSettings["SentRewardKey"];

		public bool IsReusable => false;

		public static bool ValidSentRewardIP(string ip)
		{
			string getSentRewardIp = GetSentRewardIP;
			if (!string.IsNullOrEmpty(getSentRewardIp) && !getSentRewardIp.Split('|').Contains(ip))
			{
				return false;
			}
			return true;
		}

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";
			try
			{
				int num = 1;
				if (ValidSentRewardIP(context.Request.UserHostAddress))
				{
					string str1 = HttpUtility.UrlDecode(context.Request["content"]);
					string getSentRewardKey = GetSentRewardKey;
					string[] strArray = BaseInterface.CreateInterface().UnEncryptSentReward(str1, ref num, getSentRewardKey);
					if (strArray.Length == 8 && num != 5 && num != 6 && num != 7)
					{
						_ = strArray[0];
						_ = strArray[1];
						_ = strArray[2];
						int.Parse(strArray[3]);
						int.Parse(strArray[4]);
						string str2 = strArray[5];
						if (checkParam(ref str2))
						{
							new PlayerBussiness();
						}
						else
						{
							num = 4;
						}
					}
				}
				else
				{
					num = 3;
				}
				context.Response.Write(num);
			}
			catch (Exception ex)
			{
				log.Error("SentReward", ex);
			}
		}

		private bool checkParam(ref string param)
		{
			int num1 = 0;
			string str1 = "1";
			int num2 = 9;
			int num3 = 0;
			string str2 = "0";
			string str3 = "10";
			string str4 = "20";
			string str5 = "30";
			string str6 = "40";
			string str7 = "1";
			string str8 = "0";
			if (!string.IsNullOrEmpty(param))
			{
				string[] strArray1 = param.Split('|');
				int length = strArray1.Length;
				if (length > 0)
				{
					param = "";
					int index1 = 0;
					string[] array = strArray1;
					foreach (string str9 in array)
					{
						char[] chArray = new char[1]
						{
							','
						};
						string[] strArray2 = str9.Split(chArray);
						if (strArray2.Length != 0)
						{
							strArray1[index1] = "";
							strArray2[2] = ((int.Parse(strArray2[2]) < num1 || string.IsNullOrEmpty(strArray2[2].ToString())) ? str1 : strArray2[2]);
							strArray2[3] = ((int.Parse(strArray2[3].ToString()) < num3 || int.Parse(strArray2[3].ToString()) > num2 || string.IsNullOrEmpty(strArray2[3].ToString())) ? num3.ToString() : strArray2[3]);
							strArray2[4] = ((strArray2[4] == str2 || strArray2[4] == str3 || strArray2[4] == str4 || strArray2[4] == str5 || (strArray2[4] == str6 && !string.IsNullOrEmpty(strArray2[4].ToString()))) ? strArray2[4] : str2);
							strArray2[5] = ((strArray2[5] == str2 || strArray2[5] == str3 || strArray2[5] == str4 || strArray2[5] == str5 || (strArray2[5] == str6 && !string.IsNullOrEmpty(strArray2[5].ToString()))) ? strArray2[5] : str2);
							strArray2[6] = ((strArray2[6] == str2 || strArray2[6] == str3 || strArray2[6] == str4 || strArray2[6] == str5 || (strArray2[6] == str6 && !string.IsNullOrEmpty(strArray2[6].ToString()))) ? strArray2[6] : str2);
							strArray2[7] = ((strArray2[7] == str2 || strArray2[7] == str3 || strArray2[7] == str4 || strArray2[7] == str5 || (strArray2[7] == str6 && !string.IsNullOrEmpty(strArray2[7].ToString()))) ? strArray2[7] : str2);
							strArray2[8] = ((strArray2[8] == str7 || (strArray2[8] == str8 && !string.IsNullOrEmpty(strArray2[8]))) ? strArray2[8] : str7);
						}
						for (int index2 = 0; index2 < 9; index2++)
						{
							strArray1[index1] = strArray1[index1] + strArray2[index2] + ",";
						}
						strArray1[index1] = strArray1[index1].Remove(strArray1[index1].Length - 1, 1);
						index1++;
					}
					for (int index3 = 0; index3 < length; index3++)
					{
						param = param + strArray1[index3] + "|";
					}
					param = param.Remove(param.Length - 1, 1);
					return true;
				}
			}
			return false;
		}
	}
}
