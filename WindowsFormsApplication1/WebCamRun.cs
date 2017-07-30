using Bin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class WebCamRun : Form
    {

        #region members

        ColorTracking colorTracking;
        private bool stateRun;

        #endregion

        public WebCamRun()
        {
            InitializeComponent();
            colorTracking = new ColorTracking();
            ScanWebCam();
        }


        #region Object Methods

        private void button2_Click(object sender, EventArgs e)
        {
            colorTracking.Dispose();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Run();
        }
        private void panel1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            panel1.BackColor = colorDialog.Color;

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
            //colorTracking.SelectedWebCam = comboBox1.SelectedIndex;
        }

        #endregion

        #region Private Methods

        private void Run()
        {
            colorTracking.OpenCam();
            colorTracking.Frame = int.Parse(numericUpDown1.Value.ToString());
            colorTracking.FilterColorRGB = new AForge.Imaging.RGB(panel1.BackColor);
        }
        private void ScanWebCam()
        {
            foreach (WebCam webCam in colorTracking.WebCamList)
            {
                comboBox1.Items.Add(webCam.Name);
            }
        }

        #endregion




    }
}
