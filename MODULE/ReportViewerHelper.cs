using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AAA
{
    public class ReportViewerHelper
    {
        private int m_currentPageIndex;
        private IList<Stream> m_streams;
        public LocalReport report = new LocalReport();
        private PrintDocument printDoc = new PrintDocument();
        private Boolean IsYoko;
        private string paperSize;

        public string width = "21";
        public string height = "29.7";
        public string top = "0.5";
        public string bottom = "0.5";
        public string left = "2";
        public string right = "2";

        #region コンストラクタ
        public ReportViewerHelper(string paperSize, string printerName = "Microsoft XPS Document Writer", Boolean IsYoko = false)
        {
            printDoc.PrinterSettings.PrinterName = printerName;
            this.IsYoko = IsYoko;
            this.paperSize = paperSize;

            //XPSファイルとして保存する場合
            if (printerName == "Microsoft XPS Document Writer")
            {
                printDoc.PrinterSettings.PrintFileName = "test.xps";
                Process.Start(Application.StartupPath);
                printDoc.PrinterSettings.PrintToFile = true;
            }
            report.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
        }
        #endregion

        #region 処理
        // Routine to provide to the report renderer, in order to
        //    save an image for each page of the report.
        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }
        // Export the given report as an EMF (Enhanced Metafile) file.
        public void Export()
        {
            string deviceInfo =
              @"<DeviceInfo> " +
                "<OutputFormat>EMF</OutputFormat> " +
                "<PageWidth>" + width + "cm</PageWidth>" +
                "<PageHeight>" + height + "cm</PageHeight>" +
                "<MarginTop>" + top + "cm</MarginTop>" +
                "<MarginLeft>" + left + "cm</MarginLeft> " +
                "<MarginRight>" + right + "cm</MarginRight> " +
                "<MarginBottom>" + bottom + "cm</MarginBottom> " +
            "</DeviceInfo>";
            if (IsYoko)
            {
                deviceInfo =
              @"<DeviceInfo> " +
                "<OutputFormat>EMF</OutputFormat> " +
                "< PageWidth>" + height + "cm</PageWidth>" +
                "<PageHeight>" + width + "cm</PageHeight>" +
                "<MarginTop>" + top + "cm</MarginTop>" +
                "<MarginLeft>" + left + "cm</MarginLeft> " +
                "<MarginRight>" + right + "cm</MarginRight> " +
                "<MarginBottom>" + bottom + "cm</MarginBottom> " +
            "</DeviceInfo>";
            }
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
                stream.Position = 0;
        }
        // Handler for PrintPageEvents
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        public void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                // 用紙の向きを設定(横：true、縦：false)
                printDoc.DefaultPageSettings.Landscape = IsYoko;
                #region サイズ用紙設定
                PaperKind pk = new PaperKind();
                switch (paperSize)
                {
                    case "A4":
                        pk = PaperKind.A4;
                        break;
                    case "B4":
                        pk = PaperKind.B4;
                        break;
                    case "B5":
                        pk = PaperKind.B5;
                        break;
                    case "A5":
                        pk = PaperKind.A5;
                        break;
                }
                foreach (PaperSize ps in printDoc.PrinterSettings.PaperSizes)
                {
                    if (ps.Kind == pk)
                    {
                        printDoc.DefaultPageSettings.PaperSize = ps;
                    }
                }
                #endregion
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }
        /// <summary>
        /// 印刷の実行
        /// </summary>
        public void Run()
        {
            try
            {
                //reportをemfに変換する
                Export();
                //印刷を実行する
                Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷に失敗しました：" + ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }
        #endregion

        /// <summary>
        ///  印刷設定1
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="bs"></param>
        /// <param name="datasetName"></param>
        /// <param name="paramList"></param>
        public void Run(string reportPath, BindingSource bs, string datasetName, List<ReportParameter> paramList = null)
        {
            try
            {
                report.ReportPath = reportPath;
                ReportDataSource rds = new ReportDataSource();
                rds.Name = datasetName;
                rds.Value = bs;
                report.DataSources.Add(rds);
                if (paramList != null)
                {
                    report.SetParameters(paramList);
                }
                Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷に失敗しました：" + ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }

        /// <summary>
        ///  印刷設定2
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="dt"></param>
        /// <param name="datasetName"></param>
        /// <param name="paramList"></param>
        public void Run(string reportPath, DataTable dt, string datasetName, List<ReportParameter> paramList = null)
        {
            try
            {
                report.ReportPath = reportPath;
                ReportDataSource rds = new ReportDataSource();
                rds.Name = datasetName;
                rds.Value = dt;
                report.DataSources.Add(rds);
                if (paramList != null)
                {
                    report.SetParameters(paramList);
                }
                Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷に失敗しました：" + ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }

        /// <summary>
        /// ローカルレポートから印刷を実行する
        /// </summary>
        /// <param name="lr"></param>
        public void Run(LocalReport lr)
        {
            try
            {
                report = lr;
                Run();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷に失敗しました：" + ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }

        /// <summary>
        ///  印刷を実行する
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="dt"></param>
        /// <param name="datasetName"></param>
        /// <param name="paramList"></param>
        public void OpenForm(string reportPath, DataTable dt, string datasetName, List<ReportParameter> paramList)
        {
            try
            {
                ReportViewer rv = new ReportViewer();
                report = rv.LocalReport;

                report.ReportPath = reportPath;
                ReportDataSource rds = new ReportDataSource();
                rds.Name = datasetName;
                rds.Value = dt;
                report.DataSources.Add(rds);

                //リポートフォーム表示する
                Form form = new Form();
                form.Show();
                form.WindowState = FormWindowState.Maximized;
                form.Controls.Add(rv);
                rv.Dock = DockStyle.Fill;
                rv.SetDisplayMode(DisplayMode.PrintLayout);
                rv.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷に失敗しました：" + ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reportPath"></param>
        /// <param name="bs"></param>
        /// <param name="datasetName"></param>
        /// <param name="paramList"></param>
        public void OpenForm(string reportPath, BindingSource bs, string datasetName, List<ReportParameter> paramList = null)
        {
            try
            {
                ReportViewer rv = new ReportViewer();
                report = rv.LocalReport;

                report.ReportPath = reportPath;
                ReportDataSource rds = new ReportDataSource();
                rds.Name = datasetName;
                rds.Value = bs;
                report.DataSources.Add(rds);

                //リポートフォーム表示する
                Form form = new Form();
                form.Show();
                form.WindowState = FormWindowState.Maximized;
                form.Controls.Add(new Button() { Text = "aa" });
                form.Controls.Add(rv);
                //rv.Dock = DockStyle.Fill;
                rv.SetDisplayMode(DisplayMode.PrintLayout);

                rv.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("印刷に失敗しました：" + ex.Message + Environment.NewLine + ex.InnerException.Message);
            }
        }

        /// <summary>
        /// サブレポート用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LocalReport_SubreportProcessing(object sender, Microsoft.Reporting.WinForms.SubreportProcessingEventArgs e)
        {
            //サブレポートにサブレポートの番号がわたるように設定してあるので、
            LocalReport parentReport = (LocalReport)sender;

            //親レポートに含まれるデータソースを名前で検索できるようにDictionaryに
            var parentSources = new Dictionary<string, ReportDataSource>();
            foreach (ReportDataSource parentSource in parentReport.DataSources)
            {
                parentSources.Add(parentSource.Name, parentSource);
            }

            //サブレポート内に必要なデータソース名を調べる
            foreach (string sourceName in e.DataSourceNames)
            {
                if (parentSources.ContainsKey(sourceName))
                {
                    //サブレポート内にあるデータソース一覧に親レポートと同じデータソース名が見つかったら
                    //サブレポートにも親レポートのデータソースを登録する
                    e.DataSources.Add(parentSources[sourceName]);
                }
                else
                {
                    //親レポートにない場合や名前が違う場合は継承させられないので
                    //必要なデータソースを追加してやる
                    //DataTable tableDummy=new DataTable();
                    //ReportDataSource subSource = new ReportDataSource(sourceName, tableDummy);
                }
            }
        }

        #region デストラクタ
        /// <summary>
        /// デストラクタ
        /// </summary>
        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
        #endregion
    }
}
