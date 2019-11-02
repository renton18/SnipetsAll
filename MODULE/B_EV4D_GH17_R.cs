using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace AAA
{
    public class B_EV4D_GH17_R
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern System.IntPtr CreateFont(int nHeight, int nWidth, int nEscapement, int nOrientation, int fnWeight, bool fdwItalic, bool fdwUnderline, bool fdwStrikeOut, int fdwCharSet, int fdwOutputPrecision, int fdwClipPrecision, int fdwQuality, int fdwPitchAndFamily, string lpszFace);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern System.IntPtr SelectObject(System.IntPtr hObject, System.IntPtr hFont);
        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        static extern int TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool DeleteObject(System.IntPtr hObject);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern int BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateSolidBrush(uint crColor);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern int SetTextColor(IntPtr hdc, int crColor);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern int SetBkColor(IntPtr hdc, int crColor);
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        static extern bool Rectangle(IntPtr hdc, int left, int top, int right, int bottom);

        int pageCount = 0;
        string printerName = "";
        DataTable dt = new DataTable();
        Bitmap canvas;
        List<int> attributeList = new List<int>();
        List<string[]> stringList = new List<string[]>();

        #region コンストラクタ
        public B_EV4D_GH17_R(string printerName, DataTable dt, List<string[]> stringList, List<int> attributeList)
        {
            this.attributeList = attributeList;
            this.stringList = stringList;
            this.printerName = printerName;
            this.dt = dt;
        }
        #endregion

        #region 印刷イメージを取得する
        public Bitmap GetPrintImage(PictureBox pictureBox)
        {
            canvas = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics g = Graphics.FromImage(canvas);
            IntPtr hdc = g.GetHdc();
            PrintImage(hdc);
            //デバイスコンテキストの開放
            g.ReleaseHdc(hdc);
            return canvas;
        }
        #endregion

        #region 印刷処理
        public void Print()
        {
            //データテーブルにデータがない場合
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("印刷するデータがありません。");
                return;
            }

            pageCount = 0;
            PrintDocument pd = new PrintDocument();
            pd.PrinterSettings.PrinterName = printerName;
            //プリンタが設定されてない場合はXPSファイルとして出力する
            if (pd.PrinterSettings.PrinterName == "")
            {
                pd.PrinterSettings.PrinterName = "Microsoft XPS Document Writer";
                pd.PrinterSettings.PrintFileName = "test.xps";
                //ポップアップを表示させない
                pd.DefaultPageSettings.PrinterSettings.PrintToFile = true;
                pd.PrintController = new StandardPrintController();
                Process.Start(Application.StartupPath);
            }

            pd.PrintPage += new PrintPageEventHandler(this.PdPrintPage);
            pd.Print();
        }
        private void PdPrintPage(object sender, PrintPageEventArgs e)
        {
            //デバイスコンテキスト取得
            IntPtr hdc = e.Graphics.GetHdc();
            PrintImage(hdc);
            pageCount++;
            e.HasMorePages = true;
            //デバイスコンテキストの開放
            e.Graphics.ReleaseHdc(hdc);
            if (dt.Rows.Count <= pageCount)
            {
                //次のページがないことを通知する
                e.HasMorePages = false;
            }
        }
        #endregion

        #region 印刷イメージ作成メソッド
        /// <summary>
        /// 印刷イメージ作成メソッド
        /// </summary>
        /// <param name="hdc"></param>
        private void PrintImage(IntPtr hdc)
        {
            //バーコードの印字 バーコードの場合は9番目の引数を0にする必要あり
            String barString = dt.Rows[pageCount][0].ToString();
            IntPtr barFont1 = CreateFont(attributeList[0], attributeList[1], 0, 0, 0, false, false, false, 0, 0, 0, 0, 0, stringList[0][2]);
            SelectObject(hdc, barFont1);
            TextOut(hdc, attributeList[2], attributeList[3], barString, barString.Length);
            DeleteObject(barFont1);
            IntPtr barFont11 = CreateFont(attributeList[0] / 5, attributeList[1] / 2, 0, 0, 0, false, false, false, 128, 0, 0, 0, 0, stringList[0][1]);
            SelectObject(hdc, barFont11);
            TextOut(hdc, attributeList[2] + attributeList[0], attributeList[3] + attributeList[1] * 2, barString, Encoding.GetEncoding("Shift_JIS").GetByteCount(barString));
            DeleteObject(barFont11);

            //文字列1
            SetString(hdc, attributeList[4], attributeList[5], attributeList[6], attributeList[7], stringList[1][0], stringList[1][1], dt.Rows[pageCount][1].ToString(), stringList[1][2]);
            SetString(hdc, attributeList[8], attributeList[9], attributeList[10], attributeList[11], stringList[2][0], stringList[2][1], dt.Rows[pageCount][2].ToString(), stringList[2][2]);
            SetString(hdc, attributeList[12], attributeList[13], attributeList[14], attributeList[15], stringList[3][0], stringList[3][1], dt.Rows[pageCount][3].ToString(), stringList[3][2]);
            SetString(hdc, attributeList[16], attributeList[17], attributeList[18], attributeList[19], stringList[4][0], stringList[4][1], dt.Rows[pageCount][4].ToString(), stringList[4][2]);
            SetString(hdc, attributeList[20], attributeList[21], attributeList[22], attributeList[23], stringList[5][0], stringList[5][1], dt.Rows[pageCount][5].ToString(), stringList[5][2]);
            SetString(hdc, attributeList[24], attributeList[25], attributeList[26], attributeList[27], stringList[6][0], stringList[6][1], dt.Rows[pageCount][6].ToString(), stringList[6][2]);
            SetString(hdc, attributeList[28], attributeList[29], attributeList[30], attributeList[31], stringList[7][0], stringList[7][1], dt.Rows[pageCount][7].ToString(), stringList[7][2]);
            SetString(hdc, attributeList[32], attributeList[33], attributeList[34], attributeList[35], stringList[8][0], stringList[8][1], dt.Rows[pageCount][8].ToString(), stringList[8][2]);


            //縦文字囲い
            Rectangle(hdc, attributeList[40], attributeList[41], attributeList[42], attributeList[43]);
            //縦文字
            IntPtr mFont10 = CreateFont(attributeList[36], attributeList[37], 900, 0, 0, false, false, false, 128, 0, 0, 0, 0, stringList[9][2]);
            SelectObject(hdc, mFont10);
            String verticalString = dt.Rows[pageCount][9].ToString();
            TextOut(hdc, attributeList[38], attributeList[39], verticalString, Encoding.GetEncoding("Shift_JIS").GetByteCount(verticalString));
            DeleteObject(mFont10);

            //バーコードの印字 バーコードの場合は9番目の引数を0にする必要あり
            String barString2 = dt.Rows[pageCount][10].ToString();
            if (barString2.Trim() != "")
            {
                IntPtr barFont2 = CreateFont(attributeList[44], attributeList[45], 0, 0, 0, false, false, false, 0, 0, 0, 0, 0, stringList[10][2]);
                SelectObject(hdc, barFont2);
                TextOut(hdc, attributeList[46], attributeList[47], barString2, barString2.Length);
                DeleteObject(barFont2);
            }
        }
        private void SetString(IntPtr hdc, int h, int w, int x, int y, string hyou, string hyouFont, string data, string dataFont)
        {

            IntPtr mFont = CreateFont(h, w, 0, 0, 0, false, false, false, 128, 0, 0, 0, 0, dataFont);
            SelectObject(hdc, mFont);
            TextOut(hdc, x + w * 6, y, data, Encoding.GetEncoding("Shift_JIS").GetByteCount(data));
            DeleteObject(mFont);
            IntPtr labelFont = CreateFont(h, w, 0, 0, 0, false, false, false, 128, 0, 0, 0, 0, hyouFont);
            SelectObject(hdc, labelFont);
            TextOut(hdc, x, y, hyou, Encoding.GetEncoding("Shift_JIS").GetByteCount(hyou));
            DeleteObject(labelFont);
        }
        #endregion
    }
}
