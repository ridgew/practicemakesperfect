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
            this.SuspendLayout();
            // 
            // comboBox_Com
            // 
            this.comboBox_Com.FormattingEnabled = true;
            this.comboBox_Com.Location = new System.Drawing.Point(149, 58);
            this.comboBox_Com.Name = "comboBox_Com";
            this.comboBox_Com.Size = new System.Drawing.Size(121, 20);
            this.comboBox_Com.TabIndex = 0;
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
            this.comboBox_comPl.Location = new System.Drawing.Point(149, 104);
            this.comboBox_comPl.Name = "comboBox_comPl";
            this.comboBox_comPl.Size = new System.Drawing.Size(121, 20);
            this.comboBox_comPl.TabIndex = 0;
            // 
            // textBox_TestTxt
            // 
            this.textBox_TestTxt.Location = new System.Drawing.Point(149, 154);
            this.textBox_TestTxt.Multiline = true;
            this.textBox_TestTxt.Name = "textBox_TestTxt";
            this.textBox_TestTxt.Size = new System.Drawing.Size(242, 45);
            this.textBox_TestTxt.TabIndex = 1;
            // 
            // button_Test
            // 
            this.button_Test.Location = new System.Drawing.Point(158, 227);
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
            this.label1.Location = new System.Drawing.Point(58, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "可用串口";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 163);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "扫码数据";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(58, 107);
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
            this.cbxCard12.Location = new System.Drawing.Point(306, 104);
            this.cbxCard12.Name = "cbxCard12";
            this.cbxCard12.Size = new System.Drawing.Size(72, 16);
            this.cbxCard12.TabIndex = 4;
            this.cbxCard12.Text = "12位IC卡";
            this.cbxCard12.UseVisualStyleBackColor = true;
            this.cbxCard12.CheckedChanged += new System.EventHandler(this.cbxCard12_CheckedChanged);
            // 
            // COMForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 286);
            this.Controls.Add(this.cbxCard12);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
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
    }
}