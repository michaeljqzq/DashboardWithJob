using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MSDN.BlogDashboardWebJob
{
    public class ProfileApiHelper
    {
        private string applicationKey;
        private string profileApiUrlPattern;

        public ProfileApiHelper(string _profileApiUrlPattern, string _applicationKey)
        {
            applicationKey = _applicationKey;
            profileApiUrlPattern = _profileApiUrlPattern;
        }

        public bool CheckIfUserIsMSFT(string guid)
        {
            if (guid == "")
            {
                return false;
            }
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-ms-applicationKey", applicationKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(String.Format(profileApiUrlPattern, guid)).Result;
                response.EnsureSuccessStatusCode();
                dynamic responseJsonObject = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
                if (responseJsonObject.Affiliations != null)
                {
                    foreach (var affiliation in responseJsonObject.Affiliations)
                    {
                        if (affiliation == "MSFT")
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
