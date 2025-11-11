using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using Bussiness.CenterService;
using Bussiness.Interface;
using log4net;
using Road.Flash;

namespace Tank.Request
{
	[WebService(Namespace = "http://tempuri.org/")]
	[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class ServerList : IHttpHandler
	{
		private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static string agentId => ConfigurationManager.AppSettings["ServerID"];

		public bool IsReusable => false;

		public void ProcessRequest(HttpContext context)
		{
			bool flag = false;
			string str = "Fail!";
			int num = 0;
			XElement xelement = new XElement("Result");
			try
			{
				if (BaseInterface.CheckRnd(context.Request["rnd"]))
				{
					using (CenterServiceClient centerServiceClient = new CenterServiceClient())
					{
						foreach (ServerData current in (IEnumerable<ServerData>)centerServiceClient.GetServerList())
						{
							if (current.State != -1)
							{
								//current.Port = int.Parse(ConfigurationManager.AppSettings["FakePort"]);
								//current.Ip = "222.255.115.133";
								num += current.Online;
								xelement.Add(FlashUtils.CreateServerInfo(current.Id, current.Name, current.Ip, current.Port, current.State, current.MustLevel, current.LowestLevel, current.Online));
							}
						}
					}
				}
				flag = true;
				str = "Success!";
			}
			catch (Exception ex)
			{
				log.Error("Load server list error:", ex);
			}
			xelement.Add(new XAttribute("value", flag));
			xelement.Add(new XAttribute("message", str));
			xelement.Add(new XAttribute("total", num));
			xelement.Add(new XAttribute("agentId", agentId));
			xelement.Add(new XAttribute("AreaName", "a" + agentId));
			xelement.Add(new XAttribute("Info", agentId));
			context.Response.ContentType = "text/plain";
			context.Response.Write(xelement.ToString(check: false));
		}
	}
}
