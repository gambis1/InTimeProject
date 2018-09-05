using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime.Logic
{
    class TimeTracker
    {
        private Stopwatch stopwatch = new Stopwatch();
        private TimeSpan startingTime;
        private int trackerId;
        private int projectId;
        private int personId;
        private InTimeDbEntities intimeDb = new InTimeDbEntities();

        public TimeTracker(int projectId, int personId)
        {
            this.projectId = projectId;
            this.personId = personId;
        }

        public string Start()
        {
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
                //trackerId =
                //    (from TimeTrack in timeTracksList
                //     where TimeTrack.ProjectId == projectId
                //     where TimeTrack.PersonId == personId
                //     where TimeTrack.WorkDate == );
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
            stopwatch.Start();

            return startingTime.ToString();
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

        public string UpdateSecond()
        {
            TimeSpan ts = stopwatch.Elapsed + startingTime;
            return String.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);
        }

        public bool IsRunning()
        {
            return stopwatch.IsRunning;
        }
    }
}
