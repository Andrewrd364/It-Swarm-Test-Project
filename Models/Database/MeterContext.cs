using Microsoft.EntityFrameworkCore;

namespace It_Swarm_Test_Project.Models.Database
{
    public class MeterContext : DbContext
    {
        public DbSet<Apartment> Apartments { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
        public DbSet<MeterReplacementHistory> MeterReplacementHistories { get; set; }

        public MeterContext(DbContextOptions<MeterContext> options) : base(options) { }
    }
}
