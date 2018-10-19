using System;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace COM_TEST
{
    //https://blog.csdn.net/FairyStepWGL/article/details/51315822

    public partial class COMForm : Form
    {
        CommBar commBar;

        bool buttonBool = false;//指示按钮是开还是关

        public COMForm()
        {
            InitializeComponent();

            commBar = new CommBar();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void COMForm_Load(object sender, EventArgs e)
        {
            this.comboBox_Com.Items.Clear();
            this.comboBox_Com.Items.Add("请选择COM口");
            string[] comNames = commBar.GetComName();
            for (int i = 0; i < comNames.Length; i++)
            {
                this.comboBox_Com.Items.Add(comNames[i]);
            }
            this.comboBox_Com.SelectedIndex = 0;
            comboBox_comPl.SelectedItem = "9600";
        }

        private void button_Test_Click(object sender, EventArgs e)
        {
            //关闭
            if (buttonBool)
            {
                buttonBool = false;
                commBar.Close();
                this.button_Test.Text = "点击测试";
            }
            //开始
            else if (!buttonBool)
            {
                buttonBool = true;
                this.button_Test.Text = "正在测试……";

                //注册一该串口
                commBar.SerialPortValue(this.comboBox_Com.Text,
                     Convert.ToInt32(this.comboBox_comPl.Text),
                     Convert.ToInt32(cbxDataBits.Text.Trim()),
                    (StopBits)Enum.Parse(typeof(StopBits), cbxStopBits.Text.Trim()),
                    (Parity)Enum.Parse(typeof(Parity), cbxParity.Text.Trim()),
                    (Handshake)Enum.Parse(typeof(Parity), cbxHandShaking.Text.Trim())
                    );
                //打开串口
                if (commBar.Open())
                    //关联事件处理程序
                    commBar.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(serialPort_DataReceived);
            }
        }

        //用来为文本框赋值
        private void CodeText(CommBar commBar)
        {
            this.textBox_TestTxt.Text = commBar.Code;
        }

        //委托，指向CodeText方法
        private delegate void ModifyButton_dg(CommBar commBar);

        //串口接收接收事件处理程序
        //每当串口讲到数据后激发
        void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(100);
            byte[] m_recvBytes = new byte[commBar.serialPort.BytesToRead];//定义缓冲区大小
            int result = commBar.serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);//从串口读取数据
            if (result <= 0)
                return;
            if (cbxCard12.Checked)
                commBar.Code = ByteToCardNum(m_recvBytes);//对数据进行转换
            else
                commBar.Code = Encoding.ASCII.GetString(m_recvBytes, 0, m_recvBytes.Length);//对数据进行转换
            this.Invoke(new ModifyButton_dg(CodeText), commBar);//调用委托，将值传给文本框
            commBar.serialPort.DiscardInBuffer();
        }

        private static string ByteToCardNum(byte[] byteAr)
        {
            string str = string.Empty;
            try
            {
                if ((byteAr == null) || (byteAr.Length != 12))
                {
                    return str;
                }

                byte[] buffer = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    buffer[i] = byteAr[i + 7];
                }

                return BitConverter.ToUInt32(buffer, 0).ToString();
            }
            catch (Exception)
            {

            }
            return str;
        }

        private void COMForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            commBar.Close();
            commBar.serialPort.Dispose();
        }

        private void cbxCard12_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxCard12.Checked)
                comboBox_comPl.SelectedItem = "9600";
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

}
