using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace irovagoMVC.Models
{
    public class LogMVCModel
    {
        public int logID { get; set; }
        public string actor { get; set; }
        public Nullable<int> actorID { get; set; }
        public string operation { get; set; }
        public string relatedTable { get; set; }
        public Nullable<int> relatedRecordID { get; set; }
    }
}