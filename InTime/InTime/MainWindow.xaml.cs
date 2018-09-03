using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using InTime.User;
using InTime.Admin;
using InTime.Logic;
using System.Windows.Threading;
using System.Data.Entity;
using System.Runtime.InteropServices;

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon inTimeIcon = new NotifyIcon();
        private Stopwatch project1_stopwatch;
        private TimeTracker timeTracker;
        private DispatcherTimer secondstimer;
        private DispatcherTimer minutesTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitalizeTrackers();
            this.Loaded += new RoutedEventHandler(WindowPositioned);
            this.Hide();

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/StoppedIcon.ico"); // vecchio percorso icona: InTime.Properties.Resources.InTimeIcon;
            inTimeIcon.Visible = true;
            inTimeIcon.Text = "Timer fermo";
            inTimeIcon.Click += new EventHandler(this.inTimeIcon_Click);            
        }


        /*-------------------------------------------------------------- FINESTRA --------------------------------------------------------------*/

        private void WindowPositioned(object sender, RoutedEventArgs e) // posizione finestra sopra icona
        {
            // rilevazione posizione mouse
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            var mousePosition = new System.Windows.Point(point.X, point.Y);
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            double mouseX = transform.Transform(mousePosition).X;

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = mouseX - this.Width/2;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        private void inTimeIcon_Click(object sender, EventArgs e) // apertura finestra al click su icona
        {
            this.Show();
            this.Activate(); // focus sulla finestra
            this.WindowState = WindowState.Normal;
        }

        protected override void OnDeactivated(EventArgs e) // nascondere finestra quando perde focus (es. click fuori)
        {
            base.OnDeactivated(e);
            this.Hide();
        }


        /*-------------------------------------------------------------- TASTI MENU --------------------------------------------------------------*/

        private void AdministratorButton_Click(object sender, RoutedEventArgs e)
        {
            AdministratorWindow administratorWindow = new AdministratorWindow();
            administratorWindow.Show();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();
            userWindow.Show();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Vuoi uscire dall'applicazione?", "Esci", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                timeTracker.Stop();
                project1_stopwatch.Stop();
                inTimeIcon.Dispose(); // fa scomparire l'icona dalla barra
                System.Windows.Application.Current.Shutdown();
            } else
            {
                this.Show();
            }            
        }


        /*-------------------------------------------------------------- TASTO PROGETTO 1 --------------------------------------------------------------*/

        private void TimeProject1_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick1;
            timer.Start();
            project1_stopwatch.Start();

            timeTracker.Stop();


            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
            inTimeIcon.Text = "Progetto 1: 00:00";
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            TimeSpan ts = project1_stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Project1Time.Text = elapsedTime;
            inTimeIcon.Text = "Progetto 1: " + elapsedTime;
        }

        /*-------------------------------------------------------------- TASTO PROGETTO 2 --------------------------------------------------------------*/

        // test dispatcher

        private void TimeProject2_Click(object sender, RoutedEventArgs e)
        {
            secondstimer.Start();
            minutesTimer.Start();


            timeTracker = new TimeTracker(1, 1);
            Project2Time.Text = timeTracker.Start();

            project1_stopwatch.Stop();

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
        }



        /*-------------------------------------------------------------- TASTO STOP --------------------------------------------------------------*/

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            StopAll();
        }

        private void StopAll()
        {
            timeTracker.Stop();
            secondstimer.Stop();
            minutesTimer.Stop();

            project1_stopwatch.Stop();
            timeTracker.Stop();


            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/StoppedIcon.ico");
            inTimeIcon.Text = "Timer fermo";
        }


        /*-------------------------------------------------------------- TRACKERS --------------------------------------------------------------*/

        private void InitalizeTrackers()
        {
            project1_stopwatch = new Stopwatch();

            secondstimer = new DispatcherTimer();
            secondstimer.Interval = TimeSpan.FromSeconds(1);
            secondstimer.Tick += time_UpdateUI;

            minutesTimer = new DispatcherTimer();
            minutesTimer.Interval = TimeSpan.FromMinutes(1);
            minutesTimer.Tick += timer_UpdateDataBase;
        }

        void timer_UpdateDataBase(object sender, EventArgs e)
        {
            timeTracker.Update();
            if (GetIdleTime() >= 300000)
            {
                StopAll();
            }
        }

        void time_UpdateUI(object sender, EventArgs e)
        {
            string elapsedTime = timeTracker.UpdateSecond();
            Project2Time.Text = elapsedTime;
            inTimeIcon.Text = "Progetto 2: " + elapsedTime;
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        internal struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInput = new LASTINPUTINFO();
            lastInput.cbSize = (uint)Marshal.SizeOf(lastInput);
            GetLastInputInfo(ref lastInput);
            return (uint)Environment.TickCount - lastInput.dwTime);
        }
    }
}
