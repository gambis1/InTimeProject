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
using System.Windows.Shapes;

namespace InTime.User
{
    public partial class UserWindow : Window
    {
        private static InTimeDbEntities intimeDb = new InTimeDbEntities();
        private static UserWindow userWindow;

        private static Guid userUniqueIdentifier;
        private static Person currentUser;

        private static List<Project> userProjects;
        private List<Assignment> userAssignments;

        public UserWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            currentUser = GetCurrentUser();
            GetDbData();
        }

        /*-------------------------------------------------------------- TASTI --------------------------------------------------------------*/

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*-------------------------------------------------------------- LISTBOX --------------------------------------------------------------*/

        public Person GetCurrentUser()
        {
            // TO DO: OTTENERE userUniqueIdentifier LEGGENDO FILE SETTINGS
            // TO DO 2: userUniqueIdentifier STATICA ALL'AVVIO DI MAINWINDOW

            userUniqueIdentifier = new Guid("fca345b4-e009-4fa3-a9b6-64210b584199");

            DbSet<Person> personDbList = intimeDb.People;
            return (from Person in personDbList
                    where Person.AccessCode == userUniqueIdentifier
                    select Person).Single();
        }

        private void GetDbData()
        {
            DbSet<Assignment> assignmentDBList = intimeDb.Assignments;

            userAssignments =
                (from Assignment in assignmentDBList
                 orderby Assignment.Active
                 where Assignment.PersonId == currentUser.Id
                 where Assignment.Active == true // assignment attivo
                 select Assignment).ToList();

            AssignmentList.Items.Clear();

            // FORSE HO CAPITO COME FUNZIONA ENTITY FRAMEWORK
            //DbSet<Project> projectDBList = intimeDb.Projects;
            //List<Project> projectList = // tutti i progetti
            //    (from Project in projectDBList
            //     orderby Project.Id
            //     select Project).ToList();
            //userProjects = new List<Project>(); // da usare per elencare tutti i progetti dell'utente

            foreach (Assignment assignment in userAssignments)
            {
                ListBoxItem itm = new ListBoxItem();

                itm.Content = assignment.Project.ProjectName;
                itm.Tag = assignment.Project;
                
                // FORSE HO CAPITO COME FUNZIONA ENTITY FRAMEWORK cit.
                //foreach (Project project in projectList)
                //{
                //    if (project.Id == assignment.ProjectId)
                //    {
                //        userProjects.Add(project); // aggiunge ai progetti dell'utente
                //        itm.Content = project.ProjectName;
                //    }
                //}

                AssignmentList.Items.Add(itm);
                itm.Selected += new RoutedEventHandler(AssignmentSelected);
            }
        }

        private void AssignmentSelected(object sender, RoutedEventArgs e)
        {
            ListBoxItem selectedListBoxAssignment = (ListBoxItem)sender;
            PopulateSelectedAssignment(selectedListBoxAssignment);
        }

        /*-------------------------------------------------------------- POPOLARE CONTENUTI --------------------------------------------------------------*/

        private void PopulateSelectedAssignment(ListBoxItem listBoxAssignment)
        {
            Project currentProject = (Project)listBoxAssignment.Tag;

            //string projectName = listBoxAssignment.Content.ToString();
            //ProjectName.Text = projectName;

            //DbSet<Project> projectList = intimeDb.Projects;

            //var queryDesc = (from Project in projectList
            //                 where Project.ProjectName == projectName
            //                 select Project.Description).FirstOrDefault();

            // PROPRIETA' DEL PROGETTO

            ProjectName.Text = currentProject.ProjectName;
            AssignmentDescription.Text = currentProject.Description;
            Customer.Text = currentProject.Customer;
            if(currentProject.DateCreation != null)
            {
                CreationDate.Text = ((DateTime)currentProject.DateCreation).ToShortDateString();
            }
            else
            {
                CreationDate.Text = "";
            }

            if(currentProject.ProjectAssignedTime != null)
            {
                TimeSpan timespan = TimeSpan.FromTicks((long)currentProject.ProjectAssignedTime);
                EstimatedTime.Text = TimeTracker.ToString(timespan);
            }
            

            //DbSet<TimeTrack> timeTracks = intimeDb.TimeTracks;

            //var queryTime = (from TimeTrack in timeTracks
            //                 join Project in projectList on TimeTrack.ProjectId equals Project.Id
            //                 select TimeTrack.WorkTime).FirstOrDefault().ToString();

            //WorkTime.Text = queryTime;

            // TO DO: aggiungere metodo che aggiorna anche la datagrid
        }

        /*-------------------------------------------------------------- SINGLETON --------------------------------------------------------------*/

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
