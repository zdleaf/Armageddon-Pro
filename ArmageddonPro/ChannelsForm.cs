using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using BrightIdeasSoftware;


namespace WindowsFormsApplication1
{
    public partial class ChannelsForm : Torbo.DockableForm
    {
        public int shadedheight;
        public bool shaded = false;

        private ChatForm frmMain;
        private ArrayList users;

        public ChannelsForm(ChatForm frmform, ArrayList frmusers)
        {
            InitializeComponent();

            this.ShowInTaskbar = false;

            frmMain = frmform;
            users = frmusers;

            DoubleBuffered = true;
            Opacity = 1;

            object[] flags = new object[128];
            for (int i = 0, j = 0; i <= 126; i += 2, j++)
            {
                {
                    flags[i] = j;
                    flags[i + 1] = flagsList.Images[j];
                }
            }
            flag.Renderer = new MappedImageRenderer(flags);

            object[] ranks = new object[26];
            for (int i = 0, j = 0; i <= 24; i += 2, j++)
            {
                {
                    ranks[i] = j;
                    ranks[i + 1] = rankList.Images[j];
                }
            }
            rank.Renderer = new MappedImageRenderer(ranks);
        }

        // Window shade method
        const int WM_NCHITTEST = 0x84;
        const int HTCLIENT = 0x1;
        const int HTCAPTION = 0x2;
        private const int WM_NCLBUTTONDBLCLK = 0xA3;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if (m.Result.ToInt32() == HTCLIENT) m.Result = new IntPtr(HTCAPTION);
                    break;
                case WM_NCLBUTTONDBLCLK:
                    if (shaded == false)
                    {
                        shadedheight = Height;
                        resize.Visible = false;
                        Height = 14;
                        shaded = true;
                    }
                    else
                    {
                        Height = shadedheight;
                        resize.Visible = true;
                        shaded = false;
                    }
                    m.Msg = 0; //Discard double-click
                    break;
                default:
                    //Make sure you pass unhandled messages back to the default message handler.
                    base.WndProc(ref m);
                    break;
            }
        }

        private bool mSizing;
        private Point mMousePos;

        private void resize_MouseDown(object sender, MouseEventArgs e)
        {
            mSizing = e.Button == MouseButtons.Left;
            if (mSizing) resize.Cursor = Cursors.SizeNWSE;
            mMousePos = e.Location;
        }

        private void resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (mSizing)
            {
                Size adj = new Size(e.Location.X - mMousePos.X, e.Location.Y - mMousePos.Y);
                this.Size += adj;
                mMousePos = e.Location - adj; // NOTE: picturebox moves too! 
            }
        }

        private void resize_MouseUp(object sender, MouseEventArgs e)
        {
            if (mSizing) resize.Cursor = Cursors.Default;
            mSizing = false;
        }

        private void userlist_DoubleClick(object sender, EventArgs e)
        {
            if (userlist.SelectedIndex != -1)
            {
                user row = (user)userlist.SelectedObject;
                Control ctrl = frmMain.tabexists(row.username.ToString());
                if (ctrl == null)
                {
                    frmMain.newtab(row.username.ToString(), null, true, false);
                }
                else
                {
                    frmMain.focustab(ctrl);
                }

            }
        }


    }
}
