using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Imaging;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        #region Members

        Bin.ColorTracking colorTracking;
        bool runState;
        Color defaultForumColor;

        #endregion

        #region Constructor

        public Form1()
        {
            InitializeComponent();
            defaultForumColor = this.BackColor;
            colorTracking = new Bin.ColorTracking();
            colorTracking.OnRenderFrame += ColorTracking_OnRenderFrame;
            colorTracking.OnTracking += ColorTracking_OnTracking;
        }

        private void ColorTracking_OnRenderFrame(Bitmap bitmap, EventArgs e)
        {
            //pictureBox2.Image = bitmap;
        }

        private void ColorTracking_OnTracking(Rectangle findObject, EventArgs e)
        {
            Cursor.Position = new System.Drawing.Point(findObject.X + (findObject.Width / 2), findObject.Y + (findObject.Height / 2));
        }

        #endregion

        #region Object Methods

        private void button1_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                Bitmap bitmap = new Bitmap(pictureBox1.Image);
                colorTracking.FilterColorRGB = new RGB(bitmap.GetPixel(e.X, e.Y));
                textBox1.BackColor = bitmap.GetPixel(e.X, e.Y);
                textBox1.ForeColor = Color.FromArgb(bitmap.GetPixel(e.X, e.Y).A, (byte)~bitmap.GetPixel(e.X, e.Y).R, (byte)~bitmap.GetPixel(e.X, e.Y).G, (byte)~bitmap.GetPixel(e.X, e.Y).B);
            }
            catch
            {
            }
        }

        private void theardSample_Tick(object sender, EventArgs e)
        {
            colorTracking.Tracke((Bitmap)pictureBox1.Image);
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            textBox1.BackColor = colorDialog.Color;
            textBox1.ForeColor = textBox1.ForeColor = Color.FromArgb(colorDialog.Color.A, (byte)~colorDialog.Color.R, (byte)~colorDialog.Color.G, (byte)~colorDialog.Color.B);

        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == DialogResult.OK)
                pictureBox1.Image = AForge.Imaging.Image.FromFile(openDialog.FileName);

        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R)
                Run();
            if (e.Control && e.KeyCode == Keys.S)
                Stop();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            WebCamRun webcamView = new WebCamRun();
            webcamView.Show();
        }

        #endregion

        #region Private Methods

        private void RunStateControl()
        {
            if (runState)
                button1.Enabled = false;
            else
                button1.Enabled = true;
        }
        private void Run()
        {
            runState = true;
            //pictureBox2.Image = pictureBox1.Image;
            colorTracking.FilterColorRGB = new RGB(textBox1.BackColor);
            BackColor = defaultForumColor;
            theardSample.Start();
            RunStateControl();
        }
        private void Stop()
        {
            runState = false;
            pictureBox2.Image = null;
            BackColor = Color.Red;
            theardSample.Stop();
            RunStateControl();
        }

        #endregion

    }
}
