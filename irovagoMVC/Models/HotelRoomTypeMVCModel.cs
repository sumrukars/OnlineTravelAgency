using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class HotelRoomTypeMVCModel
    {
        public int hotelRoomTypeID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public Nullable<int> hotelID { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public Nullable<int> roomTypeID { get; set; }
    }
}