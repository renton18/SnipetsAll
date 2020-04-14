using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace AAA
{
    //ADO.NET
    //参照追加必要なし
    class SQLSERVER
    {
        private string connectionStr = "";
        private SqlConnection connection;
        public SqlTransaction transaction;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="connectionStr"></param>
        public SQLSERVER(string connectionStr)
        {
            this.connectionStr = connectionStr;
        }
        /// <summary>
        /// open
        /// </summary>
        public void Open()
        {
            try
            {
                this.connection = new SqlConnection(connectionStr);
                this.connection.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// close
        /// </summary>
        public void Close()
        {
            if (this.connection == null)
            {
                return;
            }
            this.connection.Close();
            this.connection.Dispose();
        }

        /// <summary>
        /// select(パラメータあり)
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramDict">The parameter dictionary.</param>
        /// <returns></returns>
        public DataTable Select(string sql, Dictionary<string, Object> paramDict)
        {
            var datatable = new DataTable();
            using (var command = this.connection.CreateCommand())
            {
                try
                {
                    command.CommandText = sql;
                    // パラメータ代入
                    foreach (KeyValuePair<string, Object> item in paramDict)
                    {
                        command.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }
                    var adapter = new SqlDataAdapter(command);
                    adapter.Fill(datatable);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return datatable;
        }
        /// <summary>
        /// select(パラメータなし)
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public DataTable Select(string sql)
        {
            return this.Select(sql, new Dictionary<string, Object>());
        }

        /// <summary>
        /// insert update delete(パラメータあり)
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="paramDict">The parameter dictionary.</param>
        /// <returns>実行結果件数</returns>
        public int ExecuteNonQuery(string sql, Dictionary<string, Object> paramDict)
        {
            int resultCount = 0;
            using (var command = new SqlCommand() { Connection = connection, Transaction = transaction })
            {
                try
                {
                    command.CommandText = sql;
                    //パラメータ代入
                    foreach (KeyValuePair<string, Object> item in paramDict)
                    {
                        command.Parameters.Add(new SqlParameter(item.Key, item.Value));
                    }
                    resultCount = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return resultCount;
        }

        /// <summary>
        /// insert update delete(パラメータなし)
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>実行結果件数</returns>
        public int ExecuteNonQuery(string sql)
        {
            return this.ExecuteNonQuery(sql, new Dictionary<string, Object>());
        }

        /// <summary>
        /// トランザクション開始
        /// </summary>
        public void BeginTransaction()
        {
            this.transaction = this.connection.BeginTransaction();
        }

        /// <summary>
        /// コミット
        /// </summary>
        public void Commit()
        {
            if (this.transaction.Connection != null)
            {
                this.transaction.Commit();
                this.transaction.Dispose();
            }
        }

        /// <summary>
        /// ロールバック
        /// </summary>
        public void RollBack()
        {
            if (this.transaction == null)
            {
                return;
            }
            if (this.transaction.Connection == null)
            {
                return;
            }
            this.transaction.Rollback();
            this.transaction.Dispose();
        }

        /// <summary>
        ///  ストアド実行
        /// </summary>
        /// <param name="procedureName">プロシージャネーム</param>
        /// <returns>正常=0</returns>
        public int StoreProcedure(string procedureName, List<SqlParameter> para = null)
        {
            int ret = 0;
            using (var command = new SqlCommand() { Connection = connection, Transaction = transaction })
            {
                try
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = procedureName;
                    if (para != null)
                    {
                        command.Parameters.AddRange(para.ToArray());
                    }
                    command.ExecuteNonQuery();
                    //返り値がある場合
                    if (para != null)
                    {
                        ret = (int)command.Parameters["ReturnValue"].Value;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return ret;
            }
        }

        /// <summary>
        /// OUTPUT句
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string ExecuteScalar(string sql)
        {
            string ret = "";
            using (var command = new SqlCommand() { Connection = connection, Transaction = transaction })
            {
                try
                {
                    command.CommandText = sql;
                    var returnSeq = command.ExecuteScalar();
                    if (DBNull.Value.Equals(returnSeq) == false)
                    {
                        ret = (string)returnSeq;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ret;
        }

    }
}