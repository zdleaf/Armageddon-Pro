using System;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Timers;
using Microsoft.Win32;

using BrightIdeasSoftware;

namespace WindowsFormsApplication1
{

    public partial class GamesForm : Torbo.DockableForm
    {

        public int shadedheight;
        public bool shaded = false;
        public string channel;
        public Color bordercolor = Color.Black;
        public ArrayList chanlist = new ArrayList();

        public class game
        {
            public int flag;
            public int padlock;
            public string gamename;
            public string host;
            public string ip;
            public string id;
            public string chan;
        }

        public game storedgame;
        private ChatForm frmMain;

        public GamesForm(ChatForm frmform)
        {
            frmMain = frmform;
            InitializeComponent();

            this.ShowInTaskbar = false;

            chanlist.Add("AnythingGoes");
            chanlist.Add("PartyTime");
            chanlist.Add("Help");
            chanlist.Add("RopersHeaven");

            chan.FreeSpaceProportion = 23;
            host.FreeSpaceProportion = 23;
            gamename.FreeSpaceProportion = 30;
            ip.FreeSpaceProportion = 23;

            // Refresh the game list at a specified interval
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 5 seconds.
            aTimer.Interval = 5000;
            aTimer.Enabled = true;

            // object[] array of key/image pairs
            // ie 0, image0, 1, image1, 2, image2...
            // 0     1      2    3     4     5
            object[] flagsandimages = new object[128];
            for (int i = 0, j = 0; i <= 126; i += 2, j++)
            {
                {
                    flagsandimages[i] = j;
                    flagsandimages[i + 1] = flagsList.Images[j];
                }
            }

            flag.Renderer = new MappedImageRenderer(flagsandimages);
            paddy.Renderer = new MappedImageRenderer(0, padlock.Images[0], 1, padlock.Images[1]);


        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // Make default host channel to #AnythingGoes
            host_chan_dropdown.SelectedIndex = 0;

            // Attempt to open the key
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn");

            // If the return value is null, the key doesn't exist
            if (key == null)
            {
                // The key doesn't exist; create it / open it
                key = Registry.CurrentUser.CreateSubKey("Software\\zincldn");
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            updategamelist();
        }

        private void get_wormnat_port()
        {
        }

        public void updategamelist()
        {
            ArrayList games = new ArrayList();
            foreach (string channel in chanlist)
            {
                string sURL;
                sURL = "http://wormnet1.team17.com/wormageddonweb/GameList.asp?Channel=" + channel;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sURL);
                request.Method = "GET";
                request.UserAgent = "T17Client/1.2";

                Stream objStream = null;
                try
                {
                    objStream = request.GetResponse().GetResponseStream();
                }
                catch (Exception)
                {
                }

                if (objStream == null)
                {
                    // MessageBox.Show("error: stream is null");
                    return;
                }

                StreamReader objReader = new StreamReader(objStream);

                string sLine = "";


                int i = 0;
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null && sLine.IndexOf("<GAMELIST") == -1)
                    {
                        i++;
                        string[] array = sLine.Split(' ');
                        string gamename;
                        /*
                         * Gamelist response and encoding
                        <GAMELIST START>
                        <GAME ßnormal_1vs1 Nele 79.175.112.80:17011 52 1 0 5876685 0><BR>
                        <GAME xRaWxViks xRaWxViks 93.86.14.40:17011 52 1 0 5876686 0><BR>
                        <GAMELISTEND>

                        gamename (1)
                        username/host (2) 
                        ip:port (3) 
                        flag (4) 
                        unknown (5) 
                        padlock 6 (1 padlock, 0 no padlock)
                        game id (7)

                        \U00DF 'ß'

                        */

                        // If game name starts with ß, this signifies game was hosted with a beta version of the game - now irrelevant
                        if (array[1][0].Equals('ß'))
                        {
                            // Remove the unrequired ß symbol
                            gamename = array[1].Substring(1, array[1].Length - 1);
                            // gamename = "ß" + gamename;
                        }
                        else
                        {
                            gamename = array[1];
                        }
                        
                        game game = new game();
                        game.flag = int.Parse(array[4]);
                        game.padlock = int.Parse(array[6]);
                        // Replace() the Unicode replacement character with a space
                        game.gamename = gamename.Replace('�', ' ');
                        game.host = array[2];
                        game.ip = array[3];
                        game.id = array[7];
                        game.chan = channel;
                        games.Add(game);

                    }
                }
            }
            /*
            if (gamelist != null && gamelist.SelectedItem != null)
            {
                object select = gamelist.SelectedItem;
                game selectgame = (game)select;
                foreach (game game in games)
                {
                    if (selectgame.id == game.id)
                    {
                        select = game;
                        set_selected_item(select);
                    }
                }
            }
             */

            gamelist.SetObjects(games);

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

        public void readfile(string name)
        {
            StringBuilder newFile = new StringBuilder();
            string temp = "";
            string path;
            int length;

            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\zincldn");
            if (key.GetValue("path") != null)
            {
                path = (string)key.GetValue("path");
            }
            else
            {
                // path = @"C:\\Windows\\win.ini"; 
                path = @"C:\Team17\Worms Armageddon\WA.ini"; //path when using wkPrivateCfg.dll
            }

            if (File.Exists(path) == false)
            {
                MessageBox.Show("Error: Path to win.ini not found");
                return;
            }

            string[] file = File.ReadAllLines(path);
            foreach (string line in file)
            {
                if (line.Contains("PlayerName="))
                {
                    length = line.Length - 11;
                    temp = line.Remove(11, length);
                    temp = temp.Insert(11, name);
                    newFile.Append(temp + "\r\n");
                    continue;

                }
                newFile.Append(line + "\r\n");
            }
            File.WriteAllText(path, newFile.ToString(), Encoding.GetEncoding(0));

        }

        private void refresh_Click(object sender, EventArgs e)
        {
            updategamelist();

            if (gamelist.Visible == false)
            {
                gamelist.Visible = true;
                settings_panel.Visible = false;
                host_panel.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
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

        private void gamelist_CellRightClick(object sender, CellRightClickEventArgs e)
        {
            if (e.Model != null)
            {
                e.MenuStrip = this.DecideRightClickMenu(e.Model, e.Column, false);
            }
            else
            {
                e.MenuStrip = this.DecideRightClickMenu(e.Model, e.Column, true);
            }
        }

        private ContextMenuStrip DecideRightClickMenu(object model, object column, bool blank)
        {
            CustomProfessionalColors colorTable = new CustomProfessionalColors();
            contextMenu.Renderer = new ToolStripProfessionalRenderer(colorTable);
            contextMenu.Items.Clear();
            contextMenu.Padding = new Padding(0);
            contextMenu.Margin = new Padding(0);
            contextMenu.ClientSize = new Size(108, 140);
            //contextMenu.Height = -2;
            //contextMenu.AutoSize = true;

            ToolStripMenuItem item1 = new ToolStripMenuItem();
            item1.AutoSize = false;
            item1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            item1.ForeColor = System.Drawing.SystemColors.ControlLight;
            item1.Size = new System.Drawing.Size(107, 18);
            item1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            item1.Text = "join";


            ToolStripMenuItem item2 = new ToolStripMenuItem();
            item2.AutoSize = false;
            item2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            item2.ForeColor = System.Drawing.SystemColors.ControlLight;
            item2.Size = new System.Drawing.Size(107, 18);
            item2.Text = "copy url";

            ToolStripSeparator sep1 = new ToolStripSeparator();

            ToolStripMenuItem refresh1 = new ToolStripMenuItem();
            refresh1.AutoSize = false;
            refresh1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            refresh1.ForeColor = System.Drawing.SystemColors.ControlLight;
            refresh1.Size = new System.Drawing.Size(107, 18);
            refresh1.Text = "refresh";

            ToolStripSeparator sep2 = new ToolStripSeparator();

            ToolStripMenuItem host1 = new ToolStripMenuItem();
            host1.AutoSize = false;
            host1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            host1.ForeColor = System.Drawing.SystemColors.ControlLight;
            host1.Size = new System.Drawing.Size(107, 18);
            //host1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
            //host1.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            host1.Text = "host";

            ToolStripSeparator sep4 = new ToolStripSeparator();

            item1.Click += new System.EventHandler(contextjoinClick);
            item2.Click += new System.EventHandler(contextcopyClick);
            refresh1.Click += new System.EventHandler(contextrefreshClick);

            switch (blank)
            {
                //////////////////////////////
                case false:
                    //////////////////////////////
                    game game = (game)model;
                    storedgame = game; // Store game information in global var so contextjoinClick handler can access

                    ToolStripMenuItem ip1 = new ToolStripMenuItem();
                    ip1.AutoSize = false;
                    ip1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
                    ip1.ForeColor = System.Drawing.SystemColors.ControlLight;
                    ip1.Size = new System.Drawing.Size(125, 16);
                    ip1.Margin = new System.Windows.Forms.Padding(1, 0, 1, 0);
                    ip1.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
                    ip1.Text = game.ip.ToString();

                    contextMenu.Items.Add(item1);
                    contextMenu.Items.Add(item2);
                    contextMenu.Items.Add(sep1);
                    contextMenu.Items.Add(refresh1);
                    contextMenu.Items.Add(sep2);
                    contextMenu.Items.Add(host1);
                    return contextMenu;
                ///////////////////////////////////
                default:
                    ///////////////////////////////////

                    contextMenu.Items.Add(refresh1);
                    contextMenu.Items.Add(sep1);
                    contextMenu.Items.Add(host1);
                    return contextMenu;
            }
        }

        class CustomProfessionalColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            { get { return Color.FromArgb(64, 64, 64); } }
            public override Color MenuBorder
            { get { return Color.FromArgb(64, 64, 64); } }
            public override Color MenuItemBorder
            { get { return Color.FromArgb(48, 48, 48); } }
        }

        private void contextrefreshClick(object sender, EventArgs e)
        {
            updategamelist();
        }

        private void contextcopyClick(object sender, EventArgs e)
        {
            // Set correct scheme depending on channel
            // #ag wa://123.456.789:17011?gameID=1234567&scheme=Pf,Be
            // #pt &scheme=Ba
            // #rh &scheme=Pf
            string urlhandler = "wa://" + storedgame.ip + "?gameID=" + storedgame.id;
            if (storedgame.chan == "AnythingGoes") urlhandler += "&scheme=Pf,Be";
            if (storedgame.chan == "PartyTime") urlhandler += "&scheme=Ba";
            if (storedgame.chan == "RopersHeaven") urlhandler += "&scheme=Pf";
            Clipboard.SetText(urlhandler);
        }

        private void contextjoinClick(object sender, EventArgs e)
        {
            // Set correct scheme depending on channel
            // #ag wa://123.456.789:17011?gameID=1234567&scheme=Pf,Be
            // #pt &scheme=Ba
            // #rh &scheme=Pf
            string urlhandler = "wa://" + storedgame.ip + "?gameID=" + storedgame.id;
            if (storedgame.chan == "AnythingGoes") urlhandler += "&scheme=Pf,Be";
            if (storedgame.chan == "PartyTime") urlhandler += "&scheme=Ba";
            if (storedgame.chan == "RopersHeaven") urlhandler += "&scheme=Pf";
            System.Diagnostics.Process.Start(urlhandler);
        }

        private void gamelist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            game game = (game)gamelist.SelectedObject;
            storedgame = game;
            string urlhandler = "wa://" + storedgame.ip + "?gameID=" + storedgame.id;
            if (storedgame.chan == "AnythingGoes") urlhandler += "&scheme=Pf,Be";
            if (storedgame.chan == "PartyTime") urlhandler += "&scheme=Ba";
            if (storedgame.chan == "RopersHeaven") urlhandler += "&scheme=Pf";
            System.Diagnostics.Process.Start(urlhandler);
            // debug: form1.txtLog.AppendText("[ " + urlhandler + " ]");
        }

        private void refresh_mousedown(object sender, MouseEventArgs e)
        {
            refresh.BackgroundImage = Properties.Resources.refresh3_next;
        }

        private void refresh_mouseup(object sender, MouseEventArgs e)
        {
            refresh.BackgroundImage = Properties.Resources.refresh3;
        }

        private void host_mousedown(object sender, MouseEventArgs e)
        {
            host_btn.BackgroundImage = Properties.Resources.host2_next;
        }

        private void host_mouseup(object sender, MouseEventArgs e)
        {
            host_btn.BackgroundImage = Properties.Resources.host2;
        }

        private void settings_mousedown(object sender, MouseEventArgs e)
        {
            settings.BackgroundImage = Properties.Resources.settings2_next;
        }

        private void settings_mouseup(object sender, MouseEventArgs e)
        {
            settings.BackgroundImage = Properties.Resources.settings2;
        }

        private void settings_click(object sender, EventArgs e)
        {
            if (settings_panel.Visible == false)
            {
                settings_panel.Visible = true;
                host_panel.Visible = false;
                gamelist.Visible = false;
            }
            else if (settings_panel.Visible == true)
            {
                gamelist.Visible = true;
                host_panel.Visible = false;
                settings_panel.Visible = false;
            }
        }


        private void settings_close_click(object sender, EventArgs e)
        {
            gamelist.Visible = true;
            settings_panel.Visible = false;
            readfile(wormnet_name.Text);
        }

        private void bkgcolor_btn_click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            this.BackColor = colorDialog1.Color;
            settings_panel.BackColor = colorDialog1.Color;
            panel1.BackColor = colorDialog1.Color;
            bkg_color_color.BackColor = colorDialog1.Color;
            gamelist.BackColor = colorDialog1.Color;
            settings_panel.BackColor = colorDialog1.Color;
        }

        private void gamelist_bkg_click(object sender, EventArgs e)
        {
            colorDialog2.ShowDialog();
            gamelist.BackColor = colorDialog2.Color;
        }

        private void item_color_1_click(object sender, EventArgs e)
        {
            colorDialog3.ShowDialog();
        }

        private void item_color_2_Click(object sender, EventArgs e)
        {
            colorDialog4.ShowDialog();
            gamelist.AlternateRowBackColor = colorDialog4.Color;
        }



        private void highlight_color_click(object sender, EventArgs e)
        {
            colorDialog5.ShowDialog();
            gamelist.HighlightBackgroundColor = colorDialog5.Color;
        }

        private void text_color_click(object sender, EventArgs e)
        {
            colorDialog6.ShowDialog();
            gamelist.ForeColor = colorDialog6.Color;
        }

        private void Form4_Paint(object sender, PaintEventArgs e)
        {
            /* 1PX LINE BORDER
              
            Graphics surface = this.CreateGraphics();
            Pen pen1 = new Pen(bordercolor, 1.0f);
            Rectangle border = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            surface.DrawRectangle(pen1, border);

            */
        }

        private void border_color_Click(object sender, EventArgs e)
        {
            colorDialog7.ShowDialog();
            bordercolor = colorDialog7.Color;
        }

        private void Form4_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void resize_MouseHover(object sender, EventArgs e)
        {
            resize.Cursor = Cursors.SizeNWSE;
        }

        private void alwaysontop_changed(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                this.TopMost = true;
                frmMain.alwaysontop = 1;
            }
            else
            {
                this.TopMost = false;
                frmMain.alwaysontop = 0;
            }
        }

        private void hidechat_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                frmMain.hidechat();
            }
            else
            {
                frmMain.showchat();
            }
        }

        private void host_btn_Click(object sender, EventArgs e)
        {
            // ---HOST GAME HERE---
            // NEW PAGE (LIKE SETTINGS) ASKING FOR GAME NAME + CHANNEL TO HOST IN
            // HTTP GET HOST WITH RESPONSE (GAMEID)
            // START WA.EXE WITH CORRECT PARAMS

            if (host_panel.Visible == false)
            {
                host_panel.Visible = true;
                gamelist.Visible = false;
                settings_panel.Visible = false;
            }
            else if (host_panel.Visible == true)
            {
                host_panel.Visible = false;
                gamelist.Visible = true;
                settings_panel.Visible = false;
            }

            // END
        }
    }

}




