namespace WindowsFormsApplication1
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            this.panel1 = new System.Windows.Forms.Panel();
            this.channelList = new BrightIdeasSoftware.ObjectListView();
            this.channelName = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.channelUsercount = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.channelTopic = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.resize = new System.Windows.Forms.PictureBox();
            this.shade = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.channelList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.shade)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.channelList);
            this.panel1.Location = new System.Drawing.Point(0, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(258, 241);
            this.panel1.TabIndex = 19;
            // 
            // channelList
            // 
            this.channelList.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.channelList.AllColumns.Add(this.channelName);
            this.channelList.AllColumns.Add(this.channelUsercount);
            this.channelList.AllColumns.Add(this.channelTopic);
            this.channelList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.channelList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.channelList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.channelName,
            this.channelUsercount,
            this.channelTopic});
            this.channelList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.channelList.ForeColor = System.Drawing.Color.White;
            this.channelList.FullRowSelect = true;
            this.channelList.HasCollapsibleGroups = false;
            this.channelList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.channelList.HighlightBackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(48)))), ((int)(((byte)(48)))));
            this.channelList.Location = new System.Drawing.Point(0, 0);
            this.channelList.MultiSelect = false;
            this.channelList.Name = "channelList";
            this.channelList.OwnerDraw = true;
            this.channelList.ShowGroups = false;
            this.channelList.Size = new System.Drawing.Size(258, 241);
            this.channelList.SortGroupItemsByPrimaryColumn = false;
            this.channelList.TabIndex = 18;
            this.channelList.UseCompatibleStateImageBehavior = false;
            this.channelList.View = System.Windows.Forms.View.Details;
            this.channelList.DoubleClick += new System.EventHandler(this.channelList_doubleclick);
            // 
            // channelName
            // 
            this.channelName.AspectName = "channelName";
            this.channelName.FillsFreeSpace = true;
            this.channelName.Width = 1;
            // 
            // channelUsercount
            // 
            this.channelUsercount.AspectName = "channelUsercount";
            this.channelUsercount.Width = 30;
            // 
            // channelTopic
            // 
            this.channelTopic.AspectName = "channelTopic";
            this.channelTopic.FillsFreeSpace = true;
            this.channelTopic.Width = 1;
            // 
            // resize
            // 
            this.resize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resize.Image = ((System.Drawing.Image)(resources.GetObject("resize.Image")));
            this.resize.Location = new System.Drawing.Point(249, 256);
            this.resize.Name = "resize";
            this.resize.Size = new System.Drawing.Size(7, 7);
            this.resize.TabIndex = 17;
            this.resize.TabStop = false;
            this.resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.resize_MouseDown);
            this.resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.resize_MouseMove);
            this.resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.resize_MouseUp);
            // 
            // shade
            // 
            this.shade.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.shade.Image = ((System.Drawing.Image)(resources.GetObject("shade.Image")));
            this.shade.Location = new System.Drawing.Point(247, 3);
            this.shade.Name = "shade";
            this.shade.Size = new System.Drawing.Size(5, 6);
            this.shade.TabIndex = 16;
            this.shade.TabStop = false;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(257, 264);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.resize);
            this.Controls.Add(this.shade);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form3";
            this.Text = "Form3";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.channelList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.shade)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox shade;
        private System.Windows.Forms.PictureBox resize;
        private BrightIdeasSoftware.OLVColumn channelName;
        private BrightIdeasSoftware.OLVColumn channelUsercount;
        private BrightIdeasSoftware.OLVColumn channelTopic;
        private System.Windows.Forms.Panel panel1;
        protected internal BrightIdeasSoftware.ObjectListView channelList;

    }
}