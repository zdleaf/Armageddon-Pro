using Microsoft.Win32;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices; //for DllImport
using System.Threading;
using System.Windows.Forms;


namespace ArmageddonPro
{

    public partial class Chat : Torbo.DockableForm
    {
        // Stop autoscroll in textbox when scrollbar is not at bottom
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar, ref SCROLLINFO lpsi);
        [StructLayout(LayoutKind.Sequential)]
        struct SCROLLINFO
        {
            public uint cbSize;
            public uint fMask;
            public int nMin;
            public int nMax;
            public uint nPage;
            public int nPos;
            public int nTrackPos;
        }
        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3
        }
        private enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
        }
        public bool IsScrollBarAtBottom(RichTextBox textBox)
        {
            SCROLLINFO si = new SCROLLINFO();
            si.cbSize = (uint)Marshal.SizeOf(si);
            si.fMask = (int)(ScrollInfoMask.SIF_ALL);
            GetScrollInfo(textBox.Handle, (int)ScrollBarDirection.SB_VERT, ref si);
            return (si.nTrackPos + si.nPage >= si.nMax -1);
        }
        // End autoscroll

        private StreamWriter swSender;
        private StreamReader srReceiver;
        private TcpClient tcpServer;
        private delegate void UpdateLogCallback(string strMessage);
        private delegate void CloseConnectionCallback(string strReason);
        private Thread thrMessaging;
        private IPAddress ipAddr;
        internal bool Connected;
        public string username;
        public string channel;
        public string hostname;
        public int detached;
        public string currentchan;
        public int shadedheight;
        public bool shaded = false;
        public int chathidden; // Show chat windows or just gamelist
        public int alwaysontop;

        // For error logging
        // TextWriter tw = new StreamWriter("C:\log2.txt");

        public ArrayList users = new ArrayList();
        public ArrayList channels = new ArrayList();

        private Users frmUserlist;
        private Games frmGamelist;
        private Channels frmChanlist;



        public Chat()
        {

            frmUserlist = new Users(this, users);
            frmChanlist = new Channels(this, channels);
            frmGamelist = new Games(this, frmUserlist, frmChanlist);
            
            ConnectForm(frmChanlist);
            ConnectForm(frmGamelist);
            ConnectForm(frmUserlist);
            frmGamelist.ConnectForm(frmChanlist);
            frmUserlist.ConnectForm(frmGamelist);

            // On application exit, disconnect first and save settings to registry
            Application.ApplicationExit += new EventHandler(OnApplicationExit);
            InitializeComponent();

            DoubleBuffered = true;

            server.SelectedIndex = 0;
            comboChan.SelectedIndex = 0;
            tabPageEX1.Text = "connect";

        }

        public void newtab(string name, string text, bool focus, bool highlight)
        {
            TabPage newPage = new TabPage(name);
            newPage.Name = name;

            if (highlight == true) newPage.ForeColor = Color.Red;
            else newPage.ForeColor = Color.White;

            newPage.BackColor = tabPageEX1.BackColor;
            tabControlEX1.TabPages.Add(newPage);

            if (focus == true) tabControlEX1.SelectedTab = newPage;

            RichTextBox tb = new RichTextBox();
            // TextBox will be named "tbTabPage1, tbTabPage2 ...."
            tb.Name = "tb" + newPage.Name;
            tb.Parent = newPage;
            tb.Multiline = true;
            tb.Width = txtLog.Width;
            tb.Height = txtLog.Height;
            tb.BackColor = txtLog.BackColor;
            tb.ForeColor = txtLog.ForeColor;
            tb.BorderStyle = txtLog.BorderStyle;
            tb.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            tb.Dock = DockStyle.Fill;
            tb.Location = txtLog.Location;
            tb.Text = text;
            tb.ReadOnly = true;
            newPage.Controls.Add(tb);
        }

        // Window shade method
        const int WM_NCHITTEST = 0x84;
        const int HTCLIENT = 0x1;
        const int HTCAPTION = 0x2;
        private const int WM_NCLBUTTONDBLCLK = 0xA3;
        const int WM_ACTIVATEAPP = 0x001C;

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_ACTIVATEAPP:
                    if (m.WParam != IntPtr.Zero)
                    {
                        frmUserlist.BringToFront();
                        frmChanlist.BringToFront();
                        frmGamelist.BringToFront();
                    }
                    else base.WndProc(ref m);
                    break;
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if (m.Result.ToInt32() == HTCLIENT) m.Result = new IntPtr(HTCAPTION);
                    break;
                case WM_NCLBUTTONDBLCLK:
                    if (shaded == false)
                    {
                        shadedheight = Height;
                        resize.Visible = false;
                        txtMessage.Visible = false;
                        Height = 14;
                        shaded = true;
                    }
                    else
                    {
                        Height = shadedheight;
                        resize.Visible = true;
                        txtMessage.Visible = true;
                        shaded = false;
                    }
                    m.Msg = 0; // Discard double-click
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
        // end window shade

        // The event handler for application exit
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (Connected == true)
            {
                // Closes the connections, streams, etc.
                Connected = false;
                swSender.Close();
                srReceiver.Close();
                tcpServer.Close();
            }

            // SAVE SETTINGS TO REGISTRY
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn", true);

            key.SetValue("rank", rank.SelectedIndex);
            key.SetValue("flag", flags.SelectedIndex);
            if (username != null) { key.SetValue("user", username); }

            key.SetValue("X", Location.X);
            key.SetValue("Y", Location.Y);

            if (shaded == false)
            {
                key.SetValue("width", Size.Width);
                key.SetValue("height", Size.Height);
            }
            else if (shaded == true)
            {
                key.SetValue("width", Size.Width);
                key.SetValue("height", shadedheight);
            }

            key.SetValue("userlistX", frmChanlist.Location.X);
            key.SetValue("userlistY", frmChanlist.Location.Y);
            key.SetValue("userlistwidth", frmChanlist.Size.Width);
            key.SetValue("userlistheight", frmChanlist.Size.Height);

            key.SetValue("gamelistX", frmGamelist.Location.X);
            key.SetValue("gamelistY", frmGamelist.Location.Y);
            key.SetValue("gamelistwidth", frmGamelist.Size.Width);
            key.SetValue("gamelistheight", frmGamelist.Size.Height);

            key.SetValue("chanlistX", frmUserlist.Location.X);
            key.SetValue("chanlistY", frmUserlist.Location.Y);
            key.SetValue("chanlistwidth", frmUserlist.Size.Width);
            key.SetValue("chanlistheight", frmUserlist.Size.Height);

            if (chathidden == 1)
            {
                key.SetValue("chathidden", 1);
            }
            else
            {
                key.SetValue("chathidden", 0);
            }

            if (alwaysontop == 1)
            {
                key.SetValue("alwaysontop", 1);
            }
            else 
            {
                key.SetValue("alwaysontop", 0);
            }
            
        }

        private void InitializeConnection()
        {

            // Open new Tab, and connect() within this new tab

            // Parse the IP address from the TextBox into an IPAddress object
            IPAddress[] ips = Dns.GetHostAddresses(hostname);
            ipAddr = ips[0];
            
            /*
            try
            {
                ipAddr = IPAddress.Parse(ip);

            }
            catch (ArgumentNullException e)
            {
                MessageBox.Show("SERVER IS NULL",
                e.ToString(),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);
                tcpServer.Close();
                return;
            }

            catch (FormatException e)
            {
                MessageBox.Show("IP OR SERVER NOT VALID",
                e.ToString(),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);
                tcpServer.Close();
                return;
            }
            */
            //            !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //            newtab("newconnect", "newmsg", false, false);
            //            !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Start a new TCP connection to the chat server
            tcpServer = new TcpClient();
            try
            {
                tcpServer.Connect(ipAddr, 6667);
            }
            catch (SocketException e)
            {
                MessageBox.Show("Unable to Connect",
                e.ToString(),
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Information);
                tcpServer.Close();

                return;

            }
            tabPageEX1.Text = hostname;
            Connected = true;
            btnConnect.Text = "Disconnect";
            // Send the username/password to the server
            swSender = new StreamWriter(tcpServer.GetStream());
            swSender.WriteLine("PASS ELSILRACLIHP");
            string nick = "NICK ";
            username = txtUser.Text;
            nick += username;
            swSender.WriteLine(nick);
            swSender.WriteLine("USER Username * * :" + flags.SelectedIndex + " " + rank.SelectedIndex + " ZincLdn");

            swSender.WriteLine("JOIN " + channel);

            swSender.WriteLine("WHO");
            swSender.WriteLine("LIST");

            swSender.Flush();
        
            // Start the thread for receiving messages and further communication
            thrMessaging = new Thread(new ThreadStart(ReceiveMessages));
            thrMessaging.Start();
        }

        private void ReceiveMessages()
        {

            srReceiver = new StreamReader(tcpServer.GetStream());

            /*
            // If the first character of the response is 1, connection was successful
            
            string ConResponse = srReceiver.ReadLine();
            // If the first character is a 1, connection was successful
            if (ConResponse[0] != null)
            {
                if (ConResponse[0] == '1')
                {
                    // Update the form to tell it we are now connected
                    this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { "Connected Successfully!" });
                }
            }
            else // If the first character is not a 1 (probably a 0), the connection was unsuccessful
            {
                string Reason = "Not Connected: ";
                // Extract the reason out of the response message. The reason starts at the 3rd character
                Reason += ConResponse.Substring(2, ConResponse.Length - 2);
                // Update the form with the reason why we couldn't connect
                this.Invoke(new CloseConnectionCallback(this.CloseConnection), new object[] { Reason });
                // Exit the method
                return;
            }
            */

            // While we are successfully connected, read incoming lines from the server

            while (Connected)
            {
                try
                {
                    this.Invoke(new UpdateLogCallback(this.UpdateLog), new object[] { srReceiver.ReadLine() });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }

        }


        private void UpdateLog(string strMessage)
        {

            // JOIN
            if (strMessage.IndexOf("JOIN") != -1)
            {
                int i;
                i = strMessage.LastIndexOf(":");
                currentchan = strMessage.Substring(i + 1);

                int i3;
                i3 = strMessage.IndexOf("!");
                string name = strMessage.Substring(1, i3 - 1);

                // If channel comes up with no tab, create that tab
                if (!tabControlEX1.TabPages.ContainsKey(currentchan))
                {
                    newtab(currentchan, "", false, false);
                }
                else
                {
                    // Do a WHO on them, so the who handler can add them to list
                    swSender.WriteLine("WHO " + name);
                    swSender.Flush();
                    // Add join msg to correct chan
                    Control[] ctrl = this.Controls.Find("tb" + currentchan, true);
                    if (ctrl.Length != 0)
                    {
                        RichTextBox prv = (RichTextBox)ctrl[0];
                        string joinstring = "\r\n [ " + name + " joined ]";
                        appendx(prv, Color.FromArgb(44, 44, 44), joinstring);
                    }
                }

            }
            // PART/QUIT
            //[14:07:46] :`zincldn!`zincldn@127.0.0.1 PART :#Help
            if (strMessage.IndexOf("PART") != -1)
            {
                int i5 = strMessage.IndexOf("!");
                string quitname = strMessage.Substring(1, i5 - 1);

                string tabname = currentchan; // This is set when we find a join request response

                // Remove them from the userlist
                foreach (User item in users)
                {
                    if (item.username == quitname)
                    {
                        users.Remove(item);
                        break;
                    }
                }

                frmUserlist.userlist.SetObjects(users);

                Control[] ctrl = this.Controls.Find("tb" + tabname, true);
                if (ctrl.Length != 0)
                {
                    RichTextBox prv = (RichTextBox)ctrl[0];
                    string partstring = "\r\n [ " + quitname + " left channel ]";
                    appendx(prv, Color.FromArgb(44, 44, 44), partstring);
                }
            }

            if (strMessage.IndexOf("QUIT") != -1)
            {
                int i5 = strMessage.IndexOf("!");
                string quitname = strMessage.Substring(1, i5 - 1);

                int i6 = strMessage.LastIndexOf(":");
                string quitmsg = strMessage.Substring(i6 + 1);

                string tabname = currentchan;

                // Remove them from the userlist
                foreach (User item in users)
                {
                    if (item.username == quitname)
                    {
                        users.Remove(item);
                        break;
                    }
                }

                frmUserlist.userlist.SetObjects(users);

                Control[] ctrl = this.Controls.Find("tb" + tabname, true);
                if (ctrl.Length != 0)
                {
                    RichTextBox prv = (RichTextBox)ctrl[0];
                    string quitstring = "\r\n [ " + quitname + " quit: " + quitmsg + " ]";
                    appendx(prv, Color.FromArgb(44, 44, 44), quitstring);
                }

            }

            // PRIVMSG
            // :zincldn!zincldn@127.0.0.1 PRIVMSG #Help :chan
            // :zincldn!zincldn@127.0.0.1 PRIVMSG hi :private

            if (strMessage.IndexOf("PRIVMSG") != -1)
            {
                // name is between the first colon and a "!"
                int i4 = strMessage.IndexOf("!");
                string username = strMessage.Substring(1, i4 - 1);

                // message comes after the second ":"
                int i5 = strMessage.IndexOf(":", 1);
                string message = strMessage.Substring(i5 + 1);

                string[] split = strMessage.Split(' ');

                // check whether there is already a TextBox for the #channel or username
                RichTextBox privateMsgBox = new RichTextBox();
                if (split[2].Contains('#')) { privateMsgBox = textboxExists(split[2]); } 
                else { privateMsgBox = textboxExists(username); }

                // if textbox exists, append to it, otherwise make a new tab.
                if (privateMsgBox != null)
                {
                    TabPage tab = (TabPage)privateMsgBox.Parent;
                    // highlight the tab name in Red to signify a new message
                    if (tabControlEX1.SelectedTab != tab)
                    {
                        tab.ForeColor = Color.Red;
                    }
                    tabControlEX1.Refresh();
                    string msgText = "\r\n[ " + username + " ] " + message;
                    appendx(privateMsgBox, Color.White, msgText);
                }
                else
                {
                    newtab(username, "\r\n [" + username + "] " + message, false, true);
                }
            }
            // If it's a NAMEs reply
            // :localhost 352 zincldn #Help `zincldn 127.0.0.1 localhost `zincldn H :0 0[flag] 0[rank] :UK ProSnooper
            // :wormnet1.team17.com 352 zincldn #Help ~WebSnoop no.address.for.you wormnet1.team17.com WebSnoop H :0 0 4294967309 UK http://snoop.worms2d.info/
            // :wormnet1.team17.com 352 zincldn #AnythingGoes Username no.address.for.you wormnet1.team17.com Figaro H :0 44 0 UK ProSnooper2
            if (strMessage.IndexOf("352") != -1)
            {
                string[] whoarray = strMessage.Split(' ');
                bool exists = false;

                User user = new User();
                user.username = whoarray[7];
                user.channel = whoarray[6];

                int i5 = strMessage.IndexOf(":", 1);
                // Error checking for invalid or malformed rank/flag information.. is int? if not display ??? and ? rank and flags
                try
                {
                    user.flag = int.Parse(strMessage.Substring(i5 + 3, 2));
                    user.rank = int.Parse(strMessage.Substring(i5 + 5, 2));
                }
                catch
                {
                }

                foreach (User item in users)
                {
                    if (item.username == whoarray[7])
                    { exists = true; }
                }

                // 3 is channel
                if (whoarray[3] == channel)
                {
                    if (exists == false)
                    {
                        users.Add(user);
                    }

                }

                frmUserlist.userlist.SetObjects(users);

            }

            // If channel does not exist or was not specified

            if (strMessage.IndexOf("403") != -1)
            {
                // Send LIST
                swSender.WriteLine("LIST");
                swSender.Flush();
                // Get channels from 322 reply
            }

            // CHANNEL "LIST" REPLY
            // :wormnet1.team17.com 322 zincldn #Help 8 :05 A place to get help, or help others
            // :localhost 322 `zincldn #AnythingGoes 0 Open�games�with�'Rope�Knocking'�allowed�&�blood�fx :
            if (strMessage.IndexOf("322") != -1)
            {
                string[] channellist = strMessage.Split(' ');
                Channel chan = new Channel();
                chan.channelName = channellist[3].Replace('�', ' ');
                chan.channelUsercount = Int32.Parse(channellist[4]);

                int first = strMessage.IndexOf(channellist[5]);

                chan.channelTopic = strMessage.Substring(first, strMessage.Length - first).Replace('�', ' ');

                bool exists = false;

                foreach (Channel channel1 in channels)
                {
                    if (channel1.channelName == chan.channelName)
                    {
                        exists = true;
                        chan = channel1;
                    }
                }

                if (exists == false)
                {
                    channels.Add(chan);
                    frmChanlist.channelList.SetObjects(channels);
                }
                else
                {
                    chan.channelUsercount = Int32.Parse(channellist[4]);
                    frmChanlist.channelList.RefreshObject(chan);
                }


            }

            // Append text also scrolls the TextBox to the bottom each time
            txtLog.AppendText(strMessage + "\r\n");

            // text file for error logging
            // tw.WriteLine(strMessage);

            // NICK IN USE
            if (strMessage.IndexOf(" 433 ") != -1)
            {
                CloseConnection(); 
            }


        }

        public void rawsend(string data)
        {
            swSender.WriteLine(data);
            swSender.Flush();
        }

        public void appendx(RichTextBox textbox, Color color, String text)
        {
            textbox.SelectionStart = textbox.TextLength; // Move cursor to the END before appending
            textbox.SelectionColor = color;
            if (IsScrollBarAtBottom(textbox) == true)
            {
                textbox.AppendText(text);
                textbox.ScrollToCaret();
            }
            else
            {
                textbox.AppendText(text);
            }
        }

        public Control tabexists(string tabname)
        {
            Control[] ctrl2 = new Control[1];
            ctrl2 = this.Controls.Find(tabname, true);
            if (ctrl2.Length != 0) // i.e. tab is found
            {
                return ctrl2[0]; // Return first tab found
            }
            else return null; // Else return null
        }

        public RichTextBox textboxExists(string textbox_name)
        {
            Control[] ctrl2 = new Control[1];
            ctrl2 = this.Controls.Find("tb" + textbox_name, true);
            if (ctrl2.Length != 0) // i.e. tb is found
            {
                return (RichTextBox)ctrl2[0]; // Return first textbox found
            }
            else return null; // Else return null
        }

        public void focustab(Control tab)
        {
            tabControlEX1.SelectedTab = (TabPage)tab;
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If the key is Enter
            if (e.KeyChar == (char)13)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            // form PRIVMSG DEST MSG string

            string tabname = tabControlEX1.SelectedTab.Name; // destination
            string mesg = "privmsg " + tabname + " :" + txtMessage.Text;
            // add to textbox
            Control[] ctrl = this.Controls.Find("tb" + tabname, true);
            if (ctrl.Length != 0)
            {
                RichTextBox prv = (RichTextBox)ctrl[0];
                prv.AppendText("\r\n [" + txtUser.Text + "] " + txtMessage.Text);
            }

            // Send raw command to server
            if (tabControlEX1.SelectedTab.Text == hostname)
            {
                mesg = txtMessage.Text;
            }

            // Send to server
            if (txtMessage.Lines.Length >= 1)
            {
                swSender.WriteLine(mesg);
                swSender.Flush();
                txtMessage.Lines = null;
            }
            txtMessage.Text = "";
        }

        private void CloseConnection()
        {
            txtLog.AppendText("\r\n Disconnected \r\n");
            btnConnect.Text = "Connect";
            server.Enabled = true;
            txtUser.Enabled = true;
            Connected = false;
            swSender.Close();
            srReceiver.Close();
            tcpServer.Close();
            frmUserlist.userlist.SetObjects(null);
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCloseTab_Click(object sender, EventArgs e)
        {
            if (tabControlEX1.SelectedTab.Name != "tabPageEX1")
            {
                tabControlEX1.TabPages.Remove(tabControlEX1.SelectedTab);
            }
        }

        private void splitmoved(object sender, SplitterEventArgs e)
        {
            txtMessage.Focus();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text == "Connect")
            {
                comboChan.Focus();
                channel = comboChan.SelectedText;
                if (channel == "")
                {
                    channel = comboChan.SelectedItem.ToString();
                }

                server.Focus();

                hostname = server.SelectedText;
                if (hostname == "")
                {
                    hostname = server.SelectedItem.ToString();
                }

                InitializeConnection();
                txtMessage.Focus();
            }
            else if (btnConnect.Text == "Disconnect")
            {
                CloseConnection();
                txtMessage.Focus();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // FLAGS
            this.flags.ItemHeight = 18;
            string[] flagsstr;
            flagsstr = new string[frmUserlist.flagsList.Images.Count];
            for (int i = 0; i < frmUserlist.flagsList.Images.Count; i++)
            {
                this.flags.Items.Add(frmUserlist.flagsList.Images[i]);

            }
            this.flags.DropDownStyle = ComboBoxStyle.DropDownList;
            this.flags.DrawMode = DrawMode.OwnerDrawVariable;
            this.flags.Width = this.frmUserlist.flagsList.ImageSize.Width + 24;


            // RANKS
            this.rank.ItemHeight = 18;
            string[] ranks;
            ranks = new string[frmUserlist.rankList.Images.Count];
            for (int i = 0; i < frmUserlist.rankList.Images.Count; i++)
            {
                this.rank.Items.Add(frmUserlist.rankList.Images[i]);

            }
            this.rank.DropDownStyle = ComboBoxStyle.DropDownList;
            this.rank.DrawMode = DrawMode.OwnerDrawVariable;
            this.rank.Width = this.frmUserlist.rankList.ImageSize.Width + 20;

            // LOAD SETTINGS FROM REGISTRY
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn");

            // If the return value is null, the key doesn't exist
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\zincldn");
            }

            if (key.GetValue("X") != null)
            { Location = new Point((int)key.GetValue("X"), (int)key.GetValue("Y")); }

            if (key.GetValue("width") != null)
            {
                Size = new Size((int)key.GetValue("width"), (int)key.GetValue("height"));
            }


            if (key.GetValue("userlistX") != null)
            { frmChanlist.Location = new Point((int)key.GetValue("userlistX"), (int)key.GetValue("userlistY")); }

            if (key.GetValue("userlistwidth") != null)
            {
                frmChanlist.Size = new Size((int)key.GetValue("userlistwidth"), (int)key.GetValue("userlistheight"));
            }


            if (key.GetValue("gamelistX") != null)
            { frmGamelist.Location = new Point((int)key.GetValue("gamelistX"), (int)key.GetValue("gamelistY")); }

            if (key.GetValue("gamelistwidth") != null)
            {
                frmGamelist.Size = new Size((int)key.GetValue("gamelistwidth"), (int)key.GetValue("gamelistheight"));
            }

            if (key.GetValue("chanlistX") != null)
            { frmUserlist.Location = new Point((int)key.GetValue("chanlistX"), (int)key.GetValue("chanlistY")); }

            if (key.GetValue("chanlistwidth") != null)
            {
                frmUserlist.Size = new Size((int)key.GetValue("chanlistwidth"), (int)key.GetValue("chanlistheight"));
            }

            if ((int)key.GetValue("chathidden") == 1)
            {
                frmGamelist.Show();
                frmGamelist.hidechat.Checked = true;
                chathidden = 1;
            }
            else 
            {
                frmUserlist.Show();
                frmGamelist.Show();
                chathidden = 0;
            }

            if ((int)key.GetValue("alwaysontop") == 1)
            {
                frmGamelist.alwaysontop.Checked = true;
            }
            else 
            {
                frmGamelist.alwaysontop.Checked = false;
            }

            if (key.GetValue("user") != null)
            { txtUser.Text = (string)key.GetValue("user"); }

            if (key.GetValue("rank") != null)
            { rank.SelectedIndex = (int)key.GetValue("rank"); }

            if (key.GetValue("flag") != null)
            { flags.SelectedIndex = (int)key.GetValue("flag"); }

        }

        private void flags_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {

                e.Graphics.DrawImage(frmUserlist.flagsList.Images[e.Index],
                e.Bounds.Left, e.Bounds.Top);
            }

        }

        private void rank_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index != -1)
            {

                e.Graphics.DrawImage(frmUserlist.rankList.Images[e.Index],
                e.Bounds.Left, e.Bounds.Top);
            }
        }

        private void txtUser_textChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn", true);
            key.SetValue("user", txtUser.Text);
        }

        private void rank_indexChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn", true);
            key.SetValue("rank", rank.SelectedIndex);
        }

        private void flag_indexChanged(object sender, EventArgs e)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn", true);
            key.SetValue("flag", flags.SelectedIndex);
        }

        public string time()
        {
            string dateTime = DateTime.Now.ToString("HH:mm:ss");
            return dateTime;
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

        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            txtMessage.Focus();
            /*
            if (tabControlEX1.SelectedIndex == 0)
            {
                channelList.Visible = true;
            }
            else
            {
                channelList.Visible = false;
            }
             * */
        }

        private void tabControl_Selected(object sender, TabControlEventArgs e)
        {
            tabControlEX1.SelectedTab.ForeColor = Color.White;
        }

        public void showchat()
        {
            chathidden = 0;
            this.Show();
            frmUserlist.Show();
            frmUserlist.Visible = false;
            frmGamelist.Show();
            frmChanlist.Show();
            // Switch which form shows in taskbar - otherwise we have issues when clicking from taskbar to bring to front/when hiding chat
            this.ShowInTaskbar = true;
            frmGamelist.ShowInTaskbar = false;
        }

        public void hidechat()
        {
            chathidden = 1;
            frmGamelist.Show();
            this.Hide();
            frmUserlist.Hide();
            frmChanlist.Hide();
            this.ShowInTaskbar = false;
            frmGamelist.ShowInTaskbar = true;
        }

        // Hide main window on Load if chathidden registry setting set - unable to do this on Load so needs to be overridden afterwards
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (chathidden == 1)
            {
                hidechat();
            }
        }
        
        /* ALTERNATIVE METHOD - chat screen flashes up before hiding so not as good as above
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (chathidden == 1)
            { this.Hide(); }
        }
        */

        private void showChanList_Click(object sender, EventArgs e)
        {
            if (frmChanlist.Visible == true)
            { frmChanlist.Visible = false; }
            else
            { frmChanlist.Visible = true; }
        }


    }

    public class User
    {
        public int flag;
        public int rank;
        public string username;
        public string channel;
    }

    public class Channel
    {
        public string channelName;
        public int channelUsercount;
        public string channelTopic;
    }

}
