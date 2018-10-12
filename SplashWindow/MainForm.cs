using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SplashWindow
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(string[] args)
            : this()
        {
            if (args.Length > 0 && args[0] != null)
                this.CommandLineParam = args[0];
        }

        string CommandLineParam;

        private void MainForm_Activated(object sender, EventArgs e)
        {
            this.TopMost = false;
        }
    }
}
