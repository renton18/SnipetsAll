﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AAA
{
    public static class SQLSERVERHelper
    {
        public static string logConnection = "";
        public static string loginId = "NoLoginUser";
        #region Logを出力する
        public static void Log(string LOG_LEVEL, string ERROR_MESSAGE, string SUBJECT, string MESSAGE, string UPDT_MESSAGE = "")
        {
            SQLSERVER DB = new SQLSERVER(logConnection);
            try
            {
                DB.Open();
                DB.ExecuteNonQuery(
                 "INSERT INTO SERVERLOG ( " +
                        "       [TIME_STAMP] " +
                        "       ,[LOG_LEVEL] " +
                        "       ,[PG_ID] " +
                        "       ,[ERROR_MESSAGE] " +
                        "       ,[SUBJECT] " +
                        "       ,[MESSAGE] " +
                        "       ,[UPDT_MESSAGE] " +
                        "       ,[UPDTDT] " +
                        "       ,[UPDTID] " +
                        "       ,[UPDTTRM] " +
                        "       ) " +
                        "VALUES ( " +
                        "       CONVERT(VARCHAR, GETDATE(), 120) " +
                        "       ," + LOG_LEVEL + " " + //0:情報、1:エラー
                        "       ,'" + Path.GetFileName(Environment.GetCommandLineArgs()[0]).Replace(".vshost", "") + "' " +
                        "       , '" + ERROR_MESSAGE.Replace("'", "''") + "' " +
                        "       , '" + SUBJECT.Replace("'", "''") + "' " +
                        "       ,'" + MESSAGE.Replace("'", "''") + "' " +
                        "       ,'" + UPDT_MESSAGE.Replace("'", "''") + "' " +
                        "       ,CONVERT(VARCHAR, GETDATE(), 120) " +
                        "       ,'" + loginId + "' " +
                        "       ,'" + Dns.GetHostName() + "' " +
                        "       )");
            }
            catch (Exception ex)
            {
                File.AppendAllText("ErrorLog.txt", DateTime.Now.ToString("yyyy/MM/dd (dddd) hh時mm分ss秒") + " " + ex.Message + "      " + ex.StackTrace);
                MessageBox.Show("データベースに接続ができませんでした(" + Application.StartupPath + @"\ErrorLog.txt)：" + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                DB.Close();
            }
        }
        #endregion

        #region  検索
        public static DataTable Search(string sql, string connection)
        {
            var dt = new DataTable();
            SQLSERVER DB = new SQLSERVER(connection);
            try
            {
                DB.Open();
                dt = DB.Select(sql);
            }
            catch (Exception)
            {
                //SQLSERVERHelper.Log("1", ex.Message + Environment.NewLine + (ex.InnerException == null ? "" : Environment.NewLine + ex.InnerException.Message), "検索", sql, "NoLoginUser");
                //MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                DB.Close();
            }
            return dt;
        }
        #endregion

        #region  挿入
        public static void Insert(string sql, string connection)
        {
            SQLSERVER DB = new SQLSERVER(connection);
            try
            {
                DB.Open();
                DB.ExecuteNonQuery(sql);
                SQLSERVERHelper.Log("0", "", "挿入", sql);
            }
            catch (Exception)
            {
                //SQLSERVERHelper.Log("1", ex.Message + Environment.NewLine + ex.StackTrace, errorTitle, sql, "NoLoginUser");
                //MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                DB.Close();
            }
        }
        #endregion

        #region  更新 
        // update [マスタ] set [名前] = '旧姓たなか' 
        // output deleted.*, inserted.*
        // where [名前] = 'たなか'
        public static void Update(string sql, string connection, string[] outputCol = null)
        {
            string difference = "";
            #region OUTPUT句のSQL生成
            if (outputCol == null)
            {
                var outputSql = " OUTPUT ";
                foreach (string item in outputCol)
                {
                    outputSql = outputSql + " '" + item + " 「 ' + inserted." + item + " + '　」 => 「 ' + deleted." + item + " + ' 」  ' +";
                }
                outputSql = outputSql.TrimEnd('+');
                sql = sql.Insert(sql.IndexOf("WHERE"), outputSql);
            }
            #endregion
            SQLSERVER DB = new SQLSERVER(connection);
            try
            {
                DB.Open();
                difference = DB.ExecuteScalar(sql);
                SQLSERVERHelper.Log("0", "", "更新", sql, difference);
            }
            catch (Exception)
            {
                //SQLSERVERHelper.Log("1", ex.Message + Environment.NewLine + ex.StackTrace, errorTitle, sql, "NoLoginUser", difference);
                //MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                DB.Close();
            }
        }
        #endregion

        #region  更新 
        // トランザクション用
        public static void UpdateTran(string sql, SQLSERVER DB, string[] outputCol = null)
        {
            string difference = "";
            #region OUTPUT句のSQL生成
            if (outputCol == null)
            {
                var outputSql = " OUTPUT ";
                foreach (string item in outputCol)
                {
                    outputSql = outputSql + " '" + item + " 「 ' + inserted." + item + " + '　」 => 「 ' + deleted." + item + " + ' 」  ' +";
                }
                outputSql = outputSql.TrimEnd('+');
                sql = sql.Insert(sql.IndexOf("WHERE"), outputSql);
            }
            #endregion
            try
            {
                difference = DB.ExecuteScalar(sql);
                SQLSERVERHelper.Log("0", "", "更新", sql, difference);
            }
            catch (Exception)
            {
                //SQLSERVERHelper.Log("1", ex.Message + Environment.NewLine + ex.StackTrace, errorTitle, sql, "NoLoginUser", difference);
                //MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
        }
        #endregion

        #region  削除
        public static void Delete(string sql, string connection)
        {
            SQLSERVER DB = new SQLSERVER(connection);
            try
            {
                DB.Open();
                DB.ExecuteNonQuery(sql);
                SQLSERVERHelper.Log("0", "", "削除", sql);
            }
            catch (Exception)
            {
                //SQLSERVERHelper.Log("1", ex.Message + Environment.NewLine + ex.StackTrace, errorTitle, sql, "NoLoginUser");
                //MessageBox.Show("エラー発生:" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                DB.Close();
            }
        }
        #endregion
    }
}
