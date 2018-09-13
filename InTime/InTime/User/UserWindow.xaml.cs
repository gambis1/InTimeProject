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

        private static Person currentUser;

        private List<Assignment> userAssignments;

        public UserWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            currentUser = MainWindow.currentUser;
            GetAssignmentsInListbox();
        }

        /*-------------------------------------------------------------- TASTI --------------------------------------------------------------*/

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /*-------------------------------------------------------------- LISTBOX --------------------------------------------------------------*/

        

        private void GetAssignmentsInListbox()
        {
            DbSet<Assignment> assignmentDBList = intimeDb.Assignments;

            userAssignments =
                (from Assignment in assignmentDBList
                 orderby Assignment.Active
                 where Assignment.PersonId == currentUser.Id
                 where Assignment.Active == true // SOLO ASSIGNMENT ATTIVO
                 select Assignment).ToList();

            AssignmentList.Items.Clear();

            foreach (Assignment assignment in userAssignments)
            {
                ListBoxItem itm = new ListBoxItem();

                itm.Content = assignment.Project.ProjectName;
                itm.Tag = assignment.Project;

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

            // PROPRIETA' DEL PROGETTO
            ProjectName.Text = currentProject.ProjectName;
            Customer.Text = currentProject.Customer;
            if (currentProject.ProjectAssignedTime != null)
            {
                TimeSpan timespan = TimeSpan.FromTicks((long)currentProject.ProjectAssignedTime);
                EstimatedTime.Text = TimeTracker.ToString(timespan);
            }
            else
            {
                EstimatedTime.Text = "";
            }
            // TO DO: TEMPO TOTALE
            if (currentProject.DateCreation != null)
            {
                CreationDate.Text = ((DateTime)currentProject.DateCreation).ToShortDateString();
            }
            else
            {
                CreationDate.Text = "";
            }
            // TO DO: TRACCIA: Sì / NO
            AssignmentDescription.Text = currentProject.Description;

            DbSet<TimeTrack> timeTracksDBSet = intimeDb.TimeTracks;

            // TUTTI I TIMETRACK DEL PROGETTO
            List<TimeTrack> timetracksList = (from TimeTrack in timeTracksDBSet
                             where TimeTrack.ProjectId == currentProject.Id
                             where TimeTrack.PersonId == currentUser.Id
                             select TimeTrack).ToList();

            long personTotalTicks = 0;
            foreach (TimeTrack track in timetracksList) // calcola il tempo totale della persona
            {
                personTotalTicks += (long)track.WorkTime;
            }
            TotalWorkTime.Text = TimeTracker.ToString(TimeSpan.FromTicks(personTotalTicks));

            // LISTA TIMETRACKS DATAGRID

            List<TimeTrackViewModel> dataGridTimeTracks = new List<TimeTrackViewModel>();

            foreach (TimeTrack timeTrack in timetracksList)
            {
                TimeTrackViewModel timetrackvm = new TimeTrackViewModel();
                timetrackvm.WorkDate = ((DateTime)timeTrack.WorkDate).ToShortDateString();


                timetrackvm.WorkTime = TimeTracker.ToString(TimeSpan.FromTicks((long)timeTrack.WorkTime));

                dataGridTimeTracks.Add(timetrackvm);
            }

            CollectionViewSource itemCollectionViewSource;
            itemCollectionViewSource = (CollectionViewSource)(FindResource("ItemCollectionViewSource"));
            itemCollectionViewSource.Source = dataGridTimeTracks;
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
