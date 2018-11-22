namespace ArmageddonPro
{
    partial class ChatForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatForm));
            this.shade = new System.Windows.Forms.PictureBox();
            this.resize = new System.Windows.Forms.PictureBox();
            this.tabControlEX1 = new Dotnetrix.Controls.TabControlEX();
            this.tabPageEX1 = new Dotnetrix.Controls.TabPageEX();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.flags = new System.Windows.Forms.ComboBox();
            this.rank = new System.Windows.Forms.ComboBox();
            this.server = new System.Windows.Forms.ComboBox();
            this.comboChan = new System.Windows.Forms.ComboBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.tabPageEX2 = new Dotnetrix.Controls.TabPageEX();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnCloseTab = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.shade)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).BeginInit();
            this.tabControlEX1.SuspendLayout();
            this.tabPageEX1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // shade
            // 
            this.shade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shade.Image = ((System.Drawing.Image)(resources.GetObject("shade.Image")));
            this.shade.Location = new System.Drawing.Point(609, 4);
            this.shade.Name = "shade";
            this.shade.Size = new System.Drawing.Size(5, 6);
            this.shade.TabIndex = 11;
            this.shade.TabStop = false;
            // 
            // resize
            // 
            this.resize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resize.Image = ((System.Drawing.Image)(resources.GetObject("resize.Image")));
            this.resize.Location = new System.Drawing.Point(620, 374);
            this.resize.Name = "resize";
            this.resize.Size = new System.Drawing.Size(7, 7);
            this.resize.TabIndex = 10;
            this.resize.TabStop = false;
            this.resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resize_MouseDown);
            this.resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.resize_MouseMove);
            this.resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.resize_MouseUp);
            // 
            // tabControlEX1
            // 
            this.tabControlEX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlEX1.Appearance = Dotnetrix.Controls.TabAppearanceEX.FlatTab;
            this.tabControlEX1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tabControlEX1.Controls.Add(this.tabPageEX1);
            this.tabControlEX1.Controls.Add(this.tabPageEX2);
            this.tabControlEX1.FlatBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.tabControlEX1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlEX1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(193)))), ((int)(((byte)(193)))), ((int)(((byte)(193)))));
            this.tabControlEX1.HotColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tabControlEX1.Location = new System.Drawing.Point(0, 17);
            this.tabControlEX1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlEX1.Name = "tabControlEX1";
            this.tabControlEX1.SelectedIndex = 0;
            this.tabControlEX1.SelectedTabColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tabControlEX1.Size = new System.Drawing.Size(628, 342);
            this.tabControlEX1.TabColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tabControlEX1.TabIndex = 8;
            this.tabControlEX1.UseVisualStyles = false;
            this.tabControlEX1.SelectedIndexChanged += new System.EventHandler(this.SelectedIndexChanged);
            this.tabControlEX1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl_Selected);
            // 
            // tabPageEX1
            // 
            this.tabPageEX1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tabPageEX1.Controls.Add(this.panel1);
            this.tabPageEX1.Controls.Add(this.txtLog);
            this.tabPageEX1.Location = new System.Drawing.Point(4, 25);
            this.tabPageEX1.Name = "tabPageEX1";
            this.tabPageEX1.Size = new System.Drawing.Size(620, 313);
            this.tabPageEX1.TabIndex = 0;
            this.tabPageEX1.Text = "tabPageEX1";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.flags);
            this.panel1.Controls.Add(this.rank);
            this.panel1.Controls.Add(this.server);
            this.panel1.Controls.Add(this.comboChan);
            this.panel1.Controls.Add(this.txtUser);
            this.panel1.Controls.Add(this.btnConnect);
            this.panel1.Location = new System.Drawing.Point(0, 227);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(618, 83);
            this.panel1.TabIndex = 14;
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(447, 43);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(106, 21);
            this.button3.TabIndex = 16;
            this.button3.Text = "Channel list";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // flags
            // 
            this.flags.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flags.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flags.DropDownWidth = 28;
            this.flags.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flags.FormattingEnabled = true;
            this.flags.ItemHeight = 13;
            this.flags.Location = new System.Drawing.Point(437, 14);
            this.flags.Name = "flags";
            this.flags.Size = new System.Drawing.Size(43, 21);
            this.flags.TabIndex = 15;
            this.flags.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.flags_DrawItem);
            this.flags.SelectedIndexChanged += new System.EventHandler(this.flag_indexChanged);
            // 
            // rank
            // 
            this.rank.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.rank.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rank.FormattingEnabled = true;
            this.rank.Location = new System.Drawing.Point(483, 14);
            this.rank.MaxDropDownItems = 6;
            this.rank.Name = "rank";
            this.rank.Size = new System.Drawing.Size(70, 21);
            this.rank.TabIndex = 14;
            this.rank.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.rank_DrawItem);
            this.rank.SelectedIndexChanged += new System.EventHandler(this.rank_indexChanged);
            // 
            // server
            // 
            this.server.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.server.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.server.ForeColor = System.Drawing.Color.White;
            this.server.FormattingEnabled = true;
            this.server.Items.AddRange(new object[] {
            "wormnet1.team17.com",
            "127.0.0.1"});
            this.server.Location = new System.Drawing.Point(14, 15);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(199, 21);
            this.server.TabIndex = 11;
            // 
            // comboChan
            // 
            this.comboChan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.comboChan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboChan.ForeColor = System.Drawing.Color.White;
            this.comboChan.FormattingEnabled = true;
            this.comboChan.Items.AddRange(new object[] {
            "#AnythingGoes",
            "#PartyTime",
            "#RopersHeaven",
            "#Help"});
            this.comboChan.Location = new System.Drawing.Point(14, 43);
            this.comboChan.Name = "comboChan";
            this.comboChan.Size = new System.Drawing.Size(116, 21);
            this.comboChan.TabIndex = 13;
            // 
            // txtUser
            // 
            this.txtUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtUser.ForeColor = System.Drawing.Color.White;
            this.txtUser.Location = new System.Drawing.Point(232, 15);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(199, 20);
            this.txtUser.TabIndex = 10;
            this.txtUser.TextChanged += new System.EventHandler(this.txtUser_textChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Location = new System.Drawing.Point(136, 43);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(77, 21);
            this.btnConnect.TabIndex = 12;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.ForeColor = System.Drawing.Color.White;
            this.txtLog.Location = new System.Drawing.Point(0, 0);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(620, 225);
            this.txtLog.TabIndex = 0;
            // 
            // tabPageEX2
            // 
            this.tabPageEX2.Location = new System.Drawing.Point(4, 25);
            this.tabPageEX2.Name = "tabPageEX2";
            this.tabPageEX2.Size = new System.Drawing.Size(620, 313);
            this.tabPageEX2.TabIndex = 1;
            this.tabPageEX2.Text = "settings/options";
            // 
            // txtMessage
            // 
            this.txtMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMessage.ForeColor = System.Drawing.Color.White;
            this.txtMessage.Location = new System.Drawing.Point(0, 362);
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.Size = new System.Drawing.Size(628, 20);
            this.txtMessage.TabIndex = 1;
            this.txtMessage.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMessage_KeyPress);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnExit.BackgroundImage")));
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.Location = new System.Drawing.Point(618, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(5, 6);
            this.btnExit.TabIndex = 15;
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCloseTab
            // 
            this.btnCloseTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseTab.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCloseTab.BackgroundImage")));
            this.btnCloseTab.FlatAppearance.BorderSize = 0;
            this.btnCloseTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCloseTab.Location = new System.Drawing.Point(552, 4);
            this.btnCloseTab.Name = "btnCloseTab";
            this.btnCloseTab.Size = new System.Drawing.Size(5, 6);
            this.btnCloseTab.TabIndex = 16;
            this.btnCloseTab.UseVisualStyleBackColor = true;
            this.btnCloseTab.Click += new System.EventHandler(this.btnCloseTab_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.ClientSize = new System.Drawing.Size(628, 382);
            this.ControlBox = false;
            this.Controls.Add(this.btnCloseTab);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.shade);
            this.Controls.Add(this.resize);
            this.Controls.Add(this.tabControlEX1);
            this.Controls.Add(this.txtMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "ArmageddonPro";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.shade)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).EndInit();
            this.tabControlEX1.ResumeLayout(false);
            this.tabPageEX1.ResumeLayout(false);
            this.tabPageEX1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMessage;
        private Dotnetrix.Controls.TabControlEX tabControlEX1;
        private Dotnetrix.Controls.TabPageEX tabPageEX1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox flags;
        private System.Windows.Forms.ComboBox rank;
        public System.Windows.Forms.ComboBox server;
        public System.Windows.Forms.ComboBox comboChan;
        public System.Windows.Forms.TextBox txtUser;
        public System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.PictureBox resize;
        private System.Windows.Forms.PictureBox shade;
        private Dotnetrix.Controls.TabPageEX tabPageEX2;
        public System.Windows.Forms.TextBox txtLog;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnCloseTab;
    }
}

