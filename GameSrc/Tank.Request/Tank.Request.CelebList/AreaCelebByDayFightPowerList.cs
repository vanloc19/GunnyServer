using log4net;
using System.Reflection;
using System.Web;
using System.Web.Services;

namespace Tank.Request.CelebList
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class AreaCelebByDayFightPowerList : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write(Build(context));
        }

        public static string Build(HttpContext context)
        {
            if (!csFunction.ValidAdminIP(context.Request.UserHostAddress))
            {
                return "AreaCelebByDayFightPowerList Fail!";
            }
            return Build();
        }
        public static string Build()
        {
            return csFunction.BuildAreaCelebUsers("AreaCelebByDayFightPowerList", 6, "AreaCelebByDayFightPowerList_Out");
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
