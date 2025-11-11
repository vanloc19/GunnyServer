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
using System.Threading;

namespace Tank.Request
{
    public partial class KitoffUser : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private SqlConnection SqlConnection;
        private static int _count;
        private static Timer _timer;
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
            int result = 0;
            try
            {
				string userHostAddress = Context.Request.UserHostAddress;;
                if (ValidLoginIP(userHostAddress))//(context.Request.UserHostAddress;))
                {
                    CenterServiceClient centerServiceClient = new CenterServiceClient();
                    _count = 6;
                    _timer = new Timer(sendMaintenanceMessage, null, 0, 60000);
                    result = 1;
                }
                else
                {
                    result = 0;
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
        private void sendMaintenanceMessage(object state)
        {
            _count--;
            CenterServiceClient centerServiceClient = new CenterServiceClient();
            centerServiceClient.SystemNotice("[Hệ thống] Server sẽ bảo trì sau " + _count  + " phút nữa, vui lòng thoát game để tránh mất đồ!");
            if (_count == 0)
            {
                _timer.Dispose();
                if (_timer != null)
                {
                    DataTable listByState = getOnlinePlayers(1);
                    for (int i = 0; i < listByState.Rows.Count; i++)
                    {
                        //Response.Write(listByState.Rows[i]["UserID"]+ "\r\n");
                        var userId = Convert.ToInt32(listByState.Rows[i]["UserID"]);
                        centerServiceClient.KitoffUser(userId, "Bảo trì Server, vui lòng quay lại sau");
                    }
                }
                _timer = null;
            }
        }
    }
}
