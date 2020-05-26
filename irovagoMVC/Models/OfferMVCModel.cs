using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class OfferMVCModel
    {

        public int offerID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public Nullable<int> price { get; set; }
        public Nullable<int> agencyID { get; set; }
        public Nullable<int> hotelID { get; set; }
        public Nullable<int> roomTypeID { get; set; }
        public bool isFavorited { get; set; }
        public virtual AgencyMVCModel Agency { get; set; }
        public virtual HotelMVCModel Hotel { get; set; }
        public virtual ICollection<FavoriteOffersMVCModel> FavoriteOffers { get; set; }
        public virtual RoomTypeMVCModel RoomType { get; set; }
    }
}