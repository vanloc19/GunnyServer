// ddtankconnect.ConnectDataBase
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public class ConnectDataBase
{
	private SqlConnection SQLConnect;

	private static string mConnectionString = ConfigurationSettings.AppSettings["conString"];

	private string mMessage = "";

	public static string Prefix => "";

	public static string ConnectionString
	{
		get
		{
			return mConnectionString;
		}
		set
		{
			mConnectionString = value;
		}
	}

	public string Message => mMessage;

	public bool StillConnect
	{
		get
		{
			if (SQLConnect == null || SQLConnect.State == ConnectionState.Closed)
			{
				return false;
			}
			return true;
		}
	}

	public ConnectDataBase()
	{
		SQLConnect = new SqlConnection(ConnectionString);
	}

	public ConnectDataBase(string StrConnect)
	{
		try
		{
			SQLConnect = new SqlConnection();
			SQLConnect.ConnectionString = StrConnect;
			SQLConnect.Open();
			mConnectionString = StrConnect;
		}
		catch (Exception ex)
		{
			mMessage = ex.ToString();
		}
		finally
		{
			SQLConnect.Close();
		}
	}

	public void CloseConnect()
	{
		SQLConnect.Close();
	}

	public DataTable Query(string strQuery)
	{
		DataTable dataTable = new DataTable();
		try
		{
			SQLConnect.Open();
			if (StillConnect)
			{
				try
				{
					SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(strQuery, SQLConnect);
					sqlDataAdapter.Fill(dataTable);
				}
				catch (Exception ex)
				{
					mMessage = ex.Message;
				}
			}
			SQLConnect.Close();
		}
		catch
		{
		}
		return dataTable;
	}

	public int ExcuteGetRowAffected(string strCommand)
	{
		if (StillConnect)
		{
			try
			{
				SqlCommand sqlCommand = new SqlCommand(strCommand, SQLConnect);
				return sqlCommand.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				mMessage = ex.Message;
				return 0;
			}
		}
		return 0;
	}

	public int ExcuteGetRowAffected(SqlCommand cm)
	{
		if (StillConnect)
		{
			try
			{
				return cm.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				mMessage = ex.Message;
				return 0;
			}
		}
		return 0;
	}

	public bool Excute(string strCommand)
	{
		SQLConnect.Open();
		if (StillConnect)
		{
			try
			{
				SqlCommand sqlCommand = new SqlCommand(strCommand, SQLConnect);
				sqlCommand.ExecuteNonQuery();
				SQLConnect.Close();
				return true;
			}
			catch (Exception ex)
			{
				mMessage = ex.Message;
				SQLConnect.Close();
				return false;
			}
		}
		SQLConnect.Close();
		return false;
	}

	public SqlDataReader ExcuteReaderOneRecord(string strCommand)
	{
		if (StillConnect)
		{
			try
			{
				SqlCommand sqlCommand = new SqlCommand(strCommand, SQLConnect);
				return sqlCommand.ExecuteReader();
			}
			catch (Exception ex)
			{
				mMessage = ex.Message;
				return null;
			}
		}
		return null;
	}
}
