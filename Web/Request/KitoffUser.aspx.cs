using System;
using System.Configuration;
using System.Data;
using System.Linq;
using log4net;
using System.Reflection;
using Bussiness;
using System.Data;
using System.Data.SqlClient;
using Bussiness.CenterService;

namespace Tank.Request
{
    public partial class KitoffUser : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection SqlConnection;
        public static string GetAdminIP
        {
            get
            {
                return ConfigurationSettings.AppSettings["AdminIP"];
            }
        }

        public static bool ValidLoginIP(string ip)
        {
            string ips = GetAdminIP;
            if (string.IsNullOrEmpty(ips) || ips.Split('|').Contains(ip))
                return true;
            return false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            bool result = false;
            try
            {
                if (ValidLoginIP(Context.Request.UserHostAddress))
                {
                    DataTable listByState = getOnlinePlayers(1);
                    CenterServiceClient centerServiceClient = new CenterServiceClient();

                    for (int i = 0; i < listByState.Rows.Count; i++)
                    {
                        Response.Write(listByState.Rows[i]["UserID"]+ "\r\n");
                        //centerServiceClient.KitoffUser(listByState.Rows[i]["UserID"].ToInt(), "Bảo trì Server, vui lòng quay lại sau");
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("GetAdminIP:", ex);
            }

            Response.Write(result);
        }

        private static DataTable getOnlinePlayers(int iState)
        {
            string connectionString = ConnectDataBase.ConnectionString;
            string cmdText = "SELECT * FROM [Sys_Users_Detail] WHERE State=@State";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            try
            {
                SqlCommand sqlCommand = new SqlCommand(cmdText, sqlConnection);
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.Parameters.Add("@State", SqlDbType.Int).Value = iState;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                sqlCommand.Dispose();
                return dataTable;
            }
            catch
            {
                return null;
            }
        }

    }

    /*
     * 		public bool KitoffUser(int playerID, string msg)
		{
			return base.Channel.KitoffUser(playerID, msg);
		}*/
}
