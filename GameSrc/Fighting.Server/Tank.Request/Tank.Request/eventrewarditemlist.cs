using System.Collections.Generic;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using SqlDataProvider.Data;

namespace Tank.Request
{
	public class eventrewarditemlist : IHttpHandler
	{
		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			if (csFunction.ValidAdminIP(context.Request.UserHostAddress))
			{
				context.Response.Write(Bulid(context));
			}
			else
			{
				context.Response.Write("IP is not valid!");
			}
		}

		public static string Bulid(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			XElement result = new XElement("Result");
			try
			{
				using (ProduceBussiness produceBussiness = new ProduceBussiness())
				{
					Dictionary<int, Dictionary<int, EventRewardInfo>> dictionary1 = new Dictionary<int, Dictionary<int, EventRewardInfo>>();
					EventRewardInfo[] allEventRewardInfo = produceBussiness.GetAllEventRewardInfo();
					EventRewardGoodsInfo[] eventRewardGoods = produceBussiness.GetAllEventRewardGoods();
					EventRewardInfo[] array = allEventRewardInfo;
					foreach (EventRewardInfo eventRewardInfo in array)
					{
						eventRewardInfo.AwardLists = new List<EventRewardGoodsInfo>();
						if (!dictionary1.ContainsKey(eventRewardInfo.ActivityType))
						{
							Dictionary<int, EventRewardInfo> dictionary2 = new Dictionary<int, EventRewardInfo>();
							dictionary2.Add(eventRewardInfo.SubActivityType, eventRewardInfo);
							dictionary1.Add(eventRewardInfo.ActivityType, dictionary2);
						}
						else if (!dictionary1[eventRewardInfo.ActivityType].ContainsKey(eventRewardInfo.SubActivityType))
						{
							dictionary1[eventRewardInfo.ActivityType].Add(eventRewardInfo.SubActivityType, eventRewardInfo);
						}
					}
					EventRewardGoodsInfo[] array2 = eventRewardGoods;
					foreach (EventRewardGoodsInfo eventRewardGoodsInfo in array2)
					{
						if (dictionary1.ContainsKey(eventRewardGoodsInfo.ActivityType) && dictionary1[eventRewardGoodsInfo.ActivityType].ContainsKey(eventRewardGoodsInfo.SubActivityType))
						{
							dictionary1[eventRewardGoodsInfo.ActivityType][eventRewardGoodsInfo.SubActivityType].AwardLists.Add(eventRewardGoodsInfo);
						}
					}
					XElement xelement1 = null;
					foreach (Dictionary<int, EventRewardInfo> value in dictionary1.Values)
					{
						foreach (EventRewardInfo current1 in value.Values)
						{
							if (xelement1 == null)
							{
								xelement1 = new XElement("ActivityType", new XAttribute("value", current1.ActivityType));
							}
							XElement xelement2 = new XElement("Items", new XAttribute("SubActivityType", current1.SubActivityType), new XAttribute("Condition", current1.Condition));
							foreach (EventRewardGoodsInfo current2 in current1.AwardLists)
							{
								XElement xelement3 = new XElement("Item", new XAttribute("TemplateId", current2.TemplateId), new XAttribute("StrengthLevel", current2.StrengthLevel), new XAttribute("AttackCompose", current2.AttackCompose), new XAttribute("DefendCompose", current2.DefendCompose), new XAttribute("LuckCompose", current2.LuckCompose), new XAttribute("AgilityCompose", current2.AgilityCompose), new XAttribute("IsBind", current2.IsBind), new XAttribute("ValidDate", current2.ValidDate), new XAttribute("Count", current2.Count));
								xelement2.Add(xelement3);
							}
							xelement1.Add(xelement2);
						}
						result.Add(xelement1);
						xelement1 = null;
					}
					flag = true;
					str = "Success!";
				}
			}
			catch
			{
			}
			result.Add(new XAttribute("value", flag));
			result.Add(new XAttribute("message", str));
			return csFunction.CreateCompressXml(context, result, "eventrewarditemlist", isCompress: true);
		}
	}
}
