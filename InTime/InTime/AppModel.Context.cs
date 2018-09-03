namespace InTime
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class inTimeDbEntities : DbContext
    {
        public inTimeDbEntities()
            : base("name=inTimeDbEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<TimeTrack> TimeTracks { get; set; }
    }
}