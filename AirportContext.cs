using AirportLogic.Models;
using AirportModels;
using Microsoft.EntityFrameworkCore;

namespace AirportWebApi.Data
{
    public class AirportContext : DbContext
    {
        public AirportContext(DbContextOptions<AirportContext> options) : base(options)
        {
        }

        public DbSet<Flight> Flights { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Plane> Planes { get; set; }
       


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Plane>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.HasOne(p => p.Flight)
                    .WithOne(f => f.Plane)
                    .HasForeignKey<Flight>(f => f.PlaneId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(p => p.Location)
                    .WithMany(l => l.Planes)
                    .HasForeignKey(p => p.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(p => p.Status)
                    .IsRequired()
                    .HasConversion<string>();
            });

            modelBuilder.Entity<Flight>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.HasOne(f => f.Plane)
                    .WithOne(p => p.Flight)
                    .HasForeignKey<Flight>(f => f.PlaneId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasKey(l => l.Id);
                entity.HasMany(l => l.Planes)
                    .WithOne(p => p.Location)
                    .HasForeignKey(p => p.LocationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            #region PlaneData
            modelBuilder.Entity<Plane>().HasData(
                new Plane
                {
                    Id = 1,
                    PlaneCode = "1",
                    FlightId = 1,
                    LocationId = "1",
                    Status = PlaneStatus.Landing
                },
                new Plane
                {
                    Id = 2,
                    PlaneCode = "2",
                    FlightId = 2,
                    LocationId = "2",
                    Status = PlaneStatus.Landing
                },
                new Plane
                {
                    Id = 3,
                    PlaneCode = "3",
                    FlightId = 3,
                    LocationId = "3",
                    Status = PlaneStatus.Landing
                },
                new Plane
                {
                    Id = 4,
                    PlaneCode = "4",
                    FlightId = 4,
                    LocationId = "6",
                    Status = PlaneStatus.TakingOff
                },
                new Plane
                {
                    Id = 5,
                    PlaneCode = "5",
                    FlightId = 5,
                    LocationId = "7",
                    Status = PlaneStatus.TakingOff
                }); ;


            #endregion
            #region FlightData
            modelBuilder.Entity<Flight>().HasData(
               new Flight
               {
                   Id = 1,
                   FlightNumber = "AAA",
                   PlaneId = 1,
               },

               new Flight
               {
                   Id = 2,
                   FlightNumber = "BBB",
                   PlaneId = 2
               },
               new Flight
               {
                   Id = 3,
                   FlightNumber = "CCC",
                   PlaneId = 3
               },
               new Flight
               {
                   Id = 4,
                   FlightNumber = "DDD",
                   PlaneId = 4
               },
               new Flight
               {
                   Id = 5,
                   FlightNumber = "EEE",
                   PlaneId = 5
               });


            #endregion
            #region LocationData
            modelBuilder.Entity<Location>().HasData(
                new Location
                {
                    Id = "1",
                    TimeInStation = TimeSpan.FromSeconds(1)
                },
                new Location
                {
                    Id = "2",
                    TimeInStation = TimeSpan.FromSeconds(1)

                },
                new Location
                {
                    Id = "3",
                    TimeInStation = TimeSpan.FromSeconds(1)
                },
                new Location
                {
                    Id = "4",
                    TimeInStation = TimeSpan.FromSeconds(5)
                },
                new Location
                {
                    Id = "5",
                    TimeInStation = TimeSpan.FromSeconds(3)
                },
                new Location
                {
                    Id = "6",
                    TimeInStation = TimeSpan.FromSeconds(5)
                },
                new Location
                {
                    Id = "7",
                    TimeInStation = TimeSpan.FromSeconds(5)
                },
                new Location
                {
                    Id = "8",
                    TimeInStation = TimeSpan.FromSeconds(3)
                },
                new Location
                {
                    Id = "9",
                    TimeInStation = TimeSpan.FromSeconds(1)
                });
            #endregion

        }



    }
}
