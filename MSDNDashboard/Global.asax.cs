using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Azure.KeyVault;
using System.Web.Configuration;
using MSDNDashboard.DAL;
using MSDNDashboard.Util;

namespace MSDNDashboard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Database.SetInitializer(new DataContextInitializer());

            //// I put my GetToken method in a Utils class. Change for wherever you placed your method.
            //var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(EncryptionHelper.GetToken));

            //var sec = kv.GetSecretAsync(WebConfigurationManager.AppSettings["SecretUri"]).Result.Value;

            ////I put a variable in a Utils class to hold the secret for general  application use.
            //EncryptionHelper.EncryptSecret = sec;
        }
    }
}
