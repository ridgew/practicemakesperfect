using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace COM_TEST
{
    /// <summary>
    /// 扫描枪工作类
    /// </summary>
    public class CommBar
    {
        //串口引用
        public SerialPort serialPort;

        //存储转换的数据值
        public string Code { get; set; }

        public CommBar()
        {
            serialPort = new SerialPort();
        }

        //串口状态
        public bool IsOpen
        {
            get
            {
                return serialPort.IsOpen;
            }
        }

        //打开串口
        public bool Open()
        {
            if (serialPort.IsOpen)
            {
                Close();
            }
            serialPort.Open();
            if (serialPort.IsOpen)
            {
                return true;
            }
            else
            {
                MessageBox.Show("串口打开失败！");
                return false;
            }
        }

        //关闭串口
        public void Close()
        {
            serialPort.Close();
        }

        //定入数据，这里没有用到
        public void WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                serialPort.Write(send, offSet, count);
            }
        }

        //获取可用串口
        public string[] GetComName()
        {
            string[] names = null;
            try
            {
                names = SerialPort.GetPortNames(); // 获取所有可用串口的名字
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("找不到串口");
            }
            return names;
        }

        //注册一个串口
        public void SerialPortValue(string portName, int baudRate)
        {
            //串口名
            serialPort.PortName = portName;
            //波特率
            serialPort.BaudRate = baudRate;
            //数据位
            serialPort.DataBits = 8;
            //两个停止位
            serialPort.StopBits = System.IO.Ports.StopBits.One;
            //无奇偶校验位
            serialPort.Parity = System.IO.Ports.Parity.None;
            serialPort.ReadTimeout = 100;
            //commBar.serialPort.WriteTimeout = -1;

            //serialPort.ReceivedBytesThreshold = 12;
            //serialPort.Handshake = Handshake.RequestToSend;
            //serialPort.ReadTimeout = 200;
            //serialPort.WriteTimeout = 200;

        }
    }
}
