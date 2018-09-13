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
using Button = System.Windows.Controls.Button;

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon inTimeIcon = new NotifyIcon();        
        InTimeDbEntities intimeDb = new InTimeDbEntities();
        public static Person currentUser;

        private static Assignment currentTrackedAssignment;
        private static TextBlock currentUITime;

        private Stopwatch stopwatch;
        private TimeTracker timeTracker;
        private DispatcherTimer secondstimer;
        private DispatcherTimer minutesTimer;

        public MainWindow()
        {
            InitializeComponent();
            InitalizeTrackers();
            this.Loaded += new RoutedEventHandler(WindowPositioned);
            this.Hide();

            GetCurrentUser();
            HideAdminButton(AdministratorButton);

            PopulateTrackingWindow();

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/StoppedIcon.ico"); // vecchio percorso icona: InTime.Properties.Resources.InTimeIcon;
            inTimeIcon.Visible = true;
            inTimeIcon.Text = "Timer fermo";
            inTimeIcon.Click += new EventHandler(this.inTimeIcon_Click);

            UserWindow userWindow = new UserWindow();
            userWindow.Show();

            //AdministratorWindow administratorWindow = new AdministratorWindow();
            //administratorWindow.Show();
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
            AdministratorWindow.IsOpened();
        }

        private void UserButton_Click(object sender, RoutedEventArgs e)
        {
            UserWindow.IsOpened();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Vuoi uscire dall'applicazione?", "Esci", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                if (timeTracker.IsRunning())
                    timeTracker.Stop();
                stopwatch.Stop();
                inTimeIcon.Dispose(); // fa scomparire l'icona dalla barra
                System.Windows.Application.Current.Shutdown();
            } else
            {
                this.Show();
            }            
        }

        /*-------------------------------------------------------------- CREAZIONE LISTA PROGETTI --------------------------------------------------------------*/

        private void PopulateTrackingWindow()
        {
            DbSet<Assignment> AssignmentsDBSet = intimeDb.Assignments;
            List<Assignment> assignmentList = (from Assignment in AssignmentsDBSet
                                               where Assignment.PersonId == currentUser.Id
                                               select Assignment).ToList();

            foreach(Assignment assignment in assignmentList)
            {
                AddProject(assignment);
            }
        }

        private void AddProject(Assignment assignment)
        {
            string projectName = assignment.Project.ProjectName;

            // DOCKPANEL
            DockPanel dockPnl = new DockPanel
            {
                Name = "dockPnl",
                Margin = new Thickness(10)
            };

            // CURRENT TIME TRACKER
            TextBlock projectTimeBlock = new TextBlock()
            {
                Name = "projectTimeBlock",
                Text = "00:00:00",
                FontSize = 15,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
            };
            dockPnl.Children.Add(projectTimeBlock);
            DockPanel.SetDock(projectTimeBlock, Dock.Right);

            Object[] projectArray = new Object[] { assignment, projectTimeBlock };

            // START TRACKING BUTTON
            Button startBtn = new Button
            {
                Name = "trackBtn",
                Tag = projectArray,
                Content = "Lavora a \"" + projectName + "\"",
                Padding = new Thickness(10),
                Margin = new Thickness(0, 10, 0, 0)
            };
            startBtn.Click += StartTracking_Click;
            dockPnl.Children.Add(startBtn);
            DockPanel.SetDock(startBtn, Dock.Bottom);

            // PROJECT NAME LABEL
            TextBlock projectLabel = new TextBlock
            {
                Name = "projectLabel",
                Text = projectName,
                FontSize = 15,
                VerticalAlignment = VerticalAlignment.Center,
            };
            dockPnl.Children.Add(projectLabel);
            DockPanel.SetDock(projectLabel, Dock.Left);

            ProjectButtons.Children.Add(dockPnl);
            ProjectButtons.Children.Add(new Separator());
        }

        /*-------------------------------------------------------------- TASTI PROGETTI --------------------------------------------------------------*/

        private void StartTracking_Click(object sender, RoutedEventArgs e)
        {
            Object[] assignmentAndTimeBlock = (Object[])((Button)sender).Tag;

            currentTrackedAssignment = (Assignment)assignmentAndTimeBlock[0];
            currentUITime = (TextBlock)assignmentAndTimeBlock[1];

            secondstimer.Start();
            minutesTimer.Start();
            timeTracker = new TimeTracker(currentTrackedAssignment.ProjectId, currentTrackedAssignment.PersonId);
            currentUITime.Text = timeTracker.Start(); // DA FINIRE: timetracker.start() aggiorna project 2 -> da dinamizzare

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
        }

        /*-------------------------------------------------------------- TASTO PROGETTO 1 --------------------------------------------------------------*/

        private void TimeProject1_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick1;
            timer.Start();
            stopwatch.Start();

            if (timeTracker.IsRunning())
                timeTracker.Stop(); // ricordiamoci che è provvisorio e non va

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
            inTimeIcon.Text = "Progetto 1: 00:00";
        }

        void timer_Tick1(object sender, EventArgs e)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            Project1Time.Text = elapsedTime;
            inTimeIcon.Text = "Progetto 1: " + elapsedTime;
        }

        /*-------------------------------------------------------------- TASTO PROGETTO 2 --------------------------------------------------------------*/

        private void TimeProject2_Click(object sender, RoutedEventArgs e)
        {
            secondstimer.Start();
            minutesTimer.Start();

            timeTracker = new TimeTracker(10, 1);
            Project2Time.Text = timeTracker.Start();

            stopwatch.Stop(); // TO DO: da dinamizzare

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/PlayingIcon.ico");
        }


        /*-------------------------------------------------------------- TASTO STOP --------------------------------------------------------------*/

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            StopAll();
        }

        private void StopAll() // stoppa tutti i timer
        {
            timeTracker.Stop();
            secondstimer.Stop();
            minutesTimer.Stop();
            stopwatch.Stop();
            timeTracker.Stop();

            inTimeIcon.Icon = new System.Drawing.Icon("../../Resources/StoppedIcon.ico");
            inTimeIcon.Text = "Timer fermo";
        }


        /*-------------------------------------------------------------- TIME TRACKERS --------------------------------------------------------------*/

        private void InitalizeTrackers() // avvia tutto
        {
            stopwatch = new Stopwatch();

            secondstimer = new DispatcherTimer();
            secondstimer.Interval = TimeSpan.FromSeconds(1);
            secondstimer.Tick += time_UpdateUI;

            minutesTimer = new DispatcherTimer();
            minutesTimer.Interval = TimeSpan.FromMinutes(1);
            minutesTimer.Tick += timer_UpdateDataBase;
            minutesTimer.Tick += timer_checkIdle;
        }

        void time_UpdateUI(object sender, EventArgs e) // aggiorna il timer dell'interfaccia
        {
            string elapsedTime = timeTracker.GetCurrentTime();
            Project2Time.Text = elapsedTime;
            inTimeIcon.Text = "Progetto 2: " + elapsedTime;
        }

        void timer_UpdateDataBase(object sender, EventArgs e) // aggiorna tempo nel database
        {
            timeTracker.Update();
        }

        void timer_checkIdle(object sender, EventArgs e) // verifica inattività superiore ai 5 minuti
        {
            if (IdleDetection.GetIdleTime() >= 300000)
            {
                StopAll();

                MessageBoxResult idle = System.Windows.MessageBox.Show("A causa di un'inattività prolungata, il timer del progetto è stato arrestato. Clicca nuovamente su un progetto per ricominciare a tracciare il tempo di lavoro.", "InTime", MessageBoxButton.OK);
                this.Show();
            }
        }

        /*-------------------------------------------------------------- SETTINGS --------------------------------------------------------------*/

        private void GetCurrentUser()
        {
            try
            {
                Guid userUniqueIdentifier = Properties.Settings.Default.UniqueIdentifier;

                InTimeDbEntities intimeDb = new InTimeDbEntities();

                DbSet<Person> personDbList = intimeDb.People;

                currentUser = (from Person in personDbList
                               where Person.AccessCode == userUniqueIdentifier
                               select Person).Single();
            }
            catch (Exception e)
            {
                FirstLogin firstLogin = new FirstLogin();
                firstLogin.ShowDialog();
                firstLogin.Close();
                GetCurrentUser();
            }
        }

        private void HideAdminButton(System.Windows.Controls.Button AdminButton)
        {
            if (currentUser.Id != 1) // ADMIN
            {
                AdminButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
