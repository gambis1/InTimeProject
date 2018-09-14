using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace InTime.Logic
{
    class TimeTracker
    {
        private Stopwatch stopwatch = new Stopwatch();
        private static TimeSpan startingTime;

        private static int trackerId;
        private Assignment assignment;

        private static InTimeDbEntities intimeDb = new InTimeDbEntities();

        public TimeTracker(Assignment assignment)
        {
            this.assignment = assignment;
        }

        public string Start()
        {
            string startingTime = GetStartingTime(assignment);
            stopwatch.Start();

            return startingTime;
        }

        public static string GetStartingTime(Assignment assignment)
        {
            int projectId = assignment.ProjectId;
            int personId = assignment.PersonId;

            DbSet<TimeTrack> timeTracksList = intimeDb.TimeTracks;

            DateTime? trackingDate =
                 (from TimeTrack in timeTracksList
                  orderby TimeTrack.Id descending
                  where TimeTrack.ProjectId == projectId
                  where TimeTrack.PersonId == personId
                  select TimeTrack.WorkDate).FirstOrDefault();

            startingTime = new TimeSpan(0, 0, 0);

            if (trackingDate != DateTime.Now.Date) // data non presente in database: nuova data, startingtime = 0
            {
                DateTime newDate = DateTime.Now.Date;

                TimeTrack newRecord = new TimeTrack();
                newRecord.WorkDate = newDate;
                newRecord.WorkTime = startingTime.Ticks;
                newRecord.PersonId = personId;
                newRecord.ProjectId = projectId;

                intimeDb.TimeTracks.Add(newRecord);
                intimeDb.SaveChanges();
                trackerId = newRecord.Id;
            }
            else // data già presente: query sul tempo già tracciato
            {
                TimeTrack record =
                (from TimeTrack in timeTracksList
                 orderby TimeTrack.Id descending
                 where TimeTrack.ProjectId == projectId
                 where TimeTrack.PersonId == personId
                 where TimeTrack.WorkDate == trackingDate
                 select TimeTrack).First(); // tempo relativo alla data 'trackingDate'

                startingTime = new TimeSpan((long)record.WorkTime);
                trackerId = record.Id;
            }

            return TimeTracker.ToString(startingTime);
        }

        public void Update()
        {
            DbSet<TimeTrack> timeTracksList = intimeDb.TimeTracks;

            TimeTrack record =
                (from TimeTrack in timeTracksList
                where TimeTrack.Id == trackerId
                select TimeTrack).First();

            record.WorkTime += stopwatch.Elapsed.Ticks;
            intimeDb.SaveChanges();
        }

        public void Stop()
        {
            if (stopwatch.IsRunning)
            {
                Update();
                stopwatch.Stop();
            }
        }

        public string GetCurrentTime()
        {
            TimeSpan ts = stopwatch.Elapsed + startingTime;
            return ToString(ts);
        }

        public bool IsRunning()
        {
            return stopwatch.IsRunning;
        }

        public static string ToString(TimeSpan timeSpan)
        {
            return String.Format("{0:00}:{1:00}:{2:00}", timeSpan.TotalHours, timeSpan.Minutes, timeSpan.Seconds);
        } 
    }
}
