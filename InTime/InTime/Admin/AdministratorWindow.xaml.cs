using InTime.Logic;
using InTime.ViewModel;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace InTime.Admin
{
    public partial class AdministratorWindow : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();
        private List<Project> projectList;
        private List<Person> personList;

        private Project selectedProject;
        private ListBoxItem selectedListBoxProject;

        private static AdministratorWindow administratorWindow; // per singleton

        public AdministratorWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            GetProjectsInList();

            DbSet<Person> personDbList = intimeDb.People;
            personList = (from Person in personDbList
                          orderby Person.PersonName
                          select Person).ToList();
        }

        /*-------------------------------------------------------------- TASTI --------------------------------------------------------------*/

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void NewProjectButton_Click(object sender, RoutedEventArgs e)
        {   
            InTime.Project newProject = new InTime.Project();
            newProject.ProjectName = "Nuovo progetto";

            intimeDb.Projects.Add(newProject);
            intimeDb.SaveChanges();

            GetProjectsInList(); 
        }

        /*-------------------------------------------------------------- LISTBOX --------------------------------------------------------------*/

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
                itm.Tag = project;
                ProjectList.Items.Add(itm);
                itm.Selected += new RoutedEventHandler(ProjectSelected);
            }
        }

        private void ProjectSelected(object sender, RoutedEventArgs e)
        {
            selectedListBoxProject = (ListBoxItem)sender;
            PopulateSelectedProject(selectedListBoxProject);
        }

        /*-------------------------------------------------------------- COMBOBOX --------------------------------------------------------------*/

        public void GetPeopleListInComboBox(List<Assignment> assignmentList)
        {
            ComboPerson.Items.Clear();

            foreach (Person person in personList)
            {
                ComboBoxItem comboItem = new ComboBoxItem();
                comboItem.Content = person.PersonName;
                comboItem.Tag = person.Id;
                ComboPerson.Items.Add(comboItem);

                foreach (Assignment assignment in assignmentList)
                {
                    if (assignment.PersonId == person.Id)
                    {
                        ComboPerson.Items.Remove(comboItem);
                    }
                }
            }
        }

        private void AddGridPerson_Click(object sender, RoutedEventArgs e)
        {
            if (selectedProject != null)
            {
                NewAssignment(selectedProject.Id);
            }
        }

        private void NewAssignment(int projectId)
        {
            if(ComboPerson.SelectedItem != null)
            {
                int personId = (int)((ComboBoxItem)ComboPerson.SelectedItem).Tag;

                Assignment newAssignment = new Assignment
                {
                    PersonId = personId,
                    ProjectId = projectId,
                    Date = DateTime.Now,
                    Active = true
                };

                intimeDb.Assignments.Add(newAssignment);
                intimeDb.SaveChanges();

                PopulateSelectedProject(selectedListBoxProject);
            }
        }

        /*-------------------------------------------------------------- DETTAGLIO ASSIGNMENT --------------------------------------------------------------*/

        private void AssignmentGrid_RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AssignmentForDataGrid ass = AssignmentGrid.SelectedItem as AssignmentForDataGrid;
            AssignmentDetailWindow assignment = new AssignmentDetailWindow(ass.personId, selectedProject.Id);
        }

        /*-------------------------------------------------------------- POPOLARE CONTENUTI --------------------------------------------------------------*/

        private void PopulateSelectedProject(ListBoxItem listBoxProject)
        {
            DbSet<TimeTrack> timeTracksDBSet = intimeDb.TimeTracks;
            DbSet<Assignment> AssignmentsDBSet = intimeDb.Assignments;
            
            selectedProject = (Project)listBoxProject.Tag;


            // POPOLA LA DATAGRID

            // seleziona tutti gli assignment per un progetto
            List<Assignment> assignmentList = (from Assignment in AssignmentsDBSet
                                               where Assignment.ProjectId == selectedProject.Id
                                               select Assignment).ToList();

            List<AssignmentForDataGrid> dataGridAssignments = new List<AssignmentForDataGrid>();

            long projectTotalTicks = 0; // serve per il tempo totale del progetto

            foreach (Assignment assignment in assignmentList)
            {
                int personId = assignment.PersonId;

                List<TimeTrack> timetracksList = (from TimeTrack in timeTracksDBSet // seleziona tutti i timetrack di UNA persona per IL progetto selezionato
                                                  where TimeTrack.ProjectId == selectedProject.Id
                                                  where TimeTrack.PersonId == personId
                                                  select TimeTrack).ToList();

                long personTotalTicks = 0;
                foreach (TimeTrack track in timetracksList) // calcola il tempo totale della persona
                {
                    personTotalTicks += (long)track.WorkTime;
                }
                projectTotalTicks += personTotalTicks;

                // LISTA ASSIGNMENTS
                AssignmentForDataGrid assignmentForDataGrid = new AssignmentForDataGrid(); // creazione Assignment per DataGrid

                assignmentForDataGrid.name = assignment.Person.PersonName;
                assignmentForDataGrid.time = TimeTracker.ToString(TimeSpan.FromTicks(personTotalTicks));
                assignmentForDataGrid.active = assignment.Active;
                assignmentForDataGrid.personId = assignment.PersonId;
                dataGridAssignments.Add(assignmentForDataGrid);
            }

            // COMBOBOX: FILTRO LISTA PERSONE
            GetPeopleListInComboBox(assignmentList);

            // PROPRIETà DEL PROGETTO (GROUPBOX)

            TotalWorkTime.Text = TimeTracker.ToString(TimeSpan.FromTicks(projectTotalTicks));

            ProjectName.Text = selectedProject.ProjectName; // not null
            Customer.Text = selectedProject.Customer; // null
            Description.Text = selectedProject.Description; // null

            if (selectedProject.ProjectAssignedTime != null)
            {
                TimeSpan timespan = TimeSpan.FromTicks((long)selectedProject.ProjectAssignedTime);
                EstimatedTime.Text = TimeTracker.ToString(timespan);
            } // null
            else
            {
                EstimatedTime.Text = "";
            }

            if(selectedProject.DueDate != null)
            {
                DueDate.Text = ((DateTime)selectedProject.DueDate).ToShortDateString();
            } // null
            else
            {
                DueDate.Text = "";
            }

            if (selectedProject.DateCreation != null)
            {
                CreationDate.Text = ((DateTime)selectedProject.DateCreation).ToShortDateString();
            } // null
            else
            {
                CreationDate.Text = "";
            }

            if (selectedProject.Active) {  // not null
                Active.Text = "Sì";
            } // not null
            else
            { 
                Active.Text = "No";
            }

            // BINDING
            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = dataGridAssignments;
        }

        /*-------------------------------------------------------------- SINGLETON --------------------------------------------------------------*/

        public static void IsOpened()
        {
            if (administratorWindow == null)
            {
                administratorWindow = new AdministratorWindow();
                administratorWindow.Closed += (sender, args) => administratorWindow = null;
                administratorWindow.Show();
            }
        }
    }
}
