using MSDNDashboard.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using MSDNDashboard.Models;

namespace MSDN.BlogDashboardWebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new WebJobScheduler();
            while (true)
            {
                Job j = scheduler.GetNextJob();
                if (j == null)
                {
                    Console.WriteLine("Checked : next job is null");
                }
                if (j != null)
                {
                    Task.Run(() => { scheduler.StartJob(j); });
                }
                Thread.Sleep(1000*Convert.ToInt32(ConfigurationManager.AppSettings["CheckTimeSpanInSeconds"]));
            }
        }
    }
}
