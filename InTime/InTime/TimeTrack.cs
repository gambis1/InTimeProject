//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace InTime
{
    using System;
    using System.Collections.Generic;
    
    public partial class TimeTrack
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int PersonId { get; set; }
        public Nullable<int> AssignedTime { get; set; }
        public System.TimeSpan WorkTime { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual Project Project { get; set; }
    }
}