using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class HotelMVCModel
    {
        public int hotelID { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string url { get; set; }
        public string explanation { get; set; }

        public string imgHotel { get; set; }

        public HttpPostedFileBase hotelImageFile { get; set; }

    }
}