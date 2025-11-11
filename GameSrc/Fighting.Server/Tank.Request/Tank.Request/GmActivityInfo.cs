using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
using Bussiness;
using log4net;
using Road.Flash;
using SqlDataProvider.Data;

namespace Tank.Request
{
    /// <summary>
    /// Summary description for GmActivityInfo
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GmActivityInfo : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
            bool value = false;
            string message = "Fail!";
            XElement result = new XElement("Result");

            try
            {
                using (PlayerBussiness db = new PlayerBussiness())
                {
                    SqlDataProvider.Data.GmActivityInfo[] gmactives = db.GetAllGmActivity();
                    GmGiftInfo[] gmgifts = db.GetAllGmGift();
                    GmActiveConditionInfo[] gmconditions = db.GetAllGmActiveCondition();
                    GmActiveRewardInfo[] gmawards = db.GetAllGmActiveReward();

                    foreach (SqlDataProvider.Data.GmActivityInfo gm in gmactives)
                    {
                        XElement element = new XElement("ActiveInfo");

                        element.Add(FlashUtils.CreateGmActivity(gm));

                        XElement elementGb = new XElement("ActiveGiftBag");

                        IEnumerable temp_gm = gmgifts.Where(s => s.activityId == gm.activityId);
                        foreach (GmGiftInfo gift in temp_gm)
                        {
                            elementGb.Add(FlashUtils.CreateGmGift(gift));

                            // find condition
                            List<GmActiveConditionInfo> temp_gmCon = gmconditions.Where(s => s.giftbagId == gift.giftbagId).ToList();

                            if (temp_gmCon.Count > 0)
                            {
                                XElement elementCon = new XElement("ActiveCondition");
                                foreach (GmActiveConditionInfo con in temp_gmCon)
                                {
                                    elementCon.Add(FlashUtils.CreateGmActiveCondition(con));
                                }
                                elementGb.Add(elementCon);
                            }

                            // find awards
                            List<GmActiveRewardInfo> temp_gmAw = gmawards.Where(s => s.giftId == gift.giftbagId).ToList();
                            if (temp_gmAw.Count > 0)
                            {
                                XElement elementAw = new XElement("ActiveReward");
                                foreach (GmActiveRewardInfo con in temp_gmAw)
                                {
                                    elementAw.Add(FlashUtils.CreateGmActiveReward(con));
                                }
                                elementGb.Add(elementAw);
                            }
                        }

                        element.Add(elementGb);

                        result.Add(element);
                    }

                    value = true;
                    message = "Success!";
                }
            }
            catch (Exception ex)
            {
                log.Error("GMActivityInfo", ex);
            }

            result.Add(new XAttribute("value", value));
            result.Add(new XAttribute("message", message));
            //log.Error(result.ToString(false));
            //return result.ToString(false);
            return csFunction.CreateCompressXml(context, result, "GMActivityInfo", true);
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
