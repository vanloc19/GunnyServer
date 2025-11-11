using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class IMFriendsBbs : IHttpHandler
	{
		public interface IAgentFriends
		{
			string FriendsString(string uid);
		}

		public class Normal : IAgentFriends
		{
			private string Url;

			public static string FriendInterface => ConfigurationManager.AppSettings["FriendInterface"];

			public string FriendsString(string uid)
			{
				try
				{
					if (FriendInterface == "")
					{
						return string.Empty;
					}
					string err = "";
					Url = string.Format(CultureInfo.InvariantCulture, FriendInterface, new object[1]
					{
						uid
					});
					string page = WebsResponse.GetPage(Url, "", "utf-8", out err);
					if (err == "")
					{
						return page;
					}
					throw new Exception(err);
				}
				catch (Exception ex)
				{
					if (log.IsErrorEnabled)
					{
						log.Error("Normal：", ex);
					}
				}
				return string.Empty;
			}
		}

		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str1 = "Fail!";
			XElement xelement1 = new XElement("Result");
			Normal normal = new Normal();
			StringBuilder stringBuilder1 = new StringBuilder();
			string uid = HttpContext.Current.Request.Params["Uid"];
			string s = normal.FriendsString(uid);
			DataSet dataSet = new DataSet();
			if (s != "")
			{
				try
				{
					dataSet.ReadXml(new StringReader(s));
					for (int index2 = 0; index2 < dataSet.Tables["item"].DefaultView.Count; index2++)
					{
						stringBuilder1.Append(dataSet.Tables["item"].DefaultView[index2]["UserName"].ToString() + ",");
					}
				}
				catch (Exception ex2)
				{
					if (log.IsErrorEnabled)
					{
						log.Error("Get Table Item ", ex2);
					}
				}
			}
			if (stringBuilder1.Length <= 1 || s == "")
			{
				xelement1.Add(new XAttribute("value", flag));
				xelement1.Add(new XAttribute("message", str1));
				context.Response.ContentType = "text/plain";
				context.Response.Write(xelement1.ToString(check: false));
				return;
			}
			string[] strArray = stringBuilder1.ToString().Split(',');
			ArrayList arrayList = new ArrayList();
			StringBuilder stringBuilder2 = new StringBuilder(4000);
			for (int index = 0; index < strArray.Count() && !(strArray[index] == ""); index++)
			{
				if (stringBuilder2.Length + strArray[index].Length < 4000)
				{
					stringBuilder2.Append(strArray[index] + ",");
					continue;
				}
				arrayList.Add(stringBuilder2.ToString());
				stringBuilder2.Remove(0, stringBuilder2.Length);
			}
			arrayList.Add(stringBuilder2.ToString());
			try
			{
				for (int index3 = 0; index3 < arrayList.Count; index3++)
				{
					string str2 = arrayList[index3].ToString();
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						FriendInfo[] friendsBbs = playerBussiness.GetFriendsBbs(str2);
						for (int index4 = 0; index4 < friendsBbs.Count(); index4++)
						{
							DataRow[] dataRowArray = dataSet.Tables["item"].Select("UserName='" + friendsBbs[index4].UserName + "'");
							XElement xelement2 = new XElement("Item", new XAttribute("NickName", friendsBbs[index4].NickName), new XAttribute("UserName", friendsBbs[index4].UserName), new XAttribute("UserId", friendsBbs[index4].UserID), new XAttribute("Photo", (dataRowArray[0]["Photo"] == null) ? "" : dataRowArray[0]["Photo"].ToString()), new XAttribute("PersonWeb", (dataRowArray[0]["PersonWeb"] == null) ? "" : dataRowArray[0]["PersonWeb"].ToString()), new XAttribute("IsExist", friendsBbs[index4].IsExist), new XAttribute("OtherName", (dataRowArray[0]["OtherName"] == null) ? "" : dataRowArray[0]["OtherName"].ToString()));
							xelement1.Add(xelement2);
						}
					}
				}
				flag = true;
				str1 = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("IMFriendsGood", ex);
			}
			xelement1.Add(new XAttribute("value", flag));
			xelement1.Add(new XAttribute("message", str1));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement1.ToString(check: false));
		}
	}
}
