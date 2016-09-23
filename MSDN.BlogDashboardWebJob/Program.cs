using MSDNDashboard.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDNDashboard.Models;

namespace MSDN.BlogDashboardWebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            //Database.SetInitializer(new DataContextInitializer());

            var scheduler = new WebJobScheduler();
            Job j = scheduler.GetNextJob();
            if (j != null)
            {
                scheduler.StartJob(j);
            }
        }
    }
}
