using System.Diagnostics;

namespace IDNO新旧変換
{
    class SQLSERVER_BALK
    {
        public string tableName { get; set; } = "";
        public string serverName { get; set; } = "";
        public string id { get; set; } = "";
        public string pass { get; set; } = "";
        public string importFilePath { get; set; } = "";
        public string exportFilePath { get; set; } = "";
        public string formatFilePath { get; set; } = "";
        public string log { get; set; } = "";

        #region インポート
        public void Import()
        {
            var proc = new Process();
            proc.StartInfo.FileName = "BCP";
            log = tableName + @" IN """ + importFilePath + @""" -f """ + formatFilePath + @""" -S " + serverName + " -U " + id + " -P " + pass;
            proc.StartInfo.Arguments = log;
            proc.Start();
            //終了するまで最大10秒間だけ待機する
            proc.WaitForExit(10000);
        }
        #endregion

        #region エクスポート
        public void Export()
        {
            var proc = new Process();
            proc.StartInfo.FileName = "BCP";
            log = tableName + @" OUT """ + exportFilePath + @""" -f """ + formatFilePath + @""" -S " + serverName + " -U " + id + " -P " + pass;
            proc.StartInfo.Arguments = log;
            proc.Start();
            //終了するまで最大10秒間だけ待機する
            proc.WaitForExit(10000);
        }
        #endregion
    }
}
