﻿namespace MSDNDashboardLibrary.Models
{
    public class Blog
    {
        public int ID { get; set; }
        public int BlogID { get; set; }
        public int JobID { get; set; }
        public string Url { get; set; }
        public BlogStatus Status { get; set; }

    }

    public enum BlogStatus
    {
        Disabled,
        ZeroAdmin,
        NoMSFTAdmin,
        Normal,
        Error
    }
}