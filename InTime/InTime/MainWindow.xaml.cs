﻿using System;
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
using Hardcodet.Wpf.TaskbarNotification;
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

            this.Hide();
            inTimeIcon.DoubleClick += new EventHandler(this.inTimeIcon_DoubleClick);

            inTimeIcon.Visible = true;

            inTimeIcon.Icon = InTime.Properties.Resources.InTimeIcon;            
        }

        private void inTimeIcon_DoubleClick(object sender, EventArgs e)
        {
                this.Show();
                this.WindowState = WindowState.Normal;
        }

        private void AdministratorButton_Click(object sender, RoutedEventArgs e)
        {
            AdministratorWindow administratorWindow = new AdministratorWindow();
            administratorWindow.Show();
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
            TimeTextBlock.Text = elapsedTime;
        }
    }
}
