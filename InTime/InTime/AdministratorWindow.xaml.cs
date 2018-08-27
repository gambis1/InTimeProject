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
using System.Xml.Linq;

namespace InTime
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        inTimeDbEntities intimeDb = new inTimeDbEntities();

        public AdministratorWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetDbData();
            GetDbPerson();
        }

        // aggiungere un nuovo progetto al database
        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            
                InTime.Project newProject = new InTime.Project();
                newProject.ProjectName = "Nuovo progetto";

                intimeDb.Projects.Add(newProject);
                intimeDb.SaveChanges();

                GetDbData(); 
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
            string projectName = itm.Content.ToString();
            TextBlockProject.Text = projectName;

            DbSet<Project> projectList = intimeDb.Projects;

            var queryDesc = (from Project in projectList
                        where Project.ProjectName == projectName
                        select Project.Description).FirstOrDefault();

            Description.Text = queryDesc;

            DbSet<TimeTrack> timeTracks = intimeDb.TimeTracks;

            var queryTime = (from TimeTrack in timeTracks
                            join Project in projectList on TimeTrack.ProjectId equals Project.Id
                            select TimeTrack.WorkTime).FirstOrDefault().ToString();

            WorkTime.Text = queryTime;

            // TO DO: aggiungere metodo che aggiorna anche la datagrid
        }

        public void GetDbPerson()
        {
            DbSet<Person> personList = intimeDb.People;

            var query = from Person in personList
                        orderby Person.Id
                        select Person.PersonName;

            List<string> list = query.ToList();
            ComboPerson.Items.Clear();

            foreach (string personName in list)
            {
                ComboBoxItem comboItem = new ComboBoxItem();
                comboItem.Content = personName;
                ComboPerson.Items.Add(comboItem);

                // TO DO: fare in modo che se una persona sta già nella datagrid non compaia qui
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddPerson_Click(object sender, RoutedEventArgs e)
        {
            AddPersonWindow addPersonWindow = new AddPersonWindow();
            addPersonWindow.Show();
        }

        private void AddGridPerson_Click(object sender, RoutedEventArgs e)
        {
            string personName = ComboPerson.Text;

            DbSet<TimeTrack> timeTracks = intimeDb.TimeTracks;
            DbSet<Person> people = intimeDb.People;

            var queryPerson = (from Person in people
                              where Person.PersonName == personName
                              select Person.Id).FirstOrDefault();

            var queryWorkTime = (from TimeTrack in timeTracks
                        where TimeTrack.PersonId == queryPerson
                        select TimeTrack.WorkTime).FirstOrDefault().ToString();

            NameGrid.Items.Add(new NameTimeForGrid { name = personName, time = queryWorkTime });
        }
    }

    public class NameTimeForGrid
    {
        public string name { get; set; }
        public string time { get; set; }
    }
}
