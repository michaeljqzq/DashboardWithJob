using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MSDNDashboard.DAL;
using MSDNDashboard.Models;

namespace MSDNDashboard.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var db = new DataContext();
            return View(db.Jobs.ToList());
        }

        public ActionResult AjaxUpdate(int jobId)
        {
            // status:
            // 0 Idle
            // 1 Scheduled
            // 2 Running
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
            if (Model.Any(j => j.Status == JobStatus.Scheduled))
            {
                status = 1;
            }
            ViewBag.status = status;
            
            return PartialView("CurrentStatePartial",db.Jobs.Find(jobId));
        }
    }
}