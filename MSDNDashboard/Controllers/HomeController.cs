using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSDNDashboard.DAL;
using MSDNDashboard.Models;
using System.Data.Entity.SqlServer;
using System.Text;

namespace MSDNDashboard.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var db = new DataContext();
            List<Job> jobList = db.Jobs.Where(j => j.Status == JobStatus.Running || j.Status == JobStatus.Scheduled).ToList();
            jobList.AddRange(db.Jobs.Where(j => j.Status == JobStatus.Succeeded || j.Status == JobStatus.Failed).OrderByDescending(j => j.ID).Take(10).ToList());
            return View(jobList);
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
                status = 2;
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
                message = "The job will run in a minute.",
                timestamp = DateTime.Now.ToString()
            });

        }

        public ActionResult GetJobById(int jobid)
        {
            var db = new DataContext();
            return PartialView("JobDetailPartial", db.Jobs.Find(jobid));
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
            foreach (var blog in job.BlogList.Where(b=>b.Status == BlogStatus.NoMSFTAdmin || b.Status == BlogStatus.ZeroAdmin || b.Status == BlogStatus.Error).OrderBy(b=>b.Status))
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