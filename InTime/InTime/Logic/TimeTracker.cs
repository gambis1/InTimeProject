using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime.Logic
{
    class TimeTracker
    {
        private Stopwatch stopwatch = new Stopwatch();
        private DateTime trackingDate;
        private TimeSpan startingTime;
        private int projectId;
        private int personId;
        inTimeDbEntities intimeDb = new inTimeDbEntities();

        public TimeTracker(int projectId, int personId)
        {
            this.projectId = projectId;
            this.personId = personId;
        }

        public void Start()
        {
            // TO DO: query sul nome del progetto: 'latestDate' = ultima data registrata

            trackingDate = new DateTime(2018, 9, 1); // DEBUG,HARDCODED

            if (trackingDate.Date != DateTime.Now.Date) // data non presente in database: nuova data, startingtime = 0
            {
                DateTime newDate = DateTime.Now.Date;
                startingTime = new TimeSpan(0, 0, 0);
            } else // data già presente: query sul tempo già tracciato
            {
                // TO DO: query per tempo tracciato

                startingTime = new TimeSpan(1, 30, 0); // DEBUG,HARDCODED
            }

            stopwatch.Reset();
            stopwatch.Start();
        }

        public TimeSpan Update()
        {
            if (this.IsRunning)
            {
                // registrare sul database
                InTime.TimeTrack timeTrack = new TimeTrack();
                TimeSpan trackedTime = stopwatch.Elapsed + startingTime;
                timeTrack.PersonId = personId;
                timeTrack.ProjectId = projectId;
                timeTrack.WorkTime = trackedTime;
                timeTrack.WorkDate = trackingDate;

                intimeDb.TimeTracks.Add(timeTrack);
                intimeDb.SaveChanges();

                return trackedTime;
            } else
            {
                return startingTime;
            }
        }

        public void Stop()
        {
            if (this.IsRunning) {
                Update();
                stopwatch.Stop();
            }
        }

        public bool IsRunning
        {
            get { return stopwatch.IsRunning; }
        }
    }
}
