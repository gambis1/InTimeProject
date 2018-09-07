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

namespace InTime.Admin
{
    /// <summary>
    /// Interaction logic for AssignmentDetail.xaml
    /// </summary>
    public partial class AssignmentDetailWindow : Window
    {
        InTimeDbEntities intimeDb = new InTimeDbEntities();

        public AssignmentDetailWindow(int personId, int projectId)
        {
            DbSet<TimeTrack> timeTracksDBSet = intimeDb.TimeTracks;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            
            List<TimeTrack> timetracksList = (from TimeTrack in timeTracksDBSet
                                              where TimeTrack.ProjectId == projectId
                                              where TimeTrack.PersonId == personId
                                              select TimeTrack).ToList();

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
            this.Show();
        }
    }
}
