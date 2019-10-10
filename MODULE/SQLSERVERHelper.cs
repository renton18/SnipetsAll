using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 直送分ID0
{
    public static class SQLSERVERHelper
    {
        #region Logを出力する
        public static void Log(string LOG_LEVEL, string ERROR_MESSAGE, string SUBJECT, string MESSAGE, string UPDTID)
        {
            try
            {
                SQLSERVER trustDb = new SQLSERVER(COMMON.TRUSTConnection);
                trustDb.Open();
                trustDb.ExecuteNonQuery(
                        "INSERT INTO SERVERLOG ( " +
                        "	[TIME_STAMP] " +
                        "	,[LOG_LEVEL] " +
                        "	,[PG_ID] " +
                        "	,[ERROR_MESSAGE] " +
                        "	,[SUBJECT] " +
                        "	,[MESSAGE] " +
                        "	,[UPDTDT] " +
                        "	,[UPDTID] " +
                        "	,[UPDTTRM] " +
                        "	) " +
                        "VALUES ( " +
                        "	getdate() " +
                        "	,1 " + //0:情報、1:エラー
                        "	,'" + Path.GetFileName(Environment.GetCommandLineArgs()[0]).Replace(".vshost", "") + "' " +
                        "	, '" + ERROR_MESSAGE.Replace("'", "''") + "' " +
                        "	, '" + SUBJECT.Replace("'", "''") + "' " +
                        "	,'" + MESSAGE.Replace("'", "''") + "' " +
                        "	,getdate() " +
                        "	,'" + UPDTID + "' " +
                        "	,'" + Dns.GetHostName() + "' " +
                        "	)");
                trustDb.Close();
            }
            catch (Exception ex)
            {
                File.AppendAllText("ErrorLog.txt", DateTime.Now.ToString("yyyy/MM/dd (dddd) hh時mm分ss秒") + " " + ex.Message + Environment.NewLine);
            }

        }
        #endregion

        #region  データグリッドビュー初期表示用
        public static DataTable GetInitData(string filedate)
        {
            var sql = "";
            var dt = new DataTable();
            try
            {
                SQLSERVER TRUST = new SQLSERVER(COMMON.TRUSTConnection);
                TRUST.Open();
                sql = "SELECT filedate, idno, Idzero, UpdateDt, CreateDt " +
                        "FROM T_IdnoToIdzero " +
                        "WHERE filedate ='" + filedate + "' " +
                        "ORDER BY Idno";

                dt = TRUST.Select(sql);
            }
            catch (Exception ex)
            {
                SQLSERVERHelper.Log("1", ex.Message, "初期表示", sql, "NoLoginUser");
                MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message);
            }
            return dt;
        }
        #endregion

        #region  検索用
        public static DataTable Search(string filedate, string idno)
        {
            var sql = "";
            var dt = new DataTable();
            try
            {
                SQLSERVER TRUST = new SQLSERVER(COMMON.TRUSTConnection);
                TRUST.Open();
                sql = "SELECT filedate, idno, Idzero, UpdateDt, CreateDt " +
                        "FROM T_IdnoToIdzero " +
                        "WHERE filedate ='" + filedate + "' " +
                        "AND Idno like'%" + idno + "%' " +
                        "ORDER BY Idno";

                dt = TRUST.Select(sql);
            }
            catch (Exception ex)
            {
                SQLSERVERHelper.Log("1", ex.Message, "検索", sql, "NoLoginUser");
                MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message);
            }
            return dt;
        }
        #endregion
    }
}
