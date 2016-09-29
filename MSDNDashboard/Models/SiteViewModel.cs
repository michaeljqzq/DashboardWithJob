using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MSDNDashboard.Models
{
    public class SiteViewModel
    {
        public SiteBrand Brand { get; set; }
        [Required(ErrorMessage = "Site name can't be empty")]
        [Display(Name = "Blog site name")]
        public string Name { get; set; }

        public IEnumerable<SelectListItem> BrandItems
        {
            get
            {
                var allBrands = Enum.GetValues(typeof (SiteBrand));
                List<SelectListItem> result = new List<SelectListItem>();
                for(int i=0;i<allBrands.Length;i++)
                {
                    result.Add(new SelectListItem()
                    {
                        Text = allBrands.GetValue(i).ToString(),
                        Value = i.ToString()
                    });
                }
                return result;
            }
        } 
    }

    public enum SiteBrand
    {
        MSDN,
        Technet
    }
}