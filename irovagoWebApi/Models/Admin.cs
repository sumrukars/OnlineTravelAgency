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
    
    public partial class Admin
    {
        public Admin()
        {
            this.Logins = new HashSet<Login>();
        }
    
        public int adminID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
    
        public virtual ICollection<Login> Logins { get; set; }
    }
}