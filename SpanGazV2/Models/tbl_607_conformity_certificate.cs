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
    
    public partial class tbl_607_conformity_certificate
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_607_conformity_certificate()
        {
            this.tbl_607_bottle = new HashSet<tbl_607_bottle>();
        }
    
        public int ID { get; set; }
        public System.DateTime fabrication_date { get; set; }
        public System.DateTime expiration_date { get; set; }
        public Nullable<int> FK_ID_real_gaz_content { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_bottle> tbl_607_bottle { get; set; }
    }
}
