﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré à partir d'un modèle.
//
//     Des modifications manuelles apportées à ce fichier peuvent conduire à un comportement inattendu de votre application.
//     Les modifications manuelles apportées à ce fichier sont remplacées si le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SpanGazV2.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class database_tc2Entities : DbContext
    {
        public database_tc2Entities()
            : base("name=database_tc2Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_607_actors> tbl_607_actors { get; set; }
        public virtual DbSet<tbl_607_bottle> tbl_607_bottle { get; set; }
        public virtual DbSet<tbl_607_comments> tbl_607_comments { get; set; }
        public virtual DbSet<tbl_607_conformity_certificate> tbl_607_conformity_certificate { get; set; }
        public virtual DbSet<tbl_607_gaz> tbl_607_gaz { get; set; }
        public virtual DbSet<tbl_607_gaz_reporting> tbl_607_gaz_reporting { get; set; }
        public virtual DbSet<tbl_607_gaz_type> tbl_607_gaz_type { get; set; }
        public virtual DbSet<tbl_607_join_bottle_gaz> tbl_607_join_bottle_gaz { get; set; }
        public virtual DbSet<tbl_607_location> tbl_607_location { get; set; }
        public virtual DbSet<tbl_607_made_tolerance> tbl_607_made_tolerance { get; set; }
        public virtual DbSet<tbl_607_order> tbl_607_order { get; set; }
        public virtual DbSet<tbl_607_order_details> tbl_607_order_details { get; set; }
        public virtual DbSet<tbl_607_packaging> tbl_607_packaging { get; set; }
        public virtual DbSet<tbl_607_provider> tbl_607_provider { get; set; }
        public virtual DbSet<tbl_607_real_gaz_content> tbl_607_real_gaz_content { get; set; }
        public virtual DbSet<tbl_607_shipping_delivery> tbl_607_shipping_delivery { get; set; }
        public virtual DbSet<tbl_607_shipping_details> tbl_607_shipping_details { get; set; }
        public virtual DbSet<tbl_607_shipping_request> tbl_607_shipping_request { get; set; }
        public virtual DbSet<tbl_607_shipping_request_details> tbl_607_shipping_request_details { get; set; }
        public virtual DbSet<tbl_607_testing_tolerance> tbl_607_testing_tolerance { get; set; }
        public virtual DbSet<tbl_607_theorical_content> tbl_607_theorical_content { get; set; }
        public virtual DbSet<tbl_607_units> tbl_607_units { get; set; }
    }
}