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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        inTimeDbEntities intimeDb = new inTimeDbEntities();

        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetDbData();
        }

        // aggiungere un nuovo progetto al database
        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            string projectName = NewProjectNameBox.Text;

            if (projectName != "")
            {
                InTime.Project newProject = new InTime.Project();
                newProject.ProjectName = projectName;

                intimeDb.Projects.Add(newProject);
                intimeDb.SaveChanges();

                GetDbData();
                NewProjectNameBox.Text = "";
            }
            else
            {
                MessageBox.Show("Il nome del progetto non è stato inserito. Inserire un nome progetto valido, quindi riprovare.","Nome progetto non valido",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

            foreach(string projectName in list)
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

            MessageBox.Show(itm.Content.ToString());
        }
    }
}
