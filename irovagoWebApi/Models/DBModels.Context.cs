﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Agency> Agencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<FavoriteOffer> FavoriteOffers { get; set; }
        public DbSet<HotelRoomType> HotelRoomTypes { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
}