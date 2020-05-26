using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class LoginMVCModel
    {
        public int loginID { get; set; }
        public string email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string password { get; set; }

        public Nullable<int> customerID { get; set; }
        public Nullable<int> agencyID { get; set; }

        public Nullable<int> adminID { get; set; }

        public virtual AgencyMVCModel Agency { get; set; }
        public virtual CustomerMVCModel Customer { get; set; }

        public virtual AdminMVCModel Admin { get; set; }



    }
}