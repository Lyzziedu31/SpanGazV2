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
    
    public partial class tbl_607_shipping_request
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_607_shipping_request()
        {
            this.tbl_607_shipping_delivery = new HashSet<tbl_607_shipping_delivery>();
            this.tbl_607_shipping_request_details = new HashSet<tbl_607_shipping_request_details>();
        }
    
        public int ID { get; set; }
        public System.DateTime request_date { get; set; }
        public int FK_order { get; set; }
        public int FK_ID_actors { get; set; }
        public string shipping_request_ref { get; set; }
    
        public virtual tbl_607_actors tbl_607_actors { get; set; }
        public virtual tbl_607_order tbl_607_order { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_shipping_delivery> tbl_607_shipping_delivery { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_shipping_request_details> tbl_607_shipping_request_details { get; set; }
    }
}
