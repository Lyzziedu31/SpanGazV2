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
    
    public partial class tbl_607_units
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_607_units()
        {
            this.tbl_607_gaz = new HashSet<tbl_607_gaz>();
            this.tbl_607_gaz1 = new HashSet<tbl_607_gaz>();
            this.tbl_607_gaz2 = new HashSet<tbl_607_gaz>();
        }
    
        public int ID { get; set; }
        public string unit { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_gaz> tbl_607_gaz { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_gaz> tbl_607_gaz1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_gaz> tbl_607_gaz2 { get; set; }
    }
}