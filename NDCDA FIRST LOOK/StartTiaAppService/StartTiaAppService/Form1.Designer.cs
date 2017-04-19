namespace StartTiaAppService
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.label3 = new System.Windows.Forms.Label();
            this.lblstate = new System.Windows.Forms.Label();
            this.txtlog = new System.Windows.Forms.RichTextBox();
            this.btnConn = new System.Windows.Forms.Button();
            this.btnDis = new System.Windows.Forms.Button();
            this.notifyIcon2 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip6 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.grpSetting = new System.Windows.Forms.GroupBox();
            this.txtprintUser = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtdatabase = new System.Windows.Forms.TextBox();
            this.cmbAuth = new System.Windows.Forms.ComboBox();
            this.txtserver = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.userid = new System.Windows.Forms.TextBox();
            this.pass = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label11 = new System.Windows.Forms.Label();
            this.contextMenuStrip6.SuspendLayout();
            this.grpSetting.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "Current State : ";
            // 
            // lblstate
            // 
            this.lblstate.AutoSize = true;
            this.lblstate.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstate.Location = new System.Drawing.Point(117, 13);
            this.lblstate.Name = "lblstate";
            this.lblstate.Size = new System.Drawing.Size(92, 16);
            this.lblstate.TabIndex = 1;
            this.lblstate.Text = "Connecting..";
            // 
            // txtlog
            // 
            this.txtlog.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtlog.Location = new System.Drawing.Point(468, 44);
            this.txtlog.Name = "txtlog";
            this.txtlog.Size = new System.Drawing.Size(436, 248);
            this.txtlog.TabIndex = 2;
            this.txtlog.Text = "";
            this.txtlog.WordWrap = false;
            this.txtlog.TextChanged += new System.EventHandler(this.txtlog_TextChanged);
            // 
            // btnConn
            // 
            this.btnConn.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConn.Location = new System.Drawing.Point(542, 309);
            this.btnConn.Name = "btnConn";
            this.btnConn.Size = new System.Drawing.Size(86, 26);
            this.btnConn.TabIndex = 8;
            this.btnConn.Text = "Start";
            this.btnConn.UseVisualStyleBackColor = true;
            this.btnConn.Click += new System.EventHandler(this.btnConn_Click);
            // 
            // btnDis
            // 
            this.btnDis.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDis.Location = new System.Drawing.Point(771, 309);
            this.btnDis.Name = "btnDis";
            this.btnDis.Size = new System.Drawing.Size(114, 26);
            this.btnDis.TabIndex = 9;
            this.btnDis.Text = "Stop";
            this.btnDis.UseVisualStyleBackColor = true;
            this.btnDis.Click += new System.EventHandler(this.btnDis_Click);
            // 
            // notifyIcon2
            // 
            this.notifyIcon2.ContextMenuStrip = this.contextMenuStrip6;
            this.notifyIcon2.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon2.Icon")));
            this.notifyIcon2.Text = "TiaERP@App Service";
            this.notifyIcon2.Visible = true;
            // 
            // contextMenuStrip6
            // 
            this.contextMenuStrip6.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.toolStripMenuItem2});
            this.contextMenuStrip6.Name = "contextMenuStrip6";
            this.contextMenuStrip6.Size = new System.Drawing.Size(99, 92);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(98, 22);
            this.toolStripMenuItem1.Text = "Edit";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Image = global::StartTiaAppService.Properties.Resources.blue_player_play1;
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.startToolStripMenuItem.Text = "Start";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Image = global::StartTiaAppService.Properties.Resources.blue_player_stop;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.stopToolStripMenuItem.Text = "Stop";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::StartTiaAppService.Properties.Resources.blue_go_out;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(98, 22);
            this.toolStripMenuItem2.Text = "Exit";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click_1);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // grpSetting
            // 
            this.grpSetting.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.grpSetting.Controls.Add(this.txtprintUser);
            this.grpSetting.Controls.Add(this.label10);
            this.grpSetting.Controls.Add(this.btnConnect);
            this.grpSetting.Controls.Add(this.label8);
            this.grpSetting.Controls.Add(this.label9);
            this.grpSetting.Controls.Add(this.txtdatabase);
            this.grpSetting.Controls.Add(this.cmbAuth);
            this.grpSetting.Controls.Add(this.txtserver);
            this.grpSetting.Controls.Add(this.groupBox2);
            this.grpSetting.Controls.Add(this.label5);
            this.grpSetting.Controls.Add(this.label4);
            this.grpSetting.Enabled = false;
            this.grpSetting.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSetting.Location = new System.Drawing.Point(14, 38);
            this.grpSetting.Name = "grpSetting";
            this.grpSetting.Size = new System.Drawing.Size(425, 311);
            this.grpSetting.TabIndex = 5;
            this.grpSetting.TabStop = false;
            this.grpSetting.Text = "Setting Window";
            // 
            // txtprintUser
            // 
            this.txtprintUser.Location = new System.Drawing.Point(162, 227);
            this.txtprintUser.Name = "txtprintUser";
            this.txtprintUser.Size = new System.Drawing.Size(232, 22);
            this.txtprintUser.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 230);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 14);
            this.label10.TabIndex = 11;
            this.label10.Text = "PrintQUser";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.Location = new System.Drawing.Point(157, 262);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(83, 35);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(25, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 14);
            this.label8.TabIndex = 10;
            this.label8.Text = "Database Name";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(25, 240);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(0, 14);
            this.label9.TabIndex = 6;
            // 
            // txtdatabase
            // 
            this.txtdatabase.Location = new System.Drawing.Point(162, 64);
            this.txtdatabase.Name = "txtdatabase";
            this.txtdatabase.Size = new System.Drawing.Size(232, 22);
            this.txtdatabase.TabIndex = 2;
            this.txtdatabase.Enter += new System.EventHandler(this.txtdatabase_Enter);
            this.txtdatabase.Leave += new System.EventHandler(this.txtdatabase_Leave);
            // 
            // cmbAuth
            // 
            this.cmbAuth.FormattingEnabled = true;
            this.cmbAuth.Items.AddRange(new object[] {
            "Windows Authentication",
            "SQL Server Authentication"});
            this.cmbAuth.Location = new System.Drawing.Point(162, 97);
            this.cmbAuth.Name = "cmbAuth";
            this.cmbAuth.Size = new System.Drawing.Size(232, 22);
            this.cmbAuth.TabIndex = 3;
            this.cmbAuth.SelectedIndexChanged += new System.EventHandler(this.cmbAuth_SelectedIndexChanged);
            this.cmbAuth.Enter += new System.EventHandler(this.cmbAuth_Enter);
            this.cmbAuth.Leave += new System.EventHandler(this.cmbAuth_Leave);
            // 
            // txtserver
            // 
            this.txtserver.Location = new System.Drawing.Point(162, 33);
            this.txtserver.Name = "txtserver";
            this.txtserver.Size = new System.Drawing.Size(232, 22);
            this.txtserver.TabIndex = 1;
            this.txtserver.TextChanged += new System.EventHandler(this.txtserver_TextChanged);
            this.txtserver.Enter += new System.EventHandler(this.txtserver_Enter);
            this.txtserver.Leave += new System.EventHandler(this.txtserver_Leave);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.userid);
            this.groupBox2.Controls.Add(this.pass);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(28, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(378, 83);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // userid
            // 
            this.userid.Location = new System.Drawing.Point(134, 15);
            this.userid.Name = "userid";
            this.userid.Size = new System.Drawing.Size(232, 22);
            this.userid.TabIndex = 4;
            this.userid.Enter += new System.EventHandler(this.userid_Enter);
            this.userid.Leave += new System.EventHandler(this.userid_Leave);
            // 
            // pass
            // 
            this.pass.Location = new System.Drawing.Point(134, 46);
            this.pass.Name = "pass";
            this.pass.PasswordChar = '*';
            this.pass.Size = new System.Drawing.Size(232, 22);
            this.pass.TabIndex = 5;
            this.pass.Enter += new System.EventHandler(this.pass_Enter);
            this.pass.Leave += new System.EventHandler(this.pass_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 49);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(69, 14);
            this.label7.TabIndex = 4;
            this.label7.Text = "Password";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(19, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 14);
            this.label6.TabIndex = 3;
            this.label6.Text = "User ID";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 100);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "Authentication";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 14);
            this.label4.TabIndex = 0;
            this.label4.Text = "SQL Server Name";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 300000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 60000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(475, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(0, 13);
            this.label11.TabIndex = 10;
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(934, 371);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.grpSetting);
            this.Controls.Add(this.btnDis);
            this.Controls.Add(this.btnConn);
            this.Controls.Add(this.txtlog);
            this.Controls.Add(this.lblstate);
            this.Controls.Add(this.label3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TiaERP@App Service";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.contextMenuStrip6.ResumeLayout(false);
            this.grpSetting.ResumeLayout(false);
            this.grpSetting.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip4;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblstate;
        private System.Windows.Forms.RichTextBox txtlog;
        private System.Windows.Forms.Button btnConn;
        private System.Windows.Forms.Button btnDis;
        private System.Windows.Forms.NotifyIcon notifyIcon2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip6;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.GroupBox grpSetting;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbAuth;
        private System.Windows.Forms.TextBox txtserver;
        private System.Windows.Forms.TextBox userid;
        private System.Windows.Forms.TextBox pass;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtdatabase;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TextBox txtprintUser;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label11;
    }
}

