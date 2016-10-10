using System;
using System.Collections.Generic;
using MSDNDashboard.Util;
using MSDNDashboardLibrary.Models;
using MySql.Data.MySqlClient;

namespace MSDNDashboardLibrary
{
    public class BlogDatabaseConnector
    {
        private MySqlConnection connection;

        public BlogDatabaseConnector()
        {
            connection = new MySqlConnection(EncryptionHelper.Configs["BlogsDbConnectionString"]);
            connection.Open();
        }

        public List<Blog> GetBlogs(int jobId)
        {
            string query = "select blog_id,domain,path from wp_blogs where path<>'/'";
            MySqlCommand cmd = new MySqlCommand(query,connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<Blog> blogList = new List<Blog>();
            while (reader.Read())
            {
                blogList.Add(new Blog()
                {
                    BlogID = Convert.ToInt32(reader["blog_id"]),
                    JobID = jobId,
                    Url = "https://" + reader["domain"] + reader["path"],
                    Status = BlogStatus.Disabled
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
                blogAdminsList.Add(metaValue);
            }
            reader.Close();
            return blogAdminsList;
        }

        public bool IsBlogSiteEnabled(int blogId)
        {
            string query = "select option_value from wp_"+blogId+"_options where option_name='ilmds_splash_page_enabled'";
            bool result = true;
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<string> blogAdminsList = new List<string>();
            while (reader.Read() == true)
            {
                string optionValue = Convert.ToString(reader["option_value"]);
                if (optionValue == "y")
                {
                    result = false;
                }
            }
            reader.Close();
            return result;
        }

        private void ExecuteNonQuery(string command)
        {
            MySqlCommand comm = connection.CreateCommand();
            comm.CommandText = command;
            comm.ExecuteNonQuery();
        }

        public int GetBlogIdByPath(string path, string brand)
        {
            string query = "select blog_id from wp_blogs where path='/" + path + "/' and domain='blogs." + brand + ".microsoft.com'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            List<Blog> blogList = new List<Blog>();
            int result = -1;
            while (reader.Read())
            {
                result = Convert.ToInt32(reader["blog_id"]);
            }
            reader.Close();
            return result;
        }

        public void InsertUserRoleOptions(int blogId)
        {
            string command = "insert into wp_{0}_options (option_name,option_value,autoload) values('wp_{0}_user_roles','a:5:{{s:13:\"administrator\";a:2:{{s:4:\"name\";s:13:\"Administrator\";s:12:\"capabilities\";a:61:{{s:13:\"switch_themes\";b:1;s:11:\"edit_themes\";b:1;s:16:\"activate_plugins\";b:1;s:12:\"edit_plugins\";b:1;s:10:\"edit_users\";b:1;s:10:\"edit_files\";b:1;s:14:\"manage_options\";b:1;s:17:\"moderate_comments\";b:1;s:17:\"manage_categories\";b:1;s:12:\"manage_links\";b:1;s:12:\"upload_files\";b:1;s:6:\"import\";b:1;s:10:\"edit_posts\";b:1;s:17:\"edit_others_posts\";b:1;s:20:\"edit_published_posts\";b:1;s:13:\"publish_posts\";b:1;s:10:\"edit_pages\";b:1;s:4:\"read\";b:1;s:8:\"level_10\";b:1;s:7:\"level_9\";b:1;s:7:\"level_8\";b:1;s:7:\"level_7\";b:1;s:7:\"level_6\";b:1;s:7:\"level_5\";b:1;s:7:\"level_4\";b:1;s:7:\"level_3\";b:1;s:7:\"level_2\";b:1;s:7:\"level_1\";b:1;s:7:\"level_0\";b:1;s:17:\"edit_others_pages\";b:1;s:20:\"edit_published_pages\";b:1;s:13:\"publish_pages\";b:1;s:12:\"delete_pages\";b:1;s:19:\"delete_others_pages\";b:1;s:22:\"delete_published_pages\";b:1;s:12:\"delete_posts\";b:1;s:19:\"delete_others_posts\";b:1;s:22:\"delete_published_posts\";b:1;s:20:\"delete_private_posts\";b:1;s:18:\"edit_private_posts\";b:1;s:18:\"read_private_posts\";b:1;s:20:\"delete_private_pages\";b:1;s:18:\"edit_private_pages\";b:1;s:18:\"read_private_pages\";b:1;s:12:\"delete_users\";b:1;s:12:\"create_users\";b:1;s:17:\"unfiltered_upload\";b:1;s:14:\"edit_dashboard\";b:1;s:14:\"update_plugins\";b:1;s:14:\"delete_plugins\";b:1;s:15:\"install_plugins\";b:1;s:13:\"update_themes\";b:1;s:14:\"install_themes\";b:1;s:11:\"update_core\";b:1;s:10:\"list_users\";b:1;s:12:\"remove_users\";b:1;s:13:\"promote_users\";b:1;s:18:\"edit_theme_options\";b:1;s:13:\"delete_themes\";b:1;s:6:\"export\";b:1;s:15:\"unfiltered_html\";b:1;}}}}s:6:\"editor\";a:2:{{s:4:\"name\";s:6:\"Editor\";s:12:\"capabilities\";a:34:{{s:17:\"moderate_comments\";b:1;s:17:\"manage_categories\";b:1;s:12:\"manage_links\";b:1;s:12:\"upload_files\";b:1;s:15:\"unfiltered_html\";b:1;s:10:\"edit_posts\";b:1;s:17:\"edit_others_posts\";b:1;s:20:\"edit_published_posts\";b:1;s:13:\"publish_posts\";b:1;s:10:\"edit_pages\";b:1;s:4:\"read\";b:1;s:7:\"level_7\";b:1;s:7:\"level_6\";b:1;s:7:\"level_5\";b:1;s:7:\"level_4\";b:1;s:7:\"level_3\";b:1;s:7:\"level_2\";b:1;s:7:\"level_1\";b:1;s:7:\"level_0\";b:1;s:17:\"edit_others_pages\";b:1;s:20:\"edit_published_pages\";b:1;s:13:\"publish_pages\";b:1;s:12:\"delete_pages\";b:1;s:19:\"delete_others_pages\";b:1;s:22:\"delete_published_pages\";b:1;s:12:\"delete_posts\";b:1;s:19:\"delete_others_posts\";b:1;s:22:\"delete_published_posts\";b:1;s:20:\"delete_private_posts\";b:1;s:18:\"edit_private_posts\";b:1;s:18:\"read_private_posts\";b:1;s:20:\"delete_private_pages\";b:1;s:18:\"edit_private_pages\";b:1;s:18:\"read_private_pages\";b:1;}}}}s:6:\"author\";a:2:{{s:4:\"name\";s:6:\"Author\";s:12:\"capabilities\";a:10:{{s:12:\"upload_files\";b:1;s:10:\"edit_posts\";b:1;s:20:\"edit_published_posts\";b:1;s:13:\"publish_posts\";b:1;s:4:\"read\";b:1;s:7:\"level_2\";b:1;s:7:\"level_1\";b:1;s:7:\"level_0\";b:1;s:12:\"delete_posts\";b:1;s:22:\"delete_published_posts\";b:1;}}}}s:11:\"contributor\";a:2:{{s:4:\"name\";s:11:\"Contributor\";s:12:\"capabilities\";a:5:{{s:10:\"edit_posts\";b:1;s:4:\"read\";b:1;s:7:\"level_1\";b:1;s:7:\"level_0\";b:1;s:12:\"delete_posts\";b:1;}}}}s:10:\"subscriber\";a:2:{{s:4:\"name\";s:10:\"Subscriber\";s:12:\"capabilities\";a:2:{{s:4:\"read\";b:1;s:7:\"level_0\";b:1;}}}}}}','yes');";
            command = String.Format(command, blogId);
            ExecuteNonQuery(command);
        }

        ~BlogDatabaseConnector()
        {
            connection.Close();
        }
    }
}
