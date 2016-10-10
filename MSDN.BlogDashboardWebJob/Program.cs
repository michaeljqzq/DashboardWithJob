using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using MSDNDashboard.Util;
using MSDNDashboardLibrary;
using MSDNDashboardLibrary.Models;

namespace MSDN.BlogDashboardWebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            EncryptionHelper.InitilizeKV(ConfigurationManager.AppSettings["KVSecretUri"], ConfigurationManager.AppSettings["KVThumbprint"], ConfigurationManager.AppSettings["KVClientId"]);
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
