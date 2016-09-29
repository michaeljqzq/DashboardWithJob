using System;
using System.Collections.Generic;

namespace MSDNDashboardLibrary.Models
{
    public class Job
    {
        public int ID { get; set; }
        public DateTime StartTimestamp { get; set; }
        public DateTime FinishTimestamp { get; set; }
        public bool IsManual { get; set; }
        public JobStatus Status { get; set; }
        public int TotalNumber { get; set; }
        public int CurrentNumber { get; set; }
        public virtual ICollection<Blog> BlogList { get; set; } 
    }

    public enum JobStatus
    {
        Scheduled,
        Running,
        Succeeded,
        Failed,
        Canceled
    }
}