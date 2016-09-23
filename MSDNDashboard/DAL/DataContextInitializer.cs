using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MSDNDashboard.Models;

namespace MSDNDashboard.DAL
{
    public class DataContextInitializer : DropCreateDatabaseAlways<DataContext>
    {
        protected override void Seed(DataContext context)
        {
            Config c1 = new Config(){Key = "RepeatedRunInDays" , Value = "7"};
            Job j1 = new Job(){StartTimestamp = DateTime.Now.AddDays(-1),FinishTimestamp = DateTime.Now,IsManual = false,Status = JobStatus.Scheduled};
            context.Configs.Add(c1);
            context.Jobs.Add(j1);
            context.SaveChanges();
        }
    }
}