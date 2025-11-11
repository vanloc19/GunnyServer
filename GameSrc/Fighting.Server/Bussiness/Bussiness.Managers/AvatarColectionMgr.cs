using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using SqlDataProvider.Data;

namespace Bussiness.Managers
{
    public class AvatarColectionMgr
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static List<ClothPropertyTemplateInfo> m_clothPropertyTemplateInfo =
            new List<ClothPropertyTemplateInfo>();

        private static Dictionary<int, List<ClothGroupTemplateInfo>> m_clothGroupTemplateInfo =
            new Dictionary<int, List<ClothGroupTemplateInfo>>();

        private static Dictionary<int, ClothPropertyTemplateInfo> m_clothPropertyTemplates =
            new Dictionary<int, ClothPropertyTemplateInfo>();

        #region 掉落加载

        /// <summary>
        /// 掉落加载
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            return ReLoad();
        }

        public static bool ReLoad()
        {
            try
            {
                List<ClothPropertyTemplateInfo> tempClothPropertyTemplate = LoadClothPropertyTemplateDb();
                Dictionary<int, List<ClothGroupTemplateInfo>> tempClothGroupTemplateInfo =
                    LoadClothGroupTemplateInfoDb(tempClothPropertyTemplate);
                if (tempClothPropertyTemplate.Count > 0)
                {
                    Interlocked.Exchange(ref m_clothPropertyTemplateInfo, tempClothPropertyTemplate);
                    Interlocked.Exchange(ref m_clothGroupTemplateInfo, tempClothGroupTemplateInfo);
                }

                return true;
            }
            catch (Exception e)
            {
                log.Error("AvatarColectionMgr", e);
            }

            return false;
        }

        /// <summary>
        /// 获到全部掉落条件
        /// </summary>
        /// <returns></returns>
        public static List<ClothPropertyTemplateInfo> LoadClothPropertyTemplateDb()
        {
            using (ProduceBussiness db = new ProduceBussiness())
            {
                ClothPropertyTemplateInfo[] infos = db.GetAllClothPropertyTemplateInfos();
                foreach (ClothPropertyTemplateInfo info in infos)
                {
                    if (!m_clothPropertyTemplates.ContainsKey(info.ID))
                    {
                        m_clothPropertyTemplates.Add(info.ID, info);
                    }
                }

                return infos != null ? infos.ToList() : null;
            }
        }

        /// <summary>
        /// 获取掉落物品
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, List<ClothGroupTemplateInfo>> LoadClothGroupTemplateInfoDb(
            List<ClothPropertyTemplateInfo> tempClothPropertyTemplates)
        {
            Dictionary<int, List<ClothGroupTemplateInfo>> list = new Dictionary<int, List<ClothGroupTemplateInfo>>();

            using (ProduceBussiness db = new ProduceBussiness())
            {
                ClothGroupTemplateInfo[] infos = db.GetAllClothGroupTemplateInfos();
                foreach (ClothPropertyTemplateInfo info in tempClothPropertyTemplates)
                {
                    IEnumerable<ClothGroupTemplateInfo> temp = infos.Where(s => s.ID == info.ID);
                    list.Add(info.ID, temp.ToList());
                }
            }

            return list;
        }

        /// <summary>
        /// 查找掉落物品
        /// </summary>
        /// <param name="dropId"></param>
        /// <returns></returns>
        public static List<ClothGroupTemplateInfo> FindClothGroupTemplateInfo(int groupId)
        {
            if (m_clothGroupTemplateInfo.ContainsKey(groupId))
            {
                return m_clothGroupTemplateInfo[groupId];
            }

            return new List<ClothGroupTemplateInfo>();
        }

        public static ClothGroupTemplateInfo FindClothGroupTemplateInfo(int groupId, int itemId, int sex)
        {
            if (m_clothGroupTemplateInfo.ContainsKey(groupId))
            {
                foreach (ClothGroupTemplateInfo info in m_clothGroupTemplateInfo[groupId])
                {
                    if ((info.TemplateID == itemId || info.OtherTemplateID.Contains(itemId.ToString())) && info.Sex == sex)
                        return info;
                }
            }

            return null;
        }

        public static List<ClothPropertyTemplateInfo> GetClothPropertyTemplate()
        {
            if (m_clothPropertyTemplateInfo != null)
            {
                return m_clothPropertyTemplateInfo;
            }

            return null;
        }

        #endregion

        public static ClothPropertyTemplateInfo FindClothPropertyTemplate(int dataId)
        {
            if (m_clothPropertyTemplates.ContainsKey(dataId))
            {
                return m_clothPropertyTemplates[dataId];
            }

            return null;
        }
    }
}