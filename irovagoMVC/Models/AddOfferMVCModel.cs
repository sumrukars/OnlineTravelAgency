using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class AddOfferMVCModel
    {
      
        public OfferMVCModel offer { get; set; }

        public IEnumerable<HotelMVCModel> hotelList { get; set; }

        public IEnumerable<RoomTypeMVCModel> roomList { get; set; }
    }
}