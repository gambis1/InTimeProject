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

namespace InTime.Admin
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class AdministratorWindow : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();
        private static AdministratorWindow administratorWindow;

        public AdministratorWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetDbData();
            GetDbPerson();
        }

        public static void IsOpened()
        {
            if (administratorWindow == null)
            {
                administratorWindow = new AdministratorWindow();
                administratorWindow.Closed += (sender, args) => administratorWindow = null;
                administratorWindow.Show();
            }
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
            this.Close();
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

            DbSet<Project> projectList = intimeDb.Projects;

            var selectedProject = (from Project in projectList
                        where Project.ProjectName == projectName
                        select Project).FirstOrDefault();

            DbSet<TimeTrack> timeTracksDBSet = intimeDb.TimeTracks;
            DbSet<Assignment> AssignmentsDBSet = intimeDb.Assignments;

            var workTimeList = (from TimeTrack in timeTracksDBSet
                         where selectedProject.Id == TimeTrack.ProjectId
                         select TimeTrack.WorkTime).ToList(); // VARI id delle assegnazioni (a persone) di quel SINGOLO progetto

            TimeSpan totalWorkTime = new TimeSpan();
            foreach(TimeSpan times in workTimeList)
            {
                totalWorkTime += times;
            }

            WorkTime.Text = totalWorkTime.ToString(); // tempo di lavoro totale effettuato

            // TO DO: aggiungere metodo che aggiorna anche la datagrid

            // PROPRIETà DEL PROGETTO (STATICHE)
            TextBlockProject.Text = projectName;

            TextBlockCustomers.Text = selectedProject.Customer; // null
            Description.Text = selectedProject.Description; // null
            TextBlockDateCreation.Text = selectedProject.DateCreation.ToString(); // null
            TextBlockEstimatedTime.Text = selectedProject.ProjectAssignedTime.ToString(); // null

            if (selectedProject.Active)
                TextBlockActive.Text = "Sì";
            else
                TextBlockActive.Text = "No";
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
