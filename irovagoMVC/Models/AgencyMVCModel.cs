using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class AgencyMVCModel
    {
        public int agencyID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string address { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string phone { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string brand { get; set; }

        public int offerCount { get; set; }
    }
}