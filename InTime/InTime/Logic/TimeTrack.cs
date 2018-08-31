using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTime.Logic
{
    class TimeTrack
    {
        private Stopwatch stopwatch;
        private TimeSpan startingTime;

        // TO DO: tenere presente la data
        // se è il primo track del giorno, creare un nuovo record del database, altrimenti aggiornare quello esistente

        public void Start(DateTime elapsedTime)
        {
            // convertire 'elapsedTime' in ore minuti secondi (?)
            int hours = 1;
            int minutes = 20;
            int seconds = 0;

            startingTime = new TimeSpan(hours, minutes, seconds);
            stopwatch.Start();
        }

        public DateTime Update()
        {
            TimeSpan trackedTime = stopwatch.Elapsed + startingTime;
            
            // registrare sul database

            return new DateTime();
        }

        public void Stop()
        {
            Update();
            stopwatch.Stop();
        }
    }
}
