﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSDNDashboard.Models;
using MySql.Data.MySqlClient;

namespace MSDN.BlogDashboardWebJob
{
    public class BlogDatabaseConnector
    {
        private MySqlConnection connection;

        public BlogDatabaseConnector(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }

        public List<Blog> GetBlogs(int jobId)
        {
            string query = "select blog_id,domain,path from wp_blogs limit 30";
            MySqlCommand cmd = new MySqlCommand(query,connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<Blog> blogList = new List<Blog>();
            while (reader.Read() == true)
            {
                blogList.Add(new Blog()
                {
                    BlogID = Convert.ToInt32(reader["blog_id"]),
                    JobID = jobId,
                    Url = "https://"+reader["domain"]+reader["path"]
                });
            }
            reader.Close();
            return blogList;
        }

        public List<string> GetBlogAdmins(int blogId)
        {
            string query = "select a.meta_value from wp_usermeta a inner join wp_usermeta b on a.user_id=b.user_id where a.meta_key='_user_puid' and b.meta_key='wp_" + blogId + "_user_level' and b.meta_value='10'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<string> blogAdminsList = new List<string>();
            while (reader.Read() == true)
            {
                string metaValue = Convert.ToString(reader["meta_value"]);
                if (metaValue != "")
                {
                    blogAdminsList.Add(metaValue);
                }
            }
            reader.Close();
            return blogAdminsList;
        } 

        ~BlogDatabaseConnector()
        {
            connection.Close();
        }
    }
}
