using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness
{
    public class AreaBussiness : BaseCrossBussiness
    {
        public AreaConfigInfo[] GetAllAreaConfig()
        {
            List<AreaConfigInfo> infos = new List<AreaConfigInfo>();
            SqlDataReader reader = null;
            try
            {
                db.GetReader(ref reader, "SP_AreaConfig_All");
                while (reader.Read())
                {
                    infos.Add(InitAreaConfigInfo(reader));
                }
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled)
                    log.Error("InitAreaConfigInfo", e);
            }
            finally
            {
                if (reader != null && !reader.IsClosed)
                    reader.Close();
            }

            return infos.ToArray();
        }

        public AreaConfigInfo InitAreaConfigInfo(SqlDataReader dr)
        {
            AreaConfigInfo info = new AreaConfigInfo();
            info.AreaID = (int)dr["AreaID"];
            info.AreaServer = (dr["AreaServer"] == null) ? "" : dr["AreaServer"].ToString();
            info.AreaName = (dr["AreaName"] == null) ? "" : dr["AreaName"].ToString();
            info.DataSource = (dr["DataSource"] == null) ? "" : dr["DataSource"].ToString();
            info.Catalog = (dr["Catalog"] == null) ? "" : dr["Catalog"].ToString();
            info.UserID = (dr["UserID"] == null) ? "" : dr["UserID"].ToString();
            info.Password = (dr["Password"] == null) ? "" : dr["Password"].ToString();
            info.RequestUrl = (dr["RequestUrl"] == null) ? "" : dr["RequestUrl"].ToString();
            info.CrossChatAllow = (bool)dr["CrossChatAllow"];
            info.CrossPrivateChat = (bool)dr["CrossPrivateChat"];
            info.Version = (dr["Version"] == null) ? "" : dr["Version"].ToString();
            return info;
        }
    }
}