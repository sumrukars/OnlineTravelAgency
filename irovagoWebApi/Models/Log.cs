//------------------------------------------------------------------------------
// <auto-generated>
//    Bu kod bir şablondan oluşturuldu.
//
//    Bu dosyada el ile yapılan değişiklikler uygulamanızda beklenmedik davranışa neden olabilir.
//    Kod yeniden oluşturulursa, bu dosyada el ile yapılan değişikliklerin üzerine yazılacak.
// </auto-generated>
//------------------------------------------------------------------------------

namespace irovagoWebApi.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log
    {
        public int logID { get; set; }
        public string actor { get; set; }
        public Nullable<int> actorID { get; set; }
        public string operation { get; set; }
        public string relatedTable { get; set; }
        public Nullable<int> relatedRecordID { get; set; }
    }
}