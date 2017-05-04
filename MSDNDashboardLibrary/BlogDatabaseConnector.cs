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

        public bool isCETheme(int blogId)
        {
            string query = "select option_value from wp_"+blogId+"_options where option_name='template'";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read() == true)
            {
                string metaValue = Convert.ToString(reader["option_value"]);
                if (metaValue == "ceblog-2016")
                {
                    reader.Close();
                    return true;
                }
            }
            reader.Close();
            return false;
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

        public void UpdateRewriteRules(int blogId)
        {
            const string VALUE =
                @"a:114:{s:34:""sitemap(-+([a-zA-Z0-9_-]+))?\.xml$"";s:40:""index.php?xml_sitemap=params=$matches[2]"";s:38:""sitemap(-+([a-zA-Z0-9_-]+))?\.xml\.gz$"";s:49:""index.php?xml_sitemap=params=$matches[2];zip=true"";s:35:""sitemap(-+([a-zA-Z0-9_-]+))?\.html$"";s:50:""index.php?xml_sitemap=params=$matches[2];html=true"";s:38:""sitemap(-+([a-zA-Z0-9_-]+))?\.html.gz$"";s:59:""index.php?xml_sitemap=params=$matches[2];html=true;zip=true"";s:11:""^wp-json/?$"";s:22:""index.php?rest_route=/"";s:14:""^wp-json/(.*)?"";s:33:""index.php?rest_route=/$matches[1]"";s:17:""comment-badges/?$"";s:34:""index.php?post_type=comment-badges"";s:47:""comment-badges/feed/(feed|rdf|rss|rss2|atom)/?$"";s:51:""index.php?post_type=comment-badges&feed=$matches[1]"";s:42:""comment-badges/(feed|rdf|rss|rss2|atom)/?$"";s:51:""index.php?post_type=comment-badges&feed=$matches[1]"";s:34:""comment-badges/page/([0-9]{1,})/?$"";s:52:""index.php?post_type=comment-badges&paged=$matches[1]"";s:47:""category/(.+?)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:52:""index.php?category_name=$matches[1]&feed=$matches[2]"";s:42:""category/(.+?)/(feed|rdf|rss|rss2|atom)/?$"";s:52:""index.php?category_name=$matches[1]&feed=$matches[2]"";s:23:""category/(.+?)/embed/?$"";s:46:""index.php?category_name=$matches[1]&embed=true"";s:35:""category/(.+?)/page/?([0-9]{1,})/?$"";s:53:""index.php?category_name=$matches[1]&paged=$matches[2]"";s:17:""category/(.+?)/?$"";s:35:""index.php?category_name=$matches[1]"";s:44:""tag/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:42:""index.php?tag=$matches[1]&feed=$matches[2]"";s:39:""tag/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:42:""index.php?tag=$matches[1]&feed=$matches[2]"";s:20:""tag/([^/]+)/embed/?$"";s:36:""index.php?tag=$matches[1]&embed=true"";s:32:""tag/([^/]+)/page/?([0-9]{1,})/?$"";s:43:""index.php?tag=$matches[1]&paged=$matches[2]"";s:14:""tag/([^/]+)/?$"";s:25:""index.php?tag=$matches[1]"";s:45:""type/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:50:""index.php?post_format=$matches[1]&feed=$matches[2]"";s:40:""type/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:50:""index.php?post_format=$matches[1]&feed=$matches[2]"";s:21:""type/([^/]+)/embed/?$"";s:44:""index.php?post_format=$matches[1]&embed=true"";s:33:""type/([^/]+)/page/?([0-9]{1,})/?$"";s:51:""index.php?post_format=$matches[1]&paged=$matches[2]"";s:15:""type/([^/]+)/?$"";s:33:""index.php?post_format=$matches[1]"";s:42:""comment-badges/[^/]+/attachment/([^/]+)/?$"";s:32:""index.php?attachment=$matches[1]"";s:52:""comment-badges/[^/]+/attachment/([^/]+)/trackback/?$"";s:37:""index.php?attachment=$matches[1]&tb=1"";s:72:""comment-badges/[^/]+/attachment/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:67:""comment-badges/[^/]+/attachment/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:67:""comment-badges/[^/]+/attachment/([^/]+)/comment-page-([0-9]{1,})/?$"";s:50:""index.php?attachment=$matches[1]&cpage=$matches[2]"";s:48:""comment-badges/[^/]+/attachment/([^/]+)/embed/?$"";s:43:""index.php?attachment=$matches[1]&embed=true"";s:31:""comment-badges/([^/]+)/embed/?$"";s:47:""index.php?comment-badges=$matches[1]&embed=true"";s:35:""comment-badges/([^/]+)/trackback/?$"";s:41:""index.php?comment-badges=$matches[1]&tb=1"";s:55:""comment-badges/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:53:""index.php?comment-badges=$matches[1]&feed=$matches[2]"";s:50:""comment-badges/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:53:""index.php?comment-badges=$matches[1]&feed=$matches[2]"";s:43:""comment-badges/([^/]+)/page/?([0-9]{1,})/?$"";s:54:""index.php?comment-badges=$matches[1]&paged=$matches[2]"";s:50:""comment-badges/([^/]+)/comment-page-([0-9]{1,})/?$"";s:54:""index.php?comment-badges=$matches[1]&cpage=$matches[2]"";s:39:""comment-badges/([^/]+)(?:/([0-9]+))?/?$"";s:53:""index.php?comment-badges=$matches[1]&page=$matches[2]"";s:31:""comment-badges/[^/]+/([^/]+)/?$"";s:32:""index.php?attachment=$matches[1]"";s:41:""comment-badges/[^/]+/([^/]+)/trackback/?$"";s:37:""index.php?attachment=$matches[1]&tb=1"";s:61:""comment-badges/[^/]+/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:56:""comment-badges/[^/]+/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:56:""comment-badges/[^/]+/([^/]+)/comment-page-([0-9]{1,})/?$"";s:50:""index.php?attachment=$matches[1]&cpage=$matches[2]"";s:37:""comment-badges/[^/]+/([^/]+)/embed/?$"";s:43:""index.php?attachment=$matches[1]&embed=true"";s:48:"".*wp-(atom|rdf|rss|rss2|feed|commentsrss2)\.php$"";s:18:""index.php?feed=old"";s:20:"".*wp-app\.php(/.*)?$"";s:19:""index.php?error=403"";s:18:"".*wp-register.php$"";s:23:""index.php?register=true"";s:32:""feed/(feed|rdf|rss|rss2|atom)/?$"";s:27:""index.php?&feed=$matches[1]"";s:27:""(feed|rdf|rss|rss2|atom)/?$"";s:27:""index.php?&feed=$matches[1]"";s:8:""embed/?$"";s:21:""index.php?&embed=true"";s:20:""page/?([0-9]{1,})/?$"";s:28:""index.php?&paged=$matches[1]"";s:41:""comments/feed/(feed|rdf|rss|rss2|atom)/?$"";s:42:""index.php?&feed=$matches[1]&withcomments=1"";s:36:""comments/(feed|rdf|rss|rss2|atom)/?$"";s:42:""index.php?&feed=$matches[1]&withcomments=1"";s:17:""comments/embed/?$"";s:21:""index.php?&embed=true"";s:44:""search/(.+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:40:""index.php?s=$matches[1]&feed=$matches[2]"";s:39:""search/(.+)/(feed|rdf|rss|rss2|atom)/?$"";s:40:""index.php?s=$matches[1]&feed=$matches[2]"";s:20:""search/(.+)/embed/?$"";s:34:""index.php?s=$matches[1]&embed=true"";s:32:""search/(.+)/page/?([0-9]{1,})/?$"";s:41:""index.php?s=$matches[1]&paged=$matches[2]"";s:14:""search/(.+)/?$"";s:23:""index.php?s=$matches[1]"";s:47:""author/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:50:""index.php?author_name=$matches[1]&feed=$matches[2]"";s:42:""author/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:50:""index.php?author_name=$matches[1]&feed=$matches[2]"";s:23:""author/([^/]+)/embed/?$"";s:44:""index.php?author_name=$matches[1]&embed=true"";s:35:""author/([^/]+)/page/?([0-9]{1,})/?$"";s:51:""index.php?author_name=$matches[1]&paged=$matches[2]"";s:17:""author/([^/]+)/?$"";s:33:""index.php?author_name=$matches[1]"";s:69:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/feed/(feed|rdf|rss|rss2|atom)/?$"";s:80:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&feed=$matches[4]"";s:64:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/(feed|rdf|rss|rss2|atom)/?$"";s:80:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&feed=$matches[4]"";s:45:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/embed/?$"";s:74:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&embed=true"";s:57:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/page/?([0-9]{1,})/?$"";s:81:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&paged=$matches[4]"";s:39:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/?$"";s:63:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]"";s:56:""([0-9]{4})/([0-9]{1,2})/feed/(feed|rdf|rss|rss2|atom)/?$"";s:64:""index.php?year=$matches[1]&monthnum=$matches[2]&feed=$matches[3]"";s:51:""([0-9]{4})/([0-9]{1,2})/(feed|rdf|rss|rss2|atom)/?$"";s:64:""index.php?year=$matches[1]&monthnum=$matches[2]&feed=$matches[3]"";s:32:""([0-9]{4})/([0-9]{1,2})/embed/?$"";s:58:""index.php?year=$matches[1]&monthnum=$matches[2]&embed=true"";s:44:""([0-9]{4})/([0-9]{1,2})/page/?([0-9]{1,})/?$"";s:65:""index.php?year=$matches[1]&monthnum=$matches[2]&paged=$matches[3]"";s:26:""([0-9]{4})/([0-9]{1,2})/?$"";s:47:""index.php?year=$matches[1]&monthnum=$matches[2]"";s:43:""([0-9]{4})/feed/(feed|rdf|rss|rss2|atom)/?$"";s:43:""index.php?year=$matches[1]&feed=$matches[2]"";s:38:""([0-9]{4})/(feed|rdf|rss|rss2|atom)/?$"";s:43:""index.php?year=$matches[1]&feed=$matches[2]"";s:19:""([0-9]{4})/embed/?$"";s:37:""index.php?year=$matches[1]&embed=true"";s:31:""([0-9]{4})/page/?([0-9]{1,})/?$"";s:44:""index.php?year=$matches[1]&paged=$matches[2]"";s:13:""([0-9]{4})/?$"";s:26:""index.php?year=$matches[1]"";s:58:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/attachment/([^/]+)/?$"";s:32:""index.php?attachment=$matches[1]"";s:68:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/attachment/([^/]+)/trackback/?$"";s:37:""index.php?attachment=$matches[1]&tb=1"";s:88:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/attachment/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:83:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/attachment/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:83:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/attachment/([^/]+)/comment-page-([0-9]{1,})/?$"";s:50:""index.php?attachment=$matches[1]&cpage=$matches[2]"";s:64:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/attachment/([^/]+)/embed/?$"";s:43:""index.php?attachment=$matches[1]&embed=true"";s:53:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)/embed/?$"";s:91:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&embed=true"";s:57:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)/trackback/?$"";s:85:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&tb=1"";s:77:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:97:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&feed=$matches[5]"";s:72:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:97:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&feed=$matches[5]"";s:65:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)/page/?([0-9]{1,})/?$"";s:98:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&paged=$matches[5]"";s:72:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)/comment-page-([0-9]{1,})/?$"";s:98:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&cpage=$matches[5]"";s:61:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/([^/]+)(?:/([0-9]+))?/?$"";s:97:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&name=$matches[4]&page=$matches[5]"";s:47:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/([^/]+)/?$"";s:32:""index.php?attachment=$matches[1]"";s:57:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/([^/]+)/trackback/?$"";s:37:""index.php?attachment=$matches[1]&tb=1"";s:77:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:72:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:72:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/([^/]+)/comment-page-([0-9]{1,})/?$"";s:50:""index.php?attachment=$matches[1]&cpage=$matches[2]"";s:53:""[0-9]{4}/[0-9]{1,2}/[0-9]{1,2}/[^/]+/([^/]+)/embed/?$"";s:43:""index.php?attachment=$matches[1]&embed=true"";s:64:""([0-9]{4})/([0-9]{1,2})/([0-9]{1,2})/comment-page-([0-9]{1,})/?$"";s:81:""index.php?year=$matches[1]&monthnum=$matches[2]&day=$matches[3]&cpage=$matches[4]"";s:51:""([0-9]{4})/([0-9]{1,2})/comment-page-([0-9]{1,})/?$"";s:65:""index.php?year=$matches[1]&monthnum=$matches[2]&cpage=$matches[3]"";s:38:""([0-9]{4})/comment-page-([0-9]{1,})/?$"";s:44:""index.php?year=$matches[1]&cpage=$matches[2]"";s:27:"".?.+?/attachment/([^/]+)/?$"";s:32:""index.php?attachment=$matches[1]"";s:37:"".?.+?/attachment/([^/]+)/trackback/?$"";s:37:""index.php?attachment=$matches[1]&tb=1"";s:57:"".?.+?/attachment/([^/]+)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:52:"".?.+?/attachment/([^/]+)/(feed|rdf|rss|rss2|atom)/?$"";s:49:""index.php?attachment=$matches[1]&feed=$matches[2]"";s:52:"".?.+?/attachment/([^/]+)/comment-page-([0-9]{1,})/?$"";s:50:""index.php?attachment=$matches[1]&cpage=$matches[2]"";s:33:"".?.+?/attachment/([^/]+)/embed/?$"";s:43:""index.php?attachment=$matches[1]&embed=true"";s:16:""(.?.+?)/embed/?$"";s:41:""index.php?pagename=$matches[1]&embed=true"";s:20:""(.?.+?)/trackback/?$"";s:35:""index.php?pagename=$matches[1]&tb=1"";s:40:""(.?.+?)/feed/(feed|rdf|rss|rss2|atom)/?$"";s:47:""index.php?pagename=$matches[1]&feed=$matches[2]"";s:35:""(.?.+?)/(feed|rdf|rss|rss2|atom)/?$"";s:47:""index.php?pagename=$matches[1]&feed=$matches[2]"";s:28:""(.?.+?)/page/?([0-9]{1,})/?$"";s:48:""index.php?pagename=$matches[1]&paged=$matches[2]"";s:35:""(.?.+?)/comment-page-([0-9]{1,})/?$"";s:48:""index.php?pagename=$matches[1]&cpage=$matches[2]"";s:24:""(.?.+?)(?:/([0-9]+))?/?$"";s:47:""index.php?pagename=$matches[1]&page=$matches[2]"";}";
            string command = "update wp_{0}_options set option_value='{1}' where option_name='rewrite_rules'";
            command = String.Format(command, blogId, VALUE);
            ExecuteNonQuery(command);
        }

        ~BlogDatabaseConnector()
        {
            connection.Close();
        }
    }
}
