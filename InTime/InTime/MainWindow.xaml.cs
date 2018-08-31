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

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NotifyIcon inTimeIcon = new NotifyIcon();
        Stopwatch timeProject = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(WindowPositioned);

            // ICONA
            this.Hide();
            inTimeIcon.Icon = InTime.Properties.Resources.InTimeIcon;
            inTimeIcon.Visible = true;

            inTimeIcon.Click += new EventHandler(this.inTimeIcon_Click);
        }

        // POSIZIONE FINESTRA
        private void WindowPositioned(object sender, RoutedEventArgs e)
        {
            // RILEVAZIONE POSIZIONE MOUSE
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            var mousePosition = new System.Windows.Point(point.X, point.Y);
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            double mouseX = transform.Transform(mousePosition).X;

            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = mouseX - this.Width/2;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        // CLICK SU ICONA
        private void inTimeIcon_Click(object sender, EventArgs e)
        {
            this.Show();
            this.Activate(); // focus su finestra
            this.WindowState = WindowState.Normal;
        }

        // FOCUS FUORI DALLA FINESTRA PER NASCONDERLA
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            this.Hide();
        }

        private void AdministratorButton_Click(object sender, RoutedEventArgs e)
        {
            AdministratorWindow administratorWindow = new AdministratorWindow();
            administratorWindow.Show();
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

        private void TimeProject1_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(RunTime));
            thread.Start();
        }

        private void TimeProject2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RunTime()
        {
            TimeSpan ts = timeProject.Elapsed;
            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
            timeProject.Start();
            //TimeTextBlock.Text = elapsedTime;
        }
    }
}
