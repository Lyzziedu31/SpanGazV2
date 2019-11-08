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
    using System.ComponentModel.DataAnnotations;

    public partial class tbl_607_actors
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_607_actors()
        {
            this.tbl_607_gaz_reporting = new HashSet<tbl_607_gaz_reporting>();
            this.tbl_607_shipping_request = new HashSet<tbl_607_shipping_request>();
        }

        public int id { get; set; }
        [RegularExpression(@"[A-Z0-9]+[\\][A-Z0-9]+", ErrorMessage = "Please enter: Domain + \\ + UID")]
        [Required]
        public string id_uid { get; set; }

        [RegularExpression(@"^(([A-za-z]+[\s]{1}[A-za-z]+)|([A-Za-z]+))$", ErrorMessage = "Please enter: First Uppercase + Lowcase (no symbol allowed)")]
        [Required]
        public string first_name { get; set; }

        [RegularExpression(@"^(([A-Z]+[\s]{1}[A-Z]+)|([A-Z]+))$", ErrorMessage = "Please enter: Uppercase (no symbol allowed)")]
        [Required]
        public string last_name { get; set; }

        [Required]
        public string role { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_gaz_reporting> tbl_607_gaz_reporting { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_607_shipping_request> tbl_607_shipping_request { get; set; }
    }
}
