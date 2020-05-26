using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class CustomerMVCModel
    {
        public int customerID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string name { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string surname { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string phone { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string address { get; set; }

        public int favoritedOfferCount { get; set; }

    }
}