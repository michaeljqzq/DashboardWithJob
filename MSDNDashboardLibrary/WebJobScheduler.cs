﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MSDNDashboardLibrary.DAL;
using MSDNDashboardLibrary.Models;

namespace MSDNDashboardLibrary
{
    public class WebJobScheduler
    {
        private static object _lock = new object();
        public async void StartJob(Job job)
        {
            // To avoid exception reference to http://stackoverflow.com/questions/14033193/entity-framework-provider-type-could-not-be-loaded
            var x = typeof(System.Data.Entity.SqlServer.SqlProviderServices); 

            Console.WriteLine("Job {0} started.",job.ID);
            using(var db = new DataContext())
            {
                BlogDatabaseConnector blogDatabaseConnector = null;
                ProfileApiHelper profileApiHelper = null;
                try {
                    blogDatabaseConnector = new BlogDatabaseConnector();
                    profileApiHelper = new ProfileApiHelper();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Connect mysql or profile API initilization failed. Details : {0}", e.StackTrace);
                    job.Status = JobStatus.Failed;
                    job.FinishTimestamp = DateTime.Now;
                    db.Entry(job).State = EntityState.Modified;
                    db.SaveChanges();
                    return;
                }
                job.Status = JobStatus.Running;
                db.Entry(job).State = EntityState.Modified;
                await db.SaveChangesAsync();
                List<Blog> blogList = new List<Blog>();
                try
                {
                    blogList = blogDatabaseConnector.GetBlogs(job.ID);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Get blog list failed. Details : {0}", e.StackTrace);
                    job.Status = JobStatus.Failed;
                    job.FinishTimestamp = DateTime.Now;
                    db.Entry(job).State = EntityState.Modified;
                    db.SaveChanges();
                    return;
                }
                Console.WriteLine("Blog ID list fetched.Total number : " + blogList.Count);
                job.TotalNumber = blogList.Count;
                db.Entry(job).State = EntityState.Modified;
                await db.SaveChangesAsync();

                int i = 0;

                foreach (var _blog in blogList)
                {
                    if (!blogDatabaseConnector.IsBlogSiteEnabled(_blog.BlogID))
                    {
                        job.TotalNumber--;
                        db.Entry(job).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        continue;
                    }
                    //var js1 = Stopwatch.StartNew();
                    //List<string> blogAdmins = blogDatabaseConnector.GetBlogAdmins(_blog.BlogID);
                    //js1.Stop();
                    //var js2 = Stopwatch.StartNew();
                    // Console.WriteLine("\n---------------------------\nScanning site : {0} (id:{1}). Found admin numbers:{2}\n",_blog.Url,_blog.BlogID,blogAdmins.Count);
                    //BlogStatus status = BlogStatus.NoMSFTAdmin;
                    //if(blogAdmins.Count==0)
                    //{
                    //    status = BlogStatus.ZeroAdmin;
                    //}
                    //foreach (var blogAdmin in blogAdmins)
                    //{
                    //    Console.WriteLine("Fetch user : {0} .", blogAdmin);
                    //    try
                    //    {
                    //        if (profileApiHelper.CheckIfUserIsMSFT(blogAdmin))
                    //        {
                    //            status = BlogStatus.Normal;
                    //            break;
                    //        }
                    //    }
                    //    catch (Exception e)
                    //    {
                    //        Console.WriteLine("[exception] : "+e.StackTrace);
                    //        status = BlogStatus.Error;
                    //        break;
                    //    }
                    //}
                    //js2.Stop();
                    //Console.WriteLine("\nComplete scanning site : {0}. Result : {1}. Time taken:{2} ms(Mysql:{3} ms, Api:{4} ms).", _blog.BlogID, status.ToString(), js1.ElapsedMilliseconds + js2.ElapsedMilliseconds, js1.ElapsedMilliseconds, js2.ElapsedMilliseconds);
                    //_blog.Status = status;
                    _blog.Status = blogDatabaseConnector.isCETheme(_blog.BlogID)
                        ? BlogStatus.ZeroAdmin
                        : BlogStatus.Normal;
                    db.Blogs.Add(_blog);
                    job.CurrentNumber = ++i;
                    db.Entry(job).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                }

                job.Status = JobStatus.Succeeded;
                job.FinishTimestamp = DateTime.Now;
                db.Entry(job).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        public Job GetNextJob()
        {
            using (var db = new DataContext())
            {
                Job oldestScheduledJob = db.Jobs.Where(j => j.Status == JobStatus.Scheduled).OrderBy(j=>j.StartTimestamp).FirstOrDefault();
                if (oldestScheduledJob == default(Job))
                {
                    return null;
                }
                if (CheckIfExistRunningJob())
                {
                    oldestScheduledJob.Status=JobStatus.Canceled;
                    db.SaveChangesAsync();
                    return null;
                }
                return oldestScheduledJob;
            }
        }

        private bool CheckIfExistRunningJob()
        {
            using (var db = new DataContext())
            {
                return db.Jobs.Any(j => j.Status == JobStatus.Running);
            }
        }
    }
}
