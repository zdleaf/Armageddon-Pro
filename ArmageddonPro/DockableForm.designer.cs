namespace Torbo
{
    partial class DockableForm
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
            this.SuspendLayout();
            // 
            // DockableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "DockableForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "DockableForm";
            this.ResizeBegin += new System.EventHandler(this.DockableForm_ResizeBegin);
            this.Shown += new System.EventHandler(this.DockableForm_Shown);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DockableForm_FormClosed);
            this.Move += new System.EventHandler(this.DockableForm_Move);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DockableForm_MouseMove);
            this.ResizeEnd += new System.EventHandler(this.DockableForm_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion
    }
}