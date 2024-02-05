using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Windows.Forms;

namespace DownloadDataFTP
{
    public class DbOpen
    {
        public static string gstrDbConn = "";	//DB 연결정보
        //public static string gstrDbConn = @"server=10.10.10.80\dbserver;uid=e2max;pwd=kyg007!;database=kona";	//DB 연결정보

        #region DataSet 에러 처리
		private static DataSet DataSetError(string Msg)
		{
			DataSet ds = null;
			ds = new DataSet();
			ds.Tables.Add();

			DataRow newRow = ds.Tables[0].NewRow();
			ds.Tables[0].Rows.Add(newRow);

			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();
			ds.Tables[0].Columns.Add();

			ds.Tables[0].Rows[0][0] = "ER";
			ds.Tables[0].Rows[0][1] = Msg;
			ds.Tables[0].Rows[0][2] = "";
			ds.Tables[0].Rows[0][3] = "";
			ds.Tables[0].Rows[0][4] = "";

			return ds;
		}
		#endregion

		#region DataSet 에러 처리
		private static DataTable DataTableError(string Msg)
		{
			DataTable dt = null;
			dt = new DataTable();

			DataRow newRow = dt.NewRow();
			dt.Rows.Add(newRow);

			dt.Columns.Add();
			dt.Columns.Add();
			dt.Columns.Add();
			dt.Columns.Add();
			dt.Columns.Add();

			dt.Rows[0][0] = "ER";
			dt.Rows[0][1] = Msg;
			dt.Rows[0][2] = "";
			dt.Rows[0][3] = "";
			dt.Rows[0][4] = "";

			return dt;
		}
		#endregion

		#region DB Connection
		public static string DBOpen()
		{
			return gstrDbConn;
		}
		#endregion

		#region DB Connection & Open
		public static SqlConnection DBCON()	//DB 열기
		{
			SqlConnection dbcon = null;
			string dbConString = DBOpen();
			dbcon = new SqlConnection(dbConString);
			dbcon.Open();
			return dbcon;
		}
		#endregion

		#region DB 트랜잭션으로 NonQuery 처리 600초
		public static string TranNonQuery(string Query, string Msg)	//DB 트랜잭션으로 NonQuery 처리
		{
			string RtnMsg;
			SqlConnection dbConn = DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
			cmd.Transaction = Trans;
			cmd.CommandTimeout = 6000000;

			try
			{
				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
				Trans.Commit();

				RtnMsg = Msg;
			}
			catch(Exception e)
			{
				Trans.Rollback();
                RtnMsg = e.ToString();
			}
			dbConn.Close();

            return RtnMsg;
		}
		#endregion

		#region DB 트랜잭션으로 NonQuery 처리 6000초
		public static string TranNonQueryLongTime(string Query, string Msg)	//DB 트랜잭션으로 NonQuery 처리
		{
			string RtnMsg;
			SqlConnection dbConn = DBCON();
			SqlCommand cmd = dbConn.CreateCommand();
			SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
			cmd.Transaction = Trans;
			cmd.CommandTimeout = 60000000;

			try
			{
				cmd.CommandText = Query;
				cmd.ExecuteNonQuery();
				Trans.Commit();

				RtnMsg = Msg;
			}
			catch(Exception e)
			{
				Trans.Rollback();
                RtnMsg = e.ToString();
			}
			dbConn.Close();

			return RtnMsg;
		}
		#endregion

		#region 트랜잭션 처리 TranDataSet 사용
		public static DataSet TranDataSet(string Query, SqlConnection dbConn, SqlTransaction trans)
		{
			DataSet ds = null;
			try
			{
				SqlCommand cmd = new SqlCommand(Query, dbConn);
				cmd.Transaction = trans;
				cmd.CommandTimeout = 6000000;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				ds=new DataSet();
				da.Fill(ds);
			}
			catch(Exception e)
			{
				
				ds = DataSetError(e.Message.ToString());
			}
			return ds;
		}
		#endregion

		#region 트랜잭션 처리 DataTable 사용
		public static DataTable TranDataTable(string Query)
		{
			DataTable dt = null;
			try
			{
				SqlConnection dbConn = DBCON();
				SqlCommand cmd = new SqlCommand(Query, dbConn);
				SqlTransaction Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 6000000;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				dt = new DataTable();
				da.Fill(dt);
			}
			catch(Exception e)
			{
				
				dt = DataTableError(e.Message.ToString());
			}

			return dt;
		}
		#endregion

		#region 트랜잭션 처리 TranDataTable 사용
		public static DataTable TranDataTable(string Query, SqlConnection dbConn, SqlTransaction trans)
		{
			DataTable dt = null;
			try
			{
				SqlCommand cmd = new SqlCommand(Query, dbConn);
				cmd.Transaction = trans;
				cmd.CommandTimeout = 6000000;
				SqlDataAdapter da = new SqlDataAdapter(cmd);
				dt=new DataTable();
				da.Fill(dt);
			}
			catch(Exception e)
			{
				
				dt = DataTableError(e.Message.ToString());
			}

			return dt;
		}
		#endregion

		#region TranDataTable2 DB 트랜잭션으로 NonQuery 처리 600초
		public static DataTable TranDataTable2(string Query)	//DB 트랜잭션으로 NonQuery 처리
		{
			SqlConnection dbConn = null;
			SqlTransaction Trans = null;
			DataTable dt = null;
			try
			{
				dbConn = DBCON();
				SqlCommand cmd = dbConn.CreateCommand();
				Trans = dbConn.BeginTransaction(IsolationLevel.ReadCommitted);
				cmd.Transaction = Trans;
				cmd.CommandTimeout = 6000000;

				cmd.CommandText = Query;

				SqlDataAdapter da = new SqlDataAdapter(cmd);
				dt = new DataTable();
				da.Fill(dt);

				if(dt.Rows[0][0].ToString() == "ER"){goto Exit;}
				 
			}
			catch(Exception e)
			{
				Trans.Rollback();
				
				dt = DataTableError(e.Message.ToString());
			}
			Exit:
				dbConn.Close();
			return dt;
		}
		#endregion

		#region 트랜잭션 처리하지 않은 DataSet 사용
		public static DataSet NoTranDataSet(string Query)
		{
			DataSet ds = null;
			try
			{
				SqlConnection dbcon = DBCON();
				string query = Query;
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				cmd.CommandTimeout = 0;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				ds = new DataSet();
				adp.Fill(ds);
				dbcon.Close();
			}
			catch(Exception e)
			{
			
				ds = DataSetError(e.Message.ToString());
			}
			return ds;
		}
		#endregion

		#region 트랜잭션 처리하지 않은 DataTable 사용
		public static DataTable NoTranDataTable(string Query)
		{
			DataTable dt = null;
			try
			{
				SqlConnection dbcon = DBCON();
				string query = Query;
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				cmd.CommandTimeout = 0;
				SqlDataAdapter adp = new SqlDataAdapter(cmd);
				dt = new DataTable();
				adp.Fill(dt);
				dbcon.Close();
			}
			catch(Exception e)
			{
			
				dt = DataTableError(e.Message.ToString());
			}
			return dt;
		}
		#endregion

		#region 트랜잭션 처리하지 않은 NonQuery
		public static void NoTranNonQuery(string Query)
		{
			try
			{
				SqlConnection dbcon = DBCON();
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				cmd.ExecuteNonQuery();
				dbcon.Close();
			}
			catch(Exception e)
			{
                MessageBox.Show(e.ToString());
			}
		}
		#endregion

		#region 트랜잭션 처리하지 않은 NonQuery
		public static object NoTranScalar(string Query)
		{
			object obj = null;
			try
			{
				SqlConnection dbcon = DBCON();
				SqlCommand cmd = new SqlCommand(Query, dbcon);
				obj = cmd.ExecuteScalar();
				dbcon.Close();
			}
			catch(Exception e)
			{
                MessageBox.Show(e.ToString());
			}
			return obj;
		}
		#endregion

    }
}
