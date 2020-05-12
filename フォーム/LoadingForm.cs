using System;
using System.Drawing;
using System.Windows.Forms;

namespace 受託外注サブシステム.VIEW
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
        }

        private void LoadingForm_Load(object sender, EventArgs e)
        {
            // フォームの境界線、タイトルバーを無しに設定
            this.FormBorderStyle = FormBorderStyle.None;
            // フォームの背景を黒に設定
            //this.BackColor = Color.White;
            // フォームの不透明度を60%に設定（半透明化）
            this.Opacity = 60;

            int radius = 20;
            int diameter = radius * 2;
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();

            // 左上
            gp.AddPie(0, 0, diameter, diameter, 180, 90);
            // 右上
            gp.AddPie(this.Width - diameter, 0, diameter, diameter, 270, 90);
            // 左下
            gp.AddPie(0, this.Height - diameter, diameter, diameter, 90, 90);
            // 右下
            gp.AddPie(this.Width - diameter, this.Height - diameter, diameter, diameter, 0, 90);
            // 中央
            gp.AddRectangle(new Rectangle(radius, 0, this.Width - diameter, this.Height));
            // 左
            gp.AddRectangle(new Rectangle(0, radius, radius, this.Height - diameter));
            // 右
            gp.AddRectangle(new Rectangle(this.Width - radius, radius, radius, this.Height - diameter));

            this.Region = new Region(gp);
        }

        public Boolean isLoding = true;
        private void Loading()
        {
            do
            {
                Application.DoEvents();
            } while (isLoding);
            this.Close();
        }

        private void LoadingForm_Shown(object sender, EventArgs e)
        {
            Loading();
        }
    }
}
