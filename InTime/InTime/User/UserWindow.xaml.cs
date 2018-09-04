using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace InTime.User
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();
        private static UserWindow userWindow;

        public UserWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetDbData();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GetDbData()
        {
            DbSet<Project> projectList = intimeDb.Projects;

            var query =
                from Project in projectList
                orderby Project.Id
                select Project.ProjectName;

            List<string> list = query.ToList();

            ProjectList.Items.Clear();

            foreach (string projectName in list)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = projectName;
                ProjectList.Items.Add(itm);
                itm.Selected += new RoutedEventHandler(ListBoxItem_Selected);
            }

        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            ListBoxItem itm = (ListBoxItem)sender;
            string projectName = itm.Content.ToString();
            TextBlockProject.Text = projectName;

            DbSet<Project> projectList = intimeDb.Projects;

            var queryDesc = (from Project in projectList
                             where Project.ProjectName == projectName
                             select Project.Description).FirstOrDefault();

            Description.Text = queryDesc;

            DbSet<TimeTrack> timeTracks = intimeDb.TimeTracks;

            //var queryTime = (from TimeTrack in timeTracks
            //                 join Project in projectList on TimeTrack.ProjectId equals Project.Id
            //                 select TimeTrack.WorkTime).FirstOrDefault().ToString();

            //WorkTime.Text = queryTime;

            // TO DO: aggiungere metodo che aggiorna anche la datagrid
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void IsOpened()
        {
            if (userWindow == null)
            {
                userWindow = new UserWindow();
                userWindow.Closed += (sender, args) => userWindow = null;
                userWindow.Show();
            }
        }
    }
}
