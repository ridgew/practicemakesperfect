using Common;
using System;
using System.Windows.Forms;

namespace USBHookTes
{
    public partial class Form1 : Form
    {
        BardCodeHook BarCode = new BardCodeHook();

        public Form1()
        {
            InitializeComponent();
            BarCode.BarCodeEvent += new BardCodeHook.BardCodeDeletegate(BarCode_BarCodeEvent);
        }

        private delegate void ShowInfoDelegate(BardCodeHook.BarCodes barCode);
        private void ShowInfo(BardCodeHook.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                textBox1.Text = barCode.KeyName;
                textBox2.Text = barCode.VirtKey.ToString();
                textBox3.Text = barCode.ScanCode.ToString();
                textBox4.Text = barCode.Ascll.ToString();
                textBox5.Text = barCode.Chr.ToString();
                textBox6.Text = barCode.IsValid ? barCode.BarCode : "";//是否为扫描枪输入，如果为true则是 否则为键盘输入
                textBox7.Text += " " + barCode.KeyName;
                lblMsg.Text = barCode.IsValid ? "(已扫码)" : "(未扫码)";
            }
        }

        //C#中判断扫描枪输入与键盘输入
        //Private DateTime _dt = DateTime.Now;  //定义一个成员函数用于保存每次的时间点
        //private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    DateTime tempDt = DateTime.Now;          //保存按键按下时刻的时间点
        //    TimeSpan ts = tempDt .Subtract(_dt);     //获取时间间隔
        //    if (ts.Milliseconds > 50)                           //判断时间间隔，如果时间间隔大于50毫秒，则将TextBox清空
        //        textBox1.Text = "";
        //    dt = tempDt ;
        //}

        void BarCode_BarCodeEvent(BardCodeHook.BarCodes barCode)
        {
            ShowInfo(barCode);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            BarCode.Start();
            textBox8.Focus();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            BarCode.Stop();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text.Length > 0)
            {
                MessageBox.Show("条码长度：" + textBox6.Text.Length + "\n条码内容：" + textBox6.Text, "系统提示");
            }
        }
    }
}
