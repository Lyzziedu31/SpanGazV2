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
    
    public partial class tbl_607_shipping_details
    {
        public int ID { get; set; }
        public int delivery_quantity { get; set; }
        public int FK_ID_shipping_delivery { get; set; }
        public int FK_ID_gaz { get; set; }
    }
}