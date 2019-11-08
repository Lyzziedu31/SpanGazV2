//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tbl_607_order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_607_order()
        {
            this.tbl_607_bottle = new HashSet<tbl_607_bottle>();
            this.tbl_607_order_details = new HashSet<tbl_607_order_details>();
            this.tbl_607_shipping_request = new HashSet<tbl_607_shipping_request>();
        }
    
        public int ID { get; set; }
        public System.DateTime start_date { get; set; }
        public System.DateTime end_date { get; set; }
        public string order_number { get; set; }
        public bool shipping_request_active { get; set; }
        public bool item_reception_active { get; set; }
        public int FK_ID_provider { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_bottle> tbl_607_bottle { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_order_details> tbl_607_order_details { get; set; }
        public virtual tbl_607_provider tbl_607_provider { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_shipping_request> tbl_607_shipping_request { get; set; }
    }
}