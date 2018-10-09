using System.Data;
using System.Windows.Forms;

namespace WinForm_Scaner
{

    public partial class Scaner : Form
    {
        private ScanerHook listener = new ScanerHook();
        DataTable dgv_lst = new DataTable();

        public Scaner()
        {
            InitializeComponent();

            dgv_lst.Columns.Add(new DataColumn("KeyDownCount", typeof(int)));
            dgv_lst.Columns.Add(new DataColumn("message", typeof(int)));
            dgv_lst.Columns.Add(new DataColumn("paramH", typeof(int)));
            dgv_lst.Columns.Add(new DataColumn("paramL", typeof(int)));
            dgv_lst.Columns.Add(new DataColumn("CurrentChar", typeof(string)));
            dgv_lst.Columns.Add(new DataColumn("Result", typeof(string)));
            dgv_lst.Columns.Add(new DataColumn("isShift", typeof(bool)));
            dgv_lst.Columns.Add(new DataColumn("CurrentKey", typeof(string)));

            listener.ScanerEvent += Listener_ScanerEvent;
        }

        private void Listener_ScanerEvent(ScanerHook.ScanerCodes codes)
        {
            dgv_lst.Rows.Add(new object[] {
                codes.KeyDownCount,
                codes.Event.message,
                codes.Event.paramH,
                codes.Event.paramL,
                codes.CurrentChar,
                codes.Result,
                codes.isShift,
                codes.CurrentKey });
        }

        private void Scaner_Load(object sender, System.EventArgs e)
        {
            dataGridView1.DataSource = dgv_lst;
            listener.Start();
        }

        private void Scaner_FormClosing(object sender, FormClosingEventArgs e)
        {
            listener.Stop();
        }
    }


}
