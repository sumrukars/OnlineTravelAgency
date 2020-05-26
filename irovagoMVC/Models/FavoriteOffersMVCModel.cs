using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class FavoriteOffersMVCModel
    {
        public int favoriteOfferID { get; set; }
        public Nullable<int> customerID { get; set; }
        public Nullable<int> offerID { get; set; }

        public virtual CustomerMVCModel Customer { get; set; }
        public virtual OfferMVCModel Offer { get; set; }
    }
}