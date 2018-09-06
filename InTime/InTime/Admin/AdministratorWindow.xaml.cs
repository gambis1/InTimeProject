using InTime.Logic;
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

        private List<Project> projectList;
        private List<Assignment> assignmentList;

        public AdministratorWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetProjectsInList();
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

                GetProjectsInList(); 
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

        private void GetProjectsInList()
        {
            DbSet<Project> projectDbList = intimeDb.Projects;

            projectList =
                (from Project in projectDbList
                 orderby Project.Id
                select Project).ToList();

            ProjectList.Items.Clear();

            foreach(Project project in projectList)
            {
                ListBoxItem itm = new ListBoxItem();
                itm.Content = project.ProjectName;
                itm.Tag = project.Id;
                ProjectList.Items.Add(itm);
                itm.Selected += new RoutedEventHandler(ProjectSelected);
            }
        }

        private void ProjectSelected(object sender, RoutedEventArgs e)
        {
            DbSet<TimeTrack> timeTracksDBSet = intimeDb.TimeTracks;
            DbSet<Assignment> AssignmentsDBSet = intimeDb.Assignments;

            ListBoxItem itm = (ListBoxItem)sender;
            int projectId = (int)itm.Tag;

            Project selectedProject = new Project();

            foreach(Project project in projectList) // pesca dalla lista dei progetti quello selezionato
            {
                if(projectId == project.Id)
                {
                    selectedProject = project;
                }
            }

            var workTimeList = (from TimeTrack in timeTracksDBSet
                         where selectedProject.Id == TimeTrack.ProjectId
                         select TimeTrack.WorkTime).Cast<long>().ToList(); // VARI id delle assegnazioni (a persone) di quel SINGOLO progetto

            long totalWorkTime = 0;
            foreach(long ticks in workTimeList)
            {
                totalWorkTime += ticks;
            }

            TimeSpan totalTime = TimeSpan.FromTicks(totalWorkTime);
            TotalWorkTime.Text = InTime.Logic.TimeTracker.ToString(totalTime); // tempo di lavoro totale


            // DATI PER GROUPBOX (PROPRIETà STATICHE DEL PROGETTO)
            ProjectName.Text = selectedProject.ProjectName; // not null
            Customer.Text = selectedProject.Customer; // null
            Description.Text = selectedProject.Description; // null
            EstimatedTime.Text = selectedProject.ProjectAssignedTime.ToString(); // null

            DateTime? creationDate = selectedProject.DateCreation; // null
            if (creationDate != null)
            {
                CreationDate.Text = ((DateTime)creationDate).ToShortDateString();
            } else
            {
                CreationDate.Text = "";
            }

            if (selectedProject.Active) // not null
                Active.Text = "Sì";
            else
                Active.Text = "No";
            // FINE DATI PER GROUPBOX






            // POPOLA LA DATAGRID

            List<Assignment> assignmentList = (from Assignment in AssignmentsDBSet // seleziona tutti gli assignment per un progetto
                                               where Assignment.ProjectId == projectId
                                               select Assignment).ToList();

            List<AssignmentForDataGrid> dataGridAssignments = new List<AssignmentForDataGrid>();

            foreach (Assignment assignment in assignmentList)
            {
                int personId = assignment.PersonId;

                List<TimeTrack> timetracksList = (from TimeTrack in timeTracksDBSet // seleziona tutti i timetrack di UNA persona per IL progetto selezionato
                                  where TimeTrack.ProjectId == projectId
                                  where TimeTrack.PersonId == personId
                                  select TimeTrack).ToList();

                long totalTicks = 0;
                foreach(TimeTrack track in timetracksList)
                {
                    totalTicks += (long)track.WorkTime;
                }
                TimeSpan personTotalWorktime = TimeSpan.FromTicks(totalTicks);

                AssignmentForDataGrid assignmentForDataGrid = new AssignmentForDataGrid();
                assignmentForDataGrid.name = personId.ToString();
                assignmentForDataGrid.time = TimeTracker.ToString(personTotalWorktime);
                dataGridAssignments.Add(assignmentForDataGrid);
            }

            // BINDING
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = dataGridAssignments;







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

            // AssignmentGrid.Items.Add(new NameTimeForGrid { name = personName, time = queryWorkTime });
        }
    }

    public class AssignmentForDataGrid
    {
        public string name { get; set; }
        public string time { get; set; }
    }
}
