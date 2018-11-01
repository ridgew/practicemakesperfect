using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LoadMask
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object[] objs = new object[2];
            objs[0] = "a";
            objs[1] = "b";
            LoadingForm f = new LoadingForm(objs, this.openFileCallBack);
            f.ShowDialog(this);
        }

        public delegate void MethodParamInvoker(string val);

        private void openFileCallBack(object obj, LoadingForm form)
        {
            try
            {
                object[] objs = (object[])obj;

                string fileName = objs[1].ToString();
                MethodParamInvoker mpi = new MethodParamInvoker(form.setLoadingText);
                this.BeginInvoke(mpi, "加载文件.");

                System.Threading.Thread.Sleep(60 * 1000);
            }
            catch (Exception e)
            {
                MessageBox.Show("openFileCallBack:" + e.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            object param = new object();
            OpaqueCommand cmd = new OpaqueCommand();
            cmd.ShowOpaqueLayer(this, 123, true);
            Thread thread = new Thread(new ParameterizedThreadStart(this.loadDataFun));//有参线程函数
            thread.IsBackground = true;
            thread.Start(param);//传线程函数参数.
        }

        private void loadDataFun(object callParam)
        {
            try
            {
                
                Thread.Sleep(30 * 1000);

                MethodInvoker mi = new MethodInvoker(this.Close);
                this.BeginInvoke(mi);
            }
            catch (Exception e)
            {
                MessageBox.Show("loadDataFun:" + e.Message);
            }

        }
    }
}
