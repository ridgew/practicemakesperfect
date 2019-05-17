using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.AddIn.Hosting;
using System.Windows.Threading;
using System.Threading;

namespace ApplicationHost
{
    /// <summary>
    /// The main host application. Simply shows a list of AddIns and the 
    /// results of running the Addin
    /// </summary>
    public partial class Window1 : Window
    {
        #region Data
        private AutomationHost automationHost;
        private HostView.NumberProcessorHostView addin;
        #endregion

        #region Ctor
        public Window1()
        {
            InitializeComponent();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads a list of all available AddIns
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {                     
            string path = Environment.CurrentDirectory;            
            AddInStore.Update(path);

            string[] s = AddInStore.RebuildAddIns(path);

            IList<AddInToken> tokens = AddInStore.FindAddIns(typeof(HostView.NumberProcessorHostView), path);
            lstAddIns.ItemsSource = tokens;
            automationHost = new AutomationHost(progressBar);
        }


        /// <summary>
        /// Use the selected AddIn
        /// </summary>
        private void btnUseAddin_Click(object sender, RoutedEventArgs e)
        {
            if (lstAddIns.SelectedIndex != -1)
            {
                // get selected addin
                AddInToken token = (AddInToken)lstAddIns.SelectedItem;
                addin = token.Activate<HostView.NumberProcessorHostView>(AddInSecurityLevel.Internet);
                addin.Initialize(automationHost);

                // process addin on new thread
                Thread thread = new Thread(RunBackgroundAddIn);
                thread.Start();
            }
            else
                MessageBox.Show("You need to select an addin first");


        }

        /// <summary>
        /// Runs Selected AddIn new thread
        /// </summary>
        private void RunBackgroundAddIn()
        {            
            // Do the work.
            List<int> numbersProcessed = addin.ProcessNumbers(1, 20);
            
            // update UI on UI thread
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate()
                    {
                        lstNumbers.ItemsSource = numbersProcessed;
                        progressBar.Value = 0;

                        // Release the add-in
                        addin = null;
                    }
                );                            
        }

        /// <summary>
        /// Scoll list of results
        /// </summary>
        private void lstNumbers_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            svNumbers.ScrollToVerticalOffset(svNumbers.VerticalOffset - (double)e.Delta);
        }
        #endregion
    }


    /// <summary>
    /// A wrapper class that allows the reported progress within the
    /// <see cref="HostView.HostObject">host view </see> to display
    /// progress on a ProgressBar within the host app
    /// </summary>
    internal class AutomationHost : HostView.HostObject
    {
        #region Data
        private ProgressBar progressBar;
        #endregion

        #region Ctor
        public AutomationHost(ProgressBar progressBar)
        {
            this.progressBar = progressBar;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Override <see cref="HostView.HostObject">host view </see>
        /// ReportProgress method
        /// </summary>
        /// <param name="progressPercent"></param>
        public override void ReportProgress(int progressPercent)
        {
            // Update the UI on the UI thread.
            progressBar.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate()
                {
                    progressBar.Value = progressPercent;
                }
            );
        }
        #endregion
    }
}
