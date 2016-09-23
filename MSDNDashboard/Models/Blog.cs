using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MSDNDashboard.Models
{
    public class Blog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int BlogID { get; set; }
        public int JobID { get; set; }
        public string Url { get; set; }
        public BlogStatus Status { get; set; }

    }

    public enum BlogStatus
    {
        ZeroAdmin,
        NoMSFTAdmin,
        Normal,
        Error
    }
}