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

        /// <summary>
		/// Occurs when [mainform is loaded and ready for action.].
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-06-29</remarks>
		private event EventHandler MainformLoadedEvent;

        private List<EventHandler> MainformLoadedDelegates = new List<EventHandler>();

        /// <summary>
        /// Occurs when [mainform loaded event].
        /// </summary>
        /// <remarks>Documented by Dev02, 2009-06-29</remarks>
        public event EventHandler MainformLoaded
        {
            add
            {
                MainformLoadedEvent += value;
                MainformLoadedDelegates.Add(value);
            }
            remove
            {
                MainformLoadedEvent -= value;
                MainformLoadedDelegates.Remove(value);
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            this.TopMost = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.SuspendIPC = true;

            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                e.Cancel = true;

                try
                {
                    Program.EndIPC();
                }
                finally
                {
                    Environment.Exit(-1);
                }
            }

            try
            {
                Program.EndIPC();
                e.Cancel = false;
            }
            finally
            {
                //reactivate IPC server in case the closing process was canceled
                Program.SuspendIPC = false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //[ML-551] Main window not in foreground
            //It is important to focus MainForm once - otherwise, the SplashScreen has focus (because it was here first). 
            //SplashScreen closed => Windows focused the last used program
            Activate();

            BringToFront();
            TopMost = true;
            TopMost = false;

            if (MainformLoadedEvent != null)
            {
                OnMainformLoaded();
            }
        }

        /// <summary>
		/// Called when [mainform loaded].
		/// </summary>
		/// <remarks>Documented by Dev02, 2009-06-29</remarks>
		private void OnMainformLoaded()
        {
            Program.SuspendIPC = false;
            this.Refresh();

            MainformLoadedEvent(this, EventArgs.Empty);

            foreach (EventHandler e in MainformLoadedDelegates)
                MainformLoadedEvent -= e;

            MainformLoadedDelegates.Clear();
        }
    }
}
