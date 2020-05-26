using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class RoomTypeMVCModel
    {
        public int roomTypeID { get; set; }
        public string name { get; set; }
        public string type { get; set; }

        public string displayName { get; set; }

    }
}