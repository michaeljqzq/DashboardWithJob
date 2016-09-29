using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Azure.KeyVault;

namespace MSDNDashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //var db = new DataContext();
            //if(db.Jobs.ToList().Count == 0)
            //{
            //    Job j1 = new Job() { StartTimestamp = DateTime.Now.AddDays(-1), FinishTimestamp = DateTime.Now, IsManual = false, Status = JobStatus.Scheduled };
            //    db.Jobs.Add(j1);
            //    db.SaveChanges();
            //}


            //Database.SetInitializer(new DataContextInitializer());

            //// I put my GetToken method in a Utils class. Change for wherever you placed your method.
            //var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(EncryptionHelper.GetToken));

            //var sec = kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]).Result.Value;

            ////I put a variable in a Utils class to hold the secret for general  application use.
            //EncryptionHelper.EncryptSecret = sec;
        }
    }
}
