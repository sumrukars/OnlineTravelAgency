using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace irovagoMVC.Models
{
    public class ManageHotelMVCModel
    {
        public HotelMVCModel hotel { get; set; }

        public RoomTypeMVCModel roomType { get; set; }

        public List<RoomTypeMVCModel> roomList { get; set; }

        public List<RoomTypeMVCModel> hotelRoomList { get; set; }
    }
}