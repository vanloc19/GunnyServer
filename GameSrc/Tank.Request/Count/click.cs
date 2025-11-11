using Ajax;
using Bussiness;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace Count
{
    public class click : Page
    {
        protected HtmlForm form1;

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(click));
        }

        [AjaxMethod]
        public string Logoff(
          string App_Id,
          string Direct_Url,
          string Referry_Url,
          string Begin_time,
          string ScreenW,
          string ScreenH,
          string Color,
          string Flash)
        {
            HttpContext current = HttpContext.Current;
            Dictionary<string, string> clientInfos = new Dictionary<string, string>();
            try
            {
                clientInfos.Add("Application_Id", App_Id);
                string userHostAddress = current.Request.UserHostAddress;
                string userAgent = current.Request.UserAgent == null ? "无" : current.Request.UserAgent;
                if (current.Request.ServerVariables["HTTP_UA_CPU"] == null)
                    clientInfos.Add("CPU", "未知");
                else
                    clientInfos.Add("CPU", current.Request.ServerVariables["HTTP_UA_CPU"]);
                clientInfos.Add("OperSystem", click.GetOSNameByUserAgent(userAgent));
                clientInfos.Add("IP", userHostAddress);
                clientInfos.Add("IPAddress", userHostAddress);
                if (current.Request.Browser.ClrVersion == (Version)null)
                    clientInfos.Add(".NETCLR", "不支持");
                else
                    clientInfos.Add("NETCLR", current.Request.Browser.ClrVersion.ToString());
                clientInfos.Add("Browser", current.Request.Browser.Browser + current.Request.Browser.Version);
                clientInfos.Add("ActiveX", current.Request.Browser.ActiveXControls ? "True" : "False");
                clientInfos.Add("Cookies", current.Request.Browser.Cookies ? "True" : "False");
                clientInfos.Add("CSS", current.Request.Browser.SupportsCss ? "True" : "False");
                clientInfos.Add("Language", current.Request.UserLanguages[0]);
                string serverVariable = current.Request.ServerVariables["HTTP_ACCEPT"];
                if (serverVariable == null)
                    clientInfos.Add("Computer", "False");
                else if (serverVariable.IndexOf("wap") > -1)
                    clientInfos.Add("Computer", "False");
                else
                    clientInfos.Add("Computer", "True");
                clientInfos.Add("Platform", current.Request.Browser.Platform);
                clientInfos.Add("Win16", current.Request.Browser.Win16 ? "True" : "False");
                clientInfos.Add("Win32", current.Request.Browser.Win32 ? "True" : "False");
                if (current.Request.ServerVariables["HTTP_ACCEPT_ENCODING"] == null)
                    clientInfos.Add("AcceptEncoding", "无");
                else
                    clientInfos.Add("AcceptEncoding", current.Request.ServerVariables["HTTP_ACCEPT_ENCODING"]);
                clientInfos.Add("UserAgent", userAgent);
                clientInfos.Add("Referry", Referry_Url);
                clientInfos.Add("Redirect", Direct_Url);
                clientInfos.Add("TimeSpan", Begin_time.ToString());
                clientInfos.Add("ScreenWidth", ScreenW);
                clientInfos.Add("ScreenHeight", ScreenH);
                clientInfos.Add(nameof(Color), Color);
                clientInfos.Add(nameof(Flash), Flash);
                CountBussiness.InsertContentCount(clientInfos);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return "ok";
        }

        private static string GetOSNameByUserAgent(string userAgent)
        {
            string str = "未知";
            if (userAgent.Contains("NT 6.0"))
                str = "Windows Vista/Server 2008";
            else if (userAgent.Contains("NT 5.2"))
                str = "Windows Server 2003";
            else if (userAgent.Contains("NT 5.1"))
                str = "Windows XP";
            else if (userAgent.Contains("NT 5"))
                str = "Windows 2000";
            else if (userAgent.Contains("NT 4"))
                str = "Windows NT4";
            else if (userAgent.Contains("Me"))
                str = "Windows Me";
            else if (userAgent.Contains("98"))
                str = "Windows 98";
            else if (userAgent.Contains("95"))
                str = "Windows 95";
            else if (userAgent.Contains("Mac"))
                str = "Mac";
            else if (userAgent.Contains("Unix"))
                str = "UNIX";
            else if (userAgent.Contains("Linux"))
                str = "Linux";
            else if (userAgent.Contains("SunOS"))
                str = "SunOS";
            return str;
        }
    }
}