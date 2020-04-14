using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AAA
{
    static class ShareFolderConnection
    {
        //接続切断するWin32 API を宣言
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int WNetCancelConnection2(string lpName, Int32 dwFlags, bool fForce);
        //認証情報を使って接続するWin32 API宣言
        [DllImport("mpr.dll", EntryPoint = "WNetAddConnection2", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern int WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, Int32 dwFlags);
        //WNetAddConnection2に渡す接続の詳細情報の構造体
        [StructLayout(LayoutKind.Sequential)]
        internal struct NETRESOURCE
        {
            public int dwScope;//列挙の範囲
            public int dwType;//リソースタイプ
            public int dwDisplayType;//表示オブジェクト
            public int dwUsage;//リソースの使用方法
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpLocalName;//ローカルデバイス名。使わないならNULL。
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpRemoteName;//リモートネットワーク名。使わないならNULL
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpComment;//ネットワーク内の提供者に提供された文字列
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpProvider;//リソースを所有しているプロバイダ名
        }

        public static string shareName;

        public static Boolean Connect(string userId, string pass)
        {
            //string destFilePath = @"C:\test.txt";  // コピー先のローカルパス  
            //string sourceFilePath = @"\\filesv\共有フォルダ\test.txt";  // コピー対象の共有されたファイルのUNCパス  
            //string shareName = @"\\filesv\フォルダ"; // 共有パス　\\sparePC\C$などもOK  
            NETRESOURCE netResource = new NETRESOURCE();
            netResource.dwScope = 0;
            netResource.dwType = 1;
            netResource.dwDisplayType = 0;
            netResource.dwUsage = 0;
            netResource.lpLocalName = ""; // ネットワークドライブにする場合は"z:"などドライブレター設定  
            netResource.lpRemoteName = shareName;
            netResource.lpProvider = "";

            int ret = 0;
            try
            {
                //既に接続してる場合があるので一旦切断する
                ret = WNetCancelConnection2(shareName, 0, true);
                //共有フォルダに認証情報を使って接続
                ret = WNetAddConnection2(ref netResource, pass, userId, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ネットワーク共有フォルダに接続できませんでした。ネットワーク状態を確認下さい" + Environment.NewLine + ex.Message);
                return false;
            }
            return true;
        }

        #region 切断
        public static Boolean DisConnet()
        {
            try
            {
                //切断
                WNetCancelConnection2(shareName, 0, true);
            }
            catch (Exception ex)
            {
                //エラー処理
                return false;
            }
            return true;
        }
        #endregion

        #region 上書きコピー
        public static Boolean OverWriteCopy(string destFilePath, string sourceFilePath)
        {
            try
            {
                File.Copy(sourceFilePath, destFilePath);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("ファイルコピーに失敗しました。");
                return false;
            }
            return true;
        }
        #endregion
    }
}
