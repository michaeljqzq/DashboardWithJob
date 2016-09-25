using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MSDNDashboard.Models;

namespace MSDNDashboard.DAL
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DataContext")
        {
            Database.SetInitializer(new DataContextInitializer());
        }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Config> Configs { get; set; } 
    }
}