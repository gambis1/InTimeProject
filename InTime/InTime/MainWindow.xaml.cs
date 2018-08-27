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

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;

        public MainWindow()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                WindowState = WindowState.Normal;
                InitializeComponent();
            }
            this.Activate();
        }

        private void AdministratorButton_Click(object sender, RoutedEventArgs e)
        {
            AdministratorWindow administratorWindow = new AdministratorWindow();
            administratorWindow.Show();
        }
    }
}
