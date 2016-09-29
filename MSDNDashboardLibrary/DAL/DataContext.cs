using System.Data.Entity;
using MSDNDashboardLibrary.Models;

namespace MSDNDashboardLibrary.DAL
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DataContext")
        {
            //Database.SetInitializer(new DataContextInitializer());
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Config> Configs { get; set; } 
    }
}