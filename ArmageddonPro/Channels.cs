using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Timers;

namespace ArmageddonPro
{
    public partial class Channels : Torbo.DockableForm
    {

        public int shadedheight;
        public bool shaded = false;
        private Chat frmChat;

        public Channels(Chat frmform, ArrayList frmchannels)
        {
            InitializeComponent();
            frmChat = frmform;

            channelName.FreeSpaceProportion = 11;
            channelUsercount.FreeSpaceProportion = 2;
            channelTopic.FreeSpaceProportion = 30;
            channelUsercount.MinimumWidth = 20;

            // User a system timer to refresh channels
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 60 seconds.
            aTimer.Interval = 60000;
            aTimer.Enabled = true;
            
            this.ShowInTaskbar = false;

        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (frmChat.Connected != false && this.Visible == true)
            {
                frmChat.RawSend("LIST");
            }
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
                    base.WndProc(ref m);
                    break;
            }
        }

        private bool mSizing;
        private Point mMousePos;
        private void resize_MouseUp(object sender, MouseEventArgs e)
        {
            if (mSizing) resize.Cursor = Cursors.Default;
            mSizing = false;
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

        private void resize_MouseDown(object sender, MouseEventArgs e)
        {
            mSizing = e.Button == MouseButtons.Left;
            if (mSizing) resize.Cursor = Cursors.SizeNWSE;
            mMousePos = e.Location;
        }

        // Join channel if double clicked
        private void channelList_doubleclick(object sender, EventArgs e)
        {
            Channel chan = (Channel)channelList.SelectedObject;
            Control ctrl = frmChat.tabexists(chan.channelName);
            if (ctrl == null)
            {
                frmChat.newtab(chan.channelName, "", false, false);
                frmChat.RawSend("JOIN " + chan.channelName);
            }
            else
            {
                frmChat.FocusTab(ctrl);
            }
            
        }

    }
}
