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
    public class CelebByDayGiftGp : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Write(Build(context));
        }


        public static string Build(HttpContext context)
        {
            // Removed IP check to allow public access from game client
            // if (!csFunction.ValidAdminIP(context.Request.UserHostAddress))
            // {
            //     return "CelebByDayGiftGp Fail!";
            // }
            return Build();
        }


        public static string Build()
        {
            return csFunction.BuildCelebUsers("CelebByDayGiftGp", 12, "CelebByDayGiftGp_Out");
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

