﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Text;
using MSDNDashboard.Models;
using MSDNDashboard.Util;
using MSDNDashboardLibrary;
using MSDNDashboardLibrary.DAL;
using MSDNDashboardLibrary.Models;

namespace MSDNDashboard.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [BasicAuthentication("msdn", "#Bugsfor$")]
        public ActionResult Index()
        {
            var db = new DataContext();
            var num = Convert.ToInt32(ConfigurationManager.AppSettings["NumbersInHistoryJobs"]);
            List<Job> jobList = db.Jobs.Where(j => j.Status == JobStatus.Running || j.Status == JobStatus.Scheduled).ToList();
            jobList.AddRange(db.Jobs.Where(j => j.Status == JobStatus.Succeeded || j.Status == JobStatus.Failed).OrderByDescending(j => j.ID).Take(num).ToList());
            return View(jobList);
        }

        [BasicAuthentication("msdn", "blogdashboard")]
        [HttpGet]
        public ActionResult FixUserRole()
        {
            return View(new SiteViewModel());
        }

        [HttpPost]
        public ActionResult FixUserRole(SiteViewModel site)
        {
            BlogDatabaseConnector blogDatabase = new BlogDatabaseConnector();
            return fixSite(site, blogDatabase.InsertUserRoleOptions);
        }

        [BasicAuthentication("msdn", "blogdashboard")]
        [HttpGet]
        public ActionResult FixRewrite()
        {
            return View(new SiteViewModel());
        }

        [HttpPost]
        public ActionResult FixRewrite(SiteViewModel site)
        {
            BlogDatabaseConnector blogDatabase = new BlogDatabaseConnector();
            return fixSite(site, blogDatabase.UpdateRewriteRules);
        }

        private ActionResult fixSite(SiteViewModel site,Action<int> databaseMethod)
        {
            if (ModelState.IsValid)
            {
                BlogDatabaseConnector blogDatabase = new BlogDatabaseConnector();
                int blogId = blogDatabase.GetBlogIdByPath(site.Name.TrimStart('/').TrimEnd('/'), site.Brand.ToString().ToLower());
                if (blogId == -1)
                {
                    ViewBag.isSuccess = false;
                    ViewBag.message = "Can't find blog with the name :" + site.Name;
                    return View(site);
                }
                string mysqlResult = "";
                try
                {
                    databaseMethod(blogId);
                }
                catch (Exception e)
                {
                    mysqlResult = "Error writing database : " + e.StackTrace;
                    Trace.TraceError(mysqlResult); // do nothing since database item already exists
                }


                RedisConnector redis = new RedisConnector();
                if (!redis.RemoveSiteOptionCache(blogId))
                {
                    ViewBag.isSuccess = false;
                    ViewBag.message = "Can't refresh cache " + mysqlResult;
                    return View(site);
                }
                ViewBag.isSuccess = true;
                ViewBag.message = "Successfully fixed user role " + mysqlResult;
            }
            return View(site);
        }

        public ActionResult AjaxUpdate(int jobId)
        {
            // status:
            // 0 Not Running
            // 1 Running
            var db = new DataContext();
            int status = 0;
            var Model = db.Jobs.ToList();
            Job currentDisplayJob = Model.FirstOrDefault(j => j.Status == JobStatus.Running);

            if (currentDisplayJob != default(Job))
            {
                status = 1;
            }
            else
            {
                currentDisplayJob = Model.Where(j => j.Status == JobStatus.Succeeded || j.Status == JobStatus.Failed).OrderByDescending(j => j.StartTimestamp).FirstOrDefault();
            }

            Job nextScheduledJob = Model.Where(j => j.Status == JobStatus.Scheduled).OrderBy(j => j.StartTimestamp).FirstOrDefault();
            ViewBag.nextScheduledJob = nextScheduledJob;
            ViewBag.status = status;
            
            return PartialView("CurrentStatePartial",db.Jobs.Find(jobId));
        }

        public ActionResult TriggerJob()
        {
            var db = new DataContext();
            if(db.Jobs.Any(j=>j.Status == JobStatus.Running))
            {
                return Json(new
                {
                    success = "false",
                    message = "There's a job running"
                });
            }
            if(db.Jobs.Any(j=>j.Status == JobStatus.Scheduled && SqlFunctions.DateDiff("minute",DateTime.Now,j.StartTimestamp)<3))
            {
                return Json(new
                {
                    success = "false",
                    message = "There's already a job scheduled"
                });
            }
            Job newJob = new Job()
            {
                StartTimestamp = DateTime.Now,
                FinishTimestamp = DateTime.Now,
                IsManual = true,
                Status = JobStatus.Scheduled
            };
            db.Jobs.Add(newJob);
            db.SaveChanges();
            return Json(new
            {
                success = "true",
                message = "The page will automatically refresh when job starts.",
                jobid = newJob.ID,
                timestamp = DateTime.Now.ToString()
            });

        }

        public ActionResult GetJobById(int jobid)
        {
            var db = new DataContext();
            return PartialView("JobDetailPartial", db.Jobs.Find(jobid));
        }

        public ActionResult GetJobHistory()
        {
            var db = new DataContext();
            var num = Convert.ToInt32(ConfigurationManager.AppSettings["NumbersInHistoryJobs"]);
            List<Job> jobList = db.Jobs.Where(j => j.Status == JobStatus.Running || j.Status == JobStatus.Scheduled).ToList();
            jobList.AddRange(db.Jobs.Where(j => j.Status == JobStatus.Succeeded || j.Status == JobStatus.Failed).OrderByDescending(j => j.ID).Take(num).ToList());
            return PartialView("JobHistoryPartial", jobList);
        }

        public ActionResult GetCsvFromJob(int jobid)
        {
            var db = new DataContext();
            Job job = db.Jobs.Find(jobid);
            if (job == null)
            {
                return new EmptyResult();
            }
            StringBuilder output = new StringBuilder();
            output.Append("BlogID,BlogUrl,Status\n");
            foreach (var blog in job.BlogList.OrderBy(b=>b.Status))
            {
                output.Append("\"");
                output.Append(blog.BlogID);
                output.Append("\",\"");
                output.Append(blog.Url);
                output.Append("\",\"");
                output.Append(blog.Status);
                output.Append("\"\n");
            }
            return File(System.Text.Encoding.Default.GetBytes(output.ToString()), System.Net.Mime.MediaTypeNames.Text.Plain,"job_"+jobid+".csv");
        }
    }
}