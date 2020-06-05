using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Media;
using System.Drawing.Design;


namespace MarioGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }
        public StreamReader STR;
        public StreamWriter STW;
        public string Received;
        public string Received2;
        public string ToBeSent;
        public bool isConnected;
        private TcpClient client;
        public bool Begin = false;
        public bool twoplayers = false;
        bool RightisPressed = false;
        bool LeftisPressed = false;
        bool UpisPressed = false;
        bool DisPressed = false;
        bool AisPressed = false;
        bool WisPressed = false;
        int CreatureCount = 10;
        int timertickcounter = 0;
        Font f;
        Graphics g;
        List<items> map;
        SolidBrush b;
        Creature Mario;
        List<Creature> Enemies;
        Creature luigi;
        int[] CurrentPlayer = new int[2];
        int[] OtherPlayer = new int[2];
        Image[] Map = new Image[5];
        Image[] TurtlesR = new Image[5];
        Image[] TurtlesL = new Image[5];
        Image[] MarioL = new Image[5];
        Image[] MarioR = new Image[4];
        Image[] LuigiL = new Image[5];
        Image[] LuigiR = new Image[4];
        Image[] FlippedTurtle = new Image[4];
      
        Image ElevatedBlock;
        public bool deadmusic=false;
        public bool Ldeadmusic = false;


        SoundPlayer Jump = new SoundPlayer();
        SoundPlayer Gameover = new SoundPlayer();
        SoundPlayer Mariodead = new SoundPlayer();
        SoundPlayer Hitblock = new SoundPlayer();
        SoundPlayer Stomp = new SoundPlayer();

        public bool isHost = true;
        IPAddress HostIP;
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {

                MarioL[i] = Image.FromFile(Application.StartupPath + @"\Mario\" + (i + 1) + ".png");
            }
            for (int i = 0; i < 4; i++)
            {

                MarioR[i] = Image.FromFile(Application.StartupPath + @"\Mario\Right\" + (i + 1) + ".png");
            }
            for (int i = 0; i < 5; i++)
            {

                LuigiL[i] = Image.FromFile(Application.StartupPath + @"\Luigi\" + (i + 1) + ".png");
            }
            for (int i = 0; i < 4; i++)
            {

                LuigiR[i] = Image.FromFile(Application.StartupPath + @"\Luigi\Right\" + (i + 1) + ".png");
            }
            for (int i = 0; i < 5; i++)
            {

                Map[i] = Image.FromFile(Application.StartupPath + @"\map\" + (i) + ".png");
            }
            for (int i = 0; i < 5; i++)
            {

                TurtlesR[i] = Image.FromFile(Application.StartupPath + @"\Turtles\Alive\" + (i + 1) + ".png");
            }
            for (int i = 0; i < 3; i++)
            {

                FlippedTurtle[i] = Image.FromFile(Application.StartupPath + @"\Turtles\Dead\" + (i + 1) + ".png");
            }
            for (int i = 0; i < 5; i++)
            {

                TurtlesL[i] = Image.FromFile(Application.StartupPath + @"\Turtles\Alive\Left\" + (i + 1) + ".png");
            }
            
            
            map = new List<items>();
         
            items TL = new items(0, ClientRectangle.Height / 4 - 2, ClientRectangle.Width / 2 - 150, 30, Map[4]);
            items TR = new items(ClientRectangle.Width - ClientRectangle.Width / 2 + 150, ClientRectangle.Height / 4 - 2, ClientRectangle.Width / 2 - 150, 30, Map[4]);
            items M = new items(ClientRectangle.Width / 2 - 450, ClientRectangle.Height / 2 - 1, 900, 30, Map[3]);
            items ML = new items(0, ClientRectangle.Height / 2 + 29, 225, 30, Map[2]);
            items MR = new items(ClientRectangle.Width - 225, ClientRectangle.Height / 2 + 29, 225, 30, Map[2]);
            items BL = new items(0, 3 * ClientRectangle.Height / 4, ClientRectangle.Width / 2 - 270, 30, Map[1]);
            items BR = new items(ClientRectangle.Width - ClientRectangle.Width / 2 + 270, 3 * ClientRectangle.Height / 4, ClientRectangle.Width / 2 - 270, 30, Map[1]);
            items GR = new items(0, ClientRectangle.Height - 60, ClientRectangle.Width, 60, Map[0]);

            luigi = new Creature(ClientRectangle.Width - 360, ClientRectangle.Height - 150, 60, 90, Map[0]);
            luigi.cdirection = true;

            Mario = new Creature(ClientRectangle.Width - 300, ClientRectangle.Height - 150, 60, 90, Map[0]);
            ElevatedBlock = Image.FromFile(Application.StartupPath + @"\elevatedblock.png");
            Enemies = new List<Creature>();

            Jump.SoundLocation = Application.StartupPath + @"\MarioSounds\Jump.wav";
            Gameover.SoundLocation = Application.StartupPath + @"\MarioSounds\Gameover.wav";
            Stomp.SoundLocation = Application.StartupPath + @"\MarioSounds\Stomp.wav";
            Hitblock.SoundLocation = Application.StartupPath + @"\MarioSounds\Hitblock.wav";
            Mariodead.SoundLocation = Application.StartupPath + @"\MarioSounds\Mariodead.wav";


            Jump.LoadAsync();
            Gameover.LoadAsync();
            Stomp.LoadAsync();
            Hitblock.LoadAsync();
            Mariodead.LoadAsync();

            
            map.Add(GR);
            map.Add(BL);
            map.Add(BR);
            map.Add(ML);
            map.Add(MR);
            map.Add(M);
            map.Add(TL);

            map.Add(TR);


            axWindowsMediaPlayer1.URL = Application.StartupPath + @"\MarioSounds\Intro.mp3";



        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            if (Begin)
                draw(g);
            else
                g.Clear(this.BackColor);
           
        }


        private void draw(Graphics g)
        {
          //  ClientControl();

            foreach (items x in map)
                g.DrawImage(x.CurrentImage, x.actual);

            foreach (Creature c in Enemies)
                g.DrawImage(c.CurrentImage, c.actual);
            g.DrawImage(Image.FromFile(Application.StartupPath + @"\Pipes\11.png"), new Rectangle(-50, 130, 192, 100));
            g.DrawImage(Image.FromFile(Application.StartupPath + @"\Pipes\1.png"), new Rectangle(ClientRectangle.Width - 142, 130, 192, 100));
            g.DrawImage(Image.FromFile(Application.StartupPath + @"\Pipes\22.png"), new Rectangle(-50, ClientRectangle.Height - 160, 192, 100));
            g.DrawImage(Image.FromFile(Application.StartupPath + @"\Pipes\2.png"), new Rectangle(ClientRectangle.Width - 142, ClientRectangle.Height - 160, 192, 100));
            if (twoplayers)
            {
                g.DrawImage(luigi.CurrentImage, luigi.actual);
                if (luigi.hitblock)
                    g.DrawImage(ElevatedBlock, luigi.hitarea);
            }


            g.DrawImage(Mario.CurrentImage, Mario.actual);
            if (Mario.hitblock)
                g.DrawImage(ElevatedBlock, Mario.hitarea);



        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (Begin)
            {

                if (e.KeyCode == Keys.Up)
                {


                     ToBeSent = "U";
                    UpisPressed = true;
                    if(!Mario.isDead)
                        Jump.Play();
                    if (!Mario.IsJumping && Mario.MarioCollideBot(map))
                        Mario.MarioJump2(map, UpisPressed);


                }


                else if (e.KeyCode == Keys.Left)
                {

                    ToBeSent = "L";
                    LeftisPressed = true;


                }

                else if (e.KeyCode == Keys.Right)
                {

                     ToBeSent = "R";
                    RightisPressed = true;



                }


                if (twoplayers)
                {
                    if (e.KeyCode == Keys.W)
                    {



                        WisPressed = true;
                        if(!luigi.isDead)
                            Jump.Play();
                        if (!luigi.IsJumping && luigi.MarioCollideBot(map))
                            luigi.MarioJump2(map, WisPressed);


                    }


                    else if (e.KeyCode == Keys.A)
                    {
                        AisPressed = true;
                        
                    }

                    else if (e.KeyCode==Keys.D)
                        {
                        DisPressed = true;


                    }
                }

                if (isConnected)
                 backgroundWorker2_Sender.RunWorkerAsync();
                
                Invalidate();
        }
    }
    private void Form1_KeyUp(object sender, KeyEventArgs e)
    {
        if (Begin)
        {
            switch (e.KeyCode)
            {
                case (Keys.NumPad0):


                    break;
                case (Keys.Up):
                    Mario.Up = false;
                    UpisPressed = false;
                       ToBeSent = "!U";
                    break;
                case (Keys.Left):
                    LeftisPressed = false;
                        ToBeSent = "!L";
                    break;
                case (Keys.Right):

                    RightisPressed = false;
                        ToBeSent = "!R";
                    break;
            }
            if (twoplayers)
                switch (e.KeyCode)
                {  /////////////////////////////////////// LUIGI CONTROLS //////////////////////////////////////////////////////
                    case (Keys.W):
                        luigi.Up = false;
                        WisPressed = false;
                        break;
                    case (Keys.A):
                        AisPressed = false;

                        break;
                    case (Keys.D):

                        DisPressed = false;
                        break;

                }
               
                    if (isConnected)
                        backgroundWorker3_Sender.RunWorkerAsync();
                }
    }
    private void ClientControl()
    {
        if (Begin)
        {
            switch (Received)
            {
                case ("U"):
                    WisPressed = true;
                    if (!luigi.IsJumping && luigi.MarioCollideBot(map))
                        luigi.MarioJump2(map, WisPressed);
                    break;
                case ("L"):
                    AisPressed = true;
                    break;
                case ("R"):
                    DisPressed = true;
                    break;
                case ("!U"):
                    luigi.Up = false;
                    WisPressed = false;
                    break;
                case ("!R"):
                    DisPressed = false;
                    break;
                case ("!L"):
                    AisPressed = false;
                    break;
            }
        }
    }
    private void timer1_Tick(object sender, EventArgs e)
    {
            ////////////////////////////////////////////////// SEND/RECEIVE /////////////////////////////////////////////////


            ClientControl();
            if (!deadmusic)
                if (Mario.isDead || luigi.isDead)
                {
                    Mariodead.Play();
                    deadmusic = true;
                }
            if(!Ldeadmusic)
             if(luigi.isDead&&Mario.isDead)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.pause();
                    Gameover.Play();
                    Ldeadmusic = true;
                }

           

        
            



            /////////////////////////////////////////////// LUIGI FUNCTIONS /////////////////////////////////////////////////
            if (twoplayers)
        {
            luigi.Inertia();
            luigi.MoveMario(WisPressed, AisPressed, DisPressed, map, this, LuigiR, LuigiL);
            luigi.HitCreature(Enemies, map, FlippedTurtle, Stomp);
            luigi.HitMario(Enemies, LuigiL);
            luigi.gravity(map);

            luigi.HitBlock(map);

        }

        ///////////////////////////////////////////// MARIO FUNCTIONS //////////////////////////////////////////////

        Mario.Inertia();
        Mario.MoveMario(UpisPressed, LeftisPressed, RightisPressed, map, this, MarioR, MarioL);
        Mario.moveCreature(Enemies, this, TurtlesL, TurtlesR);
        Mario.HitCreature(Enemies, map, FlippedTurtle, Stomp);
        Mario.HitBlock(map);
        Mario.gravity(map);
        Mario.HitMario(Enemies, MarioL);
        foreach (Creature c in Enemies)
            c.gravity(map);

        Invalidate();
    }




    private void timer1_Tick_1(object sender, EventArgs e)
    {
        if (timertickcounter < CreatureCount)
        {

            timertickcounter++;
            Mario.createCreatures(Enemies, timertickcounter, this, TurtlesR[0]);
        }
        Invalidate();
    }




    /////////////////////////////////////////////////////// SOCKET PROGRAMMING ///////////////////////////////////////////////////////////////////////////////////////
    private void hostToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //TcpListener listener = new TcpListener(IPAddress.Parse("192.168.1.17"), 10000);
        //listener.Start();
        //client = listener.AcceptTcpClient();
        //isConnected = true;
        //STR = new StreamReader(client.GetStream());
        //STW = new StreamWriter(client.GetStream());
        //STW.AutoFlush = true;

        //backgroundWorker1_Receiver.RunWorkerAsync();
        //backgroundWorker2_Sender.WorkerSupportsCancellation = true;
        //backgroundWorker3_Sender.WorkerSupportsCancellation = true;

    }

    private void backgroundWorker1_Receiver_DoWork(object sender, DoWorkEventArgs e)
    {
        while (client.Connected)
        {
            try
            {
                Received = STR.ReadLine();
               
            }
            catch (Exception x)
            {
                MessageBox.Show(x.Message.ToString());
            }

        }
    }

    private void backgroundWorker2_Sender_DoWork(object sender, DoWorkEventArgs e)
    {
        if (isConnected)
            if (client.Connected)
            {
                STW.WriteLine(ToBeSent);
            }
    }

    private void joinToolStripMenuItem_Click(object sender, EventArgs e)
    {
        //client = new TcpClient();
        //IPEndPoint IP_End = new IPEndPoint(IPAddress.Parse("192.168.1.17"), 10000);
        //try
        //{
        //    client.Connect(IP_End);
        //    if (client.Connected)
        //    {
        //        MessageBox.Show("Connected!");
        //        isConnected = true;
        //        if(Received=="Start")

        //        STW = new StreamWriter(client.GetStream());
        //        STR = new StreamReader(client.GetStream());
        //        STW.AutoFlush = true;
        //        backgroundWorker1_Receiver.RunWorkerAsync();
        //        backgroundWorker2_Sender.WorkerSupportsCancellation = true;
        //    }
        //}
        //catch (Exception x)
        //{
        //    MessageBox.Show(x.Message.ToString());
        //}
    }

    private void backgroundWorker3_Sender_DoWork(object sender, DoWorkEventArgs e)
    {
        if (isConnected)
            if (client.Connected)
            {
                STW.WriteLine(ToBeSent);
            }
    }
    //////////////////////////////////////////////////////  ANIMATION TIMER ///////////////////////////////////////////////////////////////
    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        foreach (Creature c in Enemies)
        {
            if (c.CisHit)

            {
                if (c.Counter < 2)
                    c.Counter++;
                else
                    c.Counter = 0;
                c.CurrentImage = FlippedTurtle[c.Counter];
            }
            else
            {
                if (c.Counter < 4)
                    c.Counter++;
                else
                    c.Counter = 0;
            }

        }

        if (Mario.Counter < 2)
            Mario.Counter++;
        else
            Mario.Counter = 0;

        if (twoplayers)
        {
            if (luigi.Counter < 2)
                luigi.Counter++;
            else
                luigi.Counter = 0;
        }
    }
    //////////////////////////////////////// STARTING MENU AND FORM FUNCTIONS ///////////////////////////////////////
    private void StartGame()
    {
            HideFormControls();
        GravityTimer.Enabled = true;
        AnimationTimer.Enabled = true;
        CreatureCreationTimer.Enabled = true;
        Enemies.Clear();
            if (isHost)
            {
                Mario.actual.X = 300;
                Mario.actual.Y = ClientRectangle.Height - 150;
            }
            else
            {
                Mario.actual.X = ClientRectangle.Width - 360;
                Mario.actual.Y = ClientRectangle.Height - 150;
            }
            Mario.isDead = false;
             
        Mario.Counter = 0;
        Mario.DeadCreatureCounter = 0;
        if (twoplayers)
        {
                if (isHost)
                {

                    luigi.actual.X = ClientRectangle.Width - 360;
                    luigi.actual.Y = ClientRectangle.Height - 150;
                }
                else
                {
                    luigi.actual.X = 300;
                    luigi.actual.Y = ClientRectangle.Height - 150;
                }
                luigi.isDead = false;
            luigi.Counter = 0;
            luigi.DeadCreatureCounter = 0;
        }
        timertickcounter = 0;
    }
    private void resetToolStripMenuItem_Click(object sender, EventArgs e)
    {
            Begin = false;
            twoplayers = false;
            GravityTimer.Enabled = false;
            AnimationTimer.Enabled = false;
            Enemies.Clear();
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            pictureBox1.Visible = true;
            

        }

    private void startToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void button3_Click(object sender, EventArgs e)
    {
        button4.Visible = true;
        button6.Visible = true;
        button3.Visible = false;
    }

    private void button4_Click(object sender, EventArgs e)
    {
        twoplayers = true;
        isHost = true;
        TcpListener listener = new TcpListener(IPAddress.Parse(GetLocalIPAddress()), 10000);
        listener.Start();
        client = listener.AcceptTcpClient();
        isConnected = true;
          
            isConnected = true;
            twoplayers = true;
            StartGame();
            Begin = true;
           
        STR = new StreamReader(client.GetStream());
        STW = new StreamWriter(client.GetStream());
        STW.AutoFlush = true;

        backgroundWorker1_Receiver.RunWorkerAsync();
        backgroundWorker2_Sender.WorkerSupportsCancellation = true;
        backgroundWorker3_Sender.WorkerSupportsCancellation = true;


    }

    private void button6_Click(object sender, EventArgs e)
    {
        textBox1.Visible = true;
        isHost = false;

    }
    public static string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
        ConnectButton.Visible = true;
    }

    private void ConnectButton_Click(object sender, EventArgs e)
    {
        twoplayers = true;
        HostIP = IPAddress.Parse(textBox1.Text);
        client = new TcpClient();
        IPEndPoint IP_End = new IPEndPoint(HostIP, 10000);
        try
        {
            client.Connect(IP_End);
            if (client.Connected)
            {
               
                isConnected = true;
                    twoplayers = true;
                    StartGame();
                    Begin = true;
                
               
                STW = new StreamWriter(client.GetStream());
                STR = new StreamReader(client.GetStream());
                STW.AutoFlush = true;
                backgroundWorker1_Receiver.RunWorkerAsync();
                backgroundWorker2_Sender.WorkerSupportsCancellation = true;
            }
        }
        catch (Exception x)
        {
            MessageBox.Show(x.Message.ToString());
        }
    }

    private void button4_MouseHover(object sender, EventArgs e)
    {
        label1.Visible = true;
        label1.Text += GetLocalIPAddress();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        
        StartGame();
        Begin = true;
    }

    private void HideFormControls()
    {
        button1.Visible = false;
        button2.Visible = false;
        button3.Visible = false;
        button4.Visible = false;
        ConnectButton.Visible = false;
        pictureBox1.Visible = false;
        label1.Visible = false;
        textBox1.Visible = false;
        button6.Visible = false;
           
    }

    private void button2_Click(object sender, EventArgs e)
    {
            Begin = true;
            twoplayers = true;
            StartGame();
        
    }

    private void button5_Click(object sender, EventArgs e)
    {
        Begin = true;
      
        twoplayers = true;
        StartGame();

       
      
    }
}
}

