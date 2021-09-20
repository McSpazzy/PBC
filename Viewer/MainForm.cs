using System;
using System.Drawing.Imaging;
using System.Windows.Forms;
using PBC2PNG;
using PBCLib;

namespace Viewer
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            var file = @"E:\FldOutE00.pbc";

            var pbc = new PBC(file);

            var bm = PBCDraw.Draw(pbc, 16, true, true);

            bm.Save(file.Replace("pbc", "png"), ImageFormat.Png);

            picOut.Image = bm;
        }

    }
}
