namespace COM_TEST
{
    partial class COMForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox_Com = new System.Windows.Forms.ComboBox();
            this.comboBox_comPl = new System.Windows.Forms.ComboBox();
            this.textBox_TestTxt = new System.Windows.Forms.TextBox();
            this.button_Test = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbxCard12 = new System.Windows.Forms.CheckBox();
            this.cbxDataBits = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cbxStopBits = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.cbxParity = new System.Windows.Forms.ComboBox();
            this.cbxHandShaking = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBox_Com
            // 
            this.comboBox_Com.FormattingEnabled = true;
            this.comboBox_Com.Location = new System.Drawing.Point(99, 21);
            this.comboBox_Com.Name = "comboBox_Com";
            this.comboBox_Com.Size = new System.Drawing.Size(121, 20);
            this.comboBox_Com.TabIndex = 0;
            this.comboBox_Com.Text = "COM8";
            // 
            // comboBox_comPl
            // 
            this.comboBox_comPl.FormattingEnabled = true;
            this.comboBox_comPl.Items.AddRange(new object[] {
            "2400",
            "4800",
            "7200",
            "9600",
            "14400 ",
            "19200",
            "28800",
            "38400",
            "57600",
            "115200",
            "128000",
            "230400",
            "460800",
            "921600"});
            this.comboBox_comPl.Location = new System.Drawing.Point(328, 21);
            this.comboBox_comPl.Name = "comboBox_comPl";
            this.comboBox_comPl.Size = new System.Drawing.Size(121, 20);
            this.comboBox_comPl.TabIndex = 0;
            this.comboBox_comPl.Text = "9600";
            // 
            // textBox_TestTxt
            // 
            this.textBox_TestTxt.Location = new System.Drawing.Point(99, 140);
            this.textBox_TestTxt.Multiline = true;
            this.textBox_TestTxt.Name = "textBox_TestTxt";
            this.textBox_TestTxt.Size = new System.Drawing.Size(363, 85);
            this.textBox_TestTxt.TabIndex = 1;
            // 
            // button_Test
            // 
            this.button_Test.Location = new System.Drawing.Point(177, 240);
            this.button_Test.Name = "button_Test";
            this.button_Test.Size = new System.Drawing.Size(140, 23);
            this.button_Test.TabIndex = 2;
            this.button_Test.Text = "测试";
            this.button_Test.UseVisualStyleBackColor = true;
            this.button_Test.Click += new System.EventHandler(this.button_Test_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "可用串口";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "扫码数据";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "选择波动率";
            // 
            // cbxCard12
            // 
            this.cbxCard12.AutoSize = true;
            this.cbxCard12.Checked = true;
            this.cbxCard12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbxCard12.Location = new System.Drawing.Point(377, 244);
            this.cbxCard12.Name = "cbxCard12";
            this.cbxCard12.Size = new System.Drawing.Size(72, 16);
            this.cbxCard12.TabIndex = 4;
            this.cbxCard12.Text = "12位IC卡";
            this.cbxCard12.UseVisualStyleBackColor = true;
            this.cbxCard12.CheckedChanged += new System.EventHandler(this.cbxCard12_CheckedChanged);
            // 
            // cbxDataBits
            // 
            this.cbxDataBits.FormattingEnabled = true;
            this.cbxDataBits.Items.AddRange(new object[] {
            "8",
            "7"});
            this.cbxDataBits.Location = new System.Drawing.Point(99, 61);
            this.cbxDataBits.Name = "cbxDataBits";
            this.cbxDataBits.Size = new System.Drawing.Size(121, 20);
            this.cbxDataBits.TabIndex = 5;
            this.cbxDataBits.Text = "8";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Data Bits";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(258, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 12);
            this.label5.TabIndex = 3;
            this.label5.Text = "Stop Bits";
            // 
            // cbxStopBits
            // 
            this.cbxStopBits.FormattingEnabled = true;
            this.cbxStopBits.Items.AddRange(new object[] {
            "None",
            "One",
            "Two",
            "OnePointFive"});
            this.cbxStopBits.Location = new System.Drawing.Point(328, 61);
            this.cbxStopBits.Name = "cbxStopBits";
            this.cbxStopBits.Size = new System.Drawing.Size(121, 20);
            this.cbxStopBits.TabIndex = 5;
            this.cbxStopBits.Text = "One";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 99);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 3;
            this.label6.Text = "Parity";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(251, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "HandShaking";
            // 
            // cbxParity
            // 
            this.cbxParity.FormattingEnabled = true;
            this.cbxParity.Items.AddRange(new object[] {
            "None",
            "Even",
            "Mark",
            "Odd",
            "Space"});
            this.cbxParity.Location = new System.Drawing.Point(98, 96);
            this.cbxParity.Name = "cbxParity";
            this.cbxParity.Size = new System.Drawing.Size(121, 20);
            this.cbxParity.TabIndex = 5;
            this.cbxParity.Text = "None";
            // 
            // cbxHandShaking
            // 
            this.cbxHandShaking.FormattingEnabled = true;
            this.cbxHandShaking.Items.AddRange(new object[] {
            "None",
            "XOnXOff",
            "RequestToSend",
            "RequestToSendXOnXOff"});
            this.cbxHandShaking.Location = new System.Drawing.Point(328, 96);
            this.cbxHandShaking.Name = "cbxHandShaking";
            this.cbxHandShaking.Size = new System.Drawing.Size(127, 20);
            this.cbxHandShaking.TabIndex = 5;
            this.cbxHandShaking.Text = "None";
            // 
            // COMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 286);
            this.Controls.Add(this.cbxHandShaking);
            this.Controls.Add(this.cbxStopBits);
            this.Controls.Add(this.cbxParity);
            this.Controls.Add(this.cbxDataBits);
            this.Controls.Add(this.cbxCard12);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Test);
            this.Controls.Add(this.textBox_TestTxt);
            this.Controls.Add(this.comboBox_comPl);
            this.Controls.Add(this.comboBox_Com);
            this.Name = "COMForm";
            this.Text = "COMForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.COMForm_FormClosing);
            this.Load += new System.EventHandler(this.COMForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_Com;
        private System.Windows.Forms.ComboBox comboBox_comPl;
        private System.Windows.Forms.TextBox textBox_TestTxt;
        private System.Windows.Forms.Button button_Test;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox cbxCard12;
        private System.Windows.Forms.ComboBox cbxDataBits;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbxStopBits;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cbxParity;
        private System.Windows.Forms.ComboBox cbxHandShaking;
    }
}