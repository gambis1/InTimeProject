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

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon inTimeIcon = new NotifyIcon();
        Stopwatch timeProject1 = new Stopwatch();
        Stopwatch timeProject2 = new Stopwatch();
        TimeTracker timeTracker;

        public MainWindow()
        {
            InitializeComponent();
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
            timeProject1.Start();
            timeProject2.Stop();

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
            inTimeIcon.Text = "Progetto 1: 00:00";
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            TimeSpan ts = timeProject1.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Project1Time.Text = elapsedTime;
            inTimeIcon.Text = "Progetto 1: " + elapsedTime;
        }

        private void RunTime()
        {
            timeTracker = new TimeTracker(1);
            timeTracker.Start();
            //TimeSpan ts = timeProject.Elapsed;
            //string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            //timeProject.Start();
            //TimeTextBlock.Text = elapsedTime;
        }


        /*-------------------------------------------------------------- TASTO PROGETTO 2 --------------------------------------------------------------*/

        // test dispatcher

        private void TimeProject2_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick2;
            timer.Start();
            timeProject2.Start();
            timeProject1.Stop();

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
            
        }

        void timer_Tick2(object sender, EventArgs e)
        {
            TimeSpan ts = timeProject2.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Project2Time.Text = elapsedTime;
            inTimeIcon.Text = "Progetto 2: " + elapsedTime;
        }

        /*-------------------------------------------------------------- TASTO STOP --------------------------------------------------------------*/

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            // TO DO: metodo per fermare il timer
            //Console.WriteLine(timeTracker.Update().ToString());
            //timeTracker.Stop();

            timeProject1.Stop();
            timeProject2.Stop();
            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/StoppedIcon.ico");
            inTimeIcon.Text = "Timer fermo";
        }


    }
}
