using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data;
using MySql;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;

namespace SmartCloudElevatorDataModel
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        public DataContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)=>
            optionsBuilder.UseMySql(ConnectionString);
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HotelElevatorRobot>().HasKey(t => new { t.HotelId, t.UniqueRobotSN, t.ElevatorId });
            modelBuilder.Entity<HotelElevator>().HasKey(t => new { t.ElevatorId, t.HotelId });
            modelBuilder.Entity<HotelRobot>().HasKey(t => new { t.UniqueRobotSN, t.HotelId });

            base.OnModelCreating(modelBuilder);
        }
        private string ConnectionString { get => JsonConvert.DeserializeObject<JObject>(File.ReadAllText("SmartCloudElevatorDataModel.json"))["mySQLConnectionString"].ToString(); }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<HotelElevatorRobot> HotelElevatorRobots { get; set; }
        public DbSet<RobotCompany> RobotCompanys { get; set; }
        public DbSet<RobotMap> RobotMaps { get; set; }
        public DbSet<ElevatorIdModule> ElevatorIdMudules { get; set; }
        public DbSet<ElevatorCompany> ElevatorCompanys { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HotelElevator> HotelElevators { get; set; }
        public DbSet<HotelRobot> HotelRobots { get; set; }

    }
}
